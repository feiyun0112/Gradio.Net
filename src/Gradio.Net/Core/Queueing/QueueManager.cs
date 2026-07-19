using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net.Core.Queueing;

public class QueueManager
{
    private readonly ConcurrentDictionary<string, Channel<EventMessage>> _pendingMessagesPerSession = new();
    private readonly ConcurrentDictionary<string, Channel<EventMessage>> _pendingMessagesPerEvent = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _pendingEventIdsSession = new();
    private readonly ConcurrentDictionary<string, QueueEvent> _eventIdsToEvents = new();
    private readonly ConcurrentDictionary<string, EventQueue> _eventQueuesPerConcurrencyId = new();
    private readonly ConcurrentDictionary<string, QueueAnalytics> _eventAnalytics = new();

    private readonly SemaphoreSlim _pendingMessageLock = new(1, 1);
    private readonly SemaphoreSlim _deleteLock = new(1, 1);

    private readonly int _maxThreadCount;
    private readonly int? _maxSize;
    private readonly int? _defaultConcurrencyLimit;
    private readonly double _updateIntervals;
    private readonly bool _liveUpdates;

    private static readonly JsonSerializerOptions _logJsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
    };

    private bool _stopped;
    private readonly List<Task> _backgroundTasks = new();
    private readonly ILogger<QueueManager> _logger;

    public App? App { get; set; }

    public bool Stopped => _stopped;

    public QueueManager(
        int maxThreadCount = 40,
        int? maxSize = null,
        int? defaultConcurrencyLimit = 1,
        double updateIntervals = 1.0,
        bool liveUpdates = false,
        ILogger<QueueManager>? logger = null)
    {
        _maxThreadCount = maxThreadCount;
        _maxSize = maxSize;
        _defaultConcurrencyLimit = defaultConcurrencyLimit;
        _updateIntervals = updateIntervals;
        _liveUpdates = liveUpdates;
        _logger = logger ?? LoggerFactory.Create(b => b.AddConsole()).CreateLogger<QueueManager>();
    }

    public void Start()
    {
        _stopped = false;
        _backgroundTasks.Add(Task.Run(() => StartProcessing()));
        _backgroundTasks.Add(Task.Run(() => StartProgressUpdates()));
        if (!_liveUpdates)
        {
            _backgroundTasks.Add(Task.Run(() => NotifyClients()));
        }
    }

    public async Task Stop()
    {
        _stopped = true;
        await Task.WhenAll(_backgroundTasks);
    }

    public async Task<(bool success, string eventIdOrError, string state)> Push(
        PredictBodyInternal body,
        string? username = null)
    {
        if (body.FnIndex == null)
        {
            return (false, "No function index provided.", "error");
        }

        if (_maxSize.HasValue && TotalQueueSize >= _maxSize.Value)
        {
            return (false, $"Queue is full. Max size is {_maxSize.Value} and size is {TotalQueueSize}.", "queue_full");
        }

        BlockFunction? fn;
        try
        {
            var blocks = App?.GetBlocks();
            fn = blocks == null ? null : RouteUtils.GetFn(blocks, null, body);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, "error");
        }

        // Run validator if the function has one, before enqueuing
        if (fn?.Validator != null)
        {
            var (allValid, validationDetail) = RunValidator(fn, body.Data);
            if (!allValid)
            {
                var detailJson = System.Text.Json.JsonSerializer.Serialize(validationDetail);
                return (false, detailJson, "validator_error");
            }
        }

        var concurrencyId = fn?.ConcurrencyId ?? body.FnIndex?.ToString() ?? "default";
        var sessionHash = body.SessionHash ?? Guid.NewGuid().ToString("N");
        var concurrencyLimit = ResolveConcurrencyLimit(fn);

        // Create or get event queue for this concurrency group
        var eventQueue = _eventQueuesPerConcurrencyId.GetOrAdd(
            concurrencyId,
            _ => new EventQueue(concurrencyId, concurrencyLimit)
        );

        if (concurrencyLimit.HasValue)
        {
            // Keep the strictest limit across functions sharing this concurrency group.
            if (!eventQueue.ConcurrencyLimit.HasValue || concurrencyLimit.Value < eventQueue.ConcurrencyLimit.Value)
            {
                eventQueue.ConcurrencyLimit = concurrencyLimit;
            }
        }

        // Create new event
        var evt = new QueueEvent(sessionHash, concurrencyId, username)
        {
            Data = body,
            Fn = fn,
            FnIndex = body.FnIndex?.ToString(),
            ApiName = fn?.ApiName ?? body.FnIndex?.ToString()
        };

        // Initialize session message channel if needed
        await _pendingMessageLock.WaitAsync();
        try
        {
            if (!_pendingMessagesPerSession.ContainsKey(sessionHash))
            {
                _pendingMessagesPerSession[sessionHash] = Channel.CreateUnbounded<EventMessage>();
            }

            if (!_pendingEventIdsSession.ContainsKey(sessionHash))
            {
                _pendingEventIdsSession[sessionHash] = new HashSet<string>();
            }

            _pendingEventIdsSession[sessionHash].Add(evt.Id);
        }
        finally
        {
            _pendingMessageLock.Release();
        }

        // Add to queues
        _eventIdsToEvents[evt.Id] = evt;
        eventQueue.Add(evt);

        // Record analytics
        _eventAnalytics[evt.Id] = new QueueAnalytics
        {
            Time = DateTime.UtcNow,
            Status = "queued",
            Function = evt.ApiName,
            SessionHash = sessionHash
        };

        // Python parity: send estimation immediately to session channel while event is guaranteed to be in queue.
        // BroadcastEstimations has a race condition (StartProcessing can dequeue before it iterates).
        // Sending directly here ensures the SSE client always receives an estimation message.
        if (_pendingMessagesPerSession.TryGetValue(sessionHash, out var estChannel))
        {
            await estChannel.Writer.WriteAsync(new EstimationMessage
            {
                EventId = evt.Id,
                Rank = 0,
                QueueSize = eventQueue.Count,
                RankEta = null
            });
        }

        return (true, evt.Id, "success");
    }

    private static (bool allValid, List<Dictionary<string, object>> detail) RunValidator(
        BlockFunction fn,
        List<object>? rawInputs)
    {
        var detail = new List<Dictionary<string, object>>();

        if (fn.Validator == null || rawInputs == null)
            return (true, detail);

        // Get the validator delegate's parameter types for type conversion
        var validatorMethodInfo = fn.Validator.Method;
        var validatorParams = validatorMethodInfo.GetParameters();

        // Get parameter names from the main function (for parameter_name field)
        var paramNames = new List<string>();
        if (fn.Fn != null)
        {
            paramNames = fn.Fn.Method.GetParameters().Select(p => p.Name ?? string.Empty).ToList();
        }

        // Convert raw inputs to typed values matching the validator signature
        var typedInputs = new List<object?>();
        for (int i = 0; i < validatorParams.Length; i++)
        {
            var rawVal = i < rawInputs.Count ? rawInputs[i] : null;
            var targetType = validatorParams[i].ParameterType;
            typedInputs.Add(ConvertToParameterType(rawVal, targetType));
        }

        // Invoke the validator
        object? validationResponse;
        try
        {
            validationResponse = fn.Validator.DynamicInvoke(typedInputs.ToArray());
        }
        catch
        {
            return (true, detail);
        }

        // Process the response – mirrors QueueingUtils.ProcessValidationResponse
        if (validationResponse is System.Collections.IList responseList)
        {
            for (int i = 0; i < responseList.Count; i++)
            {
                var item = responseList[i];
                if (item is Dictionary<string, object> dict &&
                    dict.TryGetValue("__type__", out var t) && t?.ToString() == "validate")
                {
                    var paramName = i < paramNames.Count ? paramNames[i] : $"parameter_{i}";
                    var entry = new Dictionary<string, object>(dict)
                    {
                        ["parameter_name"] = paramName
                    };
                    detail.Add(entry);
                }
                else
                {
                    var paramName = i < paramNames.Count ? paramNames[i] : $"parameter_{i}";
                    detail.Add(new Dictionary<string, object>
                    {
                        ["__type__"] = "validate",
                        ["is_valid"] = true,
                        ["message"] = string.Empty,
                        ["parameter_name"] = paramName
                    });
                }
            }
        }
        else if (validationResponse is Dictionary<string, object> singleDict)
        {
            detail.Add(singleDict);
        }

        var allValid = detail.Count == 0 ||
                       detail.All(x => x.TryGetValue("is_valid", out var v) && v is bool b && b);
        return (allValid, detail);
    }

    private static object? ConvertToParameterType(object? rawValue, Type targetType)
    {
        if (rawValue == null)
        {
            if (Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
                return null;
            return Activator.CreateInstance(targetType);
        }

        var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (nonNullable.IsInstanceOfType(rawValue))
            return rawValue;

        if (rawValue is System.Text.Json.JsonElement je)
        {
            try
            {
                if (nonNullable == typeof(string))
                    return je.ValueKind == System.Text.Json.JsonValueKind.String ? je.GetString() : je.ToString();
                if (nonNullable == typeof(bool))
                    return je.GetBoolean();
                if (nonNullable == typeof(int)) return je.GetInt32();
                if (nonNullable == typeof(long)) return je.GetInt64();
                if (nonNullable == typeof(float)) return je.GetSingle();
                if (nonNullable == typeof(double))
                    return je.ValueKind == System.Text.Json.JsonValueKind.Number ? je.GetDouble() : 0.0;
                if (nonNullable == typeof(decimal)) return je.GetDecimal();
                return System.Text.Json.JsonSerializer.Deserialize(je.GetRawText(), nonNullable);
            }
            catch
            {
                return rawValue;
            }
        }

        try
        {
            return Convert.ChangeType(rawValue, nonNullable, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            return rawValue;
        }
    }

    private int? ResolveConcurrencyLimit(BlockFunction? fn)
    {
        if (fn == null)
        {
            return _defaultConcurrencyLimit;
        }

        if (fn.ConcurrencyLimit is string limitStr)
        {
            return string.Equals(limitStr, "default", StringComparison.OrdinalIgnoreCase)
                ? _defaultConcurrencyLimit
                : null;
        }

        if (fn.ConcurrencyLimit is int limitInt)
        {
            return limitInt;
        }

        if (fn.ConcurrencyLimit is long limitLong)
        {
            return (int)limitLong;
        }

        if (fn.ConcurrencyLimit is null)
        {
            return null;
        }

        try
        {
            return Convert.ToInt32(fn.ConcurrencyLimit);
        }
        catch
        {
            return _defaultConcurrencyLimit;
        }
    }

    public async Task<bool> CancelEvent(string eventId, string sessionHash)
    {
        if (_eventIdsToEvents.TryGetValue(eventId, out var evt))
        {
            evt.Alive = false;
            evt.Closed = true;

            // Send completion message
            if (_pendingMessagesPerSession.TryGetValue(sessionHash, out var channel))
            {
                var message = new ProcessCompletedMessage
                {
                    EventId = eventId,
                    Success = true,
                    Output = new Dictionary<string, object>()
                };

                await channel.Writer.WriteAsync(message);
            }

            // Remove from pending events
            if (_pendingEventIdsSession.TryGetValue(sessionHash, out var eventIds))
            {
                eventIds.Remove(eventId);
            }

            return true;
        }

        return false;
    }

    public Channel<EventMessage>? GetSessionChannel(string sessionHash)
    {
        _pendingMessagesPerSession.TryGetValue(sessionHash, out var channel);
        return channel;
    }

    public Channel<EventMessage>? GetEventChannel(string eventId)
    {
        // First try to find the event-specific channel
        if (_pendingMessagesPerEvent.TryGetValue(eventId, out var eventChannel))
        {
            return eventChannel;
        }

        // If not found, create one and link to session channel
        if (_eventIdsToEvents.TryGetValue(eventId, out var evt))
        {
            var channel = Channel.CreateUnbounded<EventMessage>();
            _pendingMessagesPerEvent[eventId] = channel;

            // Also send messages from session channel filtered by event ID
            if (_pendingMessagesPerSession.TryGetValue(evt.SessionHash, out var sessionChannel))
            {
                // Create a background task to forward messages for this event
                Task.Run(async () =>
                {
                    try
                    {
                        await foreach (var message in sessionChannel.Reader.ReadAllAsync())
                        {
                            if (message.EventId == eventId)
                            {
                                await channel.Writer.WriteAsync(message);

                                // Close channel on completion or error
                                if (message is ProcessCompletedMessage || message is UnexpectedErrorMessage)
                                {
                                    channel.Writer.Complete();
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        channel.Writer.Complete();
                    }
                });
            }

            return channel;
        }

        return null;
    }

    public QueueEvent? GetEvent(string eventId)
    {
        _eventIdsToEvents.TryGetValue(eventId, out var evt);
        return evt;
    }

    public async Task CleanEvents(string sessionHash)
    {
        await _deleteLock.WaitAsync();
        try
        {
            if (_pendingEventIdsSession.TryGetValue(sessionHash, out var eventIds))
            {
                foreach (var eventId in eventIds.ToList())
                {
                    if (_eventIdsToEvents.TryRemove(eventId, out var evt))
                    {
                        evt.Alive = false;
                        evt.Closed = true;
                    }
                }
                _pendingEventIdsSession.TryRemove(sessionHash, out _);
            }
        }
        finally
        {
            _deleteLock.Release();
        }
    }

    public async Task<bool> CompleteEventForSession(string sessionHash, string? eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            return false;
        }

        await _pendingMessageLock.WaitAsync();
        try
        {
            if (_pendingEventIdsSession.TryGetValue(sessionHash, out var eventIds))
            {
                eventIds.Remove(eventId);
                return eventIds.Count == 0;
            }
        }
        finally
        {
            _pendingMessageLock.Release();
        }

        return false;
    }

    public QueueStatus GetStatus()
    {
        return new QueueStatus
        {
            QueueSize = TotalQueueSize,
            ActiveWorkers = _eventQueuesPerConcurrencyId.Values.Sum(q => q.CurrentConcurrency),
            MaxWorkers = _maxThreadCount
        };
    }

    private int TotalQueueSize =>
        _eventQueuesPerConcurrencyId.Values.Sum(q => q.Count);

    private async Task StartProcessing()
    {
        while (!_stopped)
        {
            try
            {
                bool processedAny = false;

                foreach (var (concurrencyId, eventQueue) in _eventQueuesPerConcurrencyId)
                {
                    // Check if we can process more events for this concurrency group
                    if (eventQueue.ConcurrencyLimit.HasValue &&
                        eventQueue.CurrentConcurrency >= eventQueue.ConcurrencyLimit.Value)
                    {
                        continue;
                    }

                    // Try to get next event
                    if (eventQueue.TryDequeue(out var evt) && evt != null)
                    {
                        eventQueue.CurrentConcurrency++;
                        processedAny = true;

                        // Process event in background
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await ProcessEvent(evt);
                            }
                            finally
                            {
                                eventQueue.CurrentConcurrency--;
                            }
                        });
                    }
                }

                if (!processedAny)
                {
                    await Task.Delay(10); // Sleep when no work
                }
            }
            catch (Exception ex)
            {
                // Log error but continue processing
            }
        }
    }

    private async Task ProcessEvent(QueueEvent evt)
    {
        if (!evt.Alive || evt.Closed)
            return;

        evt.StartedAt = DateTime.UtcNow;

        try
        {
            // Send process starts message
            await SendMessage(evt, new ProcessStartsMessage
            {
                EventId = evt.Id,
                Eta = null
            });

            if (evt.Data == null)
            {
                throw new InvalidOperationException("No event data provided.");
            }

            var app = App;
            if (app == null)
            {
                throw new InvalidOperationException("QueueManager is not attached to an App instance.");
            }

            var fn = evt.Fn ?? RouteUtils.GetFn(app.GetBlocks(), null, evt.Data);

            evt.Data.EventId ??= evt.Id;
            evt.Data.SessionHash ??= evt.SessionHash;

            HttpRequest? httpRequest = evt.Data.Request as HttpRequest;
            object? grRequest = null;

            // Python parity: event.request is stored at queue-join time (body.request = request in
            // queue_join_helper). Use the pre-captured GrRequest snapshot so the user's function
            // receives a non-null Request even when the original HTTP connection has already closed.
            if (evt.Data.GrRequest != null)
            {
                grRequest = evt.Data.GrRequest;
                // Ensure session hash is set on the captured request
                if (grRequest is Request capturedReq && string.IsNullOrWhiteSpace(capturedReq.SessionHash))
                {
                    capturedReq.SessionHash = evt.SessionHash;
                }
            }
            else if (evt.Data.Request != null || httpRequest != null)
            {
                grRequest = RouteUtils.CompileGrRequest(evt.Data, fn, evt.Username, httpRequest!);
            }
            else
            {
                // Fallback: create an empty Request with session hash so it is never null
                grRequest = new Request(request: null, username: evt.Username, sessionHash: evt.SessionHash);
            }

            string rootPath;
            if (httpRequest != null)
            {
                var routePath = RouteUtils.GetApiCallPath(httpRequest);
                rootPath = RouteUtils.GetRootUrl(httpRequest, routePath, app.GetBlocks().RootPath ?? "");
            }
            else
            {
                rootPath = app.GetBlocks().RootPath ?? "";
            }

            var isStreamConnection = string.Equals(fn.Connection, "stream", StringComparison.OrdinalIgnoreCase);
            var firstCallStopwatch = Stopwatch.StartNew();
            var output = await RouteUtils.CallProcessApi(app, evt.Data, grRequest, fn, rootPath);
            firstCallStopwatch.Stop();
            var firstIterationSeconds = firstCallStopwatch.Elapsed.TotalSeconds;
            evt.RunTimeSeconds += firstIterationSeconds;
            evt.CallCount++;

            if (isStreamConnection && fn.TimeLimit.HasValue)
            {
                output["is_generating"] = evt.RunTimeSeconds < fn.TimeLimit.Value && !evt.Closed;
            }

            if (IsGenerating(output))
            {
                _logger.LogInformation("[ProcessEvent] session={Session} event={Event} fn={Fn} is_generating=true, entering loop",
                    evt.SessionHash, evt.Id, fn.ApiName);
                Dictionary<string, object> lastOutput = output;
                int iterCount = 0;

                while (IsGenerating(output) && evt.Alive && !evt.Closed)
                {
                    lastOutput = output;
                    iterCount++;
                    _logger.LogInformation("[ProcessEvent] session={Session} event={Event} iter={Iter} sending process_generating",
                        evt.SessionHash, evt.Id, iterCount);
                    var remainingTimeLimit = GetRemainingTimeLimit(fn, evt, firstIterationSeconds);

                    if (isStreamConnection)
                    {
                        await SendMessage(evt, new ProcessStreamingMessage
                        {
                            EventId = evt.Id,
                            Success = true,
                            Output = output,
                            TimeLimit = remainingTimeLimit
                        });
                    }
                    else
                    {
                        await SendMessage(evt, new ProcessGeneratingMessage
                        {
                            EventId = evt.Id,
                            Success = true,
                            Output = output,
                            TimeLimit = remainingTimeLimit
                        });
                    }

                    if (isStreamConnection)
                    {
                        // Python parity: wait for next stream payload, or close/timeout.
                        var timeoutSeconds = Math.Max(0.0, (fn.TimeLimit ?? 30.0) - firstIterationSeconds);
                        var signaled = await WaitForSignalOrTimeout(evt, timeoutSeconds);
                        if (!signaled || evt.Closed)
                        {
                            break;
                        }
                    }

                    if (!evt.Alive || evt.Closed)
                    {
                        break;
                    }

                    var iterationStopwatch = Stopwatch.StartNew();
                    output = await RouteUtils.CallProcessApi(app, evt.Data, grRequest, fn, rootPath);
                    iterationStopwatch.Stop();

                    evt.RunTimeSeconds += iterationStopwatch.Elapsed.TotalSeconds;
                    evt.CallCount++;

                    if (isStreamConnection && fn.TimeLimit.HasValue)
                    {
                        output["is_generating"] = evt.RunTimeSeconds < fn.TimeLimit.Value && !evt.Closed;
                    }
                }

                // Python parity: send process_completed with is_generating=false.
                // output at this point has final=true data (full last frame) from handle_streaming_diffs.
                var completedOutput = new Dictionary<string, object>(output);
                _logger.LogInformation("[ProcessEvent] session={Session} event={Event} loop finished after {Iter} iterations, sending process_completed is_generating={IsGen} data=lastOutput",
                    evt.SessionHash, evt.Id, iterCount, IsGenerating(output));
                await SendMessage(evt, new ProcessCompletedMessage
                {
                    EventId = evt.Id,
                    Success = true,
                    Output = completedOutput
                });
            }
            else
            {
                await SendMessage(evt, new ProcessCompletedMessage
                {
                    EventId = evt.Id,
                    Success = true,
                    Output = output
                });
            }

            evt.CompletedAt = DateTime.UtcNow;

            // Update analytics
            if (_eventAnalytics.TryGetValue(evt.Id, out var analytics))
            {
                analytics.Status = "success";
                analytics.ProcessTime = evt.ProcessingTime?.TotalSeconds;
            }
        }
        catch (Exception ex)
        {
            var err = ex;
            while (err.InnerException != null &&
                   (err is System.Reflection.TargetInvocationException || err is AggregateException))
            {
                err = err.InnerException;
            }

            // Build error payload – mirrors Python's error_payload() in utils.py
            // If the exception is gr.Error (AppError), include duration/visible/title
            var errorOutput = new Dictionary<string, object> { ["error"] = err.Message };
            if (err is Gradio.Net.Core.Exceptions.Error gradioError)
            {
                errorOutput["duration"] = (object?)gradioError.Duration ?? (object)10.0;
                errorOutput["visible"] = gradioError.Visible;
                errorOutput["title"] = gradioError.Title ?? "Error";
            }

            await SendMessage(evt, new ProcessCompletedMessage
            {
                EventId = evt.Id,
                Success = false,
                Output = errorOutput
            });

            // Update analytics
            if (_eventAnalytics.TryGetValue(evt.Id, out var analytics))
            {
                analytics.Status = "failed";
            }
        }
    }

    public void SetProgress(string eventId, List<Gradio.Net.Core.TrackedIterable> iterables)
    {
        if (iterables == null || !_eventIdsToEvents.TryGetValue(eventId, out var evt) || !evt.Alive)
            return;

        var progressData = iterables.Select(iterable => new ProgressData
        {
            Index = iterable.Index.HasValue ? (int?)Convert.ToInt32(iterable.Index.Value) : null,
            Length = iterable.Length.HasValue ? (int?)Convert.ToInt32(iterable.Length.Value) : null,
            Unit = iterable.Unit ?? "steps",
            Progress = iterable.Progress,
            Desc = iterable.Desc
        }).ToList();

        evt.Progress = new ProgressMessage
        {
            EventId = eventId,
            ProgressData = progressData
        };
        evt.ProgressPending = true;
    }

    public void SendLogMessage(
        string eventId,
        string log,
        string title,
        string level = "info",
        double? duration = 10,
        bool visible = true)
    {
        if (!_eventIdsToEvents.TryGetValue(eventId, out var evt) || !evt.Alive)
            return;

        var logMsg = new LogMessage
        {
            EventId = evt.Id,
            Log = log,
            Level = level,
            Duration = duration,
            Visible = visible,
            Title = title
        };

        if (_pendingMessagesPerSession.TryGetValue(evt.SessionHash, out var channel))
        {
            channel.Writer.TryWrite(logMsg);
        }
        else
        {
            _logger.LogWarning("[QueueManager.SendLogMessage] No session channel for session={Session}, log dropped.", evt.SessionHash);
        }
    }

    private async Task SendMessage(QueueEvent evt, EventMessage message)
    {
        if (!evt.Alive)
        {
            _logger.LogDebug("[QueueManager.SendMessage] Skipping msg={Msg} - event not alive session={Session} event={Event}",
                message.Msg, evt.SessionHash, evt.Id);
            return;
        }

        message.EventId = evt.Id;

        try
        {
            // Log abbreviated json (truncate large output fields)
            string logJson = JsonSerializer.Serialize(message, message.GetType(), _logJsonOptions);
            if (logJson.Length > 500) logJson = logJson[..500] + "...";
            _logger.LogInformation("[QueueManager.SendMessage] session={Session} event={Event} msg={Msg} json={Json}",
                evt.SessionHash, evt.Id, message.Msg, logJson);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("[QueueManager.SendMessage] Failed to serialize message for logging: {Ex}", ex.Message);
        }

        if (_pendingMessagesPerSession.TryGetValue(evt.SessionHash, out var channel))
        {
            await channel.Writer.WriteAsync(message);
        }
        else
        {
            _logger.LogWarning("[QueueManager.SendMessage] No session channel for session={Session}, message msg={Msg} dropped. Known sessions: [{Sessions}]",
                evt.SessionHash, message.Msg, string.Join(", ", _pendingMessagesPerSession.Keys));
        }
    }

    private static bool IsGenerating(Dictionary<string, object>? output)
    {
        if (output == null)
        {
            return false;
        }

        if (output.TryGetValue("is_generating", out var generatingObj) && generatingObj is bool generating)
        {
            return generating;
        }

        return false;
    }

    private static double? GetRemainingTimeLimit(BlockFunction fn, QueueEvent evt, double firstIterationSeconds)
    {
        if (!fn.TimeLimit.HasValue)
        {
            return null;
        }

        var remaining = fn.TimeLimit.Value - firstIterationSeconds;
        if (!string.Equals(fn.Connection, "stream", StringComparison.OrdinalIgnoreCase))
        {
            remaining = fn.TimeLimit.Value - evt.RunTimeSeconds;
        }

        return Math.Max(0.0, remaining);
    }

    private static async Task<bool> WaitForSignalOrTimeout(QueueEvent evt, double timeoutSeconds)
    {
        if (evt.Closed || !evt.Alive)
        {
            return false;
        }

        if (timeoutSeconds <= 0)
        {
            return false;
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            await evt.Signal.WaitAsync(cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private async Task BroadcastEstimations(string concurrencyId, int queueIndex)
    {
        // Calculate and broadcast queue position estimations
        if (!_eventQueuesPerConcurrencyId.TryGetValue(concurrencyId, out var queue))
        {
            return;
        }

        // Get average processing time for this concurrency group
        var avgProcessingTime = CalculateAverageProcessingTime(concurrencyId);

        // Broadcast estimation to each event in the queue
        lock (queue)
        {
            for (int i = 0; i < queue.QueueEvents.Count; i++)
            {
                var evt = queue.QueueEvents[i];
                if (evt.Alive && !evt.Closed)
                {
                    var estimatedWaitTime = avgProcessingTime * (i + 1 - queue.CurrentConcurrency);
                    var eta = estimatedWaitTime > 0 ? (double?)estimatedWaitTime : null;

                    _ = SendMessage(evt, new EstimationMessage
                    {
                        EventId = evt.Id,
                        Rank = i,
                        QueueSize = queue.Count,
                        RankEta = eta
                    });
                }
            }
        }

        await Task.CompletedTask;
    }

    private double CalculateAverageProcessingTime(string concurrencyId)
    {
        // Calculate average processing time from completed events
        var recentEvents = _eventAnalytics.Values
            .Where(e => e.Status == "success" && e.ProcessTime.HasValue)
            .OrderByDescending(e => e.Time)
            .Take(10)
            .ToList();

        if (recentEvents.Any())
        {
            return recentEvents.Average(e => e.ProcessTime ?? 1.0);
        }

        return 1.0; // Default to 1 second if no data
    }

    public Dictionary<string, object> GetCachedEventAnalyticsSummary()
    {
        var all = _eventAnalytics.Values
            .Where(e => e.Status is "success" or "failed")
            .ToList();

        if (all.Count == 0)
            return new Dictionary<string, object> { ["functions"] = new Dictionary<string, object>() };

        var grouped = all.GroupBy(e => e.Function ?? "unknown");
        var functions = new Dictionary<string, object>();

        foreach (var group in grouped)
        {
            var fnEvents = group.ToList();
            int success = fnEvents.Count(e => e.Status == "success");
            int total = fnEvents.Count;
            double? successRate = total > 0 ? (double)success / total : (double?)null;

            var times = fnEvents
                .Where(e => e.ProcessTime.HasValue)
                .Select(e => e.ProcessTime!.Value)
                .OrderBy(t => t)
                .ToArray();

            double p50 = 0, p90 = 0, p99 = 0;
            if (times.Length > 0)
            {
                p50 = times[Math.Max(0, (int)(times.Length * 0.50) - 1)];
                p90 = times[Math.Max(0, (int)(times.Length * 0.90) - 1)];
                p99 = times[Math.Max(0, (int)(times.Length * 0.99) - 1)];
            }

            functions[group.Key] = new Dictionary<string, object>
            {
                ["success_rate"] = successRate ?? (object)DBNull.Value,
                ["process_time_percentiles"] = new Dictionary<string, object>
                {
                    ["50th"] = p50,
                    ["90th"] = p90,
                    ["99th"] = p99
                },
                ["total_requests"] = total
            };
        }

        return new Dictionary<string, object> { ["functions"] = functions };
    }

    private async Task StartProgressUpdates()
    {
        while (!_stopped)
        {
            try
            {
                // Send progress updates for running events
                var runningEvents = _eventIdsToEvents.Values
                    .Where(e => e.StartedAt.HasValue && !e.CompletedAt.HasValue && e.Alive)
                    .ToList();

                foreach (var evt in runningEvents)
                {
                    // Python parity: only send progress messages, no periodic heartbeats during processing.
                    // Heartbeats are only sent by the SSE handler when the queue is idle for 15+ seconds.
                    if (evt.ProgressPending && evt.Progress != null)
                    {
                        await SendMessage(evt, evt.Progress);
                        evt.ProgressPending = false;
                    }
                }

                await Task.Delay(100);
            }
            catch (Exception ex)
            {
            }
        }
    }

    private async Task NotifyClients()
    {
        while (!_stopped)
        {
            try
            {
                // Python parity: periodically broadcast queue estimations only.
                if (TotalQueueSize > 0)
                {
                    foreach (var concurrencyId in _eventQueuesPerConcurrencyId.Keys)
                    {
                        await BroadcastEstimations(concurrencyId, 0);
                    }
                }

                await Task.Delay((int)(_updateIntervals * 1000));
            }
            catch (Exception ex)
            {
            }
        }
    }
}

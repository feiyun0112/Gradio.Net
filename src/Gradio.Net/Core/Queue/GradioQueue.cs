using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The outcome of asking the queue to run something, as the join endpoint reports it.
/// </summary>
/// <param name="Success">Whether the run was admitted.</param>
/// <param name="Detail">The event id when it was, and why it was not when it was not.</param>
/// <param name="State">Which refusal this was, which is what decides the status code.</param>
internal sealed record PushResult(bool Success, string Detail, string State)
{
    /// <summary>Refused because the application's queue is full.</summary>
    internal const string QueueFull = "queue_full";

    /// <summary>Refused because a validator rejected the input.</summary>
    internal const string ValidatorError = "validator_error";

    /// <summary>Refused because the request itself was wrong.</summary>
    internal const string Error = "error";

    /// <summary>Admitted.</summary>
    internal const string SuccessState = "success";
}

/// <summary>
/// The queue: where a binding's runs wait their turn, and where the messages that describe them to
/// a client come from.
/// </summary>
/// <remarks>
/// <para>
/// This is a port of the Python version's <c>queueing.Queue</c>, and it is deliberately a port
/// rather than a .NET-idiomatic redesign: the message sequence is the protocol the vendored
/// frontend consumes, so the scheduling rules that decide it — the grouping by concurrency id, the
/// limit that group carries, the order messages are pushed in, the coalescing of progress — are
/// part of the contract rather than implementation detail.
/// </para>
/// <para>
/// Two structures carry the state. Each concurrency group holds the events waiting for a slot, and
/// each session holds the channel its messages are written to. Everything under <c>_gate</c> is
/// touched from request threads and from the scheduler, so the lock is the boundary between them;
/// the channels are thread-safe on their own and are deliberately outside it.
/// </para>
/// <para>
/// The scheduler is started by the host, not by a first request: a queue that started lazily would
/// have to decide what a request that arrives first does, and the Python version starts it with the
/// application.
/// </para>
/// </remarks>
internal sealed class GradioQueue : IAsyncDisposable
{
    /// <summary>How many sessions' message channels are kept before the oldest is dropped.</summary>
    private const int SessionCapacity = 2000;

    private readonly Blocks _blocks;
    private readonly ISessionStore _sessions;
    private readonly StreamingOutputs _streaming;
    private readonly PendingStreams _streams;
    private readonly TimeProvider _clock;
    private readonly ProfilingOptions _profiling;
    private readonly GradioQueueStatus _status;
    private readonly RunHistoryRecorder? _history;
    private readonly EventAnalytics? _analytics;
    private readonly IAnalyticsSink? _analyticsSink;
    private readonly TraceCollector? _traces;
    private readonly GradioMetrics? _metrics;
    private readonly ILogger<GradioQueue> _logger;
    private readonly Action<ILogger, string, Exception?> _logValidatorFailed;
    private readonly Action<ILogger, string, Exception?> _logRunFailed;
    private readonly Action<ILogger, string, Exception?> _logUnloadFailed;

    /// <summary>Guards the groups, the events, the session id sets and the active job slots.</summary>
    private readonly object _gate = new();

    private readonly ConcurrentDictionary<string, Channel<JsonObject>> _messagesPerSession = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, HashSet<string>> _eventIdsPerSession = new(StringComparer.Ordinal);
    private readonly Dictionary<string, QueueEvent> _events = new(StringComparer.Ordinal);
    private readonly Dictionary<string, EventQueue> _groups = new(StringComparer.Ordinal);
    private readonly Dictionary<Dependency, ProcessTime> _processTime = [];
    private readonly Dictionary<Dependency, ProcessTime> _runTime = [];
    private readonly LinkedList<string> _sessionOrder = new();
    private readonly CancellationTokenSource _stopping = new();
    private QueueSettings _settings;
    private readonly TimeSpan _sleepWhenFree;
    private readonly TimeSpan _progressTick;

    private List<List<QueueEvent>?> _activeJobs = [];
    private Task? _processing;
    private Task? _progress;
    private Task? _notifications;
    private bool _stopped;
    private bool _started;
    private bool _disposed;

    /// <summary>Creates the queue for one application.</summary>
    /// <param name="blocks">The application whose bindings it runs.</param>
    /// <param name="sessions">The sessions whose state the runs read and write.</param>
    /// <param name="clock">The clock the estimates are measured with.</param>
    /// <param name="logger">Where the queue reports what it refuses.</param>
    /// <param name="streams">The streams the runs fill, which the playlist route reads.</param>
    /// <param name="profiling">Whether runs are traced.</param>
    /// <param name="status">Where this queue reports whether it is running, and how deep it is.</param>
    /// <param name="history">
    /// Where finished runs are recorded, or <see langword="null"/> for a queue whose runs are not.
    /// </param>
    /// <param name="analytics">
    /// Where each admitted run is described for the monitoring panel, or <see langword="null"/> for a
    /// queue nothing observes.
    /// </param>
    /// <param name="traces">
    /// Where a profiled run's trace goes, or <see langword="null"/> for an application that is not
    /// profiled.
    /// </param>
    /// <param name="metrics">
    /// Where the queue's depth and its requests' latency are published, or <see langword="null"/> for
    /// a queue that publishes nothing.
    /// </param>
    /// <param name="telemetry">
    /// Where a run that raised is reported, or <see langword="null"/> for a queue whose errors go no
    /// further than its log.
    /// </param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <remarks>
    /// The two sleeps are the Python version's, and they are platform-dependent there for a reason
    /// that carries over: a poll of a millisecond is cheap on a platform whose scheduler is, and
    /// the coarser one on Windows costs latency the progress loop would otherwise pay in wakeups.
    /// </remarks>
    /// <remarks>
    /// Public so that the container can build it: the built-in container only considers public
    /// constructors, and the type stays internal to the assembly.
    /// </remarks>
    public GradioQueue(
        Blocks blocks,
        ISessionStore sessions,
        TimeProvider clock,
        ILogger<GradioQueue> logger,
        PendingStreams streams,
        ProfilingOptions profiling,
        GradioQueueStatus status,
        RunHistoryRecorder? history = null,
        EventAnalytics? analytics = null,
        TraceCollector? traces = null,
        GradioMetrics? metrics = null,
        IAnalyticsSink? telemetry = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(streams);
        ArgumentNullException.ThrowIfNull(profiling);
        ArgumentNullException.ThrowIfNull(status);

        _blocks = blocks;
        _sessions = sessions;
        _clock = clock;
        _logger = logger;
        _streams = streams;
        _profiling = profiling;
        _status = status;
        _streaming = new StreamingOutputs(streams);
        _history = history;
        _analytics = analytics;
        _traces = traces;
        _metrics = metrics;
        _analyticsSink = telemetry;

        // A session the store dropped is a session nobody will play a stream of, so what it left
        // behind is forgotten rather than kept for a playlist no one will ask for.
        _sessions.Evicted += eviction => _streams.EndSession(eviction.Session.Hash);

        _logValidatorFailed = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "GradioQueueValidatorFailed"),
            "A validator for '{ApiName}' threw; the input is refused.");

        _logRunFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(2, "GradioQueueRunFailed"),
            "The queue failed while running '{ApiName}'.");

        _logUnloadFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(3, "GradioUnloadFailed"),
            "The handler bound to '{ApiName}' failed when the page went away.");

        // What the application asked for is read when the queue starts, not here: the container
        // builds the queue while the host is being assembled, which is before the application has
        // had its say — a `Queue(maxSize: …)` written between the two would otherwise be read by
        // nobody and silently do nothing. Until then the record holds its own defaults, so a queue
        // that was never started behaves like one nobody configured rather than like a third state.
        _settings = new QueueSettings();
        _sleepWhenFree = TimeSpan.FromMilliseconds(OperatingSystem.IsWindows() ? 50 : 1);
        _progressTick = TimeSpan.FromMilliseconds(OperatingSystem.IsWindows() ? 100 : 10);
    }

    /// <summary>The most events the queue holds, or <see langword="null"/> for no limit.</summary>
    internal int? MaxSize => _settings.MaxSize;

    /// <summary>How many events are waiting for a slot.</summary>
    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _groups.Values.Sum(group => group.Waiting.Count);
            }
        }
    }

    /// <summary>How many runs are holding a slot.</summary>
    internal int ActiveCount
    {
        get
        {
            lock (_gate)
            {
                return _activeJobs.Count(job => job is not null);
            }
        }
    }

    /// <summary>Whether the queue has been closed.</summary>
    internal bool Stopped => _stopped;

    /// <summary>Starts the scheduler loops.</summary>
    /// <param name="workerCount">How many runs may be in flight at once, across all groups.</param>
    /// <remarks>
    /// Called once, by the host, before it serves anything.
    /// </remarks>
    internal void Start(int workerCount)
    {
        if (_started)
        {
            return;
        }

        _started = true;
        _stopped = false;
        _activeJobs = [.. Enumerable.Repeat<List<QueueEvent>?>(null, Math.Max(1, workerCount))];

        // What the application asked for, read now rather than when the container built this queue.
        _settings = _blocks.QueueSettings;

        // What a probe and an exporter read: the queue says it is running, and says where its own
        // numbers are read from. Both are set here rather than in the constructor because a queue
        // that has not started has nothing waiting and nothing running.
        _status.Running = true;
        _status.Attach(() => Count, () => ActiveCount);

        _processing = Task.Run(() => ProcessingAsync(_stopping.Token));
        _progress = Task.Run(() => ProgressAsync(_stopping.Token));

        if (!_settings.LiveUpdates)
        {
            _notifications = Task.Run(() => NotificationsAsync(_stopping.Token));
        }
    }

    /// <summary>
    /// Admits a run, or refuses it and says why.
    /// </summary>
    /// <param name="body">What the client sent.</param>
    /// <param name="username">Who is asking, or <see langword="null"/>.</param>
    /// <returns>Whether the run was admitted, and either its id or the reason it was not.</returns>
    internal async Task<PushResult> PushAsync(PredictBody body, string? username)
    {
        ArgumentNullException.ThrowIfNull(body);

        if (body.FnIndex is not { } index || _blocks.DependencyAt(index) is not { } dependency)
        {
            return new PushResult(false, "No function index provided.", PushResult.Error);
        }

        if (MaxSize is { } max && Count >= max)
        {
            return new PushResult(false, $"Queue is full. Max size is {max} and size is {Count}.", PushResult.QueueFull);
        }

        Group(dependency);

        var state = SessionStateFor(body.SessionHash);

        if (dependency.Validator is { } validator
            && await ValidateAsync(dependency, validator, body, username, state).ConfigureAwait(false) is { } refusal)
        {
            return refusal;
        }

        var queueEvent = new QueueEvent(
            body.SessionHash ?? string.Empty,
            dependency,
            body,
            username,
            _clock,
            state);

        if (body.SessionHash is null)
        {
            body.SessionHash = queueEvent.SessionHash;
        }

        lock (_gate)
        {
            Session(queueEvent.SessionHash);
            EventIds(queueEvent.SessionHash).Add(queueEvent.Id);
            _events[queueEvent.Id] = queueEvent;
        }

        // The panel's record of this request, written at the moment it was admitted. What happens to
        // it afterwards is what the record's status follows; a request that is never scheduled keeps
        // saying it is queued, which is what happened to it.
        _analytics?.Enqueued(queueEvent.Id, dependency.ApiName, queueEvent.SessionHash, Now());

        // A batched run's id is the batch's, not any one event's: the client is told the id of the
        // batch it joined, and the individual events are never named to it.
        body.EventId = dependency.Batch ? null : queueEvent.Id;

        EventQueue group;
        lock (_gate)
        {
            group = _groups[dependency.ConcurrencyGroup];
            group.Waiting.Add(queueEvent);

            // The estimate is worked out while the queue still holds the event. The scheduler takes
            // its events under this same lock, so an estimate computed after it had taken this one
            // would describe a queue that no longer contains it and say nothing at all — and the
            // estimate is a message the client is owed: it is what the frontend draws a position
            // from, and what a client that joined a fast queue would otherwise never see.
            BroadcastEstimations(group, after: group.Waiting.Count - 1);
        }

        return new PushResult(true, queueEvent.Id, PushResult.SuccessState);
    }

    /// <summary>Drops an event that has not started yet.</summary>
    /// <param name="eventId">The event to drop.</param>
    /// <returns>A task that completes once it is gone.</returns>
    internal Task RemoveFromQueueAsync(string eventId)
    {
        lock (_gate)
        {
            if (!_events.TryGetValue(eventId, out var queueEvent))
            {
                return Task.CompletedTask;
            }

            _groups[queueEvent.ConcurrencyGroup].Waiting.Remove(queueEvent);
            _events.Remove(eventId);
            queueEvent.Dispose();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops describing a session's events, and drops the ones that have not started.
    /// </summary>
    /// <param name="sessionHash">The session to clean, or <see langword="null"/> to clean by event.</param>
    /// <param name="eventId">The event to clean, or <see langword="null"/> to clean by session.</param>
    /// <returns>A task that completes once the queue has been cleaned.</returns>
    /// <remarks>
    /// This is what a client that went away triggers: the runs it left behind have nobody to be
    /// described to, so they are marked dead and the queued ones are dropped. A run already in
    /// flight keeps going — there is no way to stop a .NET method from outside — but it stops
    /// producing messages, and it releases its slot when it returns.
    /// </remarks>
    internal Task CleanEventsAsync(string? sessionHash = null, string? eventId = null)
    {
        lock (_gate)
        {
            foreach (var job in _activeJobs)
            {
                if (job is null)
                {
                    continue;
                }

                foreach (var queued in job.Where(queued => Matches(queued, sessionHash, eventId)))
                {
                    queued.Alive = false;
                }
            }

            var removed = new List<QueueEvent>();
            foreach (var group in _groups.Values)
            {
                removed.AddRange(group.Waiting.Where(queued => Matches(queued, sessionHash, eventId)));
            }

            foreach (var queued in removed)
            {
                _groups[queued.ConcurrencyGroup].Waiting.Remove(queued);
                _events.Remove(queued.Id);
                queued.Dispose();
            }

            if (sessionHash is not null)
            {
                _streams.EndStreams(sessionHash);
            }

            if (sessionHash is not null && _eventIdsPerSession.TryGetValue(sessionHash, out var ids))
            {
                foreach (var queued in removed)
                {
                    ids.Remove(queued.Id);
                }

                if (ids.Count == 0)
                {
                    _eventIdsPerSession.TryRemove(sessionHash, out _);
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>Feeds a running event the next chunk the browser sent.</summary>
    /// <param name="eventId">The run to feed.</param>
    /// <param name="body">The chunk, as the browser sent it.</param>
    /// <returns>Whether the run was found.</returns>
    /// <remarks>
    /// A streamed run waits between steps for exactly this: the browser posts each chunk to
    /// <c>/stream/{event_id}</c>, and the run takes the next step of its handler when it arrives. A
    /// run that is not waiting still receives it — the signal is taken by the next wait — which is
    /// what makes a chunk that arrives early count rather than be lost.
    /// </remarks>
    internal bool PushStreamInput(string eventId, PredictBody body)
    {
        ArgumentNullException.ThrowIfNull(body);

        lock (_gate)
        {
            if (!_events.TryGetValue(eventId, out var queued))
            {
                return false;
            }

            queued.Body = body;
            queued.Signal();

            return true;
        }
    }

    /// <summary>Ends a running event, which is how the browser stops a stream.</summary>
    /// <param name="eventId">The run to end.</param>
    /// <returns>Whether the run was found.</returns>
    /// <remarks>
    /// The run is not cancelled: it is told that no more input is coming, so it finishes with what
    /// it has. That is the difference between this and <see cref="CancelEvent"/>, and the reason a
    /// stream ends with a completed message rather than with an error.
    /// </remarks>
    internal bool CloseStream(string eventId)
    {
        lock (_gate)
        {
            if (!_events.TryGetValue(eventId, out var queued))
            {
                return false;
            }

            queued.Closed = true;
            queued.Signal();

            return true;
        }
    }

    /// <summary>Stops a run, which a cancel request does.</summary>
    /// <param name="eventId">The run to stop.</param>
    /// <returns>Whether the run was found.</returns>
    /// <remarks>
    /// A .NET method cannot be stopped from outside, so what this does is signal the token the
    /// handler was called with: a handler that observes it stops where it is, and one that does not
    /// runs to completion with nobody listening for its result.
    /// </remarks>
    internal bool CancelEvent(string eventId)
    {
        lock (_gate)
        {
            if (!_events.TryGetValue(eventId, out var queued))
            {
                return false;
            }

            queued.Cancel();

            return true;
        }
    }

    /// <summary>Whether a session is still waiting on a run.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="eventId">The run.</param>
    /// <returns>Whether the session is waiting on it.</returns>
    internal bool SessionHasEvent(string sessionHash, string eventId)
    {
        lock (_gate)
        {
            return _eventIdsPerSession.TryGetValue(sessionHash, out var ids) && ids.Contains(eventId);
        }
    }

    /// <summary>Records that a run is over, and says whether it was the last one.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="eventId">The run that finished.</param>
    /// <returns>Whether the session is waiting on nothing now.</returns>
    /// <remarks>
    /// A run can complete twice — a client that cancels twice, say — so removing an id that is
    /// already gone is not a failure, and the answer is about what is left rather than about what
    /// this call removed.
    /// </remarks>
    internal bool CompleteEvent(string sessionHash, string eventId)
    {
        lock (_gate)
        {
            if (!_eventIdsPerSession.TryGetValue(sessionHash, out var ids))
            {
                return true;
            }

            ids.Remove(eventId);

            if (ids.Count > 0)
            {
                return false;
            }

            _eventIdsPerSession.TryRemove(sessionHash, out _);

            return true;
        }
    }

    /// <summary>What the whole queue is doing, as the status endpoint reports it.</summary>
    /// <returns>The message.</returns>
    internal JsonObject Status() => QueueMessages.QueueStatus(Count);

    /// <summary>The channel a session's messages are written to, creating it if it is new.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <returns>The channel's reader.</returns>
    internal ChannelReader<JsonObject> OpenSession(string sessionHash)
    {
        lock (_gate)
        {
            return Session(sessionHash).Reader;
        }
    }

    /// <summary>Whether a session's channel exists.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <returns>Whether the session is open.</returns>
    internal bool HasSession(string sessionHash) => _messagesPerSession.ContainsKey(sessionHash);

    /// <summary>Closes the session a page was holding, and runs what the application bound to it ending.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <returns>A task that completes once the session is gone.</returns>
    /// <remarks>
    /// <para>
    /// The session's state is marked closed rather than dropped: the page has gone, but the user may
    /// come back, and a reload that found nothing would look like the application had forgotten
    /// them. How long it is kept is <see cref="SessionState.ClosedTimeToLive"/>.
    /// </para>
    /// <para>
    /// This is also the moment the application's <c>unload</c> bindings run, because it is the
    /// moment the page is known to be gone. A page holds two connections that can end — the stream
    /// that carries its runs, and the heartbeat that says it is alive — and both call this, so the
    /// work happens once: the first call is the one that finds the session open.
    /// </para>
    /// <para>
    /// A session that no page ever held is <see cref="DiscardSessionAsync"/> instead: an agent
    /// calling a tool is not a visitor leaving.
    /// </para>
    /// </remarks>
    internal Task CloseSessionAsync(string sessionHash) => CloseAsync(sessionHash, pageGone: true);

    /// <summary>Closes a session that no page was holding, running nothing.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <returns>A task that completes once the session is gone.</returns>
    /// <remarks>
    /// The sessions an agent's tool call runs in are made for the call and dropped with it: nothing
    /// was bound to a page ending, because there was no page. What is left to do is what
    /// <see cref="CloseSessionAsync"/> does for a page's session minus the work an application binds
    /// to a visitor leaving.
    /// </remarks>
    internal Task DiscardSessionAsync(string sessionHash) => CloseAsync(sessionHash, pageGone: false);

    /// <summary>Closes a session, and says whether a page was holding it.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="pageGone">Whether a page was holding this session, and has now gone.</param>
    /// <returns>A task that completes once the session is gone.</returns>
    private async Task CloseAsync(string sessionHash, bool pageGone)
    {
        if (_messagesPerSession.TryRemove(sessionHash, out var channel))
        {
            channel.Writer.TryComplete();
        }

        lock (_gate)
        {
            _sessionOrder.Remove(sessionHash);
        }

        var state = StateOf(sessionHash);

        // An application that runs something when a visitor leaves needs somewhere to remember that
        // it did, and the session's own flag is that memory — so a page whose visitor never ran
        // anything still gets the session it is closing, rather than a second run on the next close.
        // The store bounds it like any other session, by capacity, which is the same bound the Python version's
        // own holder puts on the sessions it keeps after their page has gone.
        if (state is null && pageGone && _blocks.BindsUnload)
        {
            state = _sessions.GetOrAdd(sessionHash).State;
        }

        await CleanEventsAsync(sessionHash: sessionHash).ConfigureAwait(false);

        if (state is null)
        {
            return;
        }

        // Every session that ends is closed, which is what releases its values on the closed
        // session's clock. The first close is the end of it; the ones after that are the same end
        // arriving twice. Only a page's session has work bound to that moment.
        var ended = state.TryClose();

        if (ended && pageGone)
        {
            await RunUnloadAsync(state).ConfigureAwait(false);
        }
    }

    /// <summary>The values a session holds, or <see langword="null"/> when the store has no session under that name.</summary>
    /// <param name="sessionHash">The session.</param>
    private SessionState? StateOf(string sessionHash) =>
        _sessions.TryGet(sessionHash, out var session) ? session.State : null;

    /// <summary>Runs what the application bound to the end of a session.</summary>
    /// <param name="state">The values of the session that ended.</param>
    /// <returns>A task that completes once every handler has run.</returns>
    /// <remarks>
    /// <para>
    /// The runs are not queued: there is no client left to be told how they went, and a queue slot
    /// spent on one would be a slot the next visitor waits for. They are also not cancelled with the
    /// request that noticed the end — that request is already aborted, and the work being done is
    /// the work the abort is about.
    /// </para>
    /// <para>
    /// What a handler reads is the session's own state and the last values its session was shown:
    /// the page cannot send anything, and a handler that asked for a component's value would
    /// otherwise read nothing at all.
    /// </para>
    /// </remarks>
    private async Task RunUnloadAsync(SessionState state)
    {
        foreach (var dependency in _blocks.UnloadBindings)
        {
            if (dependency.Handler is null)
            {
                continue;
            }

            var run = new DependencyRun(dependency.Shape, _clock, state);

            try
            {
                var inputs = LastValuesOf(dependency.Inputs, state);

                // One tracker for the whole run rather than one per chunk: a handler that reports
                // progress reports it to the same place every time, whether or not anyone reads it.
                var progress = new Progress();
                var output = await ChunkAsync(run, inputs, progress).ConfigureAwait(false);

                // A streaming handler is drained rather than abandoned: its chunks are for nobody,
                // but the handler's own `finally` runs only when its enumerator is finished.
                while (output.IsGenerating)
                {
                    output = await ChunkAsync(run, inputs, progress).ConfigureAwait(false);
                }
            }
            catch (Exception failure)
            {
                // One handler that fails is not a reason to skip the others: they were bound by one
                // application for one moment, and that moment is over either way.
                _logUnloadFailed(_logger, dependency.ApiName, failure);
            }
            finally
            {
                await run.ResetAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>Takes one chunk of a run nobody is watching.</summary>
    /// <param name="run">The run.</param>
    /// <param name="inputs">The values the handler's inputs carry.</param>
    /// <param name="progress">Where a handler that reports progress reports it.</param>
    /// <returns>What the run produced.</returns>
    /// <remarks>
    /// The two runs that are not the queue's are the validator's, whose verdict is the answer, and
    /// the work an application does when a page goes away, whose answer nobody reads. Neither has a
    /// client to cancel for, so neither is given a cancellation token of anybody's.
    /// </remarks>
    private static Task<RunOutput> ChunkAsync(DependencyRun run, IReadOnlyList<JsonArray> inputs, Progress progress) =>
        run.NextAsync(inputs, batched: false, progress, eventData: null, CancellationToken.None);

    /// <summary>The values a session's components were last seen carrying, one array per input.</summary>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="state">The values of the session that ended.</param>
    /// <returns>One single-value array per input, empty for a value the session never saw.</returns>
    /// <remarks>
    /// The session remembers what a run read and what it produced, so what is replayed here is the
    /// last value each component was shown — the only thing left of a page that cannot answer. A
    /// component the session never saw, because nothing ran, reads nothing.
    /// </remarks>
    private static List<JsonArray> LastValuesOf(IReadOnlyList<Block> inputs, SessionState state)
    {
        var values = new List<JsonArray>(inputs.Count);

        foreach (var input in inputs)
        {
            var single = new JsonArray();

            if (state.TryRecall(input.Id, out var value))
            {
                single.Add(value?.DeepClone());
            }

            values.Add(single);
        }

        return values;
    }

    /// <summary>Puts a message on a session's channel.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="message">The message.</param>
    /// <returns>Whether the session was open to receive it.</returns>
    /// <remarks>
    /// A message for a session that is not open is dropped rather than opening one: the session is
    /// what a client's connection created, and a message is not a reason to keep one alive.
    /// </remarks>
    internal bool Enqueue(string sessionHash, JsonObject message)
    {
        if (!_messagesPerSession.TryGetValue(sessionHash, out var channel))
        {
            return false;
        }

        return channel.Writer.TryWrite(message);
    }

    /// <summary>Reports progress for a run, which is coalesced rather than sent.</summary>
    /// <param name="eventId">The run reporting.</param>
    /// <param name="units">One entry per thing the run reports on.</param>
    internal void SetProgress(string eventId, JsonArray units)
    {
        lock (_gate)
        {
            foreach (var job in _activeJobs)
            {
                if (job is null)
                {
                    continue;
                }

                foreach (var queued in job.Where(queued => queued.Id == eventId))
                {
                    queued.PendingProgress = units;
                    queued.ProgressPending = true;
                }
            }
        }
    }

    /// <summary>Stops the scheduler and releases the sessions.</summary>
    /// <returns>A task that completes once everything has stopped.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _stopped = true;
        _status.Running = false;

        await _stopping.CancelAsync().ConfigureAwait(false);

        foreach (var loop in new[] { _processing, _progress, _notifications })
        {
            if (loop is null)
            {
                continue;
            }

            try
            {
                await loop.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // The loop was cancelled, which is how it stops.
            }
        }

        _stopping.Dispose();

        foreach (var channel in _messagesPerSession.Values)
        {
            channel.Writer.TryComplete();
        }

        List<QueueEvent> remaining;
        lock (_gate)
        {
            remaining = [.. _events.Values];
            _events.Clear();
        }

        foreach (var queued in remaining)
        {
            queued.Dispose();
        }
    }

    /// <summary>Whether an event is the one named, by session or by id.</summary>
    private static bool Matches(QueueEvent queued, string? sessionHash, string? eventId) =>
        (sessionHash is not null && queued.SessionHash == sessionHash)
        || (eventId is not null && queued.Id == eventId);

    /// <summary>The limit the application's default resolves to.</summary>
    /// <remarks>
    /// The application's setting wins over the environment, which is the Python version's order,
    /// and the environment exists so that a deployment can cap an application without changing its
    /// code.
    /// </remarks>
    private static int? ResolveDefaultLimit(QueueSettings settings)
    {
        if (settings.DefaultConcurrencyLimit.Value is { } configured)
        {
            return configured < 0 ? null : configured;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("GRADIO_DEFAULT_CONCURRENCY_LIMIT");

        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment.Trim().Equals("none", StringComparison.OrdinalIgnoreCase)
                ? null
                : int.Parse(fromEnvironment.Trim(), CultureInfo.InvariantCulture);
        }

        return 1;
    }

    /// <summary>The values a session holds, opening the session when the client named one.</summary>
    /// <param name="sessionHash">The session the client named, or <see langword="null"/>.</param>
    /// <returns>The session's values, or <see langword="null"/> when the client named no session.</returns>
    /// <remarks>
    /// A caller that named no session gets no session of its own here: the run still works, because
    /// a run without a session keeps a session's worth of state to itself, but nothing outlives it.
    /// The routes require a session, so that is the direct-call path rather than the served one.
    /// </remarks>
    private SessionState? SessionStateFor(string? sessionHash) =>
        sessionHash is { Length: > 0 } ? _sessions.GetOrAdd(sessionHash).State : null;

    /// <summary>The session's channel, creating it if this is the first time it is named.</summary>
    private Channel<JsonObject> Session(string sessionHash)
    {
        if (_messagesPerSession.TryGetValue(sessionHash, out var existing))
        {
            return existing;
        }

        var channel = Channel.CreateUnbounded<JsonObject>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

        _messagesPerSession[sessionHash] = channel;
        _sessionOrder.AddLast(sessionHash);

        while (_sessionOrder.Count > SessionCapacity)
        {
            var oldest = _sessionOrder.First!.Value;
            _sessionOrder.RemoveFirst();

            if (_messagesPerSession.TryRemove(oldest, out var dropped))
            {
                dropped.Writer.TryComplete();
            }
        }

        return channel;
    }

    /// <summary>The ids a session is waiting on, creating the set if this is the first one.</summary>
    /// <remarks>
    /// Called under <c>_gate</c>, because the set itself is not thread-safe: the dictionary around it
    /// is, so that a session that has never been seen can be given its set without a second lock.
    /// </remarks>
    private HashSet<string> EventIds(string sessionHash) =>
        _eventIdsPerSession.GetOrAdd(sessionHash, static _ => new HashSet<string>(StringComparer.Ordinal));

    /// <summary>Makes sure the group a binding belongs to exists, and holds the lowest limit set.</summary>
    /// <remarks>
    /// The limit a group carries is the lowest any of its bindings asked for, because the group is
    /// what the limit applies to: a group is a set of bindings that share a resource, so the
    /// binding that is most careful about that resource decides for all of them.
    /// </remarks>
    private void Group(Dependency dependency)
    {
        var limit = dependency.ConcurrencyLimit.Value is { } declared
            ? declared < 0 ? null : declared
            : ResolveDefaultLimit(_settings);

        lock (_gate)
        {
            if (!_groups.TryGetValue(dependency.ConcurrencyGroup, out var group))
            {
                _groups[dependency.ConcurrencyGroup] = new EventQueue(dependency.ConcurrencyGroup, limit);

                return;
            }

            if (limit is not null && (group.Limit is null || limit < group.Limit))
            {
                group.Limit = limit;
            }
        }
    }

    /// <summary>Runs a binding's validator over the input, before anything is queued.</summary>
    /// <param name="dependency">The binding whose validator to run.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="body">What the client sent.</param>
    /// <param name="userName">Who is asking, or <see langword="null"/>.</param>
    /// <param name="state">The session's values, or <see langword="null"/> when the caller named no session.</param>
    /// <returns>How the input was refused, or <see langword="null"/> when it was accepted.</returns>
    /// <remarks>
    /// <para>
    /// A validator returns nothing to accept and something to reject, which is the contract ticket
    /// 07 set for it. It runs outside the queue, which is the Python version's rule: a request that
    /// will be refused should be refused without taking a slot.
    /// </para>
    /// <para>
    /// The validator is told who is calling, as the handler it guards will be: the Python version
    /// hands it the same request, so a validator that refuses an input on a per-user rule can.
    /// </para>
    /// <para>
    /// A validator that raises is not the same as one that answers "no", and the two are refused
    /// with different statuses: a rejection is the endpoint doing its job, while a raise is the
    /// server failing to answer. The Python version distinguishes them at exactly this point.
    /// </para>
    /// </remarks>
    private async Task<PushResult?> ValidateAsync(
        Dependency dependency,
        Delegate validator,
        PredictBody body,
        string? userName,
        SessionState? state)
    {
        var inputs = new List<JsonArray>();
        foreach (var value in body.Data)
        {
            inputs.Add([value?.DeepClone()]);
        }

        // A run with no outputs has nowhere to put what its handler answered, and a validator's
        // answer is the whole point of running it — so the run is given one output to carry it, and
        // the answer is read from that slot. The Python version reads the validator's return value directly;
        // here it comes back through the same postprocessing path every result takes, with the
        // postprocessing switched off.
        IReadOnlyList<Block> carrier = dependency.Inputs.Count > 0 ? [dependency.Inputs[0]] : [];

        var run = new DependencyRun(
            new RunShape(
                validator,
                dependency.Inputs,
                carrier,
                Preprocess: true,
                Postprocess: false,
                TrackedStateIds: [],

                // The answer is a verdict, not a value any client saw: remembering it would put
                // "too short" into the session as the field's contents.
                Remembers: false),
            _clock,
            state,
            new Request(userName, body.SessionHash));

        try
        {
            var result = await ChunkAsync(run, inputs, new Progress()).ConfigureAwait(false);
            var value = result.Data.Count > 0 ? result.Data[0] : null;
            var rejection = value?.ToJsonString();

            return rejection is null
                ? null
                : new PushResult(false, rejection, PushResult.ValidatorError);
        }
        catch (Exception error)
        {
            _logValidatorFailed(_logger, dependency.ApiName, error);

            return new PushResult(false, error.Message, PushResult.Error);
        }
    }

    /// <summary>Takes the next run the queue can start, if there is one.</summary>
    /// <param name="batch">Whether the run is a batch.</param>
    /// <returns>The events to run together, or <see langword="null"/> when nothing can start.</returns>
    /// <remarks>
    /// The groups are visited in a random order so that one busy group cannot starve the others by
    /// always being first; within a group the head of the queue goes first, and a batching binding
    /// takes as many of the events behind it as its batch size allows.
    /// </remarks>
    private List<QueueEvent>? NextEvents(out bool batch)
    {
        batch = false;

        var ids = _groups.Keys.ToArray();
        Random.Shared.Shuffle(ids);

        foreach (var id in ids)
        {
            var group = _groups[id];

            if (group.Waiting.Count == 0 || (group.Limit is { } limit && group.CurrentConcurrency >= limit))
            {
                continue;
            }

            var first = group.Waiting[0];
            var events = new List<QueueEvent> { first };
            batch = first.Dependency.Batch;

            if (batch)
            {
                events.AddRange(group.Waiting
                    .Skip(1)
                    .Where(queued => ReferenceEquals(queued.Dependency, first.Dependency))
                    .Take(first.Dependency.MaxBatchSize - 1));
            }

            foreach (var queued in events)
            {
                group.Waiting.Remove(queued);
            }

            return events;
        }

        return null;
    }

    /// <summary>The scheduler: hands runs to free slots for as long as the queue is open.</summary>
    private async Task ProcessingAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!_stopped)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (Count == 0 || !HasFreeSlot())
                {
                    await Task.Delay(_sleepWhenFree, cancellationToken).ConfigureAwait(false);

                    continue;
                }

                List<QueueEvent>? events;
                bool batch;

                lock (_gate)
                {
                    events = NextEvents(out batch);
                }

                if (events is null)
                {
                    await Task.Delay(_sleepWhenFree, cancellationToken).ConfigureAwait(false);

                    continue;
                }

                Begin(events, batch);

                if (_settings.LiveUpdates)
                {
                    BroadcastEstimations(_groups[events[0].ConcurrencyGroup]);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Stopping.
        }
        finally
        {
            _stopped = true;
        }
    }

    /// <summary>Claims a slot for a run and starts it.</summary>
    /// <param name="events">The events to run together.</param>
    /// <param name="batch">Whether the handler takes them as one batch.</param>
    /// <remarks>
    /// The slot is registered with this exact list, because that is what releases it: the run's own
    /// bookkeeping narrows the list as clients go away, so the two cannot be the same object.
    /// </remarks>
    private void Begin(List<QueueEvent> events, bool batch)
    {
        var dependency = events[0].Dependency;
        double beginTime;
        long startedAt;

        lock (_gate)
        {
            var slot = _activeJobs.IndexOf(null);
            if (slot < 0)
            {
                return;
            }

            _activeJobs[slot] = events;

            var group = _groups[dependency.ConcurrencyGroup];
            group.CurrentConcurrency++;

            beginTime = Now();
            startedAt = _clock.GetTimestamp();
            group.StartTimesPerDependency.TryGetValue(dependency, out var starts);
            starts ??= [];
            starts.Add(beginTime);
            group.StartTimesPerDependency[dependency] = starts;
        }

        _ = ProcessEventsAsync(events, batch, beginTime, startedAt, dependency);
    }

    /// <summary>Whether any run slot is free.</summary>
    private bool HasFreeSlot()
    {
        lock (_gate)
        {
            return _activeJobs.Contains(null);
        }
    }

    /// <summary>Runs a group of events and describes what happened to each of them.</summary>
    /// <param name="scheduled">
    /// The events the slot was claimed for. The list is the one the slot was registered with, and it
    /// is what the slot is released by, so it is kept apart from the events that are still alive to
    /// be described.
    /// </param>
    /// <param name="batch">Whether the handler takes them as one batch.</param>
    /// <param name="beginTime">When the slot was claimed, for the estimate the waiting clients see.</param>
    /// <param name="startedAt">The same moment, as a clock reading the run's duration is measured from.</param>
    /// <param name="dependency">The binding being run.</param>
    /// <remarks>
    /// The shape is the Python version's, message for message: starts, then either a completed
    /// message or a run of generating messages followed by a completed one, with an error described
    /// as a completed message that says it failed.
    /// </remarks>
    private async Task ProcessEventsAsync(
        List<QueueEvent> scheduled,
        bool batch,
        double beginTime,
        long startedAt,
        Dependency dependency)
    {
        var awake = new List<QueueEvent>();
        RequestTrace? trace = null;
        RunOutput? response = null;

        try
        {
            foreach (var queued in scheduled)
            {
                if (!queued.Alive)
                {
                    continue;
                }

                Send(queued, QueueMessages.ProcessStarts(queued.Id, AverageProcessTimeOf(dependency)));
                _analytics?.Started(queued.Id);
                awake.Add(queued);
            }

            if (awake.Count == 0)
            {
                return;
            }

            var events = awake;
            var inputs = InputsOf(events, batch);
            var progress = new Progress(units => SetProgress(events[0].Id, units));
            trace = BeginTrace(dependency, events[0]);

            // What the handler threw, if it did. The failure is described where it is caught and
            // reported where the run's messages are written, which are two places, so it travels.
            Exception? failed = null;

            // Taken before the run rather than after it: a record says when the run started, and the
            // time a write happened to reach the recorder is a different fact.
            var beganAt = RunHistoryKeys.Now();

            // The session is open for the whole run, not just for the probe: what the run computes is
            // filed under it, and a scope that closed before the computation would put a per-session
            // cache's answers in the namespace shared by everybody.
            var cached = dependency.Handler?.Target as ICachedHandler;
            using var session = cached?.Cache.OpenSession(events[0].SessionHash);

            try
            {
                // A binding whose handler is cached is asked first whether the answer is already
                // known. A hit is answered here, without the run reaching the handler's body — and
                // without a slot being held for work that is not going to happen. A miss is the
                // ordinary path below, which computes the answer and keeps it.
                if (await ProbeAsync(events[0], cached, inputs, batch, progress).ConfigureAwait(false) is { } hit)
                {
                    // A cached stream is replayed as the sequence it was: the chunks before the last
                    // one are sent as generating messages and the last one as the completion, which
                    // is the same shape a computed stream has.
                    for (var chunk = 0; chunk < hit.Count - 1; chunk++)
                    {
                        for (var index = 0; index < awake.Count; index++)
                        {
                            Send(awake[index], QueueMessages.ProcessGenerating(
                                awake[index].Id,
                                Payload(hit[chunk], index, batch, dependency),
                                success: true));
                        }
                    }

                    for (var index = 0; index < awake.Count; index++)
                    {
                        Send(awake[index], Completed(
                            awake[index],
                            Payload(hit[^1], index, batch, dependency, usedCache: "full"),
                            success: true));
                    }

                    // The answer the run was going to compute is the one it replayed, so the run is
                    // described as having produced it: the panel and the latency histogram see a
                    // request that was answered, which is what happened to it.
                    response = hit[^1];

                    return;
                }

                response = await RunAsync(events[0], inputs, batch, progress).ConfigureAwait(false);
                response = await _streaming.ApplyAsync(response, dependency, events[0]).ConfigureAwait(false);
                Measure(dependency, response);
            }
            catch (Exception failure)
            {
                Failed(dependency, failure);
                failed = failure;
                response = null;

                foreach (var queued in awake)
                {
                    Send(queued, Completed(queued, ErrorPayload(failure), success: false, title: "Error"));
                }
            }

            if (response is { IsGenerating: true })
            {
                while (response is { IsGenerating: true })
                {
                    for (var index = 0; index < awake.Count; index++)
                    {
                        Send(awake[index], QueueMessages.ProcessGenerating(
                            awake[index].Id,
                            Payload(response, index, batch, dependency),
                            success: true));
                    }

                    awake.RemoveAll(queued => !queued.Alive);
                    if (awake.Count == 0)
                    {
                        return;
                    }

                    if (awake[0].Streaming && !await WaitForChunkAsync(awake[0], dependency).ConfigureAwait(false))
                    {
                        Send(awake[0], Completed(
                            awake[0],
                            response is null ? ErrorPayload(failed) : Payload(response, 0, batch, dependency),
                            success: response is not null));

                        return;
                    }

                    try
                    {
                        response = await RunAsync(awake[0], inputs, batch, progress).ConfigureAwait(false);
                        response = await _streaming.ApplyAsync(response, dependency, awake[0]).ConfigureAwait(false);
                        Measure(dependency, response);
                    }
                    catch (Exception failure)
                    {
                        Failed(dependency, failure);
                        failed = failure;
                        response = null;
                    }
                }

                if (response is not null)
                {
                    for (var index = 0; index < awake.Count; index++)
                    {
                        Send(awake[index], Completed(
                            awake[index],
                            Payload(response, index, batch, dependency),
                            success: true));
                    }
                }
                else
                {
                    foreach (var queued in awake)
                    {
                        Send(queued, Completed(queued, ErrorPayload(failed), success: false, title: "Error"));
                    }
                }
            }
            else if (response is not null)
            {
                for (var index = 0; index < awake.Count; index++)
                {
                    Send(awake[index], Completed(
                        awake[index],
                        Payload(response, index, batch, dependency),
                        success: true));
                }
            }

            if (response is { IsGenerating: false })
            {
                // A finished run is handed to the history recorder, which decides whether it is one
                // worth keeping: a public endpoint's last answer is, and half a stream is not. The
                // write happens after the answer has been sent, so a run never waits for a receipt.
                _history?.Record(dependency, SubmittedInputs(events, batch), response.Data, beganAt);
            }

            if (response is not null)
            {
                var duration = _clock.GetElapsedTime(startedAt).TotalSeconds;
                lock (_gate)
                {
                    ProcessTimeOf(_processTime, dependency).Add(duration);
                }
            }
        }
        catch (Exception error)
        {
            Failed(dependency, error);
        }
        finally
        {
            // A generator still holds an enumerator, and that enumerator still holds whatever the
            // handler opened before it yielded. Closing it is what lets a cancelled or abandoned
            // run reach its own `finally`, and what the Python version does with its iterators.
            foreach (var queued in scheduled)
            {
                await queued.Run.ResetAsync().ConfigureAwait(false);
                _streaming.EndRun(queued);
            }

            Describe(scheduled, awake, dependency, response, startedAt, trace);

            Release(scheduled, dependency, beginTime);
        }
    }

    /// <summary>
    /// Writes a run's failure to the log, and reports it to the sink if the application asked for
    /// one.
    /// </summary>
    /// <param name="dependency">The binding that failed.</param>
    /// <param name="failure">What it threw.</param>
    /// <remarks>
    /// <para>
    /// One place for both because a run can fail in three of them — the first chunk of a stream, a
    /// later one, and the loop around both — and a report that only happened at one of them would
    /// describe an application that fails less often than it does.
    /// </para>
    /// <para>
    /// The message rather than the exception, because the payload is a report and not a dump: it is
    /// the same sentence the operator's log carries, and a stack trace would say which machine the
    /// application runs on.
    /// </para>
    /// </remarks>
    private void Failed(Dependency dependency, Exception failure)
    {
        // A refusal that said it was not to be printed is already on its way to the user, which is
        // the Python version's rule and the difference between a validator and a crash.
        if (ErrorPayloads.IsLogWorthy(failure))
        {
            _logRunFailed(_logger, dependency.ApiName, failure);
        }

        ApplicationAnalytics.Failed(_blocks, _analyticsSink, failure.Message);
    }

    /// <summary>
    /// Tells the observers how the run went: the panel's records, the latency histogram, and the
    /// trace if one was started.
    /// </summary>
    /// <param name="scheduled">The events the slot was claimed for.</param>
    /// <param name="awake">The ones that were still being described when the run ended.</param>
    /// <param name="dependency">The binding that ran.</param>
    /// <param name="response">What the run produced, or <see langword="null"/> when it produced nothing.</param>
    /// <param name="startedAt">When the slot was claimed.</param>
    /// <param name="trace">The run's trace, or <see langword="null"/> when it is not profiled.</param>
    /// <remarks>
    /// <para>
    /// A run that was never described to anybody is <c>cancelled</c> rather than <c>failed</c>: the
    /// client left, which is not the handler's fault, and a panel that counted those as errors would
    /// report an application as broken every time somebody closed a tab.
    /// </para>
    /// <para>
    /// The processing time is only known for a run that produced an answer, which is the Python
    /// version's rule and the reason a failed run reports nothing rather than zero: a duration of
    /// zero would be read as an instant answer.
    /// </para>
    /// </remarks>
    private void Describe(
        List<QueueEvent> scheduled,
        List<QueueEvent> awake,
        Dependency dependency,
        RunOutput? response,
        long startedAt,
        RequestTrace? trace)
    {
        var elapsed = _clock.GetElapsedTime(startedAt).TotalSeconds;

        foreach (var queued in scheduled)
        {
            var status = !awake.Contains(queued) || queued.Cancelled
                ? AnalyticsStatus.Cancelled
                : response is null
                    ? AnalyticsStatus.Failed
                    : AnalyticsStatus.Success;

            _analytics?.Finished(queued.Id, status, response is null ? null : elapsed);
            _metrics?.RecordRequest(
                dependency.ApiName,
                status,
                _clock.GetElapsedTime(queued.EnqueueTimestamp).TotalSeconds);
        }

        if (trace is not null)
        {
            _traces?.Add(trace);
        }
    }

    /// <summary>Starts the trace of a run that has claimed a slot.</summary>
    /// <param name="dependency">The binding being run.</param>
    /// <param name="queued">The event whose id and session the trace carries.</param>
    /// <returns>The trace, or <see langword="null"/> when the application is not profiled.</returns>
    /// <remarks>
    /// The queue wait is measured here rather than inside the run, because the wait happened before
    /// the run existed: it is the difference between the moment the client joined and the moment a
    /// slot was found for it.
    /// </remarks>
    private RequestTrace? BeginTrace(Dependency dependency, QueueEvent queued)
    {
        if (_traces is null || !_profiling.Enabled)
        {
            return null;
        }

        var trace = new RequestTrace(queued.Id, dependency.ApiName, queued.SessionHash, Now());

        trace.Add(
            ProfilingPhases.QueueWait,
            _clock.GetElapsedTime(queued.EnqueueTimestamp).TotalMilliseconds);

        queued.Run.Trace = trace;

        return trace;
    }

    /// <summary>Waits for the browser to send the next chunk of a streamed run.</summary>
    /// <param name="queued">The event the run belongs to.</param>
    /// <param name="dependency">The binding being run.</param>
    /// <returns>
    /// <see langword="false"/> when the browser closed the stream or the wait ran out, which is when
    /// the run is over; <see langword="true"/> when there is another chunk to take.
    /// </returns>
    /// <remarks>
    /// A run that waits out its time limit without a chunk ends, which is the Python version's rule
    /// for the browser that went away without closing: the alternative is a handler that is
    /// advanced forever on input that will never arrive.
    /// </remarks>
    private static async Task<bool> WaitForChunkAsync(QueueEvent queued, Dependency dependency)
    {
        var signalled = await queued
            .WaitForSignalAsync(StreamingOutputs.WaitOf(dependency), CancellationToken.None)
            .ConfigureAwait(false);

        return signalled && !queued.Closed;
    }

    /// <summary>Releases the slot a run held and forgets the events it was holding.</summary>
    private void Release(List<QueueEvent> events, Dependency dependency, double beginTime)
    {
        lock (_gate)
        {
            var group = _groups[dependency.ConcurrencyGroup];
            group.CurrentConcurrency--;

            if (group.StartTimesPerDependency.TryGetValue(dependency, out var starts))
            {
                starts.Remove(beginTime);
                if (starts.Count == 0)
                {
                    group.StartTimesPerDependency.Remove(dependency);
                }
            }

            var slot = _activeJobs.FindIndex(job => ReferenceEquals(job, events));
            if (slot >= 0)
            {
                _activeJobs[slot] = null;
            }

            foreach (var queued in events)
            {
                _events.Remove(queued.Id);
                queued.Dispose();
            }
        }
    }

    /// <summary>The values the client sent, as a record keeps them.</summary>
    /// <param name="events">The events of the run.</param>
    /// <param name="batch">Whether the events were a batch.</param>
    /// <returns>
    /// The values of the one event, or one array of values per event for a batched run.
    /// </returns>
    /// <remarks>
    /// A batch is recorded as the batch it was: a record of a run that was given four samples and
    /// answered four times, filed as if it had been given one, would describe a different run.
    /// </remarks>
    private static JsonArray SubmittedInputs(List<QueueEvent> events, bool batch)
    {
        if (!batch)
        {
            return (JsonArray)events[0].Body.Data.DeepClone();
        }

        var samples = new JsonArray();

        foreach (var queued in events)
        {
            samples.Add(queued.Body.Data.DeepClone());
        }

        return samples;
    }

    /// <summary>The raw values the handler is called with, one array per input component.</summary>
    private static List<JsonArray> InputsOf(List<QueueEvent> events, bool batch)
    {
        var inputs = new List<JsonArray>();

        for (var index = 0; index < events[0].Body.Data.Count; index++)
        {
            var values = new JsonArray();
            foreach (var queued in events)
            {
                values.Add(index < queued.Body.Data.Count ? queued.Body.Data[index]?.DeepClone() : null);
            }

            inputs.Add(values);
        }

        return inputs;
    }

    /// <summary>Runs the handler once, or takes the next chunk of a streamed run.</summary>
    /// <remarks>
    /// The event's own payload is handed to the run as well as the values, because a handler may
    /// name one of the event-data classes: what the browser said about the thing it acted on is not
    /// something any input component carries. A streamed run reads the newest body, which is the
    /// chunk the browser last sent.
    /// </remarks>
    private static Task<RunOutput> RunAsync(
        QueueEvent queued,
        IReadOnlyList<JsonArray> inputs,
        bool batch,
        Progress progress) =>
        queued.Run.NextAsync(inputs, batch, progress, queued.Body.EventData, queued.CancellationToken);

    /// <summary>Asks a cached binding whether the answer is already known.</summary>
    /// <param name="queued">The event the run belongs to.</param>
    /// <param name="cached">The binding's cache, or <see langword="null"/> when it has none.</param>
    /// <param name="inputs">The values the client sent.</param>
    /// <param name="batch">Whether the values are a batch.</param>
    /// <param name="progress">The tracker to hand a handler that asks for one.</param>
    /// <returns>
    /// The run's chunks — one, for a run that is not a stream — or <see langword="null"/> when the
    /// answer has to be computed.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The run is the ordinary one, entered in probe mode: everything before the handler's body
    /// happens — the inputs are preprocessed, the outputs are postprocessed — and the body itself is
    /// skipped, which is the whole point. A miss raises <see cref="CacheMissError"/> out of the
    /// handler call, before anything is produced, so the caller can run the binding for real without
    /// having to undo anything.
    /// </para>
    /// <para>
    /// The session is opened first, so that a cache that keeps sessions apart looks in the right
    /// one; a binding that is not cached answers <see langword="null"/> immediately, which is every
    /// binding of an application that never asked for caching.
    /// </para>
    /// </remarks>
    private static async Task<List<RunOutput>?> ProbeAsync(
        QueueEvent queued,
        ICachedHandler? cached,
        IReadOnlyList<JsonArray> inputs,
        bool batch,
        Progress progress)
    {
        if (cached is null)
        {
            return null;
        }

        using var probe = cached.Cache.Probe();

        var chunks = new List<RunOutput>();

        try
        {
            // A cached stream is replayed from the chunks that were kept, one call per chunk: the
            // run is the same object, so the second chunk is described as the edit that turns the
            // first into it — which is what the client applies.
            var response = await RunAsync(queued, inputs, batch, progress).ConfigureAwait(false);
            chunks.Add(response);

            while (response.IsGenerating)
            {
                response = await RunAsync(queued, inputs, batch, progress).ConfigureAwait(false);
                chunks.Add(response);
            }

            return chunks;
        }
        catch (CacheMissError)
        {
            return null;
        }
    }

    /// <summary>The message a client is told a run produced, for one event of the run.</summary>
    /// <param name="output">What the handler produced.</param>
    /// <param name="sample">Which sample of a batch the event is, or any negative number when the run is not a batch.</param>
    /// <param name="batch">Whether the run was a batch.</param>
    /// <param name="dependency">The binding that ran.</param>
    /// <param name="usedCache">
    /// Whether the answer came from the cache, as the payload spells it, or <see langword="null"/>
    /// for a run that computed it.
    /// </param>
    /// <returns>The payload.</returns>
    /// <remarks>
    /// A batched run produces one payload per sample, and each event of the batch is told the
    /// sample that was its own input; a single run's payload is the same for every event, which is
    /// why it is built here rather than in the runner. The sample is the event's position in the
    /// batch rather than a count of calls, because it is the position that says which input it
    /// brought.
    /// </remarks>
    private JsonObject Payload(RunOutput output, int sample, bool batch, Dependency dependency, string? usedCache = null)
    {
        var data = output.Data;

        if (batch)
        {
            data = sample >= 0 && sample < output.Data.Count && output.Data[sample] is JsonArray slice
                ? slice
                : [];
        }

        return new JsonObject
        {
            ["data"] = data.DeepClone(),
            ["is_generating"] = output.IsGenerating,
            ["duration"] = output.Duration,
            ["average_duration"] = AverageRunTimeOf(dependency),
            ["render_config"] = null,
            ["changed_state_ids"] = ChangedStateIdsOf(output),
            ["used_cache"] = usedCache,
        };
    }

    /// <summary>The state components a run changed, as the payload spells them.</summary>
    /// <param name="output">What the run produced.</param>
    /// <returns>The ids.</returns>
    /// <remarks>
    /// The frontend dispatches a state component's <c>change</c> event from this list, because the
    /// component itself has no frontend to raise one.
    /// </remarks>
    private static JsonArray ChangedStateIdsOf(RunOutput output)
    {
        var ids = new JsonArray();

        foreach (var id in output.ChangedStateIds)
        {
            ids.Add(id);
        }

        return ids;
    }

    /// <summary>Records how long a handler call took, for the average a client is shown.</summary>
    /// <param name="dependency">The binding that ran.</param>
    /// <param name="output">What the call produced, and how long it took.</param>
    /// <remarks>
    /// Every call is measured, a streamed run's chunks included, and the measurement is taken
    /// before the message describing the call is built: the average the Python version reports
    /// includes the call being reported, so a first run reports its own duration rather than
    /// nothing.
    /// </remarks>
    internal void Measure(Dependency dependency, RunOutput output)
    {
        lock (_gate)
        {
            ProcessTimeOf(_runTime, dependency).Add(output.Duration);
        }
    }

    /// <summary>The message that says a run is over, with what the payload carries about it.</summary>
    /// <param name="queued">The event the message is for.</param>
    /// <param name="payload">What the run produced, or the description of why it failed.</param>
    /// <param name="success">Whether the run succeeded.</param>
    /// <param name="title">The title an error carries, or <see langword="null"/> when it succeeded.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// Three fields of the message are read back out of the payload rather than computed here,
    /// which is the Python version's rule and looks odd until the two are told apart: the payload
    /// describes the call, and the message describes the run. A run that used no cache still
    /// reports how long its call took, under the field named for a cached one.
    /// </remarks>
    private static JsonObject Completed(
        QueueEvent queued,
        JsonObject payload,
        bool success,
        string? title = null) =>
        QueueMessages.ProcessCompleted(
            queued.Id,
            payload,
            success,
            title,
            usedCache: UsedCacheOf(payload),
            cacheDuration: NumberOf(payload, "duration"),
            averageDuration: NumberOf(payload, "average_duration"));

    /// <summary>Whether a payload says a cache was used, which only two values can say.</summary>
    private static string? UsedCacheOf(JsonObject payload)
    {
        var used = JsonValues.Text(payload["used_cache"]);

        return used is "full" or "partial" ? used : null;
    }

    /// <summary>One of a payload's numbers, or <see langword="null"/> when it has none there.</summary>
    private static double? NumberOf(JsonObject payload, string name) => JsonValues.Number(payload[name]);

    /// <summary>How long the binding takes on average, or <see langword="null"/> when it never ran.</summary>
    internal double? AverageRunTimeOf(Dependency dependency)
    {
        lock (_gate)
        {
            return _runTime.TryGetValue(dependency, out var time) && time.Count > 0 ? time.Average : null;
        }
    }

    /// <summary>
    /// How long the binding takes to run, on average, for the estimate a waiting client is shown.
    /// </summary>
    /// <remarks>
    /// A different average from <see cref="AverageRunTimeOf"/>: this one measures the whole run as
    /// the queue saw it, and is what the estimate is drawn from, while that one measures the
    /// handler's own call and is what the completion message reports. The Python version keeps
    /// both.
    /// </remarks>
    private double? AverageProcessTimeOf(Dependency dependency)
    {
        lock (_gate)
        {
            return _processTime.TryGetValue(dependency, out var time) && time.Count > 0 ? time.Average : null;
        }
    }

    /// <summary>The accumulator for a binding, created on first use.</summary>
    private static ProcessTime ProcessTimeOf(Dictionary<Dependency, ProcessTime> times, Dependency dependency)
    {
        if (!times.TryGetValue(dependency, out var time))
        {
            time = new ProcessTime();
            times[dependency] = time;
        }

        return time;
    }

    /// <summary>The payload a failed run is described with.</summary>
    /// <param name="error">What the handler threw, or <see langword="null"/> when the run ended for
    /// another reason — a client that left, say.</param>
    /// <returns>The payload.</returns>
    /// <remarks>
    /// Which of the two kinds of failure this was is what decides whether the client reads the
    /// text: a refusal is always shown, a crash only when the application turned error text on
    /// (<see cref="Blocks.ShowError"/>). The Python version's rule, and the reason this takes the
    /// exception rather than only a flag.
    /// </remarks>
    private JsonObject ErrorPayload(Exception? error) => ErrorPayloads.Of(error, _blocks.ShowError);

    /// <summary>Sends a message about an event, unless nobody is left to tell.</summary>
    /// <param name="queued">The event the message is about.</param>
    /// <param name="message">The message.</param>
    /// <remarks>
    /// A cancelled run says nothing more. The Python version's cancellation kills the task that
    /// would have said it, so the completion a client sees for a cancelled run is the one the
    /// cancel request puts on the stream — and a second one, from a handler that is still
    /// unwinding, would be a message the Python version never sends.
    /// </remarks>
    private void Send(QueueEvent queued, JsonObject message)
    {
        if (!queued.Alive || queued.Cancelled)
        {
            return;
        }

        message["event_id"] = queued.Id;
        Enqueue(queued.SessionHash, message);
    }

    /// <summary>Tell every waiting event where it stands, and when it is expected to return.</summary>
    /// <param name="group">The group to describe.</param>
    /// <param name="after">Only describe events at or past this rank, or <see langword="null"/> for all.</param>
    /// <remarks>
    /// The estimate is the handler's average time plus how long the events ahead will take plus how
    /// long until a slot frees — the Python version's formula, kept as it is because the number the
    /// client draws comes from it.
    /// </remarks>
    private void BroadcastEstimations(EventQueue group, int? after = null)
    {
        double waitSoFar = 0;
        double? timeTillAvailableWorker = 0;

        lock (_gate)
        {
            if (group.CurrentConcurrency == group.Limit)
            {
                var expectedEndTimes = new List<double>();

                foreach (var (dependency, starts) in group.StartTimesPerDependency)
                {
                    if (!_processTime.TryGetValue(dependency, out var processTime))
                    {
                        timeTillAvailableWorker = null;

                        break;
                    }

                    var perRun = dependency.Connection == Dependency.StreamConnection
                        ? dependency.TimeLimit ?? 0
                        : processTime.Average;

                    expectedEndTimes.AddRange(starts.Select(start => start + perRun));
                }

                if (timeTillAvailableWorker is not null && expectedEndTimes.Count > 0)
                {
                    timeTillAvailableWorker = Math.Max(expectedEndTimes.Min() - Now(), 0);
                }
            }
        }

        var rank = 0;
        foreach (var queued in group.Waiting.ToArray())
        {
            var processTime = AverageProcessTimeOf(queued.Dependency);

            var rankEta = processTime is not null && timeTillAvailableWorker is not null
                ? processTime + waitSoFar + timeTillAvailableWorker
                : null;

            if (after is null || rank >= after)
            {
                Send(queued, QueueMessages.Estimation(queued.Id, rank, group.Waiting.Count, rankEta));
            }

            if (group.Limit is null)
            {
                waitSoFar = 0;
            }
            else if (processTime is not null)
            {
                waitSoFar += processTime.Value / group.Limit.Value;
            }
            else
            {
                waitSoFar = double.NaN;
            }

            rank++;
        }
    }

    /// <summary>Sends the most recent progress report of every running event.</summary>
    /// <remarks>
    /// The coalescing is the point: a report that arrives between two ticks replaces the one before
    /// it, so what a client sees is the newest state rather than every state.
    /// </remarks>
    private async Task ProgressAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!_stopped)
            {
                List<QueueEvent> running;

                lock (_gate)
                {
                    running = [.. _activeJobs.Where(job => job is not null).SelectMany(job => job!)];
                }

                foreach (var queued in running)
                {
                    if (queued is not { ProgressPending: true, PendingProgress: { } units })
                    {
                        continue;
                    }

                    queued.ProgressPending = false;
                    Send(queued, QueueMessages.Progress(queued.Id, units));
                }

                await Task.Delay(_progressTick, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Stopping.
        }
    }

    /// <summary>Re-describes the queue periodically, for an application that asked for that.</summary>
    private async Task NotificationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!_stopped)
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds), cancellationToken).ConfigureAwait(false);

                if (Count == 0)
                {
                    continue;
                }

                foreach (var group in _groups.Values.ToArray())
                {
                    BroadcastEstimations(group);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Stopping.
        }
    }

    /// <summary>The wall clock, in seconds, as the estimates measure it.</summary>
    private double Now() => _clock.GetUtcNow().ToUnixTimeMilliseconds() / 1000d;
}

using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// Answers the endpoints a client uses to have something run and to watch it happen.
/// </summary>
/// <remarks>
/// <para>
/// Six routes, and they are one capability because they are one conversation: a client joins the
/// queue, reads the messages that describe the run from a stream, can ask what the queue is doing,
/// can cancel what it started, can ask the server to reset, and keeps a second connection open so
/// that the server can tell when it has gone away.
/// </para>
/// <para>
/// The stream's framing is the Python version's to the character — <c>data: {json}\n\n</c> per
/// message, with no event name — because the vendored frontend parses it as it is. That is also why
/// the JSON is written with relaxed escaping: the Python version's serializer leaves non-ASCII text
/// as text, and a client that reads the bytes rather than parsing them would see a difference.
/// </para>
/// </remarks>
internal sealed class GradioQueueEndpoints : IGradioEndpoint
{
    private const string JoinPath = "/gradio_api/queue/join";
    private const string DataPath = "/gradio_api/queue/data";
    private const string StatusPath = "/gradio_api/queue/status";
    private const string CancelPath = "/gradio_api/cancel";
    private const string ResetPath = "/gradio_api/reset";
    private const string HeartbeatPath = "/gradio_api/heartbeat/{session_hash}";

    /// <summary>How long the stream waits for a message before looking at the connection again.</summary>
    private static readonly TimeSpan ReadTimeout = TimeSpan.FromSeconds(10);

    private readonly Blocks _blocks;
    private readonly GradioQueue _queue;
    private readonly GradioAuthentication _authentication;
    private readonly TimeProvider _clock;
    private readonly ILogger<GradioQueueEndpoints> _logger;
    private readonly Action<ILogger, string, int, string, Exception?> _logRefused;
    private readonly TimeSpan _heartbeatRate;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose caches a session's end is reported to.</param>
    /// <param name="queue">The queue runs wait in.</param>
    /// <param name="authentication">Who is asking, which a run carries to its handler.</param>
    /// <param name="clock">The clock the heartbeat waits with.</param>
    /// <param name="heartbeat">How often a stream says it is alive.</param>
    /// <param name="logger">Where a refused request is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioQueueEndpoints(
        Blocks blocks,
        GradioQueue queue,
        GradioAuthentication authentication,
        TimeProvider clock,
        HeartbeatInterval heartbeat,
        ILogger<GradioQueueEndpoints> logger)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(heartbeat);
        ArgumentNullException.ThrowIfNull(logger);

        _blocks = blocks;
        _queue = queue;
        _authentication = authentication;
        _clock = clock;
        _logger = logger;

        _logRefused = LoggerMessage.Define<string, int, string>(
            LogLevel.Information,
            new EventId(1, "GradioRunRefused"),
            "A run was refused ({State}) with {Status}: {Detail}.");

        _heartbeatRate = heartbeat.Interval;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", JoinPath).Key,
        new GradioRoute("GET", DataPath).Key,
        new GradioRoute("GET", StatusPath).Key,
        new GradioRoute("POST", CancelPath).Key,
        new GradioRoute("POST", ResetPath).Key,
        new GradioRoute("GET", HeartbeatPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Request.Path.Value switch
        {
            JoinPath => JoinAsync(context),
            DataPath => DataAsync(context),
            StatusPath => StatusAsync(context),
            CancelPath => CancelAsync(context),
            ResetPath => ResetAsync(context),
            _ => HeartbeatAsync(context),
        };
    }

    /// <summary>Admits a run to the queue, or refuses it and says why.</summary>
    private async Task JoinAsync(HttpContext context)
    {
        var body = PredictBody.Parse(await ReadAsync(context).ConfigureAwait(false));

        if (body.SessionHash is null)
        {
            await ProblemAsync(context, StatusCodes.Status400BadRequest, "Session hash not found.")
                .ConfigureAwait(false);

            return;
        }

        if (_queue.Stopped)
        {
            await ProblemAsync(context, StatusCodes.Status503ServiceUnavailable, "Queue is stopped.")
                .ConfigureAwait(false);

            return;
        }

        var result = await _queue.PushAsync(
            body,
            await _authentication.UserNameOfAsync(context).ConfigureAwait(false)).ConfigureAwait(false);

        if (!result.Success)
        {
            var status = result.State switch
            {
                PushResult.QueueFull => StatusCodes.Status503ServiceUnavailable,
                PushResult.ValidatorError => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest,
            };

            _logRefused(_logger, result.State, status, result.Detail, null);

            await ProblemAsync(context, status, result.Detail).ConfigureAwait(false);

            return;
        }

        await WriteJsonAsync(context, new JsonObject { ["event_id"] = result.Detail }, StatusCodes.Status200OK)
            .ConfigureAwait(false);
    }

    /// <summary>Streams the messages that describe a session's runs.</summary>
    /// <remarks>
    /// <para>
    /// The stream ends on its own when the last run it was waiting for completes, and it ends when
    /// the client goes away — in which case the runs it left behind stop being described and the
    /// ones that had not started are dropped. That is the whole point of reading the connection
    /// state here: nothing else tells the server that nobody is listening.
    /// </para>
    /// <para>
    /// A heartbeat goes out on this stream as well as on its own route, because the two answer
    /// different questions: this one keeps the stream from looking dead to anything between the
    /// client and the server, and the other one is how the server finds out the client is gone.
    /// </para>
    /// </remarks>
    private async Task DataAsync(HttpContext context)
    {
        var sessionHash = context.Request.Query["session_hash"].ToString();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        // The headers go out now rather than with the first message: a stream that has nothing to
        // say yet is still a stream, and a client waiting for the response to begin would
        // otherwise wait for the first heartbeat.
        await context.Response.StartAsync(context.RequestAborted).ConfigureAwait(false);

        // A stream for a session nothing joined has nobody to describe. The Python version answers that with
        // the same unexpected_error a client arriving too late gets, and answering it here rather
        // than opening the session matters: an opened session would never be used, and every such
        // request would push a live one out of the cache.
        if (!_queue.HasSession(sessionHash))
        {
            await WriteEventAsync(context, QueueMessages.UnexpectedError(
                "404: Not Found",
                sessionNotFound: true)).ConfigureAwait(false);

            return;
        }

        using var beating = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
        var reader = _queue.OpenSession(sessionHash);
        var heartbeat = HeartbeatAsync(sessionHash, beating.Token);

        try
        {
            while (true)
            {
                if (!_queue.HasSession(sessionHash))
                {
                    await WriteEventAsync(context, QueueMessages.UnexpectedError(
                        "404: Not Found",
                        sessionNotFound: true)).ConfigureAwait(false);

                    return;
                }

                JsonObject? message = null;
                var drained = false;

                // A fresh source per read: a cancelled one stays cancelled, so reusing it would
                // turn the second read into an immediate cancellation and the loop into a spin.
                using var readTimeout = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
                readTimeout.CancelAfter(ReadTimeout);

                try
                {
                    if (await reader.WaitToReadAsync(readTimeout.Token).ConfigureAwait(false))
                    {
                        reader.TryRead(out message);
                    }
                    else
                    {
                        drained = true;
                    }
                }
                catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
                {
                    // Nothing arrived within the read timeout. The Python version looks at the connection again
                    // and keeps waiting, which is what the loop does next.
                }

                if (context.RequestAborted.IsCancellationRequested)
                {
                    return;
                }

                // The session's channel is closed, which is what the host stopping or the session
                // being cleaned up does. Nothing more will ever arrive on this connection, and
                // waiting for it would be waiting for a message nobody will send.
                if (drained)
                {
                    if (_queue.Stopped)
                    {
                        await WriteEventAsync(
                            context,
                            QueueMessages.UnexpectedError("Server stopped unexpectedly.")).ConfigureAwait(false);
                    }

                    return;
                }

                // A stopped queue describes itself rather than the message it was about to send:
                // the run that message belongs to is not going to finish.
                if (_queue.Stopped)
                {
                    message = QueueMessages.UnexpectedError("Server stopped unexpectedly.");
                }

                if (message is null)
                {
                    continue;
                }

                await WriteEventAsync(context, message).ConfigureAwait(false);

                if (!IsCompletion(message, out var eventId))
                {
                    continue;
                }

                // The event is done. When it was the last one this session was waiting for, there is
                // nothing more to stream, and saying so is what lets the client stop reading rather
                // than wait for a connection that will never produce anything again.
                if (!_queue.CompleteEvent(sessionHash, eventId))
                {
                    continue;
                }

                await WriteEventAsync(context, QueueMessages.CloseStream()).ConfigureAwait(false);

                return;
            }
        }
        catch (OperationCanceledException)
        {
            // The client went away, which is handled below.
        }
        finally
        {
            await beating.CancelAsync().ConfigureAwait(false);
            await heartbeat.ConfigureAwait(false);

            if (context.RequestAborted.IsCancellationRequested)
            {
                await _queue.CloseSessionAsync(sessionHash).ConfigureAwait(false);
            }
        }
    }

    /// <summary>Puts a heartbeat on a session's stream for as long as the stream is open.</summary>
    /// <param name="sessionHash">The session the stream belongs to.</param>
    /// <param name="cancellationToken">Cancels the heartbeat when the stream ends.</param>
    /// <returns>A task that completes when the heartbeat stops.</returns>
    /// <remarks>
    /// The heartbeat waits before it sends, so a stream that opens and closes quickly never
    /// produces one — which is what makes a short run's message sequence the same every time.
    /// </remarks>
    private async Task HeartbeatAsync(string sessionHash, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_heartbeatRate, _clock, cancellationToken).ConfigureAwait(false);

                // The session can be gone by the time the heartbeat wakes up, which is not a
                // failure: there is simply nobody left to keep alive.
                if (!_queue.HasSession(sessionHash))
                {
                    return;
                }

                _queue.Enqueue(sessionHash, QueueMessages.Heartbeat());
            }
        }
        catch (OperationCanceledException)
        {
            // The stream ended, which is what stops the heartbeat.
        }
    }

    /// <summary>Reports how many runs are waiting.</summary>
    private Task StatusAsync(HttpContext context) =>
        WriteJsonAsync(context, _queue.Status(), StatusCodes.Status200OK);

    /// <summary>Stops a run, and tells the client its stream is over.</summary>
    /// <remarks>
    /// Two things happen, and both are needed. The run's own cancellation token is signalled, so
    /// that a handler which observes it stops where it is; and the client is told the run is over,
    /// so that the connection it is holding can be released. The Python version's <c>/cancel</c>
    /// does the second and cancels the asyncio task for the first — a .NET method cannot be stopped
    /// from outside, so the token is the equivalent and the handler has to cooperate with it.
    /// </remarks>
    private async Task CancelAsync(HttpContext context)
    {
        var body = await ReadAsync(context).ConfigureAwait(false);
        var sessionHash = Text(body, "session_hash") ?? string.Empty;
        var eventId = Text(body, "event_id") ?? string.Empty;

        _queue.CancelEvent(eventId);
        await _queue.RemoveFromQueueAsync(eventId).ConfigureAwait(false);

        var running = _queue.SessionHasEvent(sessionHash, eventId);

        if (running)
        {
            _queue.Enqueue(sessionHash, QueueMessages.ProcessCompleted(
                eventId,
                new JsonObject(),
                success: true));
        }

        await WriteJsonAsync(context, new JsonObject { ["success"] = true }, StatusCodes.Status200OK)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Answers the reset request, which does nothing.
    /// </summary>
    /// <param name="context">The request and the response to write.</param>
    /// <returns>A task that completes when the response has been written.</returns>
    /// <remarks>
    /// The Python version's own comment says the same thing: all of the cancelling and resetting is
    /// done by <c>/cancel</c>, and this endpoint exists because the frontend calls it. Answering it
    /// with success rather than with the placeholder is what keeps a client that calls it from
    /// thinking the capability is missing.
    /// </remarks>
    private static Task ResetAsync(HttpContext context) =>
        WriteJsonAsync(context, new JsonObject { ["success"] = true }, StatusCodes.Status200OK);

    /// <summary>
    /// Keeps a session alive for as long as the client is listening, and reclaims it when it stops.
    /// </summary>
    /// <remarks>
    /// The client opens this so that the server can tell the difference between a session that is
    /// idle and one whose browser has been closed. Nothing is read from it — the point is the
    /// disconnect, which is when the session and everything it was holding are dropped.
    /// </remarks>
    private async Task HeartbeatAsync(HttpContext context)
    {
        var sessionHash = context.Request.RouteValues["session_hash"]?.ToString() ?? string.Empty;

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        await context.Response.StartAsync(context.RequestAborted).ConfigureAwait(false);

        try
        {
            while (!context.RequestAborted.IsCancellationRequested)
            {
                await context.Response.WriteAsync("data: ALIVE\n\n", context.RequestAborted).ConfigureAwait(false);
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

                await Task.Delay(_heartbeatRate, _clock, context.RequestAborted).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // The client went away.
        }
        finally
        {
            // The browser is gone, so the caches that were kept for it are forgotten with the rest of
            // its session: the Python version clears them on the same disconnect, and a per-session cache that
            // was never cleared would be a leak with a session-shaped key.
            CacheSessions.Clear(_blocks, sessionHash);

            await _queue.CloseSessionAsync(sessionHash).ConfigureAwait(false);
        }
    }

    /// <summary>Whether a message says a run is over, and which run.</summary>
    private static bool IsCompletion(JsonObject message, out string eventId)
    {
        eventId = JsonValues.Text(message["event_id"]) ?? string.Empty;

        return JsonValues.Text(message["msg"]) == QueueMessageKinds.ProcessCompleted
            && eventId.Length > 0;
    }

    /// <summary>Writes one server-sent event.</summary>
    private static async Task WriteEventAsync(HttpContext context, JsonObject message)
    {
        await context.Response.WriteAsync($"data: {JsonText.Serialize(message)}\n\n", context.RequestAborted)
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>Writes a JSON body.</summary>
    private static async Task WriteJsonAsync(HttpContext context, JsonNode document, int status)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsync(JsonText.Serialize(document), context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Writes a refusal the way the Python framework's own endpoints do.</summary>
    /// <remarks>
    /// The body is <c>{"detail": ...}</c> because that is what the framework the Python version is
    /// built on answers with, and a client written against the Python server reads that field.
    /// </remarks>
    private static Task ProblemAsync(HttpContext context, int status, string detail) =>
        WriteJsonAsync(context, new JsonObject { ["detail"] = detail }, status);

    /// <summary>Reads a request body as JSON.</summary>
    private static async Task<JsonNode?> ReadAsync(HttpContext context)
    {
        try
        {
            return await JsonNode.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>A string field of a parsed body, or <see langword="null"/>.</summary>
    private static string? Text(JsonNode? document, string name) =>
        document is JsonObject body ? JsonValues.Text(body[name]) : null;
}

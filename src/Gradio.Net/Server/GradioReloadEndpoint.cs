using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The stream a page in development mode watches for the application having changed.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's framing is three named events, and the vendored frontend listens for two of
/// them: a <c>reload</c> carries <c>{}</c> and makes the page fetch its config again, an
/// <c>error</c> carries the failure's text as a JSON string and makes the page show it, and a
/// <c>heartbeat</c> carries <c>{}</c> so that neither end mistakes a quiet connection for a dead
/// one.
/// </para>
/// <para>
/// The stream never ends on its own: it ends when the client goes away, which is what
/// <see cref="HttpContext.RequestAborted"/> reports. The Python version polls the connection twenty
/// times a second to notice that; this waits on the change signal and on the heartbeat deadline
/// instead, so a stream with nothing to say costs a timer rather than a loop.
/// </para>
/// <para>
/// One deliberate difference from the Python version, recorded here rather than left to be
/// discovered: the heartbeat interval is the server's (<see cref="HeartbeatInterval"/>), not this
/// route's. The Python version hardcodes fifteen seconds here while reading
/// <c>GRADIO_HEARTBEAT_INTERVAL</c> for its queue streams; an operator who shortened that variable
/// to survive an idle-connection timeout would otherwise have to shorten this route a second way.
/// The default is the same fifteen seconds.
/// </para>
/// </remarks>
internal sealed class GradioReloadEndpoint : IGradioEndpoint
{
    private const string RoutePath = "/gradio_api/dev/reload";

    private readonly GradioReloadSignal _signal;
    private readonly TimeProvider _clock;
    private readonly TimeSpan _heartbeatRate;

    /// <summary>Creates the capability.</summary>
    /// <param name="signal">Where a change to the application is announced.</param>
    /// <param name="clock">The clock the heartbeat is measured with.</param>
    /// <param name="heartbeat">How often the stream says it is alive.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioReloadEndpoint(GradioReloadSignal signal, TimeProvider clock, HeartbeatInterval heartbeat)
    {
        ArgumentNullException.ThrowIfNull(signal);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(heartbeat);

        _signal = signal;
        _clock = clock;
        _heartbeatRate = heartbeat.Interval;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("GET", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        // The headers go out now: a client waiting for the response to begin would otherwise wait
        // for the first change or the first heartbeat, either of which can be fifteen seconds away.
        // The flush is not redundant — starting the response marks it started, and it is the flush
        // that puts the header bytes on the wire; without it a client sees nothing until the first
        // frame, which is exactly the wait this is here to avoid.
        await context.Response.StartAsync(context.RequestAborted).ConfigureAwait(false);
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

        var seen = _signal.ChangeCount;
        var nextHeartbeat = _clock.GetUtcNow() + _heartbeatRate;

        try
        {
            while (!context.RequestAborted.IsCancellationRequested)
            {
                var remaining = nextHeartbeat - _clock.GetUtcNow();

                var change = await _signal
                    .WaitForChangeAsync(seen, remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero, _clock, context.RequestAborted)
                    .ConfigureAwait(false);

                if (change is { } announced)
                {
                    seen = announced.Count;

                    // A change does not restart the heartbeat: the Python version counts from the last
                    // heartbeat, not from the last thing the stream said.
                    var (name, data) = Frame(announced);

                    await WriteAsync(context, name, data).ConfigureAwait(false);

                    continue;
                }

                await WriteAsync(context, "heartbeat", "{}").ConfigureAwait(false);
                nextHeartbeat = _clock.GetUtcNow() + _heartbeatRate;
            }
        }
        catch (OperationCanceledException)
        {
            // The client went away, which is how this stream ends.
        }
    }

    /// <summary>What a change is said as: the event name, and the frame's data.</summary>
    /// <param name="change">The announced change.</param>
    /// <returns>
    /// <c>error</c> with the failure's text as a JSON string, or <c>reload</c> with <c>{}</c>.
    /// </returns>
    /// <remarks>
    /// The name and the payload are decided together because they say the same thing twice: a
    /// client that listened for one name and read the other's shape would be reading a frame
    /// the Python version never sends.
    /// </remarks>
    private static (string Name, string Data) Frame(GradioReloadSignal.Change change) =>
        change.IsError
            ? ("error", JsonText.Serialize(JsonValue.Create(change.Message ?? string.Empty)!))
            : ("reload", "{}");

    /// <summary>Writes one frame, and makes sure it leaves.</summary>
    /// <param name="context">The request and the response to write.</param>
    /// <param name="name">The event name the client listens for.</param>
    /// <param name="data">The frame's data, already JSON.</param>
    private static async Task WriteAsync(HttpContext context, string name, string data)
    {
        await context.Response
            .WriteAsync($"event: {name}\ndata: {data}\n\n", context.RequestAborted)
            .ConfigureAwait(false);

        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }
}

using System.Text.Json.Nodes;
using System.Threading.Tasks;
namespace Gradio.Net;
/// <summary>
/// One admission to the queue: a binding a client asked to run, and everything the queue knows
/// about that run.
/// </summary>
/// <remarks>
/// <para>
/// The event owns its own lifetime rather than borrowing the connection's. A client can go away
/// while its run is queued — that is what the heartbeat and the close-stream message are for — and
/// the run has to be able to carry on being described to nobody, or to be dropped, without the
/// queue holding a reference to a request that no longer exists.
/// </para>
/// <para>
/// The id is drawn per event rather than taken from the client, which is the Python version's rule
/// and what makes it safe to name an event in a cancel request: two clients that both sent the same
/// session hash still have different events.
/// </para>
/// </remarks>
internal sealed class QueueEvent : IDisposable
{
    private TaskCompletionSource _signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly CancellationTokenSource _cancellation = new();
    private readonly object _lifetime = new();
    private bool _disposed;

    /// <summary>Admits a run.</summary>
    /// <param name="sessionHash">The session the client named, or empty when it named none.</param>
    /// <param name="dependency">The binding to run.</param>
    /// <param name="body">What the client sent.</param>
    /// <param name="username">Who is asking, or <see langword="null"/> when the application has no authentication.</param>
    /// <param name="clock">The clock the wait and the run are measured with.</param>
    /// <param name="state">The session's values, or <see langword="null"/> when the run belongs to no session.</param>
    internal QueueEvent(
        string sessionHash,
        Dependency dependency,
        PredictBody body,
        string? username,
        TimeProvider clock,
        SessionState? state = null)
    {
        Id = Guid.NewGuid().ToString("N");

        // A caller that named no session gets one of its own: the Python version makes the event id stand in
        // for the missing session, so that the messages describing this run still have somewhere to
        // go. The routes require a session, so this is the direct-call path rather than the served
        // one — and it is the Python version's rule either way.
        SessionHash = string.IsNullOrEmpty(sessionHash) ? Id : sessionHash;
        Dependency = dependency;
        Body = body;
        Username = username;
        EnqueueTimestamp = clock.GetTimestamp();
        Run = new DependencyRun(
            dependency.Shape,
            clock,
            state,
            new Request(username, SessionHash));
    }

    /// <summary>The event's identity, unique within the process.</summary>
    internal string Id { get; }

    /// <summary>The session the run belongs to, which is the event's own id when the client named none.</summary>
    internal string SessionHash { get; }

    /// <summary>The binding being run.</summary>
    internal Dependency Dependency { get; }

    /// <summary>What the client sent, which a batched run rewrites.</summary>
    /// <remarks>
    /// Settable because a streamed event is fed after it was admitted: each chunk the browser sends
    /// arrives as a new body on <c>/stream/{event_id}</c>, and the run that is already going reads
    /// the newest one.
    /// </remarks>
    internal PredictBody Body { get; set; }

    /// <summary>Who is asking, or <see langword="null"/>.</summary>
    internal string? Username { get; }

    /// <summary>The group this run is counted against.</summary>
    internal string ConcurrencyGroup => Dependency.ConcurrencyGroup;

    /// <summary>
    /// Whether the run may still be described to its client.
    /// </summary>
    /// <value>
    /// Set to <see langword="false"/> when the client disconnects or the session is cleaned up:
    /// there is nobody left to tell, and a message sent to a gone session would sit in a channel
    /// nobody drains.
    /// </value>
    internal bool Alive { get; set; } = true;

    /// <summary>Whether the run was closed by the client rather than finishing.</summary>
    /// <remarks>
    /// Read by the Python version's <c>is_finished</c>, which decides whether a streamed run has
    /// been told to stop: the browser closes the stream when the user stops recording, and the run
    /// then ends with what it has rather than waiting for a chunk that will never come.
    /// </remarks>
    internal bool Closed { get; set; }

    /// <summary>Whether this run is fed by the browser rather than started once.</summary>
    /// <remarks>
    /// A binding on a component's <c>stream</c> event, which is the only kind that speaks the
    /// <c>stream</c> protocol. Such a run waits for each chunk before taking its next step, and a
    /// chunk that never comes ends it after the binding's time limit.
    /// </remarks>
    internal bool Streaming => Dependency.Connection == Dependency.StreamConnection;

    /// <summary>The moment the event was admitted, for the queue-wait measurement.</summary>
    internal long EnqueueTimestamp { get; }

    /// <summary>
    /// The run's own progress reporting, and the last report not yet sent.
    /// </summary>
    /// <remarks>
    /// Progress is coalesced rather than forwarded: a handler that reports on every iteration of a
    /// long loop would otherwise push more messages than the connection can usefully carry, so the
    /// queue keeps the most recent report and sends it on its next tick (the Python version's
    /// <c>progress_pending</c>).
    /// </remarks>
    internal JsonArray? PendingProgress { get; set; }

    /// <summary>Whether a report is waiting to be sent.</summary>
    internal bool ProgressPending { get; set; }

    /// <summary>The run's state: the handler's arguments, and the generator it is draining.</summary>
    internal DependencyRun Run { get; }

    /// <summary>Cancels the run, which a <c>/cancel</c> request does.</summary>
    internal CancellationToken CancellationToken => _cancellation.Token;

    /// <summary>Raised when the run should stop.</summary>
    /// <remarks>
    /// A cancellation that arrives after the run was let go is dropped rather than thrown: the two
    /// come from different requests and can race, and a client cancelling something that has already
    /// finished is not an error.
    /// </remarks>
    internal void Cancel()
    {
        lock (_lifetime)
        {
            if (_disposed)
            {
                return;
            }

            _cancellation.Cancel();
        }
    }

    /// <summary>Whether the run has been cancelled.</summary>
    internal bool Cancelled => _cancellation.IsCancellationRequested;

    /// <summary>Wakes a waiting run, which a streamed run does when the client sends more input.</summary>
    internal void Signal()
    {
        lock (_lifetime)
        {
            _signal.TrySetResult();
        }
    }

    /// <summary>Waits until the run is signalled.</summary>
    /// <param name="timeout">How long to wait before giving up, which is the binding's time limit.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>A task that completes when <see cref="Signal"/> is called.</returns>
    /// <remarks>
    /// The signal is taken as well as waited for: a streamed run takes one chunk per signal, so the
    /// next wait has to be for the next one. Two signals that arrive before the run gets to wait are
    /// one signal, which is what a coalescing mailbox does — the browser sends chunks faster than a
    /// handler consumes them, and running the handler twice on the same newest chunk is not what
    /// either of them means.
    /// </remarks>
    internal async Task<bool> WaitForSignalAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Task signal;
        lock (_lifetime)
        {
            signal = _signal.Task;
        }

        try
        {
            await signal.WaitAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            return false;
        }

        lock (_lifetime)
        {
            if (_signal.Task.IsCompleted)
            {
                _signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }

        return true;
    }

    /// <summary>Releases the run's cancellation source.</summary>
    /// <remarks>
    /// Disposed when the queue lets the event go rather than when the run ends: a handler may still
    /// be holding the token it was given, and releasing the source underneath it would turn a
    /// cancellation into an object-disposed failure.
    /// </remarks>
    public void Dispose()
    {
        lock (_lifetime)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _cancellation.Dispose();
        }
    }
}

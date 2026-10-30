namespace Gradio.Net;
/// <summary>
/// The work an application does once, when it starts being served.
/// </summary>
/// <remarks>
/// <para>
/// The Python version runs two things at that moment: it starts its queue, and it awaits the
/// coroutines collected in <c>Blocks.extra_startup_events</c> — a list a component appends to when
/// it has something to prepare before the first run, which is how caching warms itself up. The
/// first of those is the host's business here (the queue is started with the host rather than on
/// the first request, see <c>GradioQueueHostedService</c>), so what this type holds is the second:
/// the list, and the flag that says it has been walked.
/// </para>
/// <para>
/// The Python version keeps the flag on its <c>App</c> rather than on the <c>Blocks</c>. Here the
/// container owns one application and one application owns the flag, because one container hosts
/// one application: the two objects the Python version separates have the same lifetime, and
/// splitting them would let a second host reset the flag of an application it is not serving.
/// </para>
/// <para>
/// Running them twice is not a second run: the flag is set only after every event has completed, so
/// an event that throws leaves the application unstarted and the next caller tries again — which is
/// the Python version's order too, where the flag is set after the await and an exception skips it.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    private readonly List<Func<CancellationToken, Task>> _startupEvents = [];

    // One caller runs the events; the others are told they were not the one. Not disposed with the
    // application: leaving the scope does not end the application, so a semaphore
    // disposed there would be disposed for a type that is still in use.
    private readonly SemaphoreSlim _startupGate = new(1, 1);

    private bool _startupEventsTriggered;

    /// <summary>Whether this application has run its startup events.</summary>
    /// <remarks>
    /// The answer the startup-events route gives on every call after the first, and the reason that
    /// route is not a placeholder: the flag is a statement about work that happened, not about the
    /// route existing.
    /// </remarks>
    public bool StartupEventsTriggered => _startupEventsTriggered;

    /// <summary>Registers work to run when the application starts being served.</summary>
    /// <param name="startupEvent">The work, given the token that cancels the startup.</param>
    /// <returns>This application, so that the call can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="startupEvent"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The application is already being served, so the event would never run: the startup events run
    /// once, when serving begins.
    /// </exception>
    /// <remarks>
    /// Events run in the order they were registered, once. Registering one is a write to the
    /// application, so it is refused once the application is sealed — the alternative is an event
    /// that is accepted and then silently never runs.
    /// </remarks>
    public Blocks AddStartupEvent(Func<CancellationToken, Task> startupEvent)
    {
        ArgumentNullException.ThrowIfNull(startupEvent);

        ThrowIfSealed();

        _startupEvents.Add(startupEvent);

        return this;
    }

    /// <summary>Runs the startup events, if they have not run yet.</summary>
    /// <param name="cancellationToken">Cancels the startup.</param>
    /// <returns>Whether this call was the one that ran them.</returns>
    /// <remarks>
    /// The return value is what makes the startup-events route answer <c>true</c> once and
    /// <c>false</c> afterwards, which is the Python version's answer and the only thing a client
    /// can observe about work that leaves no other trace.
    /// </remarks>
    public async Task<bool> RunStartupEventsAsync(CancellationToken cancellationToken = default)
    {
        await _startupGate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_startupEventsTriggered)
            {
                return false;
            }

            // A copy, because an event is allowed to register another: walking the list itself would
            // then run the new one in the same pass, which is not what "the events that were
            // registered when the application started" means.
            foreach (var startupEvent in _startupEvents.ToArray())
            {
                await startupEvent(cancellationToken).ConfigureAwait(false);
            }

            _startupEventsTriggered = true;

            return true;
        }
        finally
        {
            _startupGate.Release();
        }
    }
}

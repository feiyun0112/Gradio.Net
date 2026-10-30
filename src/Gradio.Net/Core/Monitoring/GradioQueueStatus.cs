namespace Gradio.Net;
/// <summary>
/// How the queue is doing, as the queue itself reports it: whether it is running, how many runs are
/// waiting for a slot, and how many are holding one.
/// </summary>
/// <remarks>
/// <para>
/// One object rather than three ways to ask. The queue writes to it when it starts and stops, and
/// reads its own numbers into it; the metrics instruments sample it, and the health check reports it,
/// so an operator's alert rule and their orchestrator's probe cannot disagree about how deep the
/// queue is.
/// </para>
/// <para>
/// The numbers are read through a delegate rather than copied on every change: a depth that is
/// sampled when somebody asks costs one lock per scrape, and a depth that was pushed would cost a
/// write per admission whether or not anybody is listening.
/// </para>
/// </remarks>
public sealed class GradioQueueStatus
{
    private Func<int>? _waiting;
    private Func<int>? _active;

    /// <summary>Whether the queue's scheduler is running.</summary>
    /// <value>
    /// <see langword="false"/> until the host starts the queue, and again once it stops.
    /// </value>
    /// <remarks>
    /// This is what a readiness probe asks. A host that is listening but whose queue has not started
    /// admits runs to a queue nobody drains, which is exactly the state an orchestrator should not
    /// send traffic into.
    /// </remarks>
    public bool Running { get; internal set; }

    /// <summary>How many runs are waiting for a slot.</summary>
    public int Depth => _waiting?.Invoke() ?? 0;

    /// <summary>How many runs are holding a slot.</summary>
    public int Active => _active?.Invoke() ?? 0;

    /// <summary>Says where the queue's numbers are read from.</summary>
    /// <param name="waiting">How many runs are waiting.</param>
    /// <param name="active">How many runs are running.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal void Attach(Func<int> waiting, Func<int> active)
    {
        ArgumentNullException.ThrowIfNull(waiting);
        ArgumentNullException.ThrowIfNull(active);

        _waiting = waiting;
        _active = active;
    }
}

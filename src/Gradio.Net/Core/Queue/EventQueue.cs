namespace Gradio.Net;
/// <summary>
/// One concurrency group: the events waiting for a slot, and how many slots are taken.
/// </summary>
/// <remarks>
/// <para>
/// A group is what a concurrency limit applies to. Bindings that name the same
/// <c>concurrency_id</c> share one, and so do bindings that run the same handler — the Python
/// version groups by the identity of the function, so binding one function twice limits the two
/// together, which is the behaviour a caller relying on a shared resource expects.
/// </para>
/// <para>
/// The start times are kept per binding rather than per group because the estimate for a waiting
/// event depends on which handler it is waiting for: a group whose slots are held by two different
/// handlers frees its next one when the earliest of them finishes.
/// </para>
/// </remarks>
internal sealed class EventQueue
{
    /// <summary>Creates a group.</summary>
    /// <param name="concurrencyId">The name the group is known by.</param>
    /// <param name="limit">How many runs may be in flight at once, or <see langword="null"/> for no limit.</param>
    internal EventQueue(string concurrencyId, int? limit)
    {
        ConcurrencyId = concurrencyId;
        Limit = limit;
    }

    /// <summary>The group's name.</summary>
    internal string ConcurrencyId { get; }

    /// <summary>
    /// How many runs of this group may be in flight at once.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for no limit. Lowered when a binding that belongs to the group asks
    /// for less than the group currently allows — never raised, because the group already has runs
    /// admitted under the lower figure.
    /// </value>
    internal int? Limit { get; set; }

    /// <summary>The events waiting for a slot, oldest first.</summary>
    internal List<QueueEvent> Waiting { get; } = [];

    /// <summary>How many runs of this group are in flight.</summary>
    internal int CurrentConcurrency { get; set; }

    /// <summary>When the in-flight runs of each binding started.</summary>
    internal Dictionary<Dependency, List<double>> StartTimesPerDependency { get; } = [];
}

/// <summary>
/// A running average of how long a binding takes.
/// </summary>
/// <remarks>
/// The Python version keeps two of these per binding, and so does the queue: one is the estimate a
/// waiting client is shown, the other is the average the completion message reports. They are fed
/// from different measurements — the handler call, and the run as a whole — so they are not the
/// same number even though the arithmetic is.
/// </remarks>
internal sealed class ProcessTime
{
    private double _total;
    private int _count;

    /// <summary>How many runs have been measured.</summary>
    internal int Count => _count;

    /// <summary>How long a run takes on average, in seconds, or zero when nothing was measured.</summary>
    internal double Average => _count == 0 ? 0 : _total / _count;

    /// <summary>Records one run.</summary>
    /// <param name="duration">How long it took, in seconds.</param>
    internal void Add(double duration)
    {
        _total += duration;
        _count++;
    }
}

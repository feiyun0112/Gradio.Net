using System.Globalization;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What the application remembers about the runs it has admitted, and the summary of them the
/// monitoring panel draws (the Python version's <c>Queue.event_analytics</c> and
/// <c>compute_analytics_summary</c>).
/// </summary>
/// <remarks>
/// <para>
/// Two shapes live here because the Python version keeps two. The records are per event and say
/// what happened to that request; the summary is per binding and says how it is doing — a success
/// rate, the percentiles of its processing time, and how many requests it has ever been given. The
/// panel reads the summary, and the records are what the summary is computed from.
/// </para>
/// <para>
/// The record set is bounded: a long-lived application would otherwise grow a dictionary for every
/// request it ever served. The oldest record is dropped when the bound is reached, which is the
/// Python version's rule, and the per-binding request count is kept separately so that dropping a
/// record does not rewrite history — <c>total_requests</c> counts what was admitted, not what is
/// still remembered.
/// </para>
/// <para>
/// The summary is cached and recomputed at most once every <c>n</c> new records, because
/// recomputing it costs a sort of every record and the panel polls it. The Python version's
/// frequency knob is the same one, read from the same variable.
/// </para>
/// </remarks>
internal sealed class EventAnalytics
{
    /// <summary>How many records are kept before the oldest is dropped.</summary>
    private const int DefaultMaxEvents = 10_000;

    /// <summary>How many new records may arrive between two recomputations of the summary.</summary>
    private const int DefaultCacheFrequency = 1;

    private readonly object _gate = new();
    private readonly LinkedList<AnalyticsRecord> _order = new();
    private readonly Dictionary<string, LinkedListNode<AnalyticsRecord>> _records = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> _recordedPerFunction = new(StringComparer.Ordinal);
    private readonly int _maxEvents;
    private readonly int _cacheFrequency;

    private int _recorded;
    private int _countAtLastCache;
    private JsonObject _cached = EmptySummary();

    /// <summary>Creates the record set the environment configures.</summary>
    /// <remarks>
    /// Public so that the container can build it, as <see cref="GradioQueue"/> is: the built-in
    /// container only considers public constructors, and the type stays internal to the assembly.
    /// </remarks>
    public EventAnalytics()
        : this(MaxEventsFromEnvironment(), CacheFrequencyFromEnvironment())
    {
    }

    /// <summary>Creates a record set with an explicit bound and cache frequency.</summary>
    /// <param name="maxEvents">How many records to keep; at least one.</param>
    /// <param name="cacheFrequency">How many new records may arrive between recomputations; at least one.</param>
    internal EventAnalytics(int maxEvents, int cacheFrequency)
    {
        _maxEvents = Math.Max(1, maxEvents);
        _cacheFrequency = Math.Max(1, cacheFrequency);
    }

    /// <summary>How many records are being kept.</summary>
    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _records.Count;
            }
        }
    }

    /// <summary>Records that a run was admitted.</summary>
    /// <param name="eventId">The run's id.</param>
    /// <param name="function">The binding's API name.</param>
    /// <param name="sessionHash">The session that asked, or <see langword="null"/>.</param>
    /// <param name="time">When it was admitted, as a wall-clock instant in seconds.</param>
    internal void Enqueued(string eventId, string function, string? sessionHash, double time)
    {
        lock (_gate)
        {
            if (_records.TryGetValue(eventId, out var existing))
            {
                _order.Remove(existing);
                _records.Remove(eventId);
            }

            var record = new AnalyticsRecord(eventId, function, sessionHash, time);
            _records[eventId] = _order.AddLast(record);
            _recorded++;
            _recordedPerFunction[function] = _recordedPerFunction.GetValueOrDefault(function) + 1;

            while (_order.Count > _maxEvents)
            {
                var oldest = _order.First;
                if (oldest is null)
                {
                    break;
                }

                _order.RemoveFirst();
                _records.Remove(oldest.Value.Id);
            }
        }
    }

    /// <summary>Records that a run has claimed a slot and is being processed.</summary>
    /// <param name="eventId">The run's id.</param>
    internal void Started(string eventId)
    {
        lock (_gate)
        {
            if (TryGet(eventId) is { } record)
            {
                record.Status = AnalyticsStatus.Processing;
            }
        }
    }

    /// <summary>Records how a run ended.</summary>
    /// <param name="eventId">The run's id.</param>
    /// <param name="status">One of <see cref="AnalyticsStatus"/>'s end states.</param>
    /// <param name="processTime">How long the run took, in seconds, or <see langword="null"/> when it did not run.</param>
    /// <remarks>
    /// This is where the summary is recomputed, which is the Python version's arrangement: its
    /// queue computes it when a run ends and its summary route answers with whatever was last
    /// computed. The difference matters for a panel that refreshes itself — a run that was polled
    /// while it was still going would otherwise spend the recomputation it is owed on a reading
    /// that says "still running", and the panel would keep saying so after the run was over.
    /// </remarks>
    internal void Finished(string eventId, string status, double? processTime)
    {
        lock (_gate)
        {
            if (TryGet(eventId) is { } record)
            {
                record.Status = status;
                record.ProcessTime = processTime;
            }

            if (_recorded - _countAtLastCache >= _cacheFrequency)
            {
                _cached = Compute();
                _countAtLastCache = _recorded;
            }
        }
    }

    /// <summary>The summary the panel reads, as the last run's end computed it.</summary>
    /// <returns>The document, shaped as <c>{"functions": {...}}</c>.</returns>
    /// <remarks>
    /// Reading does not recompute. The document costs a sort of every record, and a panel polls it
    /// every few seconds; the Python version's route answers from its cache for the same reason.
    /// What the reader sees is therefore "the application as of the last run that ended", which is
    /// the answer a panel can act on — a run that has not ended has no outcome to report.
    /// </remarks>
    internal JsonObject Summary()
    {
        lock (_gate)
        {
            return (JsonObject)_cached.DeepClone();
        }
    }

    /// <summary>The newest records, for a panel that lists them.</summary>
    /// <param name="count">How many to return at most.</param>
    /// <returns>The records, oldest first.</returns>
    internal IReadOnlyList<JsonObject> Recent(int count)
    {
        lock (_gate)
        {
            var records = new List<JsonObject>();

            foreach (var record in _order)
            {
                records.Add(record.ToJson());
            }

            return count >= records.Count ? records : records.GetRange(records.Count - count, count);
        }
    }

    /// <summary>The record of one run, if it is still kept.</summary>
    /// <param name="eventId">The run's id.</param>
    /// <returns>The record, or <see langword="null"/>.</returns>
    private AnalyticsRecord? TryGet(string eventId) =>
        _records.TryGetValue(eventId, out var node) ? node.Value : null;

    /// <summary>Groups the records by binding and summarises each group.</summary>
    /// <returns>The summary document.</returns>
    private JsonObject Compute()
    {
        var byFunction = new Dictionary<string, List<AnalyticsRecord>>(StringComparer.Ordinal);

        foreach (var record in _order)
        {
            if (!byFunction.TryGetValue(record.Function, out var group))
            {
                group = [];
                byFunction[record.Function] = group;
            }

            group.Add(record);
        }

        var functions = new JsonObject();

        foreach (var (name, group) in byFunction)
        {
            var success = group.Count(record => record.Status == AnalyticsStatus.Success);
            var failed = group.Count(record => record.Status == AnalyticsStatus.Failed);
            var decided = success + failed;

            functions[name] = new JsonObject
            {
                ["success_rate"] = decided == 0 ? null : (double)success / decided,
                ["process_time_percentiles"] = new JsonObject
                {
                    ["50th"] = Percentile(group, 50),
                    ["90th"] = Percentile(group, 90),
                    ["99th"] = Percentile(group, 99),
                },
                ["total_requests"] = _recordedPerFunction.GetValueOrDefault(name, group.Count),
            };
        }

        return new JsonObject { ["functions"] = functions };
    }

    /// <summary>One percentile of a binding's processing times.</summary>
    /// <param name="group">The binding's records.</param>
    /// <param name="percent">Which percentile, 0 to 100.</param>
    /// <returns>The percentile, or <see langword="null"/> when it is not a number.</returns>
    /// <remarks>
    /// The interpolation is the one the Python version gets from numpy's default: rank the values,
    /// then read between the two neighbours of the fractional position. A record whose processing
    /// time is not known makes the answer unknown rather than zero — numpy carries a NaN through
    /// the percentile the same way, and the panel is told nothing rather than told a number that is
    /// wrong.
    /// </remarks>
    private static double? Percentile(List<AnalyticsRecord> group, double percent)
    {
        if (group.Count == 0)
        {
            return null;
        }

        var values = new List<double>(group.Count);

        foreach (var record in group)
        {
            if (record.ProcessTime is not { } value)
            {
                return null;
            }

            values.Add(value);
        }

        values.Sort();

        return Percentiles.At(values, percent);
    }

    /// <summary>The summary of an application nothing has run in.</summary>
    /// <returns>The document.</returns>
    private static JsonObject EmptySummary() => new() { ["functions"] = new JsonObject() };

    /// <summary>How many records the environment says to keep.</summary>
    /// <returns>The bound, defaulting when the variable says nothing usable.</returns>
    /// <remarks>
    /// A variable that is not a positive number is ignored rather than fatal: it is read while the
    /// container is being built, and a typo there should not keep an application from starting.
    /// </remarks>
    private static int MaxEventsFromEnvironment() =>
        PositiveFromEnvironment("GRADIO_ANALYTICS_MAX_EVENTS", DefaultMaxEvents);

    /// <summary>How often the environment says to recompute the summary.</summary>
    /// <returns>The frequency, defaulting when the variable says nothing usable.</returns>
    private static int CacheFrequencyFromEnvironment() =>
        PositiveFromEnvironment("GRADIO_ANALYTICS_CACHE_FREQUENCY", DefaultCacheFrequency);

    /// <summary>One positive integer from the environment.</summary>
    /// <param name="name">The variable's name.</param>
    /// <param name="fallback">What to use when it says nothing usable.</param>
    /// <returns>The value.</returns>
    private static int PositiveFromEnvironment(string name, int fallback) =>
        int.TryParse(
            Environment.GetEnvironmentVariable(name),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var value) && value > 0
            ? value
            : fallback;
}

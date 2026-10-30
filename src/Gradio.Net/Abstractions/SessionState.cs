using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The values a session's stateful components hold, each with the lifetime it was given
/// </summary>
/// <remarks>
/// <para>
/// The Python version calls this <c>SessionState</c> and keeps it in a <c>StateHolder</c> keyed by
/// session hash. Here it hangs off <see cref="Session"/>, so that a session and the values it
/// carries cannot come apart: whatever drops the session drops the values with it.
/// </para>
/// <para>
/// A value is a JSON tree rather than the object the application stored, because JSON is the one
/// shape this framework can copy the way the Python version copies a Python object: two sessions
/// reading the same component must not end up sharing one mutable object, and a value written by
/// one run must be the value the next run reads.
/// </para>
/// <para>
/// A value's lifetime is measured from the moment it was <em>set</em> rather than from the last
/// time the session was touched, which is the Python version's rule and the reason it is per entry:
/// a <c>State</c> declares its own <c>time_to_live</c>, and two states in one session can outlive
/// each other. A value that was only ever read is not dated at all and so never ages out, which is
/// the Python version's rule too.
/// </para>
/// <para>
/// Each operation is atomic — the lock is what keeps two requests from corrupting the map — but a
/// run's read-then-write window is not a transaction: two runs of one session that start at the
/// same time both read the value from before either wrote, and the last write wins. The Python
/// version has the same property, and it is what "the session's value" means for a state that two
/// handlers share; a handler that needs to serialise them says so with a concurrency limit.
/// </para>
/// <para>
/// Nothing is deleted here on a timer. <see cref="RemoveExpired"/> is what a sweep calls, and it
/// invokes each entry's callback with the value it drops — that callback is the application's, and
/// it is the only reason this type knows about a component at all.
/// </para>
/// </remarks>
public sealed class SessionState
{
    private readonly object _gate = new();
    private readonly Dictionary<int, Entry> _values = [];
    private readonly Dictionary<int, JsonNode?> _shown = [];

    private bool _closed;

    /// <summary>How long the values of a session whose page has gone are kept.</summary>
    /// <value>
    /// One hour, so that a reload can pick the session up again — the Python version's figure, and
    /// its reason. A test run shortens it to a second, which the Python version does with the same
    /// environment variable so that a test need not wait an hour.
    /// </value>
    /// <remarks>
    /// Read once, when the session is created, rather than at each expiry check: a lifetime that
    /// changed halfway through a session would reclaim a value against a rule it was never given.
    /// </remarks>
    public TimeSpan ClosedTimeToLive { get; } =
        Environment.GetEnvironmentVariable("GRADIO_IS_E2E_TEST") is { Length: > 0 }
            ? TimeSpan.FromSeconds(1)
            : TimeSpan.FromHours(1);

    /// <summary>How many values the session holds, expired ones included.</summary>
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _values.Count;
            }
        }
    }

    /// <summary>Whether the page that owned this session has gone.</summary>
    /// <remarks>
    /// A closed session keeps its values for <see cref="ClosedTimeToLive"/> rather than for each
    /// value's own lifetime: the user may come back, and a reload that found nothing would look
    /// like the application had forgotten them.
    /// </remarks>
    public bool IsClosed
    {
        get
        {
            lock (_gate)
            {
                return _closed;
            }
        }
    }

    /// <summary>Reads a value without creating one.</summary>
    /// <param name="blockId">The id of the component the value belongs to.</param>
    /// <param name="value">The value, when the session holds one.</param>
    /// <returns>Whether the session holds a value for that component.</returns>
    public bool TryGet(int blockId, out JsonNode? value)
    {
        lock (_gate)
        {
            if (_values.TryGetValue(blockId, out var entry))
            {
                value = entry.Value;

                return true;
            }

            value = null;

            return false;
        }
    }

    /// <summary>
    /// Reads a value, creating it from the component's initial value when the session has none.
    /// </summary>
    /// <param name="blockId">The id of the component the value belongs to.</param>
    /// <param name="create">Produces the value to start from; called only when there is none.</param>
    /// <returns>The value the session holds.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="create"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// <paramref name="create"/> is a factory rather than a value because the value it produces is
    /// this session's own: two sessions reading the same component must not end up sharing one
    /// mutable object, which is what the Python version's deep copy of the initial value is for.
    /// </para>
    /// <para>
    /// A value created here never ages out, which is the Python version's rule: it records a
    /// lifetime only when a value is <em>written</em>, so a state that is only ever read is kept
    /// for as long as the session is. That is the documented meaning of the lifetime — "after it is
    /// created or updated" — applied to the moment the framework can date: the write.
    /// </para>
    /// </remarks>
    public JsonNode? GetOrAdd(int blockId, Func<JsonNode?> create)
    {
        ArgumentNullException.ThrowIfNull(create);

        lock (_gate)
        {
            if (_values.TryGetValue(blockId, out var existing))
            {
                return existing.Value;
            }

            var entry = new Entry(create(), timeToLive: null, onDelete: null, setAt: default);
            _values[blockId] = entry;

            return entry.Value;
        }
    }

    /// <summary>Stores a value, starting its lifetime again.</summary>
    /// <param name="blockId">The id of the component the value belongs to.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="timeToLive">How long the value lives from now, or <see langword="null"/> for ever.</param>
    /// <param name="onDelete">Called with the value when it is dropped, or <see langword="null"/> for none.</param>
    /// <param name="now">The moment to measure the value's lifetime from.</param>
    public void Set(
        int blockId,
        JsonNode? value,
        TimeSpan? timeToLive,
        Action<JsonNode?>? onDelete,
        DateTimeOffset now)
    {
        lock (_gate)
        {
            _values[blockId] = new Entry(value, timeToLive, onDelete, now);
        }
    }

    /// <summary>Records the value a component was last seen carrying.</summary>
    /// <param name="blockId">The id of the component.</param>
    /// <param name="value">The value, as the protocol carries it.</param>
    /// <remarks>
    /// <para>
    /// A state's value is the session's, and lives in <see cref="_values"/> with a lifetime and a
    /// callback. Every other component's value belongs to the browser — except that the server
    /// sees it go past whenever a run reads or writes it, and remembering it is what lets a share
    /// link open the application as it was rather than as it started.
    /// </para>
    /// <para>
    /// Nothing ages out here: what is remembered is the last thing the client was shown, and it is
    /// replaced rather than accumulated. The Python version keeps the same thing in the same place
    /// — the session's copy of the config — for the same reason.
    /// </para>
    /// </remarks>
    public void Remember(int blockId, JsonNode? value)
    {
        lock (_gate)
        {
            _shown[blockId] = value?.DeepClone();
        }
    }

    /// <summary>Reads the value a component was last seen carrying.</summary>
    /// <param name="blockId">The id of the component.</param>
    /// <param name="value">The value, when the session remembers one.</param>
    /// <returns>Whether the session remembers a value for that component.</returns>
    public bool TryRecall(int blockId, out JsonNode? value)
    {
        lock (_gate)
        {
            return _shown.TryGetValue(blockId, out value);
        }
    }

    /// <summary>Drops a value, calling its callback.</summary>
    /// <param name="blockId">The id of the component the value belongs to.</param>
    /// <returns>Whether the session held one.</returns>
    public bool Remove(int blockId)
    {
        Entry? dropped;

        lock (_gate)
        {
            if (!_values.Remove(blockId, out dropped))
            {
                return false;
            }
        }

        dropped.Delete();

        return true;
    }

    /// <summary>Drops every value whose lifetime has elapsed, calling their callbacks.</summary>
    /// <param name="now">The moment to measure the lifetimes against.</param>
    /// <returns>How many values were dropped.</returns>
    /// <remarks>
    /// The callbacks run outside the lock, because a callback is the application's code and may
    /// read the session it is being told about.
    /// </remarks>
    public int RemoveExpired(DateTimeOffset now)
    {
        List<Entry> dropped = [];

        lock (_gate)
        {
            foreach (var (blockId, entry) in _values.ToArray())
            {
                if (!entry.HasExpired(now, _closed ? ClosedTimeToLive : entry.TimeToLive))
                {
                    continue;
                }

                _values.Remove(blockId);
                dropped.Add(entry);
            }
        }

        foreach (var entry in dropped)
        {
            entry.Delete();
        }

        return dropped.Count;
    }

    /// <summary>Drops every value, calling their callbacks.</summary>
    /// <returns>How many values were dropped.</returns>
    public int Clear()
    {
        List<Entry> dropped;

        lock (_gate)
        {
            dropped = [.. _values.Values];
            _values.Clear();
        }

        foreach (var entry in dropped)
        {
            entry.Delete();
        }

        return dropped.Count;
    }

    /// <summary>
    /// Marks the session as one whose page has gone, which lengthens every value's remaining
    /// lifetime to <see cref="ClosedTimeToLive"/>, and says whether this call was the one that did
    /// it.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> for the first close of a session, <see langword="false"/> for every
    /// close after it.
    /// </returns>
    /// <remarks>
    /// The answer is what makes "the work an application does when a visitor leaves" happen once:
    /// a page has more than one connection that can end — the stream that carries its runs and the
    /// heartbeat that says it is alive — and each of them reports the end it sees. The first report
    /// is the session ending; the rest are the same end arriving twice.
    /// </remarks>
    public bool TryClose()
    {
        lock (_gate)
        {
            if (_closed)
            {
                return false;
            }

            _closed = true;

            return true;
        }
    }

    /// <summary>One value, and the lifetime it was given.</summary>
    private sealed class Entry(JsonNode? value, TimeSpan? timeToLive, Action<JsonNode?>? onDelete, DateTimeOffset setAt)
    {
        internal JsonNode? Value { get; } = value;

        internal TimeSpan? TimeToLive { get; } = timeToLive;

        /// <summary>When the value was written, or <see cref="DateTimeOffset.MinValue"/> if it never was.</summary>
        private DateTimeOffset SetAt { get; } = setAt;

        /// <summary>Whether the value has a date to measure a lifetime from.</summary>
        /// <remarks>
        /// A value that was only ever read has none, and so never ages out — not even when the
        /// session closes, because the Python version never gave it a lifetime to begin with.
        /// </remarks>
        private bool IsDated => SetAt != default;

        private Action<JsonNode?>? OnDelete { get; } = onDelete;

        internal bool HasExpired(DateTimeOffset now, TimeSpan? timeToLive) =>
            IsDated && timeToLive is { } life && now - SetAt >= life;

        internal void Delete() => OnDelete?.Invoke(Value);
    }
}

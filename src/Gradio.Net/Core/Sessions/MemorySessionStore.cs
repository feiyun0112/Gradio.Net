using System.Diagnostics.CodeAnalysis;
namespace Gradio.Net;
/// <summary>
/// The default <see cref="ISessionStore"/>: an in-memory store that evicts the least recently used
/// session when it is at capacity and reclaims sessions that have been idle for their time to live.
/// </summary>
/// <remarks>
/// <para>
/// The policy mirrors the Python version's <c>StateHolder</c>, which keeps an <c>OrderedDict</c> of
/// sessions and drops the oldest when it exceeds <c>state_session_capacity</c>. The Python version
/// has no session-level time to live, so the TTL here is the framework's own addition; it is
/// configured through <see cref="SessionStoreOptions"/> like the capacity is.
/// </para>
/// <para>
/// Options are read once, at construction, so mutating the options object afterwards has no effect.
/// </para>
/// <para>
/// The two bounds are independent: capacity eviction drops the least recently used session without
/// consulting the time to live, and a session whose time to live has elapsed is reclaimed when it
/// is next accessed or when <see cref="RemoveExpired"/> sweeps it. An expired session can therefore
/// occupy capacity until either happens.
/// </para>
/// <para>
/// A session this store drops has its state dropped with it, which is what runs the application's
/// delete callbacks: the store is what decides a session is over, so it is also what ends the
/// values that session was holding. That happens after the lock has been released, because a
/// callback is application code and may call back into the store.
/// </para>
/// <para>
/// The store is safe for concurrent use: every operation takes one lock, and the
/// <see cref="Evicted"/> event is raised after that lock has been released.
/// </para>
/// </remarks>
public sealed class MemorySessionStore : ISessionStore
{
    private readonly SessionStoreOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly object _gate = new();
    private readonly Dictionary<string, LinkedListNode<Session>> _sessions = new(StringComparer.Ordinal);

    /// <summary>Sessions in recency order: the head is the least recently used.</summary>
    private readonly LinkedList<Session> _recency = new();

    /// <summary>Creates a store.</summary>
    /// <param name="options">The capacity and time to live to enforce.</param>
    /// <param name="timeProvider">The clock the time to live is measured against.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <see cref="SessionStoreOptions.Capacity"/> is less than 1, or
    /// <see cref="SessionStoreOptions.TimeToLive"/> is zero or negative.
    /// </exception>
    public MemorySessionStore(SessionStoreOptions options, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(timeProvider);

        if (options.Capacity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                options.Capacity,
                $"{nameof(SessionStoreOptions)}.{nameof(SessionStoreOptions.Capacity)} must be at least 1.");
        }

        if (options.TimeToLive is { } timeToLive && timeToLive <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                timeToLive,
                $"{nameof(SessionStoreOptions)}.{nameof(SessionStoreOptions.TimeToLive)} must be positive when it is set; " +
                "leave it null for sessions that never expire.");
        }

        _options = options;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public event Action<SessionEviction>? Evicted;

    /// <inheritdoc />
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _sessions.Count;
            }
        }
    }

    /// <inheritdoc />
    public Session GetOrAdd(string sessionHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionHash);

        var now = _timeProvider.GetUtcNow();
        var evictions = new PendingEvictions();

        Session session;
        lock (_gate)
        {
            if (_sessions.TryGetValue(sessionHash, out var existing))
            {
                if (!IsExpired(existing.Value, now))
                {
                    Touch(existing, now);

                    return existing.Value;
                }

                Drop(existing, ref evictions);
                evictions.Add(existing.Value, SessionEvictionReason.Expired);
            }

            session = new Session(sessionHash, now);
            _sessions.Add(sessionHash, _recency.AddLast(session));

            while (_sessions.Count > _options.Capacity)
            {
                var oldest = _recency.First!;
                Drop(oldest, ref evictions);
                evictions.Add(oldest.Value, SessionEvictionReason.Capacity);
            }
        }

        evictions.Release(Evicted);

        return session;
    }

    /// <inheritdoc />
    public bool TryGet(string sessionHash, [NotNullWhen(true)] out Session? session)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionHash);

        var now = _timeProvider.GetUtcNow();
        var evictions = new PendingEvictions();

        lock (_gate)
        {
            session = null;

            if (!_sessions.TryGetValue(sessionHash, out var node))
            {
                return false;
            }

            if (IsExpired(node.Value, now))
            {
                Drop(node, ref evictions);
                evictions.Add(node.Value, SessionEvictionReason.Expired);
            }
            else
            {
                Touch(node, now);
                session = node.Value;
            }
        }

        evictions.Release(Evicted);

        return session is not null;
    }

    /// <inheritdoc />
    public bool Remove(string sessionHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionHash);

        var evictions = new PendingEvictions();

        lock (_gate)
        {
            if (!_sessions.TryGetValue(sessionHash, out var node))
            {
                return false;
            }

            Drop(node, ref evictions);
        }

        evictions.Release(Evicted);

        return true;
    }

    /// <inheritdoc />
    public int RemoveExpired()
    {
        var now = _timeProvider.GetUtcNow();
        var evictions = new PendingEvictions();

        lock (_gate)
        {
            var node = _recency.First;
            while (node is not null)
            {
                var next = node.Next;

                if (IsExpired(node.Value, now))
                {
                    Drop(node, ref evictions);
                    evictions.Add(node.Value, SessionEvictionReason.Expired);
                }

                node = next;
            }
        }

        evictions.Release(Evicted);

        return evictions.Count;
    }

    /// <inheritdoc />
    public void Clear()
    {
        var evictions = new PendingEvictions();

        lock (_gate)
        {
            evictions.Drop([.. _recency]);

            _sessions.Clear();
            _recency.Clear();
        }

        evictions.Release(Evicted);
    }

    /// <inheritdoc />
    public IReadOnlyList<Session> Snapshot()
    {
        lock (_gate)
        {
            return [.. _recency];
        }
    }

    private bool IsExpired(Session session, DateTimeOffset now) =>
        _options.TimeToLive is { } timeToLive && now - session.LastAccessedAt >= timeToLive;

    /// <summary>Marks a session as the most recently used one. Must be called under <c>_gate</c>.</summary>
    private void Touch(LinkedListNode<Session> node, DateTimeOffset now)
    {
        _recency.Remove(node);
        _recency.AddLast(node);
        node.Value.LastAccessedAt = now;
    }

    /// <summary>Removes a session. Must be called under <c>_gate</c>.</summary>
    /// <param name="node">The session's place in the recency list.</param>
    /// <param name="dropped">Where the session is recorded, so that its state can be released.</param>
    private void Drop(LinkedListNode<Session> node, ref PendingEvictions dropped)
    {
        _recency.Remove(node);
        _sessions.Remove(node.Value.Hash);
        dropped.Drop(node.Value);
    }

    /// <summary>
    /// What one operation dropped on its own, so that the state can be released and
    /// <see cref="Evicted"/> raised once the lock has been released — a callback is application
    /// code and is allowed to call back into the store, which it could not do from under the lock.
    /// </summary>
    private struct PendingEvictions
    {
        private List<SessionEviction>? _entries;
        private List<Session>? _dropped;

        internal readonly int Count => _entries?.Count ?? 0;

        /// <summary>Records a session the store dropped on its own, and why.</summary>
        internal void Add(Session session, SessionEvictionReason reason) =>
            (_entries ??= []).Add(new SessionEviction(session, reason));

        /// <summary>Records sessions that were dropped without being an eviction.</summary>
        internal void Drop(IEnumerable<Session> sessions) => (_dropped ??= []).AddRange(sessions);

        /// <summary>Records one session that was dropped without being an eviction.</summary>
        internal void Drop(Session session) => (_dropped ??= []).Add(session);

        /// <summary>Releases what was dropped, then reports the evictions.</summary>
        /// <param name="handler">The <see cref="Evicted"/> subscribers, or <see langword="null"/>.</param>
        /// <remarks>
        /// Releasing a session clears its state, which is what invokes the application's delete
        /// callbacks. It happens before the eviction is reported, so that a handler told a session
        /// is gone finds nothing left behind it.
        /// </remarks>
        internal readonly void Release(Action<SessionEviction>? handler)
        {
            if (_dropped is not null)
            {
                foreach (var session in _dropped)
                {
                    session.State.Clear();
                }
            }

            if (_entries is null || handler is null)
            {
                return;
            }

            foreach (var eviction in _entries)
            {
                handler(eviction);
            }
        }
    }
}

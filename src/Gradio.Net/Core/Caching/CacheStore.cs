using System.Collections;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The entries behind a <see cref="Cache"/>: a bounded store that forgets the least recently used
/// entry first, and can keep a session's entries apart from everyone else's.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>_CacheStore</c>. Two bounds rather than one, because either can be the
/// one that matters: a cache of a thousand small answers is a different problem from a cache of
/// three large ones, and an application that sets both is asking for whichever it reaches first.
/// </para>
/// <para>
/// The session and probe flags are ambient rather than arguments, because the call that needs them
/// is the handler's own: a wrapped function is invoked by the framework with the arguments the
/// binding gave it, and there is nowhere in that call to put "this run belongs to session S and is
/// only asking whether the answer is known". Both are <see cref="AsyncLocal{T}"/> instance fields, so
/// two runs in flight at once cannot see each other's, and two applications in one process cannot
/// see each other's caches.
/// </para>
/// </remarks>
internal sealed class CacheStore
{
    /// <summary>The namespace an entry belongs to when no session is open.</summary>
    /// <remarks>
    /// The Python version's name for it. A run outside a session — a direct call, a value computed
    /// at startup — still has to go somewhere, and putting it here is what keeps it from being
    /// cleared when some unrelated session ends.
    /// </remarks>
    internal const string GlobalNamespace = "_global";

    /// <summary>The name the manual API stores an entry's own key under.</summary>
    /// <remarks>
    /// The Python version's name for it. A wrapped function's entry has no key of its own — it is
    /// filed under the digest of the arguments — so this is what tells the two kinds apart when
    /// they are listed.
    /// </remarks>
    internal const string KeyOfEntry = "_key";

    /// <summary>The name a wrapped function stores its answer under.</summary>
    internal const string ValueOfEntry = "value";

    private readonly CacheOptions _options;
    private readonly object _gate = new();
    private readonly Dictionary<string, Entry> _entries = new(StringComparer.Ordinal);
    private readonly LinkedList<string> _leastRecentFirst = new();
    private readonly AsyncLocal<string?> _session = new();
    private readonly AsyncLocal<bool> _probing = new();
    private long _bytes;

    internal CacheStore(CacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    /// <summary>Whether a miss is raised rather than computed.</summary>
    internal bool Probing => _probing.Value;

    /// <summary>Whether each session gets its own namespace.</summary>
    internal bool PerSession => _options.PerSession;

    /// <summary>How many entries the store holds.</summary>
    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    /// <summary>Runs something with a session open.</summary>
    /// <param name="sessionHash">The session whose namespace the run belongs to, or <see langword="null"/> for none.</param>
    /// <returns>A scope that closes the session again.</returns>
    /// <remarks>
    /// The flag is set on the caller's flow before the work is awaited, which is what makes it
    /// visible to the handler: an <see cref="AsyncLocal{T}"/> value set in one method flows into
    /// everything that method awaits.
    /// </remarks>
    internal IDisposable OpenSession(string? sessionHash) => Scope.To(this, sessionHash, _probing.Value);

    /// <summary>Runs something that may only ask whether the answer is known.</summary>
    /// <returns>A scope that ends probe mode.</returns>
    internal IDisposable Probe() => Scope.To(this, _session.Value, probing: true);

    /// <summary>Looks an entry up, and marks it as the most recently used.</summary>
    /// <param name="keyHash">The digest the entry is filed under.</param>
    /// <returns>The entry, or <see langword="null"/> when the store holds none.</returns>
    internal Dictionary<string, object?>? Get(string keyHash)
    {
        var key = Namespaced(keyHash);

        lock (_gate)
        {
            if (!_entries.TryGetValue(key, out var entry))
            {
                return null;
            }

            Touch(entry);

            return entry.Values;
        }
    }

    /// <summary>Files an entry, replacing one under the same key, and forgets what no longer fits.</summary>
    /// <param name="keyHash">The digest to file it under.</param>
    /// <param name="entry">The entry.</param>
    internal void Put(string keyHash, Dictionary<string, object?> entry)
    {
        var key = Namespaced(keyHash);
        var size = _options.MaxMemory is null ? 0 : Estimate(entry);

        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var existing))
            {
                _bytes -= existing.Size;
                _bytes += size;
                _entries[key] = new Entry(entry, existing.Node, size);
                Touch(_entries[key]);
            }
            else
            {
                _entries[key] = new Entry(entry, _leastRecentFirst.AddLast(key), size);
                _bytes += size;
            }

            // Both branches: replacing an entry with a larger one is how a store that only evicts on
            // insertion ends up over its memory bound without ever having grown.
            Evict();
        }
    }

    /// <summary>Forgets everything.</summary>
    internal void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
            _leastRecentFirst.Clear();
            _bytes = 0;
        }
    }

    /// <summary>Forgets one session's entries.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <remarks>
    /// A store that is not per-session has nothing to forget: its entries belong to no session, and
    /// clearing them when one visitor leaves would throw away everyone else's answers.
    /// </remarks>
    internal void ClearSession(string sessionHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionHash);

        if (!_options.PerSession)
        {
            return;
        }

        var prefix = sessionHash + ":";

        lock (_gate)
        {
            foreach (var key in _entries.Keys.Where(key => key.StartsWith(prefix, StringComparison.Ordinal)).ToList())
            {
                Forget(key);
            }
        }
    }

    /// <summary>The keys of the entries this session can see.</summary>
    /// <returns>One entry per key, in the order they were filed.</returns>
    /// <remarks>
    /// An entry filed by a wrapped function has no key of its own — it is filed under the digest of
    /// the arguments — so it reads back as <see langword="null"/>, which is the Python version's
    /// answer too.
    /// </remarks>
    internal IReadOnlyList<object?> Keys()
    {
        var prefix = _options.PerSession ? SessionPrefix() : null;

        lock (_gate)
        {
            return _entries
                .Where(pair => prefix is null || pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                .Select(pair => pair.Value.Values.TryGetValue(KeyOfEntry, out var key) ? key : null)
                .ToList();
        }
    }

    /// <summary>The key an entry is filed under, with its session's namespace in front.</summary>
    /// <param name="keyHash">The digest.</param>
    /// <returns>The key.</returns>
    private string Namespaced(string keyHash) =>
        _options.PerSession ? SessionPrefix() + keyHash : keyHash;

    private string SessionPrefix() => (_session.Value ?? GlobalNamespace) + ":";

    /// <summary>Forgets the least recently used entries until both bounds hold.</summary>
    /// <remarks>
    /// The last entry is never evicted for memory: a single answer larger than the whole budget is
    /// still the answer, and forgetting it would mean the cache could never hold what the
    /// application asked it to.
    /// </remarks>
    private void Evict()
    {
        while (_leastRecentFirst.First is { } oldest)
        {
            var overCount = _options.MaxSize > 0 && _entries.Count > _options.MaxSize;
            var overMemory = _options.MaxMemory is { } limit && _bytes > limit && _entries.Count > 1;

            if (!overCount && !overMemory)
            {
                return;
            }

            Forget(oldest.Value);
        }
    }

    /// <summary>Marks an entry as the most recently used.</summary>
    /// <param name="entry">The entry.</param>
    private void Touch(Entry entry)
    {
        _leastRecentFirst.Remove(entry.Node);
        _leastRecentFirst.AddLast(entry.Node);
    }

    /// <summary>Forgets one entry, wherever it is being forgotten from.</summary>
    /// <param name="key">The key it is filed under.</param>
    private void Forget(string key)
    {
        if (!_entries.Remove(key, out var entry))
        {
            return;
        }

        _bytes -= entry.Size;
        _leastRecentFirst.Remove(entry.Node);
    }

    /// <summary>An estimate of how much memory an entry holds, in bytes.</summary>
    /// <param name="entry">The entry.</param>
    /// <returns>The estimate.</returns>
    private static long Estimate(Dictionary<string, object?> entry)
    {
        long total = 0;

        foreach (var value in entry.Values)
        {
            total += Estimate(value);
        }

        return total;
    }

    private static long Estimate(object? value) =>
        value switch
        {
            null => 0,
            string text => 24 + (2L * text.Length),
            byte[] bytes => 24 + bytes.Length,
            JsonNode node => 24 + System.Text.Encoding.UTF8.GetByteCount(node.ToJsonString()),
            IReadOnlyDictionary<string, object?> map => map.Values.Sum(Estimate),
            IDictionary dictionary => dictionary.Values.Cast<object?>().Sum(Estimate),
            IEnumerable sequence => sequence.Cast<object?>().Sum(Estimate),
            _ => 64,
        };

    /// <summary>One entry: what it holds, where it sits in the order, and how big it is.</summary>
    /// <param name="Values">What was stored.</param>
    /// <param name="Node">Its place in the least-recently-used order.</param>
    /// <param name="Size">Its estimated size, or zero when the store is not bounded by memory.</param>
    /// <remarks>
    /// One record rather than three dictionaries kept in step: an entry that is in one and missing
    /// from another is a store that has forgotten what it holds.
    /// </remarks>
    private sealed record Entry(Dictionary<string, object?> Values, LinkedListNode<string> Node, long Size);

    /// <summary>Puts the ambient flags back the way they were.</summary>
    private sealed class Scope(CacheStore store, string? session, bool probing) : IDisposable
    {
        private bool _disposed;

        /// <summary>Sets both flags, and remembers what they were.</summary>
        /// <param name="store">The store to open.</param>
        /// <param name="session">The session to open, or <see langword="null"/> for none.</param>
        /// <param name="probing">Whether a miss raises.</param>
        /// <returns>The scope.</returns>
        internal static Scope To(CacheStore store, string? session, bool probing)
        {
            var scope = new Scope(store, store._session.Value, store._probing.Value);

            store._session.Value = session;
            store._probing.Value = probing;

            return scope;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            store._session.Value = session;
            store._probing.Value = probing;
        }
    }
}

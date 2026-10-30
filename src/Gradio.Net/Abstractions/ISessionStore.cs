using System.Diagnostics.CodeAnalysis;

namespace Gradio.Net;
/// <summary>
/// Holds the live sessions of one application: an identity-keyed store that bounds itself by
/// <see cref="SessionStoreOptions.Capacity"/> and <see cref="SessionStoreOptions.TimeToLive"/>.
/// </summary>
/// <remarks>
/// <para>
/// The store is the seam that lets sessions move off the process later; the default implementation
/// keeps them in memory.
/// </para>
/// <para>
/// Capacity eviction and TTL reclamation are part of the contract rather than an implementation
/// detail, because a session that disappears is observable: it is reported through
/// <see cref="Evicted"/>, and a session that was dropped simply misses on <see cref="TryGet"/>.
/// </para>
/// <para>
/// Sessions belong to the application the store was registered in, not to a render root: one store
/// serves one app. Rendering two apps in one process means two containers, and therefore two
/// stores.
/// </para>
/// <para>
/// Dropping a session drops the state it was holding, whichever way it was dropped — eviction, an
/// elapsed time to live, an explicit <see cref="Remove"/>, or <see cref="Clear"/>. That is where
/// the application's delete callbacks run, so an implementation that keeps sessions elsewhere has
/// to clear <see cref="Session.State"/> when it lets one go; otherwise a state that has a callback
/// would be forgotten rather than released.
/// </para>
/// </remarks>
public interface ISessionStore
{
    /// <summary>
    /// The number of sessions currently retained.
    /// </summary>
    /// <value>
    /// Counts what is still held, including a session whose <see cref="SessionStoreOptions.TimeToLive"/>
    /// has elapsed but which has neither been accessed nor swept since. Implementations reclaim
    /// lazily, so this is a bound on memory, not a count of live sessions.
    /// </value>
    int Count { get; }

    /// <summary>
    /// Raised when the store drops a session on its own: <see cref="SessionEvictionReason.Capacity"/>
    /// or <see cref="SessionEvictionReason.Expired"/>.
    /// </summary>
    /// <remarks>
    /// An explicit <see cref="Remove"/> or <see cref="Clear"/> is the caller's own doing and does
    /// not raise this event. Handlers run outside the store's lock, so a handler may call back
    /// into the store.
    /// </remarks>
    event Action<SessionEviction>? Evicted;

    /// <summary>
    /// Returns the session for <paramref name="sessionHash"/>, creating it if the store does not
    /// have one. Reading a session is an access, so it refreshes recency and the time to live.
    /// </summary>
    /// <param name="sessionHash">The session identity handed out by the frontend.</param>
    /// <returns>
    /// The existing session — refreshed and marked most recently used — or a brand new one.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="sessionHash"/> is empty.</exception>
    Session GetOrAdd(string sessionHash);

    /// <summary>
    /// Looks up a session without creating one. Like <see cref="GetOrAdd"/>, a hit counts as an
    /// access; a session whose time to live has elapsed is reclaimed and reported as a miss.
    /// </summary>
    /// <param name="sessionHash">The session identity handed out by the frontend.</param>
    /// <param name="session">The session, when it is present and not expired.</param>
    /// <returns><see langword="true"/> when the session was found and returned.</returns>
    /// <exception cref="ArgumentException"><paramref name="sessionHash"/> is empty.</exception>
    bool TryGet(string sessionHash, [NotNullWhen(true)] out Session? session);

    /// <summary>
    /// Drops a session explicitly.
    /// </summary>
    /// <param name="sessionHash">The session identity handed out by the frontend.</param>
    /// <returns><see langword="true"/> when a session was removed.</returns>
    /// <exception cref="ArgumentException"><paramref name="sessionHash"/> is empty.</exception>
    bool Remove(string sessionHash);

    /// <summary>
    /// Sweeps every session whose <see cref="SessionStoreOptions.TimeToLive"/> has elapsed.
    /// </summary>
    /// <returns>How many sessions were reclaimed.</returns>
    int RemoveExpired();

    /// <summary>
    /// A consistent copy of the sessions currently retained.
    /// </summary>
    /// <returns>
    /// The sessions as they were when the copy was taken. Sessions added or dropped afterwards are
    /// not in it, and a session in it may be dropped while the caller is looking at it.
    /// </returns>
    /// <remarks>
    /// This exists for the sweep that reclaims expired <em>state</em>, which is per value rather
    /// than per session: something has to walk the sessions to find the values inside them, and
    /// that something must not hold the store's lock while it runs application code.
    /// </remarks>
    IReadOnlyList<Session> Snapshot();

    /// <summary>Drops every session.</summary>
    void Clear();
}

namespace Gradio.Net;
/// <summary>
/// Reclaims the values sessions hold once their lifetime has elapsed.
/// </summary>
/// <remarks>
/// <para>
/// The Python version runs <c>delete_all_expired_state</c> on a one-second loop for the lifetime of
/// the application, and it does the same thing this does: walk the sessions, and inside each one
/// drop the values whose own time to live has run out. A value is not reclaimed on the request that
/// reads it, because the request that would notice is the one the user makes an hour later — by
/// then the value has been holding whatever it held in the meantime.
/// </para>
/// <para>
/// The store's own time to live is swept first, so that a session that has been idle long enough
/// goes with the values it carried. A store that drops a session clears its state, which is where
/// the application's delete callbacks run (see <see cref="ISessionStore"/>).
/// </para>
/// <para>
/// One sweep takes the store's snapshot and never its lock: the callbacks it runs are the
/// application's code, and a callback that called back into the store from under the store's lock
/// would deadlock.
/// </para>
/// </remarks>
internal sealed class ExpiredStateSweeper
{
    private readonly ISessionStore _sessions;
    private readonly TimeProvider _clock;

    /// <summary>Creates a sweeper for one application.</summary>
    /// <param name="sessions">The sessions to sweep.</param>
    /// <param name="clock">The clock the lifetimes are measured with.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <remarks>
    /// Public so that the container can build it: the built-in container only considers public
    /// constructors, and the type stays internal to the assembly.
    /// </remarks>
    public ExpiredStateSweeper(ISessionStore sessions, TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(clock);

        _sessions = sessions;
        _clock = clock;
    }

    /// <summary>Drops every expired session, and every expired value inside the ones left.</summary>
    /// <returns>How many sessions and values were dropped.</returns>
    internal int Sweep()
    {
        var now = _clock.GetUtcNow();
        var dropped = _sessions.RemoveExpired();

        foreach (var session in _sessions.Snapshot())
        {
            dropped += session.State.RemoveExpired(now);
        }

        return dropped;
    }
}

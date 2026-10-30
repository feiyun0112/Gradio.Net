namespace Gradio.Net;
/// <summary>
/// One frontend session as held by an <see cref="ISessionStore"/>: an identity plus the two
/// timestamps the store uses to enforce <see cref="SessionStoreOptions.Capacity"/> and
/// <see cref="SessionStoreOptions.TimeToLive"/>.
/// </summary>
/// <remarks>
/// This is the kernel-level session. The per-session state a <c>State</c> component holds hangs
/// off it as <see cref="State"/>; the store itself only owns identity, recency and lifetime.
/// </remarks>
public sealed class Session
{
    /// <summary>Creates a session that has just been created and accessed.</summary>
    /// <param name="hash">The session identity handed out by the frontend.</param>
    /// <param name="createdAt">The moment the session was created.</param>
    /// <exception cref="ArgumentException"><paramref name="hash"/> is empty.</exception>
    public Session(string hash, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrEmpty(hash);

        Hash = hash;
        CreatedAt = createdAt;
        LastAccessedAt = createdAt;
    }

    /// <summary>The session identity handed out by the frontend.</summary>
    public string Hash { get; }

    /// <summary>The moment this session was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The moment this session was last accessed. The store moves it forward on every hit, which
    /// is what makes <see cref="SessionStoreOptions.TimeToLive"/> an idle timeout.
    /// </summary>
    public DateTimeOffset LastAccessedAt { get; internal set; }

    /// <summary>
    /// The values this session's stateful components hold.
    /// </summary>
    /// <remarks>
    /// It belongs to the session rather than to the store because a value outlives neither: the
    /// session is what a request names, and a store that dropped it would leave the values with
    /// nobody to read them.
    /// </remarks>
    public SessionState State { get; } = new();
}

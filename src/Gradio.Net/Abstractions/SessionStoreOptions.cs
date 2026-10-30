namespace Gradio.Net;
/// <summary>
/// Configuration of an <see cref="ISessionStore"/>: how many sessions are retained and how long an
/// idle one survives.
/// </summary>
/// <remarks>
/// <see cref="Capacity"/> mirrors the Python version's <c>state_session_capacity</c>.
/// <see cref="TimeToLive"/> does not: the Python version's <c>time_to_live</c> belongs to a single
/// <c>State</c> component and is measured from when that component was last set, whereas this is a
/// store-wide timeout measured from the last access to the session. The Python version has no
/// session-level time to live, so this bound is the framework's own; the per-component behaviour
/// belongs to the <c>State</c> component.
/// </remarks>
public sealed class SessionStoreOptions
{
    /// <summary>The Python version's default for <c>state_session_capacity</c>.</summary>
    public const int DefaultCapacity = 10_000;

    /// <summary>
    /// The maximum number of sessions kept. When a session is added beyond this, the least
    /// recently used one is dropped first.
    /// </summary>
    /// <value>Defaults to <see cref="DefaultCapacity"/>. Must be at least 1.</value>
    public int Capacity { get; set; } = DefaultCapacity;

    /// <summary>
    /// How long a session survives without being accessed, or <see langword="null"/> for a session
    /// that never expires.
    /// </summary>
    /// <value>Defaults to <see langword="null"/>: sessions do not expire unless asked to.</value>
    public TimeSpan? TimeToLive { get; set; }
}

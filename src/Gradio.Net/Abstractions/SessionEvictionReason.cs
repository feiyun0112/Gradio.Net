namespace Gradio.Net;
/// <summary>
/// Why an <see cref="ISessionStore"/> dropped a session without being asked to.
/// </summary>
public enum SessionEvictionReason
{
    /// <summary>The store was at <see cref="SessionStoreOptions.Capacity"/> and this was the least recently used session.</summary>
    Capacity,

    /// <summary>The session had not been accessed for <see cref="SessionStoreOptions.TimeToLive"/>.</summary>
    Expired,
}

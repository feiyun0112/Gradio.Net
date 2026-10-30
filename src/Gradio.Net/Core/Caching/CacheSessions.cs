namespace Gradio.Net;
/// <summary>
/// Forgetting what a session's caches hold.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>clear_session_caches</c>. A cache that keeps sessions apart has to be
/// told when one ends, or its entries outlive the visitor they were computed for — which is the
/// whole reason a per-session cache makes the page ask for a heartbeat in the first place.
/// </para>
/// <para>
/// The caches are found by walking the application's bindings rather than from a registry of every
/// cache ever made. A registry would be static mutable state (GRADIO0001), and it would not be more
/// correct: a cache no binding uses is a cache nothing will look in again.
/// </para>
/// </remarks>
internal static class CacheSessions
{
    /// <summary>Forgets one session's entries in every cache an application's bindings use.</summary>
    /// <param name="blocks">The application.</param>
    /// <param name="sessionHash">The session that ended.</param>
    /// <remarks>
    /// A binding that is cached twice — two wrappers over one cache — is cleared twice, which costs
    /// nothing: the second pass finds nothing left to forget.
    /// </remarks>
    internal static void Clear(Blocks blocks, string sessionHash)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        if (sessionHash.Length == 0)
        {
            return;
        }

        foreach (var dependency in blocks.Dependencies)
        {
            if (dependency.Handler?.Target is ICachedHandler cached)
            {
                cached.Cache.ClearSession(sessionHash);
            }
        }
    }
}

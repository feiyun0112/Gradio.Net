namespace Gradio.Net;
/// <summary>
/// A session an <see cref="ISessionStore"/> dropped on its own, and why.
/// </summary>
/// <param name="Session">The session that was dropped.</param>
/// <param name="Reason">Whether it went for capacity or for idleness.</param>
public readonly record struct SessionEviction(Session Session, SessionEvictionReason Reason);

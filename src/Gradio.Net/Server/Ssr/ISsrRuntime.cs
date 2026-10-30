namespace Gradio.Net;
/// <summary>
/// The renderer that serves the page in front of the application.
/// </summary>
/// <remarks>
/// <para>
/// Server-side rendering is not a call the application makes: it is a second HTTP server that takes
/// the address a browser opens, renders the document itself, and forwards every other request to
/// the application behind it. That arrangement is the Python version's, and the renderer itself is
/// a Node runtime this package does not ship.
/// </para>
/// <para>
/// So the renderer is an interface rather than a process this server starts. An application that
/// renders its page some other way — a different runtime, a container sidecar, a test — registers
/// its own implementation and the server asks it, exactly as the Vibe editor asks
/// <see cref="IVibeLanguageModel"/> rather than a vendor. What the server promises
/// its caller is only this: the address it hands out is the renderer's, and disposing the server
/// disposes the renderer.
/// </para>
/// </remarks>
public interface ISsrRuntime : IAsyncDisposable
{
    /// <summary>The address a browser opens: the renderer's, not the application's.</summary>
    /// <remarks>
    /// The application itself listens on a port of its own while a renderer is in front of it, and
    /// that port is not the address to hand to a client.
    /// </remarks>
    Uri Address { get; }
}

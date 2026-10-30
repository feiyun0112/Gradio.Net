namespace Gradio.Net;
/// <summary>
/// What a renderer is asked to do: listen here, and reach the application there.
/// </summary>
/// <remarks>
/// The four values are the whole contract between the server and a renderer, which is why they
/// travel as one object rather than as four arguments: a renderer that is handed them one at a time
/// invites the caller and the implementation to disagree about which port is which.
/// </remarks>
public sealed class SsrRuntimeRequest
{
    /// <summary>
    /// The runtime the caller named, or <see langword="null"/> to use whatever the renderer finds.
    /// </summary>
    /// <remarks>
    /// Only the built-in Node renderer reads it: it is the Python version's
    /// <c>GRADIO_NODE_PATH</c>, and a renderer that is not a Node process has no use for it.
    /// </remarks>
    public string? NodePath { get; init; }

    /// <summary>The address a browser opens, as the application was configured to listen on.</summary>
    public required string Host { get; init; }

    /// <summary>
    /// The port a browser opens, or 0 to take a free one.
    /// </summary>
    /// <remarks>
    /// This is the address handed to clients, so the renderer is the process that listens on it; the
    /// application moves to a port of its own behind it.
    /// </remarks>
    public required int Port { get; init; }

    /// <summary>The port the application itself is listening on, on the loopback address.</summary>
    public required int BackendPort { get; init; }
}

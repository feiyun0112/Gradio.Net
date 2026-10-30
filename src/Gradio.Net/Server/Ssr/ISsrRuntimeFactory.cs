namespace Gradio.Net;
/// <summary>
/// Starts the renderer an application serves its page from.
/// </summary>
/// <remarks>
/// <para>
/// The one place the built-in Node runtime is chosen. Everything else in this capability talks to
/// <see cref="ISsrRuntime"/>, so replacing the renderer is replacing this implementation rather
/// than reaching into the server: the contract the server relies on is "start with these
/// four values and answer on this address", and that is what a test can hold an implementation to
/// without a runtime installed.
/// </para>
/// <para>
/// A start that fails should throw: the server undoes its own start rather than serving a page
/// nobody asked for. The exception's message is what the caller reads, so it should say what is
/// missing and how to fix it.
/// </para>
/// </remarks>
public interface ISsrRuntimeFactory
{
    /// <summary>Starts a renderer in front of an application that is already listening.</summary>
    /// <param name="request">Where the renderer should listen, and where the application is.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>The running renderer, which the caller disposes.</returns>
    /// <remarks>
    /// The application is listening before this is called: the renderer renders by asking it for
    /// the config, so there is nothing to render until it answers.
    /// </remarks>
    Task<ISsrRuntime> StartAsync(SsrRuntimeRequest request, CancellationToken cancellationToken);
}

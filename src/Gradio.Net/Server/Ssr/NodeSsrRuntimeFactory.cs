namespace Gradio.Net;
/// <summary>
/// The renderer this package starts when the caller does not name one: the Python version's
/// compiled SSR runtime, run by a Node the caller provides.
/// </summary>
/// <remarks>
/// This is the only place that decides to run a Node process, and the only place that needs the
/// runtime to be installed. A caller that renders its page some other way replaces this factory and
/// the package asks for no Node at all.
/// </remarks>
internal sealed class NodeSsrRuntimeFactory : ISsrRuntimeFactory
{
    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    /// No usable Node runtime was found, or the runtime could not be started or did not answer.
    /// </exception>
    public async Task<ISsrRuntime> StartAsync(SsrRuntimeRequest request, CancellationToken cancellationToken)
    {
        return await NodeSsrRuntime.StartAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

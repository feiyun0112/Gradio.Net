using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// Builds a real container with the kernel registered, the way a host would, so that the kernel
/// tests observe it through the same wiring the framework ships.
/// </summary>
internal static class KernelHost
{
    internal static ServiceProvider BuildProvider(Action<GradioCoreOptions>? configure = null) =>
        new ServiceCollection().AddGradioCore(configure).BuildServiceProvider();

    /// <summary>Creates a render root the way a host does: through the container, not around it.</summary>
    internal static Blocks CreateBlocks(this IServiceProvider provider) =>
        new(provider.GetRequiredService<IRenderContextAccessor>());

    /// <summary>
    /// Creates a render root that can group form components, the way a host does.
    /// </summary>
    /// <remarks>
    /// A component written straight into a root is grouped into a synthesised container at the end
    /// of the render, and making that container is the factory's job. A root without one can hold
    /// blocks that expect no container — which is what the kernel tests used before components
    /// arrived — but not components.
    /// </remarks>
    internal static Blocks CreateBlocksWithContainers(this IServiceProvider provider) =>
        new(provider.GetRequiredService<IRenderContextAccessor>(), new BlockContainerFactory());

    internal static IRenderContextAccessor CreateAccessor(this IServiceProvider provider) =>
        provider.GetRequiredService<IRenderContextAccessor>();

    internal static ISessionStore CreateSessionStore(this IServiceProvider provider) =>
        provider.GetRequiredService<ISessionStore>();
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Gradio.Net;
/// <summary>
/// Registers the framework kernel with a container.
/// </summary>
/// <remarks>
/// <para>
/// Everything the kernel needs is resolved from the container, and the kernel keeps no state of
/// its own outside it: there is no static render context to configure and no static session store
/// to reset.
/// </para>
/// <para>
/// This registers the kernel only. The hosting entry points, which also own the route table, are
/// layered on top of it.
/// </para>
/// </remarks>
public static partial class GradioServiceCollectionExtensions
{
    /// <summary>
    /// Adds the kernel services: the render context accessor, the session store, and the options
    /// they are configured through.
    /// </summary>
    /// <param name="services">The container to add to.</param>
    /// <param name="configure">
    /// Optional configuration, applied to the kernel options before anything is resolved from them.
    /// </param>
    /// <returns><paramref name="services"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddGradioCore(
        this IServiceCollection services,
        Action<GradioCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<GradioCoreOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        // A host may already have a clock; when it does not, the system clock is the right default.
        services.TryAddSingleton(TimeProvider.System);

        // The accessor carries the ambient render context in an AsyncLocal, so a single instance
        // serves every scope: a reader in one scope has to find what a writer in another one set.
        services.TryAddSingleton<IRenderContextAccessor, RenderContextAccessor>();

        // Where an application's analytics events go. The default takes them and does nothing, which
        // is what makes "the switch is on" mean "somebody may be listening" rather than "something
        // was sent". A "try add", so an application that registers
        // a sink before this call keeps it.
        services.TryAddSingleton<IAnalyticsSink, NullAnalyticsSink>();

        services.TryAddSingleton<ISessionStore>(static provider => new MemorySessionStore(
            provider.GetRequiredService<IOptions<GradioCoreOptions>>().Value.SessionStore,
            provider.GetRequiredService<TimeProvider>()));

        return services;
    }
}

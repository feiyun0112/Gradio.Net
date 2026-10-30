using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net;
/// <summary>
/// Turns the readiness check into something an orchestrator can probe.
/// </summary>
/// <remarks>
/// <para>
/// Two calls, because a host that has its own health pipeline wants to decide for itself where the
/// endpoint lives: <see cref="AddGradioHealthChecks"/> registers the check with the framework's
/// health service, and <see cref="MapGradioHealth"/> serves it at a path. A host that already maps
/// its own health endpoint calls only the first, and gets this application's readiness reported
/// alongside its other checks.
/// </para>
/// <para>
/// Both are opt-in. A health route is not in the Python version's route table, and the table is the
/// authority for what this framework exposes — an endpoint that appeared by default would be a
/// route the Python version does not have, which the route-set comparison reports as a difference.
/// </para>
/// </remarks>
public static class GradioHealthExtensions
{
    /// <summary>The name this framework's readiness check is registered under.</summary>
    public const string CheckName = "gradio";

    /// <summary>Registers the readiness check with the health service.</summary>
    /// <param name="services">The container to add to.</param>
    /// <returns><paramref name="services"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call it after <c>AddGradio</c>: the check is built from the application and the queue, which
    /// that registers.
    /// </remarks>
    public static IServiceCollection AddGradioHealthChecks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHealthChecks().AddCheck<GradioHealthCheck>(CheckName);

        return services;
    }

    /// <summary>Serves the health checks at a path.</summary>
    /// <param name="endpoints">The endpoint pipeline to add to.</param>
    /// <param name="path">Where to serve them.</param>
    /// <returns><paramref name="endpoints"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="endpoints"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    /// <remarks>
    /// The framework's own response: <c>Healthy</c>, <c>Degraded</c> or <c>Unhealthy</c> as plain
    /// text, with the status code an orchestrator reads. A host that wants a different body maps
    /// <c>MapHealthChecks</c> itself and calls only <see cref="AddGradioHealthChecks"/>.
    /// </remarks>
    public static IEndpointRouteBuilder MapGradioHealth(this IEndpointRouteBuilder endpoints, string path = "/health")
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        endpoints.MapHealthChecks(path);

        return endpoints;
    }
}

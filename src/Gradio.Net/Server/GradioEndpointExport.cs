using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net;
/// <summary>
/// Reads the endpoint set back out of a host that has been built.
/// </summary>
/// <remarks>
/// <para>
/// The set is read from the host's endpoint data source rather than from the route table, so that
/// what a comparison against the Python version sees is what the host actually serves. A table
/// entry that never became an endpoint, an endpoint registered by hand outside the table, and a
/// route registered twice are all visible here, and none of them would be visible by reading the
/// table back to itself.
/// </para>
/// <para>
/// Every endpoint is reported, including ones the table does not know about; an unknown pattern is
/// reported with its own text so that it shows up as a route the Python version does not have
/// rather than being filtered out of the comparison.
/// </para>
/// </remarks>
public static class GradioEndpointExport
{
    /// <summary>Reads every endpoint of a built host.</summary>
    /// <param name="services">The host's container, after the host has been started.</param>
    /// <returns>The endpoints, ordered by path and then method.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The host has no endpoints at all, which means it has not been started: the endpoint data
    /// source is only populated then. Reporting that here keeps a caller from reading an empty set
    /// as "this host registers nothing". The same exception reports a route table that declares two
    /// entries at one registered pattern, which would leave one of them unreachable and the other
    /// reported under the wrong path.
    /// </exception>
    public static IReadOnlyList<GradioEndpoint> From(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var table = services.GetRequiredService<GradioRouteTable>();
        var source = services.GetRequiredService<EndpointDataSource>();

        var endpoints = new List<GradioEndpoint>();
        var known = new Dictionary<(string Method, string Pattern), GradioRoute>();

        foreach (var route in table.Routes)
        {
            if (!known.TryAdd((route.Method, route.RegisteredPattern), route))
            {
                throw new InvalidOperationException(
                    $"The route table declares both '{route.Method} {route.Path}' and "
                    + $"'{known[(route.Method, route.RegisteredPattern)].Path}' at the registered pattern "
                    + $"'{route.RegisteredPattern}', so one of them can never be reached.");
            }
        }

        foreach (var endpoint in source.Endpoints.OfType<RouteEndpoint>())
        {
            var pattern = endpoint.RoutePattern.RawText ?? string.Empty;
            var methods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? [];

            // The guard is read from the mark the mapping loop stamps when it wraps a handler, not
            // from the route it registered: the question the comparison asks is what this host put
            // behind the gate, and a loop that read the table but stopped wrapping would otherwise
            // still look right.
            var requiresLogin = endpoint.Metadata.GetMetadata<GradioLoginRequired>() is not null;

            foreach (var method in methods)
            {
                var path = known.TryGetValue((method, pattern), out var route)
                    ? route.Path
                    : RouteTemplate.Normalize(pattern);

                endpoints.Add(new GradioEndpoint(method, path, pattern, requiresLogin));
            }
        }

        if (endpoints.Count == 0)
        {
            throw new InvalidOperationException(
                "The host has no endpoints to export; it has to be started before its endpoint set can be read.");
        }

        endpoints.Sort(static (left, right) =>
        {
            var byPath = string.CompareOrdinal(left.Path, right.Path);

            return byPath != 0 ? byPath : string.CompareOrdinal(left.Method, right.Method);
        });

        return endpoints;
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net;
/// <summary>
/// The MCP endpoint's routes, added to a host when the application turns out to offer one.
/// </summary>
/// <remarks>
/// <para>
/// The Python version mounts a sub-application at launch, which it can do because it knows by then
/// whether the application asked for one. A host here is assembled before the application is
/// written into it, so the answer is deferred the way the page routes' is: a data source read when
/// the host builds its matcher, which is after the application has been written and before the
/// first request.
/// </para>
/// <para>
/// An application that did not ask for an MCP endpoint adds nothing, which is what keeps a host's
/// endpoint set equal to the route table's when the capability is off — and the route-set comparison
/// records the mount separately for the case where it is on.
/// </para>
/// </remarks>
internal sealed class McpRouteDataSource : EndpointDataSource
{
    private readonly IServiceProvider _services;
    private readonly object _gate = new();
    private IReadOnlyList<Endpoint>? _endpoints;

    /// <summary>Declares the data source.</summary>
    /// <param name="services">The host's container, read for the application when the endpoints are built.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    internal McpRouteDataSource(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
    }

    /// <inheritdoc />
    public override IReadOnlyList<Endpoint> Endpoints
    {
        get
        {
            lock (_gate)
            {
                return _endpoints ??= Build();
            }
        }
    }

    /// <summary>
    /// The endpoints as they were when the matcher was built, which is the only version there is.
    /// </summary>
    /// <returns>A token that never fires.</returns>
    public override IChangeToken GetChangeToken() => new CancellationChangeToken(CancellationToken.None);

    /// <summary>Builds the mount's routes, unless the application offers no MCP endpoint.</summary>
    /// <returns>The routes, or nothing.</returns>
    /// <remarks>
    /// <para>
    /// Every address the mount answers is registered, including the ones that only refuse a method,
    /// so that a client which picks the wrong one is told so rather than told the address does not
    /// exist — which is what the Python version's sub-application does with them.
    /// </para>
    /// <para>
    /// Each of them is behind the login gate. The Python version mounts its MCP sub-application
    /// outside the per-route check, so an application that asks its readers to sign in still offers
    /// its tools to anyone who finds the address; that is a hole rather than a behaviour worth
    /// reproducing, since a tool call runs the application's handlers and the gate costs nothing
    /// when no sign-in is asked for. A client that authenticates through <c>AuthDependency</c>
    /// keeps working: the gate asks the same question of the same request.
    /// </para>
    /// </remarks>
    private List<Endpoint> Build()
    {
        var blocks = _services.GetRequiredService<Blocks>();

        if (!blocks.McpServer)
        {
            return [];
        }

        var endpoint = _services.GetRequiredService<McpEndpoint>();
        var gate = _services.GetRequiredService<GradioLoginGate>();

        (string Method, string Path)[] routes =
        [
            (HttpMethods.Get, McpEndpoint.StreamablePath),
            (HttpMethods.Post, McpEndpoint.StreamablePath),
            (HttpMethods.Get, McpEndpoint.HttpPath),
            (HttpMethods.Post, McpEndpoint.HttpPath),
            (HttpMethods.Get, McpEndpoint.SsePath),
            (HttpMethods.Post, McpEndpoint.SsePath),
            (HttpMethods.Get, McpEndpoint.MessagesPath),
            (HttpMethods.Post, McpEndpoint.MessagesPath),
            (HttpMethods.Get, McpEndpoint.SchemaPath),
        ];

        var built = new List<Endpoint>(routes.Length);

        foreach (var (method, path) in routes)
        {
            var builder = new RouteEndpointBuilder(
                GradioEndpointRouteBuilderExtensions.Guard(endpoint.InvokeAsync, gate),
                RoutePatternFactory.Parse(path),
                order: 0)
            {
                DisplayName = $"{method} {path}",
            };

            builder.Metadata.Add(new HttpMethodMetadata([method]));

            built.Add(builder.Build());
        }

        return built;
    }
}

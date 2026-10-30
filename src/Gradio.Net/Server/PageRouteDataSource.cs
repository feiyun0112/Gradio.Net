using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net;
/// <summary>
/// The route an application's pages other than the home page are served on, added to a host when
/// the application turns out to have one.
/// </summary>
/// <remarks>
/// <para>
/// The Python version registers one literal route per page at launch, which it can do because it
/// knows its pages by then. A host here is assembled before the application is written into it —
/// the container exists, <see cref="GradioEndpointRouteBuilderExtensions.MapGradio"/> runs, and
/// only then does the user's code add blocks and pages — so "does this application have pages?" has
/// no answer at registration time. This is the answer deferred: a data source that is read when the
/// host builds its matcher, which is after the application has been built and before the first
/// request is served.
/// </para>
/// <para>
/// It is one template rather than a route per page, so that the endpoint set does not grow with the
/// number of pages. A path that names no page is refused, which is what the Python version's
/// missing route would have done; a path that names one is served as that page.
/// </para>
/// <para>
/// An application with no pages adds nothing, which is what keeps a single-page host's endpoint set
/// equal to the Python route table's. The endpoints are computed once, because the
/// matcher is built once: an application that adds a page after its host has started is asking for
/// a route the Python version would also have needed to know about at launch.
/// </para>
/// </remarks>
internal sealed class PageRouteDataSource : EndpointDataSource
{
    private readonly IServiceProvider _services;
    private readonly object _gate = new();
    private IReadOnlyList<Endpoint>? _endpoints;

    /// <summary>Declares the data source.</summary>
    /// <param name="services">The host's container, read for the application when the endpoints are built.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    internal PageRouteDataSource(IServiceProvider services)
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

    /// <summary>Builds the page route, unless the application has only its home page.</summary>
    /// <returns>The route, or nothing.</returns>
    private IReadOnlyList<Endpoint> Build()
    {
        var blocks = _services.GetRequiredService<Blocks>();

        if (blocks.Pages.All(page => page.IsHome))
        {
            return [];
        }

        var frontend = _services.GetServices<IGradioEndpoint>()
            .OfType<GradioFrontendEndpoints>()
            .Single();

        var route = new GradioRoute(HttpMethods.Get, GradioFrontendEndpoints.PagesRoute);

        var builder = new RouteEndpointBuilder(
            frontend.InvokeAsync,
            RoutePatternFactory.Parse(route.RegisteredPattern),
            order: 0)
        {
            DisplayName = $"{route.Method} {route.RegisteredPattern}",
        };

        // The Python version registers the page with `@app.get`, and Starlette answers HEAD for every GET
        // route it is given, so HEAD is part of what the Python version serves rather than an extra here.
        builder.Metadata.Add(new HttpMethodMetadata([HttpMethods.Get, HttpMethods.Head]));
        builder.Metadata.Add(new GradioRouteMetadata(route));

        return [builder.Build()];
    }
}

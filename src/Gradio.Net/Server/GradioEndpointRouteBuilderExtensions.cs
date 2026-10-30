using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net;
/// <summary>
/// Maps the route table onto a host's endpoint pipeline.
/// </summary>
/// <remarks>
/// Both hosting entry points come through here, which is what makes "the two entries cannot
/// diverge" a property of the code rather than a promise: there is one registration loop, and it
/// reads one table.
/// </remarks>
public static class GradioEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Registers every endpoint in the route table, each answering with the not-implemented
    /// placeholder until its capability lands.
    /// </summary>
    /// <param name="endpoints">The endpoint pipeline to add to.</param>
    /// <returns><paramref name="endpoints"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="endpoints"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The route table is not registered; call <see cref="GradioServiceCollectionExtensions.AddGradio"/>
    /// first.
    /// </exception>
    /// <remarks>
    /// This maps routes and nothing else: the application is not sealed here, because a host
    /// assembles its pipeline before it writes the UI into the application. Sealing is what
    /// <see cref="UseGradio(WebApplication)"/> adds, and both hosting entry points go through it — a host that maps
    /// the table on its own is served an application that keeps accepting blocks.
    /// </remarks>
    public static IEndpointRouteBuilder MapGradio(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var table = endpoints.ServiceProvider.GetRequiredService<GradioRouteTable>();
        var notImplemented = endpoints.ServiceProvider.GetRequiredService<GradioNotImplementedEndpoint>();
        var gate = endpoints.ServiceProvider.GetRequiredService<GradioLoginGate>();

        var handlers = new Dictionary<string, IGradioEndpoint>(StringComparer.Ordinal);
        var claimed = new HashSet<string>(StringComparer.Ordinal);

        foreach (var endpoint in endpoints.ServiceProvider.GetServices<IGradioEndpoint>())
        {
            foreach (var key in endpoint.Routes)
            {
                handlers[key] = endpoint;
                claimed.Add(key);
            }
        }

        foreach (var route in table.Routes)
        {
            // The table decides what exists; a capability only decides whether the route it was
            // given answers for real yet.
            var handler = handlers.TryGetValue(route.Key, out var endpoint)
                ? (RequestDelegate)endpoint.InvokeAsync
                : notImplemented.Invoke;

            claimed.Remove(route.Key);

            var registration = endpoints
                .MapMethods(route.RegisteredPattern, [route.Method], Guard(route, handler, gate))
                .WithMetadata(new GradioRouteMetadata(route));

            // Stamped where the wrapping happens rather than where the table is read, so that the
            // endpoint set says which routes this host actually put behind the gate. The export
            // compares that, and a loop that stopped wrapping would show up as every guarded route
            // having lost its mark.
            if (route.RequiresLogin)
            {
                registration.WithMetadata(GradioLoginRequired.Instance);
            }
        }

        if (claimed.Count > 0)
        {
            throw new InvalidOperationException(
                $"A capability answers {string.Join(", ", claimed.Order(StringComparer.Ordinal))}, "
                + "which the Python route table does not have. A route this framework adds is "
                + "exactly what the table exists to prevent.");
        }

        MapPageRoutes(endpoints);
        MapMcpRoutes(endpoints);

        return endpoints;
    }

    /// <summary>
    /// Registers the route an application's other pages are served on, when it has one.
    /// </summary>
    /// <param name="endpoints">The endpoint pipeline to add to.</param>
    /// <remarks>
    /// <para>
    /// The Python version registers one literal route per page, because it knows its pages when it
    /// builds the application; this is one template that answers the same URLs and refuses the
    /// rest, which keeps the endpoint set the same whatever the number of pages. It is registered
    /// here rather than claimed by the capability because whether it exists at all is a property of
    /// the application — an application with one page has no such route, which is what keeps a
    /// single-page host's endpoint set equal to the table's.
    /// </para>
    /// <para>
    /// Whether the application has pages is not known yet: a host is assembled before the
    /// application is written into it, so the decision is deferred to the data source the host
    /// reads when it builds its matcher — see <see cref="PageRouteDataSource"/>.
    /// </para>
    /// </remarks>
    private static void MapPageRoutes(IEndpointRouteBuilder endpoints) =>
        endpoints.DataSources.Add(new PageRouteDataSource(endpoints.ServiceProvider));

    /// <summary>
    /// Registers the MCP endpoint's addresses, when the application turns out to offer one.
    /// </summary>
    /// <param name="endpoints">The endpoint pipeline to add to.</param>
    /// <remarks>
    /// The Python version mounts a sub-application at launch, and its routes are therefore a mount
    /// rather than entries in the route table. Whether this application has one is not known yet —
    /// the host is assembled before the application is written into it — so the decision is
    /// deferred to the data source the host reads when it builds its matcher, exactly as the page
    /// routes' is. An application that offers none registers nothing, which keeps a host's endpoint
    /// set equal to the table's.
    /// </remarks>
    private static void MapMcpRoutes(IEndpointRouteBuilder endpoints) =>
        endpoints.DataSources.Add(new McpRouteDataSource(endpoints.ServiceProvider));

    /// <summary>
    /// Puts a route that answers only to someone signed in behind the check that says so.
    /// </summary>
    /// <param name="route">The route being registered.</param>
    /// <param name="handler">What answers it.</param>
    /// <param name="gate">The refusal an unauthenticated caller gets.</param>
    /// <returns>The delegate to register.</returns>
    /// <remarks>
    /// Only the guarded routes are wrapped: an application that asks nobody to sign in keeps the
    /// plain handler, so that adding authentication to the framework costs an application that does
    /// not use it nothing. Which routes those are is <see cref="GradioRoute.RequiresLogin"/>'s
    /// statement, not this loop's — the Python version attaches its check to the same set of
    /// routes, one at a time.
    /// </remarks>
    private static RequestDelegate Guard(GradioRoute route, RequestDelegate handler, GradioLoginGate gate) =>
        route.RequiresLogin ? Guard(handler, gate) : handler;

    /// <summary>
    /// Puts a handler behind the check that says a caller has signed in.
    /// </summary>
    /// <param name="handler">What answers the request.</param>
    /// <param name="gate">The check, and the refusal an unauthenticated caller gets.</param>
    /// <returns>The delegate to register.</returns>
    /// <remarks>
    /// The table's routes ask <see cref="GradioRoute.RequiresLogin"/> first; the addresses that are
    /// not in the table — the MCP mount — come here directly, because they answer a machine and are
    /// the one part of the application that runs its handlers without a page in front of it.
    /// </remarks>
    internal static RequestDelegate Guard(RequestDelegate handler, GradioLoginGate gate) =>
        async context =>
        {
            if (!await gate.AllowsAsync(context).ConfigureAwait(false))
            {
                await gate.RefuseAsync(context).ConfigureAwait(false);

                return;
            }

            await handler(context).ConfigureAwait(false);
        };

    /// <summary>
    /// Serves an application the host built, instead of the one the container would have built.
    /// </summary>
    /// <param name="app">The application to add to.</param>
    /// <param name="application">The page to serve: the blocks the host wrote into.</param>
    /// <returns><paramref name="app"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="app"/> or <paramref name="application"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The container has already resolved an application of its own.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This is the shape an application takes when the page is written before the host exists —
    /// <c>gr.Blocks()</c> in a method that returns it, or a page built while the host is being
    /// configured. The container still hosts exactly one application; this call is what
    /// says which instance that is.
    /// </para>
    /// <para>
    /// Everything else is the parameterless overload: the same routes, the same queue, the same
    /// seal on the first request.
    /// </para>
    /// </remarks>
    public static WebApplication UseGradio(this WebApplication app, Blocks application)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(application);

        app.Services.GetRequiredService<GradioApplicationSlot>().Supply(application);

        return app.UseGradio();
    }

    /// <summary>
    /// Maps the route table onto an application, and stops the application from accepting blocks
    /// once it is being served.
    /// </summary>
    /// <param name="app">The application to add to.</param>
    /// <returns><paramref name="app"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The application this host serves is resolved from the container rather than passed in; see
    /// <see cref="GradioServiceCollectionExtensions.AddGradio"/> for why the container owns it, and
    /// <see cref="UseGradio(WebApplication, Blocks)"/> for the host that wrote its page first.
    /// </para>
    /// <para>
    /// The seal is applied by the first request rather than here, because the documented order is to
    /// map the routes, write the UI into the application, and only then start the host: sealing at
    /// this call would refuse the very blocks the application is made of. The first request is the
    /// moment serving begins, and sealing before it is handled is what makes every request see the
    /// same page.
    /// </para>
    /// </remarks>
    public static WebApplication UseGradio(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var application = app.Services.GetRequiredService<Blocks>();

        // Outermost, so that every response the application produces is a candidate — the page, the
        // stylesheets, the documents, the files — and the decisions it makes are made once, where
        // the pipeline begins. The Python version adds its own in the same place.
        app.UseGradioBrotli();

        app.Use(
            async (context, next) =>
            {
                application.Seal();

                await next(context).ConfigureAwait(false);
            });

        app.MapGradio();

        return app;
    }
}

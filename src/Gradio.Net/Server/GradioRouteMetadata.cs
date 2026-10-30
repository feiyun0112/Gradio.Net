namespace Gradio.Net;
/// <summary>
/// Marks an endpoint as one this framework registered from its route table, and carries the
/// table entry it came from.
/// </summary>
/// <remarks>
/// Registration attaches this so that the endpoint set can be read back out of a running host and
/// compared against the table, rather than against the list the registration loop happened to
/// iterate.
/// </remarks>
public sealed class GradioRouteMetadata
{
    /// <summary>Associates an endpoint with the route it was registered from.</summary>
    /// <param name="route">The table entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="route"/> is <see langword="null"/>.</exception>
    public GradioRouteMetadata(GradioRoute route)
    {
        ArgumentNullException.ThrowIfNull(route);

        Route = route;
    }

    /// <summary>The table entry this endpoint was registered from.</summary>
    public GradioRoute Route { get; }

    /// <summary>The path template of the route a request reached, or <see langword="null"/>.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The table entry's path, e.g. <c>/assets/{path:path}</c>.</returns>
    /// <remarks>
    /// A capability that answers several routes dispatches on the route the request reached rather
    /// than on the request's own path, because three of the table's routes are templates and would
    /// never equal the URL a browser asked for. This is that lookup, so that the walk from the
    /// request to its table entry is written once.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    public static string? PathOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.GetEndpoint()?.Metadata.GetMetadata<GradioRouteMetadata>()?.Route.Path;
    }
}

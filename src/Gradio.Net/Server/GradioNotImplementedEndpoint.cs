using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// Answers requests to routes that are in the table but have no implementation yet.
/// </summary>
/// <remarks>
/// <para>
/// The whole point of registering the route table before the capabilities behind it exist is that
/// "not built yet" has to be distinguishable from "not there at all". A route that were left
/// unregistered would answer 404, which is exactly what a route this framework forgot looks like;
/// this placeholder answers 501 with a body naming the route, and logs the same thing, so the two
/// cases can be told apart by a client and by an operator.
/// </para>
/// <para>
/// The handler is an instance rather than a static method so that its logger and its log delegate
/// are held in instance fields: the package declares no static mutable state at all.
/// </para>
/// </remarks>
internal sealed class GradioNotImplementedEndpoint
{
    /// <summary>The header that marks a response as coming from this placeholder.</summary>
    internal const string StatusHeader = "X-Gradio-Route-Status";

    /// <summary>The value of <see cref="StatusHeader"/>.</summary>
    internal const string StatusValue = "not_implemented";

    private readonly ILogger<GradioNotImplementedEndpoint> _logger;
    private readonly Action<ILogger, string, string, string, int, Exception?> _logNotImplemented;

    /// <summary>Creates the handler.</summary>
    /// <param name="logger">Where the placeholder reports the routes it stands in for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Public so that the container can build it: the built-in container only considers public
    /// constructors, and the type stays internal to the assembly.
    /// </remarks>
    public GradioNotImplementedEndpoint(ILogger<GradioNotImplementedEndpoint> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _logNotImplemented = LoggerMessage.Define<string, string, string, int>(
            LogLevel.Warning,
            new EventId(1, "GradioRouteNotImplemented"),
            "The route {Method} {Path} is in the Python Gradio {UpstreamVersion} route table but is not implemented yet; answering {StatusCode}.");
    }

    internal Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = context.GetEndpoint()?.Metadata.GetMetadata<GradioRouteMetadata>()?.Route;

        // Reaching this handler without a table entry would mean an endpoint was registered by hand
        // rather than from the table; reporting the request instead of failing keeps the placeholder
        // useful while making that visible in the body.
        var method = route?.Method ?? context.Request.Method;
        var path = route?.Path ?? context.Request.Path.Value ?? string.Empty;

        _logNotImplemented(
            _logger,
            method,
            path,
            GradioVersion.Upstream,
            StatusCodes.Status501NotImplemented,
            null);

        context.Response.StatusCode = StatusCodes.Status501NotImplemented;
        context.Response.Headers[StatusHeader] = StatusValue;

        return context.Response.WriteAsJsonAsync(
            new GradioNotImplementedBody(
                StatusValue,
                method,
                path,
                $"{method} {path} is part of the Gradio {GradioVersion.Upstream} route table but is not implemented yet."),
            context.RequestAborted);
    }

    /// <summary>The body a placeholder response carries.</summary>
    private sealed record GradioNotImplementedBody(
        [property: JsonPropertyName("route_status")] string RouteStatus,
        [property: JsonPropertyName("method")] string Method,
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("detail")] string Detail);
}

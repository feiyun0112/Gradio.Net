using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The endpoint a client asks, once, to start the application.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's answer is a boolean: <c>true</c> for the call that ran the application's
/// startup events, <c>false</c> for every call after it. That is the whole of what a client can
/// observe about work which, by the time it answers, has already happened — and it is why this
/// route is not a placeholder: the placeholder says the work is not built, this says whether it has
/// been done.
/// </para>
/// <para>
/// The Python version's <c>launch()</c> calls this route itself while starting, so a browser that
/// arrives afterwards is told <c>false</c>. This framework's host does not call its own routes, so
/// the first caller is the one that gets <c>true</c>. The answer means the same thing on both sides
/// — whether <em>this</em> call ran them — and the events still run exactly once.
/// </para>
/// <para>
/// Not guarded by <see cref="GradioRoute.RequiresLogin"/>: the Python version leaves it open,
/// because it is the request that starts the application and a page cannot ask for a password
/// before it is served.
/// </para>
/// </remarks>
internal sealed class GradioStartupEndpoint : IGradioEndpoint
{
    private const string RoutePath = "/gradio_api/startup-events";

    private readonly Blocks _blocks;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose startup events this runs.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    public GradioStartupEndpoint(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("GET", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var ran = await _blocks.RunStartupEventsAsync(context.RequestAborted).ConfigureAwait(false);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        await context.Response
            .WriteAsync(ran ? "true" : "false", context.RequestAborted)
            .ConfigureAwait(false);
    }
}

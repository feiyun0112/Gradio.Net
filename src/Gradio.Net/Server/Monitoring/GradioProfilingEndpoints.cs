using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The routes a profiled application answers about its runs: the traces themselves, the summary of
/// them, and the request that forgets them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version registers these three only when profiling is on, so an application that is
/// not profiled does not have them at all — a request to one is a request to a path that does not
/// exist. The route table here lists them unconditionally, because a route the Python version
/// sometimes registers is still a route this framework must not be missing; the answer an
/// application that is not profiled gives is therefore <c>404</c>, which is what the Python
/// version's missing route gives.
/// </para>
/// <para>
/// The documents are the Python version's: a list of traces, each with the phases it measured, and
/// a summary with the percentiles of every phase. Both are read by an operator's tooling rather
/// than by the frontend, so the field names are the contract and the zeros are as meaningful as the
/// numbers.
/// </para>
/// </remarks>
internal sealed class GradioProfilingEndpoints : IGradioEndpoint
{
    private const string TracesPath = "/gradio_api/profiling/traces";
    private const string SummaryPath = "/gradio_api/profiling/summary";
    private const string ClearPath = "/gradio_api/profiling/clear";

    private readonly ProfilingOptions _options;
    private readonly TraceCollector? _traces;

    /// <summary>Creates the capability.</summary>
    /// <param name="options">Whether this application is profiled.</param>
    /// <param name="traces">Where the traces are kept, or <see langword="null"/> when nothing collects them.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    public GradioProfilingEndpoints(ProfilingOptions options, TraceCollector? traces = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _traces = traces;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("GET", TracesPath).Key,
        new GradioRoute("GET", SummaryPath).Key,
        new GradioRoute("POST", ClearPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!_options.Enabled || _traces is not { } traces)
        {
            // Not registered in the Python version, so not found here: the placeholder would say "this framework
            // has not built it", which is the wrong answer for a capability that exists and is off.
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            return Task.CompletedTask;
        }

        var route = GradioRouteMetadata.PathOf(context);

        return route switch
        {
            TracesPath => TracesAsync(context, traces),
            SummaryPath => WriteJsonAsync(context, traces.Summary(), StatusCodes.Status200OK),
            ClearPath => ClearAsync(context, traces),
            _ => StatusAsync(context, StatusCodes.Status404NotFound),
        };
    }

    /// <summary>The traces, newest last.</summary>
    /// <param name="context">The request, which may name how many of the newest it wants.</param>
    /// <param name="traces">The collector.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// A <c>last_n</c> that is not a number is refused the way the framework that declared it
    /// refuses one, which is what a client reading the query string expects. A negative one is not
    /// refused: the Python version passes it straight to a slice, where it means "skip that many of
    /// the oldest".
    /// </remarks>
    private static Task TracesAsync(HttpContext context, TraceCollector traces)
    {
        int? lastN = null;

        if (context.Request.Query.TryGetValue("last_n", out var named) && named.Count > 0)
        {
            if (!int.TryParse(named.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return WriteErrorAsync(
                    context,
                    StatusCodes.Status422UnprocessableEntity,
                    "last_n must be an integer.");
            }

            lastN = parsed;
        }

        return WriteJsonAsync(context, traces.All(lastN), StatusCodes.Status200OK);
    }

    /// <summary>Forgets every trace.</summary>
    /// <param name="context">The request.</param>
    /// <param name="traces">The collector.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static Task ClearAsync(HttpContext context, TraceCollector traces)
    {
        traces.Clear();

        return WriteJsonAsync(context, new JsonObject { ["status"] = "cleared" }, StatusCodes.Status200OK);
    }

    /// <summary>Answers with nothing but a status.</summary>
    /// <param name="context">The request.</param>
    /// <param name="status">The status.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static Task StatusAsync(HttpContext context, int status)
    {
        context.Response.StatusCode = status;

        return Task.CompletedTask;
    }

    /// <summary>Answers with a failure a client can read.</summary>
    /// <param name="context">The request.</param>
    /// <param name="status">The status.</param>
    /// <param name="detail">What went wrong.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static Task WriteErrorAsync(HttpContext context, int status, string detail) =>
        WriteJsonAsync(context, new JsonObject { ["detail"] = detail }, status);

    /// <summary>Answers with a JSON document.</summary>
    /// <param name="context">The request and the response to write.</param>
    /// <param name="document">The document.</param>
    /// <param name="status">The status to answer with.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static async Task WriteJsonAsync(HttpContext context, JsonNode document, int status)
    {
        var body = Encoding.UTF8.GetBytes(JsonText.Serialize(document));

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = body.Length;

        await context.Response.Body.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }
}

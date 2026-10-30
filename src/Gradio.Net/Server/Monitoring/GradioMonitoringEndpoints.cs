using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The routes an operator watches their application through: the address that leads to the panel,
/// the summary the panel and the API page read, and the panel itself.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's three monitoring routes, answered from what this application's queue has
/// recorded. What is the Python version's and kept: the paths, the status codes, the refusal
/// bodies, the rule that a disabled application answers <c>403</c> rather than nothing, and the
/// shape of the summary — which is not only the panel's business: the API page in the vendored
/// frontend fetches <c>/monitoring/summary</c> on load and draws a success rate and three
/// percentiles from it.
/// </para>
/// <para>
/// What is deliberately different: the Python version answers the panel address with a redirect to
/// a second Gradio application it mounts beside the first. One container hosts one application
/// here, so the panel is a page rendered from the same records instead of a mounted application.
/// The operator's experience is the one the Python version prints an address for — open it, see the
/// traffic — and the numbers behind it are the same two documents.
/// </para>
/// </remarks>
internal sealed class GradioMonitoringEndpoints : IGradioEndpoint
{
    private const string PanelPath = "/monitoring";
    private const string SummaryPath = "/monitoring/summary";
    private const string KeyPath = "/monitoring/{key}";

    /// <summary>The answer a request gets when the application does not offer monitoring.</summary>
    private const string DisabledDetail = "Monitoring is not enabled.";

    /// <summary>The answer a request gets when the key is not the application's.</summary>
    private const string InvalidKeyDetail = "Invalid key.";

    private readonly Blocks _blocks;
    private readonly EventAnalytics _analytics;
    private readonly ILogger<GradioMonitoringEndpoints> _logger;
    private readonly Action<ILogger, string, Exception?> _logMonitoringUrl;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application being monitored.</param>
    /// <param name="analytics">What the application has recorded about its runs.</param>
    /// <param name="logger">Where the panel's address is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioMonitoringEndpoints(
        Blocks blocks,
        EventAnalytics analytics,
        ILogger<GradioMonitoringEndpoints> logger)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(analytics);
        ArgumentNullException.ThrowIfNull(logger);

        _blocks = blocks;
        _analytics = analytics;
        _logger = logger;

        _logMonitoringUrl = LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, "GradioMonitoringUrl"),
            "Monitoring URL: {Url}");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("GET", PanelPath).Key,
        new GradioRoute("GET", SummaryPath).Key,
        new GradioRoute("GET", KeyPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = GradioRouteMetadata.PathOf(context);

        return route switch
        {
            PanelPath => AddressAsync(context),
            SummaryPath => SummaryAsync(context),
            KeyPath => PanelAsync(context),
            _ => StatusAsync(context, StatusCodes.Status404NotFound),
        };
    }

    /// <summary>
    /// Reports where the panel is, which is what the Python version's own monitoring route does.
    /// </summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The address goes to the log rather than to the browser, exactly as the Python version prints
    /// it to the console: the key is what protects the panel, and a page that carried it would be a
    /// page that handed it to whoever asked for it.
    /// </remarks>
    private async Task AddressAsync(HttpContext context)
    {
        if (!_blocks.EnableMonitoring)
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, DisabledDetail).ConfigureAwait(false);

            return;
        }

        _logMonitoringUrl(_logger, $"{RootUrl.Of(context)}/monitoring/{_blocks.AnalyticsKey}", null);

        await WriteHtmlAsync(context, "See console for monitoring URL.").ConfigureAwait(false);
    }

    /// <summary>The per-binding summary the panel and the API page read.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task SummaryAsync(HttpContext context)
    {
        if (!_blocks.EnableMonitoring)
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, DisabledDetail).ConfigureAwait(false);

            return;
        }

        await WriteJsonAsync(context, _analytics.Summary(), StatusCodes.Status200OK).ConfigureAwait(false);
    }

    /// <summary>The panel, for a request that carries the application's key.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task PanelAsync(HttpContext context)
    {
        if (!_blocks.EnableMonitoring)
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, DisabledDetail).ConfigureAwait(false);

            return;
        }

        var key = context.Request.RouteValues["key"] as string ?? string.Empty;

        if (!Matches(key, _blocks.AnalyticsKey))
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, InvalidKeyDetail).ConfigureAwait(false);

            return;
        }

        var page = MonitoringPage.Render(
            _blocks.Title,
            _analytics.Summary(),
            _analytics.Recent(MonitoringPage.RecentCount));

        await WriteHtmlAsync(context, page).ConfigureAwait(false);
    }

    /// <summary>Whether a key a request carried is the application's.</summary>
    /// <param name="given">What the request carried.</param>
    /// <param name="expected">The application's key.</param>
    /// <returns><see langword="true"/> when they are the same.</returns>
    /// <remarks>
    /// Compared in constant time, because the key is the only thing standing in front of the panel:
    /// the Python version compares it the same way, for the same reason. Two keys of different
    /// lengths cannot be the same key, and saying so costs nothing that a comparison over the
    /// shorter one would not have already told an attacker.
    /// </remarks>
    private static bool Matches(string given, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(given),
            Encoding.UTF8.GetBytes(expected));

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

    /// <summary>Answers with an HTML document.</summary>
    /// <param name="context">The request and the response to write.</param>
    /// <param name="html">The document.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static async Task WriteHtmlAsync(HttpContext context, string html)
    {
        var body = Encoding.UTF8.GetBytes(html);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength = body.Length;

        await context.Response.Body.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }
}

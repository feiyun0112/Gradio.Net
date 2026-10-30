using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The routes a client reads its run history through: connecting a bucket, listing the buckets and
/// the runs, and fetching one file a run carried.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's five run-history routes, answered from the store the application registered
/// rather than from a Hugging Face client. What is the Python version's and kept: the paths, the
/// query parameters, the shape of each answer, and the rule that a record is listed newest first.
/// </para>
/// <para>
/// What is deliberately different: the Python version signs these routes in with the caller's
/// Hugging Face token, because the runs live in their bucket. The store here is local by default
/// and needs no credential, so the routes do not demand one — an application that plugs in a remote
/// store is expected to put its own authorization in front of them, which is also what ticket 29
/// will do for the application as a whole. The difference is written down in the scope ledger
/// rather than left for a reader to notice.
/// </para>
/// </remarks>
internal sealed class GradioRunHistoryEndpoints : IGradioEndpoint
{
    private const string ConnectPath = "/gradio_api/run-history/connect";
    private const string BucketsPath = "/gradio_api/run-history/buckets";
    private const string RecordsPath = "/gradio_api/run-history/records";
    private const string AssetPath = "/gradio_api/run-history/records/{endpoint}/{record_id}/assets/{asset_id}";

    /// <summary>The most records a page may ask for.</summary>
    private const int MaxRecordsPerPage = 200;

    private readonly Blocks _blocks;
    private readonly IRunHistoryStore _store;
    private readonly RunHistoryOptions _options;
    private readonly Action<ILogger, string, Exception?> _logHistoryFailed;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose history is read.</param>
    /// <param name="store">Where the history is kept.</param>
    /// <param name="options">Which bucket a request that names none is about.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioRunHistoryEndpoints(Blocks blocks, IRunHistoryStore store, RunHistoryOptions options)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(options);

        _blocks = blocks;
        _store = store;
        _options = options;

        _logHistoryFailed = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "GradioRunHistoryFailed"),
            "A run history request failed: {Reason}");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", ConnectPath).Key,
        new GradioRoute("GET", BucketsPath).Key,
        new GradioRoute("GET", RecordsPath).Key,
        new GradioRoute("GET", AssetPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!_blocks.RunHistory)
        {
            // An application that keeps no history has no page and no records; the Python version refuses the
            // routes it would have registered rather than answering them with nothing.
            return StatusAsync(context, StatusCodes.Status404NotFound);
        }

        var route = context.GetEndpoint()?.Metadata.GetMetadata<GradioRouteMetadata>()?.Route.Path;

        return route switch
        {
            ConnectPath => ConnectAsync(context),
            BucketsPath => BucketsAsync(context),
            RecordsPath => RecordsAsync(context),
            AssetPath => AssetAsync(context),
            _ => StatusAsync(context, StatusCodes.Status404NotFound),
        };
    }

    /// <summary>Prepares the bucket a client wants its runs in.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task ConnectAsync(HttpContext context)
    {
        var body = await ReadAsync(context).ConfigureAwait(false);
        var bucket = JsonValues.Text(body?["bucket_id"]);

        if (bucket is not { Length: > 0 })
        {
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, "invalid bucket id")
                .ConfigureAwait(false);

            return;
        }

        try
        {
            await _store.ConnectAsync(bucket, context.RequestAborted).ConfigureAwait(false);

            await WriteJsonAsync(context, new JsonObject { ["ok"] = true }, StatusCodes.Status200OK)
                .ConfigureAwait(false);
        }
        catch (RunHistoryException failure)
        {
            await FailAsync(context, failure).ConfigureAwait(false);
        }
    }

    /// <summary>The buckets this application can write to.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task BucketsAsync(HttpContext context)
    {
        try
        {
            var buckets = await _store.BucketsAsync(context.RequestAborted).ConfigureAwait(false);
            var list = new JsonArray();

            foreach (var bucket in buckets)
            {
                list.Add(bucket);
            }

            await WriteJsonAsync(context, new JsonObject { ["buckets"] = list }, StatusCodes.Status200OK)
                .ConfigureAwait(false);
        }
        catch (RunHistoryException failure)
        {
            await FailAsync(context, failure).ConfigureAwait(false);
        }
    }

    /// <summary>The newest runs of this application.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task RecordsAsync(HttpContext context)
    {
        if (BucketOf(context) is not { } bucket || !LimitOf(context, out var limit))
        {
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, "invalid request")
                .ConfigureAwait(false);

            return;
        }

        try
        {
            var records = await _store
                .ListAsync(bucket, AppIdOf(), limit, context.RequestAborted)
                .ConfigureAwait(false);

            var list = new JsonArray();

            foreach (var record in records)
            {
                list.Add(record.ToJson());
            }

            await WriteJsonAsync(context, new JsonObject { ["records"] = list }, StatusCodes.Status200OK)
                .ConfigureAwait(false);
        }
        catch (RunHistoryException failure)
        {
            await FailAsync(context, failure).ConfigureAwait(false);
        }
    }

    /// <summary>One file a run carried, served to the browser that asked for it.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task AssetAsync(HttpContext context)
    {
        if (BucketOf(context) is not { } bucket)
        {
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, "invalid bucket id")
                .ConfigureAwait(false);

            return;
        }

        var endpoint = context.Request.RouteValues["endpoint"] as string ?? string.Empty;
        var recordId = context.Request.RouteValues["record_id"] as string ?? string.Empty;
        var assetId = context.Request.RouteValues["asset_id"] as string ?? string.Empty;

        try
        {
            var asset = await _store
                .ReadAssetAsync(bucket, AppIdOf(), endpoint, recordId, assetId, context.RequestAborted)
                .ConfigureAwait(false);

            if (asset?.Data is not { } data)
            {
                await StatusAsync(context, StatusCodes.Status404NotFound).ConfigureAwait(false);

                return;
            }

            var contentType = asset.ContentType ?? "application/octet-stream";

            // A type a browser would render is served inline; anything else is a download, because a
            // file that could execute must not run on this origin.
            var renders = contentType.StartsWith("image/", StringComparison.Ordinal)
                || contentType.StartsWith("audio/", StringComparison.Ordinal)
                || contentType.StartsWith("video/", StringComparison.Ordinal)
                || contentType.StartsWith("text/", StringComparison.Ordinal)
                || contentType == "application/json";

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = renders ? contentType : "application/octet-stream";
            context.Response.Headers.ContentDisposition = renders ? "inline" : "attachment";
            context.Response.Headers.CacheControl = "private, max-age=31536000, immutable";
            context.Response.ContentLength = data.Length;

            await context.Response.Body.WriteAsync(data, context.RequestAborted).ConfigureAwait(false);
        }
        catch (RunHistoryException failure)
        {
            await FailAsync(context, failure).ConfigureAwait(false);
        }
    }

    /// <summary>The bucket a request names, or the application's own.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The bucket, or <see langword="null"/> when the one it named is not a name.</returns>
    /// <remarks>
    /// The Python version requires the parameter, because a client there is choosing among its own
    /// buckets. An application with one store has one answer, so a request that names none is
    /// answered from the configured bucket — and a request that names a bucket that is not a name
    /// is refused rather than quietly replaced.
    /// </remarks>
    private string? BucketOf(HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("bucket", out var named) || named.Count == 0)
        {
            return _options.Bucket;
        }

        var bucket = named.ToString();

        // The shape is the store's to judge: it is the thing that knows what a container name can
        // be, and it refuses one it cannot use with the status a client should be told.
        return bucket.Length == 0 ? null : bucket;
    }

    /// <summary>The number of records a request asks for.</summary>
    /// <param name="context">The request.</param>
    /// <param name="limit">The number.</param>
    /// <returns><see langword="false"/> when the request named a number outside the range.</returns>
    private static bool LimitOf(HttpContext context, out int limit)
    {
        limit = 50;

        if (!context.Request.Query.TryGetValue("limit", out var named) || named.Count == 0)
        {
            return true;
        }

        return int.TryParse(named.ToString(), System.Globalization.CultureInfo.InvariantCulture, out limit)
            && limit is >= 1 and <= MaxRecordsPerPage;
    }

    /// <summary>The folder this application's runs are filed under.</summary>
    /// <returns>The id.</returns>
    private string AppIdOf() => RunHistoryKeys.AppIdOf(_blocks);

    /// <summary>Answers with a failure the store reported.</summary>
    /// <param name="context">The request.</param>
    /// <param name="failure">What went wrong.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task FailAsync(HttpContext context, RunHistoryException failure)
    {
        _logHistoryFailed(Logger(context), failure.Message, null);

        await WriteErrorAsync(context, failure.Status, failure.Message).ConfigureAwait(false);
    }

    /// <summary>The logger to report a failure to.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The logger, when the host registered one.</returns>
    private static ILogger Logger(HttpContext context) =>
        context.RequestServices.GetService(typeof(ILogger<GradioRunHistoryEndpoints>)) as ILogger<GradioRunHistoryEndpoints>
        ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<GradioRunHistoryEndpoints>.Instance;

    /// <summary>Reads a request body as JSON.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The document, or <see langword="null"/> when it is not JSON.</returns>
    private static async Task<JsonNode?> ReadAsync(HttpContext context)
    {
        try
        {
            return await JsonNode.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
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
        var body = System.Text.Encoding.UTF8.GetBytes(JsonText.Serialize(document));

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = body.Length;

        await context.Response.Body.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }
}

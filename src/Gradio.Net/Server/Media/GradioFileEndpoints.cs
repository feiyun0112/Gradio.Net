using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net;
/// <summary>
/// Moves files in and out of an application: the upload endpoint, the progress stream that reports
/// one, and the endpoints that serve a file back.
/// </summary>
/// <remarks>
/// <para>
/// The upload endpoint and the file endpoints are one capability because they are one loop: what a
/// browser uploads lands in the application's directory, a handler reads it, and what the handler
/// produces is written back into that directory and served from it. The address a file is served at
/// is the address a component puts in its value, so the two cannot be built apart.
/// </para>
/// <para>
/// The rules are the Python version's, and they are security rules as much as behaviour: a path
/// outside the application's directory is refused rather than served, a path that names a location
/// is refused before it is resolved, a type that a browser would execute is served as a download,
/// and a range request is answered with the bytes it asked for rather than the whole file. The
/// refusals answer with the same statuses and the same <c>detail</c> body the Python version does,
/// because a client written against the Python server reads them.
/// </para>
/// </remarks>
internal sealed partial class GradioFileEndpoints : IGradioEndpoint
{
    private const string UploadPath = "/gradio_api/upload";
    private const string UploadProgressPath = "/gradio_api/upload_progress";
    private const string FilePath = "/gradio_api/file/{path:path}";
    private const string FileParameterPath = "/gradio_api/file={path_or_url:path}";
    private const string ProxyPath = "/gradio_api/proxy={url_path:path}";

    /// <summary>The prefix the parameter form of the file route carries in its value.</summary>
    /// <remarks>
    /// The Python version's template is <c>/file={path_or_url:path}</c>, which ASP.NET Core can
    /// only express as a catch-all with a constraint on its first characters, so the value that
    /// arrives still begins with the literal part of the template.
    /// </remarks>
    private const string FileParameterPrefix = "file=";

    /// <summary>How often a progress stream looks for something new.</summary>
    private static readonly TimeSpan CheckRate = TimeSpan.FromMilliseconds(50);

    /// <summary>How often a progress stream says it is alive while nothing is arriving.</summary>
    private static readonly TimeSpan HeartbeatRate = TimeSpan.FromSeconds(15);

    private readonly Blocks _blocks;
    private readonly UploadProgress _progress;
    private readonly TimeProvider _clock;
    private readonly ProfilingOptions _profiling;
    private readonly TraceCollector _traces;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose files are served.</param>
    /// <param name="progress">The tracker an upload reports to.</param>
    /// <param name="clock">The clock the progress stream's heartbeat is measured with.</param>
    /// <param name="profiling">Whether uploads are traced.</param>
    /// <param name="traces">Where an upload's trace goes.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioFileEndpoints(
        Blocks blocks,
        UploadProgress progress,
        TimeProvider clock,
        ProfilingOptions profiling,
        TraceCollector traces)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(progress);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(profiling);
        ArgumentNullException.ThrowIfNull(traces);

        _blocks = blocks;
        _progress = progress;
        _clock = clock;
        _profiling = profiling;
        _traces = traces;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", UploadPath).Key,
        new GradioRoute("GET", UploadProgressPath).Key,
        new GradioRoute("GET", FilePath).Key,
        new GradioRoute("GET", FileParameterPath).Key,
        new GradioRoute("HEAD", FileParameterPath).Key,
        new GradioRoute("GET", ProxyPath).Key,
        new GradioRoute("HEAD", ProxyPath).Key,
    ];

    /// <inheritdoc />
    /// <remarks>
    /// The dispatch is on the route the request reached rather than on its path, because four of these
    /// routes are templates whose URLs never equal their templates.
    /// </remarks>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = context.GetEndpoint()?.Metadata.GetMetadata<GradioRouteMetadata>()?.Route.Path;

        return route switch
        {
            UploadPath => UploadAsync(context),
            UploadProgressPath => ProgressAsync(context),
            FileParameterPath => FileAsync(context, FileParameterPrefix),
            FilePath => FileAsync(context, prefix: null),

            // The Python version's proxy route fetches a file from an application this one was loaded from.
            // Loading is its own batch (ticket 27), so no application can have a proxy URL yet, and
            // the Python version's answer for one that has none is the refusal this gives.
            ProxyPath => ProblemAsync(context, StatusCodes.Status400BadRequest, "This URL cannot be proxied."),
            _ => NotFoundAsync(context),
        };
    }

    /// <summary>Stores the files a request carries.</summary>
    /// <remarks>
    /// The answer is the list of paths, as a JSON array, which is what the client library reads and
    /// what the frontend turns into file objects. The files are recorded as the application's
    /// temporary files, so that an application that configured a reclamation policy cleans them up
    /// like anything else it wrote.
    /// </remarks>
    private async Task UploadAsync(HttpContext context)
    {
        var started = _profiling.Enabled ? _clock.GetTimestamp() : 0;
        var uploadId = context.Request.Query["upload_id"].ToString();
        uploadId = uploadId.Length == 0 ? null : uploadId;

        if (!FileUpload.IsMultipart(context.Request))
        {
            // The Python version refuses a request of the wrong shape before it reads anything, and answers
            // with a `detail` document rather than with the plain text a parser failure is.
            await ProblemAsync(context, StatusCodes.Status400BadRequest, "Invalid content type.").ConfigureAwait(false);

            return;
        }

        // The server's own ceiling on a request body is 30 MB by default, which would refuse a file
        // the application never limited. The limit is lifted and left to the reader, which enforces
        // the application's own limit per file and can say what it is — and which writes to disk as
        // it reads, so the body is never held whole.
        if (context.Features.Get<IHttpMaxRequestBodySizeFeature>() is { IsReadOnly: false } limit)
        {
            limit.MaxRequestBodySize = null;
        }

        if (uploadId is not null)
        {
            _progress.Track(uploadId);
        }

        var outcome = await FileUpload.ReadAsync(
            context.Request,
            _blocks.UploadFolder,
            _blocks.MaxFileSize,
            _progress,
            uploadId,
            context.RequestAborted).ConfigureAwait(false);

        if (!outcome.Accepted)
        {
            await WriteTextAsync(context, outcome.Refusal!, outcome.Status).ConfigureAwait(false);

            return;
        }

        var paths = new JsonArray();
        foreach (var path in outcome.Paths)
        {
            _blocks.TrackTempFile(path);
            paths.Add(path);
        }

        RecordUploadTrace(context, started);

        await WriteJsonAsync(context, paths, StatusCodes.Status200OK).ConfigureAwait(false);
    }

    /// <summary>Files an upload's trace, when the application is profiled.</summary>
    /// <param name="context">The upload's request, for the session it named.</param>
    /// <param name="started">When the upload began, as a clock reading.</param>
    /// <remarks>
    /// <para>
    /// An upload is traced as itself rather than as a run: it has no binding, no phases and no
    /// chunks, and the Python version files it under <c>gradio_file_upload</c> so that the summary
    /// can report what receiving files costs apart from what handling them costs.
    /// </para>
    /// <para>
    /// The session comes from the header the client sends it in, which is where the Python version
    /// reads it too — and an upload that named no session is filed with an empty one rather than
    /// with nothing, which is the value the Python version's <c>to_dict</c> would carry.
    /// </para>
    /// </remarks>
    private void RecordUploadTrace(HttpContext context, long started)
    {
        if (!_profiling.Enabled)
        {
            return;
        }

        _traces.Add(new RequestTrace(
            Guid.NewGuid().ToString(),
            TraceCollector.UploadFunction,
            context.Request.Headers["session_hash"].ToString(),
            _clock.GetUtcNow().ToUnixTimeMilliseconds() / 1000d)
        {
            UploadMs = _clock.GetElapsedTime(started).TotalMilliseconds,
        });
    }

    /// <summary>Streams how far an upload has got, until it is over.</summary>
    /// <remarks>
    /// <para>
    /// A client opens this before it starts sending, so an id that is not being tracked yet is a
    /// client that arrived first rather than one asking about nothing: the wait is the Python
    /// version's three seconds, and only then is the id refused.
    /// </para>
    /// <para>
    /// The stream ends when the upload does, which is what tells the client to stop drawing progress.
    /// A heartbeat goes out while nothing is arriving so that whatever sits between the client and
    /// the server does not decide the connection is dead.
    /// </para>
    /// </remarks>
    private async Task ProgressAsync(HttpContext context)
    {
        var uploadId = context.Request.Query["upload_id"].ToString();

        if (uploadId.Length == 0)
        {
            await WriteTextAsync(context, "Upload not found", StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        if (!await _progress.WaitTrackedAsync(uploadId, context.RequestAborted).ConfigureAwait(false))
        {
            _progress.StopTracking(uploadId);
            await WriteTextAsync(context, "Upload not found", StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        await context.Response.StartAsync(context.RequestAborted).ConfigureAwait(false);

        var heartbeat = _clock.GetTimestamp();

        try
        {
            while (!context.RequestAborted.IsCancellationRequested)
            {
                if (_progress.IsDone(uploadId))
                {
                    await WriteEventAsync(context, UploadProgress.DoneMessage()).ConfigureAwait(false);

                    return;
                }

                if (_progress.TryTake(uploadId, out var unit))
                {
                    await WriteEventAsync(context, UploadProgress.Update(unit)).ConfigureAwait(false);

                    continue;
                }

                await Task.Delay(CheckRate, _clock, context.RequestAborted).ConfigureAwait(false);

                if (_clock.GetElapsedTime(heartbeat) > HeartbeatRate)
                {
                    await WriteEventAsync(context, UploadProgress.Heartbeat()).ConfigureAwait(false);
                    heartbeat = _clock.GetTimestamp();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The client went away, which is what stops the stream.
        }
        finally
        {
            _progress.StopTracking(uploadId);
        }
    }

    /// <summary>Serves a file the application keeps.</summary>
    /// <param name="context">The request.</param>
    /// <param name="prefix">The literal part of the route's template, or <see langword="null"/> for the path form.</param>
    /// <remarks>
    /// The order of the checks is the Python version's, and it matters: a path that names a
    /// location is refused before anything is resolved, and a path that resolves to a directory or
    /// to nothing is refused before the question of whether it is the application's is asked. Only
    /// then is the file read.
    /// </remarks>
    private Task FileAsync(HttpContext context, string? prefix)
    {
        var parameter = prefix is null
            ? context.Request.RouteValues["path"] as string
            : context.Request.RouteValues["path_or_url"] as string;

        // The route value is already decoded once, which is the single decode a path gets: the
        // address a file is served at is produced by encoding the path once, so decoding it twice
        // would read a name containing a literal '%41' as an escape and look for the wrong file.
        var requested = parameter ?? string.Empty;
        if (prefix is not null && requested.StartsWith(prefix, StringComparison.Ordinal))
        {
            requested = requested[prefix.Length..];
        }

        if (MediaCache.NamesALocation(requested))
        {
            return ForbiddenAsync(context, requested);
        }

        if (requested.Length == 0 || !System.IO.File.Exists(requested) || Directory.Exists(requested))
        {
            return ForbiddenAsync(context, requested);
        }

        var full = Path.GetFullPath(requested);
        if (!IsServed(full))
        {
            return ForbiddenAsync(context, requested);
        }

        return SendAsync(context, full);
    }

    /// <summary>Whether a file is one the application exposes.</summary>
    /// <param name="path">The file, absolute.</param>
    /// <remarks>
    /// <para>
    /// The order is the Python version's, and it is what makes a block a block: a path the
    /// application refuses is refused first, whatever else would have allowed it — including the
    /// application's own two folders. Then the paths it allowed, then those two folders: the one it
    /// writes to and the one its cached examples live in.
    /// </para>
    /// <para>
    /// The lists are compared without case on every system, as the Python version compares them, so
    /// a block written with one spelling holds for another.
    /// </para>
    /// </remarks>
    private bool IsServed(string path) =>
        !IsOneOf(path, _blocks.BlockedPaths)
        && (IsOneOf(path, _blocks.AllowedPaths)
            || MediaCache.IsInFolder(path, _blocks.UploadFolder)
            || MediaCache.IsInFolder(path, ExamplesCache));

    /// <summary>Whether a file is one of the directories an application declared, or below one.</summary>
    /// <param name="path">The file, absolute.</param>
    /// <param name="paths">The directories the application declared.</param>
    /// <returns>Whether the file is one of them.</returns>
    private static bool IsOneOf(string path, IReadOnlyList<string> paths) =>
        paths.Any(directory => MediaCache.IsInFolder(path, directory, StringComparison.OrdinalIgnoreCase));

    /// <summary>Writes a file, or the part of it a range request asked for.</summary>
    /// <param name="context">The request.</param>
    /// <param name="path">The file, absolute.</param>
    private static async Task SendAsync(HttpContext context, string path)
    {
        var (contentType, download) = MediaMimeTypes.ForServing(path, allowed: false);
        var type = MediaMimeTypes.WithCharset(contentType);
        var info = new FileInfo(path);
        var name = info.Name;

        context.Response.Headers.AcceptRanges = "bytes";
        context.Response.ContentType = type;
        context.Response.Headers.ContentDisposition = DispositionOf(name, download);

        if (TryReadRange(context.Request.Headers.Range, info.Length, out var from, out var to, out var unsatisfiable))
        {
            if (unsatisfiable)
            {
                context.Response.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
                context.Response.Headers.ContentRange = $"bytes */{info.Length}";

                return;
            }

            context.Response.StatusCode = StatusCodes.Status206PartialContent;
            context.Response.Headers.ContentRange = $"bytes {from}-{to}/{info.Length}";
            context.Response.ContentLength = to - from + 1;

            if (!HttpMethods.IsHead(context.Request.Method))
            {
                await context.Response.SendFileAsync(path, from, to - from + 1, context.RequestAborted)
                    .ConfigureAwait(false);
            }

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentLength = info.Length;

        if (!HttpMethods.IsHead(context.Request.Method))
        {
            await context.Response.SendFileAsync(path, context.RequestAborted).ConfigureAwait(false);
        }
    }

    /// <summary>How a served file tells the browser to display or save it.</summary>
    /// <param name="name">The file's name.</param>
    /// <param name="download">Whether the browser is told to save it rather than display it.</param>
    /// <returns>The header value.</returns>
    /// <remarks>
    /// The name is quoted, and a name that cannot be written as it is — one with a character
    /// outside ASCII — is carried in the RFC 5987 form instead. That is the spelling the HTTP layer
    /// the Python version is built on produces, and the one a browser reads back as the same name.
    /// </remarks>
    private static string DispositionOf(string name, bool download)
    {
        var type = download ? "attachment" : "inline";
        var escaped = Uri.EscapeDataString(name);

        return string.Equals(escaped, name, StringComparison.Ordinal)
            ? $"{type}; filename=\"{name}\""
            : $"{type}; filename*=utf-8''{escaped}";
    }

    /// <summary>Reads the byte range a request asked for.</summary>
    /// <param name="header">The <c>Range</c> header, or <see langword="null"/>.</param>
    /// <param name="length">The file's size.</param>
    /// <param name="from">The first byte to send.</param>
    /// <param name="to">The last byte to send, inclusive.</param>
    /// <param name="unsatisfiable">Whether the range names nothing the file has.</param>
    /// <returns>Whether the request asked for a range at all.</returns>
    /// <remarks>
    /// <para>
    /// The Python version answers only the <c>bytes=start-end</c> form with both bounds given, and
    /// answers anything else with the whole file; that is what this does, because the two clients
    /// that matter — a browser's media element and <c>gradio_client</c> — ask for exactly that
    /// form.
    /// </para>
    /// <para>
    /// A range that ends past the file is clamped to the last byte, and one that starts past it is
    /// refused. The Python version 6.30.0 clamps to the file's length instead, which is one byte
    /// too far: the read loop then waits for a byte that never comes and the response never
    /// finishes. The Python version fixed that in the release after the pinned one, and this is the
    /// fixed rule.
    /// </para>
    /// </remarks>
    private static bool TryReadRange(
        StringValues header,
        long length,
        out long from,
        out long to,
        out bool unsatisfiable)
    {
        from = 0;
        to = 0;
        unsatisfiable = false;

        var text = header.ToString().Trim();
        var match = RangeHeader().Match(text);
        if (!match.Success)
        {
            return false;
        }

        if (!long.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out from)
            || !long.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out to))
        {
            return false;
        }

        if (from >= length || to < from)
        {
            unsatisfiable = true;

            return true;
        }

        to = Math.Min(to, length - 1);

        return true;
    }

    /// <summary>The refusal for a path the application does not serve.</summary>
    /// <param name="context">The request.</param>
    /// <param name="path">The path that was asked for.</param>
    private static Task ForbiddenAsync(HttpContext context, string path) =>
        ProblemAsync(context, StatusCodes.Status403Forbidden, $"File not allowed: {path}.");

    private static Task NotFoundAsync(HttpContext context) =>
        ProblemAsync(context, StatusCodes.Status404NotFound, "Not Found");

    /// <summary>Answers the way the Python version's HTTP layer answers a refusal.</summary>
    private static Task ProblemAsync(HttpContext context, int status, string detail) =>
        WriteJsonAsync(context, new JsonObject { ["detail"] = detail }, status);

    /// <summary>Writes one server-sent event.</summary>
    private static async Task WriteEventAsync(HttpContext context, JsonObject message)
    {
        await context.Response.WriteAsync($"data: {JsonText.Serialize(message)}\n\n", context.RequestAborted)
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }

    private static async Task WriteJsonAsync(HttpContext context, JsonNode document, int status)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsync(JsonText.Serialize(document), context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Writes a plain-text answer, which is how an upload refusal is described.</summary>
    private static async Task WriteTextAsync(HttpContext context, string text, int status)
    {
        var body = Encoding.UTF8.GetBytes(text);

        context.Response.StatusCode = status;
        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.ContentLength = body.Length;

        await context.Response.Body.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>The directory cached examples are written to.</summary>
    /// <remarks>
    /// The Python version's <c>get_cache_folder</c>: <c>GRADIO_EXAMPLES_CACHE</c>, or a directory
    /// beside the working directory. It is one of the directories the file endpoint serves from,
    /// and example caching is its own batch (ticket 26), so for now it is a directory that exists
    /// only if the application made it.
    /// </remarks>
    private static string ExamplesCache =>
        Path.GetFullPath(Environment.GetEnvironmentVariable("GRADIO_EXAMPLES_CACHE") is { Length: > 0 } configured
            ? configured
            : Path.Combine(".gradio", "cached_examples"));

    /// <summary>A range request: both bounds given, which is the only form the Python version
    /// answers.</summary>
    [GeneratedRegex(@"^bytes=(\d+)-(\d+)$", RegexOptions.CultureInvariant)]
    private static partial Regex RangeHeader();
}

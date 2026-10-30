using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Serves a media stream: the playlist a player reads, the segments it plays, the whole stream as one
/// file, and the two routes a streamed run is fed through.
/// </summary>
/// <remarks>
/// <para>
/// A stream is not a file: it grows while it is being played, so the client is told where a
/// playlist is and re-reads it as the stream grows. That is HLS, and the shape of what is served
/// here is the Python version's to the character — the playlist's header lines, the <c>#EXTINF</c>
/// per segment, the end marker that says no more are coming, the media types, and the statuses a
/// missing stream or a missing segment is refused with. A browser's own player consumes it, so a
/// difference in any of them is a difference in what a user hears.
/// </para>
/// <para>
/// The two <c>POST</c> routes are the other half of the same feature and are not about playback: they
/// are how a browser feeds a run that streams its <em>input</em> — one request per chunk, and one to
/// say there are no more. They are here because they are named by the same path prefix and because
/// both ends of a stream are one conversation.
/// </para>
/// </remarks>
internal sealed class GradioStreamEndpoints : IGradioEndpoint
{
    private const string InputPath = "/gradio_api/stream/{event_id}";
    private const string ClosePath = "/gradio_api/stream/{event_id}/close";
    private const string PlaylistPath = "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist.m3u8";
    private const string SegmentPath = "/gradio_api/stream/{session_hash}/{run}/{component_id}/{segment_id}.{ext}";
    private const string FilePath = "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist-file";

    /// <summary>The extensions a segment may be served with, which is the Python version's
    /// list.</summary>
    private static readonly string[] _extensions = [".aac", ".ts"];

    private readonly Blocks _blocks;
    private readonly GradioQueue _queue;
    private readonly PendingStreams _streams;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application the streams belong to.</param>
    /// <param name="queue">The queue a streamed run waits in.</param>
    /// <param name="streams">The streams themselves.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioStreamEndpoints(Blocks blocks, GradioQueue queue, PendingStreams streams)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(streams);

        _blocks = blocks;
        _queue = queue;
        _streams = streams;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", InputPath).Key,
        new GradioRoute("POST", ClosePath).Key,
        new GradioRoute("GET", PlaylistPath).Key,
        new GradioRoute("GET", SegmentPath).Key,
        new GradioRoute("GET", FilePath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = context.GetEndpoint()?.Metadata.GetMetadata<GradioRouteMetadata>()?.Route.Path;

        return route switch
        {
            InputPath => FeedAsync(context),
            ClosePath => CloseAsync(context),
            PlaylistPath => PlaylistAsync(context),
            SegmentPath => SegmentAsync(context),
            FilePath => FileAsync(context),
            _ => NotFoundAsync(context),
        };
    }

    /// <summary>Feeds a running event the next chunk the browser sent.</summary>
    private async Task FeedAsync(HttpContext context)
    {
        var eventId = (string?)context.Request.RouteValues["event_id"] ?? string.Empty;
        var body = PredictBody.Parse(await ReadJsonAsync(context).ConfigureAwait(false));

        if (!_queue.PushStreamInput(eventId, body))
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        await SuccessAsync(context).ConfigureAwait(false);
    }

    /// <summary>Tells a running event that the browser will send nothing more.</summary>
    private async Task CloseAsync(HttpContext context)
    {
        var eventId = (string?)context.Request.RouteValues["event_id"] ?? string.Empty;

        if (!_queue.CloseStream(eventId))
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        await SuccessAsync(context).ConfigureAwait(false);
    }

    /// <summary>Serves a stream's playlist, as it stands.</summary>
    /// <remarks>
    /// The playlist is rebuilt per request rather than stored, because it is a view of what the
    /// stream has now: a player re-reads it as it plays, and a stored copy would be a copy of the
    /// stream as it was when it was first asked for. The end marker is the one thing that does not
    /// grow: a playlist without it is a live stream, and a player polls it forever.
    /// </remarks>
    private async Task PlaylistAsync(HttpContext context)
    {
        var stream = Find(context);

        if (stream is null)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        var signature = context.Request.Query["__sign"].ToString();
        var signatureQuery = signature.Length == 0
            ? string.Empty
            : "?" + Uri.EscapeDataString("__sign") + "=" + Uri.EscapeDataString(signature);

        var playlist = new StringBuilder()
            .Append("#EXTM3U\n")
            .Append("#EXT-X-PLAYLIST-TYPE:EVENT\n")
            .Append("#EXT-X-TARGETDURATION:").Append(stream.MaxDuration).Append('\n')
            .Append("#EXT-X-VERSION:4\n")
            .Append("#EXT-X-MEDIA-SEQUENCE:0\n")
            .Append("#EXT-X-START:TIME-OFFSET=0\n");

        foreach (var segment in stream.Segments)
        {
            playlist
                .Append("#EXTINF:")
                .Append(segment.Duration.ToString("F6", CultureInfo.InvariantCulture))
                .Append(",\n")
                .Append(segment.Id)
                .Append(segment.Extension)
                .Append(signatureQuery)
                .Append('\n');
        }

        if (stream.Ended)
        {
            playlist.Append("#EXT-X-ENDLIST\n");
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/vnd.apple.mpegurl";
        context.Response.Headers.CacheControl = signature.Length == 0 ? "no-store" : "private, no-store";

        await context.Response
            .WriteAsync(playlist.ToString(), context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Serves one segment of a stream.</summary>
    private async Task SegmentAsync(HttpContext context)
    {
        var extension = "." + ((string?)context.Request.RouteValues["ext"] ?? string.Empty);

        if (!_extensions.Contains(extension, StringComparer.Ordinal))
        {
            await ProblemAsync(context, StatusCodes.Status400BadRequest, "Unsupported file extension")
                .ConfigureAwait(false);

            return;
        }

        var stream = Find(context);

        if (stream is null)
        {
            await ProblemAsync(context, StatusCodes.Status404NotFound, "Stream not found").ConfigureAwait(false);

            return;
        }

        var id = (string?)context.Request.RouteValues["segment_id"] ?? string.Empty;
        var segment = stream.Segments.FirstOrDefault(candidate => candidate.Id == id);

        if (segment is null)
        {
            await ProblemAsync(context, StatusCodes.Status404NotFound, "Segment not found").ConfigureAwait(false);

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = extension == MediaStreamChunk.AudioExtension ? "audio/aac" : "video/MP2T";

        await context.Response
            .Body
            .WriteAsync(segment.Data, context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Serves the whole stream as one file, combining the segments the first time it is asked.</summary>
    /// <remarks>
    /// The Python version remuxes the segments with ffmpeg, which this framework does not run: the
    /// combined file is the segments' bytes one after another, named after the format the component
    /// said the stream was for. For packed audio that is the whole file already — an ADTS stream
    /// concatenates into a valid one — and for video it is a transport stream with the wrong name,
    /// which is a gap this batch declares rather than hides.
    /// </remarks>
    private async Task FileAsync(HttpContext context)
    {
        var stream = Find(context);

        if (stream is null)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        stream.CombinedFile ??= Combine(stream);

        var (contentType, download) = MediaMimeTypes.ForServing(stream.CombinedFile, allowed: true);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = contentType;

        if (download)
        {
            context.Response.Headers.ContentDisposition =
                $"attachment; filename=\"{Path.GetFileName(stream.CombinedFile)}\"";
        }

        await context.Response
            .SendFileAsync(stream.CombinedFile, context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>The stream a request names, or <see langword="null"/> when it names none.</summary>
    private MediaStream? Find(HttpContext context)
    {
        var session = (string?)context.Request.RouteValues["session_hash"];
        var run = (string?)context.Request.RouteValues["run"];
        var component = context.Request.RouteValues["component_id"]?.ToString();

        return session is null
            || run is null
            || !int.TryParse(component, CultureInfo.InvariantCulture, out var componentId)
            ? null
            : _streams.Find(session, run, componentId);
    }

    /// <summary>The segments of a stream written out as one file, in the application's cache.</summary>
    /// <param name="stream">The stream to combine.</param>
    /// <returns>The path of the combined file.</returns>
    private string Combine(MediaStream stream)
    {
        var name = stream.DesiredOutputFormat is { Length: > 0 } format
            ? $"stream.{format}"
            : "stream.mp4";

        using var combined = new MemoryStream();
        foreach (var segment in stream.Segments)
        {
            combined.Write(segment.Data);
        }

        var path = MediaCache.SaveBytes(combined.ToArray(), name, _blocks.UploadFolder);
        _blocks.TrackTempFile(path);

        return path;
    }

    /// <summary>The request's body as JSON, or <see langword="null"/> when it is not one.</summary>
    private static async Task<JsonNode?> ReadJsonAsync(HttpContext context)
    {
        try
        {
            return await JsonNode
                .ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    /// <summary>Answers the success a fed or closed stream is answered with.</summary>
    private static async Task SuccessAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        await context.Response
            .WriteAsync("""{"msg":"success"}""", context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Answers with a bare status and no body, which is what a missing stream gets.</summary>
    private static Task NotFoundAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return Task.CompletedTask;
    }

    /// <summary>Answers with a status and a plain-text body, which is what a refused segment gets.</summary>
    private static async Task ProblemAsync(HttpContext context, int status, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "text/plain; charset=utf-8";

        await context.Response.WriteAsync(detail, context.RequestAborted).ConfigureAwait(false);
    }
}

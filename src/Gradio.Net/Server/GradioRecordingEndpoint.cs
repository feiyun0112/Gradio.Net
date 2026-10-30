using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The endpoint a screen recording is sent to, which answers with the recording as a file.
/// </summary>
/// <remarks>
/// <para>
/// A video component can record the screen, and what it records is not what the user wants to keep:
/// the browser sends the raw capture and the server trims and zooms it. The Python version does
/// that with ffmpeg, and returns the capture untouched when ffmpeg is not installed — which is the
/// only branch this framework has, because it does not run external processes. The trimming
/// parameters are therefore read and accepted, and not acted on.
/// </para>
/// <para>
/// The file is answered with the name the browser expects, so that a download of a recording is
/// called a recording rather than a temporary file's name.
/// </para>
/// </remarks>
internal sealed class GradioRecordingEndpoint : IGradioEndpoint
{
    private const string RoutePath = "/gradio_api/process_recording";

    /// <summary>The name the answered file carries, which is the Python version's.</summary>
    private const string RecordingName = "gradio-screen-recording.mp4";

    private readonly Blocks _blocks;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application the recording belongs to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    public GradioRecordingEndpoint(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("POST", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Request.ContentType is not { } contentType
            || !contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            await RefuseAsync(context, StatusCodes.Status400BadRequest, "Invalid content type.")
                .ConfigureAwait(false);

            return;
        }

        IFormFile? video;
        try
        {
            var form = await context.Request.ReadFormAsync(context.RequestAborted).ConfigureAwait(false);
            video = form.Files.GetFile("video");
        }
        catch (InvalidDataException error)
        {
            // A body that says it is multipart and is not is a bad request, which is what the Python version
            // answers: the parser's failure is the caller's mistake rather than the server's.
            await RefuseAsync(context, StatusCodes.Status400BadRequest, error.Message).ConfigureAwait(false);

            return;
        }

        if (video is null)
        {
            await RefuseAsync(context, StatusCodes.Status400BadRequest, "No video file provided")
                .ConfigureAwait(false);

            return;
        }

        using var buffer = new MemoryStream();
        await video.CopyToAsync(buffer, context.RequestAborted).ConfigureAwait(false);

        var path = MediaCache.SaveBytes(buffer.ToArray(), "recording.mp4", _blocks.UploadFolder);
        _blocks.TrackTempFile(path);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "video/mp4";
        context.Response.Headers.ContentDisposition = $"inline; filename=\"{RecordingName}\"";

        await context.Response.SendFileAsync(path, context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>Answers a refusal with the body FastAPI would have written.</summary>
    private static async Task RefuseAsync(HttpContext context, int status, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response
            .WriteAsync(JsonText.Serialize(new System.Text.Json.Nodes.JsonObject { ["detail"] = detail }),
                context.RequestAborted)
            .ConfigureAwait(false);
    }
}

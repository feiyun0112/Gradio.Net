using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A video input or output: uploaded, recorded from a webcam, or played.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Two of them are computed rather than stored, because
/// their defaults depend on another parameter: <see cref="IncludeAudio"/> on whether the video may
/// be uploaded, and <see cref="Buttons"/> on nothing at all — the Python version's default is the
/// download button.
/// </para>
/// <para>
/// Converting a video to a browser-playable container is ffmpeg's job in the Python version, and this
/// framework does not run external processes: a video is served as it is, and an application that
/// needs a particular container converts it before handing it over. Everything else — reading the
/// value a handler gets, describing the value a handler returns, and streaming chunks — is here.
/// </para>
/// </remarks>
[GradioBlock("video")]
[HandlerValue("string", "change", "clear", "start_recording", "stop_recording", "stop", "play", "pause", "end", "upload", "input")]
public partial class Video : Component, IStreamingOutput
{
    internal Video()
    {
    }

    /// <summary>The sample video the Python version's examples point at.</summary>
    private const string SampleVideo =
        "https://github.com/gradio-app/gradio/raw/main/gradio/media_assets/videos/world.mp4";

    /// <summary>The sources a video may be taken from.</summary>
    private static readonly IReadOnlyList<string> _allSources = ["upload", "webcam"];

    /// <summary>The buttons a video shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["download"];

    private IReadOnlyList<string> _sources = _allSources;
    private object? _value;
    private bool? _includeAudio;
    private JsonNode? _described;
    private string? _describedIn;
    private object? _subtitles;
    private JsonNode? _describedSubtitles;
    private string? _subtitlesDescribedIn;

    /// <summary>The video the component starts with.</summary>
    /// <value>
    /// A path or a URL, as the application wrote it; read back as the postprocessed value — a file
    /// object the frontend can play.
    /// </value>
    public object? Value
    {
        get
        {
            if (_described is null || !string.Equals(_describedIn, MediaFolder, StringComparison.Ordinal))
            {
                _described = Postprocess(_value);
                _describedIn = MediaFolder;
            }

            return _described;
        }
        set
        {
            _value = value;
            _described = null;
            _describedIn = null;
        }
    }

    /// <summary>The container a video is converted to, or <see langword="null"/> to leave it as it is.</summary>
    /// <remarks>
    /// Carried in the config and used to name a file written from bytes; converting one container to
    /// another is ffmpeg's job, which this framework does not do.
    /// </remarks>
    public string? Format { get; set; }

    /// <summary>Where a video may come from.</summary>
    /// <value>Upload and webcam unless the application says otherwise.</value>
    public IReadOnlyList<string> Sources
    {
        get => _sources;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            foreach (var source in value)
            {
                if (!_allSources.Contains(source, StringComparer.Ordinal))
                {
                    throw new ArgumentException(
                        $"A video's sources must be a list of {string.Join(", ", _allSources)}; "
                        + $"got '{source}'.",
                        nameof(value));
                }
            }

            _sources = value;
        }
    }

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The width of the component, in pixels or as a CSS length.</summary>
    public object? Width { get; set; }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The options the webcam is opened with.</summary>
    public WebcamOptions WebcamOptions { get; set; } = new();

    /// <summary>Whether the video's audio track is kept.</summary>
    /// <value>
    /// Whatever the application set, or — when it set nothing — whether the video may be uploaded:
    /// a recorded clip has no audio of its own to keep, which is the Python version's rule.
    /// </value>
    public bool IncludeAudio
    {
        get => _includeAudio ?? Sources.Contains("upload", StringComparer.Ordinal);
        set => _includeAudio = value;
    }

    /// <summary>Whether the video plays as soon as it is shown.</summary>
    public bool Autoplay { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <summary>Whether the video starts again when it reaches its end.</summary>
    public bool Loop { get; set; }

    /// <summary>Whether the component plays the chunks a handler yields as one stream.</summary>
    public override bool Streaming { get; set; }

    /// <summary>The watermark drawn over the video.</summary>
    public WatermarkOptions Watermark { get; set; } = new();

    /// <summary>The subtitles shown with the video.</summary>
    /// <value>
    /// A path or a file object, or a list of <see cref="Subtitle"/> entries — which are written to a
    /// WebVTT file in the cache, because that is the only subtitle format a browser plays. Read back
    /// as the file object the config carries.
    /// </value>
    public object? Subtitles
    {
        get
        {
            if (_describedSubtitles is null
                || !string.Equals(_subtitlesDescribedIn, MediaFolder, StringComparison.Ordinal))
            {
                _describedSubtitles = DescribeSubtitles(_subtitles);
                _subtitlesDescribedIn = MediaFolder;
            }

            return _describedSubtitles;
        }
        set
        {
            _subtitles = value;
            _describedSubtitles = null;
            _subtitlesDescribedIn = null;
        }
    }

    /// <summary>Where playback starts, in seconds.</summary>
    public double PlaybackPosition { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["path"] = SampleVideo,
        ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        ["orig_name"] = "world.mp4",
        ["url"] = SampleVideo,
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a file object, or a path.</param>
    /// <returns>The path of the cached video, or <see langword="null"/> when there is none.</returns>
    public override object? Preprocess(JsonNode? value) => MediaFiles.PathOf(value, MediaFolder, Root);

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL, a video's bytes, a stream of them, or a file object.</param>
    /// <returns>The video as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        var described = MediaFiles.From(
            value,
            MediaFolder,
            $"video.{Format ?? "mp4"}",
            "A video component");

        if (described?.Path is not { Length: > 0 })
        {
            return null;
        }

        return MediaFiles.Describe(described, MediaFolder, Root, measure: false).ToJson();
    }

    /// <summary>The subtitles as the config carries them.</summary>
    /// <param name="subtitles">A path, a file object, or a list of entries.</param>
    /// <returns>A file object, or <see langword="null"/> when there are no subtitles.</returns>
    /// <remarks>
    /// A list is written to a WebVTT file in the application's cache because a browser plays
    /// nothing else, which is the Python version's conversion. A file is passed through as it is:
    /// an <c>.srt</c> file would have to be converted too, and that conversion is the media
    /// utilities' batch (ticket 38).
    /// </remarks>
    private JsonObject? DescribeSubtitles(object? subtitles)
    {
        switch (subtitles)
        {
            case null:
                return null;

            case IEnumerable<Subtitle> entries:
                var vtt = WebVtt(entries);
                var path = MediaCache.SaveBytes(
                    Encoding.UTF8.GetBytes(vtt),
                    "subtitles.vtt",
                    MediaFolder);

                return MediaFiles.Describe(new FileData { Path = path }, MediaFolder, Root, measure: false)
                    .ToJson();

            case FileData file:
                return MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson();

            case JsonNode node:
                return FileData.FromJson(node) is { } described
                    ? MediaFiles.Describe(described, MediaFolder, Root, measure: false).ToJson()
                    : null;

            case string text:
                return MediaFiles.Describe(new FileData { Path = text }, MediaFolder, Root, measure: false)
                    .ToJson();

            default:
                throw new NotSupportedException(
                    "A video's subtitles are a path, a file object, or a list of subtitle entries; "
                    + $"got a {subtitles.GetType()}.");
        }
    }

    /// <summary>The entries as a WebVTT document.</summary>
    private static string WebVtt(IEnumerable<Subtitle> entries)
    {
        var document = new StringBuilder("WEBVTT\n\n");

        foreach (var entry in entries)
        {
            document.Append(Stamp(entry.Start)).Append(" --> ").Append(Stamp(entry.End)).Append('\n');
            document.Append(entry.Text).Append("\n\n");
        }

        return document.ToString();
    }

    /// <summary>A moment as WebVTT spells it: hours, minutes, seconds and milliseconds.</summary>
    private static string Stamp(double seconds)
    {
        var hours = (int)(seconds / 3600);
        var minutes = (int)(seconds % 3600 / 60);
        var rest = seconds % 60;

        return string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"{hours:00}:{minutes:00}:{rest:00.000}");
    }

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(object? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        _ => [],
    };
}

/// <summary>
/// One subtitle entry: the words, and when they are shown.
/// </summary>
/// <remarks>
/// The Python version's list form of <c>subtitles</c>, which it converts to WebVTT before the
/// frontend sees it. The times are seconds from the start of the media.
/// </remarks>
public sealed class Subtitle
{
    /// <summary>When the words appear, in seconds.</summary>
    public double Start { get; set; }

    /// <summary>When they go away, in seconds.</summary>
    public double End { get; set; }

    /// <summary>The words.</summary>
    public string Text { get; set; } = string.Empty;
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// An audio input or output: uploaded, recorded from a microphone, or played.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Two defaults depend on another parameter: the sources
/// are the microphone alone when the component streams, and the buttons are the download and share
/// buttons.
/// </para>
/// <para>
/// Converting audio between containers, and turning a sample rate and a buffer of samples into a
/// file, are the media utilities' batch (ticket 38). What is here is the file part: the path a
/// handler reads, the file object a handler's result becomes, and the stream its chunks make.
/// </para>
/// </remarks>
[GradioBlock("audio")]
[HandlerValue("string", "stream", "change", "clear", "play", "pause", "stop", "start_recording", "pause_recording", "stop_recording", "upload", "input")]
public partial class Audio : Component, IStreamingOutput
{
    internal Audio()
    {
    }

    /// <summary>The sample audio the Python version's examples point at.</summary>
    private const string SampleAudio =
        "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav";

    /// <summary>The sources audio may be taken from.</summary>
    private static readonly IReadOnlyList<string> _allSources = ["upload", "microphone"];

    /// <summary>The buttons an audio player shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["download", "share"];

    private string _type = "numpy";
    private IReadOnlyList<string>? _sources;
    private object? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The audio the component starts with.</summary>
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

    /// <summary>Where audio may come from.</summary>
    /// <value>
    /// Whatever the application set, or — when it set nothing — both sources, or the microphone
    /// alone for a component that streams, which is the Python version's rule.
    /// </value>
    [FactoryArgument(FactoryArgumentKind.Names)]
    public IReadOnlyList<string> Sources
    {
        get => _sources ?? (Streaming ? ["microphone"] : _allSources);
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            foreach (var source in value)
            {
                if (!_allSources.Contains(source, StringComparer.Ordinal))
                {
                    throw new ArgumentException(
                        $"An audio component's sources must be a list of {string.Join(", ", _allSources)}; "
                        + $"got '{source}'.",
                        nameof(value));
                }
            }

            _sources = value;
        }
    }

    /// <summary>The format the value is converted to before a handler reads it.</summary>
    /// <value><c>numpy</c> or <c>filepath</c>.</value>
    /// <remarks>
    /// <para>
    /// Carried in the config and validated here, because it is one of the component's parameters and
    /// the config is compared field by field. A handler reads a path whatever it says: turning a
    /// file into a sample rate and a buffer means decoding the audio, and every C# library that
    /// decodes the formats a browser records does it by running ffmpeg — which is the one thing this
    /// framework does not do.
    /// </para>
    /// <para>
    /// What an application does instead is what it would do with <c>filepath</c>: take the path and
    /// decode it with whatever it already uses. The exclusion is registered in the scope ledger, and
    /// this is the shape of it here rather than a refusal, because a component that threw on its own
    /// default value would be a component nobody could use.
    /// </para>
    /// </remarks>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("numpy" or "filepath"))
            {
                throw new ArgumentException(
                    $"An audio component's type must be numpy or filepath; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether the component streams what it records, or the chunks a handler yields.</summary>
    /// <remarks>
    /// Binding the component's <c>stream</c> event turns this on, which is the Python version's
    /// rule — the event is what asks for a stream, and the parameter says so in advance.
    /// </remarks>
    public override bool Streaming { get; set; }

    /// <summary>The format an audio file is saved as, or <see langword="null"/> to leave it as it is.</summary>
    /// <value><c>wav</c>, <c>mp3</c>, or <see langword="null"/>.</value>
    public string? Format
    {
        get => _format;
        set
        {
            if (value is not null && value.ToLowerInvariant() is not ("wav" or "mp3"))
            {
                throw new ArgumentException(
                    $"An audio component's format must be wav or mp3; got '{value}'.",
                    nameof(value));
            }

            _format = value?.ToLowerInvariant();
        }
    }

    private string? _format;

    /// <summary>Whether the audio plays as soon as it is shown.</summary>
    public bool Autoplay { get; set; }

    /// <summary>Whether the user may trim and edit the audio.</summary>
    public bool Editable { get; set; } = true;

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <summary>How the waveform is drawn, and what its skip buttons skip.</summary>
    public WaveformOptions WaveformOptions { get; set; } = new();

    /// <summary>Whether the audio starts again when it reaches its end.</summary>
    public bool Loop { get; set; }

    /// <summary>Whether the component opens on its microphone, ready to record.</summary>
    public bool Recording { get; set; }

    /// <summary>The subtitles shown with the audio.</summary>
    /// <value>
    /// A path or a file object, or a list of <see cref="Subtitle"/> entries, which are carried as
    /// the objects the frontend reads rather than as a file.
    /// </value>
    public object? Subtitles
    {
        get => _subtitles is null ? null : DescribeSubtitles(_subtitles);
        set => _subtitles = value;
    }

    private object? _subtitles;

    /// <summary>Where playback starts, in seconds.</summary>
    public double PlaybackPosition { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["path"] = SampleAudio,
        ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        ["orig_name"] = "audio_sample.wav",
        ["url"] = SampleAudio,
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <inheritdoc />
    /// <remarks>
    /// An audio component declares the <c>stream</c> event, so the config carries <c>streamable</c>.
    /// </remarks>
    protected internal override void WriteEventConfig(PropWriter props)
    {
        base.WriteEventConfig(props);
        props.Add("streamable", false);
    }

    /// <summary>Records that the <c>stream</c> event was bound, which is what turns streaming on.</summary>
    /// <param name="eventName">The signal's name.</param>
    /// <exception cref="InvalidOperationException">
    /// The component streams but its sources do not include the microphone, which is the Python
    /// version's <c>check_streamable</c>: a stream is what the microphone produces, so a component
    /// that refuses the microphone has nothing to stream.
    /// </exception>
    /// <remarks>
    /// The Python version's event listener does this with a callback: binding <c>.stream()</c> sets
    /// the component's <c>streaming</c> attribute, so a handler that streams does not also have to
    /// construct the component with <c>streaming: true</c>.
    /// </remarks>
    protected internal override void OnEventBound(string eventName)
    {
        base.OnEventBound(eventName);

        if (eventName != EventNames.Stream)
        {
            return;
        }

        Streaming = true;

        if (!Sources.Contains("microphone", StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                "Audio streaming only available if sources includes 'microphone'.");
        }
    }

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a file object, or a path.</param>
    /// <returns>The path of the cached audio, or <see langword="null"/> when there is none.</returns>
    public override object? Preprocess(JsonNode? value) => MediaFiles.PathOf(value, MediaFolder, Root);

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL, the audio's bytes, a stream of them, or a file object.</param>
    /// <returns>The audio as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        var described = MediaFiles.From(
            value,
            MediaFolder,
            $"audio.{Format ?? "wav"}",
            "An audio component");

        if (described?.Path is not { Length: > 0 })
        {
            return null;
        }

        return MediaFiles.Describe(described, MediaFolder, Root, measure: false).ToJson();
    }

    /// <summary>The subtitles as the config carries them.</summary>
    /// <param name="subtitles">A path, a file object, or a list of entries.</param>
    /// <returns>The entries as objects, or the file as a file object.</returns>
    private JsonNode? DescribeSubtitles(object? subtitles) => subtitles switch
    {
        null => null,

        // The Python version rewrites each entry to the start/end/text spelling the frontend reads, rather
        // than passing the application's own shape through.
        IEnumerable<Subtitle> entries => new JsonArray([.. entries.Select(entry => (JsonNode)new JsonObject
        {
            ["start"] = entry.Start,
            ["end"] = entry.End,
            ["text"] = entry.Text,
        })]),

        FileData file => MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson(),

        JsonNode node => FileData.FromJson(node) is { } described
            ? MediaFiles.Describe(described, MediaFolder, Root, measure: false).ToJson()
            : null,

        string text => MediaFiles.Describe(new FileData { Path = text }, MediaFolder, Root, measure: false)
            .ToJson(),

        _ => throw new NotSupportedException(
            "An audio component's subtitles are a path, a file object, or a list of subtitle "
            + $"entries; got a {subtitles.GetType()}."),
    };

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(object? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        _ => [],
    };
}

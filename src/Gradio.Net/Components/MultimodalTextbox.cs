using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A text field that takes files as well as text: what a chat box needs to accept a picture
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a text and a list of files (see
/// <see cref="MultimodalText"/>), and the config always carries both — an empty field is an empty
/// text and no files, which is what the frontend reads as "nothing sent yet".
/// </para>
/// <para>
/// <see cref="Sources"/> decides which buttons the field offers, <see cref="FileCount"/> whether it
/// takes one file or several, and <see cref="SubmitBtn"/> and <see cref="StopBtn"/> whether the field
/// carries the two buttons a streaming handler needs — each of them a string when the application
/// wants its own text on the button rather than the default word.
/// </para>
/// </remarks>
[GradioBlock("multimodaltextbox")]
[HandlerValue("MultimodalText", "change", "input", "submit", "focus", "blur", "stop")]
public sealed partial class MultimodalTextbox : FormComponent
{
    internal MultimodalTextbox()
    {
    }

    private static readonly IReadOnlyList<string> _defaultSources = ["upload"];
    private static readonly IReadOnlyList<string> _validSources = ["upload", "microphone"];

    private object? _value;
    private IReadOnlyList<string> _sources = _defaultSources;
    private int _maxLines = 20;

    /// <summary>The text and files the field starts with.</summary>
    /// <value>
    /// A <see cref="MultimodalText"/>, or a string read as a text with no files; an absent value is
    /// an empty field.
    /// </value>
    /// <exception cref="ArgumentException">The value is neither a text nor absent.</exception>
    public object? Value
    {
        get => _value switch
        {
            null => new MultimodalText(string.Empty),
            MultimodalText value => value,
            string text => new MultimodalText(text),
            _ => throw new ArgumentException(
                $"A multimodal textbox's value is a text or a {nameof(MultimodalText)}; "
                + $"got a {_value.GetType()}.",
                nameof(Value)),
        };
        set => _value = value;
    }

    /// <summary>Where the files may come from.</summary> <value><c>upload</c> unless the
    /// application asks for the microphone too.</value> <exception cref="ArgumentException">A
    /// source is not one the Python version offers.</exception>
    public IReadOnlyList<string> Sources
    {
        get => _sources;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            foreach (var source in value)
            {
                if (!_validSources.Contains(source, StringComparer.Ordinal))
                {
                    throw new ArgumentException(
                        $"A source must be one of {string.Join(", ", _validSources)}; got '{source}'.",
                        nameof(value));
                }
            }

            _sources = value;
        }
    }

    /// <summary>The kinds of file the field accepts, or <see langword="null"/> for any.</summary>
    public IReadOnlyList<string>? FileTypes { get; set; }

    /// <summary>Whether the field takes one file, several, or a directory.</summary>
    /// <value><c>single</c>, <c>multiple</c> or <c>directory</c>.</value>
    public string FileCount { get; set; } = "single";

    /// <summary>How many lines tall the field is drawn.</summary>
    public int Lines { get; set; } = 1;

    /// <summary>How many lines the field grows to at most.</summary>
    /// <value>The larger of <see cref="Lines"/> and the number the application gave.</value>
    public int MaxLines
    {
        get => Math.Max(Lines, _maxLines);
        set => _maxLines = value;
    }

    /// <summary>The hint shown while the field is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The label drawn above the field.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the field next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the field wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>Whether the field takes focus when the page loads.</summary>
    public bool Autofocus { get; set; }

    /// <summary>Whether the field scrolls to the bottom as text is added.</summary>
    public bool Autoscroll { get; set; } = true;

    /// <summary>The horizontal alignment of the text.</summary>
    public string? TextAlign { get; set; }

    /// <summary>Whether the text runs right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>Whether the field carries a submit button, and what it says.</summary>
    /// <value><see langword="true"/> for the default word, a string for the application's own, or <see langword="false"/> for no button.</value>
    public object? SubmitBtn { get; set; } = true;

    /// <summary>Whether the field carries a stop button, and what it says.</summary>
    /// <value><see langword="false"/> for none, <see langword="true"/> for the default word, or a string for the application's own.</value>
    public object? StopBtn { get; set; } = false;

    /// <summary>The longest text the field holds before it turns the text into a file.</summary>
    public int MaxPlainTextLength { get; set; } = 1000;

    /// <summary>The HTML attributes put on the text area.</summary>
    public object? HtmlAttributes { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.MultimodalSchema();

    /// <inheritdoc />
    /// <remarks>
    /// The example the Python version offers is a picture from its own repository, described as the
    /// protocol describes one: the address is the path and the address, the file keeps its name,
    /// and nothing about its size is claimed because nothing looked.
    /// </remarks>
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["text"] = "Describe this image",
        ["files"] = new JsonArray(new JsonObject
        {
            ["path"] = ExampleImageUrl,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "bus.png",
            ["url"] = ExampleImageUrl,
        }),
    };

    /// <summary>The picture the Python version's example names.</summary>
    private const string ExampleImageUrl =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The text and files a handler reads.</summary>
    /// <param name="value">The value the client sent.</param>
    /// <returns>The value, or <see langword="null"/> when nothing was sent.</returns>
    /// <exception cref="ArgumentException">A file is not one of the kinds the field accepts.</exception>
    /// <remarks>
    /// <para>
    /// The kinds are checked here as well as offered to the browser, because a client is not
    /// obliged to honour what the field said it accepts: the Python version refuses a file the
    /// application did not ask for rather than handing it to a handler that cannot read it.
    /// </para>
    /// <para>
    /// The message is the Python version's: the file's media type, then the kinds accepted as
    /// Python writes a list of them.
    /// </para>
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        var multimodal = MultimodalText.FromJson(value);

        if (multimodal is not null && FileTypes is { } accepted)
        {
            foreach (var file in multimodal.Files)
            {
                if (file.Path is { } path && !MediaTypes.Accepts(path, accepted))
                {
                    throw new ArgumentException(
                        $"Invalid file type: {MediaTypeOf(file)}. "
                        + MediaTypes.AcceptedFormats(accepted),
                        nameof(value));
                }
            }
        }

        return multimodal;
    }

    /// <summary>The media type a refused file is named by.</summary>
    /// <param name="file">The file the client sent.</param>
    /// <returns>The type the client gave it, or the one its extension implies.</returns>
    /// <remarks>
    /// The Python version names it from the file's own <c>mime_type</c>, which is a field of the
    /// payload the client fills in — and one this framework's uploads fill in too. A payload that
    /// left it out is read by extension, and a file whose extension names nothing is
    /// <c>application/octet-stream</c>, which is what <c>mimetypes.guess_type</c> leaves its
    /// callers to say.
    /// </remarks>
    private static string MediaTypeOf(FileData file) =>
        file.MimeType ?? MediaTypes.Of(file.Path) ?? UnknownMediaType;

    /// <summary>What a file's media type is called when neither the client nor its name says.</summary>
    private const string UnknownMediaType = "application/octet-stream";

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">A <see cref="MultimodalText"/>, a text, or <see langword="null"/>.</param>
    /// <returns>The value, with an absent one read as an empty field.</returns>
    /// <exception cref="ArgumentException">The value is neither a text nor a multimodal one.</exception>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => new MultimodalText(string.Empty).ToConfigValue(),
        MultimodalText multimodal => multimodal.ToConfigValue(),
        string text => new MultimodalText(text).ToConfigValue(),
        _ => throw new ArgumentException(
            $"A multimodal textbox's value is a text or a {nameof(MultimodalText)}; got a {value.GetType()}.",
            nameof(value)),
    };
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// An image input or output: uploaded, pasted, taken from a webcam, or displayed.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. <see cref="Sources"/> and <see cref="Buttons"/> have
/// defaults that depend on another parameter, which is why they are computed on read rather than
/// stored: the Python version computes them in its constructor, where every argument is already
/// known.
/// </para>
/// <para>
/// What this component does with an image is the Python version's: it decodes it, converts it to
/// the shape <see cref="Type"/> names — a decoded image or a path — and writes it in
/// <see cref="Format"/> on the way out, drawing a watermark if the application asked for one. The
/// two decoded shapes the Python version has are one here, because C# has one container for a
/// picture.
/// </para>
/// </remarks>
[GradioBlock("image")]
[HandlerValue("string", "change", "input", "upload", "stream", "select", "clear")]
public sealed partial class Image : Component
{
    internal Image()
    {
    }

    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The sources an image may be taken from.</summary>
    private static readonly IReadOnlyList<string> _allSources = ["upload", "webcam", "clipboard"];

    /// <summary>The buttons a component shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["download", "share", "fullscreen"];

    private string _type = "numpy";
    private IReadOnlyList<string>? _sources;
    private JsonNode? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The image the component starts with.</summary>
    /// <value>
    /// A path or a URL, as the application wrote it; read back as the postprocessed value — a file
    /// object the frontend can display.
    /// </value>
    /// <remarks>
    /// <para>
    /// Postprocessed on read for the same reason as a file's: the copy into the application's cache
    /// needs the application, which is known only after the component has been rendered into one.
    /// </para>
    /// <para>
    /// The described value is remembered per cache folder rather than once: a read before the
    /// component was rendered copies the image into the process default, and that copy is not one
    /// the application's file endpoint serves.
    /// </para>
    /// </remarks>
    public JsonNode? Value
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

    /// <summary>The format an image is saved as when it is written to the cache.</summary>
    /// <value><c>webp</c> unless the application says otherwise.</value>
    public string Format { get; set; } = "webp";

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The width of the component, in pixels or as a CSS length.</summary>
    public object? Width { get; set; }

    /// <summary>The pixel format the image is loaded as, or <see langword="null"/> to infer it.</summary>
    public string? ImageMode { get; set; } = "RGB";

    /// <summary>Where an image may come from.</summary>
    /// <value>
    /// Whatever the application set, or — when it set nothing — every source for an ordinary
    /// component and only the webcam for a streaming one, which is the Python version's rule.
    /// </value>
    public IReadOnlyList<string> Sources
    {
        get => _sources ?? (Streaming ? ["webcam"] : _allSources);
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            foreach (var source in value)
            {
                if (!_allSources.Contains(source, StringComparer.Ordinal))
                {
                    throw new ArgumentException(
                        $"An image's sources must be a list of {string.Join(", ", _allSources)}; "
                        + $"got '{source}'.",
                        nameof(value));
                }
            }

            _sources = value;
        }
    }

    /// <summary>The format the value is converted to before a handler reads it.</summary>
    /// <value><c>numpy</c>, <c>pil</c> or <c>filepath</c>.</value>
    /// <remarks>
    /// <c>numpy</c> and <c>pil</c> hand a handler a <see cref="RasterImage"/>; <c>filepath</c>
    /// hands it a path. The Python version's two decoded shapes differ in their container, and a
    /// .NET application has one — the array a numpy value is and the object a PIL one is are the
    /// same pixels.
    /// </remarks>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("numpy" or "pil" or "filepath"))
            {
                throw new ArgumentException(
                    $"An image's type must be one of numpy, pil or filepath; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <remarks>
    /// Names only, for now: a custom <c>Button</c> is a block, and the button family is its own
    /// batch (ticket 15). The Python version's default is the three built-in ones.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether the webcam feed is streamed to the server as it is captured.</summary>
    /// <remarks>
    /// The Python version's own signature types this as a boolean, and its implementation
    /// additionally understands the string <c>base64</c>, which makes an output send its image
    /// inline. That spelling is not expressible here: streaming media is its own batch (ticket 16),
    /// and a flag that is sometimes a string is not a flag.
    /// </remarks>
    public override bool Streaming { get; set; }

    /// <summary>The options the webcam is opened with.</summary>
    public WebcamOptions WebcamOptions { get; set; } = new();

    /// <summary>The text shown where an image would be dropped.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The watermark drawn on a displayed image.</summary>
    public WatermarkOptions Watermark { get; set; } = new();

    /// <summary>The alternative text a screen reader reads for the image.</summary>
    public string? AltText { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.ImageSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["path"] = SampleImage,
        ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        ["orig_name"] = "bus.png",
        ["url"] = SampleImage,
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>Records that the <c>stream</c> event was bound, which is what turns streaming on.</summary>
    /// <param name="eventName">The signal's name.</param>
    /// <exception cref="InvalidOperationException">
    /// The component streams but its sources are not the webcam alone, which is the Python
    /// version's <c>check_streamable</c>: a stream is what a camera produces, and a component that
    /// also offers uploads or the clipboard has more than one thing its value could be.
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

        if (Sources is not ["webcam"])
        {
            throw new InvalidOperationException(
                "Image streaming only available if sources is ['webcam']. Streaming not supported "
                + "with multiple sources.");
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// An image declares the <c>stream</c> event, and that event contributes <c>streamable</c> to the
    /// config — a key about the component rather than about one of its parameters, which the
    /// frontend reads before it offers to stream anything.
    /// </remarks>
    protected internal override void WriteEventConfig(PropWriter props)
    {
        base.WriteEventConfig(props);
        props.Add("streamable", false);
    }

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a file object, or a path.</param>
    /// <returns>
    /// A <see cref="RasterImage"/> when <see cref="Type"/> names one of the decoded shapes, a path
    /// when it is <c>filepath</c>, and <see langword="null"/> when there is no image.
    /// </returns>
    /// <remarks>
    /// The Python version's <c>preprocess_image</c>; the work itself is
    /// <see cref="ImagePipeline"/>'s, which the other image components ask too. The two decoded
    /// shapes — <c>numpy</c> and <c>pil</c> — are one here, because C# has one container for a
    /// picture: the array a numpy value is and the object a PIL one is are the same pixels.
    /// </remarks>
    public override object? Preprocess(JsonNode? value) =>
        ImagePipeline.Preprocess(FileData.FromJson(value), Shape);

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">
    /// A path, a URL, a <see cref="RasterImage"/>, an image's bytes, a stream of them, or a file
    /// object.
    /// </param>
    /// <returns>The image as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>postprocess_image</c>: a watermark is drawn if the application asked
    /// for one, and what comes out is written in <see cref="Format"/>. A path is described rather
    /// than read, so a handler that returns a file it already had costs no decoding — unless there
    /// is a watermark, which is the one thing that needs the pixels.
    /// </para>
    /// <para>
    /// A path outside the cache is copied into it, so that what a handler returned can be fetched
    /// from the file endpoint.
    /// </para>
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        ImagePipeline.Postprocess(value, Shape, Watermark)?.ToJson();

    /// <summary>How this component wants its images, as the pipeline takes them.</summary>
    private ImageShape Shape => new(MediaFolder, Format, ImageMode, Type, Root);

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(JsonNode? value) =>
        value is JsonObject file
        && JsonValues.Text(file["path"]) is { Length: > 0 } path
        && !MediaCache.NamesALocation(path)
            ? [path]
            : [];
}

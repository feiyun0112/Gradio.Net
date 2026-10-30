using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Two images shown as one, with a slider between them: what the user drags across reveals one over
/// the other.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a pair rather than one image, which is the
/// whole of what this component is: <c>before</c> and <c>after</c>, as a list of exactly two
/// entries.
/// </para>
/// <para>
/// As with <see cref="Image"/>, each of the two is decoded and handed over in the shape
/// <see cref="Type"/> names: a pair of <see cref="RasterImage"/> values, or a pair of paths.
/// </para>
/// </remarks>
[GradioBlock("imageslider")]
[HandlerValue("string?[]", "change", "input", "upload", "stream", "select", "clear")]
public sealed partial class ImageSlider : Component
{
    internal ImageSlider()
    {
    }

    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The buttons a slider shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["download", "fullscreen"];

    private string _type = "numpy";
    private IReadOnlyList<object?>? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The pair of images the component starts with.</summary>
    /// <value>Two paths or URLs; read back as two file objects.</value>
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
            _value = value switch
            {
                null => null,
                IReadOnlyList<object?> pair => pair,
                JsonNode node => new[] { JsonValues.ToObject(node) },
                _ => throw new ArgumentException(
                    $"An image slider's value is a pair of images; got a {value.GetType()}.",
                    nameof(value)),
            };
            _described = null;
            _describedIn = null;
        }
    }

    /// <summary>The format an image is saved as when it is written to the cache.</summary>
    public string Format { get; set; } = "webp";

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The width of the component, in pixels or as a CSS length.</summary>
    public object? Width { get; set; }

    /// <summary>The pixel format the images are loaded as, or <see langword="null"/> to infer it.</summary>
    public string? ImageMode { get; set; } = "RGB";

    /// <summary>The format the value is converted to before a handler reads it.</summary>
    /// <value><c>numpy</c>, <c>pil</c> or <c>filepath</c>.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("numpy" or "pil" or "filepath"))
            {
                throw new ArgumentException(
                    $"An image slider's type must be one of numpy, pil or filepath; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Where the slider starts, as a percentage of the image's width.</summary>
    public double SliderPosition { get; set; } = 50;

    /// <summary>The tallest the image is drawn, in pixels.</summary>
    public int MaxHeight { get; set; } = 500;

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.SliderSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonArray(Example(), Example());

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <inheritdoc />
    /// <remarks>
    /// The slider declares the <c>stream</c> event, so the config carries <c>streamable</c> — a key
    /// about the component rather than about one of its parameters.
    /// </remarks>
    protected internal override void WriteEventConfig(PropWriter props)
    {
        base.WriteEventConfig(props);
        props.Add("streamable", false);
    }

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a pair of file objects.</param>
    /// <returns>The two cached paths, or <see langword="null"/> when the pair is incomplete.</returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonArray pair || pair.Count < 2)
        {
            return null;
        }

        return new[]
        {
            Read(pair[0]),
            Read(pair[1]),
        };
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A pair of paths, URLs, images, streams or file objects.</param>
    /// <returns>The pair as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        object?[]? images = value switch
        {
            null => null,
            string text => [text, text],
            FileData file => [file, file],
            EditorValue editor => [editor.Background, editor.Composite],
            JsonArray pair => [JsonValues.ToObject(pair.ElementAtOrDefault(0)), JsonValues.ToObject(pair.ElementAtOrDefault(1))],
            System.Collections.IEnumerable items and not string => [.. items.Cast<object?>()],
            _ => throw new NotSupportedException(
                "An image slider takes a pair of paths, URLs, images, streams, or file objects; it "
                + $"was handed a {value.GetType()}."),
        };

        if (images is null)
        {
            return null;
        }

        var described = new JsonArray();
        foreach (var image in images.Take(2))
        {
            described.Add(Describe(image));
        }

        return described;
    }

    /// <summary>One image as a file object, in the application's cache.</summary>
    /// <param name="value">A path, a URL, an image, its bytes, a stream, or a file object.</param>
    /// <remarks>
    /// An image a handler made is written in the slider's format; a path it named is copied rather
    /// than decoded, which is what the Python version's <c>save_image</c> does with one.
    /// </remarks>
    private FileData? Image(object? value) => value switch
    {
        RasterImage image => ImagePipeline.Postprocess(image, Shape),
        _ => MediaFiles.From(value, MediaFolder, $"image.{Format}", "An image slider"),
    };

    /// <summary>One image described for the client.</summary>
    private JsonObject? Describe(object? value) =>
        Image(value) is { Path.Length: > 0 } file
            ? MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson()
            : null;

    /// <summary>One image of an incoming pair, as the cached path a handler reads.</summary>
    private object? Read(JsonNode? value) => ImagePipeline.Preprocess(FileData.FromJson(value), Shape);

    /// <summary>How this component wants its images, as the pipeline takes them.</summary>
    private ImageShape Shape => new(MediaFolder, Format, ImageMode, Type, Root);

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(object? value) => value switch
    {
        JsonArray pair => pair.SelectMany(Paths),
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        _ => [],
    };

    /// <summary>The sample file object the Python version's examples are written as.</summary>
    private static JsonObject Example() => new JsonObject
    {
        ["path"] = SampleImage,
        ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        ["orig_name"] = "bus.png",
        ["url"] = SampleImage,
    };
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The value an image editor hands a handler and receives from one: a background, the layers drawn
/// over it, and the composite the user sees.
/// </summary>
/// <remarks>
/// The Python version's <c>EditorValue</c> typed dictionary. Each entry is an image in whatever
/// shape the component's <see cref="ImageEditor.Type"/> names, which is why they are
/// <see cref="object"/>: a <see cref="RasterImage"/> for the decoded shapes and a path for
/// <c>filepath</c>.
/// </remarks>
public sealed class EditorValue
{
    internal EditorValue()
    {
    }

    /// <summary>The image being edited, or <see langword="null"/> for a blank canvas.</summary>
    public object? Background { get; set; }

    /// <summary>The layers drawn over the background, bottom first.</summary>
    public IReadOnlyList<object?> Layers { get; set; } = [];

    /// <summary>The image as the user sees it: the background with the layers applied.</summary>
    public object? Composite { get; set; }
}

/// <summary>
/// An image component with editing tools: brushes, an eraser, layers, cropping and a webcam
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Three of them are unions in the Python version's
/// signature and therefore take an <see cref="object"/> here: the eraser and the brush are either
/// options or <see langword="false"/> to hide the tool, and the layers are either options or a
/// boolean for the same reason.
/// </para>
/// <para>
/// What this component does with an image is what <see cref="Image"/> does: each of the three — the
/// background, the layers, the composite — is decoded and handed over in the shape
/// <see cref="Type"/> names, and written in <see cref="Format"/> on the way back.
/// </para>
/// </remarks>
[GradioBlock("imageeditor")]
[HandlerValue("EditorValue", "change", "input", "upload", "select", "clear", "apply")]
public partial class ImageEditor : Component
{
    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The sources an image may be taken from.</summary>
    private static readonly IReadOnlyList<string> _allSources = ["upload", "webcam", "clipboard"];

    private string _type = "numpy";
    private IReadOnlyList<string> _sources = _allSources;
    private object? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The image the editor starts with.</summary>
    /// <value>
    /// An <see cref="EditorValue"/>, or a single image used as both the background and the
    /// composite; read back as the file objects the config carries.
    /// </value>
    /// <remarks>
    /// Postprocessed on read for the same reason as an image's value: copying the files into the
    /// application's cache needs the application, which is known only after the component has been
    /// rendered into one.
    /// </remarks>
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

    /// <summary>The height of the editor, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The width of the editor, in pixels or as a CSS length.</summary>
    public object? Width { get; set; }

    /// <summary>The pixel format the images are converted to.</summary>
    /// <value><c>RGBA</c> unless the application says otherwise.</value>
    public string ImageMode { get; set; } = "RGBA";

    /// <summary>Where a background image may come from.</summary>
    [FactoryArgument(FactoryArgumentKind.Names)]
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
                        $"An image editor's sources must be a list of {string.Join(", ", _allSources)}; "
                        + $"got '{source}'.",
                        nameof(value));
                }
            }

            _sources = value;
        }
    }

    /// <summary>The format the images are converted to before a handler reads them.</summary>
    /// <value><c>numpy</c>, <c>pil</c> or <c>filepath</c>.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("numpy" or "pil" or "filepath"))
            {
                throw new ArgumentException(
                    $"An image editor's type must be one of numpy, pil or filepath; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The buttons drawn in the component's corner, or <see langword="null"/> for all of them.</summary>
    public IReadOnlyList<string>? Buttons { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The text shown where an image would be dropped.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The transforms the user may apply.</summary>
    /// <value><c>crop</c> and <c>resize</c> unless the application says otherwise.</value>
    public IReadOnlyList<string>? Transforms { get; set; } = ["crop", "resize"];

    /// <summary>The eraser tool's options, or <see langword="false"/> to hide it.</summary>
    /// <value>The tool's defaults unless the application says otherwise.</value>
    public object? Eraser { get; set; } = new EraserOptions();

    /// <summary>The brush tool's options, or <see langword="false"/> to hide it — and the eraser with it.</summary>
    /// <value>The tool's defaults unless the application says otherwise.</value>
    public object? Brush { get; set; } = new BrushOptions();

    /// <summary>The format an image is saved as when it is written to the cache.</summary>
    public string Format { get; set; } = "webp";

    /// <summary>The layer tool's options, or <see langword="false"/> to hide it.</summary>
    /// <value>An additional layer allowed, and one layer to start with.</value>
    public object Layers { get; set; } = new LayerOptions();

    /// <summary>The size of the canvas, as width and height in pixels.</summary>
    public IReadOnlyList<int> CanvasSize { get; set; } = [800, 800];

    /// <summary>Whether an uploaded image is scaled to the canvas rather than the canvas to it.</summary>
    public bool FixedCanvas { get; set; }

    /// <summary>The options the webcam is opened with.</summary>
    public WebcamOptions WebcamOptions { get; set; } = new();

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.EditorSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["background"] = new JsonObject
        {
            ["path"] = SampleImage,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "bus.png",
            ["url"] = SampleImage,
        },
        ["layers"] = new JsonArray(),
        ["composite"] = null,
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles =>
        Paths(Value is JsonNode described ? described : null);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: an editor value, or a single file object.</param>
    /// <returns>
    /// The background, layers and composite as cached paths — the shape the Python version's
    /// <c>filepath</c> type produces.
    /// </returns>
    public override object? Preprocess(JsonNode? value)
    {
        // A value that names a blob the browser uploaded out of band is read from that blob rather
        // than from the file objects, which for such a value are not there.
        if (value is JsonObject editorValue && JsonValues.Text(editorValue["id"]) is { Length: > 0 } id
            && Claim(id) is { } blobs)
        {
            return new EditorValue
            {
                Background = Save(blobs.Background),
                Layers = [.. blobs.Layers.Select(Save).OfType<string>()],
                Composite = Save(blobs.Composite),
            };
        }

        if (value is not JsonObject editor)
        {
            var single = FileData.FromJson(value)?.Path is { Length: > 0 } path
                ? MediaFiles.Cached(path, MediaFolder, Root)
                : null;

            return single is null ? null : new EditorValue { Background = single, Composite = single };
        }

        var layers = new List<object?>();
        if (editor["layers"] is JsonArray list)
        {
            foreach (var layer in list)
            {
                if (Read(layer) is { } drawn)
                {
                    layers.Add(drawn);
                }
            }
        }

        return new EditorValue
        {
            Background = Read(editor["background"]),
            Layers = layers,
            Composite = Read(editor["composite"]),
        };
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">An <see cref="EditorValue"/>, a single image, or a file object.</param>
    /// <returns>The three images as the protocol carries them, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsonNode node)
        {
            // A value the application set, which is the editor's own shape or a single image.
            return node is JsonObject editor && (editor["background"] is not null || editor["layers"] is not null)
                ? Describe(editor)
                : Single(node);
        }

        return value is EditorValue composite ? Describe(composite) : Single(value);
    }

    /// <summary>The three images of an editor value, described as the protocol carries them.</summary>
    private JsonObject Describe(JsonObject editor) => new JsonObject
    {
        ["background"] = File(FileData.FromJson(editor["background"])),
        ["layers"] = LayerList(editor["layers"]),
        ["composite"] = File(FileData.FromJson(editor["composite"])),
        ["id"] = null,
    };

    /// <summary>The three images of an editor value a handler returned.</summary>
    private JsonObject Describe(EditorValue editor) => new()
    {
        ["background"] = File(Image(editor.Background)),
        ["layers"] = LayerList(editor.Layers),
        ["composite"] = File(Image(editor.Composite)),
        ["id"] = null,
    };

    /// <summary>A single image, used as both the background and the composite.</summary>
    private JsonObject? Single(object value)
    {
        var file = Image(value);
        if (file is null)
        {
            return null;
        }

        return new JsonObject
        {
            ["background"] = File(file),
            ["layers"] = new JsonArray(),
            ["composite"] = File(file),
            ["id"] = null,
        };
    }

    /// <summary>The layers, each described.</summary>
    private JsonArray LayerList(IEnumerable<object?> layers)
    {
        var described = new JsonArray();

        foreach (var layer in layers)
        {
            described.Add(File(Image(layer)));
        }

        return described;
    }

    /// <summary>The layers of a value the application set, each described.</summary>
    private JsonArray LayerList(JsonNode? layers)
    {
        var described = new JsonArray();

        if (layers is JsonArray list)
        {
            foreach (var layer in list)
            {
                described.Add(File(FileData.FromJson(layer)));
            }
        }

        return described;
    }

    /// <summary>One image as a file object, in the application's cache.</summary>
    /// <param name="value">A path, a URL, an image, its bytes, a stream, or a file object.</param>
    /// <remarks>
    /// An image a handler made — including the ones this component handed it — is written in the
    /// editor's format; a path it named is copied rather than decoded.
    /// </remarks>
    private FileData? Image(object? value) => value switch
    {
        RasterImage image => ImagePipeline.Postprocess(image, Shape),
        _ => MediaFiles.From(value, MediaFolder, $"image.{Format}", "An image editor"),
    };

    /// <summary>One image described for the client, or <see langword="null"/> for none.</summary>
    private JsonObject? File(FileData? file) =>
        file?.Path is { Length: > 0 }
            ? MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson()
            : null;

    /// <summary>The bytes of a blob, written into the cache, or <see langword="null"/> for none.</summary>
    private object? Save(byte[]? bytes) =>
        bytes is null
            ? null
            : ImagePipeline.Preprocess(
                new FileData
                {
                    Path = MediaCache.SaveBytes(bytes, $"image.{Format}", MediaFolder),

                    // The name is what the pipeline reads the format from: without one it writes
                    // webp whatever the component's format says.
                    OrigName = $"image.{Format}",
                },
                Shape);

    /// <summary>One image of an incoming value, as the value a handler reads.</summary>
    private object? Read(JsonNode? value) => ImagePipeline.Preprocess(FileData.FromJson(value), Shape);

    /// <summary>How this component wants its images, as the pipeline takes them.</summary>
    private ImageShape Shape => new(MediaFolder, Format, ImageMode, Type, Root);

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(JsonNode? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        JsonObject editor => EditorPaths(editor),
        _ => [],
    };

    /// <summary>The paths of an editor value's three images.</summary>
    private static IEnumerable<string> EditorPaths(JsonObject editor)
    {
        foreach (var key in (string[])["background", "composite"])
        {
            if (editor[key] is JsonObject file
                && JsonValues.Text(file["path"]) is { Length: > 0 } path
                && !MediaCache.NamesALocation(path))
            {
                yield return path;
            }
        }

        if (editor["layers"] is JsonArray layers)
        {
            foreach (var layer in layers)
            {
                if (layer is JsonObject file
                    && JsonValues.Text(file["path"]) is { Length: > 0 } path
                    && !MediaCache.NamesALocation(path))
                {
                    yield return path;
                }
            }
        }
    }
}

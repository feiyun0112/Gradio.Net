using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// One annotation: the mask it is drawn from and the label it is drawn in.
/// </summary>
/// <param name="Mask">
/// A box — four coordinates as a tuple or a list — or a picture: a path, a URL, the image's bytes,
/// a stream, or a file object.
/// </param>
/// <param name="Label">The label, which is also the legend's text.</param>
public sealed record Annotation(object? Mask, string Label);

/// <summary>
/// A base image with labelled masks drawn over it: what an object detector or a segmentation model
/// returns, shown as one picture with a legend.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order.
/// </para>
/// <para>
/// The value is a pair — a base image and the annotations drawn on it — and each annotation is a
/// mask and the label it is drawn in. The Python version's mask is either a bounding box or a numpy
/// array of confidences; here a box is a four-tuple of coordinates and a mask that is not a box is
/// an image the application produced. The component rasterizes a box into a picture before the
/// value leaves the server, which is what the frontend is given: it draws images, not coordinates.
/// </para>
/// <para>
/// Rasterizing a box needs the base image's dimensions, so a base image that names a location the
/// server does not hold cannot carry one — its size is not known until something fetches it, which
/// is the file endpoint's subject (ticket 39). A mask that is already a picture is unaffected.
/// </para>
/// </remarks>
[GradioBlock("annotatedimage")]
[HandlerValue("object?", "change", "select")]
public sealed partial class AnnotatedImage : Component
{
    internal AnnotatedImage()
    {
    }

    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The buttons an annotated image shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["fullscreen"];

    private object? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The base image and the annotations the component starts with.</summary>
    /// <value>
    /// A pair of a base image and a sequence of <see cref="Annotation"/>s — or of <c>(mask,
    /// label)</c> pairs, which is how the Python version spells one. Read back as the value the
    /// protocol carries: a file object for the base image and one for each mask.
    /// </value>
    /// <remarks>
    /// Postprocessed on read for the same reason as a file's: rasterizing a box writes a picture
    /// into the application's cache, and the cache folder is known only after the component has
    /// been rendered into an application. The described value is remembered per cache folder, since
    /// a read before the render writes into the process default.
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

    /// <summary>The format a base image is saved as when the value is bytes rather than a path.</summary>
    /// <value><c>webp</c> unless the application says otherwise.</value>
    public string Format { get; set; } = "webp";

    /// <summary>Whether the annotations are listed beside the image.</summary>
    public bool ShowLegend { get; set; } = true;

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The width of the component, in pixels or as a CSS length.</summary>
    public object? Width { get; set; }

    /// <summary>The colour each label is drawn in.</summary>
    /// <value>Hex codes by label, or <see langword="null"/> to draw every label in red.</value>
    public IReadOnlyDictionary<string, string>? ColorMap { get; set; }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <remarks>
    /// Names only, for now: a custom <c>Button</c> is a block, and the button family is its own
    /// batch (ticket 15). The Python version's default is the fullscreen button.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.AnnotatedImageSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["image"] = new JsonObject
        {
            ["path"] = SampleImage,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "bus.png",
            ["url"] = SampleImage,
        },
        ["annotations"] = new JsonArray(),
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: the base image and its annotations.</param>
    /// <returns>
    /// The base image's path and the annotations as <c>(mask path, label)</c> pairs, or
    /// <see langword="null"/> when there is no value.
    /// </returns>
    /// <remarks>
    /// The paths are the ones the client sent, not copies: the Python version's preprocess for this
    /// component reads the payload's paths and does not move the files into the cache, unlike the
    /// components whose value the user uploads.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonObject payload)
        {
            return null;
        }

        var baseImage = FileData.FromJson(payload["image"]);
        if (baseImage?.Path is not { Length: > 0 } path)
        {
            return null;
        }

        var annotations = new List<Annotation>();

        if (payload["annotations"] is JsonArray sections)
        {
            foreach (var section in sections)
            {
                var mask = FileData.FromJson(section?["image"]);
                if (mask?.Path is { Length: > 0 } maskPath)
                {
                    annotations.Add(new Annotation(maskPath, TextOf(section?["label"])));
                }
            }
        }

        return (path, (IReadOnlyList<Annotation>)annotations);
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A pair of a base image and its annotations.</param>
    /// <returns>The base image and the rasterized masks as the protocol carries them.</returns>
    /// <exception cref="ArgumentException">The value is not such a pair.</exception>
    /// <exception cref="NotSupportedException">
    /// A mask is a box and the base image's size is not known, or the value is of a shape this
    /// component does not take.
    /// </exception>
    public override JsonNode? Postprocess(object? value)
    {
        if (PairOf(value) is not { } pair)
        {
            return null;
        }

        var (baseImage, annotations) = pair;

        var baseFile = MediaFiles.From(baseImage, MediaFolder, $"image.{Format}", "An annotated image's base image")
            ?? throw new ArgumentException(
                "An annotated image needs a base image; the pair it was handed has none.",
                nameof(value));

        var size = SizeOf(baseFile);
        var sections = new JsonArray();

        foreach (var annotation in AnnotationsOf(annotations))
        {
            var mask = MaskFile(annotation.Mask, annotation.Label, size);
            var described = MediaFiles.Describe(mask, MediaFolder, Root, measure: false);

            sections.Add(new JsonObject
            {
                ["image"] = described.ToJson(),
                ["label"] = annotation.Label,
            });
        }

        return new JsonObject
        {
            ["image"] = MediaFiles.Describe(baseFile, MediaFolder, Root, measure: false).ToJson(),
            ["annotations"] = sections,
        };
    }

    /// <summary>The file an annotation's mask is drawn into, or the picture it already is.</summary>
    /// <param name="mask">A box, a path, a URL, bytes, a stream, or a file object.</param>
    /// <param name="label">The label the annotation is drawn in, which picks the colour.</param>
    /// <param name="size">The base image's size, when it is known.</param>
    /// <returns>The mask as a file.</returns>
    /// <exception cref="NotSupportedException">A box was given but the base image's size is not known.</exception>
    /// <remarks>
    /// The Python version paints the box into an array the size of the base image — the interior
    /// half opaque, a three-pixel border fully opaque, in the label's colour — and saves it as a
    /// PNG. That is what <see cref="ImageRaster.MaskPng"/> draws, so the picture the frontend
    /// receives is the same one.
    /// </remarks>
    private FileData MaskFile(object? mask, string label, (int Width, int Height)? size)
    {
        if (BoxOf(mask) is not { } box)
        {
            return MediaFiles.From(mask, MediaFolder, "mask.png", "An annotation's mask")
                ?? throw new ArgumentException(
                    "An annotation needs a mask; one of them has none.",
                    nameof(mask));
        }

        if (size is not { } dimensions)
        {
            throw new NotSupportedException(
                "An annotated image cannot draw a box over a base image whose size it cannot read: "
                + "the image names a location the server does not hold, and fetching it is the file "
                + "endpoint's subject (ticket 39). Give the base image's bytes or a local path, or "
                + "draw the mask as a picture instead.");
        }

        var color = ImageRaster.ColorOf(ColorMapOf(label));

        return new FileData
        {
            Path = MediaCache.SaveBytes(
                ImageRaster.MaskPng(
                    dimensions.Width,
                    dimensions.Height,
                    (box.Left, box.Top, box.Right, box.Bottom),
                    color),
                "mask.png",
                MediaFolder),
        };
    }

    /// <summary>The colour a label is drawn in.</summary>
    /// <param name="label">The annotation's label.</param>
    /// <returns>The hex code, or <see langword="null"/> when the map names none.</returns>
    private string? ColorMapOf(string label) =>
        ColorMap is not null && ColorMap.TryGetValue(label, out var hex) ? hex : null;

    /// <summary>The base image's size, or <see langword="null"/> when this cannot read it.</summary>
    /// <param name="file">The base image.</param>
    /// <remarks>
    /// A file the server holds is read; a location is not, since fetching it is ticket 39's
    /// subject. Only PNG and JPEG declare their size in a header this can read.
    /// </remarks>
    private static (int Width, int Height)? SizeOf(FileData file)
    {
        if (file.Path is not { Length: > 0 } path
            || MediaCache.NamesALocation(path)
            || !System.IO.File.Exists(path))
        {
            return null;
        }

        return ImageRaster.SizeOf(System.IO.File.ReadAllBytes(path));
    }

    /// <summary>The pair a value is, or <see langword="null"/> for no value.</summary>
    /// <param name="value">What a handler returned.</param>
    private static (object? Image, object? Annotations)? PairOf(object? value) => value switch
    {
        null => null,
        ITuple pair when pair.Length == 2 => (pair[0], pair[1]),
        IReadOnlyList<object?> pair when pair.Count == 2 => (pair[0], pair[1]),
        _ => throw new ArgumentException(
            "An annotated image's value is a pair of a base image and its annotations; it was "
            + $"handed a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>The annotations a value's second entry names.</summary>
    /// <param name="annotations">The entry.</param>
    private static IEnumerable<Annotation> AnnotationsOf(object? annotations) => annotations switch
    {
        null => [],
        IEnumerable<Annotation> typed => typed,
        IEnumerable<object?> items => items.Select(AnnotationOf),
        _ => throw new ArgumentException(
            "An annotated image's annotations are a sequence of (mask, label) pairs; it was handed "
            + $"a {annotations.GetType()}.",
            nameof(annotations)),
    };

    /// <summary>One annotation, however it was spelled.</summary>
    /// <param name="item">An <see cref="Annotation"/>, or a pair of a mask and a label.</param>
    private static Annotation AnnotationOf(object? item) => item switch
    {
        Annotation annotation => annotation,
        ITuple pair when pair.Length == 2 => new Annotation(pair[0], TextOf(pair[1])),
        IReadOnlyList<object?> pair when pair.Count == 2 => new Annotation(pair[0], TextOf(pair[1])),
        _ => throw new ArgumentException(
            "An annotation is a mask and the label it is drawn in; one of them is neither.",
            nameof(item)),
    };

    /// <summary>The box a mask is, or <see langword="null"/> when the mask is a picture.</summary>
    /// <param name="mask">The mask.</param>
    /// <remarks>
    /// The Python version tells the two apart by type: a tuple of four integers is a box, and
    /// anything else is an array of confidences. Here a four-tuple is a box and anything else is a
    /// picture, since an array of confidences is not a shape this framework's handlers produce.
    /// </remarks>
    private static (int Left, int Top, int Right, int Bottom)? BoxOf(object? mask) => mask switch
    {
        ITuple box when box.Length == 4 =>
            (Integer(box[0]), Integer(box[1]), Integer(box[2]), Integer(box[3])),
        IReadOnlyList<int> box when box.Count == 4 => (box[0], box[1], box[2], box[3]),
        IReadOnlyList<object?> box when box.Count == 4 =>
            (Integer(box[0]), Integer(box[1]), Integer(box[2]), Integer(box[3])),
        _ => null,
    };

    /// <summary>A coordinate.</summary>
    /// <param name="value">The number, as a number or as the text of one.</param>
    private static int Integer(object? value) => value switch
    {
        int number => number,
        null => throw new ArgumentException("A box's coordinates are four integers; one is missing."),
        IConvertible convertible => convertible.ToInt32(CultureInfo.InvariantCulture),
        _ => throw new ArgumentException(
            $"A box's coordinates are four integers; one is a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>A label, however it was spelled.</summary>
    /// <param name="value">The label.</param>
    private static string TextOf(object? value) => value switch
    {
        null => string.Empty,
        string text => text,
        JsonNode node => JsonValues.Text(node) ?? string.Empty,
        IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
        _ => value.ToString() ?? string.Empty,
    };

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    /// <param name="value">The described value.</param>
    private static List<string> Paths(object? value)
    {
        if (value is not JsonObject data)
        {
            return [];
        }

        var paths = new List<string>();

        foreach (var file in new[] { data["image"] }.Concat(
                     data["annotations"] is JsonArray sections
                         ? sections.Select(section => section?["image"])
                         : []))
        {
            if (file is JsonObject entry
                && JsonValues.Text(entry["path"]) is { Length: > 0 } path
                && !MediaCache.NamesALocation(path))
            {
                paths.Add(path);
            }
        }

        return paths;
    }
}

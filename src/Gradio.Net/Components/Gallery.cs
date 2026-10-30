using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// One entry of a gallery: an image or a video, and the caption shown under it.
/// </summary>
/// <remarks>
/// The Python version's <c>(media, caption)</c> tuple. It is a record here rather than a value
/// tuple so that both directions of the component's value can name it — a handler returning a
/// gallery and a handler reading one speak about the same thing.
/// </remarks>
/// <param name="Media">
/// A path, a URL, the media's bytes, a file object — or, in the direction a handler reads, a
/// <see cref="RasterImage"/> for an image the gallery was asked to decode.
/// </param>
/// <param name="Caption">The caption, or <see langword="null"/> for none.</param>
public sealed record GalleryEntry(object? Media, string? Caption = null);

/// <summary>
/// A grid of images or videos, optionally captioned, that a user can look through and upload into
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a list rather than one file, and an empty
/// list is what a gallery that was never given one carries — which is the Python version's own
/// answer, and the reason <c>value</c> is always present in its props.
/// </para>
/// <para>
/// Whether an entry is an image or a video is decided from its name, which is what the Python
/// version's mime guess comes down to for the formats a gallery accepts.
/// </para>
/// </remarks>
[GradioBlock("gallery")]
[HandlerValue("IReadOnlyList<GalleryEntry>", "select", "upload", "change", "delete", "preview_open", "preview_close")]
public sealed partial class Gallery : Component
{
    internal Gallery()
    {
    }

    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>The buttons a gallery shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons =
        ["share", "download", "download_all", "fullscreen"];

    /// <summary>The sources a gallery accepts when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultSources = ["upload"];

    /// <summary>The extensions a gallery shows as a video rather than as an image.</summary>
    private static readonly string[] _videoExtensions =
        [".mp4", ".webm", ".ogg", ".ogv", ".mov", ".avi", ".mkv", ".m4v"];

    private string _type = "filepath";
    private IReadOnlyList<object?>? _value;

    /// <summary>The images or videos the gallery starts with.</summary>
    /// <value>
    /// A list of paths, URLs, <see cref="GalleryEntry"/> values, or file objects. An empty list is
    /// what a gallery that was never given a value carries, which is the Python version's own
    /// answer.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value switch
        {
            null => null,
            JsonNode node => [JsonValues.ToObject(node)],
            System.Collections.IEnumerable items and not string => [.. items.Cast<object?>()],
            _ => throw new ArgumentException(
                $"A gallery's value is a list of images or videos; got a {value.GetType()}.",
                nameof(value)),
        };
    }

    /// <summary>The format an image is saved as when it is written to the cache.</summary>
    public string Format { get; set; } = "webp";

    /// <summary>The extensions or kinds of file that may be uploaded, or <see langword="null"/> for any.</summary>
    /// <remarks>
    /// The Python version filters by this on the client and checks it again when a file arrives.
    /// The check is not here yet: refusing one is a user-facing error, and the error type that
    /// carries one belongs to the batch that builds the error hierarchy (ticket 38).
    /// </remarks>
    public IReadOnlyList<string>? FileTypes { get; set; }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The number of entries in one row.</summary>
    public int? Columns { get; set; } = 2;

    /// <summary>The number of rows in the grid, or <see langword="null"/> to fit as many as there are.</summary>
    public int? Rows { get; set; }

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>Whether clicking an entry opens it enlarged.</summary>
    public bool AllowPreview { get; set; } = true;

    /// <summary>Whether the gallery opens in preview mode, or <see langword="null"/> to leave it closed.</summary>
    public bool? Preview { get; set; }

    /// <summary>The entry selected when the page loads, or <see langword="null"/> for none.</summary>
    public int? SelectedIndex { get; set; }

    /// <summary>How an entry's thumbnail is fitted into its box.</summary>
    /// <value><c>contain</c>, <c>cover</c>, <c>fill</c>, <c>none</c>, <c>scale-down</c>, or <see langword="null"/> for the frontend's default.</value>
    public string? ObjectFit { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

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
                    $"A gallery's type must be one of numpy, pil or filepath; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>Whether the columns expand to fill the width when there are fewer entries than columns.</summary>
    public bool FitColumns { get; set; } = true;

    /// <summary>Where an image may come from when the gallery is an input.</summary>
    public IReadOnlyList<string> Sources { get; set; } = _defaultSources;

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.GallerySchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonArray(new JsonObject
    {
        ["image"] = new JsonObject
        {
            ["path"] = SampleImage,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "bus.png",
            ["url"] = SampleImage,
        },
    });

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value as JsonNode);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a list of entries, each an image or a video.</param>
    /// <returns>
    /// One <see cref="GalleryEntry"/> per item, with the media in the shape <see cref="Type"/> names,
    /// or <see langword="null"/> when the gallery is empty.
    /// </returns>
    /// <remarks>
    /// A video is always a path, whatever <see cref="Type"/> says — the Python version says so, and
    /// there is no decoded video shape — while an image is a <see cref="RasterImage"/> or a path
    /// like any other.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonArray items || items.Count == 0)
        {
            return null;
        }

        var entries = new List<GalleryEntry>();

        foreach (var item in items)
        {
            if (item is not JsonObject entry)
            {
                continue;
            }

            var video = FileData.FromJson(entry["video"]);
            var image = FileData.FromJson(entry["image"]);

            // A video is always a path — the Python version says so, and there is no decoded video shape —
            // while an image is whatever the gallery's type asks for.
            var media = video?.Path is { Length: > 0 } path
                ? (MediaCache.NamesALocation(path) ? path : MediaFiles.Cached(path, MediaFolder, Root))
                : ImagePipeline.Preprocess(image, Shape);

            if (media is null)
            {
                continue;
            }

            entries.Add(new GalleryEntry(media, JsonValues.Text(entry["caption"])));
        }

        return entries;
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A list of media, of <see cref="GalleryEntry"/> values, or of file objects.</param>
    /// <returns>The entries as the protocol carries them; an empty list for a gallery with no value.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        var entries = new JsonArray();

        if (value is null)
        {
            return entries;
        }

        if (value is string)
        {
            throw new NotSupportedException(
                "The value of a gallery must be a list of images or videos, or a list of "
                + "(media, caption) entries.");
        }

        foreach (var item in Items(value))
        {
            var (media, caption) = Split(item);
            var file = Media(media);

            if (file?.Path is not { Length: > 0 })
            {
                continue;
            }

            var described = MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson();
            var key = IsVideo(file.Path) ? "video" : "image";

            entries.Add(new JsonObject
            {
                [key] = described,
                ["caption"] = caption,
            });
        }

        return entries;
    }

    /// <summary>The entries of a value, whatever shape the list came in.</summary>
    private static IEnumerable<object?> Items(object value) => value switch
    {
        JsonArray array => array.Select(JsonValues.ToObject),
        System.Collections.IEnumerable items and not string => items.Cast<object?>(),
        _ => [value],
    };

    /// <summary>One entry, split into its media and its caption.</summary>
    private static (object? Media, string? Caption) Split(object? item) => item switch
    {
        GalleryEntry entry => (entry.Media, entry.Caption),
        object?[] pair when pair.Length == 2 => (pair[0], pair[1] as string),
        JsonArray pair when pair.Count == 2 => (JsonValues.ToObject(pair[0]), JsonValues.Text(pair[1])),
        _ => (item, null),
    };

    /// <summary>One entry's media as a file object, in the application's cache.</summary>
    /// <param name="value">A path, a URL, an image, its bytes, a stream, or a file object.</param>
    /// <returns>The file the browser is given, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// An image a handler made is written in the gallery's format, which is the one thing a gallery
    /// does to an entry it was handed; everything else is a file it was given, and a file is copied
    /// rather than decoded.
    /// </remarks>
    private FileData? Media(object? value) => value switch
    {
        RasterImage image => ImagePipeline.Postprocess(image, Shape),
        _ => MediaFiles.From(value, MediaFolder, $"image.{Format}", "A gallery entry"),
    };

    /// <summary>How this component wants its images, as the pipeline takes them.</summary>
    private ImageShape Shape => new(MediaFolder, Format, Mode: null, Type, Root);

    /// <summary>Whether a file is a video, from its name.</summary>
    private static bool IsVideo(string path) =>
        _videoExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(JsonNode? value) => value switch
    {
        JsonArray entries => entries.SelectMany(Paths),
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        JsonObject entry => Paths(entry["image"]).Concat(Paths(entry["video"])),
        _ => [],
    };
}

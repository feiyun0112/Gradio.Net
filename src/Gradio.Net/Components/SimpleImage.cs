using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// An image with nothing but the essentials: a value, a label and whether it can be downloaded
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. What it leaves out is everything the full image offers
/// on top — the sources it can be taken from, the editor, the streaming, the watermark — which is
/// the whole reason the Python version has two of them.
/// </para>
/// <para>
/// Its value is a file rather than a decoded picture: the Python version's <c>data_model</c> is
/// <c>FileData</c>, so what a handler is handed is the path of the cached copy, and what it answers
/// with is described the way every file is (see <see cref="MediaFiles"/>).
/// </para>
/// </remarks>
[GradioBlock("simpleimage")]
[HandlerValue("string", "clear", "change", "upload")]
public sealed partial class SimpleImage : Component
{
    internal SimpleImage()
    {
    }

    private JsonNode? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The image the component starts with.</summary>
    /// <value>A path or a URL, as the application wrote it; read back as the file object the config carries.</value>
    /// <remarks>
    /// Postprocessed on read for the same reason as a file's: the copy into the application's cache
    /// needs the application, which is known only after the component has been rendered into one.
    /// The described value is remembered per cache folder, so that a read before the render is not
    /// mistaken for the one that matters.
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

    /// <summary>Whether the component offers to download the image.</summary>
    public bool ShowDownloadButton { get; set; } = true;

    /// <summary>The label drawn above the image.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the image next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the image wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create(SampleImage);

    /// <summary>The image's path that a handler reads.</summary>
    /// <param name="value">The file object the client sent.</param>
    /// <returns>The cached path, or <see langword="null"/> when nothing was sent.</returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (FileData.FromJson(value)?.Path is not { Length: > 0 } path)
        {
            return null;
        }

        // A file this server does not hold is handed over as the address it is: there are no bytes
        // here to read, and downloading it is the file endpoint's subject.
        return MediaCache.NamesALocation(path) ? path : MediaFiles.Cached(path, MediaFolder, Root);
    }

    /// <summary>The file object the client receives from what a handler returned.</summary>
    /// <param name="value">A path, a URL, or a file object.</param>
    /// <returns>The file as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        JsonNode node => Describe(FileData.FromJson(node)),
        string path => Describe(new FileData { Path = path }),
        FileData file => Describe(file),
        _ => throw new ArgumentException(
            $"A simple image's value is a path, a URL or a file object; got {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>The sample image the Python version's examples point at.</summary>
    private const string SampleImage =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";

    /// <summary>One file, described as the protocol carries it.</summary>
    /// <param name="file">The file, or <see langword="null"/> for none.</param>
    /// <returns>The file object, or <see langword="null"/> when there is no path.</returns>
    private JsonObject? Describe(FileData? file) =>
        file?.Path is { Length: > 0 }
            ? MediaFiles.Describe(file, MediaFolder, Root, measure: true).ToJson()
            : null;
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A file input or output: one file, several, or a whole directory.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. A member left at its default is written only when the
/// Python version writes it: the ones the Python version defaults to a value — the file count, the
/// type, the minimum width, visibility, whether files may be reordered — always appear, and the
/// optional ones appear when the application sets them.
/// </para>
/// <para>
/// <see cref="Value"/> is the file or files the component starts with, and what it reads back is
/// the value as the config carries it: a file object, or a list of them for a component that takes
/// more than one. That is the Python version's shape too — its <c>value</c> is the postprocessed
/// file dictionary — and it is why a default value is copied into the cache when the component is
/// first described rather than when it is written.
/// </para>
/// </remarks>
[GradioBlock("file")]
[HandlerValue("string", "change", "upload", "clear", "select", "delete", "download")]
public partial class File : Component
{
    internal File()
    {
    }

    /// <summary>The sample file the Python version's examples point at.</summary>
    private const string SampleFile =
        "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";

    private string _fileCount = "single";
    private string _type = "filepath";
    private IReadOnlyList<string>? _fileTypes;
    private JsonNode? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The file or files the component starts with.</summary>
    /// <value>
    /// A path, a URL, or a list of them, as the application wrote it; read back as the postprocessed
    /// value — a file object, or a list of them when the component takes more than one.
    /// </value>
    /// <remarks>
    /// <para>
    /// The value is postprocessed when it is read rather than when it is set because a file has to
    /// be copied into the application's cache, and the application is known only once the component
    /// has been rendered into one.
    /// </para>
    /// <para>
    /// The described value is remembered per cache folder rather than once: a read before the
    /// component was rendered copies the file into the process default, and that copy is not one
    /// the application's file endpoint serves. Keying the memory on the folder is what makes the
    /// second read — after the render — describe the file again into the folder that matters.
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

    /// <summary>Whether the component takes one file, several, or a directory.</summary>
    /// <value>One of <c>single</c>, <c>multiple</c> or <c>directory</c>.</value>
    public string FileCount
    {
        get => _fileCount;
        set
        {
            if (value is not ("single" or "multiple" or "directory"))
            {
                throw new ArgumentException(
                    $"A file component's file_count must be one of single, multiple or directory; got '{value}'.",
                    nameof(value));
            }

            _fileCount = value;
        }
    }

    /// <summary>The extensions or kinds of file that may be uploaded, or <see langword="null"/> for any.</summary>
    /// <remarks>
    /// The Python version filters by this on the client and checks it again when a file arrives.
    /// The check is not here yet: refusing one is a user-facing error, and the error type that
    /// carries one belongs to the batch that builds the error hierarchy (ticket 38).
    /// </remarks>
    public IReadOnlyList<string>? FileTypes
    {
        get => _fileTypes;
        set => _fileTypes = value;
    }

    /// <summary>What the handler is given: a path, or the file's bytes.</summary>
    /// <value><c>filepath</c> or <c>binary</c>.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("filepath" or "binary"))
            {
                throw new ArgumentException(
                    $"A file component's type must be filepath or binary; got '{value}'.",
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

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    /// <remarks>
    /// A number or a string, which is the Python version's own union: the frontend reads either.
    /// </remarks>
    public object? Height { get; set; }

    /// <summary>Whether uploaded files may be reordered by dragging.</summary>
    public bool AllowReordering { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <remarks>
    /// Names only, for now: a custom <c>Button</c> is a block, and the button family is its own
    /// batch (ticket 15). The Python version's default for a file is no buttons at all.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo =>
        FileCount == "single" ? MediaSchemas.FileSchema() : MediaSchemas.ListFilesSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => Example(FileCount == "single");

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a file object, a path, or a list of either.</param>
    /// <returns>
    /// A path, the file's bytes, or a list of them — what the Python version hands a handler for
    /// the type and file count the component was built with.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The path is the cached one rather than the one the client sent: a file that is not already
    /// in the cache is copied there, so that what a handler is handed stays readable after the
    /// upload directory is cleaned up, which is what the Python version's move-to-cache does before
    /// preprocess.
    /// </para>
    /// <para>
    /// The Python version hands a path as a <c>NamedString</c>, a string that also answers
    /// <c>.name</c>. A .NET string cannot carry an extra member, so a handler receives the path
    /// itself; the name is the last segment of it, which is the same string a <c>NamedString</c>
    /// would answer with.
    /// </para>
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        if (FileCount == "single")
        {
            return Read(FileData.FromJson(value) ?? FileData.FromJson(FirstOf(value)));
        }

        var files = new List<object?>();
        if (value is JsonArray list)
        {
            foreach (var item in list)
            {
                files.Add(Read(FileData.FromJson(item)));
            }
        }
        else
        {
            files.Add(Read(FileData.FromJson(value)));
        }

        return Type == "binary"
            ? files.Cast<byte[]>().ToArray()
            : files.Cast<string>().ToArray();
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL, a list of them, or a file object.</param>
    /// <returns>The file or files as the protocol carries them, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// A path outside the cache is copied into it, which is what makes the result downloadable: the
    /// file endpoint serves the application's own directory, and a handler's output usually lives
    /// somewhere else. A URL is left as it is — the Python version downloads it, and the fetch that
    /// would is the same one the file endpoint needs (ticket 39).
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        FileCount == "single" ? Single(value) : Many(value);

    /// <summary>One file, described as the protocol carries it.</summary>
    /// <param name="file">The file, or <see langword="null"/> for none.</param>
    /// <remarks>
    /// The size and the name are filled in for a file already in the cache as well as for one that
    /// was copied into it: the Python version describes whatever it is handed, and a handler that
    /// returns an upload path is handing over a file whose size the client is told.
    /// </remarks>
    private JsonObject? Describe(FileData? file)
    {
        if (file?.Path is not { Length: > 0 })
        {
            return null;
        }

        return MediaFiles.Describe(file, MediaFolder, Root, measure: true).ToJson();
    }

    /// <summary>One file, from a path the handler returned.</summary>
    private JsonObject? Describe(string path) => Describe(new FileData { Path = path });

    /// <summary>The value of a component that takes one file.</summary>
    private JsonNode? Single(object? value) => value switch
    {
        null => null,
        JsonNode node => Describe(FileData.FromJson(node) ?? FileData.FromJson(FirstOf(node))),
        string path => Describe(path),
        FileData file => Describe(file),
        IEnumerable<string> paths => paths.Select(Describe).FirstOrDefault(),
        _ => Refuse(value),
    };

    /// <summary>The value of a component that takes several files.</summary>
    /// <param name="value">The files, or <see langword="null"/> for none.</param>
    /// <returns>The files as the protocol carries them, or <see langword="null"/> when there are none.</returns>
    /// <remarks>
    /// No value and an empty list are two different answers: the Python version returns <c>None</c>
    /// for a component that was never given one, and the props of a component whose value is
    /// <c>None</c> carry no <c>value</c> key at all. An empty list is a component that was given an
    /// empty one.
    /// </remarks>
    private JsonArray? Many(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var files = new JsonArray();

        switch (value)
        {
            case JsonNode node:
                if (node is JsonArray list)
                {
                    foreach (var item in list)
                    {
                        files.Add(Describe(FileData.FromJson(item)));
                    }
                }
                else
                {
                    files.Add(Describe(FileData.FromJson(node)));
                }

                break;

            case string path:
                files.Add(Describe(path));
                break;

            case FileData file:
                files.Add(Describe(file));
                break;

            case IEnumerable<string> paths:
                foreach (var path in paths)
                {
                    files.Add(Describe(path));
                }

                break;

            default:
                Refuse(value);
                break;
        }

        return files;
    }

    /// <summary>One file, read as what a handler is given.</summary>
    private object? Read(FileData? file)
    {
        if (file?.Path is not { Length: > 0 } path)
        {
            return null;
        }

        // A file this server does not hold is handed over as the address it is: there are no bytes
        // here to read, and downloading it is the file endpoint's subject (ticket 39).
        if (MediaCache.NamesALocation(path))
        {
            return path;
        }

        var cached = MediaFiles.Cached(path, MediaFolder, Root);

        return Type == "binary" ? System.IO.File.ReadAllBytes(cached) : cached;
    }

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(JsonNode? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        JsonArray files => files.SelectMany(Paths),
        _ => [],
    };

    /// <summary>The first entry of a list, for a component that takes one file.</summary>
    private static JsonNode? FirstOf(JsonNode value) =>
        value is JsonArray list && list.Count > 0 ? list[0] : null;

    /// <summary>The refusal for a value of a shape a file component cannot carry.</summary>
    private static JsonNode? Refuse(object value) =>
        throw new NotSupportedException(
            $"A file component takes a path or a list of paths; it was handed a {value.GetType()}.");

    /// <summary>
    /// The example a client is shown: the Python version's own sample file, as a file object.
    /// </summary>
    /// <param name="single">Whether the component takes one file.</param>
    /// <remarks>
    /// The object is spelled the way <c>gradio_client.handle_file</c> spells one, key order
    /// included, because the config is compared with the Python version's and this is the value
    /// that appears in it.
    /// </remarks>
    private static JsonNode Example(bool single)
    {
        var example = new JsonObject
        {
            ["path"] = SampleFile,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "sample_file.pdf",
            ["url"] = SampleFile,
        };

        return single ? example : new JsonArray(example);
    }
}

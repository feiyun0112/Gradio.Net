using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A button that uploads files of the kinds the application allows when it is clicked.
/// </summary>
/// <remarks>
/// <para>
/// The file the user picks is the button's value, so this is a component rather than a button with a
/// handler attached: the frontend uploads to the application's file endpoint, the button reports the
/// uploaded file, and the <c>upload</c> event is what a handler listens for. The click is there too,
/// because a user who picks the same file twice still clicks.
/// </para>
/// <para>
/// <c>type</c> decides what a handler is handed — the file's path or its bytes — and
/// <c>file_count</c> decides whether that is one value or a list. Both change the typing information
/// the config carries, which is why the component describes itself with two different schemas: one
/// file, or a list of them.
/// </para>
/// <para>
/// <c>file_types</c> is a filter rather than a promise: the Python version passes it to the file
/// picker so the browser offers only those files, and checks it again when a file arrives — which
/// is why the check is here, in the branch that hands a handler a path, and not in the one that
/// hands it bytes. What carries the refusal is an <see cref="ArgumentException"/> for now: the
/// Python version raises a user-facing error, and the type that carries one is its own batch
/// (ticket 38), so this is the same interim shape the multimodal field uses for the same refusal.
/// </para>
/// </remarks>
[GradioBlock("uploadbutton")]
[HandlerValue("string", "upload", "change")]
public sealed partial class UploadButton : Component
{
    internal UploadButton()
    {
    }

    private string _type = "filepath";
    private string _fileCount = "single";
    private JsonNode? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The text drawn on the button.</summary>
    public override string? Label { get; set; } = "Upload a File";

    /// <summary>The file or files the button starts with.</summary>
    /// <value>
    /// A path, a URL, or a list of either, as the application wrote it; read back as the file object
    /// or objects the config carries.
    /// </value>
    /// <remarks>
    /// A JSON node rather than a string, because a multiple-file button carries a list — the same
    /// reason a file component's value is one.
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

    /// <summary>How the button is drawn: <c>primary</c>, <c>secondary</c> or <c>stop</c>.</summary>
    public string Variant { get; set; } = "secondary";

    /// <summary>How large the button is: <c>sm</c>, <c>md</c> or <c>lg</c>.</summary>
    public string Size { get; set; } = "lg";

    /// <summary>The icon drawn inside the button, as a path or a URL.</summary>
    public string? Icon { get; set; }

    /// <summary>Whether the button is drawn enabled.</summary>
    /// <value>
    /// <see langword="true"/> when the application says nothing, which is the Python version's
    /// default for an upload button.
    /// </value>
    protected override bool? InteractiveWhenUnset => true;

    /// <summary>The relative width of the button next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the button wraps.</summary>
    public int? MinWidth { get; set; }

    /// <summary>What a handler is handed: the file's path, or its bytes.</summary>
    /// <value><c>filepath</c> or <c>binary</c>.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("filepath" or "binary"))
            {
                throw new ArgumentException(
                    $"An upload button's type must be filepath or binary; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>Whether the button takes one file, several, or a whole directory.</summary>
    /// <value>One of <c>single</c>, <c>multiple</c> or <c>directory</c>.</value>
    public string FileCount
    {
        get => _fileCount;
        set
        {
            if (value is not ("single" or "multiple" or "directory"))
            {
                throw new ArgumentException(
                    $"An upload button's file_count must be one of single, multiple or directory; got '{value}'.",
                    nameof(value));
            }

            _fileCount = value;
        }
    }

    /// <summary>The extensions or kinds of file the picker offers, or <see langword="null"/> for any.</summary>
    public IReadOnlyList<string>? FileTypes { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo =>
        FileCount == "single" ? MediaSchemas.FileSchemaRaw() : MediaSchemas.ListFilesSchemaRaw();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => Example();

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>
    /// The value a handler reads from the button.
    /// </summary>
    /// <param name="value">What the client sent: a file object, a path, or a list of either.</param>
    /// <returns>A path, the file's bytes, or a list of them.</returns>
    /// <remarks>
    /// <para>
    /// The path is handed over as the client sent it, which is where this differs from a file
    /// component: what a browser sends here has just been uploaded into the application's cache, so
    /// there is nothing to copy and the Python version copies nothing.
    /// </para>
    /// <para>
    /// A button that takes one file, sent a list, reads its first entry — which is what the Python
    /// version does with a list a client sent by mistake.
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
    /// The value the button receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL, a file object, or a list of them.</param>
    /// <returns>The file or files as the protocol carries them, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// The size is measured, which is what separates this component from a file component: the
    /// Python version describes an uploaded file with its size, because the browser has just sent
    /// the bytes.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        FileCount == "single" ? Single(value) : Many(value);

    /// <summary>One file, read as what a handler is given.</summary>
    /// <param name="file">The file the client sent.</param>
    /// <exception cref="ArgumentException">The file is not one of the kinds the button accepts.</exception>
    /// <remarks>
    /// The kinds are checked here for a path and not for bytes, which is the Python version's rule
    /// and its reason: a button that hands over bytes has already decided the file is wanted, while
    /// one that hands over a path can still say no — and a client is not obliged to honour the list
    /// the field offered it.
    /// </remarks>
    private object? Read(FileData? file)
    {
        if (file?.Path is not { Length: > 0 } path)
        {
            return null;
        }

        if (Type == "filepath" && FileTypes is { Count: > 0 } accepted && !MediaTypes.Accepts(path, accepted))
        {
            throw new ArgumentException(
                "Invalid file type. " + MediaTypes.AcceptedFormats(accepted),
                nameof(file));
        }

        if (MediaCache.NamesALocation(path))
        {
            return path;
        }

        return Type == "binary" ? System.IO.File.ReadAllBytes(path) : path;
    }

    /// <summary>One file, described as the protocol carries it.</summary>
    private JsonNode? Single(object? value) => value switch
    {
        null => null,
        string path => Describe(path),
        FileData file => Describe(file),
        JsonNode node => Describe(FileData.FromJson(node) ?? FileData.FromJson(FirstOf(node))),
        IEnumerable<string> paths => paths.Select(Describe).FirstOrDefault(),
        _ => Refuse(value),
    };

    /// <summary>Several files, described as the protocol carries them.</summary>
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

    /// <summary>One file, from a path.</summary>
    private JsonObject? Describe(string path) => Describe(new FileData { Path = path });

    /// <summary>One file, from a file object.</summary>
    private JsonObject? Describe(FileData? file) =>
        file?.Path is { Length: > 0 }
            ? MediaFiles.Describe(file, MediaFolder, Root, measure: true).ToJson()
            : null;

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(JsonNode? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        JsonArray files => files.SelectMany(Paths),
        _ => [],
    };

    /// <summary>The first entry of a list, for a button that takes one file.</summary>
    private static JsonNode? FirstOf(JsonNode value) =>
        value is JsonArray list && list.Count > 0 ? list[0] : null;

    /// <summary>The refusal for a value of a shape an upload button cannot carry.</summary>
    private static JsonNode? Refuse(object value) =>
        throw new NotSupportedException(
            $"An upload button takes a path or a list of paths; it was handed a {value.GetType()}.");

    /// <summary>The example a client is shown: the Python version's own sample file, or a list
    /// holding it.</summary>
    private JsonNode Example()
    {
        const string sample =
            "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";

        var example = new JsonObject
        {
            ["path"] = sample,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "sample_file.pdf",
            ["url"] = sample,
        };

        return FileCount == "single" ? example : new JsonArray(example);
    }

    /// <summary>Binds the button's <c>click</c> event to a handler that reads nothing.</summary>
    /// <param name="fn">The handler.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="preprocess">Whether the handler's value is preprocessed.</param>
    /// <param name="postprocess">Whether the handler's result is postprocessed.</param>
    /// <param name="scrollToOutput">Whether the page scrolls to the output when the run ends.</param>
    /// <param name="showProgress">How the progress animation is drawn.</param>
    /// <param name="showProgressOn">The components the progress animation is drawn on.</param>
    /// <param name="apiName">The name the endpoint is served under.</param>
    /// <param name="apiDescription">The description the endpoint carries.</param>
    /// <param name="endpointDescription">The description the OpenAPI operation carries.</param>
    /// <param name="js">A frontend function to run as well.</param>
    /// <param name="jsRunsInBrowser">Whether the function runs in the browser or on the server.</param>
    /// <param name="jsImplementation">The source of a server-side function.</param>
    /// <param name="queue">Whether the run goes through the queue.</param>
    /// <param name="batch">Whether the handler is handed the whole batch.</param>
    /// <param name="maxBatchSize">How many runs may be batched together.</param>
    /// <param name="triggerMode">Whether the run happens once, always or never.</param>
    /// <param name="concurrencyLimit">How many runs of the handler may be in flight.</param>
    /// <param name="concurrencyId">The group the concurrency limit is counted per.</param>
    /// <param name="apiVisibility">How far the endpoint is exposed.</param>
    /// <param name="timeLimit">How long a run may take.</param>
    /// <param name="streamEvery">How often a streaming run is sent.</param>
    /// <param name="cancels">The bindings this one cancels.</param>
    /// <param name="validator">A function that decides whether a run is admitted.</param>
    /// <returns>The binding.</returns>
    /// <remarks>A click reads nothing of its own, so the shape a button is bound in most often is a handler with no arguments.</remarks>
    public Dependency Click(Func<object?> fn, Block[] outputs,         bool preprocess = true,
        bool postprocess = true,
        bool scrollToOutput = false,
        ProgressDisplay? showProgress = null,
        IReadOnlyList<Block>? showProgressOn = null,
        string? apiName = null,
        string? apiDescription = null,
        string? endpointDescription = null,
        string? js = null,
        bool jsRunsInBrowser = false,
        string? jsImplementation = null,
        bool queue = true,
        bool batch = false,
        int maxBatchSize = 4,
        TriggerMode? triggerMode = null,
        ConcurrencyLimit concurrencyLimit = default,
        string? concurrencyId = null,
        ApiVisibility apiVisibility = ApiVisibility.Public,
        double? timeLimit = null,
        double streamEvery = 0.5,
        IReadOnlyList<Dependency>? cancels = null,
        Delegate? validator = null) =>
        BindEvent(Events.Click, fn, [], outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);
}

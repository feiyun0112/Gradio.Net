using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A button that downloads one file of any type when it is clicked.
/// </summary>
/// <remarks>
/// <para>
/// The file is the button's <em>value</em>, which is what makes this a component rather than a
/// button with a link: a handler can set it, and the click downloads whatever it holds. The value
/// is postprocessed when it is read — a path becomes a file object pointing at a copy in the
/// application's cache — for the same reason a file component's is: the browser fetches it from the
/// file endpoint, which serves the application's own directory.
/// </para>
/// <para>
/// Unlike a button, this one is in the API: a handler can take it as an input (which is the file's
/// path) and return one for it, and its config therefore carries the file schema.
/// </para>
/// </remarks>
[GradioBlock("downloadbutton")]
[HandlerValue("string", "change")]
public sealed partial class DownloadButton : Component
{
    internal DownloadButton()
    {
    }

    private string? _value;
    private JsonNode? _described;
    private string? _describedIn;

    /// <summary>The text drawn on the button.</summary>
    public override string? Label { get; set; } = "Download";

    /// <summary>The file the button downloads, as a path or a URL.</summary>
    /// <value>
    /// The file the application named; read back as the file object the config carries.
    /// </value>
    /// <remarks>
    /// Described when it is read rather than when it is set, because describing a file copies it
    /// into the application's cache and the application is known only once the button has been
    /// rendered into one. The memory is keyed on the cache folder for the reason a file component's
    /// is: a read before the render describes the file into the wrong one.
    /// </remarks>
    public string? Value
    {
        get => _value;
        set
        {
            _value = value;
            _described = null;
            _describedIn = null;
        }
    }

    /// <summary>The file as the config carries it.</summary>
    [UpstreamParameter("value")]
    private JsonNode? ValueFile
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
    /// default for a download button.
    /// </value>
    protected override bool? InteractiveWhenUnset => true;

    /// <summary>The relative width of the button next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the button wraps.</summary>
    public int? MinWidth { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => Example();

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles =>
        _value is { Length: > 0 } path && !MediaCache.NamesALocation(path) ? [path] : [];

    /// <summary>
    /// The value a handler reads from the button.
    /// </summary>
    /// <param name="value">The file object the client sent.</param>
    /// <returns>The file's path, or <see langword="null"/> when nothing was sent.</returns>
    /// <remarks>Rarely used: a download button is clicked, not read.</remarks>
    public override object? Preprocess(JsonNode? value) => FileData.FromJson(value)?.Path;

    /// <summary>
    /// The file the button receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL or a file object.</param>
    /// <returns>The file as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// The file is copied into the cache and described without its size, which is what the Python
    /// version does: the endpoint measures the bytes when it serves them, so the config does not
    /// have to.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        string path when path.Length > 0 => Describe(new FileData { Path = path }),
        FileData file when file.Path is { Length: > 0 } => Describe(file),
        JsonNode node => Describe(FileData.FromJson(node) ?? new FileData()),
        _ => throw new NotSupportedException(
            $"A download button takes a path or a file object; it was handed a {value.GetType()}."),
    };

    /// <summary>One file, described as the protocol carries it.</summary>
    /// <param name="file">The file.</param>
    private JsonObject? Describe(FileData file) =>
        MediaFiles.Describe(file, MediaFolder, Root, measure: false).ToJson();

    /// <summary>
    /// The example a client is shown: the Python version's own sample file, spelled the way
    /// <c>gradio_client.handle_file</c> spells one.
    /// </summary>
    private static JsonObject Example()
    {
        const string sample =
            "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";

        return new JsonObject
        {
            ["path"] = sample,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "sample_file.pdf",
            ["url"] = sample,
        };
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

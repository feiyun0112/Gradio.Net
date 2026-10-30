using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A button that carries a label and can have arbitrary <c>click</c> handlers bound to it.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The parameters it shares with every component —
/// <c>every</c>, <c>inputs</c>, <c>visible</c>, <c>interactive</c>, the element id and classes, the
/// key — are declared on the base every component shares, which is where the Python version
/// declares them.
/// </para>
/// <para>
/// A button is not an endpoint: the Python version's <see cref="Block.SkipApi"/> is true for it, so
/// its config entry carries no typing information and a handler cannot take it as a component
/// input. Its value is still part of the config — the label a handler can set by writing to the
/// button — which is why it is postprocessed rather than left as the application wrote it.
/// </para>
/// <para>
/// The four buttons the Python version derives from this one differ in their defaults rather than
/// in their parameters, so the defaults are three overridable members rather than three sets of
/// duplicated properties: a clear button says "Clear" where a button says "Run", and a duplicate
/// button is small and black.
/// </para>
/// </remarks>
[GradioBlock("button")]
[HandlerValue("string", "change")]
public partial class Button : Component
{
    internal Button()
    {
    }

    private string? _value;
    private string? _variant;
    private string? _size;
    private int? _scale;

    /// <summary>The text drawn on the button.</summary>
    /// <value>
    /// The text the button starts with; the Python version also accepts a function and calls it on
    /// load, which arrives with the load-event batch.
    /// </value>
    public virtual string? Value
    {
        get => _value ?? DefaultValue;
        set => _value = value;
    }

    /// <summary>The text drawn when the application names none.</summary>
    protected virtual string? DefaultValue => "Run";

    /// <summary>
    /// How the button is drawn: <c>primary</c>, <c>secondary</c>, <c>stop</c> or <c>huggingface</c>.
    /// </summary>
    public string Variant
    {
        get => _variant ?? DefaultVariant;
        set => _variant = value;
    }

    /// <summary>How the button is drawn when the application names no variant.</summary>
    protected virtual string DefaultVariant => "secondary";

    /// <summary>How large the button is: <c>sm</c>, <c>md</c> or <c>lg</c>.</summary>
    public string Size
    {
        get => _size ?? DefaultSize;
        set => _size = value;
    }

    /// <summary>How large the button is when the application names no size.</summary>
    protected virtual string DefaultSize => "lg";

    /// <summary>The icon drawn inside the button, as a path or a URL.</summary>
    /// <remarks>
    /// The config carries it as a file object rather than as the path the application wrote — the
    /// Python version moves a local file into the cache so that the browser can fetch it — which is
    /// what <see cref="IconFile"/> is for. Nothing here defaults to an icon: the two buttons the
    /// Python version ships one for draw it from an image that ships with the Python package, and
    /// this build has no copy of it (see the scope ledger).
    /// </remarks>
    public string? Icon { get; set; }

    /// <summary>The icon as the config carries it.</summary>
    /// <value>
    /// A file object, or <see langword="null"/> when the button draws no icon.
    /// </value>
    /// <remarks>
    /// Described when it is read rather than when it is set, because describing a file puts a copy
    /// of it in the application's cache and the application is known only once the button has been
    /// rendered into one — the same reason a file component's value is described on read.
    /// </remarks>
    [UpstreamParameter("icon")]
    protected JsonNode? IconFile
    {
        get
        {
            if (Icon is not { Length: > 0 } icon)
            {
                return null;
            }

            if (MediaCache.NamesALocation(icon))
            {
                return MediaFiles.At(icon).ToJson();
            }

            // An icon has no original name: the Python version describes the file it was handed with its path
            // and the address it is served at, and leaves the name it was uploaded under empty.
            return MediaFiles.Describe(
                new FileData { Path = icon },
                MediaFolder,
                Root,
                measure: false,
                keepName: false).ToJson();
        }
    }

    /// <summary>The address the button opens when it is clicked, or <see langword="null"/> for none.</summary>
    public string? Link { get; set; }

    /// <summary>Where the linked address opens: <c>_self</c>, <c>_blank</c>, <c>_parent</c> or <c>_top</c>.</summary>
    public string LinkTarget { get; set; } = "_self";

    /// <summary>The relative width of the button next to its neighbours.</summary>
    public int? Scale
    {
        get => _scale ?? DefaultScale;
        set => _scale = value;
    }

    /// <summary>The width below which the button wraps.</summary>
    public int? MinWidth { get; set; }

    /// <summary>Whether the button is drawn enabled.</summary>
    /// <value>
    /// <see langword="true"/> when the application says nothing, which is the Python version's
    /// default for a button — unlike a field, whose default is "inferred from whether anything
    /// reads it".
    /// </value>
    protected override bool? InteractiveWhenUnset => true;

    /// <inheritdoc />
    /// <value>
    /// Always <see langword="true"/>: a button carries a label but is not something a client calls,
    /// so its entry has no typing information and no endpoint can take it as an input.
    /// </value>
    public override bool SkipApi => true;

    /// <summary>The width the button takes when the application names none.</summary>
    /// <value>
    /// <see langword="null"/> for a button and zero for the duplicate button, which the Python
    /// version draws at its content's width.
    /// </value>
    protected virtual int? DefaultScale => null;

    /// <inheritdoc />
    /// <remarks>
    /// Never served: a button's entry is skipped by the API entirely, so this is the answer to a
    /// question nothing asks. It is a string because the value a button carries is its label.
    /// </remarks>
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <summary>
    /// The value the button receives from what a handler returned.
    /// </summary>
    /// <param name="value">The value the handler returned.</param>
    /// <returns>The label, as text.</returns>
    /// <remarks>
    /// The Python version writes whatever a handler returns as the label with <c>str()</c>, so a
    /// number becomes its text — and so does <see langword="null"/>, which is the label <c>None</c>
    /// rather than no label at all. That is worth stating because it is surprising, and because a
    /// clear button that was given another button clears it to exactly that: the same answer the
    /// Python version gives.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        JsonValue.Create(value?.ToString() ?? "None");

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

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>A single-line or multi-line text input.</summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. A member left at its default is written only when the
/// Python version writes it: the ones the Python version defaults to a value — the type, the line
/// count, the container flag, the minimum width, visibility, autoscroll — always appear, and the
/// optional ones appear when the application sets them.
/// </para>
/// <para>
/// <see cref="Value"/> is the text the component starts with. The Python version runs it through
/// the component's post-processing before it reaches the config, which for a textbox leaves
/// <see langword="null"/> as it is.
/// </para>
/// </remarks>
[GradioBlock("textbox")]
[HandlerValue("string", "change", "input", "submit", "focus", "blur", "stop")]
public partial class Textbox : FormComponent
{
    internal Textbox()
    {
    }

    /// <summary>The text the component starts with, or <see langword="null"/> for an empty one.</summary>
    public string? Value { get; set; }

    /// <summary>The kind of text field: <c>text</c>, <c>password</c> or <c>email</c>.</summary>
    public string Type { get; set; } = "text";

    /// <summary>How many lines tall the field is drawn.</summary>
    public int Lines { get; set; } = 1;

    /// <summary>How many lines the field grows to at most, when it grows at all.</summary>
    public int? MaxLines { get; set; }

    /// <summary>The hint shown while the field is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The label drawn above the field.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the field.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the field next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the field wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>Whether the field takes focus when the page loads.</summary>
    public bool Autofocus { get; set; }

    /// <summary>Whether the field scrolls to the bottom as text is added.</summary>
    public bool Autoscroll { get; set; } = true;

    /// <summary>The horizontal alignment of the text.</summary>
    public string? TextAlign { get; set; }

    /// <summary>Whether the text runs right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>The buttons drawn inside the field.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>The longest text the field accepts.</summary>
    public int? MaxLength { get; set; }

    /// <summary>Whether the field carries a submit button, and what it says.</summary>
    /// <value>
    /// <see langword="false"/> for none — the default — <see langword="true"/> for the button's
    /// default word, or a string for the application's own.
    /// </value>
    public object? SubmitBtn { get; set; } = false;

    /// <summary>Whether the field carries a stop button, and what it says.</summary>
    /// <value>
    /// <see langword="false"/> for none — the default — <see langword="true"/> for the button's
    /// default word, or a string for the application's own.
    /// </value>
    public object? StopBtn { get; set; } = false;

    /// <summary>
    /// The HTML attributes put on the input element.
    /// </summary>
    /// <remarks>Declared because the Python version declares it; not mapped to a value the config
    /// can carry yet.</remarks>
    public object? HtmlAttributes { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("Hello!!");

    // Two signatures a port writes often enough to be worth typing: the second input is what
    // makes the handler's second parameter typed, and passing the components as an array would
    // leave the handler's own type to be checked by hand.

    /// <summary>
    /// Binds the textbox's <c>change</c> event to a handler that reads two components.
    /// </summary>
    /// <param name="fn">The handler, whose first parameter is this textbox's text.</param>
    /// <param name="other">The second component the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="parameterNames">The names the API document gives the handler's parameters.</param>
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
    /// <remarks>
    /// A two-input binding is common enough to be worth a signature: the second input is what makes
    /// the handler's second parameter typed, and passing the components as an array would leave the
    /// handler's own type to be checked by hand.
    /// </remarks>
    public Dependency Change(
        Func<string, string, object?> fn,
        Textbox other,
        Block[] outputs,
        IReadOnlyList<string?>? parameterNames = null,
                bool preprocess = true,
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
        BindEvent(Events.Change, fn, [this, other], outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Binds the textbox's <c>submit</c> event to a handler that reads two components.</summary>
    /// <param name="fn">The handler, whose first parameter is this textbox's text.</param>
    /// <param name="other">The second component the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="parameterNames">The names the API document gives the handler's parameters.</param>
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
    public Dependency Submit(
        Func<string, string, object?> fn,
        Textbox other,
        Block[] outputs,
        IReadOnlyList<string?>? parameterNames = null,
                bool preprocess = true,
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
        BindEvent(Events.Submit, fn, [this, other], outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);
}

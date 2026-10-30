using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A signal that repeats at a fixed interval while it is active.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The component draws nothing: what it contributes is a
/// clock the frontend runs, and a binding to its <c>tick</c> signal is the whole of what an
/// application does with one.
/// </para>
/// <para>
/// The interval belongs to the browser rather than to the server. The Python version's frontend
/// starts a timer of <see cref="Value"/> seconds when <see cref="Active"/> is true and dispatches
/// <c>tick</c> on each one — only while the page is visible — so an application that stops the
/// timer sets <see cref="Active"/> to <see langword="false"/> and the frontend clears it. Nothing
/// on this side wakes up on its own, which is why the config carries the two numbers rather than a
/// scheduled job.
/// </para>
/// <para>
/// The name is the Python version's and it collides with <see cref="System.Threading.Timer"/> when
/// both namespaces are imported, which every file with the SDK's implicit usings does. A file that
/// needs both qualifies this one — <c>Timer</c> — the way it already has to
/// for <c>File</c>.
/// </para>
/// </remarks>
[GradioBlock("timer")]
[HandlerValue("double?", "change")]
public sealed partial class Timer : Component
{
    internal Timer()
    {
    }

    /// <summary>The interval between two ticks, in seconds.</summary>
    public double Value { get; set; } = 1;

    /// <summary>Whether the frontend ticks at all.</summary>
    /// <value>
    /// <see langword="true"/> unless the application turns it off, which is the Python version's
    /// default: a timer written into a page starts ticking.
    /// </value>
    /// <remarks>
    /// The value is written to the config and the frontend watches it, so a handler that returns a
    /// timer with this set to <see langword="false"/> stops the clock — which is the one way
    /// the Python version offers to stop one.
    /// </remarks>
    public bool Active { get; set; } = true;

    /// <inheritdoc />
    /// <value>A number of seconds, which is what the timer's value is.</value>
    public override JsonObject ApiInfo => new() { ["type"] = "number" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create(1);

    /// <summary>
    /// Whether this block ends a run of siblings that would otherwise be grouped into one container.
    /// </summary>
    /// <value>
    /// <see langword="false"/>, which is the Python version's rule for a timer: it is invisible, so
    /// a timer written between two fields must not split the form those fields belong to.
    /// </value>
    protected internal override bool BreaksGrouping => false;

    /// <summary>The interval a handler reads.</summary>
    /// <param name="value">What the client sent: the interval in seconds.</param>
    /// <returns>The interval as a number, or <see langword="null"/> when the value is not one.</returns>
    /// <remarks>
    /// The value is the interval rather than a moment, so it is read as a number and not as the
    /// generic JSON value: a timer that is an input to another binding hands that binding the
    /// interval, which is what the Python version's identity preprocessor does with it.
    /// </remarks>
    public override object? Preprocess(JsonNode? value) => JsonValues.Number(value);

    /// <summary>The interval the client receives from what a handler returned.</summary>
    /// <param name="value">The interval in seconds, or none.</param>
    /// <returns>The interval as a number, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value) => JsonValues.FromObject(value);

    /// <summary>The progress animation a tick shows unless the application says otherwise.</summary>
    private const ProgressDisplay TickProgress = ProgressDisplay.Hidden;

    /// <summary>Binds the timer's <c>tick</c> event to a handler that reads nothing.</summary>
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
    /// <remarks>
    /// A tick reads nothing — the frontend dispatches it on its own — so the shape a timer is
    /// bound in most often is a handler with no arguments. A handler that wants the interval names
    /// the timer as an input through the generated <c>Tick(fn, inputs, outputs)</c> overload.
    /// </remarks>
    public Dependency Tick(Func<object?> fn, Block[] outputs,         bool preprocess = true,
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
        BindEvent(Events.Tick, fn, [], outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>The binding parameters a tick starts with: no progress animation.</summary>
    /// <param name="trigger">The event being bound.</param>
    /// <param name="options">What the caller passed, or <see langword="null"/> for the defaults.</param>
    /// <returns>The options, with the tick's own progress default filled in when it was not.</returns>
    private protected override EventBindingOptions? OptionsFor(EventTrigger trigger, EventBindingOptions? options) =>
        trigger.Targets.Any(target => string.Equals(target.EventName, EventNames.Tick, StringComparison.Ordinal))
            ? options switch
            {
                null => new EventBindingOptions { ShowProgress = TickProgress },
                { ShowProgress: null } => options with { ShowProgress = TickProgress },
                _ => options,
            }
            : options;
}

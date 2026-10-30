namespace Gradio.Net;
/// <summary>
/// The binding that runs when a page is opened rather than when the user does something.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Blocks.load</c> binds a handler to the application itself rather than to
/// a component: the target is <c>[root id, "load"]</c>, and the frontend dispatches it once the
/// page has mounted. It is how an application fills its components from the server before the user
/// has touched anything — a preloaded example, a value computed at startup — which is why it is a
/// binding on the root rather than a component's own event.
/// </para>
/// <para>
/// The root is a block like any other here (id 0), so the trigger is an ordinary one and nothing
/// about the binding machinery had to change to carry it.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    /// <summary>The signal the page raises once it has mounted.</summary>
    public EventTrigger Loaded => new(this, EventNames.Load);

    /// <summary>Binds a handler to the page being opened.</summary>
    /// <param name="fn">The handler, or <see langword="null"/> for a frontend-only binding.</param>
    /// <param name="inputs">The components the handler reads, or <see langword="null"/> for none.</param>
    /// <param name="outputs">The components the handler's result updates, or <see langword="null"/> for none.</param>
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
    /// <returns>The binding, for chaining.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Blocks.load</c>. A handler that reads nothing is the common case — a
    /// page opening is the whole event — but one that reads a state component is how a value
    /// survives from the first load to a handler that runs later.
    /// </para>
    /// <para>
    /// Bound through <c>EventBinding.Bind</c> rather than through <c>Blocks.On</c>:
    /// the latter is the merge entry point and marks its triggers as assembled, which is visible in
    /// the config as an event-specific argument list. A load is one signal on one block, and
    /// the Python version reports no list for it.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">A named block belongs to another application.</exception>
    public Dependency Load(
        Delegate? fn,
        IReadOnlyList<Block>? inputs = null,
        IReadOnlyList<Block>? outputs = null,
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
        EventBinding.Bind(
            [Loaded],
            fn,
            inputs ?? [],
            outputs ?? [],
            preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);
}

namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Binds one signal to a handler: the Python version's <c>gr.on(btn.click,
    /// …)</c>.</summary> <param name="triggers">The signal to listen for.</param>
    /// <param name="fn">The handler, or <see langword="null"/> for a frontend-only binding.</param>
    /// <param name="inputs">The components the handler reads, or <see langword="null"/> for
    /// none.</param> <param name="outputs">The components the handler's result updates, or
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
    /// <see langword="null"/> for the defaults.</param> <returns>The binding, for chaining with
    /// <see cref="Dependency.Then"/> and its siblings.</returns>
    /// <remarks>
    /// The Python version accepts one trigger or a sequence of them, so this side does too — the
    /// "one value or a list" reading that already applies to <c>inputs</c>, down to keeping the
    /// plural name for a parameter that holds one. The list overload carries the explanation;
    /// merging several signals is the reason the function exists.
    /// </remarks>
    public static Dependency On(
        EventTrigger triggers,
        Delegate? fn = null,
        Block[]? inputs = null,
        Block[]? outputs = null,
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
        On([triggers], fn, inputs, outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Binds several signals to one handler: the Python version's <c>gr.on</c>.</summary>
    /// <param name="triggers">The signals to listen for.</param> <param name="fn">The handler, or
    /// <see langword="null"/> for a frontend-only binding.</param> <param name="inputs">The
    /// components the handler reads, or <see langword="null"/> for none.</param>
    /// <param name="outputs">The components the handler's result updates, or <see langword="null"/>
    /// for none.</param>
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
    /// <returns>The binding, for chaining with
    /// <see cref="Dependency.Then"/> and its siblings.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="triggers"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="triggers"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">The triggers name no application to bind
    /// into.</exception>
    /// <remarks>
    /// <para>
    /// This is the Python version's <c>gr.on([btn.click, tb.change], fn, …)</c>, whose triggers are
    /// <c>[btn.Events.Click, tb.Events.Change]</c> here: the binding methods took the event names,
    /// so the trigger points moved under <c>Events</c>. Nothing else about the line
    /// changes — including <c>fn</c> being optional, which is how a port writes a frontend-only
    /// binding.
    /// </para>
    /// <para>
    /// The parameters are the binding methods' own — <c>fn</c>, <c>inputs</c>, <c>outputs</c>,
    /// <c>options</c> — because this is the same call with the triggers named explicitly, and a
    /// second spelling of a binding's parameters would be a second thing to learn. Everything else
    /// the Python version passes by keyword lives in <see cref="EventBindingOptions"/>, which is
    /// where every binding method on this side keeps it.
    /// </para>
    /// <para>
    /// The application is not a parameter, and not looked up either: a trigger knows the block it
    /// belongs to, and the block knows the application it was rendered into, so the binding lands on
    /// the page whose signals it merges. That is what lets this be a top-level function
    /// with no ambient state — including when it is called after the <c>using</c> block that wrote
    /// the page has ended, which is where a port's last lines usually are.
    /// </para>
    /// <para>
    /// Merging registers one binding, not one per trigger: the served dependency carries one
    /// <c>targets</c> entry per signal and one endpoint for all of them, which is what the Python
    /// version calls "only a single API endpoint is generated for all events in the triggers list".
    /// A handler named twice therefore answers twice and is served once.
    /// </para>
    /// <para>
    /// The call is handed to the kernel unchanged — <c>EventBinding.On</c> — because the layer
    /// decides nothing. The three positional parameters fill the same fields
    /// <see cref="EventBindingOptions"/> has for them, and a parameter that was given wins over the
    /// record's: the two spellings are the same binding, so a caller that wrote everything in the
    /// record gets the binding it described, and one that wrote the handler in the call gets that.
    /// </para>
    /// </remarks>
    public static Dependency On(
        IReadOnlyList<EventTrigger> triggers,
        Delegate? fn = null,
        Block[]? inputs = null,
        Block[]? outputs = null,
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
        Delegate? validator = null)
    {
        ArgumentNullException.ThrowIfNull(triggers);

        var binding = new EventBindingOptions
        {
            Preprocess = preprocess,
            Postprocess = postprocess,
            ScrollToOutput = scrollToOutput,
            ShowProgress = showProgress,
            ShowProgressOn = showProgressOn,
            ApiName = apiName,
            ApiDescription = apiDescription,
            EndpointDescription = endpointDescription,
            Js = js,
            JsRunsInBrowser = jsRunsInBrowser,
            JsImplementation = jsImplementation,
            Queue = queue,
            Batch = batch,
            MaxBatchSize = maxBatchSize,
            TriggerMode = triggerMode,
            ConcurrencyLimit = concurrencyLimit,
            ConcurrencyId = concurrencyId,
            ApiVisibility = apiVisibility,
            TimeLimit = timeLimit,
            StreamEvery = streamEvery,
            Cancels = cancels,
            Validator = validator,
        };

        // Only what the call actually named. Overwriting unconditionally would turn the other
        // spelling — everything in the record, which is how the core API writes a merged binding —
        // into a binding with no handler, silently.
        if (fn is not null)
        {
            binding = binding with { Fn = fn };
        }

        if (inputs is not null)
        {
            binding = binding with { Inputs = [.. inputs] };
        }

        if (outputs is not null)
        {
            binding = binding with { Outputs = [.. outputs] };
        }

        return EventBinding.On(triggers, binding);
    }
}

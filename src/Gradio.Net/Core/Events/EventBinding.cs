namespace Gradio.Net;
/// <summary>
/// The entry point every event binding goes through: triggers in, one
/// <see cref="Dependency"/> out.
/// </summary>
/// <remarks>
/// <para>
/// There are two ways to reach it and they differ only in how the inputs and outputs are spelled.
/// The typed component extensions — <c>textbox.Change(fn, textbox, output)</c> — take the
/// components directly, so the compiler checks that the handler's parameters match what the
/// components carry. The <c>object[]</c> fallback — <c>blocks.On(triggers, options)</c> with
/// <see cref="EventBindingOptions.Inputs"/> — takes them as objects, for a binding assembled at
/// runtime. Both funnel into <c>Bind</c>; keeping one implementation is what stops the two
/// paths from drifting apart.
/// </para>
/// <para>
/// The root a binding belongs to comes from its triggers, its inputs or its outputs, never from a
/// container: a component already knows which application it was rendered into
/// (<see cref="EventTrigger.ResolveRoot"/>), which is why binding needs no service provider.
/// </para>
/// </remarks>
internal static class EventBinding
{
    /// <summary>Binds a set of triggers to a handler.</summary>
    /// <param name="triggers">The signals to listen for.</param>
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
    /// <returns>The binding, for chaining with <see cref="Dependency.Then"/> and its siblings.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="triggers"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="triggers"/> is empty.</exception>
    /// <remarks>
    /// This is the <c>object[]</c> fallback: the inputs and outputs arrive as objects and are
    /// checked to be blocks. The typed extensions call <c>Bind</c> directly.
    /// </remarks>
    internal static Dependency On(
        IReadOnlyList<EventTrigger> triggers,
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
        On(triggers, new EventBindingOptions
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
        });

    public static Dependency On(IReadOnlyList<EventTrigger> triggers, EventBindingOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(triggers);

        if (triggers.Count == 0)
        {
            throw new ArgumentException("A binding needs at least one trigger.", nameof(triggers));
        }

        options ??= new EventBindingOptions();

        var inputs = BlocksOf(options.Inputs, nameof(options.Inputs));
        var outputs = BlocksOf(options.Outputs, nameof(options.Outputs));

        // A signal that reached this method was assembled rather than taken from one component, and
        // that is what the Python version's event-specific argument list records: a merged set reports a list
        // even when no event contributes a name, a single component event reports nothing. A step
        // that follows another binding is not a signal, so it is left as it is — the Python version binds one
        // through the chain rather than through <c>gr.on</c>, and reports no list for it.
        var merged = triggers
            .Select(trigger => trigger.TriggerAfter is null ? trigger.AsMerged() : trigger)
            .ToArray();

        return Bind(merged, options.Fn, inputs, outputs, options);
    }

    /// <summary>Binds a set of triggers to a handler, taking the components as blocks.</summary>
    /// <param name="triggers">The signals to listen for.</param>
    /// <param name="fn">The handler, or <see langword="null"/> for a frontend-only binding.</param>
    /// <param name="inputs">The components the handler reads.</param>
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
    /// <remarks>The typed component extensions land here; they have already checked the component types.</remarks>
    internal static Dependency Bind(
        IReadOnlyList<EventTrigger> triggers,
        Delegate? fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
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
        Bind(triggers, fn, inputs, outputs, new EventBindingOptions
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
        });

    internal static Dependency Bind(
        IReadOnlyList<EventTrigger> triggers,
        Delegate? fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        EventBindingOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(triggers);

        var inputBlocks = inputs;
        var outputBlocks = outputs;

        var root = triggers[0].ResolveRoot(inputBlocks, outputBlocks);
        foreach (var trigger in triggers)
        {
            var owner = trigger.ResolveRoot(inputBlocks, outputBlocks);
            if (!ReferenceEquals(owner, root))
            {
                throw new InvalidOperationException(
                    "Every trigger of a binding has to belong to the same application; merging "
                    + "triggers from two render roots would serve a binding that neither owns.");
            }
        }

        // A component learns which of its signals are bound, because some of them change what it
        // says about itself: a chat that can be liked carries `likeable`, one whose messages can be
        // selected carries `_selectable`, and the frontend draws the affordances from those keys.
        // The Python version does the same thing in the event listener's callback, at the same moment.
        foreach (var target in triggers.SelectMany(trigger => trigger.Targets))
        {
            target.Block?.OnEventBound(target.EventName);
        }

        return root.Register(triggers, fn, inputBlocks, outputBlocks, options ?? new EventBindingOptions());
    }

    /// <summary>Turns the components a caller passed as objects into blocks.</summary>
    internal static List<Block> BlocksOf(IReadOnlyList<object> components, string parameter)
    {
        if (components.Count == 0)
        {
            return [];
        }

        var blocks = new List<Block>(components.Count);

        foreach (var component in components)
        {
            if (component is not Block block)
            {
                throw new ArgumentException(
                    $"'{parameter}' may only name blocks; one entry is a {component?.GetType().ToString() ?? "null"}.",
                    parameter);
            }

            blocks.Add(block);
        }

        return blocks;
    }
}

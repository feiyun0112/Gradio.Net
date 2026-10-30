using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One event binding: what triggers it, what it reads, what it writes, and how it runs
/// </summary>
/// <remarks>
/// <para>
/// The type is the framework's model of a binding and, at the same time, the source of that
/// binding's entry in the served config — the Python version keeps the two in one object
/// (<c>BlockFunction</c>), and the comparison is what holds them together: a property added here
/// without a field there would show up as a missing key in the config rather than as nothing.
/// </para>
/// <para>
/// The properties divide into the ones the config carries and the ones the queue reads. Both are
/// public because both are things a caller sets and a test can observe; only the first group is
/// written by <see cref="ToConfigEntry"/>.
/// </para>
/// </remarks>
public sealed class Dependency
{
    private readonly Blocks _root;
    private readonly IReadOnlyList<EventTarget> _targets;
    private readonly IReadOnlyList<Block> _inputs;
    private readonly IReadOnlyList<Block> _outputs;
    private readonly string _defaultConcurrencyGroup;

    internal Dependency(
        Blocks root,
        int id,
        IReadOnlyList<EventTarget> targets,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        Delegate? handler,
        string? js,
        bool queue,
        string apiName,
        string? apiDescription,
        EventBindingOptions options,
        IReadOnlyList<int> cancels,
        bool isGenerator,
        bool isCancelFunction,
        int? triggerAfter,
        bool triggerOnlyOnSuccess,
        bool triggerOnlyOnFailure,
        TriggerMode triggerMode,
        ApiVisibility apiVisibility,
        IReadOnlyList<string>? eventSpecificArgs,
        ProgressDisplay showProgress)
    {
        _root = root;
        _targets = targets;
        _inputs = inputs;
        _outputs = outputs;
        _defaultConcurrencyGroup = DefaultConcurrencyGroupOf(handler, id);

        Id = id;
        Handler = handler;
        Js = js;
        JsRunsInBrowser = options.JsRunsInBrowser;
        JsImplementation = options.JsImplementation;
        Queue = queue;
        ApiName = apiName;
        ApiDescription = apiDescription;
        Options = options;
        Cancels = cancels;
        IsGenerator = isGenerator;
        IsCancelFunction = isCancelFunction;
        TriggerAfter = triggerAfter;
        TriggerOnlyOnSuccess = triggerOnlyOnSuccess;
        TriggerOnlyOnFailure = triggerOnlyOnFailure;
        TriggerMode = triggerMode;
        ApiVisibility = apiVisibility;
        EventSpecificArgs = eventSpecificArgs;
        ShowProgress = showProgress;
        PagePath = root.CurrentPagePath;
    }

    /// <summary>
    /// The path of the page this binding belongs to; the empty string is the application's home page.
    /// </summary>
    /// <remarks>
    /// A binding is set up on a page — the Python version records the page current at that moment —
    /// and the config lists it under that page, which is what tells the frontend which of the
    /// application's endpoints the page is about.
    /// </remarks>
    public string PagePath { get; }

    /// <summary>The binding's index, unique within its application and stable for its
    /// lifetime.</summary> <remarks>The Python version numbers bindings from zero in the order they
    /// were set up.</remarks>
    public int Id { get; }

    /// <summary>The application this binding belongs to.</summary>
    internal Blocks Root => _root;

    /// <summary>The signals this binding listens for.</summary>
    public IReadOnlyList<EventTarget> Targets => _targets;

    /// <summary>The components whose values the handler reads, in order.</summary>
    public IReadOnlyList<Block> Inputs => _inputs;

    /// <summary>The components the handler's result updates, in order.</summary>
    public IReadOnlyList<Block> Outputs => _outputs;

    /// <summary>The handler, or <see langword="null"/> for a binding that runs only in the frontend.</summary>
    public Delegate? Handler { get; }

    /// <summary>Whether there is a handler to run on the server.</summary>
    public bool BackendFunction => Handler is not null;

    /// <summary>Frontend JavaScript run before the handler, or <see langword="null"/> for none.</summary>
    public string? Js { get; }

    /// <summary>
    /// Whether the binding's handler runs in the browser, which the config reports as <c>"js": true</c>.
    /// </summary>
    public bool JsRunsInBrowser { get; }

    /// <summary>The browser implementation of the handler, or <see langword="null"/> for none.</summary>
    public string? JsImplementation { get; }

    /// <summary>Whether the run goes through the queue.</summary>
    /// <value>
    /// <see langword="false"/> for a binding with no handler, whatever the caller asked for: there
    /// is nothing to queue, which is the Python version's rule.
    /// </value>
    public bool Queue { get; }

    /// <summary>The name the endpoint is exposed under.</summary>
    public string ApiName { get; }

    /// <summary>The description the endpoint carries in the API documentation.</summary>
    public string? ApiDescription { get; }

    /// <summary>Whether the page scrolls to the outputs when the run completes.</summary>
    public bool ScrollToOutput => Options.ScrollToOutput;

    /// <summary>How much of the progress animation the run shows.</summary>
    /// <remarks>
    /// Resolved when the binding is registered: the caller's choice when it made one, and otherwise
    /// the default of the event that was bound — which is how the Python version's per-event
    /// <c>show_progress</c> reaches the config.
    /// </remarks>
    public ProgressDisplay ShowProgress { get; }

    /// <summary>The components the progress animation is drawn on, or <see langword="null"/> for all outputs.</summary>
    public IReadOnlyList<Block>? ShowProgressOn => Options.ShowProgressOn;

    /// <summary>Whether the handler takes a batch of inputs rather than one set.</summary>
    public bool Batch => Options.Batch;

    /// <summary>The largest batch handed to a batching handler.</summary>
    public int MaxBatchSize => Options.MaxBatchSize;

    /// <summary>The bindings this one cancels when it starts, by id.</summary>
    public IReadOnlyList<int> Cancels { get; }

    /// <summary>Whether the handler streams its result rather than returning one value.</summary>
    public bool IsGenerator { get; }

    /// <summary>Whether this binding exists to cancel others.</summary>
    public bool IsCancelFunction { get; }

    /// <summary>Whether the handler receives the event's own data alongside its inputs.</summary>
    /// <remarks>
    /// Read from the handler's signature: a parameter of one of the event-data classes is filled
    /// with what the event said about itself rather than with an input, which is the Python
    /// version's rule and what its <c>collects_event_data</c> field records.
    /// </remarks>
    public bool CollectsEventData =>
        Handler is not null && DelegateInvocation.ParametersOf(Handler)
            .Any(parameter => EventData.Is(parameter.ParameterType));

    /// <summary>The binding this one follows, or <see langword="null"/> when a signal triggers it.</summary>
    public int? TriggerAfter { get; }

    /// <summary>Whether this step runs only when the binding it follows succeeded.</summary>
    public bool TriggerOnlyOnSuccess { get; }

    /// <summary>Whether this step runs only when the binding it follows failed.</summary>
    public bool TriggerOnlyOnFailure { get; }

    /// <summary>How many submissions are allowed while one is pending.</summary>
    public TriggerMode TriggerMode { get; }

    /// <summary>How far the endpoint is exposed to clients.</summary>
    public ApiVisibility ApiVisibility { get; }

    /// <summary>The <c>connection</c> value of a binding the browser feeds a chunk at a time.</summary>
    internal const string StreamConnection = "stream";

    /// <summary>The <c>connection</c> value of a binding observed over the queue's event stream.</summary>
    internal const string SseConnection = "sse";

    /// <summary>The protocol the frontend uses to observe the run.</summary>
    /// <value>
    /// <c>stream</c> when the binding listens for a component's <c>stream</c> event, and <c>sse</c>
    /// otherwise.
    /// </value>
    /// <remarks>
    /// The Python version makes this a property of the event rather than of the binding, because
    /// the two protocols are two ways of the client talking back: an <c>sse</c> run is told to
    /// start and then speaks, while a <c>stream</c> run is fed a new chunk over
    /// <c>/stream/{event_id}</c> and answers each one. A binding on a stream event is therefore the
    /// only kind that can be either, and it is the event that decides.
    /// </remarks>
    public string Connection => _targets.Any(target => target.EventName == EventNames.Stream)
        ? StreamConnection
        : SseConnection;

    /// <summary>The time limit for the run, in seconds, or <see langword="null"/> for none.</summary>
    public double? TimeLimit => Options.TimeLimit;

    /// <summary>How often streamed chunks are sent, in seconds.</summary>
    public double StreamEvery => Options.StreamEvery;

    /// <summary>Whether the input components preprocess their values before the handler runs.</summary>
    public bool Preprocess => Options.Preprocess;

    /// <summary>Whether the output components postprocess the result before it is sent.</summary>
    public bool Postprocess => Options.Postprocess;

    /// <summary>How many runs of this binding may be in flight at once.</summary>
    public ConcurrencyLimit ConcurrencyLimit => Options.ConcurrencyLimit;

    /// <summary>The concurrency group this binding belongs to, or <see langword="null"/> for none.</summary>
    public string? ConcurrencyId => Options.ConcurrencyId;

    /// <summary>
    /// The group the queue counts this binding's runs against.
    /// </summary>
    /// <value>
    /// The group the caller named, or the one derived from the handler.
    /// </value>
    /// <remarks>
    /// A binding that names no group gets one of its own — except that two bindings running the
    /// same handler share theirs, which is the Python version's rule: it groups by the identity of
    /// the function, so binding one function to two events limits the two together. That is the
    /// behaviour a caller protecting a shared resource depends on, and it is why the derived group
    /// is a property of the handler rather than of the binding.
    /// </remarks>
    internal string ConcurrencyGroup => ConcurrencyId ?? _defaultConcurrencyGroup;

    /// <summary>What the run of this binding looks like to the runner.</summary>
    internal RunShape Shape => new(Handler, Inputs, Outputs, Preprocess, Postprocess, TrackedStateIds());

    /// <summary>
    /// The stateful outputs whose change this binding reports.
    /// </summary>
    /// <returns>
    /// The ids of the state components this binding writes whose <c>change</c> signal something
    /// listens to.
    /// </returns>
    /// <remarks>
    /// A state component has no frontend to raise an event, so the server is what tells the client
    /// its value changed — and only for the ones somebody is listening to, which is the Python
    /// version's rule: reporting a value nobody watches would make every run of a stateful
    /// application carry a list of components no client acts on.
    /// </remarks>
    private IReadOnlyList<int> TrackedStateIds() =>
        [.. _outputs
            .Where(block => block is IStatefulComponent && _root.ListensForChange(block.Id))
            .Select(block => block.Id)];

    /// <summary>A function run before the handler to reject input, or <see langword="null"/> for none.</summary>
    public Delegate? Validator => Options.Validator;

    /// <summary>The event-specific argument names the config carries, or <see langword="null"/>.</summary>
    internal IReadOnlyList<string>? EventSpecificArgs { get; }

    /// <summary>The names the binding's inputs are called in the API documentation, or <see langword="null"/>.</summary>
    internal IReadOnlyList<string?>? ParameterNames => Options.ParameterNames;

    /// <summary>
    /// The description the API documentation shows for this binding.
    /// </summary>
    /// <value>
    /// The description the caller gave, or the one meant for the documentation alone, or the empty
    /// string for a binding that has neither.
    /// </value>
    /// <remarks>
    /// The two are not the same field: a caller can describe its endpoint without describing its
    /// binding, which is what the Python version's docstring fallback amounts to.
    /// </remarks>
    internal string DocumentationDescription => ApiDescription ?? Options.EndpointDescription ?? string.Empty;

    /// <summary>The binding parameters this binding was set up with.</summary>
    private EventBindingOptions Options { get; }

    /// <summary>The steps that follow this binding, as values.</summary>
    /// <value>
    /// A set carrying <c>Then</c>, <c>Success</c> and <c>Failure</c>, for an application that
    /// assembles a chained step and binds it with <c>Blocks.On</c>.
    /// </value>
    public DependencyEvents Events => new(this);

    /// <summary>Binds a step that runs after this one, whatever its outcome.</summary>
    /// <param name="fn">The handler, or <see langword="null"/> for a step that only passes values on.</param>
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
    /// <returns>The step, so that a chain can go on.</returns>
    /// <remarks>
    /// The step carries this binding's id as its <c>trigger_after</c>, which is what makes it run
    /// second. A chained step listens to no block: its target is the pair <c>[null, "then"]</c>.
    /// </remarks>
    public Dependency Then(
        Delegate? fn,
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
        BindChained(Events.Then, fn, inputs, outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Binds a step that runs after this one, only when it succeeded.</summary>
    /// <param name="fn">The handler.</param>
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
    /// <returns>The step, so that a chain can go on.</returns>
    public Dependency Success(
        Delegate? fn,
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
        BindChained(Events.Success, fn, inputs, outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Binds a step that runs after this one, only when it failed.</summary>
    /// <param name="fn">The handler.</param>
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
    /// <returns>The step, so that a chain can go on.</returns>
    public Dependency Failure(
        Delegate? fn,
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
        BindChained(Events.Failure, fn, inputs, outputs, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Binds a chained step, which is a binding with one trigger and no block behind it.</summary>
    /// <param name="trigger">Which of the three steps.</param>
    /// <param name="fn">The handler.</param>
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
    /// <returns>The step.</returns>
    private static Dependency BindChained(
        EventTrigger trigger,
        Delegate? fn,
        Block[]? inputs,
        Block[]? outputs,
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
        BindChained(trigger, fn, inputs, outputs, new EventBindingOptions
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

    private static Dependency BindChained(
        EventTrigger trigger,
        Delegate? fn,
        Block[]? inputs,
        Block[]? outputs,
        EventBindingOptions? options) =>
        EventBinding.On(
            [trigger],
            options ?? new EventBindingOptions { Fn = fn, Inputs = [.. inputs ?? []], Outputs = [.. outputs ?? []] });

    /// <summary>
    /// The group a binding that names none belongs to.
    /// </summary>
    /// <param name="handler">The binding's handler, or <see langword="null"/>.</param>
    /// <param name="id">The binding's id, used when there is no handler to group by.</param>
    /// <returns>A name two bindings share exactly when they run the same handler.</returns>
    /// <remarks>
    /// <para>
    /// The Python version groups by <c>id(fn)</c> — the identity of the function object. A .NET
    /// delegate has no such identity to hand out, so the method behind it and the object it was
    /// bound to stand in: the same method on the same target is the same function, which is the
    /// equivalence the Python version's grouping expresses.
    /// </para>
    /// <para>
    /// A handler the framework compiled — the one a flag button binds, whose arity is only known once
    /// the interface is built — has no module or token to ask for: its method is emitted at run time
    /// and answering either question throws. Its name stands in, which is what distinguishes one
    /// compiled handler from another.
    /// </para>
    /// </remarks>
    private static string DefaultConcurrencyGroupOf(Delegate? handler, int id) =>
        handler is null
            ? $"fn:{id}"
            : $"fn:{IdentityOf(handler.Method)}:{RuntimeHelpers.GetHashCode(handler.Target)}";

    /// <summary>What tells one method from another.</summary>
    /// <param name="method">The method behind a delegate.</param>
    /// <returns>The method's module and token, or its name when it was emitted at run time.</returns>
    private static string IdentityOf(MethodInfo method) =>
        method is DynamicMethod
            ? method.Name
            : $"{method.Module.ModuleVersionId}:{method.MetadataToken}";

    /// <summary>Whether a handler streams its result.</summary>
    /// <param name="fn">The handler, or <see langword="null"/>.</param>
    /// <returns>Whether the handler's result is an <see cref="IAsyncEnumerable{T}"/>.</returns>
    /// <remarks>
    /// Streaming is one of the three output shapes: a handler that returns
    /// <see cref="IAsyncEnumerable{T}"/> — directly or awaited from a task — is a generator, and
    /// the config says so. The Python version recognises the same shape from a Python generator
    /// function.
    /// </remarks>
    internal static bool StreamsResult(Delegate? fn)
    {
        var type = fn?.Method.ReturnType;
        if (type is null)
        {
            return false;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            type = type.GetGenericArguments()[0];
        }

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
    }

    /// <summary>
    /// The binding's entry in the served config.
    /// </summary>
    /// <returns>The entry, with the Python version's keys in the Python version's order.</returns>
    /// <remarks>
    /// Key order is not something a comparison sees, but it is something a reviewer sees: a config
    /// that reads like the Python version's can be diffed by eye against it, which is how the field
    /// list here was checked.
    /// </remarks>
    internal JsonObject ToConfigEntry() => new()
    {
        ["id"] = Id,
        ["targets"] = TargetsOf(),
        ["inputs"] = Ids(_inputs),
        ["outputs"] = Ids(_outputs),
        ["backend_fn"] = BackendFunction,
        ["js"] = JsScript(),
        ["queue"] = Queue,
        ["api_name"] = ApiName,
        ["api_description"] = ApiDescription,
        ["scroll_to_output"] = ScrollToOutput,
        ["show_progress"] = ProgressName(ShowProgress),
        ["show_progress_on"] = ShowProgressOn is null ? null : Ids(ShowProgressOn),
        ["batch"] = Batch,
        ["max_batch_size"] = MaxBatchSize,
        ["cancels"] = Numbers(Cancels),
        ["types"] = new JsonObject
        {
            ["generator"] = IsGenerator,
            ["cancel"] = IsCancelFunction,
        },
        ["collects_event_data"] = CollectsEventData,
        ["trigger_after"] = TriggerAfter is { } after ? JsonValue.Create(after) : null,
        ["trigger_only_on_success"] = TriggerOnlyOnSuccess,
        ["trigger_only_on_failure"] = TriggerOnlyOnFailure,
        ["trigger_mode"] = TriggerModeName(TriggerMode),
        ["api_visibility"] = ApiVisibilityName(ApiVisibility),

        // Both belong to a dynamic render, which is its own batch: nothing here is rendered into a
        // renderable, so the fields are present and null rather than absent.
        ["rendered_in"] = null,
        ["render_id"] = null,

        ["connection"] = Connection,
        ["time_limit"] = TimeLimit is { } limit ? JsonValue.Create(limit) : null,
        ["stream_every"] = StreamEvery,
        ["event_specific_args"] = EventSpecificArgs is null ? null : Strings(EventSpecificArgs),
        ["component_prop_inputs"] = new JsonArray(),
        ["js_implementation"] = JsImplementation,
    };

    /// <summary>The <c>js</c> field, which is a script, the word <c>true</c>, or nothing.</summary>
    /// <returns>The field's value.</returns>
    /// <remarks>
    /// The Python version writes the script for a binding that runs one in the frontend before its
    /// handler, and the boolean for one whose handler <em>is</em> the frontend implementation — the
    /// value its transpiler produces. The two are different keys in the same slot, which is why
    /// this is not simply the string.
    /// </remarks>
    private JsonValue? JsScript() =>
        Js is not null ? JsonValue.Create(Js) : JsRunsInBrowser ? JsonValue.Create(true) : null;

    /// <summary>The Python spelling of a progress setting.</summary>
    internal static string ProgressName(ProgressDisplay progress) => progress switch
    {
        ProgressDisplay.Full => "full",
        ProgressDisplay.Minimal => "minimal",
        _ => "hidden",
    };

    /// <summary>The Python spelling of a trigger mode.</summary>
    internal static string TriggerModeName(TriggerMode mode) => mode switch
    {
        TriggerMode.Multiple => "multiple",
        TriggerMode.AlwaysLast => "always_last",
        _ => "once",
    };

    /// <summary>The Python spelling of a visibility.</summary>
    internal static string ApiVisibilityName(ApiVisibility visibility) => visibility switch
    {
        ApiVisibility.Private => "private",
        ApiVisibility.Undocumented => "undocumented",
        _ => "public",
    };

    /// <summary>The trigger a chained step listens for.</summary>
    /// <param name="name">Which step: <c>then</c>, <c>success</c> or <c>failure</c>.</param>
    /// <param name="onlyOnSuccess">Whether the step runs only when this binding succeeded.</param>
    /// <param name="onlyOnFailure">Whether the step runs only when this binding failed.</param>
    /// <returns>The trigger, carrying this binding's id as what it follows.</returns>
    internal EventTrigger ChainedTrigger(string name, bool onlyOnSuccess, bool onlyOnFailure) =>
        EventTrigger.Chained(_root, Id, name, onlyOnSuccess, onlyOnFailure);

    private JsonArray TargetsOf()
    {
        var targets = new JsonArray();
        foreach (var target in _targets)
        {
            targets.Add(new JsonArray(
                target.BlockId is { } id ? JsonValue.Create(id) : null,
                JsonValue.Create(target.EventName)));
        }

        return targets;
    }

    private static JsonArray Ids(IReadOnlyList<Block> blocks)
    {
        var ids = new JsonArray();
        foreach (var block in blocks)
        {
            ids.Add(block.Id);
        }

        return ids;
    }

    private static JsonArray Numbers(IReadOnlyList<int> values)
    {
        var numbers = new JsonArray();
        foreach (var value in values)
        {
            numbers.Add(value);
        }

        return numbers;
    }

    private static JsonArray Strings(IReadOnlyList<string> values)
    {
        var strings = new JsonArray();
        foreach (var value in values)
        {
            strings.Add(value);
        }

        return strings;
    }
}

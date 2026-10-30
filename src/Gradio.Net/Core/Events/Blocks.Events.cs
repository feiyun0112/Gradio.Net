using System.Collections.ObjectModel;

namespace Gradio.Net;
/// <summary>
/// The bindings an application owns, and the entry point that adds one
/// </summary>
/// <remarks>
/// <para>
/// Bindings are numbered from zero in the order they were set up, which is the Python version's
/// numbering and the one the config's <c>id</c>, the <c>trigger_after</c> chain and <c>cancels</c>
/// all refer to. They live on the root rather than on the blocks they listen to because a binding
/// is a property of the application: several blocks can share one, and a chained step listens to no
/// block at all.
/// </para>
/// <para>
/// <c>On</c> is the merge entry point — the counterpart of the Python version's <c>gr.on</c>
/// — and the <c>object[]</c> fallback. The typed extensions on the components call
/// <see cref="Register"/> through <see cref="EventBinding"/>.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    private readonly List<Dependency> _dependencies = [];
    private readonly ReadOnlyCollection<Dependency> _dependenciesView;
    private int _nextDependencyId;

    /// <summary>The bindings set up on this application, in the order they were set up.</summary>
    /// <remarks>This is the list the config is built from, not a hook for tests.</remarks>
    public IReadOnlyList<Dependency> Dependencies => _dependenciesView;

    /// <summary>Binds a set of triggers to a handler, with the parameters already gathered.</summary>
    /// <param name="triggers">The signals to listen for.</param>
    /// <param name="options">The binding parameters.</param>
    /// <returns>The binding.</returns>
    internal Dependency On(IReadOnlyList<EventTrigger> triggers, EventBindingOptions? options)
    {
        // A merged set is not a signal on a component: it is assembled, and the Python version
        // records that in the config's event-specific argument list. EventBinding.On is where the
        // triggers are marked as merged, so this goes through it rather than through Register.
        //
        // The seal is checked here because that call reaches the root through the triggers rather
        // than through this application: a binding is a write into the application, and a page that
        // has already been served refuses one.
        ThrowIfSealed();

        return EventBinding.On(triggers, options);
    }

    /// <summary>Binds several signals to one handler, with the handler and its components named.</summary>
    internal Dependency On(
        IReadOnlyList<EventTrigger> triggers,
        Delegate? fn = null,
        IReadOnlyList<Block>? inputs = null,
        IReadOnlyList<Block>? outputs = null,
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
        On(
            triggers,
            new EventBindingOptions
        {
            ParameterNames = parameterNames,
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
        } with { Fn = fn, Inputs = [.. inputs ?? []], Outputs = [.. outputs ?? []] });

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
    /// <remarks>
    /// This is what merges several triggers into one endpoint: the Python version's <c>gr.on</c> is
    /// the same call with a list of triggers, and the served binding carries one <c>targets</c>
    /// entry per trigger. It is also the <c>object[]</c> fallback — the components arrive through
    /// <see cref="EventBindingOptions.Inputs"/> and <see cref="EventBindingOptions.Outputs"/>.
    /// </remarks>
    public Dependency On(
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
        EventBinding.On(triggers, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>Adds a binding to this application and gives it its id.</summary>
    internal Dependency Register(
        IReadOnlyList<EventTrigger> triggers,
        Delegate? fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        EventBindingOptions options)
    {
        // A binding is a write into the application as much as a block is: it changes the config
        // and the API description the next request is answered with, so it is refused by the same
        // seal that refuses a block (see Blocks.Seal).
        ThrowIfSealed();

        CheckOwned(inputs, nameof(inputs));
        CheckOwned(outputs, nameof(outputs));

        var cancelled = CancelledIds(options);

        var id = _nextDependencyId++;
        var (apiName, derived) = ApiNameFor(fn, options.Js, options.ApiName);
        var apiVisibility = VisibilityFor(fn, options, derived);

        // An endpoint derived from a handler's signature has no trigger at all: nothing in the page
        // raises it, and a client reaches it by name. The fields a trigger would have filled are
        // then the ones a binding with no trigger has, which is the Python version's answer for the same
        // empty list.
        var first = triggers.Count > 0 ? triggers[0] : null;

        // The binding that cancels carries no list of its own: the list belongs to the cancellation,
        // which is the second binding registered below. The Python version serves it the same way, and a
        // binding whose config named what it cancels would be a binding no client could act on.
        var dependency = new Dependency(
            this,
            id,
            Targets(triggers),
            inputs,
            outputs,
            fn,
            options.Js,
            QueueOf(fn, options),
            apiName,
            options.ApiDescription,
            options,
            [],
            Dependency.StreamsResult(fn),
            isCancelFunction: false,
            triggerAfter: first?.TriggerAfter,
            triggerOnlyOnSuccess: first?.TriggerOnlyOnSuccess ?? false,
            triggerOnlyOnFailure: first?.TriggerOnlyOnFailure ?? false,
            triggerMode: TriggerModeFor(triggers, options),
            apiVisibility: apiVisibility,
            eventSpecificArgs: MergedEventSpecificArgs(triggers),
            showProgress: ShowProgressFor(options));

        _dependencies.Add(dependency);

        // The Python version registers a second, private binding that performs the cancellation: starting this
        // binding starts that one, and that one carries the list of what it stops. It is a separate
        // dependency in the config rather than a field on this one, and the comparison is field by
        // field, so the shape has to be reproduced rather than approximated.
        //
        // Its name is deduplicated like any other: the Python version gives it "unnamed" and then makes every
        // name unique against the ones already served, so a second cancellation becomes
        // "unnamed_1" rather than colliding with the first.
        if (cancelled.Count > 0)
        {
            _dependencies.Add(new Dependency(
                this,
                _nextDependencyId++,
                Targets(triggers),
                [],
                [],
                handler: null,
                js: null,
                queue: false,
                apiName: ApiNameFor(null, null, "unnamed").Name,
                apiDescription: null,
                options,
                cancelled,
                isGenerator: false,
                isCancelFunction: true,
                triggerAfter: null,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false,
                triggerMode: TriggerMode.Once,
                apiVisibility: ApiVisibility.Private,
                eventSpecificArgs: null,

                // The Python version registers this one without a progress setting of its own, so it always
                // shows the full animation — whatever the binding it cancels asked for.
                showProgress: ProgressDisplay.Full));
        }

        return dependency;
    }

    /// <summary>
    /// Whether any binding of this application listens for a block's <c>change</c> signal.
    /// </summary>
    /// <param name="blockId">The block's id.</param>
    /// <returns>Whether a binding targets it.</returns>
    /// <remarks>
    /// A state component cannot raise its own event, so the server reports which ones changed and
    /// the frontend dispatches <c>change</c> for the ones something is bound to. That makes "is
    /// anything listening" a question the run has to answer, and the bindings are where it is
    /// answered.
    /// </remarks>
    internal bool ListensForChange(int blockId) =>
        _dependencies.Any(dependency => dependency.Targets.Any(
            target => target.BlockId == blockId && target.EventName == EventNames.Change));

    /// <summary>The ids of the bindings a binding cancels, in the order they were given.</summary>
    private List<int> CancelledIds(EventBindingOptions options)
    {
        if (options.Cancels is not { Count: > 0 } cancels)
        {
            return [];
        }

        var ids = new List<int>(cancels.Count);

        foreach (var dependency in cancels)
        {
            if (!ReferenceEquals(dependency.Root, this))
            {
                throw new InvalidOperationException(
                    "A binding can only cancel a binding of the same application; a cancels list "
                    + "crossing two render roots would serve a binding that names an id it does not own.");
            }

            ids.Add(dependency.Id);
        }

        return ids;
    }

    /// <summary>The target pairs a binding listens for, one per trigger.</summary>
    private static List<EventTarget> Targets(IReadOnlyList<EventTrigger> triggers)
    {
        var targets = new List<EventTarget>();

        foreach (var trigger in triggers)
        {
            targets.AddRange(trigger.Targets);
        }

        return targets;
    }

    /// <summary>
    /// The event-specific argument names a binding carries, or <see langword="null"/> when it
    /// carries none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The distinction between <see langword="null"/> and an empty list is the Python version's and
    /// is visible in the config: a trigger set assembled by <c>gr.on</c> reports a list — empty
    /// when no event contributes a name — while a binding to a component's own signal reports
    /// nothing at all. Reproducing it is the whole point: an application that merged its triggers
    /// and one that did not are different documents.
    /// </para>
    /// <para>
    /// No event this batch implements contributes a name, so the merged case is the only value
    /// reachable here, and it is always the empty list.
    /// </para>
    /// </remarks>
    private static List<string>? MergedEventSpecificArgs(IReadOnlyList<EventTrigger> triggers)
    {
        List<string>? merged = null;

        foreach (var trigger in triggers)
        {
            if (!trigger.IsMerged)
            {
                continue;
            }

            merged ??= [];
            if (trigger.EventSpecificArgs is { } args)
            {
                merged.AddRange(args);
            }
        }

        return merged;
    }

    /// <summary>Whether the run goes through the queue.</summary>
    /// <remarks>
    /// A binding with no handler is never queued, whatever the caller asked for: there is nothing
    /// to queue, which is the Python version's rule.
    /// </remarks>
    private static bool QueueOf(Delegate? fn, EventBindingOptions options) => fn is not null && options.Queue;

    /// <summary>
    /// The trigger mode a binding runs with.
    /// </summary>
    /// <remarks>
    /// An unset mode is not <see cref="TriggerMode.Once"/>: the Python version picks it from the
    /// first trigger's event name — <c>change</c> and <c>key_up</c> allow a second submission once
    /// the pending one completes, <c>stream</c> allows any number, and everything else allows none.
    /// A binding with no trigger has no event name to pick from, and the Python version leaves it
    /// at the default, which is <see cref="TriggerMode.Once"/>.
    /// </remarks>
    private static TriggerMode TriggerModeFor(IReadOnlyList<EventTrigger> triggers, EventBindingOptions options)
    {
        if (options.TriggerMode is { } mode)
        {
            return mode;
        }

        if (triggers.Count == 0)
        {
            return TriggerMode.Once;
        }

        return triggers[0].Targets[0].EventName switch
        {
            "change" or "key_up" => TriggerMode.AlwaysLast,
            "stream" => TriggerMode.Multiple,
            _ => TriggerMode.Once,
        };
    }

    /// <summary>
    /// How much of the progress animation a binding shows.
    /// </summary>
    /// <remarks>
    /// A binding that says nothing shows all of it. The one event with a different default —
    /// <c>tick</c>, which hides it — declares that on its own method, the way the Python version
    /// declares it on the event: the default belongs to the event rather than to the binding, and a
    /// trigger merged through <c>Blocks.On</c> gets the generic default that the Python version's
    /// <c>gr.on</c> uses.
    /// </remarks>
    private static ProgressDisplay ShowProgressFor(EventBindingOptions options) =>
        options.ShowProgress ?? ProgressDisplay.Full;

    /// <summary>
    /// How far a binding's endpoint is exposed.
    /// </summary>
    /// <param name="fn">The handler, or <see langword="null"/>.</param>
    /// <param name="options">The binding parameters, carrying what the caller asked for.</param>
    /// <param name="derivedName">
    /// Whether the name was derived rather than given, which is what decides whether the visibility
    /// is overridden.
    /// </param>
    /// <returns>The visibility the binding is served with.</returns>
    /// <remarks>
    /// A binding with no handler and no name has no endpoint anyone chose, so it is private
    /// whatever the caller asked for — the Python version's rule, and the one that keeps a binding
    /// which never reaches the server out of the API. Everything else keeps the visibility it was
    /// given: a handler is something a client can call, whether or not the caller named it, and a
    /// name the caller gave is a name the caller chose.
    /// </remarks>
    private static ApiVisibility VisibilityFor(Delegate? fn, EventBindingOptions options, bool derivedName) =>
        fn is null && derivedName ? ApiVisibility.Private : options.ApiVisibility;

    /// <summary>
    /// The name a binding's endpoint is exposed under.
    /// </summary>
    /// <param name="fn">The handler, or <see langword="null"/>.</param>
    /// <param name="js">The frontend JavaScript, or <see langword="null"/>.</param>
    /// <param name="requested">The name the caller gave, or <see langword="null"/> for none.</param>
    /// <returns>
    /// The name, made unique among the names already in use, and whether it was derived rather than
    /// given.
    /// </returns>
    /// <remarks>
    /// The Python version derives the name from the handler: the function's own name, or
    /// <c>lambda</c> for one that has none. A frontend-only binding is <c>js_fn</c> and a binding
    /// with neither is <c>unnamed</c>. A name already in use gets a numeric suffix — whoever asked
    /// for it: the Python version suffixes a name the caller gave as well as one it derived, which
    /// is how one button that binds two handlers under one name serves two endpoints. A name the
    /// caller gave is otherwise used as it stands, and that is the case the second half of the
    /// result distinguishes.
    /// </remarks>
    private (string Name, bool Derived) ApiNameFor(Delegate? fn, string? js, string? requested)
    {
        var derived = string.IsNullOrEmpty(requested);

        var name = derived
            ? fn is not null ? FunctionNameOf(fn) : js is not null ? "js_fn" : "unnamed"
            : requested!;

        var used = _dependencies.Select(dependency => dependency.ApiName).ToHashSet(StringComparer.Ordinal);
        if (!used.Contains(name))
        {
            return (name, derived);
        }

        for (var suffix = 1; ; suffix++)
        {
            var candidate = $"{name}_{suffix}";
            if (!used.Contains(candidate))
            {
                return (candidate, derived);
            }
        }
    }

    /// <summary>
    /// The name of the method behind a delegate.
    /// </summary>
    /// <remarks>
    /// A compiler-generated method — a lambda, an anonymous method — has a name that is not a legal
    /// C# identifier, so it is spelled <c>lambda</c>, which is what the Python version spells an
    /// anonymous handler.
    /// </remarks>
    private static string FunctionNameOf(Delegate fn)
    {
        var name = fn.Method.Name;

        return name.Length == 0 || name.Contains('<') ? "lambda" : name;
    }

    /// <summary>
    /// The name of the method behind a delegate, as the API and the MCP catalog spell it.
    /// </summary>
    /// <param name="fn">The handler.</param>
    /// <returns>The name, or <c>lambda</c> for a handler the compiler generated.</returns>
    internal static string NameOf(Delegate fn) => FunctionNameOf(fn);

    /// <summary>Fails when a component in a binding does not belong to this application.</summary>
    private void CheckOwned(IReadOnlyList<Block> blocks, string parameter)
    {
        foreach (var block in blocks)
        {
            if (!ReferenceEquals(block.Root, this))
            {
                throw new InvalidOperationException(
                    $"A binding may only name blocks of its own application; one entry of '{parameter}' "
                    + "belongs to another render root, or was never rendered.");
            }
        }
    }
}

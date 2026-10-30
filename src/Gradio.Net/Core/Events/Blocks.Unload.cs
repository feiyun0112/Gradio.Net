namespace Gradio.Net;
/// <summary>
/// The binding that runs when the page showing the application goes away.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Blocks.unload</c> is the other half of <see cref="Load"/>: the target is
/// <c>[root id, "unload"]</c>, and what it is for is the work an application does for one visitor
/// when that visitor leaves — releasing what it opened, writing what it collected, telling a
/// service the session is over.
/// </para>
/// <para>
/// Nothing in the page raises this signal, and that is not a gap in the frontend: a page that is
/// going away cannot be asked to make a request. The connection the page held open is the signal.
/// The server is the far end of that connection, so it is the server that runs these bindings, once,
/// when the session ends (<see cref="GradioQueue.CloseSessionAsync"/>) — which is also why an
/// application with one asks the frontend to keep a connection open at all (see the config's
/// <c>connect_heartbeat</c>).
/// </para>
/// <para>
/// A run of this binding has no client to answer and no values to be sent: the handler reads the
/// session's own state, and the components that are not stateful carry the last values the client
/// was shown. What it produces is dropped, because there is nobody left to receive it.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    /// <summary>The signal raised when the page showing the application goes away.</summary>
    /// <remarks>
    /// Exposed like any other signal so that a handler can be bound to it through
    /// <c>On</c>, which is how a caller merges it with another trigger.
    /// </remarks>
    public EventTrigger Unloaded => new(this, EventNames.Unload);

    /// <summary>Binds a handler to the page going away.</summary>
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
    /// <returns>The binding, for chaining.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Blocks.unload</c>, bound the way <see cref="Load"/> is: one signal
    /// on the application, through <c>EventBinding.Bind</c> rather than
    /// <c>Blocks.On</c>, so that the config reports no event-specific argument list for it.
    /// </para>
    /// <para>
    /// The outputs are accepted and written because a handler may have them — a session's state is
    /// the usual one — but nothing is sent anywhere with the result: the run happens after the last
    /// thing the client will ever read.
    /// </para>
    /// <para>
    /// A binding with no handler is accepted and never runs: this signal is raised by the server,
    /// and the server runs what it can call. The Python version's frontend of this release has no
    /// code that would dispatch one either, so the two agree by both doing nothing.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">A named block belongs to another application.</exception>
    public Dependency Unload(
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
            [Unloaded],
            fn,
            inputs ?? [],
            outputs ?? [],
            preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>
    /// Whether this application has work to do when a page goes away.
    /// </summary>
    /// <remarks>
    /// The question the config answers to the frontend, which is why it is a flag rather than a
    /// list: a request for the config asks it, and nothing about the answer needs the bindings
    /// themselves.
    /// </remarks>
    internal bool BindsUnload => _dependencies.Exists(IsUnload);

    /// <summary>
    /// The bindings that run when the page goes away, in the order they were set up.
    /// </summary>
    /// <remarks>
    /// The other half of the same question, asked by the session's teardown, which runs them.
    /// </remarks>
    internal IReadOnlyList<Dependency> UnloadBindings => [.. _dependencies.Where(IsUnload)];

    /// <summary>Whether a binding listens for the application's own <c>unload</c> signal.</summary>
    /// <param name="dependency">The binding.</param>
    private bool IsUnload(Dependency dependency) =>
        dependency.Targets.Any(target => target.BlockId == Id && target.EventName == EventNames.Unload);
}

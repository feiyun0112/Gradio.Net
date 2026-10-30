namespace Gradio.Net;
/// <summary>
/// The binding parameters every event binding accepts, whatever event it is for
/// </summary>
/// <remarks>
/// <para>
/// The Python version passes these as a long keyword list to every event method. Here they are one
/// record, so that the binding methods keep a readable signature and so that a parameter added
/// later does not change every overload's arity.
/// </para>
/// <para>
/// The properties divide into two kinds, and the division matters. Some are part of the served
/// dependency — the API name, the trigger mode, the time limit, the visibility — and the config
/// comparison is what holds them to the Python version's behaviour. The rest are read when a run is
/// admitted rather than when the config is built: the concurrency limit, the validator, whether to
/// preprocess. They are carried on the binding so that the queue has them, and they are absent from
/// the config because the Python version's is.
/// </para>
/// <para>
/// Two of the Python version's parameters have no counterpart here, deliberately.
/// <c>inputs_kwargs</c> passes a component's value to a handler by its parameter's name, which is
/// something a Python signature can do and a .NET delegate cannot — the order in
/// <see cref="Inputs"/> is the whole of what a delegate receives, so a second way of naming the
/// same components would describe a distinction this side cannot make. <c>key</c> identifies a
/// binding across a dynamic re-render, and dynamic re-rendering is its own batch; it is absent for
/// the same reason <see cref="Block.Key"/> is declared but unwired.
/// </para>
/// </remarks>
internal sealed record EventBindingOptions
{
    /// <summary>
    /// The handler, or <see langword="null"/> for a binding that runs only in the frontend.
    /// </summary>
    /// <remarks>
    /// The typed binding methods take it as an ordinary argument because it is what they are
    /// about; the <c>object[]</c> fallback takes it here so that one signature carries every
    /// binding parameter. A <see langword="null"/> handler with <see cref="Js"/> set is a
    /// frontend-only binding, which is the Python version's <c>fn=None, js="..."</c>.
    /// </remarks>
    public Delegate? Fn { get; init; }

    /// <summary>The components the handler reads, in order.</summary>
    /// <remarks>
    /// The fallback spelling of <c>inputs</c>: the Python version takes a component, a list, a set
    /// or nothing, and the typed overloads turn their parameters into this list before they get
    /// here.
    /// </remarks>
    public IReadOnlyList<object> Inputs { get; init; } = [];

    /// <summary>The components the handler's result updates, in order.</summary>
    public IReadOnlyList<object> Outputs { get; init; } = [];

    /// <summary>
    /// The names the handler's inputs are called in the API documentation, in order.
    /// </summary>
    /// <value>
    /// <see langword="null"/> to name them after their position. The Python version names an input
    /// after the handler's own parameter, which a .NET delegate does not carry; naming them here is
    /// how an application that wants its endpoint called by name says so.
    /// </value>
    /// <remarks>
    /// The names are what a client sends and what the OpenAPI document's request body is keyed by,
    /// so they are part of the served documents rather than a display detail. An entry left
    /// <see langword="null"/> falls back to the position, so a partial list is meaningful.
    /// </remarks>
    public IReadOnlyList<string?>? ParameterNames { get; init; }

    /// <summary>Whether the input components preprocess their values before the handler runs.</summary>
    public bool Preprocess { get; init; } = true;

    /// <summary>Whether the output components postprocess the handler's result before it is sent.</summary>
    public bool Postprocess { get; init; } = true;

    /// <summary>Whether the page scrolls to the outputs when the run completes.</summary>
    public bool ScrollToOutput { get; init; }

    /// <summary>How much of the progress animation the run shows.</summary>
    /// <value>
    /// <see langword="null"/> to let the event decide: the Python version declares the default on
    /// the event itself, and <c>tick</c> is the one that hides the animation — a timer that fires
    /// every second would otherwise repaint a spinner every second. Everything else shows all of
    /// it.
    /// </value>
    public ProgressDisplay? ShowProgress { get; init; }

    /// <summary>
    /// The components the progress animation is drawn on, or <see langword="null"/> for all outputs.
    /// </summary>
    public IReadOnlyList<Block>? ShowProgressOn { get; init; }

    /// <summary>
    /// The name the endpoint is exposed under.
    /// </summary>
    /// <value>
    /// <see langword="null"/> to derive it from the handler — the Python version uses the handler's
    /// name, and "lambda" for one that has none.
    /// </value>
    public string? ApiName { get; init; }

    /// <summary>The description the endpoint carries in the config and in the API documentation.</summary>
    /// <remarks>
    /// The Python version's <c>api_description</c>. Left <see langword="null"/>, the config reports
    /// no description — which is what the Python version reports for a binding whose caller did not
    /// give one — and the API documentation falls back to <see cref="EndpointDescription"/>.
    /// </remarks>
    public string? ApiDescription { get; init; }

    /// <summary>
    /// The description the API documentation shows when <see cref="ApiDescription"/> is unset.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for none, which leaves the documentation's description empty.
    /// </value>
    /// <remarks>
    /// The Python version falls back to the handler's docstring here, which is why the two
    /// descriptions can differ: a handler with a docstring describes its endpoint without
    /// describing its binding. A .NET method's documentation is not in its compiled form, so an
    /// application that wants the documentation to say something gives it here.
    /// </remarks>
    public string? EndpointDescription { get; init; }

    /// <summary>
    /// Frontend JavaScript to run before the handler.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for none. A binding that sets it and names no handler runs entirely
    /// in the frontend, which is what the Python version does with <c>fn=None, js="..."</c>.
    /// </value>
    public string? Js { get; init; }

    /// <summary>
    /// Whether the binding's handler runs in the browser instead of on the server.
    /// </summary>
    /// <value>
    /// <see langword="false"/> unless the caller says otherwise. The Python version's
    /// <c>js=True</c>, which the config reports as <c>"js": true</c> rather than as a script.
    /// </value>
    /// <remarks>
    /// The Python version reaches this by transpiling the Python handler to JavaScript at launch. A
    /// .NET assembly cannot be transpiled, so the browser implementation is something the caller
    /// gives — see <see cref="JsImplementation"/> — and this flag is what says the implementation
    /// is the handler rather than a step before it.
    /// </remarks>
    public bool JsRunsInBrowser { get; init; }

    /// <summary>
    /// The browser implementation of the handler, or <see langword="null"/> for none.
    /// </summary>
    /// <remarks>
    /// The <c>js_implementation</c> the config carries for a binding whose <see cref="JsRunsInBrowser"/>
    /// is set: a JavaScript function that answers what the handler would have answered. It is what
    /// makes a frontend-only step — a property update that needs no round trip — actually happen in
    /// the browser.
    /// </remarks>
    public string? JsImplementation { get; init; }

    /// <summary>Whether the run goes through the queue.</summary>
    /// <remarks>
    /// A binding with no handler is never queued, whatever this says — there is nothing to run —
    /// which is the Python version's rule.
    /// </remarks>
    public bool Queue { get; init; } = true;

    /// <summary>Whether the handler takes a batch of inputs rather than one set.</summary>
    public bool Batch { get; init; }

    /// <summary>The largest batch handed to a batching handler.</summary>
    public int MaxBatchSize { get; init; } = 4;

    /// <summary>
    /// How many submissions are allowed while one is pending.
    /// </summary>
    /// <value>
    /// <see langword="null"/> to let the event decide: the Python version makes <c>change</c> and
    /// <c>key_up</c> <see cref="TriggerMode.AlwaysLast"/>, <c>stream</c>
    /// <see cref="TriggerMode.Multiple"/>, and everything else <see cref="TriggerMode.Once"/>.
    /// </value>
    public TriggerMode? TriggerMode { get; init; }

    /// <summary>How many runs of this binding may be in flight at once.</summary>
    public ConcurrencyLimit ConcurrencyLimit { get; init; } = ConcurrencyLimit.ApplicationDefault;

    /// <summary>The concurrency group this binding belongs to, or <see langword="null"/> for none.</summary>
    /// <remarks>Bindings in one group are limited by the lowest limit set among them.</remarks>
    public string? ConcurrencyId { get; init; }

    /// <summary>How far the endpoint is exposed to clients.</summary>
    public ApiVisibility ApiVisibility { get; init; } = ApiVisibility.Public;

    /// <summary>The time limit for the run, in seconds, or <see langword="null"/> for none.</summary>
    public double? TimeLimit { get; init; }

    /// <summary>How often streamed chunks are sent, in seconds.</summary>
    public double StreamEvery { get; init; } = 0.5;

    /// <summary>
    /// The bindings this one cancels when it starts.
    /// </summary>
    /// <remarks>
    /// The Python version also registers a private binding that performs the cancellation; this
    /// framework does the same, so the served dependencies are the same shape.
    /// </remarks>
    public IReadOnlyList<Dependency>? Cancels { get; init; }

    /// <summary>
    /// A function run before the handler, to reject input.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for none. It receives the same inputs as the handler and runs without
    /// the queue, which is the Python version's rule.
    /// </value>
    public Delegate? Validator { get; init; }
}

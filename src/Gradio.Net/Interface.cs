using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// What shape an <see cref="Interface"/> has, as the Python version's <c>InterfaceTypes</c> names
/// it.
/// </summary>
/// <remarks>
/// The shape is not a setting: it is read off the components the application gave — nothing on one
/// side is an input-only interface, nothing on the other an output-only one, the same components on
/// both a unified one — and it decides which columns are drawn, which buttons exist, and whether
/// flagging is offered at all.
/// </remarks>
public enum InterfaceType
{
    /// <summary>Inputs on the left, outputs on the right — the shape most applications have.</summary>
    Standard,

    /// <summary>Inputs only: a form that does something rather than showing a result.</summary>
    InputOnly,

    /// <summary>Outputs only: a page that generates rather than being filled in.</summary>
    OutputOnly,

    /// <summary>
    /// The same components on both sides, so a result is edited in place.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>UNIFIED</c>: one column, one set of components, and the result
    /// written back into the components that produced it. It is only reachable when the input and
    /// output lists are the same instances in the same order.
    /// </remarks>
    Unified,
}

/// <summary>
/// A whole application in one call: a handler, the components it reads, and the components it writes,
/// wrapped in the layout, the buttons and the behaviours an application needs to be usable
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gr.Interface</c>, and it is the short way to build an
/// application: the same thing <see cref="Blocks"/> does by hand, with the title, the columns, the
/// submit and clear buttons, the flag buttons, the example gallery, the deep link and the API
/// endpoint already wired. Everything it renders is an ordinary block, so an application that
/// outgrows it can be rebuilt with <see cref="Blocks"/> and will describe itself the same way.
/// </para>
/// <para>
/// It renders into an application rather than being one. The Python version's <c>Interface</c> is a
/// <c>Blocks</c> subclass because Python's <c>Blocks</c> is the render root; here the render root
/// is resolved from the container — one container hosts one application — so the interface is
/// handed the application to render into, the way <see cref="Examples"/> is.
/// </para>
/// <para>
/// The components are given ids before the layout that holds them, which is the Python version's
/// numbering: a Python component is numbered when it is constructed, and an <c>Interface</c>'s
/// components are constructed before its layout. Getting that order wrong would number every block
/// differently and the served config would describe a different application.
/// </para>
/// </remarks>
public sealed class Interface
{
    private const string InputColumnUpdate = "() => ";

    /// <summary>The indentation the Python version's clear-column script ends with, reproduced as
    /// it is served.</summary>
    private const string ScriptTail = "\n            ";

    /// <summary>The browser implementation of "the deep link button is now usable".</summary>
    /// <remarks>
    /// The Python version transpiles the Python handler that produces this update; a .NET assembly
    /// cannot be transpiled, so the implementation is written out. It is the transpiler's own
    /// output, down to its spacing, because the script is part of what the browser is handed.
    /// </remarks>
    private const string DeepLinkReadyScript =
        "function () {\n    return {\"interactive\": true, \"__type__\": \"update\"};\n}";

    private readonly Blocks _blocks;
    private readonly List<Component> _mainInputs;
    private readonly List<Component> _additionalInputs;
    private readonly List<Component> _outputs;
    private readonly List<Component> _flagComponents;
    private readonly List<(string Label, string? Value)> _flaggingOptions;
    private readonly FlaggingCallback _flaggingCallback;
    private readonly Delegate _fn;
    private readonly InterfaceType _type;
    private readonly bool _batch;
    private readonly int _maxBatchSize;
    private readonly ConcurrencyLimit _concurrencyLimit;
    private readonly ApiVisibility _apiVisibility;
    private readonly string? _apiName;
    private readonly string? _apiDescription;
    private readonly string? _apiDocstring;
    private readonly ProgressDisplay _showProgress;
    private readonly Delegate? _validator;
    private readonly IReadOnlyList<string>? _parameterNames;
    private readonly bool _live;
    private readonly int? _timeLimit;
    private readonly double _streamEvery;
    private readonly bool _stopOnGenerators;
    private readonly string? _title;
    private readonly string? _description;
    private readonly string? _article;
    private readonly object? _examples;
    private readonly bool? _cacheExamples;
    private readonly string? _cacheMode;
    private readonly int _examplesPerPage;
    private readonly IReadOnlyList<string>? _exampleLabels;
    private readonly int? _preloadExample;
    private readonly ButtonSpec? _submitButtonSpec;
    private readonly ButtonSpec? _stopButtonSpec;
    private readonly ButtonSpec? _clearButtonSpec;
    private readonly object? _accordionSpec;
    private readonly object? _deepLinkSpec;
    private readonly bool _allowDuplication;

    private Column? _inputColumn;
    private DeepLinkButton? _deepLink;

    /// <summary>Builds an application and renders it into <paramref name="blocks"/>.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="fn">The handler: one parameter per input, and a result per output.</param>
    /// <param name="inputs">
    /// The components the handler reads, as components or as the names of components, or
    /// <see langword="null"/> for none. See <see cref="ComponentCollection"/>.
    /// </param>
    /// <param name="outputs">
    /// The components the handler's result fills, written the same way as
    /// <paramref name="inputs"/>, or <see langword="null"/> for none.
    /// </param>
    /// <param name="examples">
    /// Sample inputs, shown under the fields: one entry per sample, each holding one value per input.
    /// A directory path is read as a <c>log.csv</c> inside it. See <see cref="Examples"/>.
    /// </param>
    /// <param name="cacheExamples">
    /// Whether the answers to the samples are worked out and kept, or <see langword="null"/> to read
    /// <c>GRADIO_CACHE_EXAMPLES</c>.
    /// </param>
    /// <param name="cacheMode">
    /// <c>eager</c> to work every answer out at startup, <c>lazy</c> to work each one out when it is
    /// first asked for, or <see langword="null"/> to read <c>GRADIO_CACHE_MODE</c>.
    /// </param>
    /// <param name="examplesPerPage">How many samples one page of the gallery holds.</param>
    /// <param name="exampleLabels">The labels shown instead of the samples, or <see langword="null"/> for none.</param>
    /// <param name="preloadExample">
    /// The sample to load when the page opens, or <see langword="null"/> for none — the Python
    /// version's <c>False</c>. Only a sample whose answer is cached eagerly is preloaded.
    /// </param>
    /// <param name="live">Whether changing an input runs the handler, rather than the submit button.</param>
    /// <param name="title">The heading drawn above the application, or <see langword="null"/> for none.</param>
    /// <param name="description">The text drawn under the heading, or <see langword="null"/> for none.</param>
    /// <param name="article">The text drawn under the components, or <see langword="null"/> for none.</param>
    /// <param name="flaggingMode">
    /// <c>never</c>, <c>auto</c> or <c>manual</c>, or <see langword="null"/> to read
    /// <c>GRADIO_FLAGGING_MODE</c> — which defaults to <c>manual</c>.
    /// </param>
    /// <param name="flaggingOptions">
    /// The options a user may flag a sample with: a list of strings, which become "Flag as …"
    /// buttons, or of label/value pairs. <see langword="null"/> for the one unnamed option.
    /// </param>
    /// <param name="flaggingDir">The directory the flagged samples are written into.</param>
    /// <param name="flaggingCallback">
    /// What writes the samples, or <see langword="null"/> for a <see cref="CsvLogger"/>.
    /// </param>
    /// <param name="batch">Whether the handler takes a batch of samples rather than one.</param>
    /// <param name="maxBatchSize">The largest batch handed to a batching handler.</param>
    /// <param name="apiVisibility">How far the prediction endpoint is exposed.</param>
    /// <param name="apiName">
    /// The name the prediction endpoint is exposed under, or <see langword="null"/> for the handler's
    /// own name.
    /// </param>
    /// <param name="apiDescription">
    /// The description the endpoint carries, or <see langword="null"/> for none.
    /// </param>
    /// <param name="apiDocstring">
    /// The description the API documentation shows when <paramref name="apiDescription"/> is
    /// <see langword="null"/>, which is the Python version's reading of the handler's docstring, or
    /// <see langword="null"/> for none.
    /// </param>
    /// <param name="allowDuplication">Whether the duplicate-Space button is drawn.</param>
    /// <param name="concurrencyLimit">How many runs of the handler may be in flight at once.</param>
    /// <param name="additionalInputs">
    /// The components drawn in an accordion under the main inputs, written the same way as
    /// <paramref name="inputs"/>, or <see langword="null"/> for none. They are handed to the handler
    /// after the main inputs, in the order they are given.
    /// </param>
    /// <param name="additionalInputsAccordion">
    /// The accordion those components are drawn in: a string label, or an <see cref="Accordion"/> to
    /// configure it further. <see langword="null"/> for "Additional Inputs", closed.
    /// </param>
    /// <param name="submitBtn">
    /// The submit button: a string label or a <see cref="Button"/>. <see langword="null"/> for the
    /// default, which says "Submit" in the primary variant.
    /// </param>
    /// <param name="stopBtn">The stop button, as a string or a <see cref="Button"/>.</param>
    /// <param name="clearBtn">
    /// The clear button, as a string or a <see cref="Button"/>, or <see langword="null"/> to hide it.
    /// </param>
    /// <param name="deleteCache">
    /// How often to reclaim the application's temporary files, and how old a file has to be, or
    /// <see langword="null"/> to keep them.
    /// </param>
    /// <param name="showProgress">How much of the progress animation a run shows.</param>
    /// <param name="fillWidth">Whether the application fills the viewport's width.</param>
    /// <param name="fillHeight">Whether the application fills the viewport's height.</param>
    /// <param name="timeLimit">
    /// The time limit for a streamed run, in seconds, or <see langword="null"/> for none.
    /// </param>
    /// <param name="streamEvery">How often streamed chunks are sent, in seconds.</param>
    /// <param name="deepLink">
    /// The share button: <see langword="true"/> for one that says "Share via Link", a string for one
    /// that says something else, a <see cref="DeepLinkButton"/> to configure it, or
    /// <see langword="null"/> for no button.
    /// </param>
    /// <param name="validator">
    /// A function that receives the same inputs as the handler and rejects what it does not accept,
    /// or <see langword="null"/> for none. A rejection is answered with a 422 and the message.
    /// </param>
    /// <param name="parameterNames">
    /// The names the inputs are called in the API document — one per input, in order — or
    /// <see langword="null"/> to take them from the handler's own parameters.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> or <paramref name="fn"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Neither inputs nor outputs were given, a state appears more than once on a side, an entry of
    /// a side is neither a component nor the name of one, or one of the string-or-instance
    /// parameters is neither.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The parameters are the Python version's. The shapes a Python signature can express and a C#
    /// one cannot are spelled two ways: where the alternatives are a known few, as a type that
    /// converts from each — <see cref="ButtonSpec"/> for a label <em>or</em> a button,
    /// <see cref="FlaggingOption"/> for a name <em>or</em> a pair,
    /// <see cref="ComponentCollection"/> for a component <em>or</em> the name of one <em>or</em> a
    /// list of either — and where the alternatives are open, as <see cref="object"/> checked here,
    /// which is what the accordion's label-or-accordion and the share button's four shapes do.
    /// </para>
    /// <para>
    /// One parameter is not in that list: <paramref name="fillHeight"/> is not one of the Python
    /// version's documented interface parameters, where it arrives through the <c>**kwargs</c> that
    /// <c>Interface</c> hands to <c>Blocks</c> and lands on the application as its
    /// <c>fill_height</c>. It is named here because it is part of what an interface says about
    /// itself — a tabbed interface reads it to decide how much of the height a tab gets — and
    /// setting it on the application behind the interface's back would be the only other way to say
    /// it.
    /// </para>
    /// <para>
    /// An interface is rendered once, when it is constructed: the application is built by the call, so
    /// a second one would describe a second application.
    /// </para>
    /// <para>
    /// One of the Python version's unions has no second half here: <c>api_description</c> takes a
    /// string, <c>None</c> to fall back to the handler's docstring, or <c>False</c> for no
    /// description. A delegate carries no docstring to read at run time, so <see langword="null"/>
    /// is the only "nothing" there is, and it means what <c>False</c> means in the Python version. An
    /// application that wants what the Python version's docstring would have said gives it as
    /// <paramref name="apiDocstring"/>: the config then carries no description, which is what the
    /// Python version's carries, and the API document carries the text.
    /// </para>
    /// </remarks>
    internal Interface(
        Blocks blocks,
        Delegate fn,
        ComponentCollection? inputs = null,
        ComponentCollection? outputs = null,
        object? examples = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        IReadOnlyList<string>? exampleLabels = null,
        int? preloadExample = 0,
        bool live = false,
        string? title = null,
        string? description = null,
        string? article = null,
        string? flaggingMode = null,
        IReadOnlyList<FlaggingOption>? flaggingOptions = null,
        string flaggingDir = ".gradio/flagged",
        FlaggingCallback? flaggingCallback = null,
        bool batch = false,
        int maxBatchSize = 4,
        ApiVisibility apiVisibility = ApiVisibility.Public,
        string? apiName = null,
        string? apiDescription = null,
        string? apiDocstring = null,
        bool allowDuplication = false,
        ConcurrencyLimit concurrencyLimit = default,
        ComponentCollection? additionalInputs = null,
        object? additionalInputsAccordion = null,
        ButtonSpec? submitBtn = null,
        ButtonSpec? stopBtn = null,
        ButtonSpec? clearBtn = null,
        DeleteCacheSettings? deleteCache = null,
        ProgressDisplay showProgress = ProgressDisplay.Full,
        bool fillWidth = false,
        bool fillHeight = false,
        int? timeLimit = 30,
        double streamEvery = 0.5,
        object? deepLink = null,
        Delegate? validator = null,
        IReadOnlyList<string>? parameterNames = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(fn);

        _blocks = blocks;
        _fn = fn;
        _batch = batch;
        _maxBatchSize = maxBatchSize;
        _concurrencyLimit = concurrencyLimit;
        _apiVisibility = apiVisibility;
        _apiName = apiName;
        _apiDescription = apiDescription;
        _apiDocstring = apiDocstring;
        _showProgress = showProgress;
        _validator = validator;
        _parameterNames = parameterNames;
        _live = live;
        _timeLimit = timeLimit;
        _streamEvery = streamEvery;
        _title = title;
        _description = description;
        _article = article;
        _examples = examples;
        _cacheExamples = cacheExamples;
        _cacheMode = cacheMode;
        _examplesPerPage = examplesPerPage;
        _exampleLabels = exampleLabels;
        _preloadExample = preloadExample;
        _allowDuplication = allowDuplication;
        _submitButtonSpec = submitBtn;
        _stopButtonSpec = stopBtn;
        _clearButtonSpec = clearBtn;
        _accordionSpec = additionalInputsAccordion;
        _deepLinkSpec = deepLink;
        _stopOnGenerators = StreamsResult(fn) && !live;

        _mainInputs = [.. inputs?.Components ?? []];
        _additionalInputs = [.. additionalInputs?.Components ?? []];
        _outputs = [.. outputs?.Components ?? []];

        _type = TypeOf(_mainInputs, _additionalInputs, _outputs);

        // A state is one value with two faces: an input the handler reads and an output it writes.
        // The Python version insists there is at most one of each and makes them the same component, which is
        // what lets a handler carry something from one run to the next.
        if ((StateIn(_mainInputs) ?? StateIn(_additionalInputs)) is { } state)
        {
            Replace(_mainInputs, state);
            Replace(_additionalInputs, state);
            Replace(_outputs, state);
            CheckStateCount(_mainInputs, _additionalInputs, _outputs);

            // A cached answer is worked out once and replayed, so a state that changes between runs
            // has nothing to replay. The Python version turns caching off rather than serving a wrong answer.
            _cacheExamples = false;
        }
        else
        {
            CheckStateCount(_mainInputs, _additionalInputs, _outputs);
        }

        if (_type is InterfaceType.Standard or InterfaceType.OutputOnly)
        {
            foreach (var output in _outputs)
            {
                // An output the application did not say is editable is drawn read-only: it shows what
                // the handler produced, and typing into it would be editing a result.
                output.Interactive ??= false;
            }
        }

        LabelComponents();

        _flaggingOptions = FlaggingOptionsOf(flaggingOptions);
        _flaggingCallback = flaggingCallback ?? new CsvLogger();
        FlaggingMode = FlaggingModeOf(flaggingMode);
        _flagComponents = _type == InterfaceType.Unified
            ? [.. InputComponents]
            : [.. InputComponents, .. _outputs];

        // The interface owns the application's own settings: what it is, what it is called, and how
        // it is sized are properties of the application rather than of the blocks in it.
        blocks.Mode = "interface";

        // The columns the interface was given, handed to the application because the Python version's
        // Interface is a Blocks and keeps them on the object it is. What reads them is the launch
        // report: an interface describes its own inputs and outputs, where a page of blocks can only
        // describe the components its bindings name.
        blocks.InputComponents = InputComponents;
        blocks.OutputComponents = _outputs;

        blocks.Title = title ?? "Gradio";
        // "No description" is a value here, as it is for a chat: the API document serves null for
        // an application that was given none, and an empty string for one that was never asked.
        blocks.Description = description;
        blocks.FillWidth = fillWidth;

        // The Python version's interface has no documented fill_height: it reaches the application through
        // the **kwargs that Interface hands to Blocks. It is a parameter here because a tab reads
        // it — a tabbed interface gives a tab a scale of 1 when its content fills the height — and
        // an application that fills the height should say so where it is built rather than behind
        // the interface's back.
        blocks.FillHeight = fillHeight;

        if (deleteCache is not null)
        {
            blocks.DeleteCache = deleteCache;
        }

        if (FlaggingMode != "never" && _type != InterfaceType.InputOnly)
        {
            _flaggingCallback.Setup(_flagComponents, flaggingDir);
        }

        Render();
    }

    /// <summary>The shape this interface was built in.</summary>
    public InterfaceType Type => _type;

    /// <summary>The components the handler reads, in the order they are given to it.</summary>
    /// <remarks>States included, because a state is one of the values the handler reads.</remarks>
    public IReadOnlyList<Component> Inputs => _mainInputs;

    /// <summary>The components drawn in the additional-inputs accordion.</summary>
    public IReadOnlyList<Component> AdditionalInputs => _additionalInputs;

    /// <summary>The components the handler's result fills.</summary>
    public IReadOnlyList<Component> Outputs => _outputs;

    /// <summary>Whether flagging is offered: <c>never</c>, <c>auto</c> or <c>manual</c>.</summary>
    public string FlaggingMode { get; }

    /// <summary>The submit button, or <see langword="null"/> when the interface is live.</summary>
    public Button? SubmitButton { get; private set; }

    /// <summary>The stop button, or <see langword="null"/> when the handler does not stream.</summary>
    public Button? StopButton { get; private set; }

    /// <summary>The clear button, or <see langword="null"/> when the interface draws none.</summary>
    /// <remarks>
    /// An interface the application hid the button on still has one: the Python version draws it
    /// with <c>visible: false</c>, because the bindings that empty the fields hang off it either
    /// way.
    /// </remarks>
    public ClearButton? ClearButton { get; private set; }

    /// <summary>The flag buttons, in the order the options were given.</summary>
    public IReadOnlyList<Button> FlagButtons { get; private set; } = [];

    /// <summary>The share button, or <see langword="null"/> when the application asked for none.</summary>
    public DeepLinkButton? ShareButton => _deepLink;

    /// <summary>The example gallery, or <see langword="null"/> when the application gave no samples.</summary>
    public Examples? Examples { get; private set; }

    /// <summary>The binding the submit button and the fields share, or <see langword="null"/>.</summary>
    /// <remarks>
    /// The binding a run of the handler goes through: the one that carries the endpoint's name,
    /// visibility and validator. An interface that streams and has a stop button returns the step
    /// that puts the buttons back, which is the Python version's answer — the prediction is in the
    /// middle of the chain rather than at its end.
    /// </remarks>
    public Dependency? SubmitEvent { get; private set; }

    /// <summary>All the components the handler reads, main and additional, in order.</summary>
    private IReadOnlyList<Component> InputComponents => [.. _mainInputs, .. _additionalInputs];

    /// <summary>Whether the handler's result is a stream, which is what decides the stop button.</summary>
    /// <param name="fn">The handler.</param>
    /// <returns>Whether it answers with an <see cref="IAsyncEnumerable{T}"/>.</returns>
    private static bool StreamsResult(Delegate fn) => Dependency.StreamsResult(fn);

    /// <summary>Renders the application, in the Python version's order.</summary>
    /// <remarks>
    /// The order is the whole of the layout: the deep link binds first because the Python version
    /// activates it before anything is drawn, the buttons bind in the order the columns are built,
    /// and the flag buttons, the deep-link step and the examples come after — each of which the
    /// config numbers.
    /// </remarks>
    private void Render()
    {
        using var scope = _blocks.IsRendering ? null : _blocks.BeginRender();

        ReserveComponents();

        _deepLink?.Activate();

        RenderTitleAndDescription();

        var row = _blocks.Add(new Row());

        using (_blocks.Enter(row))
        {
            if (_type is InterfaceType.Standard or InterfaceType.InputOnly or InterfaceType.Unified)
            {
                RenderInputColumn();
            }

            if (_type is InterfaceType.Standard or InterfaceType.OutputOnly)
            {
                RenderOutputColumn();
            }
        }

        SubmitEvent = AttachSubmitEvents();
        AttachClearEvents();
        AttachFlaggingEvents();

        if (SubmitEvent is not null && _deepLink is not null)
        {
            _blocks.On(
                [SubmitEvent.Events.Then],
                fn: (Func<object?>)(() => new DeepLinkButton { Interactive = true }), outputs: [_deepLink], jsRunsInBrowser: true, jsImplementation: DeepLinkReadyScript, apiVisibility: ApiVisibility.Undocumented);
        }

        RenderExamples();
        RenderArticle();

        // The Python version works an eager gallery's answers out while the application is starting, and
        // launch does not return until it has. Building the interface is that moment here: the
        // application is written before the host listens, so nothing can ask for a sample that is
        // still being computed.
        if (Examples is { CacheExamples: true, CacheMode: "eager" })
        {
            Examples.CacheAsync().GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Gives every component the application handed over its id, before the layout that holds them.
    /// </summary>
    /// <remarks>
    /// This is what makes the numbering the Python version's. A Python component is numbered when
    /// it is constructed and an interface's components are constructed before its layout, so the
    /// ids run components first and containers after; the components are then placed where they
    /// belong, one column at a time. Reserving them here rather than adding them in place is the
    /// difference between a config that matches the Python version's and one whose every id is
    /// shifted.
    /// </remarks>
    private void ReserveComponents()
    {
        _deepLink = DeepLinkButtonOf();
        if (_deepLink is not null)
        {
            _blocks.Reserve(_deepLink);
        }

        // The additional inputs are numbered before the main ones, because the Python version constructs them
        // first: the accordion's contents are read out of the arguments before the columns are.
        //
        // A component that is on both sides — a unified interface, or the state that both sides
        // share — is one component and is numbered once.
        var numbered = new HashSet<Component>(ReferenceEqualityComparer.Instance);

        foreach (var component in _additionalInputs.Concat(_mainInputs).Concat(_outputs))
        {
            if (numbered.Add(component))
            {
                _blocks.Reserve(component);
            }
        }
    }

    /// <summary>The share button the application asked for, or <see langword="null"/> for none.</summary>
    /// <returns>The button, not yet rendered.</returns>
    /// <exception cref="ArgumentException">The value is not one of the shapes a share button takes.</exception>
    /// <remarks>
    /// The Python version also draws one on a Space without being asked, which is a platform it
    /// does not serve here: an application that wants the button says so.
    /// </remarks>
    private DeepLinkButton? DeepLinkButtonOf()
    {
        var button = _deepLinkSpec switch
        {
            null or false => null,
            true => new DeepLinkButton(),
            string label => new DeepLinkButton { Value = label },
            DeepLinkButton given => given,
            _ => throw new ArgumentException(
                "The share button is a string, a DeepLinkButton, or true; got "
                + $"{_deepLinkSpec.GetType().Name}.",
                "deepLink"),
        };

        if (button is null)
        {
            return null;
        }

        // The Python version's share button starts unusable: there is no state worth sharing until a run has
        // produced one, and a step after the run is what makes it usable.
        button.Interactive = false;

        return button;
    }

    /// <summary>The heading and the description, drawn above the columns.</summary>
    private void RenderTitleAndDescription()
    {
        if (!string.IsNullOrEmpty(_title))
        {
            _blocks.Add(new Markdown
            {
                Value = $"<h1 style='text-align: center; margin-bottom: 1rem'>{_title}</h1>",
            });
        }

        if (!string.IsNullOrEmpty(_description))
        {
            _blocks.Add(new Markdown { Value = _description });
        }
    }

    /// <summary>The left-hand column: the fields, the additional inputs, and the buttons under them.</summary>
    private void RenderInputColumn()
    {
        var column = _blocks.Add(new Column());

        using (_blocks.Enter(column))
        {
            _inputColumn = _blocks.Add(new Column());

            using (_blocks.Enter(_inputColumn))
            {
                foreach (var component in _mainInputs)
                {
                    _blocks.Attach(component);
                }

                if (_additionalInputs.Count > 0)
                {
                    var accordion = _blocks.Add(AccordionOf());

                    using (_blocks.Enter(accordion))
                    {
                        foreach (var component in _additionalInputs)
                        {
                            _blocks.Attach(component);
                        }
                    }
                }
            }

            var row = _blocks.Add(new Row());

            using (_blocks.Enter(row))
            {
                switch (_type)
                {
                    case InterfaceType.Standard or InterfaceType.InputOnly:
                        ClearButton = _blocks.Add(ClearButtonOf());

                        if (!_live)
                        {
                            if (_type == InterfaceType.InputOnly && _deepLink is not null)
                            {
                                _blocks.Attach(_deepLink);
                            }

                            SubmitButton = _blocks.Add(ButtonOf(_submitButtonSpec, "Submit", "primary"));
                            AddStopButton();
                        }

                        break;

                    case InterfaceType.Unified:
                        ClearButton = _blocks.Add(ClearButtonOf());
                        SubmitButton = _blocks.Add(ButtonOf(_submitButtonSpec, "Submit", "primary"));

                        if (_deepLink is not null)
                        {
                            _blocks.Attach(_deepLink);
                        }

                        AddStopButton();
                        RenderFlagButtons(SubmitButton);

                        break;
                }
            }
        }
    }

    /// <summary>The right-hand column: the results, and the buttons under them.</summary>
    private void RenderOutputColumn()
    {
        var column = _blocks.Add(new Column());

        using (_blocks.Enter(column))
        {
            foreach (var component in _outputs)
            {
                if (component is not State)
                {
                    _blocks.Attach(component);
                }
            }

            var row = _blocks.Add(new Row());

            using (_blocks.Enter(row))
            {
                if (_deepLink is not null)
                {
                    _blocks.Attach(_deepLink);
                }

                if (_type == InterfaceType.OutputOnly)
                {
                    ClearButton = _blocks.Add(ClearButtonOf());

                    // An interface with nothing to fill in says "Generate" rather than "Submit":
                    // there is no form to submit.
                    SubmitButton = _blocks.Add(ButtonOf(_submitButtonSpec, "Generate", "primary"));
                    AddStopButton();
                }

                RenderFlagButtons(SubmitButton);

                if (_allowDuplication)
                {
                    var duplicate = _blocks.Add(new DuplicateButton
                    {
                        Scale = 1,
                        Size = "lg",
                        AutoActivate = false,
                    });

                    duplicate.Activate();
                }
            }
        }
    }

    /// <summary>Draws the flag buttons, when the application asked for them.</summary>
    /// <param name="submit">The submit button, which is the flag button of an automatic interface.</param>
    /// <remarks>
    /// Flagging a sample automatically has no button of its own: the submit button is what records
    /// what the user just sent, which is why the Python version hands the same button to both jobs.
    /// </remarks>
    private void RenderFlagButtons(Button? submit)
    {
        if (FlaggingMode == "manual")
        {
            FlagButtons =
            [
                .. _flaggingOptions.Select(option => _blocks.Add(new Button { Value = option.Label })),
            ];
        }
        else if (FlaggingMode == "auto" && submit is not null)
        {
            FlagButtons = [submit];
        }
    }

    /// <summary>Adds the stop button, when the handler streams and the interface is not live.</summary>
    private void AddStopButton()
    {
        if (_stopOnGenerators)
        {
            StopButton = _blocks.Add(ButtonOf(_stopButtonSpec, "Stop", "stop", visible: false));
        }
    }

    /// <summary>Binds the handler, to the submit button and the fields that can be submitted.</summary>
    /// <returns>The binding, which is the step a deep link and automatic flagging hang off.</returns>
    private Dependency AttachSubmitEvents()
    {
        if (_live)
        {
            return AttachLiveEvents();
        }

        var submit = SubmitButton ?? throw new InvalidOperationException(
            "An interface that is not live submits with its submit button, and this one has none.");

        var triggers = new List<EventTrigger> { submit.Events.Click };

        // A field that can be submitted is submitted by Enter as well as by the button, which is
        // what makes a form feel like a form.
        foreach (var component in InputComponents)
        {
            if (component.Events.Submit is { } field)
            {
                triggers.Add(field);
            }
        }

        if (triggers.Count == 0)
        {
            throw new InvalidOperationException(
                "An interface that is not live needs something to submit with: a submit button, or an "
                + "input component that raises submit.");
        }

        if (StopButton is not { } stop)
        {
            return _blocks.On(triggers, PredictOptions(scrollToOutput: true));
        }

        // A run that streams is interruptible, and interrupting it is two button updates around the
        // handler: the submit button goes away, the stop button appears, and the reverse when the run
        // ends however it ends.
        var predict = _blocks.On(
            triggers,
            fn: ButtonsAfterStart(), outputs: [submit, stop], queue: false, apiVisibility: ApiVisibility.Undocumented);

        var final = _blocks.On(
            [predict.Events.Then],
            PredictOptions(scrollToOutput: true));

        _blocks.On(
            [final.Events.Then],
            new EventBindingOptions
            {
                Fn = ButtonsAfterStop(),

                // Named, because the Python version's cleanup is a named function and the config carries the
                // name: the stop button binds the same one, and the second is served as "cleanup_1".
                ApiName = CleanupName,
                Outputs = [submit, stop],
                Queue = false,
                ApiVisibility = ApiVisibility.Undocumented,
            });

        _blocks.On(
            [stop.Events.Click],
            fn: ButtonsAfterStop(), apiName: CleanupName, outputs: [submit, stop], cancels: [predict], queue: false, apiVisibility: ApiVisibility.Undocumented);

        return final;
    }

    /// <summary>The name the Python version's cleanup handler is served under.</summary>
    private const string CleanupName = "cleanup";

    /// <summary>The update that starts a streamed run: the submit button away, the stop button out.</summary>
    /// <returns>A handler answering with both buttons' new state.</returns>
    private static Func<object?> ButtonsAfterStart() =>
        () => (new Button { Visible = false }, new Button { Visible = true });

    /// <summary>The update that ends a streamed run, however it ended.</summary>
    /// <returns>A handler answering with both buttons' new state.</returns>
    private static Func<object?> ButtonsAfterStop() =>
        () => (new Button { Visible = true }, new Button { Visible = false });

    /// <summary>Binds the handler to the fields' changes, for an interface that runs as they change.</summary>
    /// <returns>The binding.</returns>
    /// <remarks>
    /// A live interface has no submit button to press: the fields' own signals are the trigger, and
    /// the ones that produce a value a chunk at a time are the ones that stream.
    /// </remarks>
    private Dependency AttachLiveEvents()
    {
        if (_type == InterfaceType.OutputOnly)
        {
            _blocks.Load(_fn, null, _outputs);

            return _blocks.On(
                [SubmitButton!.Events.Click],
                fn: _fn, inputs: [.. InputComponents], outputs: [.. _outputs], parameterNames: InputParameterNames(), apiName: _apiName, apiVisibility: _apiVisibility, apiDescription: _apiDescription, endpointDescription: _apiDocstring, batch: _batch, maxBatchSize: _maxBatchSize, validator: _validator);
        }

        var triggers = new List<EventTrigger>();
        var streaming = false;

        foreach (var component in InputComponents)
        {
            if (component.Events.Stream is { } stream && component.Streaming)
            {
                triggers.Add(stream);
                streaming = true;
            }
            else
            {
                // Every component raises `change`, so the trigger is built here rather than through
                // a per-component member: the Python version asks the same question with `has_event("change")`
                // and gets the same answer for all of them.
                triggers.Add(new EventTrigger(component, EventNames.Change));
            }
        }

        return _blocks.On(
            triggers,
            new EventBindingOptions
            {
                Fn = _fn,
                Inputs = [.. InputComponents],
                Outputs = [.. _outputs],
                ParameterNames = InputParameterNames(),
                ApiName = _apiName,
                ApiVisibility = _apiVisibility,
                ApiDescription = _apiDescription,
                EndpointDescription = _apiDocstring,
                Batch = _batch,
                MaxBatchSize = _maxBatchSize,

                // A stream that repainted a spinner for every chunk would show nothing else, and a
                // run that is fed a chunk at a time must not be queued behind itself.
                ShowProgress = streaming ? ProgressDisplay.Hidden : _showProgress,
                TriggerMode = streaming ? TriggerMode.Multiple : TriggerMode.AlwaysLast,
                TimeLimit = _timeLimit,
                StreamEvery = _streamEvery,
                Validator = _validator,
            });
    }

    /// <summary>The binding parameters a prediction carries.</summary>
    /// <param name="scrollToOutput">Whether the page scrolls to the results when the run ends.</param>
    /// <returns>The options.</returns>
    private EventBindingOptions PredictOptions(bool scrollToOutput) => new()
    {
        Fn = _fn,
        Inputs = [.. InputComponents],
        Outputs = [.. _outputs],
        ParameterNames = InputParameterNames(),
        ApiName = _apiName,
        ApiVisibility = _apiVisibility,
        ApiDescription = _apiDescription,
        EndpointDescription = _apiDocstring,
        ScrollToOutput = scrollToOutput,
        Batch = _batch,
        MaxBatchSize = _maxBatchSize,
        ConcurrencyLimit = _concurrencyLimit,
        ShowProgress = _showProgress,
        Validator = _validator,
    };

    /// <summary>Binds the clear button: the fields it empties, and the column it resets.</summary>
    private void AttachClearEvents()
    {
        if (ClearButton is not { } clear)
        {
            return;
        }

        clear.Add([.. InputComponents, .. _outputs]);

        // The input column is a block that can be updated like any other, and the Python version resets its
        // width when the fields are cleared: an interface that had made room for a result gets its
        // half of the page back. The script is the Python version's, down to its trailing indentation.
        var updates = _type is InterfaceType.Standard or InterfaceType.InputOnly or InterfaceType.Unified
            ? new JsonArray(new JsonObject
            {
                ["variant"] = null,
                ["visible"] = true,
                ["__type__"] = "update",
            })
            : [];

        clear.Click(
            fn: null,
            inputs: [],
            outputs: _inputColumn is null ? [] : [_inputColumn],
             js: InputColumnUpdate + PythonJson.Dump(updates) + ScriptTail, queue: false);
    }

    /// <summary>Binds the flag buttons, when the interface offers them.</summary>
    /// <remarks>
    /// Each flag button is three bindings: one that says the sample is being saved, one that records
    /// it, and one on the clear button that puts the first back if the click never finished.
    /// </remarks>
    private void AttachFlaggingEvents()
    {
        if (FlagButtons.Count == 0 || _type == InterfaceType.InputOnly)
        {
            return;
        }

        if (FlaggingMode == "auto")
        {
            var automatic = new FlagMethod(_flaggingCallback, string.Empty, null, visualFeedback: false);

            _blocks.On(
                [SubmitEvent!.Events.Success],
                fn: automatic.Binding(_flagComponents.Count), inputs: [.. _flagComponents], outputs: [], preprocess: false, queue: false, apiVisibility: ApiVisibility.Undocumented);

            return;
        }

        if (ClearButton is not { } clear)
        {
            return;
        }

        for (var index = 0; index < FlagButtons.Count && index < _flaggingOptions.Count; index++)
        {
            var button = FlagButtons[index];
            var (label, value) = _flaggingOptions[index];
            var flagMethod = new FlagMethod(_flaggingCallback, label, value);

            button.Click(
                (Func<object?>)(() => new Button { Value = "Saving...", Interactive = false }),
                inputs: [],
                outputs: [button],
                 queue: false, apiVisibility: ApiVisibility.Undocumented);

            button.Click(
                flagMethod.Binding(_flagComponents.Count),
                inputs: [.. _flagComponents],
                outputs: [button],
                 apiName: flagMethod.Name, preprocess: false, queue: false, apiVisibility: ApiVisibility.Undocumented);

            clear.Click(
                (Func<object?>)(() => flagMethod.Reset()),
                inputs: [],
                outputs: [button],
                 apiName: flagMethod.ResetName, queue: false, apiVisibility: ApiVisibility.Undocumented);
        }
    }

    /// <summary>Draws the example gallery, when the application gave samples.</summary>
    /// <remarks>
    /// The gallery is given the inputs and outputs without their states: a state is not something a
    /// sample can fill in. The Python version also hides the additional inputs' columns, because a
    /// sample is a sample of the main fields.
    /// </remarks>
    private void RenderExamples()
    {
        if (_examples is null)
        {
            return;
        }

        var inputs = InputComponents.Where(component => component is not State).ToArray();
        var outputs = _outputs.Where(component => component is not State).ToArray();
        var mainCount = _mainInputs.Count(component => component is not State);

        Examples = new Examples(
            _blocks,
            _examples,
            inputs,
            outputs,
            _fn,
            _cacheExamples,
            _cacheMode,
            _examplesPerPage,

            // The Python version's Interface calls the helper that defaults this to nothing, so an interface's
            // gallery has no label even though a gallery built on its own says "Examples".
            label: null,
            apiName: "load_example",
            batch: _batch,
            exampleLabels: _exampleLabels,
            visibleColumns: _additionalInputs.Count > 0 && mainCount > 0
                ? [.. Enumerable.Range(0, mainCount)]
                : null,
            preload: _preloadExample);
    }

    /// <summary>Draws the article, when the application gave one.</summary>
    private void RenderArticle()
    {
        if (!string.IsNullOrEmpty(_article))
        {
            _blocks.Add(new Markdown { Value = _article });
        }
    }

    /// <summary>The accordion the additional inputs are drawn in.</summary>
    /// <returns>The accordion.</returns>
    /// <exception cref="ArgumentException">The value is neither a label nor an accordion.</exception>
    private Accordion AccordionOf() => _accordionSpec switch
    {
        null => new Accordion { Label = "Additional Inputs", Open = false },
        string label => new Accordion { Label = label },
        Accordion given => given,
        _ => throw new ArgumentException(
            "The additional-inputs accordion is a string label or an Accordion; got "
            + $"{_accordionSpec.GetType().Name}.",
            "additionalInputsAccordion"),
    };

    /// <summary>The clear button, drawn the way the application asked for it.</summary>
    /// <returns>The button.</returns>
    /// <remarks>
    /// A clear button the application hid is still a button in the tree: the Python version draws
    /// it with <c>visible: false</c> rather than leaving it out, because the bindings that clear
    /// the fields hang off it either way.
    /// </remarks>
    private ClearButton ClearButtonOf() => _clearButtonSpec switch
    {
        null => new ClearButton { Value = "Clear", Variant = "secondary" },
        { IsNone: true } => new ClearButton { Visible = false, Variant = "secondary" },
        { Button: { } given } => AsClearButton(given),
        { Label: { } label } => new ClearButton { Value = label, Variant = "secondary" },
        _ => new ClearButton { Value = "Clear", Variant = "secondary" },
    };

    /// <summary>A button the application configured, drawn as the clear button.</summary>
    /// <param name="given">The button.</param>
    /// <returns>The clear button, carrying what the application set.</returns>
    private static ClearButton AsClearButton(Button given) => new()
    {
        Value = given.Value,
        Variant = given.Variant,
        Size = given.Size,
        Visible = given.Visible,
        Interactive = given.Interactive,
        ElemId = given.ElemId,
        ElemClasses = given.ElemClasses,
    };

    /// <summary>A button drawn from the shape the application gave it.</summary>
    /// <param name="spec">A label, a button, or <see langword="null"/> for the default.</param>
    /// <param name="label">The label the default carries.</param>
    /// <param name="variant">The variant the default carries.</param>
    /// <param name="visible">Whether the default is drawn.</param>
    /// <returns>The button.</returns>
    private static Button ButtonOf(ButtonSpec? spec, string label, string variant, bool visible = true) => spec switch
    {
        { Button: { } given } => given,
        { Label: { } text } => new Button { Value = text, Variant = variant, Visible = visible },
        _ => new Button { Value = label, Variant = variant, Visible = visible },
    };

    /// <summary>
    /// Names the components the application left unnamed, so that every field and every result says
    /// what it is.
    /// </summary>
    /// <remarks>
    /// An input takes its handler parameter's name and an output is "output" — "output 2" when
    /// there are several — which is the Python version's rule and the reason a three-line interface
    /// still reads as a form. A label the application set is left alone.
    /// </remarks>
    private void LabelComponents()
    {
        var names = ParameterNamesOf(_fn);

        for (var index = 0; index < InputComponents.Count; index++)
        {
            var component = InputComponents[index];

            if (component.Label is null && index < names.Count)
            {
                component.Label = names[index];
            }
        }

        for (var index = 0; index < _outputs.Count; index++)
        {
            _outputs[index].Label ??= _outputs.Count == 1 ? "output" : $"output {index}";
        }
    }

    /// <summary>
    /// The names the handler's inputs are called in the API documentation.
    /// </summary>
    /// <returns>One name per input component.</returns>
    /// <remarks>
    /// The Python version names an endpoint's inputs after the function's parameters, so a client
    /// calls <c>predict(name="a", age=1)</c> rather than by position. A delegate written for this
    /// framework carries those names and they are read off it; one the framework built itself takes
    /// its values as a list and has no names to read, so an application that wants them states them
    /// (<c>parameterNames</c>) and this falls back to the handler for the rest.
    /// </remarks>
    private List<string?> InputParameterNames()
    {
        var names = _parameterNames is null ? ParameterNamesOf(_fn) : [.. _parameterNames];
        var count = InputComponents.Count;

        return [.. Enumerable.Range(0, count).Select(index => index < names.Count ? names[index] : null)];
    }

    /// <summary>The names the handler's parameters carry, in order.</summary>
    /// <param name="fn">The handler.</param>
    /// <returns>The names, without the parameters the framework fills itself.</returns>
    /// <remarks>
    /// The Python version reads the same list with <c>inspect.getfullargspec</c> and drops the
    /// parameters that are not components — a progress tracker, a request. Here those are the types
    /// a run fills itself, so they are the ones that are left out.
    /// </remarks>
    private static List<string> ParameterNamesOf(Delegate fn) =>
    [
        .. fn.Method.GetParameters()
            .Where(parameter => !DelegateInvocation.IsSuppliedByRun(parameter.ParameterType))
            .Select(parameter => parameter.Name ?? string.Empty),
    ];

    /// <summary>The shape the components describe.</summary>
    /// <param name="inputs">The main inputs.</param>
    /// <param name="additional">The additional inputs.</param>
    /// <param name="outputs">The outputs.</param>
    /// <returns>The shape.</returns>
    /// <exception cref="ArgumentException">Neither side has a component.</exception>
    private static InterfaceType TypeOf(
        List<Component> inputs,
        List<Component> additional,
        List<Component> outputs)
    {
        if (inputs.Count == 0 && additional.Count == 0 && outputs.Count == 0)
        {
            throw new ArgumentException(
                "An interface needs at least one input or one output; it has neither.");
        }

        if (outputs.Count == 0)
        {
            return InterfaceType.InputOnly;
        }

        if (inputs.Count == 0 && additional.Count == 0)
        {
            return InterfaceType.OutputOnly;
        }

        var all = new List<Component>(inputs);
        all.AddRange(additional);

        return all.Count == outputs.Count && all.SequenceEqual(outputs)
            ? InterfaceType.Unified
            : InterfaceType.Standard;
    }

    /// <summary>The first state among some components, or <see langword="null"/> when there is none.</summary>
    /// <param name="components">The components.</param>
    /// <returns>The state.</returns>
    private static State? StateIn(IReadOnlyList<Component> components) =>
        components.OfType<State>().FirstOrDefault();

    /// <summary>Puts one state in place of every state, so that both sides name the same one.</summary>
    /// <param name="components">The list to rewrite.</param>
    /// <param name="state">The state to use.</param>
    private static void Replace(List<Component> components, State state)
    {
        for (var index = 0; index < components.Count; index++)
        {
            if (components[index] is State)
            {
                components[index] = state;
            }
        }
    }

    /// <summary>Refuses an application whose states cannot be paired.</summary>
    /// <param name="inputs">The main inputs.</param>
    /// <param name="additional">The additional inputs.</param>
    /// <param name="outputs">The outputs.</param>
    /// <exception cref="ArgumentException">There is more than one state on a side, or one side has none.</exception>
    private static void CheckStateCount(
        IReadOnlyList<Component> inputs,
        IReadOnlyList<Component> additional,
        IReadOnlyList<Component> outputs)
    {
        var inputStates = inputs.Concat(additional).OfType<State>().Distinct().Count();
        var outputStates = outputs.OfType<State>().Distinct().Count();

        if (inputStates == 0 && outputStates == 0)
        {
            return;
        }

        if (inputStates != 1 || outputStates != 1)
        {
            throw new ArgumentException(
                "An interface that uses a state has exactly one state input and one state output; "
                + $"this one has {inputStates} and {outputStates}.");
        }
    }

    /// <summary>The flagging mode in force.</summary>
    /// <param name="requested">What the application asked for, or <see langword="null"/>.</param>
    /// <returns>The mode.</returns>
    /// <exception cref="ArgumentException">The mode is not one of the three.</exception>
    /// <remarks>
    /// The Python version reads <c>GRADIO_FLAGGING_MODE</c> when the application named none, and
    /// defaults to <c>manual</c> — a sample can be reported, but nothing is recorded unless the
    /// user says so.
    /// </remarks>
    private static string FlaggingModeOf(string? requested)
    {
        var mode = requested
            ?? Environment.GetEnvironmentVariable("GRADIO_FLAGGING_MODE")
            ?? "manual";

        return mode is "manual" or "never" or "auto"
            ? mode
            : throw new ArgumentException(
                $"The flagging mode is 'auto', 'manual' or 'never'; got '{mode}'.",
                nameof(requested));
    }

    /// <summary>The options a user may flag a sample with.</summary>
    /// <param name="options">The options the application gave, or <see langword="null"/> for none.</param>
    /// <returns>The label and the recorded value of each option.</returns>
    private static List<(string Label, string? Value)> FlaggingOptionsOf(
        IReadOnlyList<FlaggingOption>? options) =>
        options is { Count: > 0 }
            ? [.. options.Select(option => (option.Label, option.Value))]
            : [("Flag", null)];
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A conversation with a handler on the other end, built in one call
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gr.ChatInterface</c>: the conversation, the field a message is
/// typed into, and the bindings that carry a message from one to the other, wrapped in the layout,
/// the buttons and the behaviours a chat needs. Everything it draws is an ordinary block, so an
/// application that outgrows it can be rebuilt with <see cref="Blocks"/> and will describe itself
/// the same way.
/// </para>
/// <para>
/// It renders into an application rather than being one, for the reason <see cref="Interface"/> does:
/// the render root is resolved from the container, one container hosting one application, so the
/// chat is handed the application to render into.
/// </para>
/// <para>
/// The components are numbered in the Python version's order — the additional inputs first, because
/// they are constructed before the layout that holds them, then the states, then the layout — and
/// the bindings are created in the order the chain runs, because the config numbers both. A chat
/// whose ids or bindings were created in another order describes a different application.
/// </para>
/// </remarks>
public sealed partial class ChatInterface
{
    private const int DefaultExamplesPerPage = 10;

    private readonly Blocks _blocks;
    private readonly Delegate _fn;
    private readonly bool _multimodal;
    private readonly bool _editable;
    private readonly bool _autofocus;
    private readonly bool _autoscroll;
    private readonly bool _saveHistory;
    private readonly bool _runExamplesOnClick;
    private readonly bool? _cacheExamples;
    private readonly string? _cacheMode;
    private readonly string? _title;
    private readonly string? _description;
    private readonly ProgressDisplay _showProgress;
    private readonly ConcurrencyLimit _concurrencyLimit;
    private readonly string? _apiName;
    private readonly string? _apiDescription;
    private readonly ApiVisibility _apiVisibility;
    private readonly Delegate? _validator;
    private readonly string _flaggingMode;
    private readonly IReadOnlyList<string> _flaggingOptions;
    private readonly string _flaggingDir;
    private readonly object? _examples;
    private readonly string? _examplesLabel;
    private readonly IReadOnlyList<string>? _exampleLabels;
    private readonly IReadOnlyList<string>? _exampleIcons;
    private readonly object? _submitButtonSpec;
    private readonly object? _stopButtonSpec;
    private readonly object? _accordionSpec;
    private readonly List<Component> _additionalInputs;
    private readonly List<Component> _additionalOutputs;
    private readonly List<ChatExample> _exampleMessages;

    /// <summary>The additional inputs this chat numbers and draws itself.</summary>
    private readonly List<Component> _drawnInputs = [];

    /// <summary>Whether a sample carries values for the additional inputs as well as the message.</summary>
    private readonly bool _additionalInputsInExamples;

    /// <summary>The stop button's text before the chat hid it, which a run puts back.</summary>
    private readonly object? _originalStopButton;

    private Column? _chatbotColumn;

    /// <summary>Builds a chat and renders it into <paramref name="blocks"/>.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="fn">
    /// The handler: the message, the conversation so far, and then one value per additional input.
    /// </param>
    /// <param name="multimodal">
    /// Whether the field accepts files as well as text. The handler then receives a
    /// <see cref="MultimodalText"/> rather than a string as its message.
    /// </param>
    /// <param name="chatbot">The conversation component to use, or <see langword="null"/> for a default one.</param>
    /// <param name="textbox">
    /// The field to type into — a <see cref="Textbox"/> or a <see cref="MultimodalTextbox"/> — or
    /// <see langword="null"/> for one built from <paramref name="multimodal"/>.
    /// </param>
    /// <param name="additionalInputs">
    /// The components the handler reads after the conversation, or <see langword="null"/> for none.
    /// Each is a component, or the name of one — <c>"textbox"</c>, <c>"slider"</c>. They are drawn
    /// under the conversation, in an accordion.
    /// </param>
    /// <param name="additionalInputsAccordion">
    /// The accordion those components are drawn in: a string label, or an <see cref="Accordion"/> to
    /// configure it further. <see langword="null"/> for "Additional Inputs", closed.
    /// </param>
    /// <param name="additionalOutputs">
    /// The components the handler's result fills after the conversation, or <see langword="null"/>
    /// for none. They must already be drawn by the surrounding application.
    /// </param>
    /// <param name="editable">Whether a user may edit a past message and have the answer regenerated.</param>
    /// <param name="examples">
    /// The messages offered as starting points: one entry per sample. A sample is the message, or —
    /// when the additional inputs are filled in by a sample — a list whose first entry is the
    /// message and whose remaining entries are the additional values.
    /// </param>
    /// <param name="examplesLabel">
    /// The label of the gallery drawn under the conversation, or <see langword="null"/> for none.
    /// </param>
    /// <param name="exampleLabels">
    /// The labels shown instead of the samples themselves, or <see langword="null"/> for none.
    /// </param>
    /// <param name="exampleIcons">
    /// The pictures shown beside the samples, or <see langword="null"/> for none.
    /// </param>
    /// <param name="runExamplesOnClick">
    /// Whether clicking a sample sends it, or only fills the field. Ignored when a sample's answer is
    /// cached, because a cached answer is what the click is for.
    /// </param>
    /// <param name="cacheExamples">
    /// Whether the answers to the samples are worked out and kept, or <see langword="null"/> to read
    /// <c>GRADIO_CACHE_EXAMPLES</c>.
    /// </param>
    /// <param name="cacheMode">
    /// <c>eager</c> to work every answer out at startup, <c>lazy</c> to work each one out when it is
    /// first asked for, or <see langword="null"/> to read <c>GRADIO_CACHE_MODE</c>.
    /// </param>
    /// <param name="title">The heading drawn above the conversation, or <see langword="null"/> for none.</param>
    /// <param name="description">The text drawn under the heading, or <see langword="null"/> for none.</param>
    /// <param name="flaggingMode">
    /// <c>manual</c> to let a user rate a message, <c>never</c> to offer no rating, or
    /// <see langword="null"/> to read <c>GRADIO_CHAT_FLAGGING_MODE</c> — which defaults to
    /// <c>never</c>.
    /// </param>
    /// <param name="flaggingOptions">
    /// The ratings offered, or <see langword="null"/> for "Like" and "Dislike". The first two are
    /// drawn as the thumbs a chat puts beside a message; any others appear under a flag.
    /// </param>
    /// <param name="flaggingDir">The directory the ratings are recorded in.</param>
    /// <param name="analyticsEnabled">
    /// Whether the application may report what it does, or <see langword="null"/> to leave the
    /// application's own setting alone. Analytics is off unless an application asks for it, which
    /// is this framework's one deliberate departure from the Python version.
    /// </param>
    /// <param name="autofocus">Whether the field takes focus when the page loads.</param>
    /// <param name="autoscroll">Whether the conversation scrolls to a new message by itself.</param>
    /// <param name="submitBtn">
    /// The field's submit button: <see langword="true"/> for the icon, a string for that text,
    /// <see langword="false"/> for none, or <see langword="null"/> for the default, which is the icon.
    /// </param>
    /// <param name="stopBtn">
    /// The field's stop button, in the same three shapes: shown while a streaming answer is being
    /// produced.
    /// </param>
    /// <param name="concurrencyLimit">How many answers may be produced at once.</param>
    /// <param name="deleteCache">
    /// How often to reclaim the application's temporary files, and how old a file has to be, or
    /// <see langword="null"/> to keep them.
    /// </param>
    /// <param name="showProgress">How much of the progress animation a run shows.</param>
    /// <param name="fillHeight">Whether the chat fills the viewport's height.</param>
    /// <param name="fillWidth">Whether the chat fills the viewport's width.</param>
    /// <param name="apiName">
    /// The name the chat's endpoint is exposed under, or <see langword="null"/> for the handler's own
    /// name.
    /// </param>
    /// <param name="apiDescription">
    /// The description the endpoint carries: a string, <see langword="false"/> for none at all, or
    /// <see langword="null"/> to let the documentation say what it says for any handler.
    /// </param>
    /// <param name="apiVisibility">How far the chat's endpoint is exposed.</param>
    /// <param name="saveHistory">
    /// Whether conversations are kept in the browser and listed beside the chat, so that a reader can
    /// come back to one.
    /// </param>
    /// <param name="validator">
    /// A function that receives the same inputs as the handler and rejects what it does not accept,
    /// or <see langword="null"/> for none. A rejection is answered with a 422 and the message.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> or <paramref name="fn"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// A component, a button or an accordion is given in a shape the parameter does not take, a
    /// sample is not a list when the additional inputs are filled in by samples, or the flagging
    /// mode is not one of the two.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The parameters are the Python version's. Three of them are three-valued in a way a C#
    /// signature cannot express — a label <em>or</em> a flag, a description <em>or</em> "none", a
    /// string <em>or</em> a component — so they are taken as <see cref="object"/> and checked here,
    /// and <see langword="null"/> stands for "not given", which is how the repo spells every union
    /// the Python version declares.
    /// </para>
    /// <para>
    /// A chat is rendered once, when it is constructed: the application is built by the call, so a
    /// second one would describe a second application.
    /// </para>
    /// </remarks>
    internal ChatInterface(
        Blocks blocks,
        Delegate fn,
        bool multimodal = false,
        Chatbot? chatbot = null,
        Component? textbox = null,
        object? additionalInputs = null,
        object? additionalInputsAccordion = null,
        object? additionalOutputs = null,
        bool editable = false,
        object? examples = null,
        string? examplesLabel = "Examples",
        IReadOnlyList<string>? exampleLabels = null,
        IReadOnlyList<string>? exampleIcons = null,
        bool runExamplesOnClick = true,
        bool? cacheExamples = null,
        string? cacheMode = null,
        string? title = null,
        string? description = null,
        string? flaggingMode = null,
        IReadOnlyList<string>? flaggingOptions = null,
        string flaggingDir = ".gradio/flagged",
        bool? analyticsEnabled = null,
        bool autofocus = true,
        bool autoscroll = true,
        object? submitBtn = null,
        object? stopBtn = null,
        ConcurrencyLimit concurrencyLimit = default,
        DeleteCacheSettings? deleteCache = null,
        ProgressDisplay showProgress = ProgressDisplay.Minimal,
        bool fillHeight = true,
        bool fillWidth = false,
        string? apiName = null,
        object? apiDescription = null,
        ApiVisibility apiVisibility = ApiVisibility.Public,
        bool saveHistory = false,
        Delegate? validator = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(fn);

        _blocks = blocks;
        _fn = fn;
        _multimodal = multimodal;
        _editable = editable;
        _autofocus = autofocus;
        _autoscroll = autoscroll;
        _saveHistory = saveHistory;
        _runExamplesOnClick = runExamplesOnClick;
        _cacheExamples = cacheExamples;
        _cacheMode = cacheMode;
        _title = title;
        _description = description;
        _showProgress = showProgress;
        _concurrencyLimit = concurrencyLimit;
        _apiName = apiName;
        _apiDescription = ApiDescriptionOf(apiDescription);
        _apiVisibility = apiVisibility;
        _validator = validator;
        _examples = examples;
        _examplesLabel = examplesLabel;
        _exampleLabels = exampleLabels;
        _exampleIcons = exampleIcons;
        _submitButtonSpec = submitBtn;
        _stopButtonSpec = stopBtn;
        _accordionSpec = additionalInputsAccordion;
        _flaggingOptions = flaggingOptions is { Count: > 0 } ? [.. flaggingOptions] : ["Like", "Dislike"];
        _flaggingDir = flaggingDir;

        _additionalInputs = ComponentsOf(additionalInputs, nameof(additionalInputs));
        _additionalOutputs = ComponentsOf(additionalOutputs, nameof(additionalOutputs));
        CheckAccordion();
        CheckTextbox(textbox);
        CheckChatbot(chatbot);
        _flaggingMode = FlaggingModeOf(flaggingMode);

        _additionalInputsInExamples = additionalInputs is not null
            && examples is not null
            && SamplesFillAdditionalInputs(examples);

        if (additionalInputs is not null && examples is not null && !SamplesAreLists(examples))
        {
            throw new ArgumentException(
                "A chat with additional inputs takes each sample as a list: the message first, then "
                + "one value per additional input.",
                nameof(examples));
        }

        _exampleMessages = ExampleMessages(examples, exampleLabels, exampleIcons);

        // A chat that keeps its history gives the browser somewhere to keep it, and a field whose
        // stop button is hidden until a streaming answer starts. Both are decided before anything is
        // drawn, because the first block the chat renders is the one that remembers.
        _originalStopButton = StopButtonOf(stopBtn, textbox);

        blocks.Mode = "chat_interface";
        blocks.Title = title ?? "Gradio";

        // A chat's description is the application's, and "none" is a value here: the API document
        // serves null for a chat that was given no description, while a plain application serves an
        // empty string. The Python version tells the two apart the same way — its Blocks has no description at
        // all, and a chat that was given none carries a null one.
        blocks.Description = description;

        blocks.FillHeight = fillHeight;
        blocks.FillWidth = fillWidth;

        if (deleteCache is not null)
        {
            blocks.DeleteCache = deleteCache;
        }

        if (analyticsEnabled is { } analytics)
        {
            blocks.AnalyticsEnabled = analytics;
        }

        Render(chatbot, textbox);
    }

    /// <summary>The conversation.</summary>
    public Chatbot Chatbot { get; private set; } = null!;

    /// <summary>The field a message is typed into: a <see cref="Textbox"/> or a <see cref="MultimodalTextbox"/>.</summary>
    public Component Textbox { get; private set; } = null!;

    /// <summary>The conversation's messages as the server knows them, one state for the run to read.</summary>
    public State ChatbotState { get; private set; } = null!;

    /// <summary>The conversation as a handler outside the chat last set it.</summary>
    public State ChatbotValue { get; private set; } = null!;

    /// <summary>The message the user just sent, kept between the steps that handle it.</summary>
    public State SavedInput { get; private set; } = null!;

    /// <summary>Which saved conversation is open, or <see langword="null"/> when none is.</summary>
    public State ConversationId { get; private set; } = null!;

    /// <summary>The conversations the browser has kept, when the chat was asked to keep them.</summary>
    public BrowserState SavedConversations { get; private set; } = null!;

    /// <summary>The button that starts a new conversation, or <see langword="null"/> when history is off.</summary>
    public Button? NewChatButton { get; private set; }

    /// <summary>The list of saved conversations, or <see langword="null"/> when history is off.</summary>
    public Dataset? ChatHistory { get; private set; }

    /// <summary>The gallery of samples, or <see langword="null"/> when the chat was given none.</summary>
    public Examples? Examples { get; private set; }

    /// <summary>Whether a user may rate a message: <c>manual</c> or <c>never</c>.</summary>
    public string FlaggingMode => _flaggingMode;

    /// <summary>The ratings offered, in the order they are drawn.</summary>
    public IReadOnlyList<string> FlaggingOptions => _flaggingOptions;

    /// <summary>The components the handler reads after the conversation.</summary>
    public IReadOnlyList<Component> AdditionalInputs => _additionalInputs;

    /// <summary>The components the handler's result fills after the conversation.</summary>
    public IReadOnlyList<Component> AdditionalOutputs => _additionalOutputs;

    /// <summary>The binding a message goes through, once the chat has been rendered.</summary>
    public Dependency? SubmitEvent { get; private set; }

    /// <summary>Whether the handler answers a chunk at a time.</summary>
    private bool StreamsResult => Dependency.StreamsResult(_fn);

    /// <summary>Renders the chat, in the Python version's order.</summary>
    /// <param name="chatbot">The conversation component the application gave, or
    /// <see langword="null"/>.</param> <param name="textbox">The field the application gave, or
    /// <see langword="null"/>.</param>
    private void Render(Chatbot? chatbot, Component? textbox)
    {
        using var scope = _blocks.IsRendering ? null : _blocks.BeginRender();

        ReserveAdditionalInputs();

        SavedConversations = _blocks.Add(new BrowserState
        {
            DefaultValue = new JsonArray(),
            StorageKey = $"_saved_conversations_{_blocks.Id}",
        });

        ConversationId = _blocks.Add(new State());
        SavedInput = _blocks.Add(new State());
        NullComponent = _blocks.Add(new State());

        var column = _blocks.Add(new Column());

        using (_blocks.Enter(column))
        {
            RenderHeader();

            if (_saveHistory)
            {
                var row = _blocks.Add(new Row { Scale = 1 });

                using (_blocks.Enter(row))
                {
                    RenderHistoryArea();

                    _chatbotColumn = _blocks.Add(new Column { Scale = 6 });

                    using (_blocks.Enter(_chatbotColumn))
                    {
                        RenderChatbotArea(chatbot, textbox);
                        RenderFooter();
                    }
                }
            }
            else
            {
                _chatbotColumn = column;
                RenderChatbotArea(chatbot, textbox);
                RenderFooter();
            }
        }

        SetupEvents();
    }

    /// <summary>The state a handler writes when its answer is only there to be discarded.</summary>
    private State NullComponent { get; set; } = null!;

    /// <summary>
    /// Numbers the additional inputs before anything else, because the Python version constructs
    /// them first.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version builds them in <c>__init__</c>, before the <c>with self:</c> block that
    /// draws the layout, so they hold the first ids the chat's application hands out. They are
    /// placed in the accordion at the end, which is why they are numbered here and attached there.
    /// </para>
    /// <para>
    /// A component the surrounding application already drew keeps the id it was given: the chat
    /// does not number it again and does not draw it, which is the Python version's rule — a chat
    /// handed a field that is already on the page passes its value to the handler without moving
    /// it.
    /// </para>
    /// </remarks>
    private void ReserveAdditionalInputs()
    {
        foreach (var component in _additionalInputs)
        {
            if (component.Id != Block.UnrenderedId)
            {
                continue;
            }

            _blocks.Reserve(component);
            _drawnInputs.Add(component);
        }
    }

    /// <summary>The heading and the description, drawn above the conversation.</summary>
    private void RenderHeader()
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

    /// <summary>The column beside the conversation: the new-conversation button and the list of them.</summary>
    private void RenderHistoryArea()
    {
        var column = _blocks.Add(new Column { Scale = 1, MinWidth = 100 });

        using (_blocks.Enter(column))
        {
            NewChatButton = _blocks.Add(new Button
            {
                Value = TranslationKey.Of(TranslationKey.NewChat),
                Variant = "primary",
                Size = "md",
            });

            // The list draws one hidden field per conversation, which is where the Python version puts the
            // title: a dataset draws its samples through components, so a sample needs one to be
            // drawn by. It is numbered before the list itself because it is constructed first.
            var title = _blocks.Add(new Textbox { Visible = false });

            ChatHistory = _blocks.Add(new Dataset
            {
                Components = [title],
                ShowLabel = false,
                Layout = "table",
                Type = "index",
            });
        }
    }

    /// <summary>The conversation, the field, and the states that carry a message between them.</summary>
    /// <param name="chatbot">The conversation component the application gave, or <see langword="null"/>.</param>
    /// <param name="textbox">The field the application gave, or <see langword="null"/>.</param>
    private void RenderChatbotArea(Chatbot? chatbot, Component? textbox)
    {
        Chatbot = chatbot ?? new Chatbot
        {
            Label = TranslationKey.Of(TranslationKey.Chatbot),
            Scale = 1,
            Autoscroll = _autoscroll,
            Examples = _additionalInputsInExamples ? null : _exampleMessages,
        };

        if (chatbot is not null)
        {
            // A conversation the application gave draws the chat's samples rather than its own,
            // which is the Python version's rule: a chat that already offered samples would otherwise show
            // the ones it was given and never the ones it was told about.
            Chatbot.Examples = _additionalInputsInExamples ? null : _exampleMessages;
        }

        // A conversation the application gave keeps the editing rule it was built with; the chat's
        // flag only fills in one that said nothing, which is what binding the edit event does
        // in the Python version.
        if (_editable && Chatbot.Editable is null)
        {
            Chatbot.Editable = "user";
        }

        _blocks.Add(Chatbot);

        var group = _blocks.Add(new Group());

        using (_blocks.Enter(group))
        {
            var row = _blocks.Add(new Row());

            using (_blocks.Enter(row))
            {
                Textbox = textbox ?? NewTextbox();

                if (textbox is not null)
                {
                    // A field the application gave is drawn without its label, because the chat is
                    // what the field belongs to and a label above it would name the field twice.
                    // The label itself is kept: the Python version hides it rather than clearing it, and the
                    // field's own configuration is the application's.
                    Textbox.ShowLabel = false;
                }

                _blocks.Add(Textbox);
            }
        }

        // The stop button is hidden until an answer is being produced, so the value the field was
        // drawn with is kept: a run puts it back when it starts.
        SetStopButton(false);

        FakeApiButton = _blocks.Add(new Button { Value = "Fake API", Visible = false });

        ApiResponse = _blocks.Add(new JSON { Label = "Response", Visible = false });

        var conversation = Chatbot.Value;

        ChatbotState = _blocks.Add(new State { Value = MessagesToJson(conversation) });
        ChatbotValue = _blocks.Add(new State { Value = MessagesToJson(conversation) });
    }

    /// <summary>The button the API endpoint is bound to, which is never drawn.</summary>
    private Button FakeApiButton { get; set; } = null!;

    /// <summary>The component the API endpoint's answer is put in, which is never drawn.</summary>
    private JSON ApiResponse { get; set; } = null!;

    /// <summary>The gallery of samples, and the accordion the additional inputs are drawn in.</summary>
    private void RenderFooter()
    {
        if (_examples is not null)
        {
            Examples = new Examples(
                _blocks,
                _examples,
                [Textbox, .. _additionalInputs],
                [Chatbot],
                ExamplesHandler(),
                _cacheExamples,
                _cacheMode,
                DefaultExamplesPerPage,
                label: _examplesLabel,
                apiName: "load_example",

                // A sample is a sample of the whole form: when the additional inputs are filled in
                // by samples the gallery draws them, and when they are not it fills only the field
                // and the conversation is where the answer goes.
                preprocess: _additionalInputsInExamples,
                visible: _additionalInputsInExamples,
                runOnClick: false,
                preload: 0);
        }

        if (_additionalInputs.Count == 0)
        {
            return;
        }

        // Only the components this chat numbered are drawn here: one the surrounding application
        // already drew is already on the page, and a component belongs to one place in it.
        if (_drawnInputs.Count == 0)
        {
            return;
        }

        var accordion = _blocks.Add(AccordionOf());

        using (_blocks.Enter(accordion))
        {
            foreach (var component in _drawnInputs)
            {
                _blocks.Attach(component);
            }
        }
    }

    /// <summary>The field the chat types into, built from the shapes the application gave.</summary>
    /// <returns>A <see cref="MultimodalTextbox"/> for a multimodal chat, and a <see cref="Textbox"/> otherwise.</returns>
    private Component NewTextbox() => _multimodal
        ? new MultimodalTextbox
        {
            ShowLabel = false,
            Label = string.Empty,
            Placeholder = TranslationKey.Of(TranslationKey.MessagePlaceholder),
            Scale = 7,
            Autofocus = _autofocus,
            SubmitBtn = ButtonValueOf(_submitButtonSpec, "submit_btn"),
            StopBtn = StopButtonOf(_stopButtonSpec),
        }
        : new Textbox
        {
            ShowLabel = false,
            Label = string.Empty,
            Placeholder = TranslationKey.Of(TranslationKey.MessagePlaceholder),
            Scale = 7,
            Autofocus = _autofocus,
            SubmitBtn = ButtonValueOf(_submitButtonSpec, "submit_btn"),
            StopBtn = StopButtonOf(_stopButtonSpec),
        };

    /// <summary>The accordion the additional inputs are drawn in.</summary>
    /// <returns>The accordion.</returns>
    /// <exception cref="ArgumentException">The value is neither a label nor an accordion.</exception>
    private Accordion AccordionOf() => _accordionSpec switch
    {
        null => new Accordion
        {
            Label = TranslationKey.Of(TranslationKey.AdditionalInputs),
            Open = false,
        },
        string label => new Accordion { Label = label },
        Accordion given => given,
        _ => throw new ArgumentException(
            "The additional-inputs accordion is a string label or an Accordion; got "
            + $"{_accordionSpec.GetType().Name}.",
            nameof(_accordionSpec)),
    };

    /// <summary>Refuses a field that is not one a chat can type into.</summary>
    /// <param name="textbox">The field the application gave, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">
    /// The component is neither kind of field, or the application has already drawn it.
    /// </exception>
    /// <remarks>
    /// <para>
    /// A chat reads the field's value as a message, and only the two fields that carry one are
    /// messages: the Python version refuses the rest rather than drawing a component it cannot
    /// read.
    /// </para>
    /// <para>
    /// The field has to be one the application has not drawn yet, because the chat is what draws it
    /// — in the row under the conversation, where the chat's layout puts it. The Python version can
    /// accept a field its <c>with</c> block has already drawn because it unrenders it first; here
    /// nothing is drawn until it is added, so a drawn field is a field the application placed
    /// somewhere itself, and moving it would be a different application from the one the code says.
    /// </para>
    /// </remarks>
    private static void CheckTextbox(Component? textbox)
    {
        if (textbox is null)
        {
            return;
        }

        if (textbox is not Gradio.Net.Textbox and not MultimodalTextbox)
        {
            throw new ArgumentException(
                $"A chat's field is a {nameof(Textbox)} or a "
                + $"{nameof(MultimodalTextbox)}; got "
                + $"{textbox.GetType().Name}.",
                nameof(textbox));
        }

        CheckNotDrawn(textbox, nameof(textbox), "field");
    }

    /// <summary>Refuses a conversation the application has already drawn.</summary>
    /// <param name="chatbot">The conversation the application gave, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">The application has already drawn it.</exception>
    /// <remarks>The same rule the field follows, for the same reason.</remarks>
    private static void CheckChatbot(Chatbot? chatbot)
    {
        if (chatbot is not null)
        {
            CheckNotDrawn(chatbot, nameof(chatbot), "conversation");
        }
    }

    /// <summary>Refuses a component that already has a place in the application.</summary>
    /// <param name="component">The component.</param>
    /// <param name="parameter">Which parameter it came from.</param>
    /// <param name="what">What the chat calls it, for the message.</param>
    /// <exception cref="ArgumentException">The component has already been drawn.</exception>
    private static void CheckNotDrawn(Component component, string parameter, string what)
    {
        if (component.Id != Block.UnrenderedId)
        {
            throw new ArgumentException(
                $"A chat draws its own {what}, so the one it is given has to be one the application "
                + $"has not added yet; this one is already drawn as id {component.Id}.",
                parameter);
        }
    }

    /// <summary>Refuses an accordion given in a shape the parameter does not take.</summary>
    /// <exception cref="ArgumentException">The value is neither a label nor an accordion.</exception>
    private void CheckAccordion()
    {
        if (_accordionSpec is not (null or string or Accordion))
        {
            throw new ArgumentException(
                "The additional-inputs accordion is a string label or an Accordion; got "
                + $"{_accordionSpec.GetType().Name}.",
                nameof(_accordionSpec));
        }
    }

    /// <summary>The submit button's value, read from the three shapes the parameter takes.</summary>
    /// <param name="spec">The value the application gave, or <see langword="null"/> for the default.</param>
    /// <param name="parameter">Which parameter it came from, for the message when it is refused.</param>
    /// <returns>The button's value: a flag or the text it says.</returns>
    /// <exception cref="ArgumentException">The value is not a flag or a string.</exception>
    private static object ButtonValueOf(object? spec, string parameter) => spec switch
    {
        null => true,
        bool flag => flag,
        string label => label,
        _ => throw new ArgumentException(
            $"A chat's {parameter} is a flag or a string; got {spec.GetType().Name}.",
            parameter),
    };

    /// <summary>The stop button's value, read from the three shapes the parameter takes.</summary>
    /// <param name="spec">The value the application gave, or <see langword="null"/> for the default.</param>
    /// <returns>The button's value: a flag or the text it says.</returns>
    /// <exception cref="ArgumentException">The value is not a flag or a string.</exception>
    private static object StopButtonOf(object? spec) => ButtonValueOf(spec, "stop_btn");

    /// <summary>The endpoint description, read from the three shapes the parameter takes.</summary>
    /// <param name="spec">The value the application gave.</param>
    /// <returns>
    /// The description, an empty string for <see langword="false"/> — which is how the parameter says
    /// "none at all" — or <see langword="null"/> for "not given".
    /// </returns>
    /// <exception cref="ArgumentException">The value is neither a flag nor a string.</exception>
    /// <remarks>
    /// The Python version's union of a string, <c>None</c> and <c>False</c>. The three are not the
    /// same answer: <c>None</c> lets the documentation fall back to what the handler says about
    /// itself, while <c>False</c> asks for no description at all, which the API document spells as
    /// an empty string.
    /// </remarks>
    private static string? ApiDescriptionOf(object? spec) => spec switch
    {
        null => null,
        false => string.Empty,
        string description => description,
        _ => throw new ArgumentException(
            $"A chat's api_description is a string, false or nothing; got {spec.GetType().Name}.",
            nameof(spec)),
    };

    /// <summary>
    /// The stop button's value before the chat hid it, which is what a run puts back when an answer
    /// starts.
    /// </summary>
    /// <param name="spec">The value the application gave, or <see langword="null"/>.</param>
    /// <param name="textbox">The field the application gave, or <see langword="null"/>.</param>
    /// <returns>The value the field was drawn with.</returns>
    /// <remarks>
    /// A field the application built carries its own stop button, and the chat's parameter is
    /// ignored for it — the Python version warns about the conflict rather than resolving it, which
    /// here is a rule instead of a warning: what the field says wins, because the field is what is
    /// drawn.
    /// </remarks>
    private static object? StopButtonOf(object? spec, Component? textbox) => textbox switch
    {
        Textbox given => given.StopBtn,
        MultimodalTextbox given => given.StopBtn,
        _ => StopButtonOf(spec),
    };

    /// <summary>Hides or shows the field's stop button, leaving the submit button as it was.</summary>
    /// <param name="shown">Whether the stop button is drawn.</param>
    private void SetStopButton(bool shown)
    {
        var value = shown ? _originalStopButton : false;

        switch (Textbox)
        {
            case Textbox textbox:
                textbox.StopBtn = value;
                break;

            case MultimodalTextbox textbox:
                textbox.StopBtn = value;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// The submit button's value as the field was drawn with it, which a finished answer puts back.
    /// </summary>
    /// <returns>The value the field carries: a flag or the text it says.</returns>
    /// <remarks>
    /// A field the application built carries its own submit button, and the chat's parameter is
    /// ignored for it — the same rule the stop button follows, and for the same reason: what the
    /// field says is what is drawn.
    /// </remarks>
    private object? SubmitButtonValue()
    {
        var field = Textbox;

        return field switch
        {
            Textbox given => given.SubmitBtn,
            MultimodalTextbox given => given.SubmitBtn,
            _ => ButtonValueOf(_submitButtonSpec, "submit_btn"),
        };
    }

    /// <summary>The flagging mode in force.</summary>
    /// <param name="requested">What the application asked for, or <see langword="null"/>.</param>
    /// <returns><c>manual</c> or <c>never</c>.</returns>
    /// <exception cref="ArgumentException">The mode is neither.</exception>
    /// <remarks>
    /// The Python version reads <c>GRADIO_CHAT_FLAGGING_MODE</c> when the application named none
    /// and defaults to <c>never</c> — unlike an <see cref="Interface"/>, a chat records nothing
    /// unless the application says it wants to.
    /// </remarks>
    private static string FlaggingModeOf(string? requested)
    {
        var mode = requested
            ?? Environment.GetEnvironmentVariable("GRADIO_CHAT_FLAGGING_MODE")
            ?? "never";

        return mode is "manual" or "never"
            ? mode
            : throw new ArgumentException(
                $"A chat's flagging mode is 'manual' or 'never'; got '{mode}'.",
                nameof(requested));
    }

    /// <summary>A parameter that takes one component, several, or the name of one.</summary>
    /// <param name="value">The value the application gave, or <see langword="null"/> for none.</param>
    /// <param name="parameter">Which parameter it came from.</param>
    /// <returns>The components, in the order they were given.</returns>
    /// <exception cref="ArgumentException">An entry is neither a component nor a name of one.</exception>
    private static List<Component> ComponentsOf(object? value, string parameter)
    {
        var components = new List<Component>();

        if (value is null)
        {
            return components;
        }

        if (value is Component or string)
        {
            components.Add(ComponentShortcuts.Resolve(value, parameter));

            return components;
        }

        if (value is not System.Collections.IEnumerable entries || value is JsonNode)
        {
            throw new ArgumentException(
                "A component, a name, or a list of either is expected; got "
                + $"{value.GetType().Name}.",
                parameter);
        }

        foreach (var entry in entries)
        {
            components.Add(ComponentShortcuts.Resolve(entry, parameter));
        }

        return components;
    }

    /// <summary>Whether the samples carry values for the additional inputs.</summary>
    /// <param name="examples">The samples.</param>
    /// <returns>Whether any sample has a value past its message.</returns>
    private static bool SamplesFillAdditionalInputs(object examples)
    {
        foreach (var sample in Samples(examples))
        {
            if (sample is System.Collections.IList row && row.Count > 1)
            {
                for (var index = 1; index < row.Count; index++)
                {
                    if (row[index] is not null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>Whether every sample is a list, which is what a chat with additional inputs requires.</summary>
    /// <param name="examples">The samples.</param>
    /// <returns>Whether each one is a list.</returns>
    private static bool SamplesAreLists(object examples) =>
        Samples(examples).All(sample => sample is System.Collections.IList);

    /// <summary>The samples a parameter holds, whatever shape they were given in.</summary>
    /// <param name="examples">The samples.</param>
    /// <returns>One entry per sample.</returns>
    private static IEnumerable<object?> Samples(object examples) =>
        examples is System.Collections.IEnumerable entries ? entries.Cast<object?>() : [];
}

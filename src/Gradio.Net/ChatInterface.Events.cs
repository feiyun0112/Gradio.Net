using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The bindings a chat is made of: what a message goes through between the field and the
/// conversation.
/// </summary>
/// <remarks>
/// <para>
/// The chain is the Python version's, step for step, and the order is not cosmetic: a submit is
/// five steps — empty the field and keep the message, put the message in the conversation, run the
/// handler, keep the conversation, empty the field again — and each step's config entry names the
/// one before it. A chat that merged two of them would describe a different application and behave
/// differently when one of them fails.
/// </para>
/// <para>
/// The steps that update a component without running anything on the server are bound with
/// <c>queue: false</c>, which is what makes the conversation show the message before the answer is
/// worked out: those updates are produced by the browser rather than asked for.
/// </para>
/// </remarks>
public sealed partial class ChatInterface
{
    /// <summary>The name the chat's own endpoint is served under.</summary>
    private const string SubmitApiName = "_submit_fn";

    /// <summary>The value the field's submit button was drawn with, which a run puts back.</summary>
    private object? _originalSubmitButton;

    /// <summary>Sets up the bindings, in the Python version's order.</summary>
    private void SetupEvents()
    {
        var userSubmit = BindField(
            EventNames.Submit,
            (Func<object?, object?>)ClearAndSave,
            [Textbox],
            [Textbox, SavedInput],
            new EventBindingOptions
            {
                ApiName = "_clear_and_save_textbox",
                ApiVisibility = ApiVisibility.Undocumented,

                // A validator runs before the handler, and a run it can reject has to go through
                // the queue: there is nowhere else for a rejection to be produced.
                Queue = _validator is not null,
                Validator = _validator,
            });

        SubmitEvent = AttachSubmitChain(userSubmit);

        var exampleEvent = AttachExampleEvents();

        var retryEvent = AttachRetryChain();

        AttachStopEvents(userSubmit, SubmitEvent, retryEvent, exampleEvent);

        AttachUndoEvents();
        AttachOptionEvents();
        AttachClearEvents();

        if (_editable)
        {
            AttachEditEvents();
        }

        if (_saveHistory)
        {
            AttachHistoryEvents();
        }

        if (_flaggingMode != "never")
        {
            AttachFlaggingEvents();
        }

        AttachChatbotValueEvents();
    }

    /// <summary>The chain a message goes through: the answer, the conversation, and the field again.</summary>
    /// <param name="userSubmit">The step that emptied the field and kept the message.</param>
    /// <returns>The step that produced the answer, which is what a stop cancels.</returns>
    private Dependency AttachSubmitChain(Dependency userSubmit)
    {
        var appendUser = Chain(
            userSubmit,
            "_append_message_to_history",
            (Func<object?, object?, object?>)AppendUserMessage,
            [SavedInput, Chatbot],
            [Chatbot],
            queue: false);

        var answered = AnswerStep(appendUser);

        var emptied = Chain(
            SyncConversation(answered),
            "lambda",
            (Func<object?>)EnableField,
            [],
            [Textbox]);

        SaveStep(emptied);

        // The endpoint the API documentation shows, bound to a button nobody sees: a client calls
        // the chat the way it calls any other endpoint, and the answer is the conversation rather
        // than what the browser draws.
        BindField(
            EventNames.Click,
            ApiHandler(),
            [Textbox, ChatbotState, .. _additionalInputs],
            [ApiResponse, ChatbotState, .. _additionalOutputs],
            new EventBindingOptions
            {
                ApiName = _apiName ?? FunctionName(),
                ApiDescription = _apiDescription,
                ApiVisibility = _apiVisibility,
                ParameterNames = SubmitParameterNames(),
                ConcurrencyLimit = _concurrencyLimit,

                // The endpoint answers with what the handler answered rather than with what the
                // components would draw: a client wants the message, not the conversation widget.
                Postprocess = false,
            },
            FakeApiButton);

        return answered;
    }

    /// <summary>
    /// The binding a sample clicked inside the conversation goes through.
    /// </summary>
    /// <returns>
    /// The step a stop cancels — the one that produces the answer — or <see langword="null"/> when
    /// a click does not run anything.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A chat's samples are drawn inside the conversation, so a click on one raises the
    /// conversation's own signal rather than the gallery's. What the click does is the application's
    /// choice: filling the field is what a reader expects when the samples are suggestions, and
    /// running the sample is what they expect when the samples are demonstrations — and a cached
    /// sample always runs, because the answer is what was cached.
    /// </para>
    /// <para>
    /// A chat whose samples fill in the additional inputs draws them in the gallery instead, and the
    /// gallery's own bindings are what a click goes through; that is why this does nothing there.
    /// </para>
    /// </remarks>
    private Dependency? AttachExampleEvents()
    {
        if (_examples is null || _additionalInputsInExamples)
        {
            return null;
        }

        if (_cacheExamples != true && !_runExamplesOnClick)
        {
            Bind(
                Chatbot.Events.ExampleSelect,
                fn: (Func<SelectData, object?>)SampleAsFieldValue, inputs: [], outputs: [Textbox], apiName: "example_populated", apiVisibility: ApiVisibility.Undocumented);

            return null;
        }

        var clicked = Bind(
            Chatbot.Events.ExampleSelect,
            fn: (Func<SelectData, IAsyncEnumerable<object?>>)ClickSample, inputs: [], outputs: [Chatbot, SavedInput], apiName: "example_clicked", apiVisibility: ApiVisibility.Undocumented);

        var ran = clicked;

        if (_cacheExamples != true)
        {
            // The field is put out of the run's way before the answer is worked out: a reader who
            // could type into it while the sample is being answered would be answering twice.
            var prepared = Chain(
                ran,
                "lambda",
                (Func<object?>)PrepareFieldForRun,
                [],
                [Textbox],
                queue: false);

            ran = AnswerStep(prepared);
        }

        SyncConversation(ran);

        return ran;
    }

    /// <summary>Fills the field with the sample that was clicked.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The field's value: the sample's text, or its text and files.</returns>
    private object? SampleAsFieldValue(SelectData sample) =>
        _multimodal ? SampleAsMultimodal(sample) : SampleText(sample);

    /// <summary>Puts a sample into the conversation and hands its message to the run.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The conversation with the sample in it and the message, and then the cached answer.</returns>
    /// <remarks>
    /// A cached sample is answered twice: once with the question alone, so that the reader sees it
    /// immediately, and once with the conversation the cache recorded. The first chunk is what the
    /// browser draws while the second is being read.
    /// </remarks>
    private async IAsyncEnumerable<object?> ClickSample(SelectData sample)
    {
        var message = SampleAsFieldValue(sample);

        yield return EventValues.Of([MessageOfExample(sample), message]);

        if (_cacheExamples != true || Examples is null || sample.Index is not { } index)
        {
            yield break;
        }

        var cached = await Examples.LoadFromCacheAsync(index).ConfigureAwait(false);

        if (cached is not null)
        {
            yield return EventValues.Of([MessagesOf(cached), message]);
        }
    }

    /// <summary>The update that puts the stop button out while a sample is being answered.</summary>
    /// <returns>The update.</returns>
    private JsonObject PrepareFieldForRun() => PropertyUpdate.Partial(
        ("submit_btn", JsonValue.Create(false)),
        ("stop_btn", JsonValue.Create(_originalStopButton)));

    /// <summary>The text a sample carries.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The text.</returns>
    private static string SampleText(SelectData sample) =>
        JsonValues.Text(ExampleOf(sample)["text"]) ?? string.Empty;

    /// <summary>The value a multimodal field takes from a sample: its text and its files.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The value.</returns>
    private static MultimodalText SampleAsMultimodal(SelectData sample) =>
        new(SampleText(sample), FilesOf(ExampleOf(sample)));

    /// <summary>The conversation a clicked sample starts: one user message holding what it carries.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The conversation.</returns>
    private static List<ChatMessage> MessageOfExample(SelectData sample)
    {
        var example = ExampleOf(sample);
        var content = new List<ChatContent>();

        foreach (var file in FilesOf(example))
        {
            content.Add(ChatContent.FileBlock(file));
        }

        var text = JsonValues.Text(example["text"]);

        if (!string.IsNullOrEmpty(text))
        {
            content.Add(ChatContent.TextBlock(text));
        }

        return [new ChatMessage(content, ChatRole.User)];
    }

    /// <summary>What a sample carries as JSON.</summary>
    /// <param name="sample">The sample.</param>
    /// <returns>The sample's value, which is the object the browser drew.</returns>
    private static JsonObject ExampleOf(SelectData sample)
    {
        ArgumentNullException.ThrowIfNull(sample);

        return (sample.Raw as JsonObject)?["value"] as JsonObject ?? new JsonObject();
    }

    /// <summary>The files a sample carries.</summary>
    /// <param name="example">The sample.</param>
    /// <returns>The files, in the order they were given.</returns>
    /// <remarks>
    /// A file in a sample is either its path or the file object a browser sends, which is why both
    /// shapes are read here: the conversation's samples come back from the browser as the objects
    /// it drew.
    /// </remarks>
    private static List<FileData> FilesOf(JsonObject example)
    {
        var files = new List<FileData>();

        if (example["files"] is not JsonArray entries)
        {
            return files;
        }

        foreach (var entry in entries)
        {
            var path = entry switch
            {
                JsonObject described => JsonValues.Text(described["path"]),
                IReadOnlyDictionary<string, object?> described => JsonValues.Text(AsJson(described)["path"]),
                _ => JsonValues.Text(entry),
            };

            if (!string.IsNullOrEmpty(path))
            {
                files.Add(new FileData { Path = path });
            }
        }

        return files;
    }

    /// <summary>The chain a retry goes through: the last question again, and a new answer.</summary>
    /// <returns>The step that produced the answer, which is what a stop cancels.</returns>
    private Dependency AttachRetryChain()
    {
        var pop = Bind(
            Chatbot.Events.Retry,
            fn: (Func<object?, object?>)PopLastMessage, inputs: [ChatbotState], outputs: [ChatbotState, SavedInput], apiName: "_pop_last_user_message", apiVisibility: ApiVisibility.Undocumented, queue: false);

        var appendUser = Chain(
            pop,
            "_append_message_to_history",
            (Func<object?, object?, object?>)AppendUserMessage,
            [SavedInput, ChatbotState],
            [Chatbot],
            queue: false);

        var disabled = Chain(
            appendUser,
            "lambda",
            (Func<object?>)DisableField,
            [],
            [Textbox]);

        var answered = AnswerStep(disabled);

        var enabled = Chain(
            SyncConversation(answered),
            "lambda",
            (Func<object?>)EnableField,
            [],
            [Textbox]);

        SaveStep(enabled);

        return answered;
    }

    /// <summary>
    /// The bindings that make a streamed answer stoppable: the field's stop button, and the steps
    /// that swap the two buttons around a run.
    /// </summary>
    /// <param name="userSubmit">The step that emptied the field.</param>
    /// <param name="submitEvent">The step that produced the answer.</param>
    /// <param name="retryEvent">The step a retry produced.</param>
    /// <param name="exampleEvent">
    /// The step a clicked sample produced, or <see langword="null"/> when a click runs nothing.
    /// </param>
    /// <remarks>
    /// The field's stop signal carries one binding per chain, each cancelling its own, which is why
    /// the config holds two bindings on the same signal — the second being the cancellation itself,
    /// registered by the framework rather than by the chat. The Python version binds the two the
    /// same way.
    /// </remarks>
    private void AttachStopEvents(
        Dependency userSubmit,
        Dependency submitEvent,
        Dependency retryEvent,
        Dependency? exampleEvent)
    {
        _originalSubmitButton = SubmitButtonValue();

        // The stop button comes out when a run starts, and the submit button takes its place when
        // the run ends however it ends.
        var showStop = (Func<object?>)StartStopButton;
        var hideStop = (Func<object?>)StopStopButton;

        Success(userSubmit, "lambda", showStop, [], [Textbox], queue: false);

        Bind(
            Chatbot.Events.Retry,
            fn: showStop, inputs: [], outputs: [Textbox], apiName: "lambda", apiVisibility: ApiVisibility.Undocumented, queue: false);

        Chain(submitEvent, "lambda", hideStop, [], [Textbox], queue: false);
        Chain(retryEvent, "lambda", hideStop, [], [Textbox], queue: false);

        // A sample that runs is a run like any other: stopping it stops the answer it asked for,
        // and finishing it puts the two buttons back. The step hangs off the answer rather than off
        // the end of the chain, which is where the Python version hangs it.
        if (exampleEvent is not null)
        {
            Chain(exampleEvent, "lambda", hideStop, [], [Textbox], queue: false);
        }

        List<Dependency> cancellable = exampleEvent is null
            ? [submitEvent, retryEvent]
            : [submitEvent, retryEvent, exampleEvent];

        BindField(
            EventNames.Stop,
            null,
            [],
            [],
            apiVisibility: ApiVisibility.Undocumented, cancels: cancellable);
    }

    /// <summary>The binding an undo goes through: the last exchange taken back.</summary>
    private void AttachUndoEvents()
    {
        var pop = Bind(
            Chatbot.Events.Undo,
            fn: (Func<object?, object?>)PopLastMessage, inputs: [Chatbot], outputs: [Chatbot, Textbox], apiName: "_pop_last_user_message", apiVisibility: ApiVisibility.Undocumented, queue: false);

        var synchronized = SyncConversation(pop);

        SaveStep(synchronized);
    }

    /// <summary>The bindings an option a message offers goes through: it becomes the next question.</summary>
    private void AttachOptionEvents()
    {
        var clicked = Bind(
            Chatbot.Events.OptionSelect,
            fn: (Func<object?, SelectData, object?>)OptionClicked, inputs: [Chatbot], outputs: [Chatbot, SavedInput], apiName: "option_clicked", apiVisibility: ApiVisibility.Undocumented);

        SaveStep(SyncConversation(AnswerStep(clicked)));
    }

    /// <summary>The bindings clearing the conversation goes through.</summary>
    private void AttachClearEvents()
    {
        var cleared = Bind(
            Chatbot.Events.Clear,
            fn: (Func<object?, object?>)Synchronize, inputs: [Chatbot], outputs: [ChatbotState, ChatbotValue], apiName: "lambda", apiVisibility: ApiVisibility.Undocumented, queue: false);

        Chain(
            cleared,
            "_delete_conversation",
            (Func<object?, object?, object?>)DeleteConversation,
            [ConversationId, SavedConversations],
            [ConversationId, SavedConversations],
            queue: false);
    }

    /// <summary>The bindings editing a past message goes through, when the chat is editable.</summary>
    /// <remarks>
    /// An edit answers rather than merely changing what was said, so its steps are chained on
    /// success: a message that could not be answered is not one whose answer is written down.
    /// </remarks>
    private void AttachEditEvents()
    {
        var edited = Bind(
            Chatbot.Events.Edit,
            fn: (Func<object?, EditData, object?>)EditMessage, inputs: [Chatbot], outputs: [Chatbot, ChatbotState, SavedInput], apiName: "_edit_message", apiVisibility: ApiVisibility.Undocumented);

        var appendUser = Chain(
            edited,
            "_append_message_to_history",
            (Func<object?, object?, object?>)AppendUserMessage,
            [SavedInput, ChatbotState],
            [Chatbot],
            queue: false);

        var disabled = Success(
            appendUser,
            "lambda",
            (Func<object?>)DisableField,
            [],
            [Textbox]);

        var answered = Success(
            disabled,
            SubmitApiName,
            SubmitHandler(),
            [SavedInput, ChatbotState, .. _additionalInputs],
            [NullComponent, Chatbot, .. _additionalOutputs],
            showProgress: _showProgress,
            concurrencyLimit: _concurrencyLimit);

        var synchronized = Success(
            answered,
            "lambda",
            (Func<object?, object?>)Synchronize,
            [Chatbot],
            [ChatbotState, ChatbotValue],
            queue: false);

        var enabled = Chain(
            synchronized,
            "lambda",
            (Func<object?>)EnableField,
            [],
            [Textbox]);

        SaveStep(enabled);
    }

    /// <summary>The bindings that keep conversations and let a reader go back to one.</summary>
    private void AttachHistoryEvents()
    {
        var newChat = Bind(
            NewChatButton!.Events.Click,
            fn: (Func<object?>)StartConversation, inputs: [], outputs: [ConversationId, Chatbot], apiName: "lambda", apiVisibility: ApiVisibility.Undocumented, queue: false);

        Chain(
            newChat,
            "lambda",
            (Func<object?, object?>)Echo,
            [Chatbot],
            [ChatbotState],
            queue: false);

        // The list is filled when the page opens and whenever the browser's copy changes, which is
        // what makes a conversation saved in another tab appear in this one.
        _blocks.On(
            [_blocks.Loaded, SavedConversations.Events.Change],
            fn: (Func<object?, object?>)LoadChatHistory, inputs: [SavedConversations], outputs: [ChatHistory!], apiName: "_load_chat_history", apiVisibility: ApiVisibility.Undocumented, queue: false);

        var opened = Bind(
            ChatHistory!.Events.Click,
            fn: (Func<object?>)EmptyConversation, inputs: [], outputs: [Chatbot], apiName: "lambda", apiVisibility: ApiVisibility.Undocumented, queue: false, showProgress: ProgressDisplay.Hidden);

        var loaded = Chain(
            opened,
            "_load_conversation",
            (Func<object?, object?, object?>)LoadConversation,
            [ChatHistory, SavedConversations],
            [ConversationId, Chatbot],
            queue: false,
            showProgress: ProgressDisplay.Hidden);

        SyncConversation(loaded);
    }

    /// <summary>The binding that records a rating, when the chat offers ratings.</summary>
    /// <remarks>
    /// The ratings a chat offers are its feedback options, which is why the two parameters are the
    /// same list: a thumbs-up is a rating of the message it sits beside, and any other option is a
    /// flag under it.
    /// </remarks>
    private void AttachFlaggingEvents()
    {
        var logger = new ChatCsvLogger();
        logger.Setup(_flaggingDir);

        Chatbot.FeedbackOptions = _flaggingOptions;

        Bind(
            Chatbot.Events.Like,
            fn: (LikeData like, object? messages) => logger.Flag(like, messages), inputs: [Chatbot], outputs: [], apiName: "flag", parameterNames: ["messages"]);
    }

    /// <summary>The binding that puts a conversation a handler wrote into the conversation.</summary>
    /// <remarks>
    /// A handler outside the chat can write to the chat's value — that is what the state is
    /// published for — and the conversation has to follow, or the two would disagree about what was
    /// said.
    /// </remarks>
    private void AttachChatbotValueEvents()
    {
        var changed = Bind(
            ChatbotValue.Events.Change,
            fn: (Func<object?, object?>)Echo, inputs: [ChatbotValue], outputs: [Chatbot], apiName: "lambda", apiVisibility: ApiVisibility.Undocumented);

        SyncConversation(changed);
    }

    /// <summary>Binds one of the field's own signals, or a button's.</summary>
    /// <param name="eventName">The signal.</param>
    /// <param name="fn">The handler, or <see langword="null"/> for a binding with none.</param>
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
    /// <param name="target">The button to bind, or <see langword="null"/> for the field.</param>
    /// <returns>The binding.</returns>
    /// <remarks>
    /// The field is a <see cref="Textbox"/> or a <see cref="MultimodalTextbox"/> depending on the
    /// chat, and both raise the same signals; binding through the trigger rather than through one
    /// of the two typed extensions is what keeps this code from having to say which it is.
    /// </remarks>
    private Dependency BindField(
        string eventName,
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
        Delegate? validator = null,
        Block? target = null) =>
        BindField(eventName, fn, inputs, outputs, new EventBindingOptions
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
        }, target);

    private Dependency BindField(
        string eventName,
        Delegate? fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        EventBindingOptions options,
        Block? target = null) =>
        Bind(new EventTrigger(target ?? Textbox, eventName), options, fn, inputs, outputs);

    /// <summary>Binds one of a component's own signals.</summary>
    /// <param name="trigger">The signal.</param>
    /// <param name="fn">The handler, or <see langword="null"/> for a frontend-only binding.</param>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="parameterNames">The names the API document gives the handler's parameters.</param>
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
    /// A component's own event is not a merged trigger set, and the config tells the two apart: a
    /// merged set reports an event-specific argument list — empty when no event contributes a name
    /// — while a binding to a component's own signal reports nothing at all. That is why this goes
    /// through <c>EventBinding.Bind</c> rather than through <c>Blocks.On</c>: the
    /// latter is the Python version's <c>gr.on</c>, which is what an application uses to merge two
    /// signals, not what a chat uses to listen to one.
    /// </remarks>
    private static Dependency Bind(
        EventTrigger trigger,
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
        Bind(trigger, new EventBindingOptions
        {
            Fn = fn,
            Inputs = [.. inputs ?? []],
            Outputs = [.. outputs ?? []],
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
        });

    private static Dependency Bind(EventTrigger trigger, EventBindingOptions options) =>
        Bind(trigger, options, options.Fn, BlocksOf(options.Inputs), BlocksOf(options.Outputs));

    /// <summary>Binds one signal to a handler the caller names itself.</summary>
    /// <param name="trigger">The signal.</param>
    /// <param name="options">The binding parameters, for the visibility and the queue setting.</param>
    /// <param name="fn">The handler, or <see langword="null"/>.</param>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <returns>The binding.</returns>
    private static Dependency Bind(
        EventTrigger trigger,
        EventBindingOptions options,
        Delegate? fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs) =>
        EventBinding.Bind([trigger], fn, inputs, outputs, options);

    /// <summary>The blocks a binding parameter names as inputs or outputs.</summary>
    /// <param name="components">The components, as the parameter took them.</param>
    /// <returns>The blocks.</returns>
    private static List<Block> BlocksOf(IReadOnlyList<object> components) =>
        EventBinding.BlocksOf(components, "components");

    /// <summary>Adds the step that answers a message, with the handler the chat wraps.</summary>
    /// <param name="after">The step this one follows.</param>
    /// <returns>The step, which is the one a stop cancels.</returns>
    /// <remarks>
    /// Every chain that answers a message — a submit, a retry, a clicked sample, a clicked option —
    /// goes through this step, so the answer is described the same way in each: the message and the
    /// conversation the step before it produced, one value per additional input, and the chat's own
    /// concurrency limit.
    /// </remarks>
    private Dependency AnswerStep(Dependency after) =>
        Step(
            after.Events.Then,
            SubmitApiName,
            SubmitHandler(),
            [SavedInput, ChatbotState, .. _additionalInputs],
            [NullComponent, Chatbot, .. _additionalOutputs],
            queue: true,
            showProgress: _showProgress,
            concurrencyLimit: _concurrencyLimit,
            parameterNames: SubmitParameterNames());

    /// <summary>Adds the step that hands a conversation on to both of the states that keep it.</summary>
    /// <param name="after">The step this one follows.</param>
    /// <returns>The step.</returns>
    private Dependency SyncConversation(Dependency after) =>
        Chain(
            after,
            "lambda",
            (Func<object?, object?>)Synchronize,
            [Chatbot],
            [ChatbotState, ChatbotValue],
            queue: false);

    /// <summary>Adds the step that records the conversation the reader is having.</summary>
    /// <param name="after">The step this one follows.</param>
    /// <returns>The step.</returns>
    private Dependency SaveStep(Dependency after) =>
        Chain(
            after,
            "_save_conversation",
            (Func<object?, object?, object?, object?>)SaveConversation,
            [ConversationId, ChatbotState, SavedConversations],
            [ConversationId, SavedConversations],
            queue: false);

    /// <summary>Adds a step that runs when the one before it completes.</summary>
    /// <param name="after">The step this one follows.</param>
    /// <param name="apiName">The name the step is served under.</param>
    /// <param name="fn">The handler.</param>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="queue">Whether the step goes through the queue.</param>
    /// <param name="showProgress">How much of the progress animation the step shows, or <see langword="null"/> for all of it.</param>
    /// <param name="concurrencyLimit">How many of the step may run at once, or the application's default.</param>
    /// <returns>The step.</returns>
    private static Dependency Chain(
        Dependency after,
        string apiName,
        Delegate fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        bool queue = true,
        ProgressDisplay? showProgress = null,
        ConcurrencyLimit concurrencyLimit = default) =>
        Step(after.Events.Then, apiName, fn, inputs, outputs, queue, showProgress, concurrencyLimit);

    /// <summary>Adds a step that runs only when the one before it succeeded.</summary>
    /// <param name="after">The step this one follows.</param>
    /// <param name="apiName">The name the step is served under.</param>
    /// <param name="fn">The handler.</param>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="queue">Whether the step goes through the queue.</param>
    /// <param name="showProgress">How much of the progress animation the step shows, or <see langword="null"/> for all of it.</param>
    /// <param name="concurrencyLimit">How many of the step may run at once, or the application's default.</param>
    /// <returns>The step.</returns>
    private static Dependency Success(
        Dependency after,
        string apiName,
        Delegate fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        bool queue = true,
        ProgressDisplay? showProgress = null,
        ConcurrencyLimit concurrencyLimit = default) =>
        Step(after.Events.Success, apiName, fn, inputs, outputs, queue, showProgress, concurrencyLimit);

    /// <summary>Adds one step of a chain.</summary>
    /// <param name="trigger">The signal the step listens for.</param>
    /// <param name="apiName">The name the step is served under.</param>
    /// <param name="fn">The handler.</param>
    /// <param name="inputs">The components the handler reads.</param>
    /// <param name="outputs">The components the handler's result updates.</param>
    /// <param name="queue">Whether the step goes through the queue.</param>
    /// <param name="showProgress">How much of the progress animation the step shows, or <see langword="null"/> for all of it.</param>
    /// <param name="concurrencyLimit">How many of the step may run at once, or the application's default.</param>
    /// <param name="parameterNames">The names its arguments are documented under, or <see langword="null"/> for none.</param>
    /// <returns>The step.</returns>
    private static Dependency Step(
        EventTrigger trigger,
        string apiName,
        Delegate fn,
        IReadOnlyList<Block> inputs,
        IReadOnlyList<Block> outputs,
        bool queue,
        ProgressDisplay? showProgress,
        ConcurrencyLimit concurrencyLimit = default,
        IReadOnlyList<string?>? parameterNames = null) =>
        Bind(
            trigger,
            fn: fn, inputs: [.. inputs], outputs: [.. outputs], parameterNames: parameterNames, apiName: apiName, apiVisibility: ApiVisibility.Undocumented, queue: queue, showProgress: showProgress, concurrencyLimit: concurrencyLimit);

    /// <summary>The handler a submit runs, whether or not it answers a chunk at a time.</summary>
    /// <returns>
    /// A delegate that takes the message, the conversation and the additional values, and the
    /// run's request.
    /// </returns>
    private Delegate SubmitHandler() => StreamsResult
        ? CollectingDelegate.OfStreamingWithRequest(2 + _additionalInputs.Count, StreamSubmit)
        : CollectingDelegate.OfWithRequest(2 + _additionalInputs.Count, Submit);

    /// <summary>The handler the API endpoint runs: the same answer, without the drawing.</summary>
    /// <returns>
    /// A delegate that takes the message, the conversation and the additional values, and the
    /// run's request.
    /// </returns>
    private Delegate ApiHandler() => StreamsResult
        ? CollectingDelegate.OfStreamingWithRequest(2 + _additionalInputs.Count, StreamApi)
        : CollectingDelegate.OfWithRequest(2 + _additionalInputs.Count, Api);

    /// <summary>The names a submit's arguments are documented under.</summary>
    /// <returns>The message, the conversation, then nothing for each additional input.</returns>
    /// <remarks>
    /// The Python version's <c>_submit_fn</c> names its two arguments and takes the rest as
    /// <c>*args</c>, and the API document is built from that signature: the message and the
    /// conversation are named, and an additional input falls back to its position. The delegate
    /// here takes its values as a list, so the names are stated rather than read off it.
    /// </remarks>
    private List<string?> SubmitParameterNames() =>
    [
        "message",
        "history",
        .. Enumerable.Repeat<string?>(null, _additionalInputs.Count),
    ];

    /// <summary>The name the handler is served under when the application names none.</summary>
    /// <returns>The handler's method name, or <c>lambda</c> for one the compiler generated.</returns>
    private string FunctionName()
    {
        var name = _fn.Method.Name;

        return name.Length == 0 || name.Contains('<') ? "lambda" : name;
    }

    /// <summary>Calls the application's handler with the values a step produced.</summary>
    /// <param name="values">The message, the conversation, then one value per additional input.</param>
    /// <param name="request">Who is asking, for a handler that reads the run's request.</param>
    /// <returns>What the handler answered.</returns>
    /// <remarks>
    /// The parameters the framework fills itself are recognised by type, which is the same rule the
    /// queue applies to a binding's own handler: a progress tracker, a cancellation token or the
    /// run's request is not something any input carries.
    /// </remarks>
    private async Task<object?> CallAsync(object?[] values, Request request)
    {
        var parameters = DelegateInvocation.ParametersOf(_fn);
        var arguments = new object?[parameters.Length];
        var next = 0;

        for (var index = 0; index < parameters.Length; index++)
        {
            var type = parameters[index].ParameterType;

            if (type == typeof(Progress))
            {
                arguments[index] = new Progress();
            }
            else if (type == typeof(CancellationToken))
            {
                arguments[index] = CancellationToken.None;
            }
            else if (type == typeof(Request))
            {
                arguments[index] = request;
            }
            else if (next < values.Length)
            {
                arguments[index] = values[next++];
            }
        }

        return await DelegateInvocation
            .InvokeAsync(_fn, arguments, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>Answers one message.</summary>
    /// <param name="values">The message, the conversation, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>The answer and the conversation it belongs to, as one value per output.</returns>
    private async Task<object?> Submit(object?[] values, Request request)
    {
        var message = values[0];
        var history = MessagesOf(values[1]);
        var additional = values[2..];

        var response = await CallAsync([MessageForHandler(message), history, .. additional], request)
            .ConfigureAwait(false);

        var (answer, rest) = SplitAdditional(response);
        var answered = Append(answer, Append(message, history, ChatRole.User), ChatRole.Assistant);

        return EventValues.Of([answer, answered, .. rest]);
    }

    /// <summary>Answers one message a chunk at a time.</summary>
    /// <param name="values">The message, the conversation, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>Each chunk and the conversation as it stands after it.</returns>
    private async IAsyncEnumerable<object?> StreamSubmit(object?[] values, Request request)
    {
        var message = values[0];
        var history = MessagesOf(values[1]);
        var additional = values[2..];

        var asked = Append(message, history, ChatRole.User);
        var response = await CallAsync([MessageForHandler(message), history, .. additional], request)
            .ConfigureAwait(false);

        if (response is not IAsyncEnumerable<object?> chunks)
        {
            var (whole, rest) = SplitAdditional(response);

            yield return EventValues.Of([whole, Append(whole, asked, ChatRole.Assistant), .. rest]);

            yield break;
        }

        var answered = false;

        await foreach (var chunk in chunks.ConfigureAwait(false))
        {
            answered = true;
            var (piece, rest) = SplitAdditional(chunk);

            yield return EventValues.Of([piece, Append(piece, asked, ChatRole.Assistant), .. rest]);
        }

        if (!answered)
        {
            // A stream that ended without producing anything still has to leave the conversation
            // with the question in it, which is what the Python version's empty-generator branch does.
            yield return EventValues.Of([null, asked]);
        }
    }

    /// <summary>Answers one message for the API endpoint, without drawing anything.</summary>
    /// <param name="values">The message, the conversation, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>The answer and the conversation, as JSON.</returns>
    private async Task<object?> Api(object?[] values, Request request)
    {
        var message = values[0];
        var history = MessagesOf(values[1]);
        var additional = values[2..];

        var response = await CallAsync([MessageForHandler(message), history, .. additional], request)
            .ConfigureAwait(false);

        var (answer, rest) = SplitAdditional(response);
        var answered = Append(answer, Append(message, history, ChatRole.User), ChatRole.Assistant);

        return EventValues.Of([answer, MessagesToJson(answered), .. rest]);
    }

    /// <summary>Answers one message for the API endpoint, a chunk at a time.</summary>
    /// <param name="values">The message, the conversation, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>Each chunk and the conversation as it stands after it, as JSON.</returns>
    private async IAsyncEnumerable<object?> StreamApi(object?[] values, Request request)
    {
        var message = values[0];
        var history = MessagesOf(values[1]);
        var additional = values[2..];

        var asked = Append(message, history, ChatRole.User);
        var response = await CallAsync([MessageForHandler(message), history, .. additional], request)
            .ConfigureAwait(false);

        if (response is not IAsyncEnumerable<object?> chunks)
        {
            var (whole, rest) = SplitAdditional(response);

            yield return EventValues.Of(
                [whole, MessagesToJson(Append(whole, asked, ChatRole.Assistant)), .. rest]);

            yield break;
        }

        await foreach (var chunk in chunks.ConfigureAwait(false))
        {
            var (piece, rest) = SplitAdditional(chunk);

            yield return EventValues.Of(
                [piece, MessagesToJson(Append(piece, asked, ChatRole.Assistant)), .. rest]);
        }
    }

    /// <summary>Splits a handler's answer into the message and the additional outputs.</summary>
    /// <param name="response">What the handler answered.</param>
    /// <returns>The answer, and the values the additional outputs take.</returns>
    /// <remarks>
    /// A chat with additional outputs answers with a list: the message first, then one value per
    /// output. A chat without them answers with the message alone, whatever shape that is.
    /// </remarks>
    private (object? Answer, object?[] Additional) SplitAdditional(object? response)
    {
        if (_additionalOutputs.Count == 0)
        {
            return (response, []);
        }

        if (response is not System.Collections.IList values || values.Count == 0)
        {
            throw new ArgumentException(
                "A chat with additional outputs answers with a list: the message first, then one "
                + "value per additional output.",
                nameof(response));
        }

        return (values[0], [.. values.Cast<object?>().Skip(1)]);
    }

    /// <summary>Empties the field and keeps the message that was in it.</summary>
    /// <param name="message">What the user sent.</param>
    /// <returns>The update that empties the field, and the message.</returns>
    private object? ClearAndSave(object? message) => EventValues.Of([ClearField(), message]);

    /// <summary>The update that empties the field, with the message gone.</summary>
    /// <returns>The update.</returns>
    /// <remarks>
    /// The value is what the field's own postprocessing makes of an empty text, so a multimodal
    /// field is emptied with the object it holds rather than with a bare string.
    /// </remarks>
    private JsonObject ClearField() => PropertyUpdate.Partial(
        ("value", Textbox.Postprocess(string.Empty)),
        ("interactive", JsonValue.Create(false)),
        ("placeholder", JsonValue.Create(string.Empty)));

    /// <summary>The update that empties the field and makes it usable again.</summary>
    /// <returns>The update.</returns>
    private JsonObject EnableField() => PropertyUpdate.Partial(
        ("value", null),
        ("interactive", JsonValue.Create(true)));

    /// <summary>The update that makes the field unusable while an answer is being produced.</summary>
    /// <returns>The update.</returns>
    private JsonObject DisableField() => PropertyUpdate.Partial(
        ("interactive", JsonValue.Create(false)),
        ("placeholder", JsonValue.Create(string.Empty)));

    /// <summary>The update that puts the stop button out and the submit button back.</summary>
    /// <returns>The update.</returns>
    private JsonObject StartStopButton() => PropertyUpdate.Partial(
        ("submit_btn", JsonValue.Create(false)),
        ("stop_btn", JsonValue.Create(_originalStopButton)));

    /// <summary>The update that puts the submit button back and the stop button out.</summary>
    /// <returns>The update.</returns>
    private JsonObject StopStopButton() => PropertyUpdate.Partial(
        ("submit_btn", JsonValue.Create(_originalSubmitButton)),
        ("stop_btn", JsonValue.Create(false)));

    /// <summary>Adds the user's message to the conversation.</summary>
    /// <param name="message">The message.</param>
    /// <param name="history">The conversation.</param>
    /// <returns>The conversation with the message at the end.</returns>
    private object? AppendUserMessage(object? message, object? history) =>
        Append(message, MessagesOf(history), ChatRole.User);

    /// <summary>Hands a conversation on to both of the states that keep it.</summary>
    /// <param name="value">The conversation.</param>
    /// <returns>The conversation, twice.</returns>
    private object? Synchronize(object? value)
    {
        var conversation = MessagesToJson(MessagesOf(value));

        return EventValues.Of([conversation, conversation]);
    }

    /// <summary>Hands a conversation on unchanged.</summary>
    /// <param name="value">The conversation.</param>
    /// <returns>The conversation as messages.</returns>
    private object? Echo(object? value) => MessagesOf(value);

    /// <summary>Takes the last exchange back and fills the field with the question again.</summary>
    /// <param name="history">The conversation.</param>
    /// <returns>The shortened conversation, and the message the field is filled with.</returns>
    private object? PopLastMessage(object? history)
    {
        var (shortened, message) = PopLastUserMessage(MessagesOf(history));

        return EventValues.Of([MessagesToJson(shortened), message]);
    }

    /// <summary>Makes the option a user picked the next question.</summary>
    /// <param name="history">The conversation.</param>
    /// <param name="option">The option that was picked.</param>
    /// <returns>The conversation with the option in it, and the option as the field's value.</returns>
    private object? OptionClicked(object? history, SelectData option)
    {
        ArgumentNullException.ThrowIfNull(option);

        var conversation = new List<ChatMessage>(MessagesOf(history))
        {
            new(option.Value?.ToString() ?? string.Empty, ChatRole.User),
        };

        return EventValues.Of([conversation, option.Value]);
    }

    /// <summary>Cuts the conversation back to where a message was edited.</summary>
    /// <param name="history">The conversation.</param>
    /// <param name="edit">Where the edit is, and what the message now says.</param>
    /// <returns>The shortened conversation, twice, and the edited text as the field's value.</returns>
    private object? EditMessage(object? history, EditData edit)
    {
        ArgumentNullException.ThrowIfNull(edit);

        var conversation = MessagesOf(history);
        var cut = edit.Index is { } index ? conversation.Take(index).ToArray() : [.. conversation];

        return EventValues.Of([cut, MessagesToJson(cut), edit.Value]);
    }

    /// <summary>Records the conversation the user is having.</summary>
    /// <param name="index">Which saved conversation is open, or <see langword="null"/> for a new one.</param>
    /// <param name="conversation">The conversation.</param>
    /// <param name="saved">The conversations the browser has kept.</param>
    /// <returns>The index the conversation is saved under, and the list.</returns>
    private object? SaveConversation(object? index, object? conversation, object? saved)
    {
        var conversations = new List<object?>(ConversationsOf(saved));

        if (!_saveHistory)
        {
            return EventValues.Of([index, conversations]);
        }

        var document = MessagesToJson(MessagesOf(conversation));

        if (IndexOf(index) is { } position && position >= 0 && position < conversations.Count)
        {
            conversations[position] = document;

            return EventValues.Of([position, conversations]);
        }

        // A conversation that was not open yet is the newest one, which is what puts it at the top
        // of the list the reader sees.
        conversations.Insert(0, document);

        return EventValues.Of([0, conversations]);
    }

    /// <summary>Forgets the conversation that was just cleared.</summary>
    /// <param name="index">Which saved conversation was open, or <see langword="null"/> for none.</param>
    /// <param name="saved">The conversations the browser has kept.</param>
    /// <returns>No open conversation, and the list without the one that was open.</returns>
    private object? DeleteConversation(object? index, object? saved)
    {
        var conversations = new List<object?>(ConversationsOf(saved));

        if (IndexOf(index) is { } position && position >= 0 && position < conversations.Count)
        {
            conversations.RemoveAt(position);
        }

        return EventValues.Of([null, conversations]);
    }

    /// <summary>The conversations a browser kept, whatever shape the state handed back.</summary>
    /// <param name="saved">The value the state holds.</param>
    /// <returns>The conversations, in the order they are listed.</returns>
    private static IEnumerable<object?> ConversationsOf(object? saved) => saved switch
    {
        null => [],
        JsonArray array => [.. array],
        IEnumerable<object?> entries => entries,
        _ => [],
    };

    /// <summary>Lists the conversations the browser kept, by the first thing said in each.</summary>
    /// <param name="saved">The conversations.</param>
    /// <returns>The update that fills the list.</returns>
    private JsonObject? LoadChatHistory(object? saved)
    {
        var samples = new JsonArray();

        foreach (var conversation in ConversationsOf(saved))
        {
            var messages = MessagesOf(conversation);

            if (messages.Count > 0)
            {
                samples.Add(new JsonArray(GenerateChatTitle(messages)));
            }
        }

        return PropertyUpdate.Partial(("samples", samples));
    }

    /// <summary>Opens the conversation a reader picked.</summary>
    /// <param name="index">Which entry of the list was picked.</param>
    /// <param name="saved">The conversations the browser kept.</param>
    /// <returns>The index that is now open, and the conversation.</returns>
    private object? LoadConversation(object? index, object? saved)
    {
        var conversations = ConversationsOf(saved).ToArray();
        var position = IndexOf(index) ?? 0;

        if (position < 0 || position >= conversations.Length)
        {
            return EventValues.Of([null, new JsonArray()]);
        }

        return EventValues.Of([position, MessagesToJson(MessagesOf(conversations[position]))]);
    }

    /// <summary>A saved conversation's position, whatever shape the state or the list handed back.</summary>
    /// <param name="value">The value: a number, a JSON number, or nothing.</param>
    /// <returns>The position, or <see langword="null"/> when the value names none.</returns>
    /// <remarks>
    /// A state hands its value back as the CLR shape its JSON described, and a number read out of
    /// JSON is a <see cref="double"/> — so an index that came from the browser, or from a step
    /// before this one, is not an <see cref="int"/> by the time it arrives. Reading it through the
    /// number it is, rather than through the type it was built as, is what keeps a reopened
    /// conversation from being saved a second time as a new one.
    /// </remarks>
    private static int? IndexOf(object? value) => value switch
    {
        int whole => whole,
        long wide => (int)wide,
        double number => (int)number,
        decimal precise => (int)precise,
        JsonValue node => JsonValues.Number(node) is { } number ? (int)number : null,
        _ => null,
    };

    /// <summary>Empties the conversation, so that the one being opened is not drawn on top of it.</summary>
    /// <returns>An empty conversation.</returns>
    private static object? EmptyConversation() => Array.Empty<ChatMessage>();

    /// <summary>Starts a new conversation: no open one, and an empty conversation.</summary>
    /// <returns>No open conversation, and the empty conversation.</returns>
    private static object? StartConversation() => EventValues.Of([null, Array.Empty<ChatMessage>()]);
}

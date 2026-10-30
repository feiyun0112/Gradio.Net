namespace Gradio.Net;
/// <summary>
/// Applications built from something that already exists: a chat endpoint that speaks the OpenAI
/// protocol, or a service described by an OpenAPI document.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gradio.external</c>, minus the part of it that loads from
/// Hugging Face. What is here is the part that needs nothing but HTTP: a chat UI for any endpoint
/// that answers like OpenAI's, and a page of forms for any service that publishes an OpenAPI 3
/// document. Both build ordinary blocks through the public API, so a page either of them builds can
/// be taken apart and rebuilt by hand.
/// </para>
/// <para>
/// Like <see cref="Interface"/> and <see cref="ChatInterface"/>, both render into an application
/// rather than being one: the render root is resolved from the container, one container hosting one
/// application, so the loader is handed the application to render into.
/// </para>
/// <para>
/// Neither loader has been run against a third-party service end to end. What is verified is the
/// request each one builds and what it does with an answer: the tests replay a recorded
/// OpenAI-compatible response and serve a static OpenAPI document, so the shapes are checked and the
/// network is not. A real endpoint that speaks the protocol is expected to work, and
/// whether it does is the operator's to find out.
/// </para>
/// <para>
/// Hugging Face is not offered: <c>load</c>, <c>from_model</c> and <c>from_spaces</c> are outside
/// this port's scope, so nothing here reads <c>HF_TOKEN</c> or contacts
/// <c>huggingface.co</c>, and no Hugging Face SDK is referenced.
/// </para>
/// </remarks>
public static partial class External
{
    /// <summary>The API name a loaded chat's endpoint is served under when it streams.</summary>
    /// <remarks>
    /// The Python version's handler is a Python function called <c>open_api_stream</c>, and the
    /// endpoint takes its name from the function. A delegate here has no such name to give, so the
    /// name the Python version would have derived is written out — the served document is what a
    /// client reads.
    /// </remarks>
    private const string StreamingApiName = "open_api_stream";

    /// <summary>The API name a loaded chat's endpoint is served under when it does not stream.</summary>
    private const string AnsweringApiName = "open_api";

    /// <summary>
    /// Wraps an OpenAI-compatible chat endpoint in a conversation: type a message, get an answer.
    /// </summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="baseUrl">
    /// The endpoint's base URL, e.g. <c>http://localhost:11434/v1/</c>. The request goes to
    /// <c>chat/completions</c> under it, and a base URL that does not end in a slash is completed
    /// with one first, which is what the OpenAI client does with the address it is given.
    /// </param>
    /// <param name="model">The model to ask for, as the endpoint names it, e.g. <c>llama3.2</c>.</param>
    /// <param name="token">
    /// The key to send as a bearer token, or <see langword="null"/> for the placeholder the OpenAI
    /// client sends when it is given none — which is what a local server that checks nothing
    /// expects.
    /// </param>
    /// <param name="fileTypes">
    /// What the field accepts besides text. <see cref="ChatFileType.TextEncoded"/> (the default)
    /// accepts text files, which are read and appended to the prompt;
    /// <see cref="ChatFileType.Image"/> adds images, which are sent as data URLs and need a model
    /// that can see them; <see cref="ChatFileType.None"/> accepts no files at all, and the chat is
    /// then an ordinary text chat.
    /// </param>
    /// <param name="systemMessage">
    /// The instruction the conversation opens with, or <see langword="null"/> for none. It is sent
    /// with every request rather than only the first, because it is an instruction about the whole
    /// conversation.
    /// </param>
    /// <param name="streaming">
    /// Whether the answer is shown as it arrives. A streaming answer needs an endpoint that streams;
    /// one that does not is asked for the whole answer at once.
    /// </param>
    /// <param name="chatbot">The conversation component to use, or <see langword="null"/> for one
    /// that scales and keeps HTML tags.</param> <param name="textbox">The field to type into, or
    /// <see langword="null"/> for one built from <paramref name="fileTypes"/>.</param>
    /// <param name="additionalInputs">The components the endpoint is not told about but the page
    /// draws, or <see langword="null"/> for none.</param>
    /// <param name="additionalInputsAccordion">The accordion those components are drawn in, or
    /// <see langword="null"/> for the default one.</param> <param name="additionalOutputs">The
    /// components the answer's remaining values fill, or <see langword="null"/> for none.</param>
    /// <param name="editable">Whether a past message may be edited and answered again.</param>
    /// <param name="examples">The messages offered as starting points, or <see langword="null"/>
    /// for none.</param> <param name="examplesLabel">The label of the gallery the examples are
    /// drawn in, or <see langword="null"/> for none.</param> <param name="exampleLabels">The labels
    /// shown instead of the samples, or <see langword="null"/> for none.</param>
    /// <param name="exampleIcons">The pictures shown beside the samples, or <see langword="null"/>
    /// for none.</param> <param name="runExamplesOnClick">Whether clicking a sample sends it, or
    /// only fills the field.</param> <param name="cacheExamples">Whether the samples' answers are
    /// kept, or <see langword="null"/> to read the environment.</param>
    /// <param name="cacheMode"><c>eager</c> or <c>lazy</c>, or <see langword="null"/> to read the
    /// environment.</param> <param name="title">The heading above the conversation, or
    /// <see langword="null"/> for none.</param> <param name="description">The text under the
    /// heading, or <see langword="null"/> for none.</param>
    /// <param name="flaggingMode"><c>manual</c>, <c>never</c>, or <see langword="null"/> to read
    /// the environment.</param> <param name="flaggingOptions">The ratings offered, or
    /// <see langword="null"/> for like and dislike.</param> <param name="flaggingDir">Where flagged
    /// conversations are written.</param> <param name="analyticsEnabled">Whether telemetry is on,
    /// or <see langword="null"/> for the application's setting.</param>
    /// <param name="autofocus">Whether the field has the focus when the page loads.</param>
    /// <param name="autoscroll">Whether the conversation scrolls to its last message.</param>
    /// <param name="submitBtn">The field's send button: a flag or its text, or
    /// <see langword="null"/> for the default.</param> <param name="stopBtn">The field's stop
    /// button: a flag or its text, or <see langword="null"/> for the default.</param>
    /// <param name="concurrencyLimit">How many answers may be worked on at once.</param>
    /// <param name="deleteCache">When the temporary files are cleaned up, or <see langword="null"/>
    /// for never.</param> <param name="showProgress">What the page shows while an answer is being
    /// worked out.</param> <param name="fillHeight">Whether the conversation fills the page's
    /// height.</param> <param name="fillWidth">Whether the page fills its container's
    /// width.</param> <param name="apiName">The name the endpoint is served under, or
    /// <see langword="null"/> for the Python version's.</param> <param name="apiDescription">The
    /// endpoint's description in the API document, or <see langword="null"/> for none.</param>
    /// <param name="apiVisibility">Who may call the endpoint through the API.</param>
    /// <param name="saveHistory">Whether conversations are kept and listed beside the chat.</param>
    /// <param name="validator">A handler that refuses an input before the endpoint is asked, or
    /// <see langword="null"/> for none.</param> <param name="httpClient">The client to ask the
    /// endpoint with, or <see langword="null"/> for one of the chat's own.</param> <returns>The
    /// chat, once it has been rendered into <paramref name="blocks"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="baseUrl"/> or <paramref name="model"/> is empty, or
    /// <paramref name="fileTypes"/> names a kind that does not exist.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The parameters after <paramref name="streaming"/> are <see cref="ChatInterface"/>'s, because
    /// the Python version forwards its remaining keyword arguments to the chat it builds: a loaded
    /// chat is a chat, and everything a chat can be told can be told here.
    /// </para>
    /// <para>
    /// Two things are done differently from the Python version 6.30.0, both of them cases where the
    /// release cannot do what its own parameter says. A chat loaded with a system message raises on
    /// its first message the Python version: the message is built with a text for its content and
    /// then read back as a list of blocks, which a text is not. Here it is sent as a text message,
    /// and with every request rather than only the first. And a field that is handed to a
    /// multimodal chat is used as it is the Python version even when it cannot take files; here
    /// <see cref="ChatInterface"/> is what decides, as it does for a chat built by hand.
    /// </para>
    /// </remarks>
    public static ChatInterface LoadChat(
        Blocks blocks,
        string baseUrl,
        string model,
        string? token = null,
        ChatFileType fileTypes = ChatFileType.TextEncoded,
        string? systemMessage = null,
        bool streaming = true,
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
        Delegate? validator = null,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        var kinds = KindsOf(fileTypes);
        var multimodal = kinds != ChatFileType.None;
        var endpoint = new ChatEndpoint(baseUrl, model, token, httpClient);

        return new ChatInterface(
            blocks,
            Handler(endpoint, systemMessage, multimodal, streaming),
            multimodal: multimodal,
            chatbot: chatbot ?? new Chatbot { Scale = 1, AllowTags = true },
            textbox: textbox ?? (multimodal ? new MultimodalTextbox { FileTypes = ExtensionsOf(kinds) } : null),
            additionalInputs: additionalInputs,
            additionalInputsAccordion: additionalInputsAccordion,
            additionalOutputs: additionalOutputs,
            editable: editable,
            examples: examples,
            examplesLabel: examplesLabel,
            exampleLabels: exampleLabels,
            exampleIcons: exampleIcons,
            runExamplesOnClick: runExamplesOnClick,
            cacheExamples: cacheExamples,
            cacheMode: cacheMode,
            title: title,
            description: description,
            flaggingMode: flaggingMode,
            flaggingOptions: flaggingOptions,
            flaggingDir: flaggingDir,
            analyticsEnabled: analyticsEnabled,
            autofocus: autofocus,
            autoscroll: autoscroll,
            submitBtn: submitBtn,
            stopBtn: stopBtn,
            concurrencyLimit: concurrencyLimit,
            deleteCache: deleteCache,
            showProgress: showProgress,
            fillHeight: fillHeight,
            fillWidth: fillWidth,
            apiName: apiName ?? (streaming ? StreamingApiName : AnsweringApiName),
            apiDescription: apiDescription,
            apiVisibility: apiVisibility,
            saveHistory: saveHistory,
            validator: validator);
    }

    /// <summary>The handler the chat runs, in the shape the chat reads its message in.</summary>
    /// <param name="endpoint">The endpoint to ask.</param>
    /// <param name="systemMessage">The instruction the conversation opens with, or <see langword="null"/>.</param>
    /// <param name="multimodal">Whether the field hands over a text with files rather than a text.</param>
    /// <param name="streaming">Whether the answer is wanted a piece at a time.</param>
    /// <returns>The handler.</returns>
    /// <remarks>
    /// The four combinations are four delegate types rather than one that casts: a chat checks its
    /// handler's signature to decide what to hand it — a text or a text with files — and to decide
    /// whether the answer is a stream, which the config serves as <c>types.generator</c>.
    /// </remarks>
    private static Delegate Handler(
        ChatEndpoint endpoint,
        string? systemMessage,
        bool multimodal,
        bool streaming)
    {
        if (multimodal)
        {
            return streaming
                ? (Func<MultimodalText, IReadOnlyList<ChatMessage>, IAsyncEnumerable<string>>)(
                    (message, history) => InPieces(endpoint, systemMessage, message, history))
                : (Func<MultimodalText, IReadOnlyList<ChatMessage>, Task<string>>)(
                    (message, history) => Answer(endpoint, systemMessage, message, history));
        }

        return streaming
            ? (Func<string, IReadOnlyList<ChatMessage>, IAsyncEnumerable<string>>)(
                (message, history) => InPieces(endpoint, systemMessage, message, history))
            : (Func<string, IReadOnlyList<ChatMessage>, Task<string>>)(
                (message, history) => Answer(endpoint, systemMessage, message, history));
    }

    /// <summary>Asks the endpoint and waits for the whole answer.</summary>
    /// <param name="endpoint">The endpoint to ask.</param>
    /// <param name="systemMessage">The instruction the conversation opens with, or <see langword="null"/>.</param>
    /// <param name="message">What the user sent.</param>
    /// <param name="history">The conversation so far.</param>
    /// <returns>The answer.</returns>
    private static async Task<string> Answer(
        ChatEndpoint endpoint,
        string? systemMessage,
        object? message,
        IReadOnlyList<ChatMessage> history)
    {
        var conversation = await ChatConversation
            .FormatAsync(history, message, systemMessage, endpoint.Client, CancellationToken.None)
            .ConfigureAwait(false);

        return await endpoint.AskAsync(conversation, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>Asks the endpoint and passes the answer on as it arrives.</summary>
    /// <param name="endpoint">The endpoint to ask.</param>
    /// <param name="systemMessage">The instruction the conversation opens with, or <see langword="null"/>.</param>
    /// <param name="message">What the user sent.</param>
    /// <param name="history">The conversation so far.</param>
    /// <returns>The answer so far, after each piece.</returns>
    private static async IAsyncEnumerable<string> InPieces(
        ChatEndpoint endpoint,
        string? systemMessage,
        object? message,
        IReadOnlyList<ChatMessage> history)
    {
        var conversation = await ChatConversation
            .FormatAsync(history, message, systemMessage, endpoint.Client, CancellationToken.None)
            .ConfigureAwait(false);

        await foreach (var piece in endpoint
            .StreamAsync(conversation, CancellationToken.None)
            .ConfigureAwait(false))
        {
            yield return piece;
        }
    }

    /// <summary>The kinds a chat accepts, checked against the kinds that exist.</summary>
    /// <param name="fileTypes">The kinds asked for.</param>
    /// <returns>The kinds.</returns>
    /// <exception cref="ArgumentException"><paramref name="fileTypes"/> names a kind that does not exist.</exception>
    private static ChatFileType KindsOf(ChatFileType fileTypes)
    {
        const ChatFileType known = ChatFileType.TextEncoded | ChatFileType.Image;

        return (fileTypes & ~known) == 0
            ? fileTypes
            : throw new ArgumentException(
                $"A loaded chat accepts {nameof(ChatFileType.TextEncoded)}, {nameof(ChatFileType.Image)} "
                + $"or {nameof(ChatFileType.None)}; got '{fileTypes}'.",
                nameof(fileTypes));
    }

    /// <summary>The extensions a field accepts for the kinds asked for.</summary>
    /// <param name="kinds">The kinds.</param>
    /// <returns>The extensions, in the order the field serves them.</returns>
    private static List<string> ExtensionsOf(ChatFileType kinds)
    {
        var extensions = new List<string>();

        if (kinds.HasFlag(ChatFileType.TextEncoded))
        {
            extensions.AddRange(Attachments.TextExtensions);
        }

        if (kinds.HasFlag(ChatFileType.Image))
        {
            extensions.AddRange(Attachments.ImageExtensions);
        }

        return extensions;
    }
}

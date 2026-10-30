namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Draws a gallery of examples under the page being written.</summary>
    /// <param name="examples">The samples — one list per sample, or a directory of them.</param>
    /// <param name="inputs">The components a sample fills, in order.</param>
    /// <param name="outputs">The components the sample's answer is written to, or
    /// <see langword="null"/> for none.</param> <param name="fn">The handler the samples are run
    /// through, or <see langword="null"/> for samples that only fill the inputs.</param>
    /// <param name="cacheExamples">Whether the samples' answers are cached, or
    /// <see langword="null"/> to read the environment.</param> <param name="cacheMode">How they
    /// are cached — <c>eager</c> or <c>lazy</c> — or <see langword="null"/> for the Python
    /// version's default.</param> <param name="examplesPerPage">How many samples are shown per
    /// page.</param> <param name="label">The heading the gallery is drawn under.</param>
    /// <param name="elemId">The element id, or <see langword="null"/> for none.</param>
    /// <param name="runOnClick">Whether clicking a sample runs the handler as well as filling the
    /// inputs.</param> <param name="preprocess">Whether the inputs preprocess the sample before the
    /// handler runs.</param> <param name="postprocess">Whether the outputs postprocess the answer
    /// before it is sent.</param> <param name="apiVisibility">How far the sample-loading endpoint
    /// is exposed.</param> <param name="apiName">The name the sample-loading endpoint is served
    /// under.</param> <param name="apiDescription">The description the endpoint carries, or
    /// <see langword="null"/> for none.</param> <param name="batch">Whether the handler takes a
    /// batch of samples.</param> <param name="exampleLabels">The label each sample is drawn with,
    /// or <see langword="null"/> for none.</param> <param name="visibleColumns">Which columns are
    /// drawn, or <see langword="null"/> for all of them.</param> <param name="visible">Whether the
    /// gallery is drawn.</param> <param name="preload">Which sample is preloaded, or
    /// <see langword="null"/> for none.</param> <returns>The gallery, as the block it
    /// drew.</returns> <exception cref="InvalidOperationException">There is no application being
    /// written.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>gr.Examples</c>: a gallery of samples drawn where the line is
    /// written, which is why the application is not a parameter — it is the one being written, the
    /// same way <c>gr.Textbox(...)</c> finds it (<see cref="Ambient"/>).
    /// </para>
    /// <para>
    /// The components arrive as a <see cref="ComponentCollection"/>, so the three shapes the Python
    /// version accepts all compile: one component, a list of them, or the string shortcuts
    /// (<c>gr.Examples(examples, inputs: "text")</c>).
    /// </para>
    /// </remarks>
    public static Examples Examples(
        object examples,
        ComponentCollection inputs,
        ComponentCollection? outputs = null,
        Delegate? fn = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        string? label = null,
        string? elemId = null,
        bool runOnClick = false,
        bool preprocess = true,
        bool postprocess = true,
        ApiVisibility apiVisibility = ApiVisibility.Undocumented,
        string? apiName = "load_example",
        string? apiDescription = null,
        bool batch = false,
        IReadOnlyList<string>? exampleLabels = null,
        IReadOnlyList<int>? visibleColumns = null,
        Visibility visible = default,
        int? preload = 0) =>
        new(
            Application(nameof(Examples)),
            examples,
            inputs.Components,
            outputs?.Components,
            fn,
            cacheExamples,
            cacheMode,
            examplesPerPage,
            label,
            elemId,
            runOnClick,
            preprocess,
            postprocess,
            apiVisibility,
            apiName,
            apiDescription,
            batch,
            exampleLabels,
            visibleColumns,
            visible,
            preload);
}

public static partial class gr
{
    /// <summary>Builds a whole application around a handler: the Python version's
    /// <c>gr.Interface</c>.</summary> <param name="fn">The handler the page is built
    /// around.</param> <param name="inputs">The components the handler reads — one, a list, or the
    /// string shortcuts.</param> <param name="outputs">The components the handler's result updates,
    /// in the same three shapes.</param> <param name="examples">Sample inputs, or a directory of
    /// them, or <see langword="null"/> for none.</param> <param name="cacheExamples">Whether the
    /// samples' answers are cached, or <see langword="null"/> to read the environment.</param>
    /// <param name="cacheMode">How they are cached — <c>eager</c> or <c>lazy</c> — or
    /// <see langword="null"/> for the Python version's default.</param>
    /// <param name="examplesPerPage">How many samples are shown per page.</param>
    /// <param name="exampleLabels">The label each sample is drawn with, or <see langword="null"/>
    /// for none.</param> <param name="preloadExample">Which sample is preloaded, <c>false</c> for
    /// none, or <see langword="null"/> for the Python version's default (the first).</param>
    /// <param name="live">Whether the page re-runs the handler as the inputs change.</param>
    /// <param name="title">The title above the components, or <see langword="null"/> for
    /// none.</param> <param name="description">The description under the title, or
    /// <see langword="null"/> for none.</param> <param name="article">The article under the
    /// components, or <see langword="null"/> for none.</param> <param name="flaggingMode">Whether
    /// samples can be flagged — <c>never</c>, <c>auto</c> or <c>manual</c>.</param>
    /// <param name="flaggingOptions">The reasons a sample may be flagged for, or
    /// <see langword="null"/> for the Python version's.</param> <param name="flaggingDir">Where
    /// flagged samples are written.</param> <param name="flaggingCallback">How a flagged sample is
    /// written, or <see langword="null"/> for the default.</param>
    /// <param name="analyticsEnabled">Whether telemetry is on, or <see langword="null"/> to read
    /// the environment.</param> <param name="batch">Whether the handler takes a batch of
    /// inputs.</param> <param name="maxBatchSize">The largest batch handed to a batching
    /// handler.</param> <param name="apiVisibility">How far the endpoint is exposed.</param>
    /// <param name="apiName">The name the endpoint is served under, or <see langword="null"/> to
    /// derive it.</param> <param name="apiDescription">The endpoint's description: a string,
    /// <c>false</c> for none, or <see langword="null"/> for the handler's own.</param>
    /// <param name="allowDuplication">Whether the page offers to duplicate itself.</param>
    /// <param name="concurrencyLimit">How many runs of the handler may be in flight.</param>
    /// <param name="additionalInputs">Further components the handler reads, or
    /// <see langword="null"/> for none.</param> <param name="additionalInputsAccordion">How those
    /// are drawn — an <see cref="Accordion"/> or its label.</param> <param name="submitBtn">The
    /// submit button, or <see langword="null"/> for the Python version's.</param>
    /// <param name="stopBtn">The stop button, or <see langword="null"/> for the Python
    /// version's.</param> <param name="clearBtn">The clear button, or <see langword="null"/> for
    /// the Python version's.</param> <param name="deleteCache">How often the application's
    /// temporary files are cleaned, or <see langword="null"/> for never.</param>
    /// <param name="showProgress">How much of the progress animation a run shows.</param>
    /// <param name="fillWidth">Whether the page fills the width of the window.</param>
    /// <param name="timeLimit">How long a run may take, or <see langword="null"/> for no
    /// limit.</param> <param name="streamEvery">How often streamed chunks are sent.</param>
    /// <param name="deepLink">Whether the page offers a deep link, or <see langword="null"/> for
    /// no.</param> <param name="validator">A function run before the handler to reject input, or
    /// <see langword="null"/> for none.</param> <returns>The application the page was drawn
    /// into.</returns> <exception cref="ArgumentNullException"><paramref name="fn"/> is
    /// <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Interface</c> <em>is</em> an application, so this returns one: the
    /// layer builds the render root, hands it to the kernel's <see cref="Interface"/>, and gives it
    /// back — which is what makes <c>gr.Interface(...).Launch()</c> the same two lines as the
    /// Python.
    /// </para>
    /// <para>
    /// The union-typed parameters keep the Python version's shapes rather than a C# narrowing of
    /// them: <c>preloadExample</c> takes a number or <c>false</c>, and <c>apiDescription</c>
    /// takes a string, <c>false</c> or nothing — the three cases the Python version distinguishes.
    /// </para>
    /// </remarks>
    public static Blocks Interface(
        Delegate fn,
        ComponentCollection? inputs,
        ComponentCollection? outputs,
        object? examples = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        IReadOnlyList<string>? exampleLabels = null,
        object? preloadExample = null,
        bool live = false,
        string? title = null,
        string? description = null,
        string? article = null,
        string? flaggingMode = null,
        IReadOnlyList<FlaggingOption>? flaggingOptions = null,
        string flaggingDir = ".gradio/flagged",
        FlaggingCallback? flaggingCallback = null,
        bool? analyticsEnabled = null,
        bool batch = false,
        int maxBatchSize = 4,
        ApiVisibility apiVisibility = ApiVisibility.Public,
        string? apiName = null,
        object? apiDescription = null,
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
        int? timeLimit = 30,
        double streamEvery = 0.5,
        object? deepLink = null,
        Delegate? validator = null)
    {
        ArgumentNullException.ThrowIfNull(fn);

        var demo = NewApplication(analyticsEnabled);
        var (apiText, apiDocs) = DescriptionOf(apiDescription);

        using (demo.BeginRender())
        {
            _ = new Interface(
                demo,
                fn,
                inputs,
                outputs,
                examples,
                cacheExamples,
                cacheMode,
                examplesPerPage,
                exampleLabels,
                PreloadOf(preloadExample),
                live,
                title,
                description,
                article,
                flaggingMode,
                flaggingOptions,
                flaggingDir,
                flaggingCallback,
                batch,
                maxBatchSize,
                apiVisibility,
                apiName,
                apiText,
                apiDocs,
                allowDuplication,
                concurrencyLimit,
                additionalInputs,
                additionalInputsAccordion,
                submitBtn,
                stopBtn,
                clearBtn,
                deleteCache,
                showProgress,
                fillWidth,
                timeLimit: timeLimit,
                streamEvery: streamEvery,
                deepLink: deepLink,
                validator: validator);
        }

        return demo;
    }

    /// <summary>Builds a chat application around a handler: the Python version's
    /// <c>gr.ChatInterface</c>.</summary> <param name="fn">The handler the page is built
    /// around.</param> <param name="multimodal">Whether the field accepts files as well as
    /// text.</param> <param name="chatbot">The chat window to use, or <see langword="null"/> for
    /// the one the Python version builds.</param> <param name="textbox">The field to use, or
    /// <see langword="null"/> for the one the Python version builds.</param>
    /// <param name="additionalInputs">Further components the handler reads, or
    /// <see langword="null"/> for none.</param> <param name="additionalInputsAccordion">How those
    /// are drawn.</param> <param name="additionalOutputs">Components the handler's answer also
    /// updates, or <see langword="null"/> for none.</param> <param name="editable">Whether a sent
    /// message can be edited.</param> <param name="examples">The prompts the page offers, or
    /// <see langword="null"/> for none.</param> <param name="examplesLabel">The heading the
    /// prompts are drawn under.</param> <param name="exampleLabels">The label each prompt is drawn
    /// with.</param> <param name="exampleIcons">The icon each prompt is drawn with.</param>
    /// <param name="runExamplesOnClick">Whether clicking a prompt sends it.</param>
    /// <param name="cacheExamples">Whether the prompts' answers are cached.</param>
    /// <param name="cacheMode">How they are cached.</param> <param name="title">The title above
    /// the chat.</param> <param name="description">The description under the title.</param>
    /// <param name="flaggingMode">Whether messages can be flagged.</param>
    /// <param name="flaggingOptions">The reasons a message may be flagged for.</param>
    /// <param name="flaggingDir">Where flagged messages are written.</param>
    /// <param name="analyticsEnabled">Whether telemetry is on.</param>
    /// <param name="autofocus">Whether the field has focus when the page opens.</param>
    /// <param name="autoscroll">Whether the chat scrolls to the newest message.</param>
    /// <param name="submitBtn">The submit button: a label, <c>true</c> for the Python version's,
    /// <c>false</c> for none.</param> <param name="stopBtn">The stop button, in the same three
    /// spellings.</param> <param name="concurrencyLimit">How many runs may be in flight.</param>
    /// <param name="deleteCache">How often temporary files are cleaned.</param>
    /// <param name="showProgress">How much of the progress animation a run shows.</param>
    /// <param name="fillHeight">Whether the chat fills the height of the window.</param>
    /// <param name="fillWidth">Whether the page fills the width of the window.</param>
    /// <param name="apiName">The name the endpoint is served under.</param>
    /// <param name="apiDescription">The endpoint's description: a string, <c>false</c> for none,
    /// or nothing for the handler's own.</param> <param name="apiVisibility">How far the endpoint
    /// is exposed.</param> <param name="saveHistory">Whether the conversation is kept across
    /// visits.</param> <param name="validator">A function run before the handler to reject
    /// input.</param> <returns>The application the page was drawn into.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fn"/> is
    /// <see langword="null"/>.</exception>
    /// <remarks>
    /// The Python version's <c>ChatInterface</c> is an application too, so this returns one for the
    /// reason <see cref="Interface"/> does.
    /// </remarks>
    public static Blocks ChatInterface(
        Delegate fn,
        bool multimodal = false,
        Chatbot? chatbot = null,
        Component? textbox = null,
        ComponentCollection? additionalInputs = null,
        object? additionalInputsAccordion = null,
        ComponentCollection? additionalOutputs = null,
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
        ArgumentNullException.ThrowIfNull(fn);

        var demo = NewApplication(analyticsEnabled);

        using (demo.BeginRender())
        {
            _ = new ChatInterface(
                demo,
                fn,
                multimodal,
                chatbot,
                textbox,
                additionalInputs,
                additionalInputsAccordion,
                additionalOutputs,
                editable,
                examples,
                examplesLabel,
                exampleLabels,
                exampleIcons,
                runExamplesOnClick,
                cacheExamples,
                cacheMode,
                title,
                description,
                flaggingMode,
                flaggingOptions,
                flaggingDir,
                analyticsEnabled,
                autofocus,
                autoscroll,
                submitBtn,
                stopBtn,
                concurrencyLimit,
                deleteCache,
                showProgress,
                fillHeight,
                fillWidth,
                apiName,
                apiDescription,
                apiVisibility,
                saveHistory,
                validator);
        }

        return demo;
    }

    /// <summary>Puts several applications behind one tab bar: the Python version's
    /// <c>gr.TabbedInterface</c>.</summary> <param name="interfaceList">The content of each tab,
    /// as the recipe that draws it.</param> <param name="tabNames">The name of each tab, or
    /// <see langword="null"/> to number them.</param> <param name="title">The heading above the tab
    /// bar, or <see langword="null"/> for none.</param> <param name="analyticsEnabled">Whether
    /// telemetry is on, or <see langword="null"/> to read the environment.</param>
    /// <param name="tabsKwargs">The tab bar itself, or <see langword="null"/> for the one the
    /// Python version draws.</param> <returns>The application the tabs were drawn into.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interfaceList"/> is
    /// <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>interfaceList</c> holds the applications themselves, because a
    /// Python <c>Interface</c> is a <c>Blocks</c> that can be drawn into another one after the
    /// fact. A container here hosts one application, so a tab holds the recipe for its content
    /// instead — which is the same difference <c>with</c> → <c>using</c> is: the render root is a
    /// thing this framework has one of, not a thing a page can be copied into.
    /// </para>
    /// <para>
    /// <c>tabsKwargs</c> is a dictionary of the tab bar's props the Python version; here it is the
    /// tab bar, which is that dictionary with its props already set.
    /// </para>
    /// </remarks>
    public static Blocks TabbedInterface(
        IReadOnlyList<TabContent> interfaceList,
        IReadOnlyList<string>? tabNames = null,
        string? title = null,
        bool? analyticsEnabled = null,
        Tabs? tabsKwargs = null)
    {
        ArgumentNullException.ThrowIfNull(interfaceList);

        var demo = NewApplication(analyticsEnabled);

        using (demo.BeginRender())
        {
            _ = new TabbedInterface(demo, interfaceList, tabNames, title, tabsKwargs);
        }

        return demo;
    }

    /// <summary>The application a page builder draws into, with the settings it was given.</summary>
    /// <param name="analyticsEnabled">Whether telemetry is on, or <see langword="null"/> to leave the default.</param>
    /// <returns>The application, not yet rendering.</returns>
    /// <remarks>
    /// Built the way <see cref="gr.Blocks"/> builds one — the container factory is what makes a form
    /// component group into a form — but without opening the render or the ambient context: the page
    /// builders open the render themselves, because the kernel's own constructors already know how
    /// to render into an application that is not yet being rendered.
    /// </remarks>
    private static Blocks NewApplication(bool? analyticsEnabled)
    {
        var demo = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        if (analyticsEnabled is not null)
        {
            demo.AnalyticsEnabled = analyticsEnabled.Value;
        }

        return demo;
    }

    /// <summary>Reads the Python version's <c>preloadExample</c>: a sample's index, or
    /// <c>false</c> for none.</summary> <param name="value">A number, <c>false</c>, or
    /// <see langword="null"/> for the Python version's default.</param> <returns>The index, or
    /// <see langword="null"/> when no sample is preloaded.</returns>
    /// <exception cref="ArgumentException">The value is none of those.</exception>
    private static int? PreloadOf(object? value) => value switch
    {
        null => 0,
        int index => index,
        false => null,
        _ => throw new ArgumentException(
            $"'{value}' is not a preload: the Python version accepts a sample's index or false.",
            nameof(value)),
    };

    /// <summary>Reads the Python version's <c>apiDescription</c>: a string, <c>false</c>, or
    /// nothing.</summary> <param name="value">A string, <c>false</c>, or <see langword="null"/> for
    /// the handler's own.</param> <returns>The description the config carries, and the one the API
    /// document carries.</returns> <exception cref="ArgumentException">The value is none of
    /// those.</exception>
    /// <remarks>
    /// The three cases are the Python version's, and they are three: a description, no description
    /// at all (<c>False</c>), and "use the handler's docstring" (<c>None</c>) — which here is a
    /// <see langword="null"/> description and a <see langword="null"/> docstring, because a .NET
    /// method carries no docstring to fall back to.
    /// </remarks>
    private static (string? Description, string? Docstring) DescriptionOf(object? value) => value switch
    {
        null => (null, null),
        string text => (text, null),
        false => (null, string.Empty),
        _ => throw new ArgumentException(
            $"'{value}' is not a description: the Python version accepts a string, false, or nothing.",
            nameof(value)),
    };
}

namespace Gradio.Net;
public static partial class gr
{
    /// <summary>The sentinel an <c>every</c> parameter takes to mean "never reload".</summary>
    /// <value>The Python version's <c>NO_RELOAD</c>, which is the number of seconds it stands for
    /// there.</value>
    /// <remarks>
    /// <para>
    /// The Python version writes <c>every=gr.NO_RELOAD</c> on a component to say that it must not
    /// be re-described on a timer. It is a value rather than an enum because it is also a number of
    /// seconds: the same parameter takes <c>every=2</c> for "twice a second".
    /// </para>
    /// <para>
    /// A field rather than a method, because that is what it is the Python version — a name that
    /// holds a value — and a ported line reads <c>every: gr.NoReload</c>.
    /// </para>
    /// </remarks>
    public const int NoReload = 1;

    /// <summary>Marks a handler's argument as the one progress reports are written to.</summary>
    /// <param name="trackTqdm">
    /// Whether progress bars written by tqdm are reported too. Accepted so that a ported call
    /// compiles; there is no tqdm here, so nothing else is done with it.
    /// </param>
    /// <returns>The tracker a handler receives as its <see cref="Progress"/> argument.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>gr.Progress()</c> is a default argument: a handler that declares
    /// <c>progress=gr.Progress()</c> is handed the tracker for the run that called it. Here the
    /// tracker arrives as an ordinary parameter of that type, so what this builds is the
    /// object the framework fills in — a handler never constructs one itself.
    /// </para>
    /// <para>
    /// <c>trackTqdm</c> is the Python version's switch for also reporting progress bars written
    /// with tqdm. A .NET application writes none, so there is nothing to switch on: the parameter
    /// is kept because the call site has it, and it changes nothing.
    /// </para>
    /// </remarks>
    public static Progress Progress(bool trackTqdm = false)
    {
        _ = trackTqdm;

        return new Progress();
    }

    /// <summary>The translations an application's strings are drawn from.</summary>
    /// <param name="translations">
    /// One entry per locale: its code, and the catalogue of strings for it — the Python version's
    /// <c>I18n(en={...}, fr={...})</c>.
    /// </param>
    /// <returns>The translations.</returns>
    /// <remarks>
    /// The Python version takes <c>**translations</c>, which C# spells as the pairs themselves; the
    /// locale codes are the keys either way. A second declaration for a locale replaces the first,
    /// which is what a Python call with a repeated keyword cannot even write.
    /// </remarks>
    public static I18n I18n(params (string Locale, IReadOnlyDictionary<string, string> Translations)[] translations) =>
        new(translations);

    /// <summary>Exposes a handler as an endpoint, described by its signature.</summary>
    /// <param name="fn">The handler. Every parameter and every return value is described by its type.</param>
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
    /// <exception cref="ArgumentNullException"><paramref name="fn"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">There is no application being written.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>@gr.api</c>, which is a decorator and therefore a call here: the
    /// application is the one being written, so the line that declares the endpoint does not have
    /// to name it — exactly as <c>gr.Textbox(...)</c> does not.
    /// </para>
    /// <para>
    /// The endpoint itself is the kernel's: <see cref="ApiEndpoints.Api"/> builds one <c>Api</c>
    /// component per parameter and per return value, and the served API info, the OpenAPI document
    /// and an agent's tool list are all built from it. This adds nothing to that.
    /// </para>
    /// </remarks>
    public static Dependency Api(Delegate fn,         bool preprocess = true,
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
        Application(nameof(Api)).Api(fn, preprocess: preprocess, postprocess: postprocess, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, apiName: apiName, apiDescription: apiDescription, endpointDescription: endpointDescription, js: js, jsRunsInBrowser: jsRunsInBrowser, jsImplementation: jsImplementation, queue: queue, batch: batch, maxBatchSize: maxBatchSize, triggerMode: triggerMode, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, apiVisibility: apiVisibility, timeLimit: timeLimit, streamEvery: streamEvery, cancels: cancels, validator: validator);

    /// <summary>The version of gradio this application targets.</summary>
    /// <returns>The Python release, spelled the way the Python version spells it.</returns>
    /// <remarks>
    /// The Python version answers with the version in its own <c>package.json</c>, which is the
    /// release an application was written against. The same question here is the release this build
    /// mirrors — <see cref="GradioVersion.Upstream"/> — and not the Gradio.Net package's
    /// own number, which is a different versioning scheme and would answer a different question.
    /// </remarks>
    public static string GetPackageVersion() => GradioVersion.Upstream;

    /// <summary>A sample audio file, for a handler that needs one.</summary>
    /// <param name="filename">The file's name within the samples, or <see langword="null"/> for the first.</param>
    /// <returns>The path the sample was written to.</returns>
    public static string GetAudio(string? filename = null) => Media.GetAudio(filename);

    /// <summary>A sample file, for a handler that needs one.</summary>
    /// <param name="filename">The file's name within the samples, or <see langword="null"/> for the first.</param>
    /// <returns>The path the sample was written to.</returns>
    public static string GetFile(string? filename = null) => Media.GetFile(filename);

    /// <summary>A sample image, for a handler that needs one.</summary>
    /// <param name="filename">The file's name within the samples, or <see langword="null"/> for the first.</param>
    /// <returns>The path the sample was written to.</returns>
    public static string GetImage(string? filename = null) => Media.GetImage(filename);

    /// <summary>A sample video, for a handler that needs one.</summary>
    /// <param name="filename">The file's name within the samples, or <see langword="null"/> for the first.</param>
    /// <returns>The path the sample was written to.</returns>
    public static string GetVideo(string? filename = null) => Media.GetVideo(filename);

    /// <summary>A sample 3D model, for a handler that needs one.</summary>
    /// <param name="filename">The file's name within the samples, or <see langword="null"/> for the first.</param>
    /// <returns>The path the sample was written to.</returns>
    public static string GetModel3d(string? filename = null) => Media.GetModel3d(filename);

    /// <summary>The application this flow is writing into.</summary>
    /// <param name="member">The member asking, for the message when there is none.</param>
    /// <returns>The application.</returns>
    /// <exception cref="InvalidOperationException">No application is being written in this flow.</exception>
    /// <remarks>
    /// The same question a factory asks (<see cref="Ambient"/>), asked by the members that are not
    /// factories: <c>gr.Examples(...)</c> and <c>gr.Api(...)</c> draw into the page being written
    /// rather than being handed one, which is the Python version's shape for both.
    /// </remarks>
    private static Blocks Application(string member) =>
        Ambient.Current?.Root
        ?? throw new InvalidOperationException(
            $"gr.{member} has to be written inside an application: open one with gr.Blocks() first.");
}

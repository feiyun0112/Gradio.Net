using System.Reflection;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The entry point that turns a typed handler into an endpoint, without a component to bind it to
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.api</c>: the handler's signature is the whole description of the
/// endpoint, so an application that wants to be called by a machine writes the function and nothing
/// else. Each parameter becomes an <see cref="Api"/> block carrying the schema its type produced,
/// and each declared return type becomes one too, which is why the endpoint reaches the API info,
/// the OpenAPI document and an agent's tool list without any of those being written out.
/// </para>
/// <para>
/// The binding has no trigger. Nothing in the page raises it: a client reaches it by calling its
/// name, which is what the Python version's empty trigger list says and what the served config
/// reports as an empty <c>targets</c>.
/// </para>
/// <para>
/// It is an extension here rather than a member of <see cref="Blocks"/> because it is the one entry
/// point that has to build a component: <see cref="Blocks"/> lives in the core layer, which may not
/// name a concrete block type, and this is the layer that may.
/// </para>
/// </remarks>
public static class ApiEndpoints
{
    /// <summary>
    /// Exposes a handler as an endpoint, described by its signature rather than by components.
    /// </summary>
    /// <param name="blocks">The application the endpoint belongs to.</param>
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
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The application is not the ambient render context; an endpoint belongs to the application
    /// whose code declares it.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The application has been disposed.</exception>
    /// <remarks>
    /// <para>
    /// Three binding parameters are the endpoint's rather than the caller's, because the Python
    /// version's <c>gr.api</c> fixes them: it shows no progress animation (there is no page to draw
    /// one on), it never scrolls to an output (there is nothing to scroll), and its parameters are
    /// named after the handler's own — a client calls <c>add(a=1, b=2)</c> because that is what the
    /// signature said, and a .NET method does carry those names.
    /// </para>
    /// <para>
    /// A parameter of type <see cref="Progress"/> or <see cref="CancellationToken"/> is not an
    /// input: the framework supplies it, exactly as it does for any other binding.
    /// </para>
    /// </remarks>
    public static Dependency Api(
        this Blocks blocks,
        Delegate fn,
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
        Delegate? validator = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(fn);

        var options = new EventBindingOptions
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
        };

        // A batched handler declares the whole batch — its parameter is what the run hands it, which
        // is the list of samples — so the documents describe that, exactly as the Python version's do for a
        // function annotated `list[int]`. What the run converts is one *sample*, though, and that is
        // the element type: describing the batch and converting a sample are two different jobs,
        // which is why the schema and the value type are not the same type here.
        var names = new List<string?>();
        var inputs = new List<Block>();

        foreach (var parameter in fn.Method.GetParameters())
        {
            if (DelegateInvocation.IsSuppliedByRun(parameter.ParameterType))
            {
                continue;
            }

            names.Add(parameter.Name ?? string.Empty);
            inputs.Add(blocks.Add(new Api
            {
                Value = DefaultOf(parameter),
                Schema = TypeSchema.Of(parameter.ParameterType),
                ValueType = SampleOf(parameter.ParameterType, options.Batch),

                // The Python version labels each block with its position — "1st", "2nd" — because there is no
                // component to name it after and a documentation table with four rows all reading
                // "API" says nothing about which argument is which.
                Label = Ordinal(names.Count),
            }));
        }

        var outputs = new List<Block>();
        foreach (var declared in TypeSchema.ReturnsOf(fn))
        {
            outputs.Add(blocks.Add(new Api
            {
                Schema = TypeSchema.Of(declared),
                ValueType = SampleOf(declared, options.Batch),
                Label = Ordinal(outputs.Count + 1),
            }));
        }

        return blocks.Register([], fn, inputs, outputs, options with
        {
            ParameterNames = options.ParameterNames ?? names,
            ShowProgress = ProgressDisplay.Hidden,
            ScrollToOutput = false,
        });
    }

    /// <summary>What one sample of an argument is, which is the argument itself unless the run batches.</summary>
    /// <param name="type">The type the handler declared.</param>
    /// <param name="batched">Whether the binding hands the handler a batch of samples.</param>
    /// <returns>The element type for a batched binding, and <paramref name="type"/> otherwise.</returns>
    /// <remarks>
    /// <para>
    /// This is the type a value is converted to, not the type the documents describe: a batched run
    /// preprocesses one sample at a time and collects them, so the conversion happens against the
    /// element — while the schema of the argument stays what the handler declared, which is what
    /// the Python version records for a batched <c>gr.api</c> function.
    /// </para>
    /// <para>
    /// A type that is not a sequence is left as it is: a batched handler that declared something
    /// else is a handler the run cannot fill, and converting as though it were a sample would hide
    /// that rather than say it.
    /// </para>
    /// </remarks>
    private static Type SampleOf(Type type, bool batched) =>
        // A string is a sequence of characters and is not one here: its schema is `string`, so a
        // sample of it is the string itself.
        batched && type != typeof(string) && TypeSchema.ElementType(type) is { } element ? element : type;

    /// <summary>The value a parameter takes when a caller leaves it out.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The declared default as JSON, or <see langword="null"/> for one with no default.</returns>
    /// <remarks>
    /// The default is what makes a parameter optional in the API documentation, which is why it is
    /// carried on the block rather than left to the handler: a client reads it to know what it may
    /// omit, and the value it omits is the value the block holds.
    /// </remarks>
    private static JsonNode? DefaultOf(ParameterInfo parameter) =>
        parameter.HasDefaultValue && parameter.DefaultValue is { } value && value is not DBNull
            ? JsonValues.FromAnyObject(value)
            : null;

    /// <summary>A position, spelled the way the Python version spells it.</summary>
    /// <param name="position">The position, counting from one.</param>
    /// <returns>The ordinal: <c>1st</c>, <c>2nd</c>, <c>11th</c>, <c>21st</c>.</returns>
    /// <remarks>
    /// The teens are all <c>th</c> whatever their last digit says, which is the rule English
    /// actually has and the one the Python version's helper encodes.
    /// </remarks>
    private static string Ordinal(int position)
    {
        var suffix = position % 100 is >= 10 and <= 20
            ? "th"
            : (position % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };

        return position.ToString(System.Globalization.CultureInfo.InvariantCulture) + suffix;
    }
}

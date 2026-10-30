using System.Globalization;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The gallery of sample inputs an application offers under its components, and — when asked — the
/// answers to them, worked out once and kept.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.Examples</c>. It is a helper rather than a component: what it draws
/// is a <see cref="Dataset"/>, and what makes clicking one of its rows fill the application's
/// fields is a pair of bindings it sets up. The reason it is a class rather than a few lines an
/// application writes is the caching: an example whose answer is already known is shown instantly
/// instead of running the model again, and deciding whether it is known is the whole of the second
/// half.
/// </para>
/// <para>
/// The samples are kept in the application's own folder rather than in the working directory, which
/// is where the Python version puts them: a cached answer can name a file, and a file the
/// application does not serve is a file the browser cannot show.
/// </para>
/// <para>
/// Caching happens in one of two modes, which are the Python version's. <c>eager</c> works out
/// every answer when the application starts; <c>lazy</c> works out each answer the first time
/// somebody asks for it. Both write the same two files — <c>log.csv</c> with the answers and
/// <c>indices.csv</c> with which sample each answer belongs to — so a restart picks up where the
/// last run left off.
/// </para>
/// </remarks>
public sealed class Examples
{
    /// <summary>A value that says "leave this component as it is".</summary>
    /// <remarks>
    /// The Python version's <c>_UNCHANGED</c>. A row of samples may leave a column empty, and a
    /// column that is empty for this row means the field keeps what the user typed rather than
    /// being cleared.
    /// </remarks>
    private static readonly object _unchanged = new();

    private readonly Blocks _blocks;
    private readonly IReadOnlyList<Component> _inputs;
    private readonly IReadOnlyList<Component> _outputs;
    private readonly Delegate? _fn;
    private readonly List<object?[]> _examples;
    private readonly bool[] _hasExamples;
    private readonly List<object?[]> _nonNoneExamples;
    private readonly List<int> _visiblePositions;
    private readonly List<int> _visibleInputIndices;
    private readonly IReadOnlyList<Component> _visibleInputs;
    private readonly List<object?[]> _visibleExamples;
    private readonly string _directory;
    private readonly CsvLogger _cacheLogger;
    private readonly string _cacheFolder;
    private readonly string _cacheFile;
    private readonly string _indicesFile;
    private readonly SemaphoreSlim[] _cacheLocks;
    private readonly bool _preprocess;
    private readonly bool _postprocess;
    private readonly bool _batch;

    /// <summary>Builds the gallery and adds it to the application being rendered.</summary>
    /// <param name="blocks">The application the gallery belongs to.</param>
    /// <param name="examples">
    /// The samples: one entry per sample, each holding one value per input, in the inputs' order. A
    /// flat list is read as one value per sample when there is a single input. A directory path is
    /// read as a <c>log.csv</c> inside it, or as the files it holds when there is one input — which
    /// is the Python version's <c>list | str</c>, and the reason this is an <see cref="object"/> rather than a
    /// sequence.
    /// </param>    /// <param name="inputs">The components a sample fills.</param>
    /// <param name="outputs">The components the answers are shown in, or <see langword="null"/> for none.</param>
    /// <param name="fn">The handler whose answers are cached, or <see langword="null"/> for none.</param>
    /// <param name="cacheExamples">
    /// Whether the answers are cached, or <see langword="null"/> to read the environment — which is
    /// the Python version's rule: <c>GRADIO_CACHE_EXAMPLES</c> turns it on, and nothing turns it on
    /// by default.
    /// </param>
    /// <param name="cacheMode">
    /// <c>eager</c> to work every answer out at startup, <c>lazy</c> to work each one out when it is
    /// first asked for, or <see langword="null"/> to read <c>GRADIO_CACHE_MODE</c> — which defaults
    /// to <c>eager</c>.
    /// </param>
    /// <param name="examplesPerPage">How many samples one page of the gallery holds.</param>
    /// <param name="label">The label drawn above the gallery.</param>
    /// <param name="elemId">The HTML id of the gallery, or <see langword="null"/> for none.</param>
    /// <param name="runOnClick">Whether clicking a sample also runs the handler, when nothing is cached.</param>
    /// <param name="preprocess">Whether the cached run preprocesses the samples first.</param>
    /// <param name="postprocess">Whether the cached run postprocesses the answers before keeping them.</param>
    /// <param name="apiVisibility">How far the click's endpoint is exposed.</param>
    /// <param name="apiName">The name the click's endpoint is exposed under.</param>
    /// <param name="apiDescription">The description the endpoint carries, or <see langword="null"/> for none.</param>
    /// <param name="batch">Whether the handler takes a batch of samples.</param>
    /// <param name="exampleLabels">The labels shown instead of the samples, or <see langword="null"/> for none.</param>
    /// <param name="visibleColumns">
    /// The inputs the gallery shows, by position, or <see langword="null"/> for all of them. A column
    /// that is hidden is still filled when its sample is picked.
    /// </param>
    /// <param name="visible">Whether the gallery is drawn.</param>
    /// <param name="preload">
    /// The sample to load when the page opens, or <see langword="null"/> for none. Only a sample
    /// whose answer is cached eagerly is preloaded, and only when no input was given a value of its
    /// own — which is the Python version's rule, and what keeps a preload from overwriting an
    /// application's own starting state.
    /// </param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The samples, labels or columns do not describe the inputs.</exception>
    public Examples(
        Blocks blocks,
        object examples,
        IReadOnlyList<Component> inputs,
        IReadOnlyList<Component>? outputs = null,
        Delegate? fn = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        string? label = "Examples",
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
        int? preload = 0)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(inputs);

        if (inputs.Count == 0)
        {
            throw new ArgumentException("A gallery needs at least one input to fill.", nameof(inputs));
        }

        _blocks = blocks;
        _inputs = inputs;
        _outputs = outputs ?? [];
        _fn = fn;
        _preprocess = preprocess;
        _postprocess = postprocess;
        _batch = batch;

        _directory = Directory.GetCurrentDirectory();
        _examples = ReadSamples(examples, inputs.Count);

        _hasExamples = new bool[_inputs.Count];
        foreach (var row in _examples)
        {
            for (var index = 0; index < row.Length && index < _inputs.Count; index++)
            {
                if (row[index] is not null)
                {
                    _hasExamples[index] = true;
                }
            }
        }

        _nonNoneExamples = [.. _examples.Select(row => Keep(row, _hasExamples))];

        _visibleInputIndices = VisibleIndices(visibleColumns, _hasExamples, _inputs.Count);
        _visiblePositions = VisiblePositionsOf(_visibleInputIndices, _hasExamples);
        _visibleInputs = [.. _visibleInputIndices.Select(index => _inputs[index])];
        _visibleExamples = [.. _examples.Select(row => VisibleOf(row, _visibleInputIndices))];

        if (exampleLabels is not null && exampleLabels.Count != _examples.Count)
        {
            throw new ArgumentException(
                "If example labels are provided, there has to be one per sample.",
                nameof(exampleLabels));
        }

        CacheExamples = cacheExamples ?? CacheFromEnvironment(fn, outputs);
        CacheMode = CacheModeOf(cacheMode, CacheExamples);
        RunOnClick = runOnClick;
        ApiName = apiName;
        ApiDescription = apiDescription;
        ApiVisibility = apiVisibility;

        // The gallery is drawn where the application is being rendered, which is where the Python version's
        // constructor registers it. It has to be added before anything is bound to it, because a
        // binding names the id the render gives it.
        Dataset = blocks.Add(new Dataset
        {
            Components = _visibleInputs,
            Samples = _visibleExamples,
            Type = "tuple",
            Label = label,
            SamplesPerPage = examplesPerPage,
            ElemId = elemId,
            Visible = visible,
            SampleLabels = exampleLabels,
        });

        _cacheFolder = Path.Combine(CacheRoot(blocks), Dataset.Id.ToString(CultureInfo.InvariantCulture));
        _cacheFile = Path.Combine(_cacheFolder, "log.csv");
        _indicesFile = Path.Combine(_cacheFolder, "indices.csv");
        _cacheLogger = new CsvLogger(simplifyFileData: false, verbose: false, datasetFileName: "log.csv");
        _cacheLogger.Setup(_outputs, _cacheFolder);
        _cacheLocks = [.. _examples.Select(_ => new SemaphoreSlim(1, 1))];

        Create();

        if (CacheExamples && CacheMode == "eager" && preload is { } sample && CanPreload(sample))
        {
            Preload(sample);
        }
    }

    /// <summary>Whether the answers are cached.</summary>
    public bool CacheExamples { get; }

    /// <summary>Which caching mode is in force: <c>eager</c> or <c>lazy</c>.</summary>
    public string CacheMode { get; }

    /// <summary>Whether clicking a sample runs the handler when nothing is cached.</summary>
    public bool RunOnClick { get; }

    /// <summary>The gallery itself.</summary>
    public Dataset Dataset { get; }

    /// <summary>The binding that loads a cached answer, or <see langword="null"/> when nothing is cached.</summary>
    /// <remarks>
    /// The interface hangs the share button's second click off this one: an answer that was worked
    /// out is a state worth sharing, so the button becomes usable once there is one.
    /// </remarks>
    public Dependency? CacheEvent { get; private set; }

    /// <summary>The binding that fills the fields from a sample.</summary>
    public Dependency? LoadInputEvent { get; private set; }

    /// <summary>The folder the answers are kept in.</summary>
    public string CacheFolder => _cacheFolder;

    /// <summary>
    /// Works out the answers that are not cached yet, and keeps them.
    /// </summary>
    /// <param name="exampleId">One sample to work out, or <see langword="null"/> for all of them.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes when the answers are on disk.</returns>
    /// <remarks>
    /// The Python version runs this at startup for an eager gallery and from the click for a lazy
    /// one. A sample that is already in the file is skipped, which is what makes the work resumable
    /// rather than repeated: the file is the cache.
    /// </remarks>
    public async Task CacheAsync(int? exampleId = null, CancellationToken cancellationToken = default)
    {
        if (!CacheExamples || _fn is null || _outputs.Count == 0)
        {
            return;
        }

        for (var index = 0; index < _nonNoneExamples.Count; index++)
        {
            if (exampleId is { } only && index != only)
            {
                continue;
            }

            if (CachedIndex(index) is not null)
            {
                continue;
            }

            await CacheOneAsync(index, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>The answer to one sample, from the cache.</summary>
    /// <param name="exampleId">The sample's position.</param>
    /// <param name="cancellationToken">Cancels the work when the answer has to be worked out.</param>
    /// <returns>The answer, as the outputs' own values.</returns>
    /// <exception cref="InvalidOperationException">The cache holds no answer for that sample.</exception>
    public async Task<JsonNode?> LoadFromCacheAsync(int exampleId, CancellationToken cancellationToken = default)
    {
        await _cacheLocks[exampleId].WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var cached = CachedIndex(exampleId);
            if (cached is null)
            {
                await CacheOneAsync(exampleId, cancellationToken).ConfigureAwait(false);
                cached = CachedIndex(exampleId);
            }

            if (cached is not { } position)
            {
                throw new InvalidOperationException(
                    $"No answer for sample {exampleId} is in the cache, and it could not be worked out.");
            }

            var rows = ReadCsv(_cacheFile);
            if (position + 1 >= rows.Count)
            {
                throw new InvalidOperationException(
                    $"The cache names sample {exampleId} but holds no row for it.");
            }

            return CachedValues(rows[position + 1]);
        }
        finally
        {
            _cacheLocks[exampleId].Release();
        }
    }

    /// <summary>Binds the gallery's click, and the load that preloads a sample.</summary>
    /// <remarks>
    /// Two shapes, which are the Python version's. Without caching, one click fills the fields and
    /// — when the application asked for it — runs the handler. With caching, the click fills the
    /// fields and a second step shows the answer, which is a second endpoint because it is the one
    /// worth calling from outside.
    /// </remarks>
    private void Create()
    {
        if (CacheExamples && _fn is not null && _outputs.Count > 0)
        {
            var fill = Dataset.Click(
                (Func<object?, object?>)LoadExampleInput,
                [_datasetInput()],
                [.. _inputs],
                queue: false, postprocess: false, showProgress: ProgressDisplay.Hidden, apiVisibility: ApiVisibility.Undocumented);

            var load = _blocks.On(
                [fill.Events.Then],
                fn: (Func<object?, Task<object?>>)LoadExampleOutputAsync, inputs: [_datasetInput()], outputs: [.. _outputs], postprocess: false, apiName: ApiName, apiDescription: ApiDescription, apiVisibility: ApiVisibility);

            LoadInputEvent = fill;
            CacheEvent = load;

            return;
        }

        var click = Dataset.Click(
            (Func<object?, object?>)LoadExample,
            [_datasetInput()],
            [.. _visibleInputs],
            queue: false, postprocess: false, showProgress: ProgressDisplay.Hidden, apiName: ApiName, apiDescription: ApiDescription, apiVisibility: ApiVisibility);

        LoadInputEvent = click;

        if (RunOnClick && _fn is not null)
        {
            _blocks.On(
                [click.Events.Then],
                fn: _fn, inputs: [.. _inputs], outputs: [.. _outputs], apiVisibility: ApiVisibility.Undocumented);
        }
    }

    /// <summary>Fills the fields from a sample, when the answer is cached.</summary>
    /// <param name="value">The gallery's value: the sample's position and its values.</param>
    /// <returns>One update per input, in the order the inputs are in.</returns>
    private JsonNode? LoadExampleInput(object? value)
    {
        var sample = SampleOf(value);
        var processed = ProcessedExample(sample);
        var updates = new JsonArray();

        foreach (var entry in processed)
        {
            updates.Add(ReferenceEquals(entry, _unchanged) ? Update() : entry?.DeepClone());
        }

        return Single(updates);
    }

    /// <summary>Shows the answer to a sample, working it out first when it is not cached yet.</summary>
    /// <param name="value">The gallery's value.</param>
    /// <returns>The answer, as the outputs' own values.</returns>
    private async Task<object?> LoadExampleOutputAsync(object? value)
    {
        var sample = ExampleValueOf(value);

        return Single(await LoadFromCacheAsync(sample).ConfigureAwait(false) as JsonArray ?? []);
    }

    /// <summary>Fills the fields from a sample, when nothing is cached.</summary>
    /// <param name="value">The gallery's value.</param>
    /// <returns>One update per visible input.</returns>
    private JsonNode? LoadExample(object? value)
    {
        var sample = SampleOf(value);
        var processed = ProcessedExample(sample);
        var updates = new JsonArray();

        foreach (var (entry, position) in processed.Select((entry, position) => (entry, position)))
        {
            var component = _visibleInputs[position];
            var props = component.AllProps();
            props.Remove("value");

            if (!ReferenceEquals(entry, _unchanged))
            {
                props["value"] = entry?.DeepClone();
            }

            props["__type__"] = "update";
            updates.Add(props);
        }

        return Single(updates);
    }

    /// <summary>Binds the load that fills the fields with one sample when the page opens.</summary>
    /// <param name="sample">The sample's position.</param>
    private void Preload(int sample)
    {
        // The value the load reads is a state holding the sample, which is how the Python version hands the
        // page's first run something that is not a field: the sample is not in any component yet.
        var state = _blocks.Add(new State { Value = ExampleValueAsJson(sample) });

        _blocks.Load(
            (Func<object?, object?>)LoadExampleInput,
            [state],
            [.. _inputs],
            queue: false, postprocess: false, showProgress: ProgressDisplay.Hidden, apiVisibility: ApiVisibility.Undocumented);

        _blocks.Load(
            (Func<object?, Task<object?>>)LoadExampleOutputAsync,
            [state],
            [.. _outputs],
            queue: false, postprocess: false, showProgress: ProgressDisplay.Hidden, apiVisibility: ApiVisibility.Undocumented);
    }

    /// <summary>Whether a sample can be preloaded.</summary>
    /// <param name="sample">The sample's position.</param>
    /// <returns>Whether it exists and no input was given a value of its own.</returns>
    private bool CanPreload(int sample) =>
        sample >= 0
        && sample < _nonNoneExamples.Count
        && !_inputs.Any(input => input.AllProps().ContainsKey("value"));

    /// <summary>The name the click's endpoint is exposed under.</summary>
    private string? ApiName { get; init; }

    /// <summary>The description the click's endpoint carries.</summary>
    private string? ApiDescription { get; init; }

    /// <summary>The visibility the click's endpoint is exposed with.</summary>
    private ApiVisibility ApiVisibility { get; init; } = ApiVisibility.Undocumented;

    /// <summary>Works out one sample's answer and writes it down.</summary>
    /// <param name="exampleId">The sample's position.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes when the answer is written.</returns>
    private async Task CacheOneAsync(int exampleId, CancellationToken cancellationToken)
    {
        var run = new DependencyRun(
            new RunShape(_fn, _inputs, _outputs, _preprocess, _postprocess, TrackedStateIds: []),
            TimeProvider.System);

        var row = _nonNoneExamples[exampleId];
        var values = new List<JsonArray>(_inputs.Count);

        for (var index = 0; index < _inputs.Count; index++)
        {
            var value = index < row.Length ? row[index] : null;
            values.Add([_inputs[index].Postprocess(value)?.DeepClone()]);
        }

        var output = await run.NextAsync(values, batched: false, new Progress(), eventData: null, cancellationToken)
            .ConfigureAwait(false);

        var answer = output.Data;

        // A handler that streams is cached at its last chunk, which is what the Python version's generator
        // wrapper collects: the answer to a sample is where the stream ended, not where it started.
        while (output.IsGenerating)
        {
            output = await run.NextAsync(values, batched: false, new Progress(), eventData: null, cancellationToken)
                .ConfigureAwait(false);
            answer = output.Data;
        }

        _cacheLogger.Flag([.. answer.Select(value => value?.DeepClone())]);
        System.IO.File.AppendAllText(
            _indicesFile,
            exampleId.ToString(CultureInfo.InvariantCulture) + "\n");
    }

    /// <summary>The cache position a sample's answer is at, or <see langword="null"/> when it has none.</summary>
    /// <param name="exampleId">The sample's position.</param>
    /// <returns>The row's position in the cache file, not counting the header.</returns>
    private int? CachedIndex(int exampleId)
    {
        if (!System.IO.File.Exists(_indicesFile))
        {
            return null;
        }

        var lines = System.IO.File.ReadAllLines(_indicesFile);
        for (var position = 0; position < lines.Length; position++)
        {
            if (int.TryParse(lines[position], CultureInfo.InvariantCulture, out var index) && index == exampleId)
            {
                return position;
            }
        }

        return null;
    }

    /// <summary>The answer a cache row holds, as the outputs' own values.</summary>
    /// <param name="row">The row, header excluded.</param>
    /// <returns>The values, one per output.</returns>
    private JsonArray CachedValues(List<string> row)
    {
        var values = new JsonArray();

        for (var index = 0; index < _outputs.Count; index++)
        {
            var cell = index < row.Count ? row[index] : string.Empty;
            values.Add(_outputs[index].ReadFromFlag(cell));
        }

        return values;
    }

    /// <summary>The sample a gallery value names.</summary>
    /// <param name="value">The value the gallery handed the handler.</param>
    /// <returns>The sample's position.</returns>
    /// <exception cref="ArgumentException">The value does not name a sample.</exception>
    /// <remarks>
    /// Two shapes reach this: the tuple a <c>tuple</c>-typed dataset produces, and the array a
    /// preload's state holds. Both name the sample first, which is the only part the answer needs.
    /// </remarks>
    private static int ExampleValueOf(object? value) => value switch
    {
        (int index, _) => index,
        JsonArray array when array.Count > 0 && JsonValues.Int32(array[0]) is { } index => index,
        JsonValue scalar when JsonValues.Int32(scalar) is { } index => index,
        _ => throw new ArgumentException(
            "A gallery's value is the sample that was picked, and this one names none.",
            nameof(value)),
    };

    /// <summary>A sample as the JSON a preload's state holds.</summary>
    /// <param name="sample">The sample's position.</param>
    /// <returns>The position followed by the sample's values.</returns>
    private JsonArray ExampleValueAsJson(int sample)
    {
        var values = new JsonArray { sample };

        foreach (var value in _nonNoneExamples[sample])
        {
            values.Add(JsonValues.FromAnyObject(value));
        }

        return values;
    }

    /// <summary>A sample's values, in the order the inputs with examples are in.</summary>
    /// <param name="exampleId">The sample's position.</param>
    /// <returns>The values, with <see cref="_unchanged"/> where a column is hidden and the sample is not the one the gallery drew.</returns>
    private object?[] ExampleValueOfSample(int exampleId)
    {
        var values = _nonNoneExamples[exampleId];

        // A sample the gallery drew whole is the sample itself. One that has been edited in the
        // table may name every input, or only the visible ones; a hidden column keeps what the
        // fields already hold rather than being cleared.
        if (_visiblePositions.Count == _hasExamples.Count(keep => keep))
        {
            return values;
        }

        if (exampleId < _visibleExamples.Count && SameAs(values, _visibleExamples[exampleId]))
        {
            return values;
        }

        if (values.Length == _hasExamples.Count(keep => keep))
        {
            return values;
        }

        var merged = new object?[_hasExamples.Count(keep => keep)];
        Array.Fill(merged, _unchanged);

        for (var position = 0; position < _visiblePositions.Count && position < values.Length; position++)
        {
            merged[_visiblePositions[position]] = values[position];
        }

        return merged;
    }

    /// <summary>A sample's values, postprocessed by the components that draw them.</summary>
    /// <param name="sample">The sample's values, in the order the inputs with examples are in.</param>
    /// <returns>One value per input with examples, with <see cref="_unchanged"/> kept as it is.</returns>
    private JsonNode?[] ProcessedExample(object?[] sample)
    {
        var inputs = _inputs.Where((_, index) => _hasExamples[index]).ToArray();
        var processed = new JsonNode?[inputs.Length];

        for (var index = 0; index < inputs.Length; index++)
        {
            if (index >= sample.Length || ReferenceEquals(sample[index], _unchanged))
            {
                processed[index] = null;
                continue;
            }

            processed[index] = inputs[index].Postprocess(sample[index]);
        }

        return processed;
    }

    /// <summary>A gallery value, as the sample's values.</summary>
    /// <param name="value">The gallery's value.</param>
    /// <returns>The sample's values, in the order the inputs with examples are in.</returns>
    /// <remarks>
    /// Two shapes reach this — the tuple a <c>tuple</c>-typed dataset produces, and the array a
    /// preload's state holds — and both name the sample the same way, which is all the values need.
    /// </remarks>
    private object?[] SampleOf(object? value) => ExampleValueOfSample(ExampleValueOf(value));

    /// <summary>One value for a binding that has one output, and the list for one that has several.</summary>
    /// <param name="values">The values.</param>
    /// <returns>The value itself, or the list.</returns>
    /// <remarks>
    /// The Python version's <c>resolve_singleton</c>: a single output is handed one value, not a
    /// list of one. The value is copied out rather than handed over in place, because a JSON node
    /// belongs to one parent and this one is about to be given to a run that will put it somewhere
    /// else.
    /// </remarks>
    private static JsonNode Single(JsonArray values) =>
        values.Count == 1 ? values[0]!.DeepClone() : values;

    /// <summary>An update that changes nothing, which is what a hidden column sends.</summary>
    /// <returns>The update.</returns>
    private static JsonObject Update() => new() { ["__type__"] = "update" };

    /// <summary>The columns the gallery shows.</summary>
    /// <param name="visibleColumns">The columns the application asked for, or <see langword="null"/>.</param>
    /// <param name="hasExamples">Which inputs have a value in at least one sample.</param>
    /// <param name="count">How many inputs there are.</param>
    /// <returns>The inputs to draw, by position.</returns>
    /// <exception cref="ArgumentException">A column is not an input, or none of them has a value.</exception>
    private static List<int> VisibleIndices(IReadOnlyList<int>? visibleColumns, bool[] hasExamples, int count)
    {
        var withExamples = new List<int>();
        for (var index = 0; index < count; index++)
        {
            if (hasExamples[index])
            {
                withExamples.Add(index);
            }
        }

        if (visibleColumns is null)
        {
            return withExamples;
        }

        if (visibleColumns.Count == 0)
        {
            throw new ArgumentException(
                "A gallery shows at least one column.",
                nameof(visibleColumns));
        }

        foreach (var column in visibleColumns)
        {
            if (column < 0 || column >= count)
            {
                throw new ArgumentException(
                    $"A column index has to name an input; {column} does not.",
                    nameof(visibleColumns));
            }
        }

        var wanted = visibleColumns.ToHashSet();

        // None of the columns the application asked for has a sample value, so every column is
        // shown rather than an empty table — the Python version's rule.
        return withExamples.Any(wanted.Contains)
            ? [.. withExamples.Where(wanted.Contains)]
            : withExamples;
    }

    /// <summary>Where each visible column sits among the inputs that have examples.</summary>
    /// <param name="visibleInputIndices">The inputs drawn, by position among all inputs.</param>
    /// <param name="hasExamples">Which inputs have a value in at least one sample.</param>
    /// <returns>The positions within a sample's values.</returns>
    private static List<int> VisiblePositionsOf(IReadOnlyList<int> visibleInputIndices, bool[] hasExamples)
    {
        var positions = new List<int>();
        var withExamples = new List<int>();

        for (var index = 0; index < hasExamples.Length; index++)
        {
            if (hasExamples[index])
            {
                withExamples.Add(index);
            }
        }

        foreach (var input in visibleInputIndices)
        {
            positions.Add(withExamples.IndexOf(input));
        }

        return positions;
    }

    /// <summary>The values a sample shows in the gallery.</summary>
    /// <param name="row">The sample.</param>
    /// <param name="visibleInputIndices">The inputs drawn, by position among all inputs.</param>
    /// <returns>One value per drawn column.</returns>
    private static object?[] VisibleOf(object?[] row, IReadOnlyList<int> visibleInputIndices) =>
        [.. visibleInputIndices.Select(index => index < row.Length ? row[index] : null)];

    /// <summary>A sample's values with the columns that have none removed.</summary>
    /// <param name="row">The sample.</param>
    /// <param name="hasExamples">Which inputs have a value in at least one sample.</param>
    /// <returns>The values that are kept.</returns>
    private static object?[] Keep(object?[] row, bool[] hasExamples)
    {
        var kept = new List<object?>(row.Length);

        for (var index = 0; index < row.Length && index < hasExamples.Length; index++)
        {
            if (hasExamples[index])
            {
                kept.Add(row[index]);
            }
        }

        return [.. kept];
    }

    /// <summary>Whether two samples hold the same values.</summary>
    /// <param name="left">One sample.</param>
    /// <param name="right">The other.</param>
    /// <returns>Whether they are the same.</returns>
    private static bool SameAs(object?[] left, object?[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        for (var index = 0; index < left.Length; index++)
        {
            if (!JsonNode.DeepEquals(
                JsonValues.FromAnyObject(left[index]),
                JsonValues.FromAnyObject(right[index])))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Reads the samples the application gave.</summary> <param name="examples">The
    /// samples, in any of the three forms the Python version accepts.</param>
    /// <param name="inputCount">How many inputs a sample fills.</param> <returns>One row per
    /// sample.</returns> <exception cref="ArgumentException">The samples are not a sequence or a
    /// directory path.</exception> <exception cref="DirectoryNotFoundException">The directory does
    /// not exist.</exception>
    private static List<object?[]> ReadSamples(object examples, int inputCount)
    {
        if (examples is string path)
        {
            return ReadSamplesFromDirectory(path, inputCount);
        }

        if (examples is not System.Collections.IEnumerable rows)
        {
            throw new ArgumentException(
                $"Samples are a sequence or a directory path; got {examples.GetType().Name}.",
                nameof(examples));
        }

        var samples = new List<object?[]>();

        foreach (var example in rows)
        {
            samples.Add(example switch
            {
                object?[] row => row,
                IReadOnlyList<object?> row => [.. row],
                null => [null],
                _ => [example],
            });
        }

        return samples;
    }

    /// <summary>Reads the samples a directory holds.</summary>
    /// <param name="path">The directory.</param>
    /// <param name="inputCount">How many inputs a sample fills.</param>
    /// <returns>One row per sample.</returns>
    /// <exception cref="DirectoryNotFoundException">The directory does not exist.</exception>
    /// <exception cref="FileNotFoundException">There is no <c>log.csv</c> and more than one input.</exception>
    /// <remarks>
    /// The Python version's rule: a <c>log.csv</c> lists the samples, and a directory without one
    /// is only usable when there is a single input, in which case each file is a sample.
    /// </remarks>
    private static List<object?[]> ReadSamplesFromDirectory(string path, int inputCount)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Could not find examples directory: {path}");
        }

        var log = Path.Combine(path, "log.csv");
        if (!System.IO.File.Exists(log))
        {
            if (inputCount != 1)
            {
                throw new FileNotFoundException(
                    "Could not find log file (required for multiple inputs): log.csv",
                    log);
            }

            return [.. Directory.GetFileSystemEntries(path).Select(entry => new object?[] { entry })];
        }

        var rows = ReadCsv(log);

        return [.. rows.Skip(1).Select(row => (object?[])[.. row.Take(inputCount)])];
    }

    /// <summary>Reads a CSV file.</summary>
    /// <param name="path">The file.</param>
    /// <returns>One entry per row.</returns>
    private static List<List<string>> ReadCsv(string path)
    {
        var rows = new List<List<string>>();

        using var reader = new StreamReader(path, System.Text.Encoding.UTF8);
        while (ReadCsvRow(reader) is { } row)
        {
            rows.Add(row);
        }

        return rows;
    }

    /// <summary>Reads one CSV row, or <see langword="null"/> at the end of the file.</summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The cells.</returns>
    private static List<string>? ReadCsvRow(TextReader reader)
    {
        var cells = new List<string>();
        var cell = new System.Text.StringBuilder();
        var quoted = false;
        var any = false;

        while (true)
        {
            var read = reader.Read();
            if (read < 0)
            {
                if (!any)
                {
                    return null;
                }

                break;
            }

            any = true;
            var character = (char)read;

            if (quoted)
            {
                if (character == '"')
                {
                    if (reader.Peek() == '"')
                    {
                        reader.Read();
                        cell.Append('"');
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else
                {
                    cell.Append(character);
                }

                continue;
            }

            switch (character)
            {
                case '"':
                    quoted = true;
                    break;
                case ',':
                    cells.Add(cell.ToString());
                    cell.Clear();
                    break;
                case '\r':
                    break;
                case '\n':
                    cells.Add(cell.ToString());
                    return cells;
                default:
                    cell.Append(character);
                    break;
            }
        }

        cells.Add(cell.ToString());

        return cells;
    }

    /// <summary>Whether the environment turns caching on.</summary>
    /// <param name="fn">The handler, or <see langword="null"/>.</param>
    /// <param name="outputs">The outputs, or <see langword="null"/>.</param>
    /// <returns>Whether <c>GRADIO_CACHE_EXAMPLES</c> is true and there is something to cache.</returns>
    private static bool CacheFromEnvironment(Delegate? fn, IReadOnlyList<Component>? outputs)
    {
        var setting = Environment.GetEnvironmentVariable("GRADIO_CACHE_EXAMPLES");

        return setting is not null
            && (setting.Equals("true", StringComparison.OrdinalIgnoreCase)
                || setting.Equals("lazy", StringComparison.OrdinalIgnoreCase))
            && fn is not null
            && outputs is { Count: > 0 };
    }

    /// <summary>The caching mode in force.</summary>
    /// <param name="requested">What the application asked for, or <see langword="null"/>.</param>
    /// <param name="caching">Whether anything is cached at all.</param>
    /// <returns><c>eager</c> or <c>lazy</c>.</returns>
    /// <remarks>
    /// The Python version reads <c>GRADIO_CACHE_MODE</c> when the application named none and falls
    /// back to <c>eager</c> — including when the variable holds something else, which it reports
    /// and then ignores.
    /// </remarks>
    private static string CacheModeOf(string? requested, bool caching)
    {
        var mode = requested ?? Environment.GetEnvironmentVariable("GRADIO_CACHE_MODE")?.ToLowerInvariant();
        mode = mode is "eager" or "lazy" ? mode : "eager";

        return caching ? mode : "eager";
    }

    /// <summary>The folder the answers are kept in.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The folder, made if it does not exist.</returns>
    /// <remarks>
    /// <c>GRADIO_EXAMPLES_CACHE</c> when it is set, and the application's own folder otherwise —
    /// the Python version's default is relative to the working directory, which would put a second
    /// application's answers in the first one's folder and outside what the file endpoint serves.
    /// </remarks>
    private static string CacheRoot(Blocks blocks)
    {
        var root = Environment.GetEnvironmentVariable("GRADIO_EXAMPLES_CACHE");
        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(blocks.UploadFolder, "cached_examples");
        }

        Directory.CreateDirectory(root);

        return root;
    }

    /// <summary>The gallery itself, as the binding names it.</summary>
    /// <returns>The gallery.</returns>
    private Dataset _datasetInput() => Dataset;
}

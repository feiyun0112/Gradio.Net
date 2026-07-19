using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net.Core;

public class Examples
{
    private const string LOG_FILE = "log.csv";
    public bool CacheExamples { get; set; }
    public string CacheMode { get; set; }
    public object ExamplesData { get; set; }
    public List<Component> Inputs { get; set; }
    public List<Component> Outputs { get; set; }
    public Delegate? Fn { get; set; }
    public bool ApiMode { get; set; }
    public bool Preprocess { get; set; }
    public bool Postprocess { get; set; }
    public string ApiVisibility { get; set; }
    public string? ApiName { get; set; }
    public object? ApiDescription { get; set; }
    public bool Batch { get; set; }
    public List<string>? ExampleLabels { get; set; }
    public string WorkingDirectory { get; set; }
    public int Preload { get; set; }
    public Dataset Dataset { get; set; }
    public Gradio.Net.Core.Flagging.CSVLogger CacheLogger { get; set; }
    public string CachedFolder { get; set; }
    public string CachedFile { get; set; }
    public string CachedIndicesFile { get; set; }
    public bool RunOnClick { get; set; }
    public Dependency? CacheEvent { get; set; }
    public Dependency? LoadInputEvent { get; set; }
    public Dictionary<object, List<object>> NonNoneProcessedExamples { get; set; }
    public List<bool> InputHasExamples { get; set; }
    public List<Component> InputsWithExamples { get; set; }
    public List<List<object>> NonNoneExamples { get; set; }
    public Blocks? RootBlock { get; set; }

    public Examples(
        object examples,
        object inputs,
        object? outputs = null,
        Delegate? fn = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        bool apiMode = false,
        object? label = null,
        string? elemId = null,
        bool runOnClick = false,
        bool preprocess = true,
        bool postprocess = true,
        string apiVisibility = "undocumented",
        string? apiName = "load_example",
        object? apiDescription = null,
        bool batch = false,
        List<string>? exampleLabels = null,
        object? visible = null,
        object? preload = null,
        bool initiatedDirectly = true
    )
    {
        if (initiatedDirectly)
        {
        }

        // Initialize cache examples setting
        CacheExamples = false;
        if (cacheExamples == null)
        {
            var envValue = Environment.GetEnvironmentVariable("GRADIO_CACHE_EXAMPLES")?.ToLower();
            if ((envValue == "true" || envValue == "lazy") && fn != null && outputs != null)
            {
                CacheExamples = true;
            }
        }
        else
        {
            CacheExamples = cacheExamples.Value;
        }

        if (CacheExamples && (fn == null || outputs == null))
        {
            throw new ArgumentException("If caching examples, `fn` and `outputs` must be provided");
        }

        // Initialize cache mode
        CacheMode = cacheMode ?? "eager";
        var cacheModeEnv = Environment.GetEnvironmentVariable("GRADIO_CACHE_MODE");
        if (!string.IsNullOrEmpty(cacheModeEnv) && cacheMode == null)
        {
            var envMode = cacheModeEnv.ToLower();
            if (envMode == "lazy")
            {
                CacheMode = "lazy";
            }
            else if (envMode == "eager")
            {
                CacheMode = "eager";
            }
            else
            {
                CacheMode = "eager";
            }
        }

        // Convert inputs and outputs to lists
        Inputs = NormalizeComponents(inputs);
        Outputs = NormalizeComponents(outputs);

        // Process examples data
        WorkingDirectory = Directory.GetCurrentDirectory();
        ProcessExamples(examples);

        // Validate example labels
        if (exampleLabels != null && exampleLabels.Count != NonNoneExamples.Count)
        {
            throw new ArgumentException("If `example_labels` are provided, the length of `example_labels` must be the same as the number of examples.");
        }

        // Set properties
        Fn = fn;
        ApiMode = apiMode;
        Preprocess = preprocess;
        Postprocess = postprocess;
        ApiVisibility = apiVisibility;
        ApiName = apiName;
        ApiDescription = apiDescription;
        Batch = batch;
        ExampleLabels = exampleLabels;
        RunOnClick = runOnClick;
        Preload = preload is int preloadInt ? preloadInt : 0;

        // Create dataset component
        Directory.SetCurrentDirectory(WorkingDirectory);
        Dataset = new Dataset(
            components: InputsWithExamples,
            samples: NonNoneExamples.Select(ex => ex.ToList()).ToList(),
            type: "tuple",
            label: label,
            samplesPerPage: examplesPerPage,
            elemId: elemId,
            visible: visible,
            sampleLabels: exampleLabels
        );
        Directory.SetCurrentDirectory(Environment.CurrentDirectory);

        // Initialize cache
        CacheLogger = new Gradio.Net.Core.Flagging.CSVLogger(false, false, "log.csv");
        CachedFolder = Path.Combine(Utils.Utils.GetCacheFolder(), Dataset._id.ToString());

        // Reset cache if requested
        if (Environment.GetEnvironmentVariable("GRADIO_RESET_EXAMPLES_CACHE") == "True" && Directory.Exists(CachedFolder))
        {
            Directory.Delete(CachedFolder, true);
        }

        CachedFile = Path.Combine(CachedFolder, "log.csv");
        CachedIndicesFile = Path.Combine(CachedFolder, "indices.csv");
        NonNoneProcessedExamples = new Dictionary<object, List<object>>();

        // Process examples
        if (Dataset.Samples != null)
        {
            for (int index = 0; index < NonNoneExamples.Count; index++)
            {
                var example = NonNoneExamples[index];
                NonNoneProcessedExamples[example] = GetProcessedExample(example);
            }
        }

        // Show cache message
        if (CacheMode == "lazy")
        {
            if (File.Exists(CachedFile))
            {
            }
        }
    }

    private void ProcessExamples(object examples)
    {
        if (examples == null)
        {
            throw new ArgumentException("The parameter `examples` cannot be None");
        }

        var exampleList = new List<List<object>>();

        if (examples is string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Could not find examples directory: {directoryPath}");
            }

            WorkingDirectory = directoryPath;
            var logFile = Path.Combine(directoryPath, LOG_FILE);

            if (!File.Exists(logFile))
            {
                if (Inputs.Count == 1)
                {
                    // Single input, use directory files
                    var files = Directory.GetFiles(directoryPath).Select(Path.GetFileName).ToList();
                    exampleList = files.Select(f => new List<object> { f }).ToList();
                }
                else
                {
                    throw new FileNotFoundException($"Could not find log file (required for multiple inputs): {LOG_FILE}");
                }
            }
            else
            {
                // Load from log file
                using (var reader = new StreamReader(logFile))
                {
                    var csvReader = new System.Globalization.CultureInfo("en-US");
                    var lines = reader.ReadToEnd().Split('\n').Where(l => !string.IsNullOrEmpty(l)).ToList();

                    for (int i = 1; i < lines.Count; i++) // Skip header
                    {
                        var parts = lines[i].Split(',').Take(Inputs.Count).ToList();
                        exampleList.Add(parts.Cast<object>().ToList());
                    }
                }
            }
        }
        else if (TryToObjectList(examples, out var list))
        {
            if (list.Count == 0 || IsSequenceLike(list[0]))
            {
                // Nested list format
                foreach (var item in list)
                {
                    if (!TryToObjectList(item, out var row))
                    {
                        throw new ArgumentException("The parameter `examples` must either be a string directory or a list (if there is only 1 input component) or (more generally), a nested list, where each sublist represents a set of inputs.");
                    }
                    exampleList.Add(row);
                }
            }
            else if (Inputs.Count == 1)
            {
                // Single input, convert to nested list
                exampleList = list.Select(item => new List<object> { item }).ToList();
            }
            else
            {
                throw new ArgumentException("The parameter `examples` must either be a string directory or a list (if there is only 1 input component) or (more generally), a nested list, where each sublist represents a set of inputs.");
            }
        }
        else
        {
            throw new ArgumentException("The parameter `examples` must either be a string directory or a list (if there is only 1 input component) or (more generally), a nested list, where each sublist represents a set of inputs.");
        }

        // Process input has examples flags
        InputHasExamples = new List<bool>(new bool[Inputs.Count]);
        foreach (var example in exampleList)
        {
            for (int idx = 0; idx < example.Count; idx++)
            {
                if (example[idx] != null)
                {
                    try
                    {
                        InputHasExamples[idx] = true;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Ignore if more example components than inputs
                    }
                }
            }
        }

        // Filter inputs and examples
        InputsWithExamples = Inputs.Where((inp, i) => i < InputHasExamples.Count && InputHasExamples[i]).ToList();
        NonNoneExamples = exampleList.Select(example =>
            example.Where((ex, i) => i < InputHasExamples.Count && InputHasExamples[i]).ToList()
        ).ToList();

        ExamplesData = exampleList;
    }

    private static List<Component> NormalizeComponents(object? components)
    {
        if (components == null)
        {
            return new List<Component>();
        }

        if (components is Component single)
        {
            return new List<Component> { single };
        }

        if (components is IEnumerable<Component> typedSequence)
        {
            return typedSequence.Where(c => c != null).ToList();
        }

        if (components is IEnumerable sequence && components is not string)
        {
            return sequence.Cast<object?>().OfType<Component>().ToList();
        }

        return new List<Component>();
    }

    private static bool IsSequenceLike(object? value)
    {
        return value is IEnumerable && value is not string;
    }

    private static bool TryToObjectList(object? value, out List<object> list)
    {
        if (value is string || value is null)
        {
            list = new List<object>();
            return false;
        }

        if (value is IEnumerable sequence)
        {
            list = new List<object>();
            foreach (var item in sequence)
            {
                list.Add(item!);
            }
            return true;
        }

        list = new List<object>();
        return false;
    }

    public List<object> GetProcessedExample(List<object> example)
    {
        if (NonNoneProcessedExamples.TryGetValue(example, out var processed))
        {
            return processed;
        }

        var originalDirectory = Environment.CurrentDirectory;
        try
        {
            Directory.SetCurrentDirectory(WorkingDirectory);
            var result = new List<object>();
            for (int i = 0; i < InputsWithExamples.Count && i < example.Count; i++)
            {
                var component = InputsWithExamples[i];
                var sample = example[i];

                var predictionValue = component.Postprocess(sample);

                // Handle Gradio models
                if (predictionValue is GradioModel model)
                {
                    predictionValue = model.ModelDump();
                }
                else if (predictionValue is GradioRootModel<object> rootModel)
                {
                    predictionValue = rootModel.ModelDump();
                }

                // Move files to cache
                predictionValue = ProcessingUtils.MoveFilesToCache(predictionValue, component, true);
                result.Add(predictionValue);
            }

            NonNoneProcessedExamples[example] = result;
            return result;
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
        }
    }

    public void Create()
    {
        var blocksConfig = Context.GetBlocksContext();
        RootBlock = Context.RootBlock ?? (blocksConfig?.RootBlock ?? null);

        if (blocksConfig != null)
        {
            if (RootBlock != null)
            {
                RootBlock.ExtraStartupEvents.Add(StartCaching);
            }

            if (CacheExamples)
            {
                // Python parity: dataset.click(load_input).then(load_output)
                var clickTargets = new List<EventListenerMethod> { new EventListenerMethod(Dataset, "click") };

                var loadInputDelegate = new Func<object, object>(LoadExampleInput);
                var (loadInputFn, loadInputIndex) = blocksConfig.SetEventTrigger(
                    targets: clickTargets,
                    fn: loadInputDelegate,
                    inputs: new Block[] { Dataset },
                    outputs: InputsWithExamples,
                    apiName: ApiName,
                    apiDescription: ApiDescription,
                    apiVisibility: ApiVisibility,
                    queue: false,
                    preprocess: true,
                    postprocess: true
                );
                var loadInputDep = new Dependency(Dataset, loadInputFn.GetConfig(), loadInputIndex, loadInputDelegate);

                var loadOutputDelegate = new Func<object, object>(LoadExampleOutput);
                var loadOutputDep = loadInputDep.Then(
                    fn: loadOutputDelegate,
                    inputs: new Block[] { Dataset },
                    outputs: Outputs,
                    apiName: null,
                    scrollToOutput: false,
                    showProgress: "hidden",
                    queue: false,
                    apiVisibility: ApiVisibility,
                    preprocess: true,
                    postprocess: true
                );

                CacheEvent = loadOutputDep;
                LoadInputEvent = loadInputDep;

                // Handle preload (best-effort): set initial values on inputs.
                if (Preload is int preloadIndex && RootBlock != null)
                {
                    var hasDeveloperValue = InputsWithExamples.Any(inp =>
                        inp.ConstructorArgs != null
                        && inp.ConstructorArgs.ContainsKey("value")
                        && inp.ConstructorArgs["value"] != null);

                    if (!hasDeveloperValue && preloadIndex >= 0 && preloadIndex < NonNoneExamples.Count)
                    {
                        var preloadExample = NonNoneExamples[preloadIndex];
                        var processedExample = GetProcessedExample(preloadExample);
                        for (int i = 0; i < InputsWithExamples.Count && i < processedExample.Count; i++)
                        {
                            var component = InputsWithExamples[i];
                            component.ConstructorArgs["value"] = processedExample[i];
                        }
                    }
                }
            }
            else
            {
                // Basic: dataset.click(load_example)
                var clickTargets = new List<EventListenerMethod> { new EventListenerMethod(Dataset, "click") };
                var loadDelegate = new Func<object, object>(LoadExample);
                var (loadFn, loadIndex) = blocksConfig.SetEventTrigger(
                    targets: clickTargets,
                    fn: loadDelegate,
                    inputs: new Block[] { Dataset },
                    outputs: InputsWithExamples,
                    apiName: ApiName,
                    apiDescription: ApiDescription,
                    apiVisibility: ApiVisibility,
                    queue: false,
                    preprocess: true,
                    postprocess: true
                );
                var loadDep = new Dependency(Dataset, loadFn.GetConfig(), loadIndex, loadDelegate);
                LoadInputEvent = loadDep;

                if (RunOnClick)
                {
                    if (Fn == null)
                    {
                        throw new ArgumentException("Cannot run_on_click if no function is provided");
                    }

                    loadDep.Then(
                        fn: Fn,
                        inputs: Inputs,
                        outputs: Outputs,
                        apiName: null,
                        scrollToOutput: false,
                        showProgress: "full",
                        queue: true,
                        apiVisibility: ApiVisibility,
                        preprocess: true,
                        postprocess: true
                    );
                }
            }
        }
        else
        {
        }
    }

    private object LoadExampleInput(object exampleTuple)
    {
        // Load and process example input data
        if (TryGetExampleTuple(exampleTuple, out var _, out var exampleValue))
        {
            var processedExample = GetProcessedExample(exampleValue);
            return processedExample;
        }
        return exampleTuple;
    }

    private object LoadExampleOutput(object exampleTuple)
    {
        // Load cached output for example
        if (TryGetExampleTuple(exampleTuple, out var exampleId, out var _))
        {
            try
            {
                var cachedOutputs = LoadFromCache(exampleId);
                return cachedOutputs;
            }
            catch
            {
                // If cache loading fails, return empty result
                return new List<object>();
            }
        }
        return new List<object>();
    }

    private object LoadExample(object exampleTuple)
    {
        // Load example and update component values
        if (TryGetExampleTuple(exampleTuple, out var _, out var exampleValue))
        {
            var processedExample = GetProcessedExample(exampleValue);

            if (InputsWithExamples.Count == 1)
            {
                return Helpers.Update(kwargs: new[] { new KeyValuePair<string, object>("value", processedExample[0]) });
            }
            else
            {
                return InputsWithExamples.Select((comp, i) =>
                    Helpers.Update(kwargs: new[] { new KeyValuePair<string, object>("value", processedExample[i]) })
                ).Cast<object>().ToList();
            }
        }
        return null;
    }

    private static bool TryGetExampleTuple(object exampleTuple, out int exampleId, out List<object> exampleValue)
    {
        exampleId = default;
        exampleValue = new List<object>();

        if (exampleTuple == null)
        {
            Console.Error.WriteLine("[Gradio.Net][Examples] exampleTuple is null");
            return false;
        }

        if (exampleTuple is Tuple<int, List<object>> tuple)
        {
            exampleId = tuple.Item1;
            exampleValue = tuple.Item2;
            return true;
        }

        if (exampleTuple is ValueTuple<int, List<object>> valueTuple)
        {
            exampleId = valueTuple.Item1;
            exampleValue = valueTuple.Item2;
            return true;
        }

        if (exampleTuple is ValueTuple<int, object> valueTupleObj && valueTupleObj.Item2 is List<object> valueList)
        {
            exampleId = valueTupleObj.Item1;
            exampleValue = valueList;
            return true;
        }

        Console.Error.WriteLine($"[Gradio.Net][Examples] Unexpected exampleTuple type: {exampleTuple.GetType().FullName}");

        return false;
    }

    public async Task StartCaching()
    {
        if (CacheExamples)
        {
            // Check if all inputs have examples
            foreach (var example in (List<List<object>>)ExamplesData)
            {
                var nonNullCount = example.Count(ex => ex != null);
                if (nonNullCount != Inputs.Count)
                {
                    break;
                }
            }
        }

        if (CacheExamples)
        {
            await Cache();
        }
    }

    public async Task Cache(int? exampleId = null)
    {
        if (RootBlock == null)
        {
            throw new Exception("Cannot cache examples if not in a Blocks context.");
        }

        if (File.Exists(CachedFile) && exampleId == null)
        {
            return;
        }

        CacheLogger.Setup(Outputs, CachedFolder);

        // Create event trigger
        var eventListener = new EventListenerMethod(RootBlock, "load");
        var (eventId, fnIndex) = RootBlock.DefaultConfig.SetEventTrigger(
            new List<EventListenerMethod> { eventListener },
            fn: Fn,
            inputs: Inputs,
            outputs: Outputs,
            preprocess: Preprocess && !ApiMode,
            postprocess: Postprocess && !ApiMode,
            batch: Batch
        );

        // Process each example
        for (int i = 0; i < NonNoneExamples.Count; i++)
        {
            if (exampleId != null && i != exampleId)
            {
                continue;
            }

            var example = NonNoneExamples[i];
            var processedInput = GetProcessedExample(example);

            // Insert None values for inputs without examples
            for (int j = 0; j < InputHasExamples.Count; j++)
            {
                if (!InputHasExamples[j])
                {
                    processedInput.Insert(j, null);
                }
            }

            // Run prediction
            var originalDirectory = Environment.CurrentDirectory;
            try
            {
                Directory.SetCurrentDirectory(WorkingDirectory);
                // Get the block function
                if (RootBlock.DefaultConfig.Fns.TryGetValue(fnIndex, out var blockFn))
                {
                    // Process API call
                    var prediction = await RootBlock.ProcessApi(
                        blockFn: blockFn,
                        inputs: processedInput,
                        request: null,
                        state: null,
                        iterator: null,
                        sessionHash: "",
                        eventId: "",
                        eventData: null
                    );
                    var output = prediction?.ContainsKey("data") == true ? prediction["data"] as List<object> : new List<object>();
                    if (output != null)
                    {
                        CacheLogger.Flag(output);

                        // Record index
                        using (var writer = new StreamWriter(CachedIndicesFile, true))
                        {
                            writer.WriteLine(exampleId ?? i);
                        }
                    }
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        // Remove the temporary event
        if (RootBlock.DefaultConfig.Fns.ContainsKey(fnIndex))
        {
            RootBlock.DefaultConfig.Fns.Remove(fnIndex);
        }
    }

    public List<object> LoadFromCache(int exampleId)
    {
        var cachedIndex = GetCachedIndexIfCached(exampleId);
        if (cachedIndex == null)
        {
            // Cache on demand
            Cache(exampleId).Wait();

            // Get the last index
            using (var reader = new StreamReader(CachedIndicesFile))
            {
                cachedIndex = File.ReadLines(CachedIndicesFile).Count() - 1;
            }
        }

        // Read from cache file
        using (var reader = new StreamReader(CachedFile))
        {
            var lines = reader.ReadToEnd().Split('\n').Where(l => !string.IsNullOrEmpty(l)).ToList();

            if (cachedIndex.Value + 1 >= lines.Count)
            {
                throw new IndexOutOfRangeException("Cached example not found in cache file");
            }

            var exampleLine = lines[cachedIndex.Value + 1]; // Skip header
            var parts = exampleLine.Split(',');

            var output = new List<object>();
            for (int i = 0; i < Outputs.Count && i < parts.Length; i++)
            {
                var component = Outputs[i];
                var value = parts[i];

                try
                {
                    // Try to parse as dict
                    var valueAsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(value);
                    output.Add(valueAsDict);
                }
                catch
                {
                    try
                    {
                        // Try to parse as list (for File components with multiple files)
                        var valueAsList = System.Text.Json.JsonSerializer.Deserialize<List<object>>(value);
                        output.Add(valueAsList);
                    }
                    catch
                    {
                        // Fallback to component's read_from_flag
                        output.Add(component.ReadFromFlag(value));
                    }
                }
            }

            return output;
        }
    }

    private int? GetCachedIndexIfCached(int exampleId)
    {
        if (!File.Exists(CachedIndicesFile))
        {
            return null;
        }

        using (var reader = new StreamReader(CachedIndicesFile))
        {
            var lines = reader.ReadToEnd().Split('\n').Where(l => !string.IsNullOrEmpty(l)).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                if (int.TryParse(lines[i], out var idx) && idx == exampleId)
                {
                    return i;
                }
            }
        }

        return null;
    }

    public async Task<List<object>> PostprocessOutput(List<object> output)
    {
        /*
         * This is a way that we can postprocess the data manually, since we set postprocess=False in the lazy_cache
         * event handler. The reason we did that is because we don't want to postprocess data if we are loading from
         * the cache, since that has already been postprocessed. We postprocess this data manually if we are calling
         * the function using the _handle_callable_as_generator() method.
         */
        var demo = new Blocks();
        try
        {
            // Render all output components
            foreach (var outputComponent in Outputs)
            {
                outputComponent.Render();
            }

            // Add event listener method
            var eventListener = new EventListenerMethod(demo, "load");
            var (eventId, fnIndex) = demo.DefaultConfig.SetEventTrigger(
                new List<EventListenerMethod> { eventListener },
                fn: Fn,
                inputs: Inputs,
                outputs: Outputs,
                preprocess: true,
                postprocess: true,
                batch: false
            );

            // Get the function from the config
            if (demo.DefaultConfig.Fns.TryGetValue(fnIndex, out var blockFn))
            {
                // In a full implementation, this would call demo.PostprocessData()
                // For now, we'll manually postprocess each output component
                var postprocessedOutput = new List<object>();
                for (int i = 0; i < output.Count && i < Outputs.Count; i++)
                {
                    var outputComponent = Outputs[i];
                    var outputValue = output[i];
                    var postprocessedValue = outputComponent.Postprocess(outputValue);
                    postprocessedOutput.Add(postprocessedValue);
                }
                return postprocessedOutput;
            }

            return output;
        }
        finally
        {
            // Clean up
            demo.Unrender();
        }
    }
}


using System.Runtime.CompilerServices;
using Gradio.Net.Events;

namespace Gradio.Net.Core;

public class BlockFunction
{
    public BlockFunction(
        Delegate? fn,
        object inputs,
        object outputs,
        bool preprocess,
        bool postprocess,
        int id,
        bool inputsAsDict = false,
        List<(int? blockId, string eventName)>? targets = null,
        bool batch = false,
        int maxBatchSize = 4,
        object? concurrencyLimit = null,
        string? concurrencyId = null,
        bool tracksProgress = false,
        string? apiName = null,
        string? apiDescription = null,
        string? js = null,
        string showProgress = "full",
        object? showProgressOn = null,
        List<int>? cancels = null,
        bool collectsEventData = false,
        int? triggerAfter = null,
        bool triggerOnlyOnSuccess = false,
        bool triggerOnlyOnFailure = false,
        string triggerMode = "once",
        bool queue = true,
        bool scrollToOutput = false,
        string apiVisibility = "public",
        Renderable? renderable = null,
        Renderable? renderedIn = null,
        int? renderIteration = null,
        bool isCancelFunction = false,
        string connection = "sse",
        float? timeLimit = null,
        float streamEvery = 0.5f,
        List<string>? eventSpecificArgs = null,
        List<int>? componentPropInputs = null,
        string page = "",
        string? jsImplementation = null,
        object? key = null,
        Delegate? validator = null)
    {
        Fn = fn;
        Inputs = inputs;
        Outputs = outputs;
        Preprocess = preprocess;
        Postprocess = postprocess;
        Id = id;
        InputsAsDict = inputsAsDict;
        Targets = targets ?? new List<(int? blockId, string eventName)>();
        Batch = batch;
        MaxBatchSize = maxBatchSize;
        ConcurrencyLimit = concurrencyLimit ?? "default";
        ConcurrencyId = concurrencyId ?? fn?.Method.GetHashCode().ToString();
        TracksProgress = tracksProgress;
        ApiName = apiName;
        ApiDescription = apiDescription;
        Js = js;
        ShowProgress = showProgress;
        ShowProgressOn = showProgressOn;
        Cancels = cancels ?? new List<int>();
        CollectsEventData = collectsEventData;
        TriggerAfter = triggerAfter;
        TriggerOnlyOnSuccess = triggerOnlyOnSuccess;
        TriggerOnlyOnFailure = triggerOnlyOnFailure;
        TriggerMode = triggerMode;
        Queue = fn != null && queue;
        ScrollToOutput = scrollToOutput;
        ApiVisibility = apiVisibility;
        Renderable = renderable;
        RenderedIn = renderedIn;
        RenderIteration = renderIteration;
        IsCancelFunction = isCancelFunction;
        Connection = connection;
        TimeLimit = timeLimit;
        StreamEvery = streamEvery;
        EventSpecificArgsExplicit = eventSpecificArgs != null;
        EventSpecificArgs = eventSpecificArgs ?? new List<string>();
        ComponentPropInputs = componentPropInputs ?? new List<int>();
        Page = page;
        JsImplementation = jsImplementation;
        Key = key;
        Validator = validator;
        TotalRuntime = 0;
        TotalRuns = 0;

        // Check if the function is a generator
        TypesGenerator = IsGeneratorFunction(fn);

        // Call spaces_auto_wrap if spaces is available
        SpacesAutoWrap();
    }

    private bool IsGeneratorFunction(Delegate? fn)
    {
        if (fn == null)
        {
            return false;
        }

        var method = fn.Method;

        // Python parity: only treat actual generator/async-generator methods
        // (yield / async yield) as generators.
        var isIterator = method
            .GetCustomAttributes(typeof(IteratorStateMachineAttribute), inherit: true)
            .Any();
        var isAsyncIterator = method
            .GetCustomAttributes(typeof(AsyncIteratorStateMachineAttribute), inherit: true)
            .Any();

        return isIterator || isAsyncIterator;
    }

    public override string ToString()
    {
        string fnName = Fn?.Method.Name ?? "fn";
        return $"{{\"fn\": \"{fnName}\", \"preprocess\": {Preprocess}, \"postprocess\": {Postprocess}}}";
    }

    public Delegate? Fn { get; set; }
    public object Inputs { get; set; }
    public object Outputs { get; set; }
    public bool Preprocess { get; set; }
    public bool Postprocess { get; set; }
    public int Id { get; set; }
    public bool InputsAsDict { get; set; }
    public List<(int? blockId, string eventName)> Targets { get; set; }
    public bool Batch { get; set; }
    public int MaxBatchSize { get; set; }
    public object? ConcurrencyLimit { get; set; }
    public string? ConcurrencyId { get; set; }
    public bool TracksProgress { get; set; }
    public string? ApiName { get; set; }
    public string? BaseApiName { get; set; }
    public string? ApiDescription { get; set; }
    public string? Js { get; set; }
    public string ShowProgress { get; set; }
    public object? ShowProgressOn { get; set; }
    public bool Queue { get; set; }
    public bool ScrollToOutput { get; set; }
    public string ApiVisibility { get; set; }
    public Renderable? Renderable { get; set; }
    public Renderable? RenderedIn { get; set; }
    public int? RenderIteration { get; set; }
    public bool IsCancelFunction { get; set; }
    public string Connection { get; set; }
    public float? TimeLimit { get; set; }
    public float StreamEvery { get; set; }
    public bool EventSpecificArgsExplicit { get; set; }
    public List<string> EventSpecificArgs { get; set; }
    public List<int> ComponentPropInputs { get; set; }
    public string? Page { get; set; }
    public string? JsImplementation { get; set; }
    public object? Key { get; set; }
    public Delegate? Validator { get; set; }
    public List<int> Cancels { get; set; }
    public bool TypesGenerator { get; set; }
    public double TotalRuntime { get; set; }
    public int TotalRuns { get; set; }
    public bool CollectsEventData { get; set; }
    public int? TriggerAfter { get; set; }
    public bool TriggerOnlyOnSuccess { get; set; }
    public bool TriggerOnlyOnFailure { get; set; }
    public string TriggerMode { get; set; }

    public Dictionary<string, object> GetConfig()
    {
        // Convert targets to the required format
        var targetsList = new List<object>();
        foreach (var target in Targets)
        {
            targetsList.Add(new List<object> { target.blockId, target.eventName });
        }
        // Python parity: chained deps (.then / .success) have [[null, "then"]] or [[null, "success"]] as targets
        if (TriggerAfter.HasValue && Targets.Count == 0)
        {
            var chainEventName = TriggerOnlyOnSuccess ? "success" : "then";
            targetsList.Add(new List<object> { null, chainEventName });
        }

        // Extract input and output IDs
        List<int> inputIds = new List<int>();
        List<int> outputIds = new List<int>();

        if (Inputs is List<object> inputList)
        {
            inputIds = inputList.OfType<Block>().Select(b => b._id).ToList();
        }
        else if (Inputs is Block inputBlock)
        {
            inputIds.Add(inputBlock._id);
        }
        else if (Inputs is Array inputArray)
        {
            inputIds = inputArray.Cast<Block>().Select(b => b._id).ToList();
        }

        if (Outputs is List<object> outputList)
        {
            outputIds = outputList.OfType<Block>().Select(b => b._id).ToList();
        }
        else if (Outputs is Block outputBlock)
        {
            outputIds.Add(outputBlock._id);
        }
        else if (Outputs is Array outputArray)
        {
            outputIds = outputArray.Cast<Block>().Select(b => b._id).ToList();
        }

        // Convert show_progress_on to list of ids
        List<int>? showProgressOnIds = null;
        if (ShowProgressOn is List<object> showProgressOnList)
        {
            showProgressOnIds = showProgressOnList.OfType<Block>().Select(b => b._id).ToList();
        }
        else if (ShowProgressOn is Block showProgressOnBlock)
        {
            showProgressOnIds = new List<int> { showProgressOnBlock._id };
        }
        else if (ShowProgressOn is Array showProgressOnArray)
        {
            showProgressOnIds = showProgressOnArray.Cast<Block>().Select(b => b._id).ToList();
        }

        // Get js_implementation from function if not provided
        string? jsImpl = JsImplementation;
        if (jsImpl == null && Fn != null)
        {
            // Try to get js_implementation from the function's properties
            // This would require a custom attribute or property on the delegate
        }

        // Python parity: event_specific_args = self.event_specific_args
        // When EventSpecificArgsExplicit=true (set via EventListener-based events like gr.On),
        // serialize as [] even if empty. When false (e.g. gr.Render), serialize as null.
        object? eventSpecificArgsValue = null;
        if (EventSpecificArgs != null && EventSpecificArgs.Count > 0)
        {
            eventSpecificArgsValue = EventSpecificArgs;
        }
        else if (EventSpecificArgsExplicit)
        {
            eventSpecificArgsValue = new List<string>();
        }

        var config = new Dictionary<string, object>
        {
            { "id", Id },
            { "targets", targetsList },
            { "inputs", inputIds },
            { "outputs", outputIds },
            { "backend_fn", Fn != null },
            { "js", Js },
            { "queue", Queue },
            { "api_name", ApiName },
            { "api_description", ApiDescription },
            { "scroll_to_output", ScrollToOutput },
            { "show_progress", ShowProgress },
            { "show_progress_on", showProgressOnIds },
            { "batch", Batch },
            { "max_batch_size", MaxBatchSize },
            { "cancels", Cancels },
            { "types", new Dictionary<string, bool> { { "generator", TypesGenerator }, { "cancel", IsCancelFunction } } },
            { "collects_event_data", CollectsEventData },
            { "trigger_after", TriggerAfter },
            { "trigger_only_on_success", TriggerOnlyOnSuccess },
            { "trigger_only_on_failure", TriggerOnlyOnFailure },
            { "trigger_mode", TriggerMode },
            { "api_visibility", ApiVisibility },
            { "rendered_in", RenderedIn?.Id },
            { "render_id", Renderable?.Id },
            { "connection", Connection },
            { "time_limit", TimeLimit },
            { "stream_every", StreamEvery },
            { "event_specific_args", eventSpecificArgsValue },
            { "component_prop_inputs", ComponentPropInputs },
            { "js_implementation", jsImpl }
        };

        return config;
    }

    public BlockFunction Then(Delegate fn, params Block[] inputs)
    {
        // This is a simplified implementation
        // In a full implementation, this would create a new BlockFunction
        // that runs after this one completes
        return this;
    }

    public void SpacesAutoWrap()
    {
        // Implement spaces auto wrap if running in a Hugging Face Space
        var spaceId = Environment.GetEnvironmentVariable("SPACE_ID");

        if (!string.IsNullOrEmpty(spaceId) && Fn != null)
        {
            // Wrap the function to handle Spaces-specific features
            var originalFn = Fn;
            Fn = new Func<object[], object>((args) =>
            {
                try
                {
                    // Add any Spaces-specific preprocessing here
                    // For now, just call the original function
                    return originalFn.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }
    }

    public string Repr()
    {
        return ToString();
    }
}

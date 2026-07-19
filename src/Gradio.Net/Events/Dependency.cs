namespace Gradio.Net.Events;

using Gradio.Net.Core;

public class Dependency : Dictionary<string, object?>
{
    private readonly object? _trigger;

    public Delegate? Fn { get; }

    public object? AssociatedTimer { get; }

    public int? DepIndex { get; }

    public Dependency(
        object? trigger,
        Dictionary<string, object?>? keyVals,
        int? depIndex,
        Delegate? fn,
        object? associatedTimer = null)
    {
        _trigger = trigger;

        if (keyVals != null)
        {
            foreach (var kv in keyVals)
            {
                this[kv.Key] = kv.Value;
            }
        }

        Fn = fn;
        AssociatedTimer = associatedTimer;
        DepIndex = depIndex;
    }

    public Dependency Then(
        Delegate? fn = null,
        object? inputs = null,
        object? outputs = null,
        string? apiName = null,
        bool scrollToOutput = false,
        string showProgress = "full",
        bool queue = true,
        string apiVisibility = "public",
        bool preprocess = true,
        bool postprocess = true)
    {
        return CreateChainedDependency(
            fn, inputs, outputs, apiName, scrollToOutput, showProgress,
            queue, apiVisibility, preprocess, postprocess,
            triggerOnlyOnSuccess: false,
            triggerOnlyOnFailure: false);
    }

    public Dependency Success(
        Delegate? fn = null,
        object? inputs = null,
        object? outputs = null,
        string? apiName = null,
        bool scrollToOutput = false,
        string showProgress = "full",
        bool queue = true,
        string apiVisibility = "undocumented",
        bool preprocess = true,
        bool postprocess = true)
    {
        return CreateChainedDependency(
            fn, inputs, outputs, apiName, scrollToOutput, showProgress,
            queue, apiVisibility, preprocess, postprocess,
            triggerOnlyOnSuccess: true,
            triggerOnlyOnFailure: false);
    }

    public Dependency Failure(
        Delegate? fn = null,
        object? inputs = null,
        object? outputs = null,
        string? apiName = null,
        bool scrollToOutput = false,
        string showProgress = "full",
        bool queue = true,
        string apiVisibility = "undocumented",
        bool preprocess = true,
        bool postprocess = true)
    {
        return CreateChainedDependency(
            fn, inputs, outputs, apiName, scrollToOutput, showProgress,
            queue, apiVisibility, preprocess, postprocess,
            triggerOnlyOnSuccess: false,
            triggerOnlyOnFailure: true);
    }

    public object? Invoke(params object?[] args)
    {
        return Fn?.DynamicInvoke(args);
    }

    private Dependency CreateChainedDependency(
        Delegate? fn,
        object? inputs,
        object? outputs,
        string? apiName,
        bool scrollToOutput,
        string showProgress,
        bool queue,
        string apiVisibility,
        bool preprocess,
        bool postprocess,
        bool triggerOnlyOnSuccess,
        bool triggerOnlyOnFailure)
    {
        // Get the BlocksConfig context to call SetEventTrigger
        var blocksConfig = Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            throw new InvalidOperationException("Cannot chain events outside of a Blocks context");
        }

        if (!DepIndex.HasValue)
        {
            throw new InvalidOperationException("Cannot chain events: missing dependency index.");
        }

        // Python parity: chained dependencies are linked by trigger_after and use a
        // synthetic [null, "then"] target in config.
        var result = blocksConfig.SetEventTrigger(
            targets: new List<EventListenerMethod> { new EventListenerMethod(null, "then") },
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            apiName: apiName,
            scrollToOutput: scrollToOutput,
            showProgress: showProgress,
            queue: queue,
            apiVisibility: apiVisibility,
            preprocess: preprocess,
            postprocess: postprocess,
            triggerAfter: DepIndex,
            triggerOnlyOnSuccess: triggerOnlyOnSuccess,
            triggerOnlyOnFailure: triggerOnlyOnFailure,
            noTarget: false
        );

        var blockFn = result.Item1;
        var newDepIndex = result.Item2;

        // Create and return new Dependency for further chaining
        var config = blockFn?.GetConfig() ?? new Dictionary<string, object?>();
        return new Dependency(_trigger, config, newDepIndex, fn, AssociatedTimer);
    }
}

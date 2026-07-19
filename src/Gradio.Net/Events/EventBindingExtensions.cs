
namespace Gradio.Net.Events;

public static class EventBindingExtensions
{
    private static EventListener Bind(EventListener evt, object component)
    {
        var copy = evt.Copy();
        copy.Self = component;
        copy.HasTrigger = true;
        return copy;
    }

    public static Dependency On(
        this object component,
        EventListener trigger,
        Delegate? fn = null,
        object? inputs = null,
        object? outputs = null,
        string apiVisibility = "public",
        string? apiName = null,
        object? apiDescription = null,
        bool scrollToOutput = false,
        string showProgress = "full",
        object? showProgressOn = null,
        bool queue = true,
        bool batch = false,
        int maxBatchSize = 4,
        bool preprocess = true,
        bool postprocess = true,
        List<int>? cancels = null,
        string? triggerMode = null,
        string? js = null,
        object? concurrencyLimit = null,
        string? concurrencyId = null,
        int? timeLimit = null,
        float streamEvery = 0.5f,
        object? key = null,
        Delegate? validator = null)
    {
        if (component == null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        if (trigger == null)
        {
            throw new ArgumentNullException(nameof(trigger));
        }

        return Events.On(
            triggers: new List<EventListener> { Bind(trigger, component) },
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            apiVisibility: apiVisibility,
            apiName: apiName,
            apiDescription: apiDescription,
            scrollToOutput: scrollToOutput,
            showProgress: showProgress,
            showProgressOn: showProgressOn,
            queue: queue,
            batch: batch,
            maxBatchSize: maxBatchSize,
            preprocess: preprocess,
            postprocess: postprocess,
            cancels: cancels,
            triggerMode: triggerMode,
            js: js,
            concurrencyLimit: concurrencyLimit,
            concurrencyId: concurrencyId,
            timeLimit: timeLimit,
            streamEvery: streamEvery,
            key: key,
            validator: validator
        );
    }
}

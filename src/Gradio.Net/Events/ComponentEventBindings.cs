using Gradio.Net.Components;

namespace Gradio.Net.Events;

public static class ComponentEventBindings
{
    private static EventListener Bind(EventListener evt, object component)
    {
        var copy = evt.Copy();
        copy.Self = component;
        copy.HasTrigger = true;
        return copy;
    }

    private static Dependency InvokeOn(
        object component,
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

    // Audio
    public static Dependency Stream(this Audio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "minimal", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Stream, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Change(this Audio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency StopRecording(this Audio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.StopRecording, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency StartRecording(this Audio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.StartRecording, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Stop(this Audio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Stop, fn, inputs, outputs, apiName: apiName, queue: queue);

    // BrowserState
    public static Dependency Change(this BrowserState component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Button
    public static Dependency Click(this Button component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Hover(this Button component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Button component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this Button component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Chatbot
    public static Dependency Like(this Chatbot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Like, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Undo(this Chatbot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Undo, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Retry(this Chatbot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Retry, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Edit(this Chatbot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Edit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Clear(this Chatbot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);


    // Checkbox
    public static Dependency Change(this Checkbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Checkbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Checkbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // CheckboxGroup
    public static Dependency Change(this CheckboxGroup component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this CheckboxGroup component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this CheckboxGroup component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Code
    public static Dependency Change(this Code component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Code component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this Code component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Code component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    // ColorPicker
    public static Dependency Change(this ColorPicker component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this ColorPicker component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this ColorPicker component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this ColorPicker component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this ColorPicker component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Dataframe
    public static Dependency Change(this Dataframe component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Dataframe component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Dataframe component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Edit(this Dataframe component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Edit, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Dataset
    public static Dependency Click(this Dataset component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Select(this Dataset component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // DateTimeComponent
    public static Dependency Change(this DateTimeComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this DateTimeComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Dialogue
    public static Dependency Change(this Dialogue component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Dialogue component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this Dialogue component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    // DownloadButton
    public static Dependency Click(this DownloadButton component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Dropdown
    public static Dependency Change(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Focus(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency KeyUp(this Dropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.KeyUp, fn, inputs, outputs, apiName: apiName, queue: queue);

    // FileComponent
    public static Dependency Change(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Clear(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Delete(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Delete, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Download(this FileComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Download, fn, inputs, outputs, apiName: apiName, queue: queue);

    // FileExplorer
    public static Dependency Change(this FileExplorer component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Gallery
    public static Dependency Select(this Gallery component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Upload(this Gallery component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Change(this Gallery component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Delete(this Gallery component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Delete, fn, inputs, outputs, apiName: apiName, queue: queue);

    // HighlightedText
    public static Dependency Change(this HighlightedText component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this HighlightedText component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Image
    public static Dependency Stream(this Image component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "minimal", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.1f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Stream, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Clear(this Image component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Change(this Image component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this Image component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Image component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Html
    public static Dependency Click(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Change(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Upload(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Delete(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Delete, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Clear(this Html component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    // ImageEditor
    public static Dependency Clear(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Change(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Upload(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Apply(this ImageEditor component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Apply, fn, inputs, outputs, apiName: apiName, queue: queue);

    // ImageSlider
    public static Dependency Clear(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Change(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Stream(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "minimal", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Stream, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Select(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Upload(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this ImageSlider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    // JsonComponent
    public static Dependency Change(this JsonComponent component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Label
    public static Dependency Change(this Label component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Label component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Markdown
    public static Dependency Change(this Markdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Copy(this Markdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Copy, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Model3D
    public static Dependency Change(this Model3D component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this Model3D component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Edit(this Model3D component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Edit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Clear(this Model3D component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    // MultimodalTextbox
    public static Dependency Change(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Submit(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Stop(this MultimodalTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Stop, fn, inputs, outputs, apiName: apiName, queue: queue);

    // NativePlot
    public static Dependency Select(this NativePlot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency DoubleClick(this NativePlot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.DoubleClick, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Number
    public static Dependency Change(this Number component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Number component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this Number component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this Number component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Number component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    // ParamViewer
    public static Dependency Change(this ParamViewer component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this ParamViewer component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Plot
    public static Dependency Change(this Plot component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Radio
    public static Dependency Select(this Radio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Change(this Radio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Input(this Radio component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    // SimpleDropdown
    public static Dependency Change(this SimpleDropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this SimpleDropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this SimpleDropdown component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // SimpleImage
    public static Dependency Clear(this SimpleImage component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Clear, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Change(this SimpleImage component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this SimpleImage component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);

    // SimpleTextbox
    public static Dependency Change(this SimpleTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this SimpleTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Submit(this SimpleTextbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Slider
    public static Dependency Change(this Slider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Input(this Slider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Release(this Slider component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Release, fn, inputs, outputs, apiName: apiName, queue: queue);

    // State
    public static Dependency Change(this State component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Textbox
    public static Dependency Change(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Input(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Input, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Select(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Select, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Submit(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Submit, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Focus(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Focus, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Blur(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Blur, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Stop(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Stop, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Copy(this Textbox component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Copy, fn, inputs, outputs, apiName: apiName, queue: queue);

    // Timer
    public static Dependency Tick(this Components.Timer component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "hidden", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Tick, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // Video
    public static Dependency Change(this Video component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Change, fn, inputs, outputs, apiName: apiName, queue: queue);

    public static Dependency Upload(this Video component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency StopRecording(this Video component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.StopRecording, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    // UploadButton
    public static Dependency Click(this UploadButton component, Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
        => InvokeOn(component, Events.Click, fn, inputs, outputs, apiVisibility, apiName, apiDescription, scrollToOutput, showProgress, showProgressOn, queue, batch, maxBatchSize, preprocess, postprocess, cancels, triggerMode, js, concurrencyLimit, concurrencyId, timeLimit, streamEvery, key, validator);

    public static Dependency Upload(this UploadButton component, Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true)
        => InvokeOn(component, Events.Upload, fn, inputs, outputs, apiName: apiName, queue: queue);
}

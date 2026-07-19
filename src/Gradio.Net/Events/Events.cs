using System.Text.Json;
using Gradio.Net.Components;

namespace Gradio.Net.Events;

public static class Events
{
    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var chars = new List<char>(value.Length + 4);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    chars.Add('_');
                }
                chars.Add(char.ToLowerInvariant(c));
            }
            else
            {
                chars.Add(c);
            }
        }

        return new string(chars.ToArray());
    }

    private static string ResolveEventNameFromMethod(string methodName)
    {
        if (methodName.EndsWith("Async", StringComparison.Ordinal))
        {
            methodName = methodName[..^5];
        }

        return ToSnakeCase(methodName);
    }

    private static List<EventListener> ToEventListeners(List<TriggerBinding> triggers)
    {
        var listeners = new List<EventListener>(triggers.Count);

        foreach (var trigger in triggers)
        {
            if (trigger.Target == null)
            {
                throw new ArgumentException("Each trigger must be a bound component event method, e.g. `name.Submit`.");
            }

            var methodName = trigger.Method.Name;
            var eventName = ResolveEventNameFromMethod(methodName);
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("Unable to resolve event name from trigger method.");
            }

            listeners.Add(new EventListener(eventName, hasTrigger: true)
            {
                Self = trigger.Target
            });
        }

        return listeners;
    }

    private static void SetCancelEvents(List<EventListenerMethod> triggers, List<int>? cancels)
    {
        if (cancels == null || cancels.Count == 0)
        {
            return;
        }

        var rootBlock = Core.Context.GetBlocksContext();
        if (rootBlock == null)
        {
            throw new InvalidOperationException("Cannot cancel outside of a gradio.Blocks context.");
        }

        // Matches Python behavior for regular cancels:
        // root_block.set_event_trigger(triggers, fn=None, inputs=None, outputs=None,
        //     queue=False, preprocess=False, api_visibility="private", cancels=..., is_cancel_function=True)
        rootBlock.SetEventTrigger(
            targets: triggers,
            fn: null,
            inputs: null,
            outputs: null,
            queue: false,
            preprocess: false,
            postprocess: false,
            apiVisibility: "private",
            cancels: cancels,
            isCancelFunction: true
        );
    }

    public static readonly EventListener Change = new(
        "change",
        doc: "Triggered when the value of the {{ component }} changes either because of user input (e.g. a user types in a textbox) OR because of a function update (e.g. an image receives a value from the output of an event trigger). See `.input()` for a listener that is only triggered by user input."
    );

    public static readonly EventListener Input = new(
        "input",
        doc: "This listener is triggered when the user changes the value of the {{ component }}."
    );

    public static readonly EventListener Click = new(
        "click",
        doc: "Triggered when the {{ component }} is clicked."
    );

    public static readonly EventListener DoubleClick = new(
        "double_click",
        doc: "Triggered when the {{ component }} is double clicked."
    );

    public static readonly EventListener Submit = new(
        "submit",
        doc: "This listener is triggered when the user presses the Enter key while the {{ component }} is focused."
    );

    public static readonly EventListener Stop = new(
        "stop",
        doc: "This listener is triggered when the user clicks on the stop button or icon."
    );

    public static readonly EventListener Edit = new(
        "edit",
        doc: "This listener is triggered when the user edits the {{ component }} (e.g. image) using the built-in editor."
    );

    public static readonly EventListener Clear = new(
        "clear",
        doc: "This listener is triggered when the user clears the {{ component }} using the clear button for the component."
    );

    public static readonly EventListener Play = new(
        "play",
        doc: "This listener is triggered when the user plays the media in the {{ component }}."
    );

    public static readonly EventListener Pause = new(
        "pause",
        doc: "This listener is triggered when the media in the {{ component }} stops for any reason."
    );

    public static readonly EventListener End = new(
        "end",
        doc: "This listener is triggered when the user reaches the end of the media playing in the {{ component }}."
    );

    public static readonly EventListener StartRecording = new(
        "start_recording",
        doc: "This listener is triggered when the user starts recording with the {{ component }}."
    );

    public static readonly EventListener PauseRecording = new(
        "pause_recording",
        doc: "This listener is triggered when the user pauses recording with the {{ component }}."
    );

    public static readonly EventListener StopRecording = new(
        "stop_recording",
        doc: "This listener is triggered when the user stops recording with the {{ component }}."
    );

    public static readonly EventListener Focus = new(
        "focus",
        doc: "This listener is triggered when the {{ component }} is focused."
    );

    public static readonly EventListener Blur = new(
        "blur",
        doc: "This listener is triggered when the {{ component }} is unfocused/blurred."
    );

    public static readonly EventListener Upload = new(
        "upload",
        doc: "This listener is triggered when the user uploads a file into the {{ component }}."
    );

    public static readonly EventListener Release = new(
        "release",
        doc: "This listener is triggered when the user releases the mouse on this {{ component }}."
    );

    public static readonly EventListener Select = new(
        "select",
        callback: block =>
        {
            if (block is Components.Component comp)
            {
                comp.Selectable = true;
            }
        },
        doc: "Event listener for when the user selects or deselects the {{ component }}. Uses event data gradio.SelectData to carry `value` referring to the label of the {{ component }}, and `selected` to refer to state of the {{ component }}. See EventData documentation on how to use this event data"
    );

    public static readonly EventListener Stream = new(
        "stream",
        callback: block =>
        {
            if (block == null)
            {
                return;
            }

            var streamingProperty = block.GetType().GetProperty("Streaming");
            if (streamingProperty != null && streamingProperty.CanWrite)
            {
                if (streamingProperty.PropertyType == typeof(bool))
                {
                    streamingProperty.SetValue(block, true);
                }
                else if (streamingProperty.PropertyType == typeof(object))
                {
                    streamingProperty.SetValue(block, true);
                }
            }
        },
        doc: "This listener is triggered when the user streams the {{ component }}.",
        connection: "stream",
        showProgress: "minimal",
        eventSpecificArgs: new List<EventArg>
        {
            new("stream_every", "float", "The latency (in seconds) at which stream chunks are sent to the backend. Defaults to 0.5 seconds. Parameter only used for the `.stream()` event."),
            new("time_limit", "float | None", "The time limit for the function to run. Parameter only used for the `.stream()` event.", "false")
        }
    );

    public static readonly EventListener Like = new(
        "like",
        callback: block =>
        {
            // Python parity: EventListener("like", callback=lambda block: setattr(block, "likeable", True))
            var prop = block?.GetType().GetProperty("likeable",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(bool))
            {
                prop.SetValue(block, true);
            }
        },
        doc: "This listener is triggered when the user likes/dislikes from within the {{ component }}. This event has EventData of type gradio.LikeData that carries information, accessible through LikeData.index and LikeData.value. See EventData documentation on how to use this event data."
    );

    public static readonly EventListener ExampleSelect = new(
        "example_select",
        doc: "This listener is triggered when the user clicks on an example from within the {{ component }}. This event has SelectData of type gradio.SelectData that carries information, accessible through SelectData.index and SelectData.value. See SelectData documentation on how to use this event data."
    );

    public static readonly EventListener OptionSelect = new(
        "option_select",
        doc: "This listener is triggered when the user clicks on an option from within the {{ component }}. This event has SelectData of type gradio.SelectData that carries information, accessible through SelectData.index and SelectData.value. See SelectData documentation on how to use this event data."
    );

    public static readonly EventListener Load = new(
        "load",
        doc: "This listener is triggered when the {{ component }} initially loads in the browser."
    );

    public static readonly EventListener KeyUp = new(
        "key_up",
        doc: "This listener is triggered when the user presses a key while the {{ component }} is focused."
    );

    public static readonly EventListener Apply = new(
        "apply",
        doc: "This listener is triggered when the user applies changes to the {{ component }} through an integrated UI action."
    );

    public static readonly EventListener Delete = new(
        "delete",
        doc: "This listener is triggered when the user deletes and item from the {{ component }}. Uses event data gradio.DeletedFileData to carry `value` referring to the file that was deleted as an instance of FileData. See EventData documentation on how to use this event data"
    );

    public static readonly EventListener Tick = new(
        "tick",
        doc: "This listener is triggered at regular intervals defined by the {{ component }}.",
        showProgress: "hidden"
    );

    public static readonly EventListener Undo = new(
        "undo",
        doc: "This listener is triggered when the user clicks the undo button in the chatbot message."
    );

    public static readonly EventListener Retry = new(
        "retry",
        doc: "This listener is triggered when the user clicks the retry button in the chatbot message."
    );

    public static readonly EventListener Expand = new(
        "expand",
        doc: "This listener is triggered when the {{ component }} is expanded."
    );

    public static readonly EventListener Collapse = new(
        "collapse",
        doc: "This listener is triggered when the {{ component }} is collapsed."
    );

    public static readonly EventListener Download = new(
        "download",
        doc: "This listener is triggered when the user downloads a file from the {{ component }}. Uses event data gradio.DownloadData to carry information about the downloaded file as a FileData object. See EventData documentation on how to use this event data"
    );

    public static readonly EventListener Copy = new(
        "copy",
        doc: "This listener is triggered when the user copies content from the {{ component }}. Uses event data gradio.CopyData to carry information about the copied content. See EventData documentation on how to use this event data"
    );

    public static Dependency On(
        List<TriggerBinding>? triggers = null,
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
        var listenerTriggers = triggers == null ? null : ToEventListeners(triggers);

        return On(
            triggers: listenerTriggers,
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            includeEmptyEventSpecificArgs: true,
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

    public static Dependency On(
        List<EventListener>? triggers = null,
        Delegate? fn = null,
        object? inputs = null,
        object? outputs = null,
        bool includeEmptyEventSpecificArgs = false,
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
        var rootBlock = Core.Context.GetBlocksContext();
        if (rootBlock == null)
        {
            throw new InvalidOperationException("Cannot call On() outside of a gradio.Blocks context.");
        }

        // Convert triggers to EventListenerMethod list
        List<EventListenerMethod> methods;

        if (triggers == null)
        {
            // If no triggers specified, run on app load and changes to any inputs
            var inputsList = new List<object>();
            if (inputs != null)
            {
                if (inputs is System.Collections.IEnumerable enumerable && !(inputs is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                            inputsList.Add(item);
                    }
                }
                else
                {
                    inputsList.Add(inputs);
                }
            }

            methods = new List<EventListenerMethod>();
            foreach (var input in inputsList)
            {
                methods.Add(new EventListenerMethod(input, "change"));
            }

            // Add load event
            methods.Add(new EventListenerMethod(rootBlock.RootBlock, "load"));
        }
        else
        {
            // Convert EventListener triggers to EventListenerMethod
            methods = triggers.Select(t =>
            {
                // Python parity: EventListenerMethod(t.__self__ if t.has_trigger else None, t.event_name)
                var block = t.HasTrigger ? t.Self : null;
                return new EventListenerMethod(block, t.EventName);
            }).ToList();
        }

        // Python parity: if triggers have callbacks, invoke them with the bound component
        if (triggers != null)
        {
            foreach (var trigger in triggers)
            {
                trigger.Callback?.Invoke(trigger.Self ?? new object());
            }
        }

        // Determine connection type
        string connection = "sse";
        if (triggers != null && triggers.Any(t => t.Connection == "stream"))
        {
            connection = "stream";
        }

        // Collect event specific args
        List<string>? eventSpecificArgs = null;
        if (triggers != null)
        {
            var args = new List<string>();
            foreach (var t in triggers)
            {
                if (t.EventSpecificArgs != null)
                {
                    args.AddRange(t.EventSpecificArgs.Select(arg => arg.Name));
                }
            }
            if (args.Count > 0 || includeEmptyEventSpecificArgs)
            {
                eventSpecificArgs = args;
            }
        }

        // Call SetEventTrigger
        var (dep, depIndex) = rootBlock.SetEventTrigger(
            targets: methods,
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            preprocess: preprocess,
            postprocess: postprocess,
            scrollToOutput: scrollToOutput,
            showProgress: showProgress,
            showProgressOn: showProgressOn,
            apiName: apiName,
            apiDescription: apiDescription,
            js: js,
            concurrencyLimit: concurrencyLimit ?? "default",
            concurrencyId: concurrencyId,
            queue: queue,
            batch: batch,
            maxBatchSize: maxBatchSize,
            apiVisibility: apiVisibility,
            triggerMode: triggerMode,
            connection: connection,
            eventSpecificArgs: eventSpecificArgs,
            timeLimit: timeLimit,
            streamEvery: streamEvery,
            key: key,
            validator: validator
        );

        // Handle cancels
        SetCancelEvents(methods, cancels);

        // Return Dependency for chaining
        return new Dependency(null, dep.GetConfig(), depIndex, fn);
    }

    public static Dependency Api(
        Delegate? fn = null,
        string? apiName = null,
        string? apiDescription = null,
        bool queue = true,
        bool batch = false,
        int maxBatchSize = 4,
        object? concurrencyLimit = null,
        string? concurrencyId = null,
        string apiVisibility = "public",
        int? timeLimit = null,
        float streamEvery = 0.5f)
    {
        if (fn == null)
        {
            throw new ArgumentNullException(nameof(fn));
        }

        var rootBlock = Core.Context.GetBlocksContext();
        if (rootBlock == null)
        {
            throw new InvalidOperationException("Cannot call Api() outside of a gradio.Blocks context.");
        }

        static string Ordinal(int n)
        {
            // Matches the Python helper used in events.py
            var tens = n % 100;
            if (tens is >= 10 and <= 20) return $"{n}th";
            return (n % 10) switch
            {
                1 => $"{n}st",
                2 => $"{n}nd",
                3 => $"{n}rd",
                _ => $"{n}th"
            };
        }

        static Dictionary<string, object> TypeToJsonSchema(Type type)
        {
            // Minimal schema (good enough for docs + client hints).
            // For rich schemas, consider integrating a full JSON schema generator.
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)) return new Dictionary<string, object> { { "type", "string" } };
            if (type == typeof(bool)) return new Dictionary<string, object> { { "type", "boolean" } };
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                return new Dictionary<string, object> { { "type", "integer" } };
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return new Dictionary<string, object> { { "type", "number" } };

            if (type.IsArray)
            {
                return new Dictionary<string, object>
                {
                    { "type", "array" },
                    { "items", TypeToJsonSchema(type.GetElementType() ?? typeof(object)) }
                };
            }

            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                if (def == typeof(List<>) || def == typeof(IList<>) || def == typeof(IEnumerable<>))
                {
                    return new Dictionary<string, object>
                    {
                        { "type", "array" },
                        { "items", TypeToJsonSchema(type.GetGenericArguments()[0]) }
                    };
                }

                if (def == typeof(Dictionary<,>))
                {
                    return new Dictionary<string, object>
                    {
                        { "type", "object" }
                    };
                }
            }

            if (type.IsEnum)
            {
                return new Dictionary<string, object>
                {
                    { "type", "string" },
                    { "enum", Enum.GetNames(type) }
                };
            }

            // Fallback
            return new Dictionary<string, object> { { "type", "object" } };
        }

        var method = fn.Method;
        var parameters = method.GetParameters()
            .Where(p => !Gradio.Net.Utils.Utils.IsSpecialTypedParameter(p))
            .OrderBy(p => p.Position)
            .ToList();

        var inputs = new List<object>();
        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];
            var hasDefault = p.HasDefaultValue;
            var defaultValue = hasDefault ? p.DefaultValue : null;
            var schema = TypeToJsonSchema(p.ParameterType);
            var apiInfo = new Dictionary<string, string>
            {
                { "schema", JsonSerializer.Serialize(schema) },
                { "dotnet_type", p.ParameterType.FullName ?? p.ParameterType.Name }
            };
            inputs.Add(new ApiComponent(defaultValue, apiInfo, Ordinal(i + 1)));
        }

        var returnTypes = Gradio.Net.Utils.Utils.GetReturnTypes(method);
        var outputs = new List<object>();
        for (int i = 0; i < returnTypes.Count; i++)
        {
            var t = returnTypes[i];
            var schema = TypeToJsonSchema(t);
            var apiInfo = new Dictionary<string, string>
            {
                { "schema", JsonSerializer.Serialize(schema) },
                { "dotnet_type", t.FullName ?? t.Name }
            };
            outputs.Add(new ApiComponent(null, apiInfo, Ordinal(i + 1)));
        }

        var (dep, depIndex) = rootBlock.SetEventTrigger(
            targets: new List<EventListenerMethod>(),
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            preprocess: false,
            postprocess: false,
            scrollToOutput: false,
            showProgress: "hidden",
            apiName: apiName,
            apiDescription: apiDescription,
            js: null,
            concurrencyLimit: concurrencyLimit ?? "default",
            concurrencyId: concurrencyId,
            queue: queue,
            batch: batch,
            maxBatchSize: maxBatchSize,
            apiVisibility: apiVisibility,
            triggerMode: null,
            timeLimit: timeLimit,
            streamEvery: streamEvery
        );

        return new Dependency(null, dep.GetConfig(), depIndex, fn);
    }
}

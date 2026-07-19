using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Gradio.Net.Events;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

public abstract class Component : Core.Block
{
    public static readonly string TEMPLATE_DIR = "./templates/";

    public static readonly string FRONTEND_DIR = "../../frontend/";

    public static List<object> EVENTS = new List<object> {
        "click",
        "change",
        "input",
        "focus",
        "blur",
        "keydown",
        "keyup"
    };

    protected Component()
    {
        // Initialize component using ComponentMetaFactory
        Core.ComponentMetaFactory.InitializeComponent(GetType());

        LoadEventToAttach = null;
        LoadEvent = null;
        ApiMode = null;
        EventListeners = new Dictionary<string, List<EventListener>>();
        Label = null;
        Info = null;
        ShowLabel = true;
        Container = true;
        Scale = null;
        MinWidth = 160;
        Interactive = null;
        PreservedByKey = new List<string> { "value" };
        ComponentClassId = GetComponentClassId();
    }

    protected Component(bool _proxyMode) : base(_proxyMode) { }

    protected virtual string GetComponentClassId()
    {
        var type = GetType();
        var assemblyPath = type.Assembly.Location;
        var payload = $"{type.Name}_{assemblyPath}";
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public (Delegate fn, List<(Core.Block, string)> triggers, List<Core.Block> inputs)? LoadEventToAttach { get; set; }
    public Dictionary<string, object>? LoadEvent { get; set; }
    public string? ApiMode { get; set; }
    public new Dictionary<string, List<EventListener>> EventListeners { get; set; }
    public string? Label { get; set; }
    public string? Info { get; set; }
    public bool ShowLabel { get; set; }
    public bool Container { get; set; }
    public int? Scale { get; set; }
    public int? MinWidth { get; set; }
    public bool? Interactive { get; set; }
    public string ComponentClassId { get; set; }
    public bool? Selectable { get; set; }
    public object? Value { get; set; }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["info"] = Info;
        config["show_label"] = ShowLabel;
        config["container"] = Container;
        config["scale"] = Scale;
        config["min_width"] = MinWidth;
        config["interactive"] = Interactive;
        config["_selectable"] = Selectable ?? false;
        return config;
    }

    public virtual Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            { "type", GetType().Name.ToLower() }
        };
    }

    public virtual Dictionary<string, object> ApiInfoAsInput()
    {
        return ApiInfo();
    }

    public virtual Dictionary<string, object> ApiInfoAsOutput()
    {
        return ApiInfo();
    }

    public virtual List<object> ExampleInputs()
    {
        return new List<object>();
    }

    public virtual object Preprocess(object input)
    {
        return input;
    }

    public virtual object Postprocess(object output)
    {
        return output;
    }

    public virtual object ProcessExample(object value)
    {
        return Postprocess(value);
    }

    public virtual object ExamplePayload()
    {
        throw new NotImplementedException($"ExamplePayload has not been implemented for {GetType().Name}");
    }

    public virtual object ExampleValue()
    {
        throw new NotImplementedException($"ExampleValue has not been implemented for {GetType().Name}");
    }

    public override bool SkipApi => false;

    public static (Delegate? loadFn, object? initialValue) GetLoadFnAndInitialValue(object? value, Core.Block[]? inputs = null)
    {
        object? initialValue = null;
        Delegate? loadFn = null;

        if (value is Delegate del)
        {
            // If value is a callable (Delegate)
            if (inputs == null || inputs.Length == 0)
            {
                // Try to invoke it to get initial value
                try
                {
                    initialValue = del.DynamicInvoke();
                }
                catch
                {
                    initialValue = null;
                }
            }
            loadFn = del;
        }
        else
        {
            // Value is not callable, use it directly
            initialValue = value;
            loadFn = null;
        }

        return (loadFn, initialValue);
    }

    public virtual void AttachLoadEvent(Delegate loadFn, object? every, params Core.Block[] inputs)
    {
        var changeableEvents = new List<(Core.Block, string)>();

        if (inputs != null && inputs.Length > 0)
        {
            foreach (var input in inputs)
            {
                if (input is Component inputComp)
                {
                    var inputEvents = GetComponentEvents(inputComp);
                    if (inputEvents.Contains("change"))
                    {
                        changeableEvents.Add((input, "change"));
                    }
                }
            }
        }

        if (every is int || every is float || every is double)
        {
            every = new Timer(Convert.ToDouble(every));
        }

        if (every is Timer timerComp)
        {
            changeableEvents.Add((timerComp, "tick"));
        }
        else if (every != null)
        {
        }

        // Store the load event information for later attachment
        LoadEventToAttach = (loadFn, changeableEvents, inputs?.ToList() ?? new List<Core.Block>());
    }

    private static IList<object> GetComponentEvents(Component comp)
    {
        // Get EVENTS from the actual (most-derived) type, falling back to base
        var type = comp.GetType();
        while (type != null && type != typeof(object))
        {
            var field = type.GetField("EVENTS",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.DeclaredOnly);
            if (field != null)
            {
                return field.GetValue(null) as IList<object> ?? new List<object>();
            }
            type = type.BaseType;
        }
        return new List<object>();
    }

    public virtual string Flag(object payload, string flagDir = "")
    {
        // Write the component's value to a format that can be stored in csv or jsonl
        // Note: Full data_model support would require additional implementation

        if (payload is string payloadString)
        {
            var trimmed = payloadString.TrimStart();
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                try
                {
                    using var doc = JsonDocument.Parse(payloadString);
                    payload = ConvertJsonElement(doc.RootElement) ?? payloadString;
                }
                catch
                {
                    // Keep original string payload if it is not valid JSON.
                    payload = payloadString;
                }
            }
        }

        payload = NormalizePayload(payload);

        if (!string.IsNullOrEmpty(flagDir))
        {
            System.IO.Directory.CreateDirectory(flagDir);

            payload = GradioModel.Traverse(
                payload,
                obj =>
                {
                    if (obj is not Dictionary<string, object> fileDict)
                    {
                        return obj;
                    }

                    var sourcePath = GetStringFromDict(fileDict, "path");
                    if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    {
                        return obj;
                    }

                    var uniqueDir = Path.Combine(flagDir, Guid.NewGuid().ToString("N")[..20]);
                    var copied = new FileData
                    {
                        Path = sourcePath,
                        Url = GetStringFromDict(fileDict, "url"),
                        Size = GetIntFromDict(fileDict, "size"),
                        OrigName = GetStringFromDict(fileDict, "orig_name"),
                        MimeType = GetStringFromDict(fileDict, "mime_type"),
                        IsStream = GetBoolFromDict(fileDict, "is_stream") ?? false,
                        Meta = new FileDataMeta()
                    }.CopyToDir(uniqueDir);

                    return copied.ToDictionary();
                },
                FileData.IsFileData
            );
        }

        if (payload is string str)
        {
            return str;
        }

        // Serialize to JSON
        return System.Text.Json.JsonSerializer.Serialize(payload);
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertJsonElement(prop.Value) ?? string.Empty;
                    }
                    return dict;
                }
            case JsonValueKind.Array:
                {
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElement(item) ?? string.Empty);
                    }
                    return list;
                }
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var i64)) return i64;
                if (element.TryGetDouble(out var d)) return d;
                return element.ToString();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return element.ToString();
        }
    }

    private static object? NormalizePayload(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is JsonElement je)
        {
            return ConvertJsonElement(je);
        }

        if (value is IDictionary dict)
        {
            var normalized = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in dict)
            {
                var key = entry.Key?.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                normalized[key] = NormalizePayload(entry.Value) ?? string.Empty;
            }
            return normalized;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                list.Add(NormalizePayload(item) ?? string.Empty);
            }
            return list;
        }

        return value;
    }

    private static string? GetStringFromDict(Dictionary<string, object> dict, string key)
    {
        if (!dict.TryGetValue(key, out var value) || value == null)
        {
            return null;
        }

        if (value is JsonElement je)
        {
            return je.ValueKind == JsonValueKind.String ? je.GetString() : je.ToString();
        }

        return value.ToString();
    }

    private static int? GetIntFromDict(Dictionary<string, object> dict, string key)
    {
        if (!dict.TryGetValue(key, out var value) || value == null)
        {
            return null;
        }

        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var i))
            {
                return i;
            }
            if (je.ValueKind == JsonValueKind.String && int.TryParse(je.GetString(), out var istr))
            {
                return istr;
            }
            return null;
        }

        if (value is int i2)
        {
            return i2;
        }

        return int.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    private static bool? GetBoolFromDict(Dictionary<string, object> dict, string key)
    {
        if (!dict.TryGetValue(key, out var value) || value == null)
        {
            return null;
        }

        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.True || je.ValueKind == JsonValueKind.False)
            {
                return je.GetBoolean();
            }
            if (je.ValueKind == JsonValueKind.String && bool.TryParse(je.GetString(), out var bstr))
            {
                return bstr;
            }
            return null;
        }

        if (value is bool b)
        {
            return b;
        }

        return bool.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    public virtual object ReadFromFlag(object payload)
    {
        // Convert data from csv or jsonl file into component state
        // Note: Full data_model support would require additional implementation

        if (payload is string str)
        {
            try
            {
                // Try to deserialize from JSON
                return System.Text.Json.JsonSerializer.Deserialize<object>(str) ?? payload;
            }
            catch
            {
                // If deserialization fails, return as is
                return payload;
            }
        }

        return payload;
    }
    public virtual object AsExample(object example)
    {
        // Process example for dataset display
        return example;
    }

    // Event handling methods
    public Core.BlockFunction On(string eventName, Delegate handler, params Core.Block[] inputs)
    {
        // Backward-compatible API:
        // - 0/1 arg: args are inputs, output defaults to this component.
        // - 2+ args: last arg is treated as output, preceding args as inputs.
        var effectiveInputs = new List<Core.Block>();
        object effectiveOutputs = this;
        if (inputs != null && inputs.Length > 0)
        {
            if (inputs.Length >= 2)
            {
                for (var i = 0; i < inputs.Length - 1; i++)
                {
                    effectiveInputs.Add(inputs[i]);
                }
                effectiveOutputs = inputs[inputs.Length - 1];
            }
            else
            {
                effectiveInputs.Add(inputs[0]);
            }
        }

        string? apiName = null;
        var methodName = handler.Method?.Name;
        if (!string.IsNullOrEmpty(methodName))
        {
            apiName = methodName;
            var marker = apiName.IndexOf("g__", StringComparison.Ordinal);
            if (marker >= 0)
            {
                var start = marker + 3;
                var end = apiName.IndexOf('|', start);
                if (end > start)
                {
                    apiName = apiName.Substring(start, end - start);
                }
                else
                {
                    apiName = apiName.Substring(start);
                }
            }
            apiName = apiName.ToLowerInvariant();
        }

        // Route through BlocksConfig.SetEventTrigger so dependency appears in /config.
        var blocksConfig = Core.Context.GetBlocksContext();
        if (blocksConfig != null)
        {
            var (blockFn, _) = blocksConfig.SetEventTrigger(
                targets: new List<EventListenerMethod> { new(this, eventName) },
                fn: handler,
                inputs: effectiveInputs,
                outputs: effectiveOutputs,
                preprocess: true,
                postprocess: true,
                triggerMode: null,
                apiName: apiName,
                apiVisibility: "public");
            return blockFn;
        }

        // Fallback when called outside Blocks context.
        var eventListener = new EventListener(eventName, handler);
        if (!EventListeners.TryGetValue(eventName, out var listeners))
        {
            listeners = new List<EventListener>();
            EventListeners[eventName] = listeners;
        }
        listeners.Add(eventListener);

        return new Core.BlockFunction(
            fn: handler,
            inputs: effectiveInputs,
            outputs: effectiveOutputs,
            preprocess: true,
            postprocess: true,
            id: Core.Context.NextId
        );
    }

    public Dependency Change(Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null)
    {
        return EventBindingExtensions.On(component: this, trigger: Gradio.Net.Events.Events.Change, fn: fn, inputs: inputs, outputs: outputs, apiName: apiName, queue: queue, js: js, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId);
    }

    public Dependency Click(Delegate? fn = null, object? inputs = null, object? outputs = null, string apiVisibility = "public", string? apiName = null, object? apiDescription = null, bool scrollToOutput = false, string showProgress = "full", object? showProgressOn = null, bool queue = true, bool batch = false, int maxBatchSize = 4, bool preprocess = true, bool postprocess = true, List<int>? cancels = null, string? triggerMode = null, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null, int? timeLimit = null, float streamEvery = 0.5f, object? key = null, Delegate? validator = null)
    {
        return EventBindingExtensions.On(component: this, trigger: Gradio.Net.Events.Events.Click, fn: fn, inputs: inputs, outputs: outputs, apiVisibility: apiVisibility, apiName: apiName, apiDescription: apiDescription, scrollToOutput: scrollToOutput, showProgress: showProgress, showProgressOn: showProgressOn, queue: queue, batch: batch, maxBatchSize: maxBatchSize, preprocess: preprocess, postprocess: postprocess, cancels: cancels, triggerMode: triggerMode, js: js, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId, timeLimit: timeLimit, streamEvery: streamEvery, key: key, validator: validator);
    }

    public Dependency Submit(Delegate? fn = null, object? inputs = null, object? outputs = null, string? apiName = null, bool queue = true, string? js = null, object? concurrencyLimit = null, string? concurrencyId = null)
    {
        return EventBindingExtensions.On(component: this, trigger: Gradio.Net.Events.Events.Submit, fn: fn, inputs: inputs, outputs: outputs, apiName: apiName, queue: queue, js: js, concurrencyLimit: concurrencyLimit, concurrencyId: concurrencyId);
    }

    public Core.BlockFunction Click(Delegate handler, params Core.Block[] inputs)
    {
        return On("click", handler, inputs);
    }

    public Core.BlockFunction Change(Delegate handler, params Core.Block[] inputs)
    {
        return On("change", handler, inputs);
    }

    public Core.BlockFunction Input(Delegate handler, params Core.Block[] inputs)
    {
        return On("input", handler, inputs);
    }

    public Core.BlockFunction Focus(Delegate handler, params Core.Block[] inputs)
    {
        return On("focus", handler, inputs);
    }

    public Core.BlockFunction Blur(Delegate handler, params Core.Block[] inputs)
    {
        return On("blur", handler, inputs);
    }

    public Core.BlockFunction KeyDown(Delegate handler, params Core.Block[] inputs)
    {
        return On("keydown", handler, inputs);
    }

    public Core.BlockFunction KeyUp(Delegate handler, params Core.Block[] inputs)
    {
        return On("keyup", handler, inputs);
    }

    // Method to trigger event
    public new void TriggerEvent(string eventName, params object[] args)
    {
        if (EventListeners.TryGetValue(eventName, out var listeners))
        {
            foreach (var listener in listeners)
            {
                listener.Invoke(args);
            }
        }
    }
}

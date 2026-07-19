using System.Collections;
using System.Text.Json;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Events;

namespace Gradio.Net.Components;

public class ClearButton : Button
{
    public bool IsTemplate { get; } = true;

    public string? ApiName { get; set; }

    public string ApiVisibility { get; set; } = "undocumented";

    public ClearButton(
        object? components = null,
        string value = "Clear",
        string variant = "secondary",
        string size = "lg",
        object? icon = null,
        string? link = null,
        bool visible = true,
        bool interactive = true,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        int? scale = null,
        int? minWidth = null,
        string? apiName = null,
        string apiVisibility = "undocumented"
    ) : base(
        value: value,
        variant: variant,
        size: size,
        icon: icon?.ToString(),
        link: link,
        visible: visible,
        interactive: interactive,
        elemId: elemId,
        elemClasses: elemClasses as List<string>,
        key: key,
        preservedByKey: preservedByKey as string,
        scale: scale,
        minWidth: minWidth
    )
    {
        ApiName = apiName;
        ApiVisibility = apiVisibility;

        if (elemClasses is string cls)
        {
            ElemClasses = new List<string> { cls };
        }

        if (preservedByKey is string pbk)
        {
            PreservedByKey = new List<string> { pbk };
        }
        else if (preservedByKey is List<string> pbkList)
        {
            PreservedByKey = pbkList;
        }

        if (Context.GetBlocksContext() != null && render)
        {
            Add(components);
        }
    }

    public override string GetBlockName() => "button";

    public override string GetBlockClass() => "button";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config.Remove("apiName");
        config.Remove("apiVisibility");
        config["api_name"] = ApiName;
        config["api_visibility"] = ApiVisibility;
        return config;
    }

    public ClearButton Add(object? components)
    {
        if (components == null)
        {
            return this;
        }

        var normalized = NormalizeComponents(components);
        if (normalized.Count == 0)
        {
            return this;
        }

        var blocksConfig = Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            return this;
        }

        var noneValues = new List<object?>();
        var stateComponents = new List<Component>();
        var initialStates = new List<object?>();

        foreach (var component in normalized)
        {
            if (IsStateLike(component))
            {
                stateComponents.Add(component);
                initialStates.Add(TryDeepCopy(GetComponentStateValue(component)));
            }

            var noneValue = PostprocessNone(component);
            noneValues.Add(ToSerializable(noneValue));
        }

        var clearValuesJson = $"[{string.Join(", ", noneValues.Select(v => JsonSerializer.Serialize(v)))}]";

        blocksConfig.SetEventTrigger(
            targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
            fn: null,
            inputs: Array.Empty<object>(),
            outputs: normalized.Cast<object>().ToList(),
            js: $"() => {clearValuesJson}",
            preprocess: false,
            postprocess: false,
            apiName: ApiName,
            apiVisibility: "private"
        );

        if (stateComponents.Count > 0)
        {
            blocksConfig.SetEventTrigger(
                targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
                fn: (Func<object?>)(() => ResolveSingleton(initialStates.Select(TryDeepCopy).ToList())),
                inputs: null,
                outputs: stateComponents.Cast<object>().ToList(),
                apiName: ApiName,
                apiVisibility: ApiVisibility
            );
        }

        return this;
    }

    public override object? Preprocess(object? input)
    {
        return input?.ToString();
    }

    public override object? Postprocess(object? output)
    {
        return output?.ToString();
    }

    public override object ExamplePayload() => "Clear";

    public override object ExampleValue() => "Clear";

    private static List<Component> NormalizeComponents(object components)
    {
        if (components is Component single)
        {
            return new List<Component> { single };
        }

        if (components is IEnumerable<Component> typed)
        {
            return typed.Where(c => c != null).ToList();
        }

        if (components is IEnumerable enumerable && components is not string)
        {
            var result = new List<Component>();
            foreach (var item in enumerable)
            {
                if (item is Component c)
                {
                    result.Add(c);
                }
            }
            return result;
        }

        return new List<Component>();
    }

    private static bool IsStateLike(Component component)
    {
        return component.GetType().Name.Contains("state", StringComparison.OrdinalIgnoreCase);
    }

    private static object? GetComponentStateValue(Component component)
    {
        var type = component.GetType();
        var valueProp = type.GetProperty("Value");
        if (valueProp != null)
        {
            return valueProp.GetValue(component);
        }

        var defaultValueProp = type.GetProperty("DefaultValue");
        if (defaultValueProp != null)
        {
            return defaultValueProp.GetValue(component);
        }

        return null;
    }

    private static object? PostprocessNone(Component component)
    {
        try
        {
            return component.Postprocess(null);
        }
        catch
        {
            return null;
        }
    }

    private static object? ToSerializable(object? value)
    {
        if (value is GradioModel model)
        {
            return model.ModelDump();
        }

        if (value != null)
        {
            var type = value.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GradioRootModel<>))
            {
                var dump = type.GetMethod("ModelDump");
                if (dump != null)
                {
                    return dump.Invoke(value, null);
                }
            }
        }

        return value;
    }

    private static object? ResolveSingleton(IReadOnlyList<object?> values)
    {
        return values.Count == 1 ? values[0] : values.ToList();
    }

    private static object? TryDeepCopy(object? value)
    {
        if (value == null)
        {
            return null;
        }

        try
        {
            var json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize<object>(json);
        }
        catch
        {
            return value;
        }
    }
}

using System.Text.Json;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("change")]
public class JsonComponent : Component
{
    public bool Open { get; set; }
    public bool ShowIndices { get; set; }
    public object? Height { get; set; }
    public object? MaxHeight { get; set; }
    public object? MinHeight { get; set; }
    public List<string>? Buttons { get; set; }

    public JsonComponent(
        object? value = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool open = false,
        bool showIndices = false,
        object? height = null,
        object? maxHeight = null,
        object? minHeight = null,
        List<string>? buttons = null)
    {
        Open = open;
        ShowIndices = showIndices;
        Height = height;
        MaxHeight = maxHeight ?? 500;
        MinHeight = minHeight;
        Buttons = buttons ?? new List<string> { "copy" };

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "json";
    public override string GetBlockClass() => "json";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["open"] = Open;
        config["show_indices"] = ShowIndices;
        config["height"] = Height;
        config["max_height"] = MaxHeight;
        config["min_height"] = MinHeight;
        config["buttons"] = Buttons;
        return config;
    }

    public override object? Preprocess(object? input)
    {
        return input;
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        object root;
        if (value is string s)
        {
            root = ParseJsonString(s);
        }
        else
        {
            root = NormalizeToJsonCompatible(value);
        }

        return new JsonData { Root = root };
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object> { ["foo"] = "bar" };
    }

    public override object ExampleValue()
    {
        return new Dictionary<string, object> { ["foo"] = "bar" };
    }

    public override object ReadFromFlag(object payload)
    {
        if (payload is string s)
        {
            return ParseJsonString(s);
        }

        return payload;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any valid json"
        };
    }

    public override object AsExample(object example)
    {
        var processed = Postprocess(example) as JsonData;
        return processed?.Root ?? example;
    }

    private static object ParseJsonString(string s)
    {
        try
        {
            return JsonSerializer.Deserialize<object>(s) ?? new Dictionary<string, object>();
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Invalid JSON string.", ex);
        }
    }

    private static object NormalizeToJsonCompatible(object value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize<object>(json) ?? new Dictionary<string, object>();
        }
        catch
        {
            // Fallback to string conversion for unsupported runtime objects.
            return value.ToString() ?? string.Empty;
        }
    }
}

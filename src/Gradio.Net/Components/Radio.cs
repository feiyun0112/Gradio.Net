using System.Collections;
using System.Text.Json;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("select")]
[Events.Event("change")]
[Events.Event("input")]
public class Radio : FormComponent
{
    public List<(string Name, object Value)> Choices { get; set; } = new();
    public string Type { get; set; } = "value";
    public bool Rtl { get; set; }
    public List<object> Buttons { get; set; } = new();

    public Radio(
        object? choices = null,
        object? value = null,
        string type = "value",
        string? label = null,
        string? info = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool rtl = false,
        List<object>? buttons = null)
    {
        Choices = NormalizeChoices(choices);

        var validTypes = new[] { "value", "index" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        Type = type;
        Rtl = rtl;
        Buttons = buttons ?? new List<object>();

        Value = value;
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
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

    public override string GetBlockName() => "radio";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["choices"] = Choices.Select(c => new List<object> { c.Name, c.Value }).ToList();
        config["type"] = Type;
        config["rtl"] = Rtl;
        config["buttons"] = Buttons;
        return config;
    }

    public override object ExamplePayload()
    {
        return Choices.Count > 0 ? Choices[0].Value : null!;
    }

    public override object ExampleValue()
    {
        return Choices.Count > 0 ? Choices[0].Value : null!;
    }

    public override object? Preprocess(object? payload)
    {
        payload = NormalizePayload(payload);

        if (payload == null)
        {
            return null;
        }

        var choiceValues = Choices.Select(c => c.Value).ToList();
        if (!ContainsValue(choiceValues, payload))
        {
            throw new Error($"Value: {FormatValue(payload)} (type: {payload.GetType()}) is not in the list of choices: [{string.Join(", ", choiceValues)}]");
        }

        if (Type == "value")
        {
            return payload;
        }

        if (Type == "index")
        {
            return IndexOfValue(choiceValues, payload);
        }

        throw new Error($"Unknown type: {Type}. Please choose from: 'value', 'index'.");
    }

    public override object? Postprocess(object? value)
    {
        return value;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["enum"] = Choices.Select(c => c.Value).ToList(),
            ["title"] = "Radio",
            ["type"] = "string"
        };
    }

    private static List<(string Name, object Value)> NormalizeChoices(object? choices)
    {
        var result = new List<(string Name, object Value)>();
        if (choices == null)
        {
            return result;
        }

        if (choices is IEnumerable enumerable && choices is not string)
        {
            foreach (var item in enumerable)
            {
                if (item is IEnumerable pair && item is not string)
                {
                    var pairList = new List<object>();
                    foreach (var p in pair)
                    {
                        pairList.Add(p!);
                    }

                    if (pairList.Count >= 2)
                    {
                        result.Add((pairList[0]?.ToString() ?? string.Empty, pairList[1]!));
                        continue;
                    }
                }

                result.Add((item?.ToString() ?? string.Empty, item!));
            }

            return result;
        }

        result.Add((choices.ToString() ?? string.Empty, choices));
        return result;
    }

    private static bool ContainsValue(List<object> values, object? target)
    {
        return values.Any(v => ValueEquals(v, target));
    }

    private static int? IndexOfValue(List<object> values, object? target)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (ValueEquals(values[i], target))
            {
                return i;
            }
        }
        return null;
    }

    private static bool ValueEquals(object? a, object? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        if (a.Equals(b)) return true;
        return string.Equals(a.ToString(), b.ToString(), StringComparison.Ordinal);
    }

    private static object? NormalizePayload(object? payload)
    {
        if (payload is not JsonElement je)
        {
            return payload;
        }

        return je.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String => je.GetString(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number =>
                je.TryGetInt32(out var i) ? i :
                je.TryGetInt64(out var l) ? l :
                je.TryGetDouble(out var d) ? d :
                je.ToString(),
            _ => je.ToString()
        };
    }

    private static string FormatValue(object? value)
    {
        if (value == null) return "null";
        return value is string s ? $"'{s}'" : value.ToString() ?? "null";
    }
}

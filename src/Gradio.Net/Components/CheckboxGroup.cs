using System.Collections;
using System.Text.Json;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
public class CheckboxGroup : FormComponent
{
    public List<(string Name, object Value)> Choices { get; set; } = new();

    public string Type { get; set; } = "value";

    public bool ShowSelectAll { get; set; }

    public List<string> Buttons { get; set; } = new();

    public CheckboxGroup(
        object choices = null,
        object value = null,
        string type = "value",
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool showSelectAll = false,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        List<string>? buttons = null)
    {
        Choices = NormalizeChoices(choices);

        var validTypes = new[] { "value", "index" };
        if (!validTypes.Contains(type))
        {
            throw new ArgumentException($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        Type = type;
        ShowSelectAll = showSelectAll;

        Value = value == null ? new List<object>() : Postprocess(value);
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
        Buttons = buttons ?? new List<string>();
    }

    private static List<(string Name, object Value)> NormalizeChoices(object choices)
    {
        var result = new List<(string Name, object Value)>();
        if (choices == null)
        {
            return result;
        }

        if (choices is IEnumerable enumerable && choices is not string)
        {
            foreach (var c in enumerable)
            {
                if (c is IEnumerable pair && c is not string)
                {
                    var pairList = new List<object>();
                    foreach (var p in pair)
                    {
                        pairList.Add(p);
                    }

                    if (pairList.Count >= 2)
                    {
                        result.Add((pairList[0]?.ToString() ?? string.Empty, pairList[1]));
                        continue;
                    }
                }

                result.Add((c?.ToString() ?? string.Empty, c));
            }

            return result;
        }

        // single primitive value fallback
        result.Add((choices.ToString() ?? string.Empty, choices));
        return result;
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["choices"] = Choices.Select(c => new List<object> { c.Name, c.Value }).ToList();
        config["value"] = Value;
        config["type"] = Type;
        config["show_select_all"] = ShowSelectAll;
        config["buttons"] = Buttons;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            {
                "items", new Dictionary<string, object>
                {
                    { "enum", Choices.Select(c => c.Value?.ToString() ?? string.Empty).ToList() },
                    { "type", "string" }
                }
            },
            { "title", "Checkbox Group" },
            { "type", "array" }
        };
    }

    public override object Preprocess(object payload)
    {
        var selected = ToObjectList(payload);
        var choiceValues = Choices.Select(c => c.Value).ToList();

        foreach (var value in selected)
        {
            if (!ContainsValue(choiceValues, value))
            {
                throw new Error($"Value: {FormatValue(value)} (type: {value?.GetType()}) is not in the list of choices: [{string.Join(", ", choiceValues)}]");
            }
        }

        if (Type == "value")
        {
            return selected;
        }

        if (Type == "index")
        {
            var indices = new List<int?>();
            foreach (var value in selected)
            {
                var idx = IndexOfValue(choiceValues, value);
                indices.Add(idx >= 0 ? idx : null);
            }
            return indices;
        }

        throw new ArgumentException($"Unknown type: {Type}. Please choose from: 'value', 'index'.");
    }

    public override object Postprocess(object value)
    {
        if (value == null)
        {
            return new List<object>();
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var result = new List<object>();
            foreach (var item in enumerable)
            {
                result.Add(item);
            }
            return result;
        }

        return new List<object> { value };
    }

    public override List<object> ExampleInputs()
    {
        if (Choices.Count == 0)
        {
            return new List<object>();
        }

        return new List<object> { new List<object> { Choices[0].Value } };
    }

    private static List<object> ToObjectList(object payload)
    {
        if (payload == null)
        {
            return new List<object>();
        }

        if (payload is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.Null || je.ValueKind == JsonValueKind.Undefined)
            {
                return new List<object>();
            }

            if (je.ValueKind == JsonValueKind.Array)
            {
                var arr = new List<object>();
                foreach (var item in je.EnumerateArray())
                {
                    arr.Add(NormalizeJsonElement(item));
                }
                return arr;
            }

            return new List<object> { NormalizeJsonElement(je) };
        }

        if (payload is IEnumerable enumerable && payload is not string)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                if (item is JsonElement itemJe)
                {
                    list.Add(NormalizeJsonElement(itemJe));
                }
                else
                {
                    list.Add(item);
                }
            }
            return list;
        }

        return new List<object> { payload };
    }

    private static object? NormalizeJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var i) ? i : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.Array => element.EnumerateArray().Select(NormalizeJsonElement).ToList(),
            _ => element.ToString()
        };
    }

    private static bool ContainsValue(List<object> values, object target)
    {
        return values.Any(v => ValueEquals(v, target));
    }

    private static int IndexOfValue(List<object> values, object target)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (ValueEquals(values[i], target)) return i;
        }
        return -1;
    }

    private static bool ValueEquals(object a, object b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;

        // Python-style permissive equality for numeric/string data coming from JSON
        if (a.Equals(b)) return true;
        return string.Equals(a.ToString(), b.ToString(), StringComparison.Ordinal);
    }

    private static string FormatValue(object value)
    {
        if (value == null) return "null";
        if (value is string s) return $"'{s}'";
        return value.ToString();
    }
}

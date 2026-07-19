using System.Collections;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
[Events.Event("focus")]
[Events.Event("blur")]
[Events.Event("key_up")]
public class Dropdown : FormComponent
{
    public List<(string Name, object Value)> Choices { get; set; } = new();
    public string Type { get; set; } = "value";
    public bool? Multiselect { get; set; }
    public bool AllowCustomValue { get; set; }
    public int? MaxChoices { get; set; }
    public bool Filterable { get; set; }

    public Dropdown(
        object? choices = null,
        object? value = null,
        string type = "value",
        bool? multiselect = null,
        bool allowCustomValue = false,
        int? maxChoices = null,
        bool filterable = true,
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
        object? preservedByKey = null)
    {
        Choices = NormalizeChoices(choices);

        var validTypes = new[] { "value", "index" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        Type = type;
        Multiselect = multiselect;

        // Default-value sentinel parity (best-effort):
        // if caller does not provide value, select first choice unless multiselect then []
        if (value == null)
        {
            if (multiselect == true)
            {
                value = new List<object>();
            }
            else if (Choices.Count > 0)
            {
                value = Choices[0].Value;
            }
        }

        if (multiselect == true && value is string s)
        {
            value = new List<object> { s };
        }

        if (multiselect != true && maxChoices != null)
        {
        }

        if (!filterable && allowCustomValue)
        {
            filterable = true;
        }

        MaxChoices = maxChoices;
        AllowCustomValue = allowCustomValue;
        Filterable = filterable;

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

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["choices"] = Choices.Select(c => new List<object> { c.Name, c.Value }).ToList();
        config["type"] = Type;
        config["multiselect"] = Multiselect;
        config["allow_custom_value"] = AllowCustomValue;
        config["max_choices"] = MaxChoices;
        config["filterable"] = Filterable;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        var enumValues = Choices.Select(c => c.Value).ToList();
        if (Multiselect == true)
        {
            return new Dictionary<string, object>
            {
                ["type"] = "array",
                ["items"] = new Dictionary<string, object>
                {
                    ["type"] = "string",
                    ["enum"] = enumValues
                }
            };
        }

        return new Dictionary<string, object>
        {
            ["type"] = "string",
            ["enum"] = enumValues
        };
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var choiceValues = Choices.Select(c => c.Value).ToList();

        if (!AllowCustomValue)
        {
            if (payload is IEnumerable enumerable && payload is not string)
            {
                foreach (var value in enumerable)
                {
                    if (!ContainsValue(choiceValues, value))
                    {
                        throw new Error($"Value: {FormatValue(value)} (type: {value?.GetType()}) is not in the list of choices: [{string.Join(", ", choiceValues)}]");
                    }
                }
            }
            else if (!ContainsValue(choiceValues, payload))
            {
                throw new Error($"Value: {FormatValue(payload)} is not in the list of choices: [{string.Join(", ", choiceValues)}]");
            }
        }

        if (Type == "value")
        {
            return payload;
        }

        if (Type == "index")
        {
            if (payload is IEnumerable pEnum && payload is not string)
            {
                var indices = new List<int?>();
                foreach (var choice in pEnum)
                {
                    indices.Add(IndexOfValue(choiceValues, choice));
                }
                return indices;
            }

            return IndexOfValue(choiceValues, payload);
        }

        throw new Error($"Unknown type: {Type}. Please choose from: 'value', 'index'.");
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (Multiselect == true)
        {
            if (value is not IEnumerable || value is string)
            {
                value = new List<object> { value };
            }

            foreach (var v in (IEnumerable)value)
            {
                WarnIfInvalidChoice(v);
            }

            return value;
        }

        WarnIfInvalidChoice(value);
        return value;
    }

    public override object ExamplePayload()
    {
        if (Multiselect == true)
        {
            return Choices.Count > 0 ? new List<object> { Choices[0].Value } : new List<object>();
        }

        return Choices.Count > 0 ? Choices[0].Value : null;
    }

    public override object ExampleValue()
    {
        if (Multiselect == true)
        {
            return Choices.Count > 0 ? new List<object> { Choices[0].Value } : new List<object>();
        }

        return Choices.Count > 0 ? Choices[0].Value : null;
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

    private void WarnIfInvalidChoice(object? value)
    {
        if (AllowCustomValue)
        {
            return;
        }

        var choiceValues = Choices.Select(c => c.Value).ToList();
        if (ContainsValue(choiceValues, value))
        {
            return;
        }

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

    private static string FormatValue(object? value)
    {
        if (value == null) return "null";
        return value is string s ? $"'{s}'" : value.ToString() ?? "null";
    }
}

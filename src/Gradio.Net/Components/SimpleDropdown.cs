using System.Collections;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
public class SimpleDropdown : FormComponent
{
    public List<(string Name, object Value)> Choices { get; set; } = new();

    public SimpleDropdown(
        object? choices = null,
        object? value = null,
        string? label = null,
        string? info = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
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

        Value = value;
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? ShowLabel;
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

    public override string GetBlockName() => "simpledropdown";

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "string",
            ["enum"] = Choices.Select(c => c.Value).ToList()
        };
    }

    public override object ExamplePayload() => Choices.Count > 0 ? Choices[0].Value : null!;

    public override object ExampleValue() => Choices.Count > 0 ? Choices[0].Value : null!;

    public override object? Preprocess(object? payload) => payload;

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        WarnIfInvalidChoice(value);
        return value;
    }

    public override object ProcessExample(object value)
    {
        return Choices.FirstOrDefault(c => Equals(c.Value, value)).Name;
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["choices"] = Choices.Select(c => new List<object> { c.Name, c.Value }).ToList();
        return config;
    }

    private void WarnIfInvalidChoice(object value)
    {
        var allValues = Choices.Select(c => c.Value).ToList();
        if (!allValues.Any(v => Equals(v, value)))
        {
        }
    }

    private static List<(string Name, object Value)> NormalizeChoices(object? choices)
    {
        var result = new List<(string Name, object Value)>();
        if (choices == null) return result;

        if (choices is IEnumerable enumerable && choices is not string)
        {
            foreach (var c in enumerable)
            {
                if (c is IEnumerable pair && c is not string)
                {
                    var pairList = new List<object>();
                    foreach (var p in pair) pairList.Add(p!);
                    if (pairList.Count >= 2)
                    {
                        result.Add((pairList[0]?.ToString() ?? string.Empty, pairList[1]!));
                        continue;
                    }
                }
                result.Add((c?.ToString() ?? string.Empty, c!));
            }
        }

        return result;
    }
}

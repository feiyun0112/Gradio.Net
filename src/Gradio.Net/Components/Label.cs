using System.Collections;
using System.Text.Json;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("select")]
public class Label : Component
{
    public const string CONFIDENCES_KEY = "confidences";

    public int? NumTopClasses { get; set; }
    public string? Color { get; set; }
    public bool ShowHeading { get; set; }

    public Label(
        object? value = null,
        int? numTopClasses = null,
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
        string? color = null,
        bool showHeading = true)
    {
        NumTopClasses = numTopClasses;
        Color = color;
        ShowHeading = showHeading;

        Value = value != null ? Postprocess(value) : new Dictionary<string, object>();
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

    public override string GetBlockName() => "label";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["num_top_classes"] = NumTopClasses;
        config["color"] = Color;
        config["show_heading"] = ShowHeading;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var data = payload as LabelData ?? ParseLabelData(payload);
        if (data == null)
        {
            return null;
        }

        if (data.Confidences == null)
        {
            return data.Label;
        }

        var output = new Dictionary<string, double?>();
        foreach (var c in data.Confidences)
        {
            var key = c.Label?.ToString() ?? string.Empty;
            output[key] = c.Confidence;
        }

        return output;
    }

    public override object Postprocess(object value)
    {
        if (value == null)
        {
            return new Dictionary<string, object>();
        }

        if (value is IDictionary dict && dict.Count == 0)
        {
            return new Dictionary<string, object>();
        }

        if (value is string s && s.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && File.Exists(s))
        {
            var text = File.ReadAllText(s);
            var parsed = JsonSerializer.Deserialize<LabelData>(text);
            return parsed ?? new LabelData();
        }

        if (value is string or int or float or double or long or decimal)
        {
            return new LabelData { Label = value.ToString() };
        }

        if (value is IDictionary<string, object> typed)
        {
            return FromConfidenceDict(typed.ToDictionary(k => k.Key, v => v.Value));
        }

        if (value is IDictionary untyped)
        {
            var converted = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in untyped)
            {
                converted[entry.Key?.ToString() ?? string.Empty] = entry.Value!;
            }
            return FromConfidenceDict(converted);
        }

        throw new Error(
            "The `Label` output interface expects one of: a string label, or an int label, a float label, or a dictionary whose keys are labels and values are confidences.");
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
        {
            ["label"] = "Cat",
            ["confidences"] = new List<Dictionary<string, object>>
            {
                new() { ["label"] = "cat", ["confidence"] = 0.9 },
                new() { ["label"] = "dog", ["confidence"] = 0.1 }
            }
        };
    }

    public override object ExampleValue()
    {
        return new Dictionary<string, object>
        {
            ["cat"] = 0.9,
            ["dog"] = 0.1
        };
    }

    private LabelData FromConfidenceDict(Dictionary<string, object> value)
    {
        // Python accepts shape with nested "confidences" payload.
        if (value.TryGetValue(CONFIDENCES_KEY, out var confObj))
        {
            // If already list[{label,confidence}] treat it directly.
            if (confObj is IEnumerable confEnum && confObj is not string)
            {
                var confs = new List<LabelConfidence>();
                foreach (var item in confEnum)
                {
                    if (item is IDictionary<string, object> d)
                    {
                        confs.Add(new LabelConfidence
                        {
                            Label = d.TryGetValue("label", out var l) ? l : null,
                            Confidence = d.TryGetValue("confidence", out var c) ? ToDoubleOrNull(c) : null
                        });
                    }
                }

                if (confs.Count > 0)
                {
                    var sorted = confs
                        .OrderByDescending(x => x.Confidence ?? double.MinValue)
                        .ToList();
                    if (NumTopClasses.HasValue)
                    {
                        sorted = sorted.Take(NumTopClasses.Value).ToList();
                    }

                    return new LabelData
                    {
                        Label = sorted[0].Label,
                        Confidences = sorted
                    };
                }
            }

            if (confObj is IDictionary<string, object> confDict)
            {
                value = confDict.ToDictionary(k => k.Key, v => v.Value);
            }
        }

        var sortedPred = value
            .Select(kv => new LabelConfidence
            {
                Label = kv.Key,
                Confidence = ToDoubleOrNull(kv.Value)
            })
            .OrderByDescending(x => x.Confidence ?? double.MinValue)
            .ToList();

        if (sortedPred.Count == 0)
        {
            return new LabelData();
        }

        if (NumTopClasses.HasValue)
        {
            sortedPred = sortedPred.Take(NumTopClasses.Value).ToList();
        }

        return new LabelData
        {
            Label = sortedPred[0].Label,
            Confidences = sortedPred
        };
    }

    private static LabelData? ParseLabelData(object payload)
    {
        if (payload is LabelData ld)
        {
            return ld;
        }

        if (payload is Dictionary<string, object> dict)
        {
            var data = new LabelData();
            if (dict.TryGetValue("label", out var l))
            {
                data.Label = l;
            }

            if (dict.TryGetValue("confidences", out var c) && c is IEnumerable enumerable && c is not string)
            {
                var list = new List<LabelConfidence>();
                foreach (var item in enumerable)
                {
                    if (item is Dictionary<string, object> cd)
                    {
                        list.Add(new LabelConfidence
                        {
                            Label = cd.TryGetValue("label", out var cl) ? cl : null,
                            Confidence = cd.TryGetValue("confidence", out var cc) ? ToDoubleOrNull(cc) : null
                        });
                    }
                }
                data.Confidences = list;
            }

            return data;
        }

        return null;
    }

    private static double? ToDoubleOrNull(object? obj)
    {
        if (obj == null) return null;
        if (obj is double d) return d;
        if (obj is float f) return f;
        if (obj is int i) return i;
        if (obj is long l) return l;
        if (obj is decimal m) return (double)m;
        return double.TryParse(obj.ToString(), out var x) ? x : null;
    }
}

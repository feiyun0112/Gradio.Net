using System.Collections;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("select")]
public class HighlightedText : Component
{
    public Dictionary<string, string>? ColorMap { get; set; }
    public bool ShowLegend { get; set; }
    public bool ShowInlineCategory { get; set; }
    public bool CombineAdjacent { get; set; }
    public string AdjacentSeparator { get; set; }
    public bool Rtl { get; set; }

    public HighlightedText(
        object? value = null,
        Dictionary<string, string>? colorMap = null,
        bool showLegend = false,
        bool showInlineCategory = true,
        bool combineAdjacent = false,
        string adjacentSeparator = "",
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
        bool? interactive = null,
        bool rtl = false)
    {
        ColorMap = colorMap;
        ShowLegend = showLegend;
        ShowInlineCategory = showInlineCategory;
        CombineAdjacent = combineAdjacent;
        AdjacentSeparator = adjacentSeparator;
        Rtl = rtl;

        Value = value;
        Label = label;
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

    public override string GetBlockName() => "highlightedtext";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["color_map"] = ColorMap;
        config["show_legend"] = ShowLegend;
        config["show_inline_category"] = ShowInlineCategory;
        config["combine_adjacent"] = CombineAdjacent;
        config["adjacent_separator"] = AdjacentSeparator;
        config["rtl"] = Rtl;
        return config;
    }

    public override object ExamplePayload()
    {
        return new List<Dictionary<string, object?>>
        {
            new() { ["token"] = "The", ["class_or_confidence"] = null },
            new() { ["token"] = "quick", ["class_or_confidence"] = "adj" }
        };
    }

    public override object ExampleValue()
    {
        return new List<Tuple<string, object?>>
        {
            Tuple.Create("The", (object?)null),
            Tuple.Create("quick", (object?)"adj"),
            Tuple.Create("brown", (object?)"adj"),
            Tuple.Create("fox", (object?)"noun")
        };
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var data = ToHighlightedData(payload);
        return data?.ModelDump();
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var tuples = NormalizeInput(value);

        if (CombineAdjacent)
        {
            var output = new List<Tuple<string, object?>>();
            string? runningText = null;
            object? runningCategory = null;

            foreach (var (text, category) in tuples)
            {
                if (runningText == null)
                {
                    runningText = text;
                    runningCategory = category;
                }
                else if (SameCategory(category, runningCategory))
                {
                    runningText += AdjacentSeparator + text;
                }
                else if (string.IsNullOrEmpty(text))
                {
                    // Skip empty slices (python parity)
                }
                else
                {
                    output.Add(Tuple.Create(runningText, runningCategory));
                    runningText = text;
                    runningCategory = category;
                }
            }

            if (runningText != null)
            {
                output.Add(Tuple.Create(runningText, runningCategory));
            }

            tuples = output;
        }

        return new HighlightedTextData
        {
            Root = tuples.Select(o => new HighlightedTokenData
            {
                Token = o.Item1,
                ClassOrConfidence = o.Item2
            }).ToList()
        };
    }

    private static bool SameCategory(object? a, object? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        return string.Equals(a.ToString(), b.ToString(), StringComparison.Ordinal);
    }

    private static HighlightedTextData? ToHighlightedData(object payload)
    {
        if (payload is HighlightedTextData data)
        {
            return data;
        }

        if (payload is Dictionary<string, object> dict && dict.TryGetValue("root", out var rootObj))
        {
            var tuples = NormalizeInput(rootObj);
            return new HighlightedTextData
            {
                Root = tuples.Select(t => new HighlightedTokenData
                {
                    Token = t.Item1,
                    ClassOrConfidence = t.Item2
                }).ToList()
            };
        }

        return null;
    }

    private static List<Tuple<string, object?>> NormalizeInput(object value)
    {
        // dict format: {"text": ..., "entities": [{"start":..,"end":..,"entity"|"entity_group":..}, ...]}
        if (value is Dictionary<string, object> dict && dict.ContainsKey("text") && dict.ContainsKey("entities"))
        {
            return DictToTuples(dict);
        }

        var result = new List<Tuple<string, object?>>();

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                if (item is Tuple<string, object?> t)
                {
                    result.Add(Tuple.Create(t.Item1, t.Item2));
                    continue;
                }

                if (item is object[] arr && arr.Length >= 2)
                {
                    result.Add(Tuple.Create(arr[0]?.ToString() ?? string.Empty, arr[1]));
                    continue;
                }

                if (item is IList list && list.Count >= 2)
                {
                    result.Add(Tuple.Create(list[0]?.ToString() ?? string.Empty, list[1]));
                    continue;
                }

                if (item is Dictionary<string, object> tokenDict)
                {
                    var token = tokenDict.TryGetValue("token", out var tok) ? tok?.ToString() ?? string.Empty : string.Empty;
                    tokenDict.TryGetValue("class_or_confidence", out var coc);
                    result.Add(Tuple.Create(token, coc));
                }
            }

            return result;
        }

        throw new Error("Unsupported value format for HighlightedText component.");
    }

    private static List<Tuple<string, object?>> DictToTuples(Dictionary<string, object> value)
    {
        var text = value["text"]?.ToString() ?? string.Empty;
        if (value["entities"] is not IEnumerable entitiesEnum)
        {
            throw new Error("Expected 'entities' to be a list in HighlightedText dict format.");
        }

        var entities = new List<Dictionary<string, object>>();
        foreach (var e in entitiesEnum)
        {
            if (e is Dictionary<string, object> ed)
            {
                entities.Add(ed);
            }
        }

        if (entities.Count == 0)
        {
            return new List<Tuple<string, object?>> { Tuple.Create(text, (object?)null) };
        }

        entities = entities
            .OrderBy(x => Convert.ToInt32(x["start"]))
            .ToList();

        var listFormat = new List<Tuple<string, object?>>();
        var index = 0;
        foreach (var entity in entities)
        {
            var start = Convert.ToInt32(entity["start"]);
            var end = Convert.ToInt32(entity["end"]);
            var category = entity.ContainsKey("entity") ? entity["entity"] : entity.GetValueOrDefault("entity_group");

            listFormat.Add(Tuple.Create(text.Substring(index, Math.Max(0, start - index)), (object?)null));
            listFormat.Add(Tuple.Create(text.Substring(start, Math.Max(0, end - start)), category));
            index = end;
        }

        listFormat.Add(Tuple.Create(text.Substring(Math.Min(index, text.Length)), (object?)null));
        return listFormat;
    }
}

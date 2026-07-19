using System.Collections;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("upload")]
public class ParamViewer : Component
{
    public Dictionary<string, ParamViewerParameter> ParamValue { get; set; }
    public string Language { get; set; }
    public List<string>? Linkify { get; set; }
    public string? Header { get; set; }
    public object AnchorLinks { get; set; }
    public object? MaxHeight { get; set; }

    public ParamViewer(
        Dictionary<string, ParamViewerParameter>? value = null,
        string language = "python",
        List<string>? linkify = null,
        object? every = null,
        object? inputs = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        string? header = "Parameters",
        object? anchorLinks = null,
        object? maxHeight = null)
    {
        ParamValue = value ?? new Dictionary<string, ParamViewerParameter>();
        Language = language;
        Linkify = linkify;
        Header = header;
        AnchorLinks = anchorLinks ?? false;
        MaxHeight = maxHeight;

        Value = value;
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "paramviewer";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["language"] = Language;
        config["linkify"] = Linkify;
        config["header"] = Header;
        config["anchor_links"] = AnchorLinks;
        config["max_height"] = MaxHeight;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        return payload == null ? null : Normalize(payload);
    }

    public override object? Postprocess(object? value)
    {
        return value == null ? null : Normalize(value);
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
        {
            ["array"] = new Dictionary<string, object>
            {
                ["type"] = "numpy",
                ["description"] = "any valid json",
                ["default"] = "None"
            }
        };
    }

    public override object ExampleValue()
    {
        return ExamplePayload();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any valid json"
        };
    }

    private static Dictionary<string, ParamViewerParameter> Normalize(object payload)
    {
        if (payload is Dictionary<string, ParamViewerParameter> typed)
        {
            return typed;
        }

        var output = new Dictionary<string, ParamViewerParameter>();

        if (payload is IDictionary<string, object> dict)
        {
            foreach (var kv in dict)
            {
                output[kv.Key] = ToParameter(kv.Value);
            }
            return output;
        }

        if (payload is IDictionary raw)
        {
            foreach (DictionaryEntry kv in raw)
            {
                var key = kv.Key?.ToString() ?? string.Empty;
                output[key] = ToParameter(kv.Value);
            }
            return output;
        }

        return output;
    }

    private static ParamViewerParameter ToParameter(object? value)
    {
        if (value is ParamViewerParameter p)
        {
            return p;
        }

        if (value is IDictionary<string, object> dict)
        {
            return new ParamViewerParameter
            {
                Type = dict.TryGetValue("type", out var t) ? t?.ToString() ?? string.Empty : string.Empty,
                Description = dict.TryGetValue("description", out var d) ? d?.ToString() ?? string.Empty : string.Empty,
                Default = dict.TryGetValue("default", out var def) ? def?.ToString() : null
            };
        }

        if (value is IDictionary raw)
        {
            string type = string.Empty;
            string description = string.Empty;
            string? def = null;
            foreach (DictionaryEntry de in raw)
            {
                var k = de.Key?.ToString();
                if (k == "type") type = de.Value?.ToString() ?? string.Empty;
                else if (k == "description") description = de.Value?.ToString() ?? string.Empty;
                else if (k == "default") def = de.Value?.ToString();
            }

            return new ParamViewerParameter { Type = type, Description = description, Default = def };
        }

        return new ParamViewerParameter();
    }
}

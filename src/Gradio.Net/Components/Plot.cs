using System.Reflection;
using System.Text.Json;
using Gradio.Net.Core;

namespace Gradio.Net.Components;

[Events.Event("change")]
public class Plot : Component
{
    public string Format { get; set; }

    public Plot(
        object? value = null,
        string format = "webp",
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
        object? preservedByKey = null)
    {
        Format = format;

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

    public override string GetBlockName() => "plot";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        // Python: include bokeh version when available.
        config["bokeh_version"] = null;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        return payload;
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is PlotData pd)
        {
            return pd;
        }

        if (TryFromDictionary(value, out var fromDict))
        {
            return fromDict;
        }

        var typeName = value.GetType().FullName?.ToLowerInvariant() ?? string.Empty;

        if (typeName.Contains("matplotlib"))
        {
            return new PlotData
            {
                Type = "matplotlib",
                Plot = ProcessingUtils.EncodePlotToBase64(value, Format)
            };
        }

        if (typeName.Contains("bokeh"))
        {
            return new PlotData
            {
                Type = "bokeh",
                Plot = JsonSerializer.Serialize(value)
            };
        }

        var isAltair = typeName.Contains("altair");
        var dtype = isAltair ? "altair" : "plotly";

        return new PlotData
        {
            Type = dtype,
            Plot = TryToJson(value)
        };
    }

    public override object ExamplePayload() => null!;

    public override object ExampleValue() => null!;

    private static bool TryFromDictionary(object value, out PlotData? plot)
    {
        plot = null;
        if (value is not Dictionary<string, object> dict)
        {
            return false;
        }

        if (!dict.TryGetValue("type", out var t) || !dict.TryGetValue("plot", out var p))
        {
            return false;
        }

        plot = new PlotData
        {
            Type = t?.ToString() ?? string.Empty,
            Plot = p?.ToString() ?? string.Empty
        };
        return true;
    }

    private static string TryToJson(object value)
    {
        var toJson = value.GetType().GetMethod("ToJson", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
        if (toJson != null)
        {
            try
            {
                var json = toJson.Invoke(value, null)?.ToString();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return json;
                }
            }
            catch
            {
                // fall through to serializer
            }
        }

        try
        {
            return JsonSerializer.Serialize(value);
        }
        catch
        {
            return value.ToString() ?? string.Empty;
        }
    }
}

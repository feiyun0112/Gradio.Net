using System.Text.Json;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("submit")]
[Events.Event("focus")]
[Events.Event("blur")]
public class Number : FormComponent
{
    public int? Precision { get; set; }
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
    public double Step { get; set; }
    public string? Placeholder { get; set; }
    public List<object> Buttons { get; set; } = new();

    public Number(
        object? value = null,
        string? label = null,
        string? placeholder = null,
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
        List<object>? buttons = null,
        int? precision = null,
        double? minimum = null,
        double? maximum = null,
        double step = 1)
    {
        Precision = precision;
        Minimum = minimum;
        Maximum = maximum;
        Step = step;
        Placeholder = placeholder;
        Buttons = buttons ?? new List<object>();

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

        var (loadFn, initialValue) = GetLoadFnAndInitialValue(value);
        if (loadFn != null)
        {
            // Value is callable: compute initial value, postprocess, then attach load event
            var processedInitial = initialValue != null ? Postprocess(initialValue) : null;
            Value = processedInitial;
            var inputBlocks = ExtractInputBlocks(inputs);
            AttachLoadEvent(loadFn, every, inputBlocks);
        }
        else
        {
            Value = value;
        }
    }

    private static Core.Block[] ExtractInputBlocks(object? inputs)
    {
        if (inputs == null) return Array.Empty<Core.Block>();
        if (inputs is Core.Block single) return new[] { single };
        if (inputs is System.Collections.IEnumerable enumerable and not string)
        {
            var list = new List<Core.Block>();
            foreach (var item in enumerable)
                if (item is Core.Block b) list.Add(b);
            return list.ToArray();
        }
        return Array.Empty<Core.Block>();
    }

    public override string GetBlockName() => "number";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["precision"] = Precision;
        config["minimum"] = Minimum;
        config["maximum"] = Maximum;
        config["step"] = Step;
        config["placeholder"] = Placeholder;
        config["buttons"] = Buttons;
        return config;
    }

    public static object RoundToPrecision(double num, int? precision)
    {
        if (precision == null)
        {
            return num;
        }

        if (precision == 0)
        {
            return (int)Math.Round(num, 0, MidpointRounding.AwayFromZero);
        }

        return Math.Round(num, precision.Value, MidpointRounding.AwayFromZero);
    }

    public static void RaiseIfOutOfBounds(double num, double? minimum, double? maximum)
    {
        if (minimum != null && num < minimum)
        {
            throw new Error($"Value {num} is less than minimum value {minimum}.");
        }

        if (maximum != null && num > maximum)
        {
            throw new Error($"Value {num} is greater than maximum value {maximum}.");
        }
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        if (!TryToDouble(payload, out var num))
        {
            return null;
        }

        RaiseIfOutOfBounds(num, Minimum, Maximum);
        return RoundToPrecision(num, Precision);
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (!TryToDouble(value, out var num))
        {
            return null;
        }

        return RoundToPrecision(num, Precision);
    }

    public override Dictionary<string, object> ApiInfo()
    {
        if (Precision == 0)
        {
            return new Dictionary<string, object> { ["type"] = "integer" };
        }

        return new Dictionary<string, object> { ["type"] = "number" };
    }

    public override object ExamplePayload()
    {
        var v = Minimum ?? 3;
        return RoundToPrecision(v, Precision);
    }

    public override object ExampleValue()
    {
        var v = Minimum ?? 3;
        return RoundToPrecision(v, Precision);
    }

    public override object ReadFromFlag(object payload)
    {
        if (payload is string s)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(s) ?? payload;
            }
            catch
            {
                return payload;
            }
        }

        return payload;
    }

    private static bool TryToDouble(object value, out double result)
    {
        switch (value)
        {
            case double d:
                result = d;
                return true;
            case float f:
                result = f;
                return true;
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            case decimal m:
                result = (double)m;
                return true;
            default:
                return double.TryParse(value.ToString(), out result);
        }
    }
}

using System.Text.Json;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("release")]
public class Slider : FormComponent
{
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public int? Precision { get; set; }
    public double Step { get; set; }
    public List<string>? Buttons { get; set; }

    public Slider(
        double minimum = 0,
        double maximum = 100,
        object? value = null,
        double? step = null,
        int? precision = null,
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
        bool randomize = false,
        List<string>? buttons = null,
        double? min = null,
        double? max = null)
    {
        var effectiveMinimum = min ?? minimum;
        var effectiveMaximum = max ?? maximum;

        Minimum = effectiveMinimum;
        Maximum = effectiveMaximum;
        Precision = precision;
        Step = step ?? ComputeDefaultStep(effectiveMinimum, effectiveMaximum);
        Buttons = buttons;

        if (randomize)
        {
            value = GetRandomValue();
        }

        value ??= effectiveMinimum;

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

    public override string GetBlockName() => "slider";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["minimum"] = Minimum;
        config["maximum"] = Maximum;
        config["step"] = Step;
        config["precision"] = Precision;
        config["buttons"] = Buttons;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = Precision == 0 ? "integer" : "number",
            ["description"] = $"numeric value between {Minimum} and {Maximum}"
        };
    }

    public override object ExamplePayload() => Minimum;

    public override object ExampleValue() => Minimum;

    public double GetRandomValue()
    {
        var nSteps = (int)((Maximum - Minimum) / Step);
        if (nSteps < 0) nSteps = 0;
        var random = new Random();
        var stepIndex = random.Next(0, nSteps + 1);
        var value = Minimum + (stepIndex * Step);

        var nDecimals = DecimalPlaces(Step);
        if (nDecimals > 0)
        {
            value = Math.Round(value, nDecimals, MidpointRounding.AwayFromZero);
        }

        return value;
    }

    public override object? Postprocess(object? value)
    {
        var v = value == null ? Minimum : ToDouble(value);
        return Number.RoundToPrecision(v, Precision);
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var v = ToDouble(payload);
        Number.RaiseIfOutOfBounds(v, Minimum, Maximum);
        return Number.RoundToPrecision(v, Precision);
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

    private static double ComputeDefaultStep(double minimum, double maximum)
    {
        var difference = maximum - minimum;
        if (difference <= 0)
        {
            return 1;
        }

        var power = Math.Floor(Math.Log10(difference) - 2);
        return Math.Pow(10, power);
    }

    private static int DecimalPlaces(double value)
    {
        var text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var idx = text.IndexOf('.');
        return idx < 0 ? 0 : text.Length - idx - 1;
    }

    private static double ToDouble(object value)
    {
        return value switch
        {
            double d => d,
            float f => f,
            int i => i,
            long l => l,
            decimal m => (double)m,
            _ => double.Parse(value.ToString() ?? "0", System.Globalization.CultureInfo.InvariantCulture)
        };
    }
}

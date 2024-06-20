using Gradio.Net.Enums;
using Gradio.Net.jinja2;

namespace Gradio.Net;

public class Slider : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveReleaseEvent
{
    internal Slider() { }
    internal decimal? Step { get; set; }
    internal decimal? Minimum { get; set; }
    internal decimal? Maximum { get; set; }
    internal bool? Randomize { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
          { nameof(Minimum), 0 },
           { nameof(Maximum), 100 },
             { nameof(ShowLabel), true},
                { nameof(Container), true },

        { nameof(MinWidth), 160 },

             { nameof(Visible), true},


          { nameof(Render), true },
            { nameof(Random), false },

    };
    protected override object? GetDefaultProp(string name)
    {
        Dictionary<string, object> result = _defaultProps;
        if (name == nameof(Step))
        {
            decimal difference = this.GetPropertyValue<decimal>(nameof(Maximum)) - this.GetPropertyValue<decimal>(nameof(Minimum));
            double power = Math.Floor(Math.Log10(double.Parse(difference.ToString())) - 2);

            result[nameof(Step)] = decimal.Parse(Math.Pow(10, double.Parse(power.ToString())).ToString());
        }

        if (name == nameof(Value))
        {
            if (this.GetPropertyValue<bool>(nameof(Randomize)))
            {
                result[nameof(Value)] = this.GetRandomValue();
            }
        }

        return result.ContainsKey(name) ? result[name] : null;
    }

    public static decimal Payload(object obj)
    {
        if (obj == null)
        {
            return decimal.MinValue;
        }

        if (obj is decimal str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect decimal actual {obj.GetType()}");
    }

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "decimal" } };
    }

    internal decimal GetRandomValue()
    {
        int nSteps = (int)((this.GetPropertyValue<decimal>(nameof(Maximum)) - this.GetPropertyValue<decimal>(nameof(Minimum))) / this.GetPropertyValue<decimal>(nameof(Step)));
        Random random = new();
        int step = random.Next(0, nSteps);
        decimal value = this.GetPropertyValue<decimal>(nameof(Minimum)) + this.GetPropertyValue<decimal>(nameof(Step)) * this.GetPropertyValue<decimal>(nameof(Step));
        int nDecimals = Math.Max(step.ToString().Reverse().ToList().IndexOf('.') + 1, 0);
        if (nDecimals > 0)
        {
            value = Math.Round(value, nDecimals);
        }
        return value;

    }

    internal override object PreProcess(object data)
    {
        if (data == null)
        {
            return decimal.MinValue;
        }

        decimal result = decimal.Parse(data.ToString());
        return result;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return decimal.MinValue;
        }

        decimal result = decimal.Parse(data.ToString());
        return result;
    }
}

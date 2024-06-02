using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Slider : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveReleaseEvent
    {
        internal Slider() { }
        internal decimal Step { get; set; }
        internal decimal Minimum { get;  set; }
        internal decimal Maximum { get;  set; }

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
            int nSteps = (int)((Maximum - Minimum) / Step);
            Random random = new Random();
            int step = random.Next(0, nSteps);
            var value = Minimum + Step * Step;
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

            var result = decimal.Parse(data.ToString());
            return result;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null)
            {
                return decimal.MinValue;
            }

            var result = decimal.Parse(data.ToString());
            return result;
        }
    }
}

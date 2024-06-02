using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Number : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSubmitEvent, IHaveFocusEvent
    {
        internal Number() { }
        internal decimal Step { get; set; }
        internal int? Precision { get;  set; }
        internal decimal? Minimum { get;  set; }
        internal decimal? Maximum { get;  set; }

        public static decimal? Payload(object obj)
        {
            if (obj == null)
            {
                return null;
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

        internal override object PreProcess(object data)
        {            
            if (data == null)
            {
                return null;
            }

            var result = decimal.Parse(data.ToString());
            return result;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null)
            {
                return null;
            }

            var result = decimal.Parse(data.ToString());
            return result;
        }
    }
}

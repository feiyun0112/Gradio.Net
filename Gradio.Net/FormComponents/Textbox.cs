using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Textbox : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSelectEvent, IHaveSubmitEvent, IHaveFocusEvent, IHaveBlurEvent
    {
        internal Textbox() { }
        internal int Lines { get;   set; }
        internal int MaxLines { get;   set; }
        internal string Placeholder { get;   set; }
        internal bool Autofocus { get;  set; }
        internal bool Autoscroll { get;  set; }
        internal TextboxType Type { get;  set; }
        internal TextboxTextAlign TextAlign { get;  set; }
        internal bool ShowCopyButton { get;  set; }

        public static string Payload(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string str)
            {
                return str;
            }

            throw new ArgumentException($"Payload Type expect string actual {obj.GetType()}");
        }

        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }

        internal override object PreProcess(object data)
        {
            string result = null;
            if (data == null)
            {
                return result;
            }

            result = data.ToString();
            return result;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null)
            {
                return null;
            }

            return data.ToString();
        }
    }
}

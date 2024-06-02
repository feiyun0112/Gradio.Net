using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class MultimodalTextbox : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSelectEvent, IHaveSubmitEvent, IHaveFocusEvent, IHaveBlurEvent
    {
        internal MultimodalTextbox() { }
        internal object SubmitBtn { get;  set; }
        internal int Lines { get;   set; }
        internal int MaxLines { get;   set; }
        internal string Placeholder { get;   set; }
        internal bool Autofocus { get;  set; }
        internal bool Autoscroll { get;  set; }
        
        internal TextboxTextAlign TextAlign { get;  set; }
        

        public static MultimodalData Payload(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is MultimodalData str)
            {
                return str;
            }

            throw new ArgumentException($"Payload Type expect MultimodalData actual {obj.GetType()}");
        }

        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }

        internal override object PreProcess(object data)
        {
            MultimodalData result = null;
            if (data == null)
            {
                return result;
            }
             
            result = JsonUtils.Deserialize< MultimodalData>( data.ToString());
            return result;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            MultimodalData result = null;
            if (data == null)
            {
                return result;
            }

            result = JsonUtils.Deserialize<MultimodalData>(data.ToString());
            return result;
        }
    }
}

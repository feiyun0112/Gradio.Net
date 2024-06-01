using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gradio.Net
{
    public class Button : Component
    {
        internal Button() { }

        internal string Icon { get;  set; }
        internal ButtonVariant Variant { get;   set; }
        internal ButtonSize? Size { get;   set; }
        internal string Link { get;   set; }


        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }

           }
}

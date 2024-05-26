using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Textbox : FormComponent
    {
        internal int Lines { get;   set; }
        internal int MaxLines { get;   set; }
        internal string Placeholder { get;   set; }
        internal bool Autofocus { get;  set; }
        internal bool Autoscroll { get;  set; }
        internal TextboxType Type { get;  set; }
        internal TextboxTextAlign TextAlign { get;  set; }
        internal bool ShowCopyButton { get;  set; }

        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }

        
    }
}

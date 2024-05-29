using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Tab : Blocks
    {
        internal string Label { get;  set; }
        internal string ComponentId { get;  set; }
        protected override string GetTypeName()
        {
            return "tabitem";  
        }

    }
}

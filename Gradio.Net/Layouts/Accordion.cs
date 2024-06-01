using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Accordion : Blocks
    {
        internal Accordion() { }
        internal string Label { get;  set; }
        internal bool Open { get;  set; }
    }
}

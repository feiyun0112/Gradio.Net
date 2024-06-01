using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Column : Blocks
    {
        internal Column() { }
        internal int Scale { get;  set; }
        internal int MinWidth { get;  set; }
        internal ColumnVariant Variant { get;  set; }
    }
}

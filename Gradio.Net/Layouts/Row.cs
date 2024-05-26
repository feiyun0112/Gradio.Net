using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Row : Blocks
    {
        internal RowVariant Variant { get;  set; }
        internal bool EqualHeight { get; set; } = true;
    }
}

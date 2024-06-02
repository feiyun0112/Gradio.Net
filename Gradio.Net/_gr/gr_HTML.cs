using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static HTML HTML(string value="",
        string label=null,
        decimal? every=null,
        bool showLabel=true,
        bool visible  = true,
        string elemId= null,
        IEnumerable<string> elemClasses=null)
        {
            var block = new HTML() {
                Value= value,
                Label= label,
                Every= every,
                ShowLabel= showLabel,
                Visible= visible,
                ElemId= elemId,
               ElemClasses= elemClasses,
            };
            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

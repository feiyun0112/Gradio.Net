using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Markdown Markdown(string value="",
        string label=null,
        float? every=null,
        bool showLabel=true,
        bool rtl  = false,
        IEnumerable<Dictionary<string,object>> latex_delimiters=null,
        bool visible  = true,
        string elemId= null,
        IEnumerable<string> elemClasses=null,
        bool render = true,
        string key =null,
        bool sanitizeHtml  = true,
        bool lineBreaks=false,
        bool headerLinks=false)
        {
            var block = new Markdown() {
                Value= value,
                Label= label,
                Every= every,
                ShowLabel= showLabel,
                Rtl= rtl,
                LatexDelimiters= latex_delimiters,
                Visible= visible,
                ElemId= elemId,
               ElemClasses= elemClasses,
               Render= render,
                Key= key,
                SanitizeHtml= sanitizeHtml,
                LineBreaks= lineBreaks,
                HeaderLinks= headerLinks,
            };
            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Group Group(            
            bool visible = true,
            string elemId = null,
            IEnumerable<string> elemClasses = null,
            bool render = true
        )
        {
            var block = new Group();

            block.Visible = visible;
            block.ElemId = elemId;
            block.ElemClasses = elemClasses;
            block.Render = render;

            Context.SetCurrentBlocks(block);

            return block;
        }
    }
}

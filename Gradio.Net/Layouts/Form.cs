using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public class Form : Blocks
    {
        internal int MinWidth { get; set; }
        internal int Scale { get; set; }

        public override void Add(Block item)
        {
            var formComponet = item as FormComponent;
            if (this.ParentBlocks is Row row)
            {
                var scale = formComponet.Scale;
                var minWidth = formComponet.MinWidth;
                this.Scale += scale.HasValue ? scale.Value : 1;
                this.MinWidth += minWidth.HasValue ? minWidth.Value : 0;
            }
            else if (this.ParentBlocks is Blocks blocks && blocks.FillHeight)
            {
                var scale = formComponet.Scale;
                this.Scale += scale.HasValue ? scale.Value : 1;
            }

            base.Add(item);
        }

    }
}

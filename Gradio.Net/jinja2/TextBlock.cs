using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.jinja2
{
    internal sealed class TextBlock : Block
    {
        internal TextBlock(string content):base(content) { }

        internal override BlockTypes Type => BlockTypes.Text;

        internal override string Render(Dictionary<string, object> vars)
        {
            return this.Content;
        }
    }
}

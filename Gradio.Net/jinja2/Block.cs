using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.jinja2
{
    internal abstract class Block
    {
        internal Block(string content)
        { 
            Content = content;
        }
        internal virtual BlockTypes Type => BlockTypes.Undefined;

        internal string Content { get; private set; }

        internal abstract string Render(Dictionary<string,object> vars);

        public override string ToString()
        {
            return $"{Type}:{Content}";
        }
    }
}

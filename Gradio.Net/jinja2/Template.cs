using Gradio.Net.jinja2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gradio.Net.jinja2
{
    internal class Template
    {
        private readonly string _source;

        internal Template(string source)
        {
            this._source = source;

        }

        internal string Render(Dictionary<string, object> vars)
        {
            var blocks = new TemplateTokenizer().Tokenize(this._source);
            var result = new StringBuilder();
            foreach ( var block in blocks )
            {
                result.Append(block.Render(vars));
            }
            return result.ToString();
        }
    }
}

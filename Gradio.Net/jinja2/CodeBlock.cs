using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gradio.Net.jinja2
{
    internal sealed class CodeBlock : Block
    {
        internal CodeBlock(string content):base(content) { }

        internal override BlockTypes Type => BlockTypes.Code;

        internal override string Render(Dictionary<string, object> vars)
        {
            //todo: dynamic render

            if (this.Content.Equals("config | toorjson"))
            {
                return JsonSerializer.Serialize(vars["config"]); 
            }
            else if (this.Content.StartsWith("config['"))
            {
                if (vars["config"] is Dictionary<string, object> dictConfig)
                {
                    Regex regex = new Regex(@"config\['(.*?)'\]");
                    var match = regex.Match(this.Content);
                    var key = match.Groups[1].Value;
                    if (dictConfig.ContainsKey(key))
                    {
                        return dictConfig[key].ToString();
                    }

                    return "";
                }
            }
            else if (this.Content.StartsWith("config.get('"))
            {
                if (vars["config"] is Dictionary<string, object> dictConfig)
                {
                    Regex regex = new Regex(@"config.get\('(.*?)', \{\}\)\.get\('(.*?)', '(.*?)'\)");
                    var match = regex.Match(this.Content);
                    var key = match.Groups[1].Value;
                    if (dictConfig.ContainsKey(key))
                    {
                        if (dictConfig[key] is Dictionary<string, object> dictInner)
                        {
                            var innerKey = match.Groups[2].Value;
                            if (dictInner.ContainsKey(innerKey))
                            {
                                return dictInner[innerKey].ToString();
                            }

                            
                        }
                    }

                    return match.Groups[3].Value;
                }
            }

            return this.Content;
        }
    }
}

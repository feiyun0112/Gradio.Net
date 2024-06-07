using System.Text.Json;
using System.Text.RegularExpressions;

namespace Gradio.Net.jinja2;

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
                Regex regex = new(@"config\['(.*?)'\]");
                Match match = regex.Match(this.Content);
                string key = match.Groups[1].Value;
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
                string key = "body_css";
                if (dictConfig.ContainsKey(key))
                {
                    if (dictConfig[key] is Dictionary<string, string> dictInner)
                    {
                        return string.Join(";\r\n", dictInner.Select(d => $"--{d.Key.Replace("--", "").Replace("_", "-")}:{(d.Value.ToString().StartsWith("*") ? $"var({d.Value.ToString().Replace("--", "").Replace("*", "--").Replace("_", "-")})" : $"{d.Value.ToString().Replace("_", "-")}")} !important"));
                    }
                }
                return "";
            }
        }

        return this.Content;
    }
}

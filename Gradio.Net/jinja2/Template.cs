using System.Text;

namespace Gradio.Net.jinja2;

public class Template
{
    private readonly string _source;

    public Template(string source)
    {
        this._source = source;

    }

    public string Render(Dictionary<string, object> vars)
    {
        List<Block> blocks = new TemplateTokenizer().Tokenize(this._source);
        StringBuilder result = new();
        foreach (Block block in blocks )
        {
            result.Append(block.Render(vars));
        }
        return result.ToString();
    }
}

namespace Gradio.Net.jinja2;

internal sealed class TextBlock : Block
{
    internal TextBlock(string content) : base(content) { }

    internal override BlockTypes Type => BlockTypes.Text;

    internal override string Render(Dictionary<string, object> vars)
    {
        return this.Content;
    }
}


namespace Gradio.Net.Core.Themes.Utils;

public class Font
{
    public string Name { get; }

    public Font(string name)
    {
        Name = name;
    }

    public override string ToString()
        => Name is "sans-serif" or "serif" or "monospace" or "cursive" or "fantasy" ? Name : $"'{Name}'";

    public virtual Dictionary<string, string?> Stylesheet() => new() { ["url"] = null, ["css"] = null };
}


namespace Gradio.Net.Core.Themes.Utils;

public class GoogleFont : Font
{
    public IEnumerable<int> Weights { get; }

    public GoogleFont(string name, IEnumerable<int>? weights = null) : base(name)
    {
        Weights = weights ?? new[] { 400, 600 };
    }

    public override Dictionary<string, string?> Stylesheet()
    {
        var joined = string.Join(';', Weights);
        var url = $"https://fonts.googleapis.com/css2?family={Name.Replace(" ", "+")}:wght@{joined}&display=swap";
        return new Dictionary<string, string?> { ["url"] = url, ["css"] = null };
    }
}

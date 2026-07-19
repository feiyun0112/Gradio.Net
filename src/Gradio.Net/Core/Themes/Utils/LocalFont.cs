using System.Text;

namespace Gradio.Net.Core.Themes.Utils;

public class LocalFont : Font
{
    public IEnumerable<int> Weights { get; }

    public LocalFont(string name, IEnumerable<int>? weights = null) : base(name)
    {
        Weights = weights ?? new[] { 400, 700 };
    }

    public override Dictionary<string, string?> Stylesheet()
    {
        var sb = new StringBuilder();
        foreach (var weight in Weights)
        {
            var weightName = weight == 400 ? "Regular" : weight == 700 ? "Bold" : weight.ToString();
            sb.AppendLine($"@font-face {{");
            sb.AppendLine($"  font-family: '{Name}';");
            sb.AppendLine($"  src: url('static/fonts/{Name.Replace(" ", "")}/{Name.Replace(" ", "")}-{weightName}.woff2') format('woff2');");
            sb.AppendLine($"  font-weight: {weightName};");
            sb.AppendLine($"  font-style: normal;");
            sb.AppendLine("}");
        }

        return new Dictionary<string, string?> { ["url"] = null, ["css"] = sb.ToString() };
    }
}

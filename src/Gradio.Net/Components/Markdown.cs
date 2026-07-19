
namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("copy")]
public class Markdown : Component
{
    public bool Rtl { get; set; }
    public List<Dictionary<string, object>> LatexDelimiters { get; set; }
    public bool SanitizeHtml { get; set; }
    public bool LineBreaks { get; set; }
    public bool HeaderLinks { get; set; }
    public object? Height { get; set; }
    public object? MaxHeight { get; set; }
    public object? MinHeight { get; set; }
    public List<string>? Buttons { get; set; }
    public bool Padding { get; set; }

    public Markdown(
        object? value = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool rtl = false,
        List<Dictionary<string, object>>? latexDelimiters = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool sanitizeHtml = true,
        bool lineBreaks = false,
        bool headerLinks = false,
        object? height = null,
        object? maxHeight = null,
        object? minHeight = null,
        List<string>? buttons = null,
        bool container = false,
        bool padding = false)
    {
        Rtl = rtl;
        LatexDelimiters = latexDelimiters ?? new List<Dictionary<string, object>>
        {
            new()
            {
                ["left"] = "$$",
                ["right"] = "$$",
                ["display"] = true
            }
        };
        SanitizeHtml = sanitizeHtml;
        LineBreaks = lineBreaks;
        HeaderLinks = headerLinks;
        Height = height;
        MaxHeight = maxHeight;
        MinHeight = minHeight;
        Buttons = buttons;
        Padding = padding;

        Value = value is string s ? CleanDoc(s) : value;
        Label = label;
        ShowLabel = showLabel ?? false;
        Container = container;
        MinWidth = null;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "markdown";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config.Remove("min_width");
        config["rtl"] = Rtl;
        config["latex_delimiters"] = LatexDelimiters;
        config["sanitize_html"] = SanitizeHtml;
        config["line_breaks"] = LineBreaks;
        config["header_links"] = HeaderLinks;
        config["height"] = Height;
        config["max_height"] = MaxHeight;
        config["min_height"] = MinHeight;
        config["buttons"] = Buttons;
        config["padding"] = Padding;
        return config;
    }

    public override object? Preprocess(object? input)
    {
        return input?.ToString();
    }

    public override object? Postprocess(object? output)
    {
        if (output == null)
        {
            return null;
        }

        return CleanDoc(output.ToString() ?? string.Empty);
    }

    public override object ExamplePayload() => "# Hello!";

    public override object ExampleValue() => "# Hello!";

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object> { ["type"] = "string" };
    }

    private static string CleanDoc(string text)
    {
        var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var lines = normalized.Split('\n').ToList();

        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines.RemoveAt(lines.Count - 1);

        if (lines.Count == 0) return string.Empty;

        var indents = lines
            .Where(l => l.Trim().Length > 0)
            .Select(l => l.TakeWhile(ch => ch == ' ' || ch == '\t').Count())
            .ToList();

        var commonIndent = indents.Count == 0 ? 0 : indents.Min();
        if (commonIndent > 0)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Length >= commonIndent)
                {
                    lines[i] = line.Substring(commonIndent);
                }
            }
        }

        return string.Join("\n", lines);
    }
}

using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("focus")]
[Events.Event("blur")]
public class Code : Component
{
    public static readonly List<string?> Languages = new()
    {
        "python",
        "c",
        "cpp",
        "markdown",
        "latex",
        "json",
        "html",
        "css",
        "javascript",
        "jinja2",
        "typescript",
        "yaml",
        "dockerfile",
        "shell",
        "r",
        "sql",
        "sql-msSQL",
        "sql-mySQL",
        "sql-mariaDB",
        "sql-sqlite",
        "sql-cassandra",
        "sql-plSQL",
        "sql-hive",
        "sql-pgSQL",
        "sql-gql",
        "sql-gpSQL",
        "sql-sparkSQL",
        "sql-esper",
        null
    };

    public string? Language { get; set; }
    public int Lines { get; set; }
    public int? MaxLines { get; set; }
    public bool WrapLines { get; set; }
    public bool ShowLineNumbers { get; set; }
    public bool Autocomplete { get; set; }

    public Code(
        object? value = null,
        string? language = null,
        object? every = null,
        object? inputs = null,
        int lines = 5,
        int? maxLines = null,
        string? label = null,
        bool? interactive = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool wrapLines = false,
        bool showLineNumbers = true,
        bool autocomplete = false)
    {
        if (!Languages.Contains(language))
        {
            throw new Error($"Language {language} not supported.");
        }

        Value = value;
        Language = language;
        Lines = lines;
        MaxLines = maxLines.HasValue ? Math.Max(lines, maxLines.Value) : null;
        WrapLines = wrapLines;
        ShowLineNumbers = showLineNumbers;
        Autocomplete = autocomplete;

        Label = label;
        Interactive = interactive;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
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

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["language"] = Language;
        config["lines"] = Lines;
        config["max_lines"] = MaxLines;
        config["wrap_lines"] = WrapLines;
        config["show_line_numbers"] = ShowLineNumbers;
        config["autocomplete"] = Autocomplete;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        return payload?.ToString();
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is Tuple<object, object> || value.GetType().IsValueType && value.GetType().FullName?.StartsWith("System.ValueTuple", StringComparison.Ordinal) == true)
        {
            throw new Error(
                "Code component does not support returning files as tuples anymore. Please read the file contents and return as str instead.");
        }

        return value.ToString()?.Trim();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object> { ["type"] = "string" };
    }

    public override object ExamplePayload() => "print('Hello World')";

    public override object ExampleValue() => "print('Hello World')";

    public override object ProcessExample(object value)
    {
        return base.ProcessExample(value);
    }
}

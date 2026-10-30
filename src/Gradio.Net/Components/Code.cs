using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A code editor: text with a language, line numbers and the option to wrap long lines
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is the code as text, and what the component
/// adds to a textbox is how that text is shown: the language it is highlighted as, whether line
/// numbers are drawn, whether long lines wrap, and how tall the editor is.
/// </para>
/// <para>
/// The editor is not a form component, which is why a page holding one does not group it into a
/// form with the fields around it: the Python version's code editor is meant to stand on its own.
/// </para>
/// </remarks>
[GradioBlock("code")]
[HandlerValue("string", "change", "input", "focus", "blur")]
public sealed partial class Code : Component
{
    internal Code()
    {
    }

    /// <summary>The languages the Python version's editor highlights.</summary>
    private static readonly IReadOnlyList<string> _languages =
    [
        "python", "c", "cpp", "markdown", "latex", "json", "html", "css", "javascript",
        "jinja2", "typescript", "yaml", "dockerfile", "shell", "r", "sql", "sql-msSQL",
        "sql-mySQL", "sql-mariaDB", "sql-sqlite", "sql-cassandra", "sql-plSQL", "sql-hive",
        "sql-pgSQL", "sql-gql", "sql-gpSQL", "sql-sparkSQL", "sql-esper",
    ];

    private static readonly IReadOnlyList<string> _defaultButtons = ["copy", "download"];

    private int? _maxLines;
    private string? _value;
    private string? _language;

    /// <summary>The code the editor starts with, or <see langword="null"/> for none.</summary>
    /// <value>The code without the whitespace around it, which is what the Python version
    /// carries.</value>
    public string? Value
    {
        get => _value?.Trim();
        set => _value = value;
    }

    /// <summary>The language the code is highlighted as, or <see langword="null"/> for
    /// none.</summary> <exception cref="ArgumentException">The language is not one the Python
    /// version's editor knows.</exception>
    public string? Language
    {
        get => _language;
        set
        {
            if (value is not null && !_languages.Contains(value, StringComparer.Ordinal))
            {
                throw new ArgumentException(
                    $"Language {value} not supported.",
                    nameof(value));
            }

            _language = value;
        }
    }

    /// <summary>How many lines tall the editor is drawn.</summary>
    public int Lines { get; set; } = 5;

    /// <summary>
    /// How many lines the editor grows to at most, or <see langword="null"/> to fill its container.
    /// </summary>
    /// <value>The larger of <see cref="Lines"/> and the number the application gave.</value>
    public int? MaxLines
    {
        get => _maxLines is null ? null : Math.Max(Lines, _maxLines.Value);
        set => _maxLines = value;
    }

    /// <summary>The label drawn above the editor.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the editor next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the editor wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether long lines wrap instead of scrolling sideways.</summary>
    public bool WrapLines { get; set; }

    /// <summary>Whether line numbers are drawn.</summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>Whether the editor suggests completions.</summary>
    public bool Autocomplete { get; set; }

    /// <summary>The buttons drawn in the editor's toolbar.</summary>
    /// <value><c>copy</c> and <c>download</c> unless the application names others.</value>
    public IReadOnlyList<string> Buttons { get; set; } = _defaultButtons;

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("print('Hello World')");

    /// <summary>The code a handler reads.</summary>
    /// <param name="value">The code the client sent.</param>
    /// <returns>The code, or <see langword="null"/>.</returns>
    public override object? Preprocess(JsonNode? value) =>
        value is null ? null : JsonValues.Text(value) ?? value.ToString();

    /// <summary>The code the client receives from what a handler returned.</summary>
    /// <param name="value">The code.</param>
    /// <returns>The code without the whitespace around it, or <see langword="null"/>.</returns>
    public override JsonNode? Postprocess(object? value) =>
        value is null ? null : JsonValue.Create(value.ToString()?.Trim());
}

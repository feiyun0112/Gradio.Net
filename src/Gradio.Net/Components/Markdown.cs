using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Markdown rendered as HTML, with math, code highlighting and inline HTML
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a string of Markdown the frontend renders:
/// KaTeX for the math, highlight.js for the code, and sanitization for the inline HTML — all of
/// which the vendored frontend does, so nothing here interprets it.
/// </para>
/// <para>
/// What the frontend receives is the <em>cleaned</em> text: the Python version runs
/// <c>inspect.cleandoc</c> on the value before it reaches the config, which strips common
/// indentation and surrounding blank lines. A handler that writes an indented literal, or a
/// template that starts with a newline, would otherwise send the indentation to the frontend as
/// part of the text.
/// </para>
/// </remarks>
[GradioBlock("markdown")]
[HandlerValue("string?", "change", "copy")]
public sealed partial class Markdown : Component
{
    private string? _value;
    private IReadOnlyList<LatexDelimiter> _latexDelimiters = [new LatexDelimiter()];

    /// <summary>Declares a Markdown component with no content of its own.</summary>
    /// <remarks>
    /// A constructor rather than an initializer because two of the defaults are not the component
    /// base's: the Python version draws Markdown outside a container, which is also why it shows no
    /// label unless the application asks for one.
    /// </remarks>
    internal Markdown() => Container = false;

    /// <summary>The Markdown the component starts with, or <see langword="null"/> for none.</summary>
    /// <value>
    /// The text cleaned the way the Python version cleans it — <c>inspect.cleandoc</c>, with tabs
    /// expanded to the next tab stop and common indentation stripped — so the same text has to
    /// leave this framework the same way the same text leaves the Python version.
    /// </value>
    public string? Value
    {
        get => _value is null ? null : ChatText.Cleaned(_value);
        set => _value = value;
    }

    /// <summary>The label drawn above the rendered text.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; }

    /// <summary>Whether the text runs right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>The delimiters LaTeX is rendered between.</summary>
    /// <value>
    /// One <c>$$</c> delimiter unless the application says otherwise; an empty list turns LaTeX
    /// rendering off.
    /// </value>
    public IReadOnlyList<LatexDelimiter> LatexDelimiters
    {
        get => _latexDelimiters;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _latexDelimiters = value;
        }
    }

    /// <summary>Whether HTML in the Markdown is sanitized before it is rendered.</summary>
    /// <value><see langword="true"/> unless the application says otherwise.</value>
    public bool SanitizeHtml { get; set; } = true;

    /// <summary>Whether a single newline in the Markdown ends a line.</summary>
    /// <value>
    /// <see langword="false"/> unless the application says otherwise: the Python version ignores a
    /// single new line unless asked to treat it as a break.
    /// </value>
    public bool LineBreaks { get; set; }

    /// <summary>Whether headings carry anchor links the reader may copy.</summary>
    /// <value><see langword="false"/> unless the application says otherwise.</value>
    public bool HeaderLinks { get; set; }

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The tallest the component is drawn, in pixels or as a CSS length.</summary>
    public object? MaxHeight { get; set; }

    /// <summary>The shortest the component is drawn, in pixels or as a CSS length.</summary>
    public object? MinHeight { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <value>
    /// <see langword="null"/> unless the application asks for one: the Python version writes no
    /// list at all for a component with no buttons.
    /// </value>
    public IReadOnlyList<string>? Buttons { get; set; }

    /// <summary>Whether the component is drawn with padding of its own.</summary>
    public bool Padding { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("# Hello!");

    /// <summary>
    /// The text a handler reads.
    /// </summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The text as the JSON it is, since Markdown makes no claim about its shape beyond being text.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);

    /// <summary>
    /// The text the client receives from what a handler returned.
    /// </summary>
    /// <param name="value">A string, or <see langword="null"/> for none.</param>
    /// <returns>The cleaned text, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// The Python version runs <c>inspect.cleandoc</c> on the value before it reaches the config,
    /// so the same text has to leave this framework the same way — the frontend renders the
    /// difference.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        string text => JsonValue.Create(ChatText.Cleaned(text)),
        _ => JsonValues.FromObject(value),
    };
}

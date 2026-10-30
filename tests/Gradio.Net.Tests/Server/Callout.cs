using System.Text.Json.Nodes;
using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A custom HTML component an application defines for itself: a tinted box with a heading and a
/// body.
/// </summary>
/// <remarks>
/// <para>
/// This is the extension point the framework offers an application that wants a component of its
/// own without writing a frontend: subclass <see cref="Html"/>, give it a template, a stylesheet
/// and a little JavaScript, and add the parameters the template reads. It is written here the way
/// an application writes one — in the application's own project, not the framework's — so that the
/// manifest page proves a component defined this way is described the way the Python version
/// describes the same class.
/// </para>
/// <para>
/// Two things make it a component of its own rather than an HTML component with different
/// templates. Its parameters are written beside the HTML component's, which is where the Python
/// version puts a subclass's own constructor parameters; and they are written into the extra-props
/// dictionary as well, because that is what the template reads (<c>${tone}</c>, <c>${heading}</c>).
/// The config's <c>type</c> stays <c>html</c> — the frontend draws every one of these with the same
/// component — while <c>name</c> and <c>component_class_name</c> name the class.
/// </para>
/// <para>
/// The templates are written as concatenated literals rather than as a raw string so that the
/// leading newline and the trailing indentation the Python version's triple-quoted strings carry
/// are visible here: the frontend runs these strings, so a difference in whitespace is a difference
/// in what a user sees.
/// </para>
/// </remarks>
internal sealed class Callout : Html
{
    /// <summary>Declares the component with its templates.</summary>
    public Callout()
    {
        HtmlTemplate = HtmlTemplateSource;
        CssTemplate = CssTemplateSource;
        JsOnLoad = JsOnLoadSource;
    }

    /// <summary>How the box is tinted: <c>info</c>, <c>warning</c>, or a name the stylesheet knows.</summary>
    public string Tone { get; set; } = "info";

    /// <summary>The heading drawn above the body, or empty for none.</summary>
    public string Heading { get; set; } = string.Empty;

    /// <summary>The two parameters the template reads, as a custom component's parameters are carried.</summary>
    public override JsonObject? ExtraProperties => new()
    {
        ["tone"] = Tone,
        ["heading"] = Heading,
    };

    /// <summary>The props this component writes, in the subclass's parameter order.</summary>
    /// <param name="props">The props being built.</param>
    /// <remarks>
    /// The Python version writes a subclass's own constructor parameters first, in the order the
    /// subclass declares them, and then the HTML component's — which is what writing them before
    /// the base does. The base writes <c>value</c> again, which replaces the key where it already
    /// is rather than moving it, so the order stays the Python version's.
    /// </remarks>
    protected internal override void WriteProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("value", Value);
        props.Add("tone", Tone);
        props.Add("heading", Heading);
        base.WriteProps(props);
    }

    /// <summary>The template that draws the box.</summary>
    private const string HtmlTemplateSource =
        "\n"
        + "        <div class=\"callout callout-${tone}\">\n"
        + "            ${heading ? `<h4 class=\"callout-heading\">${heading}</h4>` : ''}\n"
        + "            <div class=\"callout-body\">${value}</div>\n"
        + "        </div>\n"
        + "        ";

    /// <summary>The stylesheet that tints the box.</summary>
    private const string CssTemplateSource =
        "\n"
        + "        .callout { border-left: 4px solid var(--color-accent); padding: var(--spacing-md); }\n"
        + "        .callout-warning { border-left-color: #f59e0b; }\n"
        + "        .callout-heading { margin: 0 0 var(--spacing-sm); font-weight: 600; }\n"
        + "        ";

    /// <summary>The JavaScript that raises the component's click event when a link in the body is clicked.</summary>
    private const string JsOnLoadSource =
        "\n"
        + "        element.querySelectorAll('.callout-body a').forEach(link => {\n"
        + "            link.addEventListener('click', () => trigger('click'));\n"
        + "        });\n"
        + "        ";
}

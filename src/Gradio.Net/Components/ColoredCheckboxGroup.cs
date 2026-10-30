using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A set of tickable options, each drawn in a colour of the application's choosing
/// </summary>
/// <remarks>
/// <para>
/// The Python version builds this component out of its custom-HTML one rather than out of its
/// checkbox group: the template draws the boxes, the stylesheet colours them, and the JavaScript
/// keeps the component's value in step with which boxes are ticked. The parameters are therefore
/// the HTML component's plus three of its own — the options, the colours and the value — and they
/// travel in the config's extra-props dictionary, which is where a custom component's own
/// parameters go.
/// </para>
/// <para>
/// The templates are the Python version's, character for character. They are not configuration: the
/// frontend runs them, so a difference in whitespace would be a difference in what a user sees.
/// </para>
/// </remarks>
[GradioBlock("html")]
public sealed partial class ColoredCheckboxGroup : Html
{
    private object? _value;

    /// <summary>Declares the component with its templates.</summary>
    internal ColoredCheckboxGroup()
    {
        HtmlTemplate = HtmlTemplateSource;
        CssTemplate = CssTemplateSource;
        JsOnLoad = JsOnLoadSource;
    }

    /// <summary>The options offered, by their text.</summary>
    public IReadOnlyList<string> Choices { get; set; } = [];

    /// <summary>The colours the options are drawn in, one per option.</summary>
    public IReadOnlyList<string> Colors { get; set; } = [];

    /// <summary>The options ticked when the page loads.</summary> <value>The options ticked, always
    /// as a list: the Python version answers an empty list for none.</value>
    public override object? Value
    {
        get => _value ?? Array.Empty<string>();
        set => _value = value switch
        {
            null => null,
            IReadOnlyList<string> options => options,
            string option => new[] { option },
            _ => throw new ArgumentException(
                $"A coloured checkbox group's value is a list of options; got a {value.GetType()}.",
                nameof(value)),
        };
    }

    /// <summary>The options and the colours, as a custom component's parameters are carried.</summary>
    public override JsonObject? ExtraProperties => new()
    {
        ["choices"] = Strings(Choices),
        ["colors"] = Strings(Colors),
    };

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["items"] = new JsonObject
        {
            ["enum"] = Strings(Choices),
            ["type"] = "string",
        },
        ["title"] = "Checkbox Group",
        ["type"] = "array",
    };

    /// <summary>The template that draws the boxes.</summary>
    private const string HtmlTemplateSource =
        "\n"
        + "        <div class=\"colored-checkbox-container\">\n"
        + "            ${label ? `<label class=\"container-label\">${label}</label>` : ''}\n"
        + "            <div class=\"colored-checkbox-group\">\n"
        + "                ${choices.map((choice, i) => `\n"
        + "                    <label class=\"checkbox-label\" data-color-index=\"${i}\">\n"
        + "                        <input type=\"checkbox\" value=\"${choice}\" ${(value || []).includes(choice) ? 'checked' : ''}>\n"
        + "                        ${choice}\n"
        + "                    </label>\n"
        + "                `).join('')}\n"
        + "            </div>\n"
        + "        </div>\n"
        + "        ";

    /// <summary>The stylesheet that colours the boxes.</summary>
    private const string CssTemplateSource =
        "\n"
        + "        .colored-checkbox-container {\n"
        + "            border: 1px solid var(--border-color-primary);\n"
        + "            border-radius: var(--radius-lg);\n"
        + "            padding: var(--spacing-lg);\n"
        + "        }\n"
        + "        .container-label { display: block; margin-bottom: var(--spacing-md); }\n"
        + "        .colored-checkbox-group { display: flex; flex-direction: column; gap: 6px; }\n"
        + "        .checkbox-label { display: flex; align-items: center; cursor: pointer; }\n"
        + "        .checkbox-label input { margin-right: 8px; }\n"
        + "        ${choices.map((choice, i) => `.checkbox-label[data-color-index=\"${i}\"] { color: ${colors[i]}; }`).join(' ')}\n"
        + "        ";

    /// <summary>The JavaScript that keeps the value in step with the boxes.</summary>
    private const string JsOnLoadSource =
        "\n"
        + "        const checkboxes = element.querySelectorAll('input[type=\"checkbox\"]');\n"
        + "        checkboxes.forEach(checkbox => {\n"
        + "            checkbox.addEventListener('change', () => {\n"
        + "                props.value = Array.from(checkboxes)\n"
        + "                    .filter(cb => cb.checked)\n"
        + "                    .map(cb => cb.value);\n"
        + "            });\n"
        + "        });\n"
        + "        ";

    /// <summary>A list of strings as a JSON array.</summary>
    /// <param name="items">The strings.</param>
    /// <returns>A new array.</returns>
    private static JsonArray Strings(IReadOnlyList<string> items)
    {
        var array = new JsonArray();

        foreach (var item in items)
        {
            array.Add(item);
        }

        return array;
    }
}

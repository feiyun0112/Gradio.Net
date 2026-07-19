
namespace Gradio.Net.Components;

public class ColoredCheckboxGroup : Html
{
    public List<string> Choices { get; set; }
    public List<string> Colors { get; set; }

    public ColoredCheckboxGroup(
        List<string> choices,
        List<string> colors,
        List<string>? value = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool showLabel = false,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
        : base(
            value: value ?? new List<string>(),
            label: label,
            htmlTemplate: HtmlTemplateValue,
            cssTemplate: CssTemplateValue,
            jsOnLoad: JsOnLoadValue,
            applyDefaultCss: false,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            props: new Dictionary<string, object>
            {
                ["choices"] = choices,
                ["colors"] = colors,
                ["label"] = label ?? string.Empty
            })
    {
        Choices = choices;
        Colors = colors;
    }

    public override string GetBlockName() => "colored_checkbox_group";

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["items"] = new Dictionary<string, object>
            {
                ["enum"] = Choices,
                ["type"] = "string"
            },
            ["title"] = "Checkbox Group",
            ["type"] = "array"
        };
    }

    public override object ExamplePayload() => new List<string>();

    public override object ExampleValue() => new List<string>();

    private const string HtmlTemplateValue = @"
        <div class=""colored-checkbox-container"">
            ${label ? `<label class=""container-label"">${label}</label>` : ''}
            <div class=""colored-checkbox-group"">
                ${choices.map((choice, i) => `
                    <label class=""checkbox-label"" data-color-index=""${i}"">
                        <input type=""checkbox"" value=""${choice}"" ${(value || []).includes(choice) ? 'checked' : ''}>
                        ${choice}
                    </label>
                `).join('')}
            </div>
        </div>
        ";

    private const string CssTemplateValue = @"
        .colored-checkbox-container {
            border: 1px solid var(--border-color-primary);
            border-radius: var(--radius-lg);
            padding: var(--spacing-lg);
        }
        .container-label { display: block; margin-bottom: var(--spacing-md); }
        .colored-checkbox-group { display: flex; flex-direction: column; gap: 6px; }
        .checkbox-label { display: flex; align-items: center; cursor: pointer; }
        .checkbox-label input { margin-right: 8px; }
        ${choices.map((choice, i) => `.checkbox-label[data-color-index=""${i}""] { color: ${colors[i]}; }`).join(' ')}
        ";

    private const string JsOnLoadValue = @"
        const checkboxes = element.querySelectorAll('input[type=""checkbox""]');
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', () => {
                props.value = Array.from(checkboxes)
                    .filter(cb => cb.checked)
                    .map(cb => cb.value);
            });
        });
        ";
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A set of tickable options of which any number may be chosen.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. A choice is a label and a value (see
/// <see cref="Choice"/>), and what the component carries is the list of the values that are ticked
/// — never the labels, which belong to the screen rather than to the handler.
/// </para>
/// <para>
/// <see cref="Type"/> decides whether a handler reads the values or their positions. The choice is
/// the Python version's, and it changes what <see cref="Preprocess"/> answers rather than what the
/// config says: the frontend always sends values, and the conversion happens on the way in.
/// </para>
/// </remarks>
[GradioBlock("checkboxgroup")]
[HandlerValue("IReadOnlyList<object?>", "change", "input")]
public sealed partial class CheckboxGroup : FormComponent
{
    internal CheckboxGroup()
    {
    }

    private object? _value;
    private string _type = "value";

    /// <summary>The options offered.</summary>
    public IReadOnlyList<Choice> Choices { get; set; } = [];

    /// <summary>The options ticked when the page loads.</summary>
    /// <value>
    /// The values, always as a list: the Python version accepts a single value as shorthand for a
    /// list of one and answers an empty list for none, which is what the config carries.
    /// </value>
    public object? Value
    {
        get => Choice.AsList(_value);
        set => _value = value;
    }

    /// <summary>What the component hands a handler: the values ticked, or their positions.</summary>
    /// <value><c>value</c> or <c>index</c>; anything else is refused.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("value" or "index"))
            {
                throw new ArgumentException(
                    $"A checkbox group's type must be value or index; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the options.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>Whether the group offers a box that ticks every option at once.</summary>
    public bool ShowSelectAll { get; set; }

    /// <summary>The relative width of the group next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the group wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn beside the group.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["items"] = new JsonObject
        {
            ["enum"] = Choice.EnumOf(Choices),
            ["type"] = "string",
        },
        ["title"] = "Checkbox Group",
        ["type"] = "array",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs =>
        Choices.Count == 0 ? null : new JsonArray(JsonValues.FromObject(Choices[0].Value));

    /// <summary>The values a handler reads.</summary>
    /// <param name="value">The values the client sent.</param>
    /// <returns>The values, or their positions when <see cref="Type"/> is <c>index</c>.</returns>
    /// <exception cref="ArgumentException">A value is not one of the choices.</exception>
    /// <remarks>
    /// The refusal is the Python version's, down to the spelling — see <see cref="Choice.Refuse"/>,
    /// which is where the message lives. What this method adds is that the group reads the JSON
    /// rather than the converted values: an int and a float are the same number here and two
    /// different types to the Python version, and the message says which one arrived.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        // Nothing sent is nothing ticked, which is what an empty list would say and what the Python version's
        // frontend sends for a group nobody has touched.
        if (value is null)
        {
            return Array.Empty<object?>();
        }

        var sent = value is JsonArray list ? [.. list] : new List<JsonNode?> { value };
        var ticked = new List<object?>(sent.Count);

        foreach (var entry in sent)
        {
            Choice.Refuse(entry, Choices);
            ticked.Add(JsonValues.ToObject(entry));
        }

        return Type == "value"
            ? ticked
            : ticked.Select(entry => (object?)Choice.IndexOf(Choices, entry)).ToArray();
    }

    /// <summary>The values the client receives from what a handler returned.</summary>
    /// <param name="value">The values, or a single one.</param>
    /// <returns>The values as a list.</returns>
    public override JsonNode? Postprocess(object? value) =>
        JsonValues.FromObject(Choice.AsList(value));

}

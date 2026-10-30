using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A set of options of which exactly one may be chosen.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a single choice's value, and an absent one
/// stays absent rather than becoming the first option: the Python version's radio starts with
/// nothing selected, and a config that filled that in would make a required answer look answered.
/// </para>
/// <para>
/// <see cref="Type"/> is the checkbox group's: <c>value</c> hands the handler the chosen value,
/// <c>index</c> its position.
/// </para>
/// </remarks>
[GradioBlock("radio")]
[HandlerValue("object?", "change", "input")]
public sealed partial class Radio : FormComponent
{
    internal Radio()
    {
    }

    private string _type = "value";

    /// <summary>The options offered.</summary>
    public IReadOnlyList<Choice> Choices { get; set; } = [];

    /// <summary>The option chosen when the page loads, or <see langword="null"/> for none.</summary>
    public object? Value { get; set; }

    /// <summary>What the component hands a handler: the chosen value, or its position.</summary>
    /// <value><c>value</c> or <c>index</c>; anything else is refused.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("value" or "index"))
            {
                throw new ArgumentException(
                    $"A radio's type must be value or index; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The label drawn above the options.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the group next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the group wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>Whether the options run right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>The buttons drawn beside the group.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["enum"] = Choice.EnumOf(Choices),
        ["title"] = "Radio",
        ["type"] = "string",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs =>
        Choices.Count == 0 ? null : JsonValues.FromObject(Choices[0].Value);

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">The value the client sent.</param>
    /// <returns>The value, or its position when <see cref="Type"/> is <c>index</c>.</returns>
    /// <exception cref="ArgumentException">The value is not one of the choices.</exception>
    /// <remarks>
    /// The refusal is the Python version's, down to the spelling — see <see cref="Choice.Refuse"/>,
    /// which is where the message lives.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        var chosen = JsonValues.ToObject(value);

        Choice.Refuse(value, Choices);

        return Type == "value" ? chosen : Choice.IndexOf(Choices, chosen);
    }
}

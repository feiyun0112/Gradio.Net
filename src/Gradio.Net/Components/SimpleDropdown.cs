using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A dropdown with nothing but the essentials: a list of options and the one selected
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. What it leaves out is everything the full dropdown
/// offers on top — several selections, custom values, filtering, a limit on how many are shown —
/// which is the whole reason the Python version has two of them.
/// </para>
/// <para>
/// Its value is a single one, and an absent value stays absent rather than falling back to the first
/// option: the simple dropdown starts with nothing selected.
/// </para>
/// </remarks>
[GradioBlock("simpledropdown")]
[HandlerValue("object?", "change", "input")]
public sealed partial class SimpleDropdown : FormComponent
{
    internal SimpleDropdown()
    {
    }

    /// <summary>The options offered.</summary>
    public IReadOnlyList<Choice> Choices { get; set; } = [];

    /// <summary>The option selected when the page loads, or <see langword="null"/> for none.</summary>
    public object? Value { get; set; }

    /// <summary>The label drawn above the list.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the list next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the list wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = "string",
        ["enum"] = Choice.EnumOf(Choices),
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs =>
        Choices.Count == 0 ? null : JsonValues.FromObject(Choices[0].Value);

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The value, or <see langword="null"/>.</returns>
    /// <remarks>
    /// The Python version warns when the value is not one of the choices and serves it anyway: a
    /// handler may legitimately answer with a value the list does not offer, and refusing it would
    /// turn a warning into an error the application cannot see coming.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        value is null ? null : JsonValues.FromObject(value);
}

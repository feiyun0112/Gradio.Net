using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>A colour, carried as the hex string a browser understands.</summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a string and stays one: the Python version
/// neither parses the colour nor validates its spelling, so <c>#ff0000</c>, <c>red</c> and a value
/// that is neither are all carried as they were given.
/// </para>
/// <para>
/// The Python version's colour picker is not a form component, so a page holding one does not group
/// it into a form with the fields around it — which is worth saying because it looks like a field.
/// </para>
/// </remarks>
[GradioBlock("colorpicker")]
[HandlerValue("string", "change", "input", "release", "submit", "focus", "blur")]
public sealed partial class ColorPicker : Component
{
    internal ColorPicker()
    {
    }

    /// <summary>The colour the picker starts on, or <see langword="null"/> for none.</summary>
    public string? Value { get; set; }

    /// <summary>The label drawn above the picker.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the picker next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the picker wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("#000000");

    /// <summary>The colour a handler reads.</summary>
    /// <param name="value">The colour the client sent.</param>
    /// <returns>The colour as text, or <see langword="null"/>.</returns>
    public override object? Preprocess(JsonNode? value) =>
        value is null ? null : JsonValues.Text(value) ?? value.ToString();

    /// <summary>The colour the client receives from what a handler returned.</summary>
    /// <param name="value">The colour.</param>
    /// <returns>The colour as text, or <see langword="null"/>.</returns>
    public override JsonNode? Postprocess(object? value) =>
        value is null ? null : JsonValue.Create(value.ToString());
}

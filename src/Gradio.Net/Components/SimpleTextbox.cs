using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A text field with nothing but the essentials: a value, a hint, a label and a direction
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The simple textbox is a form component like the full
/// one, so it is grouped into a form with its neighbours; what it leaves out is everything the full
/// textbox offers on top — the field type, the line count, the buttons and the length limit — which
/// is the whole reason the Python version has two of them.
/// </para>
/// <para>
/// A value is text, and an absent value stays absent: the Python version passes
/// <see langword="null"/> through rather than turning it into an empty string, so a handler can
/// tell "nothing was sent" from "an empty field was sent".
/// </para>
/// </remarks>
[GradioBlock("simpletextbox")]
[HandlerValue("string", "change", "input", "submit")]
public sealed partial class SimpleTextbox : FormComponent
{
    internal SimpleTextbox()
    {
    }

    /// <summary>The text the component starts with, or <see langword="null"/> for an empty one.</summary>
    public string? Value { get; set; }

    /// <summary>The hint shown while the field is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The label drawn above the field.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the field next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the field wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>Whether the text runs right to left.</summary>
    public bool Rtl { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("Hello!!");

    /// <summary>The text a handler reads.</summary>
    /// <param name="value">The text the client sent.</param>
    /// <returns>The text, or <see langword="null"/> when nothing was sent.</returns>
    /// <remarks>
    /// The Python version coerces whatever arrives into a string, so a client that sent a number is
    /// read as its text rather than refused. Nothing is turned into an empty string: an absent
    /// value stays absent.
    /// </remarks>
    public override object? Preprocess(JsonNode? value) =>
        value is null ? null : JsonValues.Text(value) ?? value.ToString();

    /// <summary>The text the client receives from what a handler returned.</summary>
    /// <param name="value">The value the handler returned.</param>
    /// <returns>The text, or <see langword="null"/>.</returns>
    public override JsonNode? Postprocess(object? value) =>
        value is null ? null : JsonValue.Create(value.ToString());
}

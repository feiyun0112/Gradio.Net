using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>A box the user ticks, carrying a boolean.</summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is never absent — an unticked box is
/// <see langword="false"/> rather than nothing — which is why it is a <see cref="bool"/> and not a
/// nullable one.
/// </para>
/// <para>
/// A checkbox is the one component whose label turns itself on: the Python version's own config
/// hook sets <c>show_label</c> when a label was given, so an application that asked for a labelled
/// box without a label has its request corrected rather than obeyed. That is why
/// <see cref="ShowLabel"/> is overridden here.
/// </para>
/// </remarks>
[GradioBlock("checkbox")]
[HandlerValue("bool", "change", "input")]
public sealed partial class Checkbox : FormComponent
{
    internal Checkbox()
    {
    }

    /// <summary>Whether the box starts ticked.</summary>
    public bool Value { get; set; }

    /// <summary>The label drawn beside the box.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>
    /// Whether the label is drawn.
    /// </summary>
    /// <value>
    /// Whether a label was given, or whatever the base says: the Python version's checkbox hook
    /// shows the label whenever there is one, because a labelled box with its label hidden shows
    /// nothing but an unexplained tick.
    /// </value>
    public override bool ShowLabel
    {
        get => Label is not null || base.ShowLabel;
        set => base.ShowLabel = value;
    }

    /// <summary>The relative width of the box next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the box wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn beside the box.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "boolean" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create(true);

    /// <summary>The flag a handler reads.</summary>
    /// <param name="value">Whether the box was ticked.</param>
    /// <returns>The flag, or <see langword="null"/> when nothing was sent.</returns>
    public override object? Preprocess(JsonNode? value) =>
        value is null ? null : JsonValues.Flag(value);

    /// <summary>The flag the client receives from what a handler returned.</summary>
    /// <param name="value">The value the handler returned.</param> <returns>The flag, read the way
    /// the Python version reads one: anything empty or zero is unticked.</returns>
    public override JsonNode? Postprocess(object? value) => JsonValue.Create(value switch
    {
        null => false,
        bool flag => flag,
        int number => number != 0,
        long number => number != 0,
        double number => number != 0,
        string text => text.Length > 0,
        _ => true,
    });
}

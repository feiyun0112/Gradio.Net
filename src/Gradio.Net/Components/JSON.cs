using System.Text.Json;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Arbitrary JSON, shown as a collapsible tree.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is JSON itself rather than a string: an
/// application hands over an object, an array, or a string that already holds JSON, and what the
/// frontend draws is the parsed document.
/// </para>
/// <para>
/// The component is an output in practice — it has no editor — which is why its schema says only
/// that the value is valid JSON.
/// </para>
/// </remarks>
[GradioBlock("json")]
[HandlerValue("object?", "change")]
public sealed partial class JSON : Component
{
    internal JSON()
    {
    }

    /// <summary>The buttons a JSON view shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["copy"];

    private object? _value;
    private IReadOnlyList<string>? _buttons;

    /// <summary>The document the component starts with.</summary>
    /// <value>
    /// An object, an array, a string holding JSON, or <see langword="null"/> for an empty view.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>The label drawn above the document.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the document next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the document wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether every node starts expanded.</summary>
    /// <remarks>Without it the frontend collapses everything deeper than three levels.</remarks>
    public bool Open { get; set; }

    /// <summary>Whether a list's indices are drawn beside its entries.</summary>
    public bool ShowIndices { get; set; }

    /// <summary>The height of the view, in pixels or as a CSS length, or <see langword="null"/> to fit the content.</summary>
    public object? Height { get; set; }

    /// <summary>The greatest height of the view, in pixels or as a CSS length.</summary>
    public object? MaxHeight { get; set; } = 500;

    /// <summary>The least height of the view, in pixels or as a CSS length.</summary>
    public object? MinHeight { get; set; }

    /// <summary>The buttons drawn in the view's corner.</summary>
    /// <value><c>copy</c> unless the application names others; an empty list shows none.</value>
    public IReadOnlyList<string> Buttons
    {
        get => _buttons ?? _defaultButtons;
        set => _buttons = value;
    }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any valid json",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject { ["foo"] = "bar" };

    /// <summary>The document a handler reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The document as CLR values — dictionaries, arrays and scalars.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);

    /// <summary>A cell of a flagging log read back as this component's value.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The document the cell holds.</returns>
    /// <remarks>
    /// The component's value is a document, so its cell is JSON; the apostrophe a spreadsheet guard
    /// may have added in front is undone on the way back in.
    /// </remarks>
    public override JsonNode? ReadFromFlag(string cell) => ParseEscapedJson(cell);

    /// <summary>The document the client receives from what a handler returned.</summary>
    /// <param name="value">An object, an array, a string holding JSON, or none.</param>
    /// <returns>The parsed document, or <see langword="null"/> for none.</returns>
    /// <exception cref="ArgumentException">A string was given and it does not hold JSON.</exception>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        JsonNode node => node.DeepClone(),
        string text => Parse(text),
        _ => JsonValues.FromAnyObject(value),
    };

    /// <summary>A string parsed as JSON.</summary>
    /// <param name="text">The string.</param>
    /// <returns>The document it holds.</returns>
    /// <exception cref="ArgumentException">The string is not JSON.</exception>
    /// <remarks>
    /// The Python version raises its user-facing <c>Error</c> here, which the frontend shows as the
    /// component's failure. The error type that carries one is its own batch (ticket 38); until
    /// then this is an argument error, and the message says the same thing.
    /// </remarks>
    private static JsonNode? Parse(string text)
    {
        try
        {
            return JsonNode.Parse(text);
        }
        catch (JsonException error)
        {
            throw new ArgumentException($"Invalid JSON string: {error.Message}", nameof(text), error);
        }
    }
}

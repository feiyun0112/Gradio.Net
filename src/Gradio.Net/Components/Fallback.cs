using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A component that carries any JSON value and draws it as a tree.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The Python version ships this component as the template
/// its command line writes when an application asks for a component of its own, and it is also a
/// perfectly usable component on its own: a value whose shape the application decides, shown
/// read-only.
/// </para>
/// <para>
/// The value is <see cref="JsonNode"/> rather than an arbitrary object because that is the one
/// shape the config can carry without the application describing it: a map, a list, a number and a
/// string all reach the frontend as the JSON they are, and a handler reads them back as the CLR
/// values they carry.
/// </para>
/// <para>
/// The class is not sealed. The Python version ships it as the template its command line writes
/// when an application asks for a component of its own, so a .NET application can derive from it
/// and add members of its own. A derived class reports its own name as the block type, which is
/// what a custom frontend bundle is looked up by; the extension point that serves one is its own
/// batch (ticket 36).
/// </para>
/// </remarks>
[GradioBlock("fallback")]
[HandlerValue("object?", "change")]
public partial class Fallback : Component
{
    internal Fallback()
    {
    }

    private string? _info;

    /// <summary>The value the component shows.</summary>
    /// <value>Any JSON value, or <see langword="null"/> for an empty view.</value>
    public JsonNode? Value { get; set; }

    /// <summary>The label drawn above the value.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn below the label.</summary>
    /// <value>
    /// The application's note, or <see langword="null"/> when it gave none or an empty one:
    /// the Python version writes the key only when the note says something.
    /// </value>
    public string? Info
    {
        get => _info is { Length: > 0 } ? _info : null;
        set => _info = value;
    }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; }

    /// <inheritdoc />
    /// <value>Any valid JSON, which is what the Python version declares for a component with no
    /// shape of its own.</value>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any valid json",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject { ["foo"] = "bar" };

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The value as CLR values — dictionaries, arrays and scalars.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">Any value the application produced.</param>
    /// <returns>The value as the JSON it is.</returns>
    /// <remarks>
    /// The general conversion rather than the narrow one: the value's shape is the application's,
    /// so a dictionary, a record or an array of whatever it likes has to be serialized rather than
    /// turned into its type name (see <see cref="JsonValues.FromAnyObject"/>).
    /// </remarks>
    public override JsonNode? Postprocess(object? value) => JsonValues.FromAnyObject(value);
}

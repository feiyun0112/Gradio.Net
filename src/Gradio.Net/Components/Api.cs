using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A value an API endpoint carries where no frontend component exists.
/// </summary>
/// <remarks>
/// <para>
/// The Python version builds one of these for every parameter and every return value of a handler
/// decorated with <c>gr.api</c>: the endpoint is described by the type hints, and the block that
/// stands for each of them carries the schema those hints produced rather than drawing a field.
/// Nothing in the config tells the frontend to draw it, which is why it has no label, no container
/// and no width — only a value, a schema and a name.
/// </para>
/// <para>
/// The schema is the one thing the component adds to the mapping. It is declared here as
/// <see cref="Schema"/> and written to the config under the Python version's parameter name,
/// <c>_api_info</c>; the same document is what <see cref="ApiInfo"/> answers, so the block's schema
/// and its entry in the API description cannot disagree.
/// </para>
/// </remarks>
[GradioBlock("api")]
public sealed partial class Api : Component
{
    internal Api()
    {
    }

    /// <summary>The value the endpoint's argument takes when the client sends none.</summary>
    /// <value>Any JSON value, or <see langword="null"/> for an argument with no default.</value>
    public JsonNode? Value { get; set; }

    /// <summary>The JSON schema of the value, derived from the handler's type hint.</summary>
    /// <value>
    /// The schema document, which is what the API description and the OpenAPI document are built
    /// from. An empty object for a component whose application declared none, which describes
    /// nothing rather than claiming a shape.
    /// </value>
    /// <remarks>
    /// Named for what it is rather than for the Python version's parameter: <c>_api_info</c> is not
    /// a member a C# application would write, and <see cref="ApiInfo"/> is taken by the property
    /// the config entry reads. <see cref="UpstreamParameterAttribute"/> keeps the mapping complete.
    /// </remarks>
    [UpstreamParameter("_api_info")]
    public JsonObject Schema { get; set; } = new();

    /// <summary>The name the endpoint's argument is shown under.</summary> <value><c>API</c>, which
    /// is the Python version's default for a block that has no other name to give.</value>
    public override string? Label { get; set; } = "API";

    /// <summary>
    /// The CLR type the endpoint's argument was declared with, or <see langword="null"/> for a
    /// block an application built by hand.
    /// </summary>
    /// <value>
    /// The type a handler's signature named, when <c>Blocks.Api</c> derived this block from it. A
    /// block an application constructed itself has no such type: its value is whatever its schema
    /// says, which is why <see cref="Preprocess"/> and <see cref="Postprocess"/> fall back to the
    /// general conversion.
    /// </value>
    /// <remarks>
    /// Not a prop: the Python version has no counterpart, because a Python function's hint
    /// <em>is</em> its schema and there is nothing else to keep. Here the schema is a document and
    /// the type is what turns a caller's JSON back into the value the handler declared, so the two
    /// are kept apart rather than reconstructed from one another.
    /// </remarks>
    internal Type? ValueType { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// A copy rather than the stored document: the config entry asks for the schema three times —
    /// as the input schema, the output schema and the schema itself — and a JSON node can only
    /// belong to one parent.
    /// </remarks>
    public override JsonObject ApiInfo => (JsonObject)Schema.DeepClone();

    /// <inheritdoc />
    /// <value>
    /// The value the application gave, or the string <c>...</c> when it gave none, which is
    /// the Python version's example for an argument whose value is unknown.
    /// </value>
    public override JsonNode? ExampleInputs => Value?.DeepClone() ?? JsonValue.Create("...");

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The value as CLR values.</returns>
    /// <remarks>
    /// The declared type when there is one — an endpoint derived from annotations promised its
    /// handler a value of that type, and a JSON number is not an <c>int</c> — and the general
    /// conversion for a block an application built by hand.
    /// </remarks>
    public override object? Preprocess(JsonNode? value) =>
        ValueType is { } type ? TypedValues.From(value, type) : JsonValues.ToObject(value);

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">Any value the handler produced.</param>
    /// <returns>The value as the JSON it is.</returns>
    /// <remarks>
    /// The declared type when there is one, so that a record's properties are spelled the way the
    /// endpoint's schema spells them; the general conversion otherwise, because an API endpoint's
    /// value is described by its schema rather than by a component's shape, and a dictionary or a
    /// record is serialized rather than turned into its type name.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        ValueType is { } type ? TypedValues.To(value, type) : JsonValues.FromAnyObject(value);
}

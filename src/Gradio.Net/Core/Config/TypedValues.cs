using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The CLR values an endpoint derived from type annotations reads and writes
/// </summary>
/// <remarks>
/// <para>
/// An endpoint described by <see cref="TypeSchema"/> hands its handler the types its signature
/// declares, and reports what it returned as the JSON those types stand for. That is the one thing
/// a component's own preprocessing cannot do: a textbox's value is a string whatever the handler
/// wanted, while an endpoint derived from annotations promised the handler an <c>int</c> — and a
/// JSON number arrives as a <c>double</c>, which a delegate cannot be called with.
/// </para>
/// <para>
/// The mapping is the serializer's, with <see cref="TypeSchema.Json"/>'s options, so that the
/// document a caller sends and the values the handler receives are the same document. Three values
/// are read by hand rather than by the serializer: any JSON, which has no CLR shape to deserialize
/// into; a file sent as the address of one, which is what an MCP client can send; and a value that
/// is absent, which becomes the type's default rather than an error — a parameter a client left out
/// is a parameter the handler declared a default for.
/// </para>
/// </remarks>
internal static class TypedValues
{
    /// <summary>The CLR value a JSON value stands for.</summary>
    /// <param name="value">The JSON value, or <see langword="null"/> for one that was not sent.</param>
    /// <param name="type">The type the handler declared.</param>
    /// <returns>The value to hand the handler.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="JsonException">
    /// The value does not describe the declared type, which is a caller's mistake rather than a
    /// handler's.
    /// </exception>
    internal static object? From(JsonNode? value, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type == typeof(object))
        {
            return JsonValues.ToObject(value);
        }

        if (type == typeof(JsonNode))
        {
            return value?.DeepClone();
        }

        if (type == typeof(JsonElement))
        {
            return value is null ? default : JsonSerializer.SerializeToElement(value);
        }

        if (value is null)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        // A file an MCP client sends is the address of one rather than the object a component would
        // carry, which is the shape the tool's schema advertises (see McpCatalog).
        if (type == typeof(FileData) && value is JsonValue address && address.TryGetValue<string>(out var path))
        {
            return new FileData { Path = path };
        }

        return JsonSerializer.Deserialize(value, type, TypeSchema.Json);
    }

    /// <summary>The JSON a CLR value is reported as.</summary>
    /// <param name="value">The value the handler returned.</param>
    /// <param name="type">The type the handler declared.</param>
    /// <returns>The JSON value, or <see langword="null"/> for a value that was not there.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A <see cref="JsonNode"/> is passed through and a value that describes its own JSON is asked,
    /// because both already know the shape they want to be written in; everything else is the
    /// serializer's answer for the type the handler declared.
    /// </remarks>
    internal static JsonNode? To(object? value, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (value is null)
        {
            return null;
        }

        if (value is JsonNode node)
        {
            return node.DeepClone();
        }

        if (value is IConfigValue described)
        {
            return described.ToConfigValue().DeepClone();
        }

        return JsonSerializer.SerializeToNode(value, type, TypeSchema.Json);
    }
}

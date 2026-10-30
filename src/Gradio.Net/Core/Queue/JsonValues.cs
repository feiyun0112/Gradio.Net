using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// Converts between the JSON a client sends and the CLR values a handler works with.
/// </summary>
/// <remarks>
/// <para>
/// The queue carries JSON because that is what crosses the wire, and a handler takes CLR values
/// because that is what a .NET method can be. Something has to sit between the two, and this is
/// that one place: a component's pre- and post-processing say <em>what</em> a value means for it,
/// while the conversion itself is the same for all of them.
/// </para>
/// <para>
/// The mapping is deliberately narrow — the JSON scalar types plus arrays and objects — because a
/// component that needs more than that describes it itself: a file component turns a path into a
/// file handle in its own preprocessing, not here.
/// </para>
/// </remarks>
internal static class JsonValues
{
    /// <summary>A string field, or <see langword="null"/> when the node is not one.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The text, or <see langword="null"/>.</returns>
    /// <remarks>
    /// Reading a named field of a protocol message is the commonest thing the queue and its
    /// endpoints do, and it is always the same two lines. They live here so that "a field that is
    /// not a string is not a string" is decided once rather than at each call site.
    /// </remarks>
    internal static string? Text(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;

    /// <summary>A number field, or <see langword="null"/> when the node is not one.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The number, or <see langword="null"/>.</returns>
    internal static double? Number(JsonNode? node)
    {
        if (node is not JsonValue value)
        {
            return null;
        }

        // A number that was parsed from a request is read as a double; one this framework built in
        // memory — a value an application set, a value a handler returned — keeps the type it was
        // built with, so the whole-number cases are asked for as well.
        if (value.TryGetValue<double>(out var number))
        {
            return number;
        }

        if (value.TryGetValue<int>(out var whole))
        {
            return whole;
        }

        if (value.TryGetValue<long>(out var wide))
        {
            return wide;
        }

        return value.TryGetValue<decimal>(out var precise) ? (double)precise : null;
    }

    /// <summary>An integer field, or <see langword="null"/> when the node is not one.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The number, or <see langword="null"/>.</returns>
    internal static int? Int32(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<int>(out var number) ? number : null;

    /// <summary>A whole number field, or <see langword="null"/> when the node is not one.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The number, or <see langword="null"/>.</returns>
    /// <remarks>
    /// A file's size is a <see langword="long"/> rather than an <see langword="int"/>, and the
    /// protocol does not say which one it sent: a client that wrote the size as a whole number
    /// reaches this as either, so both are accepted.
    /// </remarks>
    internal static long? Long(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<long>(out var number) ? number : null;

    /// <summary>A boolean field, defaulting to <see langword="false"/>.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The flag, or <see langword="false"/> when the node is not one.</returns>
    internal static bool Flag(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<bool>(out var flag) && flag;

    /// <summary>The CLR value a JSON node carries.</summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <returns>The value, or <see langword="null"/> for a JSON null.</returns>
    internal static object? ToObject(JsonNode? node) => node switch
    {
        null => null,
        JsonArray array => array.Select(ToObject).ToArray(),
        JsonObject value => value.ToDictionary(pair => pair.Key, pair => ToObject(pair.Value), StringComparer.Ordinal),
        JsonValue value => Scalar(value),
        _ => null,
    };

    /// <summary>The JSON a CLR value is written as.</summary>
    /// <param name="value">The value, or <see langword="null"/>.</param>
    /// <returns>The node, or <see langword="null"/> for a null value.</returns>
    /// <remarks>
    /// A <see cref="JsonNode"/> is passed through rather than converted, which is what lets a
    /// component whose postprocessing already produces JSON say so directly.
    /// </remarks>
    internal static JsonNode? FromObject(object? value) => value switch
    {
        null => null,
        JsonNode node => node,
        string text => JsonValue.Create(text),
        bool flag => JsonValue.Create(flag),
        double number => JsonValue.Create(number),
        float number => JsonValue.Create((double)number),
        int number => JsonValue.Create(number),
        long number => JsonValue.Create(number),
        decimal number => JsonValue.Create((double)number),
        IReadOnlyList<object?> list => new JsonArray([.. list.Select(FromObject)]),
        _ => JsonValue.Create(value.ToString()),
    };

    /// <summary>
    /// The JSON a value of any shape is written as, for a value whose shape is the application's
    /// rather than a component's.
    /// </summary>
    /// <param name="value">The value, or <see langword="null"/>.</param>
    /// <returns>The node, or <see langword="null"/> for a null value.</returns>
    /// <remarks>
    /// <see cref="FromObject"/> is deliberately narrow because a component's value has a known
    /// shape. A state's does not: the application may store a dictionary, a record, an array of
    /// whatever it likes, and turning one of those into its type name would be worse than
    /// serializing it. This is the mapping for that case, and the one place it happens.
    /// </remarks>
    internal static JsonNode? FromAnyObject(object? value) => value switch
    {
        null => null,
        JsonNode node => node.DeepClone(),

        // A value that says how it is written is asked rather than serialized: a multimodal text
        // names its fields `text` and `files`, and a serializer would write the property names it
        // happens to have in C# instead.
        IConfigValue described => described.ToConfigValue().DeepClone(),
        _ => JsonSerializer.SerializeToNode(value, value.GetType()),
    };

    /// <summary>The CLR value a JSON scalar carries.</summary>
    /// <remarks>
    /// Booleans are tried before numbers because JSON's <c>true</c> is not a number, and numbers
    /// before strings because a JSON number is not a string; the order is what keeps a component
    /// that expects a number from being handed its own text.
    /// </remarks>
    private static object? Scalar(JsonValue value)
    {
        if (value.TryGetValue<bool>(out var flag))
        {
            return flag;
        }

        if (value.TryGetValue<string>(out var text))
        {
            return text;
        }

        // Numbers are read as doubles, whatever they were built as, so that a value this framework
        // made in memory and one that arrived in a request are the same value to a handler.
        return Number(value);
    }
}

using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// Collects a block's props in the order the Python version declares its constructor parameters.
/// </summary>
/// <remarks>
/// <para>
/// Two rules live here rather than in every component. A <see langword="null"/> is not written: the
/// Python version deletes every <c>None</c> from the props before it serves them, so "the parameter
/// is absent" and "the parameter is null" mean the same thing to the frontend, and a component that
/// left an optional parameter unset must not send it.
/// </para>
/// <para>
/// The order props are added in is preserved. JSON objects are unordered as far as a comparison is
/// concerned, but the Python version renders them in constructor-parameter order, and a config that
/// reads like the Python version's is a config a reviewer can diff by eye.
/// </para>
/// </remarks>
public sealed class PropWriter
{
    private readonly JsonObject _props = new();
    private readonly bool _keepNulls;

    /// <summary>Creates a writer that leaves null props out, as the Python version serves
    /// them.</summary>
    public PropWriter()
    {
    }

    /// <summary>Creates a writer that writes null props as null.</summary>
    /// <param name="keepNulls">
    /// Whether an unset parameter is written as <c>null</c> rather than left out.
    /// </param>
    /// <remarks>
    /// The Python version serves a component's props with the nulls removed, but it hands another
    /// component the same props with the nulls in place: a dataset copies each of its components'
    /// props to describe how it draws a sample, and that copy is made from the in-memory config
    /// rather than from the served one. One writer with two modes is how both shapes come from one
    /// mapping.
    /// </remarks>
    internal PropWriter(bool keepNulls) => _keepNulls = keepNulls;

    /// <summary>Adds a prop, unless its value is <see langword="null"/> and nulls are left
    /// out.</summary> <param name="name">The Python parameter name.</param> <param name="value">The
    /// value, or <see langword="null"/> to leave the prop out.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or
    /// whitespace.</exception> <exception cref="NotSupportedException"><paramref name="value"/> is
    /// of a type this writer cannot serialize.</exception>
    public void Add(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (value is null)
        {
            if (_keepNulls)
            {
                _props[name] = null;
            }

            return;
        }

        _props[name] = Serialize(value);
    }

    /// <summary>The props written so far.</summary>
    internal JsonObject Build() => _props;

    /// <summary>The JSON a prop's value is written as.</summary>
    /// <remarks>
    /// <para>
    /// Two kinds of value are written as <c>null</c> rather than left out, because that is what the
    /// Python version's serializer makes of them and the key has to stay: a number that is not
    /// finite — orjson writes <c>Infinity</c> as <c>null</c>, which is how
    /// <c>State.time_to_live</c> reports "never expires" — and a callable, whose Python repr is not
    /// something a second implementation could reproduce (<c>State.delete_callback</c>). Writing
    /// them as null keeps the key set equal to the Python version's, so "not ours to compute" stays
    /// distinguishable from "forgotten".
    /// </para>
    /// <para>
    /// This is the one rule for "what a prop's value is written as", so a partial update uses it
    /// too: the props a handler answers with are merged into a component by the client, and a value
    /// written one way in a config and another way in an update would describe two components.
    /// </para>
    /// </remarks>
    internal static JsonNode? Serialize(object value) => value switch
    {
        JsonNode node => node.DeepClone(),
        string text => JsonValue.Create(text),
        bool flag => JsonValue.Create(flag),
        int number => JsonValue.Create(number),
        long number => JsonValue.Create(number),
        double number => double.IsFinite(number) ? JsonValue.Create(number) : null,
        decimal number => JsonValue.Create(number),
        Delegate => null,
        IConfigValue structured => structured.ToConfigValue(),

        // A list of structured values — a chat's messages, a chatbot's delimiters — is written as
        // an array of the objects each of them says it is. The interface is covariant, so a list of
        // any one such type reaches this case.
        IReadOnlyList<IConfigValue> structured => ToArray(structured),
        IReadOnlyList<string?> items => ToArray(items),

        // A list of documents — the props a dataset copies from the components it draws, which are
        // already JSON and must be written as they are rather than converted from a CLR shape.
        IReadOnlyList<JsonNode> documents => ToArray(documents),

        // A map of strings — a colour map, which is an object whose every value is a colour.
        IReadOnlyDictionary<string, string> map => ToObject(map),

        // A list of scalars whose type the application decides — a dropdown that allows several
        // choices holds values that may be text or numbers, and which one a given entry is is the
        // application's answer rather than the component's. Each entry is written as the JSON value
        // it carries, so that an entry that is a number stays one.
        IReadOnlyList<object?> values => ToArray(values),

        // A list of numbers — a canvas's size, a scene's background colour — and a list of numbers
        // that may be absent, which is a camera position whose angles the application left to the
        // frontend. Both are written as the JSON values they carry rather than as text.
        IReadOnlyList<int> numbers => ToArray([.. numbers.Select(number => (object?)number)]),
        IReadOnlyList<double> numbers => ToArray([.. numbers.Select(number => (object?)number)]),
        IReadOnlyList<int?> numbers => ToArray([.. numbers.Select(number => (object?)number)]),
        IReadOnlyList<double?> numbers => ToArray([.. numbers.Select(number => (object?)number)]),
        _ => throw new NotSupportedException(
            $"A prop of type {value.GetType()} cannot be written to the config; "
            + "the prop writer knows JSON values, numbers, strings, booleans, string lists and "
            + "lists of values that describe their own JSON."),
    };

    private static JsonArray ToArray(IReadOnlyList<string?> items)
    {
        var array = new JsonArray();
        foreach (var item in items)
        {
            array.Add(item);
        }

        return array;
    }

    private static JsonArray ToArray(IReadOnlyList<IConfigValue> items)
    {
        var array = new JsonArray();
        foreach (var item in items)
        {
            array.Add(item.ToConfigValue());
        }

        return array;
    }

    private static JsonArray ToArray(IReadOnlyList<JsonNode> documents)
    {
        var array = new JsonArray();
        foreach (var document in documents)
        {
            array.Add(document.DeepClone());
        }

        return array;
    }

    private static JsonObject ToObject(IReadOnlyDictionary<string, string> map)
    {
        var document = new JsonObject();
        foreach (var (key, value) in map)
        {
            document[key] = value;
        }

        return document;
    }

    private static JsonArray ToArray(IReadOnlyList<object?> values)
    {
        var array = new JsonArray();
        foreach (var value in values)
        {
            array.Add(JsonValues.FromObject(value));
        }

        return array;
    }
}

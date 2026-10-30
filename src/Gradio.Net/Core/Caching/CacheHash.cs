using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The digest a value is cached under: the Python version's <c>cache_hash</c>.
/// </summary>
/// <remarks>
/// <para>
/// A cache key has to be a value's content rather than its identity, because the second call that
/// should hit the cache is a different object carrying the same value. The Python version hashes a
/// structural spelling of the value — <c>L[...]</c> for a sequence, <c>D{...}</c> for a mapping
/// with its keys sorted, <c>S{...}</c> for a set, the digest of the bytes for binary — and takes
/// the SHA-256 of that text. The same spelling is produced here, which is what makes the two sides
/// agree on the digest of a value they both hold: a test can compute a digest with the Python
/// version's own function and compare it with this one.
/// </para>
/// <para>
/// Two spellings have no counterpart and are documented rather than reproduced: a Python tuple
/// spells as <c>T[...]</c> while every .NET sequence is a list, and a number that arrives as JSON
/// is read as a <see cref="double"/> here, so an integral number hashes as <c>1.0</c> where Python
/// would hash the integer <c>1</c>. A digest never crosses the language boundary — it names an
/// entry in one process's cache — so what matters is that the same value hashes the same way on the
/// side that holds it.
/// </para>
/// </remarks>
public static class CacheHash
{
    /// <summary>The digest of a value.</summary>
    /// <param name="value">The value to hash.</param>
    /// <returns>The digest, as lower-case hexadecimal.</returns>
    /// <exception cref="NotSupportedException">
    /// The value cannot be hashed: it is an object with no state to spell.
    /// </exception>
    /// <remarks>
    /// The value may be anything a handler works with: a JSON document, a primitive, a sequence, a
    /// mapping, a byte array, or an object whose public properties are its state. A value with no
    /// structural spelling is refused rather than hashed by identity, because a key that changes
    /// between two equal values never hits.
    /// </remarks>
    public static string Of(object? value) =>
        Digest(SHA256.HashData(Encoding.UTF8.GetBytes(Repr(value))));

    /// <summary>The structural spelling of a value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text the digest is taken of.</returns>
    internal static string Repr(object? value)
    {
        switch (value)
        {
            case null:
                return "None";

            case bool flag:
                return flag ? "True" : "False";

            case string text:
                return PythonRepr.Of(text);

            case char character:
                return PythonRepr.Of(character.ToString());

            case double number:
                return PythonRepr.Of(number);

            case float single:
                return PythonRepr.Of(single);

            case decimal precise:
                return PythonRepr.Of((double)precise);

            case byte[] bytes:
                return Digest(SHA256.HashData(bytes));

            case JsonNode node:
                return ReprOf(node);

            case JsonElement element:
                return ReprOf(JsonNode.Parse(element.GetRawText()));

            case Enum member:
                return Repr(member.ToString());

            case DateTime or DateTimeOffset or TimeSpan or Guid:
                return PythonRepr.Of(value.ToString()!);

            case IReadOnlyDictionary<string, object?> map:
                return Mapping(map);

            case IDictionary dictionary:
                return Mapping(dictionary);

            case IEnumerable sequence:
                return IsSet(sequence.GetType()) ? Set(sequence) : Sequence(sequence);
        }

        var type = value.GetType();

        if (type.IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture)!;
        }

        return Mapping(value);
    }

    /// <summary>The spelling of a JSON document, which is the shape most values arrive in.</summary>
    /// <param name="node">The document.</param>
    /// <returns>The text.</returns>
    private static string ReprOf(JsonNode? node)
    {
        switch (node)
        {
            case null:
                return "None";

            case JsonArray array:
                return Sequence(array);

            case JsonObject map:
                return Mapping(map);

            case JsonValue scalar:
                return ReprOfScalar(scalar);

            default:
                return PythonRepr.Of(node.ToJsonString());
        }
    }

    /// <summary>The spelling of a JSON scalar, by the type it carries.</summary>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// A number is asked for as an integer first, because that is what Python's JSON reader makes
    /// of <c>1</c>: the digest of a request's own value should not depend on the reader that turned
    /// it into a CLR value.
    /// </remarks>
    private static string ReprOfScalar(JsonValue scalar)
    {
        if (scalar.TryGetValue<bool>(out var flag))
        {
            return flag ? "True" : "False";
        }

        if (scalar.TryGetValue<string>(out var text))
        {
            return PythonRepr.Of(text);
        }

        if (scalar.TryGetValue<int>(out var whole))
        {
            return whole.ToString(CultureInfo.InvariantCulture);
        }

        if (scalar.TryGetValue<long>(out var wide))
        {
            return wide.ToString(CultureInfo.InvariantCulture);
        }

        if (scalar.TryGetValue<double>(out var number))
        {
            return PythonRepr.Of(number);
        }

        if (scalar.TryGetValue<decimal>(out var precise))
        {
            return PythonRepr.Of((double)precise);
        }

        return "None";
    }

    /// <summary>The spelling of a mapping: its keys sorted, then each pair.</summary>
    /// <param name="pairs">The mapping's entries, each key paired with the text it spells as.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// <para>
    /// The pairs are sorted by the spelling of their key, which is what makes two mappings with the
    /// same content hash the same way however they were built.
    /// </para>
    /// <para>
    /// One implementation for every mapping shape, because there must be one answer: a dictionary
    /// and the JSON object that describes it are the same value, and two spellings would be two
    /// keys. What differs between the shapes is only how a key is spelled, and that is what the
    /// caller hands over.
    /// </para>
    /// </remarks>
    private static string Mapping(IEnumerable<(string Key, string Value)> pairs)
    {
        var spelled = pairs
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => $"{pair.Key}:{pair.Value}");

        return $"D{{{string.Join(",", spelled)}}}";
    }

    /// <summary>The spelling of a dictionary whose keys are strings.</summary>
    /// <param name="pairs">The entries.</param>
    /// <returns>The text.</returns>
    private static string Mapping(IEnumerable<KeyValuePair<string, object?>> pairs) =>
        Mapping(pairs.Select(pair => (PythonRepr.Of(pair.Key), Repr(pair.Value))));

    /// <summary>The spelling of a dictionary whose keys are anything.</summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// A key is spelled as the value it is rather than as text, which is what a Python dictionary
    /// does: <c>{1: "a"}</c> has the key <c>1</c>, not <c>"1"</c>.
    /// </remarks>
    private static string Mapping(IDictionary dictionary) =>
        Mapping(dictionary.Cast<DictionaryEntry>().Select(entry => (Repr(entry.Key), Repr(entry.Value))));

    /// <summary>The spelling of a JSON object.</summary>
    /// <param name="map">The object.</param>
    /// <returns>The text.</returns>
    private static string Mapping(JsonObject map) =>
        Mapping(map.Select(pair => (PythonRepr.Of(pair.Key), Repr(pair.Value))));

    /// <summary>The spelling of an object's own state.</summary>
    /// <param name="value">The object.</param>
    /// <returns>The text.</returns>
    /// <exception cref="NotSupportedException">The object has no readable state.</exception>
    /// <remarks>
    /// The Python version falls back to <c>vars(obj)</c>, which is the object's attribute
    /// dictionary. The counterpart here is the object's readable public properties, in name order;
    /// an object with none has nothing to spell and is refused, because hashing it by identity
    /// would produce a key that never hits twice.
    /// </remarks>
    private static string Mapping(object value)
    {
        var properties = value.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
            .ToList();

        if (properties.Count == 0)
        {
            throw new NotSupportedException(
                $"CacheHash cannot hash an object of type {value.GetType().Name}. "
                + "Preprocess your inputs into hashable types before passing them.");
        }

        return Mapping(properties.Select(property => new KeyValuePair<string, object?>(
            property.Name,
            property.GetValue(value))));
    }

    private static string Sequence(IEnumerable items)
    {
        var spelled = new List<string>();

        foreach (var item in items)
        {
            spelled.Add(Repr(item));
        }

        return $"L[{string.Join(",", spelled)}]";
    }

    /// <summary>The spelling of a set: its members sorted, because a set has no order.</summary>
    /// <param name="items">The members.</param>
    /// <returns>The text.</returns>
    private static string Set(IEnumerable items)
    {
        var spelled = new List<string>();

        foreach (var item in items)
        {
            spelled.Add(Repr(item));
        }

        spelled.Sort(StringComparer.Ordinal);

        return $"S{{{string.Join(",", spelled)}}}";
    }

    /// <summary>Whether a type is a set, whose members have no order to hash in.</summary>
    /// <param name="type">The type.</param>
    /// <returns><see langword="true"/> when it is a set.</returns>
    /// <remarks>
    /// Asked of the type rather than of the value because a <c>HashSet&lt;string&gt;</c> is not an
    /// <c>ISet&lt;object?&gt;</c>: the interface is invariant, so the element type has to be looked
    /// through.
    /// </remarks>
    private static bool IsSet(Type type) =>
        type.GetInterfaces().Any(implemented =>
            implemented.IsGenericType
            && implemented.GetGenericTypeDefinition() == typeof(ISet<>));

    /// <summary>A digest as lower-case hexadecimal, which is how the Python version spells
    /// one.</summary> <param name="bytes">The digest.</param> <returns>The text.</returns>
    private static string Digest(byte[] bytes) => Convert.ToHexString(bytes).ToLowerInvariant();
}

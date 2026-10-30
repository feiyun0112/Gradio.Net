using System.Globalization;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A value spelled the way Python spells it in a message: <c>repr()</c>, and the name <c>type()</c>
/// prints.
/// </summary>
/// <remarks>
/// <para>
/// The message a component raises when it refuses a value is the Python version's, character for
/// character, because it is what the user reads when the input is rejected and what the operator
/// finds in the log afterwards. The Python version builds those messages with f-strings, so a value
/// appears as <c>repr()</c> spells it — <c>'a'</c> for text, <c>2</c> for a whole number, <c>['a',
/// 'b']</c> for a list — and a type appears as <c>type()</c> prints it, <c>&lt;class 'str'&gt;</c>.
/// Reproducing the message means reproducing both, which is what this type is for.
/// </para>
/// <para>
/// It is not <see cref="PythonRepr"/>, and the two are worth keeping apart: that one spells a value
/// for a cache key, where only the <c>repr</c> matters, and this one spells a value for a message,
/// which also says what type Python would have called it.
/// </para>
/// <para>
/// The JSON overloads are not a convenience. A value that arrived as JSON has no Python type of its
/// own here: <c>2</c> and <c>2.0</c> are both a number to this framework, while the Python
/// version's parser reads them as <c>int</c> and <c>float</c> and prints them differently. What the
/// client wrote is what decides, so the spelling is read from the text rather than from the number
/// it becomes.
/// </para>
/// </remarks>
internal static class PythonText
{
    /// <summary>A JSON value, spelled as <c>repr()</c> spells it.</summary>
    /// <param name="value">The value, or <see langword="null"/> for <c>None</c>.</param>
    /// <returns>The text.</returns>
    internal static string Repr(JsonNode? value) => value switch
    {
        null => "None",
        JsonArray list => ReprOf(list.Select(item => item)),
        JsonObject map => "{" + string.Join(", ", map.Select(pair => $"{Quote(pair.Key)}: {Repr(pair.Value)}")) + "}",
        JsonValue scalar => ReprOfScalar(scalar),
        _ => "None",
    };

    /// <summary>A value this framework holds, spelled as <c>repr()</c> spells it.</summary>
    /// <param name="value">The value, or <see langword="null"/> for <c>None</c>.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// This is the spelling of a choice's value, which was written as a C# scalar rather than
    /// arriving as JSON: a choice of <c>2</c> is an <see cref="int"/> and prints as <c>2</c>, and one
    /// of <c>2.0</c> is a <see cref="double"/> and prints as <c>2.0</c>.
    /// </remarks>
    internal static string Repr(object? value) => value switch
    {
        null => "None",
        string text => Quote(text),
        bool flag => flag ? "True" : "False",
        double number => PythonRepr.Of(number),
        float number => PythonRepr.Of((double)number),
        decimal number => PythonRepr.Of((double)number),
        JsonNode node => Repr(node),
        IFormattable number => number.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "None",
    };

    /// <summary>A JSON value, spelled as <c>str()</c> spells it.</summary>
    /// <param name="value">The value, or <see langword="null"/> for <c>None</c>.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// The difference from <see cref="Repr(JsonNode?)"/> is a text value: <c>str()</c> writes it as
    /// it is and <c>repr()</c> quotes it. The Python version's dropdown message uses one for a
    /// value inside a list and the other for a value on its own, which is an inconsistency of the
    /// Python version's that this reproduces rather than tidies.
    /// </remarks>
    internal static string Str(JsonNode? value) =>
        value is JsonValue scalar && scalar.TryGetValue<string>(out var text) ? text : Repr(value);

    /// <summary>A sequence of values, spelled as <c>repr()</c> spells a list.</summary>
    /// <param name="values">The values.</param>
    /// <returns>The text, as <c>['a', 'b', 2]</c>.</returns>
    internal static string ReprOf(IEnumerable<object?> values) =>
        "[" + string.Join(", ", values.Select(Repr)) + "]";

    /// <summary>A sequence of texts, spelled as <c>repr()</c> spells a list of them.</summary>
    /// <param name="values">The texts.</param>
    /// <returns>The text, as <c>['.png', '.jpg']</c>.</returns>
    internal static string ReprOf(IEnumerable<string> values) =>
        "[" + string.Join(", ", values.Select(Quote)) + "]";

    /// <summary>The name <c>type()</c> prints for a JSON value.</summary>
    /// <param name="value">The value, or <see langword="null"/> for <c>None</c>.</param>
    /// <returns>The name, as <c>&lt;class 'str'&gt;</c>.</returns>
    /// <remarks>
    /// There is no overload for a value this framework holds: the type a message names is always
    /// the one Python would have given what the client sent, and what the client sent is JSON.
    /// </remarks>
    internal static string TypeName(JsonNode? value) => value switch
    {
        null => "<class 'NoneType'>",
        JsonArray => "<class 'list'>",
        JsonObject => "<class 'dict'>",
        JsonValue scalar => TypeNameOfScalar(scalar),
        _ => "<class 'NoneType'>",
    };

    /// <summary>A text, quoted the way <c>repr()</c> quotes one.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The quoted text.</returns>
    private static string Quote(string text) => PythonRepr.Of(text);

    /// <summary>A JSON scalar, spelled as <c>repr()</c> spells it.</summary>
    /// <param name="value">The scalar.</param>
    /// <returns>The text.</returns>
    private static string ReprOfScalar(JsonValue value)
    {
        if (value.TryGetValue<bool>(out var flag))
        {
            return flag ? "True" : "False";
        }

        if (value.TryGetValue<string>(out var text))
        {
            return Quote(text);
        }

        // A number keeps the spelling the client wrote: the Python version's parser makes an int of "2" and a
        // float of "2.0", and the two print differently.
        var written = value.ToJsonString();

        return IsFloat(written) ? PythonRepr.Of(value.GetValue<double>()) : written;
    }

    /// <summary>The name <c>type()</c> prints for a JSON scalar.</summary>
    /// <param name="value">The scalar.</param>
    /// <returns>The name.</returns>
    private static string TypeNameOfScalar(JsonValue value)
    {
        if (value.TryGetValue<bool>(out _))
        {
            return "<class 'bool'>";
        }

        if (value.TryGetValue<string>(out _))
        {
            return "<class 'str'>";
        }

        return IsFloat(value.ToJsonString()) ? "<class 'float'>" : "<class 'int'>";
    }

    /// <summary>Whether a number was written with a decimal point or an exponent.</summary>
    /// <param name="written">The number as the JSON has it.</param>
    /// <returns>Whether the Python version would read it as a float.</returns>
    private static bool IsFloat(string written) =>
        written.Contains('.', StringComparison.Ordinal)
        || written.Contains('e', StringComparison.Ordinal)
        || written.Contains('E', StringComparison.Ordinal);
}

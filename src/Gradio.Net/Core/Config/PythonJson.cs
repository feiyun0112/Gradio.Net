using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A JSON value spelled the way Python's <c>json.dumps</c> spells it, for the places where the text
/// is part of what an application says rather than a document a client parses.
/// </summary>
/// <remarks>
/// <para>
/// Two places need this. A component that clears others writes the values it clears them to into a
/// fragment of JavaScript, and the Python version builds that fragment by dumping a list with
/// <c>json.dumps</c>. The command-line snippet in the API documentation is the other: it shows the
/// body a caller would type, dumped with <c>indent=2</c>. Both are served to the browser and
/// compared with the Python version's, so the spelling is observable — Python puts a space after
/// each colon, and .NET's serializer does not.
/// </para>
/// <para>
/// The rules are Python's defaults: <c>", "</c> and <c>": "</c> between entries, non-ASCII text
/// escaped as <c>\uXXXX</c>, and a float written with a decimal point even when it is whole, which
/// is the difference between <c>1.0</c> and <c>1</c>. Text is escaped the way Python escapes it, so
/// a string carrying a quote or a newline reaches the browser as the same fragment.
/// </para>
/// </remarks>
internal static class PythonJson
{
    /// <summary>Writes a value as Python's <c>json.dumps</c> writes it.</summary>
    /// <param name="value">The value to write.</param>
    /// <returns>The text, without a trailing newline.</returns>
    internal static string Dump(JsonNode? value) => Dump(value, indent: 0);

    /// <summary>Writes a value as Python's <c>json.dumps</c> writes it, with one entry per
    /// line.</summary> <param name="value">The value to write.</param>
    /// <param name="indent">
    /// How many spaces each level is indented by, or zero for one line. Python's own default is
    /// zero; a snippet a reader copies is written with two.
    /// </param>
    /// <returns>The text, without a trailing newline.</returns>
    /// <remarks>
    /// An indented dump separates entries with a newline rather than with <c>", "</c>, and a value
    /// that holds nothing is still written on one line — <c>{}</c> and <c>[]</c> — which is what
    /// Python does and what keeps a snippet's shape recognizable.
    /// </remarks>
    internal static string Dump(JsonNode? value, int indent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(indent);

        var text = new StringBuilder();
        Write(text, value, indent, level: 0);

        return text.ToString();
    }

    private static void Write(StringBuilder text, JsonNode? value, int indent, int level)
    {
        switch (value)
        {
            case null:
                text.Append("null");
                return;

            case JsonArray array:
                text.Append('[');

                for (var index = 0; index < array.Count; index++)
                {
                    if (index > 0)
                    {
                        text.Append(indent > 0 ? "," : ", ");
                    }

                    NewLine(text, indent, level + 1);
                    Write(text, array[index], indent, level + 1);
                }

                NewLine(text, indent, array.Count > 0 ? level : -1);
                text.Append(']');
                return;

            case JsonObject obj:
                text.Append('{');

                var first = true;
                foreach (var (name, entry) in obj)
                {
                    if (!first)
                    {
                        text.Append(indent > 0 ? "," : ", ");
                    }

                    first = false;
                    NewLine(text, indent, level + 1);
                    WriteText(text, name);
                    text.Append(": ");
                    Write(text, entry, indent, level + 1);
                }

                NewLine(text, indent, obj.Count > 0 ? level : -1);
                text.Append('}');
                return;

            default:
                WriteScalar(text, value);
                return;
        }
    }

    /// <summary>Starts a new line at one level's indentation, or does nothing on one line.</summary>
    /// <param name="text">The text being built.</param>
    /// <param name="indent">How many spaces a level is, or zero for one line.</param>
    /// <param name="level">The level to indent to, or a negative number for no newline at all.</param>
    private static void NewLine(StringBuilder text, int indent, int level)
    {
        if (indent <= 0 || level < 0)
        {
            return;
        }

        text.Append('\n').Append(' ', indent * level);
    }

    private static void WriteScalar(StringBuilder text, JsonNode value)
    {
        if (value.GetValueKind() == System.Text.Json.JsonValueKind.True)
        {
            text.Append("true");
            return;
        }

        if (value.GetValueKind() == System.Text.Json.JsonValueKind.False)
        {
            text.Append("false");
            return;
        }

        if (value.GetValueKind() == System.Text.Json.JsonValueKind.String)
        {
            WriteText(text, value.GetValue<string>());
            return;
        }

        switch (value)
        {
            // An integer is asked for first: a document that says 1 is the integer 1, and Python
            // dumps it as "1" rather than as the float "1.0". A value this framework built from a
            // double only answers the double branch, which is what keeps 1.0 spelled as 1.0.
            case JsonValue scalar when scalar.TryGetValue<int>(out var whole):
                text.Append(whole.ToString(CultureInfo.InvariantCulture));
                return;

            case JsonValue scalar when scalar.TryGetValue<long>(out var wide):
                text.Append(wide.ToString(CultureInfo.InvariantCulture));
                return;

            case JsonValue scalar when scalar.TryGetValue<double>(out var number):
                text.Append(Number(number));
                return;

            case JsonValue scalar when scalar.TryGetValue<decimal>(out var number):
                text.Append(Number((double)number));
                return;

            default:
                text.Append(value.ToJsonString());
                return;
        }
    }

    /// <summary>A number as Python writes it.</summary>
    /// <param name="number">The number.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// A whole float keeps its <c>.0</c>, because Python's float repr always says it is a float; an
    /// exponent is spelled with a lower-case <c>e</c> and a two-digit exponent, as Python spells
    /// it.
    /// </remarks>
    private static string Number(double number)
    {
        if (!double.IsFinite(number))
        {
            return number > 0 ? "Infinity" : double.IsNaN(number) ? "NaN" : "-Infinity";
        }

        if (number == Math.Floor(number) && Math.Abs(number) < 1e16)
        {
            return number.ToString("0.0", CultureInfo.InvariantCulture);
        }

        var text = number.ToString("R", CultureInfo.InvariantCulture);
        var exponent = text.IndexOfAny(['E', 'e']);

        if (exponent < 0)
        {
            return text;
        }

        var mantissa = text[..exponent];
        var power = int.Parse(text[(exponent + 1)..], CultureInfo.InvariantCulture);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{mantissa}e{(power < 0 ? "-" : "+")}{Math.Abs(power):00}");
    }

    /// <summary>Writes text as Python writes it, which is ASCII with the characters that need it
    /// escaped.</summary> <param name="text">The text being built.</param> <param name="value">The
    /// string.</param>
    private static void WriteText(StringBuilder text, string value)
    {
        text.Append('"');

        foreach (var character in value)
        {
            switch (character)
            {
                case '"':
                    text.Append("\\\"");
                    break;
                case '\\':
                    text.Append("\\\\");
                    break;
                case '\n':
                    text.Append("\\n");
                    break;
                case '\r':
                    text.Append("\\r");
                    break;
                case '\t':
                    text.Append("\\t");
                    break;
                case '\b':
                    text.Append("\\b");
                    break;
                case '\f':
                    text.Append("\\f");
                    break;
                default:
                    if (character < 0x20 || character > 0x7e)
                    {
                        text.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        text.Append(character);
                    }

                    break;
            }
        }

        text.Append('"');
    }
}

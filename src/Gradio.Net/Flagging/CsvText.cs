using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The two pieces of Python's <c>csv</c> module that the flagging loggers need: how a cell is
/// written, and how a value that a spreadsheet could read as a formula is made safe.
/// </summary>
/// <remarks>
/// Shared rather than written twice because the two loggers — the one an interface writes with and
/// the one a chat writes with — have to produce the same dialect: a file an application reads back
/// with a spreadsheet is the artifact, and a second spelling of the quoting rules would be a second
/// dialect.
/// </remarks>
internal static class CsvText
{
    /// <summary>One row, comma separated, with a CRLF after it.</summary>
    /// <param name="values">The cells.</param>
    /// <returns>The line, as it is written to the file.</returns>
    /// <remarks>
    /// Python's <c>csv</c> module ends every row with CRLF, which is what an application reading
    /// the file back with a spreadsheet expects to find.
    /// </remarks>
    internal static string Row(IReadOnlyList<object?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var line = new StringBuilder();

        for (var index = 0; index < values.Count; index++)
        {
            if (index > 0)
            {
                line.Append(',');
            }

            line.Append(Quote(Sanitize(values[index])));
        }

        line.Append("\r\n");

        return line.ToString();
    }

    /// <summary>Quotes a cell the way Python's csv module does.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The cell, quoted if it has to be.</returns>
    internal static string Quote(string cell) =>
        cell.IndexOfAny([',', '"', '\r', '\n']) >= 0
            ? "\"" + cell.Replace("\"", "\"\"", StringComparison.Ordinal) + "\""
            : cell;

    /// <summary>A cell as a string, refusing to let a spreadsheet read it as a formula.</summary>
    /// <param name="value">The cell.</param>
    /// <returns>The text to write.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>sanitize_value_for_csv</c>: a value starting with <c>=</c>,
    /// <c>+</c>, <c>-</c>, <c>@</c>, a tab or a newline — or carrying one of those after a comma —
    /// is prefixed with an apostrophe, which is the spreadsheet convention for "this is text".
    /// Numbers are left alone, because a number cannot be a formula.
    /// </para>
    /// <para>
    /// A missing value is the text <c>None</c>, because that is what the Python version's rule
    /// produces: it coerces anything that is not text with <c>str()</c>, and <c>str(None)</c> is
    /// <c>"None"</c>. The loggers that want an empty cell for a sample they were given nothing for
    /// say so before they get here — that guard is the Python version's too, and it is in a
    /// different place.
    /// </para>
    /// </remarks>
    internal static string Sanitize(object? value)
    {
        if (value is null)
        {
            return "None";
        }

        if (value is int or long or double or float or decimal)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;

        // A cell that is JSON is written as JSON rather than escaped as text: normalizing it is what
        // makes two cells that describe the same value the same bytes, and it is also what keeps a
        // list of numbers from being mistaken for a formula — "[1, -2]" is not dangerous, and a
        // leading apostrophe on it would be a lie about what the cell holds.
        if (text.StartsWith('{') || text.StartsWith('['))
        {
            try
            {
                return PythonJson.Dump(JsonNode.Parse(text));
            }
            catch (System.Text.Json.JsonException)
            {
                // Not JSON after all, so it is text and is treated as text.
            }
        }

        return UnsafeStart(text) || UnsafeSequence(text) ? "'" + text : text;
    }

    private static bool UnsafeStart(string text) =>
        text.Length > 0 && text[0] is '=' or '+' or '-' or '@' or '\t' or '\n';

    private static bool UnsafeSequence(string text) =>
        text.Contains(",=", StringComparison.Ordinal)
        || text.Contains(",+", StringComparison.Ordinal)
        || text.Contains(",-", StringComparison.Ordinal)
        || text.Contains(",@", StringComparison.Ordinal)
        || text.Contains(",\t", StringComparison.Ordinal)
        || text.Contains(",\n", StringComparison.Ordinal);

    /// <summary>Now, as the three loggers spell it.</summary>
    /// <returns>The time, to the microsecond.</returns>
    /// <remarks>
    /// The Python version writes <c>str(datetime.datetime.now())</c>, which carries the
    /// microseconds — except when they are zero, where Python leaves the fraction out. Both are
    /// reproduced: a row written on a whole second is a row without a fraction, not a row with
    /// <c>.000000</c> on it.
    /// </remarks>
    internal static string Timestamp()
    {
        var now = System.DateTime.Now;

        return now.Microsecond == 0
            ? now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            : now.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
    }
}

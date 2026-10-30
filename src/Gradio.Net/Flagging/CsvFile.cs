using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The file a flagging logger writes: how a row is appended, how many the file holds, and what a
/// file-valued cell is reduced to.
/// </summary>
/// <remarks>
/// Shared by every logger for the same reason the dialect is shared: the four loggers differ in what
/// they decide to write, not in how a CSV file is written. Keeping the file mechanics here is what
/// makes "the two loggers produce the same kind of file" a property of the code rather than a
/// coincidence.
/// </remarks>
internal static class CsvFile
{
    /// <summary>Appends one row.</summary>
    /// <param name="path">The file.</param>
    /// <param name="values">The cells.</param>
    internal static void Append(string path, IReadOnlyList<object?> values) =>
        System.IO.File.AppendAllText(path, CsvText.Row(values), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

    /// <summary>How many samples the file holds.</summary>
    /// <param name="path">The file.</param>
    /// <param name="hasHeader">Whether the first row is a header rather than a sample.</param>
    /// <returns>The number of samples.</returns>
    /// <remarks>
    /// Counted by reading the file back rather than by keeping a tally, which is what the Python
    /// version does: the file is the record, and a tally that disagreed with it would be a second
    /// answer to the same question.
    /// </remarks>
    internal static int Count(string path, bool hasHeader = true)
    {
        using var reader = new StreamReader(path, Encoding.UTF8);
        var rows = 0;

        while (ReadRow(reader) is not null)
        {
            rows++;
        }

        return Math.Max(0, hasHeader ? rows - 1 : rows);
    }

    /// <summary>Reads one CSV row, or <see langword="null"/> at the end of the file.</summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The cells.</returns>
    /// <remarks>
    /// Enough of Python's dialect to read back what these classes write: quotes protect commas,
    /// quotes and newlines, and a doubled quote inside a quoted cell is one quote.
    /// </remarks>
    internal static List<string>? ReadRow(TextReader reader)
    {
        var cells = new List<string>();
        var cell = new StringBuilder();
        var quoted = false;
        var any = false;

        while (true)
        {
            var read = reader.Read();
            if (read < 0)
            {
                if (!any && cells.Count == 0 && cell.Length == 0)
                {
                    return null;
                }

                break;
            }

            any = true;
            var character = (char)read;

            if (quoted)
            {
                if (character == '"')
                {
                    if (reader.Peek() == '"')
                    {
                        reader.Read();
                        cell.Append('"');
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else
                {
                    cell.Append(character);
                }

                continue;
            }

            switch (character)
            {
                case '"':
                    quoted = true;
                    break;

                case ',':
                    cells.Add(cell.ToString());
                    cell.Clear();
                    break;

                case '\r':
                    break;

                case '\n':
                    cells.Add(cell.ToString());
                    return cells;

                default:
                    cell.Append(character);
                    break;
            }
        }

        cells.Add(cell.ToString());

        return cells;
    }

    /// <summary>The first row of a file, or <see langword="null"/> when it has none.</summary>
    /// <param name="path">The file.</param>
    /// <returns>The cells.</returns>
    internal static List<string>? FirstRow(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8);

        return ReadRow(reader);
    }

    /// <summary>A file object recorded as a string, reduced to its path.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The path, when the cell was a file object; the cell itself otherwise.</returns>
    /// <remarks>
    /// The Python version's <c>simplify_file_data_in_str</c>. A log is read by people and by
    /// spreadsheets, and a whole file object in one cell is neither.
    /// </remarks>
    internal static string SimplifyFileData(string cell)
    {
        JsonNode? parsed;
        try
        {
            parsed = JsonNode.Parse(cell);
        }
        catch (System.Text.Json.JsonException)
        {
            return cell;
        }

        var simplified = ReplaceFiles(parsed);

        return simplified is JsonValue scalar && scalar.TryGetValue<string>(out var path)
            ? path
            : PythonJson.Dump(simplified);
    }

    /// <summary>Replaces every file object in a value with the path it names.</summary>
    /// <param name="node">The value.</param>
    /// <returns>The value, with its files reduced to paths.</returns>
    /// <remarks>
    /// Every file object, not just one at the top: a gallery's answer is a list of them and a JSON
    /// component's may be a document full of them, and the Python version walks the whole value for
    /// the same reason — a cell that kept one file object would be a cell a spreadsheet cannot
    /// show.
    /// </remarks>
    private static JsonNode? ReplaceFiles(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject file when Component.IsFileObject(file):
                return JsonValue.Create(JsonValues.Text(file["path"]) ?? string.Empty);

            case JsonObject map:
            {
                var copy = new JsonObject();

                foreach (var (key, child) in map)
                {
                    copy[key] = ReplaceFiles(child);
                }

                return copy;
            }

            case JsonArray array:
            {
                var copy = new JsonArray();

                foreach (var child in array)
                {
                    copy.Add(ReplaceFiles(child));
                }

                return copy;
            }

            default:
                return node?.DeepClone();
        }
    }

    /// <summary>The directory one component's sample files go into.</summary>
    /// <param name="directory">The log's own directory.</param>
    /// <param name="label">The component's label, which names the directory.</param>
    /// <returns>The directory's path, which may not exist yet.</returns>
    /// <remarks>
    /// The label is the application's text, so it is made safe to use as a name; the same rule for
    /// all four loggers, because a log that was written into differently named directories
    /// depending on which logger wrote it would be a log nobody could find the files in.
    /// </remarks>
    internal static string SampleDirectory(string directory, string label) =>
        Path.Combine(directory, FileNames.StripInvalidCharacters(label));

    /// <summary>The name a component's column is headed with.</summary>
    /// <param name="component">The component.</param>
    /// <param name="index">Its position among the components being recorded.</param>
    /// <returns>The label, or the position's own name when there is none.</returns>
    /// <remarks>
    /// A label that is empty counts as none, which is the Python version's <c>getattr(component,
    /// "label", None) or f"component {idx}"</c>: a falsy label falls back to the position. The
    /// smallest logger is the one that does not ask — it names its directory after the label as it
    /// stands.
    /// </remarks>
    internal static string LabelOf(Component component, int index) =>
        string.IsNullOrEmpty(component.Label) ? $"component {index}" : component.Label;

    /// <summary>One cell of a sample, as a logger records it.</summary>
    /// <param name="component">The component the value belongs to.</param>
    /// <param name="value">The value the client sent.</param>
    /// <param name="directory">The directory the sample's files go into.</param>
    /// <param name="simplifyFileData">Whether a file object is reduced to its path.</param>
    /// <returns>The cell.</returns>
    /// <remarks>
    /// The two loggers that write a header share these two rules and no others. An update is
    /// recorded as the update rather than as a value — a sample that is a property update is not a
    /// value the component ever held, and turning it into one would record something the user never
    /// saw — and it is spelled the way Python spells a dict, because the log is the artifact. A
    /// missing value is an empty cell rather than the "null" a component would write, which is the
    /// guard the Python version puts in front of the component for these two and only these two.
    /// </remarks>
    internal static string Cell(
        Component component,
        JsonNode? value,
        string directory,
        bool simplifyFileData)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (PropertyUpdate.Is(value))
        {
            return PythonText.Repr(value);
        }

        var cell = component.Flag(value, directory);

        return simplifyFileData ? SimplifyFileData(cell) : cell;
    }
}

using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The oldest logger: every sample appended to one <c>log.csv</c>, with a header row and with the
/// three extra columns always present.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>ClassicCSVLogger</c>, kept for the applications that were written
/// against it. It is the predecessor of <see cref="CsvLogger"/> and differs in the three ways that
/// logger was written to fix: it writes a header, it always writes <c>flag</c> and <c>username</c>
/// columns even when they are empty, and it appends to one file whatever the columns say — so a
/// component added later produces rows that no longer line up with the header. That is the
/// behaviour, not a defect to repair: an application that chose this logger is reading the file it
/// has always read.
/// </para>
/// <para>
/// It also does not lock, which is the Python version's implementation: two users flagging at the
/// same moment can interleave a row. <see cref="CsvLogger"/> is what to use instead, and what an
/// interface that names no callback gets.
/// </para>
/// </remarks>
public sealed class ClassicCsvLogger : FlaggingCallback
{
    private IReadOnlyList<Component> _components = [];
    private string _directory = string.Empty;

    /// <summary>Creates the logger the way the Python version's default constructor does.</summary>
    public ClassicCsvLogger()
    {
    }

    /// <summary>Creates the logger with the choice the Python version's constructor
    /// takes.</summary>
    /// <param name="simplifyFileData">
    /// Whether a file value is recorded as its path rather than as the whole file object.
    /// </param>
    public ClassicCsvLogger(bool simplifyFileData)
    {
        SimplifyFileData = simplifyFileData;
    }

    /// <summary>Whether a file value is recorded as its path rather than as the whole file object.</summary>
    public bool SimplifyFileData { get; init; } = true;

    /// <summary>The file the rows are written to.</summary>
    public string? DatasetFile { get; private set; }

    /// <inheritdoc />
    public override void Setup(IReadOnlyList<Component> components, string directory)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentException.ThrowIfNullOrEmpty(directory);

        _components = components;
        _directory = directory;

        Directory.CreateDirectory(_directory);
        DatasetFile = Path.Combine(_directory, "log.csv");
    }

    /// <inheritdoc />
    /// <remarks>
    /// The columns are fixed by the components, in the order the interface flags them, followed by
    /// the flag option, the username and the time. A row is written even when a component's value is
    /// missing: the columns are the shape of the file, and a row with a hole in it is still a row.
    /// </remarks>
    public override int Flag(
        IReadOnlyList<JsonNode?> values,
        string? flagOption = null,
        string? username = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        var path = DatasetFile ?? Path.Combine(_directory, "log.csv");
        var isNew = !System.IO.File.Exists(path);
        var row = new List<object?>();

        for (var index = 0; index < _components.Count; index++)
        {
            var label = CsvFile.LabelOf(_components[index], index);
            var directory = CsvFile.SampleDirectory(_directory, label);
            var value = index < values.Count ? values[index] : null;

            row.Add(CsvFile.Cell(_components[index], value, directory, SimplifyFileData));
        }

        // Always the same three columns, which is the difference from CsvLogger: an application
        // reading this file was written expecting them. A sample flagged without an option is
        // recorded as "None" rather than as an empty cell, because that is what writing Python's
        // missing value into a CSV produces — and the username beside it is the empty string,
        // because the Python version guards that one and not this one.
        row.Add(flagOption);
        row.Add(username ?? string.Empty);
        row.Add(CsvText.Timestamp());

        if (isNew)
        {
            CsvFile.Append(path, Headers());
        }

        CsvFile.Append(path, row);

        return CsvFile.Count(path);
    }

    /// <summary>The header row: the components' labels, then the three extra columns.</summary>
    /// <returns>The cells.</returns>
    private List<object?> Headers()
    {
        var headers = new List<object?>(_components.Count + 3);

        for (var index = 0; index < _components.Count; index++)
        {
            headers.Add(CsvFile.LabelOf(_components[index], index));
        }

        headers.Add("flag");
        headers.Add("username");
        headers.Add("timestamp");

        return headers;
    }
}

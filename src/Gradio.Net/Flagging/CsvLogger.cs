using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// What happens to a sample a user flags: the interface hands it to one of these, and the
/// implementation decides where the record goes.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>FlaggingCallback</c>. The two methods are the two halves of the feature:
/// one is called once, before the first sample can be flagged, to say which components are being
/// recorded and where; the other is called for every sample, and returns how many have been
/// recorded so far — which is what the button's own feedback is drawn from.
/// </para>
/// <para>
/// The samples arrive as the values the client sent, not as the values a handler would be handed:
/// flagging records what the user submitted. <see cref="Component.Flag"/> is what turns one of
/// them into a cell.
/// </para>
/// </remarks>
public abstract class FlaggingCallback
{
    /// <summary>Tells the callback what will be flagged, and where to put it.</summary>
    /// <param name="components">The components whose values are recorded, in order.</param>
    /// <param name="directory">The directory the records go into.</param>
    public abstract void Setup(IReadOnlyList<Component> components, string directory);

    /// <summary>Records one flagged sample.</summary>
    /// <param name="values">One value per component, as the client sent them.</param>
    /// <param name="flagOption">The option the user chose, or <see langword="null"/> for the default.</param>
    /// <param name="username">Who flagged it, or <see langword="null"/> when nobody is signed in.</param>
    /// <returns>How many samples have been recorded.</returns>
    public abstract int Flag(
        IReadOnlyList<JsonNode?> values,
        string? flagOption = null,
        string? username = null);
}

/// <summary>
/// The default callback: one CSV row per flagged sample, in a file the callback creates and keeps
/// adding to.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>CSVLogger</c>, which is what an interface that names no callback gets.
/// It differs from the older logger in three ways that matter: it only carries a <c>flag</c> column
/// when an option was chosen and a <c>username</c> column when somebody was signed in, it starts a
/// new <c>dataset2.csv</c> when the set of columns changes rather than appending rows that no
/// longer line up, and it takes a lock so that two users flagging at once cannot interleave a row.
/// </para>
/// <para>
/// The file is written the way Python's <c>csv</c> module writes one — comma separated, quoted with
/// <c>"</c> only where a field needs it, and a CRLF after every row — because the file is the
/// artifact, and an application that reads it back with a spreadsheet should find what the Python
/// version would have written.
/// </para>
/// </remarks>
public sealed class CsvLogger : FlaggingCallback
{
    private readonly object _gate = new();
    private IReadOnlyList<Component> _components = [];
    private string _directory = string.Empty;
    private string? _path;
    private bool _first = true;

    /// <summary>Creates the logger the way the Python version's default constructor does.</summary>
    public CsvLogger()
    {
    }

    /// <summary>Creates the logger with the choices the Python version's constructor
    /// takes.</summary>
    /// <param name="simplifyFileData">
    /// Whether a file value is recorded as its path rather than as the whole file object.
    /// </param>
    /// <param name="verbose">Whether creating or reusing a dataset file is reported.</param>
    /// <param name="datasetFileName">
    /// The name of the file to write, or <see langword="null"/> for <c>dataset1.csv</c> and its
    /// successors.
    /// </param>
    public CsvLogger(bool simplifyFileData = true, bool verbose = true, string? datasetFileName = null)
    {
        SimplifyFileData = simplifyFileData;
        Verbose = verbose;
        DatasetFileName = datasetFileName;
    }

    /// <summary>Whether a file value is recorded as its path rather than as the whole file object.</summary>
    public bool SimplifyFileData { get; init; } = true;

    /// <summary>Whether creating or reusing a dataset file is reported.</summary>
    public bool Verbose { get; init; } = true;

    /// <summary>Where <see cref="Verbose"/> reports go.</summary> <value>The process's standard
    /// output, which is where the Python version's <c>print</c> goes.</value>
    /// <remarks>
    /// A writer rather than an <c>ILogger</c> because of when the callback is built: an application
    /// constructs it — or an <see cref="Interface"/> constructs the default one — before any host
    /// exists to hand out a logger, so there is nothing to inject. It is a seam all the same: a host
    /// that wants these two lines in its own log points it at one, and a test reads them without
    /// touching the process's console.
    /// </remarks>
    public TextWriter Output { get; init; } = Console.Out;

    /// <summary>The name of the file to write, or <see langword="null"/> for the numbered default.</summary>
    public string? DatasetFileName { get; init; }

    /// <summary>The file the rows are being written to, once one has been created.</summary>
    public string? DatasetFile => _path;

    /// <inheritdoc />
    public override void Setup(IReadOnlyList<Component> components, string directory)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentException.ThrowIfNullOrEmpty(directory);

        _components = components;
        _directory = directory;
        _path = null;
        _first = true;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The columns are decided by the first sample and then left alone, which is why a later sample
    /// carrying a flag option cannot add one: the Python version makes the same choice, and a log
    /// whose rows have different lengths is not a log.
    /// </remarks>
    public override int Flag(
        IReadOnlyList<JsonNode?> values,
        string? flagOption = null,
        string? username = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        lock (_gate)
        {
            if (_first)
            {
                var extra = new List<string>();
                if (flagOption is not null)
                {
                    extra.Add("flag");
                }

                if (username is not null)
                {
                    extra.Add("username");
                }

                CreateDatasetFile(extra);
                _first = false;
            }

            var row = new List<object?>();

            for (var index = 0; index < values.Count && index < _components.Count; index++)
            {
                var component = _components[index];
                var label = CsvFile.LabelOf(component, index);
                var directory = CsvFile.SampleDirectory(_directory, label);

                row.Add(CsvFile.Cell(component, values[index], directory, SimplifyFileData));
            }

            if (flagOption is not null)
            {
                row.Add(flagOption);
            }

            if (username is not null)
            {
                row.Add(username);
            }

            row.Add(CsvText.Timestamp());

            CsvFile.Append(_path!, row);

            return CsvFile.Count(_path!);
        }
    }

    /// <summary>Creates the file the rows go into, or finds the one they already go into.</summary>
    /// <param name="extraHeaders">The columns after the components' own.</param>
    /// <remarks>
    /// The headers are the components' labels, so a file whose columns would change starts a new
    /// one: the Python version scans <c>dataset*.csv</c> for the highest number and compares its
    /// first row, which is what keeps an application that added a component from appending to a
    /// file whose columns no longer describe it.
    /// </remarks>
    private void CreateDatasetFile(List<string> extraHeaders)
    {
        Directory.CreateDirectory(_directory);

        var headers = new List<string>(_components.Count + extraHeaders.Count + 1);
        for (var index = 0; index < _components.Count; index++)
        {
            headers.Add(CsvFile.LabelOf(_components[index], index));
        }

        headers.AddRange(extraHeaders);
        headers.Add("timestamp");

        if (DatasetFileName is { Length: > 0 } named)
        {
            _path = Path.Combine(_directory, named);
        }
        else
        {
            _path = NextDatasetFile(headers);
        }

        if (System.IO.File.Exists(_path))
        {
            if (Verbose)
            {
                Output.WriteLine($"Using existing dataset file at: {_path}");
            }

            return;
        }

        CsvFile.Append(_path, headers);

        if (Verbose)
        {
            Output.WriteLine($"Created dataset file at: {_path}");
        }
    }

    /// <summary>The file a row belongs in, given the columns it would have.</summary>
    /// <param name="headers">The columns.</param>
    /// <returns>The highest-numbered file if its columns agree, and a new one otherwise.</returns>
    /// <remarks>
    /// The comparison is against the columns as they were <em>written</em>, not as they were given:
    /// a label a spreadsheet could read as a formula goes into the file with an apostrophe in front,
    /// so comparing the file's first row with the bare label would make every restart look like a
    /// change of columns and start a file the log did not need.
    /// </remarks>
    private string NextDatasetFile(List<string> headers)
    {
        var files = Directory.GetFiles(_directory, "dataset*.csv");
        if (files.Length == 0)
        {
            return Path.Combine(_directory, "dataset1.csv");
        }

        var numbered = files
            .Select(file => (File: file, Number: NumberIn(Path.GetFileNameWithoutExtension(file))))
            .Where(entry => entry.Number is not null)
            .OrderByDescending(entry => entry.Number)
            .ToList();

        if (numbered.Count == 0)
        {
            return Path.Combine(_directory, "dataset1.csv");
        }

        var latest = numbered[0];
        var written = headers.Select(header => CsvText.Sanitize(header)).ToList();

        return CsvFile.FirstRow(latest.File) is { } existing && existing.SequenceEqual(written)
            ? latest.File
            : Path.Combine(_directory, $"dataset{latest.Number + 1}.csv");
    }

    /// <summary>The first number in a file's name, or <see langword="null"/> when it has none.</summary>
    /// <param name="name">The name, without its extension.</param>
    /// <returns>The number.</returns>
    private static int? NumberIn(string name)
    {
        var digits = new StringBuilder();

        foreach (var character in name)
        {
            if (char.IsAsciiDigit(character))
            {
                digits.Append(character);
            }
            else if (digits.Length > 0)
            {
                break;
            }
        }

        return digits.Length > 0 && int.TryParse(digits.ToString(), CultureInfo.InvariantCulture, out var number)
            ? number
            : null;
    }
}

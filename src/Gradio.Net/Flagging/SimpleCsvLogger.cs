using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The smallest logger: one <c>log.csv</c>, one row per sample, no header and no columns of its own.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>SimpleCSVLogger</c>, which exists to be read rather than to be used — it
/// is the example of what a callback has to do, and its two methods are the whole of it. What it
/// writes is what the components' own <see cref="Component.Flag"/> produced, in the order they were
/// given, with nothing added: no flag option, no username, no timestamp.
/// </para>
/// <para>
/// The row count it answers with is the number of rows in the file minus one, because the Python
/// version's reads the file as a log that starts with a header. This logger writes none, so the
/// first sample is reported as zero — which is the Python version's answer, and is kept rather than
/// corrected: the number is drawn on a button, and an application that switched loggers mid-flight
/// would otherwise see it jump.
/// </para>
/// </remarks>
public sealed class SimpleCsvLogger : FlaggingCallback
{
    private IReadOnlyList<Component> _components = [];
    private string _directory = string.Empty;

    /// <summary>Creates the logger.</summary>
    public SimpleCsvLogger()
    {
    }

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
    /// A sample's files go into a directory named after the component's label, which is the one thing
    /// this logger does beyond handing values to the components. Every sample is handed over as it
    /// is — this logger has no rule about updates and none about a missing value — so a cell is
    /// whatever the component makes of it, which is what makes it the example of the interface
    /// rather than a logger with opinions of its own.
    /// </remarks>
    public override int Flag(
        IReadOnlyList<JsonNode?> values,
        string? flagOption = null,
        string? username = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        var path = DatasetFile ?? Path.Combine(_directory, "log.csv");
        var row = new List<object?>();

        for (var index = 0; index < _components.Count; index++)
        {
            var label = _components[index].Label ?? string.Empty;
            var directory = CsvFile.SampleDirectory(_directory, label);
            Directory.CreateDirectory(directory);

            row.Add(_components[index].Flag(
                index < values.Count ? values[index] : null,
                directory));
        }

        CsvFile.Append(path, row);

        return CsvFile.Count(path, hasHeader: false) - 1;
    }
}

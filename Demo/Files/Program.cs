using System.Text;
using Gradio.Net;

// A file in, a table and a file out.
//
// Two rules meet in this page, and both are the port's rather than the demo's:
//
//  * A table is handed over as rows and headers. There is no pandas here, so a handler that wants a
//    table gets one built by hand — `DataframeTable` is the headers and the rows, and nothing is
//    inferred from them.
//  * A file that changes format is converted in the handler. Nothing in the package starts another
//    process, so there is no converter to call: what the file becomes, this code makes. That is the
//    same rule that keeps ffmpeg out of the package — an audio or video file would be transcoded
//    here too, with a library the application chose.
//
//     upload.change(convert, upload, [table, download])

static (DataframeTable Table, string Converted) Convert(string path)
{
    // The path is the cached copy of what was uploaded, so it stays readable while this runs.
    // `System.IO.File` is spelled out because `File` alone is the component in this file.
    var lines = System.IO.File.ReadAllLines(path);

    // A CSV reader in one line, because the demo is about the shapes rather than about CSV. A real
    // one would quote fields and handle the encoding; `Microsoft.VisualBasic.FileIO` and CsvHelper
    // both do, and either is a package reference away.
    static string[] Split(string line) =>
        line.Split(',').Select(field => field.Trim().Trim('"')).ToArray();

    string[] headers = lines.Length == 0 ? [] : Split(lines[0]);
    var rows = lines
        .Skip(1)
        .Where(line => line.Length > 0)
        .Select(line => (IReadOnlyList<object?>)Split(line).Cast<object?>().ToArray())
        .ToList();

    // The download: the same rows, tab separated. A format change the handler makes itself.
    var converted = Path.Combine(Path.GetTempPath(), $"rows-{Guid.NewGuid():N}.tsv");

    var text = new StringBuilder();
    text.AppendLine(string.Join('\t', headers));

    foreach (var row in rows)
    {
        text.AppendLine(string.Join('\t', row.Select(value => value?.ToString() ?? string.Empty)));
    }

    System.IO.File.WriteAllText(converted, text.ToString());

    return (new DataframeTable(headers, rows), converted);
}

var demo = gr.Blocks(title: "Files");

using (demo)
{
    gr.Markdown(
        """
        # Files

        Upload a CSV file. The page shows its rows as a table and offers the same rows back as a
        tab-separated file — both made by the handler, not by a library the framework ships.
        """);

    var upload = gr.File(label: "CSV file", fileTypes: [".csv"], type: "filepath");

    var table = gr.Dataframe(label: "Rows", interactive: false);

    var download = gr.File(label: "As tab-separated text", interactive: false);

    // Two outputs, so the handler answers with a tuple: the first value fills the table, the second
    // the download. The typed `Change` takes a handler that answers one
    // value; a handler that answers two is bound raw, with its own signature describing both.
    upload.Change(
        Convert,
        [upload],
        [table, download],
        apiName: "convert");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using File = System.IO.File;
using Xunit;

namespace Gradio.Net.Tests.Flagging;

/// <summary>
/// What each logger writes, held to the file the Python version's logger writes for the same
/// samples.
/// </summary>
/// <remarks>
/// <para>
/// The assertion is the file's bytes, not a parsed structure. A log is an artifact: it is opened in a
/// spreadsheet, appended to by a second process, and read back by an application that was written
/// against the file it has always had. A logger that wrote the same rows in a different dialect
/// would be a different file, and "the same rows" is not what anybody's reader is built on.
/// </para>
/// <para>
/// The three loggers differ in exactly three things — a header or not, the extra columns always or
/// only when they have something in them, and one file or numbered ones — and each difference has a
/// test here.
/// </para>
/// </remarks>
public sealed class FlaggingLogTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "gradio-flagging-" + Guid.NewGuid().ToString("N"));

    public FlaggingLogTests() => Directory.CreateDirectory(_root);

    [Fact]
    public void The_simple_logger_writes_one_headerless_row_per_sample()
    {
        var logger = new SimpleCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        Assert.Equal(0, logger.Flag([JsonValue.Create("a")], flagOption: "wrong", username: "ada"));
        Assert.Equal(1, logger.Flag([JsonValue.Create("b")]));

        // No header, no flag column, no username column: the row is the sample and nothing else.
        Assert.Equal("a\r\nb\r\n", Read("log.csv"));
    }

    [Fact]
    public void The_classic_logger_always_writes_the_three_extra_columns()
    {
        var logger = new ClassicCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([JsonValue.Create("a")], flagOption: "wrong", username: "ada");

        var lines = Lines("log.csv");

        Assert.Equal("Input,flag,username,timestamp", lines[0]);
        Assert.StartsWith("a,wrong,ada,", lines[1], StringComparison.Ordinal);
        Assert.Equal(2, lines.Length);
    }

    [Fact]
    public void The_classic_logger_writes_its_header_once()
    {
        var logger = new ClassicCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([JsonValue.Create("a")]);
        logger.Flag([JsonValue.Create("b")]);

        var lines = Lines("log.csv");

        // A header per sample would be a file no reader can use; the columns are written when the
        // file is created and never again.
        Assert.Equal(3, lines.Length);
        Assert.Equal("Input,flag,username,timestamp", lines[0]);
        Assert.StartsWith("a,", lines[1], StringComparison.Ordinal);
        Assert.StartsWith("b,", lines[2], StringComparison.Ordinal);
    }

    [Fact]
    public void The_classic_logger_counts_the_samples_it_has_written()
    {
        var logger = new ClassicCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        Assert.Equal(1, logger.Flag([JsonValue.Create("a")]));
        Assert.Equal(2, logger.Flag([JsonValue.Create("b")]));
    }

    [Fact]
    public void The_default_logger_writes_only_the_columns_it_has_something_for()
    {
        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([JsonValue.Create("a")]);

        var lines = Lines(Path.Combine("dataset1.csv"));

        Assert.Equal("Input,timestamp", lines[0]);
        Assert.StartsWith("a,", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public void The_default_logger_adds_the_flag_and_username_columns_when_the_first_sample_has_them()
    {
        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([JsonValue.Create("a")], flagOption: "wrong", username: "ada");

        var lines = Lines("dataset1.csv");

        Assert.Equal("Input,flag,username,timestamp", lines[0]);
        Assert.StartsWith("a,wrong,ada,", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public void A_sample_is_written_into_a_directory_named_after_its_component()
    {
        var logger = new SimpleCsvLogger();
        logger.Setup([new Textbox { Label = "Why: this?" }], _root);

        logger.Flag([JsonValue.Create("a")]);

        // The label is a directory name, so the characters a file system refuses are removed rather
        // than escaped: "Why: this?" becomes "Why this".
        Assert.True(Directory.Exists(Path.Combine(_root, "Why this")));
    }

    [Fact]
    public void The_default_logger_starts_a_new_file_when_the_columns_change()
    {
        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Textbox { Label = "Input" }], _root);
        logger.Flag([JsonValue.Create("a")]);

        // A second interface flagging into the same directory with different components: the rows
        // no longer describe the header, so they get a file of their own.
        var second = new CsvLogger(verbose: false);
        second.Setup([new Textbox { Label = "Input" }, new Textbox { Label = "Output" }], _root);
        second.Flag([JsonValue.Create("b"), JsonValue.Create("c")]);

        Assert.Equal("Input,timestamp", Lines("dataset1.csv")[0]);
        Assert.Equal("Input,Output,timestamp", Lines("dataset2.csv")[0]);
    }

    [Fact]
    public void The_default_logger_reuses_its_file_when_a_label_needs_sanitizing()
    {
        var first = new CsvLogger(verbose: false);
        first.Setup([new Textbox { Label = "-Input" }], _root);
        first.Flag([JsonValue.Create("a")]);

        // A label a spreadsheet could read as a formula is written with an apostrophe in front, so
        // the file's first row holds the apostrophe. Comparing that row against the bare label
        // would make every restart look like a change of columns.
        var second = new CsvLogger(verbose: false);
        second.Setup([new Textbox { Label = "-Input" }], _root);
        second.Flag([JsonValue.Create("b")]);

        Assert.Single(Directory.GetFiles(_root, "dataset*.csv"));

        var lines = Lines("dataset1.csv");

        Assert.Equal("'-Input,timestamp", lines[0]);
        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void The_classic_logger_writes_none_for_a_sample_flagged_without_an_option()
    {
        var logger = new ClassicCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([JsonValue.Create("a")]);

        // The three extra columns are always written, and an option that was not chosen is written
        // the way Python writes a missing value — "None", not an empty cell. The username beside it
        // is the empty string, because the Python version guards that one and not this one.
        Assert.StartsWith("a,None,,", Lines("log.csv")[1], StringComparison.Ordinal);
    }

    [Fact]
    public void An_update_is_recorded_as_python_spells_it()
    {
        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([Update()]);

        // A sample that is a property update is written as the update itself, spelled the way
        // Python spells a dict: single quotes, and True where JSON writes true. A spreadsheet cannot
        // hold the object, so what is recorded is the text of it.
        Assert.Contains(
            """{'__type__': 'update', 'value': 'x', 'visible': True}""",
            Read("dataset1.csv"),
            StringComparison.Ordinal);
    }

    [Fact]
    public void The_simple_logger_hands_an_update_to_the_component()
    {
        var logger = new SimpleCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([Update()]);

        // This logger has no rule of its own about updates: it hands the sample to the component,
        // and a component writes a value it cannot call text as JSON.
        Assert.Contains("{\"\"__type__\"\": \"\"update\"\"", Read("log.csv"), StringComparison.Ordinal);
        Assert.DoesNotContain("'__type__'", Read("log.csv"), StringComparison.Ordinal);
    }

    [Fact]
    public void The_simple_logger_writes_a_missing_value_the_way_json_does()
    {
        var logger = new SimpleCsvLogger();
        logger.Setup([new Textbox { Label = "Input" }], _root);

        logger.Flag([null]);

        // Handing the sample over is what makes this logger simple, and a component writes a value
        // it was given nothing for as "null" — the two loggers that guard the sample themselves
        // write an empty cell instead.
        Assert.Equal("null\r\n", Read("log.csv"));
    }

    [Fact]
    public void A_column_without_a_label_is_named_after_its_position()
    {
        var classic = new ClassicCsvLogger();
        classic.Setup([new Textbox { Label = "" }, new Textbox()], _root);
        classic.Flag([JsonValue.Create("a"), JsonValue.Create("b")]);

        // An empty label counts as none — the Python version's `label or f"component {idx}"` — so the column
        // is named after the position rather than left blank, and the sample's files go into a
        // directory named that way rather than into the log's own.
        Assert.Equal("component 0,component 1,flag,username,timestamp", Lines("log.csv")[0]);

        Directory.CreateDirectory(Path.Combine(_root, "csv"));

        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Textbox { Label = "" }], Path.Combine(_root, "csv"));
        logger.Flag([JsonValue.Create("a")]);

        Assert.Equal("component 0,timestamp", Lines(Path.Combine("csv", "dataset1.csv"))[0]);
    }

    [Fact]
    public void A_chat_rating_that_says_nothing_is_written_the_way_a_missing_value_is()
    {
        var logger = new ChatCsvLogger();
        logger.Setup(_root);

        logger.Flag(new LikeData(JsonNode.Parse("""{"index": 0, "liked": null}""")), new JsonArray());

        // The rating is passed through and the cell writer spells a missing value "None" — the same
        // rule as the flag column of the classic log. Four cells under five headers is the Python version's
        // own defect, and it is reproduced.
        var lines = Lines("log.csv");

        Assert.Equal("conversation,index,value,flag,timestamp", lines[0]);
        Assert.StartsWith("[],0,None,", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public void Two_samples_whose_files_share_a_name_are_both_kept()
    {
        var first = Write("one", "photo.png", "first");
        var second = Write("two", "photo.png", "second");

        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Image { Label = "Photo" }], _root);

        logger.Flag([FileObject(first)]);
        logger.Flag([FileObject(second)]);

        var cells = Lines("dataset1.csv")
            .Skip(1)
            .Select(line => line.Split(',')[0])
            .ToArray();

        // Each copy gets a directory of its own, which is the Python version's rule: two samples whose files
        // are both called "photo.png" are two files rather than one that overwrote the other — the
        // first row would otherwise point at the second one's bytes.
        Assert.NotEqual(cells[0], cells[1]);
        Assert.Equal("first", System.IO.File.ReadAllText(cells[0]));
        Assert.Equal("second", System.IO.File.ReadAllText(cells[1]));
    }

    [Fact]
    public void A_file_value_is_recorded_as_its_path()
    {
        var path = Path.Combine(_root, "photo.png");
        System.IO.File.WriteAllText(path, "x");

        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Image { Label = "Photo" }], _root);

        // A file object is what the client sends and what a component's own flagging copies; a log
        // that held the whole object in one cell would be unreadable in a spreadsheet. What is
        // recorded is where the copy went — the log owns its own files.
        logger.Flag([FileObject(path)]);

        var cell = Lines("dataset1.csv")[1].Split(',')[0];

        Assert.StartsWith(Path.Combine(_root, "Photo"), cell, StringComparison.Ordinal);
        Assert.EndsWith("photo.png", cell, StringComparison.Ordinal);
        Assert.DoesNotContain("meta", Read("dataset1.csv"), StringComparison.Ordinal);
    }

    [Fact]
    public void A_file_value_is_kept_whole_when_the_logger_was_asked_to()
    {
        var path = Path.Combine(_root, "photo.png");
        System.IO.File.WriteAllText(path, "x");

        var logger = new CsvLogger(simplifyFileData: false, verbose: false, datasetFileName: "kept.csv");
        logger.Setup([new Image { Label = "Photo" }], _root);

        // The example cache reads its rows back into file objects, so it asks for the object rather
        // than the path.
        logger.Flag([FileObject(path)]);

        Assert.Contains("\"meta\"", Read("kept.csv"), StringComparison.Ordinal);
    }

    [Fact]
    public void A_json_cell_is_written_the_way_python_writes_it()
    {
        var logger = new CsvLogger(verbose: false);
        logger.Setup([new JSON { Label = "Document" }], _root);

        logger.Flag([JsonNode.Parse("""{"a":1,"b":[1,-2]}""")]);

        var written = Read("dataset1.csv");

        // Normalized rather than escaped: Python's json.dumps puts a space after the colon and after
        // the comma, and a list that happens to contain a negative number is not a formula — so no
        // apostrophe goes in front of it.
        Assert.Contains("\"\"a\"\": 1", written, StringComparison.Ordinal);
        Assert.Contains("[1, -2]", written, StringComparison.Ordinal);
        Assert.DoesNotContain("'[", written, StringComparison.Ordinal);
    }

    [Fact]
    public void A_file_inside_a_list_is_recorded_as_its_path_too()
    {
        var path = Path.Combine(_root, "photo.png");
        System.IO.File.WriteAllText(path, "x");

        var logger = new CsvLogger(verbose: false);
        logger.Setup([new Gallery { Label = "Pictures" }], _root);

        logger.Flag([new JsonArray(FileObject(path))]);

        // A gallery's answer is a list of file objects, and every one of them is reduced: a cell
        // that kept one would be a cell a spreadsheet cannot show.
        var cell = Lines("dataset1.csv")[1];

        Assert.Contains("photo.png", cell, StringComparison.Ordinal);
        Assert.DoesNotContain("meta", cell, StringComparison.Ordinal);
    }

    /// <summary>A file object the way a browser sends one.</summary>
    private static JsonObject FileObject(string path) =>
        new()
        {
            ["path"] = path,
            ["url"] = "http://localhost/gradio_api/file=" + Path.GetFileName(path),
            ["size"] = 1,
            ["orig_name"] = Path.GetFileName(path),
            ["mime_type"] = "image/png",
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        };

    /// <summary>A sample that is a property update rather than a value.</summary>
    private static JsonObject Update() =>
        new()
        {
            ["__type__"] = "update",
            ["value"] = "x",
            ["visible"] = true,
        };

    /// <summary>Writes a file into a directory of its own, and answers with its path.</summary>
    private string Write(string directory, string name, string content)
    {
        var folder = Path.Combine(_root, directory);
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, name);
        System.IO.File.WriteAllText(path, content);

        return path;
    }

    private string Read(string name) => System.IO.File.ReadAllText(Path.Combine(_root, name), Encoding.UTF8);

    private string[] Lines(string name) =>
        Read(name).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}

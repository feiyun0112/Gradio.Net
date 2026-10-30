using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A table of values a handler can return, or that a user can edit.
/// </summary>
/// <param name="Headers">The column headers.</param>
/// <param name="Rows">The rows, each with one value per header.</param>
/// <remarks>
/// The .NET stand-in for the dataframe a handler reads when <see cref="Dataframe.Type"/> is
/// <c>pandas</c> or <c>polars</c>: this framework carries no dataframe library, and a table with
/// named columns is the shape a handler needs. <c>array</c> and <c>numpy</c> hand over the rows
/// alone, which is what those two types are.
/// </remarks>
public sealed record DataframeTable(IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<object?>> Rows);

/// <summary>
/// A spreadsheet-like table: editable, searchable, with columns that can be pinned or held still
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The 6.x parameter set is the one that matters here: the
/// two counts are pairs of a number and a mode, the deprecated <c>col_count</c> is kept beside
/// <c>column_count</c> because the Python version still writes both, and the toolbar options —
/// search, row numbers, pinned and static columns — are what makes the table usable rather than
/// merely displayed.
/// </para>
/// <para>
/// The value is postprocessed on read, as a file component's is: a table's wire form is an object
/// with headers, rows and metadata, and the headers a table that was given none carries are derived
/// from its column count. Reading it back through the postprocessing is what makes the config say
/// what the Python version says.
/// </para>
/// </remarks>
[GradioBlock("dataframe")]
[HandlerValue("object?", "change", "input", "select", "edit")]
public partial class Dataframe : Component
{
    internal Dataframe()
    {
    }

    /// <summary>The datatypes a column may be.</summary>
    private static readonly IReadOnlyList<string> _columnTypes =
        ["str", "number", "bool", "date", "markdown", "html", "image", "auto"];

    /// <summary>The shapes a value may be handed to a handler as.</summary>
    private static readonly IReadOnlyList<string> _resultTypes = ["pandas", "numpy", "array", "polars"];

    /// <summary>The searches the toolbar offers.</summary>

    /// <summary>The delimiters LaTeX is rendered between when the application names none.</summary>
    private static readonly IReadOnlyList<LatexDelimiter> _defaultLatexDelimiters = [new LatexDelimiter()];

    private object? _value;
    private IReadOnlyList<string>? _headers;
    private object? _rowCount;
    private object? _columnCount;
    private object? _datatype = "str";
    private string _type = "pandas";
    private IReadOnlyList<LatexDelimiter>? _latexDelimiters;
    private IReadOnlyList<object?>? _columnWidths;

    /// <summary>The table the component starts with.</summary>
    /// <value>
    /// A list of rows, a list of scalars (one column), a table, a document with <c>headers</c> and
    /// <c>data</c>, or the path of a CSV file; read back as the postprocessed value — the object the
    /// frontend draws.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>The column headers.</summary>
    /// <value>
    /// The application's headers, or — when it named none — the column positions as text, which is
    /// what the Python version carries.
    /// </value>
    /// <exception cref="ArgumentException">The headers do not match the column count.</exception>
    public IReadOnlyList<string> Headers
    {
        get
        {
            var count = ColumnCountNumber;

            if (_headers is not { } headers)
            {
                return [.. Enumerable.Range(1, count)
                    .Select(position => position.ToString(CultureInfo.InvariantCulture))];
            }

            if (headers.Count != count)
            {
                throw new ArgumentException(
                    $"The length of the headers list must be equal to the column_count. The column "
                    + $"count is set to {count} but 'headers' has {headers.Count} items.",
                    nameof(Headers));
            }

            return headers;
        }

        set => _headers = value;
    }

    /// <summary>The number of rows the table starts with, and whether it may grow.</summary>
    /// <value>A number, or a pair of a number and <c>dynamic</c> or <c>fixed</c>.</value>
    public object? RowCount
    {
        get => CountOf(_rowCount, 3);
        set => _rowCount = value;
    }

    /// <summary>The fewest and most rows the user may leave the table with, or
    /// <see langword="null"/> for no limit.</summary> <remarks>Carried in the config but not
    /// enforced by the server: the Python version marks the parameter as not yet
    /// implemented.</remarks>
    public IReadOnlyList<int?>? RowLimits { get; set; }

    /// <summary>The deprecated spelling of <see cref="ColumnCount"/>.</summary>
    /// <value>The same pair <see cref="ColumnCount"/> carries.</value>
    /// <remarks>
    /// The Python version accepts the parameter, warns that it is going away, and then overwrites
    /// it with <c>column_count</c>; the config carries both keys with the same value. Setting it
    /// therefore does nothing, which is what the Python version does too.
    /// </remarks>
    public object? ColCount
    {
        get => ColumnCount;
        set
        {
        }
    }

    /// <summary>The number of columns the table starts with, and whether it may grow.</summary>
    /// <value>A number, or a pair of a number and <c>dynamic</c> or <c>fixed</c>.</value>
    public object? ColumnCount
    {
        get => CountOf(_columnCount, _headers is { Count: > 0 } headers ? headers.Count : 3);
        set => _columnCount = value;
    }

    /// <summary>The fewest and most columns the user may leave the table with, or <see langword="null"/> for no limit.</summary>
    /// <remarks>Carried in the config but not enforced by the server, for the same reason as <see cref="RowLimits"/>.</remarks>
    public IReadOnlyList<int?>? ColumnLimits { get; set; }

    /// <summary>What each column holds, as one name for every column or one per column.</summary>
    /// <value>
    /// <c>str</c>, <c>number</c>, <c>bool</c>, <c>date</c>, <c>markdown</c>, <c>html</c>,
    /// <c>image</c>, or <c>auto</c> to infer it from the value.
    /// </value>
    public object? Datatype
    {
        get => _datatype switch
        {
            null => "str",
            "auto" => InferDatatype(),
            var declared => declared,
        };
        set => _datatype = value;
    }

    /// <summary>The shape a value is handed to a handler as.</summary>
    /// <value><c>pandas</c>, <c>numpy</c>, <c>array</c> or <c>polars</c>; anything else is refused.</value>
    /// <remarks>
    /// <para>
    /// Four names for two shapes. <c>pandas</c> and <c>polars</c> name a table — headers with rows
    /// — and here that is <see cref="DataframeTable"/>; <c>array</c> and <c>numpy</c> name the rows
    /// on their own, which is a list of lists. The two array names answer with the same shape on
    /// purpose rather than by omission: a numpy array is how Python carries a grid of values, and a
    /// list of rows is how C# carries one, so there is nothing for the second name to add.
    /// </para>
    /// <para>
    /// What the two table names do <em>not</em> bring is the libraries: a Styler and date parsing
    /// are the application's, and nothing here is a pandas or a polars object. The cells keep the
    /// types they arrived with — a number is a number — and <c>datatype="auto"</c> still names each
    /// column's type from its first row, which is the Python version's own rule. A handler that
    /// wants a data frame builds one from the table it was given.
    /// </para>
    /// </remarks>
    public string Type
    {
        get => _type;
        set
        {
            if (!_resultTypes.Contains(value, StringComparer.Ordinal))
            {
                throw new ArgumentException(
                    $"Invalid value for parameter 'type': {value}. Please choose from one of: "
                    + $"{string.Join(", ", _resultTypes)}",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The delimiters LaTeX is rendered between.</summary>
    /// <value>One pair of <c>$$</c> delimiters unless the application says otherwise.</value>
    public IReadOnlyList<LatexDelimiter> LatexDelimiters
    {
        get => _latexDelimiters ?? _defaultLatexDelimiters;
        set => _latexDelimiters = value;
    }

    /// <summary>The label drawn above the table.</summary>
    public override string? Label { get; set; }

    /// <summary>The greatest height of the table, in pixels or as a CSS length.</summary>
    public object? MaxHeight { get; set; } = 500;

    /// <summary>The relative width of the table next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the table wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether the text in a cell wraps.</summary>
    public bool Wrap { get; set; }

    /// <summary>Whether a single newline in a markdown cell is a line break.</summary>
    public bool LineBreaks { get; set; } = true;

    /// <summary>The width of each column, in pixels or as a percentage.</summary>
    /// <value>
    /// What the application asked for, with a bare number written as pixels — the Python version's
    /// normalization, done here so that the config carries the widths the frontend reads.
    /// </value>
    public IReadOnlyList<object?> ColumnWidths
    {
        get => _columnWidths is null ? [] : Widths(_columnWidths);
        set => _columnWidths = value;
    }

    /// <summary>The buttons drawn in the table's corner.</summary>
    /// <value>
    /// <see langword="null"/> unless the application names them, which is the Python version's
    /// answer: the frontend shows its own set when the key is absent. <c>fullscreen</c> and
    /// <c>copy</c> are the two it knows.
    /// </value>
    public IReadOnlyList<string>? Buttons { get; set; }

    /// <summary>Whether a column of row numbers is drawn.</summary>
    public bool ShowRowNumbers { get; set; }

    /// <summary>How many characters a cell shows before truncating, or <see langword="null"/> for all of them.</summary>
    public int? MaxChars { get; set; }

    /// <summary>The search the toolbar offers.</summary>
    /// <value><c>none</c> for no toolbar, <c>search</c> to filter, or <c>filter</c> to narrow.</value>
    /// <remarks>
    /// Not validated, because the Python version does not validate it: the value is carried to the
    /// frontend, which draws the toolbar it names. Refusing it here would make a page the Python
    /// version serves unreachable.
    /// </remarks>
    public string ShowSearch { get; set; } = "none";

    /// <summary>How many columns from the left are pinned while the table scrolls sideways.</summary>
    public int? PinnedColumns { get; set; }

    /// <summary>The columns the user may not edit.</summary>
    public IReadOnlyList<int> StaticColumns { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => DataSchemas.DataframeSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["headers"] = new JsonArray("a", "b"),
        ["data"] = new JsonArray(new JsonArray("foo", "bar")),
    };

    /// <summary>The table a handler reads.</summary>
    /// <param name="value">What the client sent: an object with headers and rows.</param>
    /// <returns>
    /// The rows for <c>array</c> and <c>numpy</c>, a <see cref="DataframeTable"/> for the two
    /// dataframe types, or <see langword="null"/> when nothing was sent.
    /// </returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonObject document)
        {
            return null;
        }

        var headers = document["headers"] is JsonArray headerArray
            ? headerArray.Select(Text).ToArray()
            : [];

        var rows = document["data"] is JsonArray data && !IsEmpty(data)
            ? RowsOf(data)
            : [];

        return Type is "array" or "numpy" ? rows : new DataframeTable(headers, rows);
    }

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">A table, rows, scalars, a document, a CSV path, or none.</param>
    /// <returns>The table as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is of a shape a table cannot be made of.</exception>
    public override JsonNode? Postprocess(object? value)
    {
        var headers = HeadersOf(value);
        var data = IsEmpty(value) ? [] : CellsOf(value);
        var effective = headers.Count > 0 ? headers.ToList() : [.. Headers];

        if (data.Count == 0)
        {
            return Table(effective, [], MetadataOf(value));
        }

        var width = data[0].Count;

        if (effective.Count > width)
        {
            effective = [.. effective.Take(width)];
        }
        else if (effective.Count < width)
        {
            effective = [.. effective, .. Enumerable.Range(effective.Count + 1, width - effective.Count)
                .Select(position => position.ToString(CultureInfo.InvariantCulture))];
        }

        return Table(effective, data, MetadataOf(value));
    }

    /// <summary>The number of columns, as the first half of the pair the config carries.</summary>
    private int ColumnCountNumber =>
        Convert.ToInt32(((IReadOnlyList<object?>)ColumnCount!)[0], CultureInfo.InvariantCulture);

    /// <summary>What each column holds, inferred from the first row.</summary>
    /// <returns>One name per column, or <c>str</c> for a table with no rows.</returns>
    private object InferDatatype()
    {
        var rows = IsEmpty(_value) ? [] : CellsOf(_value!);

        return rows.Count == 0 ? "str" : rows[0].Select(Category).ToArray();
    }

    /// <summary>The datatype a value is a column of.</summary>
    private static string Category(object? cell) => cell switch
    {
        bool => "bool",
        int or long or double or float or decimal => "number",
        DateTime or DateTimeOffset => "date",
        _ => "str",
    };

    /// <summary>A count and its mode, as the pair the Python version stores.</summary>
    /// <param name="count">What the application set: a number, a pair, or nothing.</param>
    /// <param name="fallback">The count to use when nothing was set.</param>
    private static IReadOnlyList<object?> CountOf(object? count, int fallback) => count switch
    {
        null => [fallback, "dynamic"],
        int number => [number, "dynamic"],
        long number => [(int)number, "dynamic"],
        ValueTuple<int, string> pair => [pair.Item1, pair.Item2],
        IReadOnlyList<object?> pair when pair.Count == 2 => [pair[0], pair[1]],
        _ => throw new ArgumentException(
            $"A dataframe's count is a number or a pair of a number and a mode; got a "
            + $"{count.GetType()}.",
            nameof(count)),
    };

    /// <summary>The widths, with a bare number written as pixels.</summary>
    private static IReadOnlyList<object?> Widths(IReadOnlyList<object?> widths) =>
    [
        .. widths.Select(width => width switch
        {
            string text when text.EndsWith("px", StringComparison.Ordinal)
                || text.EndsWith('%')
                || string.Equals(text, "auto", StringComparison.Ordinal) => text,
            _ => $"{Convert.ToString(width, CultureInfo.InvariantCulture)}px",
        }),
    ];

    /// <summary>Whether a value holds no rows.</summary>
    private static bool IsEmpty(object? value) => value switch
    {
        null => true,
        DataframeTable table => table.Rows.Count == 0 || table.Rows[0].Count == 0,
        JsonObject document when document["data"] is JsonArray rows =>
            rows.Count == 0 || rows[0] is JsonArray { Count: 0 },
        JsonArray rows => rows.Count == 0 || rows[0] is JsonArray { Count: 0 },
        IReadOnlyList<object?> rows =>
            rows.Count == 0 || rows[0] is IReadOnlyList<object?> { Count: 0 },
        string path => new FileInfo(path).Length == 0,
        _ => false,
    };

    /// <summary>The headers a value carries, or none when it carries none.</summary>
    private static IReadOnlyList<string> HeadersOf(object? value) => value switch
    {
        DataframeTable table => table.Headers,
        JsonObject document when document["headers"] is JsonArray headers => [.. headers.Select(Text)],
        IReadOnlyDictionary<string, object?> document
            when document.TryGetValue("headers", out var headers)
            && headers is IReadOnlyList<object?> items => [.. items.Select(TextOf)],
        string path => CsvRows(path) is { Count: > 0 } rows
            ? [.. rows[0].Select(cell => cell?.ToString() ?? string.Empty)]
            : [],
        _ => [],
    };

    /// <summary>The styling a value carries, or <see langword="null"/> for none.</summary>
    private static JsonNode? MetadataOf(object? value) => value switch
    {
        JsonObject document when document["metadata"] is { } metadata => metadata.DeepClone(),
        IReadOnlyDictionary<string, object?> document
            when document.TryGetValue("metadata", out var metadata) => JsonValues.FromAnyObject(metadata),
        _ => null,
    };

    /// <summary>The rows a value holds.</summary>
    /// <exception cref="ArgumentException">The value is of a shape no row list can be made from.</exception>
    private static List<IReadOnlyList<object?>> CellsOf(object? value) => value switch
    {
        null => [],
        DataframeTable table => [.. table.Rows],
        JsonObject document => RowsOf(document["data"] as JsonArray ?? []),
        JsonArray rows => RowsOf(rows),
        string path => [.. CsvRows(path).Skip(1)],
        IReadOnlyList<object?> rows => RowsOf(rows),
        _ => throw new ArgumentException(
            $"A dataframe is made of a table, a list of rows, a list of values, a document with "
            + $"'data', or a CSV path; it was handed a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>The rows of a JSON array: rows of cells, or one column of values.</summary>
    private static List<IReadOnlyList<object?>> RowsOf(JsonArray rows)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var result = new List<IReadOnlyList<object?>>(rows.Count);
        var singleColumn = rows[0] is not JsonArray;

        foreach (var row in rows)
        {
            var cells = row as JsonArray;
            result.Add(singleColumn || cells is null
                ? new object?[] { JsonValues.ToObject(row) }
                : [.. cells.Select(JsonValues.ToObject)]);
        }

        return result;
    }

    /// <summary>The rows of a CLR list: rows of cells, or one column of values.</summary>
    private static List<IReadOnlyList<object?>> RowsOf(IReadOnlyList<object?> rows)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var result = new List<IReadOnlyList<object?>>(rows.Count);
        var singleColumn = rows[0] is not IReadOnlyList<object?>;

        foreach (var row in rows)
        {
            result.Add(singleColumn || row is not IReadOnlyList<object?> cells
                ? new object?[] { Cell(row) }
                : [.. cells.Select(Cell)]);
        }

        return result;
    }

    /// <summary>A cell as the JSON it carries.</summary>
    private static object? Cell(object? value) =>
        value is JsonNode node ? JsonValues.ToObject(node) : value;

    /// <summary>A table as the protocol carries it.</summary>
    private static JsonObject Table(
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<object?>> rows,
        JsonNode? metadata = null) => new()
    {
        ["headers"] = new JsonArray([.. headers.Select(header => (JsonNode?)JsonValue.Create(header))]),
        ["data"] = new JsonArray([.. rows.Select(row => (JsonNode?)new JsonArray(
            [.. row.Select(cell => JsonValues.FromAnyObject(cell))]))]),
        ["metadata"] = metadata,
    };

    /// <summary>A header as text, whatever the client sent.</summary>
    private static string Text(JsonNode? node) =>
        JsonValues.Text(node) ?? JsonValues.Number(node)?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

    /// <summary>A header a CLR value carries, as text.</summary>
    private static string TextOf(object? value) => value switch
    {
        null => string.Empty,
        string text => text,
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
    };

    /// <summary>The rows of a CSV file.</summary>
    /// <param name="path">The file.</param>
    /// <returns>Every row, the header row first.</returns>
    /// <remarks>
    /// A CSV path is one of the shapes the Python version accepts as a value, and reading one is
    /// what makes it useful. This is an RFC 4180 reader — quoted fields, doubled quotes, either
    /// line ending — and not pandas' parser: no type inference, no date parsing, no encoding
    /// guessing. A file whose first row is not its header is read with that row as headers, which
    /// is the Python version's default.
    /// </remarks>
    private static List<IReadOnlyList<object?>> CsvRows(string path)
    {
        var text = System.IO.File.ReadAllText(path);
        var rows = new List<IReadOnlyList<object?>>();
        var row = new List<object?>();
        var field = new StringBuilder();
        var quoted = false;

        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];

            if (quoted)
            {
                if (character == '"')
                {
                    if (index + 1 < text.Length && text[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else
                {
                    field.Append(character);
                }

                continue;
            }

            switch (character)
            {
                case '"':
                    quoted = true;
                    break;

                case ',':
                    row.Add(field.ToString());
                    field.Clear();
                    break;

                case '\r':
                    break;

                case '\n':
                    row.Add(field.ToString());
                    field.Clear();
                    rows.Add(row.ToArray());
                    row.Clear();
                    break;

                default:
                    field.Append(character);
                    break;
            }
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row.ToArray());
        }

        return rows;
    }
}

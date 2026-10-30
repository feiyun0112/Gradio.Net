using System.Collections;
using System.Globalization;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A chart's data as the frontend draws it: the columns, the rows, and what kind of value each
/// column holds.
/// </summary>
/// <param name="Columns">The column names, in the order they were given.</param>
/// <param name="Data">The rows, each in the order of <paramref name="Columns"/>.</param>
/// <param name="Datatypes">
/// Each column's kind — <c>quantitative</c>, <c>nominal</c> or <c>temporal</c> — which is what tells
/// the chart whether to scale a column as a number, group it as a category, or read it as a moment.
/// </param>
/// <param name="Mark">How the chart draws the rows, when it is not the component's own mark.</param>
/// <remarks>
/// <para>
/// The Python version's <c>PlotData</c> in <c>native_plot.py</c>: a pydantic model with these four
/// fields. It is a record here rather than a bare dictionary so that both directions of the
/// component's value can name it — a handler reading a chart and one returning a chart speak about
/// the same thing.
/// </para>
/// <para>
/// A component always draws with its own mark — a line plot with a line — so
/// <paramref name="Mark"/> is filled in from the component when a value is written, and is read
/// back from the frontend when a value is read. The value that reaches the config is the four
/// fields as an object, which is the shape the Python version serves.
/// </para>
/// </remarks>
public sealed record NativePlotData(
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyList<object?>> Data,
    IReadOnlyDictionary<string, string> Datatypes,
    string? Mark = null);

/// <summary>
/// A chart drawn natively by the frontend from a table.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The parameters say what the chart is about — which
/// column is which axis, what colours the series, how the axes are labelled — and the frontend does
/// the drawing, so none of them has an effect on this side beyond being carried to the browser.
/// </para>
/// <para>
/// The Python version's <c>NativePlot</c> is a base class its three charts derive from, and it is
/// the base rather than a chart of its own: <c>gradio.utils.get_all_components</c> leaves it out of
/// the component list, so it has no parameter manifest entry and cannot be written by an
/// application. That is why this class is abstract and the three subclasses are what carry the
/// block names.
/// </para>
/// <para>
/// The value is a table, because the Python version's is a pandas <c>DataFrame</c>. There is no
/// pandas here, so a table is a map from a column's name to its values — which is what a
/// <c>DataFrame</c> is to the code that reads it — and the kinds the chart needs are inferred from
/// the values the way pandas infers them from a column's dtype. A value that is already in the
/// served shape, or a <see cref="NativePlotData"/>, is passed through instead.
/// </para>
/// </remarks>
[HandlerValue("NativePlotData?", "change")]
[HandlerValue("object?", "select", "double_click")]
public abstract partial class NativePlot : Component
{
    private object? _value;

    /// <summary>The table the chart draws.</summary>
    /// <value>
    /// A map from a column's name to that column's values, a <see cref="NativePlotData"/>, an object
    /// already in the served shape, or <see langword="null"/> for no data.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>The column drawn along the x axis.</summary>
    public string? X { get; set; }

    /// <summary>The column drawn along the y axis.</summary>
    public string? Y { get; set; }

    /// <summary>The column that decides a row's series, and therefore its colour.</summary>
    public string? Color { get; set; }

    /// <summary>The title drawn above the chart.</summary>
    public string? Title { get; set; }

    /// <summary>The title of the x axis; the column's name when the application names none.</summary>
    public string? XTitle { get; set; }

    /// <summary>The title of the y axis; the column's name when the application names none.</summary>
    public string? YTitle { get; set; }

    /// <summary>The title of the colour legend.</summary>
    public string? ColorTitle { get; set; }

    /// <summary>
    /// How the x values are grouped before they are drawn.
    /// </summary>
    /// <value>
    /// A number of bins for a numeric column, or a duration such as <c>"1h"</c> for a date column,
    /// written with an <c>s</c>, <c>m</c>, <c>h</c> or <c>d</c> suffix.
    /// </value>
    public object? XBin { get; set; }

    /// <summary>How the y values of a group are combined into one.</summary>
    /// <value><c>sum</c>, <c>mean</c>, <c>median</c>, <c>min</c> or <c>max</c>.</value>
    public string? YAggregate { get; set; }

    /// <summary>The colour each series is drawn in.</summary>
    public IReadOnlyDictionary<string, string>? ColorMap { get; set; }

    /// <summary>The series listed in the legend.</summary>
    /// <value>Every series when the application names none.</value>
    public IReadOnlyList<string>? ColorsInLegend { get; set; }

    /// <summary>The limits of the x axis.</summary>
    /// <value>
    /// Two entries, either of which may be absent so that the axis keeps its own bound on that
    /// side.
    /// </value>
    public IReadOnlyList<double?>? XLim { get; set; }

    /// <summary>The limits of the y axis, as <see cref="XLim"/> is for the x axis.</summary>
    public IReadOnlyList<double?>? YLim { get; set; }

    /// <summary>How far the x axis labels are turned, in degrees clockwise.</summary>
    public double XLabelAngle { get; set; }

    /// <summary>How far the y axis labels are turned, in degrees clockwise.</summary>
    public double YLabelAngle { get; set; }

    /// <summary>A format for the x axis labels, as the charting library spells it.</summary>
    public string? XAxisFormat { get; set; }

    /// <summary>A format for the y axis labels, as the charting library spells it.</summary>
    public string? YAxisFormat { get; set; }

    /// <summary>Whether the x axis labels are drawn.</summary>
    /// <value>
    /// <see cref="Visibility.Shown"/> unless the application turns them off, or asks for
    /// <see cref="Visibility.Hidden"/> — the Python version's third value, which asks for the
    /// labels to be dropped while the axis itself stays.
    /// </value>
    public Visibility XAxisLabelsVisible { get; set; } = Visibility.Shown;

    /// <summary>The note drawn below the chart.</summary>
    public string? Caption { get; set; }

    /// <summary>The order the x values are drawn in.</summary>
    /// <value>
    /// <c>x</c>, <c>y</c>, <c>-x</c>, <c>-y</c>, or a list of category names in the order to draw
    /// them.
    /// </value>
    public object? Sort { get; set; }

    /// <summary>What a hovered point shows.</summary>
    /// <value>
    /// <c>axis</c> for the axis columns, <c>all</c> for every column, <c>none</c> for nothing, or a
    /// list of the columns to show.
    /// </value>
    public object? Tooltip { get; set; } = "axis";

    /// <summary>The height of the chart in pixels.</summary>
    public int? Height { get; set; }

    /// <summary>The label drawn above the chart.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the chart next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the chart wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn in the chart's corner.</summary>
    /// <value>
    /// <c>fullscreen</c> and <c>export</c>, which the frontend draws for itself; no buttons unless
    /// the application asks for one.
    /// </value>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>The block the frontend draws this chart with.</summary>
    /// <remarks>
    /// All three charts are drawn by one frontend component; what tells them apart is the mark the
    /// value carries, not the block name.
    /// </remarks>
    public override string BlockType => "nativeplot";

    /// <summary>Whether the label is drawn when the application says nothing about it.</summary>
    /// <value>
    /// Whether a label was given: the Python version turns the label off for a chart that has none,
    /// so that an unlabelled chart is not left with an empty strip above it.
    /// </value>
    protected override bool ShowLabelWhenUnset => Label is not null;

    /// <summary>How this chart draws its rows.</summary>
    protected abstract string Mark { get; }

    /// <inheritdoc />
    /// <remarks>The Python version's <c>{"type": {}, "description": "any valid json"}</c>: a
    /// chart's value is the table above, which no single schema describes.</remarks>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any valid json",
    };

    /// <summary>The table a handler reads.</summary>
    /// <param name="value">What the client sent: the served shape of a table.</param>
    /// <returns>The table, or <see langword="null"/> when the chart holds none.</returns>
    public override object? Preprocess(JsonNode? value) => value is JsonObject document
        ? new NativePlotData(
            Strings(document["columns"]),
            Rows(document["data"]),
            Kinds(document["datatypes"]),
            JsonValues.Text(document["mark"]))
        : null;

    /// <summary>The table the client receives from what a handler returned.</summary>
    /// <param name="value">
    /// A dictionary from a column's name to that column's values, a <see cref="NativePlotData"/>, an
    /// object already in the served shape, or none.
    /// </param>
    /// <returns>The table as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <exception cref="ArgumentException">The value is of a shape no table can be made from.</exception>
    /// <remarks>
    /// The dictionary is read as <see cref="IDictionary"/> rather than as
    /// <c>IReadOnlyDictionary&lt;string, IReadOnlyList&lt;object?&gt;&gt;</c> because a dictionary is
    /// invariant in its value type: <c>Dictionary&lt;string, object?[]&gt;</c> — the spelling an
    /// application reaches for first — is not that interface, and refusing it would make the obvious
    /// spelling a trap rather than a type.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        JsonObject document => document.DeepClone(),
        NativePlotData table => Write(table with { Mark = Mark }),
        IDictionary columns => Write(FromColumns(columns)),
        _ => throw new ArgumentException(
            "A native plot is drawn from a table: a dictionary from a column's name to its values, "
            + $"or a NativePlotData. It was handed a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>A table as the protocol carries it.</summary>
    /// <param name="table">The table.</param>
    /// <returns>The four fields the Python version's model declares, in its order.</returns>
    private static JsonObject Write(NativePlotData table) => new()
    {
        ["columns"] = new JsonArray([.. table.Columns.Select(column => (JsonNode?)JsonValue.Create(column))]),
        ["data"] = new JsonArray([.. table.Data.Select(row => (JsonNode?)new JsonArray([.. row.Select(Cell)]))]),
        ["datatypes"] = new JsonObject([.. table.Datatypes.Select(kind =>
            KeyValuePair.Create(kind.Key, (JsonNode?)JsonValue.Create(kind.Value)))]),
        ["mark"] = table.Mark,
    };

    /// <summary>A table built from a dictionary of columns.</summary>
    /// <param name="columns">Each column's name and its values.</param>
    /// <returns>The columns, the rows, and each column's kind.</returns>
    /// <exception cref="ArgumentException">
    /// A key is not text, or a column is not a list of values.
    /// </exception>
    /// <remarks>
    /// The rows are as long as the longest column, and a shorter column is padded with
    /// <see langword="null"/>: a table whose columns disagree in length is one a <c>DataFrame</c>
    /// could not hold either, and padding is what keeps the rows rectangular rather than dropping
    /// values. The columns are in the order the dictionary enumerates them, which is the order an
    /// application wrote them in for every dictionary it is likely to use.
    /// </remarks>
    private NativePlotData FromColumns(IDictionary columns)
    {
        var names = new List<string>(columns.Count);
        var values = new List<IReadOnlyList<object?>>(columns.Count);

        foreach (DictionaryEntry entry in columns)
        {
            if (entry.Key is not string name)
            {
                throw new ArgumentException(
                    "A column's name is text; the table has a key of type "
                    + $"{entry.Key?.GetType().ToString() ?? "null"}.",
                    nameof(columns));
            }

            names.Add(name);
            values.Add(Column(name, entry.Value));
        }

        var length = values.Select(column => column.Count).DefaultIfEmpty(0).Max();

        var rows = new List<IReadOnlyList<object?>>(length);
        for (var index = 0; index < length; index++)
        {
            rows.Add([.. values.Select(column => index < column.Count ? column[index] : null)]);
        }

        return new NativePlotData(
            names,
            rows,
            names.Zip(values).ToDictionary(
                column => column.First,
                column => KindOf(column.Second),
                StringComparer.Ordinal),
            Mark);
    }

    /// <summary>One column's values.</summary>
    /// <param name="name">The column's name, for the message when it is not a column.</param>
    /// <param name="values">What the dictionary held under that name.</param>
    /// <returns>The values, in the order they were given.</returns>
    /// <exception cref="ArgumentException">The value is not a list of values.</exception>
    /// <remarks>
    /// Text is refused before the enumerable cases are tried: a string is a sequence of characters,
    /// so it would otherwise read as a column of one-character values rather than as the mistake it
    /// is.
    /// </remarks>
    private static IReadOnlyList<object?> Column(string name, object? values) => values switch
    {
        null => [],
        string => throw new ArgumentException(
            $"The column '{name}' is text rather than a list of values; a column holds one value per "
            + "row.",
            nameof(values)),
        IEnumerable<object?> items => [.. items],
        IEnumerable items => [.. items.Cast<object?>()],
        _ => throw new ArgumentException(
            $"The column '{name}' is a {values.GetType()} rather than a list of values.",
            nameof(values)),
    };

    /// <summary>What kind of value a column holds.</summary>
    /// <param name="values">The column's values.</param>
    /// <returns><c>quantitative</c>, <c>temporal</c> or <c>nominal</c>.</returns>
    /// <remarks>
    /// The Python version asks pandas what a column's dtype is; here there is no dtype, so the
    /// values are asked instead, by the same three questions pandas answers: is it a number
    /// (booleans included, as pandas counts them), is it a moment, and otherwise it is a category.
    /// An empty column is nominal, which is what a column of nothing is to a chart — a set of
    /// categories with no members rather than a number with no value.
    /// </remarks>
    private static string KindOf(IReadOnlyList<object?> values)
    {
        var present = values.Where(value => value is not null).ToArray();

        if (present.Length == 0)
        {
            return "nominal";
        }

        if (present.All(IsNumber))
        {
            return "quantitative";
        }

        return present.All(value => value is global::System.DateTime
            or global::System.DateTimeOffset or global::System.DateOnly)
            ? "temporal"
            : "nominal";
    }

    /// <summary>Whether a value is a number, as pandas counts them.</summary>
    private static bool IsNumber(object? value) => value
        is sbyte or byte or short or ushort or int or uint or long or ulong
        or float or double or decimal or bool;

    /// <summary>One cell as the protocol carries it.</summary>
    /// <remarks>
    /// A moment is written as milliseconds since the epoch, which is how pandas serializes a date
    /// column and what the frontend reads back into a date. A moment with no time zone is read as
    /// UTC, as a naive pandas timestamp is.
    /// </remarks>
    private static JsonNode? Cell(object? value) => value switch
    {
        null => null,
        JsonNode node => node.DeepClone(),
        global::System.DateTime moment => JsonValue.Create(Milliseconds(moment)),
        global::System.DateTimeOffset moment => JsonValue.Create(moment.ToUnixTimeMilliseconds()),
        global::System.DateOnly day => JsonValue.Create(Milliseconds(day.ToDateTime(TimeOnly.MinValue))),
        bool flag => JsonValue.Create(flag),
        string text => JsonValue.Create(text),
        sbyte number => JsonValue.Create(number),
        byte number => JsonValue.Create(number),
        short number => JsonValue.Create(number),
        ushort number => JsonValue.Create(number),
        int number => JsonValue.Create(number),
        uint number => JsonValue.Create(number),
        long number => JsonValue.Create(number),
        ulong number => JsonValue.Create(number),
        float number => JsonValue.Create(number),
        double number => JsonValue.Create(number),
        decimal number => JsonValue.Create(number),
        _ => JsonValue.Create(Convert.ToString(value, CultureInfo.InvariantCulture)),
    };

    /// <summary>The milliseconds between the epoch and a moment.</summary>
    /// <remarks>
    /// A moment that knows it is local is converted rather than relabelled; one with no zone is read
    /// as UTC, which is what a naive pandas timestamp is.
    /// </remarks>
    private static long Milliseconds(global::System.DateTime moment) => moment.Kind switch
    {
        DateTimeKind.Local => new global::System.DateTimeOffset(moment).ToUnixTimeMilliseconds(),
        _ => new global::System.DateTimeOffset(global::System.DateTime.SpecifyKind(moment, DateTimeKind.Utc))
            .ToUnixTimeMilliseconds(),
    };

    /// <summary>The strings of a JSON array, skipping entries that are not strings.</summary>
    private static string[] Strings(JsonNode? node) => node is JsonArray array
        ? [.. array.Select(item => JsonValues.Text(item) ?? string.Empty)]
        : [];

    /// <summary>The rows of a JSON array.</summary>
    private static object?[][] Rows(JsonNode? node) => node is JsonArray rows
        ? [.. rows.Select(row => row is JsonArray cells
            ? cells.Select(JsonValues.ToObject).ToArray()
            : [])]
        : [];

    /// <summary>The column kinds of a JSON object.</summary>
    private static Dictionary<string, string> Kinds(JsonNode? node) => node is JsonObject kinds
        ? kinds.ToDictionary(kind => kind.Key, kind => JsonValues.Text(kind.Value) ?? "nominal", StringComparer.Ordinal)
        : [];
}

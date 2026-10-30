using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The plot family as an application uses it: the defaults a chart carries when nothing is said, the
/// table it is drawn from, and the drawing a plot passes through.
/// </summary>
/// <remarks>
/// <para>
/// The parameters an application sets are compared with the Python version's field by field, on the
/// manifest page. What a document cannot show is the conversion behind the value: the Python
/// version turns a pandas <c>DataFrame</c> into the four fields the frontend draws, and this side
/// turns a map of columns into the same four. The two are written on both sides, so the inference
/// is asserted here too.
/// </para>
/// <para>
/// A plot's value is the other half of that: the Python version converts a figure from one of four
/// plotting libraries, and there are none here — so what is left of that conversion is the
/// passthrough, and the shape it refuses.
/// </para>
/// </remarks>
public sealed class PlotFamilyTests
{
    [Fact]
    public async Task The_plot_family_carries_upstreams_defaults()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new LinePlot());
                blocks.Add(new BarPlot());
                blocks.Add(new ScatterPlot());
                blocks.Add(new LinePlot { Label = "Trend" });
                blocks.Add(new Plot());
            }
        });

        var config = await GetConfigAsync(host, cancellationToken);

        // All three charts are drawn by one frontend component; what tells them apart is the mark
        // in the value and the class name the props carry.
        foreach (var (id, name) in new[] { (1, "lineplot"), (2, "barplot"), (3, "scatterplot") })
        {
            var chart = Props(config, id);
            Assert.Equal(name, chart["name"]!.GetValue<string>());
            Assert.Equal(0, chart["x_label_angle"]!.GetValue<double>());
            Assert.Equal(0, chart["y_label_angle"]!.GetValue<double>());
            Assert.True(chart["x_axis_labels_visible"]!.GetValue<bool>());
            Assert.Equal("axis", chart["tooltip"]!.GetValue<string>());
            Assert.True(chart["container"]!.GetValue<bool>());
            Assert.Equal(160, chart["min_width"]!.GetValue<int>());
            Assert.Empty(Assert.IsType<JsonArray>(chart["buttons"]));
            Assert.Equal(["value"], Strings(chart["preserved_by_key"]));

            // A chart with no label is not given an empty strip to draw one in: the Python version turns the
            // label off when the application named none.
            Assert.False(chart["show_label"]!.GetValue<bool>());

            // Nothing was set that would make a key, so the keys are absent rather than null.
            foreach (var absent in new[] { "value", "x", "y", "color", "title", "x_bin", "sort", "height" })
            {
                Assert.False(chart.ContainsKey(absent), $"{name} carries '{absent}'");
            }

            Assert.False(chart["_selectable"]!.GetValue<bool>());
        }

        // The chart that was given a label draws it.
        Assert.True(Props(config, 4)["show_label"]!.GetValue<bool>());

        var plot = Props(config, 5);
        Assert.Equal("plot", plot["name"]!.GetValue<string>());
        Assert.Equal("webp", plot["format"]!.GetValue<string>());
        Assert.True(plot["show_label"]!.GetValue<bool>());
        Assert.Equal(160, plot["min_width"]!.GetValue<int>());
        Assert.Equal(["value"], Strings(plot["preserved_by_key"]));

        // The bokeh version is not a parameter: the Python version adds it after them, and it is null when
        // bokeh is not installed — which is every installation of this framework.
        Assert.False(plot.ContainsKey("bokeh_version"));
        Assert.False(plot.ContainsKey("value"));

        // A chart describes itself as "any valid json", because its value is a table no schema
        // describes; a plot describes the two fields of a drawing.
        var components = Assert.IsType<JsonArray>(config["components"]);
        var chartInfo = components[0]!["api_info"]!.AsObject();
        Assert.Empty(Assert.IsType<JsonObject>(chartInfo["type"]));
        Assert.Equal("any valid json", chartInfo["description"]!.GetValue<string>());

        var plotInfo = components[4]!["api_info"]!.AsObject();
        Assert.Equal("PlotData", plotInfo["title"]!.GetValue<string>());
        Assert.Equal(
            ["altair", "bokeh", "plotly", "matplotlib"],
            Strings(plotInfo["properties"]!["type"]!["enum"]));
        Assert.Equal(["type", "plot"], Strings(plotInfo["required"]));
    }

    [Fact]
    public void A_native_plot_draws_the_table_it_was_given()
    {
        // A map of columns is what a table is here, and the four fields the frontend draws are made
        // from it: the columns in the order given, the rows in that order, and the kind of each
        // column — inferred the way pandas infers it from a dtype.
        // The spelling an application reaches for first: a dictionary from a name to an array of
        // that column's values. It is not `IReadOnlyDictionary<string, IReadOnlyList<object?>>` — a
        // dictionary is invariant in its value type — which is why the component reads the
        // dictionary rather than that interface.
        var chart = new LinePlot
        {
            Value = new Dictionary<string, object?[]>
            {
                ["x"] = [1, 2, 3],
                ["y"] = [4.5, 5.5],
                ["series"] = ["a", "b", "a"],
                ["when"] =
                [
                    new System.DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new System.DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    new System.DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                ],
                ["flag"] = [true, false, true],
            },
        };

        var value = Assert.IsType<JsonObject>(chart.Value);
        Assert.Equal(["x", "y", "series", "when", "flag"], Strings(value["columns"]));

        // The shorter column is padded rather than the rows being ragged, and a moment is written
        // as milliseconds since the epoch — which is what pandas writes and what the chart reads
        // back into a date.
        Assert.Equal(
            """[[1,4.5,"a",1577836800000,true],[2,5.5,"b",1577923200000,false],[3,null,"a",1578009600000,true]]""",
            value["data"]!.ToJsonString());

        // Numbers and booleans are quantitative (pandas counts a boolean as a number), a date is
        // temporal, and anything else is a category.
        var datatypes = Assert.IsType<JsonObject>(value["datatypes"]);
        Assert.Equal("quantitative", datatypes["x"]!.GetValue<string>());
        Assert.Equal("quantitative", datatypes["flag"]!.GetValue<string>());
        Assert.Equal("nominal", datatypes["series"]!.GetValue<string>());
        Assert.Equal("temporal", datatypes["when"]!.GetValue<string>());

        // The mark belongs to the component rather than to the table, which is why the same table
        // draws three different charts.
        Assert.Equal("line", value["mark"]!.GetValue<string>());
        Assert.Equal(
            "bar",
            Assert.IsType<JsonObject>(
                new BarPlot { Value = new Dictionary<string, IReadOnlyList<object?>> { ["x"] = [1] } }.Value)["mark"]!
                .GetValue<string>());
        Assert.Equal(
            "point",
            Assert.IsType<JsonObject>(
                new ScatterPlot { Value = new Dictionary<string, IReadOnlyList<object?>> { ["x"] = [1] } }.Value)["mark"]!
                .GetValue<string>());

        // Reading the value back gives the table a handler works with, mark included.
        var read = Assert.IsType<NativePlotData>(chart.Preprocess(value));
        Assert.Equal(["x", "y", "series", "when", "flag"], read.Columns);
        Assert.Equal(3, read.Data.Count);
        Assert.Equal("line", read.Mark);
        Assert.Equal("temporal", read.Datatypes["when"]);

        // A value that is already in the served shape is passed through rather than converted,
        // which is how the Python version passes a dictionary through.
        var served = new JsonObject { ["columns"] = new JsonArray("a"), ["mark"] = "native" };
        Assert.Equal(served.ToJsonString(), chart.Postprocess(served)!.ToJsonString());

        Assert.Null(chart.Postprocess(null));
        Assert.Null(chart.Preprocess(null));
        Assert.Throws<ArgumentException>(() => { _ = chart.Postprocess(42); });

        // A value that is not a column of values is refused by name rather than read as one value:
        // text is a sequence of characters, so it would otherwise become a column of letters.
        var refused = Assert.Throws<ArgumentException>(
            () => { _ = chart.Postprocess(new Dictionary<string, object?> { ["x"] = "not a column" }); });
        Assert.Contains("'x'", refused.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_plot_carries_the_drawing_it_was_given()
    {
        var plot = new Plot { Value = new PlotData("altair", """{"mark":"bar"}""") };

        // The passthrough: the library and its drawing, as the Python version's own conversion would have
        // left them.
        var written = Assert.IsType<JsonObject>(plot.Value);
        Assert.Equal("altair", written["type"]!.GetValue<string>());
        Assert.Equal("""{"mark":"bar"}""", written["plot"]!.GetValue<string>());

        // A document with the two fields is read as the model, which is the shape a handler that
        // speaks JSON produces — and only those two fields travel, as the Python version's model keeps only
        // what it declares.
        plot.Value = new JsonObject { ["type"] = "plotly", ["plot"] = "{}", ["extra"] = 1 };
        var readBack = Assert.IsType<JsonObject>(plot.Value);
        Assert.Equal("plotly", readBack["type"]!.GetValue<string>());
        Assert.Equal("{}", readBack["plot"]!.GetValue<string>());
        Assert.False(readBack.ContainsKey("extra"));

        // The library is checked, because a name the frontend cannot draw is a plot that renders
        // as nothing rather than as a plot.
        Assert.Throws<ArgumentException>(() => { _ = plot.Postprocess(new PlotData("vega", "{}")); });
        Assert.Throws<ArgumentException>(() => { _ = plot.Postprocess(42); });

        var read = Assert.IsType<PlotData>(plot.Preprocess(new JsonObject { ["type"] = "bokeh", ["plot"] = "{}" }));
        Assert.Equal("bokeh", read.Type);
        Assert.Equal("{}", read.Plot);

        Assert.Null(plot.Postprocess(null));
        Assert.Null(plot.Preprocess(null));
    }

    [Fact]
    public async Task A_chart_binds_the_three_events_upstream_declares()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var chart = blocks.Add(new LinePlot());
                var answer = blocks.Add(new Textbox());

                chart.Select(_ => "selected", [chart], [answer]);
                chart.DoubleClick(_ => "double", [chart], [answer]);
            }
        });

        var config = await GetConfigAsync(host, cancellationToken);
        var dependencies = Assert.IsType<JsonArray>(config["dependencies"]);

        // The signals are the Python version's spellings, because they are what the frontend dispatches on.
        Assert.Equal(
            ["select", "double_click"],
            dependencies.Select(entry => entry!["targets"]![0]![1]!.GetValue<string>()));

        // A chart that can be brushed says so, which is the Python version's `_selectable`: it is turned on
        // when something listens for a selection, and off for every chart nobody listens to.
        Assert.True(Props(config, 1)["_selectable"]!.GetValue<bool>());

        // A plot's set is the one signal it has.
        Assert.Equal("change", Assert.Single(new Plot().Events.Change.Targets).EventName);
        Assert.Equal("select", Assert.Single(new LinePlot().Events.Select.Targets).EventName);
        Assert.Equal("double_click", Assert.Single(new BarPlot().Events.DoubleClick.Targets).EventName);
    }

    /// <summary>The served config.</summary>
    private static async Task<JsonObject> GetConfigAsync(GradioTestHost host, CancellationToken cancellationToken)
    {
        using var response = await host.Client.GetAsync("/config", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return Assert.IsType<JsonObject>(JsonNode.Parse(body));
    }

    /// <summary>One component's props, by id.</summary>
    private static JsonObject Props(JsonObject config, int id) =>
        config["components"]!
            .AsArray()
            .Single(component => component!["id"]!.GetValue<int>() == id)!["props"]!
            .AsObject();

    private static string[] Strings(JsonNode? node) =>
        [.. node!.AsArray().Select(item => item!.GetValue<string>())];
}

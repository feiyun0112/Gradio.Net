using System.Globalization;
using Gradio.Net;

// A chart is data. The two ways to get one here are the two shapes the protocol has:
//
//  * `gr.LinePlot` (and `BarPlot`, `ScatterPlot`) draws a table itself: the handler answers with a
//    column per key, and the frontend draws the rows. This is the replacement for "return a pandas
//    DataFrame" — the table is handed over as columns, and nothing is inferred beyond what the
//    values say.
//  * `gr.Plot` passes a drawing through. The Python version converts a matplotlib, plotly, altair or bokeh
//    figure into JSON on the server; there are no such libraries here, so the JSON is what an
//    application brings — a `PlotData` naming the library and carrying its drawing.
//
//     gr.LinePlot(value=df, x="day", y="visits")          # the Python version
//     gr.LinePlot(value: DailyVisits(days), x: "day", y: "visits")   # here: the columns themselves

// A bar chart the way the plotting library would have written it, made by hand. Plotly's drawing is
// a JSON document, which is what `gr.Plot` carries; a matplotlib figure would be a base64 image and
// a bokeh model its own JSON.
const string Bars = """
{
  "data": [{ "type": "bar", "x": ["Mon", "Tue", "Wed", "Thu", "Fri"], "y": [12, 18, 9, 22, 15] }],
  "layout": { "title": { "text": "Sign-ups this week" }, "margin": { "t": 40 } }
}
""";

static Dictionary<string, object?[]> DailyVisits(double days)
{
    var count = (int)days;
    var day = new object?[count];
    var visits = new object?[count];
    var signups = new object?[count];

    for (var index = 0; index < count; index++)
    {
        day[index] = string.Create(CultureInfo.InvariantCulture, $"day {index + 1}");
        visits[index] = 40 + ((index * 17) % 23);
        signups[index] = 6 + ((index * 5) % 9);
    }

    // The columns, in the order they are drawn in. A `Dictionary<string, object?[]>` is the spelling
    // an application reaches for first, and it is the one the chart accepts.
    return new Dictionary<string, object?[]>
    {
        ["day"] = day,
        ["visits"] = visits,
        ["signups"] = signups,
    };
}

var demo = gr.Blocks(title: "Charts");

using (demo)
{
    gr.Markdown(
        """
        # Charts

        **A chart drawn from a table.** The handler answers with columns; the slider says how many
        days of them.

        **A drawing the application made.** This one is a plotly document written out by hand — the
        shape a plotting library would have produced, handed to the page as it is.
        """);

    var days = gr.Slider(minimum: 3, maximum: 30, value: 10, step: 1, label: "Days");

    var chart = gr.LinePlot(
        value: DailyVisits(10),
        x: "day",
        y: "visits",
        title: "Visits per day",
        label: "Line plot");

    gr.Button("Draw", variant: "primary")
        .Click(DailyVisits, [days], [chart], apiName: "draw");

    gr.Plot(value: new PlotData("plotly", Bars), label: "Plot");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

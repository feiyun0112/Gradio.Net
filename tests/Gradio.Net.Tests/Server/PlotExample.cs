using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A page holding one chart, so that a browser can be asked what it makes of it.
/// </summary>
/// <remarks>
/// <para>
/// The chart is drawn from a table the application gives it, and it is the only component on the
/// page that can be interacted with: a double click on it runs a handler, and the handler's answer
/// lands in the textbox beside it. That is what "a chart is interactive" means here — the frontend
/// draws the plot from the value, and a gesture on the drawing travels the whole way back.
/// </para>
/// <para>
/// The table is small on purpose: the assertion is that a chart appears and that a gesture on it is
/// heard, not that a charting library draws a particular shape.
/// </para>
/// </remarks>
internal static class PlotExample
{
    /// <summary>The answer the handler writes, which is how a double click is observed.</summary>
    internal const string Answer = "double-clicked";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var chart = blocks.Add(new LinePlot
            {
                Value = new Dictionary<string, IReadOnlyList<object?>>
                {
                    ["x"] = [1, 2, 3],
                    ["y"] = [4, 5, 6],
                },
                X = "x",
                Y = "y",
                Height = 240,
            });

            var answer = blocks.Add(new Textbox { Label = "Answer" });

            chart.DoubleClick(_ => Answer, [chart], [answer]);
        }
    }
}

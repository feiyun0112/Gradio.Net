using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The page the form-semantics tests are about.
/// </summary>
/// <remarks>
/// <para>
/// The Python version has no form an application can write, so what this page exercises is the
/// grouping rule and nothing else. Three containers answer three different questions about it. A
/// button written between two runs of fields ends the first — a block that breaks the grouping is a
/// boundary, so the row holds two forms. A state written between two fields leaves the run alone —
/// an invisible component is not a boundary, and stays a child of the row while the fields around
/// it are grouped without it. A compact column switches the grouping off, so its fields belong to
/// no form. The two fields written after it are a run of their own, and the page's fourth form.
/// </para>
/// <para>
/// The bindings are the other half. The first names every field on the page as an input — five of
/// them in three forms, two in none — and is what a submission looks like once it reaches the
/// server: one run, seven values. The other two are fields' own events, bound as they would be on
/// any page — grouping is a layout rule, so it neither adds nor removes a binding, and a form has no
/// events of its own to fire.
/// </para>
/// <para>
/// The parameters are named after the components they read, because that is what an endpoint's
/// inputs are called in the API documentation: the Python version reads the handler's own parameter
/// names, and this side of the case gives a binding the same names explicitly.
/// </para>
/// </remarks>
internal static class FormPage
{
    /// <summary>The title the page and the OpenAPI document carry.</summary>
    internal const string Title = "Form page";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;

        // Fixed on both sides of the case rather than left to the defaults, which differ on
        // purpose.
        blocks.AnalyticsEnabled = true;

        Textbox first;
        Textbox second;
        Textbox third;
        Textbox fourth;
        Textbox sixth;
        Textbox seventh;
        Textbox output;
        Button go;
        Number fifth;
        Number status;

        using (blocks.BeginRender())
        {
            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                first = blocks.Add(new Textbox { Label = "First" });
                second = blocks.Add(new Textbox { Label = "Second" });
                go = blocks.Add(new Button { Value = "Go" });
                third = blocks.Add(new Textbox { Label = "Third" });
            }

            var secondRow = blocks.Add(new Row());

            using (blocks.Enter(secondRow))
            {
                fourth = blocks.Add(new Textbox { Label = "Fourth" });
                blocks.Add(new State { Value = "kept" });
                fifth = blocks.Add(new Number { Label = "Fifth" });
            }

            var compact = blocks.Add(new Column { Variant = "compact" });

            using (blocks.Enter(compact))
            {
                sixth = blocks.Add(new Textbox { Label = "Sixth" });
                seventh = blocks.Add(new Textbox { Label = "Seventh" });
            }

            output = blocks.Add(new Textbox { Label = "Output" });
            status = blocks.Add(new Number { Label = "Status" });
        }

        go.Click(
            (Func<string, string, string, string, double, string, string, string>)Join,
            [first, second, third, fourth, fifth, sixth, seventh],
            [output],
            apiName: "join",
            endpointDescription: "Every field of the page, in one run.",
            parameterNames: ["First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh"]);

        first.Change(Shout, [first], [output], apiName: "shout", endpointDescription: "One field's own event.", parameterNames: ["First"]);

        fifth.Change(
            Count,
            [fifth],
            [status],
            apiName: "count",
            endpointDescription: "A number field's own event, so that the field that submits is not always a textbox.",
            parameterNames: ["Fifth"]);
    }

    /// <summary>Every field of the page, in one run.</summary>
    private static string Join(
        string first,
        string second,
        string third,
        string fourth,
        double fifth,
        string sixth,
        string seventh) =>
        string.Join(" | ", first, second, third, fourth, fifth, sixth, seventh);

    /// <summary>One field's own event.</summary>
    private static object? Shout(string first) => first.ToUpperInvariant();

    /// <summary>A number field's own event, so that the field that submits is not always a textbox.</summary>
    private static object? Count(double fifth) => fifth + 1;
}

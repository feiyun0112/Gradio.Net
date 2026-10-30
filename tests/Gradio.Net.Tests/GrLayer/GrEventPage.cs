using System.Text.Json.Nodes;
using Gradio.Net;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// One page written twice — once with the layer, once with the core API — so that the two can be
/// held against each other.
/// </summary>
/// <remarks>
/// <para>
/// The claim ticket 43 makes is that a page written with <c>gr</c> is the same page: the same
/// config and the same messages while it runs. The only way to check that is to write it both ways
/// and compare, which is what this type exists for — the two methods are the same page line for
/// line, and everything that differs between them is the difference between the two APIs.
/// </para>
/// <para>
/// The page carries the four shapes the ticket names: a typed binding and a step chained onto it
/// (<c>Then</c>), two triggers merged into one endpoint (<c>gr.On</c>), a handler that answers
/// <c>gr.Skip()</c>, and one that answers <c>gr.Update(...)</c>. The core half writes those last
/// two as the documents they are, because that is what a core application has to write — which is
/// what makes the comparison a claim about the layer's values rather than about its plumbing.
/// </para>
/// <para>
/// The order of the lines is the same on both sides on purpose: a binding's id is its position, and
/// the id is what <c>trigger_after</c> and the config's order are made of, so two pages that bound
/// the same things in a different order would differ for a reason that is not the layer's.
/// </para>
/// </remarks>
internal static class GrEventPage
{
    /// <summary>The title both pages carry.</summary>
    internal const string Title = "Event page, written twice";

    /// <summary>What the textbox is typed into, on both sides.</summary>
    internal const string TypedText = "ada";

    /// <summary>The bindings the runs below ask for, by endpoint name and the values they read.</summary>
    /// <returns>One entry per binding, in the order the page binds them.</returns>
    /// <remarks>
    /// Built per call rather than held in a field: the values are handed to a queue that may keep
    /// them, and a shared document that one caller cloned and another did not would be a test that
    /// passes or fails depending on who ran first.
    /// </remarks>
    internal static (string Api, JsonArray Data)[] Runs() =>
    [
        ("upper", new JsonArray(TypedText)),
        ("merged", new JsonArray(TypedText)),
        ("skip", new JsonArray()),
        ("update", new JsonArray(TypedText)),
    ];

    /// <summary>Writes the page with the layer.</summary>
    /// <returns>The application, after the scope it was written in has ended.</returns>
    internal static Blocks Layer()
    {
        Blocks demo;

        using (demo = gr.Blocks(title: Title))
        {
            var name = gr.Textbox(label: "Name");
            var greeting = gr.Textbox(label: "Greeting");
            var status = gr.Textbox(label: "Status");
            var button = gr.Button("Greet");

            // A binding, and the three steps that can follow one: the return value is the chain.
            var upper = name.Change(Upper, [name], [greeting], apiName: "upper");
            upper.Then(Record, [greeting], [status], apiName: "record");
            upper.Success(Record, [greeting], [status], apiName: "on_success");
            upper.Failure(Record, [greeting], [status], apiName: "on_failure");

            // Two signals, one endpoint: the trigger points are the ones that moved under `Events`.
            gr.On(
                [button.Events.Click, name.Events.Submit],
                Echo,
                [name],
                [status],
                apiName: "merged");

            // A handler that answers "leave it alone", and one that answers "change these props".
            button.Click(() => gr.Skip(), [greeting], apiName: "skip");

            name.Blur(
                _ => gr.Update(new Update
                {
                    Value = string.Empty,
                    Label = "Reset",
                }),
                [name],
                [status],
                apiName: "update");
        }

        return demo;
    }

    /// <summary>Writes the same page with the core API.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <remarks>
    /// The bindings are the same calls, because a binding method lives on the component and the
    /// layer does not copy it. What differs is how the blocks are built and placed, and
    /// where the merged binding comes from: <see cref="Blocks.On"/> here, <c>gr.On</c> there.
    /// </remarks>
    internal static void Core(Blocks blocks)
    {
        blocks.Title = Title;

        Textbox name;
        Textbox greeting;
        Textbox status;
        Button button;

        using (blocks.BeginRender())
        {
            name = blocks.Add(new Textbox { Label = "Name" });
            greeting = blocks.Add(new Textbox { Label = "Greeting" });
            status = blocks.Add(new Textbox { Label = "Status" });
            button = blocks.Add(new Button { Value = "Greet" });
        }

        var upper = name.Change(Upper, [name], [greeting], apiName: "upper");
        upper.Then(Record, [greeting], [status], apiName: "record");
        upper.Success(Record, [greeting], [status], apiName: "on_success");
        upper.Failure(Record, [greeting], [status], apiName: "on_failure");

        blocks.On(
            [button.Events.Click, name.Events.Submit],
            fn: Echo, inputs: [name], outputs: [status], apiName: "merged");

        button.Click(
            () => new JsonObject { ["__type__"] = "update" },
            [greeting],
            apiName: "skip");

        name.Blur(
            _ => new JsonObject { ["value"] = string.Empty, ["label"] = "Reset", ["__type__"] = "update" },
            [name],
            [status],
            apiName: "update");
    }

    private static object? Upper(string value) => value.ToUpperInvariant();

    private static string Record(string value) => $"seen {value}";

    private static string Echo(string value) => value;
}

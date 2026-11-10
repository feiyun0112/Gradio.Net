using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A page whose second step is chained onto the first: it reads a component the first step never
/// wrote, writes a component the first step never touched, and counts its own runs.
/// </summary>
/// <remarks>
/// <para>
/// It exists for <c>BrowserEndToEndTests</c>, which loads the page in a real browser and clicks it.
/// The chain is the part no document comparison can settle: a <c>then</c> step is dispatched by the
/// frontend once the step before it is done, and the values it is handed are the ones its own
/// <c>inputs</c> name — not the ones the step before it produced.
/// </para>
/// <para>
/// The count is what makes "dispatched once" observable. The step answers with the number of times
/// it has run, so a page that runs it twice writes <c>seed:2</c> where it should write
/// <c>seed:1</c> — which is what the server did as well as the client when the server ran chains of
/// its own.
/// </para>
/// </remarks>
internal sealed class ChainExample
{
    /// <summary>The text the browser types into the field the first step reads.</summary>
    internal const string Typed = "hello";

    /// <summary>What the first step answers, which is the typed text shouted back.</summary>
    internal const string Answered = "HELLO";

    /// <summary>The value the field the chained step reads carries before anyone types.</summary>
    internal const string Seed = "seed";

    /// <summary>What the chained step answers the first time it runs, and only the first time.</summary>
    internal const string Chained = "seed:1";

    /// <summary>The title the page carries, which the config repeats.</summary>
    internal const string Title = "Chain example";

    private int _runs;

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal void Build(Blocks blocks)
    {
        blocks.Title = Title;

        Textbox input;
        Textbox output;
        Textbox seed;
        Textbox chained;

        using (blocks.BeginRender())
        {
            input = blocks.Add(new Textbox { Label = "A", SubmitBtn = true });
            output = blocks.Add(new Textbox { Label = "Out" });
            seed = blocks.Add(new Textbox { Label = "B", Value = Seed });
            chained = blocks.Add(new Textbox { Label = "Chained" });
        }

        var first = input.Submit(Shout, [input], [output], apiName: "shout");

        // The step reads B rather than what the step before it wrote, and writes a component the
        // step before it never touched — which is what "a step may use other inputs and outputs"
        // has to mean for the values to be read from the page rather than handed on.
        first.Then(Count, [seed], [chained], apiName: "count");
    }

    private static object? Shout(string value) => value.ToUpperInvariant();

    private string Count(string value) => $"{value}:{++_runs}";
}

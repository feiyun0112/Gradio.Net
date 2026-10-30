using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The smallest application a person can use in a browser: type into a field, click the button it
/// draws, read the answer in another field.
/// </summary>
/// <remarks>
/// <para>
/// It exists for <c>BrowserEndToEndTests</c>, which loads the page in a real browser and clicks it.
/// Everything the browser needs is here and nothing else is: two components and one binding, so
/// that a failure is about the protocol rather than about a component's parameters.
/// </para>
/// <para>
/// The button is the textbox's own submit button rather than a button component, because the button
/// family is a later batch (ticket 15) and this example has to be writable today. It is the same
/// thing to the browser: a button inside the field that raises <c>submit</c>.
/// </para>
/// </remarks>
internal static class ClickExample
{
    /// <summary>The text the browser types, and the text it should read back.</summary>
    internal const string Typed = "hello";

    /// <summary>What the handler answers, which is the typed text shouted back.</summary>
    internal const string Answered = "HELLO";

    /// <summary>The title the page carries, which the config repeats.</summary>
    internal const string Title = "Click example";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;

        Textbox input;
        Textbox output;

        using (blocks.BeginRender())
        {
            input = blocks.Add(new Textbox { Label = "A", SubmitBtn = true });
            output = blocks.Add(new Textbox { Label = "Out" });
        }

        input.Submit(Shout, [input], [output], apiName: "shout");
    }

    private static object? Shout(string value) => value.ToUpperInvariant();
}

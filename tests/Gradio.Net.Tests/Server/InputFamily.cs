using Gradio.Net;
using DateTime = Gradio.Net.DateTime;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// A page holding the text and number family, so that a browser can be asked whether it can be
/// typed into.
/// </summary>
/// <remarks>
/// <para>
/// The manifest page proves what the components <em>say</em>; this one is for what a person can
/// <em>do</em> with them. It holds one of each component a user types into, with the label each is
/// found by in the browser, and nothing else — a page with fewer moving parts is a page whose
/// failure says which component failed.
/// </para>
/// <para>
/// The fields are the inputs of one binding, and that is what makes them editable: the Python
/// version infers a component's <c>interactive</c> from whether anything reads it, and a page with
/// no binding at all draws every field disabled. The binding itself answers the text it was given,
/// so the page stays a page of fields rather than a page of behaviour.
/// </para>
/// <para>
/// The simple textbox and the simple dropdown are not here, and the reason is the Python version's:
/// the frontend bundle the Python version 6.30.0 ships has no component of either name — a page
/// that uses one makes the browser ask for a custom component bundle, which neither implementation
/// serves. Their config is compared by the manifest page like every other component's; what cannot
/// be shown is that a browser draws them, because the Python version's own browser does not.
/// </para>
/// </remarks>
internal static class InputFamily
{
    /// <summary>The text typed into the textbox.</summary>
    internal const string Typed = "typed here";

    /// <summary>The number typed into the number field.</summary>
    internal const string Counted = "42";

    /// <summary>The option selected in the dropdown.</summary>
    internal const string Chosen = "medium";

    /// <summary>The option ticked in the coloured checkbox group.</summary>
    internal const string Ticked = "second";

    /// <summary>The option chosen in the radio group.</summary>
    internal const string Picked = "green";

    /// <summary>The text typed into the multimodal field.</summary>
    internal const string Message = "with a picture";

    /// <summary>The moment typed into the date and time field.</summary>
    internal const string Moment = "2020-10-01 05:20:15";

    /// <summary>The colour typed into the colour picker.</summary>
    internal const string Tint = "#00ff00";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var name = blocks.Add(new Textbox { Label = "Name", Placeholder = "Your name" });
            var age = blocks.Add(new Number { Label = "Age" });
            var level = blocks.Add(new Slider { Label = "Level", Minimum = 0, Maximum = 10, Value = 5 });
            var agree = blocks.Add(new Checkbox { Label = "Agree" });
            var letters = blocks.Add(new CheckboxGroup
            {
                Label = "Letters",
                Choices = [new Choice("alpha"), new Choice("beta")],
            });
            var size = blocks.Add(new Dropdown
            {
                Label = "Size",
                Choices = [new Choice("small"), new Choice(Chosen), new Choice("large")],
                Value = null,
            });
            var one = blocks.Add(new Radio
            {
                Label = "One of them",
                Choices = [new Choice("red"), new Choice(Picked), new Choice("blue")],
            });
            var message = blocks.Add(new MultimodalTextbox { Label = "Message" });
            var tint = blocks.Add(new ColorPicker { Label = "Tint" });
            var when = blocks.Add(new DateTime { Label = "When", Type = "string" });
            var snippet = blocks.Add(new Code { Label = "Snippet", Value = "x = 1", Language = "python" });
            var coloured = blocks.Add(new ColoredCheckboxGroup
            {
                Label = "Coloured",
                Choices = ["first", Ticked],
                Colors = ["#111111", "#222222"],
            });

            // One binding, every field an input of it: that is what tells the frontend the fields
            // are for reading rather than for showing.
            name.Submit(
                text => text,
                [name, age, level, agree, letters, size, one, message, tint, when, snippet, coloured],
                []);
        }
    }
}

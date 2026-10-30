using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// What a factory produces: the same block the core API produces, spelled the Python way.
/// </summary>
/// <remarks>
/// <para>
/// The claim a port depends on is that <c>gr.Textbox(value: "x")</c> and
/// <c>new Textbox { Value = "x" }</c> are not two things that behave alike — they are the same
/// thing, built two ways. The strongest form of that is the config: the document a browser boots
/// from, compared byte for byte, for the whole page rather than for the one block.
/// </para>
/// <para>
/// <c>app_id</c> is the one field left out of the comparison: it identifies the application, and two
/// applications are two applications.
/// </para>
/// </remarks>
public sealed class GrFactoryTests
{
    [Fact]
    public void A_factory_builds_the_same_page_as_the_core_api()
    {
        Func<string, string> greet = name => $"Hello, {name}!";

        var core = GrFactories.ConfigOf(demo =>
        {
            var input = demo.Add(new Textbox
            {
                Label = "Name",
                Placeholder = "type here",
                MaxLines = 3,
                Info = "the name",
                Scale = 2,
                MaxLength = 40,
                ElemClasses = ["wide"],
                Visible = Visibility.Collapsed,
            });

            var output = demo.Add(new Textbox { Label = "Greeting" });

            demo.Add(new Button { Value = "Greet", Variant = "primary", Size = "sm" })
                .Click(greet, [input], [output]);
        });

        // The ported side writes the lines and nothing else: a factory puts its block on the page
        // being written, so `gr.Textbox(...)` is a line of the page rather than a value to be added.
        var ported = GrFactories.LayerConfigOf(() =>
        {
            var input = gr.Textbox(
                label: "Name",
                placeholder: "type here",
                maxLines: 3,
                info: "the name",
                scale: 2,
                maxLength: 40,
                elemClasses: "wide",
                visible: false);

            var output = gr.Textbox(label: "Greeting");

            gr.Button("Greet", variant: "primary", size: "sm")
                .Click(greet, [input], [output]);
        });

        Assert.Equal(core.ToJsonString(), ported.ToJsonString());
    }

    [Fact]
    public void A_factory_that_is_given_nothing_builds_the_block_the_core_api_builds()
    {
        // The defaults are the Python version's, which is the half of a signature a port relies on most: a
        // parameter left out has to mean what it means in the Python version, not what this build finds
        // convenient.
        Assert.Equal(
            GrFactories.ConfigOf(demo => demo.Add(new Textbox())).ToJsonString(),
            GrFactories.LayerConfigOf(() => gr.Textbox()).ToJsonString());
        Assert.Equal(
            GrFactories.ConfigOf(demo => demo.Add(new Button())).ToJsonString(),
            GrFactories.LayerConfigOf(() => gr.Button()).ToJsonString());
    }

    [Fact]
    public void A_factory_returns_the_core_type_itself()
    {
        // Not a wrapper: `outputs` takes the block directly, which is the line a port writes most.
        Assert.IsType<Textbox>(gr.Textbox());
        Assert.IsType<Button>(gr.Button());

        // The application too — and it is disposed rather than dropped, because the factory opens
        // the render the Python version's `with` opens, and an application nobody closes would leave that
        // scope open for the rest of the flow.
        using var demo = gr.Blocks();
        Assert.IsType<Blocks>(demo);
    }

    [Theory]
    [InlineData("hidden", "\"hidden\"")]
    [InlineData(false, "false")]
    [InlineData(true, "true")]
    public void Visible_accepts_the_three_spellings_upstream_accepts(object visible, string expected)
    {
        // The Python version's `visible` is `bool | Literal["hidden"]`, and the third value is not a nuance: a
        // block hidden that way keeps the space it would have taken.
        var entry = LayerEntryOf(() => gr.Textbox(visible: visible));

        Assert.Equal(expected, entry["props"]!["visible"]!.ToJsonString());
    }

    [Fact]
    public void A_value_the_framework_cannot_read_is_refused_rather_than_ignored()
    {
        // `inputs` takes one component or several; a string is neither, and quietly wrapping it
        // would put a block nobody meant into the page.
        Assert.Throws<ArgumentException>(() => gr.Textbox(inputs: "not a component"));
        Assert.Throws<ArgumentException>(() => gr.Textbox(elemClasses: 7));
    }

    [Fact]
    public void The_parameters_a_render_keeps_are_upstreams_default()
    {
        // `preserved_by_key="value"` the Python version: a re-render with the same key keeps what the user
        // typed rather than resetting it to the constructor's value.
        var entry = LayerEntryOf(() => gr.Textbox());

        Assert.Equal("""["value"]""", entry["props"]!["preserved_by_key"]!.ToJsonString());
    }

    [Fact]
    public void A_list_of_choices_is_written_the_way_upstream_writes_it()
    {
        // `gr.Radio(choices=["a", "b"])` is the line a port writes, and it has to compile: a choice
        // whose label is its own text is what a bare value means in the Python version, so the factory's
        // `IReadOnlyList<Choice>?` accepts the strings and the numbers without a wrapper at the
        // call site. A pair keeps its explicit spelling, because `("Two", 2)` reads two ways.
        var entry = LayerEntryOf(() => gr.Radio(choices: ["a", "b", 2]), type: "radio");

        Assert.Equal(
            """[["a","a"],["b","b"],["2",2]]""",
            entry["props"]!["choices"]!.ToJsonString());
    }

    /// <summary>The config entry of the one block a page written by the layer holds.</summary>
    /// <param name="build">Writes the page.</param>
    /// <param name="type">The block's type in the config, which is the class name lower-cased.</param>
    /// <returns>The entry, found by its type rather than by its position.</returns>
    private static JsonObject LayerEntryOf(Action build, string type = "textbox")
    {
        Blocks demo;

        using (demo = gr.Blocks())
        {
            build();
        }

        return demo.GetConfig()["components"]!.AsArray()
            .OfType<JsonObject>()
            .Single(entry => entry["type"]!.GetValue<string>() == type);
    }
}

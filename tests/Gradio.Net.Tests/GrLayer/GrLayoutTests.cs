using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The layouts, written the way the Python version writes them: a factory for the block and a scope
/// for what goes inside it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>with gr.Row():</c> is two things at once — a block placed where the line
/// is, and a scope for the lines under it — and <c>using (gr.Row())</c> is both of them, because
/// the row is entered when it is built and closed when it is disposed. What this file is about is
/// the tree that produces: it has to be the tree the core API produces, because a port's nesting is
/// the part of its layout nobody reads twice.
/// </para>
/// <para>
/// The factories are written by hand rather than generated, because a layout's factory
/// and its scope are one feature. The parameter names, their order and their defaults are still
/// compared mechanically — <see cref="GrParametersTests"/> and <see cref="GrDefaultsTests"/> walk
/// every factory, layouts included — so the hand-written half is the prose, not the signature.
/// </para>
/// </remarks>
public sealed class GrLayoutTests
{
    /// <summary>The class list every layout is given in the shared-parameter test.</summary>
    private static readonly string[] _classes = ["c"];

    /// <summary>The list a re-render keeps, in the same test.</summary>
    private static readonly string[] _preserved = ["v"];

    [Fact]
    public void A_layout_is_a_block_placed_where_it_is_written_and_a_scope_around_what_follows()
    {
        // The Python version:
        //
        //     with gr.Row():
        //         with gr.Column(scale=2):
        //             gr.Markdown("left")
        //         with gr.Tabs():
        //             with gr.Tab("One"):
        //                 gr.Markdown("right")
        //
        // Read as a tree: the row holds a column and a tabs; the tabs holds a tab; each of the two
        // holds its markdown. Written here exactly as the Python reads: `with` is `using`, and a
        // line inside a block is a line inside the block.
        using var demo = gr.Blocks();

        gr.Textbox(label: "Outside");

        using (gr.Row())
        {
            using (gr.Column(scale: 2))
            {
                gr.Markdown(value: "left");
            }

            using (gr.Tabs())
            {
                using (gr.Tab("One"))
                {
                    gr.Markdown(value: "right");
                }
            }
        }

        // `tabitem` rather than `tab`: the class is a `Tab` and the config's type is the Python version's
        // spelling of it, which is what the frontend dispatches on.
        Assert.Equal(
            "blocks[textbox, row[column[markdown], tabs[tabitem[markdown]]]]",
            Shape(demo.GetConfig()));
    }

    [Fact]
    public void The_nine_layouts_upstream_exports_are_factories_returning_the_core_types()
    {
        // The return type is the point: `outputs` and `inputs` take a block, and a layout's factory
        // that returned a wrapper would make every nested line unwrap twice.
        Assert.IsType<Row>(gr.Row());
        Assert.IsType<Column>(gr.Column());
        Assert.IsType<Group>(gr.Group());
        Assert.IsType<Accordion>(gr.Accordion());
        Assert.IsType<Tabs>(gr.Tabs());
        Assert.IsType<Tab>(gr.Tab());
        Assert.IsType<Sidebar>(gr.Sidebar());
        Assert.IsType<Walkthrough>(gr.Walkthrough());
        Assert.IsType<WalkthroughStep>(gr.Step());
    }

    [Fact]
    public void TabItem_is_the_other_spelling_of_Tab()
    {
        // The Python version exports `TabItem = Tab`, so a ported file writes whichever of the two it wrote in
        // Python, and the two have to build the same block.
        var fromAlias = GrFactories.EntryOf(() => gr.TabItem("One"));
        var fromPrimary = GrFactories.EntryOf(() => gr.Tab("One"));

        Assert.Equal(fromPrimary.ToJsonString(), fromAlias.ToJsonString());
    }

    [Fact]
    public void Leaving_the_using_block_groups_what_was_written_and_finishes_the_render()
    {
        // The Python version's `with` ends by grouping: a form component written inside a column is put into
        // the form it expects when the block closes, and the page's load bindings are attached then.
        // `using (var demo = gr.Blocks())` has to end the same way — otherwise a ported application
        // would serve a page whose fields were never grouped, and nothing between the braces says so.
        Blocks demo;

        using (demo = gr.Blocks())
        {
            using (gr.Column())
            {
                gr.Textbox(label: "Name");
            }
        }

        Assert.True(demo.IsRendered);

        var column = Assert.Single(demo.Registered.OfType<Column>());
        var form = Assert.Single(column.Children.OfType<Form>());

        Assert.IsType<Textbox>(Assert.Single(form.Children));
    }

    [Fact]
    public void The_multipage_application_written_with_gr_is_the_one_the_core_api_writes()
    {
        // The acceptance the ticket asks for, in its strongest form: the same application written
        // twice — layouts, components, a binding, a load, a second page — compared as the document a
        // browser boots from. Only `app_id` is left out: two applications are two applications.
        static void Core(Blocks demo)
        {
            var name = demo.Add(new Textbox { Label = "Name" });

            using (demo.Enter(demo.Add(new Row { Variant = "panel", EqualHeight = true })))
            {
                using (demo.Enter(demo.Add(new Column { Scale = 2, MinWidth = 200 })))
                {
                    demo.Add(new Textbox { Label = "Inner", Placeholder = "type here" });
                }

                using (demo.Enter(demo.Add(new Tabs { Selected = "Two" })))
                {
                    using (demo.Enter(demo.Add(new Tab { Label = "One" })))
                    {
                        demo.Add(new Markdown { Value = "# one" });
                    }

                    using (demo.Enter(demo.Add(new Tab { Label = "Two", RenderChildren = true })))
                    {
                        demo.Add(new Markdown { Value = "# two" });
                    }
                }
            }

            demo.Add(new Button { Value = "Greet", Variant = "primary" })
                .Click(Greet, [name], [], apiName: "greet");

            demo.Load(Reload, [name]);

            using (demo.Route("About"))
            {
                demo.Add(new Markdown { Value = "about" });
            }
        }

        static void Ported(Blocks demo)
        {
            var name = gr.Textbox(label: "Name");

            using (gr.Row(variant: "panel", equalHeight: true))
            {
                using (gr.Column(scale: 2, minWidth: 200))
                {
                    gr.Textbox(label: "Inner", placeholder: "type here");
                }

                using (gr.Tabs(selected: "Two"))
                {
                    using (gr.Tab("One"))
                    {
                        gr.Markdown(value: "# one");
                    }

                    using (gr.Tab("Two", renderChildren: true))
                    {
                        gr.Markdown(value: "# two");
                    }
                }
            }

            gr.Button("Greet", variant: "primary")
                .Click(Greet, [name], [], apiName: "greet");

            demo.Load(Reload, [name]);

            using (demo.Route("About"))
            {
                gr.Markdown(value: "about");
            }
        }

        var core = GrFactories.ConfigOf(Core);
        var ported = GrFactories.LayerConfigOf(Ported);

        Assert.Equal(core.ToJsonString(), ported.ToJsonString());

        // Two empty documents would be equal too, so the comparison is anchored: the tree is the
        // one both sides were written to produce, the second page is there, and the bindings are
        // the two the application made. Every textbox sits in a form, the one written straight into
        // the application included, which is the grouping every page gets whether it mentions a form
        // or not.
        Assert.Equal(
            "blocks[form[textbox], row[column[form[textbox]], tabs[tabitem[markdown], tabitem[markdown]]], button, markdown]",
            Shape(ported));
        Assert.Equal(2, ported["pages"]!.AsArray().Count);
        Assert.Equal(2, ported["dependencies"]!.AsArray().Count);
    }

    private static string Greet(string name) => $"Hello, {name}!";

    private static void Reload(string name)
    {
        _ = name;
    }

    [Fact]
    public void A_layout_disposed_after_its_render_ended_does_nothing()
    {
        // The row below is still open when the application's scope ends, so it still holds the scope
        // it was entered with. Disposing it afterwards must not group it: the render it belonged to
        // is over, and grouping it now would add nodes to an application that has already been
        // written — one that may already be serving.
        Row row;
        Blocks demo;

        using (demo = gr.Blocks())
        {
            row = gr.Row();
            gr.Textbox(label: "Inside");
        }

        var written = demo.GetConfig().ToJsonString();

        row.Dispose();

        Assert.Equal(written, demo.GetConfig().ToJsonString());
    }

    [Fact]
    public void Every_layout_writes_the_parameters_they_all_share()
    {
        // The seven every block declares are written by one helper, and a factory that forgot to
        // call it would compile and build a block that ignores half of what it was given.
        Block[] layouts =
        [
            gr.Row(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Column(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Group(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Accordion(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Tabs(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Tab(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Sidebar(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Walkthrough(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
            gr.Step(elemId: "x", visible: "hidden", elemClasses: _classes, key: "k", preservedByKey: _preserved, render: false),
        ];

        foreach (var layout in layouts)
        {
            Assert.Equal("x", layout.ElemId);
            Assert.Equal(Visibility.Hidden, layout.Visible);
            Assert.Equal(_classes, layout.ElemClasses);
            Assert.Equal("k", layout.Key);
            Assert.Equal(_preserved, ((BlockContext)layout).PreservedByKey);

            // `render` is the one that is not shared with the others: a block written with it off is
            // numbered and placed nowhere, which is what an interface's own components do. Asserted
            // for every layout, because a factory that dropped it would be green on the other eight.
            Assert.False(layout.Render);
        }
    }

    [Fact]
    public void Each_layout_writes_the_parameters_of_its_own()
    {
        // One assertion per parameter that is not shared, because the hand-written signature is
        // where an "almost right" copy would hide: the name is checked against the Python version by the
        // parameter comparison, and what it writes is checked here.
        var row = gr.Row(variant: "panel", scale: 3, height: 100, maxHeight: 200, minHeight: 50, equalHeight: true, showProgress: true);

        Assert.Equal("panel", row.Variant);
        Assert.Equal(3, row.Scale);
        Assert.Equal(100, row.Height);
        Assert.Equal(200, row.MaxHeight);
        Assert.Equal(50, row.MinHeight);
        Assert.True(row.EqualHeight);
        Assert.True(row.ShowProgress);

        var column = gr.Column(scale: 2, minWidth: 200, variant: "compact", showProgress: true);

        Assert.Equal(2, column.Scale);
        Assert.Equal(200, column.MinWidth);
        Assert.Equal("compact", column.Variant);
        Assert.True(column.ShowProgress);

        var accordion = gr.Accordion(label: "More", open: false, height: "10rem", maxHeight: 400);

        Assert.Equal("More", accordion.Label);
        Assert.False(accordion.Open);
        Assert.Equal("10rem", accordion.Height);
        Assert.Equal(400, accordion.MaxHeight);

        var sidebar = gr.Sidebar(label: "Panel", open: false, width: 480, position: "right");

        Assert.Equal("Panel", sidebar.Label);
        Assert.False(sidebar.Open);
        Assert.Equal(480, sidebar.Width);
        Assert.Equal("right", sidebar.Position);

        var tabs = gr.Tabs(selected: "Two", overflowBehavior: "wrap");

        Assert.Equal("Two", tabs.Selected);
        Assert.Equal("wrap", tabs.OverflowBehavior);

        var tab = gr.Tab(
            "One",
            interactive: false,
            id: "first",
            alignment: "right",
            scale: 2,
            height: 300,
            maxHeight: 600,
            renderChildren: true);

        Assert.Equal("One", tab.Label);
        Assert.False(tab.Interactive);
        Assert.Equal("first", tab.TabId);
        Assert.Equal("right", tab.Alignment);
        Assert.Equal(2, tab.Scale);
        Assert.Equal(300, tab.Height);
        Assert.Equal(600, tab.MaxHeight);
        Assert.True(tab.RenderChildren);

        var walkthrough = gr.Walkthrough(selected: 2);

        Assert.Equal(2, walkthrough.Selected);

        var step = gr.Step("Second", interactive: false, id: 2, scale: 3);

        Assert.Equal("Second", step.Label);
        Assert.False(step.Interactive);
        Assert.Equal(2, step.StepId);
        Assert.Equal(3, step.Scale);
    }

    /// <summary>The layout tree of a config, as the block types nested by name.</summary>
    /// <param name="config">The document a browser boots from.</param>
    /// <returns>The tree as text: <c>blocks[row[textbox], tabs[tab]]</c>.</returns>
    /// <remarks>
    /// By type rather than by id, because the ids are assigned in render order and a tree written as
    /// <c>0(1(2))</c> says nothing a reader can check against the code that wrote it. What the ticket
    /// asks about is the nesting, and this prints the nesting. The root is a block context rather
    /// than a component, so it is not in the component list the ids are looked up in — it is the
    /// application, and its type is the application's.
    /// </remarks>
    private static string Shape(JsonObject config)
    {
        var types = config["components"]!.AsArray()
            .OfType<JsonObject>()
            .ToDictionary(entry => entry["id"]!.GetValue<int>(), entry => entry["type"]!.GetValue<string>());

        return Shape(config["layout"]!.AsObject(), types);

        static string Shape(JsonObject node, IReadOnlyDictionary<int, string> types)
        {
            var id = node["id"]!.GetValue<int>();
            var type = types.TryGetValue(id, out var named) ? named : "blocks";

            return node["children"] is JsonArray { Count: > 0 } children
                ? $"{type}[{string.Join(", ", children.Select(child => Shape(child!.AsObject(), types)))}]"
                : type;
        }
    }
}

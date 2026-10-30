using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// Layouts as the render builds them: where their children go, what the grouping does inside them,
/// and what the config says about them.
/// </summary>
/// <remarks>
/// The nesting is the point of these tests. A layout is the first thing that can hold a container,
/// and the grouping a container runs is what decides the order of the ids — an order the config is
/// compared against the Python version field by field, so "the tree looks right" is not the
/// property being held; "the tree is numbered the way the Python version numbers it" is.
/// </remarks>
public sealed class LayoutTests
{
    [Fact]
    public void A_layout_holds_the_blocks_written_inside_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox first;
        Textbox second;

        using (blocks.BeginRender())
        {
            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                first = blocks.Add(new Textbox { Label = "A" });
                second = blocks.Add(new Textbox { Label = "B" });
            }
        }

        var rowBlock = Assert.Single(blocks.Children.OfType<Row>());

        // Two form components written straight into a row are grouped into a form of the row's
        // own, which is the Python version's rule for every container that allows the grouping.
        var form = Assert.Single(rowBlock.Children.OfType<Form>());

        Assert.Equal([first, second], form.Children);
        Assert.Same(form, first.Parent);
        Assert.Same(form, second.Parent);
        Assert.Same(rowBlock, form.Parent);
        Assert.Same(blocks, rowBlock.Parent);
    }

    /// <summary>
    /// An HTML component holds the blocks written inside it, and its template draws them.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>with gr.HTML(...):</c>. Its HTML is a <c>BlockContext</c> as well as
    /// a component, which is how a template can say <c>@children</c>; here a component that answers
    /// <see cref="Block.LayoutChildren"/> is one a scope can be opened over, and the children end
    /// up in the layout tree under the component — which is what the frontend reads to place them.
    /// </remarks>
    [Fact]
    public void An_html_component_holds_the_blocks_written_inside_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Html html;
        Textbox child;

        using (blocks.BeginRender())
        {
            html = blocks.Add(new Html
            {
                HtmlTemplate = "<div class='shell'>@children</div>",
                Value = "a shell",
            });

            using (blocks.Enter(html))
            {
                child = blocks.Add(new Textbox { Label = "Inside" });
            }
        }

        // The textbox is a form component, so the grouping puts it into a form inside the component
        // — which is what the Python version's HTML does too: it is a BlockContext like any other, and a
        // context groups what was written into it when its scope closes.
        var form = Assert.IsType<Form>(Assert.Single(html.Children));

        Assert.Equal([child], form.Children);
        Assert.Same(form, child.Parent);
        Assert.Same(html, form.Parent);

        // The layout tree is what the frontend walks, so the child has to be there rather than only
        // in the component's own list.
        var node = blocks.GetConfig()["layout"]!["children"]!.AsArray()
            .Select(entry => entry!.AsObject())
            .Single(entry => entry["id"]!.GetValue<int>() == html.Id);

        Assert.Equal(
            [form.Id],
            node["children"]!.AsArray().Select(entry => entry!["id"]!.GetValue<int>()));
    }

    /// <summary>
    /// A block that holds nothing cannot be entered, and says so.
    /// </summary>
    /// <remarks>
    /// The scope is what makes the tree, so a component that cannot hold children has nowhere to put
    /// what is written inside it. Refusing is the honest answer: silently adding to the container
    /// around it would put blocks in a branch the application did not write them in.
    /// </remarks>
    [Fact]
    public void Entering_a_block_that_holds_nothing_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var textbox = blocks.Add(new Textbox { Label = "Not a container" });

            var refused = Assert.Throws<InvalidOperationException>(() => blocks.Enter(textbox));

            Assert.Contains("does not hold other blocks", refused.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_container_the_grouping_synthesises_is_numbered_before_the_siblings_that_follow()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                blocks.Add(new Textbox { Label = "In the row" });
            }

            blocks.Add(new Textbox { Label = "After the row" });
        }

        // The Python version groups a container's children when that container closes, so the form it
        // synthesises inside the row is numbered before the textbox written after the row, and the
        // second form — the one the root's own grouping makes — comes last. Numbering both at the
        // end of the render would swap 3 and 4, and the config would describe a different
        // application than the Python version's for the same code.
        Assert.Equal(
            [typeof(Row), typeof(Textbox), typeof(Form), typeof(Textbox), typeof(Form)],
            blocks.Registered.Select(block => block.GetType()));

        Assert.Equal([1, 2, 3, 4, 5], blocks.Registered.Select(block => block.Id));
    }

    [Fact]
    public void A_compact_layout_does_not_group_the_components_written_into_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var column = blocks.Add(new Column { Variant = "compact" });

            using (blocks.Enter(column))
            {
                blocks.Add(new Textbox { Label = "A" });
                blocks.Add(new Textbox { Label = "B" });
            }
        }

        // A compact layout is drawn as one block; a form inside it would be a second border.
        var columnBlock = Assert.Single(blocks.Children.OfType<Column>());

        Assert.Equal(2, columnBlock.Children.Count);
        Assert.Empty(columnBlock.Children.OfType<Form>());
    }

    [Fact]
    public void A_form_synthesised_in_a_row_takes_the_width_of_the_components_it_holds()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                blocks.Add(new Textbox { Label = "Wide", Scale = 2 });
                blocks.Add(new Textbox { Label = "Narrow", Scale = 1, MinWidth = 100 });
            }
        }

        var rowBlock = Assert.Single(blocks.Children.OfType<Row>());
        var form = Assert.Single(rowBlock.Children.OfType<Form>());

        // The Python version's Form.add_child: in a row both the scale and the minimum width of every
        // component it holds are added up, which is how a form takes its share of the row rather
        // than all of it.
        Assert.Equal(3, form.Scale);
        Assert.Equal(260, form.MinWidth);
    }

    [Fact]
    public void A_tab_written_straight_into_a_page_is_grouped_into_a_tabs()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Tab tab;

        using (blocks.BeginRender())
        {
            tab = blocks.Add(new Tab { Label = "First" });
        }

        var tabs = Assert.Single(blocks.Children.OfType<Tabs>());

        Assert.Same(tabs, tab.Parent);
        Assert.Equal([tab], tabs.Children);
    }

    [Fact]
    public void A_tab_is_a_tabitem_in_the_config_and_a_tab_in_its_props()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Tab tab;

        using (blocks.BeginRender())
        {
            tab = blocks.Add(new Tab { Label = "First", TabId = 7, RenderChildren = true });
        }

        var entry = Entry(blocks, tab);

        // The Python version separates the two names: the type is what the frontend dispatches on, and the
        // prop is the class name an application is rebuilt from.
        Assert.Equal("tabitem", entry["type"]!.GetValue<string>());

        var props = entry["props"]!.AsObject();

        Assert.Equal("tab", props["name"]!.GetValue<string>());
        Assert.Equal(7, props["id"]!.GetValue<int>());
        Assert.True(props["render_children"]!.GetValue<bool>());
        Assert.Equal("left", props["alignment"]!.GetValue<string>());
    }

    [Fact]
    public void A_layout_writes_every_parameter_upstream_declares()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Row row;

        using (blocks.BeginRender())
        {
            row = blocks.Add(new Row
            {
                Variant = "panel",
                Scale = 2,
                Height = 200,
                MaxHeight = "20rem",
                MinHeight = 100,
                EqualHeight = true,
                ShowProgress = true,
                ElemId = "the-row",
                ElemClasses = ["wide"],
            });
        }

        var props = Entry(blocks, row)["props"]!.AsObject();

        Assert.Equal("panel", props["variant"]!.GetValue<string>());
        Assert.Equal(2, props["scale"]!.GetValue<int>());
        Assert.Equal(200, props["height"]!.GetValue<int>());

        // A height is a number of pixels or a CSS length, and both reach the config as what they
        // are: the Python version writes whichever one the application gave it.
        Assert.Equal("20rem", props["max_height"]!.GetValue<string>());
        Assert.Equal(100, props["min_height"]!.GetValue<int>());
        Assert.True(props["equal_height"]!.GetValue<bool>());
        Assert.True(props["show_progress"]!.GetValue<bool>());
        Assert.Equal("the-row", props["elem_id"]!.GetValue<string>());
        Assert.Equal(["wide"], props["elem_classes"]!.AsArray().Select(item => item!.GetValue<string>()));
        Assert.True(props["visible"]!.GetValue<bool>());
        Assert.Equal([], props["preserved_by_key"]!.AsArray());
        Assert.False(props.ContainsKey("render"));
    }

    /// <summary>
    /// A block can be shown, collapsed, or hidden — and the config spells all three differently.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>visible</c> is a boolean or the word <c>hidden</c>, and the frontend
    /// tells the last two apart: a collapsed block gives up its place in the layout, a hidden one
    /// keeps it. A boolean-only property could not describe an application the Python version
    /// describes, which is why the third value is a value of its own rather than a flag.
    /// </remarks>
    [Fact]
    public void A_layout_is_shown_collapsed_or_hidden()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Row shown;
        Row collapsed;
        Row hidden;

        using (blocks.BeginRender())
        {
            shown = blocks.Add(new Row());
            collapsed = blocks.Add(new Row { Visible = false });
            hidden = blocks.Add(new Row { Visible = Visibility.Hidden });
        }

        Assert.Equal("true", Entry(blocks, shown)["props"]!["visible"]!.ToJsonString());
        Assert.Equal("false", Entry(blocks, collapsed)["props"]!["visible"]!.ToJsonString());
        Assert.Equal("\"hidden\"", Entry(blocks, hidden)["props"]!["visible"]!.ToJsonString());

        // Both kinds of "not drawn" read as false, which is what code asking the question wants;
        // only the spelling in the config separates them.
        Assert.False(collapsed.Visible);
        Assert.False(hidden.Visible);
        Assert.True(hidden.Visible.KeepsSpace);
    }

    [Fact]
    public void A_tabs_refuses_a_third_overflow_behaviour()
    {
        var tabs = new Tabs();

        Assert.Throws<ArgumentException>(() => tabs.OverflowBehavior = "scroll");
        Assert.Equal("menu", tabs.OverflowBehavior);
    }

    [Fact]
    public void Entering_a_layout_that_is_not_open_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var outer = blocks.Add(new Row());
            var inner = blocks.Add(new Row());

            using (blocks.Enter(outer))
            {
                // The inner row was written after the outer one closed, so it is the outer row's
                // sibling rather than its child; entering it now would move blocks into a branch
                // they were not written under.
                Assert.Throws<InvalidOperationException>(() => blocks.Enter(inner));
            }
        }
    }

    private static JsonObject Entry(Blocks blocks, Block block) =>
        blocks.GetConfig()["components"]!
            .AsArray()
            .Select(component => component!.AsObject())
            .Single(component => component["id"]!.GetValue<int>() == block.Id);
}

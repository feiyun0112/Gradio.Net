using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The two moments a block is given an identity and a place: a block added with
/// <see cref="Block.Render"/> turned off is numbered where it was constructed and placed where it is
/// finally written.
/// </summary>
/// <remarks>
/// The distinction exists because the Python version numbers a block when it is constructed and
/// attaches it when it is rendered, and an <c>Interface</c> is the case where the two orders
/// disagree: its components are built before the layout that holds them, so they are numbered first
/// and appear in the tree second. Everything here is about the two lists that difference touches —
/// the children a layout draws and the components the config describes — and about the refusal that
/// keeps a block from being rendered twice.
/// </remarks>
public sealed class ReservedBlockTests
{
    [Fact]
    public void A_reserved_block_is_numbered_where_it_was_added_and_placed_where_it_is_rendered()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox reserved;

        using (blocks.BeginRender())
        {
            reserved = blocks.Add(new Textbox { Label = "First", Render = false });

            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                blocks.Attach(reserved);
            }
        }

        // The id came from the add, which is before the row; the place came from the attach, which
        // is inside it.
        Assert.Equal(1, reserved.Id);
        Assert.Equal(2, blocks.Children[0].Id);
        Assert.Same(blocks.Children[0], reserved.Parent!.Parent);

        // The config describes the row and the form inside it, not the component at the top level.
        Assert.Equal(
            [typeof(Row), typeof(Textbox), typeof(Form)],
            blocks.Registered.Select(block => block.GetType()));
        Assert.Single(blocks.Children);
    }

    [Fact]
    public void A_reserved_block_the_render_never_placed_is_not_described()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox reserved;

        using (blocks.BeginRender())
        {
            reserved = blocks.Add(new Textbox { Render = false });
            blocks.Add(new Number { Label = "Kept" });
        }

        Assert.Equal(1, reserved.Id);
        Assert.Empty(blocks.Children.OfType<Textbox>());
        Assert.DoesNotContain(reserved, blocks.Registered);
    }

    [Fact]
    public void A_block_cannot_be_placed_twice()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox reserved;

        using (blocks.BeginRender())
        {
            reserved = blocks.Add(new Textbox { Render = false });
            blocks.Attach(reserved);
        }

        using (blocks.BeginRender())
        {
            var refused = Assert.Throws<InvalidOperationException>(() => blocks.Attach(reserved));

            Assert.Contains("already been rendered", refused.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_block_another_root_reserved_cannot_be_placed_here()
    {
        using var provider = KernelHost.BuildProvider();
        using var first = provider.CreateBlocksWithContainers();
        using var second = provider.CreateBlocksWithContainers();

        Textbox reserved;

        using (first.BeginRender())
        {
            reserved = first.Add(new Textbox { Render = false });
        }

        using (second.BeginRender())
        {
            var refused = Assert.Throws<InvalidOperationException>(() => second.Attach(reserved));

            Assert.Contains("not waiting to be rendered by this application", refused.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_block_that_was_never_added_cannot_be_placed()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var refused = Assert.Throws<InvalidOperationException>(() => blocks.Attach(new Textbox()));

            Assert.Contains("not waiting to be rendered", refused.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_root_answers_whether_it_is_the_one_being_rendered()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        using var other = provider.CreateBlocks();

        Assert.False(blocks.IsRendering);

        using (blocks.BeginRender())
        {
            Assert.True(blocks.IsRendering);
            Assert.False(other.IsRendering);
        }

        Assert.False(blocks.IsRendering);
    }
}

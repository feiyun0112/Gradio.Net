using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The render context is ambient for the length of a render scope and gone afterwards. It is what
/// replaces the process-wide static context, so that a render is a scope rather than a mode the
/// whole process is in.
/// </summary>
public sealed class RenderContextTests
{
    [Fact]
    public void Nothing_is_ambient_before_a_render_starts()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();

        Assert.Null(accessor.Current);
    }

    [Fact]
    public void A_render_scope_makes_its_own_root_the_ambient_context()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var blocks = provider.CreateBlocks();

        using (blocks.BeginRender())
        {
            var context = accessor.Current;

            Assert.NotNull(context);
            Assert.Same(blocks, context.Root);
        }
    }

    [Fact]
    public void The_context_is_no_longer_readable_once_the_render_scope_has_ended()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var blocks = provider.CreateBlocks();

        using (blocks.BeginRender())
        {
            Assert.NotNull(accessor.Current);
        }

        Assert.Null(accessor.Current);
    }

    [Fact]
    public void Nested_render_scopes_restore_the_outer_context()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var outer = provider.CreateBlocks();
        using var inner = provider.CreateBlocks();

        using (outer.BeginRender())
        {
            using (inner.BeginRender())
            {
                Assert.Same(inner, accessor.Current!.Root);
            }

            Assert.Same(outer, accessor.Current!.Root);
        }

        Assert.Null(accessor.Current);
    }

    [Fact]
    public void Disposing_a_render_scope_twice_does_not_disturb_the_outer_scope()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var outer = provider.CreateBlocks();
        using var inner = provider.CreateBlocks();

        using (outer.BeginRender())
        {
            var scope = inner.BeginRender();

            scope.Dispose();
            scope.Dispose();

            Assert.Same(outer, accessor.Current!.Root);
        }
    }

    [Fact]
    public void Disposing_the_root_releases_a_render_scope_that_is_still_open()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        var blocks = provider.CreateBlocks();

        blocks.BeginRender();
        Assert.NotNull(accessor.Current);

        // A scope that was left open would otherwise keep its context readable for the rest of the
        // flow; disposing the root - which is what a container does with a service it owns - ends it.
        blocks.Dispose();

        Assert.Null(accessor.Current);
    }

    [Fact]
    public void A_root_can_render_again_after_its_using_block_ends()
    {
        // Dispose ends the scopes; it does not take the application with it.
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var blocks = provider.CreateBlocks();

        blocks.Dispose();

        using var scope = blocks.BeginRender();

        Assert.Same(blocks, accessor.Current!.Root);
    }

    [Fact]
    public void A_layout_left_open_does_not_capture_what_is_written_after_the_block()
    {
        // A layout entered and never left — because the code inside it threw, say — must not stay
        // the place blocks go: what is written after the using block belongs to the application,
        // not to the layout the block happened to end inside.
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        var blocks = provider.CreateBlocksWithContainers();
        Row row;
        ProbeBlock inside;

        using (blocks.BeginRender())
        {
            row = blocks.Add(new Row());
            blocks.Enter(row);
            inside = blocks.Add(new ProbeBlock());
        }

        // The layout did hold what was written inside it, so the assertion below is about the
        // block written afterwards rather than about an empty row.
        Assert.Same(row, inside.Parent);
        Assert.Null(accessor.Current);

        ProbeBlock after;
        using (blocks.BeginRender())
        {
            after = blocks.Add(new ProbeBlock());
        }

        Assert.Same(blocks, after.Parent);
        Assert.Single(row.Children);
    }

    [Fact]
    public async Task A_render_in_one_flow_is_invisible_to_another_flow()
    {
        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        using var blocks = provider.CreateBlocks();

        var entered = new TaskCompletionSource();
        var release = new TaskCompletionSource();

        var rendering = Task.Run(
            async () =>
            {
                using (blocks.BeginRender())
                {
                    entered.SetResult();
                    await release.Task;
                }
            },
            TestContext.Current.CancellationToken);

        await entered.Task;

        // The rendering flow has a context; this one must not see it.
        Assert.Null(accessor.Current);

        release.SetResult();
        await rendering;

        Assert.Null(accessor.Current);
    }
}

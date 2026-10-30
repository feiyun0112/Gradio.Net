using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// Two render roots coexist in one process without seeing each other: separate id sequences,
/// separate children, and an ambient context that follows the flow rather than the process.
/// </summary>
public sealed class BlocksTests
{
    [Fact]
    public void Ids_are_allocated_per_root_rather_than_per_process()
    {
        using var provider = KernelHost.BuildProvider();
        using var first = provider.CreateBlocks();
        using var second = provider.CreateBlocks();

        using (first.BeginRender())
        {
            first.Add(new ProbeBlock());
            first.Add(new ProbeBlock());
        }

        using (second.BeginRender())
        {
            second.Add(new ProbeBlock());
        }

        // A process-wide counter would have handed the second root's first block id 4.
        Assert.Equal(0, first.Id);
        Assert.Equal([1, 2], first.Children.Select(child => child.Id));
        Assert.Equal(0, second.Id);
        Assert.Equal([1], second.Children.Select(child => child.Id));
    }

    [Fact]
    public void Two_roots_rendered_one_after_the_other_keep_their_own_children()
    {
        using var provider = KernelHost.BuildProvider();
        using var first = provider.CreateBlocks();
        using var second = provider.CreateBlocks();

        var firstChild = Render(first, "a");
        var secondChild = Render(second, "b");

        Assert.Same(first, firstChild.Parent);
        Assert.Same(second, secondChild.Parent);
        Assert.Equal("a", firstChild.Name);
        Assert.Equal("b", secondChild.Name);
        Assert.Single(first.Children);
        Assert.Single(second.Children);
    }

    [Fact]
    public async Task Roots_rendered_concurrently_do_not_cross_contaminate()
    {
        const int RootCount = 8;
        const int BlocksPerRoot = 32;

        using var provider = KernelHost.BuildProvider();
        var accessor = provider.CreateAccessor();
        var roots = Enumerable.Range(0, RootCount).Select(_ => provider.CreateBlocks()).ToArray();

        try
        {
            var renders = roots.Select(root => Task.Run(
                async () =>
                {
                    using var scope = root.BeginRender();

                    for (var index = 0; index < BlocksPerRoot; index++)
                    {
                        // Yield so that the renders interleave; a shared ambient context would show up
                        // here as a root seeing a block that belongs to another one.
                        await Task.Yield();

                        Assert.Same(root, accessor.Current!.Root);
                        root.Add(new ProbeBlock());
                    }
                },
                TestContext.Current.CancellationToken)).ToArray();

            await Task.WhenAll(renders);
        }
        finally
        {
            foreach (var root in roots)
            {
                root.Dispose();
            }
        }

        foreach (var root in roots)
        {
            Assert.Equal(BlocksPerRoot, root.Children.Count);
            Assert.Equal(Enumerable.Range(1, BlocksPerRoot), root.Children.Select(child => child.Id));
            Assert.All(root.Children, child => Assert.Same(root, child.Parent));
        }
    }

    [Fact]
    public void Adding_a_block_outside_a_render_scope_is_rejected()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        Assert.Throws<InvalidOperationException>(() => blocks.Add(new ProbeBlock()));
    }

    [Fact]
    public void Adding_a_block_while_a_different_root_is_rendering_is_rejected()
    {
        using var provider = KernelHost.BuildProvider();
        using var rendering = provider.CreateBlocks();
        using var other = provider.CreateBlocks();

        using (rendering.BeginRender())
        {
            Assert.Throws<InvalidOperationException>(() => other.Add(new ProbeBlock()));
            Assert.Empty(other.Children);
        }
    }

    [Fact]
    public void A_block_can_only_be_rendered_once()
    {
        using var provider = KernelHost.BuildProvider();
        using var first = provider.CreateBlocks();
        using var second = provider.CreateBlocks();
        var block = new ProbeBlock();

        using (first.BeginRender())
        {
            first.Add(block);
        }

        using (second.BeginRender())
        {
            Assert.Throws<InvalidOperationException>(() => second.Add(block));
        }

        Assert.Same(first, block.Parent);
    }

    [Fact]
    public void An_application_can_still_be_written_after_its_using_block_ends()
    {
        // Dispose means "leave the scope", not "destroy the application": the Python version's
        // `with gr.Blocks() as demo:` has to land on `using` unchanged, and the code after the
        // block is what launches it.
        using var provider = KernelHost.BuildProvider();
        Blocks blocks;

        using (blocks = provider.CreateBlocks())
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new NamedProbeBlock { Name = "inside" });
            }
        }

        using (blocks.BeginRender())
        {
            blocks.Add(new NamedProbeBlock { Name = "after" });
        }

        Assert.Equal(["inside", "after"], blocks.Registered.Select(block => ((NamedProbeBlock)block).Name));
        Assert.True(blocks.IsRendered);
    }

    [Fact]
    public void A_second_render_groups_what_it_writes_the_way_the_first_one_did()
    {
        // The application is written in two goes - which is what `using` gives an author who
        // leaves the block and writes more - and the second go is grouped the way the first was: a
        // form component written straight into the application still ends up in a form, because
        // the application is one application, not two.
        using var provider = KernelHost.BuildProvider();
        var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "first" });
        }

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "second" });
        }

        var textboxes = blocks.Registered.OfType<Textbox>().ToArray();

        Assert.Equal(2, textboxes.Length);
        Assert.All(textboxes, textbox => Assert.IsType<Form>(textbox.Parent));
    }

    [Fact]
    public void A_sealed_application_refuses_new_blocks()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        blocks.Seal();

        var error = Assert.Throws<InvalidOperationException>(() => blocks.BeginRender());

        // The refusal has to say what happened: the application is not disposed, it is being served,
        // and the fix is to write the UI before starting the host.
        Assert.Contains("already being served", error.Message, StringComparison.Ordinal);
        Assert.Contains("before starting the host", error.Message, StringComparison.Ordinal);
        Assert.True(blocks.IsSealed);
    }

    [Fact]
    public void Sealing_twice_is_sealing_once()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        blocks.Seal();
        blocks.Seal();

        // The second seal is not an error, and the application is refused just the same.
        Assert.True(blocks.IsSealed);
        Assert.Throws<InvalidOperationException>(() => blocks.BeginRender());
    }

    [Fact]
    public void A_sealed_application_refuses_a_binding()
    {
        // A binding is a write too: it changes the config and the API description the next request
        // is answered with.
        using var provider = KernelHost.BuildProvider();
        var blocks = provider.CreateBlocksWithContainers();
        Textbox textbox;

        using (blocks.BeginRender())
        {
            textbox = blocks.Add(new Textbox());
        }

        blocks.Seal();

        Assert.Throws<InvalidOperationException>(
            () => textbox.Change(
                (Func<string, string>)(static value => value),
                [textbox],
                [textbox]));
    }

    private static NamedProbeBlock Render(Blocks root, string name)
    {
        using var scope = root.BeginRender();

        return root.Add(new NamedProbeBlock { Name = name });
    }

    private sealed class NamedProbeBlock : Block
    {
        internal string Name { get; init; } = string.Empty;
    }
}

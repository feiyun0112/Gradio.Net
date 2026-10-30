using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The two bindings that belong to the application rather than to a component: the one that runs
/// when a page opens, and the one that runs when it goes away.
/// </summary>
/// <remarks>
/// The application is a block like any other here — id 0 — so a binding to its own signal is an
/// ordinary binding whose target is the root. What these assert is that the pair is spelled that
/// way in the config, because the first half is dispatched by the frontend from exactly that
/// target and the second is found by the server from it.
/// </remarks>
public sealed class PageLifecycleTests
{
    [Fact]
    public void A_load_and_an_unload_binding_name_the_application_itself()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        var ran = new List<string>();

        using (blocks.BeginRender())
        {
            blocks.Load((Action)(() => ran.Add("load")));
            blocks.Unload((Action)(() => ran.Add("unload")));
        }

        var targets = blocks.GetConfig()["dependencies"]!
            .AsArray()
            .Select(entry => entry!["targets"]!.ToJsonString())
            .ToArray();

        Assert.Equal(["[[0,\"load\"]]", "[[0,\"unload\"]]"], targets);

        // Binding one is not running it: a load is what the page raises when it opens, and an unload
        // is what the server raises when the session ends.
        Assert.Empty(ran);
    }

    [Fact]
    public void An_unload_binding_may_read_a_state_and_write_none()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        State state;

        using (blocks.BeginRender())
        {
            state = blocks.Add(new State());
        }

        var binding = blocks.Unload((Action<object?>)(_ => { }), [state]);

        // A handler that reads a state has no output to write: the page is gone, so what it did with
        // the value is its own business.
        Assert.Equal([state], binding.Inputs);
        Assert.Empty(binding.Outputs);
    }

    [Fact]
    public void An_application_that_binds_nothing_to_the_end_of_a_page_has_no_unload_work()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        Textbox field;

        using (blocks.BeginRender())
        {
            field = blocks.Add(new Textbox());
            blocks.Load((Action)(() => { }));
        }

        field.Change((Func<string, string>)(value => value), [field], [field]);

        // A load is the page opening, which the frontend dispatches. Nothing here is about the page
        // ending, so the server has nothing to run when it does — and nothing on the page has to
        // tell it that it did.
        Assert.DoesNotContain(
            blocks.GetConfig()["dependencies"]!.AsArray(),
            entry => entry!["targets"]!.ToJsonString().Contains("unload", StringComparison.Ordinal));
        Assert.False(blocks.GetConfig()["connect_heartbeat"]!.GetValue<bool>());
    }
}

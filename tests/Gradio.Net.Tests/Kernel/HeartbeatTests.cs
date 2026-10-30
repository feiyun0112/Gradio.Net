using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// <c>connect_heartbeat</c>, the config field that says whether the frontend must keep a connection
/// open for as long as the page is.
/// </summary>
/// <remarks>
/// The Python version's rule is one question — does anything on the page outlive a single run? —
/// asked of the things this framework can have: a value the session holds, a binding the client
/// feeds a chunk at a time, and work bound to the visitor leaving. Each is asserted here, because
/// the failure they prevent is invisible from the page: a session that needs a heartbeat and is
/// served without one is a session the server never learns has gone.
/// </remarks>
public sealed class HeartbeatTests
{
    [Fact]
    public void A_page_of_fields_asks_for_no_heartbeat()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox());
        }

        // A field is redrawn from its own value on every run; nothing about it outlives the run, so
        // a connection kept open would only tell the server about a page it already knows about.
        Assert.False(Heartbeat(blocks));
    }

    [Fact]
    public void A_page_that_keeps_a_value_in_the_session_asks_for_one()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new State());
        }

        // The session holds the value, so the server has something to reclaim, and the heartbeat is
        // how it finds out that the browser is gone.
        Assert.True(Heartbeat(blocks));
    }

    [Fact]
    public void A_page_with_a_stream_binding_asks_for_one()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var field = blocks.Add(new Textbox());

            blocks.On(
                [new EventTrigger(field, EventNames.Stream)],
                fn: (Func<string, string>)(value => value));
        }

        // A stream binding is fed chunk by chunk, so there is no single request whose end could
        // mean "done" — which is the other half of the Python version's rule.
        Assert.True(Heartbeat(blocks));
    }

    [Fact]
    public void A_page_whose_binding_is_cached_per_session_asks_for_one()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        var cache = new Cache(new CacheOptions { PerSession = true });

        using (blocks.BeginRender())
        {
            var field = blocks.Add(new Textbox());

            blocks.On(
                [new EventTrigger(field, EventNames.Change)],
                fn: cache.Wrap<Func<string, string>>(value => value), inputs: [field]);
        }

        // The cache forgets a session's entries when the session ends, which is a thing the server
        // has to be told about — so a page whose binding is cached that way needs the connection.
        Assert.True(Heartbeat(blocks));
    }

    [Fact]
    public void A_page_whose_binding_is_cached_for_everyone_asks_for_none()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        var cache = new Cache();

        using (blocks.BeginRender())
        {
            var field = blocks.Add(new Textbox());

            blocks.On(
                [new EventTrigger(field, EventNames.Change)],
                fn: cache.Wrap<Func<string, string>>(value => value), inputs: [field]);
        }

        // A shared cache has nothing per session to forget, so the page is served without a
        // connection it would have no use for.
        Assert.False(Heartbeat(blocks));
    }

    [Fact]
    public void A_page_that_runs_something_when_the_visitor_leaves_asks_for_one()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox());
        }

        blocks.Unload((Action)(() => { }));

        // Nothing on this page outlives a run, and it still needs the connection: the connection
        // ending is the only thing that tells the server the visitor has gone, and a handler bound
        // to that moment is a handler that would otherwise never run.
        Assert.True(Heartbeat(blocks));
    }

    /// <summary>The value the application's config carries for the field.</summary>
    private static bool Heartbeat(Blocks blocks) =>
        blocks.GetConfig()["connect_heartbeat"]!.GetValue<bool>();
}

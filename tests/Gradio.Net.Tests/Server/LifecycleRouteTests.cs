using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The two routes that describe the application's own lifecycle rather than one of its
/// capabilities: starting it, and telling a development page that it changed.
/// </summary>
/// <remarks>
/// <para>
/// Both are observed the way a client observes them, over HTTP: the startup route answers a boolean
/// and the reload route answers a stream, and neither has anything else to say. What the boolean
/// means — that the work ran exactly once — is asserted where it happens, in the kernel
/// (<c>StartupEventsTests</c>); what is asserted here is that the host is what runs it.
/// </para>
/// <para>
/// They share the queue's collection because one of them shortens the heartbeat interval through
/// the environment variable the queue's own tests change, and the variable is process-wide.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class LifecycleRouteTests
{
    private const string StartupPath = "/gradio_api/startup-events";
    private const string ReloadPath = "/gradio_api/dev/reload";

    /// <summary>How long a frame may take to arrive before the test calls it missing.</summary>
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    [Fact]
    public async Task The_startup_route_says_the_application_has_already_been_started()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(logs: null, cancellationToken);

        using var first = await host.Client.GetAsync(StartupPath, cancellationToken);
        using var second = await host.Client.GetAsync(StartupPath, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal("application/json", first.Content.Headers.ContentType?.MediaType);

        // The host runs the application's startup work when it starts, which is the Python version's order:
        // its launch asks this very route, and an app mounted on another one runs them from the
        // host's lifespan. So a client that asks afterwards is told the work is done — which is
        // what the Python version tells it too.
        Assert.Equal("false", await first.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal("false", await second.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task The_host_runs_the_application_startup_events_once()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var runs = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
            blocks.AddStartupEvent(_ =>
            {
                Interlocked.Increment(ref runs);

                return Task.CompletedTask;
            }));

        using var first = await host.Client.GetAsync(StartupPath, cancellationToken);
        using var second = await host.Client.GetAsync(StartupPath, cancellationToken);

        // The event ran without anyone asking the route for it, and asking twice more did not run
        // it again: the route reports the work rather than being the only thing that does it.
        Assert.Equal("false", await first.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal("false", await second.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal(1, runs);
        Assert.True(host.Services.GetRequiredService<Blocks>().StartupEventsTriggered);
    }

    [Fact]
    public async Task A_change_is_streamed_to_a_page_watching_for_one()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(logs: null, cancellationToken);
        await using var stream = await SseStream.OpenAsync(host.BaseAddress, ReloadPath, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, stream.StatusCode);
        Assert.Equal("text/event-stream", stream.ContentType);

        var signal = host.Services.GetRequiredService<GradioReloadSignal>();

        signal.AlertReload();

        var first = await stream.UntilAsync(frame => frame.Name == "reload", Patience, cancellationToken);

        Assert.NotNull(first);
        Assert.Equal("{}", first.Value.Data);

        // The second change is what proves the stream keeps listening: a waiter that missed the
        // pulse it was not yet waiting for would report the first change and then go quiet.
        signal.AlertReload();

        Assert.NotNull(await stream.UntilAsync(frame => frame.Name == "reload", Patience, cancellationToken));
    }

    [Fact]
    public async Task An_error_is_streamed_with_the_failure_text()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(logs: null, cancellationToken);
        await using var stream = await SseStream.OpenAsync(host.BaseAddress, ReloadPath, cancellationToken);

        host.Services.GetRequiredService<GradioReloadSignal>().AlertError("the module would not import");

        var frame = await stream.UntilAsync(one => one.Name == "error", Patience, cancellationToken);

        Assert.NotNull(frame);

        // The data is a JSON string rather than an object, which is what the Python version sends and what
        // the page parses before logging it.
        Assert.Equal("the module would not import", JsonNode.Parse(frame.Value.Data)!.GetValue<string>());
    }

    [Fact]
    public async Task The_reload_stream_carries_heartbeats()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second, so that a test can see one without waiting for the
        // default fifteen. The Python version reads the same variable for its queue streams; this server
        // reads it once for every stream it serves.
        using var interval = HeartbeatIntervalVariable.Set("0.2");

        await using var host = await GradioTestHost.StartStandaloneAsync(logs: null, cancellationToken);
        await using var stream = await SseStream.OpenAsync(host.BaseAddress, ReloadPath, cancellationToken);

        var frame = await stream.UntilAsync(one => one.Name == "heartbeat", Patience, cancellationToken);

        Assert.NotNull(frame);
        Assert.Equal("{}", frame.Value.Data);
    }

    [Fact]
    public async Task A_heartbeat_interval_that_cannot_be_read_is_reported_and_the_default_is_used()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var logs = new CapturingLoggerProvider();

        using var interval = HeartbeatIntervalVariable.Set("soon");

        await using var host = await GradioTestHost.StartStandaloneAsync(logs, cancellationToken);
        await using var stream = await SseStream.OpenAsync(host.BaseAddress, ReloadPath, cancellationToken);

        // The value is wrong for the deployment rather than for the request, so it is reported once
        // and the server starts anyway: refusing to start over a typo in a variable it can do
        // without would be worse than a heartbeat at the default rate.
        var warning = Assert.Single(
            logs.Entries,
            entry => entry.Level == LogLevel.Warning
                && entry.Message.Contains("GRADIO_HEARTBEAT_INTERVAL", StringComparison.Ordinal));

        Assert.Contains("soon", warning.Message, StringComparison.Ordinal);

        // And the default really is the long one: a stream that said nothing in a second is a
        // stream whose interval did not collapse to zero when the value could not be read.
        Assert.Null(await stream.NextAsync(TimeSpan.FromSeconds(1), cancellationToken));
    }

    [Fact]
    public async Task A_change_that_happened_before_the_page_connected_is_not_replayed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(logs: null, cancellationToken);

        host.Services.GetRequiredService<GradioReloadSignal>().AlertReload();

        await using var stream = await SseStream.OpenAsync(host.BaseAddress, ReloadPath, cancellationToken);

        // A stream reports what happens from the moment it is opened; the page it belongs to asked
        // for its config when it opened, so a change that predates the connection is already in
        // that config and replaying it would reload the page for nothing.
        Assert.Null(await stream.NextAsync(TimeSpan.FromSeconds(1), cancellationToken));
    }
}

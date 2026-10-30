using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The five events the framework reports, what they carry, and where they stop.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the half of the Python version's analytics this port keeps: the switch,
/// the moment each event happens, and the payload — field for field, because a payload a consumer
/// cannot read is not parity. The other half, the posting, is what the exclusion criteria rule out,
/// and the test that says so is <see cref="AnalyticsOutboundTests"/>: it watches the process's
/// outbound HTTP rather than reading the code.
/// </para>
/// <para>
/// Every test registers its own sink, which is the only thing an application has to do to receive
/// the events. An application that registers none keeps the framework's no-op, and that is the
/// default these tests hold the others against.
/// </para>
/// </remarks>
public sealed class AnalyticsTests
{
    [Fact]
    public void The_switch_is_off_until_the_application_turns_it_on()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        // Off, deliberately, where the Python version is on. The config carries the answer rather
        // than hiding it, so the difference is visible rather than implied.
        Assert.False(blocks.AnalyticsEnabled);
        Assert.False(blocks.GetConfig()["analytics_enabled"]!.GetValue<bool>());

        blocks.AnalyticsEnabled = true;

        Assert.True(blocks.GetConfig()["analytics_enabled"]!.GetValue<bool>());
    }

    [Fact]
    public void The_topics_are_upstreams_addresses()
    {
        // The strings are the contract: a sink that recognises an event does it by this, and a
        // forwarding implementation has nothing to translate. They are asserted as literals so that
        // a change to them is a change someone has to mean.
        Assert.Equal("https://api.gradio.app/pkg-version", AnalyticsTopics.VersionCheck);
        Assert.Equal("https://api.gradio.app/gradio-initiated-analytics/", AnalyticsTopics.Initiated);
        Assert.Equal("https://api.gradio.app/gradio-launched-telemetry/", AnalyticsTopics.Launched);
        Assert.Equal("gradio/vibe", AnalyticsTopics.Vibe);
        Assert.Equal("https://api.gradio.app/gradio-error-analytics/", AnalyticsTopics.Error);
    }

    [Fact]
    public async Task Starting_the_host_reports_the_application_three_times()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;
                AddTextboxes(blocks);
            });

        Assert.Equal(
            [AnalyticsTopics.VersionCheck, AnalyticsTopics.Initiated, AnalyticsTopics.Launched],
            sink.Events.Select(analyticsEvent => analyticsEvent.Topic));

        // The version check is the Python version's one request with no body: what it learns is the answer,
        // so there is nothing to report but the fact that it happened.
        Assert.Empty(sink.Only(AnalyticsTopics.VersionCheck));

        var initiated = sink.Only(AnalyticsTopics.Initiated);

        Assert.Equal("blocks", initiated["mode"]!.GetValue<string>());
        Assert.Equal("6.30.0", initiated["version"]!.GetValue<string>());
        Assert.Equal(2, initiated.Count);
    }

    [Fact]
    public async Task An_application_that_fails_to_start_reports_that_it_was_built_and_not_that_it_launched()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        // A host whose startup work throws is a host that never served anything, and the two reports
        // the Python version puts either side of that work are what says so: the application was built, and it
        // never launched.
        var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            ConfigureServices = services => services.AddSingleton<IAnalyticsSink>(sink),
            ConfigureLogging = logging => logging.ClearProviders(),
        });

        server.Blocks.AnalyticsEnabled = true;
        server.Blocks.AddStartupEvent((Func<CancellationToken, Task>)(_ =>
            throw new InvalidOperationException("the startup work broke")));

        await Assert.ThrowsAnyAsync<Exception>(() => server.StartAsync(cancellationToken));
        await server.DisposeAsync();

        Assert.Equal(
            [AnalyticsTopics.VersionCheck, AnalyticsTopics.Initiated],
            sink.Events.Select(analyticsEvent => analyticsEvent.Topic));
    }

    [Fact]
    public async Task The_launch_report_describes_what_the_application_is_made_of()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Echo,
                    [input],
                    [output],
                    apiName: "echo");
            });

        var launched = sink.Only(AnalyticsTopics.Launched);

        // The Python version's launch parameters, then the description of the application — the order it
        // merges the two dictionaries in, and the reason the keys are asserted as one list.
        Assert.Equal(
            [
                "launch_method",
                "is_google_colab",
                "is_sharing_on",
                "is_space",
                "mode",
                "custom_css",
                "theme",
                "is_custom_theme",
                "custom_js",
                "custom_head",
                "custom_css_paths",
                "custom_head_paths",
                "version",
                "is_hosted_notebook",
                "using_auth",
                "dev_mode",
                "inputs",
                "outputs",
                "targets",
                "blocks",
                "events",
                "using_custom_component",
                "custom_components",
            ],
            launched.Select(entry => entry.Key));

        // A host that is not on Colab, in a Space, sharing, or reading anything but the defaults
        // says so with a value rather than by leaving the key out.
        Assert.Equal("inline", launched["launch_method"]!.GetValue<string>());
        Assert.False(launched["is_google_colab"]!.GetValue<bool>());
        Assert.False(launched["is_sharing_on"]!.GetValue<bool>());
        Assert.False(launched["is_space"]!.GetValue<bool>());
        Assert.Equal("blocks", launched["mode"]!.GetValue<string>());
        Assert.False(launched["custom_css"]!.GetValue<bool>());
        Assert.Equal("default", launched["theme"]!.GetValue<string>());
        Assert.False(launched["is_custom_theme"]!.GetValue<bool>());
        Assert.False(launched["custom_js"]!.GetValue<bool>());
        Assert.False(launched["custom_head"]!.GetValue<bool>());
        Assert.False(launched["custom_css_paths"]!.GetValue<bool>());
        Assert.False(launched["custom_head_paths"]!.GetValue<bool>());
        Assert.Equal("6.30.0", launched["version"]!.GetValue<string>());
        Assert.False(launched["is_hosted_notebook"]!.GetValue<bool>());
        Assert.False(launched["using_auth"]!.GetValue<bool>());
        Assert.False(launched["dev_mode"]!.GetValue<bool>());

        // The description: every rendered block by name, then what the one binding targets, reads
        // and writes, and the signal it listens for.
        Assert.Equal("[\"textbox\",\"textbox\",\"form\"]", launched["blocks"]!.ToJsonString());
        Assert.Equal("[\"textbox\"]", launched["targets"]!.ToJsonString());
        Assert.Equal("[\"textbox\"]", launched["inputs"]!.ToJsonString());
        Assert.Equal("[\"textbox\"]", launched["outputs"]!.ToJsonString());
        Assert.Equal("[\"change\"]", launched["events"]!.ToJsonString());
        Assert.False(launched["using_custom_component"]!.GetValue<bool>());
        Assert.Equal("[]", launched["custom_components"]!.ToJsonString());
    }

    [Fact]
    public async Task An_interface_reports_the_columns_it_was_given()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                _ = new Interface(
                    blocks,
                    (Func<string, double, string>)Describe,
                    inputs: ["textbox", "number"],
                    outputs: "textbox");
            });

        var launched = sink.Only(AnalyticsTopics.Launched);

        // An interface knows its own columns, where a page of blocks can only say which components
        // its bindings happen to name — which is the distinction the Python version draws with
        // `blocks.input_components` and the reason this framework carries the two lists.
        Assert.Equal("interface", launched["mode"]!.GetValue<string>());
        Assert.Equal("[\"textbox\",\"number\"]", launched["inputs"]!.ToJsonString());
        Assert.Equal("[\"textbox\"]", launched["outputs"]!.ToJsonString());
    }

    [Fact]
    public async Task A_theme_of_its_own_is_reported_as_one()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;
                blocks.Theme = ThemePresets.Get("default").Set("body_background_fill", "#123456");
                AddTextboxes(blocks);
            });

        var launched = sink.Only(AnalyticsTopics.Launched);

        // The name is the preset it started from and stays that; what makes it custom is that its
        // variables no longer match any preset — which is the Python version's comparison, and the answer a
        // consumer wants.
        Assert.Equal("default", launched["theme"]!.GetValue<string>());
        Assert.True(launched["is_custom_theme"]!.GetValue<bool>());
    }

    [Fact]
    public async Task A_component_of_the_applications_own_is_reported_by_name()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                using (blocks.BeginRender())
                {
                    blocks.Add(new Callout { Heading = "Note" });
                    blocks.Add(new Textbox { Label = "A" });
                }
            });

        var launched = sink.Only(AnalyticsTopics.Launched);

        // A class this assembly does not define is the application's own, which is the Python version's test
        // asked of an assembly instead of a module.
        Assert.Equal("[\"callout\",\"textbox\",\"form\"]", launched["blocks"]!.ToJsonString());
        Assert.True(launched["using_custom_component"]!.GetValue<bool>());
        Assert.Equal("[\"callout\"]", launched["custom_components"]!.ToJsonString());
    }

    [Fact]
    public async Task Stylesheets_and_a_signer_are_reported()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;
                blocks.Css = "body { color: red; }";

                // Named rather than read: the report is about what the application asked for, and
                // the files themselves are only read when a page is served.
                blocks.CssPaths = [Path.Combine(Path.GetTempPath(), "gradio-analytics.css")];
                blocks.Auth = GradioAuth.OneAccount("someone", "secret");
                AddTextboxes(blocks);
            });

        var launched = sink.Only(AnalyticsTopics.Launched);

        Assert.True(launched["custom_css"]!.GetValue<bool>());
        Assert.True(launched["custom_css_paths"]!.GetValue<bool>());
        Assert.True(launched["using_auth"]!.GetValue<bool>());
    }

    [Fact]
    public async Task The_editor_is_reported_when_the_application_turns_it_on()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;
                blocks.VibeMode = true;
                AddTextboxes(blocks);
            });

        // The Python version posts this from the command that starts the editor; here the editor is a switch
        // and a set of routes, so the moment is the host starting with the switch on. Its payload is
        // the Python version's one key.
        Assert.Equal("{\"command\":\"vibe\"}", sink.Only(AnalyticsTopics.Vibe).ToJsonString());
        Assert.Equal(4, sink.Events.Count);
    }

    [Fact]
    public async Task An_application_that_reports_nothing_reports_nothing()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Fail,
                    [input],
                    [output],
                    apiName: "boom", queue: false);
            });

        // A page, a direct call that raises, and a run through the queue: the three moments that
        // would report something if the switch were on.
        await host.Client.GetStringAsync("/", cancellationToken);
        await FailDirectlyAsync(host, cancellationToken);
        await FailThroughTheQueueAsync(host, cancellationToken);

        Assert.Empty(sink.Events);
    }

    [Fact]
    public async Task A_run_that_raises_is_reported()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Fail,
                    [input],
                    [output],
                    apiName: "boom");
            });

        await FailThroughTheQueueAsync(host, cancellationToken);

        // The Python version's payload for an error is the message and nothing else. What is reported is what
        // the operator's log says, because a report is not a dump.
        Assert.Equal(
            "{\"error\":\"the handler broke\"}",
            sink.Only(AnalyticsTopics.Error).ToJsonString());
    }

    [Fact]
    public async Task A_direct_call_that_raises_is_reported()
    {
        var sink = new RecordingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Fail,
                    [input],
                    [output],
                    apiName: "boom", queue: false);
            });

        await FailDirectlyAsync(host, cancellationToken);

        // A direct call is a run like any other, so its failure is the same event a queued run's is.
        Assert.Equal(
            "{\"error\":\"the handler broke\"}",
            sink.Only(AnalyticsTopics.Error).ToJsonString());
    }

    [Fact]
    public async Task A_sink_that_throws_does_not_break_the_application()
    {
        var sink = new ThrowingAnalyticsSink();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            sink,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Echo,
                    [input],
                    [output],
                    apiName: "echo", queue: false);
            });

        // The host started, which means the three events of the start were reported and the sink
        // failed on each of them; and the application still answers and still runs.
        Assert.Equal(3, sink.Count);

        Assert.Contains("<gradio-app", await host.Client.GetStringAsync("/", cancellationToken), StringComparison.Ordinal);

        using var content = new StringContent(
            "{\"data\":[\"a\"]}",
            Encoding.UTF8,
            "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/run/echo", content, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void The_default_sink_is_the_frameworks_own_no_op()
    {
        using var provider = new ServiceCollection().AddGradio().BuildServiceProvider();

        var sink = provider.GetRequiredService<IAnalyticsSink>();

        // Taking an event and doing nothing with it is what "on by default means nothing leaves"
        // rests on, so the default is asserted rather than assumed.
        sink.Send(new AnalyticsEvent(AnalyticsTopics.Initiated, new JsonObject()));

        // And it is the only implementation in the assembly: a framework that shipped a second one
        // would have shipped a transport.
        var implementations = typeof(IAnalyticsSink).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(IAnalyticsSink).IsAssignableFrom(type))
            .Select(type => type.Name)
            .ToList();

        Assert.Equal(["NullAnalyticsSink"], implementations);
    }

    /// <summary>Starts a host that reports into <paramref name="sink"/>, and writes the application.</summary>
    /// <param name="sink">The sink to register.</param>
    /// <param name="build">Writes the application.</param>
    /// <returns>The started host.</returns>
    private static Task<GradioTestHost> StartAsync(IAnalyticsSink sink, Action<Blocks> build) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            build,
            services => services.AddSingleton(sink));

    /// <summary>Adds the two textboxes most of these applications are made of.</summary>
    /// <param name="blocks">The application to write into.</param>
    /// <returns>The two textboxes.</returns>
    private static (Textbox Input, Textbox Output) AddTextboxes(Blocks blocks)
    {
        Textbox input;
        Textbox output;

        using (blocks.BeginRender())
        {
            input = blocks.Add(new Textbox { Label = "A" });
            output = blocks.Add(new Textbox { Label = "B" });
        }

        return (input, output);
    }

    /// <summary>Calls a failing binding directly, and asserts that it failed.</summary>
    /// <param name="host">The host serving it.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    private static async Task FailDirectlyAsync(GradioTestHost host, CancellationToken cancellationToken)
    {
        using var content = new StringContent("{\"data\":[\"a\"]}", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/run/boom", content, cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    /// <summary>Runs a failing binding through the queue, and asserts that the run failed.</summary>
    /// <param name="host">The host serving it.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    private static async Task FailThroughTheQueueAsync(GradioTestHost host, CancellationToken cancellationToken)
    {
        using var queue = new QueueClient(host.BaseAddress);

        var fnIndex = await queue.EndpointAsync("boom", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, ["a"], "analytics", cancellationToken);

        await using var stream = await queue.OpenAsync("analytics", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.False(completed["success"]!.GetValue<bool>());
    }

    private static string Echo(string value) => value;

    private static string Describe(string name, double age) => $"{name} is {age}";

    private static string Fail(string value) => throw new InvalidOperationException("the handler broke");
}

using System.Net;
using System.Text;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The claim the exclusion criteria make about analytics: switching it on changes whether anybody
/// catches the payload, never whether anything is sent.
/// </summary>
/// <remarks>
/// <para>
/// Both tests here are about the process rather than about a document, so they run alone: one sets
/// a process-wide variable that every <see cref="Blocks"/> reads when it is constructed, and the
/// other counts every outbound HTTP request the process announces — which a test running beside it
/// would add to.
/// </para>
/// <para>
/// The second one is the reason this file exists. "Nothing is sent" is not a property of the code
/// that can be read off the code: it is a property of a running application, so it is asserted by
/// starting the host, asking for a page, and making a handler raise while a wire tap is listening.
/// </para>
/// </remarks>
[Collection(AnalyticsEnvironmentDefinition.Name)]
public sealed class AnalyticsOutboundTests
{
    [Fact]
    public void The_environment_can_turn_the_switch_on_for_every_application()
    {
        using (EnvironmentVariable.Set("GRADIO_ANALYTICS_ENABLED", "True"))
        {
            using var provider = KernelHost.BuildProvider();
            using var blocks = provider.CreateBlocks();

            // The Python version's own value, and the one an operator migrating a deployment will already
            // have set — it means the same thing here as there.
            Assert.True(blocks.AnalyticsEnabled);
        }

        using (EnvironmentVariable.Set("GRADIO_ANALYTICS_ENABLED", "false"))
        {
            using var provider = KernelHost.BuildProvider();
            using var blocks = provider.CreateBlocks();

            Assert.False(blocks.AnalyticsEnabled);
        }

        // Unset is off, which is the one place this differs from the Python version.
        using var unset = KernelHost.BuildProvider();
        using var unsetBlocks = unset.CreateBlocks();

        Assert.False(unsetBlocks.AnalyticsEnabled);
    }

    [Fact]
    public async Task Nothing_leaves_the_process_while_the_application_runs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var sink = new RecordingAnalyticsSink();

        // Before the host exists, so that nothing it does can happen unseen.
        using var watch = new OutboundWatch();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                blocks.AnalyticsEnabled = true;

                Textbox input;
                Textbox output;

                using (blocks.BeginRender())
                {
                    input = blocks.Add(new Textbox { Label = "A" });
                    output = blocks.Add(new Textbox { Label = "B" });
                }

                input.Change(
                    (Func<string, string>)Fail,
                    [input],
                    [output],
                    apiName: "boom", queue: false);
            },
            services => services.AddSingleton<IAnalyticsSink>(sink));

        // A page, then an error: the two moments a migrating implementation would report from.
        await host.Client.GetStringAsync("/", cancellationToken);

        using var content = new StringContent("{\"data\":[\"a\"]}", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/run/boom", content, cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // The four events were built and handed over, so the switch was genuinely on and the payloads
        // genuinely existed: "nothing was sent" is not true because nothing was produced.
        Assert.Equal(
            [AnalyticsTopics.VersionCheck, AnalyticsTopics.Initiated, AnalyticsTopics.Launched, AnalyticsTopics.Error],
            sink.Events.Select(analyticsEvent => analyticsEvent.Topic));

        // The tap is connected: the page and the run this test asked for are in it, from this
        // process's own client. A watch that saw nothing would make the assertion below vacuous.
        Assert.True(
            watch.Count >= 2,
            $"The outbound watch saw {watch.Count} request(s) and the listeners "
            + $"the listeners [{string.Join(", ", watch.Listeners)}] and the events "
            + $"[{string.Join(", ", watch.Events)}], so it is not listening.");

        // And every one of them was to this machine. Anything else would be a request the framework
        // made on its own account, which is the thing that is not allowed to exist.
        Assert.Empty(watch.External);
    }

    private static string Fail(string value) => throw new InvalidOperationException("the handler broke");
}

/// <summary>
/// The collection the outbound and environment tests run in: alone, because both are about the
/// process rather than about one application.
/// </summary>
/// <remarks>
/// <see cref="CollectionDefinitionAttribute.DisableParallelization"/> is what makes the collection
/// run by itself rather than beside the others, which is the whole point: a host built by any other
/// test during the window would read the variable this collection set, and its requests would land
/// in the watch.
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AnalyticsEnvironmentDefinition
{
    /// <summary>The collection's name, as the tests declare it.</summary>
    public const string Name = "analytics-environment";
}

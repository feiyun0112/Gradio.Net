using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What the framework logs, seen the way a host's log pipeline sees it: a level, an event id, and
/// the named values behind the message.
/// </summary>
/// <remarks>
/// <para>
/// Nothing in this package writes to a console — it writes through <see cref="ILogger"/> — which is
/// what lets a deployment send these entries wherever it already sends its own. A test can only
/// show that by attaching a provider of its own and reading what arrives, which is what
/// <see cref="CapturingLoggerProvider"/> is.
/// </para>
/// <para>
/// Two entries are asserted, from two unrelated parts of the framework: the address of the
/// monitoring panel, and a run that raised. One entry would prove one call site was written that
/// way; two say it is how the framework logs rather than how one method happens to.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class StructuredLoggingTests
{
    [Fact]
    public async Task The_panel_address_arrives_as_a_field()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs,
            cancellationToken,
            configure: null,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "A" });
                }
            });

        var key = host.Services.GetRequiredService<Blocks>().AnalyticsKey;
        var expected = $"{host.BaseAddress.ToString().TrimEnd('/')}/monitoring/{key}";

        using var response = await host.Client.GetAsync("/monitoring", cancellationToken);

        // The route answers the sentence the Python version answers, so the address is only ever in the log —
        // a page that carried the key would hand the panel to whoever asked for it.
        Assert.Equal("See console for monitoring URL.", await response.Content.ReadAsStringAsync(cancellationToken));

        var entry = Assert.Single(logs.Entries, written => written.EventId.Name == "GradioMonitoringUrl");

        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal(expected, entry.Properties["Url"]);
        Assert.Contains(expected, entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_run_that_raised_arrives_as_a_field()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            cancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    var input = blocks.Add(new Textbox { Label = "A" });
                    var output = blocks.Add(new Textbox { Label = "Out" });

                    input.Change(
                        (Func<string, string>)(_ => throw new InvalidOperationException("no")),
                        [input],
                        [output],
                        apiName: "broken");
                }
            });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("broken", cancellationToken);

        await queue.JoinAsync(fnIndex, ["hello"], "logged", cancellationToken);

        await using var stream = await queue.OpenAsync("logged", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var entry = Assert.Single(logs.Entries, written => written.EventId.Name == "GradioQueueRunFailed");

        // The binding is a field rather than a word inside the sentence, which is what a host needs
        // to filter on: "which endpoint failed" is a question about a value, not about the prose.
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("broken", entry.Properties["ApiName"]);
        Assert.IsType<InvalidOperationException>(entry.Exception);
        Assert.Contains("broken", entry.Message, StringComparison.Ordinal);
    }
}

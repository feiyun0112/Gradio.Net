using System.Net;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The readiness probe an orchestrator uses, seen the way it uses it: over HTTP, against a host that
/// was asked for one.
/// </summary>
/// <remarks>
/// <para>
/// A health route is not in the Python version's route table, so it is opt-in and the default host
/// serves none. The tests that follow therefore say what the host was configured with before they
/// assert what it answers.
/// </para>
/// <para>
/// The two states a host can be listening in while unable to serve are both asserted: an application
/// that was never written, and a queue that is not running.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class HealthCheckTests
{
    [Fact]
    public async Task A_host_that_was_not_asked_for_a_probe_serves_none()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Textboxes(blocks));

        using var response = await host.Client.GetAsync("/health", cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task An_application_that_was_never_written_is_not_ready()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            HealthCheckPath = "/health",
        });

        await server.StartAsync(cancellationToken);

        using var client = new HttpClient { BaseAddress = server.BaseAddress };
        using var response = await client.GetAsync("/health", cancellationToken);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_written_application_on_a_running_queue_is_healthy()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            HealthCheckPath = "/health",
        });

        Textboxes(server.Blocks);

        await server.StartAsync(cancellationToken);

        using var client = new HttpClient { BaseAddress = server.BaseAddress };
        using var response = await client.GetAsync("/health", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_host_that_has_its_own_health_pipeline_can_register_the_check_alone()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            services => services.AddGradioHealthChecks(),
            blocks => Textboxes(blocks));

        var report = await ReportAsync(host, cancellationToken);
        var entry = Assert.Single(report.Entries);

        Assert.Equal(GradioHealthExtensions.CheckName, entry.Key);
        Assert.Equal(HealthStatus.Healthy, entry.Value.Status);

        // The numbers an operator would alert on travel with the answer, so that a probe and a
        // dashboard cannot disagree about how deep the queue is.
        Assert.True(entry.Value.Data.ContainsKey("queue_depth"));
        Assert.True(entry.Value.Data.ContainsKey("active_runs"));
    }

    [Fact]
    public async Task An_application_whose_queue_has_not_started_is_not_ready()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            configure: null,
            blocks => Textboxes(blocks));

        // A queue of the same application that nobody started: the state a host is in between
        // listening and starting its hosted services.
        var check = new GradioHealthCheck(
            host.Services.GetRequiredService<Blocks>(),
            new GradioQueueStatus());

        var result = await check.CheckHealthAsync(
            new HealthCheckContext { Registration = new HealthCheckRegistration("gradio", check, null, null) },
            cancellationToken);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("The queue is not running.", result.Description);
    }

    /// <summary>
    /// The probe and the alert rule are told the same depth, because both read the queue itself.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two things report the queue — the numbers on the probe's answer and the instruments an
    /// exporter scrapes — and they must not become two answers to one question.
    /// What a test can show is the part that would break first: both readers are asked at a moment
    /// when the queue is doing something, and they answer the same numbers.
    /// </para>
    /// <para>
    /// The moment matters, and the numbers are chosen so that an idle queue could not have produced
    /// them: one handler holds the binding's only slot and two runs wait behind it, so a scrape that
    /// reported a constant zero, or a probe that read a queue of its own, would answer something
    /// other than two waiting and one running.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_probe_and_the_instruments_report_the_same_queue()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            services => services.AddGradioHealthChecks(),
            blocks =>
            {
                var (input, output) = InputAndOutput(blocks);

                input.Change(
                    (Func<string, Task<string>>)probe.Hold,
                    [input],
                    [output],
                    apiName: "held", concurrencyLimit: ConcurrencyLimit.Of(1));
            });

        using var metrics = new CapturedMetrics();

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("held", cancellationToken);

        var held = await queue.JoinAsync(fnIndex, ["a"], "probed", cancellationToken);
        await using var stream = await queue.OpenAsync("probed", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_starts"
                && message["event_id"]?.GetValue<string>() == held,
            TimeSpan.FromSeconds(15),
            cancellationToken));

        await queue.JoinAsync(fnIndex, ["b"], "probed", cancellationToken);
        await queue.JoinAsync(fnIndex, ["c"], "probed", cancellationToken);

        // The queue is admitted before it is counted — a join is answered before the run reaches
        // the waiting list — so the busy reading is waited for rather than read once.
        Assert.True(
            await Eventually.IsTrueAsync(
                async () => (await ReportAsync(host, cancellationToken)).Entries.Single().Value.Data["queue_depth"] is 2,
                TimeSpan.FromSeconds(15),
                cancellationToken),
            "The probe never reported the runs that were waiting.");

        var busy = Assert.Single((await ReportAsync(host, cancellationToken)).Entries).Value;

        metrics.Sample();

        Assert.Equal(HealthStatus.Healthy, busy.Status);
        Assert.Equal(2, busy.Data["queue_depth"]);
        Assert.Equal(1, busy.Data["active_runs"]);

        // The other reader was asked at the same moment and was told the same two numbers. A
        // listener is process-wide, so the scrape also asks every other application in the process;
        // two runs waiting behind one is a state no idle queue reports.
        Assert.Contains(2L, metrics.SamplesOf(CapturedMetrics.QueueDepth));
        Assert.Contains(1L, metrics.SamplesOf(CapturedMetrics.ActiveRuns));

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        // And the probe follows the queue back down: a reading that stayed at one would be a copy
        // taken once rather than the queue's own state.
        Assert.True(
            await Eventually.IsTrueAsync(
                async () => (await ReportAsync(host, cancellationToken)).Entries.Single().Value.Data["queue_depth"] is 0,
                TimeSpan.FromSeconds(15),
                cancellationToken),
            "The probe kept reporting a run that was waiting.");
    }

    /// <summary>Asks the application's health check what it thinks, through the host's own service.</summary>
    /// <param name="host">The host.</param>
    /// <param name="cancellationToken">Cancels the ask.</param>
    /// <returns>What the host's health pipeline reported.</returns>
    private static Task<HealthReport> ReportAsync(GradioTestHost host, CancellationToken cancellationToken) =>
        host.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync(cancellationToken);

    /// <summary>Adds a field and its output, which is what the runs above read and write.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The pair.</returns>
    private static (Textbox Input, Textbox Output) InputAndOutput(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (input, output);
        }
    }

    /// <summary>Adds the pair of textboxes these tests render, which is what makes an application written.</summary>
    /// <param name="blocks">The application.</param>
    private static void Textboxes(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "A" });
        }
    }
}

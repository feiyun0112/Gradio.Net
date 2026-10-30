using System.Diagnostics;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The numbers an operator alerts on, read the way their exporter reads them: through the .NET
/// metrics API rather than from a page.
/// </summary>
/// <remarks>
/// <para>
/// The instrument names are written out in this suite (<see cref="CapturedMetrics"/>) rather than
/// taken from the framework's constants, because they are the contract: an alert rule is written
/// against the published name, and a test that read the constant would pass whatever the name
/// became.
/// </para>
/// <para>
/// The three instruments answer three different questions, and each test asks one of them: what the
/// queue is doing right now (a sample), how long a request took end to end (a histogram), and
/// whether the queue a probe sees is the queue an alert rule sees.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class MetricsTests
{
    /// <summary>How long the held handler keeps its slot, and so how long the other run waits.</summary>
    private static readonly TimeSpan Held = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task A_run_publishes_its_latency_and_the_queue_publishes_its_depth()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            configure: null,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    var input = blocks.Add(new Textbox { Label = "A" });
                    var output = blocks.Add(new Textbox { Label = "Out" });

                    input.Change(
                        (Func<string, string>)(value => value),
                        [input],
                        [output],
                        apiName: "metered");
                }
            });

        using var metrics = new CapturedMetrics();

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("metered", cancellationToken);

        await queue.JoinAsync(fnIndex, ["hello"], "metered", cancellationToken);

        await using var stream = await queue.OpenAsync("metered", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var measured = await Eventually.IsTrueAsync(
            () => Task.FromResult(metrics.DurationsOf("metered").Count > 0),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(measured, "The run published no latency.");

        // The gauges are read when an exporter asks rather than pushed, which is what a scrape does.
        metrics.Sample();

        Assert.Contains(CapturedMetrics.QueueDepth, metrics.Published);
        Assert.Contains(CapturedMetrics.ActiveRuns, metrics.Published);
        Assert.Contains(CapturedMetrics.RequestDuration, metrics.Published);

        var depth = metrics.SamplesOf(CapturedMetrics.QueueDepth);
        var active = metrics.SamplesOf(CapturedMetrics.ActiveRuns);

        Assert.NotEmpty(depth);
        Assert.NotEmpty(active);
        Assert.All(depth, answer => Assert.True(answer >= 0));
        Assert.All(active, answer => Assert.True(answer >= 0));

        // The two tags are what makes the histogram answerable: a latency without the binding it
        // belongs to says nothing about which endpoint is slow.
        var durations = metrics.DurationsOf("metered");

        Assert.All(durations, measurement => Assert.True(measurement.Value >= 0));
        Assert.Contains(durations, measurement => measurement.Tags["gradio.status"] as string == "success");
    }

    /// <summary>
    /// The gauges are the queue's own numbers, sampled when somebody asks.
    /// </summary>
    /// <remarks>
    /// The interesting reading is the one taken while the queue is not idle: a gauge that reported a
    /// constant zero would pass any assertion made after the run is over. One handler holds the
    /// binding's only slot and a second run waits for it, so a scrape at that moment has to say one
    /// waiting and one running — which is also the state an alert rule fires on.
    /// </remarks>
    [Fact]
    public async Task The_queue_reports_what_is_waiting_and_what_is_running_while_it_is_busy()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, Task<string>>)probe.Hold,
                [input],
                [output],
                apiName: "held", concurrencyLimit: ConcurrencyLimit.Of(1));
        });

        using var metrics = new CapturedMetrics();

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("held", cancellationToken);

        var first = await queue.JoinAsync(fnIndex, ["a"], "busy", cancellationToken);
        await using var stream = await queue.OpenAsync("busy", cancellationToken);

        // The first run is inside the handler, which does not return until the test lets it go.
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", first),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // The second run is admitted and cannot start: the binding's only slot is taken. A join is
        // answered before the run reaches the waiting list, so the reading is waited for rather
        // than taken once — which is also how an exporter would see it, one scrape at a time.
        await queue.JoinAsync(fnIndex, ["b"], "busy", cancellationToken);

        // One of the answers to a scrape is this queue's: a listener is process-wide, so the scrape
        // also asks every other application in the process, and only the reading taken while the
        // queue is busy is one an idle queue could not have produced.
        var busy = await Eventually.IsTrueAsync(
            () =>
            {
                metrics.Sample();

                return Task.FromResult(
                    metrics.SamplesOf(CapturedMetrics.QueueDepth).Contains(1L)
                    && metrics.SamplesOf(CapturedMetrics.ActiveRuns).Contains(1L));
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(busy, "The queue never reported one run waiting and one running.");

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
    }

    /// <summary>
    /// A request's latency is the whole wait: from the moment it was admitted to the moment its last
    /// message was described.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Three runs, one per way of getting the measurement wrong. The waiting run's handler answers
    /// immediately and its latency is still as long as the time it spent waiting for a slot, so the
    /// measurement does not start when the slot was found. The held run's handler is held for the
    /// same time, so a latency of zero for it would mean nothing was measured at all.
    /// </para>
    /// <para>
    /// The streamed run is the one that says where the measurement <em>ends</em>. Its first chunk is
    /// produced at once and its last one a second later, so a latency taken at the first answer
    /// would be a few milliseconds while a latency taken at the run's end is a second. Nothing else
    /// distinguishes the two: for a handler that answers once, returning and saying so are the same
    /// moment.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_latency_spans_the_wait_and_the_run_that_followed_it()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            // Two bindings of their own handlers, in one concurrency group: the held run takes the
            // group's only slot and the other run's handler — which answers at once — has to wait
            // for it. Two different handlers would be two groups, and nothing would wait.
            input.Change(
                (Func<string, Task<string>>)probe.Hold,
                [input],
                [output],
                apiName: "holder", concurrencyId: "shared", concurrencyLimit: ConcurrencyLimit.Of(1));

            input.Change(
                (Func<string, string>)(value => value),
                [input],
                [output],
                apiName: "waiter", concurrencyId: "shared", concurrencyLimit: ConcurrencyLimit.Of(1));

            // Its own group, so that it runs when the other two are over rather than queueing
            // behind them: what it measures is the run itself, not another wait.
            input.Change(
                (Func<string, IAsyncEnumerable<string>>)ChunkThenWait,
                [input],
                [output],
                apiName: "streamer");
        });

        using var metrics = new CapturedMetrics();

        using var queue = new QueueClient(host.BaseAddress);
        var holderIndex = await queue.EndpointAsync("holder", cancellationToken);
        var waiterIndex = await queue.EndpointAsync("waiter", cancellationToken);

        var held = await queue.JoinAsync(holderIndex, ["a"], "spanning", cancellationToken);
        await using var stream = await queue.OpenAsync("spanning", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", held),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        await queue.JoinAsync(waiterIndex, ["b"], "spanning", cancellationToken);

        // Long enough that the two latencies cannot be mistaken for the time the handlers took: the
        // waiting run's handler is instant, and the held run's handler is held for this long.
        var clock = Stopwatch.StartNew();
        await Task.Delay(Held, cancellationToken);

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var observed = await Eventually.IsTrueAsync(
            () => Task.FromResult(
                metrics.DurationsOf("holder").Count > 0 && metrics.DurationsOf("waiter").Count > 0),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(observed, "Both runs were not measured.");

        clock.Stop();

        var lowerBound = Held.TotalSeconds * 0.8;
        var upperBound = clock.Elapsed.TotalSeconds + 5;

        foreach (var measurement in metrics.DurationsOf("holder").Concat(metrics.DurationsOf("waiter")))
        {
            Assert.InRange(measurement.Value, lowerBound, upperBound);
        }

        // The streamed run: its first answer arrives at once and the run ends a second later.
        var streamerIndex = await queue.EndpointAsync("streamer", cancellationToken);
        var started = Stopwatch.StartNew();

        await queue.JoinAsync(streamerIndex, ["s"], "streaming", cancellationToken);

        await using var streamed = await queue.OpenAsync("streaming", cancellationToken);

        Assert.NotNull(await streamed.UntilAsync(
            message => QueueSession.Kind(message) == "process_generating",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var untilFirstChunk = started.Elapsed;

        Assert.True(await streamed.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var streamedMeasured = await Eventually.IsTrueAsync(
            () => Task.FromResult(metrics.DurationsOf("streamer").Count > 0),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(streamedMeasured, "The streamed run was not measured.");

        Assert.True(
            untilFirstChunk < Held / 2,
            $"The first chunk took {untilFirstChunk.TotalSeconds:0.###}s, so this run does not "
            + "distinguish a latency taken at the first answer from one taken at the end.");

        Assert.All(
            metrics.DurationsOf("streamer"),
            measurement => Assert.InRange(measurement.Value, lowerBound, started.Elapsed.TotalSeconds + 5));
    }

    /// <summary>A generator whose first chunk is immediate and whose last one is a second later.</summary>
    /// <param name="value">What the run was handed.</param>
    /// <returns>The chunks.</returns>
    private static async IAsyncEnumerable<string> ChunkThenWait(string value)
    {
        yield return value;

        await Task.Delay(Held);

        yield return value + "-last";
    }

    /// <summary>Adds the pair of textboxes these bindings read and write.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The pair.</returns>
    private static (Textbox Input, Textbox Output) Textboxes(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (input, output);
        }
    }
}

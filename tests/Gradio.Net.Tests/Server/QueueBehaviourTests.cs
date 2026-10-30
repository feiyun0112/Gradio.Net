using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What the queue does that a message sequence cannot show: how many runs are in flight at once,
/// what a cancel request does to one that is, and what happens to a session whose client left.
/// </summary>
/// <remarks>
/// <para>
/// Every test here drives the queue the way a client does — join, read the stream, cancel, ask the
/// queue what it is doing — so what is asserted is what a client can see. The handler is the test's
/// own code, and what it observes about itself (how many were inside it at once, whether its token
/// was signalled) is what a user's handler could observe too.
/// </para>
/// <para>
/// They share a collection because one of them changes how often the queue heartbeats, through a
/// process-wide environment variable, and the message-sequence comparison must not be reading a
/// stream while that is in force.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class QueueBehaviourTests
{
    [Fact]
    public async Task A_binding_runs_one_at_a_time_while_its_limit_is_one()
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
                apiName: "slow", concurrencyLimit: ConcurrencyLimit.Of(1));
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        var first = await queue.JoinAsync(fnIndex, ["a"], "serial", cancellationToken);
        var second = await queue.JoinAsync(fnIndex, ["b"], "serial", cancellationToken);

        await using var stream = await queue.OpenAsync("serial", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", first),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // The first run is holding the binding's only slot, so the second must not start.
        Assert.Null(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", second),
            TimeSpan.FromMilliseconds(500),
            cancellationToken));

        Assert.Equal(1, probe.Peak);

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
        Assert.Equal(2, stream.Messages.Count(message => QueueSession.Is(message, "process_completed")));
        Assert.Equal(1, probe.Peak);
    }

    [Fact]
    public async Task Two_bindings_that_run_the_same_handler_are_limited_together()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);
            var handler = (Func<string, Task<string>>)probe.Hold;

            // One function bound twice. The Python version groups by the identity of the function, so the two
            // bindings share one limit even though they are two endpoints — which is what a caller
            // protecting a shared resource is relying on.
            input.Change(handler, [input], [output], apiName: "first");
            input.Change(handler, [input], [output], apiName: "second");
        });

        using var queue = new QueueClient(host.BaseAddress);

        var firstIndex = await queue.EndpointAsync("first", cancellationToken);
        var secondIndex = await queue.EndpointAsync("second", cancellationToken);

        var first = await queue.JoinAsync(firstIndex, ["a"], "grouped", cancellationToken);
        var second = await queue.JoinAsync(secondIndex, ["b"], "grouped", cancellationToken);

        await using var stream = await queue.OpenAsync("grouped", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", first),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        Assert.Null(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", second),
            TimeSpan.FromMilliseconds(500),
            cancellationToken));

        Assert.Equal(1, probe.Peak);

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
        Assert.Equal(1, probe.Peak);
    }

    [Fact]
    public async Task Two_bindings_that_run_different_handlers_run_at_the_same_time()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            // Two different methods, so two groups and two limits. Without this half of the rule,
            // the test above would pass on a queue that ran everything one at a time.
            input.Change(
                (Func<string, Task<string>>)probe.Hold,
                [input],
                [output],
                apiName: "first");

            input.Change(
                (Func<string, Task<string>>)probe.HoldOther,
                [input],
                [output],
                apiName: "second");
        });

        using var queue = new QueueClient(host.BaseAddress);

        var firstIndex = await queue.EndpointAsync("first", cancellationToken);
        var secondIndex = await queue.EndpointAsync("second", cancellationToken);

        await queue.JoinAsync(firstIndex, ["a"], "apart", cancellationToken);
        await queue.JoinAsync(secondIndex, ["b"], "apart", cancellationToken);

        Assert.True(
            await probe.WaitForStartAsync(2, TimeSpan.FromSeconds(15), cancellationToken),
            "Both runs should have reached their handlers while neither had returned.");

        Assert.Equal(2, probe.Peak);

        probe.Release();

        await using var stream = await queue.OpenAsync("apart", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
    }

    [Fact]
    public async Task Cancelling_a_running_event_stops_it_and_ends_the_stream()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, CancellationToken, Task<string>>)probe.HoldUntilCancelled,
                [input],
                [output],
                apiName: "slow");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        var eventId = await queue.JoinAsync(fnIndex, ["a"], "cancelling", cancellationToken);
        await using var stream = await queue.OpenAsync("cancelling", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", eventId),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var answer = await queue.CancelAsync("cancelling", eventId, cancellationToken);
        Assert.True(answer["success"]!.GetValue<bool>());

        // The client is told the run is over, so that the connection it is holding can be released.
        var completed = await stream.UntilAsync(
            message => QueueSession.Is(message, "process_completed", eventId),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.True(completed["success"]!.GetValue<bool>());
        Assert.Empty(completed["output"]!.AsObject());

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        // One completion, not two: the run's own is not sent, because the Python version's cancelled run is a
        // task that no longer says anything.
        Assert.Equal(1, stream.Messages.Count(message => QueueSession.Is(message, "process_completed")));

        // And the handler was actually interrupted rather than left to finish.
        Assert.True(
            await probe.WaitForCancellationAsync(TimeSpan.FromSeconds(15), cancellationToken),
            "The handler should have observed its cancellation token being signalled.");
    }

    [Fact]
    public async Task A_run_reports_its_progress_before_it_completes()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            // Reports once and then holds, so that the progress message has to arrive while the
            // run is still going rather than racing its completion.
            input.Change(
                (Func<string, Progress, Task<string>>)(async (value, progress) =>
                {
                    progress.Report(0.25, "partway");

                    return await probe.Hold(value);
                }),
                [input],
                [output],
                apiName: "reporting");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("reporting", cancellationToken);

        var eventId = await queue.JoinAsync(fnIndex, ["a"], "reporting", cancellationToken);
        await using var stream = await queue.OpenAsync("reporting", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", eventId),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var progress = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "progress",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(progress);

        var units = progress["progress_data"]!.AsArray();
        Assert.Single(units);

        var unit = units[0]!.AsObject();
        Assert.Equal(0.25, unit["progress"]!.GetValue<double>(), 3);
        Assert.Equal("partway", unit["desc"]!.GetValue<string>());
        Assert.Equal("steps", unit["unit"]!.GetValue<string>());
        Assert.Null(unit["index"]);
        Assert.Null(unit["length"]);

        // It arrived while the run was still going: that is what makes the progress bar move
        // rather than appear once the work is already done.
        Assert.DoesNotContain(stream.Messages, message => QueueSession.Kind(message) == "process_completed");

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
    }

    [Fact]
    public async Task A_session_whose_heartbeat_connection_closes_is_dropped_from_the_queue()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        // The server finds out that the client left on the next beat, and this test waits for the
        // drop with the same patience the default interval would need to produce one. A fifth of a
        // second makes the wait a wait rather than a race with the clock.
        using var interval = HeartbeatIntervalVariable.Set("0.2");

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, Task<string>>)probe.Hold,
                [input],
                [output],
                apiName: "slow", concurrencyLimit: ConcurrencyLimit.Of(1));
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        // The holder takes the binding's only slot, so the next run waits.
        var holder = await queue.JoinAsync(fnIndex, ["a"], "holder", cancellationToken);
        await using var holderStream = await queue.OpenAsync("holder", cancellationToken);

        Assert.NotNull(await holderStream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", holder),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        await queue.JoinAsync(fnIndex, ["b"], "leaver", cancellationToken);
        await WaitForQueueSizeAsync(queue, expected: 1, cancellationToken);

        // The client that asked for the waiting run goes away. The heartbeat connection is how the
        // server finds out, and finding out is what drops the run it left behind.
        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);

        await WaitForQueueSizeAsync(queue, expected: 0, cancellationToken);

        probe.Release();

        Assert.True(await holderStream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
    }

    [Fact]
    public async Task A_stream_that_is_waiting_for_a_run_carries_heartbeats()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second, so that a test can see one without waiting for the
        // default fifteen. The Python version reads the same variable, which is why the interval is not a
        // parameter of this framework's own.
        using var interval = HeartbeatIntervalVariable.Set("0.2");

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, Task<string>>)probe.Hold,
                [input],
                [output],
                apiName: "slow");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        var eventId = await queue.JoinAsync(fnIndex, ["a"], "beating", cancellationToken);
        await using var stream = await queue.OpenAsync("beating", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", eventId),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var heartbeat = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "heartbeat",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(heartbeat);

        // A heartbeat belongs to the connection rather than to a run, which is what its absent
        // event id says.
        Assert.Null(heartbeat["event_id"]);

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
    }

    [Fact]
    public async Task A_streamed_value_that_grows_is_sent_as_the_text_that_was_added()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, IAsyncEnumerable<string>>)Grow,
                [input],
                [output],
                apiName: "growing");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("growing", cancellationToken);

        await queue.JoinAsync(fnIndex, ["x"], "growing", cancellationToken);
        await using var stream = await queue.OpenAsync("growing", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var chunks = stream.Messages
            .Where(message => QueueSession.Kind(message) == "process_generating")
            .Select(message => message["output"]!["data"]!.AsArray())
            .ToArray();

        Assert.Equal(3, chunks.Length);

        // The first chunk is the whole value, because the client has nothing to apply an edit to.
        Assert.Equal("x", chunks[0][0]!.GetValue<string>());

        // Each chunk after it is the text that was added, which is what makes a streamed answer
        // cheap to deliver when only its tail changed.
        AssertAppended(chunks[1], "b");
        AssertAppended(chunks[2], "c");

        // And the last message carries the whole value again, because there is no next edit to
        // expect by then.
        var completed = stream.Messages.Last(message => QueueSession.Kind(message) == "process_completed");

        Assert.Equal("xbc", completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>());
    }

    /// <summary>Asserts that a chunk is the edit "append this text to the value itself".</summary>
    private static void AssertAppended(JsonArray data, string text)
    {
        var edits = data[0]!.AsArray();
        var edit = Assert.Single(edits)!.AsArray();

        Assert.Equal("append", edit[0]!.GetValue<string>());
        Assert.Empty(edit[1]!.AsArray());
        Assert.Equal(text, edit[2]!.GetValue<string>());
    }

    /// <summary>A generator whose value grows, which is the shape a streamed answer takes.</summary>
    private static async IAsyncEnumerable<string> Grow(string value)
    {
        yield return value;

        await Task.Yield();

        yield return value + "b";

        await Task.Yield();

        yield return value + "bc";
    }

    [Fact]
    public async Task A_batched_run_is_handed_its_samples_together_and_each_is_told_its_own()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var batchSizes = new List<int>();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<object?[], Task<object?>>)(async values =>
                {
                    bool first;

                    lock (batchSizes)
                    {
                        batchSizes.Add(values.Length);
                        first = batchSizes.Count == 1;
                    }

                    // The first run holds the binding's only slot, so that the next two are both
                    // waiting when it finishes and are handed over as one batch. Without that the
                    // two could be dispatched separately, and the test would be of whichever the
                    // scheduler happened to see first.
                    if (first)
                    {
                        await release.Task;
                    }

                    return values.Select(value => value?.ToString()?.ToUpperInvariant()).ToArray();
                }),
                [input],
                [output],
                apiName: "batched", batch: true, maxBatchSize: 2, concurrencyLimit: ConcurrencyLimit.Of(1));
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("batched", cancellationToken);

        var holder = await queue.JoinAsync(fnIndex, ["a"], "batched", cancellationToken);
        await using var stream = await queue.OpenAsync("batched", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", holder),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var first = await queue.JoinAsync(fnIndex, ["b"], "batched", cancellationToken);
        var second = await queue.JoinAsync(fnIndex, ["c"], "batched", cancellationToken);

        await WaitForQueueSizeAsync(queue, expected: 2, cancellationToken);

        release.SetResult();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        lock (batchSizes)
        {
            Assert.Equal([1, 2], batchSizes);
        }

        // Each event of the batch is told the sample it brought, not the batch's last one.
        Assert.Equal("B", CompletedData(stream, first));
        Assert.Equal("C", CompletedData(stream, second));
    }

    /// <summary>The value a run's completion message carried.</summary>
    private static string? CompletedData(QueueSession session, string eventId)
    {
        var completed = session.Messages.Single(message => QueueSession.Is(message, "process_completed", eventId));

        return completed["output"]!["data"]!.AsArray()[0]?.GetValue<string>();
    }

    [Fact]
    public async Task A_run_is_told_how_long_it_is_expected_to_take_once_its_binding_has_run()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, string>)Copy,
                [input],
                [output],
                apiName: "copy");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("copy", cancellationToken);

        // The first run of a binding has no history, so there is nothing to estimate from.
        var first = await queue.JoinAsync(fnIndex, ["a"], "estimating", cancellationToken);

        await using (var stream = await queue.OpenAsync("estimating", cancellationToken))
        {
            var estimation = await stream.UntilAsync(
                message => QueueSession.Is(message, "estimation", first),
                TimeSpan.FromSeconds(15),
                cancellationToken);

            Assert.NotNull(estimation);
            Assert.Null(estimation["rank_eta"]);

            Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));
        }

        // The second run is estimated from the first, which is what makes the number a client draws
        // meaningful rather than always absent.
        var second = await queue.JoinAsync(fnIndex, ["b"], "estimating", cancellationToken);

        await using (var stream = await queue.OpenAsync("estimating", cancellationToken))
        {
            var estimation = await stream.UntilAsync(
                message => QueueSession.Is(message, "estimation", second),
                TimeSpan.FromSeconds(15),
                cancellationToken);

            Assert.NotNull(estimation);
            Assert.NotNull(estimation["rank_eta"]);

            var starts = await stream.UntilAsync(
                message => QueueSession.Is(message, "process_starts", second),
                TimeSpan.FromSeconds(15),
                cancellationToken);

            Assert.NotNull(starts);
            Assert.NotNull(starts["eta"]);
        }
    }

    [Fact]
    public async Task A_stream_for_a_session_nothing_joined_says_so_and_ends()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, string>)Copy,
                [input],
                [output],
                apiName: "copy");
        });

        using var queue = new QueueClient(host.BaseAddress);

        // A client that opens the stream without joining has nothing to be told, and the Python version
        // answers it rather than leaving the connection open forever.
        await using var stream = await queue.OpenAsync("never-joined", cancellationToken);

        var message = await stream.NextAsync(TimeSpan.FromSeconds(15), cancellationToken);

        Assert.NotNull(message);
        Assert.Equal("unexpected_error", QueueSession.Kind(message));
        Assert.Equal("404: Not Found", message["message"]!.GetValue<string>());
        Assert.True(message["session_not_found"]!.GetValue<bool>());
    }

    [Fact]
    public async Task More_runs_than_the_queue_has_workers_all_complete()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, string>)Copy,
                [input],
                [output],
                apiName: "copy");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("copy", cancellationToken);

        // More runs than the queue has worker slots, which is forty. A run that finished without
        // giving its slot back would leave the queue handing out nothing after the fortieth, and
        // this would never see the stream close.
        const int Runs = 45;

        for (var index = 0; index < Runs; index++)
        {
            await queue.JoinAsync(fnIndex, [$"{index}"], "many", cancellationToken);
        }

        // Opened after the runs were admitted: the session's channel buffers what they said, and
        // the stream reads it back from the beginning.
        await using var stream = await queue.OpenAsync("many", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(120), cancellationToken));
        Assert.Equal(Runs, stream.Messages.Count(message => QueueSession.Kind(message) == "process_completed"));
    }

    [Fact]
    public async Task An_application_that_closed_its_direct_routes_answers_404()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                (Func<string, string>)Copy,
                [input],
                [output],
                apiName: "copy");

            // The Python version's api_open=False: a queued binding keeps its endpoint and stops answering a
            // direct call, which is what a deployment behind its own API gateway asks for.
            blocks.Queue(apiOpen: false);
        });

        using var content = new StringContent("""{"data":["a"]}""", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/run/copy", content, cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(
            """{"detail":"This API endpoint does not accept direct HTTP POST requests. Please join the queue to use this API."}""",
            await response.Content.ReadAsStringAsync(cancellationToken));

        // The switch closes the direct route, not the binding: the same run still goes through the
        // queue, which is where the page sends it.
        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("copy", cancellationToken);

        Assert.Equal("a", await QueueRuns.RunAsync(queue, fnIndex, ["a"], "direct", cancellationToken));
    }

    [Fact]
    public async Task A_queue_that_is_full_refuses_the_next_run()
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
                apiName: "slow", concurrencyLimit: ConcurrencyLimit.Of(1));

            blocks.Queue(maxSize: 1);
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        // One run holds the only slot and one waits for it, which is the whole of the queue's room.
        await queue.JoinAsync(fnIndex, ["a"], "full", cancellationToken);
        await WaitForQueueSizeAsync(queue, expected: 0, cancellationToken);

        await queue.JoinAsync(fnIndex, ["b"], "full", cancellationToken);
        await WaitForQueueSizeAsync(queue, expected: 1, cancellationToken);

        using var content = new StringContent(
            new JsonObject { ["data"] = new JsonArray("c"), ["fn_index"] = fnIndex, ["session_hash"] = "full" }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync("/gradio_api/queue/join", content, cancellationToken);

        // The Python version answers 503 and says how full it is, so a client can tell "come back later" from
        // "that request was wrong".
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Contains(
            "Queue is full",
            await response.Content.ReadAsStringAsync(cancellationToken),
            StringComparison.Ordinal);

        probe.Release();
    }

    [Fact]
    public async Task A_fixed_status_update_rate_keeps_describing_the_queue()
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
                apiName: "slow", concurrencyLimit: ConcurrencyLimit.Of(1));

            // A number rather than "auto": the queue re-describes itself on that interval instead of
            // when something changes.
            blocks.Queue(statusUpdateRate: 0.2);
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("slow", cancellationToken);

        await queue.JoinAsync(fnIndex, ["a"], "updating", cancellationToken);
        var waiting = await queue.JoinAsync(fnIndex, ["b"], "updating", cancellationToken);

        await using var stream = await queue.OpenAsync("updating", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "estimation", waiting),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // Nothing about the queue changes while the first run holds its slot, so a description that
        // arrives again is the interval doing its work rather than an answer to an event. Polled
        // rather than slept through: what the test wants is the second description, not a second.
        var describedAgain = await Eventually.IsTrueAsync(
            () => Task.FromResult(stream.Messages.Count(message => QueueSession.Is(message, "estimation", waiting)) >= 2),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(
            describedAgain,
            $"The waiting run was described {stream.Messages.Count(message => QueueSession.Is(message, "estimation", waiting))} time(s); a fixed rate of 0.2s should have described it again.");

        probe.Release();
    }

    /// <summary>Adds the pair of textboxes these bindings read and write.</summary>
    private static (Textbox Input, Textbox Output) Textboxes(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (input, output);
        }
    }

    /// <summary>A handler that returns what it was given.</summary>
    private static string Copy(string value) => value;

    /// <summary>Waits until the queue holds a given number of waiting runs.</summary>
    private static async Task WaitForQueueSizeAsync(
        QueueClient queue,
        int expected,
        CancellationToken cancellationToken)
    {
        var holds = await Eventually.IsTrueAsync(
            async () => await QueueSizeAsync(queue, cancellationToken) == expected,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(
            holds,
            $"The queue held {await QueueSizeAsync(queue, cancellationToken)} waiting run(s), and the "
            + $"test waited for {expected}.");
    }

    /// <summary>How many runs are waiting for a slot.</summary>
    private static async Task<int> QueueSizeAsync(QueueClient queue, CancellationToken cancellationToken) =>
        (await queue.StatusAsync(cancellationToken))["queue_size"]!.GetValue<int>();
}

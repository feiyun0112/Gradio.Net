using System.Globalization;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using static Gradio.Net.Tests.Server.QueueRuns;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// State as a client experiences it: a value that survives the runs of one session, stays out of
/// another session's way, and is reclaimed when its lifetime runs out.
/// </summary>
/// <remarks>
/// Everything here goes through the queue endpoints, the way a browser does, because the point of
/// a state is what happens between two requests: a test that reached into the session would be
/// testing the storage rather than the behaviour a user gets.
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class StateAndSessionTests
{
    [Fact]
    public async Task A_state_keeps_its_value_across_the_runs_of_one_session()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        Assert.Equal("0", await RunAsync(queue, fnIndex, ["a", null], "one-session", cancellationToken));
        Assert.Equal("1", await RunAsync(queue, fnIndex, ["b", null], "one-session", cancellationToken));
        Assert.Equal("2", await RunAsync(queue, fnIndex, ["c", null], "one-session", cancellationToken));
    }

    [Fact]
    public async Task Two_sessions_count_on_their_own()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        Assert.Equal("0", await RunAsync(queue, fnIndex, ["a", null], "first", cancellationToken));
        Assert.Equal("1", await RunAsync(queue, fnIndex, ["b", null], "first", cancellationToken));

        // The second session has never run, so it starts from the initial value rather than from
        // where the first one got to.
        Assert.Equal("0", await RunAsync(queue, fnIndex, ["c", null], "second", cancellationToken));
        Assert.Equal("1", await RunAsync(queue, fnIndex, ["d", null], "second", cancellationToken));
    }

    [Fact]
    public async Task A_state_output_is_not_sent_to_the_client()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        var data = await CompletedDataAsync(queue, fnIndex, ["a", null], "quiet", cancellationToken);

        Assert.Equal("0", data[0]!.GetValue<string>());

        // The state's slot is null: the value belongs to the server, and a client that received it
        // could only send it back.
        Assert.Null(data[1]);
    }

    [Fact]
    public async Task A_state_input_is_read_from_the_session_rather_than_from_the_client()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        // The client sends a value for the state's position. It is ignored: the session's value is
        // what the handler reads, which is what makes a state unforgeable.
        var data = await CompletedDataAsync(queue, fnIndex, ["a", 99], "forging", cancellationToken);

        Assert.Equal("0", data[0]!.GetValue<string>());
    }

    [Fact]
    public async Task A_run_reports_the_state_it_changed_when_something_listens_for_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var stateId = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);
            stateId = state.Id;

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");

            // The frontend dispatches a state's change event from the ids a run reports, so a state
            // nobody listens to is not worth reporting.
            state.Change(
                (Func<object?, object?>)(value => value),
                [state],
                [output],
                apiName: "watched");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        var completed = await CompletedAsync(queue, fnIndex, ["a", null], "watched-state", cancellationToken);
        var changed = completed["output"]!["changed_state_ids"]!.AsArray();

        Assert.Contains(stateId, changed.Select(id => id!.GetValue<int>()));
    }

    [Fact]
    public async Task A_run_reports_no_state_when_nothing_listens_for_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        var completed = await CompletedAsync(queue, fnIndex, ["a", null], "unwatched-state", cancellationToken);

        Assert.Empty(completed["output"]!["changed_state_ids"]!.AsArray());
    }

    [Fact]
    public async Task A_state_whose_lifetime_elapses_is_reclaimed_and_its_callback_runs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var deleted = new List<object?>();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            // A fifth of a second, so that the sweep reclaims it while the test is watching.
            state.TimeToLive = 0.2;
            state.DeleteCallback = new Action<object?>(value =>
            {
                lock (deleted)
                {
                    deleted.Add(value);
                }
            });

            input.Change(
                (Func<string, object?, (string, object?)>)Count,
                [input, state],
                [output, state],
                apiName: "count");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("count", cancellationToken);

        Assert.Equal("0", await RunAsync(queue, fnIndex, ["a", null], "expiring", cancellationToken));
        Assert.Equal("1", await RunAsync(queue, fnIndex, ["b", null], "expiring", cancellationToken));

        // The sweep runs once a second, so the value written by the second run is reclaimed shortly
        // after its lifetime elapses, and its callback is told what was dropped.
        var reclaimed = await Eventually.IsTrueAsync(
            () =>
            {
                lock (deleted)
                {
                    return Task.FromResult(deleted.Count > 0);
                }
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(reclaimed, "The expired state should have been handed to its delete callback.");

        lock (deleted)
        {
            // The value the second run stored, which is the one that aged out.
            Assert.Equal(2d, Assert.IsType<double>(Assert.Single(deleted)));
        }

        // And the next run starts from the initial value again, because the session no longer holds
        // one.
        Assert.Equal("0", await RunAsync(queue, fnIndex, ["c", null], "expiring", cancellationToken));
    }

    [Fact]
    public async Task Two_runs_of_one_session_that_overlap_each_read_the_state_they_started_with()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var probe = new QueueProbe();
        object? fastSaw = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state, output) = Counter(blocks);

            // Two different handlers, so the two runs are in different concurrency groups and really
            // do overlap: a binding runs one at a time by default, and two runs of one binding would
            // make this a queue test instead.
            // One output, the state: a run answers with one value per output, so a binding that
            // wrote two of them would have to answer with a tuple, and the held run answers with
            // the value it wants the session to hold.
            input.Submit(
                (Func<string, object?, Task<object?>>)((text, count) => probe.HoldAndRecord(count, "slow")),
                [input, state],
                [state],
                apiName: "slow");

            input.Submit(
                (Func<string, object?, (string, object?)>)((text, count) =>
                {
                    fastSaw = count;

                    return (text, "fast");
                }),
                [input, state],
                [output, state],
                apiName: "fast");

            // Reports whatever the session holds, as text: the two runs above write names rather
            // than numbers, so this one reads the state back without interpreting it.
            input.Change(
                (Func<string, object?, (string, object?)>)((text, count) => (count?.ToString() ?? "nothing", count)),
                [input, state],
                [output, state],
                apiName: "read");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var slow = await queue.EndpointAsync("slow", cancellationToken);
        var fast = await queue.EndpointAsync("fast", cancellationToken);
        var read = await queue.EndpointAsync("read", cancellationToken);

        // One stream for the session, which is what a browser opens: the two runs are followed on
        // it by their event ids rather than by opening a second one.
        var slowId = await queue.JoinAsync(slow, ["a", null], "overlapping", cancellationToken);

        await using var stream = await queue.OpenAsync("overlapping", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", slowId),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // The slow run is inside its handler and holding the value it read. The fast one now runs
        // to completion in the same session.
        var fastId = await queue.JoinAsync(fast, ["b", null], "overlapping", cancellationToken);

        var fastCompleted = await stream.UntilAsync(
            message => QueueSession.Is(message, "process_completed", fastId),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(fastCompleted);
        Assert.True(fastCompleted["success"]!.GetValue<bool>(), fastCompleted.ToJsonString());

        // Then the slow one is let go, and its completion is waited for: a handler that failed would
        // otherwise pass as one that did not run.
        probe.Release();

        var slowCompleted = await stream.UntilAsync(
            message => QueueSession.Is(message, "process_completed", slowId),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(slowCompleted);
        Assert.True(slowCompleted["success"]!.GetValue<bool>(), slowCompleted.ToJsonString());

        // Neither run saw the other's write: the slow one read before the fast one wrote, and the
        // fast one read before the slow one did. A state that leaked between them would show "slow"
        // or "fast" here rather than the value the session started with.
        Assert.Equal("0", Text(probe.Handed));
        Assert.Equal("0", Text(fastSaw));

        // The slow write was the last one, so that is what the session holds — one run's whole
        // value rather than a mixture of the two.
        Assert.Equal("slow", await RunAsync(queue, read, ["c", null], "overlapping", cancellationToken));
    }

    /// <summary>A value the runs above record, as the text two of them can be compared on.</summary>
    /// <param name="value">The value a run was handed.</param>
    /// <returns>The number as text, in the invariant culture the counter uses.</returns>
    private static string Text(object? value) =>
        Convert.ToDouble(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

    /// <summary>An application whose handler counts the runs of a session.</summary>
    private static (Textbox Input, State State, Textbox Output) Counter(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var state = blocks.Add(new State { Value = JsonValue.Create(0) });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (input, state, output);
        }
    }

    /// <summary>Reports the count it read, and stores the next one.</summary>
    private static (string, object?) Count(string text, object? count)
    {
        var current = Convert.ToDouble(count, CultureInfo.InvariantCulture);

        return (current.ToString(CultureInfo.InvariantCulture), current + 1);
    }
}

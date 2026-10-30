using System.Globalization;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using static Gradio.Net.Tests.Server.QueueRuns;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What an application does when a visitor leaves: the handler it bound with <c>unload</c>, run once
/// when the page's connection ends.
/// </summary>
/// <remarks>
/// <para>
/// A page cannot report its own end — there is no request left to make — so the connection it holds
/// open is the report. These tests drop that connection the way a browser does and assert what the
/// server ran, and with what, at that moment.
/// </para>
/// <para>
/// They share the queue's collection because they drive the heartbeat route, whose interval the
/// collection shortens; nothing here waits for a heartbeat frame, but the route is the same one.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class UnloadTests
{
    [Fact]
    public async Task The_unload_handler_runs_once_when_the_page_goes_away()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second: the route is how the server learns the page has gone,
        // and it learns it by writing to a connection that is no longer there. At the default rate
        // the notice would take a quarter of a minute, which is the interval rather than the
        // behaviour.
        using var interval = HeartbeatIntervalVariable.Set("0.2");
        var seen = new List<object?>();
        var ran = new TaskCompletionSource();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, state) = Counter(blocks);

            input.Change(
                (Func<string, object?, object?>)((_, count) => Bump(count)),
                [input, state],
                [state],
                apiName: "bump");

            blocks.Unload(
                (Action<object?>)(value =>
                {
                    lock (seen)
                    {
                        seen.Add(value);
                    }

                    ran.TrySetResult();
                }),
                [state]);
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("bump", cancellationToken);

        // One run, so that the session holds a value the unload handler has something to read.
        await CompletedAsync(queue, fnIndex, ["a", null], "leaver", cancellationToken);

        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);

        await ran.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);

        // The handler read the session's own value, not an empty one: the page sent nothing, and the
        // session is the only place the value could come from.
        var value = Assert.Single(seen);
        Assert.Equal(1d, Convert.ToDouble(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public async Task A_second_close_of_the_same_session_runs_nothing_again()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second: the route is how the server learns the page has gone,
        // and it learns it by writing to a connection that is no longer there. At the default rate
        // the notice would take a quarter of a minute, which is the interval rather than the
        // behaviour.
        using var interval = HeartbeatIntervalVariable.Set("0.2");
        var runs = 0;
        var ran = new TaskCompletionSource();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox());
            }

            blocks.Unload((Action)(() =>
            {
                Interlocked.Increment(ref runs);
                ran.TrySetResult();
            }));
        });

        using var queue = new QueueClient(host.BaseAddress);

        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);
        await ran.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);

        // A page's two connections can end one after the other — the stream carrying its runs, and
        // the heartbeat — and each reports the end it saw. The work happens once, because the second
        // report is the same end.
        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_page_whose_visitor_never_ran_anything_still_runs_what_it_bound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second: the route is how the server learns the page has gone,
        // and it learns it by writing to a connection that is no longer there. At the default rate
        // the notice would take a quarter of a minute, which is the interval rather than the
        // behaviour.
        using var interval = HeartbeatIntervalVariable.Set("0.2");
        var ran = new TaskCompletionSource();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox());
            }

            // A binding with no handler is one the server cannot run, and it does not stand in the
            // way of the one it can.
            blocks.Unload(null);
            blocks.Unload((Action)(() => ran.TrySetResult()));
        });

        using var queue = new QueueClient(host.BaseAddress);

        // Nothing was run and no state was read, so the session exists only because the application
        // has work to do when it ends. The handler runs anyway: leaving is the event.
        await queue.AbandonHeartbeatAsync("idle-visitor", cancellationToken);

        await ran.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
    }

    [Fact]
    public async Task A_streaming_unload_handler_is_drained_rather_than_abandoned()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second: the route is how the server learns the page has gone,
        // and it learns it by writing to a connection that is no longer there. At the default rate
        // the notice would take a quarter of a minute, which is the interval rather than the
        // behaviour.
        using var interval = HeartbeatIntervalVariable.Set("0.2");
        var drained = new TaskCompletionSource();
        var seen = new List<string?>();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            Textbox field;

            using (blocks.BeginRender())
            {
                field = blocks.Add(new Textbox { Label = "A" });
            }

            blocks.Unload(
                (Func<string?, IAsyncEnumerable<string>>)(value =>
                {
                    seen.Add(value);

                    return Chunks(drained);
                }),
                [field]);
        });

        using var queue = new QueueClient(host.BaseAddress);

        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);

        // The handler's own work finished: nobody read its chunks, and a run that stopped at the
        // first one would leave it suspended halfway through.
        await drained.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);

        // The page sent nothing, and this field was never shown to the client, so the handler reads
        // nothing rather than a value invented for it.
        Assert.Null(Assert.Single(seen));
    }

    [Fact]
    public async Task One_handler_that_fails_does_not_stop_the_one_bound_after_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // A heartbeat every fifth of a second: the route is how the server learns the page has gone,
        // and it learns it by writing to a connection that is no longer there. At the default rate
        // the notice would take a quarter of a minute, which is the interval rather than the
        // behaviour.
        using var interval = HeartbeatIntervalVariable.Set("0.2");
        var attempts = new List<string>();
        var ran = new TaskCompletionSource();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox());
            }

            blocks.Unload((Action)(() =>
            {
                lock (attempts)
                {
                    attempts.Add("first");
                }

                throw new InvalidOperationException("no");
            }));

            blocks.Unload((Action)(() =>
            {
                lock (attempts)
                {
                    attempts.Add("second");
                }

                ran.TrySetResult();
            }));
        });

        using var queue = new QueueClient(host.BaseAddress);

        await queue.AbandonHeartbeatAsync("leaver", cancellationToken);

        // They were bound by one application for one moment, and the moment is over either way.
        await ran.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
        Assert.Equal(["first", "second"], attempts);
    }

    /// <summary>A page with one field and one value the session keeps.</summary>
    /// <param name="blocks">The application to render into.</param>
    private static (Textbox Input, State State) Counter(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var state = blocks.Add(new State { Value = JsonValue.Create(0) });

            return (input, state);
        }
    }

    /// <summary>Reports the next count, so that the session's value moves off its initial one.</summary>
    private static object? Bump(object? count) =>
        Convert.ToDouble(count, CultureInfo.InvariantCulture) + 1;

    /// <summary>A handler that produces more than one chunk, and says so when it ran out.</summary>
    /// <param name="drained">Set once the iterator has been asked for a chunk past its last.</param>
    /// <remarks>
    /// The line after the last <c>yield</c> runs only when the run asked for one chunk more than
    /// there were, which is exactly what draining is — a run that took the first chunk and stopped
    /// would never reach it.
    /// </remarks>
    private static async IAsyncEnumerable<string> Chunks(TaskCompletionSource drained)
    {
        yield return "one";

        await Task.Yield();

        yield return "two";

        drained.TrySetResult();
    }
}

using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The work an application runs once, when it starts being served, and the flag that says it has
/// been run.
/// </summary>
/// <remarks>
/// The flag is not bookkeeping: it is the whole answer the startup-events route gives after the
/// first call, so "ran once" has to be true under concurrency, under failure, and after events are
/// registered late. Each of those is asserted here rather than over HTTP, because they are
/// properties of the application rather than of the route.
/// </remarks>
public sealed class StartupEventsTests
{
    [Fact]
    public async Task The_first_call_runs_the_events_and_says_it_did_and_the_next_one_says_it_did_not()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        var runs = 0;

        blocks.AddStartupEvent(_ =>
        {
            runs++;

            return Task.CompletedTask;
        });

        Assert.False(blocks.StartupEventsTriggered);
        Assert.True(await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));
        Assert.False(await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));

        Assert.True(blocks.StartupEventsTriggered);
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task Events_run_in_the_order_they_were_registered()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        var order = new List<int>();

        blocks.AddStartupEvent(_ =>
        {
            order.Add(1);

            return Task.CompletedTask;
        });

        blocks.AddStartupEvent(_ =>
        {
            order.Add(2);

            return Task.CompletedTask;
        });

        await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken);

        Assert.Equal([1, 2], order);
    }

    [Fact]
    public async Task An_event_that_fails_leaves_the_application_unstarted_so_the_next_call_tries_again()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        var attempts = 0;

        blocks.AddStartupEvent(_ =>
        {
            attempts++;

            // The first attempt fails the way a warm-up that cannot reach its dependency does.
            return attempts == 1
                ? Task.FromException(new InvalidOperationException("no"))
                : Task.CompletedTask;
        });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));

        // The Python version sets its flag after the await, so a failure skips it; the application is then
        // still unstarted rather than half-started, and the next caller is the one that runs them.
        Assert.False(blocks.StartupEventsTriggered);

        Assert.True(await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));
        Assert.True(blocks.StartupEventsTriggered);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task An_event_registered_while_the_events_are_running_waits_for_the_next_start()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        var ran = new List<string>();

        blocks.AddStartupEvent(_ =>
        {
            ran.Add("first");

            // Registering one from inside one is allowed; running it in the same pass is not, or
            // "the events the application started with" would depend on how fast they were.
            blocks.AddStartupEvent(_ =>
            {
                ran.Add("late");

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        });

        await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["first"], ran);
    }

    [Fact]
    public async Task Two_callers_at_once_run_the_events_once()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();
        var runs = 0;
        var started = new TaskCompletionSource();
        var release = new TaskCompletionSource();

        blocks.AddStartupEvent(async _ =>
        {
            Interlocked.Increment(ref runs);
            started.TrySetResult();

            // Held open so that the second caller arrives while the first is still running, which
            // is the only way to reach the flag being read before it is set.
            await release.Task;
        });

        var first = blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken);
        await started.Task;
        var second = blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken);

        release.SetResult();

        Assert.True(await first);
        Assert.False(await second);
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task An_application_with_no_startup_events_is_started_by_its_first_call()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        // The answer the route gives is about the work having happened, not about there having
        // been work: an application with nothing to prepare still starts once.
        Assert.True(await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));
        Assert.False(await blocks.RunStartupEventsAsync(TestContext.Current.CancellationToken));
    }
}

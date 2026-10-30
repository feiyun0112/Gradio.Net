using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What a cached binding does when a client calls it twice, seen the way a client sees it: the second
/// answer says it came from the cache, and the handler says it only ran once.
/// </summary>
/// <remarks>
/// The assertion about the handler is the one that matters. A message field is a claim about what the
/// run did; a counter inside the handler is what the run actually did, and a cache that reported a
/// hit without skipping the work would pass the first and fail the second.
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class CachingTests
{
    [Fact]
    public async Task A_cached_binding_answers_the_second_call_from_the_cache()
    {
        var runs = 0;
        var cache = new Cache();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                cache.Wrap<Func<string, string>>(value =>
                {
                    runs++;

                    return value.ToUpperInvariant();
                }),
                [input],
                [output],
                apiName: "upper");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("upper", cancellationToken);

        var first = await CompleteAsync(queue, fnIndex, "hello", "first", cancellationToken);
        var second = await CompleteAsync(queue, fnIndex, "hello", "second", cancellationToken);

        Assert.Equal("HELLO", first["output"]!["data"]![0]!.GetValue<string>());
        Assert.Null(first["output"]!["used_cache"]);
        Assert.Equal("HELLO", second["output"]!["data"]![0]!.GetValue<string>());

        // The second answer says where it came from, which is what the frontend shows and what a
        // client that asked the queue is told instead of an estimate.
        Assert.Equal("full", second["output"]!["used_cache"]!.GetValue<string>());
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_cached_binding_that_has_not_seen_the_call_computes_it()
    {
        var runs = 0;
        var cache = new Cache();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                cache.Wrap<Func<string, string>>(value =>
                {
                    runs++;

                    return value.ToUpperInvariant();
                }),
                [input],
                [output],
                apiName: "upper");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("upper", cancellationToken);

        await CompleteAsync(queue, fnIndex, "hello", "first", cancellationToken);
        var other = await CompleteAsync(queue, fnIndex, "world", "second", cancellationToken);

        // A different argument is a different entry: the probe missed and the run happened.
        Assert.Equal("WORLD", other["output"]!["data"]![0]!.GetValue<string>());
        Assert.Null(other["output"]!["used_cache"]);
        Assert.Equal(2, runs);
    }

    [Fact]
    public async Task A_cached_stream_is_replayed_rather_than_produced_again()
    {
        var runs = 0;
        var cache = new Cache();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                cache.Wrap<Func<string, IAsyncEnumerable<string>>>(Chunks),
                [input],
                [output],
                apiName: "chunks");

            async IAsyncEnumerable<string> Chunks(string value)
            {
                runs++;

                yield return $"{value}-0";

                await Task.Yield();

                yield return $"{value}-1";
            }
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("chunks", cancellationToken);

        var first = await StreamAsync(queue, fnIndex, "a", "first", cancellationToken);
        var second = await StreamAsync(queue, fnIndex, "a", "second", cancellationToken);

        // Both calls describe the stream the same way — two chunks sent as they are produced, then
        // the completion carrying the last one — because a replay is the sequence, not a summary of
        // it.
        Assert.Equal(2, first.Generating);
        Assert.Equal("a-1", first.Completed);
        Assert.Null(first.UsedCache);

        Assert.Equal(first.Generating, second.Generating);
        Assert.Equal("a-1", second.Completed);
        Assert.Equal("full", second.UsedCache);

        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_per_session_cache_does_not_answer_another_session()
    {
        var runs = 0;
        var cache = new Cache(new CacheOptions { PerSession = true });
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                cache.Wrap<Func<string, string>>(value =>
                {
                    runs++;

                    return value;
                }),
                [input],
                [output],
                apiName: "echo");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);

        var first = await CompleteAsync(queue, fnIndex, "hello", "one", cancellationToken);
        var second = await CompleteAsync(queue, fnIndex, "hello", "two", cancellationToken);
        var third = await CompleteAsync(queue, fnIndex, "hello", "one", cancellationToken);

        // The same call in another session is a different answer, because the answer may be about
        // that session; the first session's own answer is still there.
        Assert.Null(first["output"]!["used_cache"]);
        Assert.Null(second["output"]!["used_cache"]);
        Assert.Equal("full", third["output"]!["used_cache"]!.GetValue<string>());
        Assert.Equal(2, runs);
    }

    [Fact]
    public async Task A_per_session_cache_forgets_a_session_that_went_away()
    {
        var cache = new Cache(new CacheOptions { PerSession = true });
        var cancellationToken = TestContext.Current.CancellationToken;

        // The server learns the browser left when the heartbeat stream it is holding fails, which it
        // notices on the next beat — fifteen seconds by default, which is also this test's patience.
        // A fifth of a second makes the wait a wait rather than a race with the clock, and the same
        // variable is what the lifecycle tests set for the same reason.
        using var interval = HeartbeatIntervalVariable.Set("0.2");

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (input, output) = Textboxes(blocks);

            input.Change(
                cache.Wrap<Func<string, string>>(value => value),
                [input],
                [output],
                apiName: "echo");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);

        await CompleteAsync(queue, fnIndex, "hello", "gone", cancellationToken);
        Assert.Equal(1, cache.Count);

        // The heartbeat is how the server learns the browser left; the caches it was keeping for that
        // session go with it, which is why a page with a per-session cache asks for one.
        await queue.AbandonHeartbeatAsync("gone", cancellationToken);

        var forgotten = await Eventually.IsTrueAsync(
            () => Task.FromResult(cache.Count == 0),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(forgotten, $"The cache still held {cache.Count} entry of a session that left.");
    }

    /// <summary>Joins one run and reads its session's stream to the completion.</summary>
    /// <param name="queue">The client.</param>
    /// <param name="fnIndex">The binding.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="session">The session to run in.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>The completion message.</returns>
    private static async Task<JsonObject> CompleteAsync(
        QueueClient queue,
        int fnIndex,
        string value,
        string session,
        CancellationToken cancellationToken)
    {
        await queue.JoinAsync(fnIndex, [value], session, cancellationToken);

        await using var stream = await queue.OpenAsync(session, cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        return completed!;
    }

    /// <summary>Runs a streaming binding and describes the sequence it produced.</summary>
    /// <param name="queue">The client.</param>
    /// <param name="fnIndex">The binding.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="session">The session to run in.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>How many chunks arrived before the last one, the last one, and where it came from.</returns>
    private static async Task<(int Generating, string Completed, string? UsedCache)> StreamAsync(
        QueueClient queue,
        int fnIndex,
        string value,
        string session,
        CancellationToken cancellationToken)
    {
        await queue.JoinAsync(fnIndex, [value], session, cancellationToken);

        await using var stream = await queue.OpenAsync(session, cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var completion = stream.Messages.Single(message => QueueSession.Kind(message) == "process_completed");

        return (
            stream.Messages.Count(message => QueueSession.Kind(message) == "process_generating"),
            completion["output"]!["data"]![0]!.GetValue<string>(),
            completion["output"]!["used_cache"]?.GetValue<string>());
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
}

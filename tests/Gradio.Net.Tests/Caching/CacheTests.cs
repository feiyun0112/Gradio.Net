using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Caching;

/// <summary>
/// What a cached function does on a hit and on a miss, and what the cache behind it forgets.
/// </summary>
/// <remarks>
/// The counts are the assertion that matters: "the answer was cached" is not a claim about a
/// dictionary, it is the claim that the work did not happen twice.
/// </remarks>
public sealed class CacheTests
{
    [Fact]
    public void A_second_call_with_the_same_arguments_does_not_run_the_function()
    {
        var cache = new Cache();
        var runs = 0;
        var count = cache.Wrap<Func<string, int>>(text =>
        {
            runs++;

            return text.Length;
        });

        Assert.Equal(5, count("hello"));
        Assert.Equal(5, count("hello"));
        Assert.Equal(1, runs);
    }

    [Fact]
    public void A_different_argument_is_a_different_entry()
    {
        var cache = new Cache();
        var runs = 0;
        var count = cache.Wrap<Func<string, int>>(text =>
        {
            runs++;

            return text.Length;
        });

        count("hello");
        count("world");

        Assert.Equal(2, runs);
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void A_key_function_decides_what_the_call_is_judged_by()
    {
        var cache = new Cache();
        var runs = 0;
        var describe = cache.Wrap<Func<string, int, string>>(
            (text, verbose) =>
            {
                runs++;

                return verbose > 0 ? $"{text} ({text.Length})" : text;
            },
            key: arguments => arguments[0]);

        // Only the prompt matters: the second call asks for a different rendering of the same
        // prompt and gets the answer that was cached for it.
        Assert.Equal("hello", describe("hello", 0));
        Assert.Equal("hello", describe("hello", 1));
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_task_is_cached_as_what_it_resolved_to()
    {
        var cache = new Cache();
        var runs = 0;
        var load = cache.Wrap<Func<string, Task<string>>>(async text =>
        {
            runs++;
            await Task.Yield();

            return text.ToUpperInvariant();
        });

        Assert.Equal("HELLO", await load("hello"));
        Assert.Equal("HELLO", await load("hello"));
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_value_task_is_cached_as_what_it_resolved_to()
    {
        var cache = new Cache();
        var runs = 0;
        var load = cache.Wrap<Func<string, ValueTask<string>>>(text =>
        {
            runs++;

            return new ValueTask<string>(text.ToUpperInvariant());
        });

        Assert.Equal("HELLO", await load("hello"));
        Assert.Equal("HELLO", await load("hello"));
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task A_stream_is_replayed_from_the_cache_rather_than_produced_again()
    {
        var cache = new Cache();
        var runs = 0;
        var stream = cache.Wrap<Func<string, IAsyncEnumerable<string>>>(Chunks);

        Assert.Equal(["a", "b", "c"], await ReadAsync(stream("x")));
        Assert.Equal(["a", "b", "c"], await ReadAsync(stream("x")));
        Assert.Equal(1, runs);

        async IAsyncEnumerable<string> Chunks(string _)
        {
            runs++;

            yield return "a";
            await Task.Yield();
            yield return "b";
            yield return "c";
        }
    }

    [Fact]
    public async Task A_stream_that_was_abandoned_part_way_is_not_cached()
    {
        var cache = new Cache();
        var runs = 0;
        var stream = cache.Wrap<Func<int, IAsyncEnumerable<int>>>(Chunks);

        await foreach (var _ in stream(3))
        {
            break;
        }

        Assert.Equal(0, cache.Count);

        // The next caller asks again, because half an answer is not the answer.
        Assert.Equal([0, 1, 2], await ReadAsync(stream(3)));
        Assert.Equal(2, runs);

        async IAsyncEnumerable<int> Chunks(int count)
        {
            runs++;

            for (var index = 0; index < count; index++)
            {
                yield return index;
            }
        }
    }

    [Fact]
    public void The_arguments_the_framework_supplies_are_not_part_of_the_key()
    {
        var cache = new Cache();
        var runs = 0;
        var handle = cache.Wrap<Func<string, Progress, CancellationToken, string>>((text, progress, cancellation) =>
        {
            runs++;

            return text;
        });

        handle("hello", new Progress(), CancellationToken.None);
        handle("hello", new Progress(), new CancellationTokenSource().Token);

        // A progress reporter and a cancellation token differ every run and are not inputs; a key
        // that carried them would never be looked up twice.
        Assert.Equal(1, runs);
    }

    [Fact]
    public void A_probe_raises_on_a_miss_instead_of_computing()
    {
        var cache = new Cache();
        var runs = 0;
        var handle = cache.Wrap<Func<string, string>>(text =>
        {
            runs++;

            return text;
        });
        using (cache.Probe())
        {
            Assert.Throws<CacheMissError>(() => handle("hello"));
        }

        Assert.Equal(0, runs);

        // The probe left nothing behind: the ordinary call computes and keeps the answer, and the
        // next probe answers from it.
        Assert.Equal("hello", handle("hello"));
        Assert.Equal(1, runs);

        using (cache.Probe())
        {
            Assert.Equal("hello", handle("hello"));
        }
    }

    [Fact]
    public void A_per_session_cache_keeps_sessions_apart_and_forgets_one_of_them()
    {
        var cache = new Cache(new CacheOptions { PerSession = true });
        var runs = 0;
        var handle = cache.Wrap<Func<string, string>>(text =>
        {
            runs++;

            return text;
        });
        using (cache.OpenSession("session-a"))
        {
            handle("hello");
        }

        using (cache.OpenSession("session-b"))
        {
            // The same arguments in another session are not the same answer, because the answer
            // may be about that session.
            handle("hello");
        }

        Assert.Equal(2, runs);

        cache.ClearSession("session-a");

        using (cache.OpenSession("session-a"))
        {
            handle("hello");
        }

        using (cache.OpenSession("session-b"))
        {
            handle("hello");
        }

        Assert.Equal(3, runs);
    }

    [Fact]
    public void A_shared_cache_ignores_a_session_being_cleared()
    {
        var cache = new Cache();
        var runs = 0;
        var handle = cache.Wrap<Func<string, string>>(text =>
        {
            runs++;

            return text;
        });
        using (cache.OpenSession("session-a"))
        {
            handle("hello");
        }

        cache.ClearSession("session-a");

        // Nothing belonged to that session: clearing one visitor's entries would otherwise throw
        // away everyone's answers.
        using (cache.OpenSession("session-a"))
        {
            handle("hello");
        }

        Assert.Equal(1, runs);
    }

    [Fact]
    public void The_least_recently_used_entry_is_the_one_forgotten()
    {
        var cache = new Cache(new CacheOptions { MaxSize = 2 });
        var runs = new List<string>();
        var handle = cache.Wrap<Func<string, string>>(text =>
        {
            runs.Add(text);

            return text;
        });

        handle("a");
        handle("b");
        handle("a");
        handle("c");

        Assert.Equal(["a", "b", "c"], runs);

        // "a" was used again after "b", so it was "b" that had to go.
        handle("a");
        Assert.Equal(["a", "b", "c"], runs);
        handle("b");
        Assert.Equal(["a", "b", "c", "b"], runs);
    }

    [Fact]
    public void An_entry_larger_than_the_memory_budget_is_still_kept()
    {
        var cache = new Cache(new CacheOptions { MaxMemory = 8 });
        var runs = 0;
        var handle = cache.Wrap<Func<string, string>>(text =>
        {
            runs++;

            return new string('x', 1000);
        });

        handle("a");

        // One answer larger than the whole budget is still the answer; forgetting it would mean the
        // cache could never hold what the application asked it to.
        Assert.Equal(1, cache.Count);
        handle("a");
        Assert.Equal(1, runs);
    }

    [Fact]
    public void A_mapping_is_stored_and_read_back_by_the_handle()
    {
        var cache = new Cache();

        cache.Set("greeting", ("text", "hello"), ("count", 2));

        var entry = cache.Get("greeting");

        Assert.NotNull(entry);
        Assert.Equal("hello", entry["text"]);
        Assert.Equal(2, entry["count"]);
        Assert.Null(cache.Get("absent"));

        // The key is a value, not an identity: the same text read back from JSON is the same key.
        Assert.Equal("hello", cache.Get(System.Text.Json.Nodes.JsonValue.Create("greeting"))!["text"]);
    }

    [Fact]
    public void The_handle_lists_and_forgets_what_it_holds()
    {
        var cache = new Cache();

        cache.Set("a", ("value", 1));
        cache.Set("b", ("value", 2));

        Assert.Equal(["a", "b"], cache.Keys());
        Assert.Equal(2, cache.Count);

        cache.Clear();

        Assert.Empty(cache.Keys());
        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void A_function_that_returns_nothing_is_refused()
    {
        var cache = new Cache();

        var failure = Assert.Throws<ArgumentException>(() => cache.Wrap<Action<string>>(_ => { }));

        Assert.Contains("has to return something", failure.Message, StringComparison.Ordinal);
    }

    private static async Task<List<T>> ReadAsync<T>(IAsyncEnumerable<T> stream)
    {
        var values = new List<T>();

        await foreach (var value in stream)
        {
            values.Add(value);
        }

        return values;
    }
}

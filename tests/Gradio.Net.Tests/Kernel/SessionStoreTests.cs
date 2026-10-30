using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The session store bounds itself by the two knobs the Python version exposes: the capacity of
/// <c>state_session_capacity</c> and a time to live. Time is moved by hand so that the
/// assertions are exact rather than timing-dependent.
/// </summary>
public sealed class SessionStoreTests
{
    private static readonly DateTimeOffset _start = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void A_session_is_created_once_and_reused()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions(), clock);

        var created = store.GetOrAdd("session-1");
        clock.Advance(TimeSpan.FromMinutes(1));
        var reused = store.GetOrAdd("session-1");

        Assert.Same(created, reused);
        Assert.Equal(1, store.Count);
        Assert.Equal(_start, created.CreatedAt);
        Assert.Equal(_start.AddMinutes(1), reused.LastAccessedAt);
    }

    [Fact]
    public void An_unknown_session_is_a_miss_rather_than_a_new_session()
    {
        var store = new MemorySessionStore(new SessionStoreOptions(), new ManualTimeProvider(_start));

        Assert.False(store.TryGet("session-1", out var session));
        Assert.Null(session);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void Capacity_evicts_the_least_recently_used_session()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { Capacity = 2 }, clock);

        store.GetOrAdd("first");
        store.GetOrAdd("second");
        store.GetOrAdd("first");   // first becomes the most recently used one
        store.GetOrAdd("third");   // so second is the one that goes

        Assert.Equal(2, store.Count);
        Assert.True(store.TryGet("first", out _));
        Assert.False(store.TryGet("second", out _));
        Assert.True(store.TryGet("third", out _));
    }

    [Fact]
    public void An_idle_session_expires()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { TimeToLive = TimeSpan.FromMinutes(5) }, clock);

        store.GetOrAdd("session-1");

        clock.Advance(TimeSpan.FromMinutes(4));
        Assert.True(store.TryGet("session-1", out _));

        clock.Advance(TimeSpan.FromMinutes(5));
        Assert.False(store.TryGet("session-1", out _));
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void Reading_a_session_refreshes_its_time_to_live()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { TimeToLive = TimeSpan.FromMinutes(5) }, clock);

        store.GetOrAdd("session-1");

        // Twenty minutes of wall clock, but never five idle minutes.
        for (var read = 0; read < 5; read++)
        {
            clock.Advance(TimeSpan.FromMinutes(4));
            Assert.True(store.TryGet("session-1", out _));
        }

        Assert.Equal(1, store.Count);
    }

    [Fact]
    public void A_session_never_expires_when_no_time_to_live_is_configured()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions(), clock);

        store.GetOrAdd("session-1");
        clock.Advance(TimeSpan.FromDays(365));

        Assert.True(store.TryGet("session-1", out _));
    }

    [Fact]
    public void RemoveExpired_reclaims_only_the_sessions_that_have_expired()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { TimeToLive = TimeSpan.FromMinutes(5) }, clock);

        store.GetOrAdd("stale");
        clock.Advance(TimeSpan.FromMinutes(10));
        store.GetOrAdd("fresh");

        Assert.Equal(2, store.Count);
        Assert.Equal(1, store.RemoveExpired());
        Assert.Equal(1, store.Count);
        Assert.False(store.TryGet("stale", out _));
        Assert.True(store.TryGet("fresh", out _));
    }

    [Fact]
    public void Capacity_eviction_is_reported_as_a_capacity_eviction()
    {
        var store = new MemorySessionStore(new SessionStoreOptions { Capacity = 1 }, new ManualTimeProvider(_start));
        var evictions = new List<(string Hash, SessionEvictionReason Reason)>();
        store.Evicted += eviction => evictions.Add((eviction.Session.Hash, eviction.Reason));

        store.GetOrAdd("kept");
        store.GetOrAdd("dropped");

        Assert.Equal([("kept", SessionEvictionReason.Capacity)], evictions);
    }

    [Fact]
    public void An_expired_session_is_reported_as_expired_when_it_is_reached()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { TimeToLive = TimeSpan.FromMinutes(5) }, clock);
        var evictions = new List<(string Hash, SessionEvictionReason Reason)>();
        store.Evicted += eviction => evictions.Add((eviction.Session.Hash, eviction.Reason));

        store.GetOrAdd("stale");
        clock.Advance(TimeSpan.FromMinutes(5));

        Assert.False(store.TryGet("stale", out _));
        Assert.Equal([("stale", SessionEvictionReason.Expired)], evictions);
    }

    [Fact]
    public void Explicit_removal_is_not_reported_as_an_eviction()
    {
        var store = new MemorySessionStore(new SessionStoreOptions(), new ManualTimeProvider(_start));
        var evictions = new List<string>();
        store.Evicted += eviction => evictions.Add(eviction.Session.Hash);

        store.GetOrAdd("session-1");
        Assert.True(store.Remove("session-1"));
        Assert.False(store.Remove("session-1"));
        store.GetOrAdd("session-2");
        store.Clear();

        Assert.Equal(0, store.Count);
        Assert.Empty(evictions);
    }

    [Fact]
    public void The_defaults_match_upstream()
    {
        var options = new SessionStoreOptions();

        Assert.Equal(SessionStoreOptions.DefaultCapacity, options.Capacity);
        Assert.Null(options.TimeToLive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void A_capacity_below_one_is_rejected(int capacity)
    {
        var options = new SessionStoreOptions { Capacity = capacity };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MemorySessionStore(options, new ManualTimeProvider(_start)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void A_non_positive_time_to_live_is_rejected(int seconds)
    {
        var options = new SessionStoreOptions { TimeToLive = TimeSpan.FromSeconds(seconds) };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MemorySessionStore(options, new ManualTimeProvider(_start)));
    }

    [Fact]
    public void A_session_that_expires_is_reclaimed_with_its_state()
    {
        var clock = new ManualTimeProvider(_start);
        var store = new MemorySessionStore(new SessionStoreOptions { TimeToLive = TimeSpan.FromMinutes(5) }, clock);

        var deleted = new List<string?>();
        var session = store.GetOrAdd("stale");
        session.State.Set(1, JsonValue.Create("gone"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        clock.Advance(TimeSpan.FromMinutes(5));

        Assert.Equal(1, store.RemoveExpired());

        // The store is what decides a session is over, so it is also what ends the values that
        // session was holding: a callback that releases a resource would otherwise never run.
        Assert.Equal(["gone"], deleted);
        Assert.Equal(0, session.State.Count);
    }

    [Fact]
    public void An_explicitly_removed_session_takes_its_state_with_it()
    {
        var store = new MemorySessionStore(new SessionStoreOptions(), new ManualTimeProvider(_start));

        var deleted = new List<string?>();
        var session = store.GetOrAdd("session-1");
        session.State.Set(1, JsonValue.Create("gone"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        Assert.True(store.Remove("session-1"));

        Assert.Equal(["gone"], deleted);
    }

    [Fact]
    public void A_session_evicted_for_capacity_takes_its_state_with_it()
    {
        var store = new MemorySessionStore(new SessionStoreOptions { Capacity = 1 }, new ManualTimeProvider(_start));

        var deleted = new List<string?>();
        var evicted = store.GetOrAdd("evicted");
        evicted.State.Set(1, JsonValue.Create("gone"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        store.GetOrAdd("kept");

        Assert.Equal(["gone"], deleted);
    }

    [Fact]
    public void Clearing_the_store_releases_every_sessions_state()
    {
        var store = new MemorySessionStore(new SessionStoreOptions(), new ManualTimeProvider(_start));

        var deleted = new List<string?>();
        var first = store.GetOrAdd("first");
        var second = store.GetOrAdd("second");
        first.State.Set(1, JsonValue.Create("one"), null, value => deleted.Add(value?.GetValue<string>()), _start);
        second.State.Set(1, JsonValue.Create("two"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        store.Clear();

        Assert.Equal(["one", "two"], deleted);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void The_snapshot_is_a_copy_rather_than_the_store_itself()
    {
        var store = new MemorySessionStore(new SessionStoreOptions(), new ManualTimeProvider(_start));
        store.GetOrAdd("first");

        var snapshot = store.Snapshot();
        store.GetOrAdd("second");

        // A sweep walks the snapshot while requests add and drop sessions; what it sees must not
        // change under it.
        Assert.Single(snapshot);
        Assert.Equal(2, store.Count);
    }

    [Fact]
    public async Task Concurrent_access_keeps_the_store_consistent()
    {
        const int Workers = 8;
        const int Iterations = 500;
        const int DistinctHashes = 64;
        const int Capacity = 32;

        var store = new MemorySessionStore(
            new SessionStoreOptions { Capacity = Capacity },
            new ManualTimeProvider(_start));

        var workers = new Task[Workers];
        for (var worker = 0; worker < Workers; worker++)
        {
            workers[worker] = Task.Run(
                () =>
                {
                    for (var index = 0; index < Iterations; index++)
                    {
                        var hash = $"session-{index % DistinctHashes}";
                        store.GetOrAdd(hash);
                        store.TryGet(hash, out _);
                    }
                },
                TestContext.Current.CancellationToken);
        }

        await Task.WhenAll(workers);

        // Every hash was inserted, so the store is exactly at capacity, and what it reports it
        // retains is what it can actually find: the index and the recency list agree.
        Assert.Equal(Capacity, store.Count);
        Assert.Equal(
            Capacity,
            Enumerable.Range(0, DistinctHashes).Count(hash => store.TryGet($"session-{hash}", out _)));
    }
}

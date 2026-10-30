using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The kernel is configured through the container, and what is configured there is what the
/// resolved services actually do.
/// </summary>
public sealed class KernelRegistrationTests
{
    private static readonly DateTimeOffset _start = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void The_kernel_registers_the_render_context_accessor_and_the_session_store()
    {
        using var provider = KernelHost.BuildProvider();

        Assert.NotNull(provider.GetRequiredService<IRenderContextAccessor>());
        Assert.NotNull(provider.GetRequiredService<ISessionStore>());
        Assert.NotNull(provider.GetRequiredService<IOptions<GradioCoreOptions>>().Value);
    }

    [Fact]
    public void The_render_context_accessor_is_a_singleton()
    {
        using var provider = KernelHost.BuildProvider();

        // One instance for every scope: a reader in one scope has to find what a writer in another
        // one made ambient, which is only true if they share the AsyncLocal.
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();

        Assert.Same(
            first.ServiceProvider.GetRequiredService<IRenderContextAccessor>(),
            second.ServiceProvider.GetRequiredService<IRenderContextAccessor>());
    }

    [Fact]
    public void The_session_store_is_a_singleton()
    {
        using var provider = KernelHost.BuildProvider();

        using var scope = provider.CreateScope();

        Assert.Same(
            provider.GetRequiredService<ISessionStore>(),
            scope.ServiceProvider.GetRequiredService<ISessionStore>());
    }

    [Fact]
    public void Registering_the_kernel_twice_does_not_register_it_twice()
    {
        var services = new ServiceCollection();
        services.AddGradioCore();
        services.AddGradioCore();

        Assert.Single(services, descriptor => descriptor.ServiceType == typeof(ISessionStore));
        Assert.Single(services, descriptor => descriptor.ServiceType == typeof(IRenderContextAccessor));
    }

    [Fact]
    public void A_configured_capacity_is_the_capacity_that_is_enforced()
    {
        using var provider = KernelHost.BuildProvider(options => options.SessionStore.Capacity = 2);
        var store = provider.CreateSessionStore();

        store.GetOrAdd("first");
        store.GetOrAdd("second");
        store.GetOrAdd("third");

        Assert.Equal(2, store.Count);
        Assert.False(store.TryGet("first", out _));
    }

    [Fact]
    public void A_configured_time_to_live_is_the_time_to_live_that_is_enforced()
    {
        var clock = new ManualTimeProvider(_start);

        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(clock);
        services.AddGradioCore(options => options.SessionStore.TimeToLive = TimeSpan.FromMinutes(5));

        using var provider = services.BuildServiceProvider();
        var store = provider.CreateSessionStore();

        store.GetOrAdd("session-1");
        clock.Advance(TimeSpan.FromMinutes(4));
        Assert.True(store.TryGet("session-1", out _));

        clock.Advance(TimeSpan.FromMinutes(5));
        Assert.False(store.TryGet("session-1", out _));
    }

    [Fact]
    public void A_consumer_can_put_sessions_somewhere_else()
    {
        var substituted = new SubstitutedSessionStore();

        var services = new ServiceCollection();
        services.AddSingleton<ISessionStore>(substituted);
        services.AddGradioCore();

        using var provider = services.BuildServiceProvider();
        var store = provider.CreateSessionStore();

        Assert.Same(substituted, store);
        Assert.Equal("session-1", store.GetOrAdd("session-1").Hash);
        Assert.Equal(["session-1"], substituted.Requested);
    }
}

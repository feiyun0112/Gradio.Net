using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gradio.Net;
/// <summary>
/// Registers Gradio with a container, for a host that already has one.
/// </summary>
/// <remarks>
/// <para>
/// This is the entry point for adding a Gradio UI to an existing ASP.NET Core application. It
/// registers the kernel plus what hosting the route table needs; the other entry
/// point, <see cref="GradioServer"/>, registers the same things on a host it owns itself, so
/// neither can drift from the other.
/// </para>
/// </remarks>
public static partial class GradioServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Gradio kernel, the application, and the route table.
    /// </summary>
    /// <param name="services">The container to add to.</param>
    /// <param name="configure">
    /// Optional kernel configuration, applied before anything is resolved from it.
    /// </param>
    /// <returns><paramref name="services"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The application (<see cref="Blocks"/>) is owned by the container rather than by the caller:
    /// it has to be built from the container's render context accessor, and an accessor from a
    /// different container would silently fail to see the context. One container hosts
    /// one application.
    /// </remarks>
    public static IServiceCollection AddGradio(
        this IServiceCollection services,
        Action<GradioCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddGradioCore(configure);

        // The application the container serves: one the host handed over, or one built here. The
        // slot is what lets `app.UseGradio(blocks)` serve a page that was written before the host
        // existed, without a second place that knows how to build an application.
        services.TryAddSingleton<GradioApplicationSlot>();
        services.TryAddSingleton(static provider => provider
            .GetRequiredService<GradioApplicationSlot>()
            .Resolve(() => new Blocks(
                provider.GetRequiredService<IRenderContextAccessor>(),
                provider.GetRequiredService<IBlockContainerFactory>())));

        // The containers that group form components are layouts, and the kernel asks for them by
        // name so that components need not reference the layouts. This is where the two meet.
        services.TryAddSingleton<IBlockContainerFactory, BlockContainerFactory>();

        services.TryAddSingleton<GradioRouteTable>();
        services.TryAddSingleton<GradioNotImplementedEndpoint>();

        // The vendored frontend, indexed once per container: the page and every asset it loads are
        // read from this assembly rather than from files beside it.
        services.TryAddSingleton<FrontendAssets>();

        // The queue is one per application, and the host owns its lifetime: it is started with the
        // host rather than on the first request, so that "the queue is running" is a property of
        // the application rather than of traffic.
        services.TryAddSingleton<GradioQueue>();
        services.AddHostedService<GradioQueueHostedService>();

        // The application's own startup work runs with the host, after the queue is up: the Python version
        // runs both in one function, and the order matters because a startup event may enqueue
        // something. Registered as a hosted service rather than called from an entry point so that
        // both entry points run it, which is what keeps their answers to /startup-events the same.
        services.AddHostedService<GradioStartupHostedService>();

        // How often a stream tells its client it is alive, resolved once for the whole server: the
        // queue's streams and the reload stream are the same promise, and a deployment that
        // shortens it for an idle-connection timeout means both.
        services.TryAddSingleton<HeartbeatInterval>();

        // The change a development page reloads itself on. Registered rather than created by the
        // stream, because the thing that announces a change — a host's own file watcher — is not
        // the thing that serves it (see GradioReloadSignal).
        services.TryAddSingleton<GradioReloadSignal>();

        // State and temporary files are reclaimed on schedules rather than on requests: a value
        // that expired while nobody was asking for anything is exactly what the sweep exists for,
        // and a file left behind is not noticed by any request at all.
        services.TryAddSingleton<ExpiredStateSweeper>();
        services.AddHostedService<GradioSessionStateHostedService>();
        services.AddHostedService<GradioTempFilesHostedService>();

        // An upload reports to this and a second request reads from it, so it is one instance per
        // application rather than one per request.
        services.TryAddSingleton<UploadProgress>();

        // The streams a run is producing and a later request plays: the run files segments into
        // this and the playlist route reads them out, so it belongs to the application the same way
        // the queue does.
        services.TryAddSingleton<PendingStreams>();

        // Run history: the store an application gets without asking, and the recorder the run paths
        // hand finished runs to. Both are singletons because both outlive a request — the store
        // because it is a folder, and the recorder because it bounds how many writes are in flight.
        services.TryAddSingleton<RunHistoryOptions>();
        services.TryAddSingleton<IRunHistoryStore>(provider =>
            new FileRunHistoryStore(provider.GetRequiredService<RunHistoryOptions>().RootFolder));
        services.TryAddSingleton<RunHistoryRecorder>();

        // Monitoring, profiling and the numbers a probe or an exporter reads. All singletons for the
        // reason the queue is: they describe an application, not a request. The collector is
        // registered whether or not profiling is on — the queue asks the options before it writes
        // anything — so that an application can turn profiling on through the same container it
        // configures everything else in.
        services.TryAddSingleton<EventAnalytics>();
        services.TryAddSingleton<GradioQueueStatus>();
        services.TryAddSingleton<GradioMetrics>();
        services.TryAddSingleton<ProfilingOptions>();
        services.TryAddSingleton<TraceCollector>();

        // The MCP endpoint: the exchange a message goes through, the runs it starts, and the open
        // streams of the transport that keeps one. Singletons for the reason the queue is — a run
        // outlives the request that asked for it, and a stream is the request.
        services.TryAddSingleton<McpSessions>();
        services.TryAddSingleton<McpToolRunner>();
        services.TryAddSingleton<McpProtocol>();
        services.TryAddSingleton<McpEndpoint>();

        // The Vibe editor's two pieces of state: which file it edits and what it asks, and the
        // conversation and snapshots that make an undo possible. No language model is registered
        // here on purpose — an application that turns the editor on says which backend it uses, and
        // one that says nothing is told so rather than being handed a vendor.
        services.TryAddSingleton<VibeOptions>();
        services.TryAddSingleton<VibeHistory>();

        // Who is calling: the tokens a username and password produce, the set of routes that answer
        // only to someone signed in, and — for an application that offers a LoginButton — the
        // session its identity provider's cookie carries.
        services.TryAddSingleton<GradioAuthentication>();
        services.TryAddSingleton<GradioLoginGate>();
        services.TryAddSingleton<GradioOAuthSession>(static provider =>
        {
            var options = provider.GetService<OAuthProviderOptions>();

            return new GradioOAuthSession(
                provider.GetService<IGradioOAuthProvider>(),
                options?.ClientSecret,
                provider.GetRequiredService<TimeProvider>());
        });

        // An identity provider configured by the environment is the one a Space has; a host with its
        // own registers its own, and this registration steps aside because it is a "try add".
        if (OAuthProviderOptions.FromEnvironment() is { } oauth)
        {
            services.TryAddSingleton(oauth);
            services.TryAddSingleton<IGradioOAuthProvider>(static provider => new OidcOAuthProvider(
                provider.GetRequiredService<OAuthProviderOptions>(),
                time: provider.GetRequiredService<TimeProvider>()));
        }

        // Capabilities answer the table entries they have built. Registering them as a collection
        // rather than as named services keeps the mapping loop free of a list it would have to be
        // taught about every time a capability lands.
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioAuthEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioOAuthEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioDescriptionEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioQueueEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioRunEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioFrontendEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioFileEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioDeepLinkEndpoint>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioComponentServerEndpoint>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioCustomComponentEndpoint>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioStreamEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioRecordingEndpoint>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioRunHistoryEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioMonitoringEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioProfilingEndpoints>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioVibeEndpoints>());

        // The two routes that describe the application's own lifecycle rather than one of its
        // capabilities: starting it, and telling a development page that it changed.
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioStartupEndpoint>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGradioEndpoint, GradioReloadEndpoint>());

        // Two the Python version routes put a catch-all behind a literal prefix, which ASP.NET Core can only
        // express as a route constraint (see RouteTemplate). The constraint is registered here
        // rather than left to the host because a host built on the slim builder - which is what the
        // standalone entry point uses - does not register it, and the route table has to map the
        // same way in both entries.
        services.Configure<RouteOptions>(static options =>
            options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

        return services;
    }
}

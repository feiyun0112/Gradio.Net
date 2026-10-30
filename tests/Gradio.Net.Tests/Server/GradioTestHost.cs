using Gradio.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Starts one of the two hosting entry points the way a user would, so that the tests exercise
/// the wiring the package ships rather than a host assembled for testing.
/// </summary>
/// <remarks>
/// Both entry points take the same optional log sink, so that a test can observe what either one
/// logs. The standalone entry point owns its logging by default; the option is how a caller
/// replaces it.
/// </remarks>
internal sealed class GradioTestHost : IAsyncDisposable
{
    private readonly GradioServer? _standalone;
    private readonly WebApplication? _integrated;

    private GradioTestHost(GradioServer? standalone, WebApplication? integrated, Uri baseAddress)
    {
        _standalone = standalone;
        _integrated = integrated;
        BaseAddress = baseAddress;

        // No proxy, whatever the machine's environment says: the host is on loopback, and a client
        // that sends the request through a proxy sends it in the absolute form ("GET http://… "),
        // which the SSR runtime refuses - it would make these tests depend on the developer's
        // HTTP_PROXY rather than on the server.
        Client = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = baseAddress };
    }

    internal Uri BaseAddress { get; }

    internal HttpClient Client { get; }

    internal IServiceProvider Services => _standalone?.Services ?? _integrated!.Services;

    /// <param name="logs">Where the host's log goes, or <see langword="null"/> for nowhere.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <param name="build">
    /// Writes the application into the container's own <see cref="Blocks"/>, before the host
    /// listens: a request must not be able to arrive while the page is still being built.
    /// </param>
    /// <param name="configure">
    /// Services to register before Gradio's own, which is how a test replaces one of them: the
    /// registration Gradio makes is a "try add", so a service that is already there is kept.
    /// </param>
    /// <param name="options">
    /// Adjusts what the entry point was configured with — SSR, for the tests that need a second
    /// server in front of this one. Everything else stays as a caller would leave it.
    /// </param>
    internal static async Task<GradioTestHost> StartStandaloneAsync(
        ILoggerProvider? logs,
        CancellationToken cancellationToken,
        Action<Blocks>? build = null,
        Action<IServiceCollection>? configure = null,
        Action<GradioServerOptions>? options = null)
    {
        var configured = new GradioServerOptions
        {
            ServerPort = 0,
            ConfigureServices = configure,
            ConfigureLogging = logging =>
            {
                logging.ClearProviders();

                if (logs is not null)
                {
                    logging.AddProvider(logs);
                }
            },
        };

        options?.Invoke(configured);

        var server = new GradioServer(configured);

        // The application is written before the host listens, the way a user writes one: a
        // request must not be able to arrive while the page is still being built.
        build?.Invoke(server.Blocks);

        await server.StartAsync(cancellationToken);

        return new GradioTestHost(server, integrated: null, server.BaseAddress);
    }

    /// <param name="logs">Where the host's log goes, or <see langword="null"/> for nowhere.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <param name="configure">
    /// Services to register before Gradio's own, which is how a test replaces one of them: the
    /// registration Gradio makes is a "try add", so a service that is already there is kept.
    /// </param>
    /// <param name="build">
    /// Writes the application into the container's own <see cref="Blocks"/>, before the host
    /// listens: a request must not be able to arrive while the page is still being built.
    /// </param>
    /// <param name="pipeline">
    /// Middleware to add before Gradio's own routes, which is how a test stands in for a host that
    /// authenticates its requests before the framework sees them.
    /// </param>
    internal static async Task<GradioTestHost> StartIntegratedAsync(
        ILoggerProvider? logs,
        CancellationToken cancellationToken,
        Action<IServiceCollection>? configure = null,
        Action<Blocks>? build = null,
        Action<WebApplication>? pipeline = null)
    {
        var builder = WebApplication.CreateSlimBuilder();
        configure?.Invoke(builder.Services);
        builder.Services.AddGradio();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();

        if (logs is not null)
        {
            builder.Logging.AddProvider(logs);
        }

        var app = builder.Build();

        pipeline?.Invoke(app);
        app.UseGradio();

        if (build is not null)
        {
            build(app.Services.GetRequiredService<Blocks>());
        }

        await app.StartAsync(cancellationToken);

        return new GradioTestHost(standalone: null, app, new Uri(app.Urls.First(), UriKind.Absolute));
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();

        if (_standalone is not null)
        {
            await _standalone.DisposeAsync();
        }

        if (_integrated is not null)
        {
            await _integrated.DisposeAsync();
        }
    }
}

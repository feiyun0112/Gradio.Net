using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The standalone entry point: a host that serves one Gradio application.
/// </summary>
/// <remarks>
/// <para>
/// This exists so that a Gradio app can be run in a few lines without an ASP.NET Core application
/// around it. It is not a second implementation: it registers the same services as
/// <see cref="GradioServiceCollectionExtensions.AddGradio"/> and maps the same route table through
/// <see cref="GradioEndpointRouteBuilderExtensions.UseGradio(WebApplication)"/>, so the two entry points cannot
/// answer the same request differently.
/// </para>
/// <para>
/// The container this server builds owns the application, so <see cref="Blocks"/> is resolved from
/// it rather than passed in — an application built around another container's render context
/// accessor would silently render into nothing.
/// </para>
/// </remarks>
public sealed class GradioServer : IAsyncDisposable
{
    private readonly WebApplication _app;
    private readonly SsrOptions _ssr;
    private readonly ISsrRuntimeFactory _ssrRuntimeFactory;
    private readonly string _serverName;
    private readonly int _serverPort;
    private ISsrRuntime? _runtime;
    private bool _disposed;

    /// <summary>Builds a host, without starting it.</summary>
    /// <param name="options">How to listen; defaults are the Python version's.</param>
    public GradioServer(GradioServerOptions? options = null)
    {
        var effective = options ?? new GradioServerOptions();

        var builder = WebApplication.CreateSlimBuilder();

        // Before Gradio's own, so that a service the caller registers is the one that is kept.
        effective.ConfigureServices?.Invoke(builder.Services);

        builder.Services.AddGradio();

        // The health endpoint is not part of the route table — the Python version has no such route — so it is
        // registered here, where the host that wants one says so, and only when it said so.
        if (effective.HealthCheckPath is not null)
        {
            builder.Services.AddGradioHealthChecks();
        }

        // The standalone entry point is the application, so it owns its logging the way the Python version's
        // CLI does. A host that integrates Gradio through AddGradio keeps its own logging instead.
        builder.Logging.AddSimpleConsole();
        effective.ConfigureLogging?.Invoke(builder.Logging);

        _ssr = SsrOptions.Resolve(effective);
        _ssrRuntimeFactory = effective.SsrRuntimeFactory ?? new NodeSsrRuntimeFactory();
        _serverName = effective.ServerName;
        _serverPort = effective.ServerPort;

        // With SSR on, the address a browser opens belongs to the renderer and this application
        // moves behind it, to a port of its own. Binding the public address here as well would make
        // the two servers fight over it, so the port is the only thing that changes.
        builder.WebHost.UseUrls(
            _ssr.Enabled
                ? $"http://{IPAddress.Loopback}:0"
                : $"http://{effective.ServerName}:{effective.ServerPort}");

        _app = builder.Build();
        _app.UseGradio();

        if (effective.HealthCheckPath is not null)
        {
            _app.MapGradioHealth(effective.HealthCheckPath);
        }
    }

    /// <summary>The container, with the kernel and the application registered in it.</summary>
    public IServiceProvider Services => _app.Services;

    /// <summary>The application this server hosts.</summary>
    /// <remarks>Build the UI on it before starting the server.</remarks>
    public Blocks Blocks => _app.Services.GetRequiredService<Blocks>();

    /// <summary>The address the server listens on.</summary>
    /// <value>The address as resolved, so a port of 0 reports the port the operating system chose.</value>
    /// <remarks>
    /// With SSR on this is the renderer's address, because that is the one a browser can use: the
    /// application's own port is behind it and is not the address the caller asked for.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The server has not been started.</exception>
    public Uri BaseAddress =>
        _runtime is not null
            ? _runtime.Address
            : _app.Urls.Count > 0
                ? new Uri(_app.Urls.First(), UriKind.Absolute)
                : throw new InvalidOperationException(
                    $"The server has not been started; call {nameof(StartAsync)} first.");

    /// <summary>Starts listening.</summary>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>A task that completes once the server is listening.</returns>
    /// <remarks>
    /// With SSR on this is where the renderer starts, and it starts last: it renders by asking this
    /// server for the config, so there is nothing for it to render until this server is listening.
    /// A start that cannot bring the renderer up is undone rather than left half done, because an
    /// application that listens on a port nobody was told about is worse than one that did not
    /// start.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The server has been disposed.</exception>
    /// <exception cref="InvalidOperationException">
    /// SSR is on and the runtime could not be started; the message says what is missing.
    /// </exception>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _app.StartAsync(cancellationToken);

        if (!_ssr.Enabled)
        {
            return;
        }

        try
        {
            _runtime = await _ssrRuntimeFactory.StartAsync(
                new SsrRuntimeRequest
                {
                    NodePath = _ssr.NodePath,
                    Host = _serverName,
                    Port = _serverPort,
                    BackendPort = PortOf(_app.Urls.First()),
                },
                cancellationToken);
        }
        catch
        {
            await _app.StopAsync(CancellationToken.None);

            throw;
        }
    }

    /// <summary>Stops listening.</summary>
    /// <param name="cancellationToken">Cancels the stop.</param>
    /// <returns>A task that completes once the server has stopped.</returns>
    /// <remarks>With SSR on, the renderer stops first: it is the address a browser was given.</remarks>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            return;
        }

        await StopRuntimeAsync();

        await _app.StopAsync(cancellationToken);
    }

    /// <summary>Stops the server and disposes the container, and with it the application.</summary>
    /// <returns>A task that completes once everything is released.</returns>
    /// <remarks>
    /// Stopping first is what lets a hosted service finish: disposing a host releases its services
    /// without asking them to stop, so a sweep that deletes an application's temporary files would
    /// never be given the chance to.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await StopRuntimeAsync();

        await _app.StopAsync(CancellationToken.None);

        await _app.DisposeAsync();
    }

    /// <summary>The port of an address this host is listening on.</summary>
    /// <param name="address">One of the addresses the host reports after it started.</param>
    /// <returns>The port, as the operating system resolved it.</returns>
    private static int PortOf(string address) => new Uri(address, UriKind.Absolute).Port;

    /// <summary>Stops the renderer, if one is running.</summary>
    /// <returns>A task that completes once it is gone.</returns>
    private async Task StopRuntimeAsync()
    {
        if (_runtime is null)
        {
            return;
        }

        await _runtime.DisposeAsync();

        _runtime = null;
    }
}

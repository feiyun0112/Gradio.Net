using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Gradio.Net;
/// <summary>
/// The Node process that renders the page and forwards everything else to the .NET server.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's SSR is not a rendering call the server makes - it is a second HTTP server
/// in front of the first one. Node takes the address a browser opens, renders the document itself,
/// and proxies every other path to the application behind it, so the application never learns that
/// a proxy is there: the routes, the queue and the event stream are all the same ones .
/// </para>
/// <para>
/// That division is why the backend listens on a port of its own while this process is running.
/// The address a caller is given is the proxy's, and it is only handed out once the proxy has
/// answered a request: a client that connects to a port Node has opened but cannot yet serve
/// from would see a 502 rather than a page.
/// </para>
/// <para>
/// Stopping is not graceful: the runtime is killed, with its children, and the extracted tree is
/// deleted. Node's own shutdown handler is written for a process whose owner is waiting for it,
/// which is not the case here - a .NET host disposing a server has to finish.
/// </para>
/// </remarks>
internal sealed class NodeSsrRuntime : ISsrRuntime
{
    /// <summary>How long the runtime is given to answer its first request.</summary>
    private static readonly TimeSpan _startupTimeout = TimeSpan.FromSeconds(30);

    /// <summary>How long a single readiness probe may take before the next one is tried.</summary>
    private static readonly TimeSpan _probeTimeout = TimeSpan.FromSeconds(2);

    /// <summary>How long the runtime is given to exit once it has been killed.</summary>
    private static readonly TimeSpan _shutdownTimeout = TimeSpan.FromSeconds(5);

    private static readonly TimeSpan _probeInterval = TimeSpan.FromMilliseconds(200);

    private readonly Process _process;
    private readonly string _directory;
    private bool _disposed;

    private NodeSsrRuntime(Process process, string directory, Uri address)
    {
        _process = process;
        _directory = directory;
        Address = address;
    }

    /// <inheritdoc />
    public Uri Address { get; }

    /// <summary>Starts the runtime in front of an application that is already listening.</summary>
    /// <param name="request">Where to listen, and where the application is.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>The running runtime.</returns>
    /// <exception cref="InvalidOperationException">
    /// No usable runtime was found, it could not be started, it exited before it served a page, or
    /// it did not answer within <see cref="_startupTimeout"/>.
    /// </exception>
    internal static async Task<NodeSsrRuntime> StartAsync(
        SsrRuntimeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var nodePath = NodeRuntime.Locate(request.NodePath);
        var directory = SsrAssets.ExtractToTemporaryDirectory();
        var chosen = request.Port == 0 ? FreePort() : request.Port;
        var address = new Uri($"http://{request.Host}:{chosen.ToString(CultureInfo.InvariantCulture)}/");

        Process? process = null;
        try
        {
            process = Launch(nodePath, directory, request.Host, chosen, request.BackendPort);
            await WaitUntilServingAsync(process, ProbeAddress(request.Host, chosen), cancellationToken)
                .ConfigureAwait(false);

            return new NodeSsrRuntime(process, directory, address);
        }
        catch
        {
            Stop(process);
            SsrAssets.TryDelete(directory);
            throw;
        }
    }

    /// <summary>Kills the runtime and deletes the tree it was started from.</summary>
    /// <returns>A task that completes once both are gone.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!_process.HasExited)
        {
            try
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync().WaitAsync(_shutdownTimeout).ConfigureAwait(false);
            }
            catch (Exception error) when (error is InvalidOperationException or Win32Exception or TimeoutException)
            {
                // Already gone, or refusing to go: the process is not the caller's to wait for, and
                // the extracted tree below is what the next start re-creates anyway.
            }
        }

        _process.Dispose();
        SsrAssets.TryDelete(_directory);
    }

    /// <summary>Starts the runtime, with the addresses it needs to find the application.</summary>
    /// <param name="nodePath">The runtime to start.</param>
    /// <param name="directory">The extracted tree to start it from.</param>
    /// <param name="host">The address the proxy should listen on.</param>
    /// <param name="port">The port a browser opens.</param>
    /// <param name="backendPort">The port the application is listening on.</param>
    /// <returns>The process.</returns>
    /// <exception cref="InvalidOperationException">The runtime could not be started.</exception>
    private static Process Launch(string nodePath, string directory, string host, int port, int backendPort)
    {
        var startInfo = new ProcessStartInfo(nodePath)
        {
            UseShellExecute = false,
            WorkingDirectory = directory,
        };

        // The register hook has to be a module specifier Node can resolve, which on Windows means
        // a file URL rather than a path: "file://" + "D:\\..." is not one.
        startInfo.ArgumentList.Add("--import");
        startInfo.ArgumentList.Add(new Uri(Path.Combine(directory, SsrAssets.RegisterModule)).AbsoluteUri);
        startInfo.ArgumentList.Add(Path.Combine(directory, SsrAssets.ServerDirectory.Replace('/', Path.DirectorySeparatorChar)));

        // The names the Python version's runtime reads. The application is always reached over loopback,
        // because the proxy is the only thing that should be listening on the public address.
        startInfo.Environment["HOST"] = host;
        startInfo.Environment["PORT"] = port.ToString(CultureInfo.InvariantCulture);
        startInfo.Environment["GRADIO_PYTHON_HOST"] = IPAddress.Loopback.ToString();
        startInfo.Environment["GRADIO_PYTHON_PORT"] = backendPort.ToString(CultureInfo.InvariantCulture);

        try
        {
            return Process.Start(startInfo)
                ?? throw new InvalidOperationException($"'{nodePath}' did not start.");
        }
        catch (Win32Exception error)
        {
            throw new InvalidOperationException(
                $"The SSR runtime '{nodePath}' could not be started: {error.Message}", error);
        }
    }

    /// <summary>Waits until the proxy answers a request, or explains why it never did.</summary>
    /// <param name="process">The runtime that was started.</param>
    /// <param name="probe">The address to ask.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>A task that completes once the proxy has answered.</returns>
    /// <exception cref="InvalidOperationException">The runtime exited, or never answered.</exception>
    private static async Task WaitUntilServingAsync(
        Process process,
        Uri probe,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient { Timeout = _probeTimeout };
        var clock = Stopwatch.StartNew();

        while (clock.Elapsed < _startupTimeout)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // A port that accepts connections is not a server that can answer: the runtime opens
            // its socket before it has loaded the compiled app, and a request in that window is a
            // 502. So the probe asks for a page, and only a non-error answer counts.
            if (process.HasExited)
            {
                // The runtime's own output is not captured, so it went to this process's console
                // rather than into this message. Naming the runtime and the address is what tells
                // a reader which lines above to look at when several servers are starting at once.
                throw new InvalidOperationException(
                    $"The SSR runtime '{process.StartInfo.FileName}' exited with code "
                    + $"{process.ExitCode.ToString(CultureInfo.InvariantCulture)} before it answered {probe}. "
                    + "Its own output, printed above this message, says why.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, probe);
                using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if ((int)response.StatusCode < 500)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Not listening yet.
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // The probe timed out; the runtime may still be loading.
            }

            await Task.Delay(_probeInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new InvalidOperationException(
            $"The SSR runtime did not answer {probe} within {_startupTimeout.TotalSeconds:0} seconds. "
            + "Its output above says why.");
    }

    /// <summary>The address a probe can reach, which a wildcard bind is not.</summary>
    /// <param name="host">The address the proxy was told to listen on.</param>
    /// <param name="port">The port it listens on.</param>
    /// <returns>An address that can be connected to from this machine.</returns>
    private static Uri ProbeAddress(string host, int port)
    {
        var wildcard = host is "0.0.0.0" or "*" or "::" or "[::]" or "+";
        var reachable = wildcard ? IPAddress.Loopback.ToString() : host;

        return new Uri($"http://{reachable}:{port.ToString(CultureInfo.InvariantCulture)}/");
    }

    /// <summary>A port the operating system says is free.</summary>
    /// <returns>The port number.</returns>
    /// <remarks>
    /// Asking the operating system and closing the socket leaves a window in which another
    /// process could take the port. The alternative - starting the runtime on port 0 and reading
    /// the address out of its output - would make the address depend on a log line, which is a
    /// worse thing to depend on.
    /// </remarks>
    private static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    /// <summary>Kills a runtime that never became usable, if one was started.</summary>
    /// <param name="process">The process, or <see langword="null"/> when it never started.</param>
    private static void Stop(Process? process)
    {
        if (process is null)
        {
            return;
        }

        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit((int)_shutdownTimeout.TotalMilliseconds);
            }
        }
        catch (Exception error) when (error is InvalidOperationException or Win32Exception)
        {
            // Nothing left to stop.
        }

        process.Dispose();
    }
}

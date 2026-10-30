using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Starts serving an application, and waits until it is stopped.</summary>
    /// <param name="app">The application to serve.</param> <param name="serverName">The address to
    /// listen on, or <see langword="null"/> for loopback.</param> <param name="serverPort">The
    /// port to listen on, or <see langword="null"/> for the Python version's 7860.</param>
    /// <param name="preventThreadLock">Whether to return at once instead of waiting, or
    /// <see langword="null"/> to wait.</param> <param name="quiet">Whether to say nothing about the
    /// address, or <see langword="null"/> to say it.</param>
    /// <returns>
    /// The server that was started. A blocking call returns once it has stopped, so the handle is
    /// only worth taking from a caller that asked not to block.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// This is the Python version's <c>demo.launch()</c>, and it blocks for the same reason: a
    /// script that ended with it in Python kept running, and a port that returned at once would
    /// exit before anybody could open the page. <paramref name="preventThreadLock"/> is the
    /// Python version's own way out — the call returns with the server running — and it is what a
    /// caller that wants to drive the application itself (a test, a larger host) asks for.
    /// </para>
    /// <para>
    /// The host is given this application and the render context accessor it was built with, so that
    /// the container serves the object that was written into rather than one of its own. That is the
    /// one thing this layer knows about the host, and it knows it through the same
    /// <see cref="GradioServerOptions.ConfigureServices"/> hook any host has: nothing here reaches
    /// into the server's wiring.
    /// </para>
    /// <para>
    /// A port asks for port 0 when it wants the operating system to choose, which is what a test
    /// does; the Python version would treat 0 as a port to try and then walk forward from it.
    /// </para>
    /// </remarks>
    public static GradioServer Launch(
        this Blocks app,
        string? serverName = null,
        int? serverPort = null,
        bool? preventThreadLock = null,
        bool? quiet = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var accessor = app.ContextAccessor;

        var server = new GradioServer(new GradioServerOptions
        {
            ServerName = serverName ?? "127.0.0.1",
            ServerPort = serverPort ?? 7860,
            ConfigureServices = services =>
            {
                // Before the framework's own registrations, which are "try add": the application and
                // the accessor that carries its render scopes are this one's, not a second pair.
                services.AddSingleton<IRenderContextAccessor>(accessor);
                services.AddSingleton(app);
            },
            ConfigureLogging = quiet is true ? logging => logging.ClearProviders() : null,
        });

        try
        {
            server.StartAsync().GetAwaiter().GetResult();
        }
        catch
        {
            server.DisposeAsync().AsTask().GetAwaiter().GetResult();

            throw;
        }

        if (quiet is not true)
        {
            Console.WriteLine($"Running on local URL: {server.BaseAddress}");
        }

        if (preventThreadLock is true)
        {
            return server;
        }

        try
        {
            WaitForShutdown();
        }
        finally
        {
            server.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        return server;
    }

    /// <summary>Waits until the process is asked to stop.</summary>
    /// <remarks>
    /// The Python version's <c>launch()</c> loops until a thread lock is released, which is how a
    /// notebook can stop a server without killing the kernel. A console process has no such lock:
    /// what stops it is Ctrl-C, so that is what this waits for. The signal is cancelled rather than
    /// obeyed, because stopping is the caller's job — the server it started is disposed by the
    /// frame that owns it.
    /// </remarks>
    private static void WaitForShutdown()
    {
        using var stopped = new ManualResetEventSlim();

        ConsoleCancelEventHandler onCancel = (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            stopped.Set();
        };

        Console.CancelKeyPress += onCancel;

        try
        {
            stopped.Wait();
        }
        finally
        {
            Console.CancelKeyPress -= onCancel;
        }
    }
}

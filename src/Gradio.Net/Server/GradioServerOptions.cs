using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net;
/// <summary>
/// What the standalone entry point is configured with.
/// </summary>
/// <remarks>
/// This grows with the capabilities that need configuring — auth, file access, root path, queue
/// settings — as those land; it holds only what the server itself needs so far.
/// </remarks>
public sealed class GradioServerOptions
{
    /// <summary>The interface to listen on. Defaults to <c>127.0.0.1</c>, as the Python version
    /// does.</summary>
    public string ServerName { get; set; } = "127.0.0.1";

    /// <summary>
    /// The port to listen on. Defaults to 7860, as the Python version does; 0 asks the operating
    /// system for a free port, which is what a test wants.
    /// </summary>
    /// <remarks>
    /// With <see cref="SsrMode"/> on, this is the port a browser opens and the SSR runtime listens
    /// on; the application itself moves to a port of its own behind it. A port of 0 is resolved
    /// either way, so <see cref="GradioServer.BaseAddress"/> is always the address to hand out.
    /// </remarks>
    public int ServerPort { get; set; } = 7860;

    /// <summary>
    /// Whether to render the page with the SSR runtime. Off by default.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which reads <c>GRADIO_SSR_MODE</c> and treats anything but
    /// <c>true</c> as off — the same resolution, in the same order, as the Python version's
    /// <c>ssr_mode</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// Server-side rendering is a second HTTP server in front of this one: a Node runtime renders
    /// the document and forwards every other request to the application, which is how the Python
    /// version does it. Turning it on therefore requires a Node 20+ runtime the caller provides —
    /// this package does not ship one, and nothing in it needs Node when this is off.
    /// </para>
    /// <para>
    /// Only the standalone entry point can turn it on. An application that integrates Gradio
    /// through <c>AddGradio</c> owns its own address and its own proxy, so a second server in front
    /// of it is the host's decision rather than this package's; the Python version disables SSR the
    /// same way when an app is mounted on another one.
    /// </para>
    /// <para>
    /// The vendored runtime is embedded in this package, so the only thing an application has to
    /// bring is the runtime itself. Where it cannot be started, the server fails to start and says
    /// why rather than quietly serving a client-rendered page.
    /// </para>
    /// </remarks>
    public bool? SsrMode { get; set; }

    /// <summary>
    /// The Node runtime to render with, or <see langword="null"/> to use <c>GRADIO_NODE_PATH</c>
    /// and then the first <c>node</c> on <c>PATH</c>.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is the Python version's resolution order: the environment
    /// variable names the runtime when it is set, and otherwise the one on <c>PATH</c> is used.
    /// </value>
    /// <remarks>
    /// Read only when <see cref="SsrMode"/> resolves to on. The version is checked when the server
    /// starts, because a path can name anything: what matters is the version the runtime reports,
    /// not the name it was found under.
    /// </remarks>
    public string? SsrNodePath { get; set; }

    /// <summary>
    /// The renderer to start, or <see langword="null"/> to run the vendored one on Node.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which starts the Python version's compiled SSR runtime with the Node
    /// that <see cref="SsrNodePath"/> names.
    /// </value>
    /// <remarks>
    /// <para>
    /// Server-side rendering is an external dependency, so it is an interface rather than a
    /// process this package insists on. A host that renders its page another way — a
    /// different runtime, a sidecar in the same pod, a test that wants to see what the server asks
    /// for without a Node installed — supplies its own factory, and the package then needs no Node
    /// at all.
    /// </para>
    /// <para>
    /// The contract an implementation is held to is small on purpose: start with the host, port and
    /// application port it is given, answer on the address it reports, and stop when disposed.
    /// </para>
    /// </remarks>
    public ISsrRuntimeFactory? SsrRuntimeFactory { get; set; }

    /// <summary>
    /// Registers services before Gradio's own, which is how a caller replaces or adds one: the
    /// registrations Gradio makes are "try add", so a service that is already there is kept.
    /// </summary>
    /// <remarks>
    /// This is where a standalone host plugs in the things its application needs — an identity
    /// provider of its own, a store in place of the default one — without the container being
    /// assembled by hand. A host that integrates Gradio through <c>AddGradio</c> registers into its
    /// own container instead and does not use this.
    /// </remarks>
    public Action<IServiceCollection>? ConfigureServices { get; set; }

    /// <summary>
    /// Where to serve the health checks, or <see langword="null"/> to serve none.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is the Python version's behaviour: it has no health route, and
    /// a route this framework adds by default would fail the route-set comparison.
    /// </value>
    /// <remarks>
    /// <para>
    /// Set it to a path — <c>"/health"</c> is the conventional one — and the standalone host registers
    /// the readiness check and serves it there. A host that integrates Gradio through
    /// <c>AddGradio</c> has its own health pipeline and calls
    /// <see cref="GradioHealthExtensions.AddGradioHealthChecks"/> instead.
    /// </para>
    /// <para>
    /// The check is unhealthy until the application has been rendered and the queue has started, so
    /// the window between the host listening and the application being written is a window in which
    /// an orchestrator will not send it traffic.
    /// </para>
    /// </remarks>
    public string? HealthCheckPath { get; set; }

    /// <summary>
    /// Adjusts the server's logging. The console provider is added first, so this is where a
    /// caller turns it down or replaces it; a host that integrates Gradio through
    /// <c>AddGradio</c> keeps its own logging and does not use this.
    /// </summary>
    public Action<ILoggingBuilder>? ConfigureLogging { get; set; }
}

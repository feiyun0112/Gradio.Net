using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A renderer that is not Node: it serves a document of its own and records what it was asked for.
/// </summary>
/// <remarks>
/// <para>
/// Server-side rendering is an external dependency, so what the package promises is a contract
/// rather than a process: start with this host, this port and this application port; answer on
/// the address you report; stop when disposed. This
/// stands in for the runtime so that the contract is checked on machines where Node is not
/// installed, which is most of them.
/// </para>
/// <para>
/// What it does not stand in for is the document: that the vendored runtime renders the Python
/// version's page is a question only the real one can answer, and the tests that ask it skip
/// without Node.
/// </para>
/// </remarks>
internal sealed class FakeSsrRuntimeFactory : ISsrRuntimeFactory
{
    /// <summary>What the server asked for, once it has asked.</summary>
    internal SsrRuntimeRequest? Request { get; private set; }

    /// <summary>Whether the renderer the server was given has been disposed.</summary>
    internal bool Stopped { get; private set; }

    /// <inheritdoc />
    public async Task<ISsrRuntime> StartAsync(SsrRuntimeRequest request, CancellationToken cancellationToken)
    {
        Request = request;

        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls($"http://{request.Host}:{request.Port}");

        var app = builder.Build();
        app.MapGet("/", () => "a document the renderer made");

        await app.StartAsync(cancellationToken);

        return new FakeSsrRuntime(app, new Uri(app.Urls.First(), UriKind.Absolute), this);
    }

    /// <summary>The renderer, which answers one page and remembers that it was stopped.</summary>
    private sealed class FakeSsrRuntime : ISsrRuntime
    {
        private readonly WebApplication _app;
        private readonly FakeSsrRuntimeFactory _owner;

        internal FakeSsrRuntime(WebApplication app, Uri address, FakeSsrRuntimeFactory owner)
        {
            _app = app;
            _owner = owner;
            Address = address;
        }

        public Uri Address { get; }

        public async ValueTask DisposeAsync()
        {
            await _app.StopAsync(CancellationToken.None);
            await _app.DisposeAsync();

            _owner.Stopped = true;
        }
    }
}

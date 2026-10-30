using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Shared;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Server-side rendering, which is a second HTTP server in front of this one rather than a way of
/// writing the page.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the shape of that arrangement: off by default, a missing runtime named
/// in the failure rather than silently swapped for client-side rendering, the document coming from
/// the runtime when it is on, and the application's own routes still answering through it.
/// </para>
/// <para>
/// The tests that need a runtime skip where there is none, as the browser tests do: Node is an
/// opt-in dependency, and a machine without it cannot exercise this capability at all.
/// </para>
/// </remarks>
public sealed class SsrTests
{
    /// <summary>The prefix the build embeds the SSR tree under, mirrored from Gradio.Net.csproj.</summary>
    private const string EmbeddedPrefix = "Gradio.Net.ssr/";

    [Fact]
    public async Task Ssr_is_off_unless_the_application_asks_for_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build);

        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        // The .NET template: the document the browser boots the vendored frontend from.
        Assert.Contains("<base href=\"./\" />", page, StringComparison.Ordinal);
        Assert.Contains("<gradio-app", page, StringComparison.Ordinal);

        // And nothing the SSR runtime would have produced.
        Assert.DoesNotContain("__sveltekit_", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Ssr_off_does_not_look_for_a_runtime()
    {
        // The path exists and is not a runtime - it is this test assembly. A start that resolved it
        // would run it and refuse it, so the page coming back is the proof that nothing looked:
        // "off means nothing about Node is touched", not "off means the failure is
        // swallowed".
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = false;
                options.SsrNodePath = typeof(SsrTests).Assembly.Location;
            });

        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        // The client-rendered page, which carries the whole config for the browser to render from.
        // The SSR document has no such script: it is rendered, not handed a config to render with.
        Assert.Contains("<gradio-app", page, StringComparison.Ordinal);
        Assert.Contains("window.gradio_config", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_runtime_that_is_not_there_is_named_in_the_failure()
    {
        var missing = SsrNode.NoSuchRuntimePath();

        await using var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            SsrMode = true,
            SsrNodePath = missing,
        });

        ClickExample.Build(server.Blocks);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => server.StartAsync(TestContext.Current.CancellationToken));

        // What is wrong, what to install, and how to point at it: a message a reader can act on
        // without reading this repository.
        Assert.Contains(missing, error.Message, StringComparison.Ordinal);
        Assert.Contains("Node 20", error.Message, StringComparison.Ordinal);
        Assert.Contains("GRADIO_NODE_PATH", error.Message, StringComparison.Ordinal);

        // The application is not left listening on a port nobody was told about: a start that
        // failed is undone, so a client cannot reach a server it was never given the address of.
        await AssertStoppedAsync(server, "SSR failed to start");
    }

    [Fact]
    public async Task A_renderer_that_cannot_start_is_undone_and_says_why()
    {
        // A renderer that refuses, rather than one that is missing: the contract says a failed
        // start throws, and the server's half of that is what this asks - the reason is not
        // swallowed, and the application behind it is stopped again.
        var factory = new FailingSsrRuntimeFactory();

        await using var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            SsrMode = true,
            SsrRuntimeFactory = factory,
        });

        ClickExample.Build(server.Blocks);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => server.StartAsync(TestContext.Current.CancellationToken));

        // The renderer was asked, and its own words are what the caller reads: a generic "SSR
        // failed" would leave the operator with nothing to act on.
        Assert.NotNull(factory.Request);
        Assert.Contains(FailingSsrRuntimeFactory.Reason, error.Message, StringComparison.Ordinal);

        await AssertStoppedAsync(server, "the renderer refused to start");
    }

    [Fact]
    public async Task A_runtime_that_dies_at_start_is_named_and_the_application_is_undone()
    {
        var node = SsrNode.Locate();
        Assert.SkipUnless(node is not null, NoRuntime);

        // The port the renderer is told to open is already taken, so the runtime's own HTTP server
        // cannot bind and the process exits. That is the start failure a user actually hits - an
        // earlier instance of the same application still holding the port - and unlike a missing
        // runtime it is a process that was launched and then died.
        using var occupied = new TcpListener(IPAddress.Loopback, 0);
        occupied.Start();
        var port = ((IPEndPoint)occupied.LocalEndpoint).Port;

        await using var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = port,
            SsrMode = true,
            SsrNodePath = node,
        });

        ClickExample.Build(server.Blocks);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => server.StartAsync(TestContext.Current.CancellationToken));

        // Which runtime, which address, and that it exited rather than answered: "the SSR runtime
        // failed" would be true of every one of these and useful for none of them. The code itself
        // is not pinned, because which code Node exits with is Node's to decide.
        Assert.Contains(Path.GetFileName(node), error.Message, StringComparison.Ordinal);
        Assert.Contains(port.ToString(CultureInfo.InvariantCulture), error.Message, StringComparison.Ordinal);
        Assert.Contains("exited with code", error.Message, StringComparison.Ordinal);

        await AssertStoppedAsync(server, "the runtime died");
    }

    [Fact]
    public async Task The_page_comes_from_the_ssr_runtime_when_it_is_on()
    {
        var node = SsrNode.Locate();
        Assert.SkipUnless(node is not null, NoRuntime);

        await using var host = await StartAsync(node!);
        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        // The runtime's own document: a SvelteKit render with its hydration script and the app's
        // container, none of which the .NET template has.
        Assert.Contains("__sveltekit_", page, StringComparison.Ordinal);
        Assert.Contains("data-gradio-mode", page, StringComparison.Ordinal);
        Assert.Contains("gradio-container", page, StringComparison.Ordinal);
        Assert.DoesNotContain("<gradio-app", page, StringComparison.Ordinal);

        // The content a crawler reads without running a script: the application's own title,
        // rendered from the config the runtime fetched from it. The component tree is not in the
        // document - the Python version leaves it to hydration too, and the vendored runtime is the Python version's -
        // so the title is what "the page, not a placeholder" comes to here.
        Assert.Contains($"<title>{ClickExample.Title}</title>", page, StringComparison.Ordinal);

        // The address in the document is the one the browser used, not the internal port the
        // application moved to: the theme link is built from it, so a document naming the internal
        // port would send every client that is not on this machine to an address it cannot reach.
        var origin = host.BaseAddress.GetLeftPart(UriPartial.Authority);
        Assert.Contains($"root_url:\"{origin}\"", page, StringComparison.Ordinal);
        Assert.Contains($"href=\"{origin}/theme.css", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_renderer_is_asked_for_the_address_and_the_port_the_application_listens_on()
    {
        // A renderer that is not Node, so this holds on a machine without one: what the package
        // promises is a contract, and this is the contract.
        var factory = new FakeSsrRuntimeFactory();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = true;
                options.SsrRuntimeFactory = factory;
            });

        var request = factory.Request;
        Assert.NotNull(request);

        // What it was asked for, value by value: the interface to listen on, the port a browser
        // opens (0, which the renderer resolves), and the application's own port.
        Assert.Equal("127.0.0.1", request.Host);
        Assert.Equal(0, request.Port);
        Assert.NotEqual(0, request.BackendPort);

        // The application is listening before the renderer is asked, because the renderer renders
        // by asking it for the config.
        using var config = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{request.BackendPort}/") };
        using var answer = await config.GetAsync("/config", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, answer.StatusCode);

        // The address handed to a caller is the renderer's, not the application's, and the page
        // that address serves is the renderer's too.
        Assert.NotEqual(request.BackendPort, host.BaseAddress.Port);
        Assert.Equal(
            "a document the renderer made",
            await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Disposing_the_server_stops_the_renderer()
    {
        var factory = new FakeSsrRuntimeFactory();

        var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = true;
                options.SsrRuntimeFactory = factory;
            });

        var address = host.BaseAddress;

        await host.DisposeAsync();

        Assert.True(factory.Stopped, "the renderer was not disposed with the server");
        Assert.False(await ListeningAsync(address), $"the renderer is still listening on {address}");
    }

    [Fact]
    public async Task The_proxy_serves_the_runtimes_own_files_and_forwards_the_applications()
    {
        var node = SsrNode.Locate();
        Assert.SkipUnless(node is not null, NoRuntime);

        await using var host = await StartAsync(node!);

        // The runtime's own compiled client, which only it has.
        using var version = await host.Client.GetAsync("/_app/version.json", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, version.StatusCode);

        // An asset of the vendored frontend, which only the application has: same bytes as the
        // tree on disk, so the request reached it rather than a copy the runtime made.
        var asset = VendoredTree.Frontend.HashTree().Keys.First(path => path.StartsWith("assets/", StringComparison.Ordinal));
        using var forwarded = await host.Client.GetAsync("/" + asset, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, forwarded.StatusCode);
        Assert.Equal(
            VendoredTree.HashFile(Path.Combine(VendoredTree.Frontend.RootPath, asset.Replace('/', Path.DirectorySeparatorChar))),
            VendoredTree.HashBytes(await forwarded.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken)));

        // The description endpoints the page and the API client read, which are the application's.
        var config = JsonNode.Parse(
            await host.Client.GetStringAsync("/config", TestContext.Current.CancellationToken))!.AsObject();
        Assert.Equal(ClickExample.Title, config["title"]!.GetValue<string>());
        Assert.Equal(OriginOf(host), config["root"]!.GetValue<string>());

        using var info = await host.Client.GetAsync("/gradio_api/info", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, info.StatusCode);
    }

    [Fact]
    public void The_tree_in_the_assembly_is_the_vendored_tree()
    {
        var assembly = typeof(Blocks).Assembly;

        // This verifies the assembly against the directory on disk, which is the half a build
        // cannot see: a resource that was dropped from the project would still leave the
        // directory intact.
        var embedded = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(EmbeddedPrefix, StringComparison.Ordinal))
            .ToDictionary(
                name => name[EmbeddedPrefix.Length..],
                name => VendoredTree.HashBytes(Read(assembly, name)),
                StringComparer.Ordinal);

        var differences = VendoredTree.Differences(VendoredTree.Ssr.HashTree(), embedded, "embedded");

        Assert.True(differences.Count == 0, string.Join(Environment.NewLine, differences));
    }

    [Fact]
    public async Task Stopping_the_server_stops_the_runtime()
    {
        var node = SsrNode.Locate();
        Assert.SkipUnless(node is not null, NoRuntime);

        var host = await StartAsync(node!);
        var address = host.BaseAddress;

        await host.DisposeAsync();

        // The runtime is killed, so the address a browser was given stops answering rather than
        // outliving the application it was rendering for.
        var deadline = System.DateTime.UtcNow.AddSeconds(10);
        while (System.DateTime.UtcNow < deadline)
        {
            if (!await ListeningAsync(address))
            {
                return;
            }

            await Task.Delay(200, TestContext.Current.CancellationToken);
        }

        Assert.Fail($"the SSR runtime is still listening on {address} after the server was disposed");
    }

    /// <summary>What a test says instead of failing on a machine without a runtime.</summary>
    private const string NoRuntime =
        "no Node 20+ runtime was found, so the SSR capability cannot be exercised here. The package "
        + "needs none when SSR is off, which is the default.";

    /// <summary>Starts the click example with SSR on, rendered by the given runtime.</summary>
    /// <param name="node">The runtime to start.</param>
    /// <returns>The running host, whose address is the proxy's.</returns>
    private static async Task<GradioTestHost> StartAsync(string node)
    {
        return await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = true;
                options.SsrNodePath = node;
            });
    }

    /// <summary>The scheme, host and port of an address, which is what a document carries.</summary>
    /// <param name="host">The running host.</param>
    /// <returns>The origin, without a trailing slash.</returns>
    private static string OriginOf(GradioTestHost host) =>
        host.BaseAddress.GetLeftPart(UriPartial.Authority);

    /// <summary>Reads one embedded resource.</summary>
    /// <param name="assembly">The assembly to read it from.</param>
    /// <param name="name">The resource's full logical name.</param>
    /// <returns>Its bytes.</returns>
    private static byte[] Read(System.Reflection.Assembly assembly, string name)
    {
        using var stream = assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"{name} is not embedded in the assembly");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);

        return buffer.ToArray();
    }

    /// <summary>Asserts that the application is not left listening after a start that failed.</summary>
    /// <param name="server">The server whose start failed.</param>
    /// <param name="what">What failed, for the message when it did not.</param>
    /// <returns>A task that completes once the check is made.</returns>
    private static async Task AssertStoppedAsync(GradioServer server, string what)
    {
        var address = server.BaseAddress;

        Assert.False(await ListeningAsync(address), $"the application is still listening on {address} after {what}");
    }

    /// <summary>Whether something accepts connections at an address.</summary>
    /// <param name="address">The address to try.</param>
    /// <returns><see langword="true"/> when a connection was accepted.</returns>
    private static async Task<bool> ListeningAsync(Uri address)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(address.Host, address.Port, TestContext.Current.CancellationToken);

            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}

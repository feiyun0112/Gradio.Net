using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Gradio.Net.Tests.Server.QueueRuns;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The kernel as a host sees it: an application that can be written until it is served, and a
/// container that holds the application's own services beside the framework's.
/// </summary>
/// <remarks>
/// Everything here goes through <see cref="GradioTestHost"/>, so the two entry points are the ones
/// the package ships rather than a host assembled for the test.
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class KernelHostingTests
{
    [Fact]
    public async Task An_application_disposed_before_the_host_started_is_still_served()
    {
        // The Python version's `with gr.Blocks() as demo:` ends before `demo.launch()`, so an application
        // whose using block has already closed is the normal case, not an edge one: it has to
        // render, and the host has to serve it.
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Markdown { Value = "# inside" });
                }

                blocks.Dispose();

                using (blocks.BeginRender())
                {
                    blocks.Add(new Markdown { Value = "# after" });
                }
            });

        using var response = await host.Client.GetAsync("/", cancellationToken);
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Contains("# inside", page, StringComparison.Ordinal);
        Assert.Contains("# after", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_application_stops_accepting_blocks_once_the_host_is_serving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Markdown { Value = "# hello" });
                }
            });

        var blocks = host.Services.GetRequiredService<Blocks>();

        // Starting the host is not serving it: the first request is the moment the page a browser
        // gets becomes the one that was written.
        Assert.False(blocks.IsSealed);

        using var response = await host.Client.GetAsync("/", cancellationToken);
        response.EnsureSuccessStatusCode();

        Assert.True(blocks.IsSealed);

        var refused = Assert.Throws<InvalidOperationException>(() => blocks.BeginRender());

        Assert.Contains("already being served", refused.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_service_the_application_registers_is_the_one_its_handler_uses()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var greeting = new Greeting("Hello");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,

            // The application's own service, registered before Gradio's: the framework's
            // registrations are "try add", so this is the one the container keeps.
            blocks => Build(blocks, greeting),
            configure: services => services.AddSingleton(greeting));

        // The same instance the application registered, not a framework one that replaced it.
        Assert.Same(greeting, host.Services.GetRequiredService<Greeting>());

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("greet", cancellationToken);

        Assert.Equal("Hello, world", await RunAsync(queue, fnIndex, ["world"], "session", cancellationToken));
    }

    /// <summary>An application whose one binding answers with what its service said.</summary>
    private static void Build(Blocks blocks, Greeting greeting)
    {
        Textbox input;
        Textbox output;

        using (blocks.BeginRender())
        {
            input = blocks.Add(new Textbox { Label = "Name" });
            output = blocks.Add(new Textbox { Label = "Greeting" });
        }

        input.Change(
            (Func<string, string>)(name => greeting.Greet(name)),
            [input],
            [output],
            apiName: "greet");
    }

    /// <summary>A service of the application's own, not one of the framework's.</summary>
    private sealed class Greeting(string salutation)
    {
        internal string Greet(string name) => $"{salutation}, {name}";
    }
}

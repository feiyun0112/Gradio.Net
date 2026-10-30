using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// Two of the Python version's own examples, ported line for line, and the run that proves the port
/// works.
/// </summary>
/// <remarks>
/// <para>
/// This is the evidence the whole layer is for (ticket 45): not that a set comparison passes, but
/// that a page copied out of the Python version's documentation compiles here and describes itself
/// the same. The third shape — a hand-written page with event listeners — is
/// <see cref="GrAppTests.A_page_is_written_the_way_the_python_writes_it"/>, which carries the
/// Python it was copied from, and <see cref="GrEventRunTests"/>, which compares its messages.
/// </para>
/// <para>
/// <strong>What changed, line by line, and nothing else.</strong> Case only: <c>gr.interface</c> is
/// not a thing, but <c>gr.Interface</c> is. Named arguments: <c>fn=</c> is <c>fn:</c>. A string
/// shortcut list stays a string shortcut list. That is the whole of it for the quickstart: a port is
/// three edits, not a translation.
/// </para>
/// <para>
/// The chat example changes one more thing, and it is the decorator-shaped one: the Python
/// version's handler takes <c>message, history</c> and returns a string, which is a
/// <c>Func&lt;string, string&gt;</c> here because the history a stateless handler ignores is not a
/// parameter it has to declare. The alternative — making every ported chat handler declare an
/// argument it does not read — would be a deviation invented for the port's convenience rather than
/// for the language's limits.
/// </para>
/// </remarks>
public sealed class GrPortingTests
{
    [Fact]
    public async Task The_quickstart_example_ports_line_for_line()
    {
        //     import gradio as gr
        //
        //     def greet(name, intensity):
        //         return "Hello, " + name + "!" * int(intensity)
        //
        //     demo = gr.Interface(
        //         fn=greet,
        //         inputs=["text", "slider"],
        //         outputs=["text"],
        //     )
        //
        //     demo.launch()
        var demo = gr.Interface(
            (Func<string, double, string>)Greet,
            inputs: ["text", "slider"],
            outputs: ["text"],
            apiName: "greet");

        // The same page written with the core API: the two documents have to be one document.
        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (core.BeginRender())
        {
            _ = new Interface(
                core,
                (Func<string, double, string>)Greet,
                new ComponentCollection { "text", "slider" },
                new ComponentCollection { "text" },
                apiName: "greet");
        }

        Assert.Equal(ConfigOf(core), ConfigOf(demo));

        // And it runs: the handler answers what the Python answers.
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var server = demo.Launch(serverPort: 0, preventThreadLock: true, quiet: true);
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false })
        {
            BaseAddress = server.BaseAddress,
        };

        using var response = await client.PostAsJsonAsync(
            "/gradio_api/api/greet",
            new JsonObject { ["data"] = new JsonArray("Ada", 3), ["fn_index"] = 0 },
            cancellationToken);

        var answer = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("""["Hello, Ada!!!"]""", answer["data"]!.ToJsonString());
    }

    [Fact]
    public async Task The_chat_example_ports_line_for_line()
    {
        //     import gradio as gr
        //
        //     def echo(message, history):
        //         return message
        //
        //     demo = gr.ChatInterface(fn=echo, title="Echo")
        //
        //     demo.launch()
        var demo = gr.ChatInterface((Func<string, string>)(message => message), title: "Echo");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (core.BeginRender())
        {
            _ = new ChatInterface(core, (Func<string, string>)(message => message), title: "Echo");
        }

        Assert.Equal(ConfigOf(core), ConfigOf(demo));

        // The page it draws is a chat: a chat window, a field, and the endpoint the field submits to.
        var config = demo.GetConfig();
        var kinds = config["components"]!.AsArray()
            .Select(component => component!["type"]!.GetValue<string>())
            .ToList();

        Assert.Contains("chatbot", kinds);
        Assert.NotEmpty(config["dependencies"]!.AsArray());

        var cancellationToken = TestContext.Current.CancellationToken;
        await using var server = demo.Launch(serverPort: 0, preventThreadLock: true, quiet: true);
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false })
        {
            BaseAddress = server.BaseAddress,
        };

        using var page = await client.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
    }

    /// <summary>The Python version's <c>greet</c>, character for character.</summary>
    /// <param name="name">The name to greet.</param>
    /// <param name="intensity">How many exclamation marks.</param>
    /// <returns>The greeting.</returns>
    private static string Greet(string name, double intensity) =>
        "Hello, " + name + new string('!', (int)intensity);

    /// <summary>The served document, without the fields that belong to the instance.</summary>
    /// <param name="demo">The application.</param>
    /// <returns>The document as the text a client receives.</returns>
    private static string ConfigOf(Blocks demo)
    {
        var config = demo.GetConfig();
        config.Remove("app_id");

        foreach (var component in config["components"]!.AsArray())
        {
            component!["props"]?.AsObject().Remove("secret");
        }

        return config.ToJsonString();
    }
}

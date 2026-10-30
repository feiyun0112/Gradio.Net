using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The ten-line application this layer is written for: a field, a button, a second field, written
/// with the layer and started with <c>Launch</c>.
/// </summary>
/// <remarks>
/// <para>
/// It is the smallest thing that proves the layer is a layer rather than a set of static methods:
/// the application is written, the scope it was written in is left, and it still starts and
/// answers. A port's script does exactly that — the Python dedent is the C# brace — so a
/// <c>Dispose</c> that destroyed the application would break every ported file at its last line.
/// </para>
/// <para>
/// The shape is the Python version's own: <c>using (var demo = gr.Blocks())</c> is <c>with
/// gr.Blocks() as demo:</c>, one line for one line. Nothing between the braces says "start
/// rendering" — the factory opened the scope, because <c>using</c> says only what happens at the
/// closing brace — and nothing says "add this to the page" either, because a factory is a line on
/// the page it is written on.
/// </para>
/// <para>
/// What is asserted is what the ticket asks for: it starts (the page answers), it can be clicked
/// (the binding's endpoint runs), and it computes (the answer is the handler's). The click goes
/// through the served route rather than through the object, because a handler that runs when called
/// directly says nothing about whether the page can reach it.
/// </para>
/// </remarks>
public sealed class GrAppTests
{
    [Fact]
    public async Task A_ten_line_application_starts_is_clickable_and_answers()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        Blocks demo;
        Textbox input;
        Textbox output;

        using (demo = gr.Blocks(title: "Ten lines"))
        {
            input = gr.Textbox(label: "Name");
            output = gr.Textbox(label: "Greeting");

            gr.Button("Greet").Click(
                Greet,
                [input],
                [output],
                apiName: "greet");
        }

        // Leaving the scope the Python version's `with` would leave: the application is still the application.
        await using var server = demo.Launch(serverPort: 0, preventThreadLock: true, quiet: true);
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false })
        {
            BaseAddress = server.BaseAddress,
        };

        using var page = await client.GetAsync("/", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);

        using var response = await client.PostAsJsonAsync(
            "/gradio_api/api/greet",
            new JsonObject { ["data"] = new JsonArray("ada"), ["fn_index"] = 0 },
            cancellationToken);

        var answer = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("""["Hello, ada!"]""", answer["data"]!.ToJsonString());
    }

    [Fact]
    public void The_page_the_layer_builds_carries_the_blocks_it_was_given()
    {
        // The three components and one binding, in the order they were written: a factory that
        // returned a component without it being added would leave a page with nothing on it, and the
        // launch test would still pass.
        using var demo = gr.Blocks();

        var input = gr.Textbox(label: "Name");
        var output = gr.Textbox(label: "Greeting");

        gr.Button("Greet").Click(Greet, [input], [output]);

        var config = demo.GetConfig();
        var kinds = config["components"]!.AsArray()
            .Select(component => component!["type"]!.GetValue<string>())
            .ToList();

        Assert.Equal(2, kinds.Count(kind => kind == "textbox"));
        Assert.Equal(1, kinds.Count(kind => kind == "button"));
        Assert.Single(config["dependencies"]!.AsArray());
    }

    [Fact]
    public void The_queue_is_configured_with_upstreams_parameter_names()
    {
        // `demo.queue(status_update_rate=2, max_size=5)` has to compile as
        // `demo.Queue(statusUpdateRate: 2, maxSize: 5)`. The core member spells the same four
        // settings the C# way, and the two coexist because a call naming the Python version's parameters is
        // not applicable to the member — so it resolves to the layer's extension, which forwards.
        using var demo = gr.Blocks();

        demo.Queue(statusUpdateRate: 2, apiOpen: false, maxSize: 5, defaultConcurrencyLimit: 3);

        Assert.False(demo.QueueSettings.LiveUpdates);
        Assert.Equal(2, demo.QueueSettings.UpdateIntervalSeconds);
        Assert.Equal(5, demo.QueueSettings.MaxSize);
        Assert.False(demo.QueueSettings.ApiOpen);
        Assert.Equal(3, demo.QueueSettings.DefaultConcurrencyLimit.Value);

        // `"not_set"` is the Python version's word for "read GRADIO_DEFAULT_CONCURRENCY_LIMIT, else one at a
        // time", which is the same resolution the application's default limit names — so the word
        // and the omission mean one thing, not two.
        demo.Queue(defaultConcurrencyLimit: "not_set");

        Assert.Null(demo.QueueSettings.DefaultConcurrencyLimit.Value);
    }

    [Fact]
    public void The_core_spelling_of_the_queue_still_resolves_to_the_core()
    {
        // The other half of "the extension yields": a call that names the core's parameters — or
        // names nothing — is applicable to the member, and the member wins. If it did not, the
        // layer would have replaced the core's queue rather than added a spelling of it.
        using var demo = gr.Blocks();

        demo.Queue(statusUpdateRate: 0.5, maxSize: 4);

        Assert.Equal(0.5, demo.QueueSettings.UpdateIntervalSeconds);
        Assert.Equal(4, demo.QueueSettings.MaxSize);
    }

    [Fact]
    public void An_application_binds_what_runs_when_the_page_opens_and_when_it_goes_away()
    {
        // `demo.load(fn=..., inputs=[...])` and `demo.unload(fn=...)`, spelled with the names
        // the Python version gives them: both are core members already, so a ported line resolves to them
        // rather than to anything this layer adds.
        using var demo = gr.Blocks();

        var name = gr.Textbox(label: "Name");

        demo.Load(fn: Opened, inputs: [name]);
        demo.Unload(fn: Closed);

        var config = demo.GetConfig();
        var events = config["dependencies"]!.AsArray()
            .Select(dependency => dependency!["targets"]!.AsArray()[0]!.AsArray()[1]!.GetValue<string>())
            .ToList();

        Assert.Equal(["load", "unload"], events);

        // A page that has work to do when the visitor leaves asks the frontend to keep a connection
        // open, which is how the server learns the visitor left at all (ticket 12).
        Assert.True(config["connect_heartbeat"]!.GetValue<bool>());
    }

    [Fact]
    public void A_page_is_written_the_way_the_python_writes_it()
    {
        // The shape this layer exists for, line for line:
        //
        //     with gr.Blocks() as demo:
        //         gr.Markdown("Start typing below and then click **Run** to see the output.")
        //         with gr.Row():
        //             input = gr.Textbox(placeholder="What is your name?")
        //             output = gr.Textbox()
        //         btn = gr.Button("Run")
        //         btn.click(fn=greet, inputs=[input], outputs=[output, input])
        //
        // Nothing names the application: `gr.Markdown(...)` is a line on the page because a factory
        // places what it builds, and `using (gr.Row())` is the block and the scope in one call.
        var demo = Welcome();

        var shape = demo.GetConfig()["components"]!.AsArray()
            .OfType<JsonObject>()
            .ToDictionary(entry => entry["id"]!.GetValue<int>(), entry => entry["type"]!.GetValue<string>());

        // The root is a block context rather than a component, so it is not in the component list
        // the ids are looked up in — it is the application, and its type is the application's.
        static string Tree(JsonObject node, IReadOnlyDictionary<int, string> types) =>
            node["children"] is JsonArray { Count: > 0 } children
                ? $"{Type(node, types)}[{string.Join(", ", children.Select(child => Tree(child!.AsObject(), types)))}]"
                : Type(node, types);

        static string Type(JsonObject node, IReadOnlyDictionary<int, string> types) =>
            types.TryGetValue(node["id"]!.GetValue<int>(), out var named) ? named : "blocks";

        // The two fields are one form because a form component is grouped into one, and the form is
        // inside the row because that is where they were written.
        Assert.Equal(
            "blocks[markdown, row[form[textbox, textbox]], button]",
            Tree(demo.GetConfig()["layout"]!.AsObject(), shape));
    }

    [Fact]
    public void A_method_can_return_the_application_from_inside_its_using_block()
    {
        // The Python version's `with` ends where the dedent is, and the object it named is the application the
        // script then launches. A method that returns from inside its `using` block has to get the
        // same thing: the scope closes on the way out, and what comes back is still the application.
        var demo = Welcome();

        // Six blocks from five lines: the markdown, the row, the two fields, the form they were
        // grouped into, and the button.
        Assert.True(demo.IsRendered);
        Assert.Equal(6, demo.Registered.Count);
    }

    [Fact]
    public void Two_applications_written_in_one_flow_do_not_see_each_other()
    {
        // The one piece of state the layer has is per flow and stacked: an application opened
        // inside another one's scope is the one being written, and the outer one is written into
        // again when the inner scope ends. Without the stack the outer application would lose the
        // lines after the inner block, silently.
        Blocks outer;
        Blocks inner;

        using (outer = gr.Blocks())
        {
            gr.Textbox(label: "Before");

            using (inner = gr.Blocks())
            {
                gr.Markdown(value: "inside");
            }

            gr.Textbox(label: "After");
        }

        // The outer application holds the two fields and the form they were grouped into; the inner
        // one holds only what was written inside it.
        Assert.Equal(["textbox", "textbox", "form"], Types(outer));
        Assert.Equal(["markdown"], Types(inner));
    }

    [Fact]
    public void A_layout_that_is_not_closed_keeps_what_is_written_after_it()
    {
        // A layout is entered when it is built, because that is the only place C# has for the
        // opening brace of the Python version's `with`. So a layout nobody disposes stays open, which is why
        // `using` is not decoration: the row below holds the textbox written after it.
        using var demo = gr.Blocks();

        var row = gr.Row();

        gr.Textbox(label: "Inside");

        // The textbox is in the row and not in a form yet: the form is made when the row closes, and
        // this row has not closed.
        Assert.Same(row, Assert.Single(demo.Registered.OfType<Row>()));
        Assert.IsType<Textbox>(Assert.Single(row.Children));
    }

    [Fact]
    public void An_application_written_again_after_its_scope_ends_is_written_again()
    {
        // Dynamic rendering (ticket 12) is a second render on the same application, and the layer
        // finds it again: the accessor it pushed is still in the flow's stack, and it reports "not
        // rendering" only for as long as no render is open. A factory written in the second scope
        // goes on the same page.
        Blocks demo;

        using (demo = gr.Blocks())
        {
            gr.Textbox(label: "first");
        }

        using (demo.BeginRender())
        {
            gr.Textbox(label: "second");
        }

        Assert.Equal(2, Types(demo).Count(type => type == "textbox"));
    }

    /// <summary>The block types a page holds, in render order.</summary>
    /// <param name="demo">The application.</param>
    /// <returns>The config's <c>type</c> for each block, which is what the frontend dispatches on.</returns>
    private static string[] Types(Blocks demo) =>
        [.. demo.GetConfig()["components"]!.AsArray()
            .Select(component => component!["type"]!.GetValue<string>())];

    /// <summary>The application the ticket's shape is written for.</summary>
    /// <returns>The application, after the scope it was written in has ended.</returns>
    private static Blocks Welcome()
    {
        using (var blocks = gr.Blocks())
        {
            gr.Markdown("Start typing below and then click **Run** to see the output.");

            Textbox input;
            Textbox output;

            using (gr.Row())
            {
                input = gr.Textbox(placeholder: "What is your name?");
                output = gr.Textbox();
            }

            var button = gr.Button("Run");

            button.Click(
                fn: new Func<string, (string, string)>(name => ($"Welcome to Gradio.Net, {name}!", "")),
                inputs: [input],
                outputs: [output, input]);

            return blocks;
        }
    }

    private static string Greet(string name) => $"Hello, {name}!";

    private static void Opened(string name)
    {
        _ = name;
    }

    private static void Closed()
    {
    }
}

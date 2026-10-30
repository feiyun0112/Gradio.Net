using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using Timer = Gradio.Net.Timer;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// The utility components as an application uses them: the value an endpoint carries, the any-JSON
/// component, the clock the frontend runs and the workflow canvas.
/// </summary>
/// <remarks>
/// <para>
/// The manifest page proves the parameters match the Python version field for field. What it cannot
/// show is the behaviour behind them: that a tick is bound with the progress animation hidden, that
/// the functions a canvas declares are callable over the route the frontend uses, and that the
/// values of the two components whose shape is the application's cross the wire as they were given.
/// </para>
/// <para>
/// The browser half of the timer — that a tick really arrives at the interval the application set
/// — is in <see cref="BrowserEndToEndTests"/>, because only the frontend can start the clock.
/// </para>
/// </remarks>
public sealed class UtilityFamilyTests
{
    [Fact]
    public async Task The_utility_family_describes_itself_as_upstream_does()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Api
                {
                    Value = JsonValue.Create("hello"),
                    Schema = new JsonObject { ["type"] = "string" },
                    Label = "first",
                });

                blocks.Add(new Api
                {
                    Schema = new JsonObject { ["type"] = "number" },
                    Label = "second",
                });

                blocks.Add(new Fallback { Value = JsonValue.Create("x"), Label = "fb" });

                blocks.Add(new Fallback
                {
                    Value = new JsonObject { ["a"] = 1 },
                    Label = "fb2",
                    Info = "note",
                    Scale = 2,
                    MinWidth = 120,
                    Interactive = true,
                    ShowLabel = false,
                });

                blocks.Add(new Timer());

                blocks.Add(new Timer { Value = 5, Active = false });

                blocks.Add(new WorkflowCanvas());

                blocks.Add(new WorkflowCanvas
                {
                    Value = "{\"nodes\":[]}",
                    Label = "flow",
                    ShowLabel = true,
                    Container = true,
                    ServerFunctions = new Dictionary<string, Delegate>
                    {
                        ["describe"] = (Func<JsonNode?, JsonNode?>)(data => data?.DeepClone()),
                    },
                    AppView = "the-model",
                });
            }
        });

        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var components = Assert.IsType<JsonArray>(config["components"]);

        // An API component has none of the component base's layout parameters: its signature is a
        // value, a schema and a name, which is all the Python version's config carries for it. Its example
        // value is the value itself, and `...` when it has none.
        var first = components[0]!.AsObject();
        Assert.Equal("api", first["type"]!.GetValue<string>());
        Assert.False(first["skip_api"]!.GetValue<bool>());
        Assert.Equal("hello", first["example_inputs"]!.GetValue<string>());
        Assert.Equal("string", first["api_info"]!["type"]!.GetValue<string>());

        var firstProps = Props(components, id: 1);
        Assert.Equal("hello", firstProps["value"]!.GetValue<string>());
        Assert.Equal("string", firstProps["_api_info"]!["type"]!.GetValue<string>());
        Assert.Equal("first", firstProps["label"]!.GetValue<string>());
        Assert.Equal("api", firstProps["name"]!.GetValue<string>());
        Assert.False(firstProps["_selectable"]!.GetValue<bool>());
        Assert.False(firstProps.ContainsKey("show_label"));
        Assert.False(firstProps.ContainsKey("container"));

        var second = components[1]!.AsObject();
        Assert.Equal("...", second["example_inputs"]!.GetValue<string>());
        Assert.False(Props(components, id: 2).ContainsKey("value"));

        // A fallback's value is any JSON. The first carries the defaults, so its label is shown,
        // its container is on and its width is absent rather than a number.
        var fallbackProps = Props(components, id: 3);
        Assert.Equal("x", fallbackProps["value"]!.GetValue<string>());
        Assert.True(fallbackProps["show_label"]!.GetValue<bool>());
        Assert.True(fallbackProps["container"]!.GetValue<bool>());
        Assert.False(fallbackProps.ContainsKey("min_width"));
        Assert.False(fallbackProps.ContainsKey("info"));

        var described = components[2]!.AsObject();
        Assert.Equal("any valid json", described["api_info"]!["description"]!.GetValue<string>());
        Assert.Equal("bar", described["example_inputs"]!["foo"]!.GetValue<string>());

        // The second one asks for the parameters a default would not produce.
        var secondFallback = Props(components, id: 4);
        Assert.Equal(1, secondFallback["value"]!["a"]!.GetValue<int>());
        Assert.Equal("note", secondFallback["info"]!.GetValue<string>());
        Assert.False(secondFallback["show_label"]!.GetValue<bool>());
        Assert.Equal(2, secondFallback["scale"]!.GetValue<int>());
        Assert.Equal(120, secondFallback["min_width"]!.GetValue<int>());
        Assert.True(secondFallback["interactive"]!.GetValue<bool>());

        // A timer carries the interval and the flag, and nothing else: it has no label, no
        // container and no width to describe.
        var timer = Props(components, id: 5);
        Assert.Equal(1.0, timer["value"]!.GetValue<double>());
        Assert.True(timer["active"]!.GetValue<bool>());
        Assert.False(timer.ContainsKey("show_label"));
        Assert.False(timer.ContainsKey("container"));
        Assert.Equal(1, components[4]!["example_inputs"]!.GetValue<int>());
        Assert.Equal("number", components[4]!["api_info"]!["type"]!.GetValue<string>());

        var stopped = Props(components, id: 6);
        Assert.Equal(5.0, stopped["value"]!.GetValue<double>());
        Assert.False(stopped["active"]!.GetValue<bool>());

        // A canvas is skipped by the API, so its entry carries no schema and no example at all.
        var canvas = components[6]!.AsObject();
        Assert.True(canvas["skip_api"]!.GetValue<bool>());
        Assert.False(canvas.ContainsKey("api_info"));
        Assert.False(canvas.ContainsKey("example_inputs"));

        var canvasProps = Props(components, id: 7);
        Assert.False(canvasProps["show_label"]!.GetValue<bool>());
        Assert.False(canvasProps["container"]!.GetValue<bool>());
        Assert.Equal(
            ["value"],
            canvasProps["preserved_by_key"]!.AsArray().Select(item => item!.GetValue<string>()));
        Assert.False(canvasProps.ContainsKey("value"));
        Assert.False(canvasProps.ContainsKey("label"));
        Assert.False(canvasProps.ContainsKey("app_view"));
        Assert.False(canvasProps.ContainsKey("server_fns"));

        // The second one carries the workflow, the view it falls back to and the functions its
        // steps call, which the config names in `server_fns`.
        var configured = Props(components, id: 8);
        Assert.Equal("{\"nodes\":[]}", configured["value"]!.GetValue<string>());
        Assert.Equal("flow", configured["label"]!.GetValue<string>());
        Assert.True(configured["show_label"]!.GetValue<bool>());
        Assert.True(configured["container"]!.GetValue<bool>());
        Assert.Equal("the-model", configured["app_view"]!.GetValue<string>());
        Assert.Equal(
            ["describe"],
            configured["server_fns"]!.AsArray().Select(item => item!.GetValue<string>()));

        // The Python version's canvas is a container, so its layout node carries an empty children list; an
        // API component is not, so its node carries none.
        var layout = Assert.IsType<JsonArray>(config["layout"]!["children"]);
        Assert.Empty(Assert.IsType<JsonArray>(layout.Single(node => node!["id"]!.GetValue<int>() == 7)!["children"]));
        Assert.Empty(Assert.IsType<JsonArray>(layout.Single(node => node!["id"]!.GetValue<int>() == 8)!["children"]));
        Assert.False(layout.Single(node => node!["id"]!.GetValue<int>() == 1)!.AsObject().ContainsKey("children"));
    }

    [Fact]
    public async Task A_tick_shows_no_progress_and_a_change_shows_all_of_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var timer = blocks.Add(new Timer { Value = 0.5 });
                var output = blocks.Add(new Textbox { Label = "Ticks" });

                var tick = timer.Tick(
                    () => "tick",
                    [output],
                    apiName: "tick");

                timer.Change(
                    value => value,
                    [timer],
                    [output],
                    apiName: "changed");

                // A trigger merged through Blocks.On is a generic binding: the Python version's gr.on has its
                // own default and never reads the event's, so a tick bound this way shows all of it.
                blocks.On(
                    [timer.Events.Tick],
                    fn: (Func<object?>)(() => "merged"), outputs: [output], apiName: "merged");

                // A binding that cancels the tick registers a second, private binding; that one is
                // the Python version's own and carries no progress setting, so it always shows all of it.
                var stopper = blocks.Add(new Button { Value = "Stop" });

                stopper.Click(
                    () => null,
                    [],
                    apiName: "stop", cancels: [tick]);
            }
        });

        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var dependencies = Assert.IsType<JsonArray>(config["dependencies"]);

        Assert.Equal(5, dependencies.Count);

        // The Python version declares the default on the event: a tick hides the progress animation, because
        // a timer that fires every half second would otherwise repaint a spinner every half second.
        Assert.Equal("hidden", dependencies[0]!["show_progress"]!.GetValue<string>());
        Assert.Equal("tick", dependencies[0]!["targets"]![0]![1]!.GetValue<string>());
        Assert.True(dependencies[0]!["queue"]!.GetValue<bool>());

        // Everything else shows all of it, including the same signal merged into Blocks.On.
        Assert.Equal("full", dependencies[1]!["show_progress"]!.GetValue<string>());
        Assert.Equal("change", dependencies[1]!["targets"]![0]![1]!.GetValue<string>());
        Assert.Equal("full", dependencies[2]!["show_progress"]!.GetValue<string>());
        Assert.Equal("tick", dependencies[2]!["targets"]![0]![1]!.GetValue<string>());

        // The cancellation binding is the last one, and it names what it stops.
        Assert.True(dependencies[4]!["types"]!["cancel"]!.GetValue<bool>());
        Assert.Equal("full", dependencies[4]!["show_progress"]!.GetValue<string>());
        Assert.Equal(0, dependencies[4]!["cancels"]![0]!.GetValue<int>());
    }

    [Fact]
    public async Task A_workflow_canvas_calls_the_functions_the_application_registered()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new WorkflowCanvas
                {
                    ServerFunctions = new Dictionary<string, Delegate>
                    {
                        ["describe"] = (Func<JsonNode?, JsonNode?>)(data => data?.DeepClone()),
                    },
                });
            }
        });

        using var response = await host.Client.PostAsync(
            "/gradio_api/component_server",
            new StringContent(
                """{"session_hash":"utility","component_id":1,"fn_name":"describe","data":{"answer":42}}""",
                Encoding.UTF8,
                "application/json"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            """{"answer":42}""",
            (await response.Content.ReadAsStringAsync(cancellationToken)).Trim());
    }

    [Fact]
    public async Task An_api_value_and_a_fallback_value_cross_the_wire_as_json()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var api = blocks.Add(new Api
                {
                    Schema = new JsonObject { ["type"] = "object" },
                    Label = "count",
                });

                var fallback = blocks.Add(new Fallback());
                var button = blocks.Add(new Button { Value = "Go" });

                // An API component's value is described by its schema rather than by a component's
                // shape, and a fallback's is the application's: what a handler reads and what the
                // client gets back are the same JSON, through both components.
                button.Click(
                    (Func<object?, object?>)(value => new Dictionary<string, object?> { ["got"] = value }),
                    inputs: [api],
                    outputs: [fallback],
                     apiName: "echo");
            }
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);

        await queue.JoinAsync(
            fnIndex,
            new JsonArray(new JsonObject { ["a"] = 1 }),
            "api",
            cancellationToken);

        await using var stream = await queue.OpenAsync("api", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var completed = stream.Messages.Last(message => QueueSession.Kind(message) == "process_completed");

        // The handler read the value as CLR values and returned a dictionary; the fallback wrote it
        // as the JSON object it is rather than as a type name.
        Assert.Equal(1, completed["output"]!["data"]![0]!["got"]!["a"]!.GetValue<int>());
    }

    private static JsonObject Props(JsonArray components, int id) =>
        components
            .Single(component => component!["id"]!.GetValue<int>() == id)!
            ["props"]!
            .AsObject();

    private static async Task<JsonObject> GetDocumentAsync(
        GradioTestHost host,
        string path,
        CancellationToken cancellationToken)
    {
        using var response = await host.Client.GetAsync(path, cancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsObject();
    }
}

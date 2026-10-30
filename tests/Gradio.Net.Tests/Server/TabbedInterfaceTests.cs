using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Several applications behind one tab bar: what the composition draws, what it says it is, and
/// which blocks end up in which tab.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the shape of the composition: the names a tab gets when the
/// application names none, what happens when fewer names than applications are given, and what the
/// composition says about itself when the interfaces it holds say something else.
/// </para>
/// <para>
/// The interfaces are built by the tests themselves rather than shared, because each test asks a
/// different question about the tree, and a composition is rendered once: it draws the application
/// it is constructed against.
/// </para>
/// </remarks>
public sealed class TabbedInterfaceTests
{
    [Fact]
    public async Task A_tabbed_interface_describes_itself_as_one()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [new TabContent(tab => Greet(tab, "One")), new TabContent(tab => Greet(tab, "Two"))],
                tabNames: ["First", "Second"],
                title: "Compare"));

        var config = await ConfigAsync(host);

        Assert.Equal("tabbed_interface", config["mode"]!.GetValue<string>());
        Assert.Equal("Compare", config["title"]!.GetValue<string>());

        // The Python version fills the height with a tab bar and does not fill the width, whatever the
        // interfaces it holds asked for.
        Assert.True(config["fill_height"]!.GetValue<bool>());
        Assert.False(config["fill_width"]!.GetValue<bool>());

        // The heading is the composition's, drawn above the tab bar in the Python version's own markup.
        var first = config["components"]!.AsArray()[0]!;
        Assert.Equal("markdown", first["type"]!.GetValue<string>());
        Assert.Equal(
            "<h1 style='text-align: center; margin-bottom: 1rem'>Compare</h1>",
            first["props"]!["value"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_application_has_no_description_of_its_own()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [new TabContent(tab => Greet(tab, "One", description: "Say hello"))],
                title: "Compare"));

        using var response = await host.Client.GetAsync(
            "/gradio_api/openapi.json",
            TestContext.Current.CancellationToken);

        var openApi = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        // The description belongs to an interface, and the interfaces here are the tabs rather than
        // the application, so the composed one has none — which the Python version serves as an empty string.
        Assert.Equal("Compare", openApi["info"]!["title"]!.GetValue<string>());
        Assert.Equal(string.Empty, openApi["info"]!["description"]!.GetValue<string>());
    }

    [Fact]
    public async Task Each_interface_is_drawn_inside_its_own_tab()
    {
        Textbox firstInput = new();
        Textbox secondInput = new();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, string>)(value => value),
                        [firstInput],
                        [new Textbox()])),
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, string>)(value => value),
                        [secondInput],
                        [new Textbox()])),
                ],
                tabNames: ["First", "Second"]));

        var config = await ConfigAsync(host);
        var layout = config["page"]![""]!["layout"]!;

        // The page holds the tab bar, and the tab bar holds one tab per interface.
        var bar = Assert.Single(layout["children"]!.AsArray());
        var tabs = bar!["children"]!.AsArray();
        Assert.Equal(2, tabs.Count);

        Assert.Equal(
            ["First", "Second"],
            tabs.Select(tab => EntryOf(config, tab!["id"]!.GetValue<int>())["props"]!["label"]!.GetValue<string>()));

        // Each tab holds the layout its own interface drew — a row with two columns — and the
        // fields of one interface are in the tab of that interface and in no other.
        var firstTab = tabs[0]!["children"]!.AsArray();
        var secondTab = tabs[1]!["children"]!.AsArray();

        Assert.Equal("row", EntryOf(config, firstTab[0]!["id"]!.GetValue<int>())["type"]!.GetValue<string>());
        Assert.Equal("row", EntryOf(config, secondTab[0]!["id"]!.GetValue<int>())["type"]!.GetValue<string>());

        Assert.Contains(firstInput.Id, IdsUnder(firstTab));
        Assert.DoesNotContain(firstInput.Id, IdsUnder(secondTab));
        Assert.Contains(secondInput.Id, IdsUnder(secondTab));
        Assert.DoesNotContain(secondInput.Id, IdsUnder(firstTab));
    }

    [Fact]
    public async Task A_tab_without_a_name_is_named_after_its_position_from_zero()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [new TabContent(tab => Greet(tab, "One")), new TabContent(tab => Greet(tab, "Two"))]));

        var config = await ConfigAsync(host);

        // The Python version numbers the default names from zero, and the name is served, so "Tab 0" is the
        // first tab rather than a slip.
        Assert.Equal(
            ["Tab 0", "Tab 1"],
            TabItemsOf(config).Select(entry => entry["props"]!["label"]!.GetValue<string>()));
    }

    [Fact]
    public async Task Fewer_names_than_interfaces_draws_fewer_tabs()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => Greet(tab, "One")),
                    new TabContent(tab => Greet(tab, "Two")),
                ],
                tabNames: ["Only"]));

        var config = await ConfigAsync(host);

        // The Python version zips the two lists, so the shorter one decides how many tabs there are: an
        // application left without a name is not drawn under a default one.
        Assert.Single(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "tabitem");

        Assert.DoesNotContain(
            config["components"]!.AsArray(),
            entry => entry!["props"]!["label"]?.GetValue<string>() == "Two");
    }

    [Fact]
    public async Task A_tab_whose_content_fills_the_height_is_given_a_scale()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => Greet(tab, "One", fillHeight: true)),
                    new TabContent(tab => Greet(tab, "Two")),
                ],
                tabNames: ["Tall", "Short"]));

        var config = await ConfigAsync(host);
        var tabs = TabItemsOf(config);

        // The Python version gives a tab a scale of 1 when its interface fills the height and none when it
        // does not, which is how the tab bar shares the height between them. The interface says so
        // where it is built, and the tab reports what it said — the second tab inherits nothing
        // from the first, even though the height was set on the application in between.
        Assert.Equal(1, tabs[0]!["props"]!["scale"]!.GetValue<int>());
        Assert.Null(tabs[1]!["props"]!["scale"]);
    }

    [Fact]
    public async Task A_tab_is_sized_by_what_its_own_content_says()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => _ = new ChatInterface(
                        tab,
                        (Func<string, IReadOnlyList<ChatMessage>, string>)((message, history) => "hi"))),
                    new TabContent(tab => tab.Add(new Textbox())),
                ],
                tabNames: ["Chat", "Plain"]));

        var config = await ConfigAsync(host);
        var tabs = TabItemsOf(config);

        // A chat fills the height unless it is told not to, and the Python version's tabbed interface reads
        // that off the application it holds — so a tab holding a chat is given a scale of 1 without
        // the composition being told anything. The tab beside it draws plain blocks, which say
        // nothing about the height, and it is given none: what the tab before it said is not
        // inherited, because the application's value is cleared before each tab is drawn.
        Assert.Equal(1, tabs[0]!["props"]!["scale"]!.GetValue<int>());
        Assert.Null(tabs[1]!["props"]!["scale"]);
    }

    [Fact]
    public async Task A_tab_can_state_its_own_scale_instead_of_asking_its_content()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => Greet(tab, "One"), fillHeight: true),
                    new TabContent(tab => Greet(tab, "Two", fillHeight: true), fillHeight: false),
                ],
                tabNames: ["Tall", "Short"]));

        var config = await ConfigAsync(host);
        var tabs = TabItemsOf(config);

        // A tab's content is a recipe, so the application says what it cannot ask: a stated value
        // decides the scale in both directions, and only content that says nothing about the height
        // is asked.
        Assert.Equal(1, tabs[0]!["props"]!["scale"]!.GetValue<int>());
        Assert.Null(tabs[1]!["props"]!["scale"]);
    }

    [Fact]
    public async Task The_tab_bar_takes_the_parameters_the_application_gave_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [new TabContent(tab => Greet(tab, "One")), new TabContent(tab => Greet(tab, "Two"))],
                tabNames: ["First", "Second"],
                tabs: new Tabs
                {
                    OverflowBehavior = "wrap",
                    Selected = 1,
                    ElemId = "the-tabs",
                }));

        var config = await ConfigAsync(host);
        var bar = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "tabs")!;

        Assert.Equal("wrap", bar["props"]!["overflow_behavior"]!.GetValue<string>());
        Assert.Equal(1, bar["props"]!["selected"]!.GetValue<int>());
        Assert.Equal("the-tabs", bar["props"]!["elem_id"]!.GetValue<string>());
    }

    [Fact]
    public async Task Both_interfaces_keep_their_endpoints()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => Greet(tab, "One", apiName: "greet")),
                    new TabContent(tab => Greet(tab, "Two", apiName: "farewell")),
                ],
                tabNames: ["First", "Second"]));

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var info = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;
        var endpoints = info["named_endpoints"]!.AsObject().Select(endpoint => endpoint.Key).ToArray();

        // A tab's content is an application, and the endpoint it binds belongs to the composition
        // it was drawn into rather than disappearing inside the tab.
        Assert.Equal(["/greet", "/farewell"], endpoints);
    }

    [Fact]
    public async Task Each_tab_answers_with_its_own_handler()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, string>)(value => $"Hello {value}"),
                        [new Textbox()],
                        [new Textbox()],
                        apiName: "greet")),
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, double, string>)((value, times) =>
                            "Bye " + string.Join(' ', Enumerable.Repeat(value, (int)times))),
                        [new Textbox(), new Number()],
                        [new Textbox()],
                        apiName: "farewell")),
                ],
                tabNames: ["First", "Second"]));

        // Each endpoint runs the handler bound inside its own tab, against that tab's own
        // components: the second takes two inputs where the first takes one, so a build that
        // merged the two applications into one binding would answer neither.
        Assert.Equal("[\"Hello Bob\"]", (await RunAsync(host, "greet", new JsonArray("Bob"))).ToJsonString());
        Assert.Equal(
            "[\"Bye Bob Bob\"]",
            (await RunAsync(host, "farewell", new JsonArray("Bob", 2))).ToJsonString());
    }

    [Fact]
    public async Task Two_tabs_that_name_the_same_endpoint_are_given_separate_names()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, string>)(value => $"Hello {value}"),
                        [new Textbox()],
                        [new Textbox()],
                        apiName: "predict")),
                    new TabContent(tab => _ = new Interface(
                        tab,
                        (Func<string, string>)(value => $"Bye {value}"),
                        [new Textbox()],
                        [new Textbox()],
                        apiName: "predict")),
                ],
                tabNames: ["First", "Second"]));

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        var info = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        // The Python version's tabs are applications of their own, and each makes its names unique among
        // the ones already served when it is merged into the tabbed one, so the second tab's
        // "predict" is served as "predict_1" rather than disappearing behind the first.
        Assert.Equal(
            ["/predict", "/predict_1"],
            info["named_endpoints"]!.AsObject().Select(endpoint => endpoint.Key).ToArray());

        // The rename is the name and nothing else: each one still answers the handler it was
        // bound to inside its own tab.
        Assert.Equal("[\"Hello Bob\"]", (await RunAsync(host, "predict", new JsonArray("Bob"))).ToJsonString());
        Assert.Equal("[\"Bye Bob\"]", (await RunAsync(host, "predict_1", new JsonArray("Bob"))).ToJsonString());
    }

    [Fact]
    public async Task No_names_at_all_draws_no_tabs()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [new TabContent(tab => Greet(tab, "One")), new TabContent(tab => Greet(tab, "Two"))],
                tabNames: [],
                title: "Compare"));

        var config = await ConfigAsync(host);

        // The Python version zips the applications with the names, so an empty list of names is no tabs at
        // all — the other end of "fewer names than applications". The heading and the bar are
        // drawn either way, and no application is drawn, so the composition serves no endpoint.
        Assert.Empty(TabItemsOf(config));
        Assert.Contains(
            config["components"]!.AsArray(),
            entry => entry!["props"]!["value"]?.GetValue<string>()
                == "<h1 style='text-align: center; margin-bottom: 1rem'>Compare</h1>");
        Assert.Empty(config["dependencies"]!.AsArray());
    }

    [Fact]
    public async Task The_composition_says_what_the_application_is_not_the_interfaces_it_holds()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new TabbedInterface(
                blocks,
                [
                    new TabContent(tab => Greet(tab, "Greeter")),
                    new TabContent(tab => Greet(tab, "Farewell")),
                ],
                tabNames: ["First", "Second"],
                title: "Compare"));

        var config = await ConfigAsync(host);

        // Each interface claims the application it is built into — that is how it works when it is
        // built alone — so the composition has the last word about what it is.
        Assert.Equal("tabbed_interface", config["mode"]!.GetValue<string>());
        Assert.Equal("Compare", config["title"]!.GetValue<string>());

        // The interfaces' own headings are still drawn, inside their tabs.
        Assert.Equal(
            ["<h1 style='text-align: center; margin-bottom: 1rem'>Compare</h1>",
             "<h1 style='text-align: center; margin-bottom: 1rem'>Greeter</h1>",
             "<h1 style='text-align: center; margin-bottom: 1rem'>Farewell</h1>"],
            config["components"]!.AsArray()
                .Where(entry => entry!["type"]!.GetValue<string>() == "markdown"
                    && entry["props"]!["value"]!.GetValue<string>().StartsWith("<h1", StringComparison.Ordinal))
                .Select(entry => entry!["props"]!["value"]!.GetValue<string>()));
    }

    [Fact]
    public async Task A_tab_does_not_hand_the_application_its_own_cache_and_telemetry_settings()
    {
        Blocks? application = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                application = blocks;

                // What the caller told the application before it was built — the Python version's
                // analytics_enabled on the tabbed interface itself.
                blocks.AnalyticsEnabled = true;

                _ = new TabbedInterface(
                    blocks,
                    [
                        new TabContent(tab => _ = new Interface(
                            tab,
                            (Func<string, string>)(value => value),
                            [new Textbox()],
                            [new Textbox()],
                            deleteCache: new DeleteCacheSettings(
                                TimeSpan.FromMinutes(1),
                                TimeSpan.FromHours(1)))),
                    ],
                    tabNames: ["First"]);
            });

        // delete_cache and analytics are the application's settings, and a tab's content is an
        // application of its own the Python version: it sets them on itself and never on the tabbed one. So
        // the composition keeps what it was told and drops what a tab said — a tab's cache schedule
        // would otherwise start reclaiming the whole application's temporary files.
        Assert.Null(application!.DeleteCache);
        Assert.True(application.AnalyticsEnabled);
    }

    [Fact]
    public void An_application_is_required()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TabbedInterface(
            null!,
            [new TabContent(tab => Greet(tab, "One"))]));
    }

    [Fact]
    public void The_interfaces_are_required()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Assert.Throws<ArgumentNullException>(() => _ = new TabbedInterface(blocks, null!));
    }

    [Fact]
    public void A_tab_with_no_content_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var error = Assert.Throws<ArgumentException>(() => _ = new TabbedInterface(
            blocks,
            [new TabContent(tab => Greet(tab, "One")), null!]));

        Assert.Equal("contents", error.ParamName);
    }

    /// <summary>Draws one interface whose heading and description are its own.</summary>
    /// <param name="blocks">The application being built.</param>
    /// <param name="heading">The heading, or <see langword="null"/> for none.</param>
    /// <param name="description">The description, or <see langword="null"/> for none.</param>
    /// <param name="apiName">The endpoint's name.</param>
    /// <param name="fillHeight">Whether the interface fills the height of the page.</param>
    /// <returns>The interface, for the tests that want to look at what it built.</returns>
    private static Interface Greet(
        Blocks blocks,
        string? heading,
        string? description = null,
        string? apiName = null,
        bool fillHeight = false) =>
        new(
            blocks,
            (Func<string, string>)(value => value),
            [new Textbox()],
            [new Textbox()],
            title: heading,
            description: description,
            apiName: apiName,
            fillHeight: fillHeight);

    /// <summary>The config entry for a block id.</summary>
    /// <param name="config">The config document.</param>
    /// <param name="id">The id.</param>
    /// <returns>The entry.</returns>
    private static JsonNode EntryOf(JsonObject config, int id) =>
        config["components"]!.AsArray().Single(entry => entry!["id"]!.GetValue<int>() == id)!;

    /// <summary>The tabs the config lists, in the order they are drawn.</summary>
    /// <param name="config">The config document.</param>
    /// <returns>The entries, whose <c>props</c> hold the label and the scale.</returns>
    private static JsonNode[] TabItemsOf(JsonObject config) =>
        [.. config["components"]!.AsArray()
            .Where(entry => entry!["type"]!.GetValue<string>() == "tabitem")
            .Select(entry => entry!)];

    /// <summary>Every block id under a layout node, the node itself included.</summary>
    /// <param name="nodes">The layout nodes to walk.</param>
    /// <returns>The ids.</returns>
    private static List<int> IdsUnder(JsonArray nodes)
    {
        var ids = new List<int>();
        foreach (var node in nodes)
        {
            Collect(node!, ids);
        }

        return ids;
    }

    private static void Collect(JsonNode node, List<int> ids)
    {
        if (node["id"] is JsonValue value && value.TryGetValue<int>(out var id))
        {
            ids.Add(id);
        }

        if (node["children"] is not JsonArray children)
        {
            return;
        }

        foreach (var child in children)
        {
            Collect(child!, ids);
        }
    }

    /// <summary>Reads the config the application serves.</summary>
    /// <param name="host">The host.</param>
    /// <returns>The document.</returns>
    private static async Task<JsonObject> ConfigAsync(GradioTestHost host)
    {
        using var response = await host.Client.GetAsync(
            "/config",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();
    }

    /// <summary>Calls one endpoint with a set of values, as a client would.</summary>
    /// <param name="host">The host.</param>
    /// <param name="apiName">The endpoint's name.</param>
    /// <param name="data">One value per input.</param>
    /// <returns>One value per output.</returns>
    private static async Task<JsonArray> RunAsync(GradioTestHost host, string apiName, JsonArray data)
    {
        using var response = await host.Client.PostAsync(
            $"/gradio_api/run/{apiName}",
            new StringContent(
                new JsonObject { ["data"] = data }.ToJsonString(),
                Encoding.UTF8,
                "application/json"),
            TestContext.Current.CancellationToken);

        var answer = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"'{apiName}' answered {response.StatusCode}: {answer.ToJsonString()}");

        return answer["data"]!.AsArray();
    }
}

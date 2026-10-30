using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The display family as an application uses it: a Markdown that cleans what it is given, a
/// dialogue that converts between a transcript and its turns, and an HTML component that serves the
/// functions an application registered on it.
/// </summary>
/// <remarks>
/// <para>
/// The manifest page proves the parameters match the Python version field for field. What it cannot
/// show is the behaviour behind them — the text the Python version runs <c>inspect.cleandoc</c> on
/// before it reaches the config, and the two conversions a dialogue offers the frontend — because
/// that page binds nothing and calls nothing.
/// </para>
/// <para>
/// The server functions are called over HTTP, the way the browser calls them, so that what is
/// asserted is the route and the JSON it answers rather than a method invoked in-process.
/// </para>
/// </remarks>
public sealed class DisplayFamilyTests
{
    [Fact]
    public void Markdown_cleans_its_text_the_way_upstream_does()
    {
        // The Python version's constructor runs the value through `inspect.cleandoc`: common indentation is
        // stripped, tabs expand to the next tab stop, and surrounding blank lines go. A template
        // written as an indented literal would otherwise reach the frontend with its indentation.
        var markdown = new Markdown { Value = "    indented\n      more\n" };

        Assert.Equal("indented\nmore", markdown.Value);
        Assert.Equal("a\nb", markdown.Postprocess("  a\n    b\n")!.GetValue<string>());
        Assert.Null(markdown.Postprocess(null));
    }

    [Fact]
    public async Task The_display_family_describes_itself_as_upstream_does()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Markdown { Label = "Notes" });
                blocks.Add(new Dialogue { Label = "Talk" });
            }
        });

        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var components = Assert.IsType<JsonArray>(config["components"]);

        // Markdown is drawn outside a container, which is also why it shows no label unless asked:
        // the Python version's own defaults, not this framework's.
        var markdown = Props(components, id: 1);
        Assert.False(markdown["container"]!.GetValue<bool>());
        Assert.False(markdown["show_label"]!.GetValue<bool>());
        Assert.False(markdown["line_breaks"]!.GetValue<bool>());
        Assert.False(markdown["header_links"]!.GetValue<bool>());

        // The two defaults that are absent rather than empty: the Python version writes no width and no
        // button list for a Markdown that was given neither.
        Assert.False(markdown.ContainsKey("min_width"));
        Assert.False(markdown.ContainsKey("buttons"));

        var delimiters = Assert.IsType<JsonArray>(markdown["latex_delimiters"]);
        var delimiter = Assert.Single(delimiters)!.AsObject();
        Assert.Equal("$$", delimiter["left"]!.GetValue<string>());
        Assert.Equal("$$", delimiter["right"]!.GetValue<string>());
        Assert.True(delimiter["display"]!.GetValue<bool>());

        Assert.Equal(
            ["value"],
            markdown["preserved_by_key"]!.AsArray().Select(item => item!.GetValue<string>()));

        var dialogue = Props(components, id: 2);
        Assert.Equal("text", dialogue["type"]!.GetValue<string>());
        Assert.Equal("\n", dialogue["separator"]!.GetValue<string>());
        Assert.False(dialogue["submit_btn"]!.GetValue<bool>());
        Assert.Equal("both", dialogue["ui_mode"]!.GetValue<string>());
        Assert.Equal("Talk", dialogue["label"]!.GetValue<string>());
        Assert.True(dialogue["show_label"]!.GetValue<bool>());
        Assert.True(dialogue["autoscroll"]!.GetValue<bool>());
        Assert.False(dialogue["autofocus"]!.GetValue<bool>());
        Assert.Equal(160, dialogue["min_width"]!.GetValue<int>());
        Assert.Empty(Assert.IsType<JsonArray>(dialogue["buttons"]));

        // A dialogue without speakers carries no list at all, and one the user cannot edit carries
        // no note: the Python version clears the note of a non-interactive dialogue, default hint included.
        Assert.False(dialogue.ContainsKey("speakers"));
        Assert.False(dialogue.ContainsKey("info"));

        Assert.Equal(
            ["format", "unformat"],
            dialogue["server_fns"]!.AsArray().Select(item => item!.GetValue<string>()));

        var info = Assert.IsType<JsonObject>(components[1]!["api_info"]);
        Assert.Equal("DialogueModel", info["title"]!.GetValue<string>());
        Assert.Equal(2, Assert.IsType<JsonArray>(info["anyOf"]).Count);
        Assert.NotNull(info["$defs"]!["DialogueLine"]);

        var example = Assert.IsType<JsonArray>(components[1]!["example_inputs"]);
        Assert.Equal(2, example.Count);
        Assert.Equal("Speaker 1", example[0]!["speaker"]!.GetValue<string>());

        // Markdown's example is a string, which is what its api_info says it is.
        Assert.Equal("# Hello!", components[0]!["example_inputs"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_dialogue_converts_a_transcript_through_its_server_functions()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Dialogue());
            }
        });

        // Formatting: the turns become the transcript the reader copies, joined by the separator.
        var formatted = await CallServerFunctionAsync(
            host,
            """{"session_hash":"talk","component_id":1,"fn_name":"format","data":[{"speaker":"A","text":"hi"},{"speaker":"B","text":"yo"}]}""",
            cancellationToken);

        Assert.Equal("[A] hi\n[B] yo", formatted.GetValue<string>());

        // Unformatting: the transcript is parsed back into turns, and a line without brackets goes
        // to the speaker the Python version falls back on when the component names none.
        var unformatted = await CallServerFunctionAsync(
            host,
            """{"session_hash":"talk","component_id":1,"fn_name":"unformat","data":{"text":"[A] hi\n[B] yo\nplain"}}""",
            cancellationToken);

        Assert.Equal(
            """[{"speaker":"A","text":"hi"},{"speaker":"B","text":"yo"},{"speaker":"Unknown","text":"plain"}]""",
            unformatted.ToJsonString());
    }

    [Fact]
    public async Task An_html_component_serves_the_functions_the_application_registered()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Html
                {
                    Value = "<p>custom</p>",
                    ServerFunctions = new Dictionary<string, Delegate>
                    {
                        ["echo"] = (Func<JsonNode?, JsonNode?>)(data => data?.DeepClone()),

                        // A function an application wrote asynchronously, which is how the Python version's
                        // own server functions are written: the task is awaited rather than
                        // serialized.
                        ["later"] = (Func<JsonNode?, Task<JsonNode?>>)(async data =>
                        {
                            await Task.Yield();

                            return data?.DeepClone();
                        }),
                    },
                });
            }
        });

        // The names travel in server_fns, so the frontend knows what it may call.
        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var props = Props(Assert.IsType<JsonArray>(config["components"]), id: 1);
        Assert.Equal(
            ["echo", "later"],
            props["server_fns"]!.AsArray().Select(item => item!.GetValue<string>()));

        var answer = await CallServerFunctionAsync(
            host,
            """{"session_hash":"custom","component_id":1,"fn_name":"echo","data":{"answer":42}}""",
            cancellationToken);

        Assert.Equal("""{"answer":42}""", answer.ToJsonString());

        // The asynchronous function answers with its result, not with a serialized task.
        var later = await CallServerFunctionAsync(
            host,
            """{"session_hash":"custom","component_id":1,"fn_name":"later","data":{"answer":43}}""",
            cancellationToken);

        Assert.Equal("""{"answer":43}""", later.ToJsonString());
    }

    [Fact]
    public async Task An_html_component_carries_its_template_styles_script_and_head()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Html
                {
                    Value = "<p>custom</p>",
                    HtmlTemplate = "<div class=\"box\">${value}</div>",
                    CssTemplate = ".box { padding: 4px; }",
                    JsOnLoad = "element.addEventListener('click', () => trigger('click'));",
                    Head = "<script src=\"https://example.com/widget.js\"></script>",
                    ApplyDefaultCss = false,
                    Padding = true,
                    Autoscroll = true,
                    MinHeight = 80,
                    MaxHeight = 200,
                });
            }
        });

        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var props = Props(Assert.IsType<JsonArray>(config["components"]), id: 1);

        Assert.Equal("<div class=\"box\">${value}</div>", props["html_template"]!.GetValue<string>());
        Assert.Equal(".box { padding: 4px; }", props["css_template"]!.GetValue<string>());
        Assert.Equal(
            "element.addEventListener('click', () => trigger('click'));",
            props["js_on_load"]!.GetValue<string>());
        Assert.Equal(
            "<script src=\"https://example.com/widget.js\"></script>",
            props["head"]!.GetValue<string>());
        Assert.False(props["apply_default_css"]!.GetValue<bool>());
        Assert.True(props["padding"]!.GetValue<bool>());
        Assert.True(props["autoscroll"]!.GetValue<bool>());
        Assert.Equal(80, props["min_height"]!.GetValue<int>());
        Assert.Equal(200, props["max_height"]!.GetValue<int>());

        // Nothing declared the functions, so the component carries no list of them at all.
        Assert.False(props.ContainsKey("server_fns"));
    }

    [Fact]
    public async Task A_component_the_application_defines_is_drawn_as_html_under_its_own_name()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Callout
                {
                    Value = "Everything is fine.",
                    Heading = "Status",
                    Tone = "warning",
                    Label = "Note",
                });
            }
        });

        var config = await GetDocumentAsync(host, "/config", cancellationToken);
        var component = Assert.IsType<JsonArray>(config["components"])[0]!.AsObject();

        // The frontend draws a subclass with the HTML component, so `type` stays `html`; what
        // names the class is the props' `name` and `component_class_name`.
        Assert.Equal("html", component["type"]!.GetValue<string>());

        var props = component["props"]!.AsObject();
        Assert.Equal("callout", props["name"]!.GetValue<string>());
        Assert.Equal("Callout", props["component_class_name"]!.GetValue<string>());

        // The subclass's own parameters are written beside the HTML component's — which is where
        // the Python version writes a subclass's constructor parameters — and into the dictionary the
        // template reads, which is what makes them visible to the template.
        Assert.Equal("warning", props["tone"]!.GetValue<string>());
        Assert.Equal("Status", props["heading"]!.GetValue<string>());

        var extra = Assert.IsType<JsonObject>(props["props"]);
        Assert.Equal("warning", extra["tone"]!.GetValue<string>());
        Assert.Equal("Status", extra["heading"]!.GetValue<string>());

        // The templates the subclass declared travel with it, and so does its value.
        Assert.Contains("callout", props["html_template"]!.GetValue<string>(), StringComparison.Ordinal);
        Assert.Equal("Everything is fine.", props["value"]!.GetValue<string>());
    }

    [Fact]
    public void A_dialogue_reads_and_writes_its_turns()
    {
        var turns = new JsonArray(new JsonObject { ["speaker"] = "A", ["text"] = "hi" });

        // A `list` dialogue hands the handler the turns, and a text one hands it the transcript:
        // the Python version's preprocess formats the turns with the formatter and joins them.
        var list = new Dialogue { Type = "list" };
        var read = Assert.IsType<DialogueTurn[]>(list.Preprocess(turns));
        Assert.Equal(new DialogueTurn("A", "hi"), Assert.Single(read));

        var text = new Dialogue();
        Assert.Equal("[A] hi", text.Preprocess(turns));

        // A handler answers with turns — which is what a transcription produces — or with the
        // documents it was given; either way the frontend reads the same objects.
        const string Written = """[{"speaker":"A","text":"hi"}]""";
        Assert.Equal(
            Written,
            list.Postprocess(new[] { new DialogueTurn("A", "hi") })!.ToJsonString());
        Assert.Equal(Written, list.Postprocess(turns)!.ToJsonString());
        Assert.Equal("hi", list.Postprocess("hi")!.GetValue<string>());
        Assert.Null(list.Postprocess(null));

        // A value of the wrong shape is refused rather than read as an empty dialogue, which is
        // what the Python version's model validation does with it.
        Assert.Throws<ArgumentException>(() => list.Preprocess("hi"));
        Assert.Throws<NotSupportedException>(() => list.Postprocess(42));
    }

    private static JsonObject Props(JsonArray components, int id) =>
        components
            .Single(component => component!["id"]!.GetValue<int>() == id)!
            ["props"]!
            .AsObject();

    private static async Task<JsonNode> CallServerFunctionAsync(
        GradioTestHost host,
        string body,
        CancellationToken cancellationToken)
    {
        using var response = await host.Client.PostAsync(
            "/gradio_api/component_server",
            new StringContent(body, Encoding.UTF8, "application/json"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;
    }

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

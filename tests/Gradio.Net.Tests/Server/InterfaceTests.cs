using System.Net;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// An application built in one call: what it draws, what it binds, and what its buttons do when
/// they are used.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the behaviour a document cannot show: that a flagged sample reaches a
/// file, that a cached answer is worked out when the mode says so, that a share link opens the
/// application where it was left, and that a validator stops a run before the handler sees it.
/// </para>
/// <para>
/// The shapes are asserted one per test because they are what an application chooses between: an
/// interface with no outputs is not a standard one with the right column hidden, it is a different
/// set of buttons and bindings.
/// </para>
/// </remarks>
public sealed class InterfaceTests : IDisposable
{
    /// <summary>The labels the four components of the labelling case are given.</summary>
    private static readonly string?[] ExpectedLabels = ["name", "age", "output 0", "output 1"];

    /// <summary>The directory this class's applications keep their files in.</summary>
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"gradio-interface-{Guid.NewGuid():N}");

    /// <inheritdoc />
    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    [Fact]
    public async Task An_interface_describes_itself_as_one()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox()],
                [new Textbox()],
                title: "Greeter",
                description: "Say hello",
                fillWidth: true)));

        var config = await ConfigAsync(host);

        Assert.Equal("interface", config["mode"]!.GetValue<string>());
        Assert.Equal("Greeter", config["title"]!.GetValue<string>());
        Assert.True(config["fill_width"]!.GetValue<bool>());

        // The heading and the description are drawn as Markdown above the columns, and the article —
        // when there is one — below the gallery.
        var first = config["components"]!.AsArray()[0]!;
        Assert.Equal("markdown", first["type"]!.GetValue<string>());
        Assert.Equal(
            "<h1 style='text-align: center; margin-bottom: 1rem'>Greeter</h1>",
            first["props"]!["value"]!.GetValue<string>());

        // The openapi document says what the interface said it was for.
        using var response = await host.Client.GetAsync(
            "/gradio_api/openapi.json",
            TestContext.Current.CancellationToken);

        var openApi = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal("Say hello", openApi["info"]!["description"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_components_are_numbered_before_the_layout_that_holds_them()
    {
        Textbox input = new();
        Textbox output = new();
        Interface? ui = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [input],
                [output])));

        var config = await ConfigAsync(host);
        var layout = config["page"]![""]!["layout"]!;

        // The Python version numbers a component when it is constructed and an interface constructs its
        // components before its layout, so the fields come first and the columns after them.
        Assert.Equal(1, input.Id);
        Assert.Equal(2, output.Id);
        Assert.True(ui!.SubmitButton!.Id > output.Id);

        // And the tree says where they ended up: two columns under one row, each holding a form.
        var row = Assert.Single(layout["children"]!.AsArray());
        Assert.Equal(2, row!["children"]!.AsArray().Count);

        // The row holds the input column and the output column; the input column holds the column
        // the fields are drawn in, and the fields are grouped into a form inside that one.
        var inputColumn = row["children"]!.AsArray()[0]!;
        var fields = inputColumn["children"]!.AsArray()[0]!;
        var form = fields["children"]!.AsArray()[0]!;

        var formEntry = config["components"]!.AsArray()
            .Single(entry => entry!["id"]!.GetValue<int>() == form["id"]!.GetValue<int>())!;

        Assert.Equal("form", formEntry["type"]!.GetValue<string>());
        Assert.Equal(input.Id, form["children"]!.AsArray()[0]!["id"]!.GetValue<int>());
    }

    [Fact]
    public async Task An_input_is_labelled_after_its_parameter_and_an_output_says_output()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, double, string>)((name, age) => $"{name} {age}"),
                [new Textbox(), new Number()],
                [new Textbox(), new Number()])));

        var config = await ConfigAsync(host);
        var labels = config["components"]!.AsArray()
            .Where(entry => entry!["type"]!.GetValue<string>() is "textbox" or "number")
            .Select(entry => entry!["props"]!["label"]?.GetValue<string>())
            .ToArray();

        Assert.Equal(ExpectedLabels, labels);
    }

    [Fact]
    public async Task An_interface_with_no_outputs_submits_without_results()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox()],
                flaggingMode: "manual")));

        var config = await ConfigAsync(host);

        // No output column, so no flag button either: there is no result to record.
        Assert.DoesNotContain(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "button"
                && entry["props"]!["value"]?.GetValue<string>() is "Flag" or "Generate");

        Assert.Contains(
            config["components"]!.AsArray(),
            entry => entry!["props"]!["value"]?.GetValue<string>() == "Submit");
    }

    [Fact]
    public async Task An_interface_with_no_inputs_generates()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string>)(() => "hello"),
                outputs: [new Textbox()])));

        var config = await ConfigAsync(host);

        // Nothing to fill in, so the button says what it does.
        Assert.Contains(
            config["components"]!.AsArray(),
            entry => entry!["props"]!["value"]?.GetValue<string>() == "Generate");
    }

    [Fact]
    public async Task The_same_components_on_both_sides_are_one_column()
    {
        Textbox field = new();
        Interface? ui = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                [field],
                [field])));

        Assert.Equal(InterfaceType.Unified, ui!.Type);

        var config = await ConfigAsync(host);

        // One column: the result is written back into the field that produced it, so the page has
        // one form and the component appears in the config once.
        Assert.Single(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "textbox");
    }

    [Fact]
    public async Task A_streaming_handler_gets_a_stop_button_and_the_chain_upstream_serves()
    {
        Interface? ui = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, IAsyncEnumerable<string>>)Count,
                [new Textbox()],
                [new Textbox()],
                flaggingMode: "never")));

        var dependencies = (await ConfigAsync(host))["dependencies"]!.AsArray();

        Assert.NotNull(ui!.StopButton);

        // Seven bindings: the chain of five — the buttons going away, the handler, the buttons
        // coming back, the stop button's own click, and the private binding that performs the
        // cancellation — and the clear button's two.
        Assert.Equal(7, dependencies.Count);

        Assert.Equal("lambda", dependencies[0]!["api_name"]!.GetValue<string>());
        Assert.False(dependencies[0]!["queue"]!.GetValue<bool>());

        // The endpoint is named after the handler, which a C# method spells in PascalCase.
        Assert.Equal("Count", dependencies[1]!["api_name"]!.GetValue<string>());
        Assert.Equal(0, dependencies[1]!["trigger_after"]!.GetValue<int>());
        Assert.True(dependencies[1]!["types"]!["generator"]!.GetValue<bool>());

        Assert.Equal("cleanup", dependencies[2]!["api_name"]!.GetValue<string>());
        Assert.Equal(1, dependencies[2]!["trigger_after"]!.GetValue<int>());

        Assert.Equal("cleanup_1", dependencies[3]!["api_name"]!.GetValue<string>());

        // The list belongs to the cancellation, not to the binding that asked for it: the Python version
        // registers a second, private binding right after, and that one names what it stops.
        Assert.Empty(dependencies[3]!["cancels"]!.AsArray());
        Assert.Equal(0, dependencies[4]!["cancels"]!.AsArray()[0]!.GetValue<int>());
        Assert.True(dependencies[4]!["types"]!["cancel"]!.GetValue<bool>());
        Assert.Equal("private", dependencies[4]!["api_visibility"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_live_interface_runs_on_the_fields_changes()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                [new Textbox(), new Number()],
                [new Textbox()],
                live: true,
                timeLimit: 12,
                streamEvery: 0.25)));

        var config = await ConfigAsync(host);
        var predict = config["dependencies"]!.AsArray()[0]!;

        Assert.Equal(2, predict["targets"]!.AsArray().Count);
        Assert.All(
            predict["targets"]!.AsArray(),
            target => Assert.Equal("change", target![1]!.GetValue<string>()));

        // A live interface has no submit button to press, and its runs coalesce: the last change
        // wins rather than queueing behind the ones before it.
        Assert.Equal("always_last", predict["trigger_mode"]!.GetValue<string>());
        Assert.Equal(12, predict["time_limit"]!.GetValue<double>());
        Assert.Equal(0.25, predict["stream_every"]!.GetValue<double>());

        Assert.DoesNotContain(
            config["components"]!.AsArray(),
            entry => entry!["props"]!["value"]?.GetValue<string>() == "Submit");
    }

    [Fact]
    public async Task A_flagged_sample_is_written_to_the_log()
    {
        var flagged = Path.Combine(_folder, "flagged");
        var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                [new Textbox { Label = "In" }],
                [new Textbox { Label = "Out" }],
                flaggingDir: flagged)));

        var config = await ConfigAsync(host);
        var flag = config["dependencies"]!.AsArray()
            .Single(dependency => dependency!["api_name"]!.GetValue<string>() == "Flag");

        // The button is a plain button update followed by the record; what the click posts is the
        // values of the components it records, as the client sent them.
        var response = await PostAsync(host, "/gradio_api/run/Flag", new JsonObject
        {
            ["data"] = new JsonArray("hello", "HELLO"),
            ["session_hash"] = "flagging",
        });

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            string.Join(" | ", logs.Entries.Select(entry => entry.Message + " :: " + entry.Exception)));

        var file = Path.Combine(flagged, "dataset1.csv");

        Assert.True(System.IO.File.Exists(file), $"'{file}' was written");

        var rows = await System.IO.File.ReadAllLinesAsync(file, TestContext.Current.CancellationToken);

        Assert.Equal("In,Out,timestamp", rows[0]);
        Assert.StartsWith("hello,HELLO,", rows[1], StringComparison.Ordinal);

        Assert.NotEmpty(flag!["inputs"]!.AsArray());
    }

    [Fact]
    public async Task A_flag_option_is_recorded_with_the_sample()
    {
        var flagged = Path.Combine(_folder, "options");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox { Label = "In" }],
                [new Textbox { Label = "Out" }],
                flaggingOptions: ["wrong", "bad"],
                flaggingDir: flagged)));

        var config = await ConfigAsync(host);

        Assert.Equal(
            ["Flag as wrong", "Flag as bad"],
            config["components"]!.AsArray()
                .Where(entry => entry!["type"]!.GetValue<string>() == "button")
                .Select(entry => entry!["props"]!["value"]!.GetValue<string>())
                .Where(value => value.StartsWith("Flag", StringComparison.Ordinal)));

        // The second option's button records the option's value, which is what the log's extra
        // column carries.
        var second = config["dependencies"]!.AsArray()
            .Single(dependency => dependency!["api_name"]!.GetValue<string>() == "Flag_1");

        var response = await PostAsync(host, "/gradio_api/run/Flag_1", new JsonObject
        {
            ["data"] = new JsonArray("a", "b"),
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await System.IO.File.ReadAllLinesAsync(
            Path.Combine(flagged, "dataset1.csv"),
            TestContext.Current.CancellationToken);

        Assert.Equal("In,Out,flag,timestamp", rows[0]);
        Assert.StartsWith("a,b,bad,", rows[1], StringComparison.Ordinal);

        Assert.NotEmpty(second!["inputs"]!.AsArray());
    }

    [Fact]
    public async Task An_eager_gallery_works_every_answer_out_at_startup()
    {
        Interface? ui = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                [new Textbox()],
                [new Textbox()],
                examples: new object?[] { new object?[] { "a" }, new object?[] { "b" } },
                cacheExamples: true,
                cacheMode: "eager",
                preloadExample: null)));

        Assert.True(ui!.Examples!.CacheExamples);
        Assert.Equal("eager", ui.Examples.CacheMode);

        var log = Path.Combine(ui.Examples.CacheFolder, "log.csv");

        Assert.True(System.IO.File.Exists(log), $"'{log}' was written");

        var rows = await System.IO.File.ReadAllLinesAsync(log, TestContext.Current.CancellationToken);

        Assert.Equal(3, rows.Length);
        Assert.StartsWith("A,", rows[1], StringComparison.Ordinal);
        Assert.StartsWith("B,", rows[2], StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_lazy_gallery_works_an_answer_out_when_it_is_first_asked_for()
    {
        Interface? ui = null;
        var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                [new Textbox()],
                [new Textbox()],
                examples: new object?[] { new object?[] { "a" } },
                cacheExamples: true,
                cacheMode: "lazy",
                preloadExample: null)));

        Assert.Equal("lazy", ui!.Examples!.CacheMode);

        var log = Path.Combine(ui.Examples.CacheFolder, "log.csv");

        Assert.False(System.IO.File.Exists(log), "nothing is worked out before anybody asks");

        // The gallery's second step is what asks: the sample's position is its value.
        var response = await PostAsync(host, "/gradio_api/run/load_example", new JsonObject
        {
            ["data"] = new JsonArray(0),
        });

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            string.Join(
                " | ",
                logs.Entries
                    .Where(entry => entry.Exception is not null)
                    .Select(entry => entry.Message + " :: " + entry.Exception)));
        Assert.True(System.IO.File.Exists(log), $"'{log}' was written when the sample was asked for");

        var body = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal("A", body["data"]!.AsArray()[0]!.GetValue<string>());
    }

    [Fact]
    public async Task A_share_link_opens_the_application_where_it_was_left()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox()],
                [new Textbox()],
                flaggingMode: "never",
                deepLink: true)));

        var blocks = host.Services.GetRequiredService<Blocks>();
        var session = host.Services.GetRequiredService<ISessionStore>().GetOrAdd("shared");
        var field = blocks.Registered.OfType<Textbox>().First();

        // The value a field carries belongs to the browser; what the server has is the last value
        // it saw go past, which is what a run records and what a link restores.
        session.State.Remember(field.Id, JsonValue.Create("typed in"));

        using var shared = await host.Client.GetAsync(
            "/gradio_api/deep_link?session_hash=shared",
            TestContext.Current.CancellationToken);

        var name = JsonNode.Parse(await shared.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .GetValue<string>();

        Assert.NotEmpty(name);

        // Opening the link describes the state it names rather than the application's own.
        var restored = await ConfigAsync(host, $"?deep_link={name}");

        Assert.Equal("valid", restored["deep_link_state"]!.GetValue<string>());
        Assert.Contains(
            restored["components"]!.AsArray(),
            entry => entry!["id"]!.GetValue<int>() == field.Id
                && entry["props"]!["value"]?.GetValue<string>() == "typed in");

        // The page carries the same state, which is what the frontend reads to know it is restored.
        using var page = await host.Client.GetAsync(
            $"/?deep_link={name}",
            TestContext.Current.CancellationToken);

        var html = await page.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Matches("\"deep_link_state\"\\s*:\\s*\"valid\"", html);

        // And a page with no link says so, rather than saying nothing.
        using var plain = await host.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var plainHtml = await plain.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Matches("\"deep_link_state\"\\s*:\\s*\"none\"", plainHtml);
    }

    [Fact]
    public async Task A_share_link_the_server_cannot_read_is_reported_as_invalid()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox()],
                [new Textbox()],
                flaggingMode: "never",
                deepLink: true)));

        var config = await ConfigAsync(host, "?deep_link=never-written");

        Assert.Equal("invalid", config["deep_link_state"]!.GetValue<string>());

        // A name that is not a name is refused before the file system is asked anything, and the
        // application is served as it would have been.
        var traversal = await ConfigAsync(host, "?deep_link=..%2F..%2Fstate");

        Assert.Equal("invalid", traversal["deep_link_state"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_validator_stops_a_run_before_the_handler_sees_it()
    {
        var ran = false;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value =>
                {
                    ran = true;

                    return value;
                }),
                [new Textbox()],
                [new Textbox()],
                flaggingMode: "never",
                validator: (Func<string, string?>)(value => value.Length >= 3 ? null : "too short"))));

        var response = await PostAsync(host, "/gradio_api/queue/join", new JsonObject
        {
            ["data"] = new JsonArray("ab"),
            ["fn_index"] = 0,
            ["session_hash"] = "validated",
            ["trigger_id"] = 1,
            ["event_data"] = null,
        });

        // The queue refuses the run at admission, which is what the Python version does: the handler is never
        // called, and the client is told why.
        Assert.True(
            response.StatusCode == HttpStatusCode.UnprocessableEntity,
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.False(ran);
    }

    [Fact]
    public async Task A_gallery_pages_the_samples_it_was_given()
    {
        Interface? ui = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => ui = new Interface(
                blocks,
                (Func<string, string>)(value => value),
                [new Textbox()],
                [new Textbox()],
                examples: new object?[]
                {
                    new object?[] { "a" },
                    new object?[] { "b" },
                    new object?[] { "c" },
                },
                examplesPerPage: 2,
                exampleLabels: ["first", "second", "third"],
                preloadExample: null)));

        var config = await ConfigAsync(host);
        var gallery = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "dataset")!;

        Assert.NotNull(gallery["props"]);

        Assert.Equal(2, gallery["props"]!["samples_per_page"]!.GetValue<int>());
        Assert.Equal(3, gallery["props"]!["samples"]!.AsArray().Count);
        Assert.Equal(
            ["first", "second", "third"],
            gallery["props"]!["sample_labels"]!.AsArray().Select(label => label!.GetValue<string>()));

        Assert.NotNull(ui!.Examples);
    }

    /// <summary>
    /// A side of the interface can be given by name, the way the Python version's shortcuts are
    /// written.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version's <c>inputs="text"</c>: the name means the component with every default,
    /// so the page it draws is the page the same application would have drawn with the constructor
    /// spelled out. The name table is the framework's own, which is why an interface built this way
    /// and one built with a <c>new Textbox()</c> describe the same components.
    /// </para>
    /// <para>
    /// The output is given as a single component rather than a list, which is the other spelling
    /// the Python version allows and the reason the parameter is not a sequence.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task An_interface_takes_its_sides_by_name_or_one_at_a_time()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, string>)(value => value.ToUpperInvariant()),
                "text",
                new Textbox { Label = "Out" })));

        var config = await ConfigAsync(host);
        var textboxes = config["components"]!.AsArray()
            .Where(entry => entry!["type"]!.GetValue<string>() == "textbox")
            .ToArray();

        // One textbox from the name and one from the instance, and nothing else: a name that
        // resolved to a different component, or to none, would not be two textboxes.
        Assert.Equal(2, textboxes.Length);

        // The name means the component with every default, and the interface then labels it after
        // the handler's parameter — which is what it does to an unnamed input either way. The
        // instance keeps the label it was given.
        Assert.Equal("value", textboxes[0]!["props"]!["label"]!.GetValue<string>());
        Assert.Equal("Out", textboxes[1]!["props"]!["label"]!.GetValue<string>());
    }

    /// <summary>
    /// One list may hold a component and a name, which is the spelling the list has to be able to
    /// take for a name to work inside it at all.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>Sequence[str | Component]</c> mixes the two freely. A C# collection expression
    /// works out one element type for the whole list, so the only type a component and a name share
    /// is <see cref="object"/> — which is why the list takes entries rather than components and
    /// resolves them as it adds them.
    /// </remarks>
    [Fact]
    public async Task A_list_may_mix_a_component_and_a_name()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<double, string, string>)((number, text) => $"{text}: {number}"),
                [new Number(), "text"],
                "text")));

        var config = await ConfigAsync(host);
        var types = config["components"]!.AsArray()
            .Select(entry => entry!["type"]!.GetValue<string>())
            .ToArray();

        Assert.Contains("number", types);
        Assert.Equal(2, types.Count(type => type == "textbox"));
    }

    /// <summary>
    /// A name is read the way the Python version reads one: underscores are not part of it, and two
    /// names stand beside the classes they are written for.
    /// </summary>
    /// <remarks>
    /// The Python version looks a shortcut up with the underscores removed and with the extra names
    /// its component module adds — <c>Text = Textbox</c> and <c>Highlight = HighlightedText</c> —
    /// so <c>"highlighted_text"</c> names the same component as <c>"highlight"</c> and
    /// <c>"text"</c> names a textbox. Both spellings are used in the Python version's own
    /// documentation, which is why they are part of what a port has to accept rather than a
    /// convenience.
    /// </remarks>
    [Theory]
    [InlineData("highlight")]
    [InlineData("highlighted_text")]
    public async Task A_name_is_read_without_its_underscores_and_by_its_short_name(string name)
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => Build(blocks, blocks => new Interface(
                blocks,
                (Func<string, HighlightedToken[]>)(value => [new HighlightedToken(value)]),
                "text",
                name)));

        var config = await ConfigAsync(host);
        var types = config["components"]!.AsArray()
            .Select(entry => entry!["type"]!.GetValue<string>())
            .ToArray();

        Assert.Contains("textbox", types);
        Assert.Contains("highlightedtext", types);
    }

    /// <summary>
    /// A name that is not a component is refused when the interface is built.
    /// </summary>
    /// <remarks>
    /// The Python version would fail at the same moment, when it looks the shortcut up. The message
    /// names the name, because a typo in a string is the one mistake a compiler cannot catch here.
    /// </remarks>
    [Fact]
    public void A_name_that_is_not_a_component_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            var refused = Assert.Throws<ArgumentException>(() =>
            {
                _ = new Interface(
                    blocks,
                    (Func<string, string>)(value => value),
                    "nope",
                    "text");
            });

            Assert.Contains("nope", refused.Message, StringComparison.Ordinal);
        }
    }

    /// <summary>Builds an application whose files go in this class's own directory.</summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <param name="app">What to render into it.</param>
    /// <remarks>
    /// The interface is returned rather than discarded so that the tests that need to look at what
    /// it built can keep it; the ones that only read the served document ignore it.
    /// </remarks>
    private void Build(Blocks blocks, Func<Blocks, Interface> app)
    {
        blocks.UploadFolder = _folder;

        _ = app(blocks);
    }

    /// <summary>The served config, with the request's own fields in place.</summary>
    /// <param name="host">The host.</param>
    /// <param name="query">The query string, or <see langword="null"/> for none.</param>
    /// <returns>The config.</returns>
    private static async Task<JsonObject> ConfigAsync(GradioTestHost host, string? query = null)
    {
        using var response = await host.Client.GetAsync(
            "/config" + query,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();
    }

    /// <summary>Posts a body to an endpoint.</summary>
    /// <param name="host">The host.</param>
    /// <param name="path">The endpoint.</param>
    /// <param name="body">The body.</param>
    /// <returns>The response.</returns>
    private static Task<HttpResponseMessage> PostAsync(GradioTestHost host, string path, JsonObject body) =>
        host.Client.PostAsync(
            path,
            new StringContent(body.ToJsonString(), System.Text.Encoding.UTF8, "application/json"),
            TestContext.Current.CancellationToken);

    /// <summary>A handler that streams two chunks.</summary>
    /// <param name="value">The value it was given.</param>
    /// <returns>The chunks.</returns>
    private static async IAsyncEnumerable<string> Count(string value)
    {
        yield return value;

        await Task.Yield();

        yield return value;
    }
}

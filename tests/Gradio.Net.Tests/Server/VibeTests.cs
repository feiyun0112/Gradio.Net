using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using File = System.IO.File;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The Vibe editor seen the way the panel sees it: the code is read, an edit is asked for, and the
/// file, the snapshot and the answer are what come back.
/// </summary>
/// <remarks>
/// <para>
/// The model is a fake registered before Gradio's own — the seam to verify an external dependency
/// at — so what is asserted is the contract: the request the editor builds, and what it does with
/// an answer. No model is asked and no network is touched, which is the point: the real integration
/// is not verified here.
/// </para>
/// <para>
/// The demo file is a file of the test's own, registered as <see cref="VibeOptions.DemoPath"/>, so an
/// edit rewrites something disposable. The switch is <see cref="Blocks.VibeMode"/>, which is the
/// config's field as well as the routes' — a test that turned one on without the other would be
/// testing a state an application cannot be in.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class VibeTests : IDisposable
{
    /// <summary>The app every test starts from, unless it says otherwise.</summary>
    private const string DemoCode = "import gradio as gr\n\ndemo = gr.Blocks()\n";

    private readonly TempFolder _folder = new();

    [Fact]
    public async Task An_application_that_has_not_turned_the_editor_on_refuses_every_route()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(vibe: false, new FakeModel(), cancellationToken);

        using var edit = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-edit",
            new { prompt = "Add a title" },
            cancellationToken);
        using var undo = await host.Client.PostAsJsonAsync(
            "/gradio_api/undo-vibe-edit",
            new { hash = new string('a', 32) },
            cancellationToken);
        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);
        using var write = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-code",
            new { code = "x" },
            cancellationToken);
        using var starters = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-starter-queries",
            new { },
            cancellationToken);

        foreach (var response in new[] { edit, undo, read, write, starters })
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal(
                """{"detail":"Vibe editor is not enabled. Use --vibe flag to enable."}""",
                await response.Content.ReadAsStringAsync(cancellationToken));
        }
    }

    /// <summary>
    /// A malformed body is refused before the switch is asked, on every route that takes one.
    /// </summary>
    /// <remarks>
    /// FastAPI validates a route's body before it calls the function, so an application that has
    /// not turned the editor on still refuses a body that carries no field as malformed — which is
    /// the order the Python version answers in, not the order its handler code reads in. Only the
    /// three routes that declare a body can answer that way; <c>/vibe-starter-queries</c> declares
    /// none, which is why it answers the switch instead (asserted below).
    /// </remarks>
    [Theory]
    [InlineData(
        "/gradio_api/vibe-edit",
        "{}",
        """{"detail":[{"type":"missing","loc":["body","prompt"],"msg":"Field required","input":null}]}""")]
    [InlineData(
        "/gradio_api/undo-vibe-edit",
        "{}",
        """{"detail":[{"type":"missing","loc":["body","hash"],"msg":"Field required","input":null}]}""")]
    [InlineData(
        "/gradio_api/vibe-code",
        "{}",
        """{"detail":[{"type":"missing","loc":["body","code"],"msg":"Field required","input":null}]}""")]
    public async Task A_malformed_body_is_refused_before_the_switch_is_asked(
        string path,
        string body,
        string expected)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(vibe: false, new FakeModel(), cancellationToken);

        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync(path, content, cancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(expected, await response.Content.ReadAsStringAsync(cancellationToken));

        // The route that declares no body has nothing to validate, so the switch is the first thing
        // it answers.
        using var starters = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-starter-queries",
            new { },
            cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, starters.StatusCode);
    }

    [Fact]
    public async Task The_config_says_whether_the_editor_is_on()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var on = await StartAsync(vibe: true, new FakeModel(), cancellationToken);
        await using var off = await StartAsync(vibe: false, new FakeModel(), cancellationToken);

        Assert.True(await ConfigVibeModeAsync(on.Client, cancellationToken));
        Assert.False(await ConfigVibeModeAsync(off.Client, cancellationToken));
    }

    [Fact]
    public async Task The_code_route_reads_and_writes_the_file_the_deployment_names()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", DemoCode);

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken, demo);

        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(
            new JsonObject { ["code"] = DemoCode }.ToJsonString(),
            await read.Content.ReadAsStringAsync(cancellationToken));

        const string Replacement = "import gradio as gr\n\ndemo = gr.Blocks(title=\"Edited\")\n";

        using var write = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-code",
            new { code = Replacement },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, write.StatusCode);
        Assert.Equal(
            """{"success":true}""",
            await write.Content.ReadAsStringAsync(cancellationToken));

        // The file, not a copy of it in memory: the editor's whole point is that something else
        // watches this file and reloads the app from it.
        Assert.Equal(Replacement, await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    [Fact]
    public async Task An_edit_answers_with_a_snapshot_what_changed_and_the_explanation()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\nb\nc");

        var model = new FakeModel();
        model.Answer("<reasoning>Inserted a line.</reasoning>\na\nnew\nb\nc\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        var answer = await EditAsync(host.Client, "Add a line", cancellationToken);

        // A snapshot is a name the browser can hand back, and the shape the Python version draws for one.
        var snapshot = answer["hash"]!.GetValue<string>();

        Assert.Equal(32, snapshot.Length);
        Assert.All(snapshot, character => Assert.True(char.IsAsciiHexDigitLower(character)));

        Assert.Equal("Inserted a line.", answer["reasoning"]!.GetValue<string>());
        Assert.Equal(1, answer["diff_stats"]!["lines_added"]!.GetValue<int>());
        Assert.Equal(0, answer["diff_stats"]!["lines_removed"]!.GetValue<int>());

        // The explanation is out of the file and the code is in it — without the newline the answer
        // ended with, because the Python version strips the code it takes out of an answer.
        Assert.Equal("a\nnew\nb\nc", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    [Fact]
    public async Task The_file_keeps_the_code_the_answer_fenced_off()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();
        model.Answer("<reasoning>Rewrote it.</reasoning>\n```python\nimport gradio as gr\n```");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        await EditAsync(host.Client, "Rewrite this", cancellationToken);

        // The Python version takes what is between the fences, which is the code and not the newline before the
        // closing fence — so the file ends where the code does.
        Assert.Equal("import gradio as gr", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    [Fact]
    public async Task The_model_is_asked_with_the_rules_the_code_and_the_instruction()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var model = new FakeModel();
        model.Answer("a\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken);

        await EditAsync(host.Client, "Add a title", cancellationToken);

        var request = Assert.Single(model.Requests);

        // The Python version's two literals, so that a backend is asked for the same thing it would be asked
        // for in Python.
        Assert.Equal("openai/gpt-oss-120b", request.Model);
        Assert.Equal(10_000, request.MaxTokens);

        // The rules, and not the documentation the Python version fetches from gradio.app around them.
        Assert.StartsWith("Generate code for using the Gradio python library.", request.SystemPrompt, StringComparison.Ordinal);
        Assert.Contains("Respond with a full Gradio app.", request.SystemPrompt, StringComparison.Ordinal);
        Assert.DoesNotContain("gradio.app/llms.txt", request.SystemPrompt, StringComparison.Ordinal);

        Assert.Contains(DemoCode, request.UserPrompt, StringComparison.Ordinal);
        Assert.Contains("Prompt:\nAdd a title", request.UserPrompt, StringComparison.Ordinal);
        Assert.Contains("History:\nNo chat history.", request.UserPrompt, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_second_edit_is_asked_against_the_first_answer_and_the_conversation()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();
        model.Answer("a\nb\n");
        model.Answer("a\nb\nc\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        await EditAsync(host.Client, "Add b", cancellationToken);
        await EditAsync(host.Client, "Add c", cancellationToken);

        Assert.Equal(2, model.Requests.Count);

        var second = model.Requests[1].UserPrompt;

        // The file is read again rather than remembered: what the second edit rewrites is what the
        // first one left behind.
        Assert.Contains("Existing code:\n```python\na\nb\n```", second, StringComparison.Ordinal);
        Assert.Contains("User: Add b\nAssistant: a\nb\n", second, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Undoing_an_edit_puts_the_file_and_the_conversation_back()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();
        model.Answer("a\nb\n");
        model.Answer("a\nb\nc\n");
        model.Answer("a\nz\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        var first = await EditAsync(host.Client, "Add b", cancellationToken);
        await EditAsync(host.Client, "Add c", cancellationToken);

        Assert.Equal("a\nb\nc", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));

        using var undo = await host.Client.PostAsJsonAsync(
            "/gradio_api/undo-vibe-edit",
            new { hash = first["hash"]!.GetValue<string>() },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, undo.StatusCode);
        Assert.Equal("""{"success":true}""", await undo.Content.ReadAsStringAsync(cancellationToken));

        // The file is what the first edit found, and so is the conversation: an undo that left the
        // exchange behind would answer the next prompt against code that no longer exists.
        Assert.Equal("a\n", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));

        await EditAsync(host.Client, "Add z", cancellationToken);

        Assert.DoesNotContain("User: Add c", model.Requests[^1].UserPrompt, StringComparison.Ordinal);
        Assert.Contains("History:\nNo chat history.", model.Requests[^1].UserPrompt, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("not-a-snapshot")]
    [InlineData("../../etc/passwd")]
    [InlineData("0123456789abcdef0123456789abcdef")]
    public async Task A_name_that_is_not_a_snapshot_is_not_found(string hash)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken);

        using var undo = await host.Client.PostAsJsonAsync(
            "/gradio_api/undo-vibe-edit",
            new { hash },
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, undo.StatusCode);
        Assert.Equal(
            """{"detail":"Snapshot not found"}""",
            await undo.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>
    /// A name that would reach outside the snapshot folder restores nothing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The names above are refused whether or not the name is checked, because no file answers to
    /// any of them — which makes them evidence of the answer and not of the check. This one plants a
    /// file one level up from the folder the snapshots live in, exactly where <c>../planted</c>
    /// would reach, and then asks for it.
    /// </para>
    /// <para>
    /// The Python version joins the name onto the path unchecked, so the same request would read
    /// that file and write it into the application's source — a read primitive driven by a request
    /// field. Here the name is not one the editor issued, so the answer is the one a lost snapshot
    /// gets, and the file the editor edits is untouched.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_name_that_would_reach_outside_the_snapshot_folder_restores_nothing()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken, demo);

        var upload = host.Services.GetRequiredService<Blocks>().UploadFolder;
        var snapshots = Path.Combine(upload, "vibe_edit_history");

        Directory.CreateDirectory(snapshots);

        // The file a traversing name would reach: one level up from the snapshots.
        var planted = Path.Combine(upload, "planted.py");

        await System.IO.File.WriteAllTextAsync(planted, "PLANTED", cancellationToken);

        using var undo = await host.Client.PostAsJsonAsync(
            "/gradio_api/undo-vibe-edit",
            new { hash = "../planted" },
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, undo.StatusCode);
        Assert.Equal(
            """{"detail":"Snapshot not found"}""",
            await undo.Content.ReadAsStringAsync(cancellationToken));

        Assert.Equal("a\n", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    /// <summary>
    /// A snapshot whose conversation was never filed restores an empty one, which is the Python
    /// version's <c>get(hash, "")</c>.
    /// </summary>
    /// <remarks>
    /// The file and the transcript are two things that travel together: a snapshot file that this
    /// process never wrote — one left by an earlier run — still restores the code, and the
    /// conversation it has no record of is restored as nothing. An undo that kept the conversation
    /// would answer the next prompt against an exchange whose code is gone.
    /// </remarks>
    [Fact]
    public async Task A_snapshot_with_no_filed_conversation_restores_an_empty_one()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();
        model.Answer("a\nb\n");
        model.Answer("c\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        await EditAsync(host.Client, "Add b", cancellationToken);

        var upload = host.Services.GetRequiredService<Blocks>().UploadFolder;
        var snapshots = Path.Combine(upload, "vibe_edit_history");

        Directory.CreateDirectory(snapshots);

        // A name the editor would have issued, over a file it never wrote in this process.
        var orphan = new string('a', 32);

        await System.IO.File.WriteAllTextAsync(Path.Combine(snapshots, orphan + ".py"), "from an earlier run", cancellationToken);

        using var undo = await host.Client.PostAsJsonAsync(
            "/gradio_api/undo-vibe-edit",
            new { hash = orphan },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, undo.StatusCode);
        Assert.Equal("from an earlier run", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));

        await EditAsync(host.Client, "Rewrite it", cancellationToken);

        Assert.Contains("History:\nNo chat history.", model.Requests[^1].UserPrompt, StringComparison.Ordinal);
        Assert.DoesNotContain("User: Add b", model.Requests[^1].UserPrompt, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Starter_queries_are_the_lines_the_model_answered()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var model = new FakeModel();
        model.Answer("Add a title to the app\n\nAdd examples\n");

        await using var host = await StartAsync(vibe: true, model, cancellationToken);

        using var response = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-starter-queries",
            new { },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Split on the line breaks as they are, empty lines included: the Python version does not filter them,
        // so a model that answered a blank line between two questions has answered three lines.
        Assert.Equal(
            """{"starter_queries":["Add a title to the app","","Add examples"]}""",
            await response.Content.ReadAsStringAsync(cancellationToken));

        var request = Assert.Single(model.Requests);

        Assert.Contains("You are a prompt generator for a gradio vibe editor.", request.SystemPrompt, StringComparison.Ordinal);
        Assert.Contains(DemoCode, request.UserPrompt, StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_application_without_a_backend_is_told_what_to_register()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(vibe: true, model: null, cancellationToken);

        using var edit = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-edit",
            new { prompt = "Add a title" },
            cancellationToken);
        using var starters = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-starter-queries",
            new { },
            cancellationToken);

        foreach (var response in new[] { edit, starters })
        {
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains(
                nameof(IVibeLanguageModel),
                await DetailAsync(response, cancellationToken),
                StringComparison.Ordinal);
        }

        // The two routes that only touch the file still work: a backend is what generates code, not
        // what the editor is.
        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
    }

    [Fact]
    public async Task A_body_without_its_field_is_refused_the_way_fastapi_refuses_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken);

        var refused = new (string Path, string Body, string Expected)[]
        {
            (
                "/gradio_api/vibe-edit",
                """{}""",
                """{"detail":[{"type":"missing","loc":["body","prompt"],"msg":"Field required","input":null}]}"""),
            (
                "/gradio_api/undo-vibe-edit",
                """{"hash": 1}""",
                """{"detail":[{"type":"string_type","loc":["body","hash"],"msg":"Input should be a valid string","input":1}]}"""),
            (
                "/gradio_api/vibe-code",
                """not json""",
                """{"detail":[{"type":"missing","loc":["body","code"],"msg":"Field required","input":null}]}"""),
        };

        foreach (var (path, body, expected) in refused)
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await host.Client.PostAsync(path, content, cancellationToken);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Equal(expected, await response.Content.ReadAsStringAsync(cancellationToken));
        }
    }

    /// <summary>
    /// A deployment that named no demo file has nothing to read, rewrite or write.
    /// </summary>
    /// <remarks>
    /// Every route that touches the file answers the same thing, which is the one place this
    /// framework deliberately answers where the Python version does not: the Python version maps a
    /// missing file for the route that reads the code, and lets the other routes raise out of
    /// <c>open()</c> — the write route catches that and calls it <c>"Error writing file: …"</c>,
    /// the other two surface as a bodyless <c>500</c>. A deployment that named no file is one fact,
    /// so it gets one answer .
    /// </remarks>
    [Fact]
    public async Task A_deployment_that_names_no_demo_file_has_nothing_to_edit()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            vibe: true,
            new FakeModel(),
            cancellationToken,
            demoPath: string.Empty);

        foreach (var response in new[]
        {
            await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken),
            await host.Client.PostAsJsonAsync("/gradio_api/vibe-code", new { code = "x" }, cancellationToken),
            await host.Client.PostAsJsonAsync("/gradio_api/vibe-edit", new { prompt = "Add" }, cancellationToken),
            await host.Client.PostAsJsonAsync("/gradio_api/vibe-starter-queries", new { }, cancellationToken),
        })
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(
                """{"detail":"Demo file not found"}""",
                await response.Content.ReadAsStringAsync(cancellationToken));

            response.Dispose();
        }
    }

    [Fact]
    public async Task A_marker_or_a_think_block_never_reaches_the_file()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();
        model.Answer(
            "<|start|>assistant<|channel|>analysis<|message|>thinking out loud<|end|>"
            + "<|start|>assistant<|channel|>final<|message|><think>still thinking</think>"
            + "<reasoning>Done.</reasoning>\na\nb\n<|end|>");

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        var answer = await EditAsync(host.Client, "Add b", cancellationToken);

        Assert.Equal("Done.", answer["reasoning"]!.GetValue<string>());
        Assert.Equal("a\nb", await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    [Fact]
    public async Task A_model_that_fails_is_an_answer_that_says_so()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", DemoCode);

        await using var host = await StartAsync(vibe: true, new FailingModel(), cancellationToken, demo);

        using var response = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-edit",
            new { prompt = "Add a title" },
            cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            """{"detail":"Error generating code"}""",
            await response.Content.ReadAsStringAsync(cancellationToken));

        // Nothing was written: a failed generation is not an edit that half happened.
        Assert.Equal(DemoCode, await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    /// <summary>
    /// A model that answers nothing is the same failure as one that raises.
    /// </summary>
    /// <remarks>
    /// The Python version tests the answer for <c>None</c> and nothing else, so an empty answer is
    /// a rewrite and a missing one is a failure — a backend that has nothing to say is told to
    /// answer, not to hand back an empty document that would blank the application's source.
    /// </remarks>
    [Fact]
    public async Task A_model_that_answers_nothing_is_an_answer_that_says_so()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", DemoCode);

        // A model that was queued no answers has nothing to say.
        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken, demo);

        using var response = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-edit",
            new { prompt = "Add a title" },
            cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            """{"detail":"Error generating code"}""",
            await response.Content.ReadAsStringAsync(cancellationToken));

        Assert.Equal(DemoCode, await System.IO.File.ReadAllTextAsync(demo, cancellationToken));
    }

    [Theory]
    [MemberData(nameof(Diffs))]
    public async Task The_diff_counts_what_the_edit_changed(string before, string after, int added, int removed)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", before);

        var model = new FakeModel();
        model.Answer(after);

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        var answer = await EditAsync(host.Client, "Change it", cancellationToken);

        Assert.Equal(added, answer["diff_stats"]!["lines_added"]!.GetValue<int>());
        Assert.Equal(removed, answer["diff_stats"]!["lines_removed"]!.GetValue<int>());
    }

    /// <summary>
    /// The prompt carries the last five exchanges, which is the Python version's limit and its
    /// arithmetic.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The conversation is folded into the prompt as text, and an old exchange is not what the next
    /// instruction is about. The Python version trims to five <c>User:</c> pieces — and because the
    /// transcript opens with a newline, the first piece of a five-exchange transcript is the empty
    /// string, so five exchanges are trimmed too: the leading blank line becomes <c>User: </c>.
    /// That is the quirk pinned below, and it is visible in the prompt a model is actually sent.
    /// </para>
    /// <para>
    /// The eighth edit is the one that shows the limit: seven exchanges are in the conversation and
    /// the prompt carries the last five, so the first two are gone from it.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_prompt_carries_the_last_five_exchanges_and_no_more()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("demo.py", "a\n");

        var model = new FakeModel();

        for (var edit = 1; edit <= 8; edit++)
        {
            model.Answer($"code {edit}\n");
        }

        await using var host = await StartAsync(vibe: true, model, cancellationToken, demo);

        for (var edit = 1; edit <= 8; edit++)
        {
            await EditAsync(host.Client, $"ask {edit}", cancellationToken);
        }

        // The sixth edit is the first one whose conversation already holds five exchanges: the
        // leading blank line of the transcript is gone and the first exchange is still there.
        Assert.Contains(
            "History:\nUser: ask 1\nAssistant: code 1\n\nUser: ask 2",
            model.Requests[5].UserPrompt,
            StringComparison.Ordinal);

        // The eighth edit keeps the last five exchanges — asks 3 to 7 — and has dropped 1 and 2.
        Assert.Contains("History:\nUser: ask 3", model.Requests[7].UserPrompt, StringComparison.Ordinal);
        Assert.DoesNotContain("ask 1", model.Requests[7].UserPrompt, StringComparison.Ordinal);
        Assert.DoesNotContain("ask 2", model.Requests[7].UserPrompt, StringComparison.Ordinal);
        Assert.Contains("User: ask 7", model.Requests[7].UserPrompt, StringComparison.Ordinal);
    }

    /// <summary>
    /// Starter queries are cleaned in the opposite order to an edit's answer, which is the Python
    /// version's.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both routes take the markers and the think blocks out of an answer, and the Python version
    /// calls the two in the other order for this one. It is not a slip to be tidied up: the two
    /// orders differ on an answer whose think block contains a marker, and the answer below is such
    /// a one.
    /// </para>
    /// <para>
    /// With the starter-queries order — think blocks first, then markers — the block is removed
    /// whole and what is left is <c>Xtail</c>. With the edit's order the marker is taken first,
    /// which cuts the closing tag of the block out of the answer, so the block can no longer be
    /// recognised: the same answer would be reported as <c>shown&lt;/think&gt;tail</c>.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Starter_queries_are_cleaned_in_the_opposite_order_to_an_edit()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var model = new FakeModel();
        model.Answer("X<think>hidden<|start|>assistant<|channel|>final<|message|>shown</think>tail");

        await using var host = await StartAsync(vibe: true, model, cancellationToken);

        using var response = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-starter-queries",
            new { },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            """{"starter_queries":["Xtail"]}""",
            await response.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>
    /// A file that cannot be written is an answer that says so, rather than a broken response.
    /// </summary>
    /// <remarks>
    /// The path is a folder: opening it for writing fails the way a deployment with a read-only
    /// checkout fails. The Python version catches that and answers <c>500</c> with the reason in
    /// the detail; the reason is the operating system's, so only its prefix is the same on both
    /// sides.
    /// </remarks>
    [Fact]
    public async Task A_file_that_cannot_be_written_is_an_answer_that_says_so()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var folder = Path.Combine(_folder.Folder, "a-folder");
        Directory.CreateDirectory(folder);

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken, folder);

        using var write = await host.Client.PostAsJsonAsync(
            "/gradio_api/vibe-code",
            new { code = "import gradio as gr\n" },
            cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, write.StatusCode);
        Assert.StartsWith("Error writing file: ", await DetailAsync(write, cancellationToken), StringComparison.Ordinal);
    }

    /// <summary>
    /// A file that cannot be read is answered the same way, under the other verb.
    /// </summary>
    /// <remarks>
    /// The path is a folder here too: opening it for reading fails the way a deployment whose demo
    /// file was replaced by something unreadable fails. The Python version maps that failure for
    /// this route and answers the reason; the reason is the operating system's, so only its prefix
    /// is the same.
    /// </remarks>
    [Fact]
    public async Task A_file_that_cannot_be_read_is_an_answer_that_says_so()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var folder = Path.Combine(_folder.Folder, "unreadable");
        Directory.CreateDirectory(folder);

        await using var host = await StartAsync(vibe: true, new FakeModel(), cancellationToken, folder);

        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, read.StatusCode);
        Assert.StartsWith("Error reading file: ", await DetailAsync(read, cancellationToken), StringComparison.Ordinal);
    }

    /// <summary>Edits whose counts were taken from the Python implementation's own diff.</summary>
    /// <returns>One case per shape of change, and the two numbers the Python version answers
    /// with.</returns>
    /// <remarks>
    /// <para>
    /// The expected numbers are <c>difflib</c>'s, which is why the odd ones are here. The answer a
    /// model gives is stripped before it is written — the Python version strips it too — so an
    /// answer that ends with a newline leaves a file that does not: the first case below rewrites a
    /// file with itself and still counts two changed lines, because the last line lost its break.
    /// </para>
    /// <para>
    /// The three long cases are the ones that reach <c>SequenceMatcher</c>'s "popular line" rule: a
    /// line occurring in more than one per cent of a long second version cannot anchor a match, so a
    /// file that repeats one line diffs differently from one that does not. They are the cases a
    /// plain longest-common-subsequence diff would get wrong.
    /// </para>
    /// </remarks>
    public static TheoryData<string, string, int, int> Diffs
    {
        get
        {
            TheoryData<string, string, int, int> cases = [];

            cases.Add("a\nb\nc\n", "a\nb\nc\n", 1, 1);
            cases.Add("a\nb\nc", "a\nB\nc", 1, 1);
            cases.Add("a\nb\nc", "a\nnew\nb\nc", 1, 0);
            cases.Add("a\nb\nc", "a\nc", 0, 1);
            cases.Add("a\nb", "", 0, 2);
            cases.Add("", "a\nb", 2, 0);
            cases.Add("a\na\na", "a", 0, 2);
            cases.Add("a\nb\nc\nd", "c\nd\na\nb", 3, 3);
            cases.Add("a\r\nb\r\n", "a\r\nB\r\n", 1, 1);

            // Three hundred identical lines reduced to two hundred and fifty: every line is
            // "popular", so none of them can anchor a match and the diff falls back to the ends.
            cases.Add(Repeat("x\n", 300), Repeat("x\n", 250), 1, 51);

            // Two hundred and fifty unique lines, one changed, one removed, one added after.
            var unique = string.Concat(Enumerable.Range(0, 250).Select(index => $"line{index}\n"));

            cases.Add(
                unique,
                unique
                    .Replace("line120\n", "changed120\n", StringComparison.Ordinal)
                    .Replace("line121\n", string.Empty, StringComparison.Ordinal)
                    .Replace("line200\n", "line200\nextra\n", StringComparison.Ordinal),
                3,
                3);

            // Four hundred lines where every tenth is the same word: that word is popular and cannot
            // anchor a match, and one of them is still matched because a block grows over it.
            cases.Add(Mixed(400, "u"), Mixed(400, "v"), 399, 399);

            return cases;
        }
    }

    public void Dispose() => _folder.Dispose();

    /// <summary>Starts a host whose editor is on or off, with the model it is given.</summary>
    /// <param name="vibe">Whether the application is in Vibe mode.</param>
    /// <param name="model">The backend to register, or <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <param name="demoPath">The file to edit, or <see langword="null"/> for one of the test's own.</param>
    /// <returns>The host.</returns>
    private Task<GradioTestHost> StartAsync(
        bool vibe,
        IVibeLanguageModel? model,
        CancellationToken cancellationToken,
        string? demoPath = null)
    {
        var demo = demoPath ?? _folder.Write("demo.py", DemoCode);

        return GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            services =>
            {
                services.AddSingleton(new VibeOptions { DemoPath = demo });

                if (model is not null)
                {
                    services.AddSingleton(model);
                }
            },
            blocks =>
            {
                blocks.VibeMode = vibe;

                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Anything" });
                }
            });
    }

    /// <summary>Asks for an edit and reads the answer.</summary>
    /// <param name="client">The client.</param>
    /// <param name="prompt">What to ask for.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer.</returns>
    private static async Task<JsonObject> EditAsync(
        HttpClient client,
        string prompt,
        CancellationToken cancellationToken)
    {
        using var response = await client.PostAsJsonAsync(
            "/gradio_api/vibe-edit",
            new { prompt },
            cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"The edit was answered {(int)response.StatusCode}: {body}");

        return JsonNode.Parse(body)!.AsObject();
    }

    /// <summary>The config's answer about whether the editor is on.</summary>
    /// <param name="client">The client.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The flag.</returns>
    private static async Task<bool> ConfigVibeModeAsync(HttpClient client, CancellationToken cancellationToken) =>
        JsonNode.Parse(await client.GetStringAsync("/config", cancellationToken))!["vibe_mode"]!.GetValue<bool>();

    /// <summary>The <c>detail</c> of a refusal, which is the sentence a caller reads.</summary>
    /// <param name="response">The refusal.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The detail, as the text it is.</returns>
    private static async Task<string> DetailAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken) =>
        JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!["detail"]!.GetValue<string>();

    /// <summary>A line repeated a number of times.</summary>
    /// <param name="line">The line, ending in its newline.</param>
    /// <param name="count">How many.</param>
    /// <returns>The text.</returns>
    private static string Repeat(string line, int count) => string.Concat(Enumerable.Repeat(line, count));

    /// <summary>A long file where every tenth line is the same word.</summary>
    /// <param name="count">How many lines.</param>
    /// <param name="prefix">What the other lines are named after.</param>
    /// <returns>The text.</returns>
    private static string Mixed(int count, string prefix) =>
        string.Concat(
            Enumerable
                .Range(0, count)
                .Select(index => index % 10 == 0 ? "dup\n" : $"{prefix}{index}\n"));

    /// <summary>A backend that answers what it was told to, and remembers what it was asked.</summary>
    private sealed class FakeModel : IVibeLanguageModel
    {
        private readonly Queue<string?> _answers = new();

        internal List<VibeCompletionRequest> Requests { get; } = [];

        /// <summary>Queues an answer.</summary>
        /// <param name="answer">What the model says.</param>
        internal void Answer(string? answer) => _answers.Enqueue(answer);

        /// <inheritdoc />
        public Task<string?> CompleteAsync(VibeCompletionRequest request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);

            return Task.FromResult(_answers.Count > 0 ? _answers.Dequeue() : null);
        }
    }

    /// <summary>A backend that cannot reach its model.</summary>
    private sealed class FailingModel : IVibeLanguageModel
    {
        /// <inheritdoc />
        public Task<string?> CompleteAsync(VibeCompletionRequest request, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("the model is not reachable");
    }
}

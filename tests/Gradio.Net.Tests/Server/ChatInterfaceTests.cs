using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A conversation built in one call: what the chat draws, what a message goes through, and what
/// the pieces an application asks for do when they are used.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the behaviour a document cannot show: that a message reaches the
/// handler and its answer reaches the conversation, that an additional input is read and an
/// additional output is filled, that a saved conversation is listed and reopened, that a sample
/// runs when it is clicked, and that a multimodal field's files arrive as files.
/// </para>
/// <para>
/// The runs go through the endpoints a browser uses, and the conversations are asserted as JSON,
/// for the reason <see cref="ChatbotTests"/> gives: what the frontend renders is the document, not
/// the C# object this side happened to build. A chain is driven the way a browser drives one — the
/// field's own step first, then the step that answers — because the value a message was is kept in
/// a state between the two rather than sent back by the client.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class ChatInterfaceTests
{
    [Fact]
    public async Task A_chat_describes_itself_as_one()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                title: "Chatter",
                description: "Say hello",
                fillHeight: false,
                fillWidth: true,
                deleteCache: new DeleteCacheSettings(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20))));

        var config = await ConfigAsync(host);

        Assert.Equal("chat_interface", config["mode"]!.GetValue<string>());
        Assert.Equal("Chatter", config["title"]!.GetValue<string>());
        Assert.False(config["fill_height"]!.GetValue<bool>());
        Assert.True(config["fill_width"]!.GetValue<bool>());

        // The heading and the description are drawn above the conversation, as markdown, and the
        // heading is centred because a chat is the whole page rather than a part of one.
        var headings = config["components"]!.AsArray()
            .Where(entry => entry!["type"]!.GetValue<string>() == "markdown")
            .Select(entry => entry!["props"]!["value"]!.GetValue<string>())
            .ToArray();

        Assert.Equal(
            ["<h1 style='text-align: center; margin-bottom: 1rem'>Chatter</h1>", "Say hello"],
            headings);

        // The field is the chat's own: a message box with no label of its own, because the chat is
        // what it belongs to.
        var field = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "textbox")!;

        Assert.Equal(string.Empty, field["props"]!["label"]!.GetValue<string>());
        Assert.False(field["props"]!["show_label"]!.GetValue<bool>());
        Assert.Equal(
            "__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"chat_interface.message_placeholder\"}",
            field["props"]!["placeholder"]!.GetValue<string>());

        // The openapi document describes the application under the title and description it was
        // given, because a chat is the application rather than a component of one.
        using var response = await host.Client.GetAsync(
            "/gradio_api/openapi.json",
            TestContext.Current.CancellationToken);
        var openApi = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal("Say hello", openApi["info"]!["description"]!.GetValue<string>());
    }

    /// <summary>
    /// A chat's handler is told which run it is answering, the way any other handler is.
    /// </summary>
    /// <remarks>
    /// A chat binds a wrapper of its own and calls the application's handler from inside it, so the
    /// request has to be carried across that step rather than stopping at the wrapper. The parameter
    /// is not one of the binding's inputs: a chat whose handler declares one still reads the message
    /// and the conversation, which is what a build that counted it as an input would get wrong.
    /// </remarks>
    [Fact]
    public async Task A_chat_handler_reads_the_request_the_run_belongs_to()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, Request, string>)(
                    (message, _, request) => $"{message} from {request.SessionHash}")));

        await SendAsync(host, "hello", "chat-session");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "chat-session");

        // The answer goes into the conversation, which is the chat's output; what it says is the
        // handler's own words, so the request reached it.
        Assert.Contains(
            """content":[{"text":"hello from chat-session","type":"text"}""",
            data[1]!.ToJsonString(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_message_is_answered_and_the_conversation_keeps_both()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string, string>)Answer,
                additionalInputs: new object?[] { new Textbox { Value = "polite" } }));

        // The field's own step keeps the message, and the step that answers reads it back: the
        // message is not an argument the client sends twice.
        await SendAsync(host, "hello", "answer");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray(), "polite"), "answer");

        // The answer goes into the conversation rather than to an output of its own, and the
        // conversation is what the browser is told to draw.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null},{"role":"assistant","metadata":null,"content":[{"text":"polite: hello","type":"text"}],"options":null}]""",
            data[1]!.ToJsonString());
    }

    [Fact]
    public async Task The_endpoint_is_documented_with_the_message_and_not_the_conversation()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo));

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        var info = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        var endpoint = info["named_endpoints"]!["/Echo"]!;

        // A conversation is a state rather than an argument: the client sends the message, and the
        // history is what the chat keeps between the steps of a chain. The Python version skips such
        // components where it builds the document, so an endpoint documented with two parameters —
        // the message and the conversation — would be documenting something nobody can send.
        var parameter = Assert.Single(endpoint["parameters"]!.AsArray())!;

        Assert.Equal("message", parameter["parameter_name"]!.GetValue<string>());
        Assert.Equal("Textbox", parameter["component"]!.GetValue<string>());

        Assert.Single(endpoint["returns"]!.AsArray());
    }

    [Fact]
    public async Task The_field_is_emptied_and_put_out_of_the_way_when_a_message_is_sent()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo));

        var data = await RunAsync(host, "_clear_and_save_textbox", new JsonArray("hello"), "clearing");

        // What the client is told to do with the field: empty it, stop it accepting text, and take
        // the hint away, because a hint under a field that is being answered is in the way.
        var update = data[0]!.AsObject();

        Assert.Equal(string.Empty, update["value"]!.GetValue<string>());
        Assert.False(update["interactive"]!.GetValue<bool>());
        Assert.Equal(string.Empty, update["placeholder"]!.GetValue<string>());
    }

    [Fact]
    public async Task An_additional_output_receives_what_the_handler_answered_with()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                var note = new Textbox { Label = "Note" };

                using (blocks.BeginRender())
                {
                    blocks.Add(note);
                }

                _ = new ChatInterface(
                    blocks,
                    (Func<string, IReadOnlyList<ChatMessage>, string[]>)AnswerWithNote,
                    additionalOutputs: new object?[] { note });
            });

        await SendAsync(host, "hello", "outputs");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "outputs");

        // One value per output: the chat's own — the state it discards, then the conversation —
        // and then the application's, in the order it gave them.
        Assert.Null(data[0]);
        Assert.Equal(3, data.Count);
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null},{"role":"assistant","metadata":null,"content":[{"text":"hello!","type":"text"}],"options":null}]""",
            data[1]!.ToJsonString());
        Assert.Equal("you said hello", data[2]!.GetValue<string>());
    }

    [Fact]
    public async Task A_saved_conversation_is_listed_and_reopened()
    {
        var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                saveHistory: true));

        await SendAsync(host, "hello", "history");

        // The conversation is kept in the states the chat publishes, which is where a save reads it
        // from: synchronizing is what a finished answer does.
        await RunAsync(
            host,
            "lambda",
            new JsonArray(
                new JsonArray(
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = new JsonArray(new JsonObject { ["text"] = "hello", ["type"] = "text" }),
                    })),
            "history");

        // Saving a conversation that was not open yet puts it at the top of the list and makes it
        // the open one. The list is a value the browser keeps rather than the server, so what the
        // run answers with is the list sealed, which is what the browser stores and sends back.
        var saved = await RunAsync(host, "_save_conversation", new JsonArray(null, null, null), "history", logs);
        var conversations = saved[1]!.DeepClone();

        Assert.NotEqual(JsonValueKind.Null, conversations.GetValueKind());

        // The list is what the side panel draws: one entry per conversation, titled with the first
        // thing that was said in it.
        var listed = await RunAsync(host, "_load_chat_history", new JsonArray(conversations.DeepClone()), "history");

        Assert.Equal(
            """{"samples":[["hello"]],"__type__":"update"}""",
            listed[0]!.ToJsonString());

        // Opening an entry hands the conversation back to the conversation component.
        var opened = await RunAsync(host, "_load_conversation", new JsonArray(0, conversations.DeepClone()), "history");

        // The open conversation's own index is a state, so the client is told nothing about it;
        // what it is told is the conversation.
        Assert.Null(opened[0]);
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]""",
            opened[1]!.ToJsonString());
    }

    [Fact]
    public async Task A_reopened_conversation_is_saved_where_it_came_from_and_cleared_from_the_list()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                saveHistory: true));

        var conversation = new JsonArray(
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = new JsonArray(new JsonObject { ["text"] = "hello", ["type"] = "text" }),
            });

        await SendAsync(host, "hello", "reopening");
        await RunAsync(host, "lambda", new JsonArray(conversation.DeepClone()), "reopening");

        var saved = await RunAsync(host, "_save_conversation", new JsonArray(null, null, null), "reopening");
        var conversations = saved[1]!.DeepClone();

        // Opening the conversation is what puts its position into the state the save reads, which is
        // the path a reader takes: click an entry, type, send.
        await RunAsync(host, "_load_conversation", new JsonArray(0, conversations.DeepClone()), "reopening");
        await SendAsync(host, "again", "reopening");
        await RunAsync(host, "lambda", new JsonArray(conversation.DeepClone()), "reopening");

        // The index the state hands back is a number read out of JSON rather than an int, and the
        // save has to read it as one: a chat that treated it as "no conversation is open" would
        // save a second copy of the conversation every time it was reopened. What the run answers
        // with is the list sealed, so the list is read back through the step that lists it.
        var updated = await RunAsync(
            host,
            "_save_conversation",
            new JsonArray(null, null, conversations.DeepClone()),
            "reopening");

        var listed = await RunAsync(
            host,
            "_load_chat_history",
            new JsonArray(updated[1]!.DeepClone()),
            "reopening");

        Assert.Single(listed[0]!["samples"]!.AsArray());

        // Clearing the conversation forgets the entry it was opened from, rather than only
        // emptying what the page shows.
        await RunAsync(host, "_load_conversation", new JsonArray(0, conversations.DeepClone()), "reopening");

        var cleared = await RunAsync(
            host,
            "_delete_conversation",
            new JsonArray(null, conversations.DeepClone()),
            "reopening");

        var remaining = await RunAsync(
            host,
            "_load_chat_history",
            new JsonArray(cleared[1]!.DeepClone()),
            "reopening");

        Assert.Empty(remaining[0]!["samples"]!.AsArray());
    }

    [Fact]
    public async Task A_clicked_sample_fills_the_field_when_the_chat_only_suggests_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                examples: new object?[] { "hello" },
                runExamplesOnClick: false));

        var data = await RunQueuedAsync(
            host,
            "example_populated",
            new JsonArray(),
            "sample",
            new JsonObject
            {
                ["index"] = 0,
                ["value"] = new JsonObject { ["text"] = "hello", ["files"] = new JsonArray() },
            });

        Assert.Equal("hello", data[0]!.GetValue<string>());
    }

    [Fact]
    public async Task A_clicked_sample_runs_when_the_chat_offers_it_as_a_demonstration()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                examples: new object?[] { "hello" },
                runExamplesOnClick: true));

        var data = await RunQueuedAsync(
            host,
            "example_clicked",
            new JsonArray(),
            "running",
            new JsonObject
            {
                ["index"] = 0,
                ["value"] = new JsonObject { ["text"] = "hello", ["files"] = new JsonArray() },
            });

        // The sample goes into the conversation, and its message is handed on to the step that
        // answers it — which is the difference between a sample that runs and one that is only put
        // in the field.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]""",
            data[0]!.ToJsonString());

        var config = await ConfigAsync(host);
        var chain = ChainFrom(config, "example_clicked");

        Assert.Equal("example_clicked", chain[0]);
        Assert.StartsWith("lambda", chain[1], StringComparison.Ordinal);

        // The step that answers is the chat's own submit, which is the one a stop would cancel.
        Assert.StartsWith("_submit_fn", chain[2], StringComparison.Ordinal);

        // Two steps hang off the answer: the one that keeps the conversation, and the one that
        // puts the submit button back where the sample took it from. A sample that left the field
        // showing a stop button after it was answered would be a chat that cannot be typed into.
        var dependencies = config["dependencies"]!.AsArray();
        var answered = dependencies.Single(
            entry => entry!["api_name"]!.GetValue<string>() == chain[2]);
        var followers = dependencies
            .Where(entry => entry!["trigger_after"]?.GetValue<int>()
                == answered!["id"]!.GetValue<int>())
            .ToArray();
        var field = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "textbox")!["id"]!.GetValue<int>();

        Assert.Equal(2, followers.Length);
        Assert.Contains(followers, entry => entry!["outputs"]![0]!.GetValue<int>() == field);
    }

    [Fact]
    public async Task A_multimodal_chat_takes_files_with_the_message()
    {
        var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<MultimodalText, IReadOnlyList<ChatMessage>, string>)Read,
                multimodal: true));

        var config = await ConfigAsync(host);

        Assert.Contains(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "multimodaltextbox");

        // What the field hands back is the value the browser keeps: the files, and the words about
        // them, rather than one string.
        await RunAsync(
            host,
            "_clear_and_save_textbox",
            new JsonArray(new JsonObject
            {
                ["text"] = "look",
                ["files"] = new JsonArray(
                    new JsonObject
                    {
                        ["path"] = "/tmp/picture.png",
                        ["meta"] = new JsonObject { ["_type"] = "gradio.FileData" },
                    }),
            }),
            "multimodal");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "multimodal", logs);
        var conversation = data[1]!.AsArray();
        var content = conversation[0]!["content"]!.AsArray();

        // The file first, then the words about it: one user message holding both, which is the
        // shape the conversation draws.
        Assert.Equal("file", content[0]!["type"]!.GetValue<string>());
        Assert.Equal("/tmp/picture.png", content[0]!["file"]!["path"]!.GetValue<string>());
        Assert.Equal("look", content[1]!["text"]!.GetValue<string>());
        Assert.Equal("assistant", conversation[1]!["role"]!.GetValue<string>());
    }

    /// <summary>
    /// A file in the conversation reaches the handler as a file it can open.
    /// </summary>
    /// <remarks>
    /// The history is not a transcript to be read: a message holds blocks, and a file block holds
    /// the file itself, so a handler that wants to look at what the user sent earlier can. That is
    /// what makes a conversation about documents possible at all, and it is the half a test of the
    /// current message cannot show.
    /// </remarks>
    [Fact]
    public async Task A_file_in_the_conversation_reaches_the_handler_as_a_file()
    {
        using var folder = new TempFolder();
        var notes = folder.Write("notes.txt", "twelve bytes");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Measure));

        // The conversation the browser sends back, with a file in it: the state is the session's, so
        // the step that keeps the conversation is what puts it there.
        _ = await RunAsync(
            host,
            "lambda",
            new JsonArray(new JsonArray(new JsonObject
            {
                ["role"] = "user",
                ["metadata"] = null,
                ["content"] = new JsonArray(new JsonObject
                {
                    ["file"] = new JsonObject { ["path"] = notes },
                    ["type"] = "file",
                }),
                ["options"] = null,
            })),
            "files");

        await SendAsync(host, "how big is it", "files");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "files");

        // The handler read the file the conversation carried rather than the one the message named.
        var answer = data[1]!.AsArray()[^1]!["content"]![0]!["text"]!.GetValue<string>();

        Assert.Equal("12 bytes", answer);
    }

    /// <summary>
    /// The stop button appears with the label the chat gave it, and only while a run is going.
    /// </summary>
    /// <remarks>
    /// The labels the application set are in the served document. What it cannot show is when each
    /// of them is drawn: the Python version puts both on the field and swaps them around a run, so
    /// a submit sends an update that takes the submit button away and puts the stop button in its
    /// place — with the label the application named, which is the half that would be lost if the
    /// chat kept only the flag.
    /// </remarks>
    [Fact]
    public async Task The_field_carries_the_labels_the_chat_gave_its_buttons()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                submitBtn: "Send it",
                stopBtn: "Halt"));

        var config = await ConfigAsync(host);
        var field = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "textbox")!;

        // The field is drawn with the submit button and no stop button, which is what a chat that is
        // not answering looks like.
        Assert.Equal("Send it", field["props"]!["submit_btn"]!.GetValue<string>());
        Assert.False(field["props"]!["stop_btn"]!.GetValue<bool>());

        // The step a submit is followed by that writes the field: the one that swaps the two
        // buttons around a run. A submit has other followers — the step that keeps the
        // conversation — which is why this one is picked by what it writes rather than by being
        // the next one in the list.
        var fieldId = field["id"]!.GetValue<int>();
        var submit = config["dependencies"]!.AsArray().Single(
            entry => entry!["api_name"]!.GetValue<string>() == "_clear_and_save_textbox")!;
        var showStop = config["dependencies"]!.AsArray().Single(entry =>
            entry!["trigger_after"]?.GetValue<int>() == submit["id"]!.GetValue<int>()
            && entry["outputs"] is JsonArray outputs
            && outputs.Any(id => id!.GetValue<int>() == fieldId))!;

        var update = await RunAsync(
            host,
            showStop["api_name"]!.GetValue<string>(),
            new JsonArray(),
            "labels");

        Assert.Equal(
            """{"submit_btn":false,"stop_btn":"Halt","__type__":"update"}""",
            update[0]!.ToJsonString());
    }

    /// <summary>
    /// A streamed answer grows in the conversation, a chunk at a time.
    /// </summary>
    /// <remarks>
    /// The chat answers a generator by growing the message it is already writing, which is what
    /// makes a slow model look like it is typing rather than like it stopped. A document cannot
    /// show that: the bindings look the same whether the handler streams or not. What is asserted
    /// here is the sequence the queue sends — the conversation once, then only what changed — and
    /// the conversation the run finishes with.
    /// </remarks>
    [Fact]
    public async Task A_streamed_answer_grows_in_the_conversation_as_it_is_written()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, IAsyncEnumerable<string>>)Speak));

        using var queue = new QueueClient(host.BaseAddress);

        // The field's own step is what keeps the message: the step that answers reads it out of the
        // state rather than being handed it twice.
        await SendAsync(host, "hello", "streamed");

        var fnIndex = await queue.EndpointAsync("_submit_fn", cancellationToken);
        var eventId = await queue.JoinAsync(
            fnIndex,
            new JsonArray(null, new JsonArray()),
            "streamed",
            cancellationToken);

        await using var stream = await queue.OpenAsync("streamed", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        var chunks = stream.Messages
            .Where(message => QueueSession.Kind(message) == "process_generating"
                && message["event_id"]?.GetValue<string>() == eventId)
            .Select(message => message["output"]!["data"]![1])
            .ToArray();

        // The first chunk is the conversation as it stands — the question, and the answer begun —
        // and the ones after it are what changed since: the assistant's message is the same one,
        // and only the text it is missing is sent. That is what makes a long answer cheap on the
        // wire, and it is a thing no document can show.
        Assert.Equal(3, chunks.Length);

        Assert.Equal(["hello", "he"], MessagesOf(chunks[0]));

        Assert.Equal(
            """[["append",[1,"content",0,"text"],"llo"]]""",
            chunks[1]!.ToJsonString());

        Assert.Equal(
            """[["append",[1,"content",0,"text"]," there"]]""",
            chunks[2]!.ToJsonString());

        // And what the run finishes with is the whole answer in one message, under the question.
        var finished = completed!["output"]!["data"]![1]!;

        Assert.Equal(["hello", "hello there"], MessagesOf(finished));
    }

    /// <summary>
    /// A retry asks the last question again, with the answer that was wrong taken out first.
    /// </summary>
    /// <remarks>
    /// The Python version's retry is a chain rather than one step: the last exchange comes off the
    /// conversation, the question goes back into the history, and the chat answers it again. What
    /// is asserted is the first step — the conversation without the exchange, and the question
    /// handed on — because that is the part a client's payload decides, and the answer after it is
    /// the same step a submit uses.
    /// </remarks>
    [Fact]
    public async Task A_retry_takes_the_last_exchange_out_and_hands_the_question_on()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo));

        // The retry reads the conversation from the state a finished answer writes, which is the
        // step that synchronizes it.
        _ = await RunAsync(host, "lambda", new JsonArray(Conversation("hello")), "retry");

        // The pop writes two states — the shortened conversation and the question — and a state is
        // the server's, so nothing about it reaches the client. What the retry did is therefore
        // asserted through the step that reads them, which is the next one in the chain.
        var config = await ConfigAsync(host);
        var pop = BindingOf(config, "retry");

        _ = await RunAsync(host, pop["api_name"]!.GetValue<string>(), new JsonArray(new JsonNode?[] { null }), "retry");

        var append = NextStepOf(config, pop);

        var data = await RunAsync(
            host,
            append["api_name"]!.GetValue<string>(),
            new JsonArray(new JsonNode?[] { null, null }),
            "retry");

        // The answer is gone and the question is back, which is what the chat answers again.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]""",
            data[0]!.ToJsonString());
    }

    /// <summary>
    /// An undo takes the last exchange back and puts the question where the user can change it.
    /// </summary>
    /// <remarks>
    /// The same step as a retry, bound to a different signal: the Python version's undo pops the
    /// exchange and leaves the question in the field rather than answering it again, which is the
    /// difference between "say that differently" and "say it again".
    /// </remarks>
    [Fact]
    public async Task An_undo_takes_the_last_exchange_back_and_puts_the_question_in_the_field()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo));

        var undo = BindingOf(await ConfigAsync(host), "undo");

        var data = await RunAsync(
            host,
            undo["api_name"]!.GetValue<string>(),
            new JsonArray(Conversation("hello")),
            "undo");

        // The conversation is what is left, and the question is the field's new value — which is
        // what an undo is for: the user edits what they said rather than saying it again.
        Assert.Empty(data[0]!.AsArray());
        Assert.Equal("hello", data[1]!.GetValue<string>());
    }

    /// <summary>
    /// An edited message cuts the conversation back to it, and the edit becomes the next question.
    /// </summary>
    /// <remarks>
    /// The Python version's edit is not "change one message": the messages after the edit are
    /// dropped, because they answered something that is no longer being said, and the new text is
    /// handed on as the question the chat answers. The payload is what decides where the cut is,
    /// which is why the event data is sent rather than a value.
    /// </remarks>
    [Fact]
    public async Task An_edited_message_cuts_the_conversation_back_to_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                editable: true));

        var data = await RunQueuedAsync(
            host,
            "_edit_message",
            new JsonArray(Conversation("hello")),
            "edited",
            new JsonObject
            {
                ["index"] = 0,
                ["value"] = "goodbye",
                ["previous_value"] = "hello",
            });

        // Nothing is left of the exchange: an edit at the first message drops the answer that was
        // given to something no longer being said.
        Assert.Empty(data[0]!.AsArray());

        // The edit and the conversation it leaves are both kept in states, which is why the client
        // is told nothing about the two outputs after the conversation.
        Assert.Null(data[1]);
        Assert.Null(data[2]);

        // And the edit is what the chat answers next: running the step the chain goes to, with
        // nothing but the states it reads, shows the conversation it builds — the edit as the
        // question, and nothing else.
        var config = await ConfigAsync(host);
        var following = NextStepOf(config, BindingOf(config, "edit"));

        var answered = await RunAsync(
            host,
            following!["api_name"]!.GetValue<string>(),
            new JsonArray(new JsonNode?[] { null, null }),
            "edited");

        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"goodbye","type":"text"}],"options":null}]""",
            answered[0]!.ToJsonString());
    }

    /// <summary>
    /// A rated message is written to the log the application keeps, one row per rating.
    /// </summary>
    /// <remarks>
    /// A chat has no sample to flag, only a conversation and a position in it, so the file it writes
    /// is a different one from an interface's: the row names the conversation, the message, how it
    /// was rated and when. The payload is what says which message and how, which is why the event
    /// data is what the run is given.
    /// </remarks>
    [Fact]
    public async Task A_rated_message_is_written_to_the_log()
    {
        using var folder = new TempFolder();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                flaggingMode: "manual",
                flaggingDir: folder.Folder));

        var data = await RunQueuedAsync(
            host,
            "flag",
            new JsonArray(Conversation("hello")),
            "rated",
            new JsonObject { ["index"] = 1, ["value"] = "hello", ["liked"] = true });

        // Rating a message draws nothing: the run's whole effect is the row it writes.
        Assert.Empty(data);

        var rows = await System.IO.File.ReadAllLinesAsync(
            Path.Combine(folder.Folder, "log.csv"),
            TestContext.Current.CancellationToken);

        Assert.Equal("conversation,index,value,flag,timestamp", rows[0]);

        // The conversation is one cell, the message's position is the next, and the rating is the
        // third: four cells under five headers, which is the Python version's own row and is reproduced
        // because the file is the artifact.
        Assert.StartsWith("\"", rows[1], StringComparison.Ordinal);
        Assert.Contains(",1,Like,", rows[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_additional_input_already_on_the_page_is_not_drawn_again()
    {
        Textbox tone = new() { Label = "Tone" };

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(tone);
                }

                _ = new ChatInterface(
                    blocks,
                    (Func<string, IReadOnlyList<ChatMessage>, string, string>)Answer,
                    additionalInputs: tone);
            });

        var config = await ConfigAsync(host);

        // The component the application drew is drawn once, and the accordion that would hold it is
        // not drawn at all: there is nothing left for it to hold.
        Assert.Single(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "textbox"
                && entry["props"]!["label"]?.GetValue<string>() == "Tone");

        Assert.DoesNotContain(
            config["components"]!.AsArray(),
            entry => entry!["type"]!.GetValue<string>() == "accordion");
    }

    [Fact]
    public async Task A_conversation_the_application_gave_keeps_its_editing_rule_and_draws_the_chats_samples()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                chatbot: new Chatbot { Editable = "all" },
                editable: true,
                examples: new object?[] { "hello" }));

        var config = await ConfigAsync(host);
        var conversation = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "chatbot")!;

        // The chat's flag fills in an editing rule the component did not have rather than
        // replacing one it has: "all" stays "all".
        Assert.Equal("all", conversation["props"]!["editable"]!.GetValue<string>());

        // The samples are the chat's, drawn inside the conversation the application supplied.
        var samples = conversation["props"]!["examples"]!.AsArray();

        Assert.Single(samples);
        Assert.Equal("hello", samples[0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_field_the_application_gave_keeps_its_label_and_hides_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                textbox: new Textbox { Label = "Say something" }));

        var config = await ConfigAsync(host);
        var field = config["components"]!.AsArray()
            .Single(entry => entry!["type"]!.GetValue<string>() == "textbox")!;

        // The label the application gave is kept — the field's configuration is the
        // application's — and hidden, because the chat is what the field belongs to.
        Assert.Equal("Say something", field["props"]!["label"]!.GetValue<string>());
        Assert.False(field["props"]!["show_label"]!.GetValue<bool>());
    }

    [Fact]
    public async Task A_chat_without_a_description_says_it_has_none()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo));

        using var response = await host.Client.GetAsync(
            "/gradio_api/openapi.json",
            TestContext.Current.CancellationToken);
        var openApi = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        // A chat is the application, so its description is the application's — and "none" is a
        // value: the document says null rather than pretending the description is empty.
        Assert.True(openApi["info"]!.AsObject().ContainsKey("description"));
        Assert.Null(openApi["info"]!["description"]);
    }

    [Fact]
    public async Task A_component_name_stands_for_the_component_it_names()
    {
        ChatInterface? chat = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => chat = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string, string, string>)AnswerWithTwoInputs,
                additionalInputs: new object?[] { "audiogallery", "coloredcheckboxgroup" }));

        var config = await ConfigAsync(host);

        // The shortcut is part of the served application: each name becomes the component it names,
        // with every prop at its default, which is what the Python version's string shortcuts do. Both of
        // these are custom-HTML components, so what the config calls them is `html` — the class is
        // what the name reached, and that is what the test reads.
        Assert.IsType<AudioGallery>(chat!.AdditionalInputs[0]);
        Assert.IsType<ColoredCheckboxGroup>(chat.AdditionalInputs[1]);

        Assert.Equal(
            2,
            config["components"]!.AsArray()
                .Count(entry => entry!["type"]!.GetValue<string>() == "html"));
    }

    [Fact]
    public void A_chat_refuses_a_conversation_or_a_field_the_application_already_drew()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var field = new Textbox { Label = "Drawn" };
        var conversation = new Chatbot();

        using (blocks.BeginRender())
        {
            blocks.Add(field);
            blocks.Add(conversation);
        }

        // The chat draws its own field and its own conversation, so a drawn one has nowhere to go:
        // the Python version unrenders it first, which is a step this API does not have because nothing here
        // is drawn until it is added.
        var fieldError = Assert.Throws<ArgumentException>(() =>
        {
            _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                textbox: field);
        });

        var conversationError = Assert.Throws<ArgumentException>(() =>
        {
            _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                chatbot: conversation);
        });

        Assert.Equal("textbox", fieldError.ParamName);
        Assert.Equal("chatbot", conversationError.ParamName);
    }

    [Fact]
    public async Task A_chat_takes_the_analytics_switch_it_was_given()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                analyticsEnabled: true));

        var config = await ConfigAsync(host);

        // Off unless asked for, which is this framework's one deliberate departure from the Python version;
        // a chat asked for it says so, because the switch is the application's either way.
        Assert.True(config["analytics_enabled"]!.GetValue<bool>());
    }

    [Fact]
    public void A_chat_refuses_a_field_it_cannot_read()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var error = Assert.Throws<ArgumentException>(() =>
        {
            _ = new ChatInterface(
                blocks,
                (Func<string, IReadOnlyList<ChatMessage>, string>)Echo,
                textbox: new Number());
        });

        Assert.Equal("textbox", error.ParamName);
    }

    /// <summary>The served config, with the request's own fields in place.</summary>
    /// <param name="host">The host.</param>
    /// <returns>The config.</returns>
    private static async Task<JsonObject> ConfigAsync(GradioTestHost host)
    {
        using var response = await host.Client.GetAsync(
            "/config",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();
    }

    /// <summary>The binding a component's event is served by.</summary>
    /// <param name="config">The served config.</param>
    /// <param name="eventName">The event, as the frontend names it: <c>retry</c>, <c>edit</c>.</param>
    /// <returns>The dependency.</returns>
    /// <exception cref="Xunit.Sdk.XunitException">Nothing is bound to that event.</exception>
    /// <remarks>
    /// <para>
    /// A chat's own steps are found by the signal they answer rather than by name, because the names
    /// are de-duplicated: the second step called <c>_pop_last_user_message</c> is served as
    /// <c>_pop_last_user_message_1</c>, which is a property of the order the chat happened to bind
    /// them in rather than of what the step does.
    /// </para>
    /// <para>
    /// One signal may be answered by more than one step — a retry pops the conversation and also
    /// puts the stop button out — so the one wanted here is the one that reads something, which is
    /// the step that does the work rather than the one that redraws a button.
    /// </para>
    /// </remarks>
    private static JsonNode BindingOf(JsonObject config, string eventName) =>
        config["dependencies"]!.AsArray()
            .Where(entry => entry!["targets"] is JsonArray targets
                && targets.Count > 0
                && targets[0]![1]!.GetValue<string>() == eventName)
            .OrderByDescending(entry => (entry!["inputs"] as JsonArray)?.Count ?? 0)
            .First()!;

    /// <summary>The step a binding's chain goes to when it finishes.</summary>
    /// <param name="config">The served config.</param>
    /// <param name="binding">The binding.</param>
    /// <returns>The step.</returns>
    /// <exception cref="Xunit.Sdk.XunitException">The binding is the last of its chain.</exception>
    /// <remarks>
    /// The first one, because a step may be followed by several: a submit is followed by the step
    /// that keeps the conversation and by the one that puts the stop button away, and which of them
    /// comes first is the order the chat bound them in. The chains this is used on — a retry, an
    /// edit — are single-file, so the first is the only one.
    /// </remarks>
    private static JsonNode NextStepOf(JsonObject config, JsonNode binding) =>
        config["dependencies"]!.AsArray().FirstOrDefault(
            entry => entry!["trigger_after"]?.GetValue<int>() == binding["id"]!.GetValue<int>())
        ?? throw new InvalidOperationException($"'{binding["api_name"]}' is the last of its chain.");

    /// <summary>The names of the bindings a chain runs through, starting at one of them.</summary>
    /// <param name="config">The served config.</param>
    /// <param name="apiName">The binding the chain starts at.</param>
    /// <returns>The names, in the order the chain runs them.</returns>
    private static List<string> ChainFrom(JsonObject config, string apiName)
    {
        var dependencies = config["dependencies"]!.AsArray();
        var names = new List<string>();
        var current = dependencies.Single(
            entry => entry!["api_name"]!.GetValue<string>() == apiName);

        while (current is not null && names.Count < 8)
        {
            names.Add(current["api_name"]!.GetValue<string>());

            var id = current["id"]!.GetValue<int>();
            current = dependencies.FirstOrDefault(
                entry => entry!["trigger_after"]?.GetValue<int>() == id);
        }

        return names;
    }

    /// <summary>Sends a message the way the field does, which is what keeps it for the run.</summary>
    /// <param name="host">The host.</param>
    /// <param name="message">The message: a text, or the value a multimodal field holds.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <returns>A task that completes when the field has been emptied.</returns>
    private static async Task SendAsync(GradioTestHost host, JsonNode message, string sessionHash) =>
        _ = await RunAsync(host, "_clear_and_save_textbox", new JsonArray(message), sessionHash);

    /// <summary>Runs one binding once and returns what its outputs carried.</summary>
    /// <param name="host">The host.</param>
    /// <param name="apiName">The binding's name.</param>
    /// <param name="data">The values its inputs are read from.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <returns>One value per output.</returns>
    private static async Task<JsonArray> RunAsync(
        GradioTestHost host,
        string apiName,
        JsonArray data,
        string sessionHash,
        CapturingLoggerProvider? logs = null)
    {

        var body = new JsonObject
        {
            ["data"] = data,
            ["session_hash"] = sessionHash,
        };

        using var response = await host.Client.PostAsync(
            $"/gradio_api/run/{apiName}",
            new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
            TestContext.Current.CancellationToken);

        var answer = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"'{apiName}' answered {response.StatusCode}: {answer.ToJsonString()} :: "
            + string.Join(" | ", logs?.Entries.Select(entry => entry.Message + " :: " + entry.Exception) ?? []));

        return answer["data"]!.AsArray();
    }

    /// <summary>Runs one binding through the queue, which is where an event's own payload is read.</summary>
    /// <param name="host">The host.</param>
    /// <param name="apiName">The binding's name.</param>
    /// <param name="data">The values its inputs are read from.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="eventData">What the event says about the thing that raised it.</param>
    /// <returns>One value per output.</returns>
    private static async Task<JsonArray> RunQueuedAsync(
        GradioTestHost host,
        string apiName,
        JsonArray data,
        string sessionHash,
        JsonObject eventData)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync(apiName, cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, data, sessionHash, cancellationToken, eventData);

        await using var stream = await queue.OpenAsync(sessionHash, cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        Assert.True(
            completed!["output"]?["data"] is JsonArray,
            $"'{apiName}' answered: {completed.ToJsonString()}");

        return completed["output"]!["data"]!.AsArray();
    }

    /// <summary>Answers a multimodal message with the words that came with it.</summary>
    private static string Read(MultimodalText message, IReadOnlyList<ChatMessage> history) => message.Text;

    /// <summary>Reports how big the first file in the conversation is.</summary>
    /// <param name="message">The message, which this answer ignores.</param>
    /// <param name="history">The conversation.</param>
    /// <returns>The size of the first file it holds, or that it holds none.</returns>
    private static string Measure(string message, IReadOnlyList<ChatMessage> history)
    {
        foreach (var said in history)
        {
            foreach (var block in said.Content)
            {
                if (block is FileContent { File.Path: { } path })
                {
                    return $"{new FileInfo(path).Length} bytes";
                }
            }
        }

        return "no file";
    }

    /// <summary>Answers a message unchanged.</summary>
    private static string Echo(string message, IReadOnlyList<ChatMessage> history) => message;

    /// <summary>The text of every message in a conversation, in order.</summary>
    /// <param name="conversation">The conversation.</param>
    /// <returns>One entry per message.</returns>
    private static string[] MessagesOf(JsonNode? conversation) =>
        [.. conversation!.AsArray().Select(message => message!["content"]![0]!["text"]!.GetValue<string>())];

    /// <summary>A conversation of one question and its answer, as the chat serves it.</summary>
    /// <param name="text">What was said.</param>
    /// <returns>The two messages, as the JSON a client sends back.</returns>
    private static JsonNode Conversation(string text) => JsonNode.Parse(
        $$"""
        [
          {"role":"user","metadata":null,"content":[{"text":"{{text}}","type":"text"}],"options":null},
          {"role":"assistant","metadata":null,"content":[{"text":"{{text}}","type":"text"}],"options":null}
        ]
        """)!;

    /// <summary>Answers a message a piece at a time, each piece the whole answer so far.</summary>
    /// <param name="message">The message, which this answer ignores.</param>
    /// <param name="history">The conversation, which this answer ignores.</param>
    /// <returns>The answer as it grows.</returns>
    private static async IAsyncEnumerable<string> Speak(
        string message,
        IReadOnlyList<ChatMessage> history)
    {
        foreach (var word in new[] { "he", "hello", "hello there" })
        {
            yield return word;

            await Task.Yield();
        }
    }

    /// <summary>Answers a message in the tone the additional input names.</summary>
    private static string Answer(string message, IReadOnlyList<ChatMessage> history, string tone) =>
        $"{tone}: {message}";

    /// <summary>Answers a message with what the two additional inputs say about it.</summary>
    private static string AnswerWithTwoInputs(
        string message,
        IReadOnlyList<ChatMessage> history,
        string tone,
        string mood) =>
        $"{tone}, {mood}: {message}";

    /// <summary>Answers a message, and says something about it in the additional output.</summary>
    private static string[] AnswerWithNote(string message, IReadOnlyList<ChatMessage> history) =>
        [$"{message}!", $"you said {message}"];
}

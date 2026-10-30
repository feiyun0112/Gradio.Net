using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A chat loaded from an OpenAI-compatible endpoint: what it sends, what it does with the answer,
/// and what it draws.
/// </summary>
/// <remarks>
/// <para>
/// No test here talks to a model. The endpoint is <see cref="FakeService"/>, which records the
/// request and replays a response in the protocol's shape, so what is asserted is the request the
/// loader builds and the reading of an answer rather than a vendor's behaviour. A real endpoint
/// that speaks the protocol is expected to work; that it does is
/// not something this repository can show.
/// </para>
/// <para>
/// The runs go through the routes a browser uses, and the conversation is asserted as JSON, for the
/// reason <see cref="ChatInterfaceTests"/> gives: what the frontend renders is the document.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class LoadChatTests
{
    /// <summary>The text extensions the Python version's chat accepts, in the Python version's
    /// order.</summary>
    private static readonly IReadOnlyList<string> TextExtensions =
    [
        ".doc", ".docx", ".rtf", ".epub", ".odt", ".odp", ".pptx", ".txt", ".md", ".py", ".ipynb",
        ".js", ".jsx", ".html", ".css", ".java", ".cs", ".php", ".c", ".cc", ".cpp", ".cxx", ".cts",
        ".h", ".hh", ".hpp", ".rs", ".R", ".Rmd", ".swift", ".go", ".rb", ".kt", ".kts", ".ts",
        ".tsx", ".m", ".mm", ".mts", ".scala", ".dart", ".lua", ".pl", ".pm", ".t", ".sh", ".bash",
        ".zsh", ".bat", ".coffee", ".csv", ".log", ".ini", ".cfg", ".config", ".json", ".proto",
        ".yaml", ".yml", ".toml", ".sql",
    ];

    [Fact]
    public async Task A_message_is_sent_to_the_endpoint_as_the_protocol_asks_it()
    {
        var service = new FakeService().Streaming("Hel", "lo");

        await using var host = await StartAsync(service);

        await SendAsync(host, Said("hello"), "one");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "one");

        var call = Assert.Single(service.Calls);

        Assert.Equal("POST", call.Method);
        Assert.Equal("http://model.test/v1/chat/completions", call.Address);

        // The key a local server expects when it checks none is the placeholder the OpenAI client
        // sends, which is what makes a token optional.
        Assert.Equal("Bearer ***", call.Authorization);

        var body = JsonNode.Parse(call.Body)!.AsObject();

        Assert.Equal("llama3.2", body["model"]!.GetValue<string>());
        Assert.True(body["stream"]!.GetValue<bool>());

        var messages = body["messages"]!.AsArray();

        var asked = Assert.Single(messages)!.AsObject();

        Assert.Equal("user", asked["role"]!.GetValue<string>());
        Assert.Equal(
            """[{"type":"text","text":"hello"}]""",
            asked["content"]!.ToJsonString());

        // What the chat is given back is the conversation with both sides of the exchange in it,
        // the answer included.
        var conversation = data[1]!.AsArray();

        Assert.Equal(2, conversation.Count);
        Assert.Equal("hello", conversation[0]!["content"]![0]!["text"]!.GetValue<string>());

        // A direct call takes the first chunk of a streamed answer rather than waiting for the
        // last; the queue is what a browser watches, and what makes the answer grow.
        Assert.Equal("Hel", AssistantText(data[1]!));
    }

    [Fact]
    public async Task The_answer_is_shown_as_it_arrives()
    {
        var service = new FakeService().Streaming("Hel", "lo");

        await using var host = await StartAsync(service);

        await SendAsync(host, Said("hello"), "pieces");

        var cancellationToken = TestContext.Current.CancellationToken;

        using var queue = new QueueClient(host.BaseAddress);
        var index = await queue.EndpointAsync("_submit_fn", cancellationToken);
        var eventId = await queue.JoinAsync(
            index,
            new JsonArray(null, new JsonArray()),
            "pieces",
            cancellationToken);

        await using var stream = await queue.OpenAsync("pieces", cancellationToken);

        var seen = new List<string>();
        JsonNode? conversation = null;

        while (true)
        {
            var message = await stream.UntilAsync(
                candidate => candidate["event_id"]?.GetValue<string>() == eventId
                    && QueueSession.Kind(candidate) is "process_generating" or "process_completed",
                TimeSpan.FromSeconds(15),
                cancellationToken);

            Assert.NotNull(message);

            var chunk = message!["output"]!["data"]![1]!;
            var generating = QueueSession.Kind(message) == "process_generating";

            // A generating chunk is either the conversation itself — the first one, which the
            // client has nothing to apply an edit to — or the edits that turn the previous one
            // into it.
            conversation = generating && chunk is JsonArray edits && IsEditList(edits)
                ? JsonPatch.Apply(conversation, edits)
                : chunk.DeepClone();

            if (generating)
            {
                // Each step carries the answer as it stands rather than the piece that arrived, so
                // a client that replaced its last message with what it is given shows the answer
                // growing.
                seen.Add(AssistantText(conversation!));
                continue;
            }

            Assert.Equal("Hello", AssistantText(chunk));
            break;
        }

        Assert.Equal(["Hel", "Hello"], seen);
    }

    [Fact]
    public async Task A_chat_that_does_not_stream_asks_for_the_whole_answer()
    {
        var service = new FakeService().Answering(
            """{"choices":[{"message":{"role":"assistant","content":"Hi there"}}]}""");

        await using var host = await StartAsync(service, streaming: false);

        await SendAsync(host, Said("hello"), "whole");

        var data = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "whole");

        var call = Assert.Single(service.Calls);
        var body = JsonNode.Parse(call.Body)!.AsObject();

        // A request that does not stream says nothing about streaming, which is what the OpenAI
        // client sends when it is not asked for one.
        Assert.False(body.ContainsKey("stream"));
        Assert.Equal("Hi there", AssistantText(data[1]!));

        // The endpoint is served under the name the Python version's own handler gives it, which is what a
        // client calling the loaded chat reads.
        var config = await ConfigAsync(host);

        Assert.Contains(
            config["dependencies"]!.AsArray(),
            entry => entry!["api_name"]!.GetValue<string>() == "open_api");
    }

    [Fact]
    public async Task The_system_message_opens_every_request()
    {
        var service = new FakeService()
            .Streaming("one")
            .Streaming("two");

        await using var host = await StartAsync(service, systemMessage: "Be brief.");

        await SendAsync(host, Said("first"), "system");
        var first = await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "system");

        // A finished answer hands the conversation to the two states that keep it, which is the
        // step the browser runs after the answer rather than part of it.
        await RunAsync(host, "lambda", new JsonArray(first[1]!.DeepClone()), "system");

        await SendAsync(host, Said("second"), "system");
        await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "system");

        Assert.Equal(2, service.Calls.Count);

        foreach (var call in service.Calls)
        {
            var messages = JsonNode.Parse(call.Body)!["messages"]!.AsArray();

            Assert.Equal("system", messages[0]!["role"]!.GetValue<string>());
            Assert.Equal("Be brief.", messages[0]!["content"]!.GetValue<string>());
        }

        // The second request carries the conversation so far, and the instruction is still in front
        // of it: what the reader said, what the model answered, and what was just asked.
        var second = JsonNode.Parse(service.Calls[1].Body)!["messages"]!.AsArray();

        Assert.Equal(4, second.Count);
        Assert.Equal("first", second[1]!["content"]![0]!["text"]!.GetValue<string>());
        Assert.Equal("one", second[2]!["content"]![0]!["text"]!.GetValue<string>());
        Assert.Equal("second", second[3]!["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_text_file_is_read_into_the_prompt()
    {
        var service = new FakeService().Streaming("read");
        var file = WriteTemporaryFile("notes.txt", "alpha");

        try
        {
            await using var host = await StartAsync(service);

            await SendAsync(host, Said("what does this say?", file), "text-file");

            await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "text-file");

            var messages = JsonNode.Parse(Assert.Single(service.Calls).Body)!["messages"]!.AsArray();
            var asked = Assert.Single(messages)!;

            // One message, and the file's text appended to the prompt under its own name — which is
            // what makes a text file something a model without vision can read.
            Assert.Equal(
                "what does this say?\n## notes.txt\nalpha",
                asked["content"]![0]!["text"]!.GetValue<string>());
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public async Task An_image_is_sent_as_a_data_url()
    {
        var service = new FakeService().Streaming("seen");
        var file = WriteTemporaryFile("shot.png", bytes: [1, 2, 3]);

        try
        {
            await using var host = await StartAsync(
                service,
                kinds: ChatFileType.TextEncoded | ChatFileType.Image);

            await SendAsync(host, Said(string.Empty, file), "image");

            await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "image");

            var messages = JsonNode.Parse(Assert.Single(service.Calls).Body)!["messages"]!.AsArray();
            var block = Assert.Single(Assert.Single(messages)!["content"]!.AsArray())!;

            Assert.Equal("image_url", block["type"]!.GetValue<string>());
            Assert.Equal(
                "data:image/png;base64,AQID",
                block["image_url"]!["url"]!.GetValue<string>());
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public async Task A_text_file_in_the_conversation_stays_text()
    {
        var service = new FakeService().Streaming("again");
        var file = WriteTemporaryFile("readme.md", "alpha");

        try
        {
            await using var host = await StartAsync(service);

            var conversation = new JsonArray(new JsonObject
            {
                ["role"] = "user",
                ["content"] = new JsonArray(new JsonObject
                {
                    ["type"] = "file",
                    ["file"] = new JsonObject
            {
                ["path"] = file,
                ["meta"] = new JsonObject { ["_type"] = "gradio.FileData" },
            },
                }),
            });

            // The conversation a reader already had, put where a finished answer leaves it.
            await RunAsync(host, "lambda", new JsonArray(conversation.DeepClone()), "history");

            await SendAsync(host, Said("and now?"), "history");

            await RunAsync(host, "_submit_fn", new JsonArray(null, new JsonArray()), "history");

            var messages = JsonNode.Parse(Assert.Single(service.Calls).Body)!["messages"]!.AsArray();

            // The attachment that was sent as text the first time is sent as text again: sending it
            // as an image once it is in the conversation is what breaks a model that cannot see.
            Assert.Equal("text", messages[0]!["content"]![0]!["type"]!.GetValue<string>());
            Assert.Equal(
                "\n## readme.md\nalpha",
                messages[0]!["content"]![0]!["text"]!.GetValue<string>());
            Assert.Equal("and now?", messages[1]!["content"]![0]!["text"]!.GetValue<string>());
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public async Task The_field_accepts_the_files_the_kind_names()
    {
        // Text files are the default, and the field is a multimodal one because a chat that takes
        // any file is not a chat with a plain text box.
        var text = await FieldOfAsync(ChatFileType.TextEncoded);

        Assert.Equal(TextExtensions, text!["file_types"]!.AsArray().Select(entry => entry!.GetValue<string>()));

        var withImages = await FieldOfAsync(ChatFileType.TextEncoded | ChatFileType.Image);

        Assert.Equal(
            [.. TextExtensions, ".png", ".jpg", ".jpeg", ".gif", ".webp"],
            withImages!["file_types"]!.AsArray().Select(entry => entry!.GetValue<string>()));

        // No files at all is a plain field, and the chat is an ordinary text chat.
        var none = await FieldOfAsync(ChatFileType.None);

        Assert.Null(none);
    }

    [Fact]
    public async Task A_refused_request_says_what_the_endpoint_said()
    {
        var logs = new CapturingLoggerProvider();
        var service = new FakeService().Refusing(
            HttpStatusCode.NotFound,
            """{"error":{"message":"model 'llama3.2' not found"}}""");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadChat(
                blocks,
                "http://model.test/v1",
                "llama3.2",
                httpClient: service.Client()));

        await SendAsync(host, Said("hello"), "refused");

        var body = new JsonObject
        {
            ["data"] = new JsonArray(null, new JsonArray()),
            ["session_hash"] = "refused",
        };

        using var response = await host.Client.PostAsync(
            "/gradio_api/run/_submit_fn",
            new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // What the endpoint said is what a person reading the failure needs, so it travels in the
        // exception rather than being replaced by a status code.
        Assert.Contains(
            logs.Entries,
            entry => entry.Exception is HttpRequestException error
                && error.Message.Contains("model 'llama3.2' not found", StringComparison.Ordinal)
                && error.Message.Contains("404", StringComparison.Ordinal));
    }

    [Fact]
    public async Task A_loaded_chat_is_a_chat_with_every_parameter_passed_on()
    {
        var loaded = await ConfigOfAsync(blocks => _ = External.LoadChat(
            blocks,
            "http://model.test/v1",
            "llama3.2",
            token: "secret",
            fileTypes: ChatFileType.Image,
            systemMessage: "Be brief.",
            chatbot: new Chatbot { Label = "Talk", Scale = 2 },
            textbox: new MultimodalTextbox { Label = "Say", FileTypes = [".png"] },
            editable: true,
            examples: new object?[] { "hello" },
            examplesLabel: "Try",
            runExamplesOnClick: false,
            title: "Loaded",
            description: "A loaded chat",
            flaggingMode: "manual",
            flaggingOptions: ["Good", "Bad"],
            autofocus: false,
            autoscroll: false,
            concurrencyLimit: ConcurrencyLimit.Of(2),
            showProgress: ProgressDisplay.Hidden,
            fillHeight: false,
            fillWidth: true,
            apiName: "ask",
            apiDescription: "Ask the model",
            apiVisibility: ApiVisibility.Undocumented,
            saveHistory: true));

        var built = await ConfigOfAsync(blocks => _ = new ChatInterface(
            blocks,
            Answer,
            multimodal: true,
            chatbot: new Chatbot { Label = "Talk", Scale = 2 },
            textbox: new MultimodalTextbox { Label = "Say", FileTypes = [".png"] },
            editable: true,
            examples: new object?[] { "hello" },
            examplesLabel: "Try",
            runExamplesOnClick: false,
            title: "Loaded",
            description: "A loaded chat",
            flaggingMode: "manual",
            flaggingOptions: ["Good", "Bad"],
            autofocus: false,
            autoscroll: false,
            concurrencyLimit: ConcurrencyLimit.Of(2),
            showProgress: ProgressDisplay.Hidden,
            fillHeight: false,
            fillWidth: true,
            apiName: "ask",
            apiDescription: "Ask the model",
            apiVisibility: ApiVisibility.Undocumented,
            saveHistory: true));

        Assert.Equal(built.ToJsonString(), loaded.ToJsonString());
    }

    [Fact]
    public async Task A_loaded_chat_with_nothing_configured_is_the_chat_upstream_builds()
    {
        var loaded = await ConfigOfAsync(blocks => _ = External.LoadChat(
            blocks,
            "http://model.test/v1",
            "llama3.2"));

        var built = await ConfigOfAsync(blocks => _ = new ChatInterface(
            blocks,
            Answer,
            multimodal: true,
            chatbot: new Chatbot { Scale = 1, AllowTags = true },
            textbox: new MultimodalTextbox { FileTypes = TextExtensions },
            apiName: "open_api_stream"));

        Assert.Equal(built.ToJsonString(), loaded.ToJsonString());
    }

    /// <summary>Answers a message a piece at a time, for the chats built by hand.</summary>
    /// <param name="message">What the user sent.</param>
    /// <param name="history">The conversation so far.</param>
    /// <returns>The answer.</returns>
    private static async IAsyncEnumerable<string> Answer(
        MultimodalText message,
        IReadOnlyList<ChatMessage> history)
    {
        await Task.CompletedTask;

        yield return message.Text;
    }

    /// <summary>Starts a host whose application is a chat loaded from <paramref name="service"/>.</summary>
    /// <param name="service">The endpoint to load.</param>
    /// <param name="streaming">Whether the chat streams.</param>
    /// <param name="kinds">What the field accepts.</param>
    /// <param name="systemMessage">The instruction to open with.</param>
    /// <returns>The host.</returns>
    private static Task<GradioTestHost> StartAsync(
        FakeService service,
        bool streaming = true,
        ChatFileType kinds = ChatFileType.TextEncoded,
        string? systemMessage = null) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadChat(
                blocks,
                "http://model.test/v1",
                "llama3.2",
                fileTypes: kinds,
                systemMessage: systemMessage,
                streaming: streaming,
                httpClient: service.Client()));

    /// <summary>The field a chat with the given kinds draws, or <see langword="null"/> for none.</summary>
    /// <param name="kinds">What the field accepts.</param>
    /// <returns>The field's props.</returns>
    private static async Task<JsonObject?> FieldOfAsync(ChatFileType kinds)
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadChat(
                blocks,
                "http://model.test/v1",
                "llama3.2",
                fileTypes: kinds,
                httpClient: new FakeService().Client()));

        var config = await ConfigAsync(host);

        return config["components"]!.AsArray()
            .FirstOrDefault(entry => entry!["type"]!.GetValue<string>() == "multimodaltextbox")
            ?["props"]?.AsObject();
    }

    /// <summary>The config of a host whose application is built by <paramref name="build"/>.</summary>
    /// <param name="build">What to build.</param>
    /// <returns>The config, with the two values that belong to the run removed.</returns>
    /// <remarks>
    /// Two applications built the same way serve the same document; the id an installation gives
    /// itself and the address it is served from are the two values that are the run's rather than
    /// the application's.
    /// </remarks>
    private static async Task<JsonObject> ConfigOfAsync(Action<Blocks> build)
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            build);

        var config = await ConfigAsync(host);

        config.Remove("app_id");
        config.Remove("root");

        // A browser-side state encrypts what it keeps with a secret drawn at construction, so two
        // applications that are built the same way still differ in that one value.
        foreach (var component in config["components"]!.AsArray())
        {
            if (component!["props"] is JsonObject props && props.ContainsKey("secret"))
            {
                props["secret"] = "secret";
            }
        }

        return config;
    }

    /// <summary>The served config.</summary>
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

    /// <summary>Whether a generating chunk is a list of edits rather than a whole value.</summary>
    /// <param name="chunk">The chunk.</param>
    /// <returns>Whether it is an edit list.</returns>
    private static bool IsEditList(JsonArray chunk) =>
        chunk.Count > 0
        && chunk[0] is JsonArray edit
        && edit.Count == 3
        && edit[0] is JsonValue operation
        && operation.TryGetValue<string>(out var name)
        && name is "replace" or "append" or "add" or "delete";

    /// <summary>The last thing the assistant said in a conversation.</summary>
    /// <param name="conversation">The conversation, as the chat serves it.</param>
    /// <returns>The text of its last message.</returns>
    private static string AssistantText(JsonNode conversation)
    {
        var messages = conversation.AsArray();

        return messages[^1]!["content"]![0]!["text"]!.GetValue<string>();
    }

    /// <summary>The value a multimodal field hands over: what was typed and what was attached.</summary>
    /// <param name="text">The text.</param>
    /// <param name="files">The paths of the attached files.</param>
    /// <returns>The value the field keeps.</returns>
    private static JsonObject Said(string text, params string[] files)
    {
        var attached = new JsonArray();

        foreach (var file in files)
        {
            attached.Add(new JsonObject
            {
                ["path"] = file,
                ["meta"] = new JsonObject { ["_type"] = "gradio.FileData" },
            });
        }

        return new JsonObject { ["text"] = text, ["files"] = attached };
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
        string sessionHash)
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
            $"'{apiName}' answered {response.StatusCode}: {answer.ToJsonString()}");

        return answer["data"]!.AsArray();
    }

    /// <summary>Writes a file the endpoint will be sent.</summary>
    /// <param name="name">Its name, which the prompt's heading repeats.</param>
    /// <param name="text">Its text, when it is a text file.</param>
    /// <param name="bytes">Its bytes, when it is not.</param>
    /// <returns>Its path.</returns>
    private static string WriteTemporaryFile(string name, string? text = null, byte[]? bytes = null)
    {
        var directory = Path.Combine(Path.GetTempPath(), "gradio-net-external", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, name);

        if (bytes is not null)
        {
            System.IO.File.WriteAllBytes(path, bytes);
        }
        else
        {
            System.IO.File.WriteAllText(path, text ?? string.Empty);
        }

        return path;
    }
}

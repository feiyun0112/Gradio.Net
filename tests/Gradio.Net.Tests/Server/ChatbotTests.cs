using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A chat as a client uses it: a conversation goes in, a conversation comes back, and a streamed
/// one arrives a piece at a time.
/// </summary>
/// <remarks>
/// <para>
/// Everything here goes through the queue endpoints, the way a browser does. The claim being tested
/// is the one a user makes when they build a chat: that what they put in a <see cref="Chatbot"/>
/// reaches their handler as messages, that what their handler returns reaches the browser as
/// messages, and that a streamed answer is visible while it is being written rather than only when
/// it is finished.
/// </para>
/// <para>
/// The messages are asserted as JSON, because that is what the frontend renders: a test that
/// asserted the C# objects would not notice a field the protocol carries and this side forgot.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class ChatbotTests
{
    [Fact]
    public async Task A_chat_binding_reads_the_conversation_and_answers_with_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (prompt, chat) = Chat(blocks);

            prompt.Submit(
                (Func<string, IReadOnlyList<ChatMessage>, IReadOnlyList<ChatMessage>>)Reply,
                [prompt, chat],
                [chat],
                apiName: "reply");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("reply", cancellationToken);

        var history = new JsonArray(
            Message("user", "first"),
            Message("assistant", "an answer"));

        var data = await CompletedDataAsync(
            queue,
            fnIndex,
            new JsonArray("second", history),
            "chat",
            cancellationToken);

        // The history the client sent, plus the reply: a handler appends to the conversation it was
        // given, which is the whole reason the value is a list of messages rather than two strings.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"first","type":"text"}],"options":null},{"role":"assistant","metadata":null,"content":[{"text":"an answer","type":"text"}],"options":null},{"role":"user","metadata":null,"content":[{"text":"second","type":"text"}],"options":null},{"role":"assistant","metadata":null,"content":[{"text":"you said: second","type":"text"}],"options":null}]""",
            data[0]!.ToJsonString());
    }

    [Fact]
    public async Task A_streamed_answer_arrives_a_piece_at_a_time()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (prompt, chat) = Chat(blocks);

            prompt.Submit(
                (Func<string, IReadOnlyList<ChatMessage>, IAsyncEnumerable<IReadOnlyList<ChatMessage>>>)Write,
                [prompt, chat],
                [chat],
                apiName: "write");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("write", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, new JsonArray("hi", new JsonArray()), "streaming", cancellationToken);

        await using var stream = await queue.OpenAsync("streaming", cancellationToken);

        var seen = new List<string>();
        JsonNode? value = null;

        while (true)
        {
            var message = await stream.UntilAsync(
                candidate => candidate["event_id"]?.GetValue<string>() == eventId
                    && QueueSession.Kind(candidate) is "process_generating" or "process_completed",
                TimeSpan.FromSeconds(15),
                cancellationToken);

            Assert.NotNull(message);

            var chunk = message!["output"]!["data"]![0]!;
            var generating = QueueSession.Kind(message) == "process_generating";

            // A generating chunk is either the whole value — the first one, which the client has
            // nothing to apply an edit to — or the edits that turn the previous one into it.
            value = generating && chunk is JsonArray edits && IsEditList(edits)
                ? JsonPatch.Apply(value, edits)
                : chunk.DeepClone();

            var messages = value!.AsArray();

            if (generating)
            {
                seen.Add($"{messages.Count}:{messages[^1]!["content"]![0]!["text"]!.GetValue<string>()}");
                continue;
            }

            // The completion carries the whole value rather than an edit, and it is what the client
            // keeps: it has to be the value the last edit produced.
            Assert.Equal(chunk.ToJsonString(), value.ToJsonString());
            break;
        }

        // Each chunk was visible on its own: the user message first, then the answer growing one
        // word at a time. A stream that only arrived at the end would produce one line here.
        Assert.Equal(["1:hi", "2:one", "2:one two"], seen);

        // And the last message carries the whole conversation, which is what the client keeps.
        Assert.Equal(2, value!.AsArray().Count);
    }

    [Fact]
    public async Task A_reasoning_tag_arrives_as_a_message_of_its_own()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (prompt, chat) = Chat(blocks);
            chat.ReasoningTags = [new ReasoningTag("<think>", "</think>")];

            prompt.Submit(
                (Func<string, IReadOnlyList<ChatMessage>, IReadOnlyList<ChatMessage>>)Think,
                [prompt, chat],
                [chat],
                apiName: "think");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("think", cancellationToken);

        var data = await CompletedDataAsync(
            queue,
            fnIndex,
            new JsonArray("hi", new JsonArray()),
            "reasoning",
            cancellationToken);

        var messages = data[0]!.AsArray();

        // The thought is lifted out of the answer and shown as its own collapsible message, which
        // is what the reasoning tags are for.
        Assert.Equal(3, messages.Count);
        Assert.Equal("Reasoning", messages[1]!["metadata"]!["title"]!.GetValue<string>());
        Assert.Equal("thinking about it", messages[1]!["content"]![0]!["text"]!.GetValue<string>());
        Assert.Equal("the answer", messages[2]!["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_served_config_offers_the_feedback_and_reasoning_options_it_was_given()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Chatbot
                {
                    FeedbackOptions = ["Good", "Bad"],
                    FeedbackValue = ["Good", null],
                    ReasoningTags = [new ReasoningTag("<think>", "</think>")],
                    LikeUserMessage = true,
                    Examples = [new ChatExample { Text = "Hello" }],
                    Placeholder = "Ask something",
                });
            }
        });

        var config = JsonNode.Parse(await host.Client.GetStringAsync("/config", cancellationToken))!;
        var props = config["components"]![0]!["props"]!;

        // The feedback options are what the frontend draws next to a message, and the value is the
        // feedback already given; both are part of the component's description.
        Assert.Equal("""["Good","Bad"]""", props["feedback_options"]!.ToJsonString());
        Assert.Equal("""["Good",null]""", props["feedback_value"]!.ToJsonString());
        Assert.True(props["like_user_message"]!.GetValue<bool>());

        // A chat that can be liked, retried and undone says so about itself, which is how the
        // frontend knows to draw the affordances.
        Assert.False(props["likeable"]!.GetValue<bool>());
        Assert.False(props["_undoable"]!.GetValue<bool>());
        Assert.False(props["_retryable"]!.GetValue<bool>());
    }

    [Fact]
    public async Task Binding_a_signal_turns_on_the_affordance_it_offers()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            Chatbot chat;

            using (blocks.BeginRender())
            {
                chat = blocks.Add(new Chatbot { Label = "Chat" });
            }

            // A chat whose messages can be liked, retried, undone and selected. The frontend offers
            // those affordances from the config, so binding one has to change what the config says.
            chat.Like(Keep, [chat], [chat], apiName: "like");
            chat.Retry(Keep, [chat], [chat], apiName: "retry");
            chat.Undo(Keep, [chat], [chat], apiName: "undo");
            chat.Select(Keep, [chat], [chat], apiName: "select");
        });

        var config = JsonNode.Parse(await host.Client.GetStringAsync("/config", cancellationToken))!;
        var props = config["components"]![0]!["props"]!;

        Assert.True(props["likeable"]!.GetValue<bool>());
        Assert.True(props["_retryable"]!.GetValue<bool>());
        Assert.True(props["_undoable"]!.GetValue<bool>());
        Assert.True(props["_selectable"]!.GetValue<bool>());
    }

    [Fact]
    public async Task A_chat_that_binds_nothing_offers_none_of_them()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Chatbot { Label = "Chat" });
            }
        });

        var config = JsonNode.Parse(await host.Client.GetStringAsync("/config", cancellationToken))!;
        var props = config["components"]![0]!["props"]!;

        // The keys are present and false rather than absent: the frontend reads them, and "nothing
        // listens" is a different statement from "the component does not say".
        Assert.False(props["likeable"]!.GetValue<bool>());
        Assert.False(props["_retryable"]!.GetValue<bool>());
        Assert.False(props["_undoable"]!.GetValue<bool>());
        Assert.False(props["_selectable"]!.GetValue<bool>());
    }

    /// <summary>Answers with the conversation it was given.</summary>
    private static object? Keep(IReadOnlyList<ChatMessage> history) => history;

    /// <summary>Whether a chunk is a list of edits rather than a value.</summary>
    /// <remarks>
    /// An edit is a three-element array whose first element is the operation's name, so a list of
    /// them is a list of arrays. A conversation is a list of objects, which is what tells the two
    /// apart without knowing which chunk this is.
    /// </remarks>
    private static bool IsEditList(JsonArray chunk) =>
        chunk.Count > 0
        && chunk[0] is JsonArray edit
        && edit.Count == 3
        && edit[0] is JsonValue operation
        && operation.TryGetValue<string>(out var name)
        && name is "replace" or "append" or "add" or "delete";

    /// <summary>A message document, as a client sends one.</summary>
    private static JsonObject Message(string role, string text) => new()
    {
        ["role"] = role,
        ["content"] = new JsonArray(new JsonObject { ["text"] = text, ["type"] = "text" }),
    };

    /// <summary>An application with a prompt and a conversation.</summary>
    private static (Textbox Prompt, Chatbot Chat) Chat(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var prompt = blocks.Add(new Textbox { Label = "Prompt", SubmitBtn = true });
            var chat = blocks.Add(new Chatbot { Label = "Chat" });

            return (prompt, chat);
        }
    }

    /// <summary>Appends the prompt and a reply to the conversation.</summary>
    private static IReadOnlyList<ChatMessage> Reply(string prompt, IReadOnlyList<ChatMessage> history) =>
    [
        .. history,
        new ChatMessage(prompt, ChatRole.User),
        new ChatMessage($"you said: {prompt}"),
    ];

    /// <summary>Writes the answer a word at a time, yielding the conversation as it grows.</summary>
    private static async IAsyncEnumerable<IReadOnlyList<ChatMessage>> Write(
        string prompt,
        IReadOnlyList<ChatMessage> history)
    {
        // The user's message arrives on its own, then the answer grows: the first chunk is what the
        // frontend draws immediately, and each one after it is what the browser applies to it.
        List<ChatMessage> conversation = [.. history, new ChatMessage(prompt, ChatRole.User)];
        yield return conversation;

        conversation = [.. conversation, new ChatMessage("one")];
        yield return conversation;

        conversation = [.. conversation[..^1], new ChatMessage("one two")];
        yield return conversation;

        await Task.Yield();
    }

    /// <summary>Answers with a thought in the middle of it.</summary>
    private static IReadOnlyList<ChatMessage> Think(string prompt, IReadOnlyList<ChatMessage> history) =>
        [new ChatMessage("before<think>thinking about it</think>the answer")];

    /// <summary>Runs once and returns what the run's outputs carried.</summary>
    private static async Task<JsonArray> CompletedDataAsync(
        QueueClient queue,
        int fnIndex,
        JsonArray data,
        string sessionHash,
        CancellationToken cancellationToken)
    {
        var eventId = await queue.JoinAsync(fnIndex, data, sessionHash, cancellationToken);

        await using var stream = await queue.OpenAsync(sessionHash, cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        return completed!["output"]!["data"]!.AsArray();
    }
}

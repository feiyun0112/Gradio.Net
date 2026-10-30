using System.Reflection;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A chat message as the frontend and as a handler see it: the structured blocks a value is made
/// of, and the two directions they travel in.
/// </summary>
/// <remarks>
/// <para>
/// Gradio 6 has one message model and it is not a pair of strings. A message is a role plus a list
/// of content blocks — text, a file, a component — plus optional metadata and options, and the
/// frontend renders those blocks rather than parsing a tuple (spec: "Chatbot 与 6.x 消息模型").
/// The tests here assert the shape at both ends: what a handler receives when a client sends
/// messages, and what the client receives when a handler returns them.
/// </para>
/// <para>
/// The assertions are on JSON because that is what the protocol carries, and because the shape is
/// the Python version's: a field that the Python version writes as <c>null</c> has to be written,
/// not omitted, or a client that reads the document as a schema of five fields sees a different
/// document.
/// </para>
/// </remarks>
public sealed class ChatbotMessageTests
{
    [Fact]
    public void A_text_message_is_sent_as_one_text_block()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        var value = chatbot.Postprocess(Conversation([new ChatMessage("hello", ChatRole.User)]));

        // The Python version's Message model: role, metadata and options are always present, and the content
        // is a list even when there is one block in it.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]""",
            value!.ToJsonString());
    }

    [Fact]
    public void A_conversation_with_no_value_is_an_empty_list()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        Assert.Equal("[]", chatbot.Postprocess(null)!.ToJsonString());
        Assert.Equal("[]", chatbot.Postprocess(Array.Empty<ChatMessage>())!.ToJsonString());
    }

    [Fact]
    public void Text_is_cleaned_the_way_upstream_cleans_it()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        // The Python version runs a message's text through inspect.cleandoc, which is what makes a handler's
        // indented literal arrive without the indentation it was written with.
        var value = chatbot.Postprocess(Conversation([new ChatMessage("\n    first\n      second\n    ", "user")]));

        Assert.Equal(
            "first\n  second",
            value![0]!["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void A_tab_advances_to_the_next_tab_stop()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        // The Python version expands tabs to the next multiple of eight columns rather than replacing each
        // one with eight spaces, so a tab after one character gains seven spaces.
        var value = chatbot.Postprocess(Conversation([new ChatMessage("a\tb", "user")]));

        Assert.Equal("a       b", value![0]!["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void A_message_with_no_content_is_not_rendered_at_all()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        // The Python version drops a message whose content comes out empty rather than drawing an empty
        // bubble for it.
        var value = chatbot.Postprocess(Conversation([new ChatMessage([], ChatRole.User)]));

        Assert.Equal("[]", value!.ToJsonString());
    }

    [Fact]
    public void A_value_that_is_not_a_conversation_is_refused()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        // A client that sent something else sent something this component does not understand;
        // reading it as "no messages" would hide that.
        Assert.Throws<ArgumentException>(() => chatbot.Preprocess(JsonValue.Create("hello")));
    }

    [Fact]
    public void Several_blocks_are_sent_in_the_order_they_were_written()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        var value = chatbot.Postprocess(Conversation([
            new ChatMessage(
                [ChatContent.TextBlock("one"), ChatContent.FileBlock(new FileData { Path = "https://example.com/a.png" }), ChatContent.TextBlock("two")],
                ChatRole.Assistant),
        ]));

        var content = value![0]!["content"]!.AsArray();

        Assert.Equal(3, content.Count);
        Assert.Equal("text", content[0]!["type"]!.GetValue<string>());
        Assert.Equal("file", content[1]!["type"]!.GetValue<string>());
        Assert.Equal("text", content[2]!["type"]!.GetValue<string>());
    }

    [Fact]
    public void A_file_block_carries_the_file_the_way_the_protocol_carries_one()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);
        var picture = folder.Write("a.png", "not really a png");

        var value = chatbot.Postprocess(Conversation([
            new ChatMessage(
                ChatContent.FileBlock(new FileData { Path = picture }, "a picture"),
                ChatRole.User),
        ]));

        var file = value![0]!["content"]![0]!;

        // The Python version's FileMessage: the file object, the alternative text, and the type marker. The
        // media type is filled in from the name, because the frontend picks a renderer by it.
        Assert.Equal("file", file["type"]!.GetValue<string>());
        Assert.Equal("a picture", file["alt_text"]!.GetValue<string>());
        Assert.Equal("image/png", file["file"]!["mime_type"]!.GetValue<string>());
        Assert.Equal(FileData.FileMetaType, file["file"]!["meta"]!["_type"]!.GetValue<string>());

        // A file the application holds is copied into its cache, so that the file endpoint will
        // serve it; a message names it by path rather than by address, which is the Python version's shape.
        Assert.True(MediaCache.IsInFolder(file["file"]!["path"]!.GetValue<string>(), folder.Folder));
        Assert.Null(file["file"]!["url"]);
    }

    [Fact]
    public void Metadata_and_options_are_carried_through()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        var value = chatbot.Postprocess(Conversation([
            new ChatMessage("pick one", ChatRole.Assistant)
            {
                Metadata = new MessageMetadata { Title = "Thought", Id = 1, Duration = 0.5, Status = MessageStatus.Done },
                Options = [new ChatOption("first"), new ChatOption("second", "Second")],
            },
        ]));

        var message = value![0]!;

        // A metadata dictionary carries the keys it was given and no others: the Python version's is a
        // TypedDict, so an absent field is absent rather than null.
        Assert.Equal(
            """{"title":"Thought","id":1,"duration":0.5,"status":"done"}""",
            message["metadata"]!.ToJsonString());
        Assert.Equal(
            """[{"value":"first"},{"value":"second","label":"Second"}]""",
            message["options"]!.ToJsonString());
    }

    [Fact]
    public void A_reasoning_tag_splits_a_message_into_its_parts()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(
            provider,
            folder,
            out var chatbot,
            component => component.ReasoningTags = [new ReasoningTag("<think>", "</think>")]);

        var value = chatbot.Postprocess(Conversation([new ChatMessage("before<think>hmm</think>after", ChatRole.Assistant)]));
        var messages = value!.AsArray();

        // The Python version turns each tagged span into a collapsible message of its own, in the order the
        // spans appear, and leaves the prose around it as ordinary messages.
        Assert.Equal(3, messages.Count);
        Assert.Equal("before", messages[0]!["content"]![0]!["text"]!.GetValue<string>());
        Assert.Null(messages[0]!["metadata"]);
        Assert.Equal("hmm", messages[1]!["content"]![0]!["text"]!.GetValue<string>());
        Assert.Equal("Reasoning", messages[1]!["metadata"]!["title"]!.GetValue<string>());
        Assert.Equal("done", messages[1]!["metadata"]!["status"]!.GetValue<string>());
        Assert.Equal("after", messages[2]!["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void A_reasoning_tag_that_is_never_closed_is_pending()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(
            provider,
            folder,
            out var chatbot,
            component => component.ReasoningTags = [new ReasoningTag("<think>", "</think>")]);

        // A streamed answer that is still thinking has an open tag; the frontend shows a spinner
        // for it, which is why the status is part of the protocol rather than a detail.
        var messages = chatbot.Postprocess(Conversation([new ChatMessage("before<think>hmm", ChatRole.Assistant)]))!.AsArray();

        Assert.Equal(2, messages.Count);
        Assert.Equal("pending", messages[1]!["metadata"]!["status"]!.GetValue<string>());
    }

    [Fact]
    public void A_client_message_is_read_as_blocks()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        var value = chatbot.Preprocess(JsonNode.Parse(
            """
            [{"role":"user","metadata":null,"content":[{"text":"hi","type":"text"}],"options":null},
             {"role":"assistant","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]
            """));

        var messages = Assert.IsAssignableFrom<IReadOnlyList<ChatMessage>>(value);

        Assert.Equal(2, messages.Count);
        Assert.Equal(ChatRole.User, messages[0].Role);
        Assert.Equal("hi", Assert.IsType<TextContent>(Assert.Single(messages[0].Content)).Text);
        Assert.Equal("hello", messages[1].Text);
    }

    [Fact]
    public void A_client_file_block_is_read_as_a_file()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(provider, folder, out var chatbot);

        var value = chatbot.Preprocess(JsonNode.Parse(
            """
            [{"role":"user","content":[{"file":{"path":"/tmp/a.png","meta":{"_type":"gradio.FileData"}},"alt_text":"a","type":"file"}],"options":null}]
            """));

        var content = Assert.IsType<FileContent>(Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<ChatMessage>>(value)[0].Content));

        Assert.Equal("/tmp/a.png", content.File.Path);
        Assert.Equal("a", content.AltText);
    }

    [Fact]
    public void The_config_carries_the_messages_the_component_starts_with()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderChatbot(
            provider,
            folder,
            out var chatbot,
            component => component.Value = [new ChatMessage("hi", ChatRole.User), new ChatMessage("hello")]);

        // The value in the config is the normalized one: a page that starts with a conversation
        // carries the same blocks a run would have produced.
        Assert.Equal(
            """[{"role":"user","metadata":null,"content":[{"text":"hi","type":"text"}],"options":null},{"role":"assistant","metadata":null,"content":[{"text":"hello","type":"text"}],"options":null}]""",
            Entry(blocks, chatbot)["props"]!["value"]!.ToJsonString());
    }

    [Fact]
    public void The_component_offers_no_tuple_style_api()
    {
        var publicSurface = typeof(Chatbot)
            .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .ToArray();

        // Gradio 6 removed the tuple model outright: no `type="tuples"` switch, and no overload
        // that takes or returns a pair of strings as a message.
        Assert.DoesNotContain(publicSurface, member => member.Name is "Type" or "Tuples");
        Assert.DoesNotContain(
            publicSurface,
            member => member is MethodInfo method
                && method.GetParameters().Any(parameter => IsTuple(parameter.ParameterType)));
        Assert.DoesNotContain(
            publicSurface,
            member => member is MethodInfo method && IsTuple(method.ReturnType));
    }

    [Fact]
    public void A_role_that_is_not_one_of_upstreams_is_refused()
    {
        // The Python version's model accepts three roles; a fourth would be a message the frontend cannot
        // align, so it is refused where it is written rather than where it is rendered.
        Assert.Throws<ArgumentException>(() => new ChatMessage("hi", "narrator"));
    }

    /// <summary>A conversation, as a handler returns one.</summary>
    private static ChatMessage[] Conversation(params ChatMessage[] messages) => messages;

    /// <summary>Whether a type is a tuple, which the message model no longer has a place for.</summary>
    private static bool IsTuple(Type type) =>
        type == typeof(ValueTuple)
        || (type.IsGenericType
            && type.FullName is { } name
            && (name.StartsWith("System.ValueTuple`", StringComparison.Ordinal)
                || name.StartsWith("System.Tuple`", StringComparison.Ordinal)));

    /// <summary>Renders a chatbot into an application that keeps its files in a folder of the test's own.</summary>
    private static Blocks RenderChatbot(
        IServiceProvider provider,
        TempFolder folder,
        out Chatbot component,
        Action<Chatbot>? configure = null)
    {
        var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        using (blocks.BeginRender())
        {
            var chatbot = new Chatbot();
            configure?.Invoke(chatbot);
            component = blocks.Add(chatbot);
        }

        return blocks;
    }

    /// <summary>A block's entry in the served config.</summary>
    private static JsonObject Entry(Blocks blocks, Block block) =>
        blocks.GetConfig()["components"]!.AsArray()
            .Single(component => component!["id"]!.GetValue<int>() == block.Id)!
            .AsObject();
}

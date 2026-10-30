using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// How a chat turns what it is handed — a typed message, a handler's answer, a sample — into the
/// messages the conversation is made of.
/// </summary>
/// <remarks>
/// <para>
/// A chat's conversation is kept as messages in three places and crosses a JSON boundary between
/// each two: the browser sends it, the server remembers it in a state, and a handler is given it.
/// The conversions live here rather than at each crossing, because a message that came back
/// differently from the way it went in would change what the handler is asked about.
/// </para>
/// <para>
/// A message is appended rather than replaced: the Python version's history is a list, and every
/// step of the chat — a submit, a retry, an edit — appends the user's message and then the answer
/// to it.
/// </para>
/// </remarks>
public sealed partial class ChatInterface
{
    /// <summary>The longest title a saved conversation is listed under.</summary>
    private const int TitleLength = 40;

    /// <summary>A conversation as the config carries it.</summary>
    /// <param name="messages">The messages.</param>
    /// <returns>The array of messages.</returns>
    private static JsonArray MessagesToJson(IReadOnlyList<ChatMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);

        return [.. messages.Select(message => message.ToJson())];
    }

    /// <summary>A conversation as a handler reads it.</summary>
    /// <param name="value">The conversation: messages, the JSON they were kept as, or nothing.</param>
    /// <returns>The messages.</returns>
    /// <remarks>
    /// The value arrives in whichever shape the step before it produced: a state hands back the JSON
    /// it kept, a chatbot hands back messages, and a client that called the API sends JSON. All
    /// three are the same conversation, and a handler should not have to know which one it got.
    /// </remarks>
    private static IReadOnlyList<ChatMessage> MessagesOf(object? value) => value switch
    {
        null => [],
        IReadOnlyList<ChatMessage> messages => messages,
        JsonArray array => [.. array.Select(ChatMessage.FromJson)],
        JsonObject document => [ChatMessage.FromJson(document)],
        IEnumerable<object?> entries => [.. entries.Select(MessageOfEntry)],
        JsonNode node => [ChatMessage.FromJson(node)],

        // A state hands its value back as the CLR shape the JSON described, so a conversation that
        // was kept in one arrives as dictionaries rather than as nodes.
        IReadOnlyDictionary<string, object?> document => [ChatMessage.FromJson(AsJson(document))],
        _ => [],
    };

    /// <summary>One message out of a list that may hold either shape.</summary>
    /// <param name="entry">The entry.</param>
    /// <returns>The message.</returns>
    private static ChatMessage MessageOfEntry(object? entry) => entry switch
    {
        ChatMessage message => message,
        JsonNode node => ChatMessage.FromJson(node),
        IReadOnlyDictionary<string, object?> document => ChatMessage.FromJson(AsJson(document)),
        _ => throw new ArgumentException(
            $"A conversation is made of messages; one entry was a {(entry is null ? "nothing" : entry.GetType().Name)}.",
            nameof(entry)),
    };

    /// <summary>A dictionary as the JSON it was read from.</summary>
    /// <param name="document">The dictionary.</param>
    /// <returns>The object.</returns>
    private static JsonObject AsJson(IReadOnlyDictionary<string, object?> document) =>
        JsonValues.FromAnyObject(document) as JsonObject ?? new JsonObject();

    /// <summary>The messages a step's value stands for, each with the role that step gives it.</summary>
    /// <param name="message">The value: a text, a message, a multimodal value, a component, or a list of any of those.</param>
    /// <param name="role">Who is speaking.</param>
    /// <returns>The messages, in order.</returns>
    /// <remarks>
    /// The Python version's <c>_message_as_message_dict</c>. A handler may answer with a plain
    /// string — the common case — with a message that says what it is, with several messages at
    /// once, or with a component it wants shown; each of those is one or more messages once it
    /// reaches the conversation.
    /// </remarks>
    private static List<ChatMessage> MessageOf(object? message, string role)
    {
        var messages = new List<ChatMessage>();

        foreach (var entry in AsList(message))
        {
            switch (entry)
            {
                case null:
                    break;

                case ChatMessage chat:
                    chat.Role = role;
                    messages.Add(chat);
                    break;

                case string text:
                    messages.Add(new ChatMessage(text, role));
                    break;

                case MultimodalText multimodal:
                    messages.Add(MultimodalMessage(multimodal, role));
                    break;

                case Component component:
                    messages.Add(new ChatMessage(
                        ChatContent.ComponentBlock(
                            component.BlockClass,
                            null,
                            constructorArgs: null,
                            (JsonObject)component.ToConfigEntry()["props"]!.DeepClone()),
                        role));
                    break;

                case JsonObject document when document.ContainsKey("content"):
                    var parsed = ChatMessage.FromJson(document);
                    parsed.Role = role;
                    messages.Add(parsed);
                    break;

                // What a multimodal field hands back: its files and its text, which is the shape the
                // browser keeps rather than a message. The Python version treats any dictionary without a
                // `content` key as that shape, and a chat's field is the only thing that sends one.
                case JsonObject document:
                    messages.Add(MultimodalMessage(document, role));
                    break;

                case IReadOnlyDictionary<string, object?> document:
                    messages.Add(MultimodalMessage(AsJson(document), role));
                    break;

                case JsonNode node:
                    var described = ChatMessage.FromJson(node);
                    described.Role = role;
                    messages.Add(described);
                    break;

                default:
                    throw new ArgumentException(
                        $"A message is a text, a chat message or a component; got {entry.GetType().Name}.",
                        nameof(message));
            }
        }

        return messages;
    }

    /// <summary>A multimodal value as a message: its files, then its text.</summary>
    /// <param name="value">The value.</param>
    /// <param name="role">Who is speaking.</param>
    /// <returns>The message.</returns>
    private static ChatMessage MultimodalMessage(MultimodalText value, string role)
    {
        var content = new List<ChatContent>();

        foreach (var file in value.Files)
        {
            content.Add(ChatContent.FileBlock(file));
        }

        if (value.Text.Length > 0)
        {
            content.Add(ChatContent.TextBlock(value.Text));
        }

        return new ChatMessage(content, role);
    }

    /// <summary>A multimodal value that arrived as JSON, as a message: its files, then its text.</summary>
    /// <param name="value">The value, with <c>text</c> and <c>files</c> as the browser spells them.</param>
    /// <param name="role">Who is speaking.</param>
    /// <returns>The message.</returns>
    private static ChatMessage MultimodalMessage(JsonObject value, string role)
    {
        var content = new List<ChatContent>();

        if (value["files"] is JsonArray files)
        {
            foreach (var file in files)
            {
                if (FileData.FromJson(file) is { } described)
                {
                    content.Add(ChatContent.FileBlock(described));
                }
            }
        }

        if (JsonValues.Text(value["text"]) is { Length: > 0 } text)
        {
            content.Add(ChatContent.TextBlock(text));
        }

        return new ChatMessage(content, role);
    }

    /// <summary>A value read as the list it stands for.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The entries: the value itself when it is not a list, and nothing when it is absent.</returns>
    /// <remarks>
    /// A string is a value rather than a list of characters, which is what makes this different from
    /// a cast to <see cref="System.Collections.IEnumerable"/>.
    /// </remarks>
    private static IEnumerable<object?> AsList(object? value) => value switch
    {
        null => [],
        string or ChatMessage or MultimodalText or Component or JsonObject => [value],

        // A JSON array is an IEnumerable of nodes, so it is already covered by the arm below;
        // naming it here would be an arm the compiler can never reach.
        IEnumerable<object?> entries => entries,
        _ => [value],
    };

    /// <summary>The message as the handler is given it.</summary>
    /// <param name="message">The message: a text, or the value a multimodal field holds.</param>
    /// <returns>The value the handler's first parameter takes.</returns>
    /// <remarks>
    /// A text chat's handler reads a string and a multimodal chat's reads a
    /// <see cref="MultimodalText"/>, while the value between the field and the run is JSON either
    /// way: a state hands back what the browser sent. The conversion belongs here rather than in
    /// the handler, so that the two kinds of chat differ in what they are written against and not
    /// in what they have to unwrap.
    /// </remarks>
    private object? MessageForHandler(object? message) => _multimodal ? MultimodalOf(message) : message;

    /// <summary>A multimodal message as the typed value a handler reads.</summary>
    /// <param name="message">The message, in whichever shape it arrived.</param>
    /// <returns>The value.</returns>
    private static MultimodalText MultimodalOf(object? message) => message switch
    {
        MultimodalText value => value,
        JsonObject document => new MultimodalText(
            JsonValues.Text(document["text"]) ?? string.Empty,
            FilesOf(document)),
        IReadOnlyDictionary<string, object?> document => MultimodalOf(AsJson(document)),
        string text => new MultimodalText(text),
        null => new MultimodalText(string.Empty),
        _ => new MultimodalText(message.ToString() ?? string.Empty),
    };

    /// <summary>A conversation with one more message appended to it.</summary>
    /// <param name="message">The message: whatever the step produced.</param>
    /// <param name="history">The conversation so far.</param>
    /// <param name="role">Who is speaking.</param>
    /// <returns>The conversation with the message at the end.</returns>
    private static List<ChatMessage> Append(
        object? message,
        IReadOnlyList<ChatMessage> history,
        string role)
    {
        ArgumentNullException.ThrowIfNull(history);

        var extended = new List<ChatMessage>(history);
        extended.AddRange(MessageOf(message, role));

        return extended;
    }

    /// <summary>
    /// The conversation without the last thing the user said, and that thing.
    /// </summary>
    /// <param name="history">The conversation.</param>
    /// <returns>The conversation up to the last answer's question, and the question.</returns>
    /// <remarks>
    /// A retry and an undo both take the last exchange back: the answer to it is dropped, and what
    /// the user said is handed back so that the field can be filled with it again. The message is
    /// rebuilt from its own blocks so that a question with files is handed back whole.
    /// </remarks>
    private (IReadOnlyList<ChatMessage> History, object? Message) PopLastUserMessage(
        IReadOnlyList<ChatMessage> history)
    {
        if (history.Count == 0)
        {
            return (history, EmptyMessage());
        }

        var index = history.Count - 1;

        while (index >= 0 && history[index].Role == ChatRole.Assistant)
        {
            index--;
        }

        while (index >= 0 && history[index].Role == ChatRole.User)
        {
            index--;
        }

        var last = history.Skip(index + 1).Where(message => message.Role == ChatRole.User).ToArray();

        return ([.. history.Take(index + 1)], LastUserMessage(last));
    }

    /// <summary>What the field is filled with when there is no message to put back.</summary>
    /// <returns>An empty text, or an empty multimodal value.</returns>
    private object EmptyMessage() =>
        _multimodal ? new MultimodalText(string.Empty) : string.Empty;

    /// <summary>The message a user last sent, rebuilt from the messages that hold it.</summary>
    /// <param name="messages">The user's messages from the exchange being taken back.</param>
    /// <returns>The text, or the text and files for a multimodal chat.</returns>
    private object LastUserMessage(IReadOnlyList<ChatMessage> messages)
    {
        var text = string.Empty;
        var files = new List<FileData>();

        foreach (var message in messages)
        {
            foreach (var content in message.Content)
            {
                switch (content)
                {
                    case TextContent block:
                        text += block.Text;
                        break;

                    case FileContent block:
                        files.Add(block.File);
                        break;

                    default:
                        break;
                }
            }
        }

        return _multimodal ? new MultimodalText(text, files) : text;
    }

    /// <summary>The title a saved conversation is listed under.</summary>
    /// <param name="conversation">The conversation.</param>
    /// <returns>
    /// The first text the user said, cut to forty characters, with a paperclip for each file — or
    /// the word "conversation" when they said nothing.
    /// </returns>
    private static string GenerateChatTitle(IReadOnlyList<ChatMessage> conversation)
    {
        ArgumentNullException.ThrowIfNull(conversation);

        var title = string.Empty;
        var found = false;

        foreach (var message in conversation)
        {
            if (found)
            {
                break;
            }

            if (message.Role != ChatRole.User)
            {
                continue;
            }

            foreach (var content in message.Content)
            {
                if (content is TextContent text)
                {
                    title += text.Text;
                    found = true;
                }
                else
                {
                    title += "📎 ";
                }
            }
        }

        if (title.Length > TitleLength)
        {
            title = title[..TitleLength] + "...";
        }

        return title.Length > 0 ? title : TranslationKey.Of(TranslationKey.Conversation);
    }

    /// <summary>The samples a chat was given, as the examples a conversation offers.</summary>
    /// <param name="examples">The samples, or <see langword="null"/>.</param>
    /// <param name="labels">The labels shown instead of the samples, or <see langword="null"/>.</param>
    /// <param name="icons">The pictures shown beside the samples, or <see langword="null"/>.</param>
    /// <returns>The examples, in the order they were given.</returns>
    /// <remarks>
    /// A sample is the message on its own, or — for a chat whose samples fill in the additional
    /// inputs — a list whose first entry is the message. Only the message reaches the conversation:
    /// the additional values belong to the gallery, which draws them as a row.
    /// </remarks>
    private List<ChatExample> ExampleMessages(
        object? examples,
        IReadOnlyList<string>? labels,
        IReadOnlyList<string>? icons)
    {
        var messages = new List<ChatExample>();

        if (examples is null)
        {
            return messages;
        }

        var index = 0;

        foreach (var sample in Samples(examples))
        {
            var message = sample is System.Collections.IList row && row.Count > 0 ? row[0] : sample;

            var example = ExampleOf(message);

            if (labels is not null && index < labels.Count)
            {
                example.DisplayText = labels[index];
            }

            var icon = icons is not null && index < icons.Count ? icons[index] : null;

            if (icon is not null)
            {
                example.Icon = icon;
            }
            else if (_multimodal && example.Files is not { Count: > 0 })
            {
                // A multimodal example that carries no file still needs a picture to be drawn with,
                // and the Python version draws a text badge rather than nothing.
                example = example.WithTextBadge();
            }

            messages.Add(example);
            index++;
        }

        return messages;
    }

    /// <summary>One sample, as the example a conversation offers.</summary>
    /// <param name="message">The sample's message.</param>
    /// <returns>The example.</returns>
    private static ChatExample ExampleOf(object? message) => message switch
    {
        string text => new ChatExample { Text = text },
        MultimodalText multimodal => new ChatExample
        {
            Text = multimodal.Text,
            Files = [.. multimodal.Files.Select(file => file.Path ?? string.Empty)],
        },
        JsonObject document => new ChatExample
        {
            Text = JsonValues.Text(document["text"]) ?? string.Empty,
            Files = document["files"] is JsonArray files
                ? [.. files.Select(file => JsonValues.Text(file?["path"]) ?? string.Empty)]
                : [],
        },
        null => new ChatExample { Text = string.Empty },
        _ => throw new ArgumentException(
            $"A sample is a text or a multimodal value; got {message.GetType().Name}.",
            nameof(message)),
    };

    /// <summary>The handler a sample is run through, for the gallery's own cache.</summary>
    /// <returns>A delegate that takes one value per gallery input.</returns>
    /// <remarks>
    /// A sample is answered from an empty conversation: the gallery is showing what the handler
    /// says to that message, not what it says to it after everything else the user has said.
    /// </remarks>
    private Delegate ExamplesHandler()
    {
        var arity = 1 + _additionalInputs.Count;

        return StreamsResult
            ? CollectingDelegate.OfStreamingWithRequest(arity, ExamplesStreamAsync)
            : CollectingDelegate.OfWithRequest(arity, ExamplesAsync);
    }

    /// <summary>Answers one sample.</summary>
    /// <param name="values">The sample's message, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>The sample's conversation.</returns>
    /// <remarks>
    /// The handler is called with an empty conversation in the second position, which is where a
    /// sample differs from a message: what a sample is showing is what the handler says to that
    /// message on its own, not what it says to it after everything else the user has said.
    /// </remarks>
    private async Task<object?> ExamplesAsync(object?[] values, Request request)
    {
        var message = values.Length > 0 ? values[0] : null;
        var response = await CallAsync(SampleArguments(message, values), request).ConfigureAwait(false);

        return ProcessExample(message, response);
    }

    /// <summary>Answers one sample a chunk at a time.</summary>
    /// <param name="values">The sample's message, then one value per additional input.</param>
    /// <param name="request">Who is asking, which the application's handler may read.</param>
    /// <returns>The sample's conversation, as it grows.</returns>
    private async IAsyncEnumerable<object?> ExamplesStreamAsync(object?[] values, Request request)
    {
        var message = values.Length > 0 ? values[0] : null;
        var result = await CallAsync(SampleArguments(message, values), request).ConfigureAwait(false);

        if (result is not IAsyncEnumerable<object?> chunks)
        {
            yield return ProcessExample(message, result);

            yield break;
        }

        await foreach (var chunk in chunks.ConfigureAwait(false))
        {
            yield return ProcessExample(message, chunk);
        }
    }

    /// <summary>The arguments a sample's handler is called with.</summary>
    /// <param name="message">The sample's message.</param>
    /// <param name="values">The sample's message, then one value per additional input.</param>
    /// <returns>The message, an empty conversation, then the additional values.</returns>
    private static object?[] SampleArguments(object? message, object?[] values) =>
        [message, new List<ChatMessage>(), .. values[1..]];

    /// <summary>A sample's conversation: what was asked, and what was answered.</summary>
    /// <param name="message">The sample's message.</param>
    /// <param name="response">The handler's answer.</param>
    /// <returns>The messages a cached sample is replayed as.</returns>
    /// <remarks>
    /// A multimodal sample becomes one user message per file and then one for its text, which is
    /// how the Python version keeps a file and the words about it apart: the conversation shows the
    /// picture, and the words are what the handler was asked.
    /// </remarks>
    private static List<ChatMessage> ProcessExample(object? message, object? response)
    {
        var result = new List<ChatMessage>();

        switch (message)
        {
            case MultimodalText multimodal:
                foreach (var file in multimodal.Files)
                {
                    result.Add(new ChatMessage(ChatContent.FileBlock(file), ChatRole.User));
                }

                result.Add(new ChatMessage(multimodal.Text, ChatRole.User));
                break;

            case JsonObject document:
                if (document["files"] is JsonArray files)
                {
                    foreach (var file in files)
                    {
                        if (FileData.FromJson(file) is { } described)
                        {
                            result.Add(new ChatMessage(ChatContent.FileBlock(described), ChatRole.User));
                        }
                    }
                }

                result.Add(new ChatMessage(JsonValues.Text(document["text"]) ?? string.Empty, ChatRole.User));
                break;

            default:
                result.Add(new ChatMessage(message?.ToString() ?? string.Empty, ChatRole.User));
                break;
        }

        result.AddRange(MessageOf(response, ChatRole.Assistant));

        return result;
    }
}

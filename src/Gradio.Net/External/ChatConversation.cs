using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// How a loaded chat turns a conversation and a new message into what an OpenAI-compatible endpoint
/// is asked: one list of messages, each with a role and content.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>format_conversation</c>. The conversation the chat holds is
/// gradio's own shape — a role and a list of content blocks, one of which may be a file — and the
/// endpoint's is the same idea with a narrower set of blocks, so a message crosses over block by
/// block: text and components travel as they are, and a file becomes either the text appended to
/// the prompt or an image the endpoint can look at.
/// </para>
/// <para>
/// The message is appended rather than merged into the conversation, because that is what the
/// endpoint is asked: the conversation so far, and then what the user just said.
/// </para>
/// </remarks>
internal static class ChatConversation
{
    /// <summary>The conversation as the endpoint is sent it.</summary>
    /// <param name="history">The conversation so far.</param>
    /// <param name="message">What the user sent: a text, or a text with files.</param>
    /// <param name="systemMessage">The system message to open with, or <see langword="null"/> for none.</param>
    /// <param name="client">The client to fetch a file that is served rather than stored.</param>
    /// <param name="cancellationToken">Cancels the fetches.</param>
    /// <returns>The messages, in the order the endpoint reads them.</returns>
    /// <remarks>
    /// The system message is a message of its own with a text for its content, which is the shape
    /// the Python version builds it in, and it is sent with every request rather than only the
    /// first: a system message that stopped being sent once the conversation had something in it
    /// would stop being an instruction. The Python version 6.30.0 sends it only when the
    /// conversation is empty, and reads it back as a list of blocks, so a chat loaded with a system
    /// message raises on its first message — see <see cref="External.LoadChat"/> for what is done
    /// instead.
    /// </remarks>
    internal static async Task<JsonArray> FormatAsync(
        IReadOnlyList<ChatMessage> history,
        object? message,
        string? systemMessage,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(history);

        var conversation = new JsonArray();

        if (systemMessage is not null)
        {
            conversation.Add(new JsonObject
            {
                ["role"] = ChatRole.System,
                ["content"] = systemMessage,
            });
        }

        foreach (var entry in history)
        {
            conversation.Add(await FromHistoryAsync(entry, client, cancellationToken).ConfigureAwait(false));
        }

        var (text, files) = Split(message);
        var prompts = new List<string>();
        var images = new List<string>();

        foreach (var file in files)
        {
            if (Attachments.IsTextEncoded(file))
            {
                prompts.Add(await Attachments
                    .AsPromptAsync(file, client, cancellationToken)
                    .ConfigureAwait(false));
            }
            else
            {
                images.Add(file);
            }
        }

        // An image is a message of its own: the endpoint reads a message's content as a list of
        // blocks, and a picture is one block. The Python version sends them before the text so that a model
        // reading the conversation sees what was attached and then what was asked about it.
        foreach (var image in images)
        {
            conversation.Add(new JsonObject
            {
                ["role"] = ChatRole.User,
                ["content"] = new JsonArray(new JsonObject
                {
                    ["type"] = "image_url",
                    ["image_url"] = new JsonObject
                    {
                        ["url"] = await Attachments
                            .AsDataUrlAsync(image, client, cancellationToken)
                            .ConfigureAwait(false),
                    },
                }),
            });
        }

        if (text.Length > 0 || prompts.Count > 0)
        {
            conversation.Add(new JsonObject
            {
                ["role"] = ChatRole.User,
                ["content"] = new JsonArray(new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = text + string.Join("\n", prompts),
                }),
            });
        }

        return conversation;
    }

    /// <summary>One message of the conversation, as the endpoint is sent it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="client">The client to fetch a file that is served rather than stored.</param>
    /// <param name="cancellationToken">Cancels the fetches.</param>
    /// <returns>The message, with its content blocks rewritten.</returns>
    /// <remarks>
    /// The message keeps what it carries besides its content — its metadata and its options —
    /// because the Python version rewrites the content of the message it was given rather than
    /// building a new one. A file block is the block that changes: the conversation draws the
    /// attachment, and the endpoint is sent what the attachment means.
    /// </remarks>
    private static async Task<JsonObject> FromHistoryAsync(
        ChatMessage message,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var content = new JsonArray();

        foreach (var block in message.Content)
        {
            if (block is not FileContent file)
            {
                content.Add(block.ToJson());
                continue;
            }

            var path = file.File.Path ?? string.Empty;

            // A text file that has already been sent stays text. Sending it as an image on the
            // second turn is what the Python version used to do, and a model that cannot see images then
            // rejects every message after the first.
            content.Add(Attachments.IsTextEncoded(path)
                ? new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = await Attachments
                        .AsPromptAsync(path, client, cancellationToken)
                        .ConfigureAwait(false),
                }
                : new JsonObject
                {
                    ["type"] = "image_url",
                    ["image_url"] = new JsonObject
                    {
                        ["url"] = await Attachments
                            .AsDataUrlAsync(path, client, cancellationToken)
                            .ConfigureAwait(false),
                    },
                });
        }

        var json = message.ToJson();
        json["content"] = content;

        return json;
    }

    /// <summary>What a message carries: its text, and the paths of its files.</summary>
    /// <param name="message">The message: a text, or a text with files.</param>
    /// <returns>The text and the paths.</returns>
    private static (string Text, IReadOnlyList<string> Files) Split(object? message) => message switch
    {
        null => (string.Empty, []),
        string text => (text, []),
        MultimodalText multimodal => (
            multimodal.Text,
            [.. multimodal.Files.Select(file => file.Path).OfType<string>()]),

        _ => throw new ArgumentException(
            $"A chat message is a text or a {nameof(MultimodalText)}; got a {message.GetType().Name}.",
            nameof(message)),
    };
}

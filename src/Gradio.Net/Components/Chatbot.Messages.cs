using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
namespace Gradio.Net;
/// <summary>
/// The chat's message model: what a conversation is normalized into on its way to the frontend.
/// </summary>
/// <remarks>
/// <para>
/// Kept apart from the component's parameters because it changes for its own reason. The props
/// change when the Python version's constructor does; this changes when the message model does, and
/// the normalization is where that model's rules live — the text is cleaned the way the Python
/// version cleans it, a file is described as the protocol carries one, and a thought is lifted out
/// of the answer it was written into.
/// </para>
/// <para>
/// One message can become several: a message whose text holds reasoning tags is split into the
/// thoughts and the prose around them, which is what the Python version does and what the frontend
/// expects to find — one collapsible message per thought.
/// </para>
/// </remarks>
public sealed partial class Chatbot
{
    /// <summary>
    /// The normalized conversation: what the frontend renders, whatever shape it arrived in.
    /// </summary>
    /// <param name="value">A list of messages, a list of message documents, or <see langword="null"/>.</param>
    /// <returns>The messages, one per rendered message.</returns>
    /// <exception cref="NotSupportedException"><paramref name="value"/> is not a conversation.</exception>
    /// <remarks>
    /// The raw documents are accepted as well as the model, because a client that sends a
    /// conversation and a handler that echoes it back are the same conversation; requiring the
    /// caller to rebuild the model to hand back what it just received would be a trap.
    /// </remarks>
    internal IReadOnlyList<ChatMessage> Normalize(object? value)
    {
        var messages = new List<ChatMessage>();

        foreach (var message in MessagesOf(value))
        {
            messages.AddRange(NormalizeOne(message));
        }

        return messages;
    }

    /// <summary>The messages a value carries.</summary>
    private static IEnumerable<ChatMessage> MessagesOf(object? value) => value switch
    {
        null => [],
        IEnumerable<ChatMessage> messages => messages,
        IEnumerable<JsonNode?> documents => documents.Select(ChatMessage.FromJson),
        _ => throw new NotSupportedException(
            "A chat's value is a list of messages; it was handed a "
            + $"{value.GetType()}. Each message is a ChatMessage, or the document a client sent."),
    };

    /// <summary>One message, normalized, which may be several rendered messages.</summary>
    private List<ChatMessage> NormalizeOne(ChatMessage message)
    {
        var content = message.Content.Select(NormalizeContent).ToArray();

        if (ReasoningTags is not { Count: > 0 } tags)
        {
            // A message with nothing left in it is not rendered at all, which is the Python version's rule:
            // an empty bubble is a thing the user sees rather than a thing the model says.
            return content.Length == 0
                ? []
                : [Rendered(message.Role, content, message.Metadata, message.Options)];
        }

        // The Python version lifts every tagged span into a message of its own and leaves the prose around it
        // as ordinary messages; content that is not text is carried by one more message at the end.
        var rendered = new List<ChatMessage>();
        foreach (var block in content)
        {
            if (block is not TextContent text)
            {
                continue;
            }

            foreach (var (segment, isReasoning, status) in Segments(text.Text, tags))
            {
                rendered.Add(isReasoning
                    ? new ChatMessage(segment)
                    {
                        Role = message.Role,
                        Metadata = new MessageMetadata { Title = "Reasoning", Status = status },
                    }
                    : Rendered(message.Role, [new TextContent(segment)], message.Metadata, message.Options));
            }
        }

        var other = content.Where(block => block is not TextContent).ToArray();
        if (other.Length > 0)
        {
            rendered.Add(Rendered(message.Role, other, message.Metadata, message.Options));
        }

        return rendered;
    }

    /// <summary>A message made of blocks that are already normalized.</summary>
    private static ChatMessage Rendered(
        string role,
        IReadOnlyList<ChatContent> content,
        MessageMetadata? metadata,
        IReadOnlyList<ChatOption>? options) =>
        new(content, role)
        {
            Metadata = metadata,
            Options = options,
        };

    /// <summary>One content block, as the protocol carries it.</summary>
    private ChatContent NormalizeContent(ChatContent block) => block switch
    {
        TextContent text => new TextContent(ChatText.Cleaned(text.Text)),
        FileContent file => new FileContent(NormalizedFile(file.File), file.AltText),
        _ => block,
    };

    /// <summary>A file as the protocol carries it: cached when the application holds it, named when it does not.</summary>
    /// <param name="file">The file, whose path may name anything.</param>
    /// <returns>The file to send, which is a new object rather than the one the handler returned.</returns>
    /// <remarks>
    /// <para>
    /// A chat message names its file by path rather than by address, which is the Python version's
    /// shape: the frontend builds the file endpoint's address from the path. The copy into the
    /// cache is what makes that address answerable for a file a handler produced, and it is why the
    /// caller's object is left alone — a streamed answer yields the same message repeatedly, and a
    /// value that changed under the handler would be a surprise the handler never asked for.
    /// </para>
    /// <para>
    /// The media type is filled in from the name when the caller did not say, because the frontend
    /// picks a renderer by it. That is a deliberate difference from the file and image components,
    /// which leave it unset and let the endpoint guess when it serves the bytes: the Python
    /// version's chat postprocess fills it in on the message, and the message is what the frontend
    /// reads.
    /// </para>
    /// </remarks>
    private FileData NormalizedFile(FileData file)
    {
        var described = new FileData
        {
            Path = file.Path,
            Url = file.Url,
            Size = file.Size,
            OrigName = file.OrigName,
            MimeType = file.MimeType ?? MediaTypes.Of(file.Path),
            IsStream = file.IsStream,
        };

        if (described.Path is not { Length: > 0 } path
            || MediaCache.NamesALocation(path)
            || MediaCache.IsInFolder(path, MediaFolder)
            || !System.IO.File.Exists(path))
        {
            return described;
        }

        described.Path = MediaFiles.Cached(path, MediaFolder, Root);

        return described;
    }

    /// <summary>A picture the config names, as a file the browser can fetch.</summary>
    /// <param name="path">A path or an address.</param>
    /// <returns>The file, described.</returns>
    /// <remarks>
    /// An address says nothing about its media type, because the server never sees the bytes: that
    /// is what the Python version's <c>serve_static_file</c> produces, and what the config is
    /// compared against. A file the application holds is copied into its cache, so that the browser
    /// can fetch it.
    /// </remarks>
    private FileData ServedFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var file = new FileData { Path = path };

        if (!MediaCache.NamesALocation(path))
        {
            file.MimeType = MediaTypes.Of(path);
        }

        return MediaFiles.Describe(file, MediaFolder, Root, measure: true);
    }

    /// <summary>The text split into prose and the reasoning between the tags, in order.</summary>
    /// <param name="text">The text.</param>
    /// <param name="tags">The tag pairs to look for.</param>
    /// <returns>One entry per segment: its text, whether it is reasoning, and the status to give it.</returns>
    /// <remarks>
    /// The Python version's rule, down to the details that show: a span whose closing tag has not
    /// arrived yet is <c>pending</c>, because a streamed answer that is still thinking is still
    /// thinking, and the prose around a span is trimmed and dropped when it is empty.
    /// </remarks>
    private static List<(string Text, bool IsReasoning, string Status)> Segments(
        string text,
        IReadOnlyList<ReasoningTag> tags)
    {
        var pattern = string.Join(
            "|",
            tags.Select(tag => $"({Regex.Escape(tag.Open)})(.*?)(?:{Regex.Escape(tag.Close)}|$)"));

        var segments = new List<(string Text, bool IsReasoning, string Status)>();
        var last = 0;

        foreach (Match match in Regex.Matches(text, pattern, RegexOptions.Singleline))
        {
            if (match.Index > last)
            {
                var prose = text[last..match.Index].Trim();
                if (prose.Length > 0)
                {
                    segments.Add((prose, false, MessageStatus.Done));
                }
            }

            // Each alternative contributes two groups — the opening tag and what it encloses — so
            // the content is the second group of whichever alternative matched.
            string? reasoning = null;
            for (var group = 2; group < match.Groups.Count; group += 2)
            {
                if (match.Groups[group].Success)
                {
                    reasoning = match.Groups[group].Value.Trim();
                    break;
                }
            }

            if (!string.IsNullOrEmpty(reasoning))
            {
                var closed = tags.Any(tag => match.Value.EndsWith(tag.Close, StringComparison.Ordinal));
                segments.Add((reasoning, true, closed ? MessageStatus.Done : MessageStatus.Pending));
            }

            last = match.Index + match.Length;
        }

        if (last < text.Length)
        {
            var prose = text[last..].Trim();
            if (prose.Length > 0)
            {
                segments.Add((prose, false, MessageStatus.Done));
            }
        }

        return segments;
    }
}

using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One message in a conversation: a role, the blocks it is made of, and what it optionally carries
/// besides.
/// </summary>
/// <remarks>
/// <para>
/// This is gradio 6's message, and the difference from what came before it is the point of this
/// batch: a message is a role plus a list of content blocks, not a pair of strings whose position
/// said who was speaking. The blocks are what makes a message able to hold a file or a chart as
/// well as a paragraph, and what lets a handler append to a conversation without re-encoding it.
/// </para>
/// <para>
/// The wire shape is the Python version's <c>Message</c> model: <c>role</c>, <c>metadata</c>,
/// <c>content</c>, <c>options</c>, with the last three written even when they are <c>null</c> or
/// empty — a client reading the document as a schema of four fields must find all four.
/// </para>
/// </remarks>
public sealed class ChatMessage : IConfigValue
{
    private string _role = ChatRole.Assistant;

    /// <summary>Creates a message that is one block of text.</summary>
    /// <param name="text">The text.</param>
    /// <param name="role">Who is speaking; the assistant unless the application says otherwise.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public ChatMessage(string text, string role = ChatRole.Assistant)
        : this([new TextContent(text)], role)
    {
    }

    /// <summary>Creates a message that is one content block.</summary>
    /// <param name="content">The block.</param>
    /// <param name="role">Who is speaking; the assistant unless the application says otherwise.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/>.</exception>
    public ChatMessage(ChatContent content, string role = ChatRole.Assistant)
        : this([content], role)
    {
    }

    /// <summary>Creates a message made of several content blocks.</summary>
    /// <param name="content">The blocks, in the order the frontend renders them.</param>
    /// <param name="role">Who is speaking; the assistant unless the application says otherwise.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/>.</exception>
    public ChatMessage(IEnumerable<ChatContent> content, string role = ChatRole.Assistant)
    {
        ArgumentNullException.ThrowIfNull(content);

        Content = [.. content];
        Role = role;
    }

    /// <summary>Who is speaking.</summary> <value>One of <see cref="ChatRole"/>'s three
    /// roles.</value> <exception cref="ArgumentException">The role is not one the Python version
    /// knows.</exception>
    public string Role
    {
        get => _role;
        set
        {
            if (!ChatRole.IsKnown(value))
            {
                throw new ArgumentException(
                    $"A message's role must be {ChatRole.User}, {ChatRole.Assistant} or {ChatRole.System}; got '{value}'.",
                    nameof(value));
            }

            _role = value;
        }
    }

    /// <summary>The blocks the message is made of, in the order they are rendered.</summary>
    public IReadOnlyList<ChatContent> Content { get; }

    /// <summary>What the message carries besides its content, or <see langword="null"/> for nothing.</summary>
    public MessageMetadata? Metadata { get; set; }

    /// <summary>The options the message offers, or <see langword="null"/> for none.</summary>
    public IReadOnlyList<ChatOption>? Options { get; set; }

    /// <summary>The message's text, which is the text blocks joined by a newline.</summary>
    /// <value>
    /// The empty string for a message made only of files or components. A convenience rather than a
    /// protocol field: a handler that only cares about what was said reads this, and one that cares
    /// about the whole message reads <see cref="Content"/>.
    /// </value>
    public string Text => string.Join("\n", Content.OfType<TextContent>().Select(block => block.Text));

    /// <inheritdoc />
    public JsonNode ToConfigValue() => ToJson();

    /// <summary>The message as the protocol carries it.</summary>
    /// <returns>A new node.</returns>
    internal JsonObject ToJson()
    {
        var content = new JsonArray();
        foreach (var block in Content)
        {
            content.Add(block.ToJson());
        }

        JsonNode? options = null;
        if (Options is not null)
        {
            var array = new JsonArray();
            foreach (var option in Options)
            {
                array.Add(option.ToJson());
            }

            options = array;
        }

        return new JsonObject
        {
            ["role"] = Role,
            ["metadata"] = Metadata?.ToJson(),
            ["content"] = content,
            ["options"] = options,
        };
    }

    /// <summary>Reads a message a client sent.</summary>
    /// <param name="value">The message, as the protocol carried it.</param>
    /// <returns>The message.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is not an object with a role and a content field.
    /// </exception>
    /// <remarks>
    /// The content is read as blocks, but a bare string or a single block is accepted where a list
    /// belongs: the Python version's model normalizes those on the way in, and a client that sends
    /// one is sending a message rather than something else.
    /// </remarks>
    internal static ChatMessage FromJson(JsonNode? value)
    {
        if (value is not JsonObject document)
        {
            throw new ArgumentException(
                $"A chat message must be an object with a role and a content field; got '{value?.ToJsonString() ?? "null"}'.",
                nameof(value));
        }

        var role = JsonValues.Text(document["role"])
            ?? throw new ArgumentException(
                $"A chat message must name its role; got '{document.ToJsonString()}'.",
                nameof(value));

        var content = new List<ChatContent>();
        switch (document["content"])
        {
            case null:
                throw new ArgumentException(
                    $"A chat message must carry content; got '{document.ToJsonString()}'.",
                    nameof(value));

            case JsonArray blocks:
                foreach (var block in blocks)
                {
                    content.Add(ChatContent.FromJson(block));
                }

                break;

            default:
                content.Add(ChatContent.FromJson(document["content"]));
                break;
        }

        return new ChatMessage(content, role)
        {
            Metadata = MetadataOf(document["metadata"]),
            Options = OptionsOf(document["options"]),
        };
    }

    /// <summary>The options a document carries, or <see langword="null"/> for none.</summary>
    private static List<ChatOption>? OptionsOf(JsonNode? value)
    {
        if (value is not JsonArray options)
        {
            return null;
        }

        return [.. options.Select(option => new ChatOption(
            JsonValues.Text(option?["value"]) ?? string.Empty,
            JsonValues.Text(option?["label"])))];
    }

    /// <summary>The metadata a document carries, or <see langword="null"/> for none.</summary>
    private static MessageMetadata? MetadataOf(JsonNode? value)
    {
        if (value is not JsonObject metadata)
        {
            return null;
        }

        return new MessageMetadata
        {
            Title = JsonValues.Text(metadata["title"]),
            Id = JsonValues.ToObject(metadata["id"]),
            ParentId = JsonValues.ToObject(metadata["parent_id"]),
            Log = JsonValues.Text(metadata["log"]),
            Duration = JsonValues.Number(metadata["duration"]),
            Status = JsonValues.Text(metadata["status"]),
        };
    }
}

/// <summary>
/// The one text transformation the Python version applies to a message: <c>inspect.cleandoc</c>.
/// </summary>
/// <remarks>
/// A handler that writes an indented literal, or a template that starts with a newline, would
/// otherwise send the indentation to the frontend as part of the message. The Python version cleans
/// the text on the way out, so the same message has to leave this framework the same way — the
/// frontend renders the difference.
/// </remarks>
internal static class ChatText
{
    /// <summary>The column a tab advances to, which is <c>str.expandtabs</c>'s default.</summary>
    private const int TabWidth = 8;

    /// <summary>The text with its common indentation and its surrounding blank lines removed.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The cleaned text.</returns>
    internal static string Cleaned(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var lines = ExpandTabs(text).Split('\n').ToList();

        // The margin is taken from the lines after the first, because the first line of a literal
        // is where the quotes are rather than where the text starts.
        var margin = int.MaxValue;
        foreach (var line in lines.Skip(1))
        {
            var content = line.TrimStart();
            if (content.Length > 0)
            {
                margin = Math.Min(margin, line.Length - content.Length);
            }
        }

        if (lines.Count > 0)
        {
            lines[0] = lines[0].TrimStart();
        }

        if (margin < int.MaxValue)
        {
            for (var index = 1; index < lines.Count; index++)
            {
                lines[index] = lines[index].Length >= margin ? lines[index][margin..] : string.Empty;
            }
        }

        while (lines.Count > 0 && lines[^1].Length == 0)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        while (lines.Count > 0 && lines[0].Length == 0)
        {
            lines.RemoveAt(0);
        }

        return string.Join("\n", lines);
    }

    /// <summary>Tabs expanded to the next tab stop, as Python's <c>str.expandtabs</c>
    /// does.</summary> <param name="text">The text.</param> <returns>The text with spaces where its
    /// tabs were.</returns>
    /// <remarks>
    /// A tab advances to the next multiple of eight <em>columns</em> rather than standing for eight
    /// spaces, so <c>"a\tb"</c> gains seven spaces and <c>"\t b"</c> gains seven and then keeps the
    /// space. Replacing each tab with eight spaces would shift every line that has text before it.
    /// </remarks>
    private static string ExpandTabs(string text)
    {
        if (!text.Contains('\t', StringComparison.Ordinal))
        {
            return text;
        }

        var expanded = new System.Text.StringBuilder(text.Length + 8);
        var column = 0;

        foreach (var character in text)
        {
            if (character == '\t')
            {
                var spaces = TabWidth - (column % TabWidth);
                expanded.Append(' ', spaces);
                column += spaces;

                continue;
            }

            expanded.Append(character);
            column = character == '\n' ? 0 : column + 1;
        }

        return expanded.ToString();
    }
}

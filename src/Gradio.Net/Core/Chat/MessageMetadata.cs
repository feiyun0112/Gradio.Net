using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What a message carries besides its text: a title, an identity to nest it by, and how long the
/// work behind it took.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>metadata</c> dictionary, which is how a message is displayed as a
/// thought rather than as an answer: a title makes it collapsible, an id and a parent id nest one
/// thought inside another, a log and a duration are shown next to the title in a subdued font.
/// </para>
/// <para>
/// A field that was not set is left out rather than written as <c>null</c>, because the Python
/// version's is a typed dictionary rather than a model: a message that says nothing about its
/// duration says nothing, which is not the same as saying it took no time.
/// </para>
/// </remarks>
public sealed class MessageMetadata : IConfigValue
{
    private string? _status;

    /// <summary>The title shown for a thought, which is what makes the message collapsible.</summary>
    public string? Title { get; set; }

    /// <summary>The identity of this thought, which a nested thought names as its parent.</summary>
    /// <value>A number or a string, as the Python version's dictionary allows.</value>
    public object? Id { get; set; }

    /// <summary>The identity of the thought this one sits inside.</summary>
    /// <value>A number or a string, as the Python version's dictionary allows.</value>
    public object? ParentId { get; set; }

    /// <summary>A note shown next to the title, in a subdued font.</summary>
    public string? Log { get; set; }

    /// <summary>How long the work behind this message took, in seconds.</summary>
    public double? Duration { get; set; }

    /// <summary>Whether the work behind this message is still running.</summary>
    /// <value><see cref="MessageStatus.Pending"/>, <see cref="MessageStatus.Done"/>, or <see langword="null"/> for neither.</value>
    public string? Status
    {
        get => _status;
        set
        {
            if (value is not null && !MessageStatus.IsKnown(value))
            {
                throw new ArgumentException(
                    $"A message's status must be {MessageStatus.Pending} or {MessageStatus.Done}; got '{value}'.",
                    nameof(value));
            }

            _status = value;
        }
    }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => ToJson();

    /// <summary>The metadata as the protocol carries it.</summary>
    /// <returns>A new node holding the fields that were set.</returns>
    internal JsonObject ToJson()
    {
        var metadata = new JsonObject();

        if (Title is not null)
        {
            metadata["title"] = Title;
        }

        if (Id is not null)
        {
            metadata["id"] = JsonValues.FromObject(Id);
        }

        if (ParentId is not null)
        {
            metadata["parent_id"] = JsonValues.FromObject(ParentId);
        }

        if (Log is not null)
        {
            metadata["log"] = Log;
        }

        if (Duration is not null)
        {
            metadata["duration"] = Duration;
        }

        if (Status is not null)
        {
            metadata["status"] = Status;
        }

        return metadata;
    }
}

/// <summary>
/// One option a message offers, which the user may pick instead of typing
/// </summary>
/// <remarks>
/// The value is what the application receives back through the <c>option_select</c> event; the
/// label is what the user reads, and is the value itself when it is not set.
/// </remarks>
public sealed class ChatOption : IConfigValue
{
    /// <summary>Creates an option.</summary>
    /// <param name="value">The value the application receives when the option is picked.</param>
    /// <param name="label">The text shown, or <see langword="null"/> to show the value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public ChatOption(string value, string? label = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
        Label = label;
    }

    /// <summary>The value the application receives when the option is picked.</summary>
    public string Value { get; }

    /// <summary>The text shown, or <see langword="null"/> to show the value.</summary>
    public string? Label { get; }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => ToJson();

    /// <summary>The option as the protocol carries it.</summary>
    /// <returns>A new node.</returns>
    internal JsonObject ToJson() => Label is null
        ? new JsonObject { ["value"] = Value }
        : new JsonObject { ["value"] = Value, ["label"] = Label };
}

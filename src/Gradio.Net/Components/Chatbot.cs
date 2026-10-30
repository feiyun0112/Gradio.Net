using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A conversation: the messages so far, rendered as text, files and components, and able to grow
/// while an answer is being written.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. <see cref="Value"/> and <see cref="Buttons"/> have
/// defaults that are computed rather than stored — an empty conversation, and the three buttons the
/// Python version shows when the application names none — because the Python version computes them
/// where every argument is known.
/// </para>
/// <para>
/// The value is gradio 6's message model: a list of <see cref="ChatMessage"/>, each a role and a
/// list of content blocks. There is no tuple spelling and no switch that selects one, which is what
/// the Python version removed; a handler appends to the list it was given and returns it, and a
/// generator yields the list as it grows.
/// </para>
/// <para>
/// What the frontend receives is the <em>normalized</em> conversation: every text block cleaned the
/// way the Python version cleans it, every file described as the protocol carries one, and — when
/// <see cref="ReasoningTags"/> is set — every thought lifted into a collapsible message of its own.
/// That normalization is what a handler's result goes through on the way out, and what the config's
/// <c>value</c> carries, so a page that starts with a conversation and a run that produces one
/// describe it the same way.
/// </para>
/// </remarks>
[GradioBlock("chatbot")]
[HandlerValue("IReadOnlyList<ChatMessage>", "change", "select", "like", "retry", "undo", "example_select", "option_select", "edit", "copy", "clear")]
public sealed partial class Chatbot : Component
{
    internal Chatbot()
    {
    }

    /// <summary>The buttons a chat shows when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultButtons = ["share", "copy", "copy_all"];

    /// <summary>The feedback options a chat offers when the application names none.</summary>
    private static readonly IReadOnlyList<string> _defaultFeedback = ["Like", "Dislike"];

    private IReadOnlyList<ChatMessage>? _value;
    private IReadOnlyList<LatexDelimiter> _latexDelimiters = [new LatexDelimiter()];
    private IReadOnlyList<string> _buttons = _defaultButtons;
    private IReadOnlyList<string>? _feedbackOptions = _defaultFeedback;
    private IReadOnlyList<ChatExample>? _examples;
    private AvatarImages _avatars = new();
    private string? _editable;
    private string? _layout;
    private object _allowTags = true;

    /// <summary>The conversation the component starts with.</summary>
    /// <value>
    /// The messages as the application wrote them; read back as the normalized conversation, which
    /// is what the frontend renders and what the config carries.
    /// </value>
    /// <remarks>
    /// Normalized on read rather than on write for the same reason a file's value is postprocessed
    /// on read: a file block has to be copied into the application's cache, and the application is
    /// known only once the component has been rendered into one.
    /// </remarks>
    public IReadOnlyList<ChatMessage> Value
    {
        get => Normalize(_value);
        set => _value = value;
    }

    /// <summary>The label drawn above the conversation.</summary>
    public override string? Label { get; set; }

    /// <summary>Whether the conversation scrolls to its newest message.</summary>
    public bool Autoscroll { get; set; } = true;

    /// <summary>The relative width of the conversation next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the conversation wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The height of the conversation, in pixels or as a CSS length.</summary>
    public object? Height { get; set; } = 400;

    /// <summary>Whether the user may drag the conversation's corner to resize it.</summary>
    public bool Resizable { get; set; }

    /// <summary>The height the conversation grows to at most, in pixels or as a CSS length.</summary>
    public object? MaxHeight { get; set; }

    /// <summary>The height the conversation shrinks to at least, in pixels or as a CSS length.</summary>
    public object? MinHeight { get; set; }

    /// <summary>Which messages the user may edit.</summary>
    /// <value><c>user</c>, <c>all</c>, or <see langword="null"/> for none.</value>
    public string? Editable
    {
        get => _editable;
        set
        {
            if (value is not null && value is not ("user" or "all"))
            {
                throw new ArgumentException(
                    $"A chat's editable must be user, all or null; got '{value}'.",
                    nameof(value));
            }

            _editable = value;
        }
    }

    /// <summary>The delimiters LaTeX is rendered between.</summary>
    /// <value>
    /// One <c>$$</c> delimiter unless the application says otherwise; an empty list turns LaTeX
    /// rendering off.
    /// </value>
    public IReadOnlyList<LatexDelimiter> LatexDelimiters
    {
        get => _latexDelimiters;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _latexDelimiters = value;
        }
    }

    /// <summary>Whether the conversation runs right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>The buttons drawn in the conversation's toolbar.</summary>
    /// <value>
    /// <c>share</c>, <c>copy</c> and <c>copy_all</c> unless the application says otherwise.
    /// </value>
    /// <remarks>
    /// Names only, for now: the Python version also accepts a button component, and the button
    /// family is its own batch (ticket 15).
    /// </remarks>
    public IReadOnlyList<string> Buttons
    {
        get => _buttons;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _buttons = value;
        }
    }

    /// <summary>The note appended to a copied message, or <see langword="null"/> for none.</summary>
    public string? Watermark { get; set; }

    /// <summary>The pictures shown beside the messages.</summary>
    /// <value>
    /// The two pictures the application gave; an empty pair, which the frontend draws nothing for,
    /// unless it gave none.
    /// </value>
    /// <remarks>
    /// Read back with both pictures resolved into files the browser can fetch, which is the shape
    /// the config carries. An application that says nothing about avatars gets the empty pair
    /// rather than an absent key, because that is what the Python version's config carries for one.
    /// </remarks>
    public AvatarImages AvatarImages
    {
        get => _avatars.Resolved(ServedFile);
        set => _avatars = value ?? new AvatarImages();
    }

    /// <summary>Whether HTML in a message is sanitized before it is rendered.</summary>
    public bool SanitizeHtml { get; set; } = true;

    /// <summary>Whether a message's Markdown is rendered.</summary>
    public bool RenderMarkdown { get; set; } = true;

    /// <summary>The feedback options offered for a message.</summary>
    /// <value><c>Like</c> and <c>Dislike</c> unless the application says otherwise.</value>
    public IReadOnlyList<string>? FeedbackOptions
    {
        get => _feedbackOptions;
        set => _feedbackOptions = value;
    }

    /// <summary>The feedback already given, one entry per assistant message.</summary>
    /// <value>
    /// A list whose entries are an option's value or <see langword="null"/>; <see langword="null"/>
    /// for a conversation that has none.
    /// </value>
    public IReadOnlyList<string?>? FeedbackValue { get; set; }

    /// <summary>Whether a single newline in a message ends a line.</summary>
    public bool LineBreaks { get; set; } = true;

    /// <summary>How the conversation is laid out.</summary>
    /// <value><c>panel</c>, <c>bubble</c>, or <see langword="null"/> for the frontend's default.</value>
    public string? Layout
    {
        get => _layout;
        set
        {
            if (value is not null && value is not ("panel" or "bubble"))
            {
                throw new ArgumentException(
                    $"A chat's layout must be panel, bubble or null; got '{value}'.",
                    nameof(value));
            }

            _layout = value;
        }
    }

    /// <summary>The text shown while the conversation is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The examples offered before the conversation starts.</summary>
    /// <remarks>
    /// Read back with each example's icon and files resolved into files the browser can fetch.
    /// </remarks>
    public IReadOnlyList<ChatExample>? Examples
    {
        get => _examples?.Select(example => example.Resolved(ServedFile)).ToArray();
        set => _examples = value;
    }

    /// <summary>Whether a message holding a file offers to download it.</summary>
    public bool AllowFileDownloads { get; set; } = true;

    /// <summary>Whether consecutive messages from one speaker share a bubble.</summary>
    public bool GroupConsecutiveMessages { get; set; } = true;

    /// <summary>Which custom HTML tags a message may keep.</summary>
    /// <value>
    /// <see langword="true"/> for all of them, <see langword="false"/> for none, or the names of
    /// the ones to keep — the Python version's union of a flag and a list.
    /// </value>
    public object AllowTags
    {
        get => _allowTags;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is not bool && value is not IReadOnlyList<string>)
            {
                throw new ArgumentException(
                    "A chat's allow_tags must be a boolean or a list of tag names; "
                    + $"got a {value.GetType()}.",
                    nameof(value));
            }

            _allowTags = value;
        }
    }

    /// <summary>The tag pairs whose content is shown as reasoning.</summary>
    public IReadOnlyList<ReasoningTag>? ReasoningTags { get; set; }

    /// <summary>Whether the user's own messages can be liked and disliked.</summary>
    public bool LikeUserMessage { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => ChatSchemas.ChatbotSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonArray(
        new ChatMessage("Hello!", ChatRole.User).ToJson(),
        new ChatMessage("How can I help you?").ToJson());

    /// <inheritdoc />
    /// <remarks>
    /// A chat declares the three events whose <c>config_data</c> says something about the component
    /// rather than about a parameter: it can be liked, and a message can be retried or undone. Each
    /// is reported as false until a handler is bound to it, which is when the Python version's
    /// listener turns it on — the keys are written before the props, which is the order the Python
    /// version merges them in.
    /// </remarks>
    protected internal override void WriteEventConfig(PropWriter props)
    {
        base.WriteEventConfig(props);
        props.Add("_undoable", IsBound(EventNames.Undo));
        props.Add("_retryable", IsBound(EventNames.Retry));
        props.Add("likeable", IsBound(EventNames.Like));
    }

    /// <summary>
    /// The conversation a handler reads.
    /// </summary>
    /// <param name="value">The messages the client sent.</param>
    /// <returns>The messages as the message model's own types.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is neither a list nor absent.</exception>
    /// <remarks>
    /// <para>
    /// A client sends the normalized conversation — that is what it was given — so this is the
    /// inverse of <see cref="Postprocess"/> rather than a second normalization: a text block stays
    /// the text that was sent, and a file block stays the file the client named.
    /// </para>
    /// <para>
    /// An absent value is an empty conversation, which is what the Python version's preprocess
    /// answers for one. A value of another shape is refused rather than read as empty: a client
    /// that sent a string where a list of messages belongs sent something this component does not
    /// understand, and answering "no messages" would hide that.
    /// </para>
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return Array.Empty<ChatMessage>();
        }

        if (value is not JsonArray messages)
        {
            throw new ArgumentException(
                $"A chat's value is a list of messages; the client sent '{value.ToJsonString()}'.",
                nameof(value));
        }

        return messages.Select(ChatMessage.FromJson).ToArray();
    }

    /// <summary>
    /// The conversation the client receives from what a handler returned.
    /// </summary>
    /// <param name="value">The messages, or the raw messages a client sent.</param>
    /// <returns>The normalized conversation.</returns>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> is neither a list of messages nor <see langword="null"/>.
    /// </exception>
    public override JsonNode? Postprocess(object? value)
    {
        var messages = Normalize(value);
        var conversation = new JsonArray();

        foreach (var message in messages)
        {
            conversation.Add(message.ToJson());
        }

        return conversation;
    }
}

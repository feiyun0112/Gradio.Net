using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A structured dialogue, shown as turns between speakers, each tagged and coloured
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a list of turns — a speaker, a tag and
/// some text — that the frontend renders as a transcript with the colours and the layout the
/// component's parameters describe.
/// </para>
/// <para>
/// Two of the parameters are callables the application may supply: a <see cref="Formatter"/> that
/// turns a value into the transcript, and an <see cref="Unformatter"/> that turns a transcript back
/// into a value. Both are written as <see langword="null"/> in the config — a callable's Python
/// repr is not something a second implementation can reproduce — which is also what the Python
/// version's serializer makes of them.
/// </para>
/// <para>
/// Note that the Python manifest for <c>dialogue</c> does <em>not</em> declare
/// <c>preserved_by_key</c>, so this component does not write it even though the base class declares
/// the member. A parameter not in the manifest is not generated, which is how the build enforces
/// the difference.
/// </para>
/// </remarks>
[GradioBlock("dialogue")]
[HandlerValue("object?", "change", "input", "submit")]
public sealed partial class Dialogue : Component
{
    internal Dialogue()
    {
    }

    /// <summary>The hint the Python version shows when the application names no info of its
    /// own.</summary>
    private const string DefaultInfo =
        "Type colon (:) in the dialogue line to see the available tags";

    private string? _info = DefaultInfo;
    private string? _separator = "\n";

    /// <summary>The dialogue the component starts with, or <see langword="null"/> for none.</summary>
    /// <value>
    /// A list of turns, a string the <see cref="Formatter"/> turns into turns, or
    /// <see langword="null"/> for an empty dialogue. The value is arbitrary because the formatter
    /// decides its shape, so nothing here interprets it.
    /// </value>
    public object? Value { get; set; }

    /// <summary>The kind of dialogue the component holds.</summary>
    /// <value>
    /// <c>text</c> unless the application says otherwise: the Python version starts a dialogue as a
    /// single text field, and <c>list</c> makes it a list of turns.
    /// </value>
    public string? Type { get; set; } = "text";

    /// <summary>The names of the speakers a turn may belong to.</summary>
    /// <value>
    /// <see langword="null"/> unless the application names them: the Python version writes no list
    /// at all for a dialogue without speakers, which is what makes it a plain text field.
    /// </value>
    public IReadOnlyList<string>? Speakers { get; set; }

    /// <summary>
    /// The function that turns a value into the transcript the frontend renders.
    /// </summary>
    /// <value>
    /// A callable the application supplies, or <see langword="null"/> for the default. Written as
    /// <see langword="null"/> in the config — a callable's repr is not reproducible — which is what
    /// the Python version's serializer makes of it.
    /// </value>
    public Delegate? Formatter { get; set; }

    /// <summary>
    /// The function that turns a transcript back into a value.
    /// </summary>
    /// <value>
    /// The inverse of <see cref="Formatter"/>; <see langword="null"/> for the default, and written
    /// as <see langword="null"/> for the same reason.
    /// </value>
    public Delegate? Unformatter { get; set; }

    /// <summary>The tags a turn may carry, in the order they are listed.</summary>
    /// <value>An empty list unless the application names them.</value>
    public IReadOnlyList<string> Tags { get; set; } = [];

    /// <summary>The text put between two turns when the transcript is rendered as a
    /// string.</summary> <value>A newline unless the application says otherwise.</value>
    /// <exception cref="ArgumentException">The separator is a single space, which the Python
    /// version refuses because it cannot be told apart from the text.</exception>
    public string? Separator
    {
        get => _separator;
        set
        {
            if (value == " ")
            {
                throw new ArgumentException("Separator cannot be an empty string.", nameof(value));
            }

            _separator = value;
        }
    }

    /// <summary>The colour each tag is drawn in.</summary>
    /// <value>Hex codes or colour names, keyed by tag; <see langword="null"/> for the frontend's defaults.</value>
    public IReadOnlyDictionary<string, string>? ColorMap { get; set; }

    /// <summary>The label drawn above the dialogue.</summary>
    public override string? Label { get; set; } = "Dialogue";

    /// <summary>The note drawn under the dialogue.</summary>
    /// <value>
    /// The Python version's hint about tags, or the application's note — but only while the
    /// dialogue is interactive. The Python version clears it for a dialogue the user cannot edit,
    /// which is why the config of a read-only dialogue carries no note at all.
    /// </value>
    public string? Info
    {
        get => Interactive is true && _info is { Length: > 0 } ? _info : null;
        set => _info = value;
    }

    /// <summary>The hint shown while the dialogue is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The relative width of the dialogue next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the dialogue wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether the dialogue takes focus when the page loads.</summary>
    public bool Autofocus { get; set; }

    /// <summary>Whether the dialogue scrolls to its newest turn.</summary>
    public bool Autoscroll { get; set; } = true;

    /// <summary>How many lines the dialogue grows to at most, when it grows at all.</summary>
    public int? MaxLines { get; set; }

    /// <summary>The buttons drawn in the dialogue's corner.</summary>
    /// <value>No buttons unless the application asks for one.</value>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>
    /// The submit button drawn beside the dialogue.
    /// </summary>
    /// <value>
    /// A label for the button, <see langword="true"/> for the default, or <see langword="false"/>
    /// for none — the Python version's union of a string and a boolean, whose default is
    /// <see langword="false"/>.
    /// </value>
    public object? SubmitBtn { get; set; } = false;

    /// <summary>The mode the dialogue is drawn in.</summary>
    /// <value>
    /// <c>both</c> unless the application says otherwise: the Python version draws the turns and
    /// the text field together by default.
    /// </value>
    public string? UiMode { get; set; } = "both";

    /// <inheritdoc />
    /// <remarks>
    /// The Python version's <c>DialogueModel</c>: a list of turns, each a speaker and a text, or a
    /// single string. The definition of a turn travels with the schema, because the API document
    /// has no other place to name it.
    /// </remarks>
    public override JsonObject ApiInfo => new()
    {
        ["title"] = "DialogueModel",
        ["additional_description"] = null,
        ["anyOf"] = new JsonArray(
            new JsonObject
            {
                ["type"] = "array",
                ["items"] = new JsonObject { ["$ref"] = "#/$defs/DialogueLine" },
            },
            new JsonObject { ["type"] = "string" }),
        ["$defs"] = new JsonObject
        {
            ["DialogueLine"] = new JsonObject
            {
                ["title"] = "DialogueLine",
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["speaker"] = new JsonObject { ["title"] = "Speaker", ["type"] = "string" },
                    ["text"] = new JsonObject { ["title"] = "Text", ["type"] = "string" },
                },
                ["required"] = new JsonArray("speaker", "text"),
            },
        },
    };

    /// <inheritdoc />
    /// <remarks>The Python version's two example turns, which the API documentation
    /// shows.</remarks>
    public override JsonNode? ExampleInputs => new JsonArray(
        new JsonObject { ["speaker"] = "Speaker 1", ["text"] = "Hello, how are you?" },
        new JsonObject { ["speaker"] = "Speaker 2", ["text"] = "I'm fine, thank you!" });

    /// <summary>
    /// The value a handler reads.
    /// </summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>
    /// The turns as a list for a <c>list</c> dialogue, or the transcript as a string otherwise —
    /// the Python version formats the turns with the formatter and joins them with the separator,
    /// so a handler reading a text dialogue is handed text rather than the turns the browser sent.
    /// </returns>
    /// <exception cref="ArgumentException">A <c>list</c> dialogue was handed something that is not a list of turns.</exception>
    public override object? Preprocess(JsonNode? value)
    {
        if (string.Equals(Type, "list", StringComparison.Ordinal))
        {
            return TurnsOf(value);
        }

        return Format(value).GetValue<string>();
    }

    /// <summary>
    /// The value the frontend receives from what a handler returned.
    /// </summary>
    /// <param name="value">
    /// A transcript as a string, the turns as <see cref="DialogueTurn"/>s or as the documents a
    /// client sent, or <see langword="null"/> for an empty dialogue.
    /// </param>
    /// <returns>The string, or the turns written as the objects the frontend reads.</returns>
    /// <exception cref="NotSupportedException"><paramref name="value"/> is neither.</exception>
    /// <remarks>
    /// Both shapes are accepted because the Python version's own value is either one: a handler
    /// that transcribes a recording answers with turns, and one that echoes a transcript answers
    /// with the string it was given. A turn is handed back as it arrived rather than rebuilt, which
    /// is the same tolerance <see cref="Chatbot"/> has for its messages.
    /// </remarks>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is string text)
        {
            return JsonValue.Create(text);
        }

        var turns = new JsonArray();
        foreach (var turn in TurnsOfObject(value))
        {
            turns.Add(turn.ToJson());
        }

        return turns;
    }

    /// <summary>The turns a client's value carries.</summary>
    /// <param name="value">The value, or <see langword="null"/> for none.</param>
    /// <returns>The turns; empty for an absent value.</returns>
    /// <exception cref="ArgumentException">The value is not a list of turns.</exception>
    private static DialogueTurn[] TurnsOf(JsonNode? value)
    {
        if (value is null)
        {
            return [];
        }

        if (value is not JsonArray turns)
        {
            throw new ArgumentException(
                $"A dialogue's value is a list of turns; the client sent '{value.ToJsonString()}'.",
                nameof(value));
        }

        return [.. turns.Select(DialogueTurn.FromJson)];
    }

    /// <summary>The turns a handler's value carries.</summary>
    /// <param name="value">The value a handler returned.</param>
    /// <returns>The turns.</returns>
    /// <exception cref="NotSupportedException">The value is not a list of turns.</exception>
    private static IEnumerable<DialogueTurn> TurnsOfObject(object value) => value switch
    {
        IEnumerable<DialogueTurn> turns => turns,
        IEnumerable<JsonNode?> documents => documents.Select(DialogueTurn.FromJson),
        _ => throw new NotSupportedException(
            "A dialogue's value is a list of turns; it was handed a "
            + $"{value.GetType()}. Each turn is a DialogueTurn, or the document a client sent."),
    };
}

/// <summary>
/// One turn of a dialogue: who spoke, and what was said.
/// </summary>
/// <param name="Speaker">The speaker's name, as one of the dialogue's speakers.</param>
/// <param name="Text">What was said.</param>
/// <remarks>
/// The Python version's <c>DialogueLine</c> — the shape a turn takes on the wire and in the API
/// document. A record rather than a bare dictionary because a handler that produces a transcript
/// should not have to spell the keys, while a handler that echoes what it received can hand back
/// the documents instead.
/// </remarks>
public sealed record DialogueTurn(string Speaker, string Text)
{
    /// <summary>Reads a turn from the object a client sent.</summary>
    /// <param name="node">The object carrying <c>speaker</c> and <c>text</c>.</param>
    /// <returns>The turn, with an absent field read as empty.</returns>
    /// <exception cref="ArgumentException">The node is not an object.</exception>
    internal static DialogueTurn FromJson(JsonNode? node)
    {
        if (node is not JsonObject fields)
        {
            throw new ArgumentException(
                $"A dialogue turn is an object with a speaker and a text; got '{node?.ToJsonString()}'.",
                nameof(node));
        }

        return new DialogueTurn(
            JsonValues.Text(fields["speaker"]) ?? string.Empty,
            JsonValues.Text(fields["text"]) ?? string.Empty);
    }

    /// <summary>The turn as the object the frontend reads.</summary>
    /// <returns>The turn, written the way a client writes one.</returns>
    internal JsonObject ToJson() => new()
    {
        ["speaker"] = Speaker,
        ["text"] = Text,
    };
}

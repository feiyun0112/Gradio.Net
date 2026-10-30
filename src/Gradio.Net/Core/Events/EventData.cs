using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What an event says about the thing that raised it, as the handler bound to that event reads it.
/// </summary>
/// <remarks>
/// <para>
/// A binding's handler is handed the values of the components it reads, in order. Some events carry
/// something the components cannot: which row a user selected, which message they liked, where in a
/// conversation they edited. The Python version spells that as an extra argument whose type is one
/// of its event-data classes, and a handler that names one receives it wherever it sits in the
/// signature.
/// </para>
/// <para>
/// The family is the Python version's nine: a selection, a key press, a deleted file, a like, a
/// retry, an undo, an edit, a download and a copy. Every event that carries one of them is already
/// bindable here, so a handler that names one of these types receives the payload rather than
/// nothing — which is why the classes are not added one per component: the runner fills the
/// parameter by its type, and a type that is missing is a payload that silently arrives as
/// <see langword="null"/>.
/// </para>
/// <para>
/// The payload is kept as the JSON the browser sent. Reading it through a property is what makes a
/// handler's code readable, and keeping the raw value is what lets a class answer for a field this
/// framework has not modelled yet.
/// </para>
/// </remarks>
public class EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload, or <see langword="null"/> when the event carried none.</param>
    /// <remarks>
    /// Not abstract, because one event's payload is the base class itself: the Python version's
    /// <c>click</c> carries a <c>gr.EventData</c> and nothing more, so a handler that names
    /// <see cref="EventData"/> has to receive one. The classes below are the payloads that say
    /// something extra.
    /// </remarks>
    public EventData(JsonNode? raw) => Raw = raw?.DeepClone();

    /// <summary>What the browser sent, or <see langword="null"/> when the event carried nothing.</summary>
    public JsonNode? Raw { get; }

    /// <summary>A field of the payload, as the CLR value it holds.</summary>
    /// <param name="name">The field's name, as the browser spells it.</param>
    /// <returns>The value, or <see langword="null"/> when the field is absent.</returns>
    protected object? Field(string name) =>
        Raw is JsonObject payload && payload.TryGetPropertyValue(name, out var value)
            ? JsonValues.ToObject(value)
            : null;

    /// <summary>A field of the payload, as the JSON it arrived as.</summary>
    /// <param name="name">The field's name, as the browser spells it.</param>
    /// <returns>The value, or <see langword="null"/> when the field is absent.</returns>
    /// <remarks>
    /// For a field that is itself a structure — a file, a message — which the classes that carry one
    /// hand to the type that already models it rather than flattening it here.
    /// </remarks>
    protected JsonNode? Child(string name) =>
        Raw is JsonObject payload && payload.TryGetPropertyValue(name, out var value) ? value : null;

    /// <summary>A whole-number field of the payload.</summary>
    /// <param name="name">The field's name, as the browser spells it.</param>
    /// <returns>The number, or <see langword="null"/> when the field is absent or not a number.</returns>
    protected int? Number(string name) =>
        Raw is JsonObject payload && payload.TryGetPropertyValue(name, out var value)
            ? JsonValues.Int32(value)
            : null;

    /// <summary>The positions a payload names, whether it named one or a pair of them.</summary>
    /// <param name="name">The field's name, as the browser spells it.</param>
    /// <returns>The positions, in order, and empty when the payload names none.</returns>
    /// <remarks>
    /// A chat edits, retries and undoes by message, and a message can be a question together with
    /// its answer: the Python version writes one index for the first and a pair for the second, so
    /// both shapes have to read as a list.
    /// </remarks>
    protected IReadOnlyList<int> Positions(string name) => Field(name) switch
    {
        object?[] many => [.. many.Select(Whole).OfType<int>()],
        var single => Whole(single) is { } position ? [position] : [],
    };

    /// <summary>A value read as a whole number, or <see langword="null"/> when it is not one.</summary>
    /// <param name="value">The value, as the payload carried it.</param>
    /// <remarks>
    /// A number read out of a request is a <see cref="double"/> — that is the convention
    /// <c>JsonValues</c> states — while one this framework built in memory is whatever it was built
    /// as. Both arrive here, so both are read.
    /// </remarks>
    private static int? Whole(object? value) => value switch
    {
        double number => (int)number,
        int number => number,
        long number => (int)number,
        _ => null,
    };

    /// <summary>
    /// The payload of an event, read as the class the handler asked for.
    /// </summary>
    /// <param name="type">The parameter type the handler declared.</param>
    /// <param name="raw">What the browser sent.</param>
    /// <returns>The payload, or <see langword="null"/> when the type is not one of the classes here.</returns>
    /// <remarks>
    /// A run fills a parameter of one of these types rather than reading an input for it, which is
    /// why the mapping lives here: the runner asks this question, and the answer is a property of
    /// the event-data family rather than of the runner.
    /// </remarks>
    internal static EventData? Of(Type type, JsonNode? raw) => type switch
    {
        _ when type == typeof(EventData) => new EventData(raw),
        _ when type == typeof(SelectData) => new SelectData(raw),
        _ when type == typeof(KeyUpData) => new KeyUpData(raw),
        _ when type == typeof(DeletedFileData) => new DeletedFileData(raw),
        _ when type == typeof(LikeData) => new LikeData(raw),
        _ when type == typeof(RetryData) => new RetryData(raw),
        _ when type == typeof(UndoData) => new UndoData(raw),
        _ when type == typeof(EditData) => new EditData(raw),
        _ when type == typeof(DownloadData) => new DownloadData(raw),
        _ when type == typeof(CopyData) => new CopyData(raw),
        _ => null,
    };

    /// <summary>Whether a parameter type is one of the event-data classes.</summary>
    /// <param name="type">The parameter type.</param>
    /// <returns>Whether a run fills it rather than reading an input for it.</returns>
    internal static bool Is(Type type) => typeof(EventData).IsAssignableFrom(type);
}

/// <summary>
/// What a selection says: which entry was picked, and what it carried.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>SelectData</c>. A chat raises it when the user picks one of a message's
/// options, a dataset when the user picks a sample, and a checkbox when it is ticked or unticked;
/// the value is the entry's own value and the index is where it sat.
/// </para>
/// <para>
/// The three fields the Python version gives it that this does not are a table's <c>row_value</c>
/// and <c>col_value</c> and an index that is a pair rather than a number. Both belong to the data
/// family, which is the batch that will say what a pair means (ticket 17); what a text or a number
/// component raises is a single position and nothing else.
/// </para>
/// </remarks>
public sealed class SelectData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public SelectData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The value of the entry that was picked.</summary>
    public object? Value => Field("value");

    /// <summary>Where the entry sat, or <see langword="null"/> when the event named no position.</summary>
    /// <remarks>
    /// A single position for a list, and a pair for a table or a range — which is why the Python
    /// version types it as anything rather than as a number. A payload that named a pair answers
    /// with its first position here and keeps the pair in <see cref="EventData.Raw"/>; the
    /// components that raise one belong to the data family, and they are the ones that will say
    /// what a pair means (ticket 17).
    /// </remarks>
    public int? Index => Positions("index") is { Count: > 0 } positions ? positions[0] : null;

    /// <summary>
    /// Whether the entry was selected rather than deselected, which is what a checkbox raises.
    /// </summary>
    /// <value><see langword="true"/> for a payload that says nothing, which is the Python version's
    /// default.</value>
    public bool Selected => Field("selected") is bool flag ? flag : true;
}

/// <summary>
/// What a like says: which message was rated, and how.
/// </summary>
/// <remarks>
/// The Python version's <c>LikeData</c>. <see cref="Liked"/> is a boolean for the two feedback
/// options a chat offers by default and the option's own value for any other, which is why it is
/// not typed as a flag.
/// </remarks>
public sealed class LikeData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public LikeData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>Where in the conversation the rated message sits.</summary>
    public int? Index => Number("index");

    /// <summary>The message that was rated, or <see langword="null"/> when the event named none.</summary>
    public object? Value => Field("value");

    /// <summary>How it was rated: a boolean, or the value of the option that was chosen.</summary>
    public object? Liked => Field("liked");
}

/// <summary>
/// What an edit says: which message was edited, and what it now says.
/// </summary>
/// <remarks>
/// The Python version's <c>EditData</c>. An edit of a single message names one position; an edit of
/// a pair — a question and its answer — names both, which is why <see cref="Indices"/> is a list
/// and <see cref="Index"/> is the first of it.
/// </remarks>
public sealed class EditData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public EditData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The positions the edit covers, in order.</summary>
    /// <value>One position, or two when the edit covers a message and its answer.</value>
    public IReadOnlyList<int> Indices => Positions("index");

    /// <summary>The first position the edit covers, or <see langword="null"/> when it names none.</summary>
    public int? Index => Indices.Count > 0 ? Indices[0] : null;

    /// <summary>The text the message said before the edit.</summary>
    /// <value>
    /// The old content, or <see langword="null"/> when the client sent none — which is what a client
    /// that only knows what the message says now sends.
    /// </value>
    /// <remarks>
    /// The Python version reads it as a required field, so its own client always sends one. It is
    /// optional here because a payload without it is still an edit the chat can perform, and
    /// refusing one would be refusing to answer a message over a field nothing reads.
    /// </remarks>
    public object? PreviousValue => Field("previous_value");

    /// <summary>The text the message now says.</summary>
    public object? Value => Field("value");
}

/// <summary>
/// What a key press says: which key, and what the field held afterwards.
/// </summary>
/// <remarks>
/// The Python version's <c>KeyUpData</c>. <see cref="InputValue"/> is what the field is showing
/// rather than the component's own value, and the two differ for a field that commits on Enter — a
/// dropdown the user is typing into — which is exactly why the payload carries it separately.
/// </remarks>
public sealed class KeyUpData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public KeyUpData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The key that was pressed, as the browser names it.</summary>
    public string? Key => Field("key") as string;

    /// <summary>What the field held after the key was pressed.</summary>
    public string? InputValue => Field("input_value") as string;
}

/// <summary>
/// What a deletion says: which file was removed.
/// </summary>
/// <remarks>
/// The Python version's <c>DeletedFileData</c>. The payload is a file rather than a path, so it is
/// handed to <see cref="FileData"/> — the type the protocol already carries files in — instead of
/// being flattened into a string here.
/// </remarks>
public sealed class DeletedFileData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public DeletedFileData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The file that was deleted, or <see langword="null"/> when the payload named none.</summary>
    public FileData? File => FileData.FromJson(Child("file"));
}

/// <summary>
/// What a retry says: which exchange is to be produced again, and what it was.
/// </summary>
/// <remarks>
/// The Python version's <c>RetryData</c>. <see cref="Indices"/> is one position for a message on
/// its own and two for a question with its answer, the same shape an edit uses.
/// </remarks>
public sealed class RetryData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public RetryData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The positions the retry covers, in order.</summary>
    public IReadOnlyList<int> Indices => Positions("index");

    /// <summary>The first position the retry covers, or <see langword="null"/> when it names none.</summary>
    public int? Index => Indices.Count > 0 ? Indices[0] : null;

    /// <summary>The message the retry is about.</summary>
    public object? Value => Field("value");
}

/// <summary>
/// What an undo says: which exchange is to be taken back, and what it was.
/// </summary>
/// <remarks>
/// The Python version's <c>UndoData</c>, and the mirror of <see cref="RetryData"/>: the same two
/// fields, read for the exchange the user wants removed rather than repeated.
/// </remarks>
public sealed class UndoData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public UndoData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The positions the undo covers, in order.</summary>
    public IReadOnlyList<int> Indices => Positions("index");

    /// <summary>The first position the undo covers, or <see langword="null"/> when it names none.</summary>
    public int? Index => Indices.Count > 0 ? Indices[0] : null;

    /// <summary>The message the undo is about.</summary>
    public object? Value => Field("value");
}

/// <summary>
/// What a download says: which file the browser took.
/// </summary>
/// <remarks>
/// The Python version's <c>DownloadData</c>. Read through <see cref="FileData"/> for the reason
/// <see cref="DeletedFileData"/> is: the payload is a file, not a path.
/// </remarks>
public sealed class DownloadData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public DownloadData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The file that was downloaded, or <see langword="null"/> when the payload named none.</summary>
    public FileData? File => FileData.FromJson(Child("file"));
}

/// <summary>
/// What a copy says: which text was taken.
/// </summary>
/// <remarks>
/// The Python version's <c>CopyData</c>, and the simplest of the family — the value the user
/// copied, which is not necessarily what the component holds now.
/// </remarks>
public sealed class CopyData : EventData
{
    /// <summary>Creates a payload from what the browser sent.</summary>
    /// <param name="raw">The payload.</param>
    public CopyData(JsonNode? raw)
        : base(raw)
    {
    }

    /// <summary>The value that was copied.</summary>
    public object? Value => Field("value");
}

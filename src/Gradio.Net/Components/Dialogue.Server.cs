using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The dialogue's own server functions: the two conversions between a transcript and a value that
/// the frontend calls when it copies the dialogue or parses what was typed into it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version declares both with <c>@server</c>, so the config names them in
/// <c>server_fns</c> and the browser reaches them through <c>/gradio_api/component_server</c>.
/// Neither is an event: they are conversions the component offers, not handlers an application
/// binds.
/// </para>
/// <para>
/// Both fall back to the Python version's default formatter and unformatter when the application
/// supplied neither. The default formatter writes a turn as <c>[speaker] text</c>, and the default
/// unformatter reads that shape back — a line without brackets belongs to the first speaker the
/// component was given, or to <c>Unknown</c> when it names none.
/// </para>
/// </remarks>
public sealed partial class Dialogue
{
    /// <inheritdoc />
    protected internal override IReadOnlyList<string> ServerFunctionNames => ["format", "unformat"];

    /// <summary>The separator used when the application did not set one.</summary>
    private string SeparatorOrDefault => Separator ?? "\n";

    /// <summary>Calls one of this component's server functions.</summary>
    /// <param name="request">The function's name, and what the browser sent with it.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The formatted transcript, or the turns parsed from one.</returns>
    /// <exception cref="NotSupportedException">This component declares no such function.</exception>
    protected internal override ValueTask<JsonNode?> InvokeServerFunctionAsync(
        ComponentServerCall request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // A switch expression would hold the task rather than return it, which is what CA2012
        // warns about for a ValueTask: it may only be awaited or returned, never stored.
        if (request.FunctionName == "format")
        {
            return ValueTask.FromResult<JsonNode?>(Format(request.Data));
        }

        if (request.FunctionName == "unformat")
        {
            return ValueTask.FromResult<JsonNode?>(Unformat(request.Data));
        }

        return base.InvokeServerFunctionAsync(request, cancellationToken);
    }

    /// <summary>
    /// Renders a value as the transcript the reader copies.
    /// </summary>
    /// <param name="data">A string, or a list of turns.</param>
    /// <returns>The turns joined by the separator, or the string itself.</returns>
    private JsonValue Format(JsonNode? data)
    {
        if (data is JsonValue scalar && JsonValues.Text(scalar) is { } text)
        {
            return JsonValue.Create(text);
        }

        // A list of turns is what the frontend sends when the reader copies a transcript; anything
        // else carries no turns, which is the empty transcript the Python version answers with.
        DialogueTurn[] turns = data is JsonArray ? TurnsOf(data) : [];

        // The Python version treats an empty transcript as an empty string rather than as an empty list of
        // turns, so that copying an untouched dialogue yields nothing at all.
        if (turns.Length == 0 || (turns.Length == 1 && turns[0].Text.Length == 0))
        {
            return JsonValue.Create(string.Empty);
        }

        return JsonValue.Create(string.Join(SeparatorOrDefault, turns.Select(FormatTurn)));
    }

    /// <summary>
    /// Parses a transcript back into turns.
    /// </summary>
    /// <param name="data">The object the browser sent, carrying the transcript as <c>text</c>.</param>
    /// <returns>The turns, or an empty list when the transcript is blank.</returns>
    private JsonArray Unformat(JsonNode? data)
    {
        var text = data is JsonObject fields ? JsonValues.Text(fields["text"]) : null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return new JsonArray();
        }

        var defaultSpeaker = Speakers is { Count: > 0 } speakers ? speakers[0] : "Unknown";
        var turns = new JsonArray();

        foreach (var raw in text.Split(SeparatorOrDefault))
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                continue;
            }

            var turn = ParseTurn(line, defaultSpeaker);
            if (turn.Speaker.Length == 0 && turn.Text.Length == 0)
            {
                continue;
            }

            turns.Add(turn.ToJson());
        }

        return turns;
    }

    /// <summary>Formats one turn with the application's formatter, or the Python version's
    /// default.</summary> <param name="turn">The turn.</param> <returns>The formatted
    /// line.</returns>
    private string FormatTurn(DialogueTurn turn) =>
        Formatter is { } formatter
            ? formatter.DynamicInvoke(turn.Speaker, turn.Text)?.ToString() ?? string.Empty
            : $"[{turn.Speaker}] {turn.Text}";

    /// <summary>Parses one line with the application's unformatter, or the Python version's
    /// default.</summary> <param name="line">The trimmed line.</param>
    /// <param name="defaultSpeaker">The speaker a line without brackets belongs to.</param>
    /// <returns>The speaker and the text.</returns>
    private DialogueTurn ParseTurn(string line, string defaultSpeaker)
    {
        if (Unformatter is { } unformatter)
        {
            return unformatter.DynamicInvoke(line, defaultSpeaker) switch
            {
                JsonObject fields => DialogueTurn.FromJson(fields),
                IReadOnlyDictionary<string, string> map => new DialogueTurn(
                    map.GetValueOrDefault("speaker") ?? string.Empty,
                    map.GetValueOrDefault("text") ?? string.Empty),
                _ => new DialogueTurn(string.Empty, string.Empty),
            };
        }

        // The Python version's default: `[speaker] text`, and a line without brackets goes to the default
        // speaker unchanged.
        var end = line.IndexOf(']');
        if (line.StartsWith('[') && end >= 0)
        {
            return new DialogueTurn(line[1..end], line[(end + 1)..].Trim());
        }

        return new DialogueTurn(defaultSpeaker, line);
    }
}

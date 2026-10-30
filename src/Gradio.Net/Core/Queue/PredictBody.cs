using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The body of a request that asks for a binding to run: what the client sent to
/// <c>/gradio_api/queue/join</c>.
/// </summary>
/// <remarks>
/// <para>
/// The Python version calls this <c>PredictBody</c>, and the field set is its: a client may name
/// the binding by index or leave that to the endpoint it called, may carry its own event id, and
/// may send the data as a batch. Every field is optional except <see cref="Data"/> because that is
/// what the frontend sends — a call assembled by a person need not fill in a trigger id it does not
/// have.
/// </para>
/// <para>
/// The body is mutable because a run changes it: the queue stamps its own event id on it, and a
/// batched run replaces the data with the batch it collected. The Python version does the same
/// thing to the same object, and the alternative — a second type for "a body a run is using" —
/// would be a second place for the field list to drift.
/// </para>
/// </remarks>
internal sealed class PredictBody
{
    /// <summary>The session the call belongs to, or <see langword="null"/> when the client sent none.</summary>
    internal string? SessionHash { get; set; }

    /// <summary>The event id, when the client already has one.</summary>
    internal string? EventId { get; set; }

    /// <summary>The values the binding's inputs are read from, in order.</summary>
    internal JsonArray Data { get; set; } = [];

    /// <summary>Which binding to run, or <see langword="null"/> when the endpoint named it.</summary>
    internal int? FnIndex { get; set; }

    /// <summary>The block whose event triggered the call, or <see langword="null"/>.</summary>
    internal int? TriggerId { get; set; }

    /// <summary>Whether the caller sent a flat argument list rather than component values.</summary>
    internal bool SimpleFormat { get; set; }

    /// <summary>Whether the data is already a batch rather than one sample.</summary>
    internal bool Batched { get; set; }

    /// <summary>The event's own payload, or <see langword="null"/> when the event carries none.</summary>
    internal JsonNode? EventData { get; set; }

    /// <summary>
    /// Reads a body from the JSON a client sent.
    /// </summary>
    /// <param name="document">The parsed request body.</param>
    /// <returns>The body, with the fields the client left out left at their defaults.</returns>
    /// <remarks>
    /// A field of the wrong shape is ignored rather than rejected, which is the Python version's
    /// behaviour: pydantic coerces what it can and the endpoint's own checks decide what is
    /// missing. Only <c>data</c> is required, because a call with no inputs still has to say so.
    /// </remarks>
    internal static PredictBody Parse(JsonNode? document)
    {
        var body = new PredictBody();

        if (document is not JsonObject request)
        {
            return body;
        }

        body.SessionHash = JsonValues.Text(request["session_hash"]);
        body.EventId = JsonValues.Text(request["event_id"]);
        body.FnIndex = JsonValues.Int32(request["fn_index"]);
        body.TriggerId = JsonValues.Int32(request["trigger_id"]);
        body.SimpleFormat = JsonValues.Flag(request["simple_format"]);
        body.Batched = JsonValues.Flag(request["batched"]);
        body.EventData = request["event_data"]?.DeepClone();

        if (request["data"] is JsonArray data)
        {
            body.Data = (JsonArray)data.DeepClone();
        }

        return body;
    }
}

using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The framing a job-style result stream uses, which is not the queue stream's framing.
/// </summary>
/// <remarks>
/// <para>
/// The Python version serves <c>GET /call/{api_name}/{event_id}</c> from its own
/// <c>process_msg</c> rather than forwarding the queue's messages. The browser stream writes
/// <c>data: {json}</c> with the kind inside the JSON's <c>msg</c>; the job stream writes named
/// server-sent events — <c>event: generating</c>, <c>event: complete</c>, <c>event: error</c> and
/// <c>event: heartbeat</c> — and puts only the outputs in <c>data</c>. That is what the OpenAPI
/// document describes when it says the final event is <c>event: complete</c>, and what a client
/// generated from it waits for.
/// </para>
/// <para>
/// A message the job stream has no event for — an estimate, a start, a progress report, a log line
/// or the queue's own <c>close_stream</c> — is not written at all. The Python version's
/// <c>process_msg</c> answers <c>None</c> for each of them, and <c>queue_data_helper</c> skips a
/// message whose answer is <c>None</c>.
/// </para>
/// </remarks>
internal static class JobStream
{
    /// <summary>
    /// Writes one queue message as a named server-sent event, or nothing when the kind carries no
    /// job-style event.
    /// </summary>
    /// <param name="message">The queue message, in the shape the browser stream reads.</param>
    /// <returns>The event's text, or <see langword="null"/> to skip it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A completed run names <c>complete</c> when it succeeded and <c>error</c> otherwise, and a
    /// generating chunk names <c>generating</c> or <c>error</c> on the same rule — the Python
    /// version's two branches. On success the data is the run's <c>output.data</c>; on failure it
    /// is the whole <c>output</c>, which is the object that says what went wrong.
    /// </remarks>
    internal static string? Format(JsonObject message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var kind = JsonValues.Text(message["msg"]) ?? string.Empty;

        return kind switch
        {
            QueueMessageKinds.ProcessGenerating or QueueMessageKinds.ProcessStreaming =>
                Event(JsonValues.Flag(message["success"]) ? "generating" : "error", Output(message)),
            QueueMessageKinds.ProcessCompleted =>
                Event(JsonValues.Flag(message["success"]) ? "complete" : "error", Output(message)),
            QueueMessageKinds.Heartbeat => Event("heartbeat", null),
            QueueMessageKinds.UnexpectedError =>
                Event("error", JsonValues.Text(message["message"]) is { } text ? JsonValue.Create(text) : null),
            _ => null,
        };
    }

    /// <summary>The data a generated or completed message carries on the job-style stream.</summary>
    /// <param name="message">The queue message.</param>
    /// <returns>The outputs, or the whole output object when the run failed.</returns>
    /// <remarks>
    /// The Python version reads <c>output["data"]</c> for a success and <c>output</c> for a
    /// failure. Both are read with the same key here, so a payload that carries no <c>data</c>
    /// writes <c>null</c> rather than an invented empty array.
    /// </remarks>
    private static JsonNode? Output(JsonObject message)
    {
        var output = message["output"] as JsonObject;

        return JsonValues.Flag(message["success"])
            ? output?["data"]?.DeepClone()
            : output?.DeepClone();
    }

    /// <summary>One named server-sent event.</summary>
    /// <param name="name">The event's name.</param>
    /// <param name="data">Its data, which is JSON, not a pre-serialized string.</param>
    /// <returns>The frame, including the blank line that ends it.</returns>
    private static string Event(string name, JsonNode? data) =>
        $"event: {name}\ndata: {(data is null ? "null" : JsonText.Serialize(data))}\n\n";
}

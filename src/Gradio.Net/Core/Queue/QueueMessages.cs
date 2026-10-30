using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The kinds of message the queue pushes to a client, spelled as the Python version spells them
/// (<c>gradio_client.utils.ServerMessage</c>).
/// </summary>
/// <remarks>
/// The strings are the protocol. The vendored frontend switches on them, so a kind that is
/// spelled differently here is a message it cannot act on — which is why they are constants in
/// one place rather than literals at the call sites that build them.
/// </remarks>
internal static class QueueMessageKinds
{
    /// <summary>Where an event sits in the queue, and how long it is expected to wait.</summary>
    internal const string Estimation = "estimation";

    /// <summary>The handler is about to run.</summary>
    internal const string ProcessStarts = "process_starts";

    /// <summary>A generator produced another chunk.</summary>
    internal const string ProcessGenerating = "process_generating";

    /// <summary>The run is over, successfully or not.</summary>
    internal const string ProcessCompleted = "process_completed";

    /// <summary>How far a long run has got.</summary>
    internal const string Progress = "progress";

    /// <summary>A message with no event behind it, sent so a silent connection stays alive.</summary>
    internal const string Heartbeat = "heartbeat";

    /// <summary>Nothing more is coming on this connection.</summary>
    internal const string CloseStream = "close_stream";

    /// <summary>Something went wrong outside a run — a session that is gone, a stopped server.</summary>
    internal const string UnexpectedError = "unexpected_error";
}

/// <summary>
/// The messages the queue pushes, built with the Python version's field names and field order.
/// </summary>
/// <remarks>
/// <para>
/// Each message is the Python version's pydantic model dumped to JSON: every field is written,
/// including the ones that are null, because that is what <c>model_dump()</c> produces and the
/// comparison is field by field. A field left out here would read as "this framework does not
/// declare it" rather than as "it is null", which is exactly the distinction the comparison exists
/// to keep.
/// </para>
/// <para>
/// The messages are built as JSON trees rather than as a type hierarchy because that is all the
/// queue does with them: it puts them on a channel and writes them out. A hierarchy would add a
/// dispatch on the way out and buy nothing on the way in, since nothing here ever reads one back.
/// </para>
/// </remarks>
internal static class QueueMessages
{
    /// <summary>Where an event sits in the queue, and how long it is expected to wait.</summary>
    /// <param name="eventId">The event the message is about.</param>
    /// <param name="rank">How many events are ahead of it.</param>
    /// <param name="queueSize">How many events the whole queue holds.</param>
    /// <param name="rankEta">Seconds until it is expected to return, or <see langword="null"/> when unknown.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// The estimate is <see langword="null"/> until the handler has run once: the Python version
    /// derives it from the average time that handler has taken, and a handler that has never run
    /// has no average.
    /// </remarks>
    internal static JsonObject Estimation(string eventId, int rank, int queueSize, double? rankEta) => new()
    {
        ["msg"] = QueueMessageKinds.Estimation,
        ["event_id"] = eventId,
        ["rank"] = rank,
        ["queue_size"] = queueSize,
        ["rank_eta"] = rankEta is { } eta ? JsonValue.Create(eta) : null,
    };

    /// <summary>The handler is about to run.</summary>
    /// <param name="eventId">The event the message is about.</param>
    /// <param name="eta">Seconds the run is expected to take, or <see langword="null"/> when unknown.</param>
    /// <returns>The message.</returns>
    internal static JsonObject ProcessStarts(string eventId, double? eta) => new()
    {
        ["msg"] = QueueMessageKinds.ProcessStarts,
        ["event_id"] = eventId,
        ["eta"] = eta is { } value ? JsonValue.Create(value) : null,
    };

    /// <summary>How many events are waiting, with no event behind the message.</summary>
    /// <param name="queueSize">How many events are waiting.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// The status endpoint reports the queue rather than an event in it, so this is the estimation
    /// message with everything that would describe one event left null — which is the Python
    /// version's <c>get_status</c>.
    /// </remarks>
    internal static JsonObject QueueStatus(int queueSize) => new()
    {
        ["msg"] = QueueMessageKinds.Estimation,
        ["event_id"] = null,
        ["rank"] = null,
        ["queue_size"] = queueSize,
        ["rank_eta"] = null,
    };

    /// <summary>A generator produced another chunk.</summary>
    /// <param name="eventId">The event the message is about.</param>
    /// <param name="output">What the run produced so far, with the run's own fields in it.</param>
    /// <param name="success">Whether the chunk was produced without an error.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// The time limit is carried on the message and is always null here: the Python version fills
    /// it in for a streamed component, whose run is kept alive by the client rather than by the
    /// queue, and that is a different connection (ticket 16).
    /// </remarks>
    internal static JsonObject ProcessGenerating(string eventId, JsonObject output, bool success) => new()
    {
        ["msg"] = QueueMessageKinds.ProcessGenerating,
        ["event_id"] = eventId,
        ["output"] = output,
        ["success"] = success,
        ["time_limit"] = null,
    };

    /// <summary>The run is over.</summary>
    /// <param name="eventId">The event the message is about.</param>
    /// <param name="output">What the run produced, or an empty object when it was cancelled.</param>
    /// <param name="success">Whether the run succeeded.</param>
    /// <param name="title">The title an error carries, or <see langword="null"/> when it succeeded.</param>
    /// <param name="usedCache">Whether a cached result was used, or <see langword="null"/> when none was.</param>
    /// <param name="cacheDuration">How long the cached run took, or <see langword="null"/> when no cache was used.</param>
    /// <param name="averageDuration">How long the handler takes on average, or <see langword="null"/> when unknown.</param>
    /// <returns>The message.</returns>
    internal static JsonObject ProcessCompleted(
        string eventId,
        JsonObject output,
        bool success,
        string? title = null,
        string? usedCache = null,
        double? cacheDuration = null,
        double? averageDuration = null) => new()
    {
        ["msg"] = QueueMessageKinds.ProcessCompleted,
        ["event_id"] = eventId,
        ["output"] = output,
        ["success"] = success,
        ["title"] = title,
        ["used_cache"] = usedCache,
        ["cache_duration"] = cacheDuration is { } duration ? JsonValue.Create(duration) : null,
        ["avg_time"] = averageDuration is { } average ? JsonValue.Create(average) : null,
    };

    /// <summary>How far a long run has got.</summary>
    /// <param name="eventId">The event the message is about.</param>
    /// <param name="units">One entry per thing the run reports progress on.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// Progress is the one message the queue coalesces rather than forwards: a run that reports
    /// thousands of times would otherwise flood the connection, so only the most recent report
    /// between two ticks is sent (see <see cref="GradioQueue"/>).
    /// </remarks>
    internal static JsonObject Progress(string eventId, JsonArray units) => new()
    {
        ["msg"] = QueueMessageKinds.Progress,
        ["event_id"] = eventId,
        ["progress_data"] = units,
    };

    /// <summary>A message with no event behind it, sent so a silent connection stays alive.</summary>
    /// <returns>The message.</returns>
    /// <remarks>
    /// A heartbeat belongs to the connection rather than to a run, so it carries no event id —
    /// which is what distinguishes it from a message a run pushed.
    /// </remarks>
    internal static JsonObject Heartbeat() => new()
    {
        ["msg"] = QueueMessageKinds.Heartbeat,
        ["event_id"] = null,
    };

    /// <summary>Nothing more is coming on this connection.</summary>
    /// <returns>The message.</returns>
    /// <remarks>
    /// Like a heartbeat this belongs to the connection: it is what tells the client to stop
    /// reading, and it is sent once every event the session was waiting for has completed.
    /// </remarks>
    internal static JsonObject CloseStream() => new()
    {
        ["msg"] = QueueMessageKinds.CloseStream,
        ["event_id"] = null,
    };

    /// <summary>Something went wrong outside a run.</summary>
    /// <param name="message">What went wrong, as the client is told it.</param>
    /// <param name="sessionNotFound">Whether the session the client named is gone.</param>
    /// <returns>The message.</returns>
    internal static JsonObject UnexpectedError(string message, bool sessionNotFound = false) => new()
    {
        ["msg"] = QueueMessageKinds.UnexpectedError,
        ["event_id"] = null,
        ["message"] = message,
        ["session_not_found"] = sessionNotFound,
        ["success"] = false,
    };
}

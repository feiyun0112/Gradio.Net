using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One run of one endpoint, as it is kept.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>HistoryRecord</c>, field for field: the endpoint that ran, what it was
/// given, what it produced, and when it started. The values are the JSON the protocol carries
/// rather than the handler's own types, because a record outlives the run that produced it and has
/// to be readable by a page that only knows the wire format.
/// </para>
/// <para>
/// <see cref="SchemaVersion"/> is what makes a stored record readable later: a record written by a
/// newer release is refused rather than half-read, because half a record is a run described wrongly.
/// </para>
/// </remarks>
public sealed record RunHistoryRecord
{
    /// <summary>The version of the record format this build writes.</summary>
    /// <remarks>The Python version's <c>SCHEMA_VERSION</c>.</remarks>
    public const int CurrentSchemaVersion = 2;

    /// <summary>The record's own id, which sorts chronologically.</summary>
    public required string RecordId { get; init; }

    /// <summary>The endpoint the run called, as the record files it.</summary>
    public required string Endpoint { get; init; }

    /// <summary>The values the client sent, or <see langword="null"/> when it sent none.</summary>
    public JsonNode? Inputs { get; init; }

    /// <summary>The values the run produced, or <see langword="null"/> when it produced none.</summary>
    public JsonNode? Outputs { get; init; }

    /// <summary>When the run started, in UTC, to the millisecond.</summary>
    public string StartedAt { get; init; } = string.Empty;

    /// <summary>The version of the record format this record was written with.</summary>
    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    /// <summary>The record as the JSON document a store writes.</summary>
    /// <returns>The document.</returns>
    /// <remarks>
    /// The keys and their order are the Python version's, because the document is what a page reads
    /// and what a reader that was written against the Python implementation expects to find.
    /// </remarks>
    public JsonObject ToJson() =>
        new()
        {
            ["record_id"] = RecordId,
            ["endpoint"] = Endpoint,
            ["inputs"] = Inputs?.DeepClone(),
            ["outputs"] = Outputs?.DeepClone(),
            ["started_at"] = StartedAt,
            ["schema_version"] = SchemaVersion,
        };

    /// <summary>A record read back from the document a store wrote.</summary>
    /// <param name="document">The document.</param>
    /// <returns>The record.</returns>
    /// <exception cref="FormatException">
    /// The document is not a record, or was written by a newer release than this one.
    /// </exception>
    public static RunHistoryRecord FromJson(JsonNode? document)
    {
        if (document is not JsonObject entry)
        {
            throw new FormatException("A run history record is a JSON object.");
        }

        var version = entry["schema_version"] is JsonValue versionValue && versionValue.TryGetValue<int>(out var read)
            ? read
            : CurrentSchemaVersion;

        if (version > CurrentSchemaVersion)
        {
            throw new FormatException(
                $"The record was written with schema version {version}, and this build reads up to "
                + $"{CurrentSchemaVersion}.");
        }

        var recordId = Text(entry["record_id"]);
        var endpoint = Text(entry["endpoint"]);

        if (recordId.Length == 0 || endpoint.Length == 0)
        {
            throw new FormatException("A run history record names its id and its endpoint.");
        }

        return new RunHistoryRecord
        {
            RecordId = recordId,
            Endpoint = endpoint,
            Inputs = entry["inputs"]?.DeepClone(),
            Outputs = entry["outputs"]?.DeepClone(),
            StartedAt = Text(entry["started_at"]),
            SchemaVersion = version,
        };
    }

    private static string Text(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<string>(out var text) ? text : string.Empty;
}

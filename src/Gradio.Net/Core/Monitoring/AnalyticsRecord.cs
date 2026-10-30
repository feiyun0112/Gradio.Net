using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What the application remembers about one admitted run: when it arrived, which binding it runs,
/// what became of it, and how long it took (the Python version's <c>queue.event_analytics</c>
/// entry).
/// </summary>
/// <remarks>
/// <para>
/// The record is created at admission and updated twice afterwards — when the run starts and when
/// it ends — which is why the status is mutable and the arrival facts are not. A run that never
/// starts keeps saying <c>queued</c>, exactly as the Python version's does: the record describes
/// what happened to the request, and nothing happened to it.
/// </para>
/// <para>
/// The values are the ones the monitoring panel reads, so their names and their units are the
/// protocol: <c>time</c> is a wall-clock instant in seconds, <c>process_time</c> is a duration in
/// seconds, and <c>function</c> is the binding's API name rather than any name of this framework's.
/// </para>
/// </remarks>
internal sealed class AnalyticsRecord
{
    /// <summary>Creates the record of a run that has just been admitted.</summary>
    /// <param name="id">The run's id.</param>
    /// <param name="function">The binding's API name.</param>
    /// <param name="sessionHash">The session that asked, or <see langword="null"/> when none did.</param>
    /// <param name="time">When the run was admitted, as a wall-clock instant in seconds.</param>
    internal AnalyticsRecord(string id, string function, string? sessionHash, double time)
    {
        Id = id;
        Function = function;
        SessionHash = sessionHash;
        Time = time;
    }

    /// <summary>The run's id, which is what the record is filed under.</summary>
    /// <remarks>Not part of the document: the panel groups by binding and never asks for one run.</remarks>
    internal string Id { get; }

    /// <summary>The binding's API name.</summary>
    internal string Function { get; }

    /// <summary>The session that asked, or <see langword="null"/>.</summary>
    internal string? SessionHash { get; }

    /// <summary>When the run was admitted, as a wall-clock instant in seconds.</summary>
    internal double Time { get; }

    /// <summary>Where the run has got to: <c>queued</c>, <c>processing</c>, <c>success</c>,
    /// <c>failed</c> or <c>cancelled</c>.</summary>
    internal string Status { get; set; } = AnalyticsStatus.Queued;

    /// <summary>How long the run took, in seconds, or <see langword="null"/> while it has not ended.</summary>
    internal double? ProcessTime { get; set; }

    /// <summary>The record as the panel reads it.</summary>
    /// <returns>The document.</returns>
    internal JsonObject ToJson() => new()
    {
        ["time"] = Time,
        ["status"] = Status,
        ["process_time"] = ProcessTime,
        ["function"] = Function,
        ["session_hash"] = SessionHash,
    };
}

/// <summary>The states a record moves through, spelled the way the Python version spells
/// them.</summary>
/// <remarks>
/// They are the panel's vocabulary rather than an enum here: the strings go over the wire, and a
/// value the frontend does not know is worse than a constant this framework happens to share.
/// </remarks>
internal static class AnalyticsStatus
{
    /// <summary>Admitted, waiting for a slot.</summary>
    internal const string Queued = "queued";

    /// <summary>A slot was claimed and the handler is running.</summary>
    internal const string Processing = "processing";

    /// <summary>The run produced an answer.</summary>
    internal const string Success = "success";

    /// <summary>The run raised.</summary>
    internal const string Failed = "failed";

    /// <summary>The run was cancelled or its client went away.</summary>
    internal const string Cancelled = "cancelled";
}

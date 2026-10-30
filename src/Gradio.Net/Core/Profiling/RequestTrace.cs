using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What one run cost, phase by phase (the Python version's <c>profiling.RequestTrace</c>).
/// </summary>
/// <remarks>
/// <para>
/// A trace is created when a run claims a slot and handed to the collector when the run is over. The
/// phases are the parts of a run that can be slow for different reasons — waiting for a slot,
/// turning the client's values into the handler's arguments, the handler itself, turning its answer
/// back into values, and encoding a generator's next chunk as an edit — so that "this endpoint is
/// slow" can be answered with which part is slow.
/// </para>
/// <para>
/// Durations are milliseconds and they accumulate: a streamed run takes its handler several times,
/// and what an operator wants to know is what the whole run spent in it. <c>total</c> counts the
/// iterations it was added for, which is how the trace says how many chunks the run produced.
/// </para>
/// <para>
/// The fields this framework does not measure — the media sub-phases and the cache moves, which the
/// Python version instruments inside its audio, image and video components and around its file
/// caches — are carried as zero rather than left out: the document's shape is the contract, and a
/// missing key reads as "this implementation does not know about that phase" when the truth is that
/// nothing was spent in it here. What is instrumented is the six phases a run is made of, which are
/// the ones that answer "which part of this endpoint is slow".
/// </para>
/// </remarks>
internal sealed class RequestTrace
{
    /// <summary>Creates a trace for a run that is about to start.</summary>
    /// <param name="eventId">The run's id.</param>
    /// <param name="function">The binding's API name.</param>
    /// <param name="sessionHash">The session that asked.</param>
    /// <param name="timestamp">When the run was admitted, as a Unix instant in seconds.</param>
    internal RequestTrace(string? eventId, string? function, string? sessionHash, double timestamp)
    {
        EventId = eventId;
        Function = function;
        SessionHash = sessionHash;
        Timestamp = timestamp;
    }

    /// <summary>The run's id.</summary>
    internal string? EventId { get; }

    /// <summary>The binding's API name.</summary>
    internal string? Function { get; }

    /// <summary>The session that asked.</summary>
    internal string? SessionHash { get; }

    /// <summary>When the run was admitted, as a Unix instant in seconds.</summary>
    internal double Timestamp { get; }

    /// <summary>How long the run waited for a slot.</summary>
    internal double QueueWaitMs { get; set; }

    /// <summary>How long turning the client's values into arguments took.</summary>
    internal double PreprocessMs { get; set; }

    /// <summary>How long the handler itself took.</summary>
    internal double FunctionCallMs { get; set; }

    /// <summary>How long turning the handler's answer into values took.</summary>
    internal double PostprocessMs { get; set; }

    /// <summary>How long encoding a generator's next chunk as an edit took.</summary>
    internal double StreamingDiffMs { get; set; }

    /// <summary>How long the run took in all, summed over its chunks.</summary>
    internal double TotalMs { get; set; }

    /// <summary>How many chunks the run produced.</summary>
    internal int Iterations { get; set; }

    /// <summary>How long receiving an upload took.</summary>
    /// <remarks>
    /// Kept for the summary, which reports uploads apart from runs — and deliberately absent from
    /// <see cref="ToJson"/>, as it is from the Python version's <c>to_dict</c>: a trace that
    /// reaches the traces route is a run, and the upload's own timing is only ever read in
    /// aggregate.
    /// </remarks>
    internal double UploadMs { get; set; }

    /// <summary>Adds one measurement to a phase.</summary>
    /// <param name="phase">The phase, named as <see cref="ProfilingPhases"/> spells it.</param>
    /// <param name="milliseconds">How long it took.</param>
    /// <remarks>
    /// Adding rather than assigning is what makes a streamed run's numbers mean "in total": the
    /// handler is called once per chunk, and each call is a measurement of the same phase.
    /// </remarks>
    internal void Add(string phase, double milliseconds)
    {
        switch (phase)
        {
            case ProfilingPhases.QueueWait:
                QueueWaitMs += milliseconds;
                break;
            case ProfilingPhases.Preprocess:
                PreprocessMs += milliseconds;
                break;
            case ProfilingPhases.FunctionCall:
                FunctionCallMs += milliseconds;
                break;
            case ProfilingPhases.Postprocess:
                PostprocessMs += milliseconds;
                break;
            case ProfilingPhases.StreamingDiff:
                StreamingDiffMs += milliseconds;
                break;
            case ProfilingPhases.Total:
                TotalMs += milliseconds;
                Iterations++;
                break;
            default:
                // A phase this trace has no field for is one nothing measured; the Python version's
                // set_phase ignores it for the same reason.
                break;
        }
    }

    /// <summary>The trace as the traces route reports it.</summary>
    /// <returns>The document, with every phase the route knows about.</returns>
    /// <remarks>
    /// The keys and their order are the Python version's <c>to_dict</c>, including the media
    /// sub-phases that are always zero here: a client reading the document by position, or a
    /// comparison of two documents, sees the same shape on both sides.
    /// </remarks>
    internal JsonObject ToJson() => new()
    {
        ["event_id"] = EventId,
        ["fn_name"] = Function,
        ["session_hash"] = SessionHash,
        ["timestamp"] = Timestamp,
        ["queue_wait_ms"] = QueueWaitMs,
        ["preprocess_ms"] = PreprocessMs,
        ["fn_call_ms"] = FunctionCallMs,
        ["postprocess_ms"] = PostprocessMs,
        ["streaming_diff_ms"] = StreamingDiffMs,
        ["total_ms"] = TotalMs,
        ["n_iterations"] = Iterations,
        ["preprocess_move_to_cache_ms"] = 0.0,
        ["preprocess_format_image_ms"] = 0.0,
        ["postprocess_save_img_array_to_cache_ms"] = 0.0,
        ["preprocess_audio_from_file_ms"] = 0.0,
        ["postprocess_save_audio_to_cache_ms"] = 0.0,
        ["preprocess_video_ms"] = 0.0,
        ["postprocess_video_convert_video_to_playable_mp4_ms"] = 0.0,
        ["postprocess_update_state_in_config_ms"] = 0.0,
        ["postprocess_move_to_cache_ms"] = 0.0,
        ["postprocess_video_ms"] = 0.0,
        ["postprocess_save_pil_to_cache_ms"] = 0.0,
        ["postprocess_save_bytes_to_cache_ms"] = 0.0,
        ["save_file_to_cache_ms"] = 0.0,
    };
}

/// <summary>The phases a trace measures, named as the trace document names them.</summary>
internal static class ProfilingPhases
{
    /// <summary>Waiting for a slot.</summary>
    internal const string QueueWait = "queue_wait";

    /// <summary>Turning the client's values into the handler's arguments.</summary>
    internal const string Preprocess = "preprocess";

    /// <summary>The handler itself.</summary>
    internal const string FunctionCall = "fn_call";

    /// <summary>Turning the handler's answer into the values the outputs carry.</summary>
    internal const string Postprocess = "postprocess";

    /// <summary>Encoding a generator's next chunk as an edit of the previous one.</summary>
    internal const string StreamingDiff = "streaming_diff";

    /// <summary>The run in all.</summary>
    internal const string Total = "total";

    /// <summary>The phases the summary reports, in the order it reports them.</summary>
    internal static readonly IReadOnlyList<string> Summarised =
    [
        QueueWait,
        Preprocess,
        FunctionCall,
        Postprocess,
        StreamingDiff,
        Total,
    ];
}

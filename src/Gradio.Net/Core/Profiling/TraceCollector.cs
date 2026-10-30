using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The traces of the runs this application has served, and the summary of them an operator reads
/// (the Python version's <c>profiling.TraceCollector</c>).
/// </summary>
/// <remarks>
/// <para>
/// The set is bounded, because it is a debugging aid rather than a record: an application that is
/// profiled for a week must not grow a list for every run it ever served. The bound is the Python
/// version's hundred thousand, and the oldest trace is dropped when it is reached.
/// </para>
/// <para>
/// Two views of the same traces. The list answers "what did the last few runs do", one document per
/// run; the summary answers "what does a run of this application cost", with the percentiles of
/// each phase. Uploads are reported apart from runs in the summary because they are not runs: the
/// Python version files them under a name of their own, and a run's phases would be zeros for them.
/// </para>
/// </remarks>
internal sealed class TraceCollector
{
    /// <summary>How many traces are kept.</summary>
    private const int Capacity = 100_000;

    /// <summary>The function name an upload is filed under, as the Python version spells
    /// it.</summary>
    /// <remarks>
    /// Internal rather than private because the upload endpoint is what files one: the name is the
    /// only thing that tells the summary which traces are runs and which are uploads, so both sides
    /// have to spell it the same way.
    /// </remarks>
    internal const string UploadFunction = "gradio_file_upload";

    private readonly object _gate = new();
    private readonly LinkedList<RequestTrace> _traces = new();

    /// <summary>Creates an empty collector.</summary>
    /// <remarks>
    /// Public so that the container can build it; the type stays internal to the assembly, as the
    /// queue's does.
    /// </remarks>
    public TraceCollector()
    {
    }

    /// <summary>How many traces are being kept.</summary>
    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _traces.Count;
            }
        }
    }

    /// <summary>Adds a finished run's trace.</summary>
    /// <param name="trace">The trace.</param>
    internal void Add(RequestTrace trace)
    {
        lock (_gate)
        {
            _traces.AddLast(trace);

            while (_traces.Count > Capacity)
            {
                _traces.RemoveFirst();
            }
        }
    }

    /// <summary>The traces, newest last.</summary>
    /// <param name="lastN">
    /// How many of the newest to report, or <see langword="null"/> for all of them. Zero also means
    /// all of them, and a negative number skips that many of the oldest — both are what Python's
    /// <c>traces[-last_n:]</c> does, since <c>[-0:]</c> is <c>[0:]</c>.
    /// </param>
    /// <returns>One document per trace.</returns>
    internal JsonArray All(int? lastN)
    {
        lock (_gate)
        {
            var traces = _traces.ToList();

            if (lastN is { } count)
            {
                // Zero falls through to the skip: skipping nothing is returning everything, which is
                // what the slice the Python version asks for does with it.
                traces = count > 0
                    ? traces.Skip(Math.Max(0, traces.Count - count)).ToList()
                    : traces.Skip(Math.Min(traces.Count, -count)).ToList();
            }

            var documents = new JsonArray();

            foreach (var trace in traces)
            {
                documents.Add(trace.ToJson());
            }

            return documents;
        }
    }

    /// <summary>The percentile summary of every phase.</summary>
    /// <returns>The document.</returns>
    /// <remarks>
    /// <para>
    /// <c>count</c> is the number of runs the percentiles are over, which is every trace that is not
    /// an upload. A phase nothing has been measured in reports zeros rather than being left out, so
    /// that a panel drawing the phases does not have to decide what a missing one means.
    /// </para>
    /// <para>
    /// A collector that has never been given a trace reports no phases at all rather than six sets
    /// of zeros, which is the Python version's answer for an empty one: the difference says
    /// "nothing has run" as against "runs cost nothing", and a client drawing the second would be
    /// drawing a lie.
    /// </para>
    /// </remarks>
    internal JsonObject Summary()
    {
        lock (_gate)
        {
            if (_traces.Count == 0)
            {
                return new JsonObject
                {
                    ["count"] = 0,
                    ["phases"] = new JsonObject(),
                };
            }

            var runs = _traces.Where(trace => trace.Function != UploadFunction).ToList();
            var uploads = _traces.Where(trace => trace.Function == UploadFunction).ToList();

            var phases = new JsonObject();

            foreach (var phase in ProfilingPhases.Summarised)
            {
                phases[phase] = Statistics(runs.Select(trace => ValueOf(trace, phase)).ToList());
            }

            var summary = new JsonObject
            {
                ["count"] = runs.Count,
                ["phases"] = phases,
            };

            if (uploads.Count > 0)
            {
                var upload = new JsonObject { ["count"] = uploads.Count };

                foreach (var (key, value) in Statistics(uploads.Select(trace => trace.UploadMs).ToList()))
                {
                    upload[key] = value?.DeepClone();
                }

                summary["upload"] = upload;
            }

            return summary;
        }
    }

    /// <summary>Forgets every trace.</summary>
    internal void Clear()
    {
        lock (_gate)
        {
            _traces.Clear();
        }
    }

    /// <summary>One phase's measurement of one trace.</summary>
    /// <param name="trace">The trace.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>The duration in milliseconds.</returns>
    private static double ValueOf(RequestTrace trace, string phase) => phase switch
    {
        ProfilingPhases.QueueWait => trace.QueueWaitMs,
        ProfilingPhases.Preprocess => trace.PreprocessMs,
        ProfilingPhases.FunctionCall => trace.FunctionCallMs,
        ProfilingPhases.Postprocess => trace.PostprocessMs,
        ProfilingPhases.StreamingDiff => trace.StreamingDiffMs,
        ProfilingPhases.Total => trace.TotalMs,
        _ => 0.0,
    };

    /// <summary>The percentiles of one phase.</summary>
    /// <param name="values">The measurements.</param>
    /// <returns>The document, with the keys a panel reads.</returns>
    /// <remarks>
    /// The mean and the extremes are here as well as the percentiles because they answer a different
    /// question: a p99 that is much larger than the mean says a few runs are slow, and a mean that is
    /// much larger than the p50 says the slow ones are not that few.
    /// </remarks>
    private static JsonObject Statistics(List<double> values)
    {
        if (values.Count == 0)
        {
            return new JsonObject
            {
                ["p50"] = 0.0,
                ["p90"] = 0.0,
                ["p95"] = 0.0,
                ["p99"] = 0.0,
                ["mean"] = 0.0,
                ["min"] = 0.0,
                ["max"] = 0.0,
            };
        }

        values.Sort();

        return new JsonObject
        {
            ["p50"] = Percentiles.At(values, 50),
            ["p90"] = Percentiles.At(values, 90),
            ["p95"] = Percentiles.At(values, 95),
            ["p99"] = Percentiles.At(values, 99),
            ["mean"] = values.Average(),
            ["min"] = values[0],
            ["max"] = values[^1],
        };
    }
}

using System.Diagnostics.Metrics;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What an exporter would see, recorded the way the metrics API delivers it: the instruments that
/// were published, the measurements a histogram took, and the numbers an observable instrument
/// reports when it is scraped.
/// </summary>
/// <remarks>
/// <para>
/// A listener is process-wide, so a measurement made by another test's application reaches these
/// callbacks too — the meter's name is the same and nothing about the instrument says which
/// application it belongs to. Everything here is therefore kept by instrument name, and a
/// measurement is only read back through the tag that says which binding it was, or through a
/// sample taken at a moment the caller controls.
/// </para>
/// <para>
/// Sampling is a call of its own rather than something the listener does continuously, because
/// that is what a scrape is: an observable instrument is asked once, and the answer is what the
/// exporter would have published.
/// </para>
/// </remarks>
internal sealed class CapturedMetrics : IDisposable
{
    /// <summary>The meter the framework publishes under.</summary>
    internal const string MeterName = "Gradio.Net";

    /// <summary>How many runs are waiting for a slot.</summary>
    internal const string QueueDepth = "gradio.queue.depth";

    /// <summary>How many runs are holding a slot.</summary>
    internal const string ActiveRuns = "gradio.queue.active";

    /// <summary>How long a request took.</summary>
    internal const string RequestDuration = "gradio.request.duration";

    private readonly MeterListener _listener;
    private readonly object _gate = new();
    private readonly List<string> _published = [];
    private readonly List<Recorded> _durations = [];
    private readonly Dictionary<string, List<long>> _samples = new(StringComparer.Ordinal);

    internal CapturedMetrics()
    {
        _listener = new MeterListener
        {
            InstrumentPublished = (instrument, enabled) =>
            {
                if (instrument.Meter.Name != MeterName)
                {
                    return;
                }

                lock (_gate)
                {
                    _published.Add(instrument.Name);
                }

                enabled.EnableMeasurementEvents(instrument);
            },
        };

        _listener.SetMeasurementEventCallback<double>((instrument, value, measurements, _) =>
        {
            if (instrument.Name != RequestDuration)
            {
                return;
            }

            lock (_gate)
            {
                _durations.Add(new Recorded(value, Tags(measurements)));
            }
        });

        // Both observable gauges report an integer, so the instrument's name is what tells them
        // apart: a depth and a count of runs in flight are different questions. Every answer is
        // kept rather than the last one, because a scrape asks every live application's instrument
        // and the listener cannot say which application an answer came from.
        _listener.SetMeasurementEventCallback<int>((instrument, value, _, _) =>
        {
            lock (_gate)
            {
                if (!_samples.TryGetValue(instrument.Name, out var answers))
                {
                    answers = [];
                    _samples[instrument.Name] = answers;
                }

                answers.Add(value);
            }
        });

        _listener.Start();
    }

    /// <summary>The instruments that were published under the framework's meter.</summary>
    internal IReadOnlyCollection<string> Published
    {
        get
        {
            lock (_gate)
            {
                return _published.ToArray();
            }
        }
    }

    /// <summary>Asks every observable instrument for its number, as a scrape does.</summary>
    internal void Sample() => _listener.RecordObservableInstruments();

    /// <summary>Every number an observable instrument has reported, over every sample taken.</summary>
    /// <param name="instrument">The instrument's published name.</param>
    /// <returns>The numbers, oldest first; empty when it was never sampled.</returns>
    /// <remarks>
    /// A list rather than the last number, because a listener is process-wide: a scrape asks the
    /// instrument of every application in the process, and only one of the answers belongs to the
    /// application a test is watching. What a test can say is that one of them is the number it
    /// expected at the moment it asked.
    /// </remarks>
    internal IReadOnlyList<long> SamplesOf(string instrument)
    {
        lock (_gate)
        {
            return _samples.TryGetValue(instrument, out var answers) ? answers.ToArray() : [];
        }
    }

    /// <summary>The latencies recorded for one binding.</summary>
    /// <param name="function">The binding's name, as the <c>gradio.function</c> tag carries it.</param>
    /// <returns>One entry per measurement, in the order they were taken.</returns>
    internal IReadOnlyList<Recorded> DurationsOf(string function)
    {
        lock (_gate)
        {
            return _durations
                .Where(measurement => measurement.Tags.GetValueOrDefault("gradio.function") as string == function)
                .ToArray();
        }
    }

    /// <inheritdoc />
    public void Dispose() => _listener.Dispose();

    /// <summary>The measurements an instrument was taken with, as a dictionary.</summary>
    /// <param name="measurements">What the callback was handed.</param>
    /// <returns>The tags.</returns>
    private static Dictionary<string, object?> Tags(
        ReadOnlySpan<KeyValuePair<string, object?>> measurements) =>
        measurements.ToArray().ToDictionary(pair => pair.Key, pair => pair.Value);

    /// <summary>One measurement of a histogram.</summary>
    /// <param name="Value">What was recorded.</param>
    /// <param name="Tags">The measurements it was recorded with.</param>
    internal sealed record Recorded(double Value, IReadOnlyDictionary<string, object?> Tags);
}

using System.Diagnostics.Metrics;
namespace Gradio.Net;
/// <summary>
/// What an application reports about itself through the .NET metrics API, so that an operator can
/// alert on saturation without scraping a page.
/// </summary>
/// <remarks>
/// <para>
/// The Python version has no such thing: its monitoring is a panel a human opens. The two numbers
/// an orchestrator needs in order to alert are not on that panel either — how deep the queue is,
/// and how long a request takes — so they are published as instruments under one meter
/// (<c>Gradio.Net</c>), which is the abstraction the operator's exporter already reads. Nothing
/// here adds a route or a response: this is an addition to the framework, not to the protocol, and
/// the route table is therefore unaffected.
/// </para>
/// <para>
/// The queue's numbers are read when the exporter asks for them rather than pushed on every change:
/// a depth that is sampled is what an alert rule wants, and a depth that is pushed would cost a
/// measurement per admission whether anybody is listening or not.
/// </para>
/// </remarks>
internal sealed class GradioMetrics : IDisposable
{
    /// <summary>The meter every instrument is published under.</summary>
    internal const string MeterName = "Gradio.Net";

    /// <summary>How many runs are waiting for a slot.</summary>
    internal const string QueueDepthName = "gradio.queue.depth";

    /// <summary>How many runs are holding a slot.</summary>
    internal const string ActiveRunsName = "gradio.queue.active";

    /// <summary>How long a request took, from admission to its last message.</summary>
    internal const string RequestDurationName = "gradio.request.duration";

    private readonly Meter _meter;
    private readonly Histogram<double> _requestDuration;

    private bool _disposed;

    /// <summary>Creates the instruments.</summary>
    /// <param name="status">Where the queue's own numbers are read from.</param>
    /// <exception cref="ArgumentNullException"><paramref name="status"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Public so that the container can build it; the type stays internal to the assembly, as the
    /// queue's does.
    /// </remarks>
    public GradioMetrics(GradioQueueStatus status)
    {
        ArgumentNullException.ThrowIfNull(status);

        _meter = new Meter(MeterName, GradioVersion.PackageVersion);

        _requestDuration = _meter.CreateHistogram<double>(
            RequestDurationName,
            unit: "s",
            description: "How long a queued request took, from admission to its last message.");

        _meter.CreateObservableGauge(
            QueueDepthName,
            () => status.Depth,
            unit: "{event}",
            description: "How many runs are waiting for a slot.");

        _meter.CreateObservableGauge(
            ActiveRunsName,
            () => status.Active,
            unit: "{event}",
            description: "How many runs are holding a slot.");
    }

    /// <summary>Records one request's latency.</summary>
    /// <param name="function">The binding's API name, as the tag <c>gradio.function</c>.</param>
    /// <param name="status">How the run ended, as the tag <c>gradio.status</c>.</param>
    /// <param name="seconds">How long it took, from admission to its last message.</param>
    internal void RecordRequest(string function, string status, double seconds)
    {
        if (_disposed)
        {
            return;
        }

        _requestDuration.Record(
            seconds,
            new KeyValuePair<string, object?>("gradio.function", function),
            new KeyValuePair<string, object?>("gradio.status", status));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _meter.Dispose();
    }
}

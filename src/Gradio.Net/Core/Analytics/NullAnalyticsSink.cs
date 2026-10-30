namespace Gradio.Net;
/// <summary>
/// The sink an application gets when it registers none: it takes the event and does nothing with
/// it.
/// </summary>
/// <remarks>
/// <para>
/// This is the whole of what "on by default means nothing leaves" rests on. The payloads are still
/// built — a switch that also changed what the framework computes would make the events
/// unobservable in exactly the case a consumer needs them — and they stop here.
/// </para>
/// <para>
/// It is deliberately the only implementation in the assembly: a framework that shipped a second
/// one would have shipped a transport, and the point of the seam is that the other side of it does
/// not exist until an application writes it.
/// </para>
/// </remarks>
internal sealed class NullAnalyticsSink : IAnalyticsSink
{
    /// <inheritdoc />
    /// <remarks>Nothing to do, and nothing to remember: the payload is the caller's.</remarks>
    public void Send(AnalyticsEvent analyticsEvent)
    {
        // Deliberately empty. There is no transport to hand the event to.
    }
}

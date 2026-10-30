namespace Gradio.Net;
/// <summary>
/// Where an application's analytics events go: the sink the framework hands them to, and the one
/// thing an application replaces to receive them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version posts five events — a version check, an application being constructed, an
/// application being launched, the editor being turned on, and an error — to <c>api.gradio.app</c>
/// and to a Hugging Face telemetry endpoint, from a thread it starts for the purpose. This
/// framework makes no outbound request of its own, so the transport is not implemented: the payload
/// is built at the moment the event happens and is handed to whatever is registered here instead.
/// </para>
/// <para>
/// The default is the framework's own no-op — an application that registers nothing keeps building
/// the payloads and throws them away, which is what "the difference the switch makes is whether
/// anybody catches the payload, not whether anything is sent" means. No other implementation is
/// built in: one that shipped a vendor's client would be the dependency this abstraction exists to
/// remove, and what the ticket asks for is parity of the events and their payloads, not of the
/// transport.
/// </para>
/// <para>
/// To receive them, register an implementation before the host is built: before <c>AddGradio</c>,
/// or through the service configuration the standalone host takes, both of which run before the
/// framework's own registrations. (Registering it afterwards works as well — the container answers
/// with the last registration — but registering first is the order that cannot surprise.) What
/// arrives is <see cref="AnalyticsEvent.Topic"/> naming the endpoint the Python version would have
/// posted to and <see cref="AnalyticsEvent.Payload"/> holding the Python version's own body, so an
/// implementation that wants to forward the events has nothing to translate.
/// </para>
/// <para>
/// <see cref="Send"/> is called on the thread that produced the event, once, and its outcome is
/// ignored — which is the Python version's arrangement too: a run does not wait for its telemetry,
/// and an implementation that talks to a network should queue the event and return. An
/// implementation that throws does not break the application, because a reporting sink is not worth
/// a failed request.
/// </para>
/// </remarks>
public interface IAnalyticsSink
{
    /// <summary>Receives one event.</summary> <param name="analyticsEvent">The topic and the
    /// payload, as the Python version would have sent them.</param>
    void Send(AnalyticsEvent analyticsEvent);
}

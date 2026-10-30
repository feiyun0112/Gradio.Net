using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One analytics event: where the Python version would have posted it, and what it would have
/// posted.
/// </summary>
/// <remarks>
/// <para>
/// The two halves travel together because they are one message. The Python version's own call sites
/// pass the same pair to its reporting function — a topic string and a dictionary — and its
/// telemetry client reads nothing else, so this is the whole of what a replaceable sink is handed.
/// </para>
/// <para>
/// The payload is a document rather than a typed record because the Python version's is: the five
/// events carry different keys, one of them carries a list of every block on the page, and a type
/// per event would be five types to keep in step with a Python dictionary that is already the
/// contract. What is promised is that the keys and the values are the Python version's, field for
/// field.
/// </para>
/// </remarks>
public sealed class AnalyticsEvent
{
    /// <summary>Creates an event.</summary> <param name="topic">Where the Python version would have
    /// posted it; see <see cref="AnalyticsTopics"/>.</param> <param name="payload">What the Python
    /// version would have posted.</param>
    /// <exception cref="ArgumentException"><paramref name="topic"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public AnalyticsEvent(string topic, JsonObject payload)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic);
        ArgumentNullException.ThrowIfNull(payload);

        Topic = topic;
        Payload = payload;
    }

    /// <summary>Where the Python version would have posted this event.</summary>
    /// <remarks>
    /// It is the Python version's string rather than an address this framework uses: nothing here
    /// resolves it, and a sink that forwards the events is meant to send them wherever its operator
    /// says.
    /// </remarks>
    public string Topic { get; }

    /// <summary>What the Python version would have posted, with the Python version's keys and
    /// values.</summary>
    /// <remarks>
    /// The document belongs to the event, and the framework does not read it again: an
    /// implementation may keep it or take it apart.
    /// </remarks>
    public JsonObject Payload { get; }
}

/// <summary>
/// The five topics the Python version posts to, spelled as it spells them.
/// </summary>
/// <remarks>
/// <para>
/// They are here rather than in a sink so that an implementation can recognise an event without
/// hard-coding a URL, and so that the addresses appear exactly once in the tree — which is also
/// what makes "nothing here contacts them" something a reader can check: the strings are constants
/// and there is no client that uses them.
/// </para>
/// <para>
/// Two of them are not the same kind of request in the Python version: the version check is a
/// <c>GET</c> whose answer names the newest release, and the other four are <c>POST</c>s. The topic
/// is recorded as the address either way, because that is what identifies the event.
/// </para>
/// </remarks>
public static class AnalyticsTopics
{
    /// <summary>The version check: the Python version asks which release is newest.</summary>
    /// <remarks>
    /// The Python version's <c>PKG_VERSION_URL</c>, read at construction when analytics is on. Its
    /// answer is the only thing the check produces, which is why the event carries an empty
    /// payload.
    /// </remarks>
    public const string VersionCheck = "https://api.gradio.app/pkg-version";

    /// <summary>The application was constructed.</summary>
    public const string Initiated = "https://api.gradio.app/gradio-initiated-analytics/";

    /// <summary>The application was launched.</summary>
    public const string Launched = "https://api.gradio.app/gradio-launched-telemetry/";

    /// <summary>The Vibe editor was turned on.</summary> <remarks>The Python version posts this
    /// from its command line, which is why the topic is not a URL.</remarks>
    public const string Vibe = "gradio/vibe";

    /// <summary>The framework caught an error.</summary>
    public const string Error = "https://api.gradio.app/gradio-error-analytics/";
}

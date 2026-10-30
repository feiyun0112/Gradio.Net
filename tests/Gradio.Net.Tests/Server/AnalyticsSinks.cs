using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The sink a test registers: it keeps every event, in the order it was handed one.
/// </summary>
/// <remarks>
/// It is the whole of what an application has to write to receive the analytics events, so the tests
/// that assert on payloads are asserting on what a user's own implementation would see.
/// </remarks>
internal sealed class RecordingAnalyticsSink : IAnalyticsSink
{
    private readonly object _gate = new();
    private readonly List<AnalyticsEvent> _events = [];

    /// <summary>What has been handed over so far, oldest first.</summary>
    internal IReadOnlyList<AnalyticsEvent> Events
    {
        get
        {
            lock (_gate)
            {
                return [.. _events];
            }
        }
    }

    /// <summary>The events with a given topic.</summary>
    /// <param name="topic">The topic to keep; see <see cref="AnalyticsTopics"/>.</param>
    /// <returns>Those of them, oldest first.</returns>
    internal IReadOnlyList<AnalyticsEvent> WithTopic(string topic) =>
        [.. Events.Where(analyticsEvent => string.Equals(analyticsEvent.Topic, topic, StringComparison.Ordinal))];

    /// <inheritdoc />
    public void Send(AnalyticsEvent analyticsEvent)
    {
        lock (_gate)
        {
            _events.Add(analyticsEvent);
        }
    }

    /// <summary>The payload of the only event with a given topic.</summary>
    /// <param name="topic">The topic to look for.</param>
    /// <returns>Its payload.</returns>
    internal JsonObject Only(string topic) => Assert.Single(WithTopic(topic)).Payload;
}

/// <summary>
/// A sink that fails, so that a test can see what the framework does about one.
/// </summary>
internal sealed class ThrowingAnalyticsSink : IAnalyticsSink
{
    /// <summary>How many events it was handed, whether or not it kept them.</summary>
    internal int Count { get; private set; }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Always.</exception>
    public void Send(AnalyticsEvent analyticsEvent)
    {
        Count++;

        throw new InvalidOperationException("The sink is down.");
    }
}

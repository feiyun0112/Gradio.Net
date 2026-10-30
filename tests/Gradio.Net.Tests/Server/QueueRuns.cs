using System.Text.Json.Nodes;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Runs a binding once over the queue, and hands back what it said.
/// </summary>
/// <remarks>
/// Several tests need the same four steps — join, open the stream, wait for the completion that
/// carries the run's own event id, read the outputs — and each of them is about what the handler
/// did rather than about the queue. Keeping the steps in one place means a change to how a run is
/// observed is a change to one file.
/// </remarks>
internal static class QueueRuns
{
    /// <summary>Runs once and returns the text the first output received.</summary>
    internal static async Task<string> RunAsync(
        QueueClient queue,
        int fnIndex,
        JsonArray data,
        string sessionHash,
        CancellationToken cancellationToken) =>
        (await CompletedDataAsync(queue, fnIndex, data, sessionHash, cancellationToken))[0]!.GetValue<string>();

    /// <summary>Runs once and returns what the run's outputs carried.</summary>
    internal static async Task<JsonArray> CompletedDataAsync(
        QueueClient queue,
        int fnIndex,
        JsonArray data,
        string sessionHash,
        CancellationToken cancellationToken) =>
        (await CompletedAsync(queue, fnIndex, data, sessionHash, cancellationToken))["output"]!["data"]!.AsArray();

    /// <summary>Runs once and returns the completion message.</summary>
    /// <param name="queue">The queue to run it through.</param>
    /// <param name="fnIndex">Which binding to run.</param>
    /// <param name="data">The values the binding's inputs are read from.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <param name="eventData">
    /// What the event says about itself — which message was picked, which key was pressed — or
    /// <see langword="null"/> for an event that says nothing.
    /// </param>
    internal static async Task<JsonObject> CompletedAsync(
        QueueClient queue,
        int fnIndex,
        JsonArray data,
        string sessionHash,
        CancellationToken cancellationToken,
        JsonNode? eventData = null)
    {
        var eventId = await queue.JoinAsync(fnIndex, data, sessionHash, cancellationToken, eventData);

        await using var stream = await queue.OpenAsync(sessionHash, cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.True(completed["success"]!.GetValue<bool>(), completed.ToJsonString());

        return completed;
    }
}

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The tests that drive the queue, which run one at a time.
/// </summary>
/// <remarks>
/// One of them changes how often the queue heartbeats, through the environment variable the Python
/// version reads, and restores it afterwards. A heartbeat on a stream is exactly what the
/// message-sequence comparison would see as an extra message, so the two must not run at the same
/// time — and the variable is process-wide, so putting them in one collection is what keeps the
/// comparison's streams silent.
/// </remarks>
internal static class QueueCollection
{
    /// <summary>The collection's name, as the tests declare it.</summary>
    internal const string Name = "queue";
}

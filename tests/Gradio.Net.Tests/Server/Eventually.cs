namespace Gradio.Net.Tests.Server;

/// <summary>
/// Waits for something to become true, because the queue does its work in the background.
/// </summary>
/// <remarks>
/// A queue hands runs to handlers on its own schedule, so a test that asserted immediately after
/// asking for a run would be asserting about the scheduler's timing rather than about the queue.
/// Polling a condition is the honest form of "wait until it has happened", and it is the same form
/// in each place that needs it.
/// </remarks>
internal static class Eventually
{
    /// <summary>How long to wait between looks.</summary>
    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(20);

    /// <summary>Polls a condition until it holds, or the window passes.</summary>
    /// <param name="condition">What to wait for.</param>
    /// <param name="within">How long to keep looking.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>Whether it held in time.</returns>
    internal static async Task<bool> IsTrueAsync(
        Func<Task<bool>> condition,
        TimeSpan within,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(condition);

        var deadline = System.DateTime.UtcNow + within;

        while (System.DateTime.UtcNow < deadline)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(Interval, cancellationToken);
        }

        // One last look: the condition may have become true while the last wait was running.
        return await condition();
    }
}

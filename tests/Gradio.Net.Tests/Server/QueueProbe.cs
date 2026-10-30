namespace Gradio.Net.Tests.Server;

/// <summary>
/// A handler that waits to be let go, so that a test can look at what the queue does while a run is
/// still in flight.
/// </summary>
/// <remarks>
/// <para>
/// The handler is the test's own code, so what it observes — how many runs were inside it at once,
/// whether the token it was given was signalled — is something a user's handler could observe too.
/// That keeps the assertions on what a client sees while still making the queue's scheduling
/// visible: the alternative would be to reach into the queue's own bookkeeping, which would assert
/// an implementation rather than a behaviour.
/// </para>
/// <para>
/// The counts are guarded by a lock rather than by interlocked operations because they are read as
/// a pair — how many entered, and the most that were ever inside at once — and a test that saw one
/// without the other could pass for the wrong reason.
/// </para>
/// </remarks>
internal sealed class QueueProbe
{
    private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly object _gate = new();

    private int _inside;
    private int _peak;
    private int _started;
    private object? _handed;
    private volatile bool _cancelled;

    /// <summary>The most runs that were ever inside the handler at the same time.</summary>
    internal int Peak
    {
        get
        {
            lock (_gate)
            {
                return _peak;
            }
        }
    }

    /// <summary>How many runs have entered the handler.</summary>
    internal int Started
    {
        get
        {
            lock (_gate)
            {
                return _started;
            }
        }
    }

    /// <summary>Whether the token a handler was given was signalled.</summary>
    internal bool Cancelled => _cancelled;

    /// <summary>What the last run through <see cref="HoldAndRecord"/> was handed.</summary>
    /// <remarks>
    /// What a run read, as opposed to what it wrote: the two are the same for a handler that
    /// answers with its input, and different for one that answers with something of its own — which
    /// is what a test of "did this run see the other run's write" needs.
    /// </remarks>
    internal object? Handed
    {
        get
        {
            lock (_gate)
            {
                return _handed;
            }
        }
    }

    /// <summary>Lets every waiting handler return.</summary>
    internal void Release() => _release.TrySetResult();

    /// <summary>A handler that records what it was handed, waits, and answers with its own value.</summary>
    /// <param name="value">What the run was handed, which <see cref="Handed"/> reports.</param>
    /// <param name="answer">What the run answers with once the test lets it go.</param>
    /// <returns>The answer, once released.</returns>
    internal async Task<object?> HoldAndRecord(object? value, object? answer)
    {
        lock (_gate)
        {
            _handed = value;
        }

        Enter();

        try
        {
            await _release.Task;

            return answer;
        }
        finally
        {
            Leave();
        }
    }

    /// <summary>A handler that waits until the test lets it go.</summary>
    /// <param name="value">The value to return, so that the run produces something recognisable.</param>
    /// <returns>The value, once released.</returns>
    internal async Task<string> Hold(string value)
    {
        Enter();

        try
        {
            await _release.Task;

            return value;
        }
        finally
        {
            Leave();
        }
    }

    /// <summary>
    /// A second handler that waits the same way.
    /// </summary>
    /// <param name="value">The value to return, so that the run produces something recognisable.</param>
    /// <returns>The value, once released.</returns>
    /// <remarks>
    /// A different method from <see cref="Hold"/>, so the two are different concurrency groups. That
    /// is what lets a test tell "the limit is per group" from "the queue runs one thing at a time".
    /// </remarks>
    internal Task<string> HoldOther(string value) => Hold(value);

    /// <summary>A handler that waits until it is released or cancelled.</summary>    /// <param name="value">The value to return if it is released.</param>
    /// <param name="cancellationToken">The token the framework hands the handler.</param>
    /// <returns>The value, once released.</returns>
    /// <remarks>
    /// A cancelled run is reported by throwing, which is what an async handler does when it
    /// observes its token — the shape the queue has to cope with when a client cancels.
    /// </remarks>
    internal async Task<string> HoldUntilCancelled(string value, CancellationToken cancellationToken)
    {
        Enter();

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);

            return value;
        }
        catch (OperationCanceledException)
        {
            _cancelled = true;

            throw;
        }
        finally
        {
            Leave();
        }
    }

    /// <summary>Waits until a number of runs have entered the handler.</summary>
    /// <param name="count">How many.</param>
    /// <param name="within">How long to wait.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>Whether that many entered in time.</returns>
    internal Task<bool> WaitForStartAsync(int count, TimeSpan within, CancellationToken cancellationToken) =>
        Eventually.IsTrueAsync(() => Task.FromResult(Started >= count), within, cancellationToken);

    /// <summary>Waits until a handler has observed a cancellation.</summary>
    /// <param name="within">How long to wait.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>Whether it was observed in time.</returns>
    internal Task<bool> WaitForCancellationAsync(TimeSpan within, CancellationToken cancellationToken) =>
        Eventually.IsTrueAsync(() => Task.FromResult(_cancelled), within, cancellationToken);

    private void Enter()
    {
        lock (_gate)
        {
            _started++;
            _inside++;
            _peak = Math.Max(_peak, _inside);
        }
    }

    private void Leave()
    {
        lock (_gate)
        {
            _inside--;
        }
    }
}

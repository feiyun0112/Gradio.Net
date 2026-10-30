namespace Gradio.Net;
/// <summary>
/// Tells the reload stream that the application it is serving has changed.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's development server watches the demo's source file and, when it changes,
/// bumps <c>app.change_count</c>; the page, which opens <c>/gradio_api/dev/reload</c> in
/// development mode, reloads itself when the number it is watching moves. The route is part of the
/// route table here but the watcher is not: watching files belongs to a development tool, and the
/// .NET tool for that is <c>dotnet watch</c> rather than something this package reimplements .
/// </para>
/// <para>
/// So the seam is this type, and a host that has its own watcher calls it. Registered in the
/// container, so a host resolves the same instance the stream is reading from:
/// </para>
/// <code>
/// app.Services.GetRequiredService&lt;GradioReloadSignal&gt;().AlertReload();
/// </code>
/// <para>
/// A change is one of two things, and they are two methods rather than one method with a kind,
/// because they are the only two the Python version ever emits and an enum would let a caller spell
/// a third: a reload, which carries nothing, and an error, which carries the message the page logs.
/// </para>
/// <para>
/// The state is read under a lock and the waiters are woken through a task that each change
/// replaces, so a change that lands while a stream is between two reads is neither lost nor
/// delivered twice: a stream that saw the old count is woken by the pulse it was already awaiting,
/// and a stream that starts after the change sees the new count before it waits.
/// </para>
/// </remarks>
public sealed class GradioReloadSignal
{
    private readonly object _gate = new();

    private TaskCompletionSource _pulse = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private int _count;
    private bool _isError;
    private string? _message;

    /// <summary>How many changes have been announced.</summary>
    /// <remarks>
    /// The Python version's <c>app.change_count</c>. A stream remembers the value it last reported
    /// and waits for a different one.
    /// </remarks>
    public int ChangeCount
    {
        get
        {
            lock (_gate)
            {
                return _count;
            }
        }
    }

    /// <summary>Announces that the application changed, and that a page should reload itself.</summary>
    public void AlertReload() => Announce(isError: false, message: null);

    /// <summary>Announces that the application failed to reload, and why.</summary>
    /// <param name="message">What to log, usually the failure's own text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public void AlertError(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Announce(isError: true, message);
    }

    /// <summary>Waits until the application has changed, or until the wait runs out.</summary>
    /// <param name="seen">The count the caller has already reported.</param>
    /// <param name="timeout">How long to wait before giving up.</param>
    /// <param name="clock">The clock the wait is measured with.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The change, or <see langword="null"/> when the wait ran out.</returns>
    /// <remarks>
    /// The caller does the heartbeat arithmetic: what this answers is only "has the count moved",
    /// which is the same question the stream asks between two heartbeats.
    /// </remarks>
    internal async Task<Change?> WaitForChangeAsync(
        int seen,
        TimeSpan timeout,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        Task pulse;

        lock (_gate)
        {
            if (_count != seen)
            {
                return new Change(_count, _isError, _message);
            }

            // Captured under the lock, so a change that lands after this point completes the task
            // the caller is about to await rather than replacing it unnoticed.
            pulse = _pulse.Task;
        }

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var expired = Task.Delay(timeout, clock, timeoutSource.Token);
        var first = await Task.WhenAny(pulse, expired).ConfigureAwait(false);

        // The timer is stopped either way: a change that arrived leaves it running for the rest of
        // the interval, and one timer per change is a timer nobody is waiting for.
        await timeoutSource.CancelAsync().ConfigureAwait(false);

        if (ReferenceEquals(first, pulse))
        {
            return Snapshot(seen);
        }

        try
        {
            await expired.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // The wait was cancelled rather than finished: the caller decides what that means, and
            // the stream's caller treats it as the client having gone away.
        }

        cancellationToken.ThrowIfCancellationRequested();

        return null;
    }

    /// <summary>The change since <paramref name="seen"/>, or <see langword="null"/> when there is none.</summary>
    private Change? Snapshot(int seen)
    {
        lock (_gate)
        {
            return _count == seen ? null : new Change(_count, _isError, _message);
        }
    }

    private void Announce(bool isError, string? message)
    {
        TaskCompletionSource pulse;

        lock (_gate)
        {
            _count++;
            _isError = isError;
            _message = message;

            pulse = _pulse;
            _pulse = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        // Outside the lock: a continuation that ran inline would read the state under the lock it
        // was just released from, and every waiter would wake on the same thread that announced.
        pulse.SetResult();
    }

    /// <summary>One announced change: the count it produced, and what it says.</summary>
    /// <param name="Count">The count after the change.</param>
    /// <param name="IsError">Whether the change is a failure rather than a reload.</param>
    /// <param name="Message">The failure's text, for an error.</param>
    internal readonly record struct Change(int Count, bool IsError, string? Message);
}

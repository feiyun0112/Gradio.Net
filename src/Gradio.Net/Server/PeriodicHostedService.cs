using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// A hosted service that does one thing on a timer, for as long as the host runs.
/// </summary>
/// <remarks>
/// <para>
/// Both of this batch's sweeps have the same shape — start a loop with the host, wait on a timer,
/// do one sweep, stop the loop when the host stops — and only the interval and the work differ.
/// The shape is here so that it is written once: a loop that forgets to observe its cancellation,
/// or a stop that does not wait for the loop to unwind, is a defect that would have to be found
/// twice otherwise.
/// </para>
/// <para>
/// A sweep that throws is logged and the loop goes on. What a sweep runs is application code — a
/// delete callback, a file that will not delete — so letting one failure end the loop would leave
/// everything else unreclaimed because of it.
/// </para>
/// </remarks>
internal abstract class PeriodicHostedService : IHostedService, IDisposable
{
    private readonly TimeProvider _clock;
    private readonly ILogger _logger;
    private readonly Action<ILogger, string, Exception?> _logSweepFailed;
    private readonly CancellationTokenSource _stopping = new();

    private Task? _loop;

    /// <summary>Creates the service.</summary>
    /// <param name="clock">The clock the waits are measured with.</param>
    /// <param name="logger">Where a failed sweep is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    protected PeriodicHostedService(TimeProvider clock, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(logger);

        _clock = clock;
        _logger = logger;
        _logSweepFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, "GradioSweepFailed"),
            "A sweep for {What} failed; the next one will retry.");
    }

    /// <summary>How often to sweep, or <see langword="null"/> for a service with nothing to do.</summary>
    /// <remarks>
    /// Read once, when the host starts, so that a policy cannot change under a loop that is already
    /// running with a different one.
    /// </remarks>
    protected abstract TimeSpan? Interval { get; }

    /// <summary>What the sweep reclaims, as the failure message spells it.</summary>
    protected abstract string What { get; }

    /// <summary>Whether a sweep was scheduled.</summary>
    protected bool IsScheduled { get; private set; }

    /// <summary>The moment the current sweep is measured against.</summary>
    protected DateTimeOffset Now => _clock.GetUtcNow();

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (Interval is { } interval)
        {
            IsScheduled = true;

            // The loop is governed by its own token rather than by the start call's: it outlives
            // the call, and is stopped by StopAsync.
            _loop = Task.Run(() => SweepAsync(interval, _stopping.Token), CancellationToken.None);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _stopping.CancelAsync().ConfigureAwait(false);

        if (_loop is not null)
        {
            try
            {
                await _loop.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // The loop was cancelled, which is how it stops.
            }
        }

        OnStopped();
    }

    /// <inheritdoc />
    public void Dispose() => _stopping.Dispose();

    /// <summary>Does one sweep.</summary>
    protected abstract void Sweep();

    /// <summary>Finishes what a sweep could not, once the loop has stopped.</summary>
    protected virtual void OnStopped()
    {
    }

    /// <summary>Sweeps, then waits, for as long as the host runs.</summary>
    /// <param name="interval">How long to wait between sweeps.</param>
    /// <param name="cancellationToken">Cancels the loop.</param>
    /// <returns>A task that completes when the loop ends.</returns>
    private async Task SweepAsync(TimeSpan interval, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(interval, _clock);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                Sweep();
            }
            catch (Exception error)
            {
                _logSweepFailed(_logger, What, error);
            }
        }
    }
}

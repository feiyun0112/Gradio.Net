using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// Reclaims the values sessions hold once their lifetime has elapsed, for as long as the host runs.
/// </summary>
/// <remarks>
/// The Python version runs the same sweep on a one-second loop, started with the application rather
/// than by the first request: a state that expired while nobody was asking for anything is exactly
/// the state this exists to reclaim, so waiting for traffic to trigger the sweep would defeat it.
/// </remarks>
internal sealed class GradioSessionStateHostedService : PeriodicHostedService
{
    /// <summary>How often expired state is reclaimed.</summary>
    /// <remarks>
    /// One second, which is the Python version's figure. The sweep is a walk of the sessions with
    /// no work in it for a session whose values are all alive, so the cost is bounded by the number
    /// of sessions rather than by what they hold.
    /// </remarks>
    private static readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

    private readonly ExpiredStateSweeper _sweeper;

    /// <summary>Creates the service.</summary>
    /// <param name="sweeper">The sweep to run.</param>
    /// <param name="clock">The clock the wait between sweeps is measured with.</param>
    /// <param name="logger">Where a failed sweep is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioSessionStateHostedService(
        ExpiredStateSweeper sweeper,
        TimeProvider clock,
        ILogger<GradioSessionStateHostedService> logger)
        : base(clock, logger)
    {
        ArgumentNullException.ThrowIfNull(sweeper);

        _sweeper = sweeper;
    }

    /// <inheritdoc />
    protected override TimeSpan? Interval => _interval;

    /// <inheritdoc />
    protected override string What => "expired session state";

    /// <inheritdoc />
    protected override void Sweep() => _sweeper.Sweep();
}

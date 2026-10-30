using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gradio.Net;
/// <summary>
/// What a probe asks about this application: is it ready to be given traffic?
/// </summary>
/// <remarks>
/// <para>
/// Ready means two things, and both are states a host can be listening in while unable to serve:
/// the application has been written, and the queue is running. A host started before its UI was
/// written answers every request with an empty page, and a host whose queue has not started admits
/// runs to a queue nobody drains. Neither is a thing to send a user to, and neither is visible from
/// outside without asking.
/// </para>
/// <para>
/// This is an addition to the framework rather than to the protocol — the Python version has no
/// health route — which is why it is a check a host opts into rather than an endpoint that appears.
/// The route table is the authority for what this framework exposes, and a route the Python version
/// does not have would fail the route-set comparison.
/// </para>
/// </remarks>
public sealed class GradioHealthCheck : IHealthCheck
{
    private readonly Blocks _blocks;
    private readonly GradioQueueStatus _queue;

    /// <summary>Creates the check.</summary>
    /// <param name="blocks">The application whose readiness is reported.</param>
    /// <param name="queue">The queue whose state is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioHealthCheck(Blocks blocks, GradioQueueStatus queue)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(queue);

        _blocks = blocks;
        _queue = queue;
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_blocks.IsRendered)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "The application has not been rendered; write its UI before starting the host."));
        }

        if (!_queue.Running)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("The queue is not running."));
        }

        var data = new Dictionary<string, object>
        {
            ["queue_depth"] = _queue.Depth,
            ["active_runs"] = _queue.Active,
            ["blocks"] = _blocks.Registered.Count,
        };

        return Task.FromResult(HealthCheckResult.Healthy("The application is serving.", data));
    }
}

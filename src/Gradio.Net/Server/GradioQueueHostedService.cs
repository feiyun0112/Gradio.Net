using Microsoft.Extensions.Hosting;

namespace Gradio.Net;
/// <summary>
/// Starts the queue with the host, and stops it with the host.
/// </summary>
/// <remarks>
/// <para>
/// The queue is started when the application starts rather than on the first request, which is the
/// Python version's order and the one that makes "the queue is running" a property of the
/// application rather than of traffic: a request that arrives before the scheduler is up would
/// otherwise be admitted to a queue nobody is draining.
/// </para>
/// <para>
/// Stopping is what lets a shutdown finish: the scheduler loops are cancelled and the sessions are
/// released, so a host that is asked to stop does not wait for a run that will never be collected.
/// </para>
/// </remarks>
internal sealed class GradioQueueHostedService : IHostedService
{
    /// <summary>
    /// How many runs may be in flight at once across every group.
    /// </summary>
    /// <remarks>
    /// The Python version's default worker count. It is a ceiling on the whole queue rather than a
    /// thread count: the runs are asynchronous, and what this bounds is how much work an
    /// application admits at once.
    /// </remarks>
    private const int WorkerCount = 40;

    private readonly GradioQueue _queue;

    /// <summary>Creates the service.</summary>
    /// <param name="queue">The queue to start and stop.</param>
    /// <exception cref="ArgumentNullException"><paramref name="queue"/> is <see langword="null"/>.</exception>
    public GradioQueueHostedService(GradioQueue queue)
    {
        ArgumentNullException.ThrowIfNull(queue);

        _queue = queue;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _queue.Start(WorkerCount);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => _queue.DisposeAsync().AsTask();
}

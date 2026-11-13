using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
    private readonly GradioQueue _queue;
    private readonly IOptions<GradioCoreOptions> _options;

    /// <summary>Creates the service.</summary>
    /// <param name="queue">The queue to start and stop.</param>
    /// <param name="options">The kernel options, whose <c>MaxThreads</c> names the ceiling.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioQueueHostedService(GradioQueue queue, IOptions<GradioCoreOptions> options)
    {
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(options);

        _queue = queue;
        _options = options;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The ceiling is read when the host starts rather than when the container is built: an
    /// application that configures <c>AddGradio</c> or calls <c>Launch(maxThreads: ...)</c> does so
    /// before the host starts, which is also when Python's <c>launch</c> sets its own
    /// <c>max_threads</c>. A value below one is clamped to one, the way the queue's slots are.
    /// </remarks>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _queue.Start(Math.Max(1, _options.Value.MaxThreads));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => _queue.DisposeAsync().AsTask();
}

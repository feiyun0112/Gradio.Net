using Microsoft.Extensions.Hosting;

namespace Gradio.Net;
/// <summary>
/// Runs the application's startup events when the host starts, and once only — and reports what
/// the Python version reports at that moment.
/// </summary>
/// <remarks>
/// <para>
/// The Python version starts its queue and then awaits the application's extra startup events in
/// one place, and reaches that place two ways: <c>launch()</c> asks its own <c>/startup-events</c>
/// route, and an app mounted on another one runs them from the host's lifespan. This is the .NET
/// shape of the second, and it is the one both entry points go through — a hosted service runs for
/// <c>GradioServer</c> and for <c>AddGradio</c> alike, which is what keeps the two from answering
/// this route differently.
/// </para>
/// <para>
/// Registered after the queue's own service, because the Python version starts the queue first: a
/// startup event that enqueues work would otherwise arrive before there is a scheduler to take it.
/// </para>
/// <para>
/// A host that never starts the application — one that only maps the routes — leaves the events
/// unrun, and the startup-events route then answers <c>true</c> to whoever asks first. That is the
/// same statement either way: the flag says whether the work has been done, not whether the route
/// exists.
/// </para>
/// <para>
/// The analytics events are reported from here rather than by the application, and around the
/// startup work rather than all at once: the Python version reports the application being
/// constructed from its constructor and the application being launched at the end of
/// <c>launch()</c>, with the startup events in between. Here the container builds the application
/// empty and the author writes it, so "constructed" is not a moment anybody can name — the host
/// starting is the first moment it can. The two are therefore reported either side of the startup
/// events, which is the Python version's order: an application that fails to start has reported
/// that it was built and not that it launched.
/// </para>
/// </remarks>
internal sealed class GradioStartupHostedService : IHostedService
{
    private readonly Blocks _blocks;
    private readonly IAnalyticsSink _analyticsSink;

    /// <summary>Creates the service.</summary>
    /// <param name="blocks">The application whose startup events this runs.</param>
    /// <param name="analyticsSink">Where the application's analytics events go.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioStartupHostedService(Blocks blocks, IAnalyticsSink analyticsSink)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(analyticsSink);

        _blocks = blocks;
        _analyticsSink = analyticsSink;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // What the Python version reports before it does any work: the version check and the application
        // having been built. Reporting these first is also what makes a failed start say the right
        // thing — the application was built, and it never launched.
        ApplicationAnalytics.VersionCheck(_blocks, _analyticsSink);
        ApplicationAnalytics.Initiated(_blocks, _analyticsSink);

        await _blocks.RunStartupEventsAsync(cancellationToken).ConfigureAwait(false);

        // And what the Python version reports once it is up. The editor's event comes with the launch one,
        // because a host that starts with the editor on is the moment the editor was started.
        ApplicationAnalytics.Launched(_blocks, _analyticsSink);
        ApplicationAnalytics.Vibe(_blocks, _analyticsSink);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Nothing to undo: the events prepare an application for being served, and the application is
    /// not served any more. The Python version does not un-run them either — it resets the flag
    /// when the app is closed, which is what makes a relaunch start again, and this host has no
    /// relaunch.
    /// </remarks>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

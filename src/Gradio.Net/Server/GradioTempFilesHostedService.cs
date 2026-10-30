using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// Deletes the temporary files an application recorded, on the schedule it asked for.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>delete_cache</c> behaviour, and it is entirely opt-in: an
/// application that sets it is cleaned up after itself while it runs, and what it wrote is deleted
/// when it stops. An application that sets nothing keeps every file it wrote — the Python version's
/// schedule is not even started in that case — which is why nothing here runs without a policy.
/// </para>
/// <para>
/// The policy is read once, when the host starts, because that is when the Python version reads it:
/// the settings are an application's declaration about itself, not something a request changes.
/// </para>
/// </remarks>
internal sealed class GradioTempFilesHostedService : PeriodicHostedService
{
    private readonly Blocks _blocks;

    /// <summary>Creates the service.</summary>
    /// <param name="blocks">The application whose files are reclaimed.</param>
    /// <param name="clock">The clock the ages are measured with.</param>
    /// <param name="logger">Where a failed sweep is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioTempFilesHostedService(
        Blocks blocks,
        TimeProvider clock,
        ILogger<GradioTempFilesHostedService> logger)
        : base(clock, logger)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <inheritdoc />
    protected override TimeSpan? Interval => _blocks.DeleteCache?.Frequency;

    /// <inheritdoc />
    protected override string What => "expired temporary files";

    /// <inheritdoc />
    protected override void Sweep()
    {
        if (_blocks.DeleteCache is { } settings)
        {
            _blocks.DeleteTempFiles(settings.Age, Now);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// An application that asked to be cleaned up while it ran is cleaned up when it stops too: the
    /// rest of what it wrote would otherwise be left for a sweep that will never come. One that
    /// asked for nothing keeps its files, which is what it asked for.
    /// </remarks>
    protected override void OnStopped()
    {
        if (IsScheduled)
        {
            _blocks.DeleteTempFiles(age: null, Now);
        }
    }
}

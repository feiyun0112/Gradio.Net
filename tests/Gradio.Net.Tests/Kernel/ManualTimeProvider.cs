namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A clock the tests move by hand, so that time-to-live behaviour is asserted exactly rather than
/// by sleeping and hoping.
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    private DateTimeOffset _now;

    internal ManualTimeProvider(DateTimeOffset start) => _now = start;

    internal DateTimeOffset Now => _now;

    public override DateTimeOffset GetUtcNow() => _now;

    internal void Advance(TimeSpan by) => _now += by;
}

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Sets the heartbeat interval for the length of a test, and puts back whatever was there.
/// </summary>
/// <remarks>
/// The variable is process-wide and the server reads it once, when it is built, so a test that
/// wants a short interval has to set it before the host starts and restore it afterwards — which is
/// what <see cref="EnvironmentVariable"/> does. Every test that needs it is in the queue collection,
/// so two of them cannot be setting it at once, and a leaked value would make the next test's
/// stream beat at the wrong rate.
/// </remarks>
internal static class HeartbeatIntervalVariable
{
    /// <summary>Sets the variable.</summary>
    /// <param name="value">The interval, in seconds, as the variable spells it.</param>
    /// <returns>What puts the previous value back.</returns>
    internal static IDisposable Set(string value) =>
        EnvironmentVariable.Set("GRADIO_HEARTBEAT_INTERVAL", value);
}

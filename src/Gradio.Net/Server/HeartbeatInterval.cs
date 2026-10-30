using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// How often this server tells a client that a stream is still alive.
/// </summary>
/// <remarks>
/// <para>
/// One value for the whole server, resolved once, because it is one decision: a deployment that
/// shortens the interval — a Kubernetes ingress that drops idle connections in twenty seconds, say
/// — means every stream it serves, not one route's stream. The Python version reads
/// <c>GRADIO_HEARTBEAT_INTERVAL</c> for its queue streams and hardcodes fifteen seconds for the
/// reload stream; here both read this, so the knob an operator already turns turns both.
/// </para>
/// <para>
/// The default is the Python version's fifteen seconds, and <c>GRADIO_IS_E2E_TEST</c> shortens it
/// the way the Python version's own helper does, so a suite that drives a browser does not wait a
/// quarter of a minute for a frame.
/// </para>
/// <para>
/// A value that cannot be read is reported once, when this is constructed, rather than per stream:
/// the setting is wrong for the deployment, not for the request. The logger and its message are
/// instance fields rather than statics, because the package declares no static mutable state
///.
/// </para>
/// </remarks>
internal sealed class HeartbeatInterval
{
    /// <summary>The interval the Python version uses when nothing says otherwise.</summary>
    private const double DefaultSeconds = 15;

    /// <summary>The interval used while an end-to-end test is driving a browser.</summary>
    private const double EndToEndTestSeconds = 0.25;

    private readonly Action<ILogger, string, Exception?> _logRejected;

    /// <summary>Creates the value from the environment.</summary>
    /// <param name="logger">Where a value that cannot be read is reported.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    public HeartbeatInterval(ILogger<HeartbeatInterval> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logRejected = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "GradioHeartbeatIntervalIgnored"),
            "Ignoring GRADIO_HEARTBEAT_INTERVAL value '{Value}': expected a positive number of "
            + "seconds. The default is in force.");

        Interval = TimeSpan.FromSeconds(Resolve(logger));
    }

    /// <summary>The interval to wait between two heartbeats on a stream.</summary>
    public TimeSpan Interval { get; }

    private double Resolve(ILogger logger)
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_HEARTBEAT_INTERVAL");

        if (!string.IsNullOrEmpty(configured))
        {
            if (double.TryParse(configured, CultureInfo.InvariantCulture, out var seconds) && seconds > 0)
            {
                return seconds;
            }

            _logRejected(logger, configured, null);
        }

        return Environment.GetEnvironmentVariable("GRADIO_IS_E2E_TEST") is { Length: > 0 }
            ? EndToEndTestSeconds
            : DefaultSeconds;
    }
}

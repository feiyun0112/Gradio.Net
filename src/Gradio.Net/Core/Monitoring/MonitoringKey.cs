using System.Security.Cryptography;

namespace Gradio.Net;
/// <summary>
/// The key an application's monitoring panel is addressed by.
/// </summary>
/// <remarks>
/// The Python version draws it with <c>secrets.token_urlsafe(16)</c>: sixteen random bytes,
/// base64url-encoded without padding, which is twenty-two characters a URL can carry unchanged. The
/// same shape is drawn here, for the same reasons — a panel whose address is guessable is a panel
/// that is not protected, because the panel is the one monitoring route that the Python version
/// does not put behind its login check.
/// </remarks>
internal static class MonitoringKey
{
    /// <summary>How many random bytes a key is drawn from.</summary>
    private const int EntropyBytes = 16;

    /// <summary>Draws a key.</summary>
    /// <returns>The key, as twenty-two URL-safe characters.</returns>
    internal static string New() => Convert
        .ToBase64String(RandomNumberGenerator.GetBytes(EntropyBytes))
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');
}

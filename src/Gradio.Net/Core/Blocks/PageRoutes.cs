using System.Text;
using System.Text.RegularExpressions;

namespace Gradio.Net;
/// <summary>
/// The rules a page's URL suffix has to satisfy, and how one is derived from a page name.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Blocks.route</c> reads a path three ways — strips its slashes, checks
/// its characters, and refuses one of the application's own routes — and derives one from the name
/// when none is given. The rules live here rather than in the method because they are also what a
/// host needs to know before it serves the page, and because a rule written twice is a rule that
/// can disagree with itself.
/// </para>
/// <para>
/// The derivation is the Python version's <c>slugify</c>: fold to ASCII, lower-case, drop
/// everything that is not a word character, a space or a hyphen, then collapse runs of spaces and
/// hyphens into one hyphen.
/// </para>
/// </remarks>
internal static partial class PageRoutes
{
    /// <summary>
    /// The paths the application serves itself, which no page may claim.
    /// </summary>
    /// <remarks>The Python version's <c>INTERNAL_ROUTES</c>, in its order.</remarks>
    private static readonly string[] _internalRoutes =
    [
        "theme.css",
        "robots.txt",
        "pwa_icon",
        "manifest.json",
        "login",
        "logout",
        "svelte",
        "config",
        "static",
        "assets",
        "favicon.ico",
        "gradio_api",
        "monitoring",
    ];

    /// <summary>Derives a page's URL suffix from its name.</summary>
    /// <param name="name">The name the navbar shows.</param>
    /// <returns>The suffix, or the empty string when the name carries nothing a URL can hold.</returns>
    internal static string FromName(string name)
    {
        var ascii = new StringBuilder(name.Length);

        foreach (var character in name.Normalize(NormalizationForm.FormKD))
        {
            if (character < 128)
            {
                ascii.Append(character);
            }
        }

        var stripped = new StringBuilder(ascii.Length);
        foreach (var character in ascii.ToString().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character) || character is '_' or '-' || char.IsWhiteSpace(character))
            {
                stripped.Append(character);
            }
        }

        return Separators().Replace(stripped.ToString(), "-").Trim('-', '_');
    }

    /// <summary>Reads a path the application gave, and refuses one a URL suffix cannot carry.</summary>
    /// <param name="path">The path as written, with or without its slashes.</param>
    /// <returns>The suffix, without its slashes.</returns>
    /// <exception cref="ArgumentException">The path carries a character a URL suffix cannot.</exception>
    internal static string FromPath(string path)
    {
        var suffix = path.Trim('/');

        if (suffix.Length > 0 && !Allowed().IsMatch(suffix))
        {
            throw new ArgumentException(
                $"The path '{path}' contains a character a page's URL suffix cannot carry. A suffix "
                + "may hold letters, digits and -._~!$&'()*+,;=:@[]",
                nameof(path));
        }

        return suffix;
    }

    /// <summary>Whether a suffix names one of the application's own routes.</summary>
    /// <param name="suffix">The suffix to check.</param>
    /// <returns>Whether the application already serves that path.</returns>
    internal static bool IsInternalRoute(string suffix) =>
        _internalRoutes.Contains(suffix, StringComparer.Ordinal);

    /// <summary>A suffix that would leave the application's own routes: everything else is allowed.</summary>
    [GeneratedRegex(@"^[a-zA-Z0-9\-._~!$&'()*+,;=:@\[\]]+$", RegexOptions.CultureInvariant)]
    private static partial Regex Allowed();

    /// <summary>Runs of spaces and hyphens, which a derived suffix collapses into one hyphen.</summary>
    [GeneratedRegex(@"[\s-]+", RegexOptions.CultureInvariant)]
    private static partial Regex Separators();
}

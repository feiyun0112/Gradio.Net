using System.Collections.Frozen;

namespace Gradio.Net;
/// <summary>
/// Which font files the vendored frontend carries, which is the question a
/// <see cref="GoogleFont"/> asks before it decides whether to fetch anything.
/// </summary>
/// <remarks>
/// <para>
/// The Python version asks the same question of its own installed package: if the face is in
/// <c>templates/frontend/static/fonts</c>, a "Google" font is served from the same origin and no
/// request leaves the machine; if it is not, the theme points at Google's font service. The
/// vendored frontend here is that same tree, embedded in this assembly, so the answer is the set of
/// resource names rather than a directory listing.
/// </para>
/// <para>
/// The prefix is the one <c>Gradio.Net.csproj</c> gives every asset, and this is the second place
/// that spells it. A test walks the checked-in hash record and asserts that every font the
/// built-in themes name resolves here, so a change to how the frontend is embedded fails there
/// rather than by silently turning a local font into a remote one.
/// </para>
/// </remarks>
internal static class VendoredFonts
{
    /// <summary>The resource-name prefix every embedded frontend asset carries.</summary>
    private const string Prefix = "Gradio.Net.frontend/static/fonts/";

    private static readonly FrozenSet<string> _resources =
        typeof(VendoredFonts).Assembly.GetManifestResourceNames().ToFrozenSet(StringComparer.Ordinal);

    /// <summary>Whether every one of a family's weights is carried by the frontend.</summary>
    /// <param name="name">The family name.</param>
    /// <param name="weights">The weights asked for.</param>
    /// <returns><see langword="true"/> when every weight has a file.</returns>
    internal static bool Contains(string name, IEnumerable<int> weights)
    {
        var directory = LocalFont.FileName(name);

        return weights.All(weight => _resources.Contains(
            $"{Prefix}{directory}/{directory}-{LocalFont.WeightFileName(weight)}.woff2"));
    }
}

using System.Collections.Frozen;

namespace Gradio.Net;
/// <summary>
/// The type a file is sent as, guessed from its extension.
/// </summary>
/// <remarks>
/// <para>
/// The Python version guesses from the file name and then overrides <c>.js</c> to
/// <c>application/javascript</c>; the extensions that matter to a browser are listed here and
/// everything else falls back to <c>application/octet-stream</c>.
/// </para>
/// <para>
/// The font types are stated rather than guessed because the Python version's guess comes from the
/// interpreter's <c>mimetypes</c> table, which answers <c>font/*</c> on Linux and nothing at all
/// on Windows. A fixed answer is the one that is the same everywhere, and it is the one a
/// browser expects.
/// </para>
/// <para>
/// <c>text/*</c> carries <c>charset=utf-8</c>, as the Python version's file responses add it; the
/// JavaScript and WebAssembly types do not, and neither do in the Python version's.
/// </para>
/// <para>
/// It is a type of its own rather than a member of one of its two callers because the two serve
/// different things — the vendored frontend this assembly carries, and a bundle an application
/// built — and only the table is common to them. A second table would be a second answer for a
/// <c>.js</c>, and the two answers would drift the first time one of them met a type the other had
/// not.
/// </para>
/// </remarks>
internal static class ContentTypes
{
    /// <summary>Guesses a file's type from its name.</summary>
    /// <param name="path">The path or file name to guess from.</param>
    /// <returns>The content type, carrying <c>charset=utf-8</c> when it is a text type.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    internal static string Of(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var extension = Path.GetExtension(path);

        if (extension.Length == 0 || !_byExtension.TryGetValue(extension, out var contentType))
        {
            return Default;
        }

        return contentType.StartsWith("text/", StringComparison.Ordinal)
            ? contentType + "; charset=utf-8"
            : contentType;
    }

    /// <summary>What a file with an extension not in the table is sent as.</summary>
    private const string Default = "application/octet-stream";

    /// <summary>The types the Python version's file responses send, by extension.</summary>
    private static readonly FrozenDictionary<string, string> _byExtension =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".css"] = "text/css",
            [".html"] = "text/html",
            [".ico"] = "image/x-icon",
            [".js"] = "application/javascript",
            [".json"] = "application/json",
            [".map"] = "application/json",
            [".png"] = "image/png",
            [".svg"] = "image/svg+xml",
            [".ttf"] = "font/ttf",
            [".woff"] = "font/woff",
            [".woff2"] = "font/woff2",
            [".txt"] = "text/plain",
            [".wasm"] = "application/wasm",
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}

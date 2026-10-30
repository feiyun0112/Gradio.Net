using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The icon an application offers as its app icon, and the manifest that names it.
/// </summary>
/// <remarks>
/// <para>
/// A browser reads the manifest before it offers to install an application, and the manifest's job
/// is to say which image is the icon and how big it is. There are three answers, and which one
/// applies is a property of the file the application named rather than of anything a request says:
/// an application that names no icon is given the Python version's own logo, a vector is named once
/// and is good at any size, and anything else is named twice at the two sizes a launcher asks for.
/// </para>
/// <para>
/// The document is built here rather than in the endpoint because it is a document rather than an
/// answer: the endpoint decides the status and the headers, and this decides what a browser reads.
/// The field order is the Python version's, and it is reproduced rather than left to a serialiser's
/// own ordering, because a comparison against the Python version is made of the bytes.
/// </para>
/// </remarks>
internal static class AppIcon
{
    /// <summary>The route the icon is served from, which the manifest names.</summary>
    internal const string IconPath = "/pwa_icon";

    /// <summary>The size a launcher asks for on a home screen.</summary>
    internal const int SmallSize = 192;

    /// <summary>The size a launcher asks for on a splash screen.</summary>
    internal const int LargeSize = 512;

    /// <summary>
    /// The largest size the icon route will draw, which is sixteen times the largest a launcher
    /// asks for.
    /// </summary>
    /// <remarks>
    /// The Python version has no ceiling and allocates a bitmap of whatever size it is asked for,
    /// on a route that is not behind its login check. A request for a hundred thousand pixels a
    /// side is a request for forty gigabytes, so the size is bounded here and the refusal is
    /// declared in the ledger rather than left as a way to stop the process.
    /// </remarks>
    internal const int MaxSize = 4096;

    /// <summary>What a vector icon is sent as.</summary>
    internal const string VectorType = "image/svg+xml";

    /// <summary>What a scaled icon is sent as, whatever the file it was scaled from was.</summary>
    internal const string RasterType = "image/png";

    /// <summary>The Python version's own logo, as a path relative to the manifest's own
    /// address.</summary>
    private const string NoIconSource = "static/img/logo_nosize.svg";

    /// <summary>The name an application with no title is installed under.</summary>
    private const string NoTitleName = "Gradio";

    /// <summary>The icon the application named, or <see langword="null"/> when it named none.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The path, as the application gave it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// One statement of "an empty path names no icon", because two answers read it: the manifest
    /// decides which icon entries to write, and the icon routes decide whether they have anything to
    /// serve. Two statements would eventually disagree, and a manifest naming an address that
    /// answers 404 is the disagreement that matters.
    /// </remarks>
    internal static string? NamedBy(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        return blocks.FaviconPath is { Length: > 0 } icon ? icon : null;
    }

    /// <summary>Whether an icon is drawn rather than rastered.</summary>
    /// <param name="path">The icon's path, as the application named it.</param>
    /// <returns><see langword="true"/> when the file is an SVG.</returns>
    /// <remarks>
    /// The Python version compares the suffix case-sensitively, which makes an icon named
    /// <c>.SVG</c> a raster it then fails to scale. The comparison here ignores case, so the suffix
    /// decides how the icon is answered rather than how it was spelled; the ledger records the
    /// difference.
    /// </remarks>
    internal static bool IsVector(string path) =>
        Path.GetExtension(path).Equals(".svg", StringComparison.OrdinalIgnoreCase);

    /// <summary>The web app manifest an application answers with.</summary>
    /// <param name="blocks">The application the manifest describes.</param> <returns>The document,
    /// in the Python version's field order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is
    /// <see langword="null"/>.</exception>
    internal static JsonObject ManifestOf(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        return new JsonObject
        {
            ["name"] = string.IsNullOrEmpty(blocks.Title) ? NoTitleName : blocks.Title,
            ["icons"] = IconsOf(NamedBy(blocks)),
            ["start_url"] = "./",
            ["display"] = "standalone",
        };
    }

    /// <summary>The icon entries the manifest carries.</summary> <param name="icon">The icon the
    /// application named, or <see langword="null"/> for none.</param> <returns>The entries, in the
    /// order the Python version writes them.</returns>
    private static JsonArray IconsOf(string? icon)
    {
        if (icon is null)
        {
            // The Python version's logo, named relatively: a browser resolves it against the manifest's own
            // address, which is the root, so it reaches the vendored tree the page is served from.
            return [Icon(NoIconSource, "any", VectorType)];
        }

        if (IsVector(icon))
        {
            return [Icon(IconPath, "any", VectorType)];
        }

        return
        [
            Icon($"{IconPath}/{SmallSize}", $"{SmallSize}x{SmallSize}", RasterType),
            Icon($"{IconPath}/{LargeSize}", $"{LargeSize}x{LargeSize}", RasterType),
        ];
    }

    /// <summary>One icon entry.</summary>
    /// <param name="source">The address the image is served from.</param>
    /// <param name="sizes">The sizes it is good at.</param>
    /// <param name="type">What it is sent as.</param>
    /// <returns>The entry.</returns>
    private static JsonObject Icon(string source, string sizes, string type) => new()
    {
        ["src"] = source,
        ["sizes"] = sizes,
        ["type"] = type,
        ["purpose"] = "any",
    };
}

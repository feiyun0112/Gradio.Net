using System.Collections.Frozen;
using Microsoft.AspNetCore.StaticFiles;

namespace Gradio.Net;
/// <summary>
/// What a file's media type is, and whether a browser may be trusted to render it inline.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps a list of types it serves <c>inline</c> and serves everything else as a
/// download of <c>application/octet-stream</c>. The list is a security boundary rather than a
/// convenience: an inline <c>text/html</c> file is a script the application's own origin runs, so a
/// type that is not on the list is not rendered at all.
/// </para>
/// <para>
/// This lives in the server layer because it is about serving: what a file is called and how it is
/// read is the core's business, what a browser is allowed to do with it is the endpoint's.
/// </para>
/// <para>
/// The extensions the Python version teaches its MIME database — the media ones the platform gets
/// wrong or does not know — are registered here for the same reason they are registered there: an
/// <c>.m4a</c> that arrives as <c>application/octet-stream</c> is a download where a player was
/// expected.
/// </para>
/// </remarks>
public static class MediaMimeTypes
{
    /// <summary>The types that may be served inline.</summary>
    /// <remarks>
    /// The Python version's list, kept in its order for reviewability. It is a set of media and
    /// text types — nothing that a browser would execute as a document.
    /// </remarks>
    private static readonly FrozenSet<string> _safe = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "audio/aac",
        "audio/aiff",
        "audio/flac",
        "audio/mp4",
        "audio/mpeg",
        "audio/ogg",
        "audio/wav",
        "audio/webm",
        "audio/x-aiff",
        "audio/x-flac",
        "audio/x-m4a",
        "audio/x-matroska",
        "audio/x-wav",
        "video/mp4",
        "video/mpeg",
        "video/ogg",
        "video/quicktime",
        "video/webm",
        "video/x-matroska",
        "video/x-msvideo",
        "text/plain",
        "application/json",
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>The media types the platform's table gets wrong or does not know.</summary>
    private static readonly FrozenDictionary<string, string> _overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".aac"] = "audio/aac",
        [".aif"] = "audio/aiff",
        [".aifc"] = "audio/aiff",
        [".aiff"] = "audio/aiff",
        [".flac"] = "audio/flac",
        [".m4a"] = "audio/mp4",
        [".m4b"] = "audio/mp4",
        [".mka"] = "audio/x-matroska",
        [".wav"] = "audio/wav",
        [".weba"] = "audio/webm",
        [".avi"] = "video/x-msvideo",
        [".m4v"] = "video/mp4",
        [".mkv"] = "video/x-matroska",
        [".mov"] = "video/quicktime",
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FileExtensionContentTypeProvider _provider = CreateProvider();

    /// <summary>The media type of a file, from its extension.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The type, or <see langword="null"/> when the extension says nothing.</returns>
    public static string? Guess(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return _provider.TryGetContentType(path, out var contentType) ? contentType : null;
    }

    /// <summary>Whether a browser may render a type inline.</summary>
    /// <param name="mediaType">The type, or <see langword="null"/>.</param>
    /// <returns>Whether it is one of the types the Python version serves inline.</returns>
    public static bool IsSafe(string? mediaType) =>
        mediaType is not null && _safe.Contains(mediaType);

    /// <summary>
    /// The content type to answer with, and whether it is a download.
    /// </summary>
    /// <param name="path">The file's path.</param>
    /// <param name="allowed">Whether the file was served because the application allowed it.</param>
    /// <returns>The type to write, and whether the browser is told to save rather than display.</returns>
    /// <remarks>
    /// A file the application explicitly allowed is displayed whatever its type — that is the
    /// Python version's rule, and it is the operator's decision to make: naming a path as allowed
    /// is saying the files in it are safe. Everything else is displayed only when its type is one
    /// of the safe ones.
    /// </remarks>
    public static (string ContentType, bool Download) ForServing(string path, bool allowed)
    {
        var guessed = Guess(path);

        return IsSafe(guessed) || allowed
            ? (guessed ?? "application/octet-stream", false)
            : ("application/octet-stream", true);
    }

    /// <summary>
    /// The header value for a served type, with the charset a text type carries.
    /// </summary>
    /// <param name="contentType">The type.</param>
    /// <returns>The type, with <c>charset=utf-8</c> appended for a text type.</returns>
    /// <remarks>
    /// The suffix is the HTTP layer's, not the application's: the framework the Python version is
    /// built on appends it to every <c>text/*</c> response, and a browser that was told
    /// <c>text/plain</c> without it guesses a charset for text it was handed.
    /// </remarks>
    public static string WithCharset(string contentType) =>
        contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
            ? contentType + "; charset=utf-8"
            : contentType;

    private static FileExtensionContentTypeProvider CreateProvider()
    {
        var provider = new FileExtensionContentTypeProvider();

        foreach (var (extension, mediaType) in _overrides)
        {
            provider.Mappings[extension] = mediaType;
        }

        return provider;
    }
}

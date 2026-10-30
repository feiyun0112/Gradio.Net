using System.Collections.Frozen;

namespace Gradio.Net;
/// <summary>
/// The media type a file's name implies, which is what the frontend picks a renderer by.
/// </summary>
/// <remarks>
/// <para>
/// The Python version asks Python's <c>mimetypes</c> module, whose table is the operating system's;
/// there is no equivalent in the base class library, so the types a chat message realistically
/// carries are listed here and everything else is reported as unknown. Unknown is a value the
/// protocol allows — the frontend falls back to a download link — so a name this table does not
/// know is served as <see langword="null"/> rather than guessed at.
/// </para>
/// <para>
/// A caller that knows the type better than the name does says so on the <see cref="FileData"/>,
/// and this is only consulted for a file whose type is not already set.
/// </para>
/// </remarks>
internal static class MediaTypes
{
    private static readonly FrozenDictionary<string, string> _byExtension =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".png"] = "image/png",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".gif"] = "image/gif",
            [".webp"] = "image/webp",
            [".svg"] = "image/svg+xml",
            [".bmp"] = "image/bmp",
            [".avif"] = "image/avif",
            [".mp3"] = "audio/mpeg",
            [".wav"] = "audio/wav",
            [".ogg"] = "audio/ogg",
            [".m4a"] = "audio/mp4",
            [".flac"] = "audio/flac",
            [".mp4"] = "video/mp4",
            [".webm"] = "video/webm",
            [".mov"] = "video/quicktime",
            [".pdf"] = "application/pdf",
            [".txt"] = "text/plain",
            [".md"] = "text/markdown",
            [".csv"] = "text/csv",
            [".json"] = "application/json",
            [".html"] = "text/html",
            [".xml"] = "application/xml",
            [".zip"] = "application/zip",
            [".glb"] = "model/gltf-binary",
            [".gltf"] = "model/gltf+json",
            [".obj"] = "model/obj",
            [".stl"] = "model/stl",
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>The media type a path implies.</summary>
    /// <param name="path">A file's path or name.</param>
    /// <returns>The media type, or <see langword="null"/> when the name implies none this knows.</returns>
    internal static string? Of(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var extension = Path.GetExtension(path);

        return extension.Length > 0 && _byExtension.TryGetValue(extension, out var type) ? type : null;
    }

    /// <summary>Whether a file is one of the kinds an application accepts.</summary>
    /// <param name="path">The file's path or name.</param>
    /// <param name="accepted">The kinds accepted: extensions, media types, or <c>file</c> for any.</param>
    /// <returns>Whether the file is one of them.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>is_valid_file</c>, which it asks before handing a file to a handler:
    /// <c>file</c> accepts everything, an entry that starts with a dot is matched against the end
    /// of the name, and anything else is matched against the media type's kind — so <c>image</c>
    /// accepts every image and <c>text/csv</c> accepts exactly that one.
    /// </para>
    /// <para>
    /// It lives here rather than in a component because two of them ask it — the multimodal field and
    /// the upload button. The file and gallery components carry the same parameter and do not ask it
    /// yet: refusing one is a user-facing error, and the type that carries one is its own batch
    /// (ticket 38).
    /// </para>
    /// <para>
    /// A kind matched by media type is only as good as the table above: the Python version asks
    /// Python's <c>mimetypes</c>, which is the operating system's table, so <c>text</c> there
    /// accepts a <c>.py</c> file and here it does not. A kind written as an extension — which is
    /// what an application usually writes — is matched by name and does not depend on the table.
    /// </para>
    /// </remarks>
    internal static bool Accepts(string path, IReadOnlyList<string> accepted)
    {
        var name = Path.GetFileName(path).ToLowerInvariant();
        var mediaType = Of(path);

        foreach (var kind in accepted)
        {
            if (string.Equals(kind, "file", StringComparison.Ordinal))
            {
                return true;
            }

            if (kind.StartsWith('.'))
            {
                if (name.Length > kind.Length
                    && name.EndsWith(kind.ToLowerInvariant(), StringComparison.Ordinal))
                {
                    return true;
                }
            }
            else if (mediaType is not null
                && mediaType.StartsWith(kind + "/", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The sentence a refusal ends with, naming the kinds an application accepts.</summary>
    /// <param name="accepted">The kinds accepted.</param>
    /// <returns><c>Please upload a file that is one of these formats: ['.txt', '.csv']</c>.</returns>
    /// <remarks>
    /// The Python version writes this sentence twice, once for each component that refuses a file,
    /// and the two differ only in what comes before it: the multimodal field names the file's media
    /// type and the upload button does not. The half they share is the half that is here.
    /// </remarks>
    internal static string AcceptedFormats(IReadOnlyList<string> accepted) =>
        "Please upload a file that is one of these formats: " + PythonText.ReprOf(accepted);
}

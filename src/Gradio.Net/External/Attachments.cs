using System.Globalization;

namespace Gradio.Net;
/// <summary>
/// What a chat endpoint is sent for a file the user attached, and which files it is sent at all.
/// </summary>
/// <remarks>
/// <para>
/// A file does not travel to an OpenAI-compatible endpoint as a file. The Python version splits the
/// two cases this loader offers: a text-encoded file is read and appended to the prompt, and
/// anything else is sent as a data URL inside an image content block. Both conversions are here
/// because both are about the file rather than about the request.
/// </para>
/// <para>
/// The extension lists are the Python version's, in the Python version's order, because the config
/// carries them: the field a loaded chat draws says which files it accepts, and a client reading
/// that list reads exactly what the Python version's chat accepts.
/// </para>
/// </remarks>
internal static class Attachments
{
    /// <summary>The extensions a text-encoded file may have.</summary>
    /// <remarks>
    /// The Python version's <c>TEXT_FILE_EXTENSIONS</c>. The two entries spelled in capitals —
    /// <c>.R</c> and <c>.Rmd</c> — are kept as they are, and the match is case-insensitive on both
    /// sides, so a file named <c>plot.r</c> is a text file too.
    /// </remarks>
    internal static readonly IReadOnlyList<string> TextExtensions =
    [
        ".doc", ".docx", ".rtf", ".epub", ".odt", ".odp", ".pptx", ".txt", ".md", ".py", ".ipynb",
        ".js", ".jsx", ".html", ".css", ".java", ".cs", ".php", ".c", ".cc", ".cpp", ".cxx", ".cts",
        ".h", ".hh", ".hpp", ".rs", ".R", ".Rmd", ".swift", ".go", ".rb", ".kt", ".kts", ".ts",
        ".tsx", ".m", ".mm", ".mts", ".scala", ".dart", ".lua", ".pl", ".pm", ".t", ".sh", ".bash",
        ".zsh", ".bat", ".coffee", ".csv", ".log", ".ini", ".cfg", ".config", ".json", ".proto",
        ".yaml", ".yml", ".toml", ".sql",
    ];

    /// <summary>The extensions an image may have, and the media type each names.</summary>
    /// <remarks>
    /// The Python version's <c>IMAGE_FILE_EXTENSIONS</c>, in the Python version's order, paired
    /// with the type its own lookup answers for them. The two facts belong together: a list of
    /// extensions that a data URL cannot name would be a list of files a model is sent without
    /// knowing what they are.
    /// </remarks>
    private static readonly IReadOnlyList<(string Extension, string MediaType)> Images =
    [
        (".png", "image/png"),
        (".jpg", "image/jpeg"),
        (".jpeg", "image/jpeg"),
        (".gif", "image/gif"),
        (".webp", "image/webp"),
    ];

    /// <summary>The extensions an image may have.</summary>
    internal static IReadOnlyList<string> ImageExtensions { get; } = [.. Images.Select(image => image.Extension)];

    /// <summary>Whether a file is one the prompt carries as text rather than as an image.</summary>
    /// <param name="path">The file's path, or the address it is served at.</param>
    /// <returns>Whether its extension is one of <see cref="TextExtensions"/>.</returns>
    internal static bool IsTextEncoded(string path) =>
        TextExtensions.Any(extension => path.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

    /// <summary>The file as the text appended to a prompt.</summary>
    /// <param name="path">The file's path, or the address it is served at.</param>
    /// <param name="client">The client to fetch an address with.</param>
    /// <param name="cancellationToken">Cancels the fetch.</param>
    /// <returns>The heading and the contents, as the Python version writes them.</returns>
    /// <remarks>
    /// The heading is the file's own name, so that a model reading several files can tell them
    /// apart; the two newlines in front of it are the Python version's, and they are what separates
    /// one file's text from the message or from the file before it.
    /// </remarks>
    internal static async Task<string> AsPromptAsync(
        string path,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        var (name, contents) = IsAddress(path)
            ? (NameOf(path), await ReadTextFromAsync(path, client, cancellationToken).ConfigureAwait(false))
            : (Path.GetFileName(path), await System.IO.File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false));

        return string.Create(CultureInfo.InvariantCulture, $"\n## {name}\n{contents}");
    }

    /// <summary>The file as a data URL, which is how an image travels inside a message.</summary>
    /// <param name="path">The file's path, or the address it is served at.</param>
    /// <param name="client">The client to fetch an address with.</param>
    /// <param name="cancellationToken">Cancels the fetch.</param>
    /// <returns><c>data:&lt;media type&gt;;base64,&lt;bytes&gt;</c>.</returns>
    /// <remarks>
    /// A file whose extension names no media type is sent with the type left out —
    /// <c>data:;base64,</c> — which is what the Python version's own lookup answers for one.
    /// </remarks>
    internal static async Task<string> AsDataUrlAsync(
        string path,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        var bytes = await BytesAsync(path, client, cancellationToken).ConfigureAwait(false);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"data:{MediaTypeOf(path)};base64,{Convert.ToBase64String(bytes)}");
    }

    /// <summary>Whether a path is an address rather than a file on this machine.</summary>
    /// <param name="path">The path.</param>
    /// <returns>Whether it starts with <c>http://</c> or <c>https://</c>.</returns>
    internal static bool IsAddress(string path) =>
        path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    /// <summary>The name an address's last path segment carries.</summary>
    /// <param name="address">The address.</param>
    /// <returns>The segment, which is what the prompt's heading says.</returns>
    private static string NameOf(string address) =>
        Uri.TryCreate(address, UriKind.Absolute, out var uri)
            ? Path.GetFileName(uri.AbsolutePath.TrimEnd('/'))
            : Path.GetFileName(address);

    /// <summary>Fetches a text file that is served rather than stored.</summary>
    /// <param name="address">Where it is served.</param>
    /// <param name="client">The client to fetch with.</param>
    /// <param name="cancellationToken">Cancels the fetch.</param>
    /// <returns>Its text.</returns>
    private static async Task<string> ReadTextFromAsync(
        string address,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        using var response = await client
            .GetAsync(address, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>The media type a file's extension names.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The media type, or the empty string when the extension names none.</returns>
    /// <remarks>
    /// The five image extensions this loader accepts are the ones a chat can send, and each has a
    /// type of its own. Anything else is a file that arrived in a conversation from somewhere other
    /// than this field, and the Python version sends it with no type rather than guessing.
    /// </remarks>
    private static string MediaTypeOf(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        foreach (var (candidate, mediaType) in Images)
        {
            if (string.Equals(candidate, extension, StringComparison.Ordinal))
            {
                return mediaType;
            }
        }

        return string.Empty;
    }

    /// <summary>A file's bytes, whether it is stored here or served at an address.</summary>
    /// <param name="path">The file's path, or the address it is served at.</param>
    /// <param name="client">The client to fetch an address with.</param>
    /// <param name="cancellationToken">Cancels the fetch.</param>
    /// <returns>The bytes.</returns>
    internal static async Task<byte[]> BytesAsync(
        string path,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        return IsAddress(path)
            ? await client.GetByteArrayAsync(path, cancellationToken).ConfigureAwait(false)
            : await System.IO.File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
    }
}

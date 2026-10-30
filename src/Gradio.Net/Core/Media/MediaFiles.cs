namespace Gradio.Net;
/// <summary>
/// The two things every media component does with a path: put it in the application's cache, and
/// describe it as the protocol carries it.
/// </summary>
/// <remarks>
/// <para>
/// A file component and an image component differ in what they hand a handler and in which fields
/// they fill, but not in this: a path outside the cache is copied into it, the copy is recorded as
/// the application's temporary file, and the value that goes to the client names the cached file
/// with the address the file endpoint serves it at. Written once because four places do it —
/// preprocess and postprocess, on two components — and a rule copied four times is a rule that
/// drifts.
/// </para>
/// <para>
/// A value that names a location is not cached: the server does not hold it, so the browser fetches
/// it from where it is. The Python version downloads such a file instead; the fetch that would is
/// the file endpoint's own subject (ticket 39).
/// </para>
/// </remarks>
internal static class MediaFiles
{
    /// <summary>The path of a file in the application's cache, copied there unless it is already.</summary>
    /// <param name="path">The path a client sent, or a handler returned.</param>
    /// <param name="folder">The application's cache folder.</param>
    /// <param name="root">The application, so that the copy can be reclaimed; <see langword="null"/> for none.</param>
    /// <returns>The cached path, or <paramref name="path"/> itself when it names a location.</returns>
    internal static string Cached(string path, string folder, Blocks? root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (MediaCache.NamesALocation(path))
        {
            return path;
        }

        var cached = MediaCache.SaveFile(path, folder);
        root?.TrackTempFile(cached);

        return cached;
    }

    /// <summary>A file object describing a path, with the file in the application's cache.</summary>
    /// <param name="file">The file, whose <see cref="FileData.Path"/> may name anything.</param>
    /// <param name="folder">The application's cache folder.</param>
    /// <param name="root">The application, so that the copy can be reclaimed; <see langword="null"/> for none.</param>
    /// <param name="measure">
    /// Whether to fill in the size. A file component reports it; an image component leaves it
    /// unset, which is what the Python version's image postprocess does — the endpoint measures the
    /// bytes when it serves them.
    /// </param>
    /// <param name="keepName">
    /// Whether to fill in the name the file had. A file a handler returned keeps its name; an icon
    /// does not — the Python version's <c>serve_static_file</c> describes the file it was given
    /// with its path and nothing else, because an icon is not an upload and has no name to keep.
    /// </param>
    /// <returns>The same object, with the path, name, size and URL filled in.</returns>
    internal static FileData Describe(FileData file, string folder, Blocks? root, bool measure, bool keepName = true)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Path is not { Length: > 0 } path)
        {
            return file;
        }

        if (MediaCache.NamesALocation(path))
        {
            // Nothing to copy and nothing to measure: the file is somewhere else, and the browser
            // is the one that fetches it.
            file.Url = path;

            return file;
        }

        path = Cached(path, folder, root);

        file.Path = path;
        if (keepName)
        {
            file.OrigName = Path.GetFileName(path);
        }

        if (measure)
        {
            file.Size = new FileInfo(path).Length;
        }

        file.Url = MediaCache.UrlOf(path);

        return file;
    }

    /// <summary>The file a component's value carries, whatever shape the value came in.</summary>
    /// <param name="value">A file object, a path, a URL, the file's bytes, a stream of them, or none.</param>
    /// <param name="folder">The application's cache folder, for the bytes a handler produced.</param>
    /// <param name="name">The name bytes are written under, which is the component's format.</param>
    /// <param name="what">What the component is called, for the refusal's message.</param>
    /// <returns>The file, or <see langword="null"/> for no value.</returns>
    /// <remarks>
    /// Six components accept the same six shapes and differ only in what they call the file they
    /// write, so the shape of "a value that names a file" is decided once, here. It is the same
    /// argument the class makes for itself: a rule copied six times is a rule that drifts.
    /// </remarks>
    /// <exception cref="NotSupportedException">The value is of a shape no component accepts.</exception>
    internal static FileData? From(object? value, string folder, string name, string what) => value switch
    {
        null => null,
        FileData file => file,
        System.Text.Json.Nodes.JsonNode node => FileData.FromJson(node),
        string text => new FileData { Path = text },
        byte[] bytes => new FileData { Path = MediaCache.SaveBytes(bytes, name, folder) },
        Stream stream => new FileData { Path = MediaCache.SaveStream(stream, name, folder) },
        _ => throw new NotSupportedException(
            $"{what} takes a path, a URL, the file's bytes, a stream, or a file object; it was "
            + $"handed a {value.GetType()}."),
    };

    /// <summary>The path a client's value names, copied into the application's cache.</summary>
    /// <param name="value">What the client sent: a file object, a path, or none.</param>
    /// <param name="folder">The application's cache folder.</param>
    /// <param name="root">The application, so that the copy can be reclaimed; <see langword="null"/> for none.</param>
    /// <returns>The cached path, or <see langword="null"/> when the value names no file.</returns>
    /// <remarks>
    /// This is what four components hand a handler, and they hand it for the same reason: a path the
    /// client sent may be in an upload directory that is cleaned up, and a handler that reads it
    /// later would find nothing.
    /// </remarks>
    internal static string? PathOf(System.Text.Json.Nodes.JsonNode? value, string folder, Blocks? root) =>
        FileData.FromJson(value)?.Path is { Length: > 0 } path ? Cached(path, folder, root) : null;

    /// <summary>A file object for something the server does not hold.</summary>
    /// <param name="location">An address the browser fetches itself.</param>
    /// <returns>The file, with the address as both its path and its url.</returns>
    /// <remarks>
    /// The Python version's <c>serve_static_file</c> answers a URL with exactly that pair, and
    /// nothing else filled in: the server never sees the bytes, so it cannot say how big they are
    /// or what they are. The config carries it for a chat's avatar and for an example's icon.
    /// </remarks>
    internal static FileData At(string location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(location);

        return new FileData { Path = location, Url = location };
    }
}

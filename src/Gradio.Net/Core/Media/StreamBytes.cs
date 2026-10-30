using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The bytes of a chunk a handler yielded, whatever shape it came in.
/// </summary>
/// <remarks>
/// A streaming component receives the handler's own value rather than a postprocessed one — the
/// postprocessing is what the stream replaces — so the shapes it may arrive in are the ones a .NET
/// handler can return: a path, a file object, the bytes, or a stream of them.
/// </remarks>
internal static class StreamBytes
{
    /// <summary>Reads a chunk's bytes.</summary>
    /// <param name="value">A path, a file object, the bytes, or a stream of them.</param>
    /// <param name="what">What is being read, for the refusal's message.</param>
    /// <returns>The bytes.</returns>
    /// <exception cref="FileNotFoundException">The path names no file.</exception>
    internal static byte[] Read(object? value, string what) => value switch
    {
        byte[] bytes => bytes,
        Stream stream => ReadStream(stream),
        FileData file when file.Path is { Length: > 0 } path => ReadFile(path, what),
        string path => ReadFile(path, what),
        _ => throw new NotSupportedException(
            $"A streamed {what} chunk is a path, bytes, a stream, or a MediaStreamChunk; it was "
            + $"handed a {value?.GetType().ToString() ?? "null"}."),
    };

    private static byte[] ReadFile(string path, string what)
    {
        if (!System.IO.File.Exists(path))
        {
            throw new FileNotFoundException(
                $"A streamed {what} chunk names a file this server does not hold.",
                path);
        }

        return System.IO.File.ReadAllBytes(path);
    }

    private static byte[] ReadStream(Stream stream)
    {
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);

        return buffer.ToArray();
    }
}

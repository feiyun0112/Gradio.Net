using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// How responses are compressed: which encodings to offer, how hard to work, and which responses to
/// leave alone.
/// </summary>
/// <remarks>
/// <para>
/// The defaults are the Python version's — a 400-byte floor and gzip for a client that asks for it
/// and cannot take brotli — because they were chosen for a page that is mostly text and a first
/// paint that matters more than the last byte.
/// </para>
/// <para>
/// What is not here is the Python version's <c>quality</c>, <c>window</c> and <c>mode</c>. The
/// BCL's <see cref="System.IO.Compression.BrotliStream"/> takes a <c>CompressionLevel</c> rather
/// than a quality, and its <c>Optimal</c> is brotli quality 4 with a 22-bit window — the Python
/// version's own two values — with no mode to set at all. Exposing a knob that only looked like it
/// did something would be worse than not exposing it; the encoder is not reimplemented to gain one.
/// </para>
/// </remarks>
public sealed class BrotliOptions
{
    /// <summary>Responses smaller than this many bytes are sent as they are.</summary> <value>400,
    /// which is the Python version's: below it the framing costs more than the saving.</value>
    /// <remarks>
    /// The Python version's <c>minimum_size</c> is applied to the whole body, because it buffers
    /// until it can tell how big the body is. So is this.
    /// </remarks>
    public int MinimumSize { get; set; } = 400;

    /// <summary>Whether a client that accepts gzip but not brotli is answered with gzip.</summary>
    /// <value><see langword="true"/>, which is the Python version's.</value>
    public bool GzipFallback { get; set; } = true;

    /// <summary>Paths to leave uncompressed, as regular expressions.</summary>
    /// <value>Empty, which compresses every response whose path names a compressible file.</value>
    /// <remarks>
    /// The Python version passes its MCP prefix here. The path rule below would leave those
    /// responses alone anyway — no MCP route names a file — but a host that mounts something else
    /// under the same application can say so.
    /// </remarks>
    public IReadOnlyList<string> ExcludedPaths { get; set; } = [];
}

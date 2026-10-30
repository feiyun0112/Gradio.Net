using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Compresses a response with brotli, or with gzip for a client that cannot take brotli.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>BrotliMiddleware</c>, and its rules are kept because they are what the
/// two implementations are compared on: a response is compressed only when the request's path names
/// a file whose type benefits — HTML, JS, CSS, JSON, Markdown, plain text, CSV, TSV, XML, SVG — and
/// only when the client asked for the encoding. Everything else is passed through: an image is
/// already compressed, a font is compressed already, and an event stream is not a body at all.
/// </para>
/// <para>
/// The path rule is also why an event stream is never held up: its path has no extension, so it
/// never reaches this. What does reach it is a page, a stylesheet, a script, or a file a client
/// asked to download.
/// </para>
/// <para>
/// The body is held only until there is enough of it to decide: the first
/// <see cref="BrotliOptions.MinimumSize"/> bytes are kept in memory, and a response that ends
/// before that is sent as it is. One that grows past it starts compressing there and then, and the
/// rest is compressed as it is written — so a download the size of a disk costs a buffer the size
/// of the floor rather than the size of the download, which is the reason the Python version
/// compresses as it goes too.
/// </para>
/// </remarks>
public sealed class BrotliMiddleware
{
    /// <summary>The extensions whose contents benefit from compression.</summary>
    /// <remarks>
    /// The Python version's whitelist, and it is a whitelist rather than a blacklist on purpose: a
    /// media type this framework has not heard of is passed through, which is the safe answer for
    /// bytes that may already be compressed.
    /// </remarks>
    private static readonly HashSet<string> _compressible = new(StringComparer.OrdinalIgnoreCase)
    {
        ".html", ".htm", ".js", ".css", ".json", ".md", ".txt", ".csv", ".tsv", ".xml", ".svg",
    };

    private readonly BrotliOptions _options;
    private readonly List<Regex> _excluded;

    /// <summary>Creates the middleware.</summary> <param name="options">How to compress, or
    /// <see langword="null"/> for the Python version's defaults.</param>
    public BrotliMiddleware(BrotliOptions? options = null)
    {
        _options = options ?? new BrotliOptions();
        _excluded = [.. _options.ExcludedPaths.Select(pattern => new Regex(pattern, RegexOptions.Compiled))];
    }

    /// <summary>Compresses the response, when its path and the client's headers both allow it.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="next">The rest of the pipeline.</param>
    /// <returns>A task that completes once the response is written.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        if (!WantsCompression(context.Request, out var encoding))
        {
            await next(context).ConfigureAwait(false);

            return;
        }

        var original = context.Response.Body;
        var body = new ResponseCompressor(original, context.Response, encoding, _options);

        context.Response.Body = body;

        try
        {
            await next(context).ConfigureAwait(false);
        }
        finally
        {
            context.Response.Body = original;
        }

        await body.CompleteAsync(context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>Whether a request may be answered compressed, and with what.</summary>
    /// <param name="request">The request.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>Whether to compress at all.</returns>
    private bool WantsCompression(HttpRequest request, out string encoding)
    {
        encoding = string.Empty;

        var path = request.Path.Value ?? string.Empty;

        if (!NamesCompressibleFile(path) || _excluded.Any(pattern => pattern.IsMatch(path)))
        {
            return false;
        }

        var accepted = request.Headers.AcceptEncoding.ToString();

        if (accepted.Contains("br", StringComparison.OrdinalIgnoreCase))
        {
            encoding = "br";

            return true;
        }

        if (_options.GzipFallback && accepted.Contains("gzip", StringComparison.OrdinalIgnoreCase))
        {
            encoding = "gzip";

            return true;
        }

        return false;
    }

    /// <summary>Whether a path names a file whose type benefits from compression.</summary>
    /// <param name="path">The request path.</param>
    /// <returns><see langword="true"/> for one of the whitelisted extensions.</returns>
    /// <remarks>
    /// The extension is taken after the last dot, which is the Python version's rule and is what
    /// makes <c>/gradio_api/file=/tmp/data.csv</c> compressible: the path a download is served at
    /// carries the file's own name.
    /// </remarks>
    private static bool NamesCompressibleFile(string path)
    {
        var dot = path.LastIndexOf('.');

        return dot >= 0 && _compressible.Contains(path[dot..]);
    }

    /// <summary>
    /// The stream the response is written into: it decides, once, whether to compress, and then
    /// either writes the body through or compresses it as it arrives.
    /// </summary>
    /// <remarks>
    /// Three states, each entered once: holding until the body is big enough to be worth
    /// compressing, compressing, and passing through — the last for a response that arrived already
    /// encoded, which is the one case where the answer is somebody else's.
    /// </remarks>
    private sealed class ResponseCompressor : Stream
    {
        private readonly Stream _inner;
        private readonly HttpResponse _response;
        private readonly string _encoding;
        private readonly BrotliOptions _options;
        private readonly MemoryStream _buffer = new();

        private Stream? _compressor;

        internal ResponseCompressor(Stream inner, HttpResponse response, string encoding, BrotliOptions options)
        {
            _inner = inner;
            _response = response;
            _encoding = encoding;
            _options = options;
        }

        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => Write(buffer.AsSpan(offset, count));

        /// <inheritdoc />
        /// <remarks>
        /// Held rather than compressed, and held rather than written through: the response body
        /// refuses synchronous writes, so a caller that writes synchronously to it has already been
        /// refused by the server. What this does is keep the bytes so that
        /// <see cref="CompleteAsync"/> can decide about them.
        /// </remarks>
        public override void Write(ReadOnlySpan<byte> buffer) => _buffer.Write(buffer);

        /// <inheritdoc />
        public override Task WriteAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken) =>
            WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        /// <inheritdoc />
        public override ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = default) =>
            WriteCoreAsync(buffer, cancellationToken);

        /// <summary>Writes a chunk, deciding on the way whether the body is being compressed.</summary>
        /// <param name="buffer">The bytes.</param>
        /// <param name="cancellationToken">Cancels the write.</param>
        /// <returns>A task that completes once the bytes are accepted.</returns>
        private async ValueTask WriteCoreAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            if (_compressor is not null)
            {
                await _compressor.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

                return;
            }

            if (AlreadyEncoded())
            {
                await _inner.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

                return;
            }

            _buffer.Write(buffer.Span);

            if (_buffer.Length >= _options.MinimumSize)
            {
                await StartCompressingAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _compressor?.Flush();
            _inner.Flush();
        }

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken) =>
            _compressor is null
                ? _inner.FlushAsync(cancellationToken)
                : _compressor.FlushAsync(cancellationToken);

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc />
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>Ends the body: writes what was held, or finishes the compressor.</summary>
        /// <param name="cancellationToken">Cancels the write.</param>
        /// <returns>A task that completes once the body is out.</returns>
        /// <remarks>
        /// A response that never reached the floor is written as it is, with its length. That is
        /// the only case in which the length is known — a compressed body is chunked, because its
        /// length is not known until it has been compressed.
        /// </remarks>
        internal async ValueTask CompleteAsync(CancellationToken cancellationToken)
        {
            if (_compressor is not null)
            {
                await _compressor.DisposeAsync().ConfigureAwait(false);

                return;
            }

            if (AlreadyEncoded() || HasNoBody(_response.StatusCode))
            {
                return;
            }

            if (_buffer.Length >= _options.MinimumSize)
            {
                // Everything arrived through the synchronous path, so the decision was left until
                // now. The body is already in memory, which is the price of that path.
                await StartCompressingAsync(cancellationToken).ConfigureAwait(false);

                if (_compressor is not null)
                {
                    await _compressor.DisposeAsync().ConfigureAwait(false);
                }

                return;
            }

            var body = _buffer.ToArray();

            if (body.Length > 0)
            {
                _response.ContentLength = body.Length;

                await _inner.WriteAsync(body, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buffer.Dispose();
                _compressor?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>Whether the response is already encoded, which is not this middleware's to
        /// change.</summary> <returns><see langword="true"/> when something the Python version said
        /// how the body is encoded.</returns>
        /// <remarks>
        /// Asked at the first write rather than up front: the header is set by whatever produces the
        /// body, which has not run yet when this stream is installed.
        /// </remarks>
        private bool AlreadyEncoded() => _response.Headers.ContentEncoding.Count > 0;

        /// <summary>Switches to compressing, and writes what was held through the compressor.</summary>
        /// <param name="cancellationToken">Cancels the write.</param>
        /// <returns>A task that completes once the held bytes are in the compressor.</returns>
        private async ValueTask StartCompressingAsync(CancellationToken cancellationToken)
        {
            _response.Headers.ContentEncoding = _encoding;

            // The length of the compressed body is not known until it has been written, so the
            // response goes out chunked; a stale length would truncate it at the client.
            _response.ContentLength = null;
            _response.Headers.Append("Vary", "Accept-Encoding");

            // `Optimal` is brotli quality 4 with a 22-bit window — the Python version's two values — and gzip
            // at its smallest is what Starlette's fallback uses. Neither is configurable here: the
            // BCL's stream takes a level rather than a quality, and reimplementing the encoder to
            // expose one knob would be a worse trade than saying so.
            _compressor = string.Equals(_encoding, "br", StringComparison.Ordinal)
                ? new BrotliStream(_inner, CompressionLevel.Optimal, leaveOpen: true)
                : new GZipStream(_inner, CompressionLevel.SmallestSize, leaveOpen: true);

            var held = _buffer.ToArray();

            _buffer.SetLength(0);

            await _compressor.WriteAsync(held, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Whether a status has no body to compress.</summary>
        /// <param name="status">The status code.</param>
        /// <returns><see langword="true"/> for the statuses that carry none.</returns>
        private static bool HasNoBody(int status) => status is 204 or 205 or 304;
    }
}

/// <summary>
/// Adds the compression middleware to a pipeline.
/// </summary>
/// <remarks>
/// <see cref="GradioEndpointRouteBuilderExtensions.UseGradio(WebApplication)"/> adds it, so an application served by
/// either entry point has it. It is public because a host that mounts the application inside a
/// bigger pipeline may want it earlier than that — compression composes, and the outermost one wins.
/// </remarks>
public static class BrotliMiddlewareExtensions
{
    /// <summary>Compresses responses with brotli, falling back to gzip.</summary>
    /// <param name="app">The pipeline.</param> <param name="options">How to compress, or
    /// <see langword="null"/> for the Python version's defaults.</param>
    /// <returns><paramref name="app"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> is
    /// <see langword="null"/>.</exception>
    public static IApplicationBuilder UseGradioBrotli(this IApplicationBuilder app, BrotliOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        // One instance for the pipeline, made here rather than resolved per request: what it holds
        // is a configuration and a list of compiled patterns, and compiling them per request would
        // cost more than the compression saves.
        var middleware = new BrotliMiddleware(options);

        return app.Use(middleware.InvokeAsync);
    }
}

using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Gradio.Net.Core;

internal class BrotliResponder
{
    private readonly RequestDelegate _next;
    private readonly BrotliMiddlewareOptions _options;
    private bool _started;
    private bool _contentEncodingSet;
    private readonly MemoryStream _buffer;

    public BrotliResponder(RequestDelegate next, BrotliMiddlewareOptions options)
    {
        _next = next;
        _options = options;
        _started = false;
        _contentEncodingSet = false;
        _buffer = new MemoryStream();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Store the original response body stream
        var originalBody = context.Response.Body;

        try
        {
            // Use a memory stream to capture the response
            using var captureStream = new MemoryStream();
            context.Response.Body = captureStream;

            // Execute the next middleware
            await _next(context);

            // Check if content-encoding is already set
            _contentEncodingSet = context.Response.Headers.ContainsKey("Content-Encoding");

            if (_contentEncodingSet)
            {
                // Don't compress if already encoded
                captureStream.Seek(0, SeekOrigin.Begin);
                await captureStream.CopyToAsync(originalBody);
                return;
            }

            // Get the captured response body
            byte[] body = captureStream.ToArray();

            // Don't compress if body is smaller than minimum size
            if (body.Length < _options.MinimumSize)
            {
                await originalBody.WriteAsync(body);
                return;
            }

            // Apply Brotli compression
            using var compressedStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(compressedStream, GetCompressionLevel(_options.Quality), leaveOpen: true))
            {
                await brotliStream.WriteAsync(body);
                await brotliStream.FlushAsync();
            }

            byte[] compressedBody = compressedStream.ToArray();

            // Update response headers
            context.Response.Headers.Remove("Content-Length");
            context.Response.Headers.ContentEncoding = "br";
            context.Response.Headers.Append("Vary", "Accept-Encoding");
            context.Response.ContentLength = compressedBody.Length;

            // Write compressed body
            await originalBody.WriteAsync(compressedBody);
        }
        finally
        {
            // Restore the original response body stream
            context.Response.Body = originalBody;
            _buffer.Dispose();
        }
    }

    private CompressionLevel GetCompressionLevel(int quality)
    {
        // Map quality (0-11) to CompressionLevel enum
        // Quality 0-2: Fastest
        // Quality 3-6: Optimal (default)
        // Quality 7-11: SmallestSize
        return quality switch
        {
            <= 2 => CompressionLevel.Fastest,
            >= 7 => CompressionLevel.SmallestSize,
            _ => CompressionLevel.Optimal
        };
    }
}



using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Gradio.Net.Core;

public class BrotliMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BrotliMiddlewareOptions _options;
    private readonly List<Regex> _excludedHandlers;

    public BrotliMiddleware(RequestDelegate next, BrotliMiddlewareOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _excludedHandlers = options.ExcludedHandlers?
            .Select(path => new Regex(path))
            .ToList() ?? new List<Regex>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the handler should be excluded
        if (IsHandlerExcluded(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Only process HTTP requests with compressible file types
        if (!IsCompressibleFileType(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Check Accept-Encoding header
        string acceptEncoding = context.Request.Headers.AcceptEncoding.ToString();

        if (acceptEncoding.Contains("br", StringComparison.OrdinalIgnoreCase))
        {
            // Apply Brotli compression
            var responder = new BrotliResponder(_next, _options);
            await responder.InvokeAsync(context);
            return;
        }

        if (_options.GzipFallback && acceptEncoding.Contains("gzip", StringComparison.OrdinalIgnoreCase))
        {
            // Fall back to gzip compression (handled by standard ASP.NET Core middleware)
            await _next(context);
            return;
        }

        // No compression
        await _next(context);
    }

    private bool IsHandlerExcluded(string path)
    {
        return _excludedHandlers.Any(pattern => pattern.IsMatch(path));
    }

    private bool IsCompressibleFileType(string path)
    {
        var compressibleExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".html",
                ".htm",
                ".js",
                ".css",
                ".json",
                ".md",
                ".txt",
                ".csv",
                ".tsv",
                ".xml",
                ".svg"
            };

        if (path.Contains('.'))
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            return compressibleExtensions.Contains(extension);
        }

        return false;
    }
}


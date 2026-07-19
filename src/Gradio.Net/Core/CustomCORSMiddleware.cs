using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net.Core;


public class CustomCORSMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _strictCors;

    public CustomCORSMiddleware(RequestDelegate next, bool strictCors = true)
    {
        _next = next;
        _strictCors = strictCors;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            context.Response.Headers.Append("Access-Control-Max-Age", "86400");
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");

            var origin = context.Request.Headers.TryGetValue("Origin", out var origins) ? origins.FirstOrDefault() : null;
            if (origin != null && IsValidOrigin(context.Request, origin))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
            }

            context.Response.StatusCode = 204;
            return;
        }

        var requestOrigin = context.Request.Headers.TryGetValue("Origin", out var requestOrigins) ? requestOrigins.FirstOrDefault() : null;
        if (requestOrigin != null && IsValidOrigin(context.Request, requestOrigin))
        {
            context.Response.Headers.Append("Access-Control-Allow-Origin", requestOrigin);
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        }

        await _next(context);
    }

    private bool IsValidOrigin(HttpRequest request, string origin)
    {
        var localhostAliases = new HashSet<string> { "localhost", "127.0.0.1", "0.0.0.0" };

        if (!_strictCors || Environment.GetEnvironmentVariable("GRADIO_LOCAL_DEV_MODE") != null)
        {
            localhostAliases.Add("null");
        }

        var host = request.Host.Host;
        var originHost = RouteUtils.GetHostname(origin);

        return !localhostAliases.Contains(host) || localhostAliases.Contains(originHost);
    }
}


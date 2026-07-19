using Microsoft.AspNetCore.Builder;

namespace Gradio.Net.Core;

public static class BrotliMiddlewareExtensions
{
    public static IApplicationBuilder UseBrotliMiddleware(
        this IApplicationBuilder app,
        BrotliMiddlewareOptions? options = null)
    {
        return app.UseMiddleware<BrotliMiddleware>(options ?? new BrotliMiddlewareOptions());
    }

    public static IApplicationBuilder UseBrotliMiddleware(
        this IApplicationBuilder app,
        Action<BrotliMiddlewareOptions> configure)
    {
        var options = new BrotliMiddlewareOptions();
        configure(options);
        return app.UseMiddleware<BrotliMiddleware>(options);
    }
}

using Gradio.Net.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Builder;

public static class GradioServiceExtensions
{
    public static IServiceCollection AddGradio(this IServiceCollection services)
    {
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue;
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        services.AddSingleton<App>(new App());
        services.AddControllers();

        return services;
    }

    public static WebApplication UseGradio(this WebApplication app, Blocks blocks,
        bool? mcpServer = null, bool strictCors = true)
    {
        // Finalize blocks context if needed
        if (!blocks.Exited)
        {
            blocks.Exit();
        }

        // Setup MCP server if requested
        App.SetupMcpServer(blocks, new Dictionary<string, object>(), mcpServer);

        // Configure the App instance
        var appInstance = app.Services.GetRequiredService<App>();
        appInstance.ConfigureApp(blocks);

        // Shutdown hook for SSE endpoints
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            try
            {
                if (!appInstance.StopEvent.IsCancellationRequested)
                {
                    appInstance.StopEvent.Cancel();
                }
            }
            catch
            {
                // Best effort shutdown signal.
            }
        });

        // Middleware
        app.UseCors("AllowAll");
        app.UseMiddleware<BrotliMiddleware>(new BrotliMiddlewareOptions { Quality = 4 });

        // Register all Gradio routes
        App.RegisterRoutes(app, blocks);
        App.RegisterMonitoringRoutes(app, blocks, appInstance);
        App.RegisterProcessRecordingRoute(app, blocks, appInstance);
        App.RegisterVibeEditorRoutes(app, blocks, appInstance);

        return app;
    }
}

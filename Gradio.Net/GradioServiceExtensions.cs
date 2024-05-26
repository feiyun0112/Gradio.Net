
using Gradio.Net;
using Gradio.Net.jinja2;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Gradio.Net;
using static System.Runtime.CompilerServices.YieldAwaitable;
using Gradio.Net.Models;


namespace Microsoft.AspNetCore.Builder
{
    public static class GradioServiceExtensions
    {
        
        public static IServiceCollection AddGradio(this IServiceCollection services)
        {
            services.AddSingleton<App>(new App());
            services.AddHostedService<EventWorker>();
            return services;
        }

        public static WebApplication UseGradio(this WebApplication webApplication
        , Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null)
        {
            GradioServiceConfig gradioServiceConfig = new GradioServiceConfig();
            additionalConfigurationAction?.Invoke(gradioServiceConfig);

            var app = webApplication.Services.GetRequiredService<App>();

            app.SetConfig(gradioServiceConfig);

            var fileProvider = new StaticFileProvider(@"templates/frontend");
            webApplication.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider
            });

            webApplication.MapGet("/", (HttpRequest request, [FromServices] App app) =>
            {
                var template = new Template(new StreamReader(fileProvider.GetFileInfo("index.html").CreateReadStream()).ReadToEnd());
                return Results.Content(template.Render(new Dictionary<string, object>() { { "config", app.GetConfig(request) } }), "text/html");
            });

            webApplication.MapGet("/config", (HttpRequest request, [FromServices] App app) =>
            {
                return app.GetConfig(request);
            });

            webApplication.MapGet("/info", (HttpRequest request, [FromServices] App app) =>
            {
                return app.GetApiInfo(request);
            });

            webApplication.MapPost("/queue/join", async (PredictBodyIn body, [FromServices] App app) =>
            {
                return await app.QueueJoin(body);
            });

            webApplication.MapGet("/queue/data", async (HttpContext context, CancellationToken stoppingToken) =>
                {
                    context.Response.Headers.Add("Content-Type", "text/event-stream");


                    StreamWriter streamWriter = new StreamWriter(context.Response.Body);
                    await foreach (var message in app.QueueData(context.Request.Query["session_hash"].FirstOrDefault(), stoppingToken))
                    {
                        await streamWriter.WriteLineAsync(message.ProcessMsg());
                        await streamWriter.FlushAsync();
                    }
                    await streamWriter.WriteLineAsync(new CloseStreamMessage().ProcessMsg());
                    await streamWriter.FlushAsync();

                });

            return webApplication;
        }

        private static Dictionary<string, object> GetConfig(HttpRequest request)
        {
            var config = new Dictionary<string, object>();
            return config;
        }

        private static string GetTemplate(string rootPath)
        {
            var file = Path.Combine(rootPath, @"templates\frontend\index.html");

            return File.ReadAllText(file);
        }
    }
}

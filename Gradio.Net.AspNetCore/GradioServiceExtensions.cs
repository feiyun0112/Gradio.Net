
using Gradio.Net;
using Gradio.Net.jinja2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Gradio.Net.Models;
using Gradio.Net.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

public static class GradioServiceExtensions
{
    internal static string GetRootUrl(this HttpRequest httpRequest)
    {
        return $"{httpRequest.Scheme}://{httpRequest.Host}";
    }

    public static IServiceCollection AddGradio(this IServiceCollection services)
    {
        services.AddSingleton<GradioApp>();
        services.AddHostedService<EventWorker>();
        return services;
    }

    public static WebApplication UseGradio(this WebApplication webApplication
    , Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null)
    {
        webApplication.UseMiddleware<FileMiddleware>();

        GradioServiceConfig gradioServiceConfig = new();
        additionalConfigurationAction?.Invoke(gradioServiceConfig);

        GradioApp gradioApp = webApplication.Services.GetRequiredService<GradioApp>();

        gradioApp.SetConfig(gradioServiceConfig);

        StaticFileProvider fileProvider = new(gradioApp);
        webApplication.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider
        });

        webApplication.MapGet("/", (HttpRequest request, [FromServices] GradioApp app) =>
        {
            Template template = new(new StreamReader(fileProvider.GetFileInfo("index.html").CreateReadStream()).ReadToEnd());
            return Results.Content(template.Render(new Dictionary<string, object>() { { "config", app.GetConfig(request.GetRootUrl()) } }), "text/html");
        });

        webApplication.MapGet("/config", (HttpRequest request, [FromServices] GradioApp app) =>
        {
            return app.GetConfig(request.GetRootUrl());
        });

        webApplication.MapGet("/info", (HttpRequest request, [FromServices] GradioApp app) =>
        {
            return app.GetApiInfo();
        });

        webApplication.MapPost("/queue/join", async (HttpRequest request, PredictBodyIn body, [FromServices] GradioApp app) =>
        {
            return await app.QueueJoin(request.GetRootUrl(), body);
        });

        webApplication.MapGet("/queue/data", async ([FromServices] GradioApp app, HttpContext context, CancellationToken stoppingToken) =>
        {
            context.Response.Headers.Add("Content-Type", "text/event-stream");


            StreamWriter streamWriter = new(context.Response.Body);
            await foreach (SSEMessage message in app.QueueData(context.Request.Query["session_hash"].FirstOrDefault(), stoppingToken))
            {
                await streamWriter.WriteLineAsync(message.ProcessMsg());
                await streamWriter.FlushAsync();
            }
            await streamWriter.WriteLineAsync(new CloseStreamMessage().ProcessMsg());
            await streamWriter.FlushAsync();

        });

        webApplication.MapPost("/upload", async (HttpRequest request, [FromServices] GradioApp app) =>
        {
            string? uploadId = request.Query["upload_id"].First();
            IFormFileCollection files = request.Form.Files;
            return await app.Upload(uploadId, files.Select(x => (x.OpenReadStream(), x.FileName)).ToList());
        });

        webApplication.MapGet("/upload_progress", async ([FromServices] GradioApp app, HttpContext context, CancellationToken stoppingToken) =>
        {
            context.Response.Headers.Add("Content-Type", "text/event-stream");

            StreamWriter streamWriter = new(context.Response.Body);

            await streamWriter.WriteLineAsync(new CloseStreamMessage().ProcessMsg());
            await streamWriter.FlushAsync();
        });

        webApplication.MapGet("/file", ([FromServices] GradioApp app, HttpContext context) =>
        {
            return context.Request.Path;
        });

        return webApplication;
    }
}

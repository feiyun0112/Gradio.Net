
using Gradio.Net;
using Gradio.Net.jinja2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Gradio.Net.Models;
using Gradio.Net.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Builder;

public static class GradioServiceExtensions
{
    internal static string GetRootUrl(this HttpRequest httpRequest)
    {
        return $"{httpRequest.Scheme}://{httpRequest.Host}";
    }

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
            x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });

        services.AddSingleton<GradioApp>(new GradioApp());
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
            return app.GetConfig(request.GetRootUrl(), false);
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
            context.Response.Headers.Append("Content-Type", "text/event-stream");


            StreamWriter streamWriter = new(context.Response.Body);
            var sessionHash = context.Request.Query["session_hash"].FirstOrDefault();
            await foreach (SSEMessage message in app.QueueData(sessionHash, stoppingToken))
            {
                await streamWriter.WriteLineAsync(message.ProcessMsg());
                await streamWriter.FlushAsync();
            }
            await streamWriter.WriteLineAsync(new CloseStreamMessage().ProcessMsg());
            await streamWriter.FlushAsync();
            app.CloseSession(sessionHash);
        });

        webApplication.MapPost("/upload", async (HttpRequest request, [FromServices] GradioApp app) =>
        {
            string? uploadId = request.Query["upload_id"].First();
            IFormFileCollection files = request.Form.Files;
            return await app.Upload(uploadId, files.Select(x => (x.OpenReadStream(), x.FileName)).ToList());
        });

        webApplication.MapGet("/upload_progress", async ([FromServices] GradioApp app, HttpContext context, CancellationToken stoppingToken) =>
        {
            context.Response.Headers.Append("Content-Type", "text/event-stream");

            StreamWriter streamWriter = new(context.Response.Body);

            await streamWriter.WriteLineAsync(new DoneMessage().ProcessMsg());
            await streamWriter.FlushAsync();
        });

        webApplication.MapGet("/file", ([FromServices] GradioApp app, HttpContext context) =>
        {
            return context.Request.Path;
        });

        return webApplication;
    }
}


using Gradio.Net;
using Gradio.Net.jinja2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Gradio.Net.Models;
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
            x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });

        services.AddSingleton<App>(new App());
        services.AddHostedService<EventWorker>();
        return services;
    }

    public static WebApplication UseGradio(this WebApplication webApplication
    , Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null)
    {
        webApplication.UseMiddleware<FileMiddleware>();

        GradioServiceConfig gradioServiceConfig = new();
        additionalConfigurationAction?.Invoke(gradioServiceConfig);

        App app = webApplication.Services.GetRequiredService<App>();

        app.SetConfig(gradioServiceConfig);
               

        StaticFileProvider fileProvider = new(@"templates/frontend");
        webApplication.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider
        });

        webApplication.MapGet("/", (HttpRequest request, [FromServices] App app) =>
        {
            Template template = new(new StreamReader(fileProvider.GetFileInfo("index.html").CreateReadStream()).ReadToEnd());
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

        webApplication.MapPost("/queue/join", async (HttpRequest request, PredictBodyIn body, [FromServices] App app) =>
        {
            return await app.QueueJoin(request, body);
        });

        webApplication.MapGet("/queue/data", async ([FromServices] App app,HttpContext context, CancellationToken stoppingToken) =>
            {
                context.Response.Headers.Add("Content-Type", "text/event-stream");


                StreamWriter streamWriter = new(context.Response.Body);
                var sessionHash = context.Request.Query["session_hash"].FirstOrDefault();
                await foreach (SSEMessage message in app.QueueData(sessionHash, stoppingToken))
                {
                    await streamWriter.WriteLineAsync(message.ProcessMsg());
                    await streamWriter.FlushAsync();
                }
                await streamWriter.WriteLineAsync(new CloseStreamMessage().ProcessMsg());
                await streamWriter.FlushAsync();
                Context.PendingEventIdsSession.TryRemove(sessionHash, out _);
            });

        webApplication.MapPost("/upload", async (HttpRequest request,  [FromServices] App app) =>
        {
            string? uploadId = request.Query["upload_id"].First();
            IFormFileCollection files = request.Form.Files;
            return await app.Upload(uploadId, files);
        });

        webApplication.MapGet("/upload_progress", async ([FromServices] App app, HttpContext context, CancellationToken stoppingToken) =>
        {
            context.Response.Headers.Add("Content-Type", "text/event-stream");
            
            StreamWriter streamWriter = new(context.Response.Body);

            await streamWriter.WriteLineAsync(new DoneMessage().ProcessMsg());
            await streamWriter.FlushAsync();
        });

       

        webApplication.MapGet("/file", async ([FromServices] App app, HttpContext context) =>
        {
            return context.Request.Path;
            //await app.GetUploadedFile(pathOrUrl);
        });

        return webApplication;
    }

    private static Dictionary<string, object> GetConfig(HttpRequest request)
    {
        Dictionary<string, object> config = [];
        return config;
    }

    private static string GetTemplate(string rootPath)
    {
        string file = Path.Combine(rootPath, @"templates\frontend\index.html");

        return File.ReadAllText(file);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

namespace Gradio.Net.Server;

public class GradioServer
{
    private readonly IHost _host;
    private readonly ConcurrentDictionary<string, Core.Blocks> _apps = new();
    private readonly string _mountPath;
    private readonly string _templatesPath;
    private readonly Assembly _assembly;

    public GradioServer(
        Core.Blocks app,
        GradioServerOptions? options = null)
    {
        options ??= new GradioServerOptions();

        App = app;
        ServerName = options.ServerName;
        ServerPort = options.ServerPort;
        Share = options.Share;
        Debug = options.Debug;
        Auth = options.Auth;
        AuthMessage = options.AuthMessage;
        ShowError = options.ShowError;
        Height = options.Height;
        Width = options.Width;
        FaviconPath = options.FaviconPath;
        SslVerify = options.SslVerify;
        Quiet = options.Quiet;
        AllowedPaths = options.AllowedPaths;
        BlockedPaths = options.BlockedPaths;
        RootPath = options.RootPath;
        StateSessionCapacity = options.StateSessionCapacity;
        Pwa = options.Pwa;
        McpServer = options.McpServer;

        _mountPath = RootPath ?? "/";

        // Get the assembly for embedded resources
        _assembly = typeof(GradioServer).Assembly;

        // Templates are embedded resources, so we don't need a physical path
        _templatesPath = string.Empty;

        // Create ASP.NET Core host
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                var actualServerName = ServerName ?? "127.0.0.1";
                webBuilder.UseUrls($"http://{actualServerName}:{ServerPort}");
                webBuilder.ConfigureServices(ConfigureServices);
                webBuilder.Configure(Configure);
            })
            .Build();

        // Register the app
        _apps[app.AppId.ToString()] = app;
    }

    public Core.Blocks App { get; }
    public string? ServerName { get; }
    public int? ServerPort { get; }
    public bool Share { get; }
    public bool Debug { get; }
    public object? Auth { get; }
    public string? AuthMessage { get; }
    public int? MaxThreads { get; }
    public bool EnableQueue { get; }
    public bool ShowError { get; }
    public int? Height { get; }
    public int? Width { get; }
    public string? FaviconPath { get; }
    public bool SslVerify { get; }
    public bool Quiet { get; }
    public List<string>? AllowedPaths { get; }
    public List<string>? BlockedPaths { get; }
    public string? RootPath { get; }
    public int StateSessionCapacity { get; }
    public bool Pwa { get; }
    public bool McpServer { get; }
    public bool IsRunning { get; private set; }
    public string? CustomMountPath { get; }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Configure JSON options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
    }

    private void Configure(IApplicationBuilder app)
    {
        app.UseCors("AllowAll");
        app.UseRouting();

        // Serve static files from embedded resources
        // Ensure RequestPath doesn't end with a slash
        var requestPath = _mountPath == "/" ? string.Empty : _mountPath;
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(_assembly, "Gradio.Net.Templates.frontend"),
            RequestPath = requestPath
        });

        app.UseEndpoints(endpoints =>
        {
            // API endpoints
            endpoints.MapPost($"{_mountPath}api/predict", HandlePredict);
            endpoints.MapGet($"{_mountPath}api/config", HandleConfig);
            endpoints.MapGet($"{_mountPath}config", HandleConfig);
            endpoints.MapPost($"{_mountPath}api/stream", HandleStream);
            endpoints.MapGet($"{_mountPath}api/cancel", HandleCancel);

            // Theme CSS endpoint
            endpoints.MapGet($"{_mountPath}theme.css", HandleThemeCss);

            // File serving
            endpoints.MapGet($"{_mountPath}file/{{*filePath}}", HandleFile);

            // Manifest file
            endpoints.MapGet($"{_mountPath}manifest.json", HandleManifest);

            // Main page
            endpoints.MapGet(_mountPath, HandleIndex);
            endpoints.MapGet($"{_mountPath}index.html", HandleIndex);
        });
    }

    public async Task StartAsync()
    {
        if (!IsRunning)
        {
            await _host.StartAsync();
            IsRunning = true;
            App.IsRunning = true;
            App.LocalUrl = $"http://{ServerName}:{ServerPort}{_mountPath}";
        }
    }

    public async Task WaitForShutdownAsync()
    {
        await _host.WaitForShutdownAsync();
    }

    public async Task StopAsync()
    {
        if (IsRunning)
        {
            await _host.StopAsync();
            IsRunning = false;
            App.IsRunning = false;
        }
    }

    private async Task HandlePredict(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        try
        {
            // Get the app instance
            var app = App;

            if (app.Predict == null || app.InputComponents == null || app.OutputComponents == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "App not properly configured" }));
                return;
            }

            // Read and parse request body
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (requestData == null || !requestData.TryGetValue("data", out var dataObj))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Invalid request format" }));
                return;
            }

            var dataElement = (JsonElement)dataObj;
            var inputs = new List<object>();

            // Preprocess inputs
            for (int i = 0; i < dataElement.GetArrayLength() && i < app.InputComponents.Count; i++)
            {
                var component = app.InputComponents[i] as Components.Component;
                if (component != null)
                {
                    var inputData = dataElement[i].ToString();
                    var processedInput = component.Preprocess(inputData);
                    inputs.Add(processedInput);
                }
            }

            // Call the predict function
            if (app.Predict is Delegate predictMethod)
            {
                var result = predictMethod.DynamicInvoke(inputs.ToArray());

                // Postprocess outputs
                var outputs = new List<object>();
                if (app.OutputComponents.Count == 1)
                {
                    // Single output
                    var component = app.OutputComponents[0] as Components.Component;
                    if (component != null)
                    {
                        var processedOutput = component.Postprocess(result);
                        outputs.Add(processedOutput);
                    }
                }
                else if (result is IEnumerable<object> resultList)
                {
                    // Multiple outputs
                    int i = 0;
                    foreach (var output in resultList)
                    {
                        if (i < app.OutputComponents.Count)
                        {
                            var component = app.OutputComponents[i] as Components.Component;
                            if (component != null)
                            {
                                var processedOutput = component.Postprocess(output);
                                outputs.Add(processedOutput);
                            }
                        }
                        i++;
                    }
                }
                else if (result != null)
                {
                    // Handle case where result is a single value but multiple outputs are expected
                    for (int i = 0; i < app.OutputComponents.Count; i++)
                    {
                        var component = app.OutputComponents[i] as Components.Component;
                        if (component != null)
                        {
                            var processedOutput = component.Postprocess(result);
                            outputs.Add(processedOutput);
                        }
                    }
                }

                // Return the result
                var response = new
                {
                    success = true,
                    data = outputs,
                    message = string.Empty
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Predict function not found" }));
                return;
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message, traceback = ex.StackTrace }));
        }
    }

    private async Task HandleConfig(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var config = App.DefaultConfig.GetConfig();
        var json = JsonSerializer.Serialize(config);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleStream(HttpContext context)
    {
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers["Cache-Control"] = "no-cache";
        context.Response.Headers["Connection"] = "keep-alive";

        // Implementation for stream endpoint
        await context.Response.WriteAsync("data: {\"message\": \"Stream endpoint not implemented\"}\n\n");
        await context.Response.Body.FlushAsync();
    }

    private async Task HandleCancel(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var response = new { success = true, message = "Cancel endpoint not implemented" };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleFile(HttpContext context)
    {
        var filePath = context.Request.RouteValues["filePath"]?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "File path is required" }));
            return;
        }

        // Sanitize file path to prevent directory traversal
        var safeFilePath = Path.GetFileName(filePath);
        var fullPath = Path.Combine(App.UploadFileSet.ToList()[0], safeFilePath);

        if (System.IO.File.Exists(fullPath))
        {
            var fileInfo = new FileInfo(fullPath);
            context.Response.ContentType = GetContentType(fileInfo.Extension);
            await context.Response.SendFileAsync(fullPath);
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "File not found" }));
        }
    }

    private async Task HandleIndex(HttpContext context)
    {
        context.Response.ContentType = "text/html";

        const string resourceName = "Gradio.Net.Templates.frontend.index.html";
        using var stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource {resourceName} not found.");
        }

        using var reader = new StreamReader(stream);
        var htmlContent = await reader.ReadToEndAsync();

        // Get the app config and serialize to JSON
        var config = App.DefaultConfig.GetConfig();
        var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // Create API info object with the correct structure that frontend expects
        var apiInfoDict = new Dictionary<string, object>
            {
                { "named_endpoints", new Dictionary<string, object>() },
                { "unnamed_endpoints", new Dictionary<string, object>() },
                { "/api/predict", new Dictionary<string, object> { { "inputs", new List<object>() }, { "outputs", new List<object>() } } },
                { "/api/config", new Dictionary<string, object>() },
                { "/api/stream", new Dictionary<string, object>() },
                { "/api/cancel", new Dictionary<string, object>() }
            };

        var apiInfoJson = JsonSerializer.Serialize(apiInfoDict, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // Create a context for template rendering
        var templateContext = new Dictionary<string, object>
            {
                { "config", config },
                { "gradio_api_info", apiInfoDict }
            };

        // Create a template engine instance
        var templateEngine = new Core.TemplateEngine();

        // Render the HTML content with the template engine
        htmlContent = templateEngine.Render(htmlContent, templateContext);

        await context.Response.WriteAsync(htmlContent);
    }

    private async Task HandleThemeCss(HttpContext context)
    {
        context.Response.ContentType = "text/css";

        string themeCss = string.Empty;
        if (App.Theme != null)
        {
            // Check if Theme has GenerateCss method
            var generateCssMethod = App.Theme.GetType().GetMethod("GenerateCss");
            if (generateCssMethod != null)
            {
                themeCss = generateCssMethod.Invoke(App.Theme, null) as string ?? string.Empty;
            }
        }

        await context.Response.WriteAsync(themeCss);
    }

    private async Task HandleManifest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        // Check if manifest.json exists as an embedded resource
        const string resourceName = "Gradio.Net.Templates.frontend.manifest.json";
        using var stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream != null)
        {
            // If the manifest.json exists in embedded resources, return it
            using var reader = new StreamReader(stream);
            var manifestContent = await reader.ReadToEndAsync();
            await context.Response.WriteAsync(manifestContent);
        }
        else
        {
            // If manifest.json doesn't exist, return a minimal manifest
            var minimalManifest = new
            {
                name = "Gradio.NET App",
                short_name = "GradioApp",
                description = "A Gradio.NET application",
                start_url = "/",
                display = "standalone",
                background_color = "#ffffff",
                theme_color = "#000000"
            };

            var manifestJson = JsonSerializer.Serialize(minimalManifest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(manifestJson);
        }
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}

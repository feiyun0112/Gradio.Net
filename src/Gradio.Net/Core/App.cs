using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.StaticFiles;
using Gradio.Net.Events;
using Gradio.Net.Data;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Gradio.Net.Utils;
using Gradio.Net.Core.Tunneling;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.I18n;
using Gradio.Net.Core.Queueing;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Gradio.Net.Core
{
    public class App
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new FileExtensionContentTypeProvider();
        private static readonly HashSet<string> XssSafeMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp",
            "audio/mpeg",
            "audio/wav",
            "audio/ogg",
            "video/mp4",
            "video/webm",
            "video/ogg",
            "text/plain",
            "application/json"
        };

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        // No naming policy for MCP responses — property names must match the MCP spec exactly (camelCase).
        private static readonly JsonSerializerOptions McpJsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly TemplateEngine TemplateEngine = new TemplateEngine();
        private static readonly FileUploadProgress FileUploadStatuses = new FileUploadProgress();

        public static int? AppPort { get; set; }

        public Dictionary<string, string> Tokens { get; set; }
        public object Auth { get; set; }
        public string AnalyticsKey { get; set; }
        public bool MonitoringEnabled { get; set; }
        public Blocks Blocks { get; set; }
        public StateHolder StateHolder { get; set; }
        public Dictionary<string, object> Iterators { get; set; }
        public HashSet<string> IteratorsToReset { get; set; }
        public SemaphoreSlim Lock { get; set; }
        public CancellationTokenSource StopEvent { get; set; }
        public string CookieId { get; set; }
        public string QueueToken { get; set; }
        public bool StartupEventsTriggered { get; set; }
        public string UploadedFileDir { get; set; }
        public int ChangeCount { get; set; }
        public string ChangeType { get; set; }
        public string ReloadErrorMessage { get; set; }
        public List<Task> AsyncioTasks { get; set; }
        public Func<HttpRequest, string> AuthDependency { get; set; }
        public Dictionary<string, object> ApiInfo { get; set; }
        public Dictionary<string, object> AllAppInfo { get; set; }
        public Dictionary<string, string> CustomComponentHashes { get; set; }

        // Queue manager
        public QueueManager QueueManager { get; set; }

        public static HttpClient Client { get; } = new HttpClient();

        public static string ToOrJson(object value)
        {
            string json = JsonSerializer.Serialize(value, JsonOptions);
            return json
                .Replace("<", "\\u003c")
                .Replace(">", "\\u003e")
                .Replace("&", "\\u0026")
                .Replace("'", "\\u0027");
        }

        private static string GenerateRandomToken(int length = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GetUploadFolder()
        {
            string tempDir = Path.GetTempPath();
            string uploadDir = Path.Combine(tempDir, "gradio", "uploads");
            Directory.CreateDirectory(uploadDir);
            return uploadDir;
        }

        public App(Func<HttpRequest, string> authDependency = null)
        {
            Tokens = new Dictionary<string, string>();
            Auth = null;
            AnalyticsKey = GenerateRandomToken(16);
            MonitoringEnabled = false;
            Blocks = null;
            StateHolder = new StateHolder();
            Iterators = new Dictionary<string, object>();
            IteratorsToReset = new HashSet<string>();
            Lock = new SemaphoreSlim(1, 1);
            StopEvent = new CancellationTokenSource();
            CookieId = GenerateRandomToken(32);
            QueueToken = GenerateRandomToken(32);
            StartupEventsTriggered = false;
            UploadedFileDir = GetUploadFolder();
            ChangeCount = 0;
            ChangeType = null;
            ReloadErrorMessage = null;
            AsyncioTasks = new List<Task>();
            AuthDependency = authDependency;
            ApiInfo = null;
            AllAppInfo = null;
            CustomComponentHashes = new Dictionary<string, string>();

            // Initialize queue manager
            QueueManager = new QueueManager(
                maxThreadCount: 40,
                maxSize: null,
                defaultConcurrencyLimit: 1,
                updateIntervals: 1.0,
                liveUpdates: false
            );
            QueueManager.App = this;
        }

        public static async Task<HttpResponseMessage> ProxyToNode(
            HttpRequest request,
            string serverName,
            int nodePort,
            int pythonPort,
            string scheme = "http",
            string mountedPath = "")
        {
            string fullPath = request.Path.Value;
            if (!string.IsNullOrEmpty(mountedPath))
            {
                fullPath = fullPath.Replace(mountedPath, string.Empty);
            }
            if (request.QueryString.HasValue)
            {
                fullPath += request.QueryString.Value;
            }

            string url = $"{scheme}://{serverName}:{nodePort}{fullPath}";
            string serverUrl = $"{scheme}://{serverName}";

            if (pythonPort > 0)
            {
                serverUrl += $":{pythonPort}";
            }
            if (!string.IsNullOrEmpty(mountedPath))
            {
                serverUrl += mountedPath;
            }

            var headers = new Dictionary<string, StringValues>(request.Headers);
            headers["x-gradio-server"] = serverUrl;
            headers["x-gradio-port"] = pythonPort.ToString();

            if (Environment.GetEnvironmentVariable("GRADIO_LOCAL_DEV_MODE") == "1")
            {
                headers["x-gradio-local-dev-mode"] = "1";
            }

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method),
                RequestUri = new Uri(url),
                Headers = {
                    { "x-gradio-server", serverUrl },
                    { "x-gradio-port", pythonPort.ToString() }
                }
            };

            if (Environment.GetEnvironmentVariable("GRADIO_LOCAL_DEV_MODE") == "1")
            {
                httpRequest.Headers.Add("x-gradio-local-dev-mode", "1");
            }

            return await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        }

        public void ConfigureApp(Blocks blocks)
        {
            Auth = blocks.Auth;
            Blocks = blocks;
            StateHolder.SetBlocks(blocks);

            // Start queue manager
            QueueManager.App = this;
            QueueManager.Start();
        }

        public Blocks GetBlocks()
        {
            if (Blocks == null)
            {
                throw new InvalidOperationException("No Blocks has been configured for this app.");
            }
            return Blocks;
        }

        public HttpRequestMessage BuildProxyRequest(string urlPath)
        {
            if (!Uri.TryCreate(urlPath, UriKind.Absolute, out var url))
            {
                throw new InvalidOperationException("This URL cannot be proxied.");
            }

            if (Blocks == null)
            {
                throw new InvalidOperationException("Blocks is not configured.");
            }

            bool isSafeUrl = Blocks.ProxyUrls.Any(root =>
            {
                if (Uri.TryCreate(root, UriKind.Absolute, out var rootUri))
                {
                    return string.Equals(rootUri.Host, url.Host, StringComparison.OrdinalIgnoreCase);
                }
                return false;
            });

            if (!isSafeUrl)
            {
                throw new InvalidOperationException("This URL cannot be proxied.");
            }

            var headers = new Dictionary<string, string>();
            if (Context.Token != null && url.Host.EndsWith(".hf.space", StringComparison.OrdinalIgnoreCase))
            {
                headers["Authorization"] = $"Bearer {Context.Token}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var kvp in headers)
            {
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }

            return request;
        }

        public static void SetupMcpServer(
            Blocks blocks,
            Dictionary<string, object> appKwargs,
            bool? mcpServer = null)
        {
            string mcpSubpath = RouteUtils.ApiPrefix + "/mcp";

            if (mcpServer == null)
            {
                mcpServer = Environment.GetEnvironmentVariable("GRADIO_MCP_SERVER")?.ToLower() == "true";
            }

            if (mcpServer.Value)
            {
                try
                {
                    // Initialize MCP server
                    blocks.McpServerObj = new Mcp.GradioMcpServer(blocks);
                    blocks.McpServer = true;
                }
                catch (Exception e)
                {
                    blocks.McpServer = false;
                    blocks.McpError = $"Error launching MCP server: {e.Message}";
                }
            }

            // Initialize Config property
            blocks.Config = blocks.GetConfigFile();
        }

        public static WebApplication CreateApp(
            Blocks blocks,
            Dictionary<string, object> appKwargs = null,
            Func<HttpRequest, string> authDependency = null,
            bool strictCors = true,
            bool ssrMode = false,
            bool? mcpServer = null)
        {
            appKwargs = appKwargs ?? new Dictionary<string, object>();

            SetupMcpServer(blocks, appKwargs, mcpServer);

            var builder = WebApplication.CreateBuilder();

            // Configure services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddSingleton<App>(new App(authDependency));
            builder.Services.AddControllers();

            var app = builder.Build();

            // Ensure long-lived SSE endpoints terminate quickly on Ctrl+C / host shutdown.
            try
            {
                var appInstance = app.Services.GetRequiredService<App>();
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
            }
            catch
            {
                // Best effort only.
            }

            // Configure middleware
            app.UseCors("AllowAll");
            app.UseMiddleware<BrotliMiddleware>(new BrotliMiddlewareOptions { Quality = 4 });

            // SSR mode middleware
            // Server-side rendering mode requires Node.js 20+ to be installed
            // and would proxy requests to a Node.js server for rendering
            if (ssrMode)
            {
                // In a full implementation, this would:
                // 1. Start a Node.js server process
                // 2. Set up middleware to proxy non-API requests to the Node server
                // 3. Handle graceful shutdown of the Node process
            }

            // Register routes
            RegisterRoutes(app, blocks);

            // Register additional routes that need App instance (resolved from DI after build)
            var appServiceInstance = app.Services.GetRequiredService<App>();
            RegisterMonitoringRoutes(app, blocks, appServiceInstance);
            RegisterProcessRecordingRoute(app, blocks, appServiceInstance);
            RegisterVibeEditorRoutes(app, blocks, appServiceInstance);

            return app;
        }

        internal static void RegisterRoutes(WebApplication app, Blocks blocks)
        {
            // Static paths for file safety checks
            Utils.Utils.SetStaticPaths(Path.Combine(AppContext.BaseDirectory, "Templates", "frontend", "static"));

            string mainPageTemplate = "Gradio.Net.Templates.frontend.index.html";
            string sharePageTemplate = "Gradio.Net.Templates.frontend.share.html";

            // ========================================
            // MULTI-PAGE APPLICATION SUPPORT
            // ========================================

            // Register routes for each page defined in blocks.Pages
            foreach (var pageTuple in blocks.Pages)
            {
                var page = pageTuple.Item1; // Page path
                var pageName = pageTuple.Item2; // Page name
                var isVisible = pageTuple.Item3; // Is visible

                if (!isVisible || string.IsNullOrEmpty(page))
                {
                    continue; // Skip non-visible pages or empty page paths
                }

                // Register main-page routes for this page
                // Only register one path (no trailing-slash variant) to avoid AmbiguousMatchException
                // with the /{**rawPath} catch-all endpoint.
                var pagePath = $"/{page}";
                app.MapMethods(pagePath, new[] { "GET", "HEAD" }, async (HttpContext context, [FromServices] App appInstance) =>
                {
                    return await HandleMainPage(context, appInstance, page);
                }).WithOrder(-1);
            }

            // Register main page route (empty page path)
            app.MapMethods("/", new[] { "GET", "HEAD" }, async (HttpContext context, [FromServices] App appInstance) =>
            {
                return await HandleMainPage(context, appInstance, "");
            });

            // Authentication routes
            app.MapGet("/api/user", (HttpContext context, [FromServices] App appInstance) =>
            {
                return appInstance.GetCurrentUser(context.Request);
            });

            app.MapGet("/api/login_check", (HttpContext context, [FromServices] App appInstance) =>
            {
                string user = appInstance.GetCurrentUser(context.Request);
                if (appInstance.Auth == null && appInstance.AuthDependency == null || user != null)
                {
                    return Results.Ok();
                }
                return Results.Unauthorized();
            });

            app.MapGet("/api/token", (HttpContext context, [FromServices] App appInstance) =>
            {
                string token = context.Request.Cookies.TryGetValue($"access-token-{appInstance.CookieId}", out string tokenValue) ? tokenValue : null;
                return Results.Json(new { token, user = appInstance.Tokens.TryGetValue(token, out string user) ? user : null });
            });

            app.MapGet("/api/app_id", ([FromServices] App appInstance) =>
            {
                return Results.Json(new { app_id = appInstance.GetBlocks().AppId });
            });

            // Dev reload notification (SSE)
            app.MapGet("/api/dev/reload", async (HttpContext context, [FromServices] App appInstance) =>
            {
                context.Response.Headers.ContentType = "text/event-stream";
                while (!context.RequestAborted.IsCancellationRequested)
                {
                    if (appInstance.ChangeType != null)
                    {
                        var data = JsonSerializer.Serialize(new
                        {
                            change = appInstance.ChangeType,
                            change_count = appInstance.ChangeCount,
                            error = appInstance.ReloadErrorMessage
                        }, JsonOptions);
                        await context.Response.WriteAsync($"data: {data}\n\n");
                        await context.Response.Body.FlushAsync();
                        appInstance.ChangeType = null;
                    }

                    await Task.Delay(500, context.RequestAborted);
                }
            });

            // Login route
            app.MapPost("/login", async (HttpContext context, [FromServices] App appInstance) =>
            {
                var formData = await context.Request.ReadFormAsync();
                string username = formData["username"];
                string password = formData["password"];

                bool isAuthenticated = false;
                if (appInstance.Auth is Dictionary<string, string> authDict)
                {
                    if (authDict.TryGetValue(username, out var correctPassword))
                    {
                        isAuthenticated = RouteUtils.ComparePasswordsSecurely(password, correctPassword);
                    }
                }
                else if (appInstance.Auth is IEnumerable<Tuple<string, string>> authList)
                {
                    foreach (var pair in authList)
                    {
                        if (pair.Item1 == username)
                        {
                            isAuthenticated = RouteUtils.ComparePasswordsSecurely(password, pair.Item2);
                            break;
                        }
                    }
                }

                if (isAuthenticated)
                {
                    string token = GenerateRandomToken(16);
                    appInstance.Tokens[token] = username;

                    // Set cookies directly on the response
                    context.Response.Cookies.Append($"access-token-{appInstance.CookieId}", token, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    context.Response.Cookies.Append($"access-token-unsecure-{appInstance.CookieId}", token, new CookieOptions
                    {
                        HttpOnly = true
                    });

                    return Results.Json(new { success = true });
                }
                else
                {
                    return Results.BadRequest(new { detail = "Incorrect credentials." });
                }
            });

            // Logout route
            app.MapGet("/logout", (HttpContext context, [FromServices] App appInstance) =>
            {
                string user = appInstance.GetCurrentUser(context.Request);
                context.Response.Cookies.Delete($"access-token-{appInstance.CookieId}", new CookieOptions { Path = "/" });
                context.Response.Cookies.Delete($"access-token-unsecure-{appInstance.CookieId}", new CookieOptions { Path = "/" });

                // Delete tokens for the user
                if (user != null)
                {
                    var tokensToDelete = appInstance.Tokens.Where(kv => kv.Value == user).Select(kv => kv.Key).ToList();
                    foreach (var token in tokensToDelete)
                    {
                        appInstance.Tokens.Remove(token);
                    }
                }

                return Results.Redirect("/");
            });

            // API Info route
            app.MapGet("/api/info", (HttpRequest request, [FromServices] App appInstance) =>
            {
                bool allEndpoints = request.Query.TryGetValue("all_endpoints", out var allEndpointsValue) && allEndpointsValue == "true";

                if (allEndpoints)
                {
                    if (appInstance.AllAppInfo == null)
                    {
                        appInstance.AllAppInfo = appInstance.GetBlocks().GetApiInfo(allEndpoints: true);
                    }
                    return Results.Json(appInstance.AllAppInfo);
                }
                else
                {
                    if (appInstance.ApiInfo == null)
                    {
                        var apiInfo = appInstance.GetBlocks().GetApiInfo();
                        apiInfo = RouteUtils.UpdateExampleValuesToUsePublicUrl(apiInfo);
                        var root = RouteUtils.GetRootUrl(request, "/api/info", appInstance.GetBlocks().RootPath ?? appInstance.GetBlocks().CustomMountPath ?? string.Empty);
                        RouteUtils.AddCodeSnippetsToApiInfo(apiInfo, root);
                        appInstance.ApiInfo = apiInfo;
                    }
                    return Results.Json(appInstance.ApiInfo);
                }
            });

            // Python parity: /gradio_api/info (same as /api/info, using API_PREFIX)
            app.MapGet($"{RouteUtils.ApiPrefix}/info", (HttpRequest request, [FromServices] App appInstance) =>
            {
                bool allEndpoints = request.Query.TryGetValue("all_endpoints", out var allEndpointsValue) && allEndpointsValue == "true";

                if (allEndpoints)
                {
                    if (appInstance.AllAppInfo == null)
                    {
                        appInstance.AllAppInfo = appInstance.GetBlocks().GetApiInfo(allEndpoints: true);
                    }
                    return Results.Json(appInstance.AllAppInfo);
                }
                else
                {
                    if (appInstance.ApiInfo == null)
                    {
                        var apiInfo = appInstance.GetBlocks().GetApiInfo();
                        apiInfo = RouteUtils.UpdateExampleValuesToUsePublicUrl(apiInfo);
                        var root = RouteUtils.GetRootUrl(request, $"{RouteUtils.ApiPrefix}/info", appInstance.GetBlocks().RootPath ?? appInstance.GetBlocks().CustomMountPath ?? string.Empty);
                        RouteUtils.AddCodeSnippetsToApiInfo(apiInfo, root);
                        appInstance.ApiInfo = apiInfo;
                    }
                    return Results.Json(appInstance.ApiInfo);
                }
            });

            // Python parity: /gradio_api/mcp/schema
            app.MapGet($"{RouteUtils.ApiPrefix}/mcp/schema", (HttpRequest request) =>
            {
                if (blocks.McpServerObj is Mcp.GradioMcpServer mcpServer)
                {
                    var schemas = mcpServer.GetCompleteSchema(request);
                    return Results.Json(schemas);
                }
                return Results.Json(new List<object>());
            });

            // ─────────────────────────────────────────────────────────────────────
            // MCP Streamable HTTP endpoint  (JSON-RPC 2.0 over HTTP POST)
            // Mirrors the Python StreamableHTTPSessionManager at /gradio_api/mcp/
            // ─────────────────────────────────────────────────────────────────────
            Func<HttpContext, App, Task> mcpStreamableHandler = async (HttpContext context, App appInstance) =>
            {
                // Helper: build a JSON-RPC error envelope
                static object McpErr(object? id, int code, string msg) =>
                    new { jsonrpc = "2.0", error = new { code, message = msg }, id };

                // Helper: recursively convert JsonElement → plain CLR object
                static object? JsonElemToObj(JsonElement el) => el.ValueKind switch
                {
                    JsonValueKind.String => el.GetString(),
                    JsonValueKind.True => (object)true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Number => el.TryGetInt64(out var lv) ? lv : (object)el.GetDouble(),
                    JsonValueKind.Object => el.EnumerateObject()
                                              .ToDictionary(p => p.Name, p => JsonElemToObj(p.Value)),
                    JsonValueKind.Array => el.EnumerateArray()
                                              .Select(i => JsonElemToObj(i))
                                              .ToList<object?>(),
                    _ => (object?)el.ToString()
                };

                context.Response.ContentType = "application/json";

                if (blocks.McpServerObj is not Mcp.GradioMcpServer mcpServer)
                {
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(McpErr(null, -32603, "MCP server not enabled"), McpJsonOptions));
                    return;
                }

                string rawBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                JsonDocument doc;
                try { doc = JsonDocument.Parse(rawBody); }
                catch
                {
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(McpErr(null, -32700, "Parse error"), McpJsonOptions));
                    return;
                }

                using (doc)
                {
                    var root = doc.RootElement;

                    // Extract common JSON-RPC 2.0 fields
                    string? method = root.TryGetProperty("method", out var mEl) ? mEl.GetString() : null;
                    object? id = null;
                    if (root.TryGetProperty("id", out var idEl))
                        id = idEl.ValueKind == JsonValueKind.Number ? (object)idEl.GetInt64()
                           : idEl.ValueKind == JsonValueKind.String ? idEl.GetString()
                           : null;

                    JsonElement? paramsEl = root.TryGetProperty("params", out var pEl) ? pEl : null;
                    bool isNotification = id == null; // no id → MCP notification, no response expected

                    switch (method)
                    {
                        // ── initialize ────────────────────────────────────────────────────
                        case "initialize":
                            {
                                var resp = new
                                {
                                    jsonrpc = "2.0",
                                    result = new
                                    {
                                        protocolVersion = "2024-11-05",
                                        capabilities = new
                                        {
                                            tools = new { },
                                            resources = new { },
                                            prompts = new { }
                                        },
                                        serverInfo = new
                                        {
                                            name = blocks.Title ?? "Gradio App",
                                            version = "1.0"
                                        }
                                    },
                                    id
                                };
                                await context.Response.WriteAsync(
                                    JsonSerializer.Serialize(resp, McpJsonOptions));
                                return;
                            }

                        // ── notifications ────────────────────────────────────────────────
                        case "initialized":
                        case "notifications/initialized":
                        case "notifications/cancelled":
                            context.Response.StatusCode = 202;
                            context.Response.ContentType = null;
                            return;

                        // ── ping ─────────────────────────────────────────────────────────
                        case "ping":
                            if (isNotification) { context.Response.StatusCode = 202; context.Response.ContentType = null; return; }
                            await context.Response.WriteAsync(
                                JsonSerializer.Serialize(new { jsonrpc = "2.0", result = new { }, id }, McpJsonOptions));
                            return;

                        // ── tools/list ────────────────────────────────────────────────────
                        case "tools/list":
                            {
                                var tools = mcpServer.GetToolsList();
                                await context.Response.WriteAsync(
                                    JsonSerializer.Serialize(
                                        new { jsonrpc = "2.0", result = new { tools }, id },
                                        McpJsonOptions));
                                return;
                            }

                        // ── tools/call ────────────────────────────────────────────────────
                        case "tools/call":
                            {
                                if (isNotification) { context.Response.StatusCode = 202; context.Response.ContentType = null; return; }

                                string toolName = "";
                                var callArguments = new Dictionary<string, object?>();

                                if (paramsEl.HasValue)
                                {
                                    if (paramsEl.Value.TryGetProperty("name", out var nEl))
                                        toolName = nEl.GetString() ?? "";
                                    if (paramsEl.Value.TryGetProperty("arguments", out var aEl)
                                        && aEl.ValueKind == JsonValueKind.Object)
                                    {
                                        foreach (var prop in aEl.EnumerateObject())
                                            callArguments[prop.Name] = JsonElemToObj(prop.Value);
                                    }
                                }

                                if (!mcpServer.ToolToEndpoint.TryGetValue(toolName, out var toolEndpointName))
                                {
                                    await context.Response.WriteAsync(
                                        JsonSerializer.Serialize(
                                            McpErr(id, -32602, $"Unknown tool: {toolName}"), McpJsonOptions));
                                    return;
                                }

                                // Build ordered positional args from keyword arguments
                                var orderedArgs = new List<object?>();
                                var apiInfo = mcpServer.GetApiInfo();
                                if (apiInfo.TryGetValue("named_endpoints", out var neObj)
                                    && neObj is Dictionary<string, object> namedEndpoints
                                    && namedEndpoints.TryGetValue(toolEndpointName, out var epObj)
                                    && epObj is Dictionary<string, object> epInfo
                                    && epInfo.TryGetValue("parameters", out var paramsListObj)
                                    && paramsListObj is List<object> paramsList)
                                {
                                    foreach (var paramObj in paramsList)
                                    {
                                        if (paramObj is Dictionary<string, object> param
                                            && param.TryGetValue("parameter_name", out var pnObj))
                                        {
                                            string pn = pnObj?.ToString() ?? "";
                                            if (callArguments.TryGetValue(pn, out var argVal))
                                                orderedArgs.Add(argVal);
                                            else if (param.TryGetValue("parameter_default", out var defVal))
                                                orderedArgs.Add(defVal);
                                            else
                                                orderedArgs.Add(null);
                                        }
                                    }
                                }

                                var predictBody = new Data.PredictBodyInternal
                                {
                                    Data = orderedArgs.Select(a => a!).ToList(),
                                    SessionHash = Guid.NewGuid().ToString("N"),
                                    ApiName = toolEndpointName.TrimStart('/')
                                };

                                BlockFunction fn;
                                try
                                {
                                    fn = RouteUtils.GetFn(appInstance.GetBlocks(), toolEndpointName.TrimStart('/'), predictBody);
                                }
                                catch (Exception ex)
                                {
                                    await context.Response.WriteAsync(
                                        JsonSerializer.Serialize(McpErr(id, -32602, ex.Message), McpJsonOptions));
                                    return;
                                }

                                var rootUrl = RouteUtils.GetRootUrl(
                                    context.Request,
                                    RouteUtils.ApiPrefix + "/mcp",
                                    appInstance.GetBlocks().RootPath ?? "");

                                var grRequest = RouteUtils.CompileGrRequest(predictBody, fn, "mcp", context.Request);

                                try
                                {
                                    var output = await RouteUtils.CallProcessApi(
                                        appInstance, predictBody, grRequest, fn, rootUrl);

                                    List<object> outputData = new List<object>();
                                    if (output.TryGetValue("data", out var dataObj)
                                        && dataObj is List<object> dataList)
                                        outputData = dataList;

                                    var content = mcpServer.PostprocessOutputData(outputData, rootUrl);
                                    await context.Response.WriteAsync(
                                        JsonSerializer.Serialize(
                                            new { jsonrpc = "2.0", result = new { content }, id },
                                            McpJsonOptions));
                                }
                                catch (Exception ex)
                                {
                                    await context.Response.WriteAsync(
                                        JsonSerializer.Serialize(McpErr(id, -32603, ex.Message), McpJsonOptions));
                                }
                                return;
                            }

                        // ── unknown / unsupported ─────────────────────────────────────────
                        default:
                            if (isNotification) { context.Response.StatusCode = 202; context.Response.ContentType = null; return; }
                            await context.Response.WriteAsync(
                                JsonSerializer.Serialize(
                                    McpErr(id, -32601, $"Method not found: {method}"), McpJsonOptions));
                            return;
                    }
                }
            };

            app.MapPost($"{RouteUtils.ApiPrefix}/mcp", async (HttpContext ctx, [FromServices] App ai) =>
            {
                await mcpStreamableHandler(ctx, ai);
                return Results.Empty;
            });
            // Also handle /gradio_api/mcp/http and trailing-slash variants via catch-all
            app.MapPost($"{RouteUtils.ApiPrefix}/mcp/{{*rest}}", async (HttpContext ctx, [FromServices] App ai) =>
            {
                await mcpStreamableHandler(ctx, ai);
                return Results.Empty;
            });

            // OpenAPI Schema route
            app.MapGet("/api/openapi.json", ([FromServices] App appInstance) =>
            {
                var apiInfo = appInstance.GetBlocks().GetApiInfo();

                var schema = new
                {
                    openapi = "3.0.2",
                    info = new
                    {
                        title = appInstance.GetBlocks().Title ?? "Gradio App",
                        description = appInstance.GetBlocks().Description ?? "",
                        version = Utils.Utils.GetPackageVersion()
                    },
                    paths = new Dictionary<string, object>(),
                    components = new { schemas = new Dictionary<string, object>() }
                };

                if (apiInfo.TryGetValue("named_endpoints", out var namedEndpointsObj) && namedEndpointsObj is Dictionary<string, object> namedEndpoints)
                {
                    foreach (var (endpointPath, endpointInfoObj) in namedEndpoints)
                    {
                        if (endpointInfoObj is Dictionary<string, object> endpointInfo)
                        {
                            if (endpointInfo.TryGetValue("api_visibility", out var visibility) && visibility?.ToString() == "private")
                            {
                                continue;
                            }

                            var requestProperties = new Dictionary<string, object>();
                            var responseProperties = new Dictionary<string, object>();

                            if (endpointInfo.TryGetValue("parameters", out var parametersObj) && parametersObj is IEnumerable<object> parameters)
                            {
                                foreach (var paramObj in parameters)
                                {
                                    if (paramObj is Dictionary<string, object> param)
                                    {
                                        var paramName = param["parameter_name"].ToString();
                                        if (param.TryGetValue("type", out var paramTypeObj) && paramTypeObj is Dictionary<string, object> paramType)
                                        {
                                            if (paramType.ContainsKey("additional_description"))
                                            {
                                                paramType.Remove("additional_description");
                                            }
                                            if (paramType.ContainsKey("properties") && !paramType.ContainsKey("type"))
                                            {
                                                paramType["type"] = "object";
                                            }
                                            requestProperties[paramName] = paramType;
                                        }
                                    }
                                }
                            }

                            if (endpointInfo.TryGetValue("returns", out var returnsObj) && returnsObj is IEnumerable<object> returnsList)
                            {
                                int i = 0;
                                foreach (var retObj in returnsList)
                                {
                                    string retName = i == 0 ? "output" : $"output_{i}";
                                    if (retObj is Dictionary<string, object> ret && ret.TryGetValue("type", out var retTypeObj) && retTypeObj is Dictionary<string, object> retType)
                                    {
                                        if (retType.ContainsKey("additional_description"))
                                        {
                                            retType.Remove("additional_description");
                                        }
                                        if (retType.ContainsKey("properties") && !retType.ContainsKey("type"))
                                        {
                                            retType["type"] = "object";
                                        }
                                        responseProperties[retName] = retType;
                                    }
                                    i++;
                                }
                            }

                            var pathItem = new Dictionary<string, object>
                            {
                                ["post"] = new Dictionary<string, object>
                                {
                                    ["summary"] = endpointInfo.TryGetValue("description", out var desc) ? desc : $"Endpoint {endpointPath}",
                                    ["description"] = endpointInfo.TryGetValue("description", out var desc2) ? desc2 : "",
                                    ["operationId"] = endpointPath.Trim('/').Replace("/", "_"),
                                    ["requestBody"] = new Dictionary<string, object>
                                    {
                                        ["required"] = true,
                                        ["content"] = new Dictionary<string, object>
                                        {
                                            ["application/json"] = new Dictionary<string, object>
                                            {
                                                ["schema"] = new Dictionary<string, object>
                                                {
                                                    ["type"] = "object",
                                                    ["properties"] = requestProperties
                                                }
                                            }
                                        }
                                    },
                                    ["responses"] = new Dictionary<string, object>
                                    {
                                        ["200"] = new Dictionary<string, object>
                                        {
                                            ["description"] = "Successful response",
                                            ["content"] = new Dictionary<string, object>
                                            {
                                                ["application/json"] = new Dictionary<string, object>
                                                {
                                                    ["schema"] = new Dictionary<string, object>
                                                    {
                                                        ["type"] = "object",
                                                        ["properties"] = responseProperties
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            };

                            schema.paths[endpointPath] = pathItem;
                        }
                    }
                }

                return Results.Json(schema, JsonOptions);
            });

            // Config route
            app.MapGet("/api/config", (HttpContext context, [FromServices] App appInstance) =>
            {
                var deepLink = context.Request.Query.TryGetValue("deep_link", out var deepLinkValue)
                    ? deepLinkValue.ToString()
                    : string.Empty;
                var updated = BuildUpdatedConfig(context, appInstance, "/api/config", deepLink, "");
                return Results.Json(updated, JsonOptions);
            });

            app.MapGet("/config", (HttpContext context, [FromServices] App appInstance) =>
            {
                var deepLink = context.Request.Query.TryGetValue("deep_link", out var deepLinkValue)
                    ? deepLinkValue.ToString()
                    : string.Empty;
                var updated = BuildUpdatedConfig(context, appInstance, "/config", deepLink, "");
                return Results.Json(updated, JsonOptions);
            });

            // Static resources route
            app.MapGet("/static/{**path}", (string path) =>
            {
                return ServeEmbeddedResource("Gradio.Net.Templates.frontend.static", path);
            });

            // Build resources route
            app.MapGet("/assets/{**path}", (string path) =>
            {
                return ServeEmbeddedResource("Gradio.Net.Templates.frontend.assets", path);
            });

            // Custom component resources route
            app.MapGet("/custom_component/{id}/{environment}/{type}/{fileName}",
                async (HttpContext context, string id, string environment, string type, string fileName, [FromServices] App appInstance) =>
            {
                // Validate environment
                if (environment != "client" && environment != "server")
                {
                    return Results.NotFound();
                }

                try
                {
                    // Get all components from the blocks
                    var blocks = appInstance.GetBlocks();
                    var components = blocks.BlocksDict.Values;

                    // Find the component with matching ID
                    Block component = null;
                    foreach (var comp in components)
                    {
                        // In C#, we'd need to implement GetComponentClassId() method
                        // For now, use a simplified check
                        if (comp.GetType().Name.ToLowerInvariant().Contains(id.ToLowerInvariant()))
                        {
                            component = comp;
                            break;
                        }
                    }

                    if (component == null)
                    {
                        return Results.NotFound();
                    }

                    // Construct the file path
                    // In production, you'd need to implement proper component template directory resolution
                    var componentType = component.GetType();
                    var assemblyLocation = componentType.Assembly.Location;
                    var componentDir = Path.GetDirectoryName(assemblyLocation) ?? string.Empty;

                    // For custom components, look for template files
                    var templateDir = Path.Combine(componentDir, "templates", id);
                    var requestedPath = Path.Combine(type, fileName);
                    var fullPath = Path.Combine(templateDir, requestedPath);

                    // Security check - ensure path is within component directory
                    var normalizedPath = Path.GetFullPath(fullPath);
                    var normalizedTemplateDir = Path.GetFullPath(templateDir);
                    if (!normalizedPath.StartsWith(normalizedTemplateDir, StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.NotFound();
                    }

                    if (!File.Exists(fullPath))
                    {
                        return Results.NotFound();
                    }

                    // Generate ETag based on file content hash
                    var key = $"{id}-{type}-{fileName}";
                    if (!appInstance.CustomComponentHashes.TryGetValue(key, out var etag))
                    {
                        var fileContent = await File.ReadAllTextAsync(fullPath);
                        using var sha256 = System.Security.Cryptography.SHA256.Create();
                        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fileContent));
                        etag = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                        appInstance.CustomComponentHashes[key] = etag;
                    }

                    // Set cache headers
                    var headers = new Dictionary<string, string>
                    {
                        ["Cache-Control"] = "max-age=0, must-revalidate",
                        ["ETag"] = etag
                    };

                    // Check if client has cached version
                    var ifNoneMatch = context.Request.Headers["If-None-Match"].ToString();
                    if (!string.IsNullOrEmpty(ifNoneMatch) && ifNoneMatch == etag)
                    {
                        context.Response.StatusCode = 304; // Not Modified
                        foreach (var header in headers)
                        {
                            context.Response.Headers[header.Key] = header.Value;
                        }
                        return Results.Empty;
                    }

                    // Return the file with headers
                    foreach (var header in headers)
                    {
                        context.Response.Headers[header.Key] = header.Value;
                    }

                    var contentType = ContentTypeProvider.TryGetContentType(fullPath, out var ct) ? ct : "application/octet-stream";
                    return Results.File(fullPath, contentType);
                }
                catch (Exception)
                {
                    return Results.NotFound();
                }
            });

            // Favicon route
            app.MapGet("/favicon.ico", ([FromServices] App appInstance) =>
            {
                var blocks = appInstance.GetBlocks();
                if (blocks.FaviconPath == null)
                {
                    return ServeEmbeddedResource("Gradio.Net.Templates.frontend.static", "img/logo.svg");
                }
                else
                {
                    return Results.File(blocks.FaviconPath);
                }
            });

            // Svelte assets
            app.MapGet("/svelte/{**path}", (string path) =>
            {
                return ServeEmbeddedResource("Gradio.Net.Templates.frontend.assets.svelte", path);
            });

            // Reverse proxy
            app.MapMethods("/api/proxy={urlPath}", new[] { "GET", "HEAD" }, async (HttpContext context) =>
            {
                string urlPath = context.Request.RouteValues.TryGetValue("urlPath", out var urlValue) ? urlValue?.ToString() ?? string.Empty : string.Empty;
                var appInstance = context.RequestServices.GetRequiredService<App>();
                IResult result;

                try
                {
                    var proxyRequest = appInstance.BuildProxyRequest(urlPath);
                    var response = await Client.SendAsync(proxyRequest, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                    string mimeType = ContentTypeProvider.TryGetContentType(urlPath, out var contentType) ? contentType : "application/octet-stream";
                    var stream = await response.Content.ReadAsStreamAsync(context.RequestAborted);
                    if (!XssSafeMimeTypes.Contains(mimeType))
                    {
                        result = Results.File(stream, "application/octet-stream", enableRangeProcessing: true);
                    }
                    else
                    {
                        result = Results.File(stream, mimeType, enableRangeProcessing: true);
                    }
                }
                catch (Exception e)
                {
                    result = Results.BadRequest(new { detail = e.Message });
                }

                await result.ExecuteAsync(context);
            });

            // File serving
            var fileHandler = async (HttpContext context) =>
            {
                string pathOrUrl = context.Request.RouteValues.TryGetValue("pathOrUrl", out var pathValue) ? pathValue?.ToString() ?? string.Empty : string.Empty;
                var appInstance = context.RequestServices.GetRequiredService<App>();
                IResult result;

                if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var url) && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps))
                {
                    result = Results.Redirect(pathOrUrl);
                    await result.ExecuteAsync(context);
                    return;
                }

                if (RouteUtils.StartsWithProtocol(pathOrUrl))
                {
                    result = Results.StatusCode(StatusCodes.Status403Forbidden);
                    await result.ExecuteAsync(context);
                    return;
                }

                string absPath;
                try
                {
                    absPath = Path.GetFullPath(pathOrUrl);
                    if (Directory.Exists(absPath) || !File.Exists(absPath))
                    {
                        result = Results.StatusCode(StatusCodes.Status403Forbidden);
                        await result.ExecuteAsync(context);
                        return;
                    }
                }
                catch
                {
                    result = Results.StatusCode(StatusCodes.Status403Forbidden);
                    await result.ExecuteAsync(context);
                    return;
                }

                var blocks = appInstance.GetBlocks();
                if (!IsAllowedFile(absPath, blocks))
                {
                    result = Results.StatusCode(StatusCodes.Status403Forbidden);
                    await result.ExecuteAsync(context);
                    return;
                }

                string mimeType = ContentTypeProvider.TryGetContentType(absPath, out var contentType) ? contentType : "application/octet-stream";
                bool safe = XssSafeMimeTypes.Contains(mimeType);
                string contentDisposition = safe ? "inline" : "attachment";

                result = Results.File(absPath, mimeType, fileDownloadName: Path.GetFileName(absPath), enableRangeProcessing: true);
                var rawFileName = Path.GetFileName(absPath);
                var encodedFileName = Uri.EscapeDataString(rawFileName);
                context.Response.Headers["Content-Disposition"] = $"{contentDisposition}; filename=\"{encodedFileName}\"; filename*=UTF-8''{encodedFileName}";
                context.Response.Headers["Accept-Ranges"] = "bytes";
                await result.ExecuteAsync(context);
            };
            // Keep non-catch-all path for compatibility.
            app.MapMethods("/file={pathOrUrl}", new[] { "GET", "HEAD" }, fileHandler);
            app.MapMethods("/api/file={pathOrUrl}", new[] { "GET", "HEAD" }, fileHandler);
            app.MapMethods($"{RouteUtils.ApiPrefix}/file={{pathOrUrl}}", new[] { "GET", "HEAD" }, fileHandler);

            // Catch-all compatibility routes for URLs like /gradio_api/file=D:/... where '/' appears inside pathOrUrl.
            static bool TryExtractFileRoutePath(string rawPath, out string pathOrUrl)
            {
                const string prefix = "file=";
                if (!string.IsNullOrEmpty(rawPath) && rawPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    pathOrUrl = rawPath.Substring(prefix.Length);
                    return true;
                }
                pathOrUrl = string.Empty;
                return false;
            }

            app.MapMethods("/{**rawPath}", new[] { "GET", "HEAD" }, async (HttpContext context, string rawPath) =>
            {
                if (!TryExtractFileRoutePath(rawPath, out var pathOrUrl))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                context.Request.RouteValues["pathOrUrl"] = pathOrUrl;
                await fileHandler(context);
            }).WithOrder(int.MaxValue);

            app.MapMethods("/api/{**rawPath}", new[] { "GET", "HEAD" }, async (HttpContext context, string rawPath) =>
            {
                if (!TryExtractFileRoutePath(rawPath, out var pathOrUrl))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                context.Request.RouteValues["pathOrUrl"] = pathOrUrl;
                await fileHandler(context);
            });

            app.MapMethods($"{RouteUtils.ApiPrefix}/{{**rawPath}}", new[] { "GET", "HEAD" }, async (HttpContext context, string rawPath) =>
            {
                if (!TryExtractFileRoutePath(rawPath, out var pathOrUrl))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                context.Request.RouteValues["pathOrUrl"] = pathOrUrl;
                await fileHandler(context);
            });

            // Upload route
            var uploadHandler = async (HttpRequest request, [FromServices] App appInstance) =>
            {
                if (!request.HasFormContentType)
                {
                    return Results.BadRequest(new { detail = "Invalid form data." });
                }

                string uploadId = request.Query.TryGetValue("upload_id", out var uploadIdValue)
                    ? uploadIdValue.ToString()
                    : null;

                if (!string.IsNullOrEmpty(uploadId))
                {
                    FileUploadStatuses.Track(uploadId);
                }

                var form = await request.ReadFormAsync();
                var savedFiles = new List<string>();

                try
                {
                    foreach (var file in form.Files)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string targetDir = appInstance.UploadedFileDir;
                        Directory.CreateDirectory(targetDir);
                        string targetPath = Path.Combine(targetDir, $"{Guid.NewGuid()}_{fileName}");

                        await using var stream = new FileStream(targetPath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        savedFiles.Add(targetPath);
                        if (!string.IsNullOrEmpty(uploadId))
                        {
                            FileUploadStatuses.Append(uploadId, fileName, Encoding.UTF8.GetBytes("chunk"));
                        }
                    }
                }
                finally
                {
                    if (!string.IsNullOrEmpty(uploadId))
                    {
                        FileUploadStatuses.SetDone(uploadId);
                    }
                }

                return Results.Json(savedFiles, JsonOptions);
            };
            app.MapPost("/api/upload", uploadHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/upload", uploadHandler);

            // Queue status route
            app.MapGet("/api/queue/status", ([FromServices] App appInstance) =>
            {
                // Queue status placeholder
                return Results.Json(new { status = "unavailable" }, JsonOptions);
            });

            // Upload progress route
            var uploadProgressHandler = async (HttpContext context, [FromQuery] string upload_id) =>
            {
                if (string.IsNullOrWhiteSpace(upload_id))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Upload not found");
                    return;
                }

                var trackedTask = FileUploadStatuses.IsTracked(upload_id);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3), context.RequestAborted);
                var completed = await Task.WhenAny(trackedTask, timeoutTask);
                if (completed != trackedTask)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Upload not found");
                    return;
                }

                context.Response.Headers.ContentType = "text/event-stream";
                var lastHeartbeat = DateTimeOffset.UtcNow;
                var heartbeatRate = TimeSpan.FromSeconds(15);
                var checkRate = TimeSpan.FromMilliseconds(50);
                try
                {
                    while (!context.RequestAborted.IsCancellationRequested)
                    {
                        if (FileUploadStatuses.IsDone(upload_id))
                        {
                            var donePayload = JsonSerializer.Serialize(new { msg = "done" }, JsonOptions);
                            await context.Response.WriteAsync($"data: {donePayload}\n\n");
                            await context.Response.Body.FlushAsync();
                            FileUploadStatuses.StopTracking(upload_id);
                            return;
                        }

                        FileUploadProgressUnit unit;
                        try
                        {
                            unit = FileUploadStatuses.Pop(upload_id);
                        }
                        catch (FileUploadProgressNotTrackedError)
                        {
                            return;
                        }
                        catch (FileUploadProgressNotQueuedError)
                        {
                            await Task.Delay(checkRate, context.RequestAborted);
                            if (DateTimeOffset.UtcNow - lastHeartbeat > heartbeatRate)
                            {
                                var heartbeatPayload = JsonSerializer.Serialize(new { msg = "heartbeat" }, JsonOptions);
                                await context.Response.WriteAsync($"data: {heartbeatPayload}\n\n");
                                await context.Response.Body.FlushAsync();
                                lastHeartbeat = DateTimeOffset.UtcNow;
                            }
                            continue;
                        }

                        var payload = JsonSerializer.Serialize(new
                        {
                            msg = "update",
                            orig_name = unit.Filename,
                            chunk_size = unit.ChunkSize
                        }, JsonOptions);
                        await context.Response.WriteAsync($"data: {payload}\n\n");
                        await context.Response.Body.FlushAsync();
                    }
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    FileUploadStatuses.StopTracking(upload_id);
                }
            };
            app.MapGet("/upload_progress", uploadProgressHandler);
            app.MapGet("/api/upload_progress", uploadProgressHandler);
            app.MapGet($"{RouteUtils.ApiPrefix}/upload_progress", uploadProgressHandler);

            // Deep link generation
            app.MapGet("/gradio_api/deep_link", ([FromServices] App appInstance, [FromQuery] string session_hash) =>
            {
                // Feature requires session state snapshots; return empty until implemented
                if (string.IsNullOrEmpty(session_hash))
                {
                    return Results.Text(string.Empty);
                }

                return Results.Text(string.Empty);
            });

            // Theme CSS
            app.MapGet("/theme.css", (HttpContext context, [FromServices] App appInstance) =>
            {
                context.Response.ContentType = "text/css";
                string themeCss = string.Empty;
                if (appInstance.GetBlocks().Theme != null)
                {
                    var generateCssMethod = appInstance.GetBlocks().Theme.GetType().GetMethod("GenerateCss");
                    if (generateCssMethod != null)
                    {
                        themeCss = generateCssMethod.Invoke(appInstance.GetBlocks().Theme, null) as string ?? string.Empty;
                    }
                }
                return Results.Text(themeCss, "text/css");
            });

            // Robots.txt
            app.MapGet("/robots.txt", () =>
            {
                return Results.Text("User-agent: *\nDisallow: /", "text/plain");
            });

            // PWA icon
            app.MapGet("/pwa_icon/{size?}", (string size) =>
            {
                return ServeEmbeddedResource("Gradio.Net.Templates.frontend", "favicon.png");
            });

            // Manifest
            app.MapGet("/manifest.json", async (HttpContext context) =>
            {
                context.Response.ContentType = "application/json";
                var stream = GetEmbeddedResourceStream("Gradio.Net.Templates.frontend.manifest.json");
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var manifest = await reader.ReadToEndAsync();
                    return Results.Text(manifest, "application/json");
                }

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
                return Results.Json(minimalManifest, JsonOptions);
            });

            // ========================================
            // API PREDICTION ROUTES
            // ========================================

            // Predict endpoint (direct API execution without queue)
            var predictRunHandler = async (HttpContext context, [FromServices] App appInstance, string api_name) =>
            {
                return await HandlePredict(context, appInstance, api_name);
            };
            // Primary route matching Python Gradio: POST /run/{api_name}
            app.MapPost("/run/{api_name}", predictRunHandler);
            app.MapPost("/api/run/{api_name}", predictRunHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/run/{{api_name}}", predictRunHandler);

            var predictApiHandler = async (HttpContext context, [FromServices] App appInstance, string api_name) =>
            {
                return await HandlePredict(context, appInstance, api_name);
            };
            app.MapPost("/api/{api_name}", predictApiHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/api/{{api_name}}", predictApiHandler);

            // Simple predict POST (returns event_id immediately)
            var predictCallPostHandler = async (HttpContext context, [FromServices] App appInstance, string api_name) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<Data.SimplePredictBody>();
                    if (body == null)
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    // Convert SimplePredictBody to PredictBody
                    var fullBody = new Data.PredictBodyInternal
                    {
                        Data = body.Data,
                        SessionHash = body.SessionHash,
                        Request = context.Request,
                        SimpleFormat = true
                    };

                    // Get the function and set fn_index
                    var fn = RouteUtils.GetFn(appInstance.GetBlocks(), api_name, fullBody);
                    fullBody.FnIndex = fn.Id;

                    // Join queue
                    var username = appInstance.GetCurrentUser(context.Request);

                    // Python parity: snapshot the request so it remains available in ProcessEvent
                    fullBody.GrRequest = RouteUtils.SnapshotRequest(context.Request, username, fullBody.SessionHash);

                    var (success, eventIdOrError, state) = await appInstance.QueueManager.Push(fullBody, username);

                    if (!success)
                    {
                        if (state == "validator_error")
                        {
                            // Python parity: return {"detail": [...]} with HTTP 422
                            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                            context.Response.ContentType = "application/json";
                            var detail = System.Text.Json.JsonSerializer.Deserialize<object>(eventIdOrError);
                            await context.Response.WriteAsync(
                                System.Text.Json.JsonSerializer.Serialize(new { detail }));
                            return Results.Empty;
                        }

                        var statusCode = state switch
                        {
                            "queue_full" => StatusCodes.Status503ServiceUnavailable,
                            _ => StatusCodes.Status400BadRequest
                        };
                        return Results.Problem(eventIdOrError, statusCode: statusCode);
                    }

                    return Results.Json(new { event_id = eventIdOrError });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error processing request: {ex.Message}");
                }
            };
            app.MapPost("/api/call/{api_name}", predictCallPostHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/call/{{api_name}}", predictCallPostHandler);

            // Simple predict GET (SSE stream for results)
            var predictCallGetHandler = async (HttpContext context, [FromServices] App appInstance, string event_id) =>
            {
                context.Response.Headers.ContentType = "text/event-stream";
                context.Response.Headers.CacheControl = "no-cache";
                context.Response.Headers["X-Accel-Buffering"] = "no";

                var channel = appInstance.QueueManager.GetEventChannel(event_id);
                if (channel == null)
                {
                    return Results.NotFound();
                }

                try
                {
                    await foreach (var message in channel.Reader.ReadAllAsync(context.RequestAborted))
                    {
                        // Format message for simple format
                        string eventType = "heartbeat";
                        object data = null;

                        var completedMsg = message as Queueing.ProcessCompletedMessage;
                        var generatingMsg = message as Queueing.ProcessGeneratingMessage;
                        var heartbeatMsg = message as Queueing.HeartbeatMessage;
                        var errorMsg = message as Queueing.UnexpectedErrorMessage;

                        if (completedMsg != null)
                        {
                            eventType = completedMsg.Success ? "complete" : "error";
                            if (completedMsg.Output != null && completedMsg.Output.TryGetValue("data", out var outputData))
                            {
                                data = outputData;
                            }
                        }
                        else if (generatingMsg != null)
                        {
                            eventType = generatingMsg.Success ? "generating" : "error";
                            if (generatingMsg.Output != null && generatingMsg.Output.TryGetValue("data", out var outputData))
                            {
                                data = outputData;
                            }
                        }
                        else if (heartbeatMsg != null)
                        {
                            eventType = "heartbeat";
                            data = null;
                        }
                        else if (errorMsg != null)
                        {
                            eventType = "error";
                            data = errorMsg.Message;
                        }

                        if (data != null)
                        {
                            var jsonData = JsonSerializer.Serialize(data, JsonOptions);
                            await context.Response.WriteAsync($"event: {eventType}\ndata: {jsonData}\n\n");
                        }
                        else if (eventType == "heartbeat")
                        {
                            await context.Response.WriteAsync($"event: {eventType}\ndata: null\n\n");
                        }
                        await context.Response.Body.FlushAsync();

                        // Close stream on complete or error
                        if (message is ProcessCompletedMessage || message is UnexpectedErrorMessage)
                        {
                            break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Client disconnected
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync($"event: error\ndata: {JsonSerializer.Serialize(ex.Message)}\n\n");
                    await context.Response.Body.FlushAsync();
                }

                return Results.Empty;
            };
            app.MapGet("/api/call/{api_name}/{event_id}", predictCallGetHandler);
            app.MapGet($"{RouteUtils.ApiPrefix}/call/{{api_name}}/{{event_id}}", predictCallGetHandler);

            // ========================================
            // QUEUE SYSTEM ROUTES
            // ========================================

            // Queue join endpoint
            var queueJoinHandler = async (HttpContext context, [FromServices] App appInstance) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<PredictBodyInternal>();
                    if (body == null)
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    var username = appInstance.GetCurrentUser(context.Request);

                    // Python parity: body.request = request (stored at queue-join time so it
                    // remains accessible when ProcessEvent runs after the connection closes).
                    body.GrRequest = RouteUtils.SnapshotRequest(context.Request, username, body.SessionHash);

                    var (success, eventIdOrError, state) = await appInstance.QueueManager.Push(body, username);

                    if (!success)
                    {
                        if (state == "validator_error")
                        {
                            // Python parity: return {"detail": [...]} with HTTP 422
                            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                            context.Response.ContentType = "application/json";
                            var detail = System.Text.Json.JsonSerializer.Deserialize<object>(eventIdOrError);
                            await context.Response.WriteAsync(
                                System.Text.Json.JsonSerializer.Serialize(new { detail }));
                            return Results.Empty;
                        }

                        var statusCode = state switch
                        {
                            "queue_full" => StatusCodes.Status503ServiceUnavailable,
                            _ => StatusCodes.Status400BadRequest
                        };
                        return Results.Problem(eventIdOrError, statusCode: statusCode);
                    }

                    return Results.Json(new { event_id = eventIdOrError });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error joining queue: {ex.Message}");
                }
            };
            app.MapPost("/queue/join", queueJoinHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/queue/join", queueJoinHandler);

            // Queue data SSE stream
            var queueDataHandler = async (HttpContext context, [FromServices] App appInstance, string session_hash) =>
            {
                context.Response.Headers.ContentType = "text/event-stream";
                context.Response.Headers.CacheControl = "no-cache";
                context.Response.Headers["X-Accel-Buffering"] = "no";

                var sseLogger = context.RequestServices.GetService<ILoggerFactory>()
                    ?.CreateLogger("Gradio.Net.Core.QueueData");

                sseLogger?.LogInformation("[/queue/data] SSE connection opened for session={Session}", session_hash);

                var channel = appInstance.QueueManager.GetSessionChannel(session_hash);
                if (channel == null)
                {
                    sseLogger?.LogWarning("[/queue/data] No channel found for session={Session}", session_hash);
                    return Results.NotFound();
                }

                try
                {
                    var heartbeatInterval = TimeSpan.FromSeconds(15);
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                        context.RequestAborted,
                        appInstance.StopEvent.Token);
                    var streamToken = linkedCts.Token;

                    // Python parity: heartbeat fires only when channel is idle for 15+ seconds.
                    // Use WaitToReadAsync with timeout so we can send heartbeat while waiting for messages.
                    while (!streamToken.IsCancellationRequested)
                    {
                        // Wait for next message, canceling after heartbeatInterval to send heartbeat
                        using var readCts = CancellationTokenSource.CreateLinkedTokenSource(streamToken);
                        readCts.CancelAfter(heartbeatInterval);

                        bool hasMessage;
                        try
                        {
                            hasMessage = await channel.Reader.WaitToReadAsync(readCts.Token);
                        }
                        catch (OperationCanceledException) when (!streamToken.IsCancellationRequested)
                        {
                            // Idle for heartbeatInterval - send heartbeat (Python parity)
                            var heartbeat = new Queueing.HeartbeatMessage();
                            var heartbeatJson = JsonSerializer.Serialize(heartbeat, heartbeat.GetType(), JsonOptions);
                            sseLogger?.LogInformation("[/queue/data] SSE heartbeat (idle) session={Session}", session_hash);
                            await context.Response.WriteAsync($"data: {heartbeatJson}\n\n");
                            await context.Response.Body.FlushAsync();
                            continue;
                        }

                        if (!hasMessage) break; // channel completed

                        while (channel.Reader.TryRead(out var message))
                        {
                            // Serialize message to JSON
                            var json = JsonSerializer.Serialize(message, message.GetType(), JsonOptions);
                            sseLogger?.LogInformation("[/queue/data] SSE send session={Session} msg={Msg} json={Json}",
                                session_hash, message.Msg, json);
                            await context.Response.WriteAsync($"data: {json}\n\n");
                            await context.Response.Body.FlushAsync();

                            if (message is Queueing.ProcessCompletedMessage completedMessage && !string.IsNullOrWhiteSpace(completedMessage.EventId))
                            {
                                var shouldCloseStream = await appInstance.QueueManager.CompleteEventForSession(
                                    session_hash,
                                    completedMessage.EventId
                                );

                                if (shouldCloseStream)
                                {
                                    var closeStream = new Queueing.CloseStreamMessage();
                                    var closeStreamJson = JsonSerializer.Serialize(closeStream, closeStream.GetType(), JsonOptions);
                                    sseLogger?.LogInformation("[/queue/data] SSE send close_stream session={Session} json={Json}",
                                        session_hash, closeStreamJson);
                                    await context.Response.WriteAsync($"data: {closeStreamJson}\n\n");
                                    await context.Response.Body.FlushAsync();
                                    sseLogger?.LogInformation("[/queue/data] SSE stream closed for session={Session}", session_hash);
                                    goto sseStreamDone;
                                }
                            }

                            // Check if should close stream
                            if (message is CloseStreamMessage)
                            {
                                sseLogger?.LogInformation("[/queue/data] SSE CloseStreamMessage received, closing session={Session}", session_hash);
                                goto sseStreamDone;
                            }
                        }
                    }
                sseStreamDone:;
                }
                catch (OperationCanceledException)
                {
                    sseLogger?.LogInformation("[/queue/data] SSE connection cancelled/disconnected for session={Session}", session_hash);
                    // Client disconnected, clean up. During server shutdown, avoid extra work.
                    if (!appInstance.StopEvent.IsCancellationRequested)
                    {
                        await appInstance.QueueManager.CleanEvents(session_hash);
                    }
                }
                catch (Exception ex)
                {
                    sseLogger?.LogError(ex, "[/queue/data] SSE error for session={Session}", session_hash);
                    // Use Queueing.UnexpectedErrorMessage (string msg) not ServerMessages.UnexpectedErrorMessage (enum int)
                    var errorMsg = new Queueing.UnexpectedErrorMessage(ex.Message);
                    var errorJson = JsonSerializer.Serialize(errorMsg, errorMsg.GetType(), JsonOptions);
                    await context.Response.WriteAsync($"data: {errorJson}\n\n");
                    await context.Response.Body.FlushAsync();
                }

                return Results.Empty;
            };
            app.MapGet("/queue/data", queueDataHandler);
            app.MapGet($"{RouteUtils.ApiPrefix}/queue/data", queueDataHandler);

            // Cancel event
            var cancelHandler = async (HttpContext context, [FromServices] App appInstance) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<CancelBody>();
                    if (body == null)
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    var success = await appInstance.QueueManager.CancelEvent(body.EventId, body.SessionHash);
                    return Results.Json(new { success });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error canceling event: {ex.Message}");
                }
            };
            app.MapPost("/cancel", cancelHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/cancel", cancelHandler);

            // Queue status
            var queueStatusHandler = ([FromServices] App appInstance) =>
            {
                var status = appInstance.QueueManager.GetStatus();
                return Results.Json(status);
            };
            app.MapGet("/queue/status", queueStatusHandler);
            app.MapGet($"{RouteUtils.ApiPrefix}/queue/status", queueStatusHandler);

            // Reset iterator
            var resetHandler = async (HttpContext context, [FromServices] App appInstance) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<ResetBody>();
                    if (body == null)
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    if (body.EventId != null && appInstance.Iterators.ContainsKey(body.EventId))
                    {
                        await appInstance.Lock.WaitAsync();
                        try
                        {
                            appInstance.Iterators.Remove(body.EventId);
                            appInstance.IteratorsToReset.Add(body.EventId);
                        }
                        finally
                        {
                            appInstance.Lock.Release();
                        }
                    }

                    return Results.Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error resetting iterator: {ex.Message}");
                }
            };
            app.MapPost("/reset", resetHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/reset", resetHandler);

            // Heartbeat endpoint (POST)
            var heartbeatHandler = async (HttpContext context, [FromServices] App appInstance) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (body == null || !body.TryGetValue("session_hash", out var sessionHash))
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    // Keep session alive
                    var channel = appInstance.QueueManager.GetSessionChannel(sessionHash);
                    if (channel != null)
                    {
                        return Results.Json(new { success = true });
                    }

                    return Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error processing heartbeat: {ex.Message}");
                }
            };
            app.MapPost("/heartbeat", heartbeatHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/heartbeat", heartbeatHandler);

            // Heartbeat SSE endpoint (GET) - Persistent connection
            var heartbeatSseHandler = async (HttpContext context, string session_hash, [FromServices] App appInstance) =>
            {
                // Clients make a persistent connection to this endpoint to keep the session alive.
                // When the client disconnects, the session state is marked for deletion and unload events are triggered.

                var user = appInstance.GetCurrentUser(context.Request);
                var heartbeatRate = Environment.GetEnvironmentVariable("GRADIO_IS_E2E_TEST") == "1" ? 0.25 : 15.0;

                context.Response.Headers.ContentType = "text/event-stream";
                context.Response.Headers.CacheControl = "no-cache";
                context.Response.Headers["X-Accel-Buffering"] = "no";

                try
                {
                    var stopToken = appInstance.StopEvent.Token;
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                        context.RequestAborted,
                        stopToken);
                    var streamToken = linkedCts.Token;

                    while (!streamToken.IsCancellationRequested)
                    {
                        // Send heartbeat message
                        await context.Response.WriteAsync("data: ALIVE\n\n");
                        await context.Response.Body.FlushAsync();

                        // Wait for heartbeat interval or cancellation
                        await Task.Delay(TimeSpan.FromSeconds(heartbeatRate), streamToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Client disconnected or server stopping - clean up session

                    // Trigger unload events for this session
                    var blocks = appInstance.GetBlocks();
                    var unloadFnIndices = new List<int>();

                    foreach (var kvp in blocks.Fns)
                    {
                        var dep = kvp.Value;
                        if (dep.Targets != null && dep.Targets.Any(t => t.Item2 == "unload"))
                        {
                            unloadFnIndices.Add(kvp.Key);
                        }
                    }

                    // Execute unload functions
                    if (unloadFnIndices.Any())
                    {
                        var rootPath = RouteUtils.GetRootUrl(
                            context.Request,
                            $"{RouteUtils.ApiPrefix}/heartbeat/{session_hash}",
                            appInstance.GetBlocks().RootPath ?? ""
                        );

                        var predictBody = new PredictBodyInternal
                        {
                            SessionHash = session_hash,
                            Data = new List<object>()
                        };

                        // Trigger unload events in background
                        foreach (var fnIndex in unloadFnIndices)
                        {
                            if (blocks.Fns.TryGetValue(fnIndex, out var fn))
                            {
                                _ = Task.Run(async () =>
                                {
                                    try
                                    {
                                        await RouteUtils.CallProcessApi(
                                            app: appInstance,
                                            body: predictBody,
                                            grRequest: new Request(context.Request, user, session_hash),
                                            fn: fn,
                                            rootPath: rootPath
                                        );
                                    }
                                    catch
                                    {
                                        // Ignore errors in unload handlers
                                    }
                                });
                            }
                        }
                    }

                    // Mark session for deletion
                    if (appInstance.StateHolder.SessionData.TryGetValue(session_hash, out var sessionState))
                    {
                        sessionState.IsClosed = true;
                    }

                    // Cancel pending events for this session using QueueManager
                    await appInstance.QueueManager.CleanEvents(session_hash);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw
                }

                return Results.Empty;
            };
            app.MapGet("/heartbeat/{session_hash}", heartbeatSseHandler);
            app.MapGet($"{RouteUtils.ApiPrefix}/heartbeat/{{session_hash}}", heartbeatSseHandler);

            // ========================================
            // DEEP LINK SUPPORT
            // ========================================

            // Deep link endpoint - Creates and saves a deep link for the current session
            app.MapGet("/api/deep_link", async (HttpContext context, [FromServices] App appInstance) =>
            {
                var sessionHash = context.Request.Query["session_hash"].ToString();

                if (string.IsNullOrEmpty(sessionHash))
                {
                    return Results.BadRequest("session_hash is required");
                }

                if (appInstance.StateHolder.Contains(sessionHash))
                {
                    var sessionState = appInstance.StateHolder[sessionHash];
                    var components = new List<object>();

                    // Deep copy the components from session state
                    // This is a simplified version - in production you'd need proper deep copy
                    var componentsJson = JsonSerializer.Serialize(components, JsonOptions);

                    // Create URL-safe hash
                    var deepLink = RouteUtils.CreateUrlSafeHash(Encoding.UTF8.GetBytes(componentsJson));

                    // Save to file system
                    var directory = Path.Combine(appInstance.UploadedFileDir, "deep_links", deepLink);
                    Directory.CreateDirectory(directory);

                    var filePath = Path.Combine(directory, "state.json");
                    await File.WriteAllTextAsync(filePath, componentsJson);

                    return Results.Text(deepLink);
                }
                else
                {
                    return Results.Text(string.Empty);
                }
            });

            // ========================================
            // STARTUP AND LIFECYCLE ROUTES
            // ========================================

            // Startup events endpoint
            app.MapGet("/api/startup-events", async ([FromServices] App appInstance) =>
            {
                if (!appInstance.StartupEventsTriggered)
                {
                    var blocks = appInstance.GetBlocks();

                    // Trigger startup events
                    blocks.RunStartupEvents();

                    // Run extra startup events if any
                    await blocks.RunExtraStartupEvents();

                    appInstance.StartupEventsTriggered = true;
                    return Results.Json(true);
                }
                return Results.Json(false);
            });

            // ========================================
            // COMPONENT SERVER ROUTES
            // ========================================

            // Component server endpoint - handles server-side component methods
            var componentServerHandler = async (HttpContext context, [FromServices] App appInstance) =>
            {
                try
                {
                    ComponentServerRequest body;

                    // Parse request based on content type
                    if (context.Request.HasFormContentType)
                    {
                        // Handle multipart/form-data for file uploads
                        var form = await context.Request.ReadFormAsync();
                        var files = new List<Tuple<string, byte[]>>();
                        var data = new Dictionary<string, object>();

                        foreach (var file in form.Files)
                        {
                            using var memoryStream = new MemoryStream();
                            await file.CopyToAsync(memoryStream);
                            files.Add(new Tuple<string, byte[]>(file.FileName, memoryStream.ToArray()));
                        }

                        foreach (var key in form.Keys.Where(k => !form.Files.Any(f => f.Name == k)))
                        {
                            data[key] = form[key].ToString();
                        }

                        body = new ComponentServerRequest
                        {
                            Data = data,
                            Files = files,
                            ComponentId = int.Parse(form["component_id"].ToString()),
                            SessionHash = form["session_hash"].ToString(),
                            FnName = form["fn_name"].ToString()
                        };
                    }
                    else
                    {
                        // Handle JSON request
                        var jsonBody = await context.Request.ReadFromJsonAsync<Dictionary<string, object>>();
                        if (jsonBody == null)
                        {
                            return Results.BadRequest("Invalid request body");
                        }

                        body = new ComponentServerRequest
                        {
                            Data = jsonBody.TryGetValue("data", out var dataValue) ? dataValue as Dictionary<string, object> : new Dictionary<string, object>(),
                            ComponentId = Convert.ToInt32(jsonBody["component_id"]),
                            SessionHash = jsonBody["session_hash"].ToString(),
                            FnName = jsonBody["fn_name"].ToString()
                        };
                    }

                    // Get component from state or blocks
                    var state = appInstance.StateHolder[body.SessionHash];
                    object component;

                    if (state != null && state[body.ComponentId] != null)
                    {
                        component = state[body.ComponentId];
                    }
                    else
                    {
                        component = appInstance.GetBlocks().BlocksDict[body.ComponentId];
                    }

                    // Get the server function from component
                    var componentType = component.GetType();
                    var method = componentType.GetMethod(body.FnName);

                    if (method == null || !Attribute.IsDefined(method, typeof(ServerFunctionAttribute)))
                    {
                        return Results.NotFound("Server function not found or not marked as server function");
                    }

                    // Invoke the server function
                    object result;
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0)
                    {
                        result = method.Invoke(component, null);
                    }
                    else if (parameters.Length == 1 && body.Data != null)
                    {
                        result = method.Invoke(component, new[] { body.Data });
                    }
                    else
                    {
                        return Results.BadRequest("Invalid server function signature");
                    }

                    // Handle async result
                    if (result is Task task)
                    {
                        await task;
                        var resultProperty = task.GetType().GetProperty("Result");
                        if (resultProperty != null)
                        {
                            result = resultProperty.GetValue(task);
                        }
                    }

                    return Results.Json(new { output = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error processing component server request: {ex.Message}");
                }
            };
            app.MapPost("/api/component_server", componentServerHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/component_server", componentServerHandler);

            // ========================================
            // STREAMING MEDIA ROUTES (HLS)
            // ========================================

            // Stream event - used for streaming predictions
            var streamEventHandler = async (HttpContext context, [FromServices] App appInstance, string eventId) =>
            {
                try
                {
                    var body = await context.Request.ReadFromJsonAsync<PredictBody>();
                    if (body == null)
                    {
                        return Results.BadRequest("Invalid request body");
                    }

                    // Get event from queue
                    var queueEvent = appInstance.QueueManager.GetEvent(eventId);
                    if (queueEvent == null)
                    {
                        return Results.NotFound("Event not found");
                    }

                    // Convert PredictBody to PredictBodyInternal
                    var bodyInternal = new PredictBodyInternal
                    {
                        Data = body.Data,
                        SessionHash = body.SessionHash,
                        FnIndex = body.FnIndex,
                        Request = context.Request
                    };

                    // Update event data
                    queueEvent.Data = bodyInternal;
                    TrySignalQueueEvent(queueEvent);

                    return Results.Json(new { msg = "success" });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error processing stream event: {ex.Message}");
                }
            };
            app.MapPost("/api/stream/{eventId}", streamEventHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/stream/{{eventId}}", streamEventHandler);

            // Close stream event
            var closeStreamEventHandler = async ([FromServices] App appInstance, string eventId) =>
            {
                try
                {
                    var queueEvent = appInstance.QueueManager.GetEvent(eventId);
                    if (queueEvent == null)
                    {
                        return Results.NotFound("Event not found");
                    }

                    // Mark event as closed (no RunTime property in QueueEvent)
                    queueEvent.Closed = true;
                    TrySignalQueueEvent(queueEvent);

                    return Results.Json(new { msg = "success" });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error closing stream: {ex.Message}");
                }
            };
            app.MapPost("/api/stream/{eventId}/close", closeStreamEventHandler);
            app.MapPost($"{RouteUtils.ApiPrefix}/stream/{{eventId}}/close", closeStreamEventHandler);

            // HLS playlist endpoint
            app.MapGet("/api/stream/{sessionHash}/{run}/{componentId}/playlist.m3u8",
                ([FromServices] App appInstance, string sessionHash, int run, int componentId) =>
            {
                try
                {
                    var blocks = appInstance.GetBlocks();
                    var stream = blocks.GetPendingStream(sessionHash, run, componentId);

                    if (stream == null)
                    {
                        return Results.NotFound();
                    }

                    var playlist = new StringBuilder();
                    playlist.AppendLine("#EXTM3U");
                    playlist.AppendLine("#EXT-X-PLAYLIST-TYPE:EVENT");
                    playlist.AppendLine($"#EXT-X-TARGETDURATION:{stream.MaxDuration}");
                    playlist.AppendLine("#EXT-X-VERSION:4");
                    playlist.AppendLine("#EXT-X-MEDIA-SEQUENCE:0");

                    foreach (var segment in stream.Segments)
                    {
                        playlist.AppendLine($"#EXTINF:{segment.Duration:F3},");
                        playlist.AppendLine($"{segment.Id}{segment.Extension}");

                        // Add discontinuity tag for .ts segments
                        if (segment.Extension == ".ts")
                        {
                            playlist.AppendLine("#EXT-X-DISCONTINUITY");
                        }
                    }

                    if (stream.Ended)
                    {
                        playlist.AppendLine("#EXT-X-ENDLIST");
                    }

                    return Results.Text(playlist.ToString(), "application/vnd.apple.mpegurl");
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error generating playlist: {ex.Message}");
                }
            });

            // HLS segment endpoint
            app.MapGet("/api/stream/{sessionHash}/{run}/{componentId}/{segmentId}.{ext}",
                async ([FromServices] App appInstance, string sessionHash, int run, int componentId, string segmentId, string ext) =>
            {
                try
                {
                    if (ext != "aac" && ext != "ts")
                    {
                        return Results.BadRequest("Unsupported file extension");
                    }

                    var blocks = appInstance.GetBlocks();
                    var stream = blocks.GetPendingStream(sessionHash, run, componentId);

                    if (stream == null)
                    {
                        return Results.NotFound();
                    }

                    var segment = stream.Segments.FirstOrDefault(s => s.Id == segmentId && s.Extension == $".{ext}");
                    if (segment == null)
                    {
                        return Results.NotFound();
                    }

                    var contentType = ext == "ts" ? "video/MP2T" : "audio/aac";
                    return Results.File(segment.Data, contentType);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error serving segment: {ex.Message}");
                }
            });

            // Playlist file endpoint - returns combined stream file
            app.MapGet("/api/stream/{sessionHash}/{run}/{componentId}/playlist-file",
                async ([FromServices] App appInstance, string sessionHash, int run, int componentId) =>
            {
                try
                {
                    var blocks = appInstance.GetBlocks();
                    var stream = blocks.GetPendingStream(sessionHash, run, componentId);

                    if (stream == null)
                    {
                        return Results.NotFound();
                    }

                    // If combined file doesn't exist, create it
                    if (string.IsNullOrEmpty(stream.CombinedFile))
                    {
                        // Create a temporary combined file from all segments
                        var tempFile = Path.Combine(Path.GetTempPath(), $"{sessionHash}_{run}_{componentId}{stream.DesiredOutputFormat ?? ".mp4"}");

                        using (var outputStream = File.Create(tempFile))
                        {
                            foreach (var segment in stream.Segments)
                            {
                                await outputStream.WriteAsync(segment.Data);
                            }
                        }

                        stream.CombinedFile = tempFile;
                    }

                    if (string.IsNullOrEmpty(stream.CombinedFile) || !File.Exists(stream.CombinedFile))
                    {
                        return Results.NotFound("Combined file not available");
                    }

                    var contentType = ContentTypeProvider.TryGetContentType(stream.CombinedFile, out var ct)
                        ? ct
                        : "application/octet-stream";

                    return Results.File(stream.CombinedFile, contentType, enableRangeProcessing: true);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error serving combined stream: {ex.Message}");
                }
            });

            // Merged stream endpoint
            app.MapGet("/api/stream/{sessionHash}/merged.{ext}",
                async ([FromServices] App appInstance, string sessionHash, string ext) =>
            {
                try
                {
                    var blocks = appInstance.GetBlocks();
                    var stream = blocks.GetMergedStream(sessionHash, ext);

                    if (stream == null || !File.Exists(stream))
                    {
                        return Results.NotFound();
                    }

                    var contentType = ContentTypeProvider.TryGetContentType(stream, out var ct) ? ct : "application/octet-stream";
                    return Results.File(stream, contentType, enableRangeProcessing: true);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error serving merged stream: {ex.Message}");
                }
            });

        }

        private static async Task<IResult> HandlePredict(HttpContext context, App appInstance, string apiName)
        {
            try
            {
                var body = await context.Request.ReadFromJsonAsync<Data.PredictBody>();
                if (body == null)
                {
                    return Results.BadRequest("Invalid request body");
                }

                // Convert to PredictBodyInternal
                var bodyInternal = new Data.PredictBodyInternal
                {
                    Data = body.Data,
                    EventData = body.EventData,
                    FnIndex = body.FnIndex,
                    TriggerId = body.TriggerId,
                    SessionHash = body.SessionHash,
                    EventId = body.EventId,
                    ApiName = body.ApiName,
                    Batched = body.Batched,
                    Request = context.Request,
                    SimpleFormat = body.SimpleFormat
                };

                // Fallback compatibility: if a manual Flag click is routed to /run/predict
                // and payload already contains [inputs..., outputs...], persist flag data.
                TryPersistManualFlagFallback(appInstance, apiName, bodyInternal, appInstance.GetCurrentUser(context.Request));

                var fn = RouteUtils.GetFn(appInstance.GetBlocks(), apiName, bodyInternal);

                // Check if API is open for direct execution
                if (!appInstance.GetBlocks().ApiOpen && fn.Queue)
                {
                    return Results.NotFound(new
                    {
                        detail = "This API endpoint does not accept direct HTTP POST requests. Please join the queue to use this API."
                    });
                }

                var username = appInstance.GetCurrentUser(context.Request);
                var grRequest = RouteUtils.CompileGrRequest(
                    bodyInternal,
                    fn,
                    username,
                    context.Request
                );

                var rootPath = RouteUtils.GetRootUrl(
                    context.Request,
                    context.Request.Path.Value ?? string.Empty,
                    appInstance.GetBlocks().RootPath ?? ""
                );

                try
                {
                    var output = await RouteUtils.CallProcessApi(
                        appInstance,
                        bodyInternal,
                        grRequest,
                        fn,
                        rootPath
                    );

                    return Results.Json(output, JsonOptions);
                }
                catch (Exception error)
                {
                    var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    var logger = loggerFactory?.CreateLogger("Gradio.Net.HandlePredict");

                    string serializedData;
                    string serializedEventData;
                    try
                    {
                        serializedData = JsonSerializer.Serialize(bodyInternal.Data ?? new List<object>());
                    }
                    catch
                    {
                        serializedData = "<failed to serialize data>";
                    }

                    try
                    {
                        serializedEventData = JsonSerializer.Serialize(bodyInternal.EventData);
                    }
                    catch
                    {
                        serializedEventData = "<failed to serialize event_data>";
                    }

                    logger?.LogError(
                        error,
                        "HandlePredict failed. api={ApiName} fn_index={FnIndex} trigger_id={TriggerId} session_hash={SessionHash} data_count={DataCount} data={Data} event_data={EventData}",
                        apiName,
                        bodyInternal.FnIndex,
                        bodyInternal.TriggerId,
                        bodyInternal.SessionHash,
                        bodyInternal.Data?.Count ?? 0,
                        serializedData,
                        serializedEventData
                    );

                    Console.Error.WriteLine(
                        $"[Gradio.Net][HandlePredict] 500 api={apiName} fn_index={bodyInternal.FnIndex} trigger_id={bodyInternal.TriggerId} session_hash={bodyInternal.SessionHash} data_count={bodyInternal.Data?.Count ?? 0}"
                    );
                    Console.Error.WriteLine($"[Gradio.Net][HandlePredict] data={serializedData}");
                    Console.Error.WriteLine($"[Gradio.Net][HandlePredict] event_data={serializedEventData}");
                    Console.Error.WriteLine($"[Gradio.Net][HandlePredict] exception={error}");

                    var errorPayload = Utils.Utils.ErrorPayload(error, appInstance.GetBlocks().ShowError);
                    return Results.Json(errorPayload, JsonOptions, statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error processing prediction: {ex.Message}");
            }
        }

        private static void TryPersistManualFlagFallback(App appInstance, string apiName, Data.PredictBodyInternal bodyInternal, string? username)
        {
            try
            {
                if (!string.Equals(apiName, "predict", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (appInstance.GetBlocks() is not global::Gradio.Net.Interface iface)
                {
                    return;
                }

                if (!string.Equals(iface.FlaggingMode, "manual", StringComparison.OrdinalIgnoreCase) || iface.FlaggingCallback == null)
                {
                    return;
                }

                var inputCount = iface.InputComponents?.Count ?? 0;
                var outputCount = iface.OutputComponents?.Count ?? 0;
                var expectedCount = inputCount + outputCount;
                if (expectedCount <= 0)
                {
                    return;
                }

                if (bodyInternal.Data == null || bodyInternal.Data.Count < expectedCount)
                {
                    return;
                }

                var flagData = bodyInternal.Data.Take(expectedCount).ToList();
                iface.FlaggingCallback.Flag(flagData, null, username);
            }
            catch
            {
                // Best-effort fallback; prediction flow must not be interrupted.
            }
        }

        private static Stream? GetEmbeddedResourceStream(string resourceName)
        {
            var assembly = typeof(App).Assembly;
            return assembly.GetManifestResourceStream(resourceName);
        }

        private static Dictionary<string, object> ReorderConfigLikePython(Dictionary<string, object> config)
        {
            var orderedKeys = new[]
            {
                "version", "api_prefix", "mode", "app_id", "dev_mode", "vibe_mode", "analytics_enabled",
                "components", "css", "connect_heartbeat", "js", "head", "title", "space_id", "enable_queue",
                "show_error", "footer_links", "is_colab", "max_file_size", "stylesheets", "theme", "protocol",
                "body_css", "fill_height", "fill_width", "theme_hash", "pwa", "pages", "page", "mcp_server",
                "i18n_translations", "dependencies", "layout", "username", "deep_link_state", "current_page", "root"
            };

            var result = new Dictionary<string, object>();
            foreach (var key in orderedKeys)
            {
                if (config.TryGetValue(key, out var value))
                {
                    result[key] = value;
                }
            }

            foreach (var kvp in config)
            {
                if (!result.ContainsKey(kvp.Key))
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        private static Dictionary<string, object> ReorderApiInfoLikePython(Dictionary<string, object> apiInfo)
        {
            var result = new Dictionary<string, object>();

            var namedOrdered = new Dictionary<string, object>();
            if (apiInfo.TryGetValue("named_endpoints", out var namedObj) && namedObj is Dictionary<string, object> named)
            {
                foreach (var ep in named)
                {
                    if (ep.Value is Dictionary<string, object> epDict)
                    {
                        var endpoint = new Dictionary<string, object>();
                        if (epDict.TryGetValue("parameters", out var p)) endpoint["parameters"] = p;
                        if (epDict.TryGetValue("returns", out var r)) endpoint["returns"] = r;
                        if (epDict.TryGetValue("api_visibility", out var v)) endpoint["api_visibility"] = v;
                        if (epDict.TryGetValue("description", out var d)) endpoint["description"] = d;
                        foreach (var kv in epDict)
                        {
                            if (!endpoint.ContainsKey(kv.Key)) endpoint[kv.Key] = kv.Value;
                        }
                        namedOrdered[ep.Key] = endpoint;
                    }
                    else
                    {
                        namedOrdered[ep.Key] = ep.Value;
                    }
                }
            }

            result["named_endpoints"] = namedOrdered;

            if (apiInfo.TryGetValue("unnamed_endpoints", out var unnamedObj))
            {
                result["unnamed_endpoints"] = unnamedObj;
            }
            else
            {
                result["unnamed_endpoints"] = new Dictionary<string, object>();
            }

            foreach (var kv in apiInfo)
            {
                if (!result.ContainsKey(kv.Key))
                {
                    result[kv.Key] = kv.Value;
                }
            }

            return result;
        }

        private static IResult ServeEmbeddedResource(string prefix, string path)
        {
            string normalized = path.Replace("\\", "/").TrimStart('/');
            string resourceName = $"{prefix}.{normalized.Replace("/", ".")}";
            var stream = GetEmbeddedResourceStream(resourceName);
            if (stream == null)
            {
                return Results.NotFound();
            }

            string contentType = ContentTypeProvider.TryGetContentType(normalized, out var ct) ? ct : "application/octet-stream";
            return Results.File(stream, contentType, enableRangeProcessing: true);
        }

        private static void TrySignalQueueEvent(Queueing.QueueEvent queueEvent)
        {
            try
            {
                if (queueEvent.Signal.CurrentCount == 0)
                {
                    queueEvent.Signal.Release();
                }
            }
            catch (SemaphoreFullException)
            {
                // Stream updates can arrive faster than consumers drain the signal.
                // Ignore over-release to match "latest frame wins" behavior.
            }
        }

        private static bool IsAllowedFile(string absPath, Blocks blocks)
        {
            var blockedPaths = blocks.BlockedPaths ?? new List<string>();
            var allowedPaths = blocks.AllowedPaths ?? new List<string>();

            foreach (var blocked in blockedPaths)
            {
                if (Utils.Utils.IsInOrEqual(absPath, blocked))
                {
                    return false;
                }
            }

            foreach (var allowed in allowedPaths)
            {
                if (Utils.Utils.IsInOrEqual(absPath, allowed))
                {
                    return true;
                }
            }

            if (Data.StaticFiles.AllPaths.Any(p => Utils.Utils.IsInOrEqual(absPath, p)))
            {
                return true;
            }

            string uploadDir = Utils.Utils.GetUploadFolder();
            string cacheDir = Utils.Utils.GetCacheFolder();
            if (Utils.Utils.IsInOrEqual(absPath, uploadDir) || Utils.Utils.IsInOrEqual(absPath, cacheDir))
            {
                return true;
            }

            return false;
        }

        private string GetCurrentUser(HttpRequest request)
        {
            if (AuthDependency != null)
            {
                return AuthDependency(request);
            }

            string token = request.Cookies.TryGetValue($"access-token-{CookieId}", out string token1) ? token1 :
                           request.Cookies.TryGetValue($"access-token-unsecure-{CookieId}", out string token2) ? token2 : null;

            return token != null && Tokens.TryGetValue(token, out string user) ? user : null;
        }

        private static async Task<IResult> HandleMainPage(HttpContext context, App appInstance, string page)
        {
            var user = appInstance.GetCurrentUser(context.Request);
            var deepLink = context.Request.Query["deep_link"].ToString();
            var blocks = appInstance.GetBlocks();

            var root = RouteUtils.GetRootUrl(
                context.Request,
                $"/{page}",
                appInstance.GetBlocks().RootPath ?? blocks.CustomMountPath
            );

            Dictionary<string, object> config;

            // Check if user is authenticated or auth is not required
            if ((appInstance.Auth == null && appInstance.AuthDependency == null) || user != null)
            {
                config = BuildUpdatedConfig(context, appInstance, $"/{page}", deepLink, page ?? "");
                var deepLinkState = config.TryGetValue("deep_link_state", out var dls)
                    ? dls?.ToString() ?? "none"
                    : "none";

                // Filter components for this page
                var allComponents = config.TryGetValue("components", out var comps)
                    ? ToObjectList(comps)
                    : new List<object>();

                var pageConfig = config.TryGetValue("page", out var pageObj)
                    ? pageObj as Dictionary<string, object>
                    : new Dictionary<string, object>();

                var currentPageConfig = pageConfig?.TryGetValue(page ?? "", out var currentPage) == true
                    ? currentPage as Dictionary<string, object>
                    : new Dictionary<string, object>();

                var pageComponentIds = currentPageConfig?.TryGetValue("components", out var pageComps) == true
                    ? ToObjectList(pageComps)
                    : new List<object>();

                var components = allComponents.Where(c =>
                {
                    if (c is Dictionary<string, object> compDict && compDict.TryGetValue("id", out var id))
                    {
                        return LooseContains(pageComponentIds, id);
                    }
                    return false;
                }).ToList();

                config["username"] = user;
                config["deep_link_state"] = deepLinkState;
                config["components"] = components;

                // Filter dependencies for this page
                var allDependencies = config.TryGetValue("dependencies", out var deps)
                    ? ToObjectList(deps)
                    : new List<object>();

                var pageDependencyIds = currentPageConfig?.TryGetValue("dependencies", out var pageDeps) == true
                    ? ToObjectList(pageDeps)
                    : new List<object>();

                var dependencies = allDependencies.Where(d =>
                {
                    if (d is Dictionary<string, object> depDict && depDict.TryGetValue("id", out var id))
                    {
                        return LooseContains(pageDependencyIds, id);
                    }
                    return false;
                }).ToList();

                config["dependencies"] = dependencies;
                config["layout"] = currentPageConfig?.TryGetValue("layout", out var layout) == true
                    ? layout
                    : new Dictionary<string, object>();
                config["current_page"] = page ?? "";

                // Update root in config
                config = ProcessingUtils.AddRootUrl(config, root) as Dictionary<string, object> ?? config;
            }
            else if (appInstance.AuthDependency != null)
            {
                return Results.Unauthorized();
            }
            else
            {
                // Show auth page
                config = new Dictionary<string, object>
                {
                    ["auth_required"] = true,
                    ["auth_message"] = blocks.AuthMessage,
                    ["space_id"] = blocks.SpaceId,
                    ["root"] = root,
                    ["page"] = new Dictionary<string, object> { [""] = new Dictionary<string, object> { ["layout"] = new Dictionary<string, object>() } },
                    ["pages"] = new List<string> { "" },
                    ["components"] = new List<object>(),
                    ["dependencies"] = new List<object>(),
                    ["current_page"] = ""
                };
            }

            // Render template
            string templateResource = blocks.Share
                ? "Gradio.Net.Templates.frontend.share.html"
                : "Gradio.Net.Templates.frontend.index.html";

            var stream = GetEmbeddedResourceStream(templateResource);
            if (stream == null)
            {
                return Results.Problem("Template not found.");
            }

            using var reader = new StreamReader(stream);
            var template = await reader.ReadToEndAsync();
            template = ApplyBodyCssTemplateValues(template, config);

            config = ReorderConfigLikePython(config);
            if (appInstance.ApiInfo == null)
            {
                var rawApiInfo = appInstance.GetBlocks().GetApiInfo();
                rawApiInfo = RouteUtils.UpdateExampleValuesToUsePublicUrl(rawApiInfo);
                RouteUtils.AddCodeSnippetsToApiInfo(rawApiInfo, root);
                appInstance.ApiInfo = rawApiInfo;
            }
            var apiInfo = ReorderApiInfoLikePython(appInstance.ApiInfo);
            var templateContext = new Dictionary<string, object>
            {
                ["config"] = config,
                ["gradio_api_info"] = apiInfo
            };

            var html = TemplateEngine.Render(template, templateContext)
                .Replace("\r\n", "\n")
                .TrimEnd('\r', '\n');
            return Results.Text(html, "text/html");
        }

        private static string ApplyBodyCssTemplateValues(string template, Dictionary<string, object> config)
        {
            static string GetFromDictionaryObject(object? source, string key, string fallback)
            {
                if (source is Dictionary<string, object> dictObj && dictObj.TryGetValue(key, out var valueObj) && valueObj != null)
                {
                    return valueObj.ToString() ?? fallback;
                }

                if (source is IDictionary<string, string> dictString && dictString.TryGetValue(key, out var valueString) && !string.IsNullOrEmpty(valueString))
                {
                    return valueString;
                }

                if (source is IDictionary dictionary && dictionary.Contains(key) && dictionary[key] != null)
                {
                    return dictionary[key]?.ToString() ?? fallback;
                }

                return fallback;
            }

            object? bodyCss = null;
            if (config.TryGetValue("body_css", out var bodyCssObj))
            {
                bodyCss = bodyCssObj;
            }

            var bg = GetFromDictionaryObject(bodyCss, "body_background_fill", "white");
            var col = GetFromDictionaryObject(bodyCss, "body_text_color", "#1f2937");
            var bgDark = GetFromDictionaryObject(bodyCss, "body_background_fill_dark", "#0b0f19");
            var colDark = GetFromDictionaryObject(bodyCss, "body_text_color_dark", "#f3f4f6");

            return template
                .Replace("{{ config.get('body_css', {}).get('body_background_fill', 'white') }}", bg)
                .Replace("{{ config.get('body_css', {}).get('body_text_color', '#1f2937') }}", col)
                .Replace("{{ config.get('body_css', {}).get('body_background_fill_dark', '#0b0f19') }}", bgDark)
                .Replace("{{ config.get('body_css', {}).get('body_text_color_dark', '#f3f4f6') }}", colDark);
        }

        private static (List<object> Components, string DeepLinkState) LoadDeepLink(
            string deepLink,
            Dictionary<string, object> config,
            string page,
            string uploadedFileDir)
        {
            var components = config.TryGetValue("components", out var comps)
                ? ToObjectList(comps)
                : new List<object>();

            try
            {
                var path = Path.Combine(uploadedFileDir, "deep_links", deepLink, "state.json");

                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    components = JsonSerializer.Deserialize<List<object>>(json) ?? new List<object>();
                    var deepLinkState = "valid";

                    // Filter components by page if specified
                    if (!string.IsNullOrEmpty(page) && config.TryGetValue("page", out var pageObj))
                    {
                        if (pageObj is Dictionary<string, object> pages &&
                            pages.TryGetValue(page, out var pageConfig) &&
                            pageConfig is Dictionary<string, object> pageDict &&
                            pageDict.TryGetValue("components", out var pageComponents) &&
                            pageComponents != null)
                        {
                            var pageComponentsList = ToObjectList(pageComponents);
                            var pageComponentIds = new HashSet<object>(pageComponentsList);
                            components = components.Where(c =>
                            {
                                if (c is Dictionary<string, object> compDict &&
                                    compDict.TryGetValue("id", out var id))
                                {
                                    return LooseContains(pageComponentIds.ToList(), id);
                                }
                                return false;
                            }).ToList();
                        }
                    }

                    return (components, deepLinkState);
                }
                else
                {
                    return (new List<object>(), "invalid");
                }
            }
            catch (Exception)
            {
                return (new List<object>(), "invalid");
            }
        }

        private static Dictionary<string, object> BuildUpdatedConfig(
            HttpContext context,
            App appInstance,
            string routePath,
            string deepLink,
            string pageForDeepLink)
        {
            var blocks = appInstance.GetBlocks();
            // Python parity: /config should be served from the cached app config,
            // then per-request fields are applied to a copy.
            var baseConfig = blocks.Config ?? blocks.GetConfigFile();
            var config = DeepCloneConfigDictionary(baseConfig);

            config["username"] = appInstance.GetCurrentUser(context.Request);

            if (!string.IsNullOrEmpty(deepLink))
            {
                var (deepLinkComponents, deepLinkState) = LoadDeepLink(
                    deepLink,
                    config,
                    pageForDeepLink ?? "",
                    appInstance.UploadedFileDir
                );
                config["components"] = deepLinkComponents;
                config["deep_link_state"] = deepLinkState;
            }

            string root = RouteUtils.GetRootUrl(
                context.Request,
                routePath,
                blocks.RootPath ?? blocks.CustomMountPath ?? ""
            );
            config["root"] = root;

            return ProcessingUtils.AddRootUrl(config, root) as Dictionary<string, object> ?? config;
        }

        private static Dictionary<string, object> DeepCloneConfigDictionary(Dictionary<string, object>? source)
        {
            if (source == null)
            {
                return new Dictionary<string, object>();
            }

            var result = new Dictionary<string, object>(source.Count);
            foreach (var kvp in source)
            {
                result[kvp.Key] = DeepCloneConfigValue(kvp.Value)!;
            }
            return result;
        }

        private static object? DeepCloneConfigValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is double d)
            {
                if (double.IsNaN(d)) return "NaN";
                if (double.IsPositiveInfinity(d)) return "Infinity";
                if (double.IsNegativeInfinity(d)) return "-Infinity";
                return d;
            }

            if (value is float f)
            {
                if (float.IsNaN(f)) return "NaN";
                if (float.IsPositiveInfinity(f)) return "Infinity";
                if (float.IsNegativeInfinity(f)) return "-Infinity";
                return f;
            }

            if (value is string || value is bool || value is char ||
                value is byte || value is sbyte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong ||
                value is decimal ||
                value is DateTime || value is DateTimeOffset || value is Guid)
            {
                return value;
            }

            if (value is Dictionary<string, object> dict)
            {
                return DeepCloneConfigDictionary(dict);
            }

            if (value is IDictionary genericDict)
            {
                var cloned = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in genericDict)
                {
                    if (entry.Key == null)
                    {
                        continue;
                    }
                    cloned[entry.Key.ToString() ?? string.Empty] = DeepCloneConfigValue(entry.Value)!;
                }
                return cloned;
            }

            if (value is List<object> list)
            {
                return list.Select(DeepCloneConfigValue).ToList();
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var clonedList = new List<object?>();
                foreach (var item in enumerable)
                {
                    clonedList.Add(DeepCloneConfigValue(item));
                }
                return clonedList;
            }

            // Fallback: immutable/POCO values are safe to pass through.
            return value;
        }

        private static List<object> ToObjectList(object value)
        {
            if (value is List<object> list)
            {
                return list;
            }

            if (value is IEnumerable<object> enumerable && value is not string)
            {
                return enumerable.ToList();
            }

            if (value is IEnumerable nonGeneric && value is not string)
            {
                var result = new List<object>();
                foreach (var item in nonGeneric)
                {
                    result.Add(item!);
                }
                return result;
            }

            return new List<object>();
        }

        private static bool LooseContains(List<object> values, object id)
        {
            if (values.Contains(id))
            {
                return true;
            }

            var idString = id?.ToString();
            return values.Any(v => string.Equals(v?.ToString(), idString, StringComparison.Ordinal));
        }

        internal static void RegisterMonitoringRoutes(WebApplication app, Blocks blocks, App appInstance)
        {
            // Monitoring dashboard login route
            app.MapGet("/monitoring", (HttpContext context) =>
            {
                var instance = context.RequestServices.GetRequiredService<App>();
                if (!(blocks.EnableMonitoring ?? false))
                {
                    return Results.Problem("Monitoring is not enabled.", statusCode: 403);
                }

                var rootUrl = RouteUtils.GetRootUrl(
                    context.Request,
                    "/api/monitoring",
                    app.GetType().GetProperty("RootPath")?.GetValue(app) as string ?? string.Empty
                );
                var monitoringUrl = $"{rootUrl}/monitoring/{instance.AnalyticsKey}";

                return Results.Text("See console for monitoring URL.", "text/html");
            });

            // Monitoring summary endpoint
            app.MapGet("/monitoring/summary", GetMonitoringSummary);

            // Monitoring dashboard with authentication
            app.MapGet("/monitoring/{key}", (HttpContext context) =>
            {
                var instance = context.RequestServices.GetRequiredService<App>();
                var key = context.Request.RouteValues["key"]?.ToString() ?? string.Empty;

                if (!(blocks.EnableMonitoring ?? false))
                {
                    return Results.Problem("Monitoring is not enabled.", statusCode: 403);
                }

                if (!ComparePasswordsSecurely(key, instance.AnalyticsKey))
                {
                    return Results.Problem("Invalid key.", statusCode: 403);
                }

                var analyticsUrl = $"/monitoring/{instance.AnalyticsKey}/dashboard";

                if (!instance.MonitoringEnabled)
                {
                    // In C#, we would mount a monitoring dashboard app here
                    // For now, return a placeholder
                    instance.MonitoringEnabled = true;
                }

                return Results.Redirect(analyticsUrl, false, false);
            });
        }

        private static IResult GetMonitoringSummary(HttpContext context)
        {
            var instance = context.RequestServices.GetRequiredService<App>();
            var summary = instance.QueueManager?.GetCachedEventAnalyticsSummary()
                          ?? new Dictionary<string, object> { ["functions"] = new Dictionary<string, object>() };
            return Results.Json(summary);
        }

        internal static void RegisterProcessRecordingRoute(WebApplication app, Blocks blocks, App appInstance)
        {
            app.MapPost("/process_recording", async (HttpContext context, [FromServices] App instance) =>
            {
                try
                {
                    if (!context.Request.HasFormContentType)
                    {
                        return Results.BadRequest(new { detail = "Invalid content type." });
                    }

                    var form = await context.Request.ReadFormAsync();
                    var videoFiles = form.Files.GetFiles("video");

                    if (videoFiles == null || !videoFiles.Any())
                    {
                        return Results.BadRequest(new { detail = "No video file provided" });
                    }

                    var videoFile = videoFiles.First();
                    var params_ = new ProcessRecordingParams();

                    // Extract parameters
                    if (form.TryGetValue("remove_segment_start", out var startVal) &&
                        double.TryParse(startVal, out var startTime))
                    {
                        params_.RemoveSegmentStart = startTime;
                    }

                    if (form.TryGetValue("remove_segment_end", out var endVal) &&
                        double.TryParse(endVal, out var endTime))
                    {
                        params_.RemoveSegmentEnd = endTime;
                    }

                    if (form.TryGetValue("zoom_effects", out var zoomJson))
                    {
                        try
                        {
                            params_.ZoomEffects = JsonSerializer.Deserialize<List<ZoomEffect>>(zoomJson.ToString());
                        }
                        catch
                        {
                            params_.ZoomEffects = new List<ZoomEffect>();
                        }
                    }

                    // Save uploaded file
                    var tempDir = Path.GetTempPath();
                    var inputPath = Path.Combine(tempDir, $"{Guid.NewGuid()}.mp4");

                    await using (var fileStream = new FileStream(inputPath, FileMode.Create))
                    {
                        await videoFile.CopyToAsync(fileStream);
                    }

                    // Check if ffmpeg is available
                    var ffmpegPath = FindExecutable("ffmpeg");
                    if (string.IsNullOrEmpty(ffmpegPath))
                    {
                        // Return original file if ffmpeg is not available
                        return Results.File(inputPath, "video/mp4", "gradio-screen-recording.mp4");
                    }

                    var outputPath = Path.Combine(tempDir, $"{Guid.NewGuid()}_processed.mp4");

                    try
                    {
                        var (processedPath, tempFiles) = await ProcessVideoWithFfmpeg(inputPath, outputPath, params_);

                        // Return processed file
                        return Results.File(processedPath, "video/mp4", "gradio-screen-recording.mp4");
                    }
                    catch
                    {
                        // Return original file on error
                        return Results.File(inputPath, "video/mp4", "gradio-screen-recording.mp4");
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error processing recording: {ex.Message}");
                }
            });
        }

        internal static void RegisterVibeEditorRoutes(WebApplication app, Blocks blocks, App appInstance)
        {
            if (!blocks.VibeMode)
            {
                return; // Only register if vibe mode is enabled
            }

            var vibeEditHistoryDir = Path.Combine(Path.GetTempPath(), "vibe_edit_history");
            Directory.CreateDirectory(vibeEditHistoryDir);

            var chatHistory = new Dictionary<string, string> { ["history"] = string.Empty };
            var hashToChatHistory = new Dictionary<string, string>();

            // Vibe edit endpoint
            app.MapPost("/vibe-edit/", async ([FromServices] App instance, [FromBody] VibeEditBody body, HttpContext context) =>
            {
                return await HandleVibeEdit(context, body, blocks, vibeEditHistoryDir, chatHistory, hashToChatHistory);
            });

            app.MapPost("/vibe-edit", async ([FromServices] App instance, [FromBody] VibeEditBody body, HttpContext context) =>
            {
                return await HandleVibeEdit(context, body, blocks, vibeEditHistoryDir, chatHistory, hashToChatHistory);
            });

            // Undo vibe edit endpoint
            app.MapPost("/undo-vibe-edit/", async ([FromServices] App instance, [FromBody] UndoVibeEditBody body, HttpContext context) =>
            {
                return await HandleUndoVibeEdit(context, body, blocks, vibeEditHistoryDir, chatHistory, hashToChatHistory);
            });

            app.MapPost("/undo-vibe-edit", async ([FromServices] App instance, [FromBody] UndoVibeEditBody body, HttpContext context) =>
            {
                return await HandleUndoVibeEdit(context, body, blocks, vibeEditHistoryDir, chatHistory, hashToChatHistory);
            });

            // Get vibe code endpoint
            app.MapGet("/vibe-code/", async ([FromServices] App instance, HttpContext context) =>
            {
                return await GetVibeCode(context, blocks);
            });

            app.MapGet("/vibe-code", async ([FromServices] App instance, HttpContext context) =>
            {
                return await GetVibeCode(context, blocks);
            });

            // Update vibe code endpoint
            app.MapPost("/vibe-code/", async ([FromServices] App instance, [FromBody] VibeCodeBody body, HttpContext context) =>
            {
                return await UpdateVibeCode(context, body, blocks);
            });

            app.MapPost("/vibe-code", async ([FromServices] App instance, [FromBody] VibeCodeBody body, HttpContext context) =>
            {
                return await UpdateVibeCode(context, body, blocks);
            });

            // Vibe starter queries endpoint
            app.MapPost("/vibe-starter-queries/", async ([FromServices] App instance, HttpContext context) =>
            {
                return await GetVibeStarterQueries(context, blocks);
            });

            app.MapPost("/vibe-starter-queries", async ([FromServices] App instance, HttpContext context) =>
            {
                return await GetVibeStarterQueries(context, blocks);
            });
        }

        private static async Task<IResult> HandleVibeEdit(
            HttpContext context,
            VibeEditBody body,
            Blocks blocks,
            string vibeEditHistoryDir,
            Dictionary<string, string> chatHistory,
            Dictionary<string, string> hashToChatHistory)
        {
            if (!blocks.VibeMode)
            {
                return Results.Problem("Vibe editor is not enabled. Use --vibe flag to enable.", statusCode: 403);
            }

            try
            {
                // Read current code
                var demoPath = GetWatchDemoPath();
                if (string.IsNullOrEmpty(demoPath) || !File.Exists(demoPath))
                {
                    return Results.Problem("Demo file not found.", statusCode: 404);
                }

                var originalCode = await File.ReadAllTextAsync(demoPath);

                // Create snapshot
                var snapshotHash = GenerateRandomToken(32);
                var snapshotFile = Path.Combine(vibeEditHistoryDir, $"{snapshotHash}.py");
                await File.WriteAllTextAsync(snapshotFile, originalCode);

                hashToChatHistory[snapshotHash] = chatHistory["history"];

                // Generate new code using AI (simplified implementation)
                // In production, this would call an AI service
                var newCode = await GenerateCodeWithAI(originalCode, body.Prompt, chatHistory["history"]);

                // Update chat history
                chatHistory["history"] += $"\nUser: {body.Prompt}\nAssistant: {newCode}\n";

                // Extract reasoning and code
                var (reasoning, code) = SplitReasoningCode(newCode);

                // Calculate diff stats
                var (linesAdded, linesRemoved) = CalculateDiffStats(originalCode, code);

                // Write new code
                await File.WriteAllTextAsync(demoPath, code);

                return Results.Json(new VibeEditResponse
                {
                    Hash = snapshotHash,
                    DiffStats = new DiffStats
                    {
                        LinesAdded = linesAdded,
                        LinesRemoved = linesRemoved
                    },
                    Reasoning = reasoning
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error generating code: {ex.Message}", statusCode: 500);
            }
        }

        private static async Task<IResult> HandleUndoVibeEdit(
            HttpContext context,
            UndoVibeEditBody body,
            Blocks blocks,
            string vibeEditHistoryDir,
            Dictionary<string, string> chatHistory,
            Dictionary<string, string> hashToChatHistory)
        {
            if (!blocks.VibeMode)
            {
                return Results.Problem("Vibe editor is not enabled. Use --vibe flag to enable.", statusCode: 403);
            }

            var snapshotFile = Path.Combine(vibeEditHistoryDir, $"{body.Hash}.py");

            if (!File.Exists(snapshotFile))
            {
                return Results.Problem("Snapshot not found", statusCode: 404);
            }

            try
            {
                // Restore code from snapshot
                var savedContent = await File.ReadAllTextAsync(snapshotFile);
                var demoPath = GetWatchDemoPath();

                if (string.IsNullOrEmpty(demoPath))
                {
                    return Results.Problem("Demo file path not configured.", statusCode: 500);
                }

                await File.WriteAllTextAsync(demoPath, savedContent);

                // Restore chat history
                if (hashToChatHistory.TryGetValue(body.Hash, out var restoredHistory))
                {
                    chatHistory["history"] = restoredHistory;
                }

                return Results.Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error restoring snapshot: {ex.Message}", statusCode: 500);
            }
        }

        private static async Task<IResult> GetVibeCode(HttpContext context, Blocks blocks)
        {
            if (!blocks.VibeMode)
            {
                return Results.Problem("Vibe editor is not enabled. Use --vibe flag to enable.", statusCode: 403);
            }

            try
            {
                var demoPath = GetWatchDemoPath();
                if (string.IsNullOrEmpty(demoPath) || !File.Exists(demoPath))
                {
                    return Results.Problem("Demo file not found", statusCode: 404);
                }

                var code = await File.ReadAllTextAsync(demoPath);
                return Results.Json(new VibeCodeResponse { Code = code });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error reading file: {ex.Message}", statusCode: 500);
            }
        }

        private static async Task<IResult> UpdateVibeCode(HttpContext context, VibeCodeBody body, Blocks blocks)
        {
            if (!blocks.VibeMode)
            {
                return Results.Problem("Vibe editor is not enabled. Use --vibe flag to enable.", statusCode: 403);
            }

            try
            {
                var demoPath = GetWatchDemoPath();
                if (string.IsNullOrEmpty(demoPath))
                {
                    return Results.Problem("Demo file path not configured.", statusCode: 500);
                }

                await File.WriteAllTextAsync(demoPath, body.Code);
                return Results.Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error writing file: {ex.Message}", statusCode: 500);
            }
        }

        private static async Task<IResult> GetVibeStarterQueries(HttpContext context, Blocks blocks)
        {
            if (!blocks.VibeMode)
            {
                return Results.Problem("Vibe editor is not enabled. Use --vibe flag to enable.", statusCode: 403);
            }

            try
            {
                var demoPath = GetWatchDemoPath();
                if (string.IsNullOrEmpty(demoPath) || !File.Exists(demoPath))
                {
                    return Results.Problem("Demo file not found", statusCode: 404);
                }

                var code = await File.ReadAllTextAsync(demoPath);

                // Generate starter queries based on code (simplified implementation)
                var starterQueries = new List<string>
                {
                    "Add a title to the app",
                    "Add examples",
                    "Change the theme"
                };

                return Results.Json(new VibeStarterQueriesResponse { StarterQueries = starterQueries });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error generating starter queries: {ex.Message}", statusCode: 500);
            }
        }

        private static string GetWatchDemoPath()
        {
            // This would be configured in the application settings
            // For now, return a default path
            return Environment.GetEnvironmentVariable("GRADIO_WATCH_DEMO_PATH")
                   ?? Path.Combine(Path.GetTempPath(), "gradio_demo.py");
        }

        private static async Task<string> GenerateCodeWithAI(string originalCode, string prompt, string history)
        {
            // This is a placeholder. In production, this would call an AI service
            // such as OpenAI GPT or similar
            await Task.CompletedTask;

            return $"<reasoning>\nGenerated code based on prompt: {prompt}\n</reasoning>\n\n{originalCode}";
        }

        private static (string Reasoning, string Code) SplitReasoningCode(string text)
        {
            var reasoningPattern = new System.Text.RegularExpressions.Regex(
                @"<\s*reasoning\s*>\s*(?<body>.*?)\s*<\s*/\s*reasoning\s*>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline
            );

            var matches = reasoningPattern.Matches(text);
            var reasoning = string.Join("\n\n", matches.Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Groups["body"].Value.Trim()));

            var code = reasoningPattern.Replace(text, string.Empty).Trim();

            // Extract code from markdown if present
            if (code.Contains("```python\n"))
            {
                var start = code.IndexOf("```python\n") + "```python\n".Length;
                var end = code.IndexOf("\n```", start);
                code = end != -1 ? code.Substring(start, end - start) : code.Substring(start);
            }

            return (reasoning, code);
        }

        private static (int LinesAdded, int LinesRemoved) CalculateDiffStats(string originalCode, string newCode)
        {
            var originalLines = originalCode.Split('\n');
            var newLines = newCode.Split('\n');

            // Simple diff calculation
            var originalSet = new HashSet<string>(originalLines);
            var newSet = new HashSet<string>(newLines);

            var linesAdded = newLines.Count(line => !originalSet.Contains(line));
            var linesRemoved = originalLines.Count(line => !newSet.Contains(line));

            return (linesAdded, linesRemoved);
        }

        private static async Task<(string ProcessedPath, List<string> TempFiles)> ProcessVideoWithFfmpeg(
            string inputPath,
            string outputPath,
            ProcessRecordingParams params_)
        {
            var tempFiles = new List<string> { inputPath };

            // Build ffmpeg command
            var args = new List<string>();
            args.Add("-i");
            args.Add($"\"{inputPath}\"");

            // Add segment removal if specified
            if (params_.RemoveSegmentStart.HasValue && params_.RemoveSegmentEnd.HasValue)
            {
                var duration = params_.RemoveSegmentEnd.Value - params_.RemoveSegmentStart.Value;
                args.Add("-vf");
                args.Add($"\"select='not(between(t,{params_.RemoveSegmentStart},{params_.RemoveSegmentEnd}))'\"");
            }

            // Add zoom effects if specified
            if (params_.ZoomEffects != null && params_.ZoomEffects.Any())
            {
                // Simplified zoom effect implementation
                var zoomFilter = string.Join(",", params_.ZoomEffects.Select(z =>
                    $"scale=iw*{z.Scale}:ih*{z.Scale}"));
                args.Add("-vf");
                args.Add($"\"{zoomFilter}\"");
            }

            args.Add($"\"{outputPath}\"");

            // Execute ffmpeg
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = string.Join(" ", args),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            tempFiles.Add(outputPath);

            return (outputPath, tempFiles);
        }

        private static string FindExecutable(string name)
        {
            var pathVar = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathVar))
            {
                return null;
            }

            var paths = pathVar.Split(Path.PathSeparator);
            foreach (var path in paths)
            {
                var fullPath = Path.Combine(path, name);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }

                fullPath += ".exe";
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        private static bool ComparePasswordsSecurely(string a, string b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }

}

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


public static class RouteUtils
{
    // Python Gradio 6.x uses API_PREFIX = "/gradio_api".
    public const string ApiPrefix = "/gradio_api";

    public static BlockFunction GetFn(Blocks blocks, string apiName, Data.PredictBody body)
    {
        Dictionary<int, BlockFunction> fns;

        // Python behavior: if session_hash is present, use the per-session blocks_config.fns
        if (!string.IsNullOrEmpty(body.SessionHash))
        {
            var sessionState = blocks.StateHolder[body.SessionHash];
            if (sessionState.BlocksConfig is BlocksConfig sessionBlocksConfig)
            {
                fns = sessionBlocksConfig.Fns;
            }
            else
            {
                fns = blocks.DefaultConfig.Fns;
            }
        }
        else
        {
            fns = blocks.DefaultConfig.Fns;
        }

        if (body.FnIndex == null)
        {
            if (apiName != null)
            {
                foreach (var fn in fns.Values)
                {
                    if (fn.ApiName == apiName)
                    {
                        return fn;
                    }
                }
            }
            throw new FnIndexInferError($"Could not infer function index for API name: {apiName}");
        }
        else
        {
            if (fns.TryGetValue(body.FnIndex.Value, out var fn))
            {
                return fn;
            }
            throw new KeyNotFoundException($"Function with index {body.FnIndex} not found");
        }
    }

    public static Request SnapshotRequest(HttpRequest httpRequest, string? username, string? sessionHash)
    {
        if (httpRequest == null)
        {
            return new Request(request: null, username: username, sessionHash: sessionHash);
        }

        var headers = httpRequest.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        var queryParams = httpRequest.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        var cookies = httpRequest.Cookies
            .ToDictionary(c => c.Key, c => c.Value);

        object client = null;
        try
        {
            var conn = httpRequest.HttpContext?.Connection;
            if (conn != null)
            {
                client = new { Host = conn.RemoteIpAddress?.ToString(), Port = conn.RemotePort };
            }
        }
        catch { /* best-effort */ }

        string url = null;
        try { url = httpRequest.GetDisplayUrl(); } catch { /* best-effort */ }

        var kwargs = new Dictionary<string, object>
        {
            ["headers"] = headers,
            ["query_params"] = queryParams,
            ["cookies"] = cookies,
            ["url"] = url ?? string.Empty,
        };
        if (client != null) kwargs["client"] = client;

        return new Request(request: null, username: username, sessionHash: sessionHash, kwargs: kwargs);
    }

    public static object CompileGrRequest(Data.PredictBodyInternal body, BlockFunction fn, string username, HttpRequest request)
    {
        static string? FirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        static string? ExtractSessionHashFromHttpRequest(HttpRequest? httpRequest)
        {
            if (httpRequest == null)
            {
                return null;
            }

            httpRequest.Headers.TryGetValue("x-gradio-session-hash", out var headerSessionHash);

            var cookieSessionHash = httpRequest.Cookies.TryGetValue("session_hash", out var cookieValue)
                ? cookieValue
                : null;

            var querySessionHash = httpRequest.Query.TryGetValue("session_hash", out var queryValue)
                ? queryValue.ToString()
                : null;

            return FirstNonEmpty(headerSessionHash.ToString(), querySessionHash, cookieSessionHash);
        }

        // If this fn cancels jobs, then the only input we need is the current session hash
        if (fn.Cancels != null && fn.Cancels.Count > 0)
        {
            body.Data = new List<object> { body.SessionHash };
        }

        var bodyHttpRequest = body.Request as HttpRequest;
        var effectiveSessionHash = FirstNonEmpty(
            body.SessionHash,
            ExtractSessionHashFromHttpRequest(request),
            ExtractSessionHashFromHttpRequest(bodyHttpRequest),
            Guid.NewGuid().ToString("N")
        )!;

        // Keep downstream state/session restoration consistent with request injection.
        body.SessionHash = effectiveSessionHash;

        if (body.Request != null)
        {
            if (body.Batched ?? false)
            {
                return new List<Request> { new Request(request, username, effectiveSessionHash) };
            }
            else
            {
                return new Request(bodyHttpRequest ?? request, username, effectiveSessionHash);
            }
        }
        else
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "request must be provided if body.request is null");
            }
            return new Request(request, username, effectiveSessionHash);
        }
    }

    public static (SessionState, object) RestoreSessionState(App app, Data.PredictBodyInternal body)
    {
        var eventId = body.EventId;
        var sessionHash = body.SessionHash;

        SessionState sessionState;
        object iterator = null;

        if (sessionHash != null)
        {
            sessionState = app.StateHolder[sessionHash];
            // The should_reset set keeps track of the fn_indices
            // that have been cancelled. When a job is cancelled,
            // the /reset route will mark the jobs as having been reset.
            // That way if the cancel job finishes BEFORE the job being cancelled
            // the job being cancelled will not overwrite the state of the iterator.
            if (eventId == null)
            {
                iterator = null;
            }
            else if (app.IteratorsToReset.Contains(eventId))
            {
                iterator = null;
                app.IteratorsToReset.Remove(eventId);
            }
            else
            {
                app.Iterators.TryGetValue(eventId, out var iter);
                iterator = iter;
            }
        }
        else
        {
            sessionState = new SessionState(app.GetBlocks());
            iterator = null;
        }

        return (sessionState, iterator);
    }

    public static Events.EventData PrepareEventData(object blocksConfigObj, Data.PredictBodyInternal body, BlockFunction fn = null)
    {
        // Handle both BlocksConfig and object types
        Block target = null;
        if (blocksConfigObj is BlocksConfig blocksConfig)
        {
            target = body.TriggerId.HasValue ? blocksConfig.Blocks.GetValueOrDefault(body.TriggerId.Value) : null;
        }

        // Resolve event name from the block function's targets for precise type selection.
        string eventName = null;
        if (fn?.Targets != null && fn.Targets.Count > 0)
        {
            eventName = fn.Targets[0].eventName?.ToLower();
        }

        Events.EventData eventData;

        // Try to convert raw event data to Dictionary<string, object> and create specific subtype
        var dataDict = TryConvertEventDataToDict(body.EventData);
        if (dataDict != null)
        {
            // Use event name (from fn.Targets) when available for unambiguous type selection.
            // Edit, Undo, and Retry all carry index+value like SelectData, so the event name
            // is the only reliable discriminator. Fallback heuristics handle legacy/direct callers.
            if (eventName == "edit" || (eventName == null && dataDict.ContainsKey("previous_value")))
                eventData = new Events.EditData(target, dataDict);
            else if (eventName == "undo")
                eventData = new Events.UndoData(target, dataDict);
            else if (eventName == "retry")
                eventData = new Events.RetryData(target, dataDict);
            else if (dataDict.ContainsKey("liked"))
                eventData = new Events.LikeData(target, dataDict);
            else if (dataDict.ContainsKey("key") && dataDict.ContainsKey("input_value"))
                eventData = new Events.KeyUpData(target, dataDict);
            else if (dataDict.ContainsKey("index") && (dataDict.ContainsKey("value") || dataDict.ContainsKey("selected")))
                eventData = new Events.SelectData(target, dataDict);
            else
                eventData = new Events.EventData(target, body.EventData);
        }
        else
        {
            eventData = new Events.EventData(target, body.EventData);
        }

        // Set parent to None to avoid serialization issues
        if (target != null && target.Parent != null)
        {
            target.Parent = null;
        }

        return eventData;
    }

    private static Dictionary<string, object>? TryConvertEventDataToDict(object? data)
    {
        if (data == null) return null;
        if (data is Dictionary<string, object> dict) return dict;
        if (data is JsonElement je && je.ValueKind == JsonValueKind.Object)
            return ConvertJsonObjectToDict(je);
        return null;
    }

    private static Dictionary<string, object> ConvertJsonObjectToDict(JsonElement element)
    {
        var result = new Dictionary<string, object>();
        foreach (var prop in element.EnumerateObject())
        {
            result[prop.Name] = ConvertJsonElementValue(prop.Value);
        }
        return result;
    }

    private static object ConvertJsonElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number when element.TryGetInt64(out long l) => (object)l,
            JsonValueKind.Number => (object)element.GetDouble(),
            JsonValueKind.True => (object)true,
            JsonValueKind.False => (object)false,
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElementValue).ToList<object>(),
            JsonValueKind.Object => (object)ConvertJsonObjectToDict(element),
            _ => (object)string.Empty
        };
    }

    public static async Task<Dictionary<string, object>> CallProcessApi(App app, Data.PredictBodyInternal body, object grRequest, BlockFunction fn, string rootPath)
    {
        // Make the App (and its QueueManager) available to Progress callbacks via LocalContext
        var prevApp = LocalContext.CurrentApp;
        LocalContext.CurrentApp = app;
        try
        {
            var (sessionState, iterator) = RestoreSessionState(app, body);
            var eventData = PrepareEventData(sessionState.BlocksConfig, body, fn);
            var eventId = body.EventId;
            var sessionHash = body.SessionHash;
            var inputs = body.Data;

            // Queue path may not always carry an HttpRequest object. Ensure Request injection still
            // has a usable session hash for user callbacks that depend on request.session_hash.
            if (grRequest == null)
            {
                grRequest = new Request(request: null, username: null, sessionHash: sessionHash);
            }
            else if (grRequest is Request singleRequest)
            {
                if (string.IsNullOrWhiteSpace(singleRequest.SessionHash))
                {
                    singleRequest.SessionHash = sessionHash;
                }
            }
            else if (grRequest is List<Request> requestList)
            {
                for (int i = 0; i < requestList.Count; i++)
                {
                    if (requestList[i] == null)
                    {
                        requestList[i] = new Request(request: null, username: null, sessionHash: sessionHash);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(requestList[i].SessionHash))
                    {
                        requestList[i].SessionHash = sessionHash;
                    }
                }
            }

            var batchInSingleOut = !(body.Batched ?? false) && fn.Batch;
            if (batchInSingleOut)
            {
                // Python parity: inputs = [[inp] for inp in inputs]
                // Each individual input is wrapped in its own list to form a batch of size 1.
                inputs = inputs.Select(inp => (object)new List<object> { inp }).ToList();
            }

            try
            {
                var output = await app.GetBlocks().ProcessApi(
                    blockFn: fn,
                    inputs: inputs,
                    request: grRequest,
                    state: sessionState,
                    iterator: iterator,
                    sessionHash: sessionHash,
                    eventId: eventId,
                    eventData: eventData,
                    inEventListener: true,
                    simpleFormat: body.SimpleFormat,
                    rootPath: rootPath
                );

                iterator = output.TryGetValue("iterator", out var iterValue) ? iterValue : null;
                if (eventId != null && iterator != null)
                {
                    app.Iterators[eventId] = iterator;
                }
                // Remove iterator from the output dict to prevent JSON serialization errors.
                // IAsyncEnumerator<T> cannot be serialized by System.Text.Json; the iterator
                // is already stored in app.Iterators[eventId] above and is not needed in the
                // SSE output payload sent to the client.
                output.Remove("iterator");

                if (output.TryGetValue("error", out var errorValue) && errorValue != null)
                {
                    throw new Exception(errorValue.ToString());
                }

                if (batchInSingleOut && output.TryGetValue("data", out var dataValue) && dataValue is List<object> batchOutList)
                {
                    // Python parity: output["data"] = [out[0] for out in output["data"]]
                    // Each output column is a list; take the first element from each column.
                    output["data"] = batchOutList
                        .Select(col => col is List<object> colList && colList.Count > 0 ? colList[0] : col)
                        .ToList();
                }

                return output;
            }
            catch (Exception ex)
            {
                if (eventId != null && app.Iterators.TryGetValue(eventId, out var iter))
                {
                    // Close off any streams that are still open
                    var runId = iter?.GetHashCode() ?? 0;
                    var sessHash = body.SessionHash;

                    if (!string.IsNullOrEmpty(sessHash))
                    {
                        var blocks = app.GetBlocks();
                        if (blocks.PendingStreams != null &&
                            blocks.PendingStreams.TryGetValue(sessHash, out var sessionStreams))
                        {
                            if (sessionStreams.TryGetValue(runId.ToString(), out var streamDict))
                            {
                                // End all pending streams for this run
                                if (streamDict is Dictionary<string, object> streams)
                                {
                                    streams.Clear();
                                }
                            }
                        }
                    }
                }
                throw;
            }
        }
        finally
        {
            LocalContext.CurrentApp = prevApp;
        }
    }

    public static string GetFirstHeaderValue(HttpRequest request, string headerName)
    {
        if (request.Headers.TryGetValue(headerName, out var values))
        {
            return values.FirstOrDefault()?.Split(',')[0].Trim();
        }
        return null;
    }

    public static Uri GetRequestOrigin(HttpRequest request, string routePath)
    {
        var xForwardedHost = GetFirstHeaderValue(request, "X-Forwarded-Host");
        var rootUrl = xForwardedHost != null ? new Uri($"http://{xForwardedHost}") : new Uri(request.GetDisplayUrl());

        // Remove query parameters
        rootUrl = new Uri(rootUrl.GetLeftPart(UriPartial.Path));

        // Remove trailing slash
        var rootUrlString = rootUrl.ToString().TrimEnd('/');

        // Check for HTTPS
        var xForwardedProto = GetFirstHeaderValue(request, "X-Forwarded-Proto");
        if (xForwardedProto == "https")
        {
            rootUrlString = rootUrlString.Replace("http://", "https://");
        }

        // Remove route path if present (case-insensitive to handle /Up vs /up slugification)
        routePath = routePath.TrimEnd('/');
        if (!string.IsNullOrEmpty(routePath) && rootUrlString.EndsWith(routePath, StringComparison.OrdinalIgnoreCase))
        {
            rootUrlString = rootUrlString.Substring(0, rootUrlString.Length - routePath.Length);
        }

        return new Uri(rootUrlString.TrimEnd('/'));
    }

    public static string GetApiCallPath(HttpRequest request)
    {
        var queueApiUrl = $"{ApiPrefix}/queue/join";
        var genericApiUrl = $"{ApiPrefix}/call";
        var requestPath = request.Path.Value.TrimEnd('/');

        if (requestPath.EndsWith(queueApiUrl))
        {
            return queueApiUrl;
        }

        var startIndex = requestPath.LastIndexOf(genericApiUrl);
        if (startIndex >= 0)
        {
            return requestPath.Substring(startIndex);
        }

        throw new InvalidOperationException($"Request url '{request.GetDisplayUrl()}' has an unknown api call pattern.");
    }

    public static string GetRootUrl(HttpRequest request, string routePath, string rootPath)
    {
        if (!string.IsNullOrEmpty(rootPath) && Uri.IsWellFormedUriString(rootPath, UriKind.Absolute))
        {
            return rootPath.TrimEnd('/');
        }

        var origin = GetRequestOrigin(request, routePath);

        if (!string.IsNullOrEmpty(rootPath) && origin.PathAndQuery != rootPath)
        {
            origin = new Uri(origin, rootPath);
        }

        return origin.ToString().TrimEnd('/');
    }

    public static void MoveUploadedFilesToCache(List<string> files, List<string> destinations)
    {
        for (int i = 0; i < files.Count && i < destinations.Count; i++)
        {
            var src = files[i];
            var dst = destinations[i];

            var dstDir = Path.GetDirectoryName(dst);
            if (!string.IsNullOrEmpty(dstDir))
            {
                Directory.CreateDirectory(dstDir);
            }

            // Match shutil.move more closely: allow overwrite.
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }

            File.Move(src, dst, overwrite: true);
        }
    }

    public static Data.BlocksConfigDict UpdateRootInConfig(Data.BlocksConfigDict config, string root)
    {
        var previousRoot = config.Root;
        if (previousRoot == null || previousRoot != root)
        {
            config.Root = root;
            config = ProcessingUtils.AddRootUrl(config, root, previousRoot) as Data.BlocksConfigDict;
        }
        return config;
    }

    public static Dictionary<string, object> UpdateExampleValuesToUsePublicUrl(Dictionary<string, object> apiInfo)
    {
        void AddRootUrl(Dictionary<string, object> fileDict)
        {
            if (fileDict.TryGetValue("parameter_default", out var defaultValue) && defaultValue is Dictionary<string, object> defaultDict)
            {
                if (defaultDict.TryGetValue("url", out var urlValue) && urlValue is string url)
                {
                    if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    {
                        return;
                    }
                    // If the default value's url is not already a full public url,
                    // we use the example_input url. This makes it so that the example
                    // value for images, audio, and video components pass SSRF checks.
                    if (fileDict.TryGetValue("example_input", out var exampleInputValue) && exampleInputValue is Dictionary<string, object> exampleInputDict)
                    {
                        if (exampleInputDict.TryGetValue("url", out var exampleUrlValue) && exampleUrlValue is string exampleUrl)
                        {
                            defaultDict["url"] = exampleUrl;
                        }
                    }
                }
            }
        }

        return Traverse(apiInfo, AddRootUrl, d => d is Dictionary<string, object> dict && dict.ContainsKey("parameter_default"));
    }

    private static Dictionary<string, object> Traverse(Dictionary<string, object> dict, Action<Dictionary<string, object>> action, Func<object, bool> predicate)
    {
        foreach (var (key, value) in dict)
        {
            if (predicate(value))
            {
                action((Dictionary<string, object>)value);
            }
            else if (value is Dictionary<string, object> nestedDict)
            {
                Traverse(nestedDict, action, predicate);
            }
            else if (value is List<object> list)
            {
                foreach (var item in list)
                {
                    if (item is Dictionary<string, object> itemDict)
                    {
                        Traverse(itemDict, action, predicate);
                    }
                }
            }
        }
        return dict;
    }

    public static bool ComparePasswordsSecurely(string inputPassword, string correctPassword)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(inputPassword),
            Encoding.UTF8.GetBytes(correctPassword)
        );
    }

    public static bool StartsWithProtocol(string input)
    {
        var pattern = @"^(?:[a-zA-Z][a-zA-Z0-9+\-.]*://|//|\\\\|\\/|/\\)";
        return Regex.IsMatch(input, pattern);
    }

    public static string GetHostname(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }

        try
        {
            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }
            return new Uri(url).Host;
        }
        catch
        {
            return "";
        }
    }

    public static string CreateUrlSafeHash(byte[] data, int digestSize = 8)
    {
        using var hashAlgorithm = new Blake2b(digestSize * 8);
        var hash = hashAlgorithm.ComputeHash(data);
        var base64Hash = Convert.ToBase64String(hash);
        return base64Hash.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public static string Slugify(string value)
    {
        // Python behavior:
        // value = unicodedata.normalize("NFKD", value).encode("ascii", "ignore").decode("ascii")
        // value = re.sub(r"[^\w\s-]", "", value.lower())
        // return re.sub(r"[-\s]+", "-", value).strip("-_")
        if (value is null)
        {
            return string.Empty;
        }

        var normalized = value.Normalize(NormalizationForm.FormKD);
        var ascii = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            // Keep ASCII only (matches encode('ascii','ignore'))
            if (ch <= 0x7F)
            {
                ascii.Append(ch);
            }
        }

        var slug = ascii.ToString().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^\w\s-]", "");
        slug = Regex.Replace(slug, @"[-\s]+", "-");
        return slug.Trim('-', '_');
    }

    public static void DeleteFilesCreatedByApp(Blocks blocks, int? age)
    {
        var dontDelete = new HashSet<string>();

        foreach (var component in blocks.DefaultConfig.Blocks.Values)
        {
            if (component is Block block && block.KeepInCache != null)
            {
                dontDelete.UnionWith(block.KeepInCache);
            }
        }

        foreach (var tempSet in blocks.TempFileSets)
        {
            var toRemove = new HashSet<string>();
            foreach (var file in tempSet)
            {
                if (dontDelete.Contains(file))
                {
                    continue;
                }

                try
                {
                    var fileInfo = new FileInfo(file);
                    if (age == null || (DateTime.Now - fileInfo.LastWriteTime).TotalSeconds > age.Value)
                    {
                        File.Delete(file);
                        toRemove.Add(file);
                    }
                }
                catch (FileNotFoundException)
                {
                    continue;
                }
            }

            foreach (var file in toRemove)
            {
                tempSet.Remove(file);
            }
        }
    }

    public static async Task DeleteFilesOnSchedule(App app, int frequency, int age)
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(frequency));
            DeleteFilesCreatedByApp(app.GetBlocks(), age);
        }
    }

    public static Func<App, IAsyncDisposable> CreateLifespanHandler(
        Func<App, IAsyncDisposable> userLifespan = null,
        int? frequency = 1,
        int? age = 1
    )
    {
        return (app) =>
        {
            var tasks = new List<Task>();

            // Start delete state task
            var deleteStateTask = Task.Run(async () =>
            {
                while (true)
                {
                    app.StateHolder.DeleteAllExpiredState();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
            tasks.Add(deleteStateTask);

            // Start delete files task
            if (frequency.HasValue && age.HasValue)
            {
                var deleteFilesTask = DeleteFilesOnSchedule(app, frequency.Value, age.Value);
                tasks.Add(deleteFilesTask);
            }

            // Run user lifespan if provided
            IAsyncDisposable userDisposable = null;
            if (userLifespan != null)
            {
                userDisposable = userLifespan(app);
            }

            return new CompositeAsyncDisposable(async () =>
            {
                // Cancel tasks
                foreach (var task in tasks)
                {
                    if (task != null && !task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        try
                        {
                            // Try to cancel the task if it's still running
                            // Note: This requires the task to be cancellable and have a CancellationToken
                            // In many cases, tasks may not be directly cancellable
                            // Just wait a short time for task completion
                            await Task.WhenAny(task, Task.Delay(100));
                        }
                        catch
                        {
                            // Ignore cancellation errors
                        }
                    }
                }

                // Dispose user disposable
                if (userDisposable != null)
                {
                    await userDisposable.DisposeAsync();
                }

                // Delete all files on shutdown
                DeleteFilesCreatedByApp(app.GetBlocks(), null);
            });
        };
    }

    public static void AddCodeSnippetsToApiInfo(
        Dictionary<string, object> apiInfo,
        string root,
        string apiPrefix = "/gradio_api/")
    {
        if (!apiInfo.TryGetValue("named_endpoints", out var namedObj) ||
            namedObj is not Dictionary<string, object> named)
            return;

        foreach (var ep in named)
        {
            if (ep.Value is Dictionary<string, object> epInfo)
                epInfo["code_snippets"] = GenerateCodeSnippets(ep.Key, epInfo, root, apiPrefix: apiPrefix);
        }
    }

    public static Dictionary<string, string> GenerateCodeSnippets(
        string apiName,
        Dictionary<string, object> endpointInfo,
        string root,
        string? spaceId = null,
        string apiPrefix = "/gradio_api/")
    {
        var parameters = GetEndpointParameters(endpointInfo);
        var src = spaceId ?? root;
        return new Dictionary<string, string>
        {
            ["python"] = GeneratePythonSnippet(apiName, parameters, src),
            ["javascript"] = GenerateJsSnippet(apiName, parameters, src),
            ["bash"] = GenerateBashSnippet(apiName, parameters, root, apiPrefix)
        };
    }

    private static List<Dictionary<string, object?>> GetEndpointParameters(Dictionary<string, object> endpointInfo)
    {
        if (endpointInfo.TryGetValue("parameters", out var paramsObj) && paramsObj is List<object> paramsList)
            return paramsList.OfType<Dictionary<string, object?>>().ToList();
        return new List<Dictionary<string, object?>>();
    }

    private static object? GetParamValue(Dictionary<string, object?> param)
    {
        if (param.TryGetValue("parameter_has_default", out var hasDefault) && hasDefault is bool b && b)
            return param.TryGetValue("parameter_default", out var def) ? def : null;
        return param.TryGetValue("example_input", out var ex) ? ex : null;
    }

    private static string RepresentValue(object? value, string? pythonType, string lang)
    {
        if (value == null)
            return lang == "py" ? "None" : "null";
        var typeStr = pythonType ?? "";
        if (typeStr is "string" or "str")
            return $"\"{value}\"";
        if (typeStr is "number" or "float" or "int" or "integer")
            return value.ToString() ?? "0";
        if (typeStr is "boolean" or "bool")
        {
            bool bVal = value switch { bool bv => bv, string s => string.Equals(s, "true", StringComparison.OrdinalIgnoreCase), _ => false };
            return lang == "py" ? (bVal ? "True" : "False") : (bVal ? "true" : "false");
        }
        return JsonSerializer.Serialize(value);
    }

    private static string GeneratePythonSnippet(string apiName, List<Dictionary<string, object?>> parameters, string src)
    {
        var lines = new List<string> { "from gradio_client import Client", "" };
        lines.Add($"client = Client(\"{src}\")");
        lines.Add("result = client.predict(");
        foreach (var p in parameters)
        {
            var name = GetParamStr(p, "parameter_name") ?? GetParamStr(p, "label") ?? "input";
            var value = GetParamValue(p);
            var pyType = GetNestedParamStr(p, "python_type", "type");
            lines.Add($"\t{name}={RepresentValue(value, pyType, "py")},");
        }
        lines.Add($"\tapi_name=\"{apiName}\",");
        lines.Add(")");
        lines.Add("print(result)");
        return string.Join("\n", lines);
    }

    private static string GenerateJsSnippet(string apiName, List<Dictionary<string, object?>> parameters, string src)
    {
        var lines = new List<string> { "import { Client } from \"@gradio/client\";", "" };
        lines.Add($"const client = await Client.connect(\"{src}\");");
        lines.Add($"const result = await client.predict(\"{apiName}\", {{");
        foreach (var p in parameters)
        {
            var name = GetParamStr(p, "parameter_name") ?? GetParamStr(p, "label") ?? "input";
            var value = GetParamValue(p);
            var pyType = GetNestedParamStr(p, "python_type", "type");
            lines.Add($"\t\t{name}: {RepresentValue(value, pyType, "js")},");
        }
        lines.Add("});");
        lines.Add("");
        lines.Add("console.log(result.data);");
        return string.Join("\n", lines);
    }

    private static string GenerateBashSnippet(string apiName, List<Dictionary<string, object?>> parameters, string root, string apiPrefix)
    {
        var normRoot = root.TrimEnd('/');
        var normPrefix = string.IsNullOrEmpty(apiPrefix) ? "/" : apiPrefix;
        var endpointName = apiName.TrimStart('/');
        var dataValues = parameters.Select(p =>
        {
            var value = GetParamValue(p);
            var pyType = GetNestedParamStr(p, "python_type", "type");
            return RepresentValue(value, pyType, "bash");
        });
        var dataStr = string.Join(", ", dataValues);
        var baseUrl = $"{normRoot}{normPrefix}call/{endpointName}";
        return $"curl -X POST {baseUrl} -s -H \"Content-Type: application/json\" -d '{{\"data\": [{dataStr}]}}' \\\n  | awk -F'\"' '{{ print $4}}' \\\n  | read EVENT_ID; curl -N {baseUrl}/$EVENT_ID";
    }

    private static string? GetParamStr(Dictionary<string, object?> dict, string key)
        => dict.TryGetValue(key, out var val) ? val?.ToString() : null;

    private static string? GetNestedParamStr(Dictionary<string, object?> dict, string key1, string key2)
    {
        if (!dict.TryGetValue(key1, out var inner)) return null;
        if (inner is Dictionary<string, object> nested && nested.TryGetValue(key2, out var val))
            return val?.ToString();
        if (inner is Dictionary<string, object?> nested2 && nested2.TryGetValue(key2, out var val2))
            return val2?.ToString();
        return null;
    }
}

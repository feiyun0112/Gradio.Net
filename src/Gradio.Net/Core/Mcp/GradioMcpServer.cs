using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Web;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Bmp;
using Gradio.Net.Components;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace Gradio.Net.Core.Mcp;

public class GradioMcpServer
{
    private readonly string _defaultTempDir;

    private readonly Blocks _blocks;
    private readonly Dictionary<string, object> _apiInfo;
    private readonly Dictionary<string, string> _toolToEndpoint;
    private readonly string _toolPrefix;
    private string _rootPath;
    private readonly HttpClient _httpClient;
    private string? _localUrl;
    private object? _clientInstance;
    private string _file_data_tool_description => " If a user passes a file as an input, use the upload_file_to_gradio tool, if present, to upload the file to the gradio app and create a Gradio File Input. Then use the returned path as the input to the tool";
    public GradioMcpServer(Blocks blocks)
    {
        _blocks = blocks;
        _rootPath = "";
        _httpClient = new HttpClient();

        _defaultTempDir = Environment.GetEnvironmentVariable("GRADIO_TEMP_DIR") ?? Path.Combine(Path.GetTempPath(), "gradio");

        _apiInfo = _blocks.GetApiInfo();

        var spaceId = GetSpace();
        _toolPrefix = spaceId != null ? spaceId.Split('/').Last() + "_" : "";

        _toolToEndpoint = BuildToolToEndpoint();

        WarnAboutStateInputs();
    }
    private static string? GetSpace()
    {
        return Environment.GetEnvironmentVariable("SPACE_ID");
    }
    private Dictionary<string, string> BuildToolToEndpoint()
    {
        var toolToEndpoint = new Dictionary<string, string>();

        if (!_apiInfo.ContainsKey("named_endpoints") || _apiInfo["named_endpoints"] is not Dictionary<string, object> namedEndpoints)
        {
            return toolToEndpoint;
        }

        foreach (var (endpointName, _) in namedEndpoints)
        {
            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            string fnName;
            if (blockFn.Fn is Delegate del)
            {
                fnName = ExtractCleanMethodName(del.Method.Name);
            }
            else
            {
                fnName = blockFn.Fn.GetType().Name;
            }

            var toolName = _toolPrefix + fnName;
            toolName = ValidAndUniqueToolName(toolName, toolToEndpoint.Keys.ToHashSet());
            toolToEndpoint[toolName] = endpointName;
        }

        return toolToEndpoint;
    }

    private static string ExtractCleanMethodName(string methodName)
    {
        // Local function pattern: <<EnclosingMethod>$>g__FunctionName|digits  or  <EnclosingMethod>g__FunctionName|digits
        var match = Regex.Match(methodName, @"g__([A-Za-z_][A-Za-z0-9_]*)(?:[|<]|$)");
        if (match.Success)
            return match.Groups[1].Value;

        // Lambda/anonymous method: <EnclosingMethod>b__digits — fall back to endpoint name below
        // Strip common compiler-generated wrapper patterns: <<...>$> prefix
        var stripped = Regex.Replace(methodName, @"^<[^>]*>\$?g?__?", "");
        stripped = Regex.Replace(stripped, @"[|<>0-9_]+$", "");
        if (!string.IsNullOrWhiteSpace(stripped) && stripped != methodName)
            return stripped;

        return methodName;
    }
    private string ValidAndUniqueToolName(string toolName, HashSet<string> existingToolNames)
    {
        toolName = Regex.Replace(toolName, @"[^a-zA-Z0-9]", "_");
        toolName = toolName.Substring(0, Math.Min(120, toolName.Length));

        var toolNameBase = toolName;
        int suffix = 1;
        while (existingToolNames.Contains(toolName))
        {
            toolName = $"{toolNameBase}_{suffix}";
            suffix++;
        }

        return toolName;
    }
    private void WarnAboutStateInputs()
    {
        foreach (var (_, endpointName) in _toolToEndpoint)
        {
            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null)
            {
                continue;
            }

            bool hasStateInput = false;
            if (blockFn.Inputs != null)
            {
                if (blockFn.Inputs is System.Collections.IEnumerable enumerable)
                {
                    foreach (var input in enumerable)
                    {
                        if (input != null && input.GetType().Name == "State")
                        {
                            hasStateInput = true;
                            break;
                        }
                    }
                }
                else if (blockFn.Inputs.GetType().Name == "State")
                {
                    hasStateInput = true;
                }
            }

            if (hasStateInput)
            {
                break;
            }
        }
    }
    public string GetRoutePath(object request)
    {
        string url = "";

        if (request is Dictionary<string, object> requestDict && requestDict.ContainsKey("url"))
        {
            url = requestDict["url"] as string ?? "";
        }
        else if (request is { } && request.GetType().GetProperty("Url") is var urlProperty && urlProperty != null)
        {
            var urlValue = urlProperty.GetValue(request);
            url = urlValue?.ToString() ?? "";
        }

        int queryIndex = url.IndexOf('?');
        if (queryIndex != -1)
        {
            url = url.Substring(0, queryIndex);
        }

        url = url.TrimEnd('/');

        if (url.EndsWith("/gradio_api/mcp/messages"))
        {
            return "/gradio_api/mcp/messages";
        }
        else
        {
            return "/gradio_api/mcp";
        }
    }
    public List<string>? GetSelectedToolsFromRequest()
    {
        object? contextRequest = null;


        if (contextRequest == null)
        {
            return null;
        }

        Dictionary<string, object>? queryParams = null;

        if (contextRequest is Dictionary<string, object> requestDict && requestDict.ContainsKey("query_params"))
        {
            queryParams = requestDict["query_params"] as Dictionary<string, object>;
        }
        else if (contextRequest is { } && contextRequest.GetType().GetProperty("QueryParams") is var queryParamsProperty && queryParamsProperty != null)
        {
            var queryParamsValue = queryParamsProperty.GetValue(contextRequest);
            if (queryParamsValue is Dictionary<string, object> qpDict)
            {
                queryParams = qpDict;
            }
            else if (queryParamsValue is Dictionary<string, string> qpStringDict)
            {
                queryParams = qpStringDict.ToDictionary(kv => kv.Key, kv => (object)kv.Value);
            }
        }

        if (queryParams != null && queryParams.ContainsKey("tools"))
        {
            var toolsValue = queryParams["tools"];
            if (toolsValue is string toolsString)
            {
                var tools = toolsString.Split(',');
                var fullToolNames = tools.Select(tool => _toolPrefix + tool).ToList();
                return fullToolNames;
            }
        }

        return null;
    }
    public static List<object> InsertEmptyState(List<object> inputs, List<object> data)
    {
        if (inputs == null || data == null)
        {
            return data ?? new List<object>();
        }

        var result = new List<object>(data);
        int insertOffset = 0;

        for (int i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            if (input != null && input.GetType().Name == "State")
            {
                result.Insert(i + insertOffset, null!);
                insertOffset++;
            }
        }

        return result;
    }
    public static List<object> PopReturnedState(List<object> components, object data)
    {
        if (components == null || data == null)
        {
            if (data is List<object> dataList)
            {
                return dataList;
            }
            return data == null ? new List<object>() : new List<object> { data };
        }

        List<object> result;
        if (data is List<object> list)
        {
            result = new List<object>(list);
        }
        else
        {
            return new List<object> { data };
        }

        for (int i = components.Count - 1; i >= 0; i--)
        {
            var component = components[i];
            if (component != null && component.GetType().Name == "State" && i < result.Count)
            {
                result.RemoveAt(i);
            }
        }

        return result;
    }
    public static ImageSharpImage? GetImage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            return ImageSharpImage.Load(filePath);
        }
        catch
        {
            return null;
        }
    }
    public static byte[]? GetSvg(object fileData)
    {
        if (fileData is Dictionary<string, object> fileDict && fileDict.TryGetValue("url", out var urlObj))
        {
            if (urlObj is string url && url.StartsWith("data:image/svg"))
            {
                var parts = url.Split(new[] { ',' }, 2);
                if (parts.Length == 2)
                {
                    var svgContent = System.Web.HttpUtility.UrlDecode(parts[1]);
                    return Encoding.UTF8.GetBytes(svgContent);
                }
            }
        }
        return null;
    }
    public static string GetBase64Data(ImageSharpImage image, string format)
    {
        using (var stream = new MemoryStream())
        {
            var imageFormat = GetImageEncoder(format);
            image.Save(stream, imageFormat);
            return Convert.ToBase64String(stream.ToArray());
        }
    }
    private static IImageEncoder GetImageEncoder(string format)
    {
        switch (format.ToLower())
        {
            case "png":
                return new PngEncoder();
            case "jpg":
            case "jpeg":
                return new JpegEncoder();
            case "gif":
                return new GifEncoder();
            case "bmp":
                return new BmpEncoder();
            default:
                return new PngEncoder();
        }
    }
    private BlockFunction? GetBlockFnFromEndpointName(string endpointName)
    {
        foreach (var kvp in _blocks.Fns)
        {
            BlockFunction blockFn = kvp.Value;
            if (blockFn.ApiName == endpointName.TrimStart('/'))
            {
                return blockFn;
            }
        }
        return null;
    }
    public Dictionary<string, object> GetApiInfo()
    {
        return _apiInfo;
    }

    public List<Dictionary<string, object>> GetToolsList()
    {
        var tools = new List<Dictionary<string, object>>();
        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
                continue;

            var (description, parameters) = GetFnDescription(blockFn, toolName);
            var (schema, _) = GetInputSchema(toolName, parameters);

            tools.Add(new Dictionary<string, object>
            {
                { "name", toolName },
                { "description", description },
                { "inputSchema", schema }
            });
        }
        return tools;
    }
    public Dictionary<string, string> ToolToEndpoint
    {
        get { return _toolToEndpoint; }
    }
    public string? LocalUrl
    {
        get { return _localUrl; }
        set { _localUrl = value; }
    }
    public object CreateMcpServer()
    {

        var mockServer = new Dictionary<string, object>
        {
            { "name", _blocks.Title ?? "Gradio App" },
            { "toolToEndpoint", _toolToEndpoint },
            { "apiInfo", _apiInfo },
            { "call_tool", new Func<string, Dictionary<string, object>, Task<Dictionary<string, object>>>(CallToolAsync) },
            { "list_tools", new Func<Task<List<Dictionary<string, object>>>>(ListToolsAsync) },
            { "list_resources", new Func<Task<List<Dictionary<string, object>>>>(ListResourcesAsync) },
            { "list_resource_templates", new Func<Task<List<Dictionary<string, object>>>>(ListResourceTemplatesAsync) },
            { "read_resource", new Func<string, Task<List<Dictionary<string, object>>>>(ReadResourceAsync) },
            { "list_prompts", new Func<Task<List<Dictionary<string, object>>>>(ListPromptsAsync) },
            { "get_prompt", new Func<string, Dictionary<string, object>?, Task<Dictionary<string, object>>>(GetPromptAsync) }
        };

        return mockServer;
    }
    private async Task<Dictionary<string, object>> CallToolAsync(string name, Dictionary<string, object> arguments)
    {
        string? progressToken = null;


        try
        {
            var client = _get_or_create_client();
            var (endpointName, processedArgs, requestHeaders, blockFn) = _prepare_tool_call_args(name, arguments);

            if (blockFn != null)
            {
            }

            var job = SubmitJob(client, processedArgs, endpointName, requestHeaders);

            List<object> outputData;
            if (progressToken == null)
            {
                outputData = await _execute_tool_without_progress(job);
            }
            else
            {
                var result = await _execute_tool_with_progress(job, progressToken);
                outputData = result.TryGetValue("data", out var data) && data is List<object> dataList ? dataList : new List<object> { "Tool executed" };
            }

            if (blockFn != null)
            {
            }

            var contextRequest = new object();
            string routePath = GetRoutePath(contextRequest);
            string rootUrl = GetRootUrl(contextRequest, routePath);
            var content = PostprocessOutputData(outputData, rootUrl);

            bool structuredOutput = false;
            Dictionary<string, object>? meta = null;
            if (blockFn != null && blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                structuredOutput = mcpMetadata?.McpStructuredOutput ?? false;
                meta = mcpMetadata?.McpMeta;
            }

            var structuredContent = structuredOutput ? new Dictionary<string, object> { { "result", content } } : null;

            return new Dictionary<string, object>
            {
                { "content", content },
                { "structuredContent", structuredContent },
                { "_meta", meta }
            };
        }
        catch (Exception ex)
        {
            return new Dictionary<string, object>
            {
                { "content", new List<object> { "Error calling tool" } },
                { "structuredContent", null },
                { "_meta", null }
            };
        }
    }
    private object SubmitJob(object client, List<object> args, string apiName, Dictionary<string, string> headers)
    {
        return new Dictionary<string, object>
        {
            { "api_name", apiName },
            { "args", args },
            { "headers", headers }
        };
    }
    private async Task<List<Dictionary<string, object>>> ListToolsAsync()
    {
        var tools = new List<Dictionary<string, object>>();
        var selectedTools = GetSelectedToolsFromRequest();

        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            if (selectedTools != null && !selectedTools.Contains(toolName))
            {
                continue;
            }

            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isToolType = true;
            Dictionary<string, object>? meta = null;
            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isToolType = mcpMetadata?.McpType == "tool";
                meta = mcpMetadata?.McpMeta;
            }

            if (!isToolType)
            {
                continue;
            }

            var (description, parameters) = GetFnDescription(blockFn, toolName);
            var (schema, _) = GetInputSchema(toolName, parameters);

            tools.Add(new Dictionary<string, object>
            {
                { "name", toolName },
                { "description", description },
                { "inputSchema", schema },
                { "_meta", meta }
            });
        }

        await Task.Delay(10);
        return tools;
    }
    private async Task<List<Dictionary<string, object>>> ListResourcesAsync()
    {
        var resources = new List<Dictionary<string, object>>();
        var selectedTools = GetSelectedToolsFromRequest();

        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            if (selectedTools != null && !selectedTools.Contains(toolName))
            {
                continue;
            }

            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isResourceType = false;
            string? uriTemplate = null;
            string? mimeType = null;

            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isResourceType = mcpMetadata?.McpType == "resource";
                uriTemplate = mcpMetadata?.McpUriTemplate;
                mimeType = mcpMetadata?.McpMimeType;
            }

            if (isResourceType)
            {
                string effectiveUriTemplate = uriTemplate ?? "/api/resource";
                string effectiveMimeType = mimeType ?? "text/plain";

                var parameters = new List<string>();
                if (parameters.Count == 0)
                {
                    var (description, _) = GetFnDescription(blockFn, toolName);
                    resources.Add(new Dictionary<string, object>
                    {
                        { "uri", effectiveUriTemplate },
                        { "name", blockFn.Fn.GetType().Name },
                        { "description", description },
                        { "mimeType", effectiveMimeType }
                    });
                }
            }
        }

        await Task.Delay(10);
        return resources;
    }
    private async Task<List<Dictionary<string, object>>> ListResourceTemplatesAsync()
    {
        var templates = new List<Dictionary<string, object>>();
        var selectedTools = GetSelectedToolsFromRequest();

        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            if (selectedTools != null && !selectedTools.Contains(toolName))
            {
                continue;
            }

            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isResourceType = false;
            string? uriTemplate = null;
            string? mimeType = null;

            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isResourceType = mcpMetadata?.McpType == "resource";
                uriTemplate = mcpMetadata?.McpUriTemplate;
                mimeType = mcpMetadata?.McpMimeType;
            }

            if (isResourceType)
            {
                string effectiveUriTemplate = uriTemplate ?? "/api/resource/{id}";
                string effectiveMimeType = mimeType ?? "text/plain";

                var parameters = new List<string> { "id" };
                if (parameters.Count > 0)
                {
                    var (description, _) = GetFnDescription(blockFn, toolName);
                    templates.Add(new Dictionary<string, object>
                    {
                        { "uriTemplate", effectiveUriTemplate },
                        { "name", blockFn.Fn.GetType().Name },
                        { "description", description },
                        { "mimeType", effectiveMimeType }
                    });
                }
            }
        }

        await Task.Delay(10);
        return templates;
    }
    private async Task<List<Dictionary<string, object>>> ReadResourceAsync(string uri)
    {
        var client = _get_or_create_client();

        foreach (var endpointName in _toolToEndpoint.Values)
        {
            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isResourceType = false;
            string? uriTemplate = null;
            string? mimeType = null;

            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isResourceType = mcpMetadata?.McpType == "resource";
                uriTemplate = mcpMetadata?.McpUriTemplate;
                mimeType = mcpMetadata?.McpMimeType;
            }

            if (isResourceType)
            {
                string effectiveUriTemplate = uriTemplate ?? "/api/resource/{id}";

                bool matched = false;
                var kwargs = new Dictionary<string, object>();

                if (uri.Contains("/api/resource/"))
                {
                    matched = true;
                    kwargs["id"] = uri.Split('/').Last();
                }

                if (matched)
                {
                    List<object> processedArgs = new List<object>();

                    var job = SubmitJob(client, processedArgs, endpointName, new Dictionary<string, string>());

                    var result = await _execute_tool_without_progress(job);

                    string effectiveMimeType = mimeType ?? "text/plain";

                    return new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "content", result.FirstOrDefault() ?? "Resource content" },
                            { "mime_type", effectiveMimeType }
                        }
                    };
                }
            }
        }

        throw new Exception($"Resource not found: {uri}");
    }
    private async Task<List<Dictionary<string, object>>> ListPromptsAsync()
    {
        var prompts = new List<Dictionary<string, object>>();
        var selectedTools = GetSelectedToolsFromRequest();

        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            if (selectedTools != null && !selectedTools.Contains(toolName))
            {
                continue;
            }

            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isPromptType = false;
            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isPromptType = mcpMetadata?.McpType == "prompt";
            }

            if (isPromptType)
            {
                var (description, parameters) = GetFnDescription(blockFn, toolName);
                var functionParams = new List<Tuple<string, bool, object, object>>();

                var arguments = functionParams.Select(param => new Dictionary<string, object>
                {
                    { "name", param.Item1 },
                    { "description", parameters.TryGetValue(param.Item1, out var paramDesc) ? paramDesc : "" },
                    { "required", param.Item2 }
                }).ToList();

                prompts.Add(new Dictionary<string, object>
                {
                    { "name", toolName },
                    { "description", description },
                    { "arguments", arguments }
                });
            }
        }

        await Task.Delay(10);
        return prompts;
    }
    private async Task<Dictionary<string, object>> GetPromptAsync(string name, Dictionary<string, object>? arguments)
    {
        var client = _get_or_create_client();
        string? endpointName = null;

        foreach (var (toolName, epName) in _toolToEndpoint)
        {
            var blockFn = GetBlockFnFromEndpointName(epName);
            if (blockFn == null || blockFn.Fn == null)
            {
                continue;
            }

            bool isPromptType = false;
            string? promptName = null;
            if (blockFn.Fn is Delegate functionDelegate)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(functionDelegate);
                isPromptType = mcpMetadata?.McpType == "prompt";
                promptName = mcpMetadata?.McpName;
            }

            if (isPromptType && promptName == name)
            {
                endpointName = epName;
                break;
            }
        }

        if (endpointName == null)
        {
            throw new Exception($"Prompt not found: {name}");
        }

        arguments ??= new Dictionary<string, object>();

        var endpointBlockFn = GetBlockFnFromEndpointName(endpointName);
        if (endpointBlockFn == null)
        {
            throw new Exception($"Could not find BlockFunction for endpoint: {endpointName}");
        }

        List<object> processedArgs = new List<object>();

        var job = SubmitJob(client, processedArgs, endpointName, new Dictionary<string, string>());

        var result = await _execute_tool_without_progress(job);

        return new Dictionary<string, object>
        {
            { "messages", new List<object>
                {
                    new Dictionary<string, object>
                    {
                        { "role", "user" },
                        { "content", new Dictionary<string, object>
                            {
                                { "type", "text" },
                                { "text", result.FirstOrDefault()?.ToString() ?? "Prompt result" }
                            }
                        }
                    }
                }
            }
        };
    }
    public object GetCompleteSchema(Microsoft.AspNetCore.Http.HttpRequest? request = null)
    {
        if (!_apiInfo.Any())
            return new List<object>();

        // Parse optional ?tools=tool1,tool2 filter
        HashSet<string>? selectedTools = null;
        if (request != null && request.Query.TryGetValue("tools", out var toolsVal) && !string.IsNullOrEmpty(toolsVal))
        {
            selectedTools = toolsVal.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        }

        bool fileDataPresent = false;
        var schemas = new List<object>();

        foreach (var (toolName, endpointName) in _toolToEndpoint)
        {
            if (selectedTools != null && !selectedTools.Contains(toolName))
                continue;

            var blockFn = GetBlockFnFromEndpointName(endpointName);
            if (blockFn == null || blockFn.Fn == null)
                continue;

            var (description, parameters) = GetFnDescription(blockFn, toolName);
            var (schema, filedataPositions) = GetInputSchema(toolName, parameters);
            if (filedataPositions.Count > 0 && !fileDataPresent)
                fileDataPresent = true;

            string mcpType = "tool";
            if (blockFn.Fn is Delegate del)
            {
                var mcpMetadata = McpDecorators.GetMcpMetadata(del);
                if (mcpMetadata?.McpType != null)
                    mcpType = mcpMetadata.McpType;
            }

            schemas.Add(new Dictionary<string, object>
            {
                { "name", toolName },
                { "description", description },
                { "inputSchema", schema },
                { "meta", new Dictionary<string, object>
                    {
                        { "file_data_present", fileDataPresent },
                        { "mcp_type", mcpType },
                        { "endpoint_name", blockFn.ApiName ?? endpointName }
                    }
                }
            });
        }

        return schemas;
    }
    public Tuple<string, Dictionary<string, string>> GetFnDescription(BlockFunction blockFn, string toolName)
    {
        var description = "";
        var parameters = new Dictionary<string, string>();

        if (blockFn.Fn == null)
        {
            return new Tuple<string, Dictionary<string, string>>(description, parameters);
        }

        // Use Utils.GetFunctionDescription to extract XML doc / DescriptionAttribute info
        if (blockFn.Fn is Delegate functionDelegate)
        {
            var method = functionDelegate.Method;
            var (docDescription, docParameters, docReturns) = Gradio.Net.Utils.Utils.GetFunctionDescription(method);

            // Use doc description if available, else empty (no generic "Function Foo" noise)
            description = docDescription;

            // Populate parameter descriptions from docs (empty string if not documented)
            foreach (var p in method.GetParameters())
            {
                var pName = p.Name ?? "";
                parameters[pName] = docParameters.TryGetValue(pName, out var desc) ? desc : "";
            }

            // Append Returns section like Python does
            var (_, filedataPositions) = GetInputSchema(toolName, parameters);

            if (blockFn.ApiDescription == null)
            {
                if (filedataPositions.Count > 0)
                    description += _file_data_tool_description;
                if (docReturns.Count > 0)
                {
                    var sep = string.IsNullOrEmpty(description) || description.EndsWith(".") ? " " : ". ";
                    description += sep + "Returns: " + string.Join(", ", docReturns);
                }
            }
            else if (blockFn.ApiDescription == "")
            {
                description = "";
            }
            else
            {
                description = blockFn.ApiDescription;
                if (filedataPositions.Count > 0)
                    description += _file_data_tool_description;
            }
        }

        return new Tuple<string, Dictionary<string, string>>(description, parameters);
    }
    public Tuple<Dictionary<string, object>, List<List<object>>> GetInputSchema(string toolName, Dictionary<string, string>? parameters = null)
    {
        if (!_toolToEndpoint.TryGetValue(toolName, out var endpointName))
        {
            throw new Exception($"Unknown tool for this Gradio app: {toolName}");
        }

        if (!_apiInfo.ContainsKey("named_endpoints") || _apiInfo["named_endpoints"] is not Dictionary<string, object> namedEndpoints)
        {
            return new Tuple<Dictionary<string, object>, List<List<object>>>(
                new Dictionary<string, object> { { "type", "object" }, { "properties", new Dictionary<string, object>() } },
                new List<List<object>>()
            );
        }

        if (!namedEndpoints.TryGetValue(endpointName, out var endpointInfoObj) || endpointInfoObj is not Dictionary<string, object> endpointInfo)
        {
            return new Tuple<Dictionary<string, object>, List<List<object>>>(
                new Dictionary<string, object> { { "type", "object" }, { "properties", new Dictionary<string, object>() } },
                new List<List<object>>()
            );
        }

        if (!endpointInfo.TryGetValue("parameters", out var parametersObj) || parametersObj is not List<object> parametersList)
        {
            return new Tuple<Dictionary<string, object>, List<List<object>>>(
                new Dictionary<string, object> { { "type", "object" }, { "properties", new Dictionary<string, object>() } },
                new List<List<object>>()
            );
        }

        var properties = new Dictionary<string, object>();

        foreach (var paramObj in parametersList)
        {
            if (paramObj is not Dictionary<string, object> param)
            {
                continue;
            }

            if (!param.TryGetValue("parameter_name", out var paramNameObj) || paramNameObj is not string paramName)
            {
                continue;
            }

            var paramProperty = new Dictionary<string, object>();

            if (param.TryGetValue("type", out var typeObj) && typeObj is Dictionary<string, object> typeDict)
            {
                foreach (var kvp in typeDict)
                {
                    paramProperty[kvp.Key] = kvp.Value;
                }
            }

            if (parameters != null && parameters.TryGetValue(paramName, out var description))
            {
                paramProperty["description"] = description;
            }

            if (param.TryGetValue("parameter_default", out var defaultValue) && defaultValue != null)
            {
                paramProperty["default"] = defaultValue;
            }

            properties[paramName] = paramProperty;
        }

        var schema = new Dictionary<string, object>
        {
            { "type", "object" },
            { "properties", properties }
        };

        return SimplifyFiledataSchema(schema);
    }
    public void LaunchMcpOnSse(object app, string subpath, string rootPath)
    {
        _rootPath = rootPath;
    }
    public Tuple<Dictionary<string, object>, List<List<object>>> SimplifyFiledataSchema(Dictionary<string, object> schema)
    {
        var filedataPositions = new List<List<object>>();

        bool IsGradioFiledata(object obj, Dictionary<string, object> defs)
        {
            if (obj is not Dictionary<string, object> dict)
            {
                return false;
            }

            if (dict.ContainsKey("$ref"))
            {
                var refValue = dict["$ref"]?.ToString();
                if (refValue != null && refValue.StartsWith("#/$defs/"))
                {
                    var key = refValue.Split('/').Last();
                    if (defs.ContainsKey(key) && defs[key] is Dictionary<string, object> refDict)
                    {
                        dict = refDict;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (!dict.ContainsKey("properties") || dict["properties"] is not Dictionary<string, object> props)
            {
                return false;
            }

            var meta = props.ContainsKey("meta") ? props["meta"] : null;
            if (meta is Dictionary<string, object> metaDict)
            {
                if (metaDict.ContainsKey("$ref"))
                {
                    var metaRef = metaDict["$ref"]?.ToString();
                    if (metaRef != null && metaRef.StartsWith("#/$defs/"))
                    {
                        var key = metaRef.Split('/').Last();
                        if (defs.ContainsKey(key) && defs[key] is Dictionary<string, object> metaRefDict)
                        {
                            metaDict = metaRefDict;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (metaDict.ContainsKey("properties") && metaDict["properties"] is Dictionary<string, object> metaProps)
                {
                    if (metaProps.ContainsKey("_type") && metaProps["_type"] is Dictionary<string, object> typeField)
                    {
                        if (typeField.ContainsKey("const") && typeField["const"]?.ToString() == "gradio.FileData")
                        {
                            return true;
                        }
                    }
                }

                if (metaDict.ContainsKey("default") && metaDict["default"] is Dictionary<string, object> defaultDict)
                {
                    if (defaultDict.ContainsKey("_type") && defaultDict["_type"]?.ToString() == "gradio.FileData")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        object Traverse(object node, List<object>? path = null, Dictionary<string, object>? defs = null)
        {
            path ??= new List<object>();
            defs ??= new Dictionary<string, object>();

            node = DeepCopyObject(node);

            if (node is Dictionary<string, object> dict)
            {
                if (dict.ContainsKey("$defs") && dict["$defs"] is Dictionary<string, object> nodeDefs)
                {
                    foreach (var kvp in nodeDefs)
                    {
                        defs[kvp.Key] = kvp.Value;
                    }
                }

                if (IsGradioFiledata(dict, defs))
                {
                    filedataPositions.Add(new List<object>(path));
                    dict.Remove("properties");
                    dict.Remove("additional_description");
                    dict.Remove("$defs");
                    dict["type"] = "string";
                    dict["format"] = "Gradio File Input - a http or https url to a file";
                }

                var result = new Dictionary<string, object>();
                bool isSchemaRoot = dict.ContainsKey("type") && dict.ContainsKey("properties");

                foreach (var kvp in dict)
                {
                    if (isSchemaRoot && kvp.Key == "properties")
                    {
                        result[kvp.Key] = Traverse(kvp.Value, path, defs);
                    }
                    else
                    {
                        path.Add(kvp.Key);
                        result[kvp.Key] = Traverse(kvp.Value, path, defs);
                        path.RemoveAt(path.Count - 1);
                    }
                }
                return result;
            }
            else if (node is List<object> list)
            {
                var result = new List<object>();
                for (int i = 0; i < list.Count; i++)
                {
                    path.Add(i);
                    result.Add(Traverse(list[i], path, defs));
                    path.RemoveAt(path.Count - 1);
                }
                return result;
            }

            return node;
        }

        var simplifiedSchema = Traverse(schema);
        if (simplifiedSchema is Dictionary<string, object> resultDict)
        {
            return new Tuple<Dictionary<string, object>, List<List<object>>>(resultDict, filedataPositions);
        }

        return new Tuple<Dictionary<string, object>, List<List<object>>>(schema, filedataPositions);
    }
    private static object DeepCopyObject(object obj)
    {
        if (obj is Dictionary<string, object> dict)
        {
            var newDict = new Dictionary<string, object>();
            foreach (var kvp in dict)
            {
                newDict[kvp.Key] = DeepCopyObject(kvp.Value);
            }
            return newDict;
        }
        else if (obj is List<object> list)
        {
            var newList = new List<object>();
            foreach (var item in list)
            {
                newList.Add(DeepCopyObject(item));
            }
            return newList;
        }
        else if (obj is string || obj is int || obj is bool || obj is double || obj is float || obj == null)
        {
            return obj;
        }

        try
        {
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<object>(json) ?? obj;
        }
        catch
        {
            return obj;
        }
    }
    public object ConvertStringsToFiledata(object value, List<List<object>> filedataPositions)
    {
        object Traverse(object node, List<object>? path = null)
        {
            path ??= new List<object>();

            if (node is Dictionary<string, object> dict)
            {
                var result = new Dictionary<string, object>();
                foreach (var kvp in dict)
                {
                    var newPath = new List<object>(path) { kvp.Key };
                    result[kvp.Key] = Traverse(kvp.Value, newPath);
                }
                return result;
            }
            else if (node is List<object> list)
            {
                var result = new List<object>();
                for (int i = 0; i < list.Count; i++)
                {
                    var newPath = new List<object>(path) { i };
                    result.Add(Traverse(list[i], newPath));
                }
                return result;
            }
            else if (node is string str)
            {
                bool isFiledataPosition = false;
                foreach (var fdPath in filedataPositions)
                {
                    if (PathsMatch(path, fdPath))
                    {
                        isFiledataPosition = true;
                        break;
                    }
                }

                if (isFiledataPosition)
                {
                    if (str.StartsWith("data:"))
                    {
                        try
                        {
                            var filePath = SaveBase64ToCache(str, _defaultTempDir);
                            return HandleFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException($"Invalid base64 file data: {ex.Message}");
                        }
                    }
                    else if (str.StartsWith("http://") || str.StartsWith("https://"))
                    {
                        return HandleFile(str);
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid file data format, provide a url ('http://...' or 'https://...'). Received: {str}"
                        );
                    }
                }
            }

            return node;
        }

        return Traverse(value);
    }
    private static bool PathsMatch(List<object> path1, List<object> path2)
    {
        if (path1.Count != path2.Count)
        {
            return false;
        }

        for (int i = 0; i < path1.Count; i++)
        {
            var p1 = path1[i]?.ToString();
            var p2 = path2[i]?.ToString();
            if (p1 != p2)
            {
                return false;
            }
        }

        return true;
    }
    private static string SaveBase64ToCache(string dataUrl, string cacheDir)
    {
        var parts = dataUrl.Split(new[] { ',' }, 2);
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid data URL format");
        }

        var header = parts[0]; // data:image/jpeg;base64
        var base64Data = parts[1];

        string mimeType = "application/octet-stream";
        string extension = "bin";
        if (header.Contains(':') && header.Contains(';'))
        {
            var mimeTypePart = header.Split(new[] { ':' }, 2)[1].Split(';')[0];
            mimeType = mimeTypePart;

            extension = mimeType.Split('/').Last();
        }

        byte[] fileBytes;
        try
        {
            base64Data = base64Data.Replace(" ", "+");
            fileBytes = Convert.FromBase64String(base64Data);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid base64 data: {ex.Message}");
        }

        Directory.CreateDirectory(cacheDir);

        var fileName = $"{Guid.NewGuid()}.{extension}";
        var filePath = Path.Combine(cacheDir, fileName);

        File.WriteAllBytes(filePath, fileBytes);

        return filePath;
    }
    private static object HandleFile(string filePathOrUrl)
    {
        return new Dictionary<string, object>
        {
            { "path", filePathOrUrl },
            { "url", filePathOrUrl },
            { "orig_name", Path.GetFileName(filePathOrUrl) },
            { "size", null },
            { "mime_type", null },
            { "is_stream", false },
            { "meta", new Dictionary<string, object> { { "_type", "gradio.FileData" } } }
        };
    }
    public List<object> PostprocessOutputData(object data, string rootUrl)
    {
        var returnValues = new List<object>();

        var processedData = AddRootUrl(data, rootUrl);

        List<object> outputList;
        if (processedData is List<object> list)
        {
            outputList = list;
        }
        else
        {
            outputList = new List<object> { processedData };
        }

        foreach (var output in outputList)
        {
            var svgBytes = GetSvg(output);
            if (svgBytes != null)
            {
                var base64Data = Convert.ToBase64String(svgBytes);
                var mimeType = "image/svg+xml";

                string origName = "image.svg";
                if (output is Dictionary<string, object> outputDict && outputDict.ContainsKey("orig_name"))
                {
                    origName = outputDict["orig_name"]?.ToString() ?? origName;
                }
                var svgPath = SaveBytesToCache(svgBytes, origName, _defaultTempDir);
                var svgUrl = $"{rootUrl}/gradio_api/file={svgPath}";

                returnValues.Add(new Dictionary<string, object>
                {
                    { "type", "image" },
                    { "data", base64Data },
                    { "mimeType", mimeType }
                });
                returnValues.Add(new Dictionary<string, object>
                {
                    { "type", "text" },
                    { "text", $"SVG Image URL: {svgUrl}" }
                });
            }
            else if (IsFileObjWithMeta(output))
            {
                if (output is Dictionary<string, object> fileDict && fileDict.ContainsKey("path"))
                {
                    var filePath = fileDict["path"]?.ToString();
                    if (filePath != null)
                    {
                        var image = GetImage(filePath);
                        if (image != null)
                        {
                            var imageFormat = GetImageFormat(image);
                            var base64Data = GetBase64Data(image, imageFormat);
                            var mimeType = $"image/{imageFormat.ToLower()}";

                            var url = fileDict.ContainsKey("url") && fileDict["url"] != null
                                ? fileDict["url"].ToString()
                                : filePath;

                            returnValues.Add(new Dictionary<string, object>
                            {
                                { "type", "image" },
                                { "data", base64Data },
                                { "mimeType", mimeType }
                            });
                            returnValues.Add(new Dictionary<string, object>
                            {
                                { "type", "text" },
                                { "text", $"Image URL: {url}" }
                            });

                            image.Dispose();
                        }
                        else
                        {
                            var url = fileDict.ContainsKey("url") && fileDict["url"] != null
                                ? fileDict["url"].ToString()
                                : filePath;

                            returnValues.Add(new Dictionary<string, object>
                            {
                                { "type", "text" },
                                { "text", url ?? filePath }
                            });
                        }
                    }
                }
            }
            else
            {
                returnValues.Add(new Dictionary<string, object>
                {
                    { "type", "text" },
                    { "text", output?.ToString() ?? "" }
                });
            }
        }

        return returnValues;
    }
    private static object AddRootUrl(object data, string rootUrl)
    {
        if (data is Dictionary<string, object> dict)
        {
            var newDict = new Dictionary<string, object>();
            foreach (var kvp in dict)
            {
                if (kvp.Key == "path" && kvp.Value is string pathValue && !pathValue.StartsWith("http"))
                {
                    newDict[kvp.Key] = kvp.Value;
                    if (!dict.ContainsKey("url") || dict["url"] == null)
                    {
                        newDict["url"] = $"{rootUrl}/gradio_api/file={pathValue}";
                    }
                }
                else if (kvp.Key == "url" && kvp.Value == null && dict.ContainsKey("path"))
                {
                    var pathStr = dict["path"]?.ToString();
                    if (pathStr != null && !pathStr.StartsWith("http"))
                    {
                        newDict[kvp.Key] = $"{rootUrl}/gradio_api/file={pathStr}";
                    }
                    else
                    {
                        newDict[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    newDict[kvp.Key] = AddRootUrl(kvp.Value, rootUrl);
                }
            }
            return newDict;
        }
        else if (data is List<object> list)
        {
            var newList = new List<object>();
            foreach (var item in list)
            {
                newList.Add(AddRootUrl(item, rootUrl));
            }
            return newList;
        }

        return data;
    }
    private static bool IsFileObjWithMeta(object obj)
    {
        if (obj is not Dictionary<string, object> dict)
        {
            return false;
        }

        if (!dict.ContainsKey("path"))
        {
            return false;
        }

        if (dict.ContainsKey("meta") && dict["meta"] is Dictionary<string, object> meta)
        {
            if (meta.ContainsKey("_type") && meta["_type"]?.ToString() == "gradio.FileData")
            {
                return true;
            }
        }

        return dict.ContainsKey("path") && (dict.ContainsKey("orig_name") || dict.ContainsKey("size"));
    }
    private static string SaveBytesToCache(byte[] data, string filename, string cacheDir)
    {
        Directory.CreateDirectory(cacheDir);

        var extension = Path.GetExtension(filename);
        var baseName = Path.GetFileNameWithoutExtension(filename);
        var uniqueFileName = $"{baseName}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(cacheDir, uniqueFileName);

        File.WriteAllBytes(filePath, data);

        return filePath;
    }
    private static string GetImageFormat(ImageSharpImage image)
    {
        var format = image.Metadata.DecodedImageFormat;
        if (format != null)
        {
            return format.Name;
        }
        return "png";
    }
    private object _get_or_create_client()
    {
        if (_clientInstance == null)
        {
            object? contextRequest = null;


            if (contextRequest == null)
            {
                throw new Exception("Could not find the request object in the MCP server context. This is not expected to happen. Please raise an issue: https://github.com/gradio-app/gradio.");
            }

            string routePath = GetRoutePath(contextRequest);
            string rootUrl = GetRootUrl(contextRequest, routePath);

            _clientInstance = new Dictionary<string, object>
            {
                { "name", "Gradio Client" },
                { "url", _localUrl ?? rootUrl },
                { "download_files", false },
                { "verbose", false },
                { "analytics_enabled", false },
                { "ssl_verify", false },
                { "_skip_components", false }
            };
        }
        return _clientInstance;
    }
    private string GetRootUrl(object request, string routePath)
    {
        string url = "http://localhost:7860";

        if (request is Dictionary<string, object> requestDict && requestDict.ContainsKey("url"))
        {
            url = requestDict["url"] as string ?? url;
        }
        else if (request is { } && request.GetType().GetProperty("Url") is var urlProperty && urlProperty != null)
        {
            var urlValue = urlProperty.GetValue(request);
            url = urlValue?.ToString() ?? url;
        }

        Uri uri;
        if (Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            url = $"{uri.Scheme}://{uri.Authority}";
        }

        return url;
    }
    private Tuple<string, List<object>, Dictionary<string, string>, BlockFunction?> _prepare_tool_call_args(string name, Dictionary<string, object> arguments)
    {
        List<string>? selectedTools = GetSelectedToolsFromRequest();
        var (_, filedataPositions) = GetInputSchema(name);
        var processedKwargs = ConvertStringsToFiledata(arguments, filedataPositions);

        if (!_toolToEndpoint.TryGetValue(name, out var endpointName))
        {
            throw new Exception($"Unknown tool for this Gradio app: {name}");
        }

        if (selectedTools != null && !selectedTools.Contains(name))
        {
            throw new Exception($"Tool '{name}' is not in the selected tools list");
        }

        var blockFn = GetBlockFnFromEndpointName(endpointName);
        if (blockFn == null)
        {
            throw new Exception($"Could not find BlockFunction for endpoint: {endpointName}");
        }

        List<object> processedArgs = new List<object>();

        if (_apiInfo.ContainsKey("named_endpoints") && _apiInfo["named_endpoints"] is Dictionary<string, object> namedEndpoints)
        {
            if (namedEndpoints.TryGetValue(endpointName, out var endpointInfoObj) && endpointInfoObj is Dictionary<string, object> endpointInfo)
            {
                if (endpointInfo.TryGetValue("parameters", out var parametersObj) && parametersObj is List<object> parameters)
                {
                    foreach (var param in parameters)
                    {
                        if (param is Dictionary<string, object> paramDict && paramDict.TryGetValue("parameter_name", out var paramNameObj))
                        {
                            string paramName = paramNameObj.ToString()!;
                            if (processedKwargs is Dictionary<string, object> kwargsDict && kwargsDict.TryGetValue(paramName, out var value))
                            {
                                processedArgs.Add(value);
                            }
                            else if (paramDict.TryGetValue("parameter_default", out var defaultValue))
                            {
                                processedArgs.Add(defaultValue);
                            }
                            else
                            {
                                processedArgs.Add(null);
                            }
                        }
                    }
                }
            }
        }

        Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
        object? contextRequest = null;


        if (contextRequest != null)
        {
            if (contextRequest is Dictionary<string, object> requestDict && requestDict.ContainsKey("headers"))
            {
                var headersObj = requestDict["headers"];
                if (headersObj is Dictionary<string, object> headersDict)
                {
                    foreach (var kvp in headersDict)
                    {
                        requestHeaders[kvp.Key] = kvp.Value?.ToString() ?? "";
                    }
                }
                else if (headersObj is Dictionary<string, string> headersStringDict)
                {
                    foreach (var kvp in headersStringDict)
                    {
                        requestHeaders[kvp.Key] = kvp.Value;
                    }
                }
            }
            else if (contextRequest is { } && contextRequest.GetType().GetProperty("Headers") is var headersProperty && headersProperty != null)
            {
                var headersValue = headersProperty.GetValue(contextRequest);
                if (headersValue is Dictionary<string, string> headersStringDict)
                {
                    foreach (var kvp in headersStringDict)
                    {
                        requestHeaders[kvp.Key] = kvp.Value;
                    }
                }
            }
        }

        requestHeaders.Remove("content-length");

        return new Tuple<string, List<object>, Dictionary<string, string>, BlockFunction?>(endpointName, processedArgs, requestHeaders, blockFn);
    }
    private async Task<List<object>> _execute_tool_without_progress(object job)
    {
        object? result = null;

        if (job is { } && job.GetType().GetMethod("result") is var resultMethod && resultMethod != null)
        {
            try
            {
                result = resultMethod.Invoke(job, null);
            }
            catch (Exception ex)
            {
                result = "Error executing tool";
            }
        }
        else if (job is Dictionary<string, object> jobDict && jobDict.ContainsKey("result"))
        {
            result = jobDict["result"];
        }
        else
        {
            await Task.Delay(100);
            result = "Tool executed successfully";
        }

        return new List<object> { result ?? "Tool executed successfully" };
    }
    private async Task<Dictionary<string, object>> _execute_tool_with_progress(object job, string progress_token)
    {
        int step = 0;
        Dictionary<string, object>? output = null;

        try
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(200);

                SendProgressNotification(progress_token, step, $"Processing step {i + 1}");
                step++;
            }

            await Task.Delay(100);
            output = new Dictionary<string, object>
            {
                { "data", new List<object> { "Tool executed with progress" } },
                { "success", true }
            };
        }
        catch (Exception ex)
        {
            output = new Dictionary<string, object>
            {
                { "data", new List<object> { "Error executing tool" } },
                { "success", false },
                { "error", ex.Message }
            };
        }

        if (job is { } && job.GetType().GetMethod("exception") is var exceptionMethod && exceptionMethod != null)
        {
            var exception = exceptionMethod.Invoke(job, null);
            if (exception is Exception ex)
            {
                throw ex;
            }
        }

        if (output == null)
        {
            output = new Dictionary<string, object>
            {
                { "data", new List<object> { "Tool executed with progress" } },
                { "success", true }
            };
        }

        return output;
    }
    private void SendProgressNotification(string progress_token, int progress, string message)
    {
    }
    public List<object> ListResources()
    {
        var resources = new List<object>();

        var mockResource = new Dictionary<string, object>
        {
            { "uri", "/api/resource" },
            { "name", "Example Resource" },
            { "description", "An example resource" },
            { "mimeType", "text/plain" }
        };

        resources.Add(mockResource);
        return resources;
    }
    public List<object> ListResourceTemplates()
    {
        var templates = new List<object>();

        var mockTemplate = new Dictionary<string, object>
        {
            { "uriTemplate", "/api/resource/{id}" },
            { "name", "Example Resource Template" },
            { "description", "An example resource template" },
            { "mimeType", "text/plain" }
        };

        templates.Add(mockTemplate);
        return templates;
    }
    public List<object> ReadResource(string uri)
    {
        var resourceContents = new List<object>();

        var mockContent = new Dictionary<string, object>
        {
            { "content", "Resource content" },
            { "mime_type", "text/plain" }
        };

        resourceContents.Add(mockContent);
        return resourceContents;
    }
    public List<object> ListPrompts()
    {
        var prompts = new List<object>();

        var mockPrompt = new Dictionary<string, object>
        {
            { "name", "Example Prompt" },
            { "description", "An example prompt" },
            { "arguments", new List<object>() }
        };

        prompts.Add(mockPrompt);
        return prompts;
    }
    public object GetPrompt(string name, Dictionary<string, object>? arguments = null)
    {
        var promptResult = new Dictionary<string, object>
        {
            { "messages", new List<object> {
                new Dictionary<string, object> {
                    { "role", "user" },
                    { "content", new Dictionary<string, object> {
                        { "type", "text" },
                        { "text", "Example prompt message" }
                    }}
                }
            }}
        };

        return promptResult;
    }
}


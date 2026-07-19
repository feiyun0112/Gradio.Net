using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;

using Gradio.Net.Components;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Events;

namespace Gradio.Net
{
    public static class External
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static Blocks Load(
            string name,
            object src = null,
            string token = null,
            object acceptToken = null,
            object provider = null,
            Dictionary<string, object> kwargs = null
        )
        {
            if (src == null)
            {
                // Separate the repo type (e.g. "model") from repo name (e.g. "google/vit-base-patch16-224")
                var parts = name.Split('/');
                if (parts.Length <= 1)
                {
                    throw new ArgumentException(
                        "Either `src` parameter must be provided, or `name` must be formatted as {src}/{repo name}"
                    );
                }
                src = parts[0];
                name = string.Join("/", parts.Skip(1));
            }

            if (src is string srcStr && !new[] { "models", "spaces", "huggingface" }.Contains(srcStr))
            {
                throw new ArgumentException(
                    "The `src` parameter must be one of 'huggingface', 'models', 'spaces', or a function that accepts a model name (and optionally, a token), and returns a Gradio app."
                );
            }

            if (token == null && src is string srcStr2 && (srcStr2 == "models" || srcStr2 == "huggingface"))
            {
                token = Environment.GetEnvironmentVariable("HF_TOKEN");
            }

            if (src is Delegate srcDelegate)
            {
                var pcount = srcDelegate.Method.GetParameters().Length;
                return pcount switch
                {
                    >= 3 => (Blocks)srcDelegate.DynamicInvoke(name, token, kwargs),
                    2 => (Blocks)srcDelegate.DynamicInvoke(name, token),
                    _ => (Blocks)srcDelegate.DynamicInvoke(name)
                };
            }

            var acceptTokenBoolValue = acceptToken is bool b && b;
            var acceptTokenLogin = acceptToken as LoginButton;

            if (!acceptTokenBoolValue && acceptTokenLogin == null)
            {
                return LoadBlocksFromHuggingface(
                    name,
                    src as string,
                    token,
                    provider,
                    kwargs
                );
            }
            else if (acceptTokenLogin != null)
            {
                var demo = new Blocks(title: "Gradio", fillHeight: true);
                if (!acceptTokenLogin.IsRendered)
                {
                    acceptTokenLogin.Render();
                }
                return demo;
            }
            else
            {
                var demo = new Blocks(title: "Gradio", fillHeight: true);

                // Create textbox for token input
                var textbox = new Textbox(
                    label: "Enter your HuggingFace token",
                    placeholder: "hf_...",
                    showLabel: true,
                    type: "password"
                );

                var submitButton = new Button("Submit");

                return demo;
            }
        }

        public static Blocks LoadBlocksFromHuggingface(
            string name,
            string src,
            string token = null,
            object alias = null,
            object provider = null,
            Dictionary<string, object> kwargs = null
        )
        {
            if (token != null)
            {
                Context.Token = token;
            }

            if (src == "spaces")
            {
                return FromSpaces(name, token, alias as string, provider as string, kwargs);
            }
            else
            {
                return FromModel(name, token, alias as string, provider as string, kwargs);
            }
        }

        public static Blocks FromModel(
            string modelName,
            string token,
            string alias,
            string provider = null,
            Dictionary<string, object> kwargs = null
        )
        {
            var modelInfo = ExternalUtils.GetModelInfo(modelName, token);
            var pipeline = modelInfo.Item1 ?? string.Empty;

            var apiUrl = $"https://api-inference.huggingface.co/models/{modelName}";

            object inputs;
            object outputs;
            Delegate fn;

            switch (pipeline)
            {
                case "audio-classification":
                    inputs = new Audio(type: "filepath", label: "Input");
                    outputs = new Textbox(label: "Class");
                    fn = new Func<string, object>(path =>
                    {
                        var payload = BuildInputPayload(path, binaryLike: true);
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return result;
                    });
                    break;

                case "automatic-speech-recognition":
                    inputs = new Audio(type: "filepath", label: "Input");
                    outputs = new Textbox(label: "Output");
                    fn = new Func<string, string>(path =>
                    {
                        var payload = BuildInputPayload(path, binaryLike: true);
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return ExtractString(result, "text") ?? result?.ToString() ?? string.Empty;
                    });
                    break;

                case "image-classification":
                    inputs = new Image(type: "filepath", label: "Input Image");
                    outputs = new Textbox(label: "Classification");
                    fn = new Func<string, object>(path =>
                    {
                        var payload = BuildInputPayload(path, binaryLike: true);
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return result;
                    });
                    break;

                case "question-answering":
                    inputs = new List<object>
                    {
                        new Textbox(label: "Question"),
                        new Textbox(lines: 7, label: "Context")
                    };
                    outputs = new List<object>
                    {
                        new Textbox(label: "Answer"),
                        new Textbox(label: "Score")
                    };
                    fn = new Func<string, string, object>((question, context) =>
                    {
                        var payload = new Dictionary<string, object>
                        {
                            ["inputs"] = new Dictionary<string, object>
                            {
                                ["question"] = question,
                                ["context"] = context
                            }
                        };
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return new object[]
                        {
                            ExtractString(result, "answer") ?? string.Empty,
                            ExtractString(result, "score") ?? string.Empty
                        };
                    });
                    break;

                case "text-classification":
                    inputs = new Textbox(label: "Input");
                    outputs = new Textbox(label: "Classification");
                    fn = new Func<string, object>(text =>
                    {
                        var payload = new Dictionary<string, object> { ["inputs"] = text };
                        return InvokeInferenceJson(apiUrl, token, payload);
                    });
                    break;

                case "translation":
                    inputs = new Textbox(label: "Input");
                    outputs = new Textbox(label: "Translation");
                    fn = new Func<string, string>(text =>
                    {
                        var payload = new Dictionary<string, object> { ["inputs"] = text };
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return ExtractFromFirst(result, "translation_text") ?? result?.ToString() ?? string.Empty;
                    });
                    break;

                case "summarization":
                    inputs = new Textbox(label: "Input");
                    outputs = new Textbox(label: "Summary");
                    fn = new Func<string, string>(text =>
                    {
                        var payload = new Dictionary<string, object> { ["inputs"] = text };
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return ExtractFromFirst(result, "summary_text") ?? result?.ToString() ?? string.Empty;
                    });
                    break;

                case "text-generation":
                    inputs = new Textbox(label: "Text");
                    outputs = new Textbox(label: "Generated Text");
                    fn = new Func<string, string>(text =>
                    {
                        var payload = new Dictionary<string, object> { ["inputs"] = text };
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        var generated = ExtractFromFirst(result, "generated_text") ?? ExtractString(result, "generated_text");
                        return generated ?? result?.ToString() ?? string.Empty;
                    });
                    break;

                default:
                    // Fallback generic text interface
                    inputs = new Textbox(label: "Input", placeholder: "Enter your text...");
                    outputs = new Textbox(label: "Output", placeholder: "Model output will appear here");
                    fn = new Func<string, string>(text =>
                    {
                        var payload = new Dictionary<string, object> { ["inputs"] = text };
                        var result = InvokeInferenceJson(apiUrl, token, payload);
                        return result?.ToString() ?? string.Empty;
                    });
                    break;
            }

            return new Interface(fn, inputs, outputs, title: alias ?? modelName);
        }

        private static Dictionary<string, object> BuildInputPayload(string value, bool binaryLike)
        {
            if (!binaryLike)
            {
                return new Dictionary<string, object> { ["inputs"] = value };
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return new Dictionary<string, object> { ["inputs"] = string.Empty };
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == "http" || uri.Scheme == "https"))
            {
                return new Dictionary<string, object> { ["inputs"] = value };
            }

            if (File.Exists(value))
            {
                var bytes = File.ReadAllBytes(value);
                var b64 = Convert.ToBase64String(bytes);
                return new Dictionary<string, object> { ["inputs"] = b64 };
            }

            return new Dictionary<string, object> { ["inputs"] = value };
        }

        private static object InvokeInferenceJson(string apiUrl, string token, object payload)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            req.Headers.Add("X-Wait-For-Model", "true");
            if (!string.IsNullOrWhiteSpace(token))
            {
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var body = JsonSerializer.Serialize(payload);
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var resp = _httpClient.Send(req);
            if (!resp.IsSuccessStatusCode)
            {
                throw new Error($"HF Inference error ({(int)resp.StatusCode}): {resp.ReasonPhrase}");
            }

            var text = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            try
            {
                return JsonSerializer.Deserialize<object>(text);
            }
            catch
            {
                return text;
            }
        }

        private static string ExtractString(object obj, string key)
        {
            if (obj is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Object && je.TryGetProperty(key, out var prop))
                {
                    return prop.ToString();
                }
                return null;
            }

            if (obj is Dictionary<string, object> dict && dict.TryGetValue(key, out var value))
            {
                return value?.ToString();
            }

            if (obj is IDictionary<string, object> idict && idict.TryGetValue(key, out var iv))
            {
                return iv?.ToString();
            }

            return null;
        }

        private static string ExtractFromFirst(object obj, string key)
        {
            if (obj is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Array && je.GetArrayLength() > 0)
                {
                    return ExtractString(je[0], key);
                }
                return null;
            }

            if (obj is List<object> list && list.Count > 0)
            {
                return ExtractString(list[0], key);
            }
            return null;
        }

        public static Blocks FromSpaces(
            string spaceName,
            string token,
            string alias,
            string provider = null,
            Dictionary<string, object> kwargs = null
        )
        {
            if (provider != null)
            {
            }

            var spaceUrl = $"https://huggingface.co/spaces/{spaceName}";

            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(token))
            {
                headers["Authorization"] = $"Bearer {token}";
            }

            // Implement Space loading functionality
            // In a full implementation, this would:
            // 1. Fetch the Space configuration from HF API
            // 2. Parse the app.py or config to understand the interface
            // 3. Recreate the Gradio app structure

            try
            {

                // Create a basic blocks interface as placeholder
                var blocks = new Blocks(title: spaceName);

                // Add informational message
                var markdown = new Markdown(
                    $"# {spaceName}\n\n" +
                    $"This Space is loaded from: {spaceUrl}\n\n" +
                    $"Note: This is a simplified implementation. " +
                    $"Full Space loading requires API integration."
                );

                // Add a textbox for interaction
                var input = new Textbox(label: "Input", placeholder: "Enter text...");
                var output = new Textbox(label: "Output", interactive: false);
                var button = new Button("Submit");

                // Note: In a full implementation, event handlers would be set up here
                // using the Blocks event system

                return blocks;
            }
            catch (Exception ex)
            {
                var blocks = new Blocks(title: spaceName);
                var errorMsg = new Markdown($"# Error loading Space\n\n{ex.Message}");
                return blocks;
            }
        }

        public static Delegate MakeEventDataFn(object client, object endpoint)
        {
            // Create a function that accepts EventData
            // This wraps the client endpoint call to handle EventData parameters
            return new Func<object[], object>((args) =>
            {
                // In a full implementation, this would:
                // 1. Extract EventData from args
                // 2. Call the client endpoint with appropriate parameters
                // 3. Return the result
                // For now, return a placeholder
                return new Dictionary<string, object>
                {
                    ["success"] = true,
                    ["message"] = "EventData function called"
                };
            });
        }

        public static Blocks FromSpacesBlocks(string space, string token = null)
        {
            // Implement Space Blocks loading

            try
            {
                // In a full implementation, this would:
                // 1. Connect to the Space using gradio_client
                // 2. Fetch the Space's configuration
                // 3. Recreate the Blocks structure

                var blocks = new Blocks(title: $"Space: {space}");

                // Add descriptive content
                var description = new Markdown(
                    $"# {space}\n\n" +
                    $"This Space is loaded from Hugging Face.\n\n" +
                    $"API endpoint: `https://huggingface.co/spaces/{space}`"
                );

                return blocks;
            }
            catch (Exception ex)
            {
                return new Blocks(title: space);
            }
        }

        public static Interface FromSpacesInterface(
            string modelName,
            Dictionary<string, object> config,
            string alias,
            string token,
            string iframeUrl,
            Dictionary<string, object> kwargs = null
        )
        {
            // Implement Space Interface loading

            try
            {
                // Parse config to determine input/output components
                var inputs = new Textbox(label: "Input", placeholder: "Enter your input...");
                var outputs = new Textbox(label: "Output", interactive: false);

                // Create a function that would interact with the Space
                Func<string, string> fn = (input) =>
                {
                    // In a full implementation, this would call the Space's API
                    return $"[Space {modelName}] Result: {input}";
                };

                var interfaceInstance = new Interface(
                    fn,
                    inputs,
                    outputs,
                    title: modelName
                );

                return interfaceInstance;
            }
            catch (Exception ex)
            {
                var inputs = new Textbox(label: "Input");
                var outputs = new Textbox(label: "Output");
                var fn = new Func<string, string>(input => $"Error: {ex.Message}");
                return new Interface(fn, inputs, outputs);
            }
        }

        public static List<Dictionary<string, object>> FormatConversation(
            List<Dictionary<string, object>> history,
            object newMessage
        )
        {
            // Format conversation by adding new message to history
            var result = new List<Dictionary<string, object>>(history ?? new List<Dictionary<string, object>>());

            if (newMessage != null)
            {
                if (newMessage is Dictionary<string, object> messageDict)
                {
                    result.Add(messageDict);
                }
                else if (newMessage is string messageStr)
                {
                    result.Add(new Dictionary<string, object>
                    {
                        ["role"] = "user",
                        ["content"] = messageStr
                    });
                }
            }

            return result;
        }

        public static ChatInterface LoadChat(
            string baseUrl,
            string model,
            string token = null,
            string fileTypes = "text_encoded",
            string systemMessage = null,
            bool streaming = true,
            Dictionary<string, object> kwargs = null
        )
        {
            // Implement OpenAI API integration

            // Create a function that interacts with OpenAI API
            Func<string, List<Dictionary<string, object>>, string> fn = (message, history) =>
            {
                try
                {
                    // In a full implementation, this would:
                    // 1. Format the conversation history for OpenAI API
                    // 2. Make HTTP request to the API endpoint
                    // 3. Parse and return the response


                    // Build conversation context
                    var context = new List<string>();
                    if (!string.IsNullOrEmpty(systemMessage))
                    {
                        context.Add($"System: {systemMessage}");
                    }

                    if (history != null)
                    {
                        foreach (var msg in history)
                        {
                            if (msg.ContainsKey("role") && msg.ContainsKey("content"))
                            {
                                context.Add($"{msg["role"]}: {msg["content"]}");
                            }
                        }
                    }

                    // Simulate API response
                    return $"[{model}] Response to: {message}\n(This is a placeholder. Full implementation requires OpenAI API client.)";
                }
                catch (Exception ex)
                {
                    return $"Error calling API: {ex.Message}";
                }
            };

            // Create ChatInterface with the function
            var chatInterface = new ChatInterface(
                fn: fn,
                title: $"Chat with {model}",
                description: systemMessage
            );

            return chatInterface;
        }

        public static Blocks LoadOpenApi(
            object openapiSpec,
            string baseUrl,
            List<string> paths = null,
            List<string> excludePaths = null,
            List<string> methods = null,
            string authToken = null,
            Dictionary<string, object> interfaceKwargs = null
        )
        {
            // Implement OpenAPI specification loading

            try
            {
                var blocks = new Blocks(title: "OpenAPI Interface");

                // Add header
                var header = new Markdown(
                    $"# OpenAPI Interface\n\n" +
                    $"Base URL: `{baseUrl}`\n\n" +
                    $"This interface is generated from an OpenAPI v3 specification."
                );

                // In a full implementation, this would:
                // 1. Parse the OpenAPI spec (JSON/YAML)
                // 2. Extract API endpoints, parameters, and schemas
                // 3. Create Gradio components for each endpoint
                // 4. Generate request/response handlers

                // Create a sample interface for demonstration
                var endpointInput = new Textbox(
                    label: "API Endpoint",
                    placeholder: "/api/endpoint",
                    value: paths?.FirstOrDefault() ?? "/"
                );

                var paramsInput = new Textbox(
                    label: "Request Parameters (JSON)",
                    placeholder: "{\"param\": \"value\"}",
                    lines: 5
                );

                var output = new Textbox(
                    label: "Response",
                    interactive: false,
                    lines: 10
                );

                var submitButton = new Button("Send Request");

                // Note: In a full implementation, event handlers would be configured
                // to make actual HTTP requests to the OpenAPI endpoints

                return blocks;
            }
            catch (Exception ex)
            {
                var blocks = new Blocks(title: "OpenAPI Interface");
                var errorMsg = new Markdown($"# Error\n\n{ex.Message}");
                return blocks;
            }
        }
    }
}

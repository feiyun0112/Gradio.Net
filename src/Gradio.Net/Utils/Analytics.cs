using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;

namespace Gradio.Net.Utils
{
    public static class Analytics
    {
        private const string ANALYTICS_URL = "https://api.gradio.app/";
        private const string PKG_VERSION_URL = "https://api.gradio.app/pkg-version";

        private static readonly HttpClient _httpClient = new HttpClient();

        static Analytics()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public static string GetBlockName(Type className)
        {
            return className.Name.ToLower();
        }

        public static bool AnalyticsEnabled()
        {
            // Python parity: os.getenv("GRADIO_ANALYTICS_ENABLED", "True") == "True"
            var value = Environment.GetEnvironmentVariable("GRADIO_ANALYTICS_ENABLED") ?? "True";
            return value == "True";
        }

        public static void DoAnalyticsRequest(string topic, Dictionary<string, object> data)
        {
            if (!AnalyticsEnabled())
                return;

            Task.Run(() => DoNormalAnalyticsRequest(topic, data));
        }

        public static async Task DoNormalAnalyticsRequest(string topic, Dictionary<string, object> data)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, topic);
                request.Content = new FormUrlEncodedContent(data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? ""));
                await _httpClient.SendAsync(request);
            }
            catch (Exception)
            {
            }
        }

        public static async Task DoWasmAnalyticsRequest(string url, Dictionary<string, object> data)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new FormUrlEncodedContent(data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? ""));
                await _httpClient.SendAsync(request);
            }
            catch (Exception)
            {
            }
        }

        public static void VersionCheck()
        {
            try
            {
                var currentPkgVersion = Utils.GetPackageVersion();
                var response = _httpClient.GetFromJsonAsync<Dictionary<string, string>>(PKG_VERSION_URL).Result;

                if (response != null && response.TryGetValue("version", out var latestPkgVersion))
                {
                    if (Version.Parse(latestPkgVersion) > Version.Parse(currentPkgVersion))
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void InitiatedAnalytics(Dictionary<string, object> data)
        {
            if (!AnalyticsEnabled())
                return;

            var topic = $"{ANALYTICS_URL}gradio-initiated-analytics/";
            DoAnalyticsRequest(topic, data);
        }

        public static void LaunchedAnalytics(object blocksObj, Dictionary<string, object> data)
        {
            if (!AnalyticsEnabled())
                return;

            var blocksTelemetry = new List<string>();
            var inputsTelemetry = new List<string>();
            var outputsTelemetry = new List<string>();
            var targetsTelemetry = new List<string>();
            var eventsTelemetry = new List<string>();

            // Collect telemetry from blocks object
            if (blocksObj is Core.Blocks blocks)
            {
                // Collect all block names from blocks dictionary
                foreach (var block in blocks.BlocksDict.Values)
                {
                    blocksTelemetry.Add(block.GetBlockName());
                }

                // Collect event information from functions
                foreach (var fn in blocks.Fns.Values)
                {
                    // Collect targets
                    if (fn.Targets != null)
                    {
                        foreach (var target in fn.Targets)
                        {
                            if (target.blockId.HasValue && blocks.BlocksDict.TryGetValue(target.blockId.Value, out var targetBlock))
                            {
                                targetsTelemetry.Add(targetBlock.GetBlockName());
                                if (!string.IsNullOrEmpty(target.eventName))
                                {
                                    eventsTelemetry.Add(target.eventName);
                                }
                            }
                        }
                    }

                    // Collect inputs
                    if (fn.Inputs is List<int> inputsList)
                    {
                        foreach (var input in inputsList)
                        {
                            if (blocks.BlocksDict.TryGetValue(input, out var inputBlock))
                            {
                                inputsTelemetry.Add(inputBlock.GetBlockName());
                            }
                        }
                    }

                    // Collect outputs
                    if (fn.Outputs is List<int> outputsList)
                    {
                        foreach (var output in outputsList)
                        {
                            if (blocks.BlocksDict.TryGetValue(output, out var outputBlock))
                            {
                                outputsTelemetry.Add(outputBlock.GetBlockName());
                            }
                        }
                    }
                }

                // Get core components for custom component detection
                var coreComponents = new HashSet<string>
                {
                    "textbox", "number", "slider", "checkbox", "checkboxgroup", "radio",
                    "dropdown", "image", "video", "audio", "file", "dataframe",
                    "timeseries", "state", "button", "uploadbutton", "colorpicker",
                    "label", "highlightedtext", "json", "html", "gallery", "chatbot",
                    "model3d", "plot", "markdown", "code", "dataset", "row", "column",
                    "tab", "tabitem", "group", "accordion", "blocks", "interface"
                };

                var customComponents = blocksTelemetry.Where(b => !coreComponents.Contains(b)).Distinct().ToList();
                var usingCustomComponent = customComponents.Count > 0;

                List<string>? GetInputsOutputs(string mode, List<object>? components, List<string> fallback)
                {
                    // Python: if mode == "interface": return [b.get_block_name() for b in components] if components else None
                    if (string.Equals(mode, "interface", StringComparison.Ordinal))
                    {
                        if (components == null)
                        {
                            return null;
                        }

                        var result = new List<string>();
                        foreach (var c in components)
                        {
                            if (c is Core.Block b)
                            {
                                result.Add(b.GetBlockName());
                                continue;
                            }

                            if (c is int id && blocks.BlocksDict.TryGetValue(id, out var b2))
                            {
                                result.Add(b2.GetBlockName());
                                continue;
                            }

                            // Best-effort fallback.
                            result.Add(c?.GetType().Name.ToLowerInvariant() ?? "unknown");
                        }

                        return result;
                    }

                    return fallback;
                }

                var additionalData = new Dictionary<string, object>
                {
                    { "version", Utils.GetPackageVersion() },
                    { "is_hosted_notebook", Utils.IsHostedNotebook() },
                    { "using_auth", blocks.Auth != null },
                    { "dev_mode", blocks.DevMode },
                    { "inputs", GetInputsOutputs(blocks.Mode, blocks.InputComponents, inputsTelemetry) },
                    { "outputs", GetInputsOutputs(blocks.Mode, blocks.OutputComponents, outputsTelemetry) },
                    { "targets", targetsTelemetry },
                    { "blocks", blocksTelemetry },
                    { "events", eventsTelemetry },
                    { "using_custom_component", usingCustomComponent },
                    { "custom_components", customComponents }
                };

                foreach (var kvp in additionalData)
                {
                    data[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // Fallback for non-Blocks objects
                var additionalData = new Dictionary<string, object>
                {
                    { "version", Utils.GetPackageVersion() },
                    { "is_hosted_notebook", false },
                    { "using_auth", false },
                    { "dev_mode", false },
                    { "inputs", inputsTelemetry },
                    { "outputs", outputsTelemetry },
                    { "targets", targetsTelemetry },
                    { "blocks", blocksTelemetry },
                    { "events", eventsTelemetry },
                    { "using_custom_component", false },
                    { "custom_components", new List<string>() }
                };

                foreach (var kvp in additionalData)
                {
                    data[kvp.Key] = kvp.Value;
                }
            }

            var topic = $"{ANALYTICS_URL}gradio-launched-telemetry/";
            DoAnalyticsRequest(topic, data);
        }

        public static void CustomComponentAnalytics(
            string command,
            string template,
            bool? uploadPypi,
            bool? uploadDemo,
            bool? uploadSource,
            bool? generateDocs = null,
            bool? bumpVersion = null,
            string npmInstall = null,
            string pythonPath = null,
            string gradioPath = null)
        {
            var data = new Dictionary<string, object>
            {
                { "command", command },
                { "template", template },
                { "upload_pypi", uploadPypi },
                { "upload_demo", uploadDemo },
                { "upload_source", uploadSource },
                { "generate_docs", generateDocs },
                { "bump_version", bumpVersion },
                { "npm_install", npmInstall },
                { "python_path", pythonPath },
                { "gradio_path", gradioPath }
            };

            if (!AnalyticsEnabled())
                return;

            DoAnalyticsRequest("gradio/custom-components", data);
        }

        public static void VibeAnalytics()
        {
            var data = new Dictionary<string, object>
            {
                { "command", "vibe" }
            };

            if (!AnalyticsEnabled())
                return;

            DoAnalyticsRequest("gradio/vibe", data);
        }

        public static void IntegrationAnalytics(Dictionary<string, object> data)
        {
            if (!AnalyticsEnabled())
                return;

            var topic = $"{ANALYTICS_URL}gradio-integration-analytics/";
            DoAnalyticsRequest(topic, data);
        }

        public static void ErrorAnalytics(string message)
        {
            if (!AnalyticsEnabled())
                return;

            var data = new Dictionary<string, object>
            {
                { "error", message }
            };

            var topic = $"{ANALYTICS_URL}gradio-error-analytics/";
            DoAnalyticsRequest(topic, data);
        }
    }
}

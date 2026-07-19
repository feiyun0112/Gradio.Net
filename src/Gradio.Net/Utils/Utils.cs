using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Channels;
using System.Runtime.Versioning;
using System.Xml.Linq;

namespace Gradio.Net.Utils
{
    public static class Utils
    {
        // Python parity: task naming uses a '<gradio-sep>' delimiter to map task_ids -> event_ids.
        private const string GradioSep = "<gradio-sep>";

        // Best-effort task registry for CancelTasksAsync()/SetTaskName().
        // NOTE: .NET Tasks cannot be force-cancelled without cooperative CancellationTokens.
        private static readonly ConcurrentDictionary<int, string> TaskNamesById = new();
        private static readonly ConcurrentDictionary<int, CancellationTokenSource> CancellationByTaskId = new();

        public static readonly DynamicBoolean NO_RELOAD = new DynamicBoolean(true);

        public static string GetPackageVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetHashSeed()
        {
            try
            {
                var hashSeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hash_seed.txt");
                if (File.Exists(hashSeedPath))
                {
                    return File.ReadAllText(hashSeedPath).Trim();
                }
                else
                {
                    var seed = Guid.NewGuid().ToString("N");
                    File.WriteAllText(hashSeedPath, seed);
                    return seed;
                }
            }
            catch (Exception)
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        public static bool ColabCheck()
        {
            try
            {
                return Environment.GetEnvironmentVariable("COLAB_RELEASE_TAG") != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsHostedNotebook()
        {
            try
            {
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAGGLE_KERNEL_RUN_TYPE")) ||
                       Directory.Exists("/home/ec2-user/SageMaker");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IpythonCheck()
        {
            try
            {
                return Environment.GetEnvironmentVariable("IPYTHON") != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetSpace()
        {
            if (Environment.GetEnvironmentVariable("SYSTEM") == "spaces")
            {
                return Environment.GetEnvironmentVariable("SPACE_ID");
            }
            return null;
        }

        public static bool IsZeroGpuSpace()
        {
            return Environment.GetEnvironmentVariable("SPACES_ZERO_GPU") == "true";
        }

        public static string DownloadIfUrl(string article)
        {
            try
            {
                var uri = new Uri(article);
                if (uri.Scheme != "http" && uri.Scheme != "https")
                {
                    return article;
                }

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                var response = client.GetAsync(article).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception)
            {
            }
            return article;
        }

        public static string AppendUniqueSuffix(string name, List<string> listOfNames)
        {
            var setOfNames = new HashSet<string>(listOfNames);
            if (!setOfNames.Contains(name))
            {
                return name;
            }
            else
            {
                var suffixCounter = 1;
                var newName = $"{name}_{suffixCounter}";
                while (setOfNames.Contains(newName))
                {
                    suffixCounter++;
                    newName = $"{name}_{suffixCounter}";
                }
                return newName;
            }
        }

        public static bool ValidateUrl(string possibleUrl)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("gradio (https://gradio.app/; gradio-team@huggingface.co)");
                var headRequest = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, possibleUrl)).Result;
                if (headRequest.StatusCode == HttpStatusCode.Forbidden || headRequest.StatusCode == HttpStatusCode.MethodNotAllowed)
                {
                    var getRequest = client.GetAsync(possibleUrl).Result;
                    return getRequest.IsSuccessStatusCode;
                }
                return headRequest.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetExtensionFromFilePathOrUrl(string filePathOrUrl)
        {
            try
            {
                var uri = new Uri(filePathOrUrl);
                var path = uri.IsFile ? uri.LocalPath : uri.PathAndQuery;
                var extension = Path.GetExtension(path);
                return extension.Length > 1 ? extension.Substring(1) : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetCacheFolder()
        {
            var cacheFolder = Environment.GetEnvironmentVariable("GRADIO_EXAMPLES_CACHE");
            if (!string.IsNullOrEmpty(cacheFolder))
            {
                return cacheFolder;
            }
            return Path.Combine(Directory.GetCurrentDirectory(), ".gradio", "cached_examples");
        }

        public static string GetUploadFolder()
        {
            var tempDir = Environment.GetEnvironmentVariable("GRADIO_TEMP_DIR");
            if (!string.IsNullOrEmpty(tempDir))
            {
                return tempDir;
            }
            var gradioTempDir = Path.Combine(Path.GetTempPath(), "gradio");
            if (!Directory.Exists(gradioTempDir))
            {
                Directory.CreateDirectory(gradioTempDir);
            }
            return gradioTempDir;
        }

        public static string SafeJoin(string directory, string path)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            // Normalize paths
            var fullDirectory = Path.GetFullPath(directory);
            var fullPath = Path.GetFullPath(Path.Combine(directory, path));

            // Check if the resulting path is within the base directory
            if (!fullPath.StartsWith(fullDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new System.Security.SecurityException(
                    $"Attempted path traversal: {path} is outside of {directory}");
            }

            return fullPath;
        }

        public static bool CheckInUploadFolder(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return false;
            }

            try
            {
                var uploadFolder = GetUploadFolder();
                var fullFilePath = Path.GetFullPath(filepath);
                var fullUploadFolder = Path.GetFullPath(uploadFolder);
                return fullFilePath.StartsWith(fullUploadFolder, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static string RemoveHtmlTags(string rawHtml)
        {
            if (string.IsNullOrEmpty(rawHtml))
            {
                return "";
            }
            var htmlTagRegex = new Regex(@"<[^>]*>", RegexOptions.Singleline);
            return htmlTagRegex.Replace(rawHtml, "");
        }

        public static bool IsInOrEqual(string path1, string path2)
        {
            try
            {
                var p1 = Path.GetFullPath(path1);
                var p2 = Path.GetFullPath(path2);
                return p1.StartsWith(p2, StringComparison.OrdinalIgnoreCase) || p1.Equals(p2, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string SanitizeValueForCsv(object value)
        {
            if (value is float || value is double || value is int || value is long)
            {
                return value.ToString();
            }

            var strValue = value?.ToString() ?? "";
            var unsafePrefixes = new[] { "=", "+", "-", "@", "\t", "\n" };
            var unsafeSequences = new[] { ",=", ",+", ",-", ",@", ",\t", ",\n" };

            if (unsafePrefixes.Any(prefix => strValue.StartsWith(prefix)) ||
                unsafeSequences.Any(sequence => strValue.Contains(sequence)))
            {
                return $"'{strValue}";
            }
            return strValue;
        }

        public static List<object> SanitizeListForCsv(List<object> values)
        {
            var sanitizedValues = new List<object>();
            foreach (var value in values)
            {
                if (value is List<object> listValue)
                {
                    var sanitizedList = listValue.Select(v => SanitizeValueForCsv(v)).ToList<object>();
                    sanitizedValues.Add(sanitizedList);
                }
                else
                {
                    sanitizedValues.Add(SanitizeValueForCsv(value));
                }
            }
            return sanitizedValues;
        }

        public static bool DeepEqual(object obj1, object obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }
            if (obj1 == null || obj2 == null)
            {
                return false;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                };
                var json1 = JsonSerializer.Serialize(obj1, options);
                var json2 = JsonSerializer.Serialize(obj2, options);
                return json1 == json2;
            }
            catch
            {
                return obj1.Equals(obj2);
            }
        }

        public static T SafeDeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            try
            {
                var json = JsonSerializer.Serialize(obj);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return obj;
            }
        }

        public static Dictionary<string, object> DeleteNone(Dictionary<string, object> dict)
        {
            return DeleteNone(dict, new List<string>());
        }

        public static Dictionary<string, object> DeleteNone(
            Dictionary<string, object> dict,
            bool skipValue,
            List<string>? skipProps)
        {
            dict ??= new Dictionary<string, object>();
            skipProps ??= new List<string>();

            foreach (var key in dict.Keys.ToList())
            {
                if (skipProps.Contains(key))
                {
                    continue;
                }
                if (skipValue && string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (dict[key] == null)
                {
                    dict.Remove(key);
                }
            }

            return dict;
        }

        public static Dictionary<string, object> DeleteNone(Dictionary<string, object> dict, List<string> skipProps)
        {
            if (dict == null)
            {
                return new Dictionary<string, object>();
            }

            skipProps = skipProps ?? new List<string>();
            return dict
                .Where(kvp => kvp.Value != null || skipProps.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static SemaphoreSlim SafeGetLock()
        {
            return new SemaphoreSlim(1, 1);
        }

        public static ManualResetEventSlim SafeGetStopEvent()
        {
            return new ManualResetEventSlim(false);
        }

        public static async Task RunCoroInBackgroundAsync(Func<Task> func, params object[] args)
        {
            _ = Task.Run(func);
        }

        public static string GenerateRandomString(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Task<List<string>> CancelTasksAsync(HashSet<string> taskIds)
        {
            var eventIds = new List<string>();
            if (taskIds == null || taskIds.Count == 0)
            {
                return Task.FromResult(eventIds);
            }

            foreach (var kvp in TaskNamesById.ToArray())
            {
                var name = kvp.Value;
                var sepIdx = name.IndexOf(GradioSep, StringComparison.Ordinal);
                if (sepIdx <= 0)
                {
                    continue;
                }

                var taskId = name.Substring(0, sepIdx);
                var eventId = name.Substring(sepIdx + GradioSep.Length);
                if (!taskIds.Contains(taskId))
                {
                    continue;
                }

                eventIds.Add(eventId);

                if (CancellationByTaskId.TryGetValue(kvp.Key, out var cts))
                {
                    try { cts.Cancel(); } catch { /* best-effort */ }
                }

                // Remove registry entries (best-effort).
                TaskNamesById.TryRemove(kvp.Key, out _);
                CancellationByTaskId.TryRemove(kvp.Key, out _);
            }

            return Task.FromResult(eventIds);
        }

        public static void SetTaskName(Task task, string sessionHash, int fnIndex, string eventId, bool batch)
        {
            if (!batch)
            {
                // Python name format: "{session_hash}_{fn_index}<gradio-sep>{event_id}"
                if (task == null)
                {
                    return;
                }

                var taskId = $"{sessionHash}_{fnIndex}";
                var name = $"{taskId}{GradioSep}{eventId}";
                TaskNamesById[task.Id] = name;
            }
        }

        internal static void RegisterTaskCancellation(Task task, CancellationTokenSource cts)
        {
            if (task == null || cts == null)
            {
                return;
            }
            CancellationByTaskId[task.Id] = cts;
        }

        public static object GetTheme(object theme)
        {
            if (theme == null)
            {
                return BuiltInThemes.Themes["default"];
            }
            else if (theme is string themeName)
            {
                if (BuiltInThemes.Themes.ContainsKey(themeName.ToLower()))
                {
                    return BuiltInThemes.Themes[themeName.ToLower()];
                }
                else
                {
                    try
                    {
                        return BuiltInThemes.Themes["default"];
                    }
                    catch (Exception e)
                    {
                        return BuiltInThemes.Themes["default"];
                    }
                }
            }
            return theme;
        }

        public static object GetFunctionWithLocals(
            Func<object[], object> fn,
            object blocks,
            string eventId,
            bool inEventListener,
            object request,
            object state)
        {
            if (fn == null)
            {
                throw new ArgumentNullException(nameof(fn));
            }

            var blocksObj = blocks as Core.Blocks;
            var httpRequest = request as Microsoft.AspNetCore.Http.HttpRequest;
            var sessionState = state as Core.SessionState;

            object Wrapper(object[] args)
            {
                // before_fn
                Core.LocalContext.Blocks = blocksObj;
                Core.LocalContext.InEventListener = inEventListener;
                Core.LocalContext.EventId = eventId;
                Core.LocalContext.Request = httpRequest;
                if (sessionState?.BlocksConfig is Core.BlocksConfig bc)
                {
                    Core.LocalContext.BlocksConfig = bc;
                }

                void After()
                {
                    // after_fn
                    Core.LocalContext.InEventListener = false;
                    Core.LocalContext.Request = null;
                    Core.LocalContext.BlocksConfig = null;
                }

                try
                {
                    var result = fn(args);

                    // Ensure after_fn runs after Task completion when applicable.
                    if (result is Task task)
                    {
                        _ = task.ContinueWith(_ => After(), TaskScheduler.Default);
                        return result;
                    }

                    After();
                    return result;
                }
                catch
                {
                    After();
                    throw;
                }
            }

            return (Func<object[], object>)Wrapper;
        }

        public static List<int> GetCancelledFnIndices(List<Dictionary<string, object>> dependencies)
        {
            var fnIndices = new List<int>();
            if (dependencies == null || dependencies.Count == 0)
            {
                return fnIndices;
            }

            // Python parity: resolve deps by matching dep.get_config() against root_block.fns[i].get_config()
            var rootBlocks = Core.BlockContext.GetCurrentBlocks() ?? Core.LocalContext.Blocks;
            if (rootBlocks == null)
            {
                return fnIndices;
            }

            string Serialize(Dictionary<string, object> d)
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Serialize(d);
                }
                catch
                {
                    return d.ToString() ?? string.Empty;
                }
            }

            foreach (var dep in dependencies)
            {
                var depJson = Serialize(dep);
                foreach (var kvp in rootBlocks.Fns)
                {
                    var fnConfig = kvp.Value.GetConfig();
                    var fnJson = Serialize(fnConfig);
                    if (string.Equals(depJson, fnJson, StringComparison.Ordinal))
                    {
                        fnIndices.Add(kvp.Key);
                        break;
                    }
                }
            }

            return fnIndices;
        }

        public static Dictionary<string, Type> GetTypeHints(Delegate fn)
        {
            var typeHints = new Dictionary<string, Type>();
            var method = fn.Method;
            foreach (var param in method.GetParameters())
            {
                typeHints[param.Name] = param.ParameterType;
            }
            return typeHints;
        }

        public static Dictionary<string, ParameterInfo> GetFunctionParams(MethodInfo method)
        {
            if (method == null)
            {
                return new Dictionary<string, ParameterInfo>();
            }

            return method.GetParameters()
                .ToDictionary(p => p.Name ?? string.Empty, p => p);
        }

        public static Dictionary<string, ParameterInfo> GetFunctionParams(Delegate fn)
        {
            return fn != null ? GetFunctionParams(fn.Method) : new Dictionary<string, ParameterInfo>();
        }

        public static bool IsSpecialTypedParameter(ParameterInfo param)
        {
            if (param == null)
            {
                return false;
            }

            var specialTypeNames = new[]
            {
                "HttpRequest",
                "HttpContext",
                "EventData",
                "State",
                "Request",
                "GradioRequest",
                "Progress",
                "SessionState"
            };

            return specialTypeNames.Contains(param.ParameterType.Name);
        }

        public static bool IsSpecialTypedParameter(string name, Dictionary<string, Type> parameterTypes)
        {
            if (parameterTypes == null || !parameterTypes.ContainsKey(name))
            {
                return false;
            }

            var type = parameterTypes[name];
            var specialTypeNames = new[]
            {
                "HttpRequest",
                "HttpContext",
                "EventData",
                "State",
                "Request",
                "GradioRequest",
                "Progress",
                "SessionState"
            };

            return specialTypeNames.Contains(type.Name);
        }

        public static Type GetFunctionReturnType(MethodInfo method)
        {
            return method?.ReturnType ?? typeof(void);
        }

        public static Type GetFunctionReturnType(Delegate fn)
        {
            return fn != null ? GetFunctionReturnType(fn.Method) : typeof(void);
        }

        public static void CheckFunctionInputsMatch(Delegate fn, List<object> inputs, bool inputsAsDict)
        {
            if (fn == null)
            {
                throw new ArgumentNullException(nameof(fn));
            }

            var parameters = fn.Method.GetParameters();
            var nonSpecialParams = parameters.Where(p => !IsSpecialTypedParameter(p)).ToList();

            if (!inputsAsDict && inputs != null && inputs.Count != nonSpecialParams.Count)
            {
                throw new ArgumentException(
                    $"Function expects {nonSpecialParams.Count} inputs but {inputs?.Count ?? 0} were provided.");
            }
        }

        private static readonly List<string> _staticPaths = new List<string>();

        public static void SetStaticPaths(params string[] paths)
        {
            _staticPaths.Clear();
            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                try
                {
                    var fullPath = Path.GetFullPath(path);
                    _staticPaths.Add(fullPath);
                }
                catch
                {
                    // Ignore invalid paths
                }
            }
        }

        public static bool IsStaticFile(object filePath)
        {
            return _IsStaticFile(filePath, _staticPaths);
        }

        private static bool _IsStaticFile(object filePath, List<string> staticFiles)
        {
            if (filePath == null)
            {
                return false;
            }

            string? pathString = filePath switch
            {
                string str => str,
                FileInfo fileInfo => fileInfo.FullName,
                _ => filePath.ToString()
            };

            if (string.IsNullOrWhiteSpace(pathString))
            {
                return false;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(pathString);
            }
            catch
            {
                return false;
            }

            foreach (var staticPath in staticFiles)
            {
                if (string.IsNullOrWhiteSpace(staticPath))
                {
                    continue;
                }

                try
                {
                    var fullStaticPath = Path.GetFullPath(staticPath);
                    if (IsInOrEqual(fullPath, fullStaticPath))
                    {
                        return true;
                    }
                }
                catch
                {
                    // Ignore invalid static path
                }
            }

            return false;
        }

        public static long? ParseFileSize(object size)
        {
            if (size is long longSize)
            {
                return longSize;
            }
            if (size is null)
            {
                return null;
            }

            var sizeStr = size.ToString().Replace(" ", "");
            var lastDigitIndex = sizeStr.IndexOfAny(new[] { 'B', 'K', 'M', 'G', 'T' });
            if (lastDigitIndex == -1)
            {
                return long.TryParse(sizeStr, out var parsedSize) ? parsedSize : null;
            }

            if (!long.TryParse(sizeStr.Substring(0, lastDigitIndex), out var sizeInt))
            {
                return null;
            }

            var unit = sizeStr.Substring(lastDigitIndex).ToUpper();
            var multiple = unit switch
            {
                "B" => FileSize.B,
                "KB" => FileSize.KB,
                "MB" => FileSize.MB,
                "GB" => FileSize.GB,
                "TB" => FileSize.TB,
                _ => throw new ArgumentException($"Invalid file size unit: {unit}")
            };

            return sizeInt * multiple;
        }

        public static IEnumerable<string> DefaultInputLabels()
        {
            var n = 0;
            while (true)
            {
                yield return $"input {n}";
                n++;
            }
        }

        public static List<object> NoneOrSingletonToList(object value)
        {
            if (value == null)
            {
                return new List<object>();
            }
            if (value is List<object> list)
            {
                return list;
            }
            // Python parity: none_or_singleton_to_list treats list and tuple as iterables.
            // In C#, handle any IEnumerable (arrays like string[], List<string>, etc.) but NOT strings.
            if (!(value is string) && value is System.Collections.IEnumerable enumerable)
            {
                var result = new List<object>();
                foreach (var item in enumerable)
                {
                    result.Add(item);
                }
                return result;
            }
            return new List<object> { value };
        }

        public static object GetComponentInstance(object component, bool render = false)
        {
            if (component == null)
            {
                return null;
            }

            // If it's already an instance, handle render/unrender like Python's get_component_instance
            if (component is Core.Block block)
            {
                if (render && !block.IsRendered)
                {
                    block.Render();
                }
                return component;
            }

            // If it's a type, try to create an instance
            if (component is Type componentType)
            {
                try
                {
                    var instance = Activator.CreateInstance(componentType);
                    return instance;
                }
                catch
                {
                    return null;
                }
            }

            return component;
        }

        public static Dictionary<string, object> GetConstructorArgs(Type type, object kwargs)
        {
            var args = new Dictionary<string, object>();

            if (kwargs == null)
            {
                return args;
            }

            // If kwargs is a dictionary, return it
            if (kwargs is Dictionary<string, object> dict)
            {
                return new Dictionary<string, object>(dict);
            }

            // Otherwise, extract properties using reflection
            var properties = kwargs.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var value = prop.GetValue(kwargs);
                if (value != null)
                {
                    args[ToCamelCase(prop.Name)] = value;
                }
            }

            return args;
        }

        public static string GetIconPath(string icon)
        {
            if (string.IsNullOrEmpty(icon))
            {
                return null;
            }

            // If it's already a full path or URL, return as-is
            if (icon.StartsWith("http://") || icon.StartsWith("https://") || icon.StartsWith("/") || Path.IsPathRooted(icon))
            {
                return icon;
            }

            // Check if it's in the icons directory
            var iconsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "icons");
            var iconPath = Path.Combine(iconsDir, icon);

            if (File.Exists(iconPath))
            {
                return $"/assets/icons/{icon}";
            }

            // Return as-is if not found (might be a data URI or external reference)
            return icon;
        }

        private static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
            {
                return str;
            }
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        // Flagging utility methods
        public static string StripInvalidFilenameCharacters(string filename)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(filename.Where(c => !invalidChars.Contains(c)).ToArray());
            return sanitized;
        }

        public static string SimplifyFileDataInString(string data)
        {
            // Simplify file data in string
            return data;
        }

        public static bool IsPropUpdate(object sample)
        {
            // Check if object is a property update
            if (sample is Dictionary<string, object> dict)
            {
                return dict.TryGetValue("__type__", out var t) &&
                       t != null &&
                       t.ToString()!.Contains("update", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public static List<object?> GetDefaultArgs(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var defaults = new List<object?>();

            foreach (var param in parameters)
            {
                if (param.HasDefaultValue)
                {
                    defaults.Add(param.DefaultValue);
                }
                else
                {
                    defaults.Add(null);
                }
            }

            return defaults;
        }

        public static void ReassignKeys(Core.Blocks oldBlocks, Core.Blocks newBlocks)
        {
            var newKeys = new HashSet<string>(
                newBlocks.BlocksDict.Values
                    .Where(b => b.Key != null)
                    .Select(b => b.Key?.ToString() ?? "")
                    .Where(k => !string.IsNullOrEmpty(k))
            );

            void ReassignContextKeys(Core.Block? oldBlock, Core.Block newBlock)
            {
                bool sameBlockType = oldBlock?.GetType() == newBlock.GetType();
                var newBlockKey = newBlock.Key?.ToString();

                if (newBlockKey == null)
                {
                    var oldBlockKey = oldBlock?.Key?.ToString();
                    if (sameBlockType && oldBlock != null && !string.IsNullOrEmpty(oldBlockKey) && !newKeys.Contains(oldBlockKey))
                    {
                        // Check if values are equal for stateful components
                        var oldValue = oldBlock.GetType().GetProperty("Value")?.GetValue(oldBlock);
                        var newValue = newBlock.GetType().GetProperty("Value")?.GetValue(newBlock);

                        if (DeepEqual(oldValue, newValue))
                        {
                            newBlock.Key = oldBlock.Key;
                        }
                        else
                        {
                            newBlock.Key = $"__{newBlock._id}__";
                        }
                    }
                    else
                    {
                        newBlock.Key = $"__{newBlock._id}__";
                    }
                }

                // Recursively handle BlockContext children
                if (newBlock is Core.BlockContext newContext && sameBlockType && oldBlock is Core.BlockContext oldContext)
                {
                    for (int i = 0; i < newContext.Children.Count; i++)
                    {
                        var oldChild = i < oldContext.Children.Count ? oldContext.Children[i] : null;
                        ReassignContextKeys(oldChild, newContext.Children[i]);
                    }
                }
            }

            ReassignContextKeys(oldBlocks, newBlocks);
        }

        public static object? ResolveSingleton(object? value)
        {
            if (value is IList list && list.Count == 1)
            {
                return list[0];
            }
            return value;
        }

        public static List<Type> GetReturnTypes(MethodInfo method)
        {
            var returnType = method.ReturnType;
            var types = new List<Type>();

            // Handle async Task<T>
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            // Handle tuple returns
            if (returnType.IsGenericType && returnType.Name.StartsWith("Tuple`") || returnType.Name.StartsWith("ValueTuple`"))
            {
                types.AddRange(returnType.GetGenericArguments());
            }
            else if (returnType != typeof(void) && returnType != typeof(Task))
            {
                types.Add(returnType);
            }

            return types;
        }

        public static string DeepHash(object? obj)
        {
            using var sha256 = SHA256.Create();
            var json = JsonSerializer.Serialize(obj);
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public static bool IsAllowedFile(string filepath, string? allowedPath = null)
        {
            try
            {
                var absPath = Path.GetFullPath(filepath);

                if (allowedPath != null)
                {
                    var absAllowedPath = Path.GetFullPath(allowedPath);
                    return absPath.StartsWith(absAllowedPath, StringComparison.OrdinalIgnoreCase);
                }

                // Check if file is attempting directory traversal
                return !filepath.Contains("..") && !absPath.Contains("..");
            }
            catch
            {
                return false;
            }
        }

        public static string StripInvalidFilenameCharacters(string filename, int maxLength = 255)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Concat(filename.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            if (sanitized.Length > maxLength)
            {
                sanitized = sanitized.Substring(0, maxLength);
            }

            return sanitized;
        }

        public static string? GetNodePath()
        {
            try
            {
                // Check GRADIO_NODE_PATH environment variable
                var envPath = Environment.GetEnvironmentVariable("GRADIO_NODE_PATH");
                if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
                {
                    return envPath;
                }

                // On Windows, check common install locations
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var commonPaths = new[]
                    {
                        @"C:\Program Files\nodejs\node.exe",
                        @"C:\Program Files (x86)\nodejs\node.exe"
                    };

                    foreach (var path in commonPaths)
                    {
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
                else
                {
                    // On Unix systems, use 'which' command
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "which",
                            Arguments = "node",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    var result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(result) && File.Exists(result))
                    {
                        return result;
                    }
                }

                // Check PATH environment variable
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (pathVar != null)
                {
                    var paths = pathVar.Split(Path.PathSeparator);
                    var nodeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "node.exe" : "node";

                    foreach (var dir in paths)
                    {
                        var fullPath = Path.Combine(dir, nodeName);
                        if (File.Exists(fullPath))
                        {
                            return fullPath;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to null if node is not found
            }

            return null;
        }

        public static (string Description, Dictionary<string, string> Parameters, List<string> Returns) GetFunctionDescription(MethodInfo method)
        {
            if (method == null)
            {
                return ("", new Dictionary<string, string>(), new List<string>());
            }

            // Python parity target: gradio.utils.get_function_description()
            // - description: summary/description text
            // - parameters: map param name -> description
            // - returns: list of return lines

            var parameters = method.GetParameters()
                .ToDictionary(p => p.Name ?? string.Empty, _ => string.Empty);
            var returns = new List<string>();

            // Prefer DescriptionAttribute when present
            var descAttr = method.GetCustomAttributes(inherit: true)
                .OfType<System.ComponentModel.DescriptionAttribute>()
                .FirstOrDefault();
            var description = descAttr?.Description ?? string.Empty;

            try
            {
                // Parse the XML documentation file if it exists next to the assembly.
                var asm = method.DeclaringType?.Assembly;
                var asmLocation = asm?.Location;
                if (!string.IsNullOrWhiteSpace(asmLocation))
                {
                    var xmlPath = Path.ChangeExtension(asmLocation, ".xml");
                    if (File.Exists(xmlPath))
                    {
                        var xdoc = XDocument.Load(xmlPath);
                        var declaringType = method.DeclaringType?.FullName ?? string.Empty;
                        var memberPrefix = $"M:{declaringType}.{method.Name}";

                        var member = xdoc.Descendants("member")
                            .FirstOrDefault(m => (string?)m.Attribute("name") is string n && n.StartsWith(memberPrefix, StringComparison.Ordinal));

                        if (member != null)
                        {
                            static string CollapseWhitespace(string? s)
                                => string.Join(" ", (s ?? string.Empty)
                                    .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

                            var summary = CollapseWhitespace(member.Element("summary")?.Value);
                            if (!string.IsNullOrWhiteSpace(summary) && string.IsNullOrWhiteSpace(description))
                            {
                                description = summary;
                            }

                            foreach (var p in member.Elements("param"))
                            {
                                var name = (string?)p.Attribute("name") ?? string.Empty;
                                if (string.IsNullOrWhiteSpace(name))
                                    continue;
                                if (!parameters.ContainsKey(name))
                                    continue;
                                parameters[name] = CollapseWhitespace(p.Value);
                            }

                            var ret = CollapseWhitespace(member.Element("returns")?.Value);
                            if (!string.IsNullOrWhiteSpace(ret))
                            {
                                returns.Add(ret);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Documentation is optional. Ignore parsing failures.
            }

            return (description, parameters, returns);
        }

        public static Dictionary<string, object> ErrorPayload(Exception error, bool showError)
        {
            var content = new Dictionary<string, object>
            {
                ["error"] = null
            };

            // Show error if showError is true or if it's a specific Gradio error type
            bool shouldShow = showError || error is Core.Exceptions.Error;

            if (shouldShow)
            {
                if (error is Core.Exceptions.Error gradioError)
                {
                    content["error"] = gradioError.Message;
                    content["visible"] = true;
                    // Add other error properties if available
                }
                else
                {
                    content["error"] = error.Message;
                    content["visible"] = showError;
                }
            }

            return content;
        }

        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
                return default;

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(obj);
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                // Fallback for types that don't serialize well
                return obj;
            }
        }

        public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
            Dictionary<TKey, TValue> dict1,
            Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == null) return dict2 ?? new Dictionary<TKey, TValue>();
            if (dict2 == null) return new Dictionary<TKey, TValue>(dict1);

            var result = new Dictionary<TKey, TValue>(dict1);
            foreach (var kvp in dict2)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        public static bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Must start with letter or underscore
            if (!char.IsLetter(name[0]) && name[0] != '_')
                return false;

            // Rest must be letters, digits, or underscores
            return name.All(c => char.IsLetterOrDigit(c) || c == '_');
        }

        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public static List<object[]> Diff(object oldObj, object newObj)
        {
            return CompareObjects(oldObj, newObj, new List<object>());
        }

        public static bool ConnectHeartbeat(
            Dictionary<string, object> config,
            IEnumerable<Core.Block> blocks)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (!config.TryGetValue("dependencies", out var depsObj) || depsObj == null)
            {
                throw new InvalidOperationException(
                    "Dependencies not found in config. Cannot determine whether heartbeat is required.");
            }

            blocks ??= Array.Empty<Core.Block>();

            bool anyState = blocks.Any(b => b != null && b.Stateful);

            bool anyUnload = false;
            bool anyStream = false;

            if (depsObj is IEnumerable<object> deps)
            {
                foreach (var depObj in deps)
                {
                    if (depObj is not Dictionary<string, object> dep)
                    {
                        continue;
                    }

                    if (!dep.TryGetValue("targets", out var targetsObj) || targetsObj is not IEnumerable<object> targets)
                    {
                        continue;
                    }

                    foreach (var target in targets)
                    {
                        // target is typically [id, eventName]
                        if (target is IEnumerable<object> pair)
                        {
                            var list = pair.ToList();
                            if (list.Count >= 2)
                            {
                                var eventName = list[1]?.ToString();
                                if (string.Equals(eventName, "unload", StringComparison.OrdinalIgnoreCase))
                                {
                                    anyUnload = true;
                                    break;
                                }
                                if (string.Equals(eventName, "stream", StringComparison.OrdinalIgnoreCase))
                                {
                                    anyStream = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (anyUnload || anyStream)
                    {
                        break;
                    }
                }
            }

            return anyState || anyUnload || anyStream;
        }

        private static List<object[]> CompareObjects(object obj1, object obj2, List<object> path)
        {
            var edits = new List<object[]>();

            // If objects are equal, no changes
            if (Equals(obj1, obj2))
            {
                return edits;
            }

            // If types are different, replace entirely
            if (obj1?.GetType() != obj2?.GetType())
            {
                edits.Add(new object[] { "replace", path.ToList(), obj2 });
                return edits;
            }

            // String append optimization: if new string starts with old string
            if (obj1 is string str1 && obj2 is string str2 && str2.StartsWith(str1))
            {
                edits.Add(new object[] { "append", path.ToList(), str2.Substring(str1.Length) });
                return edits;
            }

            // Handle lists/arrays
            if (obj1 is IList list1 && obj2 is IList list2)
            {
                var commonLength = Math.Min(list1.Count, list2.Count);

                // Compare common elements
                for (int i = 0; i < commonLength; i++)
                {
                    var newPath = new List<object>(path) { i };
                    edits.AddRange(CompareObjects(list1[i], list2[i], newPath));
                }

                // Handle deletions
                for (int i = commonLength; i < list1.Count; i++)
                {
                    var newPath = new List<object>(path) { i };
                    edits.Add(new object[] { "delete", newPath, null });
                }

                // Handle additions
                for (int i = commonLength; i < list2.Count; i++)
                {
                    var newPath = new List<object>(path) { i };
                    edits.Add(new object[] { "add", newPath, list2[i] });
                }

                // Adjust delete indices (they shift as elements are deleted)
                int deletesSeen = 0;
                foreach (var edit in edits)
                {
                    if (edit[0] as string == "delete" && edit[1] is List<object> editPath &&
                        editPath.Count > 0 && editPath.Last() is int lastIndex)
                    {
                        editPath[editPath.Count - 1] = lastIndex - deletesSeen;
                        deletesSeen++;
                    }
                }

                return edits;
            }

            // Handle dictionaries
            if (obj1 is IDictionary dict1 && obj2 is IDictionary dict2)
            {
                // Check for modified and deleted keys
                foreach (var key in dict1.Keys)
                {
                    if (dict2.Contains(key))
                    {
                        var newPath = new List<object>(path) { key };
                        edits.AddRange(CompareObjects(dict1[key], dict2[key], newPath));
                    }
                    else
                    {
                        var newPath = new List<object>(path) { key };
                        edits.Add(new object[] { "delete", newPath, null });
                    }
                }

                // Check for added keys
                foreach (var key in dict2.Keys)
                {
                    if (!dict1.Contains(key))
                    {
                        var newPath = new List<object>(path) { key };
                        edits.Add(new object[] { "add", newPath, dict2[key] });
                    }
                }

                return edits;
            }

            // For other types, replace entirely
            edits.Add(new object[] { "replace", path.ToList(), obj2 });
            return edits;
        }
    }
}

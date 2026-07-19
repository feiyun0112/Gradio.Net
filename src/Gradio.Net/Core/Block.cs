using System.Reflection;
using System.Text;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Core
{
    public class Block
    {
        public bool IsRenderReplacement { get; set; }
        public int _id { get; set; }
        public bool Visible { get; set; }
        public string ElemId { get; set; }
        public List<string> ElemClasses { get; set; }
        public string ProxyUrl { get; set; }
        public string ShareToken { get; set; }
        public BlockContext Parent { get; set; }
        public object RenderedIn { get; set; }
        public string Page { get; set; }
        public bool IsRendered { get; set; }
        public List<Dictionary<string, object>> _constructor_args { get; set; }
        public int StateSessionCapacity { get; set; }
        public HashSet<string> TempFiles { get; set; }
        public string GRADIO_CACHE { get; set; }
        public object Key { get; set; }
        public List<string> PreservedByKey { get; set; }
        public object McpServerObj { get; set; }
        public HashSet<string> KeepInCache { get; set; }
        public bool HasLaunched { get; set; }
        public long RenderSequence { get; set; }

        // Static counter for generating unique IDs
        private static int _nextId = 0;
        private static readonly object _idLock = new object();
        private static long _nextRenderSequence = 0;
        private static readonly object _renderSequenceLock = new object();

        protected static int GenerateId()
        {
            lock (_idLock)
            {
                return _nextId++;
            }
        }

        protected internal static long GenerateRenderSequence()
        {
            lock (_renderSequenceLock)
            {
                return ++_nextRenderSequence;
            }
        }

        protected Block(bool _proxyMode)
        {
            _id = -1; // caller must overwrite
            _constructor_args = new List<Dictionary<string, object>>();
            ElemClasses = new List<string>();
            PreservedByKey = new List<string>();
            TempFiles = new HashSet<string>();
            KeepInCache = new HashSet<string>();
        }

        public Block(
            string elemId = null,
            List<string> elemClasses = null,
            bool visible = true,
            bool render = true,
            object key = null,
            List<string> preservedByKey = null,
            string proxyUrl = null)
        {
            // Check if this is a render replacement using key_to_id_map
            var keyToIdMap = LocalContext.KeyToIdMap;
            if (key != null && keyToIdMap != null && keyToIdMap.ContainsKey(key))
            {
                IsRenderReplacement = true;
                _id = keyToIdMap[key];
            }
            else
            {
                IsRenderReplacement = false;
                _id = Context.Id;
                if (key != null && keyToIdMap != null)
                {
                    keyToIdMap[key] = _id;
                }
            }

            // Initialize properties
            Visible = visible;
            ElemId = elemId;
            ElemClasses = elemClasses ?? new List<string>();
            ProxyUrl = proxyUrl;
            ShareToken = Gradio.Net.Utils.Utils.GenerateRandomString(32);
            Parent = null;
            RenderedIn = null;
            IsRendered = false;
            RenderSequence = 0;
            _constructor_args = new List<Dictionary<string, object>>();
            StateSessionCapacity = 10000;
            TempFiles = new HashSet<string>();
            GRADIO_CACHE = Gradio.Net.Utils.Utils.GetUploadFolder();
            Key = key;
            PreservedByKey = preservedByKey ?? new List<string>();
            McpServerObj = null;
            KeepInCache = new HashSet<string>();
            HasLaunched = false;

            // Add constructor arguments to tracking list
            var constructorArgs = new Dictionary<string, object>
            {
                { "elem_id", elemId },
                { "elem_classes", elemClasses },
                { "visible", visible },
                { "render", render },
                { "key", key },
                { "preserved_by_key", preservedByKey },
                { "proxy_url", proxyUrl }
            };
            _constructor_args.Add(constructorArgs);

            // Render if requested
            if (render)
            {
                Render();
            }
        }

        public virtual int? UniqueKey()
        {
            if (Key == null)
            {
                return null;
            }

            int renderedInId = 0;
            if (RenderedIn is IRenderable renderable)
            {
                renderedInId = renderable.Id;
            }
            else if (RenderedIn is Block block)
            {
                renderedInId = block._id;
            }

            return HashCode.Combine(renderedInId, Key.GetHashCode());
        }

        public virtual bool Stateful => false;

        public virtual bool SkipApi => false;

        public virtual Dictionary<string, object> ConstructorArgs
        {
            get
            {
                return _constructor_args.FirstOrDefault() ?? new Dictionary<string, object>();
            }
        }

        public virtual List<EventListener> Events => new List<EventListener>();

        public virtual IReadOnlyList<string> KeepNullProps => Array.Empty<string>();

        public virtual List<(int? blockId, string eventName)> Targets => new List<(int? blockId, string eventName)>();

        public virtual bool DefaultRender => true;

        public virtual void Render()
        {
            var rootContext = Context.GetBlocksContext();
            var renderContext = Context.GetRenderContext();
            RenderedIn = LocalContext.Renderable as object;

            // Check if already rendered
            if (rootContext != null && rootContext.Blocks.ContainsKey(_id) && !IsRenderReplacement)
            {
                throw new DuplicateBlockError($"A block with id: {_id} has already been rendered in the current Blocks.");
            }

            // Add to render context
            if (renderContext != null)
            {
                if (rootContext != null)
                {
                    Page = rootContext.RootBlock?.CurrentPage;
                }
                renderContext.Add(this);
                Parent = renderContext;
            }

            // Add to root context
            if (rootContext != null)
            {
                rootContext.Blocks[_id] = this;
                RenderSequence = GenerateRenderSequence();
                IsRendered = true;

                // If this is a Component, add temp files to the root block
                if (rootContext.RootBlock != null && this is Components.Component)
                {
                    rootContext.RootBlock.TempFileSets.Add(TempFiles);
                }
            }
        }

        public virtual void Unrender()
        {
            var rootContext = Context.GetBlocksContext();

            // Remove from parent's children
            if (Parent != null)
            {
                try
                {
                    Parent.Children.Remove(this);
                    Parent = null;
                }
                catch
                {
                    // Ignore if not in children list
                }
            }

            // Remove from root context
            if (rootContext != null)
            {
                try
                {
                    rootContext.Blocks.Remove(_id);
                    IsRendered = false;
                }
                catch
                {
                    // Ignore if not in blocks dictionary
                }
            }
        }

        public virtual string GetBlockName()
        {
            return GetType().Name.ToLower();
        }

        public virtual string GetBlockClass()
        {
            return GetType().Name.ToLower();
        }

        public virtual Type GetExpectedParent()
        {
            return null;
        }

        public virtual bool BreaksGrouping()
        {
            return true;
        }

        public virtual Dictionary<string, object> GetConfig(Type cls = null)
        {
            var config = new Dictionary<string, object>();
            cls ??= GetType();

            // Get constructor parameters from the class
            var constructors = cls.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length > 0)
            {
                var constructor = constructors[0];
                var parameters = constructor.GetParameters();

                foreach (var parameter in parameters)
                {
                    // Try to get property with same name (case-insensitive)
                    var property = cls
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(p =>
                            p.GetIndexParameters().Length == 0 &&
                            string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));
                    if (property != null && property.CanRead)
                    {
                        object value;
                        try
                        {
                            value = property.GetValue(this);
                        }
                        catch
                        {
                            continue;
                        }

                        // Convert value if it's a complex type (similar to Python's dataclasses.asdict)
                        if (value != null && IsDataClass(value.GetType()))
                        {
                            value = ConvertToDict(value);
                        }

                        // Python parity: config keys are snake_case.
                        var configKey = ToSnakeCase(parameter.Name);
                        config[configKey] = value;
                    }
                }
            }

            // Add events configuration
            foreach (var e in Events)
            {
                var eventConfig = e.ConfigData();
                if (eventConfig != null && eventConfig.Count > 0)
                {
                    // Merge event config first, then override with existing config
                    foreach (var kvp in eventConfig)
                    {
                        if (!config.ContainsKey(kvp.Key))
                        {
                            config[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }

            // Remove render property if present
            config.Remove("render");
            config.Remove("Render");

            // Add proxy_url and name
            config["proxy_url"] = ProxyUrl;
            config["name"] = GetBlockClass();

            // Add event attributes if they exist
            var eventAttributes = new[] { "_selectable", "_undoable", "_retryable", "likeable" };
            foreach (var attrName in eventAttributes)
            {
                var property = GetType().GetProperty(attrName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null && property.CanRead)
                {
                    var value = property.GetValue(this);
                    if (value != null)
                    {
                        config[attrName] = value;
                    }
                }
            }

            return config;
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var sb = new StringBuilder(name.Length + 8);
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private bool IsDataClass(Type type)
        {
            // Check if it's a simple type
            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum)
            {
                return false;
            }

            // Delegates and reflection metadata should not be traversed as dataclasses.
            if (typeof(Delegate).IsAssignableFrom(type) ||
                typeof(MemberInfo).IsAssignableFrom(type) ||
                typeof(Type).IsAssignableFrom(type))
            {
                return false;
            }

            // Collections should be serialized as collections, not object-property maps
            // (e.g. List<string> must stay [] instead of { capacity, count }).
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }

            // Check if it has the DataContract or similar attributes
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Length > 0;
        }

        private Dictionary<string, object> ConvertToDict(object obj)
        {
            if (obj == null) return null;

            var dict = new Dictionary<string, object>();
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.CanRead)
                {
                    if (prop.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    object value;
                    try
                    {
                        value = prop.GetValue(obj);
                    }
                    catch
                    {
                        continue;
                    }

                    if (value != null && IsDataClass(value.GetType()) && !value.GetType().IsEnum)
                    {
                        value = ConvertToDict(value);
                    }
                    dict[prop.Name.ToLower()] = value;
                }
            }

            return dict;
        }

        public static Dictionary<string, object> RecoverKwargs(Dictionary<string, object> props, List<string> additionalKeys = null)
        {
            return RecoverKwargs(typeof(Block), props, additionalKeys);
        }

        public virtual async Task<string> AsyncMoveResourceToBlockCache(string urlOrFilePath)
        {
            if (urlOrFilePath == null)
                return null;

            // Handle HTTP URLs - download to cache with SSRF protection
            if (urlOrFilePath.StartsWith("http://") || urlOrFilePath.StartsWith("https://"))
            {
                try
                {
                    var tempFilePath = await ProcessingUtils.AsyncSsrfProtectedDownload(
                        urlOrFilePath,
                        GRADIO_CACHE
                    );
                    TempFiles.Add(tempFilePath);
                    return tempFilePath;
                }
                catch (Exception ex)
                {
                    // Return original URL on failure
                    return urlOrFilePath;
                }
            }
            else
            {
                // Handle local file paths
                try
                {
                    var absPath = Path.GetFullPath(urlOrFilePath);

                    if (!Utils.Utils.IsInOrEqual(absPath, GRADIO_CACHE))
                    {
                        // Copy file to cache
                        var tempFilePath = ProcessingUtils.SaveFileToCache(absPath, GRADIO_CACHE);
                        TempFiles.Add(tempFilePath);
                        return tempFilePath;
                    }
                    else
                    {
                        // Already in cache
                        TempFiles.Add(absPath);
                        return absPath;
                    }
                }
                catch (FileNotFoundException)
                {
                    // This can happen if when using gr.load() and the file is on a remote Space
                    // but the file is not the `value` of the component. For example, if the file
                    // is the `avatar_image` of the `Chatbot` component. In this case, we skip
                    // copying the file to the cache and just use the remote file path.
                    return urlOrFilePath;
                }
            }
        }

        public virtual string MoveResourceToBlockCache(string urlOrFilePath)
        {
            if (urlOrFilePath == null)
                return null;

            // Handle HTTP URLs with SSRF protection
            if (urlOrFilePath.StartsWith("http://") || urlOrFilePath.StartsWith("https://"))
            {
                try
                {
                    var tempFilePath = ProcessingUtils.SaveUrlToCache(urlOrFilePath, GRADIO_CACHE);
                    TempFiles.Add(tempFilePath);
                    return tempFilePath;
                }
                catch (Exception ex)
                {
                    // Return original URL on failure
                    return urlOrFilePath;
                }
            }
            else
            {
                // Handle local file paths
                try
                {
                    var absPath = Path.GetFullPath(urlOrFilePath);

                    if (!Utils.Utils.IsInOrEqual(absPath, GRADIO_CACHE))
                    {
                        // Copy file to cache
                        var tempFilePath = ProcessingUtils.SaveFileToCache(absPath, GRADIO_CACHE);
                        TempFiles.Add(tempFilePath);
                        return tempFilePath;
                    }
                    else
                    {
                        // Already in cache
                        TempFiles.Add(absPath);
                        return absPath;
                    }
                }
                catch (FileNotFoundException)
                {
                    // This can happen if when using gr.load() and the file is on a remote Space
                    // but the file is not the `value` of the component. For example, if the file
                    // is the `avatar_image` of the `Chatbot` component. In this case, we skip
                    // copying the file to the cache and just use the remote file path.
                    return urlOrFilePath;
                }
            }
        }

        public virtual Dictionary<string, object> ServeStaticFile(object urlOrFilePath)
        {
            if (urlOrFilePath == null)
            {
                return null;
            }

            // If already a dictionary, return as is
            if (urlOrFilePath is Dictionary<string, object> dict)
            {
                return dict;
            }

            string path;
            if (urlOrFilePath is FileInfo fileInfo)
            {
                path = fileInfo.FullName;
            }
            else
            {
                path = urlOrFilePath.ToString();
            }

            // Check if it's an HTTP URL
            if (IsHttpUrl(path))
            {
                return new Dictionary<string, object>
                {
                    { "path", path },
                    { "url", path }
                };
            }

            // Local file - create FileData structure
            var data = new Dictionary<string, object>
            {
                { "path", path },
                { "meta", new Dictionary<string, object> { { "_type", "gradio.FileData" } } }
            };

            try
            {
                // Move file to cache (simplified version)
                var cachedPath = MoveResourceToBlockCache(path);
                if (cachedPath != null)
                {
                    data["path"] = cachedPath;
                    data["url"] = $"/file={cachedPath}";
                }
                return data;
            }
            catch (Exception)
            {
                // Can be raised if this function is called before the Block is fully initialized
                return data;
            }
        }

        public virtual bool CheckInUploadFolder(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return false;
            }

            return Utils.Utils.IsInOrEqual(filepath, GRADIO_CACHE);
        }

        public virtual object StripInvalidFiles(object data)
        {
            // Recursively check and remove invalid file references
            if (data == null)
                return null;

            if (data is string str)
            {
                // Check if string is a file path
                if (IsFilePath(str) && !File.Exists(str) && !IsHttpUrl(str))
                {
                    return null; // Invalid file path
                }
                return data;
            }

            if (data is FileData fileData)
            {
                // Validate file data
                if (!string.IsNullOrEmpty(fileData.Path))
                {
                    if (!File.Exists(fileData.Path) && !IsHttpUrl(fileData.Path))
                    {
                        return null; // Invalid file
                    }
                }
                return fileData;
            }

            if (data is System.Collections.IDictionary dict)
            {
                var result = new Dictionary<object, object>();
                foreach (var key in dict.Keys)
                {
                    var value = StripInvalidFiles(dict[key]);
                    if (value != null)
                    {
                        result[key] = value;
                    }
                }
                return result;
            }

            if (data is System.Collections.IList list)
            {
                var result = new List<object>();
                foreach (var item in list)
                {
                    var value = StripInvalidFiles(item);
                    if (value != null)
                    {
                        result.Add(value);
                    }
                }
                return result;
            }

            return data;
        }

        private static bool IsFilePath(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            // Check for common path indicators
            return str.Contains(Path.DirectorySeparatorChar) ||
                   str.Contains(Path.AltDirectorySeparatorChar) ||
                   Path.IsPathRooted(str);
        }

        private static bool IsHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public virtual bool IsCurrentlyStreaming()
        {
            // Check if there are any pending streams
            var rootContext = GetCurrentBlocksContext();
            if (rootContext != null && rootContext is Blocks blocks)
            {
                return blocks.PendingStreams?.ContainsKey(_id.ToString()) == true ||
                       blocks.PendingDiffStreams?.ContainsKey(_id.ToString()) == true;
            }
            return false;
        }

        private static BlockContext GetCurrentBlocksContext()
        {
            // This would need to be implemented with proper context management
            // For now, return null
            return null;
        }

        public virtual Dictionary<string, object> DeleteNullValues(Dictionary<string, object> obj)
        {
            if (obj == null)
            {
                return null;
            }

            var result = new Dictionary<string, object>();
            foreach (var kvp in obj)
            {
                if (kvp.Value != null)
                {
                    // Recursively delete null values from nested dictionaries
                    if (kvp.Value is Dictionary<string, object> nestedDict)
                    {
                        var cleaned = DeleteNullValues(nestedDict);
                        if (cleaned != null && cleaned.Count > 0)
                        {
                            result[kvp.Key] = cleaned;
                        }
                    }
                    else
                    {
                        result[kvp.Key] = kvp.Value;
                    }
                }
            }

            return result;
        }

        public virtual Type GetExpectedParentType()
        {
            return GetExpectedParent();
        }

        public virtual object GetLayout()
        {
            // Default implementation returns null
            // Derived classes can override to provide specific layout
            return null;
        }

        public static Dictionary<string, object> RecoverKwargs(Type cls, Dictionary<string, object> props, List<string> additionalKeys = null)
        {
            additionalKeys ??= new List<string>();
            var kwargs = new Dictionary<string, object>();

            // Get constructor parameters
            var constructors = cls.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length > 0)
            {
                var constructor = constructors[0];
                var parameters = constructor.GetParameters();

                foreach (var param in parameters)
                {
                    var paramName = param.Name;
                    if (props.ContainsKey(paramName) && !additionalKeys.Contains(paramName))
                    {
                        kwargs[paramName] = props[paramName];
                    }
                }
            }

            return kwargs;
        }

        public static List<Block> ActiveBlocksInCurrentContext()
        {
            var context = GetCurrentBlocksContext();
            if (context != null && context is Blocks blocks)
            {
                return blocks.BlocksDict?.Values.ToList() ?? new List<Block>();
            }
            return new List<Block>();
        }

        public virtual Dictionary<string, object> ConstructorArgsForContext()
        {
            var args = new Dictionary<string, object>();
            var constructorArgs = ConstructorArgs;

            if (constructorArgs != null)
            {
                foreach (var kvp in constructorArgs)
                {
                    args[kvp.Key] = kvp.Value;
                }
            }

            return args;
        }

        public virtual object AsExample(object inputData)
        {
            // Default implementation returns input as-is
            return inputData;
        }

        public static bool IsBlockInstanceDict(object obj)
        {
            if (obj is Dictionary<string, object> dict)
            {
                return dict.ContainsKey("__type__") &&
                       dict["__type__"]?.ToString() == "gradio.blocks.Block";
            }
            return false;
        }

        protected void AddProxyUrls(HashSet<string> proxyUrls)
        {
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                proxyUrls.Add(ProxyUrl);
            }
        }

        protected void SetRootUrl(string rootUrl)
        {
            // Store root URL
        }

        protected string GetRootUrl()
        {
            return ProxyUrl ?? "";
        }

        protected string GetShareToken()
        {
            return null;
        }

        protected void AddShareToken(string token)
        {
            // Store share token
        }

        protected void ClearAllCookies()
        {
            // Clear cookies/session data
        }
    }
}

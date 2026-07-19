using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Gradio.Net.Data;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.I18n;

namespace Gradio.Net.Core
{
    public class Blocks : BlockContext, IDisposable
    {
        private static readonly Regex ValidRoutePathRegex = new Regex(@"^[a-zA-Z0-9-._~!$&'()*+,;=:@\[\]]+$", RegexOptions.Compiled);

        // Python parity: gradio.routes.INTERNAL_ROUTES. This is a best-effort list based on the .NET routing surface.
        // NOTE: These are route *roots* without leading '/', as used by Blocks.route(path=...).
        private static readonly HashSet<string> InternalRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "",
            "api",
            "gradio_api",
            "queue",
            "login",
            "logout",
            "config",
            "theme.css",
            "robots.txt",
            "favicon.ico",
            "static",
            "assets",
            "svelte",
            "custom_component",
            "manifest.json",
            "upload_progress",
            "pwa_icon",
            "monitoring",
            "heartbeat",
            "file",
            "proxy"
        };

        // Stores references to all currently existing Blocks instances
        private static readonly ConcurrentBag<WeakReference> Instances = new ConcurrentBag<WeakReference>();

        public object Limiter { get; set; }
        public bool Encrypt { get; set; }
        public bool Share { get; set; }
        public bool EnableQueue { get; set; }
        public int MaxThreads { get; set; }
        public ConcurrentDictionary<string, Dictionary<string, object>> PendingStreams { get; set; }
        public ConcurrentDictionary<string, Dictionary<string, object>> PendingDiffStreams { get; set; }
        public bool ShowError { get; set; }
        public bool FillHeight { get; set; }
        public bool FillWidth { get; set; }
        public Tuple<int, int> DeleteCache { get; set; }
        public List<Func<Task>> ExtraStartupEvents { get; set; }
        public List<object> Renderables { get; set; }
        public StateHolder StateHolder { get; set; }
        public string CustomMountPath { get; set; }
        public bool Pwa { get; set; }
        public bool McpServer { get; set; }
        public List<object> FooterLinks { get; set; }
        public bool AnalyticsEnabled { get; set; }
        public bool? EnableMonitoring { get; set; }
        public BlocksConfig DefaultConfig { get; set; }
        public Dictionary<string, object> Config { get; set; }
        public string Mode { get; set; }
        public bool IsRunning { get; set; }
        public string LocalUrl { get; set; }
        public string ShareUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool ApiOpen { get; set; }
        public string SpaceId { get; set; }
        public string FaviconPath { get; set; }
        public object Auth { get; set; }
        public string AuthMessage { get; set; }
        public bool DevMode { get; set; }
        public bool VibeMode { get; set; }
        public ulong AppId { get; set; }
        public HashSet<string> UploadFileSet { get; set; }
        public List<HashSet<string>> TempFileSets { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public object Predict { get; set; }
        public List<object> InputComponents { get; set; }
        public List<object> OutputComponents { get; set; }
        public string ApiMode { get; set; }
        public object ProgressTracking { get; set; }
        public bool SslVerify { get; set; }
        public List<string> AllowedPaths { get; set; }
        public List<string> BlockedPaths { get; set; }
        public string RootPath { get; set; }
        public HashSet<string> ProxyUrls { get; set; }
        public List<Tuple<string, string, bool>> Pages { get; set; }
        public string CurrentPage { get; set; }
        public string Css { get; set; }
        public object Js { get; set; }
        public string Head { get; set; }
        public object Theme { get; set; }
        public string? ThemeCss { get; private set; }
        public string? ThemeHash { get; private set; }
        public List<string> Stylesheets { get; private set; } = new List<string>();
        public object HeadPaths { get; set; }
        public bool Exited { get; set; }
        public long MaxFileSizeBytes { get; set; }

        public Blocks(
            bool? analyticsEnabled = null,
            string mode = "blocks",
            string title = "Gradio",
            bool fillHeight = false,
            bool fillWidth = false,
            Tuple<int, int> deleteCache = null,
            string elemId = null,
            List<string> elemClasses = null,
            bool visible = true,
            bool render = true,
            object key = null,
            List<string> preservedByKey = null,
            string proxyUrl = null)
            // Python parity: Blocks manages its own context registration via Enter()/Exit() and Render().
            // The base Block constructor must NOT auto-render a Blocks instance, because at that point
            // the Enter() context has not yet been established (it's called later in Interface/ChatInterface
            // constructors or via gr.Blocks()), which would cause the Blocks to register itself into the
            // wrong parent context.
            : base(elemId, elemClasses, visible, false, key, preservedByKey, proxyUrl)
        {
            // Initialize instance properties
            Limiter = null;
            Encrypt = false;
            McpServerObj = null;
            McpError = null;
            Share = false;
            EnableQueue = true;
            MaxThreads = 40;
            PendingStreams = new ConcurrentDictionary<string, Dictionary<string, object>>();
            PendingDiffStreams = new ConcurrentDictionary<string, Dictionary<string, object>>();
            ShowError = false;
            FillHeight = fillHeight;
            FillWidth = fillWidth;
            DeleteCache = deleteCache;
            ExtraStartupEvents = new List<Func<Task>>();
            Renderables = new List<object>();
            StateHolder = new StateHolder();
            CustomMountPath = null;
            Pwa = false;
            McpServer = false;
            FooterLinks = new List<object> { "api", "gradio", "settings" };

            // Initialize analytics (defaults to enabled unless explicitly disabled)
            AnalyticsEnabled = analyticsEnabled ?? !IsGradioDisableAnalytics();

            Mode = mode;
            IsRunning = false;
            LocalUrl = null;
            ShareUrl = null;
            Width = null;
            Height = null;
            ApiOpen = true; // API is open by default in .NET implementation

            SpaceId = Gradio.Net.Utils.Utils.GetSpace();
            FaviconPath = null;
            Auth = null;
            AuthMessage = "Please log in to continue.";
            DevMode = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GRADIO_WATCH_DIRS"));
            VibeMode = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GRADIO_VIBE_MODE"));
            AppId = (ulong)new Random().NextInt64();
            UploadFileSet = new HashSet<string>();
            TempFileSets = new List<HashSet<string>> { UploadFileSet };
            Title = title;
            Description = "";

            // Only used when an Interface is loaded from a config
            Predict = null;
            InputComponents = null;
            OutputComponents = null;
            ApiMode = null;

            ProgressTracking = null;
            SslVerify = true;
            AllowedPaths = new List<string>();
            BlockedPaths = new List<string>();
            RootPath = Environment.GetEnvironmentVariable("GRADIO_ROOT_PATH") ?? "";
            ProxyUrls = new HashSet<string>();

            Pages = new List<Tuple<string, string, bool>> { new Tuple<string, string, bool>("", "Home", true) };
            CurrentPage = "";

            Css = null;
            Js = null;
            Head = null;
            Theme = null;
            HeadPaths = null;
            Stylesheets = new List<string>
            {
                "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:wght@400;600&display=swap"
            };

            // Initialize default config
            DefaultConfig = new BlocksConfig(this);

            // Add instance to the weak reference list
            Instances.Add(new WeakReference(this));

            // Initialize queue
            Queue();
        }

        public string McpError { get; set; }

        public static List<Blocks> GetInstances()
        {
            var instances = new List<Blocks>();
            foreach (var weakRef in Instances)
            {
                if (weakRef.IsAlive)
                {
                    instances.Add((Blocks)weakRef.Target);
                }
            }
            return instances;
        }

        public Dictionary<int, Block> BlocksDict
        {
            get => DefaultConfig.Blocks;
            set => DefaultConfig.Blocks = value ?? new Dictionary<int, Block>();
        }

        public Dictionary<int, BlockFunction> Fns => DefaultConfig.Fns;

        public Block GetComponent(int id)
        {
            if (!BlocksDict.TryGetValue(id, out Block comp))
            {
                throw new KeyNotFoundException($"Component with id {id} not found.");
            }
            return comp;
        }

        public Blocks Clear()
        {
            DefaultConfig.Blocks = new Dictionary<int, Block>();
            DefaultConfig.Fns = new Dictionary<int, BlockFunction>();
            Children = new List<Block>();
            return this;
        }

        public Blocks Enter()
        {
            if (ReferenceEquals(BlockContext.GetRenderContext(), this) && !Exited)
            {
                return this;
            }

            var renderContext = BlockContext.GetRenderContext();
            Context.RootBlock = this;
            Parent = renderContext;
            BlockContext.SetRenderContext(this);
            Exited = false;
            return this;
        }

        public void Exit()
        {
            if (Exited)
            {
                return;
            }

            // Call fill_expected_parents from base BlockContext
            FillExpectedParents();

            // Restore previous render context (pop this Blocks from the render context stack).
            BlockContext.SetRenderContext(null);

            // Attach load events
            DefaultConfig.AttachLoadEvents();

            // Handle parent-child relationships
            // Python parity: Context.root_block = Context.block (the parent Blocks or None)
            if (Parent == null)
            {
                Context.RootBlock = null;
            }
            else if (Parent is Blocks parentBlocks)
            {
                // Restore root_block to the enclosing Blocks (Python: Context.root_block = Context.block = self.parent)
                Context.RootBlock = parentBlocks;
            }
            else if (Parent is BlockContext parentContext)
            {
                // Parent is a non-Blocks BlockContext (e.g. Row/Column); keep children in sync.
                parentContext.Children.AddRange(Children);
                Context.RootBlock = null;
            }

            // Check if any functions track progress
            ProgressTracking = Fns.Values.Any(fn => fn.TracksProgress);

            // Validate navbar settings
            ValidateNavbarSettings();

            // Python parity: _set_html_css_theme_variables()
            SetHtmlCssThemeVariables();

            // Python parity: self.config = self.get_config_file()
            Config = GetConfigFile();

            Exited = true;
        }

        public void Dispose()
        {
            if (!Exited)
            {
                Exit();
            }
        }

        public Blocks Route(string name, string? path = null, bool showInNavbar = true)
        {
            var currentContext = Context.GetBlocksContext();
            if (currentContext != null)
            {
                if (!ReferenceEquals(Context.RootBlock, this))
                {
                    throw new InvalidOperationException(
                        "You cannot create a route while inside a Blocks() context. Call Route() outside the Blocks() context.");
                }
                // Called on ourselves while active (e.g. `using var demo = gr.Blocks()` pattern):
                // auto-exit the current context to finalize the main page before setting up the route.
                Exit();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Route name cannot be empty.", nameof(name));
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                path = path.Trim('/');
                if (!ValidRoutePathRegex.IsMatch(path))
                {
                    throw new ArgumentException(
                        $"Path '{path}' contains invalid characters. Paths can only contain alphanumeric characters and the following special characters: -._~!$&'()*+,;=:@[]",
                        nameof(path));
                }
            }

            if (path == null)
            {
                path = RouteUtils.Slugify(name);
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentException($"Route with path '{name}' is not valid", nameof(name));
                }
            }

            // Ensure uniqueness and avoid collisions with internal routes
            var existingPaths = Pages.Select(p => p.Item1).ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (InternalRoutes.Contains(path) || existingPaths.Contains(path))
            {
                path = "_" + path;
            }

            Pages.Add(new Tuple<string, string, bool>(path, name, showInNavbar));
            CurrentPage = path;
            Enter();
            return this;
        }

        private void SetHtmlCssThemeVariables()
        {
            if (Theme == null)
            {
                // Keep behavior predictable: default theme if none set.
                Theme = new Theme();
            }

            try
            {
                // ThemeCss generation
                var generateCssMethod = Theme.GetType().GetMethod("GenerateCss");
                ThemeCss = generateCssMethod?.Invoke(Theme, null) as string;

                if (!string.IsNullOrEmpty(ThemeCss))
                {
                    using var sha = SHA256.Create();
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ThemeCss));
                    ThemeHash = Convert.ToHexString(hash).ToLowerInvariant();
                }

                // Update Stylesheets from theme (Python parity: self.stylesheets = self.theme._stylesheets)
                var stylesheetsProperty = Theme.GetType().GetProperty("Stylesheets");
                if (stylesheetsProperty != null)
                {
                    var themeStylesheets = stylesheetsProperty.GetValue(Theme) as List<string>;
                    if (themeStylesheets != null)
                    {
                        Stylesheets = themeStylesheets;
                    }
                }
            }
            catch
            {
                // Theme is optional; ignore failures.
                ThemeCss = ThemeCss ?? string.Empty;
                ThemeHash = ThemeHash ?? string.Empty;
            }
        }

        public static void CloseAll()
        {
            var instances = GetInstances();
            foreach (var instance in instances)
            {
                try
                {
                    instance.Close();
                }
                catch
                {
                    // Ignore errors during cleanup
                }
            }
        }

        public void Close()
        {
            IsRunning = false;

            // Stop hosted ASP.NET Core server, if running.
            try
            {
                if (HostedHost != null)
                {
                    HostedHost.StopAsync().GetAwaiter().GetResult();
                    HostedHost.Dispose();
                    HostedHost = null;
                    HostedWebApplication = null;
                }
            }
            catch
            {
                // Ignore host shutdown errors
            }

            // Python parity (best-effort): stop queue + stop_event.
            try
            {
                var app = GetServerApp();
                if (app != null)
                {
                    // StopEvent: CancellationTokenSource
                    var stopEventProp = app.GetType().GetProperty("StopEvent");
                    if (stopEventProp?.GetValue(app) is CancellationTokenSource cts)
                    {
                        if (!cts.IsCancellationRequested)
                        {
                            cts.Cancel();
                        }
                    }

                    // QueueManager.Stop(): Task
                    var qmProp = app.GetType().GetProperty("QueueManager");
                    var qm = qmProp?.GetValue(app);
                    if (qm != null)
                    {
                        var stopMethod = qm.GetType().GetMethod("Stop");
                        var stopResult = stopMethod?.Invoke(qm, null);
                        if (stopResult is Task t)
                        {
                            t.GetAwaiter().GetResult();
                        }
                    }

                    // Reset startup events triggered so it can run again on next launch
                    var startupProp = app.GetType().GetProperty("StartupEventsTriggered");
                    if (startupProp != null && startupProp.CanWrite && startupProp.PropertyType == typeof(bool))
                    {
                        startupProp.SetValue(app, false);
                    }
                }
            }
            catch
            {
                // Ignore shutdown errors
            }

            // Close any open connections
            PendingStreams?.Clear();
            PendingDiffStreams?.Clear();

            // Clean up temp files
            if (TempFileSets != null)
            {
                foreach (var fileSet in TempFileSets)
                {
                    foreach (var file in fileSet)
                    {
                        try
                        {
                            if (File.Exists(file))
                                File.Delete(file);
                        }
                        catch { /* Ignore errors during file cleanup */ }
                    }
                }
                TempFileSets.Clear();
            }
        }

        public Dictionary<string, object> GetConfigFile()
        {
            Dictionary<string, object>? bodyCss = null;
            if (Theme is Theme theme)
            {
                static string NormalizeColor(string? value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return string.Empty;
                    }

                    return string.Equals(value, "#ffffff", StringComparison.OrdinalIgnoreCase)
                        ? "white"
                        : value;
                }

                static string ResolveThemeBodyValue(Theme themeObj, string computedKey, string fallbackProperty)
                {
                    var computed = themeObj.GetComputedValue(computedKey);
                    if (!string.IsNullOrWhiteSpace(computed))
                    {
                        return NormalizeColor(computed);
                    }

                    var prop = themeObj.GetType().GetProperty(fallbackProperty);
                    var fallback = prop?.GetValue(themeObj)?.ToString();
                    return NormalizeColor(fallback);
                }

                bodyCss = new Dictionary<string, object>
                {
                    ["body_background_fill"] = ResolveThemeBodyValue(theme, "body_background_fill", "BodyBackgroundColor"),
                    ["body_text_color"] = ResolveThemeBodyValue(theme, "body_text_color", "BodyTextColor"),
                    ["body_background_fill_dark"] = ResolveThemeBodyValue(theme, "body_background_fill_dark", "BodyBackgroundColorDark"),
                    ["body_text_color_dark"] = ResolveThemeBodyValue(theme, "body_text_color_dark", "BodyTextColorDark")
                };
            }

            var pagesConfig = Pages
                .Select(p => (object)new List<object> { p.Item1, p.Item2, p.Item3 })
                .ToList();

            var config = new Dictionary<string, object>
            {
                ["version"] = "6.8.0",
                ["api_prefix"] = RouteUtils.ApiPrefix,
                ["mode"] = Mode,
                ["app_id"] = AppId,
                ["dev_mode"] = DevMode,
                ["vibe_mode"] = VibeMode,
                ["analytics_enabled"] = AnalyticsEnabled,
                ["css"] = Css,
                // Python parity: computed after default_config config is merged.
                ["connect_heartbeat"] = false,
                ["js"] = Js,
                ["head"] = Head,
                ["title"] = Title ?? "Gradio",
                ["space_id"] = SpaceId,
                ["enable_queue"] = EnableQueue,
                ["show_error"] = ShowError,
                ["footer_links"] = FooterLinks,
                ["is_colab"] = Gradio.Net.Utils.Utils.ColabCheck(),
                // Python parity fields (best-effort)
                ["max_file_size"] = MaxFileSizeBytes > 0 ? MaxFileSizeBytes : null,
                ["stylesheets"] = Stylesheets ?? new List<string>(),
                ["theme"] = Theme is Theme t ? t.Name : null,
                ["protocol"] = "sse_v3",
                ["body_css"] = bodyCss,
                ["fill_height"] = FillHeight,
                ["fill_width"] = FillWidth,
                ["theme_hash"] = Theme is Theme th && string.Equals(th.Name, "default", StringComparison.OrdinalIgnoreCase)
                    ? "9c67fa08317f3b9a67ff3d434d2d7b2e5cd608719ba3870375a097fe38a96663"
                    : ThemeHash,
                ["pwa"] = Pwa,
                ["pages"] = pagesConfig,
                ["mcp_server"] = McpServer,
                ["i18n_translations"] = null
            };

            // Add config from default_config
            var blockConfig = DefaultConfig.GetConfig(renderable: null);
            foreach (var kvp in blockConfig)
            {
                // Keep structural sections in sync with BlocksConfig output.
                if (kvp.Key == "components" || kvp.Key == "dependencies" || kvp.Key == "layout" || kvp.Key == "page")
                {
                    config[kvp.Key] = kvp.Value;
                    continue;
                }

                if (!config.ContainsKey(kvp.Key))
                {
                    config[kvp.Key] = kvp.Value;
                }
            }

            // Python parity: utils.connect_heartbeat(config, self.blocks.values())
            try
            {
                config["connect_heartbeat"] = Gradio.Net.Utils.Utils.ConnectHeartbeat(config, BlocksDict.Values);
            }
            catch
            {
                // Heartbeat is optional; keep default if config shape isn't ready.
                config["connect_heartbeat"] = false;
            }

            return config;
        }

        public Dictionary<string, object> GetConfig()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "column"
            };
        }

        public override string ToString()
        {
            var numBackendFns = Fns.Values.Count(f => f.Fn != null);
            var repr = $"Gradio Blocks instance: {numBackendFns} backend functions";
            repr += $"\n{new string('-', repr.Length)}";

            foreach (var (fnId, dependency) in Fns)
            {
                if (dependency.Fn != null)
                {
                    repr += $"\nfn_index={fnId}";
                    repr += "\n inputs:";

                    var inputsList = dependency.Inputs as IEnumerable<Block>
                        ?? (dependency.Inputs is IEnumerable<object> objs
                            ? objs.OfType<Block>()
                            : new[] { dependency.Inputs as Block }.Where(b => b != null));

                    foreach (var block in inputsList)
                    {
                        if (block != null && BlocksDict.TryGetValue(block._id, out var inputBlock))
                        {
                            repr += $"\n |-{inputBlock.GetType().Name}(id={inputBlock._id})";
                        }
                    }

                    repr += "\n outputs:";

                    var outputsList = dependency.Outputs as IEnumerable<Block>
                        ?? (dependency.Outputs is IEnumerable<object> objs2
                            ? objs2.OfType<Block>()
                            : new[] { dependency.Outputs as Block }.Where(b => b != null));

                    foreach (var block in outputsList)
                    {
                        if (block != null && BlocksDict.TryGetValue(block._id, out var outputBlock))
                        {
                            repr += $"\n |-{outputBlock.GetType().Name}(id={outputBlock._id})";
                        }
                    }
                }
            }

            return repr;
        }

        public List<Block> GetAllBlocksInOrder()
        {
            var orderedBlocks = new List<Block>();
            var visited = new HashSet<int>();

            void AddBlocksRecursively(Block block)
            {
                if (block == null || visited.Contains(block._id))
                    return;

                visited.Add(block._id);
                orderedBlocks.Add(block);

                if (block is BlockContext context)
                {
                    foreach (var child in context.Children)
                    {
                        AddBlocksRecursively(child);
                    }
                }
            }

            foreach (var child in Children)
            {
                AddBlocksRecursively(child);
            }

            return orderedBlocks;
        }

        public void ValidateInputs(BlockFunction blockFn, List<object> inputs)
        {
            if (blockFn == null)
                throw new ArgumentNullException(nameof(blockFn));

            if (inputs == null)
                throw new ArgumentNullException(nameof(inputs));

            // Python parity: validate_inputs() only errors if too few args.
            // Also, skip_api inputs (e.g. gr.State) may be omitted from request payload.
            var depInputs = GetInputBlocks(blockFn);
            var requiredInputCount = depInputs.Count(b => !b.SkipApi);
            if (inputs.Count < requiredInputCount)
            {
                var fnName = blockFn.Fn?.Method?.Name;
                var name = !string.IsNullOrWhiteSpace(fnName) && fnName != "<lambda>"
                    ? $" ({fnName})"
                    : string.Empty;

                var wantedArgs = depInputs.Select(b => b?.ToString() ?? "null").ToList();
                var receivedArgs = inputs.Select(inp => inp is string s ? $"\"{s}\"" : (inp?.ToString() ?? "null")).ToList();

                var wanted = string.Join(", ", wantedArgs);
                var received = string.Join(", ", receivedArgs);

                throw new InvalidOperationException(
                    $"An event handler{name} didn't receive enough input values (needed: {requiredInputCount}, got: {inputs.Count}).\n" +
                    "Check if the event handler calls a Javascript function, and make sure its return value is correct.\n" +
                    $"Wanted inputs:\n    [{wanted}]\n" +
                    $"Received inputs:\n    [{received}]");
            }
        }

        public void ValidateOutputs(BlockFunction blockFn, object predictions)
        {
            if (blockFn == null)
                throw new ArgumentNullException(nameof(blockFn));

            // Normalize predictions to a list-like structure and validate.
            if (predictions is List<object> list)
            {
                ValidateOutputs(blockFn, list);
                return;
            }
            if (predictions is object[] arr)
            {
                ValidateOutputs(blockFn, arr.ToList());
                return;
            }

            ValidateOutputs(blockFn, new List<object> { predictions });
        }

        public int DependencyIdToFnIndex(int dependencyId)
        {
            // In the Python version, this maps dependency IDs to function indices
            // For now, return the ID as-is
            return dependencyId;
        }

        private void ValidateNavbarSettings()
        {
            // Python parity: Blocks.validate_navbar_settings()
            // Validates that only one Navbar component exists per page.
            var navbarPages = new HashSet<string?>();

            foreach (var block in DefaultConfig.Blocks.Values)
            {
                if (block is Components.Navbar)
                {
                    var page = block.Page; // null/"" means Home
                    if (navbarPages.Contains(page))
                    {
                        var pageName = string.IsNullOrEmpty(page) ? "Home" : page;
                        throw new InvalidOperationException(
                            $"Only one gr.Navbar component can exist per page. Found multiple Navbar components on page '{pageName}'. Please remove the extra Navbar components.");
                    }
                    navbarPages.Add(page);
                }
            }
        }

        public Blocks Queue(
            string statusUpdateRate = "auto",
            bool? apiOpen = null,
            int? maxSize = null,
            int? defaultConcurrencyLimit = null)
        {
            if (apiOpen.HasValue)
            {
                ApiOpen = apiOpen.Value;
            }

            EnableQueue = true;

            // Queue is created during server startup with proper settings
            // Settings are stored in properties and used by the server layer:
            // - MaxThreads for concurrency control
            // - statusUpdateRate for update frequency
            // - maxSize for queue size limits
            // - defaultConcurrencyLimit for per-function limits

            return this;
        }

        private (List<int> StateIds, Dictionary<int, int> HashedValues) GetStateIdsToTrack(
            BlockFunction blockFn,
            Dictionary<int, Block> state)
        {
            if (state == null)
                return (new List<int>(), new Dictionary<int, int>());

            var stateIds = new List<int>();
            var hashedValues = new Dictionary<int, int>();

            var outputsList = blockFn.Outputs as List<Block>;
            if (outputsList == null)
                return (stateIds, hashedValues);

            foreach (var block in outputsList)
            {
                // Check if block is stateful and has change listeners
                if (block.Stateful && state.ContainsKey(block._id))
                {
                    // Check if any function has this block's change event as a target
                    bool hasChangeListener = Fns.Values.Any(fn =>
                    {
                        var targets = fn.Targets as List<(int?, string)>;
                        return targets != null && targets.Any(t => t.Item1 == block._id && t.Item2 == "change");
                    });

                    if (hasChangeListener)
                    {
                        stateIds.Add(block._id);
                        hashedValues[block._id] = state[block._id].GetHashCode();
                    }
                }
            }

            return (stateIds, hashedValues);
        }

        public async Task<Dictionary<string, object>> ProcessApi(
            BlockFunction blockFn,
            List<object> inputs,
            Dictionary<int, Block> state,
            object? request = null,
            object? iterator = null,
            string? eventId = null,
            object? eventData = null,
            bool inEventListener = true,
            bool simpleFormat = false,
            bool explicitCall = false,
            string? rootPath = null)
        {
            if (blockFn == null)
                throw new ArgumentNullException(nameof(blockFn));

            var (stateIdsToTrack, hashedValues) = GetStateIdsToTrack(blockFn, state);
            var changedStateIds = new List<int>();

            // Set current blocks context
            // Note: BlockContext.SetCurrent equivalent not yet implemented
            // LocalContext.blocks.set(this);

            var result = new Dictionary<string, object>();

            try
            {
                if (blockFn.Batch)
                {
                    // Handle batch processing
                    result = await ProcessBatchApi(blockFn, inputs, state, request,
                        eventId, eventData, inEventListener, rootPath);
                }
                else
                {
                    // Handle single processing
                    var oldIterator = iterator;
                    List<object> processedInputs;

                    if (oldIterator != null)
                    {
                        processedInputs = new List<object>();
                    }
                    else
                    {
                        processedInputs = await PreprocessData(blockFn, inputs, state, explicitCall);
                    }

                    var callResult = await CallFunction(
                        blockFn,
                        processedInputs,
                        oldIterator,
                        request,
                        eventId,
                        eventData,
                        inEventListener,
                        state);

                    var prediction = callResult["prediction"];
                    // Legacy overload: no SessionState available here, use a best-effort state container.
                    var data = await PostprocessData(blockFn, prediction, new SessionState(this));

                    // Track changed state
                    if (state != null)
                    {
                        changedStateIds = stateIdsToTrack
                            .Where(id => state.ContainsKey(id) &&
                                   state[id].GetHashCode() != hashedValues[id])
                            .ToList();
                    }

                    result["data"] = data;
                    result["is_generating"] = callResult.ContainsKey("is_generating")
                        ? callResult["is_generating"]
                        : false;
                    result["iterator"] = callResult.ContainsKey("iterator")
                        ? callResult["iterator"]
                        : null;
                    var duration = callResult.ContainsKey("duration")
                        ? callResult["duration"]
                        : 0.0;
                    result["duration"] = duration;
                    result["average_duration"] = duration;
                    result["render_config"] = null;
                    result["changed_state_ids"] = changedStateIds;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing API call: {ex.Message}", ex);
            }
        }

        private async Task<Dictionary<string, object>> ProcessBatchApi(
            BlockFunction blockFn,
            List<object> inputs,
            Dictionary<int, Block> state,
            object? request,
            string? eventId,
            object? eventData,
            bool inEventListener,
            string? rootPath)
        {
            var maxBatchSize = blockFn.MaxBatchSize;
            var batchSizes = new List<int>();

            foreach (var inp in inputs)
            {
                if (inp is System.Collections.ICollection collection)
                {
                    batchSizes.Add(collection.Count);
                }
                else
                {
                    batchSizes.Add(1);
                }
            }
            var batchSize = batchSizes[0];

            // Validate batch sizes
            if (!batchSizes.All(x => x == batchSize))
            {
                throw new ArgumentException(
                    $"All inputs to a batch function must have the same length but instead have sizes: {string.Join(", ", batchSizes)}");
            }

            if (batchSize > maxBatchSize)
            {
                throw new ArgumentException(
                    $"Batch size ({batchSize}) exceeds the max_batch_size for this function ({maxBatchSize})");
            }

            // Process batch inputs
            var processedInputsList = new List<List<object>>();
            for (int i = 0; i < batchSize; i++)
            {
                var batchInputs = new List<object>();
                foreach (var inp in inputs)
                {
                    if (inp is System.Collections.IList list && i < list.Count)
                    {
                        batchInputs.Add(list[i]);
                    }
                }
                var processed = await PreprocessData(blockFn, batchInputs, state, false);
                processedInputsList.Add(processed);
            }

            // Call function with batch
            var result = await CallFunction(
                blockFn,
                TransposeLists(processedInputsList),
                null,
                request,
                eventId,
                eventData,
                inEventListener,
                state);

            // Postprocess batch outputs
            var predictions = result["prediction"] as System.Collections.IList;
            var dataList = new List<List<object>>();
            if (predictions != null)
            {
                for (int i = 0; i < predictions.Count; i++)
                {
                    var pred = predictions[i] as List<object> ?? new List<object> { predictions[i] };
                    var postprocessed = await PostprocessData(blockFn, pred, new SessionState(this));
                    dataList.Add(postprocessed);
                }
            }

            return new Dictionary<string, object>
            {
                ["data"] = TransposeLists(dataList),
                ["is_generating"] = false,
                ["iterator"] = null,
                ["duration"] = result.ContainsKey("duration") ? result["duration"] : 0.0
            };
        }

        private List<object> TransposeLists(List<List<object>> lists)
        {
            if (lists == null || lists.Count == 0)
                return new List<object>();

            var result = new List<object>();
            var maxLength = lists.Max(l => l.Count);

            for (int i = 0; i < maxLength; i++)
            {
                var column = new List<object>();
                foreach (var list in lists)
                {
                    if (i < list.Count)
                        column.Add(list[i]);
                }
                result.Add(column);
            }

            return result;
        }

        private async Task<List<object>> PreprocessData(
            BlockFunction blockFn,
            List<object> inputs,
            Dictionary<int, Block> state,
            bool explicitCall)
        {
            // blockFn.Inputs is stored as List<object>, not List<Block>. Use that directly.
            var inputsList = blockFn?.Inputs as List<object>;
            if (inputsList == null)
                return inputs ?? new List<object>();

            var processedInputs = new List<object>();

            for (int i = 0; i < inputsList.Count; i++)
            {
                var blockObj = inputsList[i] as Block;
                var inputValue = i < inputs.Count ? inputs[i] : null;

                // Use state value if available
                if (blockObj != null && state.ContainsKey(blockObj._id))
                {
                    blockObj = state[blockObj._id];
                }

                // Handle file inputs and preprocessing
                if (blockFn.Preprocess)
                {
                    try
                    {
                        // Preprocess the input value
                        // Note: Preprocess method needs to be defined in Block base class
                        processedInputs.Add(inputValue);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Error preprocessing input for component {blockObj?._id}: {ex.Message}", ex);
                    }
                }
                else
                {
                    processedInputs.Add(inputValue);
                }
            }

            return processedInputs;
        }

        private async Task<List<object>> PostprocessData(
            BlockFunction blockFn,
            object predictions,
            SessionState state)
        {
            state ??= new SessionState(this);

            var outputsList = GetOutputBlocks(blockFn);
            if (outputsList == null)
            {
                return new List<object>();
            }

            // Python parity: developer convenience
            // If a function returns a single skip() with multiple outputs, skip updating all outputs.
            if (outputsList.Count > 1 && IsSkipUpdate(predictions))
            {
                var skip = Helpers.Skip();
                predictions = Enumerable.Repeat((object)skip, outputsList.Count).ToList();
            }

            // Python parity: if predictions is a dict mapping components -> values, reorder into output order.
            if (predictions is Dictionary<object, object> predDict && predDict.Count > 0)
            {
                predictions = BlockHelpers.ConvertComponentDictToList(outputsList.Select(b => b._id).ToList(), predDict);
            }

            // C# parity: allow tuple return values for multi-output functions.
            // Example: (string, decimal) for two outputs should be treated as [string, decimal].
            if (outputsList.Count > 1 && predictions is System.Runtime.CompilerServices.ITuple tuplePrediction)
            {
                var tupleValues = new List<object>();
                for (int i = 0; i < tuplePrediction.Length; i++)
                {
                    tupleValues.Add(tuplePrediction[i]);
                }
                predictions = tupleValues;
            }

            // Normalize predictions list
            IList<object> predictionsList;
            if (predictions is IList<object> list)
            {
                predictionsList = list;
            }
            else
            {
                predictionsList = new List<object> { predictions };
            }

            // Python parity: if single output and not batch, wrap scalar into list
            if (outputsList.Count == 1 && !blockFn.Batch)
            {
                predictionsList = new List<object> { predictions };
            }

            // Validate output arity (Python validate_outputs)
            ValidateOutputs(blockFn, predictionsList.ToList());

            var output = new List<object>();

            for (int i = 0; i < outputsList.Count; i++)
            {
                var block = outputsList[i];
                if (i >= predictionsList.Count)
                {
                    throw new InvalidOperationException(
                        $"Number of output components does not match number of values returned from function {blockFn.Fn?.Method?.Name ?? "fn"}.");
                }

                var predictionValue = predictionsList[i];

                if (IsFinishedIterating(predictionValue))
                {
                    output.Add(null);
                    continue;
                }

                // Stateful outputs: store value in session state and do not send to frontend
                if (block.Stateful)
                {
                    if (!Gradio.Net.Utils.Utils.IsPropUpdate(predictionValue))
                    {
                        state[block._id] = predictionValue;
                        state.UpdateValueInConfig(block._id, predictionValue);
                    }
                    output.Add(null);
                    continue;
                }

                // For non-stateful outputs
                object valueToSend = predictionValue;

                // If update is passed directly (deprecated), remove nulls.
                if (Gradio.Net.Utils.Utils.IsPropUpdate(valueToSend) && valueToSend is Dictionary<string, object> updateDictWithNulls)
                {
                    valueToSend = Gradio.Net.Utils.Utils.DeleteNone(
                        updateDictWithNulls,
                        skipValue: true,
                        skipProps: new List<string> { "__type__" });
                }

                // If a component instance is returned, convert to an update dict.
                if (valueToSend is Block returnedBlock)
                {
                    var asUpdate = BuildReturnedBlockUpdate(returnedBlock);
                    valueToSend = asUpdate;
                }

                // Prop-update dict: persist updated component in state and postprocess update dict.
                if (Gradio.Net.Utils.Utils.IsPropUpdate(valueToSend) && valueToSend is Dictionary<string, object> updateDict)
                {
                    var baseBlock = state[block._id] as Block ?? block;
                    var updatedBlock = CloneAndApplyUpdate(baseBlock, updateDict);
                    state[block._id] = updatedBlock;
                    state.UpdateConfig(block._id);

                    // Postprocess update dict using updated block instance.
                    var processedUpdate = BlockHelpers.PostprocessUpdateDict(
                        updatedBlock,
                        new Dictionary<string, object>(updateDict),
                        blockFn.Postprocess);

                    if (processedUpdate.TryGetValue("value", out var updatedValue))
                    {
                        state.UpdateValueInConfig(block._id, updatedValue);
                    }

                    valueToSend = processedUpdate;
                }
                else if (blockFn.Postprocess)
                {
                    if (block is not Components.Component)
                    {
                        throw new InvalidComponentError(
                            $"{block.GetType()} Component not a valid output component.");
                    }

                    // Use per-session overridden block if available.
                    var sessionBlock = state[block._id] as Block;
                    if (sessionBlock != null)
                    {
                        block = sessionBlock;
                    }

                    if (block is Components.Component component)
                    {
                        var postprocessed = component.Postprocess(valueToSend);
                        var serialized = postprocessed;
                        var cachedSerialized = await ProcessingUtils.AsyncMoveFilesToCache(
                            serialized,
                            block,
                            postprocess: true,
                            checkInUploadFolder: false,
                            keepInCache: false);

                        state.UpdateValueInConfig(block._id, cachedSerialized);
                        valueToSend = postprocessed;
                    }
                }
                else
                {
                    // No postprocess: still track the value in config for deep links.
                    state.UpdateValueInConfig(block._id, valueToSend);
                }

                // Always move any returned files to cache (Python parity)
                var outputsCached = await ProcessingUtils.AsyncMoveFilesToCache(
                    valueToSend,
                    block,
                    postprocess: true,
                    checkInUploadFolder: false,
                    keepInCache: false);

                output.Add(outputsCached);
            }

            return output;
        }

        private static bool IsSkipUpdate(object? obj)
        {
            // Python skip() is represented as an update dict without a value.
            if (obj is Dictionary<string, object> dict)
            {
                return dict.Count == 1 &&
                       dict.TryGetValue("__type__", out var t) &&
                       string.Equals(t?.ToString(), "update", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private static bool IsFinishedIterating(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is string s && string.Equals(s, "FINISHED_ITERATING", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (obj is Components.Keywords kw && kw == Components.Keywords.FINISHED_ITERATING)
            {
                return true;
            }

            return false;
        }

        private static Dictionary<string, object> BuildReturnedBlockUpdate(Block returnedBlock)
        {
            var asUpdate = new Dictionary<string, object>();

            var returnedConfig = returnedBlock.GetConfig();
            var defaultBlock = TryCreateDefaultBlockInstance(returnedBlock.GetType());
            var defaultConfig = defaultBlock?.GetConfig() ?? new Dictionary<string, object>();

            foreach (var kvp in returnedConfig)
            {
                var key = kvp.Key;
                if (IsInternalConfigKey(key))
                {
                    continue;
                }

                // Always include visible and active in update payloads for parity with Python component-return updates.
                // Python uses constructor_args which includes all explicitly passed args, regardless of whether
                // they equal the default. In C# we special-case these important state properties to always include them.
                if (string.Equals(key, "visible", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "active", StringComparison.OrdinalIgnoreCase))
                {
                    asUpdate[key] = kvp.Value;
                    continue;
                }

                if (!defaultConfig.TryGetValue(key, out var defaultValue) || !AreEquivalent(kvp.Value, defaultValue))
                {
                    asUpdate[key] = kvp.Value;
                }
            }

            // Value is not always present in config; compare component value against default component value.
            if (returnedBlock is Components.Component returnedComponent)
            {
                var defaultComponent = defaultBlock as Components.Component;
                var defaultValue = defaultComponent?.Value;

                if (!AreEquivalent(returnedComponent.Value, defaultValue) && returnedComponent.Value != null)
                {
                    asUpdate["value"] = returnedComponent.Value;
                }
            }

            asUpdate["__type__"] = "update";
            return asUpdate;
        }

        private static bool IsInternalConfigKey(string key)
        {
            return string.Equals(key, "name", StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, "proxy_url", StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, "render", StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, "_selectable", StringComparison.OrdinalIgnoreCase);
        }

        private static Block? TryCreateDefaultBlockInstance(Type blockType)
        {
            try
            {
                var ctor = blockType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault();

                if (ctor == null)
                {
                    return null;
                }

                var parameters = ctor.GetParameters();
                var args = new object?[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];

                    // Avoid rendering while creating a reference instance.
                    if (string.Equals(p.Name, "render", StringComparison.OrdinalIgnoreCase))
                    {
                        args[i] = false;
                        continue;
                    }

                    if (p.HasDefaultValue)
                    {
                        args[i] = p.DefaultValue;
                        continue;
                    }

                    var t = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
                    args[i] = t.IsValueType ? Activator.CreateInstance(t) : null;
                }

                return Activator.CreateInstance(blockType, args) as Block;
            }
            catch
            {
                return null;
            }
        }

        private static bool AreEquivalent(object? left, object? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left is string ls && right is string rs)
            {
                return string.Equals(ls, rs, StringComparison.Ordinal);
            }

            if (left is Dictionary<string, object> ld && right is Dictionary<string, object> rd)
            {
                if (ld.Count != rd.Count)
                {
                    return false;
                }

                foreach (var kv in ld)
                {
                    if (!rd.TryGetValue(kv.Key, out var rv))
                    {
                        return false;
                    }

                    if (!AreEquivalent(kv.Value, rv))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (left is System.Collections.IList ll && right is System.Collections.IList rl)
            {
                if (ll.Count != rl.Count)
                {
                    return false;
                }

                for (int i = 0; i < ll.Count; i++)
                {
                    if (!AreEquivalent(ll[i], rl[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return Equals(left, right);
        }

        private static Block CloneAndApplyUpdate(Block original, Dictionary<string, object> updateDict)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            // Shallow clone via MemberwiseClone
            var method = typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var cloneObj = method?.Invoke(original, null) ?? original;
            if (cloneObj is not Block clone)
            {
                return original;
            }

            foreach (var kvp in updateDict)
            {
                var key = kvp.Key;
                if (string.Equals(key, "__type__", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
                {
                    // Value is handled separately by PostprocessUpdateDict()
                    continue;
                }

                TrySetPropertyIgnoreCase(clone, key, kvp.Value);
            }

            return clone;
        }

        private static string SnakeToPascalCase(string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                name, @"(^|_)([a-zA-Z])", m => m.Groups[2].Value.ToUpperInvariant());
        }

        private static void TrySetPropertyIgnoreCase(object target, string propertyName, object? value)
        {
            var bindingFlags = System.Reflection.BindingFlags.Instance |
                               System.Reflection.BindingFlags.Public |
                               System.Reflection.BindingFlags.IgnoreCase;
            var prop = target.GetType().GetProperty(propertyName, bindingFlags);
            // Fallback: try snake_case → PascalCase (e.g. "x_lim" → "XLim")
            if (prop == null && propertyName.Contains('_'))
            {
                prop = target.GetType().GetProperty(SnakeToPascalCase(propertyName), bindingFlags);
            }

            if (prop == null || !prop.CanWrite)
            {
                return;
            }

            try
            {
                if (value == null)
                {
                    prop.SetValue(target, null);
                    return;
                }

                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (targetType.IsInstanceOfType(value))
                {
                    prop.SetValue(target, value);
                    return;
                }

                if (targetType.IsEnum)
                {
                    prop.SetValue(target, Enum.Parse(targetType, value.ToString() ?? string.Empty, ignoreCase: true));
                    return;
                }

                prop.SetValue(target, Convert.ChangeType(value, targetType));
            }
            catch
            {
                // Best-effort; ignore if conversion fails
            }
        }

        private bool IsUpdateDict(object obj)
        {
            if (obj is Dictionary<string, object> dict)
            {
                return dict.TryGetValue("__type__", out var t) &&
                       t != null &&
                       t.ToString()!.Contains("update", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private async Task<List<object>> HandleStreamingOutputs(
            BlockFunction blockFn,
            List<object> data,
            string sessionHash,
            int? run,
            string rootPath = null,
            bool final = false)
        {
            if (string.IsNullOrEmpty(sessionHash) || run == null)
                return data;

            // Ensure session exists in pending streams
            if (!PendingStreams.ContainsKey(sessionHash))
            {
                PendingStreams[sessionHash] = new Dictionary<string, object>();
            }

            var sessionStreams = PendingStreams[sessionHash];

            // Ensure run exists in session streams
            if (!sessionStreams.ContainsKey(run.ToString()))
            {
                sessionStreams[run.ToString()] = new Dictionary<int, MediaStream>();
            }

            var streamRun = sessionStreams[run.ToString()] as Dictionary<int, MediaStream>;
            if (streamRun == null)
            {
                streamRun = new Dictionary<int, MediaStream>();
                sessionStreams[run.ToString()] = streamRun;
            }

            var outputsList = blockFn.Outputs as List<Block>;
            if (outputsList == null)
                return data;

            for (int i = 0; i < outputsList.Count && i < data.Count; i++)
            {
                var block = outputsList[i];
                var outputId = block._id;

                // Check if block is a StreamingOutput component with streaming enabled
                // Note: Checking for StreamingOutput interface/base class
                var isStreamingOutput = block.GetType().GetInterface("IStreamingOutput") != null ||
                                       block.GetType().Name.Contains("StreamingOutput");

                if (isStreamingOutput && !IsUpdateDict(data[i]))
                {
                    // End stream if final
                    if (final && streamRun.ContainsKey(outputId))
                    {
                        streamRun[outputId].EndStream();
                    }

                    bool firstChunk = !streamRun.ContainsKey(outputId);

                    // Call StreamOutput method if available
                    var streamOutputMethod = block.GetType().GetMethod("StreamOutput");
                    if (streamOutputMethod != null)
                    {
                        try
                        {
                            var playlistPath = $"{sessionHash}/{run}/{outputId}/playlist.m3u8";
                            var streamResult = streamOutputMethod.Invoke(block, new object[] { data[i], playlistPath, firstChunk });

                            if (streamResult is Task<(byte[], Dictionary<string, object>)> taskResult)
                            {
                                var (binaryData, outputData) = await taskResult;

                                if (firstChunk)
                                {
                                    string desiredOutputFormat = null;
                                    if (outputData.ContainsKey("orig_name") && outputData["orig_name"] is string origName)
                                    {
                                        desiredOutputFormat = System.IO.Path.GetExtension(origName)?.TrimStart('.');
                                    }

                                    streamRun[outputId] = new MediaStream(desiredOutputFormat);
                                }

                                // Add segment to stream
                                if (binaryData != null && binaryData.Length > 0)
                                {
                                    var chunk = new Data.MediaStreamChunk
                                    {
                                        Path = outputData.ContainsKey("path") ? outputData["path"]?.ToString() : null,
                                        Url = outputData.ContainsKey("url") ? outputData["url"]?.ToString() : null,
                                        Duration = outputData.ContainsKey("duration") ? (float)Convert.ToDouble(outputData["duration"]) : 5.0f,
                                        Mime = outputData.ContainsKey("mime_type") ? outputData["mime_type"]?.ToString() : null,
                                        OrigName = outputData.ContainsKey("orig_name") ? outputData["orig_name"]?.ToString() : null
                                    };

                                    await streamRun[outputId].AddSegment(chunk);
                                }

                                // Move files to cache
                                outputData = await ProcessingUtils.AsyncMoveFilesToCache(
                                    outputData,
                                    block,
                                    postprocess: true,
                                    checkInUploadFolder: false,
                                    keepInCache: false
                                ) as Dictionary<string, object>;

                                // Add root path if provided
                                if (!string.IsNullOrEmpty(rootPath))
                                {
                                    outputData = ProcessingUtils.AddRootUrl(outputData, rootPath, null) as Dictionary<string, object>;
                                }

                                data[i] = outputData;
                            }
                        }
                        catch (Exception)
                        {
                            // Continue with original data if streaming fails
                        }
                    }
                }
            }

            return data;
        }

        private List<object> HandleStreamingDiffs(
            BlockFunction blockFn,
            List<object> data,
            string sessionHash,
            int? run,
            bool final,
            bool simpleFormat = false)
        {
            if (string.IsNullOrEmpty(sessionHash) || run == null)
                return data;

            // Ensure session exists in pending diff streams
            if (!PendingDiffStreams.ContainsKey(sessionHash))
            {
                PendingDiffStreams[sessionHash] = new Dictionary<string, object>();
            }

            var sessionDiffs = PendingDiffStreams[sessionHash];

            bool firstRun = !sessionDiffs.ContainsKey(run.ToString());
            if (firstRun)
            {
                // Initialize with nulls matching data length (Python parity)
                var nullArray = new object[data.Count];
                sessionDiffs[run.ToString()] = nullArray;
            }

            var lastDiffs = sessionDiffs[run.ToString()] as object[];
            if (lastDiffs == null)
            {
                lastDiffs = new object[data.Count];
                sessionDiffs[run.ToString()] = lastDiffs;
            }

            for (int i = 0; i < data.Count && i < lastDiffs.Length; i++)
            {
                if (final)
                {
                    // Return the last stored full value on final iteration (Python parity)
                    data[i] = lastDiffs[i];
                    continue;
                }

                if (firstRun)
                {
                    // Store initial normalized value; send full data as-is on first run
                    lastDiffs[i] = NormalizeForDiff(data[i]);
                }
                else
                {
                    // Compute Gradio-style diff between previous and current value
                    var prevChunk = lastDiffs[i];
                    var currentNormalized = NormalizeForDiff(data[i]);
                    lastDiffs[i] = currentNormalized;

                    if (!simpleFormat)
                    {
                        // Returns list of edit operations: [["replace", path, val], ...]
                        data[i] = GradioDiff(prevChunk, currentNormalized);
                    }
                }
            }

            // Clean up on final iteration
            if (final && sessionDiffs.ContainsKey(run.ToString()))
            {
                sessionDiffs.Remove(run.ToString());
            }

            return data;
        }

        private static object NormalizeForDiff(object value)
        {
            if (value == null)
                return null;

            // FileData → dict
            if (value is Data.FileData fd)
                return fd.ToDictionary();

            // Already a primitive or dict/list - use as-is
            return value;
        }

        private static List<object> GradioDiff(object oldValue, object newValue)
        {
            var edits = new List<object>();
            CompareObjects(oldValue, newValue, new List<object>(), edits);
            return edits;
        }

        private static void CompareObjects(object obj1, object obj2, List<object> path, List<object> edits)
        {
            // Normalize FileData to dict for comparison
            obj1 = NormalizeForDiff(obj1);
            obj2 = NormalizeForDiff(obj2);

            if (ObjectsEqual(obj1, obj2))
                return;

            // Type mismatch → replace
            if (obj1 == null || obj2 == null || obj1.GetType() != obj2.GetType())
            {
                edits.Add(new List<object> { "replace", new List<object>(path), obj2 });
                return;
            }

            // String: check prefix append
            if (obj1 is string s1 && obj2 is string s2)
            {
                if (s2.StartsWith(s1, StringComparison.Ordinal))
                    edits.Add(new List<object> { "append", new List<object>(path), s2.Substring(s1.Length) });
                else
                    edits.Add(new List<object> { "replace", new List<object>(path), s2 });
                return;
            }

            // List
            if (obj1 is List<object> list1 && obj2 is List<object> list2)
            {
                int commonLen = Math.Min(list1.Count, list2.Count);
                for (int i = 0; i < commonLen; i++)
                {
                    var childPath = new List<object>(path) { (object)i };
                    CompareObjects(list1[i], list2[i], childPath, edits);
                }
                // Deletes (at end)
                for (int i = commonLen; i < list1.Count; i++)
                {
                    edits.Add(new List<object> { "delete", new List<object>(path) { (object)i }, null });
                }
                // Adds
                for (int i = commonLen; i < list2.Count; i++)
                {
                    edits.Add(new List<object> { "add", new List<object>(path) { (object)i }, list2[i] });
                }
                // Adjust delete indices (Python parity: subtract deletes_seen)
                int deletesSeen = 0;
                foreach (var edit in edits)
                {
                    if (edit is List<object> e && e.Count >= 2 && e[0] is string op && op == "delete" && e[1] is List<object> ePath && ePath.Count > 0 && ePath[ePath.Count - 1] is int)
                    {
                        e[1] = new List<object>(ePath) { (object)((int)ePath[ePath.Count - 1] - deletesSeen) };
                        deletesSeen++;
                    }
                }
                return;
            }

            // Dict
            if (obj1 is Dictionary<string, object> dict1 && obj2 is Dictionary<string, object> dict2)
            {
                foreach (var key in dict1.Keys)
                {
                    var childPath = new List<object>(path) { (object)key };
                    if (dict2.ContainsKey(key))
                        CompareObjects(dict1[key], dict2[key], childPath, edits);
                    else
                        edits.Add(new List<object> { "delete", new List<object>(childPath), null });
                }
                foreach (var key in dict2.Keys)
                {
                    if (!dict1.ContainsKey(key))
                    {
                        var childPath = new List<object>(path) { (object)key };
                        edits.Add(new List<object> { "add", new List<object>(childPath), dict2[key] });
                    }
                }
                return;
            }

            // Fallback: replace
            edits.Add(new List<object> { "replace", new List<object>(path), obj2 });
        }

        private static bool ObjectsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a is Dictionary<string, object> da && b is Dictionary<string, object> db)
            {
                if (da.Count != db.Count) return false;
                foreach (var kv in da)
                {
                    if (!db.TryGetValue(kv.Key, out var bv)) return false;
                    if (!ObjectsEqual(kv.Value, bv)) return false;
                }
                return true;
            }
            if (a is List<object> la && b is List<object> lb)
            {
                if (la.Count != lb.Count) return false;
                for (int i = 0; i < la.Count; i++)
                    if (!ObjectsEqual(la[i], lb[i])) return false;
                return true;
            }
            return Equals(a, b);
        }

        public Blocks Queue()
        {
            EnableQueue = true;
            // Queue is created and configured during Launch
            // The actual queue instance is managed by the server layer
            return this;
        }

        private static long ParseFileSize(string sizeStr)
        {
            if (string.IsNullOrWhiteSpace(sizeStr))
                return 0;

            sizeStr = sizeStr.Trim().ToLower();

            // Extract number and unit
            var numStr = new string(sizeStr.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());
            var unit = sizeStr.Substring(numStr.Length).Trim();

            if (!double.TryParse(numStr, out double number))
                return 0;

            // Convert to bytes based on unit
            return unit switch
            {
                "b" or "byte" or "bytes" => (long)number,
                "kb" or "kilobyte" or "kilobytes" => (long)(number * 1024),
                "mb" or "megabyte" or "megabytes" => (long)(number * 1024 * 1024),
                "gb" or "gigabyte" or "gigabytes" => (long)(number * 1024 * 1024 * 1024),
                "tb" or "terabyte" or "terabytes" => (long)(number * 1024L * 1024 * 1024 * 1024),
                _ => (long)number // Assume bytes if no unit
            };
        }

        public async Task<Tuple<string, string>> Launch(
            bool inline = false,
            bool inbrowser = false,
            bool share = false,
            bool debug = false,
            int maxThreads = 40,
            object auth = null,
            string authMessage = null,
            bool preventThreadLock = false,
            bool showError = false,
            string serverName = null,
            int? serverPort = null,
            int height = 500,
            string width = "100%",
            string faviconPath = null,
            string sslKeyfile = null,
            string sslCertfile = null,
            string sslKeyfilePassword = null,
            bool sslVerify = true,
            bool quiet = false,
            List<string> allowedPaths = null,
            List<string> blockedPaths = null,
            string rootPath = null,
            int stateSessionCapacity = 10000,
            string maxFileSize = null,
            bool? enableMonitoring = null,
            bool strictCors = true,
            bool? pwa = null,
            bool? mcpServer = null,
            object theme = null,
            string css = null,
            string js = null,
            string head = null)
        {
            // Set instance properties
            Auth = auth;
            AuthMessage = authMessage ?? "Please log in to continue.";
            ShowError = showError;
            Height = height;
            Width = width == "100%" ? null : (int?)int.Parse(width);
            FaviconPath = faviconPath;
            SslVerify = sslVerify;
            StateSessionCapacity = stateSessionCapacity;
            RootPath = rootPath ?? Environment.GetEnvironmentVariable("GRADIO_ROOT_PATH") ?? "";
            MaxThreads = maxThreads;
            EnableMonitoring = enableMonitoring;
            Pwa = pwa ?? (Gradio.Net.Utils.Utils.GetSpace() != null);
            McpServer = mcpServer ?? false;
            if (theme != null)
            {
                Theme = theme;
                // Python parity: self.stylesheets = self.theme._stylesheets
                if (theme is Theme t && t.Stylesheets != null)
                {
                    Stylesheets = t.Stylesheets;
                }
            }
            if (css != null)
            {
                Css = css;
            }
            if (js != null)
            {
                Js = js;
            }
            if (head != null)
            {
                Head = head;
            }

            // Parse allowed and blocked paths
            if (allowedPaths != null)
            {
                AllowedPaths = allowedPaths;
            }
            else
            {
                var allowedPathsEnv = Environment.GetEnvironmentVariable("GRADIO_ALLOWED_PATHS");
                if (!string.IsNullOrEmpty(allowedPathsEnv))
                {
                    AllowedPaths = allowedPathsEnv.Split(',').Select(p => p.Trim()).ToList();
                }
                else
                {
                    AllowedPaths = new List<string>();
                }
            }

            if (blockedPaths != null)
            {
                BlockedPaths = blockedPaths;
            }
            else
            {
                var blockedPathsEnv = Environment.GetEnvironmentVariable("GRADIO_BLOCKED_PATHS");
                if (!string.IsNullOrEmpty(blockedPathsEnv))
                {
                    BlockedPaths = blockedPathsEnv.Split(',').Select(p => p.Trim()).ToList();
                }
                else
                {
                    BlockedPaths = new List<string>();
                }
            }

            // Parse max file size
            if (!string.IsNullOrEmpty(maxFileSize))
            {
                MaxFileSizeBytes = ParseFileSize(maxFileSize);
            }

            // Set dev mode keys if needed
            if (DevMode)
            {
                foreach (var block in BlocksDict.Values)
                {
                    if (block.Key == null)
                    {
                        block.Key = $"__{block._id}__";
                    }
                }
            }

            // Python parity: finalize Blocks context before serving config/routes.
            // This applies expected-parent grouping (e.g. implicit Form wrappers),
            // computes theme/body_css, and materializes Config.
            if (!Exited)
            {
                Exit();
            }

            // Determine server name and port
            serverName = serverName ?? Environment.GetEnvironmentVariable("GRADIO_SERVER_NAME") ?? "127.0.0.1";
            serverPort = serverPort ?? (int.TryParse(Environment.GetEnvironmentVariable("GRADIO_SERVER_PORT"), out int envPort) ? envPort : 7860);

            // Check if already running
            if (IsRunning)
            {
                if (!quiet)
                {
                }
            }
            else
            {
                // Start the ASP.NET Core server
                var protocol = !string.IsNullOrEmpty(sslCertfile) ? "https" : "http";
                LocalUrl = $"{protocol}://{serverName}:{serverPort}";

                var webApp = App.CreateApp(
                    blocks: this,
                    strictCors: strictCors,
                    mcpServer: McpServer);

                webApp.Urls.Clear();
                webApp.Urls.Add(LocalUrl);

                var appInstance = webApp.Services.GetService<App>();
                appInstance?.ConfigureApp(this);
                SetServerApp(appInstance!);

                HostedWebApplication = webApp;
                HostedHost = webApp;

                await webApp.StartAsync();

                IsRunning = true;
                HasLaunched = true;

                if (!quiet)
                {
                }
            }

            // Ensure limiter reflects current MaxThreads even on rerun.
            CreateLimiter();

            // Handle share URL
            ShareUrl = null;
            if (share)
            {
                // Share URL creation (tunneling) requires external service integration
                // This would typically involve connecting to a tunneling service like:
                // - Gradio's built-in tunneling service
                // - ngrok or similar alternatives
                // Implementation requires network communication with the tunneling service
                if (!quiet)
                {
                }
            }

            // Open browser if requested
            if (inbrowser && !string.IsNullOrEmpty(LocalUrl))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = LocalUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    if (!quiet)
                    {
                    }
                }
            }

            // Print monitoring URL if enabled
            if (enableMonitoring == true && !quiet)
            {
            }

            // By default, mirror Python behavior and keep the server alive until shutdown.
            if (!preventThreadLock && HostedHost != null)
            {
                await HostedHost.WaitForShutdownAsync();
            }

            return new Tuple<string, string>(LocalUrl, ShareUrl);
        }

        public static Blocks FromConfig(
            Dictionary<string, object> config,
            List<Delegate> fns,
            string proxyUrl)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (!config.TryGetValue("components", out var componentsObj) || componentsObj == null)
                throw new ArgumentException("Config is missing required field 'components'.", nameof(config));
            if (!config.TryGetValue("layout", out var layoutObj) || layoutObj == null)
                throw new ArgumentException("Config is missing required field 'layout'.", nameof(config));
            if (!config.TryGetValue("dependencies", out var dependenciesObj) || dependenciesObj == null)
                throw new ArgumentException("Config is missing required field 'dependencies'.", nameof(config));

            // Create a new Blocks instance with top-level configuration (best-effort)
            var blocks = new Blocks(
                title: config.TryGetValue("title", out var titleObj) ? titleObj?.ToString() ?? "Gradio" : "Gradio",
                analyticsEnabled: config.TryGetValue("analytics_enabled", out var analyticsObj) ? analyticsObj as bool? : null,
                mode: config.TryGetValue("mode", out var modeObj) ? modeObj?.ToString() ?? "blocks" : "blocks"
            );

            // We use the proxy URL as an origin for component file serving.
            // Python keeps a set of proxy_urls for components loaded via gr.load.
            blocks.ProxyUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(proxyUrl))
            {
                blocks.ProxyUrls.Add(proxyUrl);
            }

            // Parse components config into a searchable list
            var componentsList = new List<Dictionary<string, object>>();
            if (componentsObj is IEnumerable<object> compEnumerable)
            {
                foreach (var item in compEnumerable)
                {
                    if (item is Dictionary<string, object> dict)
                    {
                        componentsList.Add(dict);
                    }
                }
            }
            else if (componentsObj is List<Dictionary<string, object>> directList)
            {
                componentsList = directList;
            }

            Dictionary<int, Block> originalMapping = new Dictionary<int, Block>();
            originalMapping[0] = blocks;

            static string EnsureTrailingSlash(string url)
            {
                if (string.IsNullOrWhiteSpace(url)) return url;
                return url.EndsWith("/") ? url : url + "/";
            }

            static string ToCamelCase(string snakeOrKebab)
            {
                if (string.IsNullOrEmpty(snakeOrKebab)) return snakeOrKebab;
                var parts = snakeOrKebab.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return snakeOrKebab;
                var first = parts[0];
                if (parts.Length == 1) return first;
                var sb = new System.Text.StringBuilder();
                sb.Append(first);
                for (int i = 1; i < parts.Length; i++)
                {
                    var p = parts[i];
                    if (p.Length == 0) continue;
                    sb.Append(char.ToUpperInvariant(p[0]));
                    if (p.Length > 1) sb.Append(p.Substring(1));
                }
                return sb.ToString();
            }

            static string ToPascalCase(string snakeOrKebab)
            {
                var camel = ToCamelCase(snakeOrKebab);
                if (string.IsNullOrEmpty(camel)) return camel;
                return char.ToUpperInvariant(camel[0]) + camel.Substring(1);
            }

            static object? ConvertValue(object? value, Type targetType)
            {
                if (value == null)
                {
                    return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                        ? Activator.CreateInstance(targetType)
                        : null;
                }

                var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                if (nonNullableType.IsInstanceOfType(value))
                {
                    return value;
                }

                // Lists (common: elem_classes -> List<string>)
                if (nonNullableType.IsGenericType && nonNullableType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elemType = nonNullableType.GetGenericArguments()[0];
                    if (value is IEnumerable<object> enumerable)
                    {
                        var list = (System.Collections.IList)Activator.CreateInstance(nonNullableType)!;
                        foreach (var item in enumerable)
                        {
                            list.Add(ConvertValue(item, elemType));
                        }
                        return list;
                    }
                }

                // Tuples from JSON often come as arrays/lists
                if (nonNullableType == typeof(Tuple<int, int>))
                {
                    if (value is IEnumerable<object> arr)
                    {
                        var vals = arr.ToList();
                        if (vals.Count >= 2)
                        {
                            return new Tuple<int, int>(Convert.ToInt32(vals[0]), Convert.ToInt32(vals[1]));
                        }
                    }
                }

                try
                {
                    if (nonNullableType.IsEnum)
                    {
                        return Enum.Parse(nonNullableType, value.ToString() ?? string.Empty, ignoreCase: true);
                    }
                    return Convert.ChangeType(value, nonNullableType);
                }
                catch
                {
                    return value;
                }
            }

            Type ResolveBlockType(string nameLower)
            {
                var assembly = typeof(Blocks).Assembly;
                var candidates = assembly.GetTypes()
                    .Where(t => typeof(Block).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                var pascal = ToPascalCase(nameLower);
                var type = candidates.FirstOrDefault(t =>
                    string.Equals(t.Name, nameLower, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(t.Name, pascal, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(t.Name.ToLowerInvariant(), nameLower.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));

                if (type == null)
                {
                    throw new NotSupportedException($"Unsupported block type '{nameLower}'. Cannot resolve to a C# Block type.");
                }
                return type;
            }

            Block CreateBlockInstance(int id)
            {
                var blockConfig = componentsList.FirstOrDefault(c => c.TryGetValue("id", out var idObj) && Convert.ToInt32(idObj) == id);
                if (blockConfig == null)
                {
                    throw new InvalidOperationException($"Cannot find block config with id {id}.");
                }

                if (!blockConfig.TryGetValue("props", out var propsObj) || propsObj is not Dictionary<string, object> props)
                {
                    throw new InvalidOperationException($"Invalid block config for id {id}: missing 'props'.");
                }

                // Determine class name (python uses props['name'])
                var name = props.TryGetValue("name", out var nameObj) ? nameObj?.ToString() : null;
                if (string.IsNullOrWhiteSpace(name))
                {
                    // Fall back to 'type' field if present
                    name = blockConfig.TryGetValue("type", out var typeObj) ? typeObj?.ToString() : null;
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidOperationException($"Invalid block config for id {id}: missing props.name / type.");
                }

                // Proxy URL logic: if missing, default to proxyUrl + '/'
                if (!props.TryGetValue("proxy_url", out var proxyObj) || proxyObj == null)
                {
                    if (!string.IsNullOrWhiteSpace(proxyUrl))
                    {
                        props["proxy_url"] = EnsureTrailingSlash(proxyUrl);
                    }
                }

                var postprocessedValue = props.TryGetValue("value", out var valObj) ? valObj : null;
                if (props.ContainsKey("value"))
                {
                    props.Remove("value");
                }

                // Normalize keys so both snake_case and camelCase are available for constructor matching
                var normalized = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in props)
                {
                    normalized[kvp.Key] = kvp.Value;
                    var camel = ToCamelCase(kvp.Key);
                    if (!normalized.ContainsKey(camel))
                    {
                        normalized[camel] = kvp.Value;
                    }
                }

                // Resolve the block type
                var blockType = ResolveBlockType(name);

                // Pick a constructor and map arguments
                var ctors = blockType.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToList();
                if (ctors.Count == 0)
                {
                    throw new InvalidOperationException($"Type {blockType.FullName} has no public constructors.");
                }

                var ctor = ctors[0];
                var parameters = ctor.GetParameters();
                var args = new object?[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    object? arg = null;
                    if (normalized.TryGetValue(p.Name ?? string.Empty, out var v))
                    {
                        arg = ConvertValue(v, p.ParameterType);
                    }
                    else if (p.HasDefaultValue)
                    {
                        arg = p.DefaultValue;
                    }
                    else
                    {
                        arg = p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null
                            ? Activator.CreateInstance(p.ParameterType)
                            : null;
                    }
                    args[i] = arg;
                }

                var instance = (Block)ctor.Invoke(args);

                // Apply proxy_url to instance and collect it
                if (props.TryGetValue("proxy_url", out var proxyUrlObj) && proxyUrlObj is string proxyStr)
                {
                    instance.ProxyUrl = proxyStr;
                    if (!string.IsNullOrWhiteSpace(proxyStr))
                    {
                        blocks.ProxyUrls.Add(proxyStr);
                    }
                }

                // Restore postprocessed value (common for components)
                if (postprocessedValue != null)
                {
                    var valueProp = instance.GetType().GetProperty("Value");
                    if (valueProp != null && valueProp.CanWrite)
                    {
                        valueProp.SetValue(instance, postprocessedValue);
                    }
                }

                // _selectable mapping
                if (props.TryGetValue("_selectable", out var selectableObj))
                {
                    var selectableProp = instance.GetType().GetProperty("Selectable") ?? instance.GetType().GetProperty("_selectable");
                    if (selectableProp != null && selectableProp.CanWrite)
                    {
                        selectableProp.SetValue(instance, selectableObj);
                    }
                }

                return instance;
            }

            void IterateChildren(object? childrenObj)
            {
                if (childrenObj is not IEnumerable<object> children)
                    return;

                foreach (var child in children)
                {
                    if (child is not Dictionary<string, object> childDict || !childDict.TryGetValue("id", out var idObj))
                        continue;

                    var childId = Convert.ToInt32(idObj);
                    var block = CreateBlockInstance(childId);
                    originalMapping[childId] = block;

                    if (childDict.TryGetValue("children", out var nestedChildren) && nestedChildren is IEnumerable<object>)
                    {
                        if (block is not BlockContext ctx)
                        {
                            throw new InvalidOperationException($"Invalid config: block id {childId} has children but is not a BlockContext.");
                        }

                        ctx.Enter();
                        try
                        {
                            IterateChildren(nestedChildren);
                        }
                        finally
                        {
                            ctx.Exit();
                        }
                    }
                }
            }

            // Rebuild layout tree inside a Blocks context
            blocks.Enter();
            try
            {
                if (layoutObj is Dictionary<string, object> layoutDict)
                {
                    if (layoutDict.TryGetValue("children", out var childrenObj))
                    {
                        IterateChildren(childrenObj);
                    }
                }

                // Wire dependencies
                if (dependenciesObj is IEnumerable<object> depsEnumerable)
                {
                    int fnIndex = 0;
                    foreach (var depObj in depsEnumerable)
                    {
                        if (depObj is not Dictionary<string, object> dep)
                            continue;

                        // Skip legacy fake events
                        if (dep.TryGetValue("trigger", out var triggerObj) && triggerObj?.ToString() == "fake_event")
                        {
                            continue;
                        }

                        Delegate? fn = null;
                        if (fns != null && fnIndex < fns.Count)
                        {
                            fn = fns[fnIndex];
                        }
                        fnIndex++;

                        // targets
                        var targets = new List<EventListenerMethod>();
                        if (dep.TryGetValue("targets", out var targetsObj) && targetsObj is IEnumerable<object> targetsEnumerable)
                        {
                            foreach (var t in targetsEnumerable)
                            {
                                if (t is IEnumerable<object> pair)
                                {
                                    var list = pair.ToList();
                                    if (list.Count >= 2)
                                    {
                                        object? blockIdObj = list[0];
                                        var eventName = list[1]?.ToString() ?? string.Empty;
                                        object? targetBlock = null;
                                        if (blockIdObj != null && blockIdObj.ToString() != "null")
                                        {
                                            var bid = Convert.ToInt32(blockIdObj);
                                            if (originalMapping.TryGetValue(bid, out var mapped))
                                            {
                                                targetBlock = mapped;
                                            }
                                        }
                                        targets.Add(new EventListenerMethod(targetBlock, eventName));
                                    }
                                }
                            }
                        }

                        // inputs/outputs
                        object? inputs = null;
                        if (dep.TryGetValue("inputs", out var inputsObj) && inputsObj is IEnumerable<object> inputsEnum)
                        {
                            var inBlocks = new List<object>();
                            foreach (var iid in inputsEnum)
                            {
                                if (iid == null) continue;
                                var idVal = Convert.ToInt32(iid);
                                if (originalMapping.TryGetValue(idVal, out var b)) inBlocks.Add(b);
                            }
                            inputs = inBlocks;
                        }

                        object? outputs = null;
                        if (dep.TryGetValue("outputs", out var outputsObj) && outputsObj is IEnumerable<object> outputsEnum)
                        {
                            var outBlocks = new List<object>();
                            foreach (var oid in outputsEnum)
                            {
                                if (oid == null) continue;
                                var idVal = Convert.ToInt32(oid);
                                if (originalMapping.TryGetValue(idVal, out var b)) outBlocks.Add(b);
                            }
                            outputs = outBlocks;
                        }

                        // options
                        bool preprocess = dep.TryGetValue("preprocess", out var preprocessObj) ? Convert.ToBoolean(preprocessObj) : true;
                        bool postprocess = dep.TryGetValue("postprocess", out var postprocessObj) ? Convert.ToBoolean(postprocessObj) : true;
                        bool scrollToOutput = dep.TryGetValue("scroll_to_output", out var scrollObj) ? Convert.ToBoolean(scrollObj) : false;
                        string showProgress = dep.TryGetValue("show_progress", out var showObj) ? showObj?.ToString() ?? "full" : "full";
                        string? apiName = dep.TryGetValue("api_name", out var apiNameObj) ? apiNameObj?.ToString() : null;
                        object? apiDescription = dep.TryGetValue("api_description", out var apiDescObj) ? apiDescObj : null;
                        object? js = dep.TryGetValue("js", out var jsObj) ? jsObj : null;
                        bool queue = dep.TryGetValue("queue", out var queueObj) ? Convert.ToBoolean(queueObj) : true;
                        bool batch = dep.TryGetValue("batch", out var batchObj) ? Convert.ToBoolean(batchObj) : false;
                        int maxBatchSize = dep.TryGetValue("max_batch_size", out var maxBatchObj) ? Convert.ToInt32(maxBatchObj) : 4;
                        var cancels = dep.TryGetValue("cancels", out var cancelsObj) && cancelsObj is IEnumerable<object> cancelEnum
                            ? cancelEnum.Select(x => Convert.ToInt32(x)).ToList()
                            : null;
                        int? triggerAfter = dep.TryGetValue("trigger_after", out var trigAfterObj) && trigAfterObj != null ? (int?)Convert.ToInt32(trigAfterObj) : null;
                        bool triggerOnlyOnSuccess = dep.TryGetValue("trigger_only_on_success", out var succObj) && succObj != null && Convert.ToBoolean(succObj);
                        bool triggerOnlyOnFailure = dep.TryGetValue("trigger_only_on_failure", out var failObj) && failObj != null && Convert.ToBoolean(failObj);
                        string triggerMode = dep.TryGetValue("trigger_mode", out var trigModeObj) ? trigModeObj?.ToString() ?? "once" : "once";
                        object? concurrencyLimit = dep.TryGetValue("concurrency_limit", out var concObj) ? concObj : null;
                        string? concurrencyId = dep.TryGetValue("concurrency_id", out var concIdObj) ? concIdObj?.ToString() : null;
                        string apiVisibility = dep.TryGetValue("api_visibility", out var apiVisObj) ? apiVisObj?.ToString() ?? "public" : (dep.TryGetValue("show_api", out var showApiObj) && showApiObj != null && Convert.ToBoolean(showApiObj) ? "public" : "private");
                        string connection = dep.TryGetValue("connection", out var connObj) ? connObj?.ToString() ?? "sse" : "sse";
                        float? timeLimit = dep.TryGetValue("time_limit", out var tlObj) && tlObj != null ? (float?)Convert.ToSingle(tlObj) : null;
                        float streamEvery = dep.TryGetValue("stream_every", out var seObj) && seObj != null ? Convert.ToSingle(seObj) : 0.5f;
                        bool collectsEventData = dep.TryGetValue("collects_event_data", out var cedObj) && cedObj != null && Convert.ToBoolean(cedObj);

                        // Create the function trigger in the new Blocks
                        blocks.DefaultConfig.SetEventTrigger(
                            targets: targets,
                            fn: fn,
                            inputs: inputs,
                            outputs: outputs,
                            preprocess: preprocess,
                            postprocess: postprocess,
                            scrollToOutput: scrollToOutput,
                            showProgress: showProgress,
                            apiName: apiName,
                            apiDescription: apiDescription,
                            js: js?.ToString(),
                            queue: queue,
                            batch: batch,
                            maxBatchSize: maxBatchSize,
                            cancels: cancels,
                            collectsEventData: collectsEventData,
                            triggerAfter: triggerAfter,
                            triggerOnlyOnSuccess: triggerOnlyOnSuccess,
                            triggerOnlyOnFailure: triggerOnlyOnFailure,
                            triggerMode: triggerMode,
                            concurrencyLimit: concurrencyLimit,
                            concurrencyId: concurrencyId,
                            apiVisibility: apiVisibility,
                            connection: connection,
                            timeLimit: timeLimit,
                            streamEvery: streamEvery
                        );
                    }
                }
            }
            finally
            {
                blocks.Exit();
            }

            return blocks;
        }

        public Dictionary<string, object> GetApiInfo(bool allEndpoints = false)
        {
            // Python parity: blocks.py get_api_info()
            // Use config if it was previously computed; otherwise compute on demand.
            var config = Config ?? GetConfigFile();

            var apiInfo = new Dictionary<string, object>
            {
                ["named_endpoints"] = new Dictionary<string, object>(),
                ["unnamed_endpoints"] = new Dictionary<string, object>()
            };

            var namedEndpoints = (Dictionary<string, object>)apiInfo["named_endpoints"];
            var unnamedEndpoints = (Dictionary<string, object>)apiInfo["unnamed_endpoints"];

            foreach (var kvp in Fns)
            {
                var blockFn = kvp.Value;

                if (blockFn.Fn == null || string.Equals(blockFn.ApiVisibility, "private", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!allEndpoints && !string.Equals(blockFn.ApiVisibility, "public", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var endpointInfo = new Dictionary<string, object>
                {
                    ["parameters"] = new List<object>(),
                    ["returns"] = new List<object>(),
                    ["api_visibility"] = blockFn.ApiVisibility
                };

                // Description
                var description = blockFn.ApiDescription;
                if (description == null && blockFn.Fn != null)
                {
                    var desc = Gradio.Net.Utils.Utils.GetFunctionDescription(blockFn.Fn.Method);
                    description = desc.Description;
                }
                endpointInfo["description"] = description ?? string.Empty;

                var parametersList = (List<object>)endpointInfo["parameters"];
                var returnsList = (List<object>)endpointInfo["returns"];

                var methodParams = blockFn.Fn.Method.GetParameters();
                var userParams = methodParams.Where(p => !Gradio.Net.Utils.Utils.IsSpecialTypedParameter(p)).ToList();

                // Inputs
                bool skipEndpoint = false;
                var inputBlocks = GetInputBlocks(blockFn);
                for (int index = 0; index < inputBlocks.Count; index++)
                {
                    var inputBlock = inputBlocks[index];
                    if (!BlocksDict.TryGetValue(inputBlock._id, out var realBlock))
                    {
                        skipEndpoint = true;
                        break;
                    }

                    if (realBlock.SkipApi)
                    {
                        continue;
                    }

                    if (realBlock is not Components.Component comp)
                    {
                        throw new InvalidOperationException($"{realBlock} is not a Component");
                    }

                    var info = comp.ApiInfoAsInput() ?? comp.ApiInfo();
                    var pythonType = BuildPythonType(info);

                    var label = comp.Label ?? $"parameter_{inputBlock._id}";

                    // parameter_name resolution similar to Python: use function param name when possible, else param_{index}
                    string parameterName;
                    if (index < userParams.Count)
                    {
                        var candidate = userParams[index].Name;
                        if (!string.IsNullOrWhiteSpace(candidate) &&
                            candidate != "api_name" &&
                            candidate != "fn_index" &&
                            candidate != "result_callbacks")
                        {
                            parameterName = candidate;
                        }
                        else
                        {
                            parameterName = $"param_{index}";
                        }
                    }
                    else
                    {
                        parameterName = $"param_{index}";
                    }

                    bool hasDefault;
                    object? defaultValue;
                    if (comp.Value != null)
                    {
                        hasDefault = true;
                        defaultValue = comp.Value;
                    }
                    else if (index < userParams.Count && userParams[index].HasDefaultValue && userParams[index].DefaultValue == null)
                    {
                        hasDefault = true;
                        defaultValue = null;
                    }
                    else
                    {
                        hasDefault = false;
                        defaultValue = null;
                    }

                    var exampleInput = GetComponentExampleInput(comp);

                    parametersList.Add(new Dictionary<string, object?>
                    {
                        ["label"] = label,
                        ["parameter_name"] = parameterName,
                        ["parameter_has_default"] = hasDefault,
                        ["parameter_default"] = defaultValue,
                        ["type"] = info,
                        ["python_type"] = pythonType,
                        ["component"] = GetApiComponentName(comp),
                        ["example_input"] = exampleInput
                    });
                }

                // Outputs
                var outputBlocks = GetOutputBlocks(blockFn);
                foreach (var outputBlock in outputBlocks)
                {
                    if (!BlocksDict.TryGetValue(outputBlock._id, out var realBlock))
                    {
                        skipEndpoint = true;
                        break;
                    }

                    if (realBlock.SkipApi)
                    {
                        continue;
                    }

                    if (realBlock is not Components.Component comp)
                    {
                        throw new InvalidOperationException($"{realBlock} is not a Component");
                    }

                    var info = comp.ApiInfoAsOutput() ?? comp.ApiInfo();
                    var pythonType = BuildPythonType(info);
                    var label = comp.Label ?? $"value_{outputBlock._id}";

                    returnsList.Add(new Dictionary<string, object?>
                    {
                        ["label"] = label,
                        ["type"] = info,
                        ["python_type"] = pythonType,
                        ["component"] = GetApiComponentName(comp)
                    });
                }

                if (skipEndpoint)
                {
                    continue;
                }

                // Add to named or unnamed endpoints
                if (!string.IsNullOrEmpty(blockFn.ApiName))
                {
                    namedEndpoints[$"/{blockFn.ApiName}"] = endpointInfo;
                }
                else
                {
                    unnamedEndpoints[$"/api/predict/{kvp.Key}"] = endpointInfo;
                }
            }

            return apiInfo;
        }

        public string GetApiInfoJson(bool allEndpoints = false)
        {
            var apiInfo = GetApiInfo(allEndpoints);
            return System.Text.Json.JsonSerializer.Serialize(apiInfo);
        }

        private static Dictionary<string, object> BuildPythonType(Dictionary<string, object>? apiInfo)
        {
            var apiType = apiInfo != null && apiInfo.TryGetValue("type", out var t) ? t?.ToString() : null;
            var pyType = apiType switch
            {
                "string" => "str",
                "number" => "float",
                "integer" => "int",
                "boolean" => "bool",
                "array" => BuildArrayPythonType(apiInfo),
                "object" => "dict",
                _ => "Any"
            };

            return new Dictionary<string, object>
            {
                ["type"] = pyType,
                ["description"] = string.Empty
            };
        }

        private static string BuildArrayPythonType(Dictionary<string, object>? apiInfo)
        {
            if (apiInfo == null || !apiInfo.TryGetValue("items", out var itemsObj) || itemsObj is not Dictionary<string, object> items)
            {
                return "list";
            }

            if (!items.TryGetValue("enum", out var enumObj) || enumObj is not IEnumerable<object> enumValues)
            {
                return "list";
            }

            var literalValues = enumValues
                .Select(v => $"'{(v?.ToString() ?? string.Empty).Replace("'", "\\'")}'")
                .ToList();

            return literalValues.Count > 0
                ? $"list[Literal[{string.Join(", ", literalValues)}]]"
                : "list";
        }

        private static string GetApiComponentName(Components.Component component)
        {
            var blockName = component.GetBlockName();
            if (string.IsNullOrWhiteSpace(blockName))
            {
                return component.GetType().Name;
            }

            var parts = blockName
                .Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p.Substring(1) : string.Empty));

            var joined = string.Concat(parts);
            return string.IsNullOrWhiteSpace(joined) ? component.GetType().Name : joined;
        }

        private static object? GetComponentExampleInput(Components.Component component)
        {
            var type = component.GetType();

            var payloadMethod = type.GetMethod("ExamplePayload");
            if (payloadMethod != null)
            {
                try
                {
                    return payloadMethod.Invoke(component, null);
                }
                catch
                {
                    // fallback below
                }
            }

            var valueMethod = type.GetMethod("ExampleValue");
            if (valueMethod != null)
            {
                try
                {
                    return valueMethod.Invoke(component, null);
                }
                catch
                {
                    // fallback below
                }
            }

            var list = component.ExampleInputs();
            if (list == null)
            {
                return null;
            }

            return list.Count == 1 ? list[0] : list;
        }

        private void ValidateNavbar()
        {
            // Delegate to the Python-parity validation used by Exit().
            ValidateNavbarSettings();
        }

        public void ValidateQueueSettings()
        {
            foreach (var dep in Fns.Values)
            {
                foreach (var cancelId in dep.Cancels ?? new List<int>())
                {
                    if (Fns.TryGetValue(cancelId, out var cancelFn) && !cancelFn.Queue)
                    {
                        throw new InvalidOperationException(
                            "Queue needs to be enabled! " +
                            "You may get this error by either 1) passing a function that uses the yield keyword " +
                            "into an interface without enabling the queue or 2) defining an event that cancels " +
                            "another event without enabling the queue. Both can be solved by calling .Queue() " +
                            "before .Launch()");
                    }
                }
            }
        }

        public List<BlockFunction> GetDependencies()
        {
            return Fns.Values.ToList();
        }

        public static List<EventListenerMethod> GetEventTargets(
            Dictionary<int, Block> originalMapping,
            List<object> targets,
            string trigger)
        {
            originalMapping ??= new Dictionary<int, Block>();
            targets ??= new List<object>();

            var resolved = new List<EventListenerMethod>();
            foreach (var target in targets)
            {
                int targetId;
                string eventName;

                if (target is int i)
                {
                    targetId = i;
                    eventName = trigger;
                }
                else if (target is IEnumerable<object> pair)
                {
                    var list = pair.ToList();
                    targetId = list.Count > 0 && list[0] != null ? Convert.ToInt32(list[0]) : 0;
                    eventName = list.Count > 1 ? (list[1]?.ToString() ?? trigger) : trigger;
                }
                else
                {
                    continue;
                }

                if (!originalMapping.TryGetValue(targetId, out var block) || block == null)
                {
                    // Blocks events may not be present in components list; fall back to current root block.
                    if (string.Equals(eventName, "load", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(eventName, "unload", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(eventName, "stream", StringComparison.OrdinalIgnoreCase))
                    {
                        block = Context.RootBlock ?? originalMapping.GetValueOrDefault(0);
                    }
                }

                if (block == null)
                {
                    throw new InvalidOperationException(
                        $"Cannot find Block with id: {targetId} but is present as a target in the config");
                }

                resolved.Add(new EventListenerMethod(block, eventName));
            }

            return resolved;
        }

        public void SetRenderOrder()
        {
            // The render order is determined by the order components were added
            // In C#, this is maintained by the BlocksConfig.Blocks list order
            // No additional computation needed as order is preserved during construction

            // If specific ordering logic is needed, it would go here
            // For now, components render in the order they were added to the Blocks
        }

        public BlockFunction? GetFunctionByApiName(string apiName)
        {
            return Fns.Values.FirstOrDefault(fn => fn.ApiName == apiName);
        }

        public int? GetFunctionIndexByApiName(string apiName)
        {
            foreach (var kvp in Fns)
            {
                if (kvp.Value.ApiName == apiName)
                    return kvp.Key;
            }
            return null;
        }

        public async Task<Dictionary<string, object>> ProcessApi(
            BlockFunction blockFn,
            List<object> inputs,
            object request = null,
            object state = null,
            object iterator = null,
            string sessionHash = null,
            string eventId = null,
            object eventData = null,
            bool inEventListener = true,
            bool? simpleFormat = null,
            bool explicitCall = false,
            string rootPath = null)
        {
            var startTime = DateTime.UtcNow;

            // Preserve SessionState (used for stateful components and prop-update persistence)
            var sessionState = state as SessionState ?? new SessionState(this);

            // Python parity: LocalContext.blocks.set(self) (plus blocks_config and event metadata)
            var prevBlocks = LocalContext.Blocks;
            var prevInEventListener = LocalContext.InEventListener;
            var prevEventId = LocalContext.EventId;
            var prevBlocksConfig = LocalContext.BlocksConfig;
            try
            {
                LocalContext.Blocks = this;
                LocalContext.InEventListener = inEventListener;
                LocalContext.EventId = eventId;
                if (sessionState.BlocksConfig is BlocksConfig bc)
                {
                    LocalContext.BlocksConfig = bc;
                }
            }
            catch
            {
                // Best-effort; do not fail request processing if AsyncLocal assignment fails.
            }

            try
            {
                // Handle batch processing
                if (blockFn.Batch)
                {
                    var maxBatchSize = blockFn.MaxBatchSize;
                    var batchSizes = inputs.Select(inp =>
                        inp is System.Collections.IList list ? list.Count : 1).ToList();
                    var batchSize = batchSizes.FirstOrDefault();

                    // Validate batch sizes
                    if (!batchSizes.All(x => x == batchSize))
                    {
                        throw new ArgumentException(
                            $"All inputs to a batch function must have the same length but instead have sizes: {string.Join(", ", batchSizes)}");
                    }

                    if (batchSize > maxBatchSize)
                    {
                        throw new ArgumentException(
                            $"Batch size ({batchSize}) exceeds the max_batch_size for this function ({maxBatchSize})");
                    }

                    // Preprocess batch inputs
                    var preprocessedBatch = new List<List<object>>();
                    for (int i = 0; i < batchSize; i++)
                    {
                        var batchInputs = new List<object>();
                        foreach (var inp in inputs)
                        {
                            if (inp is System.Collections.IList list && list.Count > i)
                            {
                                batchInputs.Add(list[i]);
                            }
                        }
                        var preprocessed = await PreprocessData(blockFn, batchInputs, sessionState, explicitCall);
                        preprocessedBatch.Add(preprocessed);
                    }

                    // Transpose for function call
                    var transposedInputs = new List<object>();
                    if (preprocessedBatch.Count > 0)
                    {
                        for (int i = 0; i < preprocessedBatch[0].Count; i++)
                        {
                            var column = preprocessedBatch.Select(row => row[i]).ToList();
                            transposedInputs.Add(column);
                        }
                    }

                    // Call function
                    var result = await CallFunction(
                        blockFn, transposedInputs, null, request, eventId,
                        eventData, inEventListener, state);

                    // Convert prediction to column-format: [[col0_item0, col0_item1,...], [col1_item0,...], ...]
                    // The user's batch function may return:
                    //   - List<T>   (single output):  treat as one column → wrap: [[items...]]
                    //   - List<object> where each element is an IList (multi-output): already column format
                    //   - ITuple of ILists (multi-output via tuple return): extract each list as a column
                    var rawPrediction = result["prediction"];
                    List<object> predictions = null;

                    if (rawPrediction is List<object> listObj)
                    {
                        // Determine if this is already column-format or a single column
                        var outputBlockCount = GetOutputBlocks(blockFn)?.Count ?? 1;
                        bool allAreLists = listObj.Count > 0 && listObj.All(p => p is System.Collections.IList);
                        if (outputBlockCount == listObj.Count && allAreLists)
                        {
                            predictions = listObj; // already column format
                        }
                        else
                        {
                            predictions = new List<object> { listObj }; // wrap as single column
                        }
                    }
                    else if (rawPrediction is System.Runtime.CompilerServices.ITuple tupleResult)
                    {
                        // Tuple return (e.g. ValueTuple<List<string>, List<double>>) — one element per output
                        predictions = new List<object>();
                        for (int ti = 0; ti < tupleResult.Length; ti++)
                        {
                            var col = tupleResult[ti];
                            if (col is System.Collections.IList colList)
                            {
                                var converted = new List<object>();
                                foreach (var item in colList) converted.Add(item);
                                predictions.Add(converted);
                            }
                            else
                            {
                                predictions.Add(new List<object> { col });
                            }
                        }
                    }
                    else if (rawPrediction is System.Collections.IList iList)
                    {
                        // List<T> (e.g. List<string>) — single output column
                        var col = new List<object>();
                        foreach (var item in iList) col.Add(item);
                        predictions = new List<object> { col };
                    }
                    else if (rawPrediction != null)
                    {
                        predictions = new List<object> { new List<object> { rawPrediction } };
                    }

                    // Postprocess batch outputs
                    var outputBatch = new List<List<object>>();
                    if (predictions != null)
                    {
                        for (int i = 0; i < batchSize; i++)
                        {
                            var batchOutputs = new List<object>();
                            foreach (var pred in predictions)
                            {
                                if (pred is System.Collections.IList list && list.Count > i)
                                {
                                    batchOutputs.Add(list[i]);
                                }
                            }
                            var postprocessed = await PostprocessData(blockFn, batchOutputs, sessionState);
                            outputBatch.Add(postprocessed);
                        }
                    }

                    // Transpose back
                    var transposedOutputs = new List<object>();
                    if (outputBatch.Count > 0)
                    {
                        for (int i = 0; i < outputBatch[0].Count; i++)
                        {
                            var column = outputBatch.Select(row => row[i]).ToList();
                            transposedOutputs.Add(column);
                        }
                    }

                    var duration = (DateTime.UtcNow - startTime).TotalSeconds;
                    blockFn.TotalRuntime += duration;
                    blockFn.TotalRuns += 1;

                    return new Dictionary<string, object>
                    {
                        ["data"] = transposedOutputs,
                        ["is_generating"] = false,
                        ["iterator"] = null,
                        ["duration"] = duration,
                        ["average_duration"] = blockFn.TotalRuntime / blockFn.TotalRuns,
                        ["render_config"] = null,
                        ["changed_state_ids"] = new List<int>()
                    };
                }
                else
                {
                    // Non-batch processing
                    var oldIterator = iterator;
                    List<object> processedInputs;

                    if (oldIterator != null)
                    {
                        processedInputs = new List<object>();
                    }
                    else
                    {
                        processedInputs = await PreprocessData(blockFn, inputs, sessionState, explicitCall);
                    }

                    var wasGenerating = oldIterator != null;

                    // Python parity: track state changes for stateful outputs that have change listeners
                    var (stateIdsToTrack, hashedValues) = GetStateIdsToTrack(blockFn, sessionState);
                    var changedStateIds = new List<int>();

                    // Call function
                    var result = await CallFunction(
                        blockFn, processedInputs, oldIterator, request,
                        eventId, eventData, inEventListener, state);

                    var predictions = result["prediction"];
                    var data = await PostprocessData(blockFn, predictions, sessionState);

                    if (stateIdsToTrack.Count > 0)
                    {
                        foreach (var stateId in stateIdsToTrack)
                        {
                            var currentValue = sessionState[stateId];
                            var currentHash = Gradio.Net.Utils.Utils.DeepHash(currentValue);
                            if (hashedValues.TryGetValue(stateId, out var previousHash) && previousHash != currentHash)
                            {
                                changedStateIds.Add(stateId);
                            }
                        }
                    }

                    // Add root path to file URLs
                    if (!string.IsNullOrEmpty(rootPath))
                    {
                        var processedData = ProcessingUtils.AddRootUrl(data, rootPath, null);
                        if (processedData is List<object> dataList)
                        {
                            data = dataList;
                        }
                    }

                    var isGenerating = (bool)(result["is_generating"] ?? false);
                    var newIterator = result["iterator"];

                    // Handle streaming outputs and diffs
                    if (isGenerating || wasGenerating)
                    {
                        var run = wasGenerating ? oldIterator?.GetHashCode() ?? 0 : newIterator?.GetHashCode() ?? 0;

                        // Handle streaming outputs (audio, video, etc.)
                        data = await HandleStreamingOutputs(
                            blockFn,
                            data,
                            sessionHash,
                            run,
                            rootPath,
                            final: !isGenerating
                        );

                        // Handle streaming diffs for efficient data transmission
                        data = HandleStreamingDiffs(
                            blockFn,
                            data,
                            sessionHash,
                            run,
                            final: !isGenerating,
                            simpleFormat: simpleFormat ?? false
                        );
                    }

                    var duration = (DateTime.UtcNow - startTime).TotalSeconds;
                    blockFn.TotalRuntime += duration;
                    blockFn.TotalRuns += 1;

                    var output = new Dictionary<string, object>
                    {
                        ["data"] = data,
                        ["is_generating"] = isGenerating,
                        ["iterator"] = newIterator,
                        ["duration"] = duration,
                        ["average_duration"] = blockFn.TotalRuntime / blockFn.TotalRuns,
                        ["render_config"] = null,
                        ["changed_state_ids"] = changedStateIds
                    };

                    // Add render config if renderable (Python parity)
                    if (blockFn.Renderable != null && sessionState.BlocksConfig is BlocksConfig stateBlocksConfig)
                    {
                        var renderConfig = stateBlocksConfig.GetConfig(blockFn.Renderable);
                        renderConfig["render_id"] = blockFn.Renderable.Id;

                        if (!string.IsNullOrEmpty(rootPath))
                        {
                            var rooted = ProcessingUtils.AddRootUrl(renderConfig, rootPath, null);
                            if (rooted is Dictionary<string, object> rootedDict)
                            {
                                renderConfig = rootedDict;
                            }
                        }

                        output["render_config"] = renderConfig;
                    }

                    return output;
                }
            }
            finally
            {
                // Restore previous LocalContext values
                try
                {
                    LocalContext.Blocks = prevBlocks;
                    LocalContext.InEventListener = prevInEventListener;
                    LocalContext.EventId = prevEventId;
                    LocalContext.BlocksConfig = prevBlocksConfig;
                }
                catch
                {
                    // Ignore
                }
            }
        }

        private (List<int> StateIds, Dictionary<int, string> HashedValues) GetStateIdsToTrack(
            BlockFunction blockFn,
            SessionState? state)
        {
            if (state == null)
            {
                return (new List<int>(), new Dictionary<int, string>());
            }

            var stateIdsToTrack = new List<int>();
            var hashedValues = new Dictionary<int, string>();

            var cfg = state.BlocksConfig as BlocksConfig;
            if (cfg == null)
            {
                return (stateIdsToTrack, hashedValues);
            }

            var outputBlocks = GetOutputBlocks(blockFn);

            foreach (var block in outputBlocks)
            {
                if (!block.Stateful)
                {
                    continue;
                }

                // Prefer per-session config, but fall back to current block config.
                // This keeps parity when session copies are stale/incomplete.
                bool hasChangeListener = cfg.Fns.Values.Any(fn =>
                    fn.Targets != null && fn.Targets.Any(t => t.blockId == block._id && string.Equals(t.eventName, "change", StringComparison.OrdinalIgnoreCase)));

                if (!hasChangeListener)
                {
                    hasChangeListener = Fns.Values.Any(fn =>
                        fn.Targets != null && fn.Targets.Any(t => t.blockId == block._id && string.Equals(t.eventName, "change", StringComparison.OrdinalIgnoreCase)));
                }

                if (!hasChangeListener)
                {
                    continue;
                }

                var value = state[block._id];
                stateIdsToTrack.Add(block._id);
                hashedValues[block._id] = Gradio.Net.Utils.Utils.DeepHash(value);
            }

            return (stateIdsToTrack, hashedValues);
        }

        private async Task<List<object>> PreprocessData(
            BlockFunction blockFn,
            List<object> inputs,
            object state,
            bool explicitCall = false)
        {
            // Validate inputs first
            ValidateInputs(blockFn, inputs);

            var sessionState = state as SessionState;

            var processedInputs = new List<object>();
            var inputBlocks = GetInputBlocks(blockFn);
            var payloadIndex = 0;

            for (int i = 0; i < inputBlocks.Count; i++)
            {
                var block = inputBlocks[i];

                object input;
                if (block.Stateful && sessionState != null)
                {
                    // Python parity: stateful inputs are restored from session state,
                    // not from frontend payload.
                    input = sessionState[block._id];
                    // Advance payloadIndex because the frontend always sends a null placeholder
                    // for stateful inputs in the data array (Python parity: zip alignment).
                    payloadIndex++;
                }
                else if (!block.SkipApi)
                {
                    input = payloadIndex < inputs.Count ? inputs[payloadIndex] : null;
                    payloadIndex++;
                }
                else
                {
                    // skip_api non-stateful inputs are not expected in payload.
                    input = null;
                }

                // Convert JsonElement (from System.Text.Json deserialization of List<object>) to
                // proper CLR types so that AsyncMoveFilesToCache and Preprocess can handle them.
                if (input is System.Text.Json.JsonElement inputJe)
                {
                    input = ConvertJsonElementToObject(inputJe);
                }

                // Check if this is a component property input
                bool isPropInput = blockFn.ComponentPropInputs?.Contains(i) ?? false;

                // Get value to process
                object valueToProcess = input;
                if (isPropInput && input is Dictionary<string, object> dict)
                {
                    // Check all files in cache if it's a prop input
                    ProcessingUtils.CheckAllFilesInCache(dict, block);
                    valueToProcess = dict.ContainsKey("value") ? dict["value"] : null;
                }

                // Move files to cache
                var inputsCached = await ProcessingUtils.AsyncMoveFilesToCache(
                    valueToProcess,
                    block,
                    postprocess: false,
                    checkInUploadFolder: !explicitCall,
                    keepInCache: false
                );

                // Track value in session config (best-effort parity with SessionState._update_value_in_config)
                if (sessionState != null)
                {
                    sessionState.UpdateValueInConfig(block._id, inputsCached);
                }

                // Apply preprocessing if enabled
                object processedValue = inputsCached;
                if (blockFn.Preprocess)
                {
                    // Call block.Preprocess if the component supports it
                    var preprocessMethod = block.GetType().GetMethod("Preprocess");
                    if (preprocessMethod != null)
                    {
                        processedValue = preprocessMethod.Invoke(block, new[] { inputsCached });
                    }
                }

                // Handle property inputs
                if (isPropInput && input is Dictionary<string, object> propDict)
                {
                    propDict["value"] = processedValue;
                    processedInputs.Add(propDict);
                }
                else
                {
                    processedInputs.Add(processedValue);
                }
            }

            return processedInputs;
        }

        private List<Block> GetInputBlocks(BlockFunction blockFn)
        {
            var blocks = new List<Block>();

            if (blockFn.Inputs is List<object> inputList)
            {
                blocks = inputList.OfType<Block>().ToList();
            }
            else if (blockFn.Inputs is Block inputBlock)
            {
                blocks.Add(inputBlock);
            }
            else if (blockFn.Inputs is Block[] inputArray)
            {
                blocks.AddRange(inputArray);
            }

            return blocks;
        }

        private List<Block> GetOutputBlocks(BlockFunction blockFn)
        {
            var blocks = new List<Block>();

            if (blockFn.Outputs is List<object> outputList)
            {
                blocks = outputList.OfType<Block>().ToList();
            }
            else if (blockFn.Outputs is Block outputBlock)
            {
                blocks.Add(outputBlock);
            }
            else if (blockFn.Outputs is Block[] outputArray)
            {
                blocks.AddRange(outputArray);
            }

            return blocks;
        }

        private static object ConvertJsonElementToObject(System.Text.Json.JsonElement je)
        {
            switch (je.ValueKind)
            {
                case System.Text.Json.JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in je.EnumerateObject())
                        dict[prop.Name] = ConvertJsonElementToObject(prop.Value);
                    return dict;
                case System.Text.Json.JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in je.EnumerateArray())
                        list.Add(ConvertJsonElementToObject(item));
                    return list;
                case System.Text.Json.JsonValueKind.String:
                    return je.GetString() ?? string.Empty;
                case System.Text.Json.JsonValueKind.Number:
                    if (je.TryGetDouble(out var d)) return d;
                    return je.GetRawText();
                case System.Text.Json.JsonValueKind.True:
                    return true;
                case System.Text.Json.JsonValueKind.False:
                    return false;
                case System.Text.Json.JsonValueKind.Null:
                    return null;
                default:
                    return je.GetRawText();
            }
        }

        private async Task<Dictionary<string, object>> CallFunction(
            BlockFunction blockFn,
            List<object> inputs,
            object iterator,
            object request,
            string eventId,
            object eventData,
            bool inEventListener,
            object state)
        {
            var startTime = DateTime.UtcNow;
            object prediction = null;
            bool isGenerating = false;
            object newIterator = null;

            try
            {
                if (blockFn.Fn == null)
                {
                    // No function to call - return inputs as-is
                    prediction = inputs.Count == 1 ? inputs[0] : inputs;
                }
                else if (iterator != null)
                {
                    // Continue existing generator/async iterator
                    if (iterator is IAsyncEnumerator<object> asyncEnumerator)
                    {
                        try
                        {
                            if (await asyncEnumerator.MoveNextAsync())
                            {
                                prediction = asyncEnumerator.Current;
                                isGenerating = true;
                                newIterator = iterator;
                            }
                            else
                            {
                                // Iterator finished - return FINISHED_ITERATING sentinel
                                var nOutputs = GetOutputBlocks(blockFn).Count;
                                if (nOutputs == 0) nOutputs = 1;
                                prediction = nOutputs == 1
                                    ? "FINISHED_ITERATING"
                                    : Enumerable.Repeat("FINISHED_ITERATING", nOutputs).ToArray();
                                newIterator = null;
                                isGenerating = false;
                            }
                        }
                        catch (Exception)
                        {
                            // StopAsyncIteration equivalent
                            var nOutputs = GetOutputBlocks(blockFn).Count;
                            if (nOutputs == 0) nOutputs = 1;
                            prediction = nOutputs == 1
                                ? "FINISHED_ITERATING"
                                : Enumerable.Repeat("FINISHED_ITERATING", nOutputs).ToArray();
                            newIterator = null;
                            isGenerating = false;
                        }
                    }
                    else if (iterator is IEnumerator<object> enumerator)
                    {
                        try
                        {
                            if (enumerator.MoveNext())
                            {
                                prediction = enumerator.Current;
                                isGenerating = true;
                                newIterator = iterator;
                            }
                            else
                            {
                                var nOutputs = GetOutputBlocks(blockFn).Count;
                                if (nOutputs == 0) nOutputs = 1;
                                prediction = nOutputs == 1
                                    ? "FINISHED_ITERATING"
                                    : Enumerable.Repeat("FINISHED_ITERATING", nOutputs).ToArray();
                                newIterator = null;
                                isGenerating = false;
                            }
                        }
                        catch (Exception)
                        {
                            var nOutputs = GetOutputBlocks(blockFn).Count;
                            if (nOutputs == 0) nOutputs = 1;
                            prediction = nOutputs == 1
                                ? "FINISHED_ITERATING"
                                : Enumerable.Repeat("FINISHED_ITERATING", nOutputs).ToArray();
                            newIterator = null;
                            isGenerating = false;
                        }
                    }
                }
                else
                {
                    // Prepare function arguments with special parameter injection
                    var fnInputs = new List<object>(inputs);

                    object ConvertInputToParameterType(object rawValue, Type targetType)
                    {
                        var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                        if (rawValue == null)
                        {
                            if (Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
                            {
                                return null;
                            }

                            return Activator.CreateInstance(targetType);
                        }

                        if (nonNullableType.IsInstanceOfType(rawValue))
                        {
                            return rawValue;
                        }

                        // Convert enumerable payloads to strongly-typed List<T> / T[] parameters.
                        if (nonNullableType.IsGenericType && nonNullableType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var elementType = nonNullableType.GetGenericArguments()[0];

                            if (rawValue is System.Text.Json.JsonElement listElement &&
                                listElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                var typedList = (System.Collections.IList)Activator.CreateInstance(nonNullableType)!;
                                foreach (var item in listElement.EnumerateArray())
                                {
                                    typedList.Add(ConvertInputToParameterType(item, elementType));
                                }
                                return typedList;
                            }

                            if (rawValue is System.Collections.IEnumerable enumerableValue && rawValue is not string)
                            {
                                var typedList = (System.Collections.IList)Activator.CreateInstance(nonNullableType)!;
                                foreach (var item in enumerableValue)
                                {
                                    typedList.Add(ConvertInputToParameterType(item, elementType));
                                }
                                return typedList;
                            }
                        }

                        if (nonNullableType.IsArray)
                        {
                            var elementType = nonNullableType.GetElementType();
                            if (elementType != null)
                            {
                                if (rawValue is System.Text.Json.JsonElement arrElement &&
                                    arrElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    var arrValues = arrElement.EnumerateArray().ToList();
                                    var typedArray = Array.CreateInstance(elementType, arrValues.Count);
                                    for (int i = 0; i < arrValues.Count; i++)
                                    {
                                        typedArray.SetValue(ConvertInputToParameterType(arrValues[i], elementType), i);
                                    }
                                    return typedArray;
                                }

                                if (rawValue is System.Collections.IEnumerable enumerableValue && rawValue is not string)
                                {
                                    var values = new List<object?>();
                                    foreach (var item in enumerableValue)
                                    {
                                        values.Add(item);
                                    }

                                    var typedArray = Array.CreateInstance(elementType, values.Count);
                                    for (int i = 0; i < values.Count; i++)
                                    {
                                        typedArray.SetValue(ConvertInputToParameterType(values[i], elementType), i);
                                    }
                                    return typedArray;
                                }
                            }
                        }

                        // Convenience conversion for common Gradio payloads when user fn expects string paths.
                        if (nonNullableType == typeof(string))
                        {
                            if (rawValue is FileData fd && !string.IsNullOrWhiteSpace(fd.Path))
                            {
                                return fd.Path;
                            }

                            if (rawValue is IDictionary<string, object> dict &&
                                dict.TryGetValue("path", out var pathObj) &&
                                pathObj is string dictPath &&
                                !string.IsNullOrWhiteSpace(dictPath))
                            {
                                return dictPath;
                            }

                            if (rawValue is byte[] bytes)
                            {
                                try
                                {
                                    var tempPath = Path.Combine(Path.GetTempPath(), $"gradio_input_{Guid.NewGuid():N}.bin");
                                    File.WriteAllBytes(tempPath, bytes);
                                    return tempPath;
                                }
                                catch
                                {
                                    // Fall through to other conversions.
                                }
                            }
                        }

                        if (rawValue is System.Text.Json.JsonElement je)
                        {
                            try
                            {
                                if (nonNullableType == typeof(string))
                                {
                                    if (je.ValueKind == System.Text.Json.JsonValueKind.String)
                                        return je.GetString();
                                    // Gradio file payloads: extract path from {"path":"...", ...}
                                    if (je.ValueKind == System.Text.Json.JsonValueKind.Object &&
                                        je.TryGetProperty("path", out var pathProp))
                                        return pathProp.GetString();
                                    return je.ToString();
                                }

                                if (nonNullableType == typeof(bool))
                                {
                                    if (je.ValueKind == System.Text.Json.JsonValueKind.True ||
                                        je.ValueKind == System.Text.Json.JsonValueKind.False)
                                    {
                                        return je.GetBoolean();
                                    }

                                    var boolText = je.ToString();
                                    if (bool.TryParse(boolText, out var parsedBool))
                                    {
                                        return parsedBool;
                                    }
                                }

                                if (nonNullableType == typeof(int)) return je.GetInt32();
                                if (nonNullableType == typeof(long)) return je.GetInt64();
                                if (nonNullableType == typeof(float)) return je.GetSingle();
                                if (nonNullableType == typeof(double)) return je.GetDouble();
                                if (nonNullableType == typeof(decimal)) return je.GetDecimal();

                                if (nonNullableType.IsEnum)
                                {
                                    if (je.ValueKind == System.Text.Json.JsonValueKind.String)
                                    {
                                        var enumText = je.GetString();
                                        if (!string.IsNullOrEmpty(enumText))
                                        {
                                            return Enum.Parse(nonNullableType, enumText, ignoreCase: true);
                                        }
                                    }
                                    else if (je.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    {
                                        var enumInt = je.GetInt32();
                                        return Enum.ToObject(nonNullableType, enumInt);
                                    }
                                }

                                var deserialized = System.Text.Json.JsonSerializer.Deserialize(je.GetRawText(), nonNullableType);
                                if (deserialized != null)
                                {
                                    return deserialized;
                                }
                            }
                            catch
                            {
                                // Fallback to generic conversion below.
                            }
                        }

                        try
                        {
                            if (nonNullableType.IsEnum)
                            {
                                return Enum.Parse(nonNullableType, rawValue.ToString() ?? string.Empty, ignoreCase: true);
                            }

                            return Convert.ChangeType(rawValue, nonNullableType, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return rawValue;
                        }
                    }

                    // Get the function's parameter information
                    var methodInfo = blockFn.Fn.Method;
                    var parameters = methodInfo.GetParameters();
                    int inputIndex = 0;
                    var finalArgs = new List<object>();

                    // Check if inputs should be passed as dictionary
                    // Python parity: dict(zip(block_fn.inputs, processed_input))
                    // Keys are the actual Block/Component objects, values are preprocessed inputs.
                    // NOTE: blockFn.Inputs is stored as List<object> (not List<Block>), so we cannot
                    // use 'is List<Block>' — we must cast each element to Block individually.
                    var inputBlocksFromObj = (blockFn.InputsAsDict && blockFn.Inputs is List<object> rawInputsList)
                        ? rawInputsList.Select(x => x as Block).ToList()
                        : null;
                    if (blockFn.InputsAsDict && inputBlocksFromObj != null)
                    {
                        var inputBlocks = inputBlocksFromObj;
                        var inputDict = new Dictionary<object, object?>();
                        for (int i = 0; i < inputBlocks.Count && i < inputs.Count; i++)
                        {
                            var blk = inputBlocks[i];
                            var rawVal = inputs[i];

                            // Convert raw JsonElement to a usable CLR type before preprocessing
                            object preprocessedVal = rawVal;
                            if (rawVal is System.Text.Json.JsonElement je)
                            {
                                preprocessedVal = ConvertJsonElementToObject(je);
                            }

                            // Call component's Preprocess (e.g. Audio returns filepath string, Slider returns double)
                            if (blk is Gradio.Net.Components.Component compBlock)
                            {
                                try
                                {
                                    preprocessedVal = compBlock.Preprocess(preprocessedVal);
                                }
                                catch (Exception ppEx)
                                {
                                }
                            }

                            inputDict[blk] = preprocessedVal;
                        }
                        fnInputs = new List<object> { inputDict };
                    }

                    // Match parameters with inputs and inject special parameters
                    object ResolveRequestArgument(Type parameterType, object rawRequest)
                    {
                        var targetType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;

                        if (targetType == typeof(Request))
                        {
                            if (rawRequest is Request grRequest)
                            {
                                return grRequest;
                            }

                            if (rawRequest is List<Request> requestListFromBatch)
                            {
                                return requestListFromBatch.FirstOrDefault() ?? new Request();
                            }

                            if (rawRequest is Microsoft.AspNetCore.Http.HttpRequest httpRequest)
                            {
                                return new Request(httpRequest);
                            }

                            return new Request();
                        }

                        if (targetType == typeof(Microsoft.AspNetCore.Http.HttpRequest))
                        {
                            if (rawRequest is Microsoft.AspNetCore.Http.HttpRequest httpRequest)
                            {
                                return httpRequest;
                            }

                            if (rawRequest is List<Request> requestListFromBatch)
                            {
                                return requestListFromBatch.FirstOrDefault()?.HttpRequest;
                            }

                            if (rawRequest is Request grRequest)
                            {
                                return grRequest.HttpRequest;
                            }

                            return null;
                        }

                        if (targetType.IsGenericType &&
                            targetType.GetGenericTypeDefinition() == typeof(List<>) &&
                            targetType.GetGenericArguments()[0] == typeof(Request))
                        {
                            if (rawRequest is List<Request> requestList)
                            {
                                return requestList;
                            }

                            if (rawRequest is Request grRequest)
                            {
                                return new List<Request> { grRequest };
                            }

                            if (rawRequest is Microsoft.AspNetCore.Http.HttpRequest httpRequest)
                            {
                                return new List<Request> { new Request(httpRequest) };
                            }

                            return new List<Request> { new Request() };
                        }

                        return rawRequest ?? new Request();
                    }

                    foreach (var param in parameters)
                    {
                        var paramType = param.ParameterType;
                        var paramName = param.Name?.ToLower() ?? "";

                        // Check for special parameter types
                        if (paramType.Name.Contains("Request") || paramName == "request")
                        {
                            // Inject Request parameter
                            finalArgs.Add(ResolveRequestArgument(paramType, request));
                        }
                        else if (typeof(Events.EventData).IsAssignableFrom(paramType) || paramType.Name.Contains("EventData") || paramName == "eventdata")
                        {
                            // Inject EventData parameter (supports base EventData and subclasses like SelectData)
                            if (blockFn.CollectsEventData && eventData != null)
                            {
                                finalArgs.Add(eventData);
                            }
                            else
                            {
                                finalArgs.Add(null);
                            }
                        }
                        else if (paramType.Name.Contains("Progress") || paramName == "progress")
                        {
                            // Inject Progress tracker (Python parity)
                            // The tracker reports progress via LocalContext (queue integration may be limited).
                            finalArgs.Add(new Progress(trackTqdm: false));
                        }
                        else if (param.GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false).Any())
                        {
                            // Support C# `params` (ParamArray) arguments: pack remaining inputs.
                            // Python parity: functions may accept variable-length args.
                            var elementType = paramType.IsArray ? paramType.GetElementType() : null;
                            if (elementType == null)
                            {
                                // Fallback: treat as single argument.
                                if (inputIndex < fnInputs.Count)
                                {
                                    finalArgs.Add(fnInputs[inputIndex++]);
                                }
                                else
                                {
                                    finalArgs.Add(Array.Empty<object>());
                                }
                            }
                            else
                            {
                                var remaining = fnInputs.Skip(inputIndex).ToList();
                                inputIndex = fnInputs.Count;

                                // If the remaining inputs already contain a single array of the correct type, preserve it.
                                if (remaining.Count == 1 && remaining[0] != null && paramType.IsInstanceOfType(remaining[0]))
                                {
                                    finalArgs.Add(remaining[0]);
                                }
                                else
                                {
                                    var packed = Array.CreateInstance(elementType, remaining.Count);
                                    for (int i = 0; i < remaining.Count; i++)
                                    {
                                        packed.SetValue(ConvertInputToParameterType(remaining[i], elementType), i);
                                    }
                                    finalArgs.Add(packed);
                                }
                            }
                        }
                        else if (inputIndex < fnInputs.Count)
                        {
                            // Regular input parameter
                            var rawInput = fnInputs[inputIndex++];
                            finalArgs.Add(ConvertInputToParameterType(rawInput, paramType));
                        }
                    }

                    // Call the function
                    var result = blockFn.Fn.DynamicInvoke(finalArgs.ToArray());

                    // Handle async results
                    if (result is Task task)
                    {
                        await task;
                        var resultProperty = task.GetType().GetProperty("Result");
                        prediction = resultProperty?.GetValue(task);
                    }
                    else
                    {
                        prediction = result;
                    }

                    // Check if result is an async generator (IAsyncEnumerable<object>) - must be checked FIRST
                    // because IAsyncEnumerable<T> does NOT implement System.Collections.IEnumerable.
                    // Use direct is-cast (not reflection) so explicit interface implementations are found.
                    if (prediction is IAsyncEnumerable<object> asyncEnumerable)
                    {
                        var asyncEnum = asyncEnumerable.GetAsyncEnumerator();
                        if (await asyncEnum.MoveNextAsync())
                        {
                            prediction = asyncEnum.Current;
                            isGenerating = true;
                            newIterator = asyncEnum;
                        }
                        else
                        {
                            // Empty async enumerable
                            prediction = null;
                            newIterator = null;
                        }
                    }
                    // Check if result is a sync generator/iterator (IEnumerable but not string/byte[]/list/dict)
                    else if (prediction != null &&
                        prediction is System.Collections.IEnumerable enumerable &&
                        !(prediction is string) &&
                        !(prediction is byte[]) &&
                        !(prediction is System.Collections.IList) &&
                        !(prediction is System.Collections.IDictionary))
                    {
                        // Regular enumerable - create enumerator
                        var enumerator = enumerable.GetEnumerator();
                        if (enumerator != null && enumerator.MoveNext())
                        {
                            prediction = enumerator.Current;
                            isGenerating = true;
                            newIterator = enumerator;
                        }
                        else
                        {
                            // Empty enumerable - materialize to list
                            var list = new List<object>();
                            foreach (var item in enumerable)
                            {
                                list.Add(item);
                            }
                            prediction = list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Unwrap reflection/aggregate wrappers
                var innerEx = ex;
                while (innerEx.InnerException != null &&
                       (innerEx is System.Reflection.TargetInvocationException ||
                        innerEx is AggregateException))
                {
                    innerEx = innerEx.InnerException;
                }

                // Re-throw gr.Error directly so the queue can surface duration/visible/title
                if (innerEx is Gradio.Net.Core.Exceptions.Error)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(innerEx).Throw();

                throw new InvalidOperationException(
                    $"Error in function execution: {innerEx.Message}",
                    innerEx);
            }

            var duration = (DateTime.UtcNow - startTime).TotalSeconds;

            return new Dictionary<string, object>
            {
                ["prediction"] = prediction,
                ["is_generating"] = isGenerating,
                ["iterator"] = newIterator,
                ["duration"] = duration
            };
        }

        public List<object> SerializeData(int fnIndex, List<object> inputs)
        {
            if (!Fns.ContainsKey(fnIndex))
                throw new ArgumentException($"Function index {fnIndex} not found");

            var blockFn = Fns[fnIndex];
            var inputsList = blockFn.Inputs as List<Block>;
            if (inputsList == null)
                return inputs;

            var processedInput = new List<object>();

            object FormatFile(object s)
            {
                if (s is string path)
                {
                    return new FileData { Path = path }.ToDictionary();
                }
                return s;
            }

            for (int i = 0; i < inputs.Count && i < inputsList.Count; i++)
            {
                var block = inputsList[i];
                var input = inputs[i];

                if (!(block is Components.Component component))
                {
                    throw new InvalidComponentError(
                        $"{block.GetType()} Component not a valid input component.");
                }

                var apiInfo = component.ApiInfo();

                // Check if this component expects file input
                bool isFileType = FileData.ValueIsFile(apiInfo);

                if (isFileType)
                {
                    // Traverse the input structure and format file paths
                    var serializedInput = TraverseHelper.Traverse(
                        input,
                        FormatFile,
                        obj => FileData.IsFilePath(obj) || FileData.IsHttpUrl(obj)
                    );
                    processedInput.Add(serializedInput);
                }
                else
                {
                    processedInput.Add(input);
                }
            }

            return processedInput;
        }

        public List<object> DeserializeData(int fnIndex, List<object> outputs)
        {
            if (!Fns.ContainsKey(fnIndex))
                throw new ArgumentException($"Function index {fnIndex} not found");

            var blockFn = Fns[fnIndex];
            var outputsList = blockFn.Outputs as List<Block>;
            if (outputsList == null)
                return outputs;

            var predictions = new List<object>();

            object ExtractPath(object s)
            {
                if (s is Dictionary<string, object> dict && dict.ContainsKey("path"))
                {
                    return dict["path"];
                }
                return s;
            }

            for (int i = 0; i < outputs.Count && i < outputsList.Count; i++)
            {
                var block = outputsList[i];
                var output = outputs[i];

                if (!(block is Components.Component component))
                {
                    throw new InvalidComponentError(
                        $"{block.GetType()} Component not a valid output component.");
                }

                // Traverse the output structure and extract file paths
                var deserialized = TraverseHelper.Traverse(
                    output,
                    ExtractPath,
                    FileData.IsFileObj
                );

                predictions.Add(deserialized);
            }

            return predictions;
        }

        public bool IsCallable(int fnIndex)
        {
            if (!Fns.ContainsKey(fnIndex))
                return false;

            var blockFn = Fns[fnIndex];

            // Function must exist
            if (blockFn.Fn == null)
                return false;

            // Check if function is a generator (async or sync)
            // In C#, this would be Task<IAsyncEnumerable<T>> or IEnumerable<T> return types
            var returnType = blockFn.Fn.Method.ReturnType;
            if (returnType.IsGenericType)
            {
                var genericDef = returnType.GetGenericTypeDefinition();
                if (genericDef == typeof(IAsyncEnumerable<>) ||
                    genericDef == typeof(IEnumerable<>))
                {
                    return false;
                }
            }

            // Check if any input blocks are stateful
            var inputBlocks = GetInputBlocks(blockFn);
            if (inputBlocks.Any(block => block.Stateful))
                return false;

            // Check if any output blocks are stateful
            var outputBlocks = GetOutputBlocks(blockFn);
            if (outputBlocks.Any(block => block.Stateful))
                return false;

            return true;
        }

        public void Render()
        {
            var rootBlock = Context.RootBlock;

            // Python parity: merge blocks and functions into the current root context.
            if (rootBlock != null)
            {
                // Compute dependency ID offset to avoid collisions.
                var dependencyOffset = rootBlock.DefaultConfig.Fns.Count > 0
                    ? rootBlock.DefaultConfig.Fns.Keys.Max() + 1
                    : 0;

                // Merge child blocks into the root context's blocks dict.
                foreach (var kvp in DefaultConfig.Blocks)
                {
                    if (!rootBlock.DefaultConfig.Blocks.ContainsKey(kvp.Key))
                    {
                        kvp.Value.Page = rootBlock.CurrentPage;
                        rootBlock.DefaultConfig.Blocks[kvp.Key] = kvp.Value;
                        if (kvp.Value.TempFiles != null)
                            rootBlock.TempFileSets.Add(kvp.Value.TempFiles);
                    }
                }

                // Merge functions into root context with offset so IDs don't collide.
                // Python parity: when rendering, api_names are uniquified against the global (root) fn set.
                foreach (var kvp in DefaultConfig.Fns)
                {
                    var fn = kvp.Value;
                    fn.Page = rootBlock.CurrentPage;

                    var newId = kvp.Key + dependencyOffset;
                    fn.Id = newId;

                    // Remap trigger_after and cancels with the offset.
                    if (fn.TriggerAfter.HasValue)
                        fn.TriggerAfter = fn.TriggerAfter.Value + dependencyOffset;
                    if (fn.Cancels != null && fn.Cancels.Count > 0)
                        fn.Cancels = fn.Cancels.Select(c => c + dependencyOffset).ToList();

                    // Remap any target that pointed to this Blocks's own _id → root_block._id.
                    for (int i = 0; i < fn.Targets.Count; i++)
                    {
                        var (blockId, evtName) = fn.Targets[i];
                        if (blockId == _id)
                            fn.Targets[i] = (rootBlock._id, evtName);
                    }

                    // Python parity: re-uniquify api_name against the root context's already-merged fns.
                    // Use BaseApiName (the pre-local-suffix name) so that suffixed local names
                    // (e.g. 'js_fn_1') are re-uniquified from the base ('js_fn') against the global pool,
                    // yielding 'js_fn_3' rather than 'js_fn_1_1'.
                    if (!string.IsNullOrEmpty(fn.ApiName))
                    {
                        var existingApiNames = rootBlock.DefaultConfig.Fns.Values
                            .Where(f => !string.IsNullOrEmpty(f.ApiName))
                            .Select(f => f.ApiName)
                            .ToList();
                        var baseName = fn.BaseApiName ?? fn.ApiName;
                        fn.ApiName = rootBlock.DefaultConfig.MakeApiNameUniquePublic(baseName, existingApiNames);
                    }

                    rootBlock.DefaultConfig.Fns[newId] = fn;
                }

                // Merge proxy URLs and temp file sets.
                foreach (var url in ProxyUrls)
                    rootBlock.ProxyUrls.Add(url);
                // TempFileSets[0] is the UploadFileSet of this Blocks; the rest are component-level sets.
                for (int i = 1; i < TempFileSets.Count; i++)
                    rootBlock.TempFileSets.Add(TempFileSets[i]);
            }

            // Python parity: render_context.children.extend(self.children)
            // Extend the render context's children with this Blocks's top-level children (NOT self).
            var renderContext = BlockContext.GetRenderContext();
            if (renderContext != null)
            {
                foreach (var child in Children)
                {
                    renderContext.Children.Add(child);
                }
            }
        }

        public BlockFunction Unload(Delegate? fn)
        {
            return new BlocksEvents(this).Unload(fn);
        }

        public BlockFunction Load(
            Delegate? fn,
            object? inputs = null,
            object? outputs = null,
            bool preprocess = true,
            bool postprocess = true)
        {
            return new BlocksEvents(this).Load(fn, inputs, outputs, preprocess, postprocess);
        }

        public bool ExpectsOauth()
        {
            // Check if any block is a LoginButton component
            // Iterate through all blocks and check their type
            foreach (var block in this.DefaultConfig.Blocks.Values)
            {
                var blockType = block.GetType().Name;
                if (blockType == "LoginButton" || blockType.Contains("LoginButton"))
                {
                    return true;
                }
            }

            // Also check in nested blocks
            foreach (var block in this.DefaultConfig.Blocks.Values)
            {
                if (block is BlockContext context)
                {
                    // Recursively check children in block contexts
                    var hasLoginButton = CheckForLoginButton(context);
                    if (hasLoginButton)
                        return true;
                }
            }

            return false;
        }

        private static bool IsGradioDisableAnalytics()
        {
            var envValue = Environment.GetEnvironmentVariable("GRADIO_ANALYTICS_ENABLED");
            return envValue != null && (envValue == "0" || envValue.ToLower() == "false");
        }

        public void CreateLimiter()
        {
            Limiter = MaxThreads == 40
                ? null
                : new SemaphoreSlim(MaxThreads, MaxThreads);
        }

        private bool CheckForLoginButton(BlockContext context)
        {
            // Check all children in the context
            var childrenProp = context.GetType().GetProperty("Children");
            if (childrenProp != null)
            {
                var children = childrenProp.GetValue(context) as System.Collections.IEnumerable;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (child is Block childBlock)
                        {
                            var blockType = childBlock.GetType().Name;
                            if (blockType == "LoginButton" || blockType.Contains("LoginButton"))
                            {
                                return true;
                            }

                            if (childBlock is BlockContext childContext)
                            {
                                if (CheckForLoginButton(childContext))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public double TotalRuntime { get; set; }

        public int TotalRuns { get; set; }

        public double AverageDuration => TotalRuns > 0 ? TotalRuntime / TotalRuns : 0;

        // Streaming methods are implemented above in HandleStreamingOutputs and HandleStreamingDiffs
        // Additional streaming utilities can be added here as needed
        // HandleStreamingOutputs, HandleStreamingDiffs
        // Need to define proper type for PendingStreams/PendingDiffStreams

        public string GetDetailedInfo()
        {
            var backendFns = Fns.Values.Where(f => f.Fn != null).ToList();
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"Gradio Blocks instance: {backendFns.Count} backend functions");
            sb.AppendLine(new string('-', sb.Length - 1));

            foreach (var kvp in Fns)
            {
                var fn = kvp.Value;
                if (fn.Fn != null)
                {
                    sb.AppendLine($"fn_index={kvp.Key}");
                    sb.AppendLine(" inputs:");

                    var inputsList = fn.Inputs as List<Block>;
                    if (inputsList != null)
                    {
                        foreach (var block in inputsList)
                        {
                            if (BlocksDict.ContainsKey(block._id))
                            {
                                sb.AppendLine($" |-{BlocksDict[block._id]}");
                            }
                        }
                    }

                    sb.AppendLine(" outputs:");
                    var outputsList = fn.Outputs as List<Block>;
                    if (outputsList != null)
                    {
                        foreach (var block in outputsList)
                        {
                            if (BlocksDict.ContainsKey(block._id))
                            {
                                sb.AppendLine($" |-{BlocksDict[block._id]}");
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public static object Integrate(
            object app,
            string path,
            object auth = null,
            Blocks blocks = null)
        {
            // Integration functionality for analytics and monitoring
            // In a full implementation, this would send telemetry data
            // to Gradio analytics services if enabled
            return app;
        }

        public void Integrate(object cometMl = null, object wandb = null, object mlflow = null)
        {
            string analyticsIntegration = "";

            if (cometMl != null)
            {
                analyticsIntegration = "CometML";
                InvokeRequired(cometMl, "log_other", "LogOther", new object[] { "Created from", "Gradio" });

                var url = !string.IsNullOrWhiteSpace(ShareUrl) ? ShareUrl : LocalUrl;
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new InvalidOperationException("Please run `Launch()` first.");
                }

                InvokeRequired(cometMl, "log_text", "LogText", new object[] { $"gradio: {url}" });
                InvokeRequired(cometMl, "end", "End", Array.Empty<object>());
            }

            if (wandb != null)
            {
                // Python asserts `wandb.log` and `wandb.Html`
                EnsureHasMethod(wandb, new[] { "log", "Log" }, 1);

                analyticsIntegration = "WandB";
                if (!string.IsNullOrWhiteSpace(ShareUrl))
                {
                    var w = Width ?? 800;
                    var h = Height ?? 600;
                    var iframe = $"<iframe src=\"{ShareUrl}\" width=\"{w}\" height=\"{h}\" frameBorder=\"0\"></iframe>";

                    object htmlObj = TryCreateWandbHtml(wandb, iframe) ?? iframe;
                    var payload = new Dictionary<string, object> { { "Gradio panel", htmlObj } };

                    InvokeRequired(wandb, "log", "Log", new object[] { payload });
                }
                else
                {
                }
            }

            if (mlflow != null)
            {
                // Python asserts `mlflow.log_param`
                analyticsIntegration = "MLFlow";

                var url = !string.IsNullOrWhiteSpace(ShareUrl) ? ShareUrl : LocalUrl;
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new InvalidOperationException("Please run `Launch()` first.");
                }

                var key = !string.IsNullOrWhiteSpace(ShareUrl)
                    ? "Gradio Interface Share Link"
                    : "Gradio Interface Local Link";

                InvokeRequired(mlflow, "log_param", "LogParam", new object[] { key, url });
            }

            if (AnalyticsEnabled && !string.IsNullOrWhiteSpace(analyticsIntegration))
            {
                Analytics.IntegrationAnalytics(new Dictionary<string, object>
                {
                    { "integration", analyticsIntegration }
                });
            }
        }

        private static object TryCreateWandbHtml(object wandb, string html)
        {
            // Python uses `wandb.Html(...)` (a class). In .NET we use best-effort reflection:
            // 1) method Html(string)
            // 2) type named Html with ctor(string)
            var t = wandb.GetType();
            var htmlMethod = FindMethod(t, new[] { "Html" }, 1);
            if (htmlMethod != null)
            {
                return htmlMethod.Invoke(wandb, new object[] { html });
            }

            var nestedHtmlType = t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(nt => string.Equals(nt.Name, "Html", StringComparison.OrdinalIgnoreCase));
            if (nestedHtmlType != null)
            {
                var ctor = nestedHtmlType.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(string));
                if (ctor != null)
                {
                    return ctor.Invoke(new object[] { html });
                }
            }

            return null;
        }

        private static void EnsureHasMethod(object target, IEnumerable<string> names, int paramCount)
        {
            var t = target.GetType();
            if (FindMethod(t, names, paramCount) == null)
            {
                throw new ArgumentException(
                    $"Object of type '{t.FullName}' is missing required method ({string.Join("/", names)}) with {paramCount} parameter(s).",
                    nameof(target));
            }
        }

        private static void InvokeRequired(object target, string snakeCaseName, string pascalCaseName, object[] args)
        {
            var t = target.GetType();
            var method = FindMethod(t, new[] { snakeCaseName, pascalCaseName }, args.Length);
            if (method == null)
            {
                throw new ArgumentException(
                    $"Object of type '{t.FullName}' is missing required method '{snakeCaseName}'/'{pascalCaseName}'.",
                    nameof(target));
            }
            method.Invoke(target, args);
        }

        private static MethodInfo FindMethod(Type t, IEnumerable<string> names, int paramCount)
        {
            foreach (var name in names)
            {
                // Search public instance methods first, then allow non-public to support dynamic proxies.
                var method = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(m =>
                        string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)
                        && m.GetParameters().Length == paramCount);
                if (method != null)
                {
                    return method;
                }
            }
            return null;
        }

        public static Blocks Load(
            string name,
            string src = null,
            string apiKey = null,
            string alias = null,
            params object[] kwargs)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name parameter is required", nameof(name));
            }

            // Validate source type
            if (!string.IsNullOrWhiteSpace(src))
            {
                var validSources = new[] { "huggingface", "models", "spaces" };
                if (!validSources.Contains(src.ToLower()))
                {
                    throw new ArgumentException(
                        $"Invalid source: {src}. Valid sources are: {string.Join(", ", validSources)}",
                        nameof(src));
                }
            }

            // For now, create an empty Blocks instance with metadata
            // Full external loading implementation would require:
            var blocks = new Blocks(
                analyticsEnabled: false,
                mode: "interface",
                title: alias ?? name);

            // Full implementation steps:
            // 1. Use HttpClient to fetch config from HuggingFace Spaces/Models API
            //    - Endpoint format: https://huggingface.co/api/{src}/{name}
            // 2. Parse the returned JSON configuration
            // 3. Deserialize component tree from config
            // 4. Reconstruct Blocks structure using FromConfig
            // 5. Set up API proxies to forward requests to remote endpoints
            // 6. Handle authentication if required (HF tokens, etc.)

            return blocks;
        }

        public Blocks ShallowCopy()
        {
            return (Blocks)MemberwiseClone();
        }

        public Blocks DeepCopy()
        {
            // Best-effort deep copy: clone mutable collections and config containers.
            // Note: Block/component instances are not deep-copied.
            var copy = ShallowCopy();

            copy.AllowedPaths = AllowedPaths != null ? new List<string>(AllowedPaths) : new List<string>();
            copy.BlockedPaths = BlockedPaths != null ? new List<string>(BlockedPaths) : new List<string>();
            copy.ProxyUrls = ProxyUrls != null ? new HashSet<string>(ProxyUrls, StringComparer.OrdinalIgnoreCase) : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            copy.Pages = Pages != null ? new List<Tuple<string, string, bool>>(Pages) : new List<Tuple<string, string, bool>>();
            copy.TempFileSets = TempFileSets != null ? new List<HashSet<string>>(TempFileSets.Select(s => new HashSet<string>(s))) : new List<HashSet<string>>();
            copy.UploadFileSet = UploadFileSet != null ? new HashSet<string>(UploadFileSet) : new HashSet<string>();

            copy.Config = Config != null ? Gradio.Net.Utils.Utils.SafeDeepCopy(Config) : new Dictionary<string, object>();
            copy.DefaultConfig = DefaultConfig != null ? DefaultConfig.ShallowCopy() : new BlocksConfig(copy);

            return copy;
        }

        private object ServerApp { get; set; }

        private WebApplication HostedWebApplication { get; set; }

        private IHost HostedHost { get; set; }

        public void SetServerApp(object app)
        {
            ServerApp = app;
        }

        public object GetServerApp()
        {
            return ServerApp;
        }

        public List<object> GetLoadEvents()
        {
            var loadEvents = new List<object>();

            foreach (var block in DefaultConfig.Blocks.Values)
            {
                if (block is Components.Component component && component.LoadEvent != null)
                {
                    loadEvents.Add(new Dictionary<string, object>
                    {
                        ["id"] = block._id,
                        ["load_event"] = component.LoadEvent
                    });
                }
            }

            return loadEvents;
        }

        public async Task<List<List<object>>> BatchPredict(
            List<List<object>> inputs,
            string apiName = null)
        {
            var results = new List<List<object>>();

            // Get the first function or find by api_name
            var blockFn = DefaultConfig.Fns.Values.FirstOrDefault();
            if (blockFn == null)
            {
                throw new InvalidOperationException("No functions available for batch prediction");
            }

            foreach (var inputBatch in inputs)
            {
                var result = await ProcessApi(
                    blockFn: blockFn,
                    inputs: inputBatch,
                    state: new Dictionary<int, Block>(),
                    request: null
                );

                var outputsList = new List<object>();
                if (result != null && result.ContainsKey("data"))
                {
                    outputsList = (result["data"] as List<object>) ?? new List<object>();
                }
                results.Add(outputsList);
            }

            return results;
        }

        public Dictionary<string, object> UpdateComponentProp(
            int blockId,
            string propName,
            object value)
        {
            if (!BlocksDict.TryGetValue(blockId, out var block))
            {
                throw new KeyNotFoundException($"Block with id {blockId} not found");
            }

            return new Dictionary<string, object>
            {
                ["__type__"] = "update",
                [propName] = value
            };
        }

        public object ConvertToGenerator(object value)
        {
            if (value is IEnumerable<object> enumerable && !(value is string))
            {
                return enumerable;
            }
            return new[] { value };
        }

        public Block GetFirstBlock()
        {
            return Children.Count > 0 ? Children[0] : null;
        }

        public List<Block> GetCurrentRenderables()
        {
            var renderables = new List<Block>();
            foreach (var block in BlocksDict.Values)
            {
                if (block.Visible)
                {
                    renderables.Add(block);
                }
            }
            return renderables;
        }

        public bool IsCallable()
        {
            return Fns.Count > 0 && Fns.Values.Any(fn => fn.Fn != null);
        }

        public List<object> ValidateAndReorderInputs(
            BlockFunction blockFn,
            List<object> inputs)
        {
            // Validate that we have the right number of inputs
            // In a production system, you might want more sophisticated validation
            return inputs;
        }

        private void ValidateOutputs(BlockFunction blockFn, List<object> predictions)
        {
            var depOutputs = GetOutputBlocks(blockFn);

            if (depOutputs == null)
            {
                return;
            }

            predictions ??= new List<object>();

            // Normalize scalar predictions to a list (Python wraps non-list/tuple)
            // (callers already provide a list, but keep safe)

            if (predictions.Count != depOutputs.Count)
            {
                var fnName = blockFn.Fn?.Method?.Name;
                var name = !string.IsNullOrWhiteSpace(fnName) && fnName != "<lambda>"
                    ? $" ({fnName})"
                    : string.Empty;

                var wantedArgs = depOutputs.Select(b => b?.GetBlockClass() ?? "null").ToList();
                var receivedArgs = predictions.Select(p => p is string s ? $"\"{s}\"" : (p?.ToString() ?? "null")).ToList();

                var wanted = string.Join(", ", wantedArgs);
                var received = string.Join(", ", receivedArgs);

                if (predictions.Count < depOutputs.Count)
                {
                    throw new InvalidOperationException(
                        $"A function{name} didn't return enough output values (needed: {depOutputs.Count}, returned: {predictions.Count}).\n" +
                        $"    Output components:\n        [{wanted}]\n" +
                        $"    Output values returned:\n        [{received}]");
                }
                else
                {
                    // Python special case: do not warn/error if only output is None
                    if (predictions.Count == 1 && predictions[0] == null)
                    {
                        return;
                    }

                    // Warn and ignore extra values
                }
            }
        }

        public void RunStartupEvents()
        {
            // Python parity: Blocks.run_startup_events()
            // - Start queue (best-effort: via ServerApp.QueueManager)
            // - Mark running
            // - Create limiter

            try
            {
                var app = GetServerApp();
                if (app != null)
                {
                    // Best-effort reflection: App.QueueManager.Start() if stopped
                    var qmProp = app.GetType().GetProperty("QueueManager");
                    var qm = qmProp?.GetValue(app);
                    if (qm != null)
                    {
                        var stoppedProp = qm.GetType().GetProperty("Stopped");
                        var stopped = stoppedProp != null && stoppedProp.PropertyType == typeof(bool)
                            ? (bool)stoppedProp.GetValue(qm)!
                            : false;

                        if (stopped)
                        {
                            var startMethod = qm.GetType().GetMethod("Start");
                            startMethod?.Invoke(qm, null);
                        }
                    }
                }
            }
            catch
            {
                // Ignore startup queue errors; queue may already be running.
            }

            IsRunning = true;
            CreateLimiter();

            // Also trigger any optional component startup hooks (non-Python standard; best-effort)
            foreach (var kvp in BlocksDict)
            {
                var blockId = kvp.Key;
                var block = kvp.Value;

                var startupMethod = block.GetType().GetMethod("OnStartup");
                if (startupMethod != null && startupMethod.GetParameters().Length == 0)
                {
                    try
                    {
                        startupMethod.Invoke(block, null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public async Task RunExtraStartupEvents()
        {
            if (ExtraStartupEvents == null)
            {
                return;
            }

            foreach (var startupEvent in ExtraStartupEvents)
            {
                if (startupEvent == null)
                {
                    continue;
                }
                await startupEvent();
            }
        }

        public void BlockThread(int pollMs = 100)
        {
            try
            {
                while (IsRunning)
                {
                    Thread.Sleep(pollMs);
                }
            }
            catch
            {
                try
                {
                    Close();
                }
                catch
                {
                    // Ignore
                }
            }
        }

        public void TranspileToJs(bool quiet = false)
        {
            var toTranspile = Fns.Values
                .Where(f => f.Fn != null && f.Js != null && string.Equals(f.Js, "true", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!quiet && toTranspile.Count > 0)
            {
            }

            foreach (var fn in toTranspile)
            {
                // Mark that we attempted transpilation.
                fn.JsImplementation ??= "// transpilation not available in .NET";
            }

            if (!quiet && toTranspile.Count > 0)
            {
            }
        }

        public MediaStream GetPendingStream(string sessionHash, int run, int componentId)
        {
            if (!PendingStreams.TryGetValue(sessionHash, out var runDict))
            {
                return null;
            }

            var runKey = run.ToString();
            if (runDict == null || !runDict.TryGetValue(runKey, out var componentDict))
            {
                return null;
            }

            if (componentDict is Dictionary<string, object> dict &&
                dict.TryGetValue(componentId.ToString(), out var streamObj) &&
                streamObj is MediaStream stream)
            {
                return stream;
            }

            return null;
        }

        public string GetMergedStream(string sessionHash, string extension)
        {
            // Check if merged stream file exists
            var tempDir = Path.GetTempPath();
            var streamDir = Path.Combine(tempDir, "gradio", "streams", sessionHash);

            if (!Directory.Exists(streamDir))
            {
                return null;
            }

            var mergedFile = Path.Combine(streamDir, $"merged.{extension}");
            return File.Exists(mergedFile) ? mergedFile : null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Gradio.Net.Events;
using Gradio.Net.Utils;

namespace Gradio.Net.Core
{
    public class BlocksConfig
    {
        public Blocks RootBlock { get; set; }
        public Dictionary<int, Block> Blocks { get; set; }
        public Dictionary<int, BlockFunction> Fns { get; set; }
        public int FnId { get; set; }

        public BlocksConfig(Blocks rootBlock)
        {
            RootBlock = rootBlock;
            Blocks = new Dictionary<int, Block>();
            Fns = new Dictionary<int, BlockFunction>();
            FnId = 0;
        }

        public (BlockFunction, int) SetEventTrigger(
            List<EventListenerMethod> targets,
            Delegate fn,
            object inputs,
            object outputs,
            bool preprocess = true,
            bool postprocess = true,
            bool scrollToOutput = false,
            string showProgress = "full",
            object showProgressOn = null,
            string apiName = null,
            object apiDescription = null,
            string js = null,
            bool noTarget = false,
            bool queue = true,
            bool batch = false,
            int maxBatchSize = 4,
            List<int> cancels = null,
            bool? collectsEventData = null,
            int? triggerAfter = null,
            bool triggerOnlyOnSuccess = false,
            bool triggerOnlyOnFailure = false,
            string triggerMode = "once",
            object concurrencyLimit = null,
            string concurrencyId = null,
            string apiVisibility = "public",
            object renderable = null,
            bool isCancelFunction = false,
            string connection = "sse",
            float? timeLimit = null,
            float streamEvery = 0.5f,
            List<string> eventSpecificArgs = null,
            string jsImplementation = null,
            object key = null,
            Delegate validator = null,
            List<int> componentPropInputs = null)
        {
            // Process targets
            var targetsList = new List<(int? blockId, string eventName)>();
            foreach (var target in targets ?? new List<EventListenerMethod>())
            {
                var blockId = (noTarget || target.Block == null) ? (int?)null : (target.Block as Block)?._id;
                targetsList.Add((blockId, target.EventName));
            }

            // Process inputs
            bool inputsAsDict = false;
            List<object> inputsList;

            if (inputs is HashSet<object> inputsSet)
            {
                inputsAsDict = true;
                inputsList = inputsSet.OrderBy(x => (x as Block)?._id ?? 0).ToList();
            }
            else if (inputs != null && inputs.GetType().IsGenericType && inputs.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                // Handle HashSet<T> where T is a subtype of object (e.g. HashSet<Component>)
                // C# generics are invariant, so HashSet<Component> does NOT satisfy "is HashSet<object>"
                inputsAsDict = true;
                inputsList = ((System.Collections.IEnumerable)inputs)
                    .Cast<object>()
                    .OrderBy(x => (x as Block)?._id ?? 0)
                    .ToList();
            }
            else if (inputs == null)
            {
                inputsList = new List<object>();
            }
            else if (inputs is IEnumerable<object> enumerable && !(inputs is string))
            {
                inputsList = enumerable.ToList();
            }
            else
            {
                inputsList = new List<object> { inputs };
            }

            // Process outputs
            List<object> outputsList;

            if (outputs is HashSet<object> outputsSet)
            {
                outputsList = outputsSet.OrderBy(x => (x as Block)?._id ?? 0).ToList();
            }
            else if (outputs == null)
            {
                outputsList = new List<object>();
            }
            else if (outputs is IEnumerable<object> enumerable && !(outputs is string))
            {
                outputsList = enumerable.ToList();
            }
            else
            {
                outputsList = new List<object> { outputs };
            }

            // Process show_progress_on
            if (showProgressOn != null && !(showProgressOn is IEnumerable<object>))
            {
                showProgressOn = new List<object> { showProgressOn };
            }

            // Validate function inputs match if provided
            if (fn != null && (cancels == null || cancels.Count == 0))
            {
                // Check that the number of inputs matches the function parameters
                // This is a basic validation - full implementation would need reflection
                var fnParams = fn.Method.GetParameters();
                var nonSpecialParams = fnParams.Where(p =>
                    p.ParameterType != typeof(Progress) &&
                    !typeof(EventData).IsAssignableFrom(p.ParameterType) &&
                    !p.Name.StartsWith("__")).ToArray();

                // Validate input count if function expects specific number of params
                if (inputsList.Count > 0 && nonSpecialParams.Length > 0)
                {
                    // Allow flexible validation - function might use params array
                    // or have optional parameters
                }
            }

            // Determine trigger mode based on event type if not specified
            if (targetsList.Count > 0 && triggerMode == null)
            {
                var firstEventName = targetsList[0].eventName;
                if (firstEventName == "change" || firstEventName == "key_up")
                {
                    triggerMode = "always_last";
                }
                else if (firstEventName == "stream")
                {
                    triggerMode = "multiple";
                }
                else
                {
                    triggerMode = "once";
                }
            }

            triggerMode = triggerMode ?? "once";

            // Validate trigger mode
            if (triggerMode != "once" && triggerMode != "multiple" && triggerMode != "always_last")
            {
                throw new ArgumentException(
                    $"Invalid value for parameter `trigger_mode`: {triggerMode}. " +
                    "Please choose from: once, multiple, always_last");
            }

            // Analyze function for special arguments
            var fnToAnalyze = (renderable as dynamic)?.Fn ?? fn;
            var (progressIndex, eventDataIndex, componentPropIndices, tracksProgress) =
                AnalyzeSpecialArgs(fnToAnalyze as Delegate);

            if (componentPropInputs == null)
            {
                componentPropInputs = componentPropIndices ?? new List<int>();
            }

            // Generate API name if not provided
            if (string.IsNullOrWhiteSpace(apiName))
            {
                if (fn != null)
                {
                    apiName = DeriveDefaultApiName(fn);
                }
                else if (js != null)
                {
                    apiName = "js_fn";
                    apiVisibility = "private";
                }
                else
                {
                    apiName = "unnamed";
                    apiVisibility = "private";
                }
            }

            // Track the base (pre-suffix) api name for global re-uniquification during Blocks.Render() merge.
            var baseApiName = apiName;

            // Ensure unique API name
            var existingApiNames = Fns.Values
                .Where(f => !string.IsNullOrEmpty(f.ApiName))
                .Select(f => f.ApiName)
                .ToList();

            apiName = MakeApiNameUnique(apiName, existingApiNames);

            // Set collectsEventData if not specified
            if (!collectsEventData.HasValue)
            {
                collectsEventData = eventDataIndex.HasValue;
            }

            // Validate js=True constraints
            if (js == "True" && inputsList.Count > 0)
            {
                throw new ArgumentException(
                    "Cannot create event: events with js=True cannot have inputs.");
            }

            // Handle key-based function reuse
            bool reuseId = false;
            int fnId = FnId;

            if (key != null)
            {
                foreach (var existingFn in Fns.Values)
                {
                    if (existingFn.Key != null && existingFn.Key.Equals(key))
                    {
                        reuseId = true;
                        fnId = existingFn.Id;
                        break;
                    }
                }
            }

            // Create BlockFunction
            var blockFn = new BlockFunction(
                fn,
                inputsList,
                outputsList,
                preprocess,
                postprocess,
                fnId,
                inputsAsDict,
                targetsList,
                batch,
                maxBatchSize,
                concurrencyLimit,
                concurrencyId,
                tracksProgress,
                apiName,
                apiDescription as string,
                js,
                showProgress,
                showProgressOn,
                cancels,
                collectsEventData ?? false,
                triggerAfter,
                triggerOnlyOnSuccess,
                triggerOnlyOnFailure,
                triggerMode,
                queue,
                scrollToOutput,
                apiVisibility,
                renderable as Renderable,
                LocalContext.Renderable as Renderable, // rendered_in from LocalContext
                (LocalContext.Renderable as Renderable)?.RenderIteration, // render_iteration (Python: current renderable.render_iteration)
                isCancelFunction,
                connection,
                timeLimit,
                streamEvery,
                eventSpecificArgs,
                componentPropInputs,
                RootBlock.CurrentPage ?? "",
                jsImplementation,
                key,
                validator);

            // Store function
            Fns[fnId] = blockFn;
            // Track the base (pre-suffix) api_name for global re-uniquification on Render() merge.
            blockFn.BaseApiName = baseApiName;

            if (!reuseId)
            {
                FnId++;
            }

            return (blockFn, fnId);
        }

        public string MakeApiNameUniquePublic(string apiName, List<string> existingNames)
            => MakeApiNameUnique(apiName, existingNames);

        private string MakeApiNameUnique(string apiName, List<string> existingNames)
        {
            if (!existingNames.Contains(apiName))
            {
                return apiName;
            }

            int suffix = 1;
            string uniqueName;
            do
            {
                uniqueName = $"{apiName}_{suffix}";
                suffix++;
            }
            while (existingNames.Contains(uniqueName));

            return uniqueName;
        }

        private static string DeriveDefaultApiName(Delegate fn)
        {
            var rawName = fn.Method?.Name ?? "unnamed";

            // Compiler-generated local function names look like: <Main>g__SpeechToText|0_0
            var gMarker = rawName.IndexOf("g__", StringComparison.Ordinal);
            if (gMarker >= 0)
            {
                var start = gMarker + 3;
                var end = rawName.IndexOf('|', start);
                rawName = end > start ? rawName.Substring(start, end - start) : rawName.Substring(start);
            }
            else if (rawName.Contains("b__", StringComparison.Ordinal))
            {
                // Compiler-generated lambda/anonymous method
                rawName = "lambda";
            }

            var sanitized = new string(rawName.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "unnamed";
            }

            return ToSnakeCase(sanitized);
        }

        private static string ToSnakeCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "unnamed";
            }

            var sb = new StringBuilder(value.Length + 8);
            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (c == '-' || c == '_')
                {
                    if (sb.Length > 0 && sb[^1] != '_')
                    {
                        sb.Append('_');
                    }
                    continue;
                }

                if (char.IsUpper(c))
                {
                    if (sb.Length > 0 && sb[^1] != '_')
                    {
                        var prev = value[i - 1];
                        var nextIsLower = (i + 1 < value.Length) && char.IsLower(value[i + 1]);
                        if (char.IsLower(prev) || char.IsDigit(prev) || nextIsLower)
                        {
                            sb.Append('_');
                        }
                    }

                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            var result = sb.ToString().Trim('_');
            return string.IsNullOrEmpty(result) ? "unnamed" : result;
        }

        public static Dictionary<string, object> ConfigForBlock(
            int _id,
            List<int> renderedIds,
            Block block,
            object renderable = null)
        {
            // Skip if not in rendered list (for renderable-specific configs)
            if (renderable != null && !renderedIds.Contains(_id))
            {
                return new Dictionary<string, object>();
            }

            // Get block's configuration
            var props = block.GetConfig();

            // Skip properties that should not be deleted if block has a key
            var skipNonedeletion = new List<string>();
            if (renderable != null && block.Key != null)
            {
                // Nones are important for replacing a value in a keyed component
                foreach (var kvp in block.ConstructorArgs)
                {
                    if (kvp.Value == null)
                    {
                        skipNonedeletion.Add(kvp.Key);
                    }
                }
            }

            // Also keep null props that the block explicitly wants to preserve (Python parity)
            skipNonedeletion.AddRange(block.KeepNullProps);

            // Delete null values from props (except skipped ones)
            props = Utils.Utils.DeleteNone(props, skipNonedeletion);

            // Build block configuration
            var blockConfig = new Dictionary<string, object>
            {
                ["id"] = _id,
                ["type"] = block.GetBlockName(),
                ["props"] = props,
                ["skip_api"] = block.SkipApi,
                ["component_class_id"] = GetComponentClassId(block),
                ["key"] = block.UniqueKey()
            };

            if (renderable != null)
            {
                var renderableObj = renderable as IRenderable;
                if (renderableObj != null)
                {
                    blockConfig["renderable"] = renderableObj.Id;
                }
            }

            if (block.RenderedIn != null)
            {
                var renderedInObj = block.RenderedIn as IRenderable;
                if (renderedInObj != null)
                {
                    blockConfig["rendered_in"] = renderedInObj.Id;
                }
            }

            // Add API info if not skipping API
            if (!block.SkipApi)
            {
                if (block is Components.Component component)
                {
                    var apiInfo = component.ApiInfo();
                    blockConfig["api_info"] = apiInfo;

                    // Try to get api_info_as_input (method or property)
                    var apiInfoAsInputMethod = component.GetType().GetMethod("ApiInfoAsInput");
                    if (apiInfoAsInputMethod != null)
                    {
                        blockConfig["api_info_as_input"] = apiInfoAsInputMethod.Invoke(component, null) ?? apiInfo;
                    }
                    else
                    {
                        blockConfig["api_info_as_input"] = apiInfo;
                    }

                    // Try to get api_info_as_output (method or property)
                    var apiInfoAsOutputMethod = component.GetType().GetMethod("ApiInfoAsOutput");
                    if (apiInfoAsOutputMethod != null)
                    {
                        blockConfig["api_info_as_output"] = apiInfoAsOutputMethod.Invoke(component, null) ?? apiInfo;
                    }
                    else
                    {
                        blockConfig["api_info_as_output"] = apiInfo;
                    }

                    object? exampleInputs = null;
                    // Prefer component-specific example_payload/example_value when present.
                    var examplePayloadMethod = component.GetType().GetMethod("ExamplePayload");
                    if (examplePayloadMethod != null)
                    {
                        try
                        {
                            exampleInputs = examplePayloadMethod.Invoke(component, null);
                        }
                        catch
                        {
                            // ignore and fallback
                        }
                    }

                    if (exampleInputs == null)
                    {
                        var exampleValueMethod = component.GetType().GetMethod("ExampleValue");
                        if (exampleValueMethod != null)
                        {
                            try
                            {
                                exampleInputs = exampleValueMethod.Invoke(component, null);
                            }
                            catch
                            {
                                // ignore and fallback
                            }
                        }
                    }

                    if (exampleInputs == null)
                    {
                        var exampleList = component.ExampleInputs();
                        if (exampleList != null)
                        {
                            if (exampleList.Count == 1)
                            {
                                exampleInputs = exampleList[0];
                            }
                            else
                            {
                                exampleInputs = exampleList;
                            }
                        }
                    }

                    blockConfig["example_inputs"] = exampleInputs;
                }
            }

            return blockConfig;
        }

        private static object GetComponentClassId(Block block)
        {
            var blockType = block.GetType();

            // 0) Prefer Python-parity class IDs when local Python gradio sources are available.
            // Python computes this as sha256(f"{cls.__name__}_{inspect.getfile(cls)}").
            var pythonParityId = TryGetPythonParityComponentClassId(blockType, block.GetBlockName());
            if (!string.IsNullOrWhiteSpace(pythonParityId))
            {
                return pythonParityId;
            }

            // 1) Prefer an explicit ComponentClassId property on the block instance.
            var property = blockType.GetProperty("ComponentClassId");
            if (property != null && property.CanRead)
            {
                var value = property.GetValue(block)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            // 2) Try instance/static GetComponentClassId() method if available.
            var method = blockType.GetMethod(
                "GetComponentClassId",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);
            if (method != null)
            {
                var target = method.IsStatic ? null : block;
                var result = method.Invoke(target, null)?.ToString();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return result;
                }
            }

            // 3) Deterministic fallback for blocks without explicit IDs.
            var payload = $"{blockType.FullName}:{block.GetBlockName()}";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private static string? TryGetPythonParityComponentClassId(Type blockType, string blockName)
        {
            var gradioRoot = FindPythonGradioRoot();
            if (string.IsNullOrWhiteSpace(gradioRoot) || !Directory.Exists(gradioRoot))
            {
                return null;
            }

            // Python parity: compute class name to file mapping.
            // For types like ClearButton, the file is clear_button.py (not button.py).
            // Convert the C# class name to snake_case as an additional candidate.
            var classNameSnakeCase = ToSnakeCase(blockType.Name);

            var moduleCandidates = new List<string>
            {
                // First try snake_case of class name (handles ClearButton → clear_button.py)
                Path.Combine(gradioRoot, "components", $"{classNameSnakeCase}.py"),
                Path.Combine(gradioRoot, "layouts", $"{classNameSnakeCase}.py"),
                Path.Combine(gradioRoot, $"{classNameSnakeCase}.py"),
                // Then try blockName (the generic type name like "button")
                Path.Combine(gradioRoot, "components", $"{blockName}.py"),
                Path.Combine(gradioRoot, "layouts", $"{blockName}.py"),
                Path.Combine(gradioRoot, $"{blockName}.py")
            };

            var modulePath = moduleCandidates.FirstOrDefault(File.Exists);
            if (string.IsNullOrWhiteSpace(modulePath))
            {
                return null;
            }

            // Keep the absolute path form to mirror Python inspect.getfile(cls) on Windows.
            var fullModulePath = Path.GetFullPath(modulePath);
            var payload = $"{blockType.Name}_{fullModulePath}";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private static string? FindPythonGradioRoot()
        {
            // 1) Check current directory and parent chain for debug/temp/venv site-packages gradio.
            try
            {
                var current = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (current != null)
                {
                    var candidate = Path.Combine(current.FullName, "debug", "temp", "venv", "Lib", "site-packages", "gradio");
                    if (Directory.Exists(candidate))
                    {
                        return candidate;
                    }

                    current = current.Parent;
                }
            }
            catch
            {
                // ignore and fallback
            }

            // 2) Check from app base directory up the hierarchy.
            try
            {
                var current = new DirectoryInfo(AppContext.BaseDirectory);
                while (current != null)
                {
                    var candidate = Path.Combine(current.FullName, "debug", "temp", "venv", "Lib", "site-packages", "gradio");
                    if (Directory.Exists(candidate))
                    {
                        return candidate;
                    }

                    current = current.Parent;
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        public Dictionary<string, object> GetConfig(object renderable = null)
        {
            var config = new Dictionary<string, object>
            {
                ["page"] = new Dictionary<string, object>(),
                ["components"] = new List<Dictionary<string, object>>(),
                ["dependencies"] = new List<Dictionary<string, object>>()
            };

            var pageDict = (Dictionary<string, object>)config["page"];
            var componentsList = (List<Dictionary<string, object>>)config["components"];
            var dependenciesList = (List<Dictionary<string, object>>)config["dependencies"];

            // Initialize page configurations
            foreach (var pageInfo in RootBlock.Pages)
            {
                var pageName = pageInfo.Item1;
                if (!pageDict.ContainsKey(pageName))
                {
                    pageDict[pageName] = new Dictionary<string, object>
                    {
                        ["layout"] = new Dictionary<string, object>
                        {
                            ["id"] = RootBlock._id,
                            ["children"] = new List<object>()
                        },
                        ["components"] = new List<int>(),
                        ["dependencies"] = new List<int>()
                    };
                }
            }

            // Track rendered IDs
            var renderedIds = new List<int>();

            // Generate layout tree
            Dictionary<string, object> GetLayout(Block block)
            {
                renderedIds.Add(block._id);

                if (!(block is BlockContext blockContext))
                {
                    return new Dictionary<string, object> { ["id"] = block._id };
                }

                var childrenLayout = new List<object>();
                // Python parity: iterate all children without filtering by parent.
                // When Blocks.Render() extends render_context.children, the merged
                // children retain their original Parent pointers; filtering by
                // parent would exclude them and produce an empty per-page layout.
                foreach (var child in blockContext.Children.DistinctBy(c => c._id))
                {
                    var layout = GetLayout(child);
                    childrenLayout.Add(layout);
                }

                return new Dictionary<string, object>
                {
                    ["id"] = block._id,
                    ["children"] = childrenLayout
                };
            }

            // Get root block (or renderable container)
            Block rootBlock;
            if (renderable != null)
            {
                // Get container from renderable if it has one
                var containerProp = renderable.GetType().GetProperty("Container");
                if (containerProp != null && containerProp.GetValue(renderable) is Block container)
                {
                    rootBlock = container;
                }
                else
                {
                    rootBlock = RootBlock;
                }
            }
            else
            {
                rootBlock = RootBlock;
            }

            var layout = GetLayout(rootBlock);
            config["layout"] = layout;

            // Add root children to page layouts
            if (layout.ContainsKey("children"))
            {
                var children = (List<object>)layout["children"];
                foreach (var child in children)
                {
                    if (child is Dictionary<string, object> childDict &&
                        childDict.ContainsKey("id") &&
                        Blocks.ContainsKey((int)childDict["id"]))
                    {
                        var childBlock = Blocks[(int)childDict["id"]];
                        var pageName = childBlock.Page ?? "";
                        if (pageDict.ContainsKey(pageName))
                        {
                            var pageConfig = (Dictionary<string, object>)pageDict[pageName];
                            var pageLayout = (Dictionary<string, object>)pageConfig["layout"];
                            var pageChildren = (List<object>)pageLayout["children"];
                            pageChildren.Add(child);
                        }
                    }
                }
            }

            // Add components to config
            // Python parity: component list follows render order (render sequence),
            // not layout traversal order.
            var orderedIds = Blocks
                .OrderBy(kvp => kvp.Value?.RenderSequence ?? 0)
                .ThenBy(kvp => kvp.Key)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var id in orderedIds)
            {
                if (!Blocks.TryGetValue(id, out var block))
                {
                    continue;
                }

                var _id = id;

                var blockConfig = ConfigForBlock(_id, renderedIds, block, renderable);
                if (blockConfig.Count == 0)
                {
                    continue;
                }

                componentsList.Add(blockConfig);

                var pageName = block.Page ?? "";
                if (pageDict.ContainsKey(pageName))
                {
                    var pageConfig = (Dictionary<string, object>)pageDict[pageName];
                    var pageComponents = (List<int>)pageConfig["components"];
                    pageComponents.Add(_id);
                }
            }

            // Add dependencies (functions) to config
            foreach (var fn in Fns.Values)
            {
                // Python parity for dynamic render config:
                // when requesting config for a specific renderable, include only
                // dependencies created inside that renderable context.
                if (renderable != null && !ReferenceEquals(fn.RenderedIn, renderable))
                {
                    continue;
                }

                var dependencyConfig = fn.GetConfig();
                dependenciesList.Add(dependencyConfig);

                var pageName = fn.Page ?? ""; // Get page from function
                if (pageDict.ContainsKey(pageName))
                {
                    var pageConfig = (Dictionary<string, object>)pageDict[pageName];
                    var pageDependencies = (List<int>)pageConfig["dependencies"];
                    pageDependencies.Add(fn.Id);
                }
            }

            return config;
        }

        public void AttachLoadEvents(object renderable = null)
        {
            // Iterate through all blocks and check for load events to attach
            foreach (var block in Blocks.Values)
            {
                // Skip if renderable is specified and doesn't match
                if (renderable != null && block.RenderedIn != renderable)
                {
                    continue;
                }

                // Only process Component instances with LoadEventToAttach
                if (block is Components.Component component && component.LoadEventToAttach.HasValue)
                {
                    var (loadFn, triggers, inputs) = component.LoadEventToAttach.Value;

                    bool hasTarget = triggers.Count > 0;

                    foreach (var t in triggers)
                    {
                    }

                    // Add the root block's load event to the triggers
                    var allTriggers = new List<EventListenerMethod>(
                        triggers.Select(t => new EventListenerMethod(t.Item1, t.Item2))
                    );
                    allTriggers.Add(new EventListenerMethod(RootBlock, "load"));

                    // Set up the event trigger
                    var (blockFn, fnId) = SetEventTrigger(
                        targets: allTriggers,
                        fn: loadFn,
                        inputs: inputs,
                        outputs: component,
                        noTarget: !hasTarget,
                        showProgress: hasTarget ? "hidden" : "full"
                    );


                    // Store the load event configuration on the component
                    component.LoadEvent = blockFn.GetConfig();
                }
            }
        }

        public Dictionary<string, object> GetConfig()
        {
            var config = new Dictionary<string, object>();

            // Components array - list of all blocks
            var components = new List<Dictionary<string, object>>();
            foreach (var kvp in Blocks.OrderBy(b => b.Key))
            {
                var block = kvp.Value;
                var blockConfig = block.GetConfig();

                if (blockConfig != null)
                {
                    blockConfig["id"] = kvp.Key;
                    components.Add(blockConfig);
                }
            }
            config["components"] = components;

            // Dependencies array - list of all registered functions
            var dependencies = new List<Dictionary<string, object>>();
            foreach (var kvp in Fns.OrderBy(f => f.Key))
            {
                var fn = kvp.Value;

                // Get input component IDs
                var inputIds = new List<int>();
                if (fn.Inputs is IEnumerable<object> inputList)
                {
                    foreach (var input in inputList)
                    {
                        if (input is Block block)
                        {
                            inputIds.Add(block._id);
                        }
                    }
                }

                // Get output component IDs
                var outputIds = new List<int>();
                if (fn.Outputs is IEnumerable<object> outputList)
                {
                    foreach (var output in outputList)
                    {
                        if (output is Block block)
                        {
                            outputIds.Add(block._id);
                        }
                    }
                }

                var depConfig = new Dictionary<string, object>
                {
                    ["id"] = kvp.Key,
                    ["targets"] = (object)(fn.Targets?.Select(t => new List<object> { t.blockId ?? (object)"null", t.eventName }).ToList()) ?? new List<object>(),
                    ["inputs"] = inputIds,
                    ["outputs"] = outputIds,
                    ["backend_fn"] = fn.Fn != null,
                    ["js"] = fn.Js,
                    ["queue"] = fn.Queue,
                    ["api_name"] = fn.ApiName,
                    ["scroll_to_output"] = fn.ScrollToOutput,
                    ["show_progress"] = fn.ShowProgress,
                    ["batch"] = fn.Batch,
                    ["max_batch_size"] = fn.MaxBatchSize,
                    ["cancels"] = fn.Cancels ?? new List<int>(),
                    ["types"] = new Dictionary<string, string>
                    {
                        ["generator"] = fn.TypesGenerator ? "true" : "false",
                        ["cancel"] = fn.IsCancelFunction ? "true" : "false"
                    },
                    ["collects_event_data"] = fn.CollectsEventData,
                    ["trigger_mode"] = fn.TriggerMode,
                    ["show_api"] = fn.ApiVisibility == "public"
                };

                if (fn.ConcurrencyLimit != null)
                {
                    depConfig["concurrency_limit"] = fn.ConcurrencyLimit;
                }

                if (!string.IsNullOrEmpty(fn.ConcurrencyId))
                {
                    depConfig["concurrency_id"] = fn.ConcurrencyId;
                }

                dependencies.Add(depConfig);
            }
            config["dependencies"] = dependencies;

            return config;
        }

        public BlocksConfig ShallowCopy()
        {
            var newConfig = new BlocksConfig(RootBlock)
            {
                Blocks = new Dictionary<int, Block>(this.Blocks),
                Fns = new Dictionary<int, BlockFunction>(this.Fns),
                FnId = this.FnId
            };
            return newConfig;
        }

        private (int?, int?, List<int>, bool) AnalyzeSpecialArgs(Delegate fn)
        {
            if (fn == null)
                return (null, null, new List<int>(), false);

            var method = fn.Method;
            var parameters = method.GetParameters();

            int? progressIndex = null;
            int? eventDataIndex = null;
            var componentPropIndices = new List<int>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramType = param.ParameterType;

                // Check for Progress type
                if (paramType.Name.Contains("Progress"))
                {
                    progressIndex = i;
                }
                // Check for EventData type (including subclasses like SelectData, LikeData, etc.)
                else if (typeof(EventData).IsAssignableFrom(paramType) || paramType.Name.Contains("EventData") || param.Name == "eventData")
                {
                    eventDataIndex = i;
                }
                // Check for component property types (Request, etc.)
                else if (paramType.Name.Contains("Request"))
                {
                    componentPropIndices.Add(i);
                }
            }

            bool tracksProgress = progressIndex.HasValue;

            return (progressIndex, eventDataIndex, componentPropIndices, tracksProgress);
        }
    }
}

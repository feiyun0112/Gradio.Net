using System.Reflection;

namespace Gradio.Net
{
    public static class Pipelines
    {
        public static Dictionary<string, object> LoadFromPipeline(object pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            var pipelineModule = GetPipelineModule(pipeline);

            Dictionary<string, object> pipelineInfo;

            if (!string.IsNullOrEmpty(pipelineModule) && pipelineModule.StartsWith("transformers.pipelines."))
            {
                pipelineInfo = PipelinesUtils.HandleTransformersPipeline(pipeline);
            }
            else if (!string.IsNullOrEmpty(pipelineModule) && pipelineModule.StartsWith("diffusers.pipelines."))
            {
                pipelineInfo = PipelinesUtils.HandleDiffusersPipeline(pipeline);
            }
            else
            {
                throw new Exception("pipeline must be a transformers.pipeline or diffusers.pipeline");
            }

            Func<object[], object> fn = (params object[] args) =>
            {
                if (pipelineInfo == null)
                {
                    throw new Exception("pipeline_info can not be None.");
                }

                var preprocess = pipelineInfo.ContainsKey("preprocess") ? pipelineInfo["preprocess"] as Func<object[], object> : null;
                var data = preprocess != null ? preprocess(args) : args;

                var pipelineOutput = InvokePipeline(pipeline, data);

                if (pipelineInfo.ContainsKey("postprocess") && pipelineInfo["postprocess"] is Delegate postprocess)
                {
                    var postParams = postprocess.Method.GetParameters().Length;
                    return postParams switch
                    {
                        0 => postprocess.DynamicInvoke(),
                        1 => postprocess.DynamicInvoke(pipelineOutput),
                        _ => postprocess.DynamicInvoke(pipelineOutput, args)
                    };
                }

                return pipelineOutput;
            };

            var interfaceInfo = new Dictionary<string, object>(pipelineInfo);
            interfaceInfo["fn"] = fn;
            interfaceInfo.Remove("preprocess");
            interfaceInfo.Remove("postprocess");

            interfaceInfo["title"] = GetPipelineTitle(pipeline, pipelineModule);

            return interfaceInfo;
        }

        public static Dictionary<string, object> LoadFromJsPipeline(object pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            var pipelineType = pipeline.GetType();
            var pipelineModule = GetPipelineModule(pipeline);

            if (string.IsNullOrEmpty(pipelineModule) || !pipelineModule.StartsWith("transformers_js_py."))
            {
                throw new Exception("pipeline must be a transformers_js_py's pipeline");
            }

            var pipelineInfo = PipelinesUtils.HandleTransformersJsPipeline(pipeline);

            Func<object[], Task<object>> fn = async (params object[] args) =>
            {
                var preprocess = pipelineInfo.ContainsKey("preprocess") ? pipelineInfo["preprocess"] as Func<object[], object> : null;
                var postprocessTakesInputs = pipelineInfo.ContainsKey("postprocess_takes_inputs") && (bool)pipelineInfo["postprocess_takes_inputs"];

                var preprocessedParams = preprocess != null ? preprocess(args) : args;

                var pipelineOutput = await InvokePipelineAsync(pipeline, preprocessedParams);

                if (pipelineInfo.ContainsKey("postprocess") && pipelineInfo["postprocess"] is Delegate postprocess)
                {
                    if (postprocessTakesInputs)
                    {
                        var merged = new object[] { pipelineOutput }.Concat(args ?? Array.Empty<object>()).ToArray();
                        return postprocess.DynamicInvoke(merged);
                    }

                    var pcount = postprocess.Method.GetParameters().Length;
                    return pcount switch
                    {
                        0 => postprocess.DynamicInvoke(),
                        1 => postprocess.DynamicInvoke(pipelineOutput),
                        _ => postprocess.DynamicInvoke(pipelineOutput, args)
                    };
                }

                return pipelineOutput;
            };

            var interfaceInfo = new Dictionary<string, object>
            {
                { "fn", fn },
                { "inputs", pipelineInfo.ContainsKey("inputs") ? pipelineInfo["inputs"] : new List<object>() },
                { "outputs", pipelineInfo.ContainsKey("outputs") ? pipelineInfo["outputs"] : new List<object>() }
            };

            try
            {
                var taskProperty = pipelineType.GetProperty("task");
                var modelProperty = pipelineType.GetProperty("model");

                if (taskProperty != null && modelProperty != null)
                {
                    var task = taskProperty.GetValue(pipeline);
                    var model = modelProperty.GetValue(pipeline);

                    if (model != null)
                    {
                        var configProperty = model.GetType().GetProperty("config");
                        if (configProperty != null)
                        {
                            var config = configProperty.GetValue(model);
                            if (config != null)
                            {
                                var nameOrPathProperty = config.GetType().GetProperty("_name_or_path");
                                if (nameOrPathProperty != null)
                                {
                                    var nameOrPath = nameOrPathProperty.GetValue(config);
                                    interfaceInfo["title"] = $"{task} ({nameOrPath})";
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                interfaceInfo["title"] = "Transformers JS Pipeline";
            }

            return interfaceInfo;
        }

        private static string GetPipelineModule(object pipeline)
        {
            if (pipeline == null) return string.Empty;

            var type = pipeline.GetType();
            if (!string.IsNullOrWhiteSpace(type.Namespace))
            {
                return type.Namespace;
            }

            var moduleProp = type.GetProperty("__module__", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? type.GetProperty("module", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var module = moduleProp?.GetValue(pipeline)?.ToString();
            if (!string.IsNullOrWhiteSpace(module))
            {
                return module;
            }

            return type.FullName ?? string.Empty;
        }

        private static string GetPipelineTitle(object pipeline, string pipelineModule)
        {
            if (!string.IsNullOrEmpty(pipelineModule) && pipelineModule.StartsWith("transformers.pipelines"))
            {
                var model = GetMemberValue(pipeline, "model");
                var config = GetMemberValue(model, "config");
                var nameOrPath = GetMemberValue(config, "name_or_path")?.ToString();
                return string.IsNullOrWhiteSpace(nameOrPath) ? "Transformers Pipeline" : nameOrPath;
            }

            if (!string.IsNullOrEmpty(pipelineModule) && pipelineModule.StartsWith("diffusers.pipelines"))
            {
                return pipeline.GetType().Name;
            }

            return "Pipeline Interface";
        }

        private static object? GetMemberValue(object? obj, string member)
        {
            if (obj == null) return null;
            var type = obj.GetType();
            var prop = type.GetProperty(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null) return prop.GetValue(obj);
            var field = type.GetField(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return field?.GetValue(obj);
        }

        private static object InvokePipeline(object pipeline, object data)
        {
            if (pipeline is Delegate d)
            {
                return InvokeDelegate(d, data);
            }

            var t = pipeline.GetType();
            var candidates = t
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name is "Invoke" or "Call" or "__call__" or "Run")
                .ToList();

            if (!candidates.Any())
            {
                // Fallback: if this object itself has an Invoke method through dynamic proxy style
                var invoke = t.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
                if (invoke != null) candidates.Add(invoke);
            }

            foreach (var m in candidates)
            {
                try
                {
                    var args = BuildInvokeArgs(m, data);
                    if (args != null)
                    {
                        return m.Invoke(pipeline, args);
                    }
                }
                catch
                {
                    // try next overload
                }
            }

            throw new Exception($"Unable to invoke pipeline of type {t.FullName}. No compatible call method found.");
        }

        private static async Task<object> InvokePipelineAsync(object pipeline, object data)
        {
            var result = InvokePipeline(pipeline, data);
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty != null ? resultProperty.GetValue(task) : null;
            }
            return result;
        }

        private static object InvokeDelegate(Delegate d, object data)
        {
            if (data is object[] arr)
            {
                return d.DynamicInvoke(arr);
            }

            return d.DynamicInvoke(data);
        }

        private static object[]? BuildInvokeArgs(MethodInfo method, object data)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return Array.Empty<object>();
            }

            if (data is object[] arr)
            {
                if (parameters.Length == arr.Length)
                {
                    return arr;
                }

                // params object[]
                if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
                {
                    return new object[] { arr };
                }

                return null;
            }

            if (parameters.Length == 1)
            {
                return new object[] { data };
            }

            return null;
        }
    }
}

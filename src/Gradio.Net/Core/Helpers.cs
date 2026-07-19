using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net.Core;

public static class Helpers
{


    public static Examples CreateExamples(
        object examples,
        object inputs,
        object? outputs = null,
        Delegate? fn = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        bool apiMode = false,
        object? label = null,
        string? elemId = null,
        bool runOnClick = false,
        bool preprocess = true,
        bool postprocess = true,
        string apiVisibility = "undocumented",
        string? apiName = "load_example",
        object? apiDescription = null,
        bool batch = false,
        List<string>? exampleLabels = null,
        object? visible = null,
        object? preload = null
    )
    {
        var examplesObj = new Examples(
            examples,
            inputs,
            outputs,
            fn,
            cacheExamples,
            cacheMode,
            examplesPerPage,
            apiMode,
            label,
            elemId,
            runOnClick,
            preprocess,
            postprocess,
            apiVisibility,
            apiName,
            apiDescription,
            batch,
            exampleLabels,
            visible,
            preload,
            false
        );
        examplesObj.Create();
        return examplesObj;
    }

    public static (List<object>, int?, int?, List<int>) SpecialArgs(
        Delegate fn,
        List<object>? inputs = null,
        HttpRequest? request = null,
        EventData? eventData = null,
        Dictionary<int, Dictionary<string, object>>? componentProps = null
    )
    {
        try
        {
            var signature = fn.Method;
            var parameters = signature.GetParameters().ToList();
            var positionalArgs = parameters.Where(p =>
                p.ParameterType.IsValueType ||
                p.ParameterType != typeof(object)
            ).ToList();

            var resultInputs = inputs ?? new List<object>();
            int? progressIndex = null;
            int? eventDataIndex = null;
            var componentPropIndices = new List<int>();

            for (int i = 0; i < positionalArgs.Count; i++)
            {
                var param = positionalArgs[i];
                var paramType = param.ParameterType;

                // Handle Progress parameter
                if (paramType == typeof(Progress) || paramType.IsAssignableFrom(typeof(Progress)))
                {
                    progressIndex = i;
                    if (resultInputs != null)
                    {
                        resultInputs.Insert(i, new Progress());
                    }
                }
                // Handle Request parameter
                else if (paramType == typeof(HttpRequest) || Nullable.GetUnderlyingType(paramType) == typeof(HttpRequest))
                {
                    if (resultInputs != null)
                    {
                        resultInputs.Insert(i, request);
                    }
                }
                // Handle EventData parameter
                else if (paramType == typeof(EventData) || paramType.IsAssignableFrom(typeof(EventData)))
                {
                    eventDataIndex = i;
                    if (resultInputs != null && eventData != null)
                    {
                        resultInputs.Insert(i, eventData);
                    }
                }
                // Handle OAuthProfile and OAuthToken types
                else if (paramType.Name == "OAuthProfile" || (Nullable.GetUnderlyingType(paramType)?.Name == "OAuthProfile"))
                {
                    if (resultInputs != null && request != null)
                    {
                        // In a full implementation, this would extract the OAuth profile from the request
                        // For now, we'll add null as a placeholder
                        resultInputs.Insert(i, null);
                    }
                }
                else if (paramType.Name == "OAuthToken" || (Nullable.GetUnderlyingType(paramType)?.Name == "OAuthToken"))
                {
                    if (resultInputs != null && request != null)
                    {
                        // In a full implementation, this would extract the OAuth token from the request
                        // For now, we'll add null as a placeholder
                        resultInputs.Insert(i, null);
                    }
                }
                // Handle Header type
                else if (paramType.Name == "Header" || (Nullable.GetUnderlyingType(paramType)?.Name == "Header"))
                {
                    if (resultInputs != null && request != null)
                    {
                        // In a full implementation, this would extract the header from the request
                        // For now, we'll add null as a placeholder
                        resultInputs.Insert(i, null);
                    }
                }
                // Handle Component type parameters
                else if (paramType.IsSubclassOf(typeof(Component)))
                {
                    componentPropIndices.Add(i);
                    if (resultInputs != null && componentProps != null && componentProps.ContainsKey(i))
                    {
                        // Create a simple namespace for component props
                        var props = componentProps[i];
                        resultInputs[i] = new SimpleNamespace(props);
                    }
                }
                // Handle default values
                else if (param.HasDefaultValue && resultInputs != null && resultInputs.Count <= i)
                {
                    resultInputs.Insert(i, param.DefaultValue ?? null);
                }
            }

            // Fill in any missing arguments with defaults
            if (resultInputs != null)
            {
                while (resultInputs.Count < positionalArgs.Count)
                {
                    int i = resultInputs.Count;
                    var param = positionalArgs[i];
                    if (!param.HasDefaultValue)
                    {
                        resultInputs.Add(null);
                    }
                    else
                    {
                        resultInputs.Add(param.DefaultValue);
                    }
                }
            }

            return (resultInputs, progressIndex, eventDataIndex, componentPropIndices);
        }
        catch (Exception ex)
        {
            return (inputs ?? new List<object>(), null, null, new List<int>());
        }
    }

    public static Dictionary<string, object> Update(
        string? elemId = null,
        object? elemClasses = null,
        object? visible = null,
        params KeyValuePair<string, object>[] kwargs
    )
    {
        var result = new Dictionary<string, object>
        {
            { "__type__", "update" }
        };

        if (elemId != null)
        {
            result["elem_id"] = elemId;
        }

        if (elemClasses != null)
        {
            result["elem_classes"] = elemClasses;
        }

        if (visible != null)
        {
            result["visible"] = visible;
        }

        foreach (var kvp in kwargs)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }

    public static Dictionary<string, object> Validate(bool isValid, string message)
    {
        return new Dictionary<string, object>
        {
            { "__type__", "validate" },
            { "is_valid", isValid },
            { "message", message }
        };
    }

    public static Dictionary<string, object> Skip()
    {
        return new Dictionary<string, object>
        {
            { "__type__", "update" }
        };
    }

    public static void LogMessage(
        string message,
        string title,
        string level = "info",
        double? duration = 10,
        bool visible = true
    )
    {
        var blocks = LocalContext.Blocks;
        var eventId = LocalContext.EventId;

        if (blocks == null || eventId == null)
        {
            // Function called outside of Gradio context
            if (level == "info" || level == "success")
                Console.WriteLine(message);
            else if (level == "warning")
                Console.Error.WriteLine($"Warning: {message}");
            return;
        }

        // Get the QueueManager from the server app and send the log message
        try
        {
            var serverApp = blocks.GetServerApp();
            if (serverApp == null) return;

            var qmProp = serverApp.GetType().GetProperty("QueueManager");
            if (qmProp?.GetValue(serverApp) is Gradio.Net.Core.Queueing.QueueManager queueManager)
            {
                queueManager.SendLogMessage(eventId, message, title, level, duration, visible);
            }
        }
        catch
        {
            // Fallback: ignore errors in logging
        }
    }

    public static void Warning(
        string message = "Warning issued.",
        double? duration = 10,
        bool visible = true,
        string title = "Warning"
    )
    {
        LogMessage(message, title: title, level: "warning", duration: duration, visible: visible);
    }

    public static void Info(
        string message = "Info issued.",
        double? duration = 10,
        bool visible = true,
        string title = "Info"
    )
    {
        LogMessage(message, title: title, level: "info", duration: duration, visible: visible);
    }

    public static void Success(
        string message = "Success.",
        double? duration = 10,
        bool visible = true,
        string title = "Success"
    )
    {
        LogMessage(message, title: title, level: "success", duration: duration, visible: visible);
    }

    public static void PatchTqdm()
    {
        // tqdm is a Python library not directly available in .NET
        // This function is a no-op in the C# implementation
    }

    public static (Progress, Delegate) CreateTracker(Delegate fn, bool trackTqdm)
    {
        var progress = new Progress(trackTqdm);
        if (!trackTqdm)
        {
            return (progress, fn);
        }

        // Wrap function to track progress
        Delegate wrappedFn = new Func<object[], object>((args) =>
        {
            try
            {
                // Call original function and track progress
                var result = fn.DynamicInvoke(args);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        });

        return (progress, wrappedFn);
    }

    public static async Task<List<object>> MergeGeneratedValuesIntoOutput(
        List<Component> components,
        List<object> generatedValues,
        List<object> output
    )
    {
        foreach (var (outputIndex, outputComponent) in components.Select((c, i) => (i, c)))
        {
            if (outputComponent is Gradio.Net.Components.IStreamingOutput streamingOutput)
            {
                var binaryChunks = new List<object>();
                string? desiredOutputFormat = null;

                for (int i = 0; i < generatedValues.Count; i++)
                {
                    var chunk = generatedValues[i];
                    if (components.Count > 1 && chunk is List<object> chunkList)
                    {
                        chunk = chunkList[outputIndex];
                    }

                    var processedChunk = outputComponent.Postprocess(chunk);
                    if (processedChunk is GradioModel model)
                    {
                        processedChunk = model.ModelDump();
                    }
                    else if (processedChunk is GradioRootModel<object> rootModel)
                    {
                        processedChunk = rootModel.ModelDump();
                    }

                    var streamChunk = await streamingOutput.StreamOutput(
                        processedChunk, "", i == 0
                    );

                    if (i == 0 && streamChunk != null && streamChunk is Dictionary<string, object> streamChunkDict1 && streamChunkDict1.TryGetValue("orig_name", out var origNameObj))
                    {
                        var origName = origNameObj.ToString();
                        if (!string.IsNullOrEmpty(origName))
                        {
                            desiredOutputFormat = Path.GetExtension(origName).Substring(1);
                        }
                    }

                    if (streamChunk != null && streamChunk is Dictionary<string, object> streamChunkDict2 && streamChunkDict2.TryGetValue("data", out var dataObj))
                    {
                        binaryChunks.Add(dataObj);
                    }
                }

                // Convert to byte[] list for CombineStream
                var byteChunks = new List<byte[]>();
                foreach (var chunk in binaryChunks)
                {
                    if (chunk is byte[] bytes)
                    {
                        byteChunks.Add(bytes);
                    }
                }

                var combinedOutput = await streamingOutput.CombineStream(
                    byteChunks, desiredOutputFormat
                );
                if (combinedOutput is GradioModel combinedModel)
                {
                    output[outputIndex] = combinedModel.ModelDump();
                }
                else
                {
                    output[outputIndex] = combinedOutput;
                }
            }
        }

        return output;
    }
}

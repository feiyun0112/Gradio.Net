using Gradio.Net.Components;
using Gradio.Net.Data;

namespace Gradio.Net.Core
{
    public static class BlockHelpers
    {
        public static Dictionary<string, object> PostprocessUpdateDict(
            Block block,
            Dictionary<string, object> updateDict,
            bool postprocess = true)
        {
            if (updateDict == null)
            {
                return new Dictionary<string, object>();
            }

            var result = new Dictionary<string, object>(updateDict);
            object value = null;
            bool hasValue = false;

            // Extract value if present
            if (result.ContainsKey("value"))
            {
                value = result["value"];
                result.Remove("value");
                hasValue = true;
            }

            // Process props if the block supports them
            Dictionary<string, object> props = null;
            if (HasProperty(block, "Props"))
            {
                props = new Dictionary<string, object>();
                var keysToRemove = new List<string>();

                foreach (var kvp in result)
                {
                    if (!HasProperty(block, kvp.Key))
                    {
                        props[kvp.Key] = kvp.Value;
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    result.Remove(key);
                }
            }

            // Update with actual block properties
            var processedDict = new Dictionary<string, object>();
            foreach (var key in result.Keys)
            {
                if (HasProperty(block, key))
                {
                    var propValue = GetPropertyValue(block, key);
                    if (propValue != null)
                    {
                        processedDict[key] = propValue;
                    }
                }
            }

            if (props != null && props.Count > 0)
            {
                processedDict["props"] = props;
            }

            // Process value if present (note: null is a valid value; Python distinguishes None from NO_VALUE)
            if (hasValue)
            {
                if (postprocess && block is Component component)
                {
                    var postprocessedValue = component.Postprocess(value);
                    processedDict["value"] = postprocessedValue;
                }
                else
                {
                    processedDict["value"] = value;
                }
            }

            processedDict["__type__"] = "update";
            return processedDict;
        }

        public static object ConvertComponentDictToList(
            List<int> outputsIds,
            object predictions)
        {
            if (predictions == null || !(predictions is Dictionary<object, object> predDict))
            {
                return predictions;
            }

            // Check if all keys are blocks
            var keys = predDict.Keys.ToList();
            var keysAreBlocks = keys.Select(k => k is Block).ToList();

            if (keysAreBlocks.All(k => k))
            {
                // All keys are blocks - reorder according to outputs_ids
                var reorderedPredictions = new List<object>();

                // Initialize with skip values
                for (int i = 0; i < outputsIds.Count; i++)
                {
                    reorderedPredictions.Add(CreateSkipValue());
                }

                // Fill in actual values
                foreach (var kvp in predDict)
                {
                    if (kvp.Key is Block component)
                    {
                        if (!outputsIds.Contains(component._id))
                        {
                            throw new InvalidOperationException(
                                $"Returned component {component} not specified as output of function.");
                        }

                        int outputIndex = outputsIds.IndexOf(component._id);
                        reorderedPredictions[outputIndex] = kvp.Value;
                    }
                }

                // Resolve singleton if needed
                return ResolveSingleton(reorderedPredictions);
            }
            else if (keysAreBlocks.Any(k => k))
            {
                // Some keys are blocks - this is an error
                throw new InvalidOperationException(
                    "Returned dictionary included some keys as Components. Either all keys must be Components " +
                    "to assign Component values, or return a List of values to assign output values in order.");
            }

            // No blocks in keys - return unchanged
            return predictions;
        }

        private static object CreateSkipValue()
        {
            // Python parity: initialize with gradio.helpers.skip() (an "update" dict without a value)
            return Helpers.Skip();
        }

        private static object ResolveSingleton(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (list.Count == 1)
            {
                return list[0];
            }

            return list;
        }

        private static string SnakeToPascal(string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                name, @"(^|_)([a-zA-Z])", m => m.Groups[2].Value.ToUpperInvariant());
        }

        private static System.Reflection.PropertyInfo FindProperty(Type type, string propertyName)
        {
            var flags = System.Reflection.BindingFlags.Public
                      | System.Reflection.BindingFlags.Instance
                      | System.Reflection.BindingFlags.IgnoreCase;
            var prop = type.GetProperty(propertyName, flags);
            if (prop == null && propertyName.Contains('_'))
            {
                prop = type.GetProperty(SnakeToPascal(propertyName), flags);
            }
            return prop;
        }

        private static bool HasProperty(Block block, string propertyName)
        {
            if (block == null || string.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            return FindProperty(block.GetType(), propertyName) != null;
        }

        private static object GetPropertyValue(Block block, string propertyName)
        {
            if (block == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            return FindProperty(block.GetType(), propertyName)?.GetValue(block);
        }
    }
}

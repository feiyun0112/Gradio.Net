using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Utils;

namespace Gradio.Net.Components
{
    [Events.Event("click")]
    [Events.Event("select")]
    public class Dataset : Component
    {
        public List<Component> Components { get; set; }
        public List<Dictionary<string, object>> ComponentProps { get; set; }
        public List<List<object>> Samples { get; set; }
        public List<List<object>> RawSamples { get; set; }
        public List<string> Headers { get; set; }
        public string Type { get; set; }
        public string? Layout { get; set; }
        public int SamplesPerPage { get; set; }
        public List<string>? SampleLabels { get; set; }
        public bool ShowLabel { get; set; }
        public bool Container { get; set; }
        public int? Scale { get; set; }
        public int MinWidth { get; set; }
        public string? ProxyUrl { get; set; }

        public Dataset(
            IEnumerable<Component>? components = null,
            List<List<object>>? samples = null,
            List<string>? headers = null,
            string type = "values",
            string? layout = null,
            int samplesPerPage = 10,
            object? label = null,
            bool showLabel = true,
            object? visible = null,
            string? elemId = null,
            object? elemClasses = null,
            bool render = true,
            object? key = null,
            object? preservedByKey = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            string? proxyUrl = null,
            List<string>? sampleLabels = null
        ) : base()
        {
            Container = container;
            Scale = scale;
            MinWidth = minWidth;
            Layout = layout;
            ShowLabel = showLabel;
            Components = components?.ToList() ?? new List<Component>();
            ComponentProps = new List<Dictionary<string, object>>();
            RawSamples = samples ?? new List<List<object>>();
            Samples = new List<List<object>>();
            Type = type;
            Label = label?.ToString();
            SamplesPerPage = samplesPerPage;
            SampleLabels = sampleLabels;
            ProxyUrl = proxyUrl;
            if (RawSamples.Count == 0)
            {
                RawSamples = new List<List<object>> { new List<object>() };
            }

            // Set properties that were previously passed to base constructor
            if (elemId != null) ElemId = elemId;
            if (elemClasses != null) ElemClasses = elemClasses as List<string> ?? new List<string>();
            if (visible != null) Visible = (bool)visible;
            if (key != null) Key = key;
            if (preservedByKey != null) PreservedByKey = preservedByKey as List<string> ?? new List<string>();

            var propsKeysToRemove = new[] { "value", "name", "_selectable" };
            foreach (var component in Components)
            {
                var props = component.GetConfig();
                foreach (var propKey in propsKeysToRemove)
                    props.Remove(propKey);
                ComponentProps.Add(props);
            }

            // Process samples
            foreach (var example in RawSamples)
            {
                var processedExample = new List<object>();
                for (int i = 0; i < Components.Count && i < example.Count; i++)
                {
                    var component = Components[i];
                    var ex = example[i];

                    // Process example if not from external space
                    if (ProxyUrl == null)
                    {
                        ex = component.AsExample(ex);
                    }

                    // Move files to cache
                    ex = Gradio.Net.Core.ProcessingUtils.MoveFilesToCache(ex, component, true);
                    processedExample.Add(ex);
                }
                Samples.Add(processedExample);
            }

            // Initialize headers
            if (headers != null)
            {
                Headers = headers;
            }
            else if (Components.All(c => c.Label == null))
            {
                Headers = new List<string>();
            }
            else
            {
                Headers = Components.Select(c => c.Label?.ToString() ?? "").ToList();
            }
        }

        public override Dictionary<string, object> GetConfig(Type? cls = null)
        {
            var config = base.GetConfig(cls);
            config["components"] = Components.Select(c => c.GetBlockName()).ToList();
            config["component_props"] = ComponentProps;
            config["sample_labels"] = SampleLabels;
            config["component_ids"] = Components.Select(c => c._id).ToList();
            config["type"] = Type;
            config["headers"] = Headers;
            config["samples_per_page"] = SamplesPerPage;
            config["layout"] = Layout;
            config["show_label"] = ShowLabel;
            config["container"] = Container;
            config["scale"] = Scale;
            config["min_width"] = MinWidth;
            return config;
        }

        public override object Preprocess(object payload)
        {
            if (payload is null)
            {
                return null;
            }

            if (int.TryParse(payload.ToString(), out var index))
            {
                if (index < 0 || index >= RawSamples.Count)
                {
                    return null;
                }

                switch (Type)
                {
                    case "index":
                        return index;
                    case "values":
                        return RawSamples[index];
                    case "tuple":
                        return (index, RawSamples[index]);
                    default:
                        return index;
                }
            }
            return payload;
        }

        public override object Postprocess(object value)
        {
            if (value is null || value is int)
            {
                return value;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var target = new List<object>();
                foreach (var item in enumerable)
                {
                    target.Add(item);
                }

                for (int i = 0; i < Samples.Count; i++)
                {
                    if (Samples[i].Count != target.Count)
                    {
                        continue;
                    }

                    bool same = true;
                    for (int j = 0; j < target.Count; j++)
                    {
                        var left = Samples[i][j];
                        var right = target[j];
                        if (!Equals(left, right) && !string.Equals(left?.ToString(), right?.ToString(), StringComparison.Ordinal))
                        {
                            same = false;
                            break;
                        }
                    }

                    if (same)
                    {
                        return i;
                    }
                }

                return null;
            }

            return value;
        }

        public override Dictionary<string, object> ApiInfo()
        {
            return new Dictionary<string, object>
            {
                { "type", "integer" },
                { "description", "index of selected example" }
            };
        }

        public override object ExamplePayload()
        {
            return 0;
        }

        public override object ExampleValue()
        {
            return new List<object>();
        }


    }
}

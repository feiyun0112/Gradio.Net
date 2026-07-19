using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Events;

namespace Gradio.Net.Components
{
    [Event("clear")]
    [Event("change")]
    [Event("stream")]
    [Event("select")]
    [Event("upload")]
    [Event("input")]
    public class Image : Component, IStreamingInput
    {
        public string Format { get; set; }
        public object Height { get; set; }
        public object Width { get; set; }
        public string ImageMode { get; set; }
        public List<string> Sources { get; set; }
        public string Type { get; set; }
        public List<string> Buttons { get; set; }
        public WebcamOptions WebcamOptions { get; set; }
        public string Placeholder { get; set; }
        public WatermarkOptions Watermark { get; set; }
        public object Streaming { get; set; }

        public Image(
            object value = null,
            string format = "webp",
            object height = null,
            object width = null,
            string imageMode = "RGB",
            object sources = null,
            string type = "numpy",
            string label = null,
            object every = null,
            object inputs = null,
            bool? showLabel = null,
            List<string> buttons = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool? interactive = null,
            object visible = null,
            bool streaming = false,
            string elemId = null,
            object elemClasses = null,
            bool render = true,
            object key = null,
            object preservedByKey = null,
            WebcamOptions webcamOptions = null,
            string placeholder = null,
            WatermarkOptions watermark = null)
        {
            Value = value;
            Label = label;
            ShowLabel = showLabel ?? true;
            Container = container;
            Scale = scale;
            MinWidth = minWidth;
            Interactive = interactive;
            Format = format;
            Height = height;
            Width = width;
            ImageMode = imageMode;

            // Validate and set type
            var validTypes = new List<string> { "numpy", "pil", "filepath" };
            if (!validTypes.Contains(type))
            {
                throw new ArgumentException($"`type` must be one of: {string.Join(", ", validTypes)}");
            }
            Type = type;

            // Validate and set sources
            var validSources = new List<string> { "upload", "webcam", "clipboard" };
            if (sources == null)
            {
                Sources = streaming ? new List<string> { "webcam" } : validSources;
            }
            else if (sources is string sourceStr && validSources.Contains(sourceStr))
            {
                Sources = new List<string> { sourceStr };
            }
            else if (sources is List<string> sourceList)
            {
                Sources = sourceList;
            }
            else if (sources is IEnumerable<string> sourceEnum)
            {
                Sources = new List<string>(sourceEnum);
            }
            else
            {
                throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
            }

            Buttons = buttons ?? new List<string> { "download", "share", "fullscreen" };
            WebcamOptions = webcamOptions ?? new WebcamOptions();
            Placeholder = placeholder;
            Watermark = watermark ?? new WatermarkOptions();
            Streaming = streaming;

            if (streaming && (Sources.Count != 1 || Sources[0] != "webcam"))
            {
                throw new ArgumentException("Image streaming only available if sources is ['webcam']. Streaming not supported with multiple sources.");
            }

            if (Value != null)
            {
                var postprocessedValue = Postprocess(Value);
                if (postprocessedValue is FileData fileData)
                {
                    postprocessedValue = fileData.ToDictionary();
                }

                Value = ProcessingUtils.MoveFilesToCache(
                    postprocessedValue,
                    this,
                    postprocess: true,
                    keepInCache: true);
            }
        }

        public override string GetBlockName()
        {
            return "image";
        }

        public override Dictionary<string, object> GetConfig(Type cls = null)
        {
            var config = base.GetConfig(cls);
            config["format"] = Format;
            config["height"] = Height;
            config["width"] = Width;
            config["image_mode"] = ImageMode;
            config["sources"] = Sources;
            config["type"] = Type;
            config["buttons"] = Buttons;
            config["webcam_options"] = WebcamOptions;
            config["placeholder"] = Placeholder;
            config["watermark"] = Watermark;
            config["streaming"] = Streaming;
            config["streamable"] = false;
            return config;
        }

        public override object Preprocess(object payload)
        {
            if (payload == null)
                return null;

            if (payload is FileData fileData)
            {
                payload = new ImageData
                {
                    Path = fileData.Path,
                    Url = fileData.Url,
                    OrigName = fileData.OrigName,
                    MimeType = fileData.MimeType
                };
            }

            return global::Gradio.Net.ImageUtils.PreprocessImage(
                payload,
                cacheDir: GRADIO_CACHE,
                format: Format,
                imageMode: ImageMode,
                type: Type);
        }

        public override object Postprocess(object value)
        {
            if (value == null)
                return null;

            return global::Gradio.Net.ImageUtils.PostprocessImage(
                value,
                cacheDir: GRADIO_CACHE,
                format: Format,
                watermark: Watermark);
        }

        public override Dictionary<string, object> ApiInfoAsOutput()
        {
            if (Streaming is string s && s == "base64")
            {
                return new Dictionary<string, object>
                {
                    ["type"] = "object",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["url"] = new Dictionary<string, object> { ["type"] = "string" }
                    }
                };
            }

            return ApiInfo();
        }

        public override Dictionary<string, object> ApiInfo()
        {
            return new Dictionary<string, object>
            {
                ["properties"] = new Dictionary<string, object>
                {
                    ["path"] = new Dictionary<string, object>
                    {
                        ["anyOf"] = new List<object>
                        {
                            new Dictionary<string, object> { ["type"] = "string" },
                            new Dictionary<string, object> { ["type"] = "null" }
                        },
                        ["default"] = null,
                        ["description"] = "Path to a local file",
                        ["title"] = "Path"
                    },
                    ["url"] = new Dictionary<string, object>
                    {
                        ["anyOf"] = new List<object>
                        {
                            new Dictionary<string, object> { ["type"] = "string" },
                            new Dictionary<string, object> { ["type"] = "null" }
                        },
                        ["default"] = null,
                        ["description"] = "Publicly available url or base64 encoded image",
                        ["title"] = "Url"
                    },
                    ["size"] = new Dictionary<string, object>
                    {
                        ["anyOf"] = new List<object>
                        {
                            new Dictionary<string, object> { ["type"] = "integer" },
                            new Dictionary<string, object> { ["type"] = "null" }
                        },
                        ["default"] = null,
                        ["description"] = "Size of image in bytes",
                        ["title"] = "Size"
                    },
                    ["orig_name"] = new Dictionary<string, object>
                    {
                        ["anyOf"] = new List<object>
                        {
                            new Dictionary<string, object> { ["type"] = "string" },
                            new Dictionary<string, object> { ["type"] = "null" }
                        },
                        ["default"] = null,
                        ["description"] = "Original filename",
                        ["title"] = "Orig Name"
                    },
                    ["mime_type"] = new Dictionary<string, object>
                    {
                        ["anyOf"] = new List<object>
                        {
                            new Dictionary<string, object> { ["type"] = "string" },
                            new Dictionary<string, object> { ["type"] = "null" }
                        },
                        ["default"] = null,
                        ["description"] = "mime type of image",
                        ["title"] = "Mime Type"
                    },
                    ["is_stream"] = new Dictionary<string, object>
                    {
                        ["default"] = false,
                        ["description"] = "Can always be set to False",
                        ["title"] = "Is Stream",
                        ["type"] = "boolean"
                    },
                    ["meta"] = new Dictionary<string, object>
                    {
                        ["additionalProperties"] = true,
                        ["default"] = new Dictionary<string, object> { ["_type"] = "gradio.FileData" },
                        ["title"] = "Meta",
                        ["type"] = "object"
                    }
                },
                ["title"] = "ImageData",
                ["type"] = "object",
                ["additional_description"] = "For input, either path or url must be provided. For output, path is always provided."
            };
        }

        public override object ExamplePayload()
        {
            return new Dictionary<string, object>
            {
                ["path"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png",
                ["meta"] = new Dictionary<string, object> { ["_type"] = "gradio.FileData" },
                ["orig_name"] = "bus.png",
                ["url"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png"
            };
        }

        public override object ExampleValue()
        {
            return "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";
        }

        // IStreamingInput implementation
        public void CheckStreamable()
        {
            if (Streaming is bool b && b && (Sources.Count != 1 || Sources[0] != "webcam"))
            {
                throw new ArgumentException("Image streaming only available if sources is ['webcam']. Streaming not supported with multiple sources.");
            }
        }
    }
}

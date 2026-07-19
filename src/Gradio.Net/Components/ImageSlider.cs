using System.Collections;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("clear")]
[Events.Event("change")]
[Events.Event("stream")]
[Events.Event("select")]
[Events.Event("upload")]
[Events.Event("input")]
public class ImageSlider : Component
{
    public string Format { get; set; }
    public object? Height { get; set; }
    public object? Width { get; set; }
    public string? ImageMode { get; set; }
    public string Type { get; set; }
    public List<string> Buttons { get; set; }
    public double SliderPosition { get; set; }
    public int MaxHeight { get; set; }

    public ImageSlider(
        object? value = null,
        string format = "webp",
        object? height = null,
        object? width = null,
        string? imageMode = "RGB",
        string type = "numpy",
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        List<string>? buttons = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        double sliderPosition = 50,
        int maxHeight = 500)
    {
        var validTypes = new[] { "numpy", "pil", "filepath" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        Format = format;
        Height = height;
        Width = width;
        ImageMode = imageMode;
        Type = type;
        Buttons = buttons ?? new List<string> { "download", "fullscreen" };
        SliderPosition = sliderPosition;
        MaxHeight = maxHeight;

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "imageslider";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["format"] = Format;
        config["height"] = Height;
        config["width"] = Width;
        config["image_mode"] = ImageMode;
        config["type"] = Type;
        config["buttons"] = Buttons;
        config["slider_position"] = SliderPosition;
        config["max_height"] = MaxHeight;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var root = ExtractRoot(payload);
        if (root == null)
        {
            return null;
        }

        return new object?[]
        {
            global::Gradio.Net.ImageUtils.PreprocessImage(root[0], GRADIO_CACHE, Format, ImageMode ?? "RGB", Type),
            global::Gradio.Net.ImageUtils.PreprocessImage(root[1], GRADIO_CACHE, Format, ImageMode ?? "RGB", Type)
        };
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string)
        {
            throw new Error("ImageSlider output must be a tuple/list of two images.");
        }

        object?[] items;
        if (value is object[] arr && arr.Length >= 2)
        {
            items = arr;
        }
        else if (value is IList list && list.Count >= 2)
        {
            items = new[] { list[0], list[1] };
        }
        else
        {
            throw new Error("ImageSlider output must contain exactly two images.");
        }

        return new SliderData
        {
            Root = new object?[]
            {
                global::Gradio.Net.ImageUtils.PostprocessImage(items[0], GRADIO_CACHE, Format),
                global::Gradio.Net.ImageUtils.PostprocessImage(items[1], GRADIO_CACHE, Format)
            }
        };
    }

    public override Dictionary<string, object> ApiInfoAsOutput()
    {
        return ApiInfo();
    }

    public override object ExamplePayload()
    {
        var url = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";
        return new object[]
        {
            new Dictionary<string, object> { ["path"] = url, ["url"] = url },
            new Dictionary<string, object> { ["path"] = url, ["url"] = url }
        };
    }

    public override object ExampleValue()
    {
        var url = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";
        return new object[] { url, url };
    }

    private static object?[]? ExtractRoot(object payload)
    {
        if (payload is SliderData sd && sd.Root != null && sd.Root.Length >= 2)
        {
            return new[] { sd.Root[0], sd.Root[1] };
        }

        if (payload is Dictionary<string, object> dict && dict.TryGetValue("root", out var rootObj))
        {
            if (rootObj is object[] arr && arr.Length >= 2)
            {
                return new[] { arr[0], arr[1] };
            }

            if (rootObj is IList list && list.Count >= 2)
            {
                return new[] { list[0], list[1] };
            }
        }

        if (payload is IList directList && directList.Count >= 2)
        {
            return new[] { directList[0], directList[1] };
        }

        return null;
    }
}

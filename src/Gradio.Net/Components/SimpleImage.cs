using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("clear")]
[Events.Event("change")]
[Events.Event("upload")]
public class SimpleImage : Component
{
    public bool ShowDownloadButton { get; set; }

    public SimpleImage(
        string? value = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool showDownloadButton = true,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        ShowDownloadButton = showDownloadButton;

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

    public override string GetBlockName() => "simpleimage";

    public override object? Preprocess(object? payload)
    {
        if (payload is FileData fd) return fd.Path;
        return payload;
    }

    public override object? Postprocess(object? value)
    {
        if (value == null) return null;
        var path = value.ToString() ?? string.Empty;
        return new FileData { Path = path, OrigName = Path.GetFileName(path) };
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
        {
            ["path"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png",
            ["url"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png"
        };
    }

    public override object ExampleValue()
    {
        return "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png";
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["show_download_button"] = ShowDownloadButton;
        return config;
    }
}

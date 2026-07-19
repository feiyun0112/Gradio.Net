using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("click")]
public class DownloadButton : Component
{
    public string Variant { get; set; }
    public string Size { get; set; }
    public object? Icon { get; set; }

    public DownloadButton(
        string label = "Download",
        object? value = null,
        object? every = null,
        object? inputs = null,
        string variant = "secondary",
        object? visible = null,
        string size = "lg",
        string? icon = null,
        int? scale = null,
        int? minWidth = null,
        bool interactive = true,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        Label = label;
        Value = value;
        Variant = variant;
        Size = size;
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

        Icon = ServeStaticFile(icon);
    }

    public override string GetBlockName() => "downloadbutton";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["value"] = Value;
        config["variant"] = Variant;
        config["size"] = Size;
        config["icon"] = Icon;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        if (payload is FileData fd)
        {
            return fd.Path;
        }

        if (payload is Dictionary<string, object> dict && dict.TryGetValue("path", out var p))
        {
            return p?.ToString();
        }

        return payload.ToString();
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var path = value.ToString();
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        return new FileData
        {
            Path = path,
            OrigName = Path.GetFileName(path)
        };
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
        {
            ["path"] = "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf",
            ["url"] = "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf"
        };
    }

    public override object ExampleValue()
    {
        return "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";
    }

    public override bool SkipApi => false;
}

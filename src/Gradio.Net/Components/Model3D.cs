using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("upload")]
[Events.Event("edit")]
[Events.Event("clear")]
public class Model3D : Component
{
    public string? DisplayMode { get; set; }
    public List<float> ClearColor { get; set; }
    public object CameraPosition { get; set; }
    public object? Height { get; set; }
    public double ZoomSpeed { get; set; }
    public double PanSpeed { get; set; }

    public Model3D(
        object? value = null,
        string? displayMode = null,
        object? clearColor = null,
        object? cameraPosition = null,
        double zoomSpeed = 1,
        double panSpeed = 1,
        object? height = null,
        string? label = null,
        bool? showLabel = null,
        object? every = null,
        object? inputs = null,
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
        DisplayMode = displayMode;
        ClearColor = ToColorList(clearColor) ?? new List<float> { 0, 0, 0, 0 };
        CameraPosition = cameraPosition ?? new object?[] { null, null, null };
        Height = height;
        ZoomSpeed = zoomSpeed;
        PanSpeed = panSpeed;

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

    public override string GetBlockName() => "model3d";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["display_mode"] = DisplayMode;
        config["clear_color"] = ClearColor;
        config["camera_position"] = CameraPosition;
        config["height"] = Height;
        config["zoom_speed"] = ZoomSpeed;
        config["pan_speed"] = PanSpeed;
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

        var s = value.ToString();
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        return new FileData
        {
            Path = s,
            OrigName = Path.GetFileName(s)
        };
    }

    public override object ProcessExample(object value)
    {
        var s = value?.ToString();
        return string.IsNullOrWhiteSpace(s) ? string.Empty : Path.GetFileName(s);
    }

    public override object ExamplePayload()
    {
        var url = "https://raw.githubusercontent.com/gradio-app/gradio/main/gradio/media_assets/models3d/Fox.gltf";
        return new Dictionary<string, object>
        {
            ["path"] = url,
            ["url"] = url
        };
    }

    public override object ExampleValue()
    {
        return "https://raw.githubusercontent.com/gradio-app/gradio/main/gradio/media_assets/models3d/Fox.gltf";
    }

    private static List<float>? ToColorList(object? clearColor)
    {
        if (clearColor is List<float> lf) return lf;
        if (clearColor is float[] fa) return new List<float>(fa);
        if (clearColor is double[] da) return new List<float>(Array.ConvertAll(da, x => (float)x));
        if (clearColor is object[] oa)
        {
            var list = new List<float>();
            foreach (var o in oa)
            {
                if (o == null) continue;
                if (float.TryParse(o.ToString(), out var f)) list.Add(f);
            }
            return list;
        }

        return null;
    }
}

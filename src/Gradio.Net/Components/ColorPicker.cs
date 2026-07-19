
namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("submit")]
[Events.Event("focus")]
[Events.Event("blur")]
public class ColorPicker : Component
{
    public ColorPicker(
        object? value = null,
        string? label = null,
        string? info = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
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
        Value = value;
        Label = label;
        Info = info;
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

    public override object? Preprocess(object? payload)
    {
        return payload?.ToString();
    }

    public override object? Postprocess(object? value)
    {
        return value?.ToString();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object> { ["type"] = "string" };
    }

    public override object ExamplePayload() => "#000000";

    public override object ExampleValue() => "#000000";
}

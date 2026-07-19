
namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("submit")]
public class SimpleTextbox : FormComponent
{
    public string? Placeholder { get; set; }
    public bool Rtl { get; set; }

    public SimpleTextbox(
        object? value = null,
        string? placeholder = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        bool rtl = false,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        Placeholder = placeholder;
        Rtl = rtl;

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
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

    public override string GetBlockName() => "simpletextbox";

    public override object? Preprocess(object? payload)
    {
        return payload == null ? null : payload.ToString();
    }

    public override object? Postprocess(object? value)
    {
        return value == null ? null : value.ToString();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object> { ["type"] = "string" };
    }

    public override object ExamplePayload() => "Hello!!";

    public override object ExampleValue() => "Hello!!";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["placeholder"] = Placeholder;
        config["rtl"] = Rtl;
        return config;
    }
}

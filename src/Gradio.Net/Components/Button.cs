namespace Gradio.Net.Components;

[Events.Event("click")]
[Events.Event("hover")]
[Events.Event("blur")]
[Events.Event("focus")]
public class Button : Component
{
    public Button(
        string? value = "Run",
        string? variant = "secondary",
        string? size = "lg",
        string? icon = null,
        bool? fullWidth = null,
        bool? interactive = null,
        string? link = null,
        string? linkTarget = "_self",
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        object? key = null,
        string? preservedByKey = null,
        int? scale = null,
        int? minWidth = null)
    {
        Value = value;
        Variant = variant;
        Size = size;
        Icon = icon;
        FullWidth = fullWidth;
        Interactive = interactive ?? true;
        Link = link;
        LinkTarget = linkTarget;
        Visible = visible;
        ShowLabel = false;
        Container = false;
        ElemId = elemId;
        ElemClasses = elemClasses ?? null;
        Key = key;
        PreservedByKey = preservedByKey != null ? new List<string> { preservedByKey } : new List<string> { "value" };
        Scale = scale;
        MinWidth = minWidth;
    }

    public string? Value { get; set; }
    public string? Variant { get; set; }
    public string? Size { get; set; }
    public string? Icon { get; set; }
    public bool? FullWidth { get; set; }
    public new bool? Interactive { get; set; }
    public string? Link { get; set; }
    public string? LinkTarget { get; set; }

    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var baseConfig = base.GetConfig(cls);
        var config = new Dictionary<string, object>
        {
            ["value"] = Value,
            ["variant"] = Variant,
            ["size"] = Size,
            ["icon"] = Icon,
            ["full_width"] = FullWidth,
            ["link"] = Link,
            ["link_target"] = LinkTarget,
            ["visible"] = Visible,
            ["interactive"] = Interactive,
            ["elem_classes"] = ElemClasses ?? new List<string>(),
            ["preserved_by_key"] = PreservedByKey ?? new List<string> { "value" },
        };
        if (baseConfig.TryGetValue("name", out var name)) config["name"] = name;
        if (baseConfig.TryGetValue("_selectable", out var selectable)) config["_selectable"] = selectable;
        if (!string.IsNullOrWhiteSpace(ElemId))
        {
            config["elem_id"] = ElemId;
        }
        return config;
    }

    public override object Preprocess(object input)
    {
        return input;
    }

    public override object Postprocess(object output)
    {
        return output?.ToString();
    }

    public override object ExamplePayload() => "Run";

    public override object ExampleValue() => "Run";
}

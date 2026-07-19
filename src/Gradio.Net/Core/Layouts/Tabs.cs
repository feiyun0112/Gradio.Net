namespace Gradio.Net.Core.Layouts;

[Events.Event("change")]
[Events.Event("select")]
public class Tabs : BlockContext
{
    public Tabs(
        object? selected = null,
        string? variant = null,
        bool? visible = null,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        Selected = selected;
        Variant = variant;
    }

    public object? Selected { get; set; }
    public string? Variant { get; set; }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["selected"] = Selected;
        if (Variant != null)
        {
            config["variant"] = Variant;
        }
        return config;
    }
}

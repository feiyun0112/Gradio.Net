namespace Gradio.Net.Core.Layouts;

[Events.Event("expand")]
[Events.Event("collapse")]
public class Accordion : BlockContext
{
    public Accordion(
        string? label = null,
        bool open = true,
        bool? visible = null,
        string? elemId = null,
        List<string>? elemClasses = null,
        bool render = true,
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        Label = label;
        Open = open;
    }

    public string? Label { get; set; }

    public bool Open { get; set; }

    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["open"] = Open;
        return config;
    }
}

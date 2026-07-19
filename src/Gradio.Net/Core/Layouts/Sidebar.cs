namespace Gradio.Net.Core.Layouts;

[Events.Event("expand")]
[Events.Event("collapse")]
public class Sidebar : BlockContext
{
    public Sidebar(
        string? label = null,
        bool open = true,
        bool? visible = null,
        string? elemId = null,
        List<string>? elemClasses = null,
        bool render = true,
        object? width = null,
        string position = "left",
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        if (position != "left" && position != "right")
        {
            throw new ArgumentException("position must be 'left' or 'right'.");
        }

        Label = label;
        Open = open;
        Width = width ?? 320;
        Position = position;
    }

    public string? Label { get; set; }
    public bool Open { get; set; }
    public object Width { get; set; }
    public string Position { get; set; }

    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["open"] = Open;
        config["width"] = Width;
        config["position"] = Position;
        return config;
    }
}

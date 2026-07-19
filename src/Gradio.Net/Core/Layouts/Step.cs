namespace Gradio.Net.Core.Layouts;

[Events.Event("select")]
public class Step : BlockContext
{
    public Step(
        string? label = null,
        bool visible = true,
        bool interactive = true,
        int? id = null,
        string? elemId = null,
        List<string>? elemClasses = null,
        int? scale = null,
        bool render = true,
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible, render, key, preservedByKey)
    {
        Label = label;
        Id = id;
        Visible = visible;
        Scale = scale;
        Interactive = interactive;
    }

    public string? Label { get; set; }
    public new int? Id { get; set; }
    public new bool Visible { get; set; }
    public int? Scale { get; set; }
    public bool Interactive { get; set; }

    public override Type? GetExpectedParent() => typeof(Walkthrough);
    public override string GetBlockName() => "walkthroughstep";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["id"] = Id;
        config["visible"] = Visible;
        config["scale"] = Scale;
        config["interactive"] = Interactive;
        return config;
    }
}

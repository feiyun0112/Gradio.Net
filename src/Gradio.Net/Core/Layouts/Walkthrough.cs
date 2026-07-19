namespace Gradio.Net.Core.Layouts;

[Events.Event("change")]
[Events.Event("select")]
public class Walkthrough : BlockContext
{
    public Walkthrough(
        int? selected = null,
        bool visible = true,
        string? elemId = null,
        List<string>? elemClasses = null,
        bool render = true,
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible, render, key, preservedByKey)
    {
        Selected = selected;
    }

    public int? Selected { get; set; }

    public override string GetBlockName() => "walkthrough";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["selected"] = Selected;
        return config;
    }
}


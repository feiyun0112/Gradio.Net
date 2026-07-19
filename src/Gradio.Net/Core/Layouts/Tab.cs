
namespace Gradio.Net.Core.Layouts;

[Events.Event("select")]
public class Tab : BlockContext
{
    public Tab(
        string? label = null,
        bool visible = true,
        bool interactive = true,
        object? id = null,
        string? elemId = null,
        List<string> elemClasses = null,
        int? scale = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null,
        bool renderChildren = false)
        : base(elemId: elemId, elemClasses: elemClasses, render: render, key: key, preservedByKey: preservedByKey)
    {
        Label = label;
        Id = id;
        Visible = visible;
        Scale = scale;
        Interactive = interactive;
        RenderChildren = renderChildren;
    }

    public string? Label { get; set; }
    public new object? Id { get; set; }
    public new bool Visible { get; set; }
    public int? Scale { get; set; }
    public bool Interactive { get; set; }
    public bool RenderChildren { get; set; }

    public override System.Type? GetExpectedParent() => typeof(Tabs);
    public override string GetBlockName() => "tabitem";

    public override Dictionary<string, object> GetConfig(System.Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["label"] = Label;
        config["id"] = Id;
        config["visible"] = Visible;
        config["scale"] = Scale;
        config["interactive"] = Interactive;
        config["render_children"] = RenderChildren;
        return config;
    }

    public static Dictionary<string, object> Update(bool? visible = null, bool? interactive = null)
    {
        return new Dictionary<string, object>
        {
            { "visible", visible },
            { "interactive", interactive },
            { "__type__", "update" }
        };
    }
}


namespace Gradio.Net.Core.Layouts;

public class Column : BlockContext
{
    public Column(
        int? scale = 1,
        int? minWidth = 320,
        string? variant = "default",
        bool? visible = null,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        Scale = scale;
        MinWidth = minWidth;
        Variant = variant;
        if (variant == "compact")
        {
            AllowExpectedParents = false;
        }
    }

    public int? Scale { get; set; }
    public int? MinWidth { get; set; }
    public string? Variant { get; set; }
    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["scale"] = Scale;
        config["min_width"] = MinWidth;
        config["variant"] = Variant;
        config["show_progress"] = ShowProgress;
        return config;
    }
}

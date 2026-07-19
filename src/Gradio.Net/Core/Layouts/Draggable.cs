namespace Gradio.Net.Core.Layouts;

public class Draggable : BlockContext
{
    public Draggable(
        string orientation = "column",
        bool? visible = null,
        string? elemId = null,
        List<string>? elemClasses = null,
        bool render = true,
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        var valid = new[] { "row", "column" };
        if (!valid.Contains(orientation))
        {
            throw new ArgumentException("orientation must be 'row' or 'column'.");
        }

        Orientation = orientation;
    }

    public string Orientation { get; set; }

    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["orientation"] = Orientation;
        return config;
    }
}

namespace Gradio.Net.Core.Layouts;

public class Row : BlockContext
{
    public Row(
        string? variant = "default",
        bool? equalHeight = false,
        int? scale = null,
        bool? visible = null,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? height = null,
        object? maxHeight = null,
        object? minHeight = null,
        bool showProgress = false,
        object? key = null,
        List<string> preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
        Variant = variant;
        EqualHeight = equalHeight;
        Scale = scale;
        Height = height;
        MaxHeight = maxHeight;
        MinHeight = minHeight;
        ShowProgress = showProgress;
        if (variant == "compact")
        {
            AllowExpectedParents = false;
        }
    }

    public string? Variant { get; set; }
    public bool? EqualHeight { get; set; }
    public int? Scale { get; set; }
    public object? Height { get; set; }
    public object? MaxHeight { get; set; }
    public object? MinHeight { get; set; }
    public override bool SkipApi => true;

    public static Dictionary<string, object> Update(bool? visible = null)
    {
        return new Dictionary<string, object>
        {
            ["visible"] = visible,
            ["__type__"] = "update"
        };
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["variant"] = Variant;
        config["equal_height"] = EqualHeight;
        config["scale"] = Scale;
        config["height"] = Height;
        config["max_height"] = MaxHeight;
        config["min_height"] = MinHeight;
        config["show_progress"] = ShowProgress;

        // Python parity key order for row in served config.
        return new Dictionary<string, object>
        {
            ["variant"] = config.ContainsKey("variant") ? config["variant"] : null,
            ["visible"] = config.ContainsKey("visible") ? config["visible"] : null,
            ["elem_classes"] = config.ContainsKey("elem_classes") ? config["elem_classes"] : ElemClasses,
            ["equal_height"] = config.ContainsKey("equal_height") ? config["equal_height"] : null,
            ["show_progress"] = config.ContainsKey("show_progress") ? config["show_progress"] : null,
            ["key"] = config.ContainsKey("key") ? config["key"] : Key,
            ["preserved_by_key"] = config.ContainsKey("preserved_by_key") ? config["preserved_by_key"] : new List<string>(),
            ["name"] = config.ContainsKey("name") ? config["name"] : GetBlockClass()
        };
    }
}

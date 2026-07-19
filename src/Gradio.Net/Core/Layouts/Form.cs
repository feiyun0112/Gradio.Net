namespace Gradio.Net.Core.Layouts;

public class Form : BlockContext
{
    public Form()
        : this(scale: 0, minWidth: 0, visible: true, elemId: null, elemClasses: null, render: true, key: null, preservedByKey: null)
    {
    }

    public Form(
        int? scale = 0,
        int? minWidth = 0,
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
    }

    public int? Scale { get; set; }
    public int? MinWidth { get; set; }
    public override bool SkipApi => true;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["preserved_by_key"] = PreservedByKey ?? new List<string>();
        config["scale"] = Scale;
        config["min_width"] = MinWidth;
        return new Dictionary<string, object>
        {
            ["scale"] = config.ContainsKey("scale") ? config["scale"] : 0,
            ["min_width"] = config.ContainsKey("min_width") ? config["min_width"] : 0,
            ["preserved_by_key"] = config.ContainsKey("preserved_by_key") ? config["preserved_by_key"] : new List<string>(),
            ["name"] = config.ContainsKey("name") ? config["name"] : GetBlockClass()
        };
    }

    public override void AddChild(Block child)
    {
        if (Parent is Row)
        {
            var childScale = TryGetIntProperty(child, "Scale");
            Scale = (Scale ?? 0) + (childScale ?? 1);
            MinWidth = (MinWidth ?? 0) + (TryGetIntProperty(child, "MinWidth") ?? 0);
        }
        else if (Parent is Column column && column.Parent is Row row && row.EqualHeight == true)
        {
            var childScale = TryGetIntProperty(child, "Scale");
            Scale = (Scale ?? 0) + (childScale ?? 1);
        }
        else if (Parent is Blocks parentBlocks && parentBlocks.FillHeight)
        {
            var childScale = TryGetIntProperty(child, "Scale");
            Scale = (Scale ?? 0) + (childScale ?? 0);
        }

        base.AddChild(child);
    }

    private static int? TryGetIntProperty(object target, string propertyName)
    {
        var prop = target.GetType().GetProperty(propertyName);
        if (prop == null || !prop.CanRead)
        {
            return null;
        }

        var value = prop.GetValue(target);
        if (value == null)
        {
            return null;
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return null;
        }
    }
}

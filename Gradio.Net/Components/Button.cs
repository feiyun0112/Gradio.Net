using Gradio.Net.Enums;

namespace Gradio.Net;

public class Button : Component, IHaveClickEvent
{
    internal Button() { }

    internal string Icon { get; set; }
    internal ButtonVariant? Variant { get; set; }
    internal ButtonSize? Size { get; set; }
    internal string Link { get; set; }
    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Value), "Run" },
        { nameof(Variant), ButtonVariant.Secondary },
        { nameof(Visible), true },
        { nameof(Interactive), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

}

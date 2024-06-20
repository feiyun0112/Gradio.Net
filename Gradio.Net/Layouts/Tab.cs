using Gradio.Net.Enums;

namespace Gradio.Net;

public class Tab : Blocks
{
    internal Tab() { }
    internal string Label { get; set; }
    internal string ComponentId { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
             { nameof(Visible), true},
              { nameof(Interactive), true},

          { nameof(Render), true },

    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;
    protected override string GetTypeName()
    {
        return "tabitem";
    }

}

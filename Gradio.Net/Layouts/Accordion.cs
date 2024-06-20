
using System.Reflection;

namespace Gradio.Net;

public class Accordion : Blocks
{
    internal Accordion() { }
    internal string Label { get; set; }
    internal bool? Open { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Open), true },
        { nameof(Visible), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;
}

using Gradio.Net.Enums;
using System.ComponentModel;

namespace Gradio.Net;

public class Group : Blocks
{
    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
           { nameof(Visible), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    internal Group() { }
}

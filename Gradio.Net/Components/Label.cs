using Gradio.Net.Models;

namespace Gradio.Net;

public class Label : Component
{
    internal Label() { }

    internal int? NumTopClasses { get; set; }
    internal string Color { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    { { nameof(Container), true },
        { nameof(MinWidth), 160 },
         { nameof(Visible), true},
          { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    internal override object PreProcess(object data)
    {
        string result = null;
        if (data == null)
        {
            return result;
        }

        result = data.ToString();
        return result;
    }
    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return new LabelData();
        }
        return new LabelData { Label = data.ToString() };
    }
}

using Gradio.Net.Enums;
using System.Runtime.Serialization;

namespace Gradio.Net;

public class State : Component, IHaveChangeEvent
{
    internal State() { }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Render), true },
    };

    internal decimal? TimeToLive { get; set; }
    [IgnoreDataMember]
    internal Action<object> DeleteCallback { get; set; }

    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "any valid json" } };
    }

}

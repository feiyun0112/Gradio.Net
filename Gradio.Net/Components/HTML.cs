namespace Gradio.Net;

public class HTML : Component
{
    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    { { nameof(Value), "" },
           { nameof(Visible), true },
        { nameof(ShowLabel), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    internal HTML()
    {
    }


    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }
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
            return null;
        }

        return data.ToString();
    }
}

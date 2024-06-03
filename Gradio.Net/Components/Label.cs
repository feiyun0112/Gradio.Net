using Gradio.Net.Models;

namespace Gradio.Net;

public class Label : Component
{
    internal Label() { }

    internal int? NumTopClasses { get;  set; }
    internal string Color { get;  set; }
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
        return new LabelData { Label= data.ToString() };
    }
}

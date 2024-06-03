namespace Gradio.Net;

public class Checkbox : FormComponent
{
    internal Checkbox() { }
    public static bool Payload(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is bool str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect bool actual {obj.GetType()}");
    }

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "bool" } };
    }

    internal override object PreProcess(object data)
    {            
        if (data == null)
        {
            return false;
        }

        bool result = bool.Parse(data.ToString());
        return result;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return false;
        }

        bool result = bool.Parse(data.ToString());
        return result;
    }
}

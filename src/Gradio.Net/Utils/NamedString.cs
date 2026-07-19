
namespace Gradio.Net.Utils;

public class NamedString
{
    public string Value { get; }

    public string Name { get; }

    public NamedString(string value)
    {
        Value = value;
        Name = value;
    }

    public static implicit operator string(NamedString namedString)
    {
        return namedString.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}

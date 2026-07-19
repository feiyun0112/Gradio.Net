
namespace Gradio.Net.Utils;

public class DynamicBoolean
{
    private bool _value;

    public DynamicBoolean(bool value)
    {
        _value = value;
    }

    public static implicit operator bool(DynamicBoolean db)
    {
        return db?._value ?? false;
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public void Set(bool value)
    {
        _value = value;
    }

    public void Set(int value)
    {
        _value = value != 0;
    }
}

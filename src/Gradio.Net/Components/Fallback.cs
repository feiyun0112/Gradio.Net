namespace Gradio.Net.Components;

public class Fallback : Component
{
    public override object? Preprocess(object? payload)
    {
        return payload;
    }

    public override object? Postprocess(object? value)
    {
        return value;
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object> { ["foo"] = "bar" };
    }

    public override object ExampleValue()
    {
        return new Dictionary<string, object> { ["foo"] = "bar" };
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any valid json"
        };
    }
}

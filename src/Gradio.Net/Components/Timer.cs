
namespace Gradio.Net.Components;

[Events.Event("tick")]
public class Timer : Component
{
    public bool Active { get; set; }

    public Timer(double value = 1, bool active = true, bool render = true)
    {
        Value = value;
        Active = active;
    }

    public override string GetBlockName() => "timer";

    public override object Preprocess(object input)
    {
        return input;
    }

    public override object Postprocess(object output)
    {
        return output;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "number"
        };
    }

    public override object ExamplePayload() => 1;

    public override object ExampleValue() => 1;

    public override bool BreaksGrouping() => false;
}

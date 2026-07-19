namespace Gradio.Net.Events;

public class CopyData : EventData
{
    public CopyData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Value = data.TryGetValue("value", out var value) ? value : null;
    }

    public object? Value { get; }
}

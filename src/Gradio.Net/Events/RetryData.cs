namespace Gradio.Net.Events;


public class RetryData : EventData
{
    public RetryData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Index = data.TryGetValue("index", out var index) ? index : null;
        Value = data.TryGetValue("value", out var value) ? value : null;
    }

    public object? Index { get; }

    public object? Value { get; }
}

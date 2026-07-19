namespace Gradio.Net.Events;

public class EditData : EventData
{
    public EditData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Index = data.TryGetValue("index", out var index) ? index : null;
        PreviousValue = data.TryGetValue("previous_value", out var previousValue) ? previousValue : null;
        Value = data.TryGetValue("value", out var value) ? value : null;
    }

    public object? Index { get; }

    public object? PreviousValue { get; }

    public object? Value { get; }
}

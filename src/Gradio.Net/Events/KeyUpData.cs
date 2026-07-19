namespace Gradio.Net.Events;

public class KeyUpData : EventData
{
    public KeyUpData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Key = data.TryGetValue("key", out var key) && key is string s ? s : string.Empty;
        InputValue = data.TryGetValue("input_value", out var inputValue) && inputValue is string iv ? iv : string.Empty;
    }

    public string Key { get; }

    public string InputValue { get; }
}


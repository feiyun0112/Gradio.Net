namespace Gradio.Net.Events;

public class SelectData : EventData
{
    public SelectData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Index = data.TryGetValue("index", out var index) ? index : null;
        Value = data.TryGetValue("value", out var value) ? value : null;
        RowValue = data.TryGetValue("row_value", out var rowValue) ? rowValue : null;
        ColValue = data.TryGetValue("col_value", out var colValue) ? colValue : null;
        Selected = data.TryGetValue("selected", out var selected) && selected is bool b ? b : true;
    }

    public object? Index { get; }

    public object? Value { get; }

    public object? RowValue { get; }

    public object? ColValue { get; }

    public bool Selected { get; }
}


namespace Gradio.Net.Events;

public class LikeData : EventData
{
    public LikeData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        Index = data.TryGetValue("index", out var index) ? index : null;
        Value = data.TryGetValue("value", out var value) ? value : null;
        Liked = data.TryGetValue("liked", out var liked) ? liked : true;
    }

    public object? Index { get; }

    public object? Value { get; }

    public object Liked { get; }
}


namespace Gradio.Net.Events;

public class EventData
{
    public EventData(Core.Block? target, object data)
    {
        Target = target;
        _data = data;
    }

    public Core.Block? Target { get; }

    protected readonly object _data;
}


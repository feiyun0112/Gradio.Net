namespace Gradio.Net.Events;

using Gradio.Net.Core;

public class EventListenerMethod
{
    public object? Block { get; set; }

    public string EventName { get; set; }

    public EventListenerMethod()
    {
        EventName = string.Empty;
    }

    public EventListenerMethod(object? block, string eventName)
    {
        Block = block;
        EventName = eventName;
    }

    public EventListenerMethod Copy()
    {
        return new EventListenerMethod(Block, EventName);
    }
}

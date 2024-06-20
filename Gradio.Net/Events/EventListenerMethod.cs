namespace Gradio.Net;

internal class EventListenerMethod
{
    public EventListenerMethod(Block block, string eventName)
    {
        Block = block;
        EventName = eventName;
    }

    internal Block Block { get; set; }
    internal string EventName { get; set; }
}

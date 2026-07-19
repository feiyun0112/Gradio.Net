namespace Gradio.Net.Events;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventAttribute : Attribute
{
    public EventAttribute(string eventName)
    {
        EventName = eventName;
    }

    public string EventName { get; set; }
    public Delegate? Handler { get; set; }
}

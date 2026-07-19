using Gradio.Net.Events;

namespace Gradio.Net.Core;

public abstract class ComponentMeta : IComponentMeta
{
    public abstract List<EventListener> EVENTS { get; }

    protected ComponentMeta()
    {
        if (EVENTS == null)
            throw new ComponentDefinitionError($"{GetType().Name} or its base classes must define an EVENTS list. If no events are supported, set it to an empty list.");

        var processedEvents = new List<EventListener>();
        foreach (var @event in EVENTS)
        {
            if (@event is EventListener eventListener)
            {
                var copiedEvent = eventListener.Copy();
                copiedEvent.SetDoc(component: GetType().Name);
                processedEvents.Add(copiedEvent);
            }
            else
            {
                throw new ComponentDefinitionError($"All events for {GetType().Name} must either be an string or an instance of EventListener.");
            }
        }

        ComponentMetaUtils.CreateOrModifyPyi(GetType(), GetType().Name, processedEvents);
    }
}

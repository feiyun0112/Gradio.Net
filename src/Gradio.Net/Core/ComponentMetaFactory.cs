using System.Reflection;
using Gradio.Net.Components;
using Gradio.Net.Events;

namespace Gradio.Net.Core;

public static class ComponentMetaFactory
{
    // Cache of already-initialized types to avoid redundant file I/O on every instantiation.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, bool> _initializedTypes = new();

    /// <summary>///</summary>
    public static void InitializeComponent(Type componentType)
    {
        // Fast path: skip if already initialized for this type.
        if (_initializedTypes.ContainsKey(componentType))
        {
            return;
        }
        var eventsField = componentType.GetField("EVENTS", BindingFlags.Static | BindingFlags.Public);
        if (eventsField == null)
        {
            bool found = false;
            var baseType = componentType.BaseType;
            while (baseType != null)
            {
                if (baseType.GetField("EVENTS", BindingFlags.Static | BindingFlags.Public) != null)
                {
                    found = true;
                    break;
                }
                baseType = baseType.BaseType;
            }

            if (!found)
            {
                throw new ComponentDefinitionError($"{componentType.Name} or its base classes must define an EVENTS list. If no events are supported, set it to an empty list.");
            }
        }

        var eventsValue = eventsField?.GetValue(null);
        var events = new List<EventListener>();
        var eventsForField = new List<object>();

        if (eventsValue is IEnumerable<object> eventsEnumerable)
        {
            foreach (var @event in eventsEnumerable)
            {
                if (@event is string eventName)
                {
                    var listener = new EventListener(eventName);
                    events.Add(listener);
                    eventsForField.Add(listener);
                }
                else if (@event is EventListener eventListener)
                {
                    var copied = eventListener.Copy();
                    events.Add(copied);
                    eventsForField.Add(copied);
                }
                else
                {
                    throw new ComponentDefinitionError($"All events for {componentType.Name} must either be an string or an instance of EventListener.");
                }
            }
        }

        foreach (var @event in events)
        {
            @event.SetDoc(component: componentType.Name);
        }

        if (eventsField != null)
        {
            eventsField.SetValue(null, eventsForField);
        }

        ComponentMetaUtils.AddEventListenersToComponent(componentType, events);

        ComponentMetaUtils.CreateOrModifyPyi(componentType, componentType.Name, events);

        // Mark as initialized so subsequent instantiations skip all of the above.
        _initializedTypes[componentType] = true;
    }
}

using Gradio.Net.Events;
using Gradio.Net.Components;

namespace Gradio.Net.Core;

public static class RenderDecorator
{
    public static Func<Delegate, Delegate> Render(
        object inputs = null,
        object triggers = null,
        bool queue = true,
        string triggerMode = "always_last",
        object concurrencyLimit = null,
        string concurrencyId = null,
        string showProgress = "full"
    )
    {
        var blocksContext = Context.GetBlocksContext();
        if (blocksContext == null)
            throw new InvalidOperationException("Reactive render must be inside a Blocks context.");

        var inputList = new List<Component>();
        if (inputs is Component component)
            inputList.Add(component);
        else if (inputs is IEnumerable<Component> components)
            inputList.AddRange(components);

        var triggerList = new List<EventListenerMethod>();
        if (triggers == null)
        {
            triggerList.Add(new EventListenerMethod(blocksContext.RootBlock, "load"));
            foreach (var input in inputList)
            {
                if (HasChangeEvent(input))
                    triggerList.Add(new EventListenerMethod(input, "change"));
            }
        }
        else
        {
            var triggerEnumerable = triggers is IEnumerable<EventListener> triggerEnum
                ? triggerEnum
                : new List<EventListener> { (EventListener)triggers };

            foreach (var trigger in triggerEnumerable)
            {
                object triggerBlock = null;
                if (trigger.HasTrigger)
                    triggerBlock = GetTriggerSelf(trigger);

                triggerList.Add(new EventListenerMethod(triggerBlock, trigger.EventName));
                trigger.Callback?.Invoke(triggerBlock);
            }
        }

        return fn =>
        {
            new Renderable(fn, inputList, triggerList, concurrencyLimit, concurrencyId, triggerMode, queue, showProgress);
            return fn;
        };
    }

    private static bool HasChangeEvent(Component component) => true;

    private static object GetTriggerSelf(EventListener trigger)
        => trigger.GetType().GetProperty("Self")?.GetValue(trigger);
}

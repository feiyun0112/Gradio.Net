using Gradio.Net.Events;

namespace Gradio.Net.Core;

public interface IComponentMeta
{
    List<EventListener> EVENTS { get; }
}

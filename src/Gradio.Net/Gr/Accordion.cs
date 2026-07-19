using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Accordion Accordion(
        string? label = null,
        bool open = true,
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var accordion = new Core.Layouts.Accordion(
            label: label,
            open: open,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        accordion.Enter();
        return accordion;
    }
}

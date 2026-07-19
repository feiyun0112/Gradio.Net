using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Draggable Draggable(
        string orientation = "column",
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var draggable = new Core.Layouts.Draggable(
            orientation: orientation,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        draggable.Enter();
        return draggable;
    }
}

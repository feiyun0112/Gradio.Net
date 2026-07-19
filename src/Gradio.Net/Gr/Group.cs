using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Group Group(
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var group = new Core.Layouts.Group(
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        group.Enter();
        return group;
    }
}

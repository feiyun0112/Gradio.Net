using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Form Form(
        int? scale = 0,
        int? minWidth = 0,
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var form = new Core.Layouts.Form(
            scale: scale,
            minWidth: minWidth,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        form.Enter();
        return form;
    }
}

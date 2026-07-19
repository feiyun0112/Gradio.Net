using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Column Column(
        int scale = 1,
        int minWidth = 320,
        string variant = "default",
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var column = new Core.Layouts.Column(
            scale: scale,
            minWidth: minWidth,
            variant: variant,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        column.Enter();
        return column;
    }
}

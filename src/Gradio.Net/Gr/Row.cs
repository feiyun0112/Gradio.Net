using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Row Row(
        string variant = "default",
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        bool equalHeight = false,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var row = new Core.Layouts.Row(
            variant: variant,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            equalHeight: equalHeight,
            key: key,
            preservedByKey: preservedByKey);
        row.Enter();
        return row;
    }
}

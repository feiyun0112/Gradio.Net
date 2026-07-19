using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Walkthrough Walkthrough(
        int? selected = null,
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var walkthrough = new Core.Layouts.Walkthrough(
            selected: selected,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        walkthrough.Enter();
        return walkthrough;
    }

    public static Core.Layouts.Step Step(
        string? label = null,
        bool visible = true,
        bool interactive = true,
        int? id = null,
        string? elemId = null,
        List<string> elemClasses = null,
        int? scale = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null)
    {
        var step = new Core.Layouts.Step(
            label: label,
            visible: visible,
            interactive: interactive,
            id: id,
            elemId: elemId,
            elemClasses: elemClasses,
            scale: scale,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
        step.Enter();
        return step;
    }
}

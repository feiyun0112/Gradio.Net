using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.Sidebar Sidebar(
        string? label = null,
        bool open = true,
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? width = null,
        string position = "left",
        object? key = null,
        List<string> preservedByKey = null)
    {
        var sidebar = new Core.Layouts.Sidebar(
            label: label,
            open: open,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            width: width,
            position: position,
            key: key,
            preservedByKey: preservedByKey);
        sidebar.Enter();
        return sidebar;
    }
}

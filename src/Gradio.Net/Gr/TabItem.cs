using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Layouts.TabItem TabItem(
        string? label = null,
        bool visible = true,
        bool interactive = true,
        object? id = null,
        string? elemId = null,
        List<string> elemClasses = null,
        int? scale = null,
        bool render = true,
        object? key = null,
        List<string> preservedByKey = null,
        bool renderChildren = false)
    {
        var tabItem = new Core.Layouts.TabItem(
            label: label,
            visible: visible,
            interactive: interactive,
            id: id,
            elemId: elemId,
            elemClasses: elemClasses,
            scale: scale,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            renderChildren: renderChildren);
        tabItem.Enter();
        return tabItem;
    }
}

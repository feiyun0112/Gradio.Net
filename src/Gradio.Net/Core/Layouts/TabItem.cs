
namespace Gradio.Net.Core.Layouts;

public class TabItem : Tab
{
    public TabItem(
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
        : base(label, visible, interactive, id, elemId, elemClasses, scale, render, key, preservedByKey, renderChildren)
    {
    }
}

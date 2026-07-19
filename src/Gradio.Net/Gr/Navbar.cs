using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Navbar Navbar(
        List<(string, string)> value = null,
        bool visible = true,
        object mainPageName = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null)
    {
        return new Navbar(
            value: value,
            visible: visible,
            mainPageName: mainPageName,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key);
    }
}

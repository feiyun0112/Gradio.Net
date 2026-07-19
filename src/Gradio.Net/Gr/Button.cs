using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Button Button(
        string value = "Run",
        string variant = "secondary",
        string size = "lg",
        string icon = null,
        bool? fullWidth = null,
        bool? interactive = null,
        string link = null,
        string linkTarget = "_self",
        bool visible = true,
        string elemId = null,
        List<string> elemClasses = null,
        object key = null,
        string preservedByKey = null,
        int? scale = null,
        int? minWidth = null)
    {
        return new Button(
            value: value,
            variant: variant,
            size: size,
            icon: icon,
            fullWidth: fullWidth,
            interactive: interactive,
            link: link,
            linkTarget: linkTarget,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            key: key,
            preservedByKey: preservedByKey,
            scale: scale,
            minWidth: minWidth);
    }
}

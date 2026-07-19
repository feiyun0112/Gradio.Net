using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static ClearButton ClearButton(
        object components = null,
        string value = "Clear",
        string variant = "secondary",
        string size = "lg",
        object icon = null,
        string link = null,
        bool visible = true,
        bool interactive = true,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        int? scale = null,
        int? minWidth = null,
        string apiName = null,
        string apiVisibility = "undocumented")
    {
        return new ClearButton(
            components: components,
            value: value,
            variant: variant,
            size: size,
            icon: icon,
            link: link,
            visible: visible,
            interactive: interactive,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            scale: scale,
            minWidth: minWidth,
            apiName: apiName,
            apiVisibility: apiVisibility);
    }
}

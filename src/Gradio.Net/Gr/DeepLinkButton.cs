using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static DeepLinkButton DeepLinkButton(
        string value = "Share via Link",
        string copiedValue = "Link Copied!",
        object inputs = null,
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
        object every = null)
    {
        return new DeepLinkButton(
            value: value,
            copiedValue: copiedValue,
            inputs: inputs,
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
            every: every);
    }
}

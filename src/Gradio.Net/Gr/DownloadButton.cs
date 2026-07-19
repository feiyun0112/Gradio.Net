using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static DownloadButton DownloadButton(
        string label = "Download",
        object value = null,
        object every = null,
        object inputs = null,
        string variant = "secondary",
        object visible = null,
        string size = "lg",
        string icon = null,
        int? scale = null,
        int? minWidth = null,
        bool interactive = true,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new DownloadButton(
            label: label,
            value: value,
            every: every,
            inputs: inputs,
            variant: variant,
            visible: visible,
            size: size,
            icon: icon,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

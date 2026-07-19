using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static DuplicateButton DuplicateButton(
        string value = "Duplicate Space",
        object every = null,
        object inputs = null,
        string variant = "huggingface",
        string size = "sm",
        object icon = null,
        string link = null,
        bool visible = true,
        bool interactive = true,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        int? scale = 0,
        int? minWidth = null,
        bool activate = true)
    {
        return new DuplicateButton(
            value: value,
            every: every,
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
            activate: activate);
    }
}

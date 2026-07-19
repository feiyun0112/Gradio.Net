using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static SimpleTextbox SimpleTextbox(
        object value = null,
        string placeholder = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        bool rtl = false,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new SimpleTextbox(
            value: value,
            placeholder: placeholder,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            rtl: rtl,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

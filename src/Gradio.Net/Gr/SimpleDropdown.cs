using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static SimpleDropdown SimpleDropdown(
        object choices = null,
        object value = null,
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new SimpleDropdown(
            choices: choices,
            value: value,
            label: label,
            info: info,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

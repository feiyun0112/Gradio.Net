using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Radio Radio(
        object? choices = null,
        object? value = null,
        string type = "value",
        string? label = null,
        string? info = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool rtl = false,
        List<object>? buttons = null)
    {
        return new Radio(
            choices: choices,
            value: value,
            type: type,
            label: label,
            info: info,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            rtl: rtl,
            buttons: buttons);
    }
}

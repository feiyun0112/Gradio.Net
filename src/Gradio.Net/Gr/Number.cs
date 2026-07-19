using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Number Number(
        object? value = null,
        string? label = null,
        string? placeholder = null,
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
        int? precision = null,
        double? minimum = null,
        double? maximum = null,
        double step = 1)
    {
        return new Number(
            value: value,
            label: label,
            placeholder: placeholder,
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
            precision: precision,
            minimum: minimum,
            maximum: maximum,
            step: step);
    }
}

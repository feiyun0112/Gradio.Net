using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Slider Slider(
        double minimum = 0,
        double maximum = 100,
        object? value = null,
        double? step = null,
        int? precision = null,
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
        bool randomize = false,
        List<string>? buttons = null,
        double? min = null,
        double? max = null)
    {
        return new Slider(
            minimum: minimum,
            maximum: maximum,
            value: value,
            step: step,
            precision: precision,
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
            randomize: randomize,
            buttons: buttons,
            min: min,
            max: max);
    }
}

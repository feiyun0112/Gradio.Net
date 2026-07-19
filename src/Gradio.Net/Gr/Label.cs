using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Label Label(
        object value = null,
        int? numTopClasses = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string color = null,
        bool showHeading = true)
    {
        return new Label(
            value: value,
            numTopClasses: numTopClasses,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            color: color,
            showHeading: showHeading);
    }
}

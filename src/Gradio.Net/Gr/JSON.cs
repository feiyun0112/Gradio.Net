using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static JsonComponent JSON(
        object value = null,
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
        bool open = false,
        bool showIndices = false,
        object height = null,
        object maxHeight = null,
        object minHeight = null,
        List<string> buttons = null)
    {
        return new JsonComponent(
            value: value,
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
            open: open,
            showIndices: showIndices,
            height: height,
            maxHeight: maxHeight,
            minHeight: minHeight,
            buttons: buttons);
    }
}

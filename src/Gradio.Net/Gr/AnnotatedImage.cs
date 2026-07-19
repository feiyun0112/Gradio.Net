using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static AnnotatedImage AnnotatedImage(
        object value = null,
        string format = "webp",
        bool showLegend = true,
        object height = null,
        object width = null,
        Dictionary<string, string> colorMap = null,
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
        List<string> buttons = null)
    {
        return new AnnotatedImage(
            value: value,
            format: format,
            showLegend: showLegend,
            height: height,
            width: width,
            colorMap: colorMap,
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
            buttons: buttons);
    }
}

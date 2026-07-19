using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static SimpleImage SimpleImage(
        string value = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool showDownloadButton = true,
        bool container = true,
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
        return new SimpleImage(
            value: value,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            showDownloadButton: showDownloadButton,
            container: container,
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

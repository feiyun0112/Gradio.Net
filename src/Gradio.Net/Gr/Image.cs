using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Image Image(
        object value = null,
        string format = "webp",
        object height = null,
        object width = null,
        string imageMode = "RGB",
        object sources = null,
        string type = "numpy",
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        List<string> buttons = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        bool streaming = false,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        WebcamOptions webcamOptions = null,
        string placeholder = null,
        WatermarkOptions watermark = null)
    {
        return new Image(
            value: value,
            format: format,
            height: height,
            width: width,
            imageMode: imageMode,
            sources: sources,
            type: type,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            buttons: buttons,
            container: container,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            streaming: streaming,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            webcamOptions: webcamOptions,
            placeholder: placeholder,
            watermark: watermark);
    }
}

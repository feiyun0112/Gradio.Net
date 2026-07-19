using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static ImageSlider ImageSlider(
        object value = null,
        string format = "webp",
        object height = null,
        object width = null,
        string imageMode = "RGB",
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
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        double sliderPosition = 50,
        int maxHeight = 500)
    {
        return new ImageSlider(
            value: value,
            format: format,
            height: height,
            width: width,
            imageMode: imageMode,
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
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            sliderPosition: sliderPosition,
            maxHeight: maxHeight);
    }
}

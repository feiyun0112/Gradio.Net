using Gradio.Net.Components;


namespace Gradio.Net;

public sealed class ImageMask : ImageEditor
{
    public bool IsTemplate { get; } = true;

    public ImageMask(
        object value = null,
        object height = null,
        object width = null,
        string imageMode = "RGBA",
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
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string placeholder = null,
        bool selectable = false,
        object transforms = null,
        Eraser eraser = null,
        Brush brush = null,
        string format = "webp",
        object layers = null,
        Tuple<int, int> canvasSize = null,
        bool fixedCanvas = false,
        WebcamOptions webcamOptions = null)
        : base(
            value: value,
            height: height,
            width: width,
            imageMode: imageMode,
            sources: sources ?? new List<string> { "upload", "webcam", "clipboard" },
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
            placeholder: placeholder,
            _selectable: selectable,
            transforms: transforms ?? new List<string> { "crop" },
            eraser: eraser,
            brush: brush ?? new Brush { Colors = new List<object> { "#000000" }, ColorMode = "fixed", DefaultColor = "#000000" },
            format: format,
            layers: layers ?? false,
            canvasSize: canvasSize ?? new Tuple<int, int>(800, 800),
            fixedCanvas: fixedCanvas,
            webcamOptions: webcamOptions)
    {
    }
}

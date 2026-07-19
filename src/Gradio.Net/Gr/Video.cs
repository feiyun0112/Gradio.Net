using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Video Video(
        object value = null,
        object sources = null,
        string format = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
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
        WebcamOptions webcamOptions = null,
        bool? includeAudio = null,
        bool autoplay = false,
        List<string> buttons = null,
        bool loop = false,
        bool streaming = false,
        object watermark = null,
        object subtitles = null,
        object height = null,
        object width = null)
    {
        return new Video(
            value: value,
            sources: sources,
            format: format,
            label: label,
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
            webcamOptions: webcamOptions,
            includeAudio: includeAudio,
            autoplay: autoplay,
            buttons: buttons,
            loop: loop,
            streaming: streaming,
            watermark: watermark,
            subtitles: subtitles,
            height: height,
            width: width);
    }
}

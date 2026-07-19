using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Audio Audio(
        object value = null,
        object sources = null,
        string type = "numpy",
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
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
        string format = null,
        bool autoplay = false,
        bool editable = true,
        List<string> buttons = null,
        WaveformOptions waveformOptions = null,
        bool loop = false,
        bool recording = false,
        bool streamable = false,
        double playbackPosition = 0,
        object subtitles = null)
    {
        return new Audio(
            value: value,
            sources: sources,
            type: type,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
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
            format: format,
            autoplay: autoplay,
            editable: editable,
            buttons: buttons,
            waveformOptions: waveformOptions,
            loop: loop,
            recording: recording,
            streamable: streamable,
            playbackPosition: playbackPosition,
            subtitles: subtitles);
    }
}

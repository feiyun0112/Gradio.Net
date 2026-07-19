using Gradio.Net.Components;


namespace Gradio.Net;

public sealed class Microphone : Audio
{
    public bool IsTemplate { get; } = true;

    public Microphone(
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
        string format = "wav",
        bool autoplay = false,
        List<string> buttons = null,
        bool editable = true,
        WaveformOptions waveformOptions = null,
        bool loop = false,
        bool recording = false,
        object subtitles = null)
        : base(
            value: value,
            sources: new List<string> { "microphone" },
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
            buttons: buttons,
            editable: editable,
            waveformOptions: waveformOptions,
            loop: loop,
            recording: recording,
            subtitles: subtitles)
    {
    }
}

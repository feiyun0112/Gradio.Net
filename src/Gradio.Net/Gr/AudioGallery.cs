using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static AudioGallery AudioGallery(
        List<string> audioUrls,
        string value = null,
        List<string> labels = null,
        int columns = 3,
        string label = null,
        object every = null,
        object inputs = null,
        bool showLabel = false,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new AudioGallery(
            audioUrls: audioUrls,
            value: value,
            labels: labels,
            columns: columns,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

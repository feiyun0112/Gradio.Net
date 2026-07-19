using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Dataset Dataset(
        IEnumerable<Component> components = null,
        List<List<object>> samples = null,
        List<string> headers = null,
        string type = "values",
        string layout = null,
        int samplesPerPage = 10,
        object label = null,
        bool showLabel = true,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        string proxyUrl = null,
        List<string> sampleLabels = null)
    {
        return new Dataset(
            components: components,
            samples: samples,
            headers: headers,
            type: type,
            layout: layout,
            samplesPerPage: samplesPerPage,
            label: label,
            showLabel: showLabel,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            container: container,
            scale: scale,
            minWidth: minWidth,
            proxyUrl: proxyUrl,
            sampleLabels: sampleLabels);
    }
}

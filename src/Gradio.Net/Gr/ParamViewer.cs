using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static ParamViewer ParamViewer(
        Dictionary<string, ParamViewerParameter> value = null,
        string language = "python",
        List<string> linkify = null,
        object every = null,
        object inputs = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string header = "Parameters",
        object anchorLinks = null,
        object maxHeight = null)
    {
        return new ParamViewer(
            value: value,
            language: language,
            linkify: linkify,
            every: every,
            inputs: inputs,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            header: header,
            anchorLinks: anchorLinks,
            maxHeight: maxHeight);
    }
}

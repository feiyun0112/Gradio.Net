using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Html Html(
        object value = null,
        string label = null,
        string htmlTemplate = "${value}",
        string cssTemplate = "",
        string jsOnLoad = "element.addEventListener('click', function() { trigger('click') });",
        bool applyDefaultCss = true,
        object every = null,
        object inputs = null,
        bool showLabel = false,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        int? minHeight = null,
        int? maxHeight = null,
        bool container = false,
        bool padding = false,
        bool autoscroll = false,
        Dictionary<string, object> props = null)
    {
        return new Html(
            value: value,
            label: label,
            htmlTemplate: htmlTemplate,
            cssTemplate: cssTemplate,
            jsOnLoad: jsOnLoad,
            applyDefaultCss: applyDefaultCss,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            minHeight: minHeight,
            maxHeight: maxHeight,
            container: container,
            padding: padding,
            autoscroll: autoscroll,
            props: props);
    }
}

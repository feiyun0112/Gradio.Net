using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Markdown Markdown(
        object value = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool rtl = false,
        List<Dictionary<string, object>> latexDelimiters = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        bool sanitizeHtml = true,
        bool lineBreaks = false,
        bool headerLinks = false,
        object height = null,
        object maxHeight = null,
        object minHeight = null,
        List<string> buttons = null,
        bool container = false,
        bool padding = false)
    {
        return new Markdown(
            value: value,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            rtl: rtl,
            latexDelimiters: latexDelimiters,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            sanitizeHtml: sanitizeHtml,
            lineBreaks: lineBreaks,
            headerLinks: headerLinks,
            height: height,
            maxHeight: maxHeight,
            minHeight: minHeight,
            buttons: buttons,
            container: container,
            padding: padding);
    }
}

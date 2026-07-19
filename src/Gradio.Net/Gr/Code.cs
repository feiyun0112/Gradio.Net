using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Code Code(
        object value = null,
        string language = null,
        object every = null,
        object inputs = null,
        int lines = 5,
        int? maxLines = null,
        string label = null,
        bool? interactive = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        bool wrapLines = false,
        bool showLineNumbers = true,
        bool autocomplete = false)
    {
        return new Code(
            value: value,
            language: language,
            every: every,
            inputs: inputs,
            lines: lines,
            maxLines: maxLines,
            label: label,
            interactive: interactive,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            wrapLines: wrapLines,
            showLineNumbers: showLineNumbers,
            autocomplete: autocomplete);
    }
}

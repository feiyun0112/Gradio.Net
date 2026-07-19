using Gradio.Net.Components;


namespace Gradio.Net;

public sealed class TextArea : Textbox
{
    public bool IsTemplate { get; } = true;

    public TextArea(
        object value = null,
        int lines = 7,
        int? maxLines = 20,
        object placeholder = null,
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        bool autofocus = false,
        bool autoscroll = true,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string type = "text",
        string textAlign = null,
        bool rtl = false,
        List<string> buttons = null,
        int? maxLength = null,
        object submitBtn = null,
        object stopBtn = null,
        InputHTMLAttributes htmlAttributes = null)
        : base(
            value: value,
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
            autofocus: autofocus,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            type: type,
            lines: lines,
            maxLines: maxLines,
            placeholder: placeholder,
            buttons: buttons,
            submitBtn: submitBtn,
            stopBtn: stopBtn,
            autoscroll: autoscroll,
            textAlign: textAlign,
            rtl: rtl,
            maxLength: maxLength,
            htmlAttributes: htmlAttributes)
    {
        Info = info;
    }
}

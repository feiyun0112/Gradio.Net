using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Dialogue Dialogue(
        object value = null,
        string type = "text",
        List<string> speakers = null,
        Func<string, string, string> formatter = null,
        Func<string, string, Dictionary<string, string>> unformatter = null,
        List<string> tags = null,
        string separator = "\n",
        Dictionary<string, string> colorMap = null,
        string label = "Dialogue",
        string info = "Type colon (:) in the dialogue line to see the available tags",
        string placeholder = null,
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
        int? maxLines = null,
        List<string> buttons = null,
        object submitBtn = null,
        string uiMode = "both")
    {
        return new Dialogue(
            value: value,
            type: type,
            speakers: speakers,
            formatter: formatter,
            unformatter: unformatter,
            tags: tags,
            separator: separator,
            colorMap: colorMap,
            label: label,
            info: info,
            placeholder: placeholder,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            autofocus: autofocus,
            autoscroll: autoscroll,
            elemClasses: elemClasses,
            render: render,
            key: key,
            maxLines: maxLines,
            buttons: buttons,
            submitBtn: submitBtn,
            uiMode: uiMode);
    }
}

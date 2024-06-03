using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static MultimodalTextbox MultimodalTextbox(
        Dictionary<string,object> value = null,
        IEnumerable<string> fileTypes =null,
        int lines = 1,
        int maxLines = 20,
        string placeholder = null,
        string label = null,
        string info = null,
        decimal? every = null,
        bool showLabel = true,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        bool visible = true,
        string elemId = null,
        bool autofocus = false,
        bool autoscroll = true,
        IEnumerable<string> elemClasses = null,
        bool render = true,
        TextboxTextAlign textAlign = TextboxTextAlign.Left,
        bool rtl = false,
        object submitBtn = null)
    {
        MultimodalTextbox blocks = new()
        {
            Value = value,
            Lines = lines,
            MaxLines = maxLines,
            Placeholder = placeholder,
            Label = label,
            Info = info,
            Every = every,
            ShowLabel = showLabel,
            Container = container,
            Scale = scale,
            MinWidth = minWidth,
            Interactive = interactive,
            Visible = visible,
            ElemId = elemId,
            Autofocus = autofocus,
            Autoscroll = autoscroll,
            ElemClasses = elemClasses,
            Render = render,
            TextAlign = textAlign,
            Rtl = rtl,
            SubmitBtn = submitBtn
        };
        Context.AddToCurrentBlocks(blocks);
        return blocks;
    }
}

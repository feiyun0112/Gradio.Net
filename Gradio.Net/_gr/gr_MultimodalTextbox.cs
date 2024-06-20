using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static MultimodalTextbox MultimodalTextbox(
        Dictionary<string, object> value = null,
        IEnumerable<string> fileTypes = null,
        int? lines = null,
        int? maxLines = null,
        string placeholder = null,
        string label = null,
        string info = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        string elemId = null,
        bool? autofocus = null,
        bool? autoscroll = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null,
        TextboxTextAlign? textAlign = null,
        bool? rtl = null,
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

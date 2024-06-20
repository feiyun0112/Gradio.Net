using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Button Button(
        string value = null,
        decimal? every = null,
        ButtonVariant? variant = null,
        ButtonSize? size = null,
        string icon = null,
        string link = null,
        bool? visible = null,
        bool? interactive = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null,
        int? scale = null,
        int? minWidth = null)
    {
        Button block = new()
        {
            Value = value,
            Every = every,
            Variant = variant,
            Size = size,
            Icon = icon,
            Link = link,
            Visible = visible,
            Interactive = interactive,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            Scale = scale,
            MinWidth = minWidth
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

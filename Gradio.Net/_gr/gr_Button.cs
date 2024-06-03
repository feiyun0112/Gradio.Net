using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Button Button(
        string value = "Run",
        decimal? every = null,
        ButtonVariant variant = ButtonVariant.Secondary,
        ButtonSize? size = null,
       string icon = null,
        string link = null,
        bool visible = true,
        bool interactive = true,
        string elemId = null,
       IEnumerable<string> elemClasses = null,
       bool render = true,
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

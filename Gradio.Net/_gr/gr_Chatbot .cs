namespace Gradio.Net;

public static partial class gr
{
    public static Chatbot Chatbot(
        IEnumerable<object> value = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null,
        string key = null,
        int? height = null,
        IEnumerable<Dictionary<string, object>> latexDelimiters = null,
        bool? rtl = null,
        bool? showShareButton = null,
        bool? showCopyButton = null,
        Tuple<string, string> avatarImages = null,
        bool? sanitizeHtml = null,
        bool? renderMarkdown = null,
        bool? bubbleFullWidth = null,
        bool? lineBreaks = null,
        bool? likeable = null,
        string layout = null,
        string placeholder = null)
    {
        Chatbot block = new();
        block.Value = value;
        block.Label = label;
        block.Every = every;
        block.ShowLabel = showLabel;
        block.Container = container;
        block.Scale = scale;
        block.MinWidth = minWidth;
        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;
        block.Key = key;
        block.Height = height;
        block.LatexDelimiters = latexDelimiters;
        block.Rtl = rtl;
        block.ShowShareButton = showShareButton;
        block.RenderMarkdown = renderMarkdown;
        block.ShowCopyButton = showCopyButton;
        block.AvatarImages = avatarImages;
        block.SanitizeHtml = sanitizeHtml;
        block.RenderMarkdown = renderMarkdown;
        block.BubbleFullWidth = bubbleFullWidth;
        block.LineBreaks = lineBreaks;
        block.Likeable = likeable;
        block.Layout = layout;
        block.Placeholder = placeholder;

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

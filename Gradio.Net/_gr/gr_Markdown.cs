namespace Gradio.Net;

public static partial class gr
{
    public static Markdown Markdown(string value = null,
    string label = null,
    decimal? every = null,
    bool? showLabel = null,
    bool? rtl = null,
    IEnumerable<Dictionary<string, object>> latex_delimiters = null,
    bool? visible = null,
    string elemId = null,
    IEnumerable<string> elemClasses = null,
    bool? render = null,
    string key = null,
    bool? sanitizeHtml = null,
    bool? lineBreaks = null,
    bool? headerLinks = null)
    {
        Markdown block = new()
        {
            Value = value,
            Label = label,
            Every = every,
            ShowLabel = showLabel,
            Rtl = rtl,
            LatexDelimiters = latex_delimiters,
            Visible = visible,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            Key = key,
            SanitizeHtml = sanitizeHtml,
            LineBreaks = lineBreaks,
            HeaderLinks = headerLinks,
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

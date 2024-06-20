namespace Gradio.Net;

public static partial class gr
{
    public static Blocks Blocks(string theme = null,
    bool? analyticsEnabled = null,
    string? mode = null,
    string title = null,
    string css = null,
    string js = null,
    string head = null,
    bool? fillHeight = null,
    Tuple<int, int> deleteCache = null)
    {
        Blocks blocks = new()
        {
            Theme = theme,
            AnalyticsEnabled = analyticsEnabled,
            Mode = mode,
            Title = title,
            Css = css,
            Js = js,
            Head = head,
            FillHeight = fillHeight,
            DeleteCache = deleteCache,
        };
        Context.SetCurrentBlocks(blocks);
        return blocks;
    }
}

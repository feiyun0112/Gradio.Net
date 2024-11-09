using Gradio.Net.Themes;

namespace Gradio.Net;

public static partial class gr
{
    public static Blocks Blocks(
    bool? analyticsEnabled = null,
    string? mode = null,
    string css = null,
    string js = null,
    string head = null,
    bool? fillHeight = null,
    Tuple<int, int> deleteCache = null)
    {
        Blocks blocks = new()
        {

            AnalyticsEnabled = analyticsEnabled,
            Mode = mode,
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

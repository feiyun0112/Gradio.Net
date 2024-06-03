namespace Gradio.Net;

public static partial class gr
{
    public static Blocks Blocks(string theme = "default",
    bool analyticsEnabled=true,
    string mode = "blocks",
    string title = "Gradio.Net",
    string css = null,
    string js = null,
    string head = null,
    bool fillHeight = false,
    Tuple<int,int> deleteCache=null)
    {
        Blocks blocks = new() {
            Theme= theme,
            AnalyticsEnabled= analyticsEnabled,
            Mode= mode,
            Title = title,
            Css= css,
            Js= js,
            Head= head,
            FillHeight= fillHeight,
            DeleteCache= deleteCache,
        };
        Context.SetCurrentBlocks(blocks);
        return blocks;
    }
}

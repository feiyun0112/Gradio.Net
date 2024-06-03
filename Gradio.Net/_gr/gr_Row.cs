using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Row Row(
        RowVariant variant = RowVariant.Default,
        bool visible = true,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool render = true,
        bool equalHeight = true)
    {
        Row blocks = new() {
            Variant= variant,
            Visible= visible,
            ElemId= elemId,
            ElemClasses= elemClasses,
            Render= render,
            EqualHeight= equalHeight,
        };
        Context.SetCurrentBlocks(blocks);
        return blocks;
    }
}

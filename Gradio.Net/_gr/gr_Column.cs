using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Column Column(
        int scale = 1,
        int minWidth = 320,
        ColumnVariant variant = ColumnVariant.Default,
        bool visible = true,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool render = true
    )
    {
        Column block = [];

        block.Scale = scale;
        block.MinWidth = minWidth;
        block.Variant = variant;

        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;

        Context.SetCurrentBlocks(block);

        return block;
    }
}

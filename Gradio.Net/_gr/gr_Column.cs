using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Column Column(
        int? scale = null,
        int? minWidth = null,
        ColumnVariant? variant = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null
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

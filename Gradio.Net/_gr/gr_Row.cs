using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Row Row(
        RowVariant? variant = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null,
        bool? equalHeight = null)
    {
        Row blocks = new()
        {
            Variant = variant,
            Visible = visible,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            EqualHeight = equalHeight,
        };
        Context.SetCurrentBlocks(blocks);
        return blocks;
    }
}

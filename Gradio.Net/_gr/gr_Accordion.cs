namespace Gradio.Net;

public static partial class gr
{
    public static Accordion Accordion(
        string label = null,
        bool? open = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null
    )
    {
        Accordion block = [];

        block.Label = label;
        block.Open = open;

        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;

        Context.SetCurrentBlocks(block);

        return block;
    }
}

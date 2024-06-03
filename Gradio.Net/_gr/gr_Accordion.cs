namespace Gradio.Net;

public static partial class gr
{
    public static Accordion Accordion(
        string label=null,        
        bool open = true,        
        bool visible = true,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool render = true
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

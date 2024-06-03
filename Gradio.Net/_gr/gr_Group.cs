namespace Gradio.Net;

public static partial class gr
{
    public static Group Group(            
        bool visible = true,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool render = true
    )
    {
        Group block = [];

        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;

        Context.SetCurrentBlocks(block);

        return block;
    }
}

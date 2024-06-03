namespace Gradio.Net;

public static partial class gr
{
    public static Tab Tab(string label = null,bool visible =true,bool interactive= true, string componentId = null, string elemId = null, string[] elemClasses = null,bool render= true)
    {
        Tab block = [];

        block.Label = label;
        block.Visible = visible;
        block.Interactive = interactive;
        block.ComponentId = componentId;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;

        Context.SetCurrentBlocks(block);

        return block;
    }
}

namespace Gradio.Net;

public static partial class gr
{
    public static Tab Tab(string label = null,
        bool? visible = null,
        bool? interactive = null,
        string componentId = null,
        string elemId = null,
        string[] elemClasses = null,
        bool? render = null)
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

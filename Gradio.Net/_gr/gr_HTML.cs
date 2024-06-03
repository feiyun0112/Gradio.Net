namespace Gradio.Net;

public static partial class gr
{
    public static HTML HTML(string value="",
    string label=null,
    decimal? every=null,
    bool showLabel=true,
    bool visible  = true,
    string elemId= null,
    IEnumerable<string> elemClasses=null)
    {
        HTML block = new() {
            Value= value,
            Label= label,
            Every= every,
            ShowLabel= showLabel,
            Visible= visible,
            ElemId= elemId,
           ElemClasses= elemClasses,
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

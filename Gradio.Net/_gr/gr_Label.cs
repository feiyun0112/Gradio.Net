namespace Gradio.Net;

public static partial class gr
{
    public static Label Label(
        string value = null,
        int? numTopClasses = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
         bool visible = true,
          string elemId = null,
           IEnumerable<string> elemClasses = null,
           bool render = true,
          string key=null,
          string color = null
       )
    {
        Label block = new()
        {
            Value = value,
            NumTopClasses= numTopClasses,
            Label= label,
            Every = every,
            ShowLabel= showLabel,
            Container= container,
            Scale = scale,
            MinWidth = minWidth,
            Visible = visible,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            Key= key,
            Color= color,
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

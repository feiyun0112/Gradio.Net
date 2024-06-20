namespace Gradio.Net;

public static partial class gr
{
    public static Label Label(
        string value = null,
        int? numTopClasses = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
         bool? visible = null,
          string elemId = null,
           IEnumerable<string> elemClasses = null,
           bool? render = null,
          string key = null,
          string color = null
       )
    {
        Label block = new()
        {
            Value = value,
            NumTopClasses = numTopClasses,
            Label = label,
            Every = every,
            ShowLabel = showLabel,
            Container = container,
            Scale = scale,
            MinWidth = minWidth,
            Visible = visible,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            Key = key,
            Color = color,
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

namespace Gradio.Net;

public static partial class gr
{
    public static Number Number(
        decimal? value = null,
        string label = null,
        string info = null,
        decimal? every = null,
        bool showLabel = true,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        bool visible = true,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool render = true,
        int? precision = null,
        decimal? minimum=null,
        decimal? maximum=null,
        decimal step =1)
    {
        Number blocks = new()
        {
            Value = value,
            Label = label,
            Info = info,
            Every = every,
            ShowLabel = showLabel,
            Container = container,
            Scale = scale,
            MinWidth = minWidth,
            Interactive = interactive,
            Visible = visible,
            ElemId = elemId,                
            ElemClasses = elemClasses,
            Render = render,
            Precision= precision,
            Minimum= minimum,
            Maximum= maximum,
            Step= step
        };
        Context.AddToCurrentBlocks(blocks);
        return blocks;
    }
}

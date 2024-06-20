namespace Gradio.Net;

public static partial class gr
{
    public static Number Number(
        decimal? value = null,
        string label = null,
        string info = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null,
        int? precision = null,
        decimal? minimum = null,
        decimal? maximum = null,
        decimal? step = null)
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
            Precision = precision,
            Minimum = minimum,
            Maximum = maximum,
            Step = step
        };
        Context.AddToCurrentBlocks(blocks);
        return blocks;
    }
}

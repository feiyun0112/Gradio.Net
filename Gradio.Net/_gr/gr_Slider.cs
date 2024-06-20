namespace Gradio.Net;

public static partial class gr
{

    public static Slider Slider(
        decimal? minimum = null,
        decimal? maximum = null,
        decimal? value = null,
        decimal? step = null,
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
        bool? randomize = null
        )
    {
        Slider block = new()
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
            Randomize = randomize,
        };
        block.Minimum = minimum;
        block.Maximum = maximum;
        block.Step = step;

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

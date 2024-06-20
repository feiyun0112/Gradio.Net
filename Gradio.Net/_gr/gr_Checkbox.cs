namespace Gradio.Net;

public static partial class gr
{
    public static Checkbox Checkbox(
        bool? value = null,
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
      string key = null
    )
    {
        Checkbox block = new();

        block.Label = label;
        block.Info = info;
        block.Every = every;
        block.ShowLabel = showLabel;
        block.Container = container;
        block.Scale = scale;
        block.MinWidth = minWidth;
        block.Interactive = interactive;
        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;
        block.Key = key;
        block.Value = value;

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

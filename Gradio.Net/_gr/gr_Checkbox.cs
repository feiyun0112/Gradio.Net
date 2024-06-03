namespace Gradio.Net;

public static partial class gr
{
    public static Checkbox Checkbox(
        bool value=false,
        string label=null,
        string info = null,
        decimal? every= null,
        bool? showLabel= null,
        bool  container =true,
       int? scale = null,
       int minWidth =160,
      bool?  interactive = null,
       bool  visible=true,
      string  elemId = null,
      IEnumerable<string>  elemClasses = null,
       bool  render=true,
      string   key=null
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

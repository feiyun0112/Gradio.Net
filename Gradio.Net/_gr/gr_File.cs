using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
        
    public static File File(
        string value = null,
        FileCount fileCount = FileCount.Single,
        IEnumerable<string> fileTypes = null,
        FileType type = FileType.Filepath,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,

        bool container = true,
        int? scale = null,
        int minWidth = 160,
        decimal? height = null,
        bool? interactive = null,
        bool visible = true,
       
        string elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        string key = null )
    {
        File block = new()
        {
            Value = value,
            FileCount = fileCount,
            FileTypes = fileTypes,
            Type = type,
            Label = label,
            Every = every,
            ShowLabel = showLabel,
            Container = container,
            Scale = scale,
            MinWidth = minWidth,
            Height = height,
            Interactive= interactive,
            Visible= visible,
            ElemId= elemId,
            ElemClasses= elemClasses,
            Render= render,
            Key= key 
        };
         

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

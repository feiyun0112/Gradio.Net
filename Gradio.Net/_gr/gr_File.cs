using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{

    public static File File(
        string value = null,
        FileCount? fileCount = null,
        IEnumerable<string> fileTypes = null,
        FileType? type = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,

        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        decimal? height = null,
        bool? interactive = null,
        bool? visible = null,

        string elemId = null,
        List<string> elemClasses = null,
        bool? render = null,
        string key = null)
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
            Interactive = interactive,
            Visible = visible,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Render = render,
            Key = key
        };


        Context.AddToCurrentBlocks(block);
        return block;
    }
}

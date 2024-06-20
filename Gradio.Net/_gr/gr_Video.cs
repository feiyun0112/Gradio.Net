using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Video Video(
        string value = null,
        string format = null,
        IEnumerable<VideoSource> sources = null,
        int? height = null,
        int? width = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        string elemId = null,
        List<string> elemClasses = null,
        bool? render = null,
        string key = null,
        bool? mirrorWebcam = null,
        bool? includeAudio = null,
        bool? autoplay = null,
        bool? showShareButton = null,
        bool? showDownloadButton = null,
        int? minLength = null,
        int? maxLength = null)
    {
        Video block = new()
        {
            Format = format,
            MirrorWebcam = mirrorWebcam,
            Height = height,
            Width = width,
            Sources = sources,
            Label = label,
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
            Key = key,
            Value = value,
            ShowShareButton = showShareButton,
            IncludeAudio = includeAudio,
            Autoplay = autoplay,
            ShowDownloadButton = showDownloadButton,
            MinLength = minLength,
            MaxLength = maxLength,
        };

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Image Image(
        string value = null,
        ImageFormat? format = null,
        int? height = null,
        int? width = null,
        ImageMode? imageMode = null,
        IEnumerable<ImageSource> sources = null,
        ImageType? type = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? showDownloadButton = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        bool? streaming = null,
        string elemId = null,
        List<string> elemClasses = null,
        bool? render = null,
        string key = null,
        bool? mirrorWebcam = null,
        bool? showShareButton = null)
    {
        Image block = new()
        {
            Format = format,
            MirrorWebcam = mirrorWebcam,
            Type = type,
            Height = height,
            Width = width,
            ImageMode = imageMode,
            Sources = sources,
            Streaming = streaming,
            ShowDownloadButton = showDownloadButton,
            ShowShareButton = showShareButton,
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
        };

        if (block.GetPropertyValue<bool>(nameof(block.Streaming)) && block.GetPropertyValue<IEnumerable<ImageSource>>(nameof(block.Sources)).Any(p => p != ImageSource.Webcam))
        {
            throw new ArgumentException("Image streaming only available if sources is ['webcam']. Streaming not supported with multiple sources.");
        }

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

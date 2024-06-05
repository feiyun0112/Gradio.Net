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
        bool container = true,        
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        bool visible = true,
        string elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        string key = null,
        bool mirrorWebcam = true,
        bool? includeAudio = null,
        bool autoplay = false,
        bool? showShareButton =null,
        bool? showDownloadButton= null,
        int? minLength=null,
        int? maxLength = null)
    {
        Video block = new()
        {
            Format = format,
            MirrorWebcam = mirrorWebcam,            
            Height = height,
            Width = width,
            Sources = sources == null? [VideoSource.Upload, VideoSource.Webcam] : sources,            
            Label= label,
            Every= every,
            ShowLabel= showLabel,
            Container = container,
            Scale= scale,
            MinWidth= minWidth,
            Interactive= interactive,
            Visible= visible,
            ElemId= elemId,
            ElemClasses= elemClasses,
            Render= render,
            Key= key,
            Value= value,
            ShowShareButton = showShareButton==null ?(GradioUtils.GetSpace() != null)  : showShareButton,
            IncludeAudio= includeAudio==null ? (sources ==null  || sources.Contains(VideoSource.Upload) ): includeAudio,
            Autoplay= autoplay,
            ShowDownloadButton= showDownloadButton,
            MinLength= minLength,
            MaxLength = maxLength,
        };

        Context.AddToCurrentBlocks(block);
        return block;
    }
}

using Gradio.Net.Enums;
using System;
using System.Data;

namespace Gradio.Net;

public static partial class gr
{ 
    public static Audio Audio(
        string value = null,             
        IEnumerable<AudioSource> sources = null,
        AudioType type = AudioType.Filepath,        
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool container = true,        
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        bool visible = true,
        bool streaming=false,
        string elemId = null,
        List<string> elemClasses = null,
        AudioFormat format = AudioFormat.Mp3,
        bool autoplay = false,
        bool? showDownloadButton = null,                
        bool? showShareButton =null,
        bool? showEditButton = true
    )
    {
        Audio block = new()
        {
            Value = value,
            Sources = sources == null? [AudioSource.Upload, AudioSource.Microphone] : sources,       
            Type=type,
            Label= label,
            Every= every,
            ShowLabel= showLabel,
            Container = container,
            Scale= scale,
            MinWidth= minWidth,
            Interactive= interactive,
            Visible= visible,
            Streaming = streaming,
            ElemId = elemId,
            ElemClasses= elemClasses,
            Format = format,
            Autoplay = autoplay,
            ShowShareButton = showShareButton==null ?(GradioUtils.GetSpace() != null)  : showShareButton,
            
            ShowDownloadButton= showDownloadButton,
            ShowEditButton = showEditButton,
        };

        if (block.Streaming.HasValue && block.Streaming.Value
            && block.Sources.Count() == 1 && block.Sources.First() == AudioSource.Upload)
        {
            throw new ArgumentException(
                "Audio streaming only available if source is 'microphone'."
            );
        }
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

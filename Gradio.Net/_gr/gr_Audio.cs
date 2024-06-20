using Gradio.Net.Enums;
using System;
using System.Data;

namespace Gradio.Net;

public static partial class gr
{
    public static Audio Audio(
        string value = null,
        IEnumerable<AudioSource> sources = null,
        AudioType? type = null,
        string label = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        bool? streaming = null,
        string elemId = null,
        List<string> elemClasses = null,
        AudioFormat? format = null,
        bool? autoplay = null,
        bool? showDownloadButton = null,
        bool? showShareButton = null,
        bool? showEditButton = true
    )
    {
        Audio block = new()
        {
            Value = value,
            Sources = sources,
            Type = type,
            Label = label,
            Every = every,
            ShowLabel = showLabel,
            Container = container,
            Scale = scale,
            MinWidth = minWidth,
            Interactive = interactive,
            Visible = visible,
            Streaming = streaming,
            ElemId = elemId,
            ElemClasses = elemClasses,
            Format = format,
            Autoplay = autoplay,
            ShowShareButton = showShareButton,

            ShowDownloadButton = showDownloadButton,
            ShowEditButton = showEditButton,
        };

        if (block.GetPropertyValue<bool>(nameof(block.Streaming))
            && block.GetPropertyValue<IEnumerable<AudioSource>>(nameof(block.Sources)).Count() == 1
            && block.GetPropertyValue<IEnumerable<AudioSource>>(nameof(block.Sources)).First() == AudioSource.Upload)
        {
            throw new ArgumentException(
                "Audio streaming only available if source is 'microphone'."
            );
        }
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

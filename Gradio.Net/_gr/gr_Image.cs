using Gradio.Net.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Image Image(
            string value = null,
            ImageFormat format = ImageFormat.Webp,
            int? height = null,
            int? width = null,
            ImageMode imageMode =  ImageMode.RGB,
            IEnumerable<ImageSource> sources = null,
            ImageType type = ImageType.Filepath,
            string label = null,
            decimal? every = null,
            bool? showLabel = null,
            bool showDownloadButton = true,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool? interactive = null,
            bool visible = true,
            bool streaming = false,
            string elemId = null,
            List<string> elemClasses = null,
            bool render = true,
            string key = null,
            bool mirrorWebcam = true,
            bool? showShareButton = null)
        {
            var block = new Image()
            {
                Format = format,
                MirrorWebcam = mirrorWebcam,
                Type = type,
                Height = height,
                Width = width,
                ImageMode = imageMode,
                Sources = sources == null?(streaming? new List<ImageSource> { ImageSource.Webcam }: new List<ImageSource> { ImageSource.Upload, ImageSource.Webcam, ImageSource.Clipboard }) : sources,
                Streaming = streaming,
                ShowDownloadButton = showDownloadButton,
                
                ShowShareButton = (GradioUtils.GetSpace() != null) ? showShareButton : true,
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
            };

            if (streaming && block.Sources.Any(p=>p!= ImageSource.Webcam ))
            {
                    throw new ArgumentException("Image streaming only available if sources is ['webcam']. Streaming not supported with multiple sources.");
            }

            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

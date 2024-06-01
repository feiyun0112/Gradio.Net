using Gradio.Net.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Chatbot  Chatbot(
            IEnumerable<object> value = null,
            string label = null,
            decimal? every = null,
            bool? showLabel = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool visible = true,
            string elemId = null,
            IEnumerable<string> elemClasses = null,
            bool render = true,
            string key = null,
            int? height = null,
            IEnumerable<Dictionary<string, object>> latexDelimiters = null,
            bool rtl = false,
            bool? showShareButton = null,
            bool showCopyButton = false,
            Tuple<string, string> avatarImages = null,
            bool sanitizeHtml = true,
            bool renderMarkdown = true,
            bool bubbleFullWidth = true,
            bool lineBreaks = true,
            bool likeable = false,
            string layout = null,
            string placeholder = null)
        {
            var block = new Chatbot();
            block.Value = value;
            block.Label = label;
            block.Every = every;
            block.ShowLabel = showLabel;
            block.Container = container;
            block.Scale = scale;
            block.MinWidth = minWidth;
            block.Visible = visible;
            block.ElemId = elemId;
            block.ElemClasses = elemClasses;
            block.Render = render;
            block.Key = key;
            block.Height = height;
            block.LatexDelimiters = latexDelimiters;
            block.Rtl = rtl;
            block.ShowShareButton = showShareButton ?? (GradioUtils.GetSpace() != null);
            block.RenderMarkdown = renderMarkdown;
            block.ShowCopyButton = showCopyButton;
            block.AvatarImages = avatarImages;
            block.SanitizeHtml = sanitizeHtml;
            block.RenderMarkdown = renderMarkdown;
            block.BubbleFullWidth = bubbleFullWidth;
            block.LineBreaks = lineBreaks;
            block.Likeable = likeable;
            block.Layout = layout;
            block.Placeholder = placeholder;

            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

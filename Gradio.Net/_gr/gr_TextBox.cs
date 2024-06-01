using Gradio.Net.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Textbox Textbox(
            string value = null,
            int lines = 1,
            int maxLines = 20,
            string placeholder = null,
            string label = null,
            string info = null,
            decimal? every = null,
            bool showLabel = true,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool? interactive = null,
            bool visible = true,
            string elemId = null,
            bool autofocus = false,
            bool autoscroll = true,
            IEnumerable<string> elemClasses = null,
            bool render = true,
            TextboxType type = TextboxType.Text,
            TextboxTextAlign textAlign = TextboxTextAlign.Left,
            bool rtl = false,
            bool showCopyButton = false)
        {
            var blocks = new Textbox()
            {
                Value = value,
                Lines = lines,
                MaxLines = maxLines,
                Placeholder = placeholder,
                Label = label,
                Info = info,
                Every = every,
                ShowLabel = showLabel,
                Container = container,
                Scale = scale,
                MinWidth = minWidth,
                Interactive = interactive,
                Visible = visible,
                ElemId = elemId,
                Autofocus = autofocus,
                Autoscroll = autoscroll,
                ElemClasses = elemClasses,
                Render = render,
                Type = type,
                TextAlign = textAlign,
                Rtl = rtl,
                ShowCopyButton = showCopyButton
            };
            Context.AddToCurrentBlocks(blocks);
            return blocks;
        }
    }
}

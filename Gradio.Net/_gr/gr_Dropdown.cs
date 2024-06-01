using Gradio.Net.Enums;
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
        public static Dropdown Dropdown(
            IEnumerable<string> choices = null,
            object value = null,
            DropdownType type = DropdownType.Value,
            bool? multiselect = null,
            bool allowCustomValue = false,
            int? maxChoices = null,
            bool filterable = true,
            string label = null,
            string info = null,
            decimal? every = null,
            bool? showLabel = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool? interactive = null,
            bool visible = true,
            string elemId = null,
            IEnumerable<string> elemClasses = null,
            bool render = true
        )
        { 
            var block = new Dropdown();
            block.Choices = choices;
            block.Type = type;
            block.Multiselect = multiselect;
            block.MaxChoices = maxChoices;
            block.AllowCustomValue = allowCustomValue;
            block.Filterable = filterable;
            
            block.Label = label;
            block.Info = info;
            block.Every = every;
            block.ShowLabel = showLabel;
            block.Container = container;
            block.Scale = scale;
            block.MinWidth = minWidth;
            block.Interactive = interactive;
            block.Visible = visible;
            block.ElemId = elemId;
            block.ElemClasses = elemClasses;
            block.Render = render;
            block.Value = value;
            
            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

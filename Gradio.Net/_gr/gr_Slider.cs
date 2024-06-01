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
       
        public static Slider Slider(
            decimal minimum = 0,
            decimal maximum = 100,            
            decimal? value = null,
            decimal? step = null,
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
            IEnumerable<string> elemClasses = null,
            bool render = true,
            bool randomize =false
            )
        {
            var block = new Slider()
            {
                Value = value,
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
                ElemClasses = elemClasses,
                Render = render,
            };
            block.Minimum = minimum;
            block.Maximum = maximum;
            if (step == null)
            {
                var difference = maximum - minimum;
                var power = Math.Floor(Math.Log10( double.Parse(difference.ToString())) - 2);
                block.Step =decimal.Parse( Math.Pow( 10,double.Parse(power.ToString())).ToString());
            }
            else
                block.Step = step.Value;
            if (randomize)
                value = block.GetRandomValue();

            Context.AddToCurrentBlocks(block);
            return block;
        }
    }
}

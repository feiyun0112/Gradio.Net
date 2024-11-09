using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Themes
{
    using System;
    using System.Collections.Generic;

    public class Size
    {

        public string Xxs { get; set; }
        public string Xs { get; set; }
        public string Sm { get; set; }
        public string Md { get; set; }
        public string Lg { get; set; }
        public string Xl { get; set; }
        public string Xxl { get; set; }
        public string Name { get; set; }

        public Size(string xxs, string xs, string sm, string md, string lg, string xl, string xxl, string name = null)
        {
            Xxs = xxs;
            Xs = xs;
            Sm = sm;
            Md = md;
            Lg = lg;
            Xl = xl;
            Xxl = xxl;
            Name = name;
        }

        public List<string> Expand()
        {
            return new List<string> { Xxs, Xs, Sm, Md, Lg, Xl, Xxl };
        }
    }

    public static class Sizes
    {
        public static Size RadiusNone { get; } = new Size(
            "0px", "0px", "0px", "0px", "0px", "0px", "0px", "radius_none"
        );

        public static Size RadiusSm { get; } = new Size(
                "1px", "1px", "2px", "4px", "6px", "8px", "12px", "radius_sm"
            );

        public static Size RadiusMd { get; } = new Size(
                "1px", "2px", "4px", "6px", "8px", "12px", "22px", "radius_md"
            );

        public static Size RadiusLg { get; } = new Size(
                "2px", "4px", "6px", "8px", "12px", "16px", "24px", "radius_lg"
            );

        public static Size RadiusXxl { get; } = new Size(
                "6px", "8px", "10px", "20px", "24px", "28px", "32px", "radius_xxl"
            );

        public static Size SpacingSm { get; } = new Size(
                "1px", "1px", "2px", "4px", "6px", "9px", "12px", "spacing_sm"
            );

        public static Size SpacingMd { get; } = new Size(
                "1px", "2px", "4px", "6px", "8px", "10px", "16px", "spacing_md"
            );

        public static Size SpacingLg { get; } = new Size(
                "2px", "4px", "6px", "8px", "10px", "14px", "28px", "spacing_lg"
            );

        public static Size TextSm { get; } = new Size(
                "8px", "9px", "11px", "13px", "16px", "20px", "24px", "text_sm"
            );

        public static Size TextMd { get; } = new Size(
                "9px", "10px", "12px", "14px", "16px", "22px", "26px", "text_md"
            );

        public static Size TextLg { get; } = new Size(
                "10px", "12px", "14px", "16px", "20px", "24px", "28px", "text_lg"
            );
    }
}

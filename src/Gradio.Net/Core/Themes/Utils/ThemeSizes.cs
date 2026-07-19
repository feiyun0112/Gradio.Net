namespace Gradio.Net.Core.Themes.Utils;

public static class ThemeSizes
{
    public static readonly ThemeSize radius_none = new("0px", "0px", "0px", "0px", "0px", "0px", "0px", "radius_none");
    public static readonly ThemeSize radius_sm = new("1px", "1px", "2px", "4px", "6px", "8px", "12px", "radius_sm");
    public static readonly ThemeSize radius_md = new("1px", "2px", "4px", "6px", "8px", "12px", "22px", "radius_md");
    public static readonly ThemeSize radius_lg = new("2px", "4px", "6px", "8px", "12px", "16px", "24px", "radius_lg");
    public static readonly ThemeSize radius_xxl = new("6px", "8px", "10px", "20px", "24px", "28px", "32px", "radius_xxl");

    public static readonly ThemeSize spacing_sm = new("1px", "1px", "2px", "4px", "6px", "9px", "12px", "spacing_sm");
    public static readonly ThemeSize spacing_md = new("1px", "2px", "4px", "6px", "8px", "10px", "16px", "spacing_md");
    public static readonly ThemeSize spacing_lg = new("2px", "4px", "6px", "8px", "10px", "14px", "28px", "spacing_lg");

    public static readonly ThemeSize text_sm = new("8px", "9px", "11px", "13px", "16px", "20px", "24px", "text_sm");
    public static readonly ThemeSize text_md = new("9px", "10px", "12px", "14px", "16px", "22px", "26px", "text_md");
    public static readonly ThemeSize text_lg = new("10px", "12px", "14px", "16px", "20px", "24px", "28px", "text_lg");
}

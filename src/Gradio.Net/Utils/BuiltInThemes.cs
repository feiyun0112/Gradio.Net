namespace Gradio.Net.Utils;

public static class BuiltInThemes
{
    public static Dictionary<string, object> Themes = new()
    {
        { "base", new Core.Theme() },
        { "default", new Core.Themes.DefaultTheme() },
        { "monochrome", new Core.Themes.MonochromeTheme() },
        { "soft", new Core.Themes.SoftTheme() },
        { "glass", new Core.Themes.GlassTheme() },
        { "origin", new Core.Themes.OriginTheme() },
        { "citrus", new Core.Themes.CitrusTheme() },
        { "ocean", new Core.Themes.OceanTheme() }
    };
}

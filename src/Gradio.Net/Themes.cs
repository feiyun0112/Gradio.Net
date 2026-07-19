using Gradio.Net.Core;
using Gradio.Net.Core.Themes;

namespace Gradio.Net;

public static class Themes
{
    public static DefaultTheme Default() => new DefaultTheme();

    public static SoftTheme Soft() => new SoftTheme();

    public static MonochromeTheme Monochrome() => new MonochromeTheme();

    public static GlassTheme Glass() => new GlassTheme();

    public static OriginTheme Origin() => new OriginTheme();

    public static CitrusTheme Citrus() => new CitrusTheme();

    public static OceanTheme Ocean() => new OceanTheme();
}

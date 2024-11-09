using Gradio.Net.Themes;

namespace Gradio.Net;

public class GradioServiceConfig
{
    [Obsolete("Use Theme")]
    public IEnumerable<string> Stylesheets { get; set; } = null;

    [Obsolete("Use Theme")]
    public Dictionary<string, string> BodyCss { get; set; } = null;

    public Theme Theme { get; set; } = Themes.Themes.Default;
    /// <summary>
    ///  @"https://fonts.googleapis.com/css2?family={name}:wght@{weight}&display=swap"
    /// </summary>
    public string GoogleFontUrlTemplate { get; set; } = @"https://fonts.googleapis.com/css2?family={name}:wght@{weight}&display=swap";

    public string Title { get; set; } = "Gradio.Net";
}

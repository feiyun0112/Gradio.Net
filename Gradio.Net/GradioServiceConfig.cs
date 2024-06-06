namespace Gradio.Net;

public class GradioServiceConfig
{
    public IEnumerable<string> Stylesheets { get; set; } = new string[] {
            "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:wght@400;600&display=swap",
            "https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
        };

    public Dictionary<string, string> BodyCss { get; set; } = new Dictionary<string, string> {
                { "body_background_fill", "white" },
                { "body_text_color", "#1f2937" },
                { "body_background_fill_dark", "#0b0f19" },
                { "body_text_color_dark", "#f3f4f6" },
            };
}

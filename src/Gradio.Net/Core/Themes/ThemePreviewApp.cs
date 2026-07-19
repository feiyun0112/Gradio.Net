namespace Gradio.Net.Core.Themes;

public static class ThemePreviewApp
{
    public static string BuildHeaderMarkdown()
    {
        return """
# Theme preview: `{THEME}`
To use this theme, set `theme='{AUTHOR}/{SPACE_NAME}'` in the launch method of Blocks or Interface.
You can append an `@` and a semantic version expression.
""";
    }
}

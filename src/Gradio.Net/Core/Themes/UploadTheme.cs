namespace Gradio.Net.Core.Themes;

public static class UploadTheme
{
    public static string Upload(
        string theme,
        string repoName,
        string? orgName = null,
        string? version = null,
        string? token = null,
        string? themeName = null,
        string? description = null)
    {
        var loaded = Theme.Load(theme);
        return loaded.PushToHub(
            repoName: repoName,
            orgName: orgName,
            version: version,
            token: token,
            themeName: themeName,
            description: description);
    }
}

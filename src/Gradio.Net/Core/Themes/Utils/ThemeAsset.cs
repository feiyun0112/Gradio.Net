using Gradio.Net.Components;


namespace Gradio.Net.Core.Themes.Utils;

public sealed class ThemeAsset
{
    public string Filename { get; }
    public string Version { get; }

    public ThemeAsset(string filenameOrVersion)
    {
        Filename = filenameOrVersion;
        if (filenameOrVersion.Contains("@", StringComparison.Ordinal))
        {
            var part = filenameOrVersion.Split('@')[1];
            Version = part.Replace(".json", string.Empty, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Version = filenameOrVersion;
        }
    }
}

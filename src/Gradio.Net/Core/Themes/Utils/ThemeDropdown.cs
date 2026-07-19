using Gradio.Net.Components;


namespace Gradio.Net.Core.Themes.Utils;

public static class ThemeDropdown
{
    public static (Dropdown component, string js) CreateThemeDropdown(string themesPath)
    {
        var assets = new List<(ThemeAsset asset, Theme theme)>();

        if (Directory.Exists(themesPath))
        {
            foreach (var file in Directory.GetFiles(themesPath, "*.json"))
            {
                var version = Path.GetFileNameWithoutExtension(file);
                var theme = Theme.Load(file);
                assets.Add((new ThemeAsset(version), theme));
            }
        }

        if (assets.Count == 0)
        {
            assets.Add((new ThemeAsset("0.0.1"), new Theme()));
        }

        var latestToOldest = assets
            .Select(a => a.asset.Version)
            .OrderByDescending(v => v, StringComparer.Ordinal)
            .ToList();

        var head = assets[0];
        var tail = assets.Skip(1).ToList();

        string MakeElseIf((ThemeAsset asset, Theme theme) t)
        {
            return $"else if (theme == '{t.asset.Version}') {{ var theme_css = `{t.theme.GetThemeCss()}` }}";
        }

        var ifStatement = $"if (theme == \"{head.asset.Version}\") {{ var theme_css = `{head.theme.GetThemeCss()}` }} {string.Join(" ", tail.Select(MakeElseIf))}";

        var component = new Dropdown(
            choices: latestToOldest,
            value: latestToOldest[0],
            render: false,
            label: "Select Version",
            container: false
        );

        var js = $@"
(theme) => {{
    if (!document.querySelector('.theme-css')) {{
        var theme_elem = document.createElement('style');
        theme_elem.classList.add('theme-css');
        document.head.appendChild(theme_elem);
    }} else {{
        var theme_elem = document.querySelector('.theme-css');
    }}
    {ifStatement}
    theme_elem.innerHTML = theme_css;
}}
";

        return (component, js);
    }
}

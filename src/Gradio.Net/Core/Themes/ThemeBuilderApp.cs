using System.Reflection;

namespace Gradio.Net.Core.Themes;

public static class ThemeBuilderApp
{
    public static string? GetDocString(string variable, Dictionary<string, string> docs)
    {
        return docs.TryGetValue(variable, out var doc) ? doc : null;
    }

    public static (List<(string group, string description, List<string> variables)> groups, List<string> flatVariables)
        GetThemeVarGroups()
    {
        // C# parity helper for builder UI: group by variable prefix.
        var props = typeof(Theme)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p => p.Name)
            .ToList();

        var groups = new List<(string group, string description, List<string> variables)>
        {
            ("Core", "Core theme variables", props)
        };

        return (groups, props);
    }
}

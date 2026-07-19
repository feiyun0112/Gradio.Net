
namespace Gradio.Net.Core.Themes.Utils;

public static class SemverMatch
{
    public static List<ThemeAsset> GetThemeAssets(IEnumerable<string> siblingFilenames)
    {
        return siblingFilenames
            .Where(f => f.StartsWith("themes/", StringComparison.OrdinalIgnoreCase))
            .Select(f => new ThemeAsset(f))
            .ToList();
    }

    public static ThemeAsset? GetMatchingVersion(List<ThemeAsset> assets, string? expression)
    {
        expression ??= "*";
        var matching = assets
            .Where(a => Matches(a.Version, expression))
            .OrderByDescending(a => ParseVersion(a.Version))
            .FirstOrDefault();
        return matching;
    }

    private static bool Matches(string version, string expression)
    {
        if (expression == "*" || string.IsNullOrWhiteSpace(expression))
        {
            return true;
        }

        var rules = expression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rules.Length == 0)
        {
            return string.Equals(version, expression, StringComparison.Ordinal);
        }

        var v = ParseVersion(version);
        foreach (var rule in rules)
        {
            if (rule.StartsWith(">="))
            {
                if (v < ParseVersion(rule[2..])) return false;
            }
            else if (rule.StartsWith("<="))
            {
                if (v > ParseVersion(rule[2..])) return false;
            }
            else if (rule.StartsWith(">"))
            {
                if (v <= ParseVersion(rule[1..])) return false;
            }
            else if (rule.StartsWith("<"))
            {
                if (v >= ParseVersion(rule[1..])) return false;
            }
            else if (rule.StartsWith("=="))
            {
                if (v != ParseVersion(rule[2..])) return false;
            }
            else
            {
                if (v != ParseVersion(rule)) return false;
            }
        }

        return true;
    }

    private static Version ParseVersion(string version)
    {
        var clean = version.Trim();
        if (clean.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            clean = clean[1..];
        }

        var parts = clean.Split('.').ToList();
        while (parts.Count < 3) parts.Add("0");
        if (parts.Count > 3) parts = parts.Take(3).ToList();

        if (!int.TryParse(parts[0], out var major)) major = 0;
        if (!int.TryParse(parts[1], out var minor)) minor = 0;
        if (!int.TryParse(parts[2], out var patch)) patch = 0;

        return new Version(major, minor, patch);
    }
}

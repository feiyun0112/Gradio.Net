namespace Gradio.Net;
/// <summary>
/// The twelve themes the Python version ships, and the name lookups that turn a configuration
/// string into one of them.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Get"/> is the Python version's <c>utils.get_theme</c> without the network: an
/// application that names a theme in its configuration gets the preset, and one that names nothing
/// gets <see cref="DefaultTheme"/>. The Python version's other branch — a name that is not a preset
/// is fetched from the Hugging Face hub — is not reproduced, because the package does not reach out
/// to a third party at run time. A name that resolves to nothing is refused rather than quietly
/// replaced by the default, which is what the Python version does after the fetch fails.
/// </para>
/// <para>
/// Every call answers with a new instance. The Python version keeps one instance of each preset in
/// a process-wide dictionary, which means one application's customisation of a preset would leak
/// into the next; here a theme is owned by the application that asked for it.
/// </para>
/// </remarks>
public static class ThemePresets
{
    /// <summary>The names <see cref="Get"/> answers to, in the Python version's order.</summary>
    /// <remarks>
    /// Twelve, which is the Python version's exported set. Its own lookup table lists nine —
    /// <c>ember</c>, <c>neon</c> and <c>cyberpunk</c> are exported but missing from it, so naming
    /// one of those there warns and falls back to the default theme. They resolve here, because a
    /// theme that exists and cannot be named is a defect rather than a behaviour worth reproducing.
    /// </remarks>
    public static IReadOnlyList<string> BuiltInNames { get; } =
    [
        "base",
        "default",
        "ember",
        "glass",
        "monochrome",
        "neon",
        "soft",
        "origin",
        "citrus",
        "ocean",
        "mario",
        "cyberpunk",
    ];

    /// <summary>The theme an application gets when it names none.</summary>
    /// <returns>A new <see cref="DefaultTheme"/>.</returns>
    public static Theme Default() => new DefaultTheme();

    /// <summary>Finds a built-in theme by name.</summary>
    /// <param name="name">The theme's name, in any case.</param>
    /// <returns>A new instance of the preset.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No built-in theme has that name.</exception>
    public static Theme Get(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return name.ToLowerInvariant() switch
        {
            "base" => new BaseTheme(),
            "default" => new DefaultTheme(),
            "ember" => new EmberTheme(),
            "glass" => new GlassTheme(),
            "monochrome" => new MonochromeTheme(),
            "neon" => new NeonTheme(),
            "soft" => new SoftTheme(),
            "origin" => new OriginTheme(),
            "citrus" => new CitrusTheme(),
            "ocean" => new OceanTheme(),
            "mario" => new MarioTheme(),
            "cyberpunk" => new CyberpunkTheme(),
            _ => throw new ArgumentException(
                $"'{name}' is not a built-in theme. The built-in themes are "
                + string.Join(", ", BuiltInNames) + ".",
                nameof(name)),
        };
    }

    /// <summary>Finds a colour ramp by the name a theme file uses.</summary>
    /// <param name="name">The ramp's name — <c>violet</c>, <c>zinc</c>, and so on.</param>
    /// <returns>The ramp.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No preset has that name.</exception>
    public static Color FindHue(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Colors.Find(name)
            ?? throw new ArgumentException(
                $"'{name}' is not a colour ramp. The ramps are "
                + string.Join(", ", Colors.All.Select(ramp => ramp.Name)) + ".",
                nameof(name));
    }

    /// <summary>Finds a size ramp from the shortcut a theme file uses.</summary>
    /// <param name="shortcut">The shortcut — <c>sm</c>, <c>md</c>, <c>lg</c>, and so on.</param>
    /// <param name="prefix">Which ramp the shortcut belongs to: <c>text</c>, <c>spacing</c> or <c>radius</c>.</param>
    /// <returns>The ramp.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No preset is named after that prefix and shortcut.</exception>
    /// <remarks>
    /// The prefix is part of the name because the same shortcut means different values: <c>md</c> is
    /// 14px of text, 6px of spacing and 6px of corner radius, and a theme file that says
    /// <c>radius_size: "md"</c> is asking for the third.
    /// </remarks>
    public static Size FindSize(string shortcut, string prefix)
    {
        ArgumentNullException.ThrowIfNull(shortcut);
        ArgumentNullException.ThrowIfNull(prefix);

        return Sizes.Find($"{prefix}_{shortcut}")
            ?? throw new ArgumentException(
                $"'{shortcut}' is not a {prefix} size. The {prefix} sizes are "
                + string.Join(", ", Sizes.All.Where(ramp => ramp.Name.StartsWith(prefix + "_", StringComparison.Ordinal))
                    .Select(ramp => ramp.Name[(prefix.Length + 1)..])) + ".",
                nameof(shortcut));
    }
}

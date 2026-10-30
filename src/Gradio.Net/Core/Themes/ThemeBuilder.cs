namespace Gradio.Net;
/// <summary>
/// Composes a custom theme: choose the raw material, then the variables, then build.
/// </summary>
/// <remarks>
/// <para>
/// This is the programmatic form of the Python version's <c>builder()</c>. The Python version's
/// builder is an interactive application that assembles a theme and prints the Python code for it;
/// the thing it assembles — a base theme, three hues, three size ramps, two font stacks, a set of
/// overrides and some custom CSS — is what this type builds directly, because the C# application
/// that wants a branded theme does not need a second UI to produce the first one.
/// </para>
/// <para>
/// Everything is optional, so the shortest use is also a valid one:
/// <code>
/// var theme = ThemeBuilder.Create().PrimaryHue(Colors.Violet).Build();
/// </code>
/// and the result is a theme an application can hand to its blocks, whose stylesheet
/// <c>/theme.css</c> serves and whose hash <c>config.theme_hash</c> carries.
/// </para>
/// </remarks>
public sealed class ThemeBuilder
{
    /// <summary>The name a built theme carries when the builder was not told one.</summary>
    public const string DefaultName = "custom";

    private readonly List<ThemeOptions> _options = [];
    private Color? _primary;
    private Color? _secondary;
    private Color? _neutral;
    private Size? _text;
    private Size? _spacing;
    private Size? _radius;
    private IReadOnlyList<Font>? _font;
    private IReadOnlyList<Font>? _mono;
    private string? _name;
    private string? _customCss;

    private ThemeBuilder()
    {
    }

    /// <summary>Starts a theme.</summary> <returns>A builder that has chosen nothing yet, and so
    /// builds the Python version's base theme.</returns>
    public static ThemeBuilder Create() => new();

    /// <summary>Chooses the hue accents are drawn from.</summary>
    /// <param name="hue">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="hue"/> is <see langword="null"/>.</exception>
    public ThemeBuilder PrimaryHue(Color hue)
    {
        _primary = hue ?? throw new ArgumentNullException(nameof(hue));

        return this;
    }

    /// <summary>Chooses the hue accents are drawn from, by name.</summary>
    /// <param name="name">The ramp's name — <c>violet</c>, <c>zinc</c>, and so on.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder PrimaryHue(string name) => PrimaryHue(ThemePresets.FindHue(name));

    /// <summary>Chooses the hue secondary accents and shadows are drawn from.</summary>
    /// <param name="hue">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="hue"/> is <see langword="null"/>.</exception>
    public ThemeBuilder SecondaryHue(Color hue)
    {
        _secondary = hue ?? throw new ArgumentNullException(nameof(hue));

        return this;
    }

    /// <summary>Chooses the hue secondary accents and shadows are drawn from, by name.</summary>
    /// <param name="name">The ramp's name.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder SecondaryHue(string name) => SecondaryHue(ThemePresets.FindHue(name));

    /// <summary>Chooses the hue backgrounds, borders and text are drawn from.</summary>
    /// <param name="hue">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="hue"/> is <see langword="null"/>.</exception>
    public ThemeBuilder NeutralHue(Color hue)
    {
        _neutral = hue ?? throw new ArgumentNullException(nameof(hue));

        return this;
    }

    /// <summary>Chooses the hue backgrounds, borders and text are drawn from, by name.</summary>
    /// <param name="name">The ramp's name.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder NeutralHue(string name) => NeutralHue(ThemePresets.FindHue(name));

    /// <summary>Chooses the ramp text sizes are drawn from.</summary>
    /// <param name="size">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="size"/> is <see langword="null"/>.</exception>
    public ThemeBuilder TextSize(Size size)
    {
        _text = size ?? throw new ArgumentNullException(nameof(size));

        return this;
    }

    /// <summary>Chooses the ramp text sizes are drawn from, by shortcut.</summary>
    /// <param name="shortcut">The shortcut — <c>sm</c>, <c>md</c>, <c>lg</c>.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder TextSize(string shortcut) => TextSize(ThemePresets.FindSize(shortcut, "text"));

    /// <summary>Chooses the ramp paddings and gaps are drawn from.</summary>
    /// <param name="size">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="size"/> is <see langword="null"/>.</exception>
    public ThemeBuilder SpacingSize(Size size)
    {
        _spacing = size ?? throw new ArgumentNullException(nameof(size));

        return this;
    }

    /// <summary>Chooses the ramp paddings and gaps are drawn from, by shortcut.</summary>
    /// <param name="shortcut">The shortcut.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder SpacingSize(string shortcut) => SpacingSize(ThemePresets.FindSize(shortcut, "spacing"));

    /// <summary>Chooses the ramp corner radii are drawn from.</summary>
    /// <param name="size">The ramp.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="size"/> is <see langword="null"/>.</exception>
    public ThemeBuilder RadiusSize(Size size)
    {
        _radius = size ?? throw new ArgumentNullException(nameof(size));

        return this;
    }

    /// <summary>Chooses the ramp corner radii are drawn from, by shortcut.</summary>
    /// <param name="shortcut">The shortcut.</param>
    /// <returns>This builder.</returns>
    public ThemeBuilder RadiusSize(string shortcut) => RadiusSize(ThemePresets.FindSize(shortcut, "radius"));

    /// <summary>Chooses the body font stack.</summary>
    /// <param name="families">The families, in the order the browser should try them.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="families"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A string converts to a <see cref="Font"/>, so <c>Font("Lora", "sans-serif")</c> reads as a
    /// stack: <c>Font("Lora", "ui-sans-serif", "sans-serif")</c>.
    /// </remarks>
    public ThemeBuilder Font(params Font[] families)
    {
        ArgumentNullException.ThrowIfNull(families);

        _font = [.. families];

        return this;
    }

    /// <summary>Chooses the monospace font stack.</summary>
    /// <param name="families">The families, in the order the browser should try them.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="families"/> is <see langword="null"/>.</exception>
    public ThemeBuilder MonoFont(params Font[] families)
    {
        ArgumentNullException.ThrowIfNull(families);

        _mono = [.. families];

        return this;
    }

    /// <summary>Sets variables.</summary>
    /// <param name="options">The values to set.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Called more than once, the later call wins for the variables it sets, and leaves the rest
    /// alone — which is what calling the Python version's <c>set()</c> twice does.
    /// </remarks>
    public ThemeBuilder Values(ThemeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options.Add(options);

        return this;
    }

    /// <summary>Names the theme.</summary>
    /// <param name="name">The name, which <c>config.theme</c> reports.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public ThemeBuilder Name(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));

        return this;
    }

    /// <summary>Appends custom CSS after the generated rules.</summary>
    /// <param name="css">The stylesheet text.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="css"/> is <see langword="null"/>.</exception>
    public ThemeBuilder CustomCss(string css)
    {
        _customCss = css ?? throw new ArgumentNullException(nameof(css));

        return this;
    }

    /// <summary>Builds the theme.</summary>
    /// <returns>The theme, ready to hand to an application.</returns>
    public BaseTheme Build()
    {
        var theme = new BaseTheme(_primary, _secondary, _neutral, _text, _spacing, _radius, _font, _mono)
        {
            Name = _name ?? DefaultName,
        };

        if (_customCss is not null)
        {
            theme.CustomCss = _customCss;
        }

        foreach (var options in _options)
        {
            theme.Set(options);
        }

        return theme;
    }
}

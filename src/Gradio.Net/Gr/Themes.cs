namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Builds a theme from the Python version's four choices: three hue ramps, three
    /// sizes, two fonts.</summary> <param name="primaryHue">The ramp accents are drawn from, or
    /// <see langword="null"/> for the base theme's.</param> <param name="secondaryHue">The ramp a
    /// second accent is drawn from, or <see langword="null"/> for the base theme's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for
    /// the base theme's.</param> <param name="textSize">The text size, or <see langword="null"/>
    /// for the base theme's.</param> <param name="spacingSize">The spacing size, or
    /// <see langword="null"/> for the base theme's.</param> <param name="radiusSize">The corner
    /// radius, or <see langword="null"/> for the base theme's.</param> <param name="font">The body
    /// font, or <see langword="null"/> for the base theme's.</param> <param name="fontMono">The
    /// monospace font, or <see langword="null"/> for the base theme's.</param> <returns>The
    /// theme.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>gr.Theme</c>, which is the base theme with whatever the caller chose
    /// on top: <c>gr.Theme(primaryHue="blue")</c> and <c>gr.Themes.Base(primaryHue="blue")</c>
    /// are the same document there, and the same here.
    /// </para>
    /// <para>
    /// The hues and sizes are the Python version's unions — <c>Color | str</c>, <c>Size | str</c>, <c>Font | str
    /// | list</c> — so they arrive as objects and are read the way the layer reads a union
    /// everywhere else: a value of the framework's own type is itself, and a string names one of the
    /// ramps, sizes or fonts the Python version ships.
    /// </para>
    /// </remarks>
    public static Theme Theme(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new BaseTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The twelve themes the Python version ships, under the name a ported file reaches
    /// them by.</summary> <value>The module <c>gr.themes</c>, as an object whose methods build one
    /// preset each.</value>
    /// <remarks>
    /// The Python version writes <c>gr.themes.Soft()</c>: <c>themes</c> is a module it exposes
    /// without listing it in <c>__all__</c>, which is why this is a property rather than a type — a
    /// member named <c>Themes</c> is what a ported line reaches, and the twelve presets are its
    /// methods.
    /// </remarks>
    public static ThemesModule Themes { get; } = new();
}

/// <summary>
/// The preset themes, reached as <c>gr.Themes.Soft()</c>.
/// </summary>
/// <remarks>
/// <para>
/// One method per theme the Python version exports: <c>Base</c>, <c>Default</c>, <c>Monochrome</c>,
/// <c>Soft</c>, <c>Glass</c>, <c>Origin</c>, <c>Citrus</c>, <c>Ocean</c>, <c>Mario</c>,
/// <c>Ember</c>, <c>Neon</c>, <c>Cyberpunk</c>. Each returns a new theme, so two applications never
/// share one — the Python version keeps a single instance of each, and one application's
/// customisation would then leak into the next.
/// </para>
/// <para>
/// Every method takes the same eight choices <see cref="gr.Theme"/> does, and a choice left out is
/// the preset's own: the presets' constructors already take those choices and already answer
/// <see langword="null"/> with the value the Python version gave them, which is what the Python
/// version's <c>Soft(**kwargs)</c> is.
/// </para>
/// <para>
/// The methods are not static because the object they hang on is <see cref="gr.Themes"/>: a ported
/// line says <c>gr.Themes.Soft()</c>, and that is a member access followed by a call.
/// </para>
/// </remarks>
public sealed class ThemesModule
{
    /// <summary>The base theme, which the others are variations of.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the
    /// base's.</param> <param name="secondaryHue">The ramp a second accent is drawn from, or
    /// <see langword="null"/> for the base's.</param> <param name="neutralHue">The ramp the greys
    /// are drawn from, or <see langword="null"/> for the base's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the base's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the
    /// base's.</param> <param name="radiusSize">The corner radius, or <see langword="null"/> for
    /// the base's.</param> <param name="font">The body font, or <see langword="null"/> for the
    /// base's.</param> <param name="fontMono">The monospace font, or <see langword="null"/> for
    /// the base's.</param> <returns>The theme.</returns> <remarks>The Python version's
    /// <c>themes.Base</c>, which is <c>gr.Theme</c> spelled as a preset.</remarks>
    public Theme Base(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        gr.Theme(
            primaryHue,
            secondaryHue,
            neutralHue,
            textSize,
            spacingSize,
            radiusSize,
            font,
            fontMono);

    /// <summary>The default theme: the one an application that names none gets.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the
    /// preset's.</param> <param name="secondaryHue">The ramp a second accent is drawn from, or
    /// <see langword="null"/> for the preset's.</param> <param name="neutralHue">The ramp the
    /// greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the
    /// preset's.</param> <param name="radiusSize">The corner radius, or <see langword="null"/> for
    /// the preset's.</param> <param name="font">The body font, or <see langword="null"/> for the
    /// preset's.</param> <param name="fontMono">The monospace font, or <see langword="null"/> for
    /// the preset's.</param> <returns>The theme.</returns> <remarks>The Python version's
    /// <c>themes.Default</c>.</remarks>
    public Theme Default(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new DefaultTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The monochrome theme: black, white and nothing between.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the
    /// preset's.</param> <param name="secondaryHue">The ramp a second accent is drawn from, or
    /// <see langword="null"/> for the preset's.</param> <param name="neutralHue">The ramp the
    /// greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the
    /// preset's.</param> <param name="radiusSize">The corner radius, or <see langword="null"/> for
    /// the preset's.</param> <param name="font">The body font, or <see langword="null"/> for the
    /// preset's.</param> <param name="fontMono">The monospace font, or <see langword="null"/> for
    /// the preset's.</param> <returns>The theme.</returns> <remarks>The Python version's
    /// <c>themes.Monochrome</c>.</remarks>
    public Theme Monochrome(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new MonochromeTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The quiet preset: indigo on gray, rounded and low-contrast.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Soft(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new SoftTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The glass theme: translucent surfaces over a blurred backdrop.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Glass(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new GlassTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The origin theme: the look gradio had before the themes were variables.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Origin(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new OriginTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The citrus theme: orange on green, square corners.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Citrus(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new CitrusTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The ocean theme: blue on slate, round and airy.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Ocean(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new OceanTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The mario theme: red on yellow, chunky borders.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Mario(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new MarioTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The ember theme: warm orange on dark grey.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Ember(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new EmberTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The neon theme: magenta on black, glowing.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Neon(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new NeonTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));

    /// <summary>The cyberpunk theme: yellow on black, monospaced.</summary>
    /// <param name="primaryHue">The ramp accents are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="secondaryHue">The ramp a second accent is drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="neutralHue">The ramp the greys are drawn from, or <see langword="null"/> for the preset's.</param>
    /// <param name="textSize">The text size, or <see langword="null"/> for the preset's.</param>
    /// <param name="spacingSize">The spacing size, or <see langword="null"/> for the preset's.</param>
    /// <param name="radiusSize">The corner radius, or <see langword="null"/> for the preset's.</param>
    /// <param name="font">The body font, or <see langword="null"/> for the preset's.</param>
    /// <param name="fontMono">The monospace font, or <see langword="null"/> for the preset's.</param>
    /// <returns>The theme.</returns>
    public Theme Cyberpunk(
        object? primaryHue = null,
        object? secondaryHue = null,
        object? neutralHue = null,
        object? textSize = null,
        object? spacingSize = null,
        object? radiusSize = null,
        object? font = null,
        object? fontMono = null) =>
        new CyberpunkTheme(
            Values.Hue(primaryHue),
            Values.Hue(secondaryHue),
            Values.Hue(neutralHue),
            Values.TextSize(textSize),
            Values.SpacingSize(spacingSize),
            Values.RadiusSize(radiusSize),
            Values.Fonts(font),
            Values.Fonts(fontMono));
}

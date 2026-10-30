namespace Gradio.Net;
/// <summary>
/// The theme every other theme is built from: the Python version's <c>Base</c>.
/// </summary>
/// <remarks>
/// <para>
/// It answers the two questions a theme is made of. The constructor chooses the raw material —
/// three colour ramps, three size ramps, two font stacks — and produces the 56 variables that carry
/// it (the hues, the sizes, the fonts). <see cref="Theme.Set(ThemeOptions)"/> then fills in the 267
/// variables that are expressed in terms of that raw material, each with the default the Python
/// version gives it.
/// </para>
/// <para>
/// An application that wants Gradio's look takes a preset (<see cref="DefaultTheme"/>); one that
/// wants its own takes this and passes a <see cref="ThemeOptions"/> to
/// <see cref="Theme.Set(ThemeOptions)"/> — or uses <see cref="ThemeBuilder"/>, which is the same
/// thing with the choices named.
/// </para>
/// </remarks>
public class BaseTheme : Theme
{
    /// <summary>Creates a theme from the Python version's defaults, or from the raw material
    /// given.</summary> <param name="primaryHue">The hue accents are drawn from. Defaults to
    /// blue.</param> <param name="secondaryHue">The hue shadows and secondary accents are drawn
    /// from. Defaults to blue.</param> <param name="neutralHue">The hue backgrounds, borders and
    /// text are drawn from. Defaults to zinc.</param> <param name="textSize">The ramp text sizes
    /// are drawn from. Defaults to the medium ramp.</param> <param name="spacingSize">The ramp
    /// paddings and gaps are drawn from. Defaults to the medium ramp.</param>
    /// <param name="radiusSize">The ramp corner radii are drawn from. Defaults to the medium
    /// ramp.</param>
    /// <param name="font">
    /// The body font stack, in the order the browser should try it. Defaults to IBM Plex Sans and
    /// the usual sans-serif fallbacks.
    /// </param>
    /// <param name="fontMono">The monospace font stack. Defaults to IBM Plex Mono and its fallbacks.</param>
    /// <remarks>
    /// The parameters are the ramps themselves rather than names of them, which is the C# spelling
    /// of the Python version's <c>primary_hue=colors.blue</c>: <see cref="Colors"/> and
    /// <see cref="Sizes"/> hold the presets, and a caller that has a name — from a configuration
    /// file, say — resolves it with <see cref="ThemePresets.FindHue"/> and
    /// <see cref="ThemePresets.FindSize"/>.
    /// </remarks>
    public BaseTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
    {
        Name = "base";

        UseRamp("primary", primaryHue ?? Colors.Blue);
        UseRamp("secondary", secondaryHue ?? Colors.Blue);
        UseRamp("neutral", neutralHue ?? Colors.Zinc);
        UseRamp("spacing", spacingSize ?? Sizes.SpacingMd);
        UseRamp("radius", radiusSize ?? Sizes.RadiusMd);
        UseRamp("text", textSize ?? Sizes.TextMd);

        UseFonts(font?.ToList() ?? DefaultFont, fontMono?.ToList() ?? DefaultMonoFont);

        Set(new ThemeOptions());
    }

    /// <summary>The body font stack a theme starts with.</summary>
    /// <remarks>
    /// Built per call rather than kept in a field: a list is mutable, and the package keeps no
    /// mutable state outside an instance.
    /// </remarks>
    internal static IReadOnlyList<Font> DefaultFont =>
        [new LocalFont("IBM Plex Sans"), "ui-sans-serif", "system-ui", "sans-serif"];

    /// <summary>The monospace font stack a theme starts with.</summary>
    internal static IReadOnlyList<Font> DefaultMonoFont =>
        [new LocalFont("IBM Plex Mono"), "ui-monospace", "Consolas", "monospace"];

    /// <summary>Writes a ramp's steps into the variables named after it.</summary>
    /// <param name="prefix">The variable prefix — <c>primary</c>, <c>spacing</c>, and so on.</param>
    /// <param name="color">The colour ramp, for a hue prefix.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    private void UseRamp(string prefix, Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        Assign($"{prefix}_50", color.C50);
        Assign($"{prefix}_100", color.C100);
        Assign($"{prefix}_200", color.C200);
        Assign($"{prefix}_300", color.C300);
        Assign($"{prefix}_400", color.C400);
        Assign($"{prefix}_500", color.C500);
        Assign($"{prefix}_600", color.C600);
        Assign($"{prefix}_700", color.C700);
        Assign($"{prefix}_800", color.C800);
        Assign($"{prefix}_900", color.C900);
        Assign($"{prefix}_950", color.C950);
    }

    /// <summary>Writes a ramp's steps into the variables named after it.</summary>
    /// <param name="prefix">The variable prefix — <c>spacing</c>, <c>radius</c> or <c>text</c>.</param>
    /// <param name="size">The size ramp.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    private void UseRamp(string prefix, Size size)
    {
        ArgumentNullException.ThrowIfNull(size);

        Assign($"{prefix}_xxs", size.Xxs);
        Assign($"{prefix}_xs", size.Xs);
        Assign($"{prefix}_sm", size.Sm);
        Assign($"{prefix}_md", size.Md);
        Assign($"{prefix}_lg", size.Lg);
        Assign($"{prefix}_xl", size.Xl);
        Assign($"{prefix}_xxl", size.Xxl);
    }
}

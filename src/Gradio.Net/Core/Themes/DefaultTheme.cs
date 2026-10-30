namespace Gradio.Net;
/// <summary>
/// The preset Gradio itself uses: orange primary, blue secondary, zinc neutrals.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Default</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class DefaultTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Default</c> gives it.
    /// </remarks>
    public DefaultTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Orange,
            secondaryHue ?? Colors.Blue,
            neutralHue ?? Colors.Zinc,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusMd,
            font ?? new Font[] { new LocalFont("Source Sans Pro", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "default";

        Set(new ThemeOptions
        {
            BorderColorAccentSubdued = "*primary_200",
            CheckboxLabelBackgroundFill = "*background_fill_primary",
            CheckboxLabelBackgroundFillDark = "*neutral_800",
            CheckboxLabelBackgroundFillHover = "*background_fill_secondary",
            CheckboxLabelBackgroundFillHoverDark = "*checkbox_label_background_fill",
            ErrorBackgroundFillDark = "*neutral_900",
            InputBackgroundFill = "white",
            InputBackgroundFillDark = "*neutral_800",
            InputBorderWidth = "1px",
            InputShadowFocus = "0 0 0 *shadow_spread *secondary_50, *shadow_inset",
            InputShadowFocusDark = "0 0 0 *shadow_spread *neutral_700, *shadow_inset",
            StatBackgroundFill = "linear-gradient(to right, *primary_400, *primary_200)",
            StatBackgroundFillDark = "linear-gradient(to right, *primary_400, *primary_600)",
            ButtonBorderWidth = "0px",
            ButtonCancelBackgroundFill = "#ef4444",
            ButtonCancelBackgroundFillDark = "#b91c1c",
            ButtonCancelBackgroundFillHover = "#dc2626",
            ButtonCancelBackgroundFillHoverDark = "#991b1b",
            ButtonCancelTextColor = "white",
            ButtonCancelTextColorDark = "white",
            ButtonCancelTextColorHover = "white",
        });
    }
}

namespace Gradio.Net;
/// <summary>
/// The original Gradio look: orange and blue over gray.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Origin</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class OriginTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Origin</c> gives it.
    /// </remarks>
    public OriginTheme(
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
            neutralHue ?? Colors.Gray,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusMd,
            font ?? new Font[] { new LocalFont("Source Sans Pro", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "origin";

        Set(new ThemeOptions
        {
            BlockShadow = "*shadow_drop",
            FormGapWidth = "1px",
            BorderColorAccentSubdued = "*primary_200",
            CheckboxLabelBackgroundFill = "linear-gradient(to top, *neutral_50, white)",
            CheckboxLabelBackgroundFillDark = "linear-gradient(to top, *neutral_900, *neutral_800)",
            CheckboxLabelBackgroundFillHover = "linear-gradient(to top, *neutral_100, white)",
            CheckboxLabelBackgroundFillHoverDark = "linear-gradient(to top, *neutral_900, *neutral_800)",
            CheckboxLabelShadow = "*shadow_drop",
            ErrorBackgroundFillDark = "*neutral_900",
            InputBackgroundFill = "white",
            InputBackgroundFillDark = "*neutral_800",
            InputBorderWidth = "1px",
            InputShadow = "0 0 0 *shadow_spread transparent, *shadow_inset",
            InputShadowFocus = "0 0 0 *shadow_spread *secondary_50, *shadow_inset",
            InputShadowFocusDark = "0 0 0 *shadow_spread *neutral_700, *shadow_inset",
            StatBackgroundFill = "linear-gradient(to right, *primary_400, *primary_200)",
            StatBackgroundFillDark = "linear-gradient(to right, *primary_400, *primary_600)",
            ButtonBorderWidth = "1px",
            ButtonBorderWidthDark = "1px",
            ButtonCancelBackgroundFill = "linear-gradient(to bottom right, #fee2e2, #fecaca)",
            ButtonCancelBackgroundFillDark = "linear-gradient(to bottom right, #dc2626, #b91c1c)",
            ButtonCancelBackgroundFillHover = "linear-gradient(to bottom right, #fee2e2, #fee2e2)",
            ButtonCancelBackgroundFillHoverDark = "linear-gradient(to bottom right, #dc2626, #dc2626)",
            ButtonCancelBorderColor = "#fecaca",
            ButtonCancelBorderColorDark = "#dc2626",
            ButtonCancelBorderColorHover = "#fecaca",
            ButtonCancelBorderColorHoverDark = "#dc2626",
            ButtonCancelTextColor = "#dc2626",
            ButtonCancelTextColorDark = "white",
            ButtonTransition = "none",
            ButtonPrimaryBackgroundFill = "linear-gradient(to bottom right, *primary_100, *primary_300)",
            ButtonPrimaryBackgroundFillDark = "linear-gradient(to bottom right, *primary_500, *primary_600)",
            ButtonPrimaryBackgroundFillHover = "linear-gradient(to bottom right, *primary_100, *primary_200)",
            ButtonPrimaryBackgroundFillHoverDark = "linear-gradient(to bottom right, *primary_500, *primary_500)",
            ButtonPrimaryBorderColor = "*primary_200",
            ButtonPrimaryBorderColorDark = "*primary_500",
            ButtonPrimaryBorderColorHover = "*button_primary_border_color",
            ButtonPrimaryTextColor = "*primary_600",
            ButtonPrimaryShadow = "*shadow_drop",
            ButtonPrimaryShadowHover = "*shadow_drop_lg",
            ButtonPrimaryShadowActive = "*shadow_inset",
            ButtonSecondaryBackgroundFill = "linear-gradient(to bottom right, *neutral_100, *neutral_200)",
            ButtonSecondaryBackgroundFillDark = "linear-gradient(to bottom right, *neutral_600, *neutral_700)",
            ButtonSecondaryBackgroundFillHover = "linear-gradient(to bottom right, *neutral_100, *neutral_100)",
            ButtonSecondaryBackgroundFillHoverDark = "linear-gradient(to bottom right, *neutral_600, *neutral_600)",
            ButtonSecondaryBorderColorHoverDark = "*neutral_600",
            ButtonSecondaryTextColor = "*neutral_800",
            ButtonSecondaryShadow = "*shadow_drop",
            ButtonSecondaryShadowHover = "*shadow_drop_lg",
            ButtonSecondaryShadowActive = "*shadow_inset",
        });
    }
}

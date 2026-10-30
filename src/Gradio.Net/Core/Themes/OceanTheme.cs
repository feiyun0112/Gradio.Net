namespace Gradio.Net;
/// <summary>
/// A cool preset: emerald and sky over zinc.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Ocean</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class OceanTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Ocean</c> gives it.
    /// </remarks>
    public OceanTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Emerald,
            secondaryHue ?? Colors.Sky,
            neutralHue ?? Colors.Zinc,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusXxl,
            font ?? new Font[] { new LocalFont("IBM Plex Sans", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "ocean";

        Set(new ThemeOptions
        {
            BlockShadow = "*shadow_drop_lg",
            CheckboxBackgroundColorSelected = "*primary_400",
            CheckboxBackgroundColorSelectedDark = "*primary_600",
            CheckboxLabelBackgroundFillSelected = "linear-gradient(120deg, *primary_400 0%, *primary_300 60%, *primary_400 100%)",
            CheckboxLabelBackgroundFillSelectedDark = "linear-gradient(120deg, *primary_600 0%, *primary_500 60%, *primary_600 100%)",
            CheckboxLabelBorderColorSelected = "*primary_400",
            CheckboxLabelBorderColorSelectedDark = "*primary_600",
            CheckboxLabelBorderWidth = "1px",
            CheckboxLabelTextColorSelected = "*button_secondary_text_color",
            CheckboxLabelTextColorSelectedDark = "*button_secondary_text_color",
            SliderColor = "*primary_400",
            SliderColorDark = "*primary_500",
            ButtonBorderWidth = "0px",
            ButtonTransformHover = "scale(1.02)",
            ButtonTransition = "all 0.1s ease-in-out",
            ButtonPrimaryBackgroundFill = "linear-gradient(120deg, *secondary_500 0%, *primary_300 60%, *primary_400 100%)",
            ButtonPrimaryBackgroundFillDark = "linear-gradient(120deg, *secondary_600 0%, *primary_500 60%, *primary_600 100%)",
            ButtonPrimaryBackgroundFillHover = "linear-gradient(120deg, *secondary_400 0%, *primary_300 60%, *primary_300 100%)",
            ButtonPrimaryBackgroundFillHoverDark = "linear-gradient(120deg, *secondary_500 0%, *primary_500 60%, *primary_500 100%)",
            ButtonPrimaryTextColor = "*button_secondary_text_color",
            ButtonPrimaryTextColorDark = "*button_secondary_text_color",
            ButtonPrimaryShadowHover = "0 1px 3px 0 *primary_200, 0 1px 2px -1px *primary_200",
            ButtonPrimaryShadowDark = "none",
            ButtonSecondaryBackgroundFill = "linear-gradient(120deg, *neutral_300 0%, *neutral_100 60%, *neutral_200 100%)",
            ButtonSecondaryBackgroundFillDark = "linear-gradient(120deg, *neutral_700 0%, *neutral_600 60%, *neutral_700 100%)",
            ButtonSecondaryBackgroundFillHover = "linear-gradient(120deg, *neutral_200 0%, *neutral_100 60%, *neutral_100 100%)",
            ButtonSecondaryBackgroundFillHoverDark = "linear-gradient(120deg, *neutral_600 0%, *neutral_600 60%, *neutral_700 100%)",
            ButtonSecondaryShadowHover = "*shadow_drop_lg",
            ButtonSecondaryShadowDark = "none",
        });
    }
}

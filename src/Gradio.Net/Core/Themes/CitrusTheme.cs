namespace Gradio.Net;
/// <summary>
/// A bright preset: amber accents on stone, with large blocks.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Citrus</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class CitrusTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Citrus</c> gives it.
    /// </remarks>
    public CitrusTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Amber,
            secondaryHue ?? Colors.Amber,
            neutralHue ?? Colors.Stone,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingLg,
            radiusSize ?? Sizes.RadiusMd,
            font ?? new Font[] { new LocalFont("Ubuntu", [400]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("Roboto Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "citrus";

        Set(new ThemeOptions
        {
            BackgroundFillPrimary = "*neutral_50",
            BlockBackgroundFill = "*neutral_100",
            BlockBorderColor = "*neutral_300",
            BlockLabelShadow = "none",
            BlockShadow = "0px 3px 0px 0px *neutral_300;",
            BlockShadowDark = "0px 3px 0px 0px *neutral_700;",
            PanelBorderWidth = "1px",
            CheckboxBackgroundColorDark = "*neutral_400",
            CheckboxBackgroundColorSelectedDark = "*primary_600",
            CheckboxLabelBackgroundFill = "*neutral_200",
            CheckboxLabelBackgroundFillDark = "*neutral_700",
            CheckboxLabelBackgroundFillHover = "*checkbox_label_background_fill",
            CheckboxLabelBackgroundFillHoverDark = "*checkbox_label_background_fill",
            CheckboxLabelBackgroundFillSelected = "*primary_400",
            CheckboxLabelBackgroundFillSelectedDark = "*primary_500",
            CheckboxLabelBorderColorSelected = "*primary_300",
            CheckboxLabelBorderColorSelectedDark = "*primary_600",
            CheckboxLabelBorderWidth = "2px",
            CheckboxLabelBorderWidthDark = "2px",
            CheckboxShadow = "none",
            CheckboxLabelTextColorSelectedDark = "*button_primary_text_color",
            InputBackgroundFill = "*neutral_50",
            InputBackgroundFillDark = "*neutral_900",
            InputBackgroundFillFocus = "*primary_50",
            InputBackgroundFillFocusDark = "none",
            InputBorderWidth = "1px",
            InputShadow = "0px -1px 0px 0px *neutral_300;",
            InputShadowDark = "0px -1px 0px 0px *neutral_700;",
            InputShadowFocus = "0px -1px 0px 0px *primary_300;",
            InputShadowFocusDark = "0px -1px 0px 0px *primary_600;",
            SliderColor = "*primary_400",
            SliderColorDark = "*primary_500",
            ButtonBorderWidth = "0px",
            ButtonCancelBackgroundFill = "#ef4444",
            ButtonCancelBackgroundFillDark = "#b91c1c",
            ButtonCancelBackgroundFillHover = "#dc2626",
            ButtonCancelBackgroundFillHoverDark = "#991b1b",
            ButtonCancelTextColor = "white",
            ButtonCancelTextColorDark = "white",
            ButtonCancelTextColorHover = "white",
            ButtonCancelShadow = "0px 3px 0px 0px rgb(248 113 113)",
            ButtonCancelShadowHover = "0px 5px 0px 0px rgb(248 113 113)",
            ButtonCancelShadowActive = "0px 2px 0px 0px rgb(248 113 113)",
            ButtonCancelShadowDark = "0px 3px 0px 0px rgb(220 38 38)",
            ButtonCancelShadowHoverDark = "0px 5px 0px 0px rgb(220 38 38)",
            ButtonCancelShadowActiveDark = "0px 2px 0px 0px rgb(220 38 38)",
            ButtonTransformHover = "translateY(-2px);",
            ButtonTransformActive = "translateY(1px);",
            ButtonTransition = "all 0.1s;",
            ButtonPrimaryBackgroundFillHover = "*button_primary_background_fill",
            ButtonPrimaryBackgroundFillHoverDark = "*button_primary_background_fill",
            ButtonPrimaryTextColor = "*button_secondary_text_color",
            ButtonPrimaryTextColorDark = "*button_secondary_text_color",
            ButtonPrimaryShadow = "0px 3px 0px 0px *primary_400;",
            ButtonPrimaryShadowHover = "0px 5px 0px 0px *primary_400;",
            ButtonPrimaryShadowActive = "0px 2px 0px 0px *primary_400;",
            ButtonPrimaryShadowDark = "0px 3px 0px 0px *primary_700;",
            ButtonPrimaryShadowHoverDark = "0px 5px 0px 0px *primary_700;",
            ButtonPrimaryShadowActiveDark = "0px 2px 0px 0px *primary_700;",
            ButtonSecondaryBackgroundFill = "*primary_400",
            ButtonSecondaryBackgroundFillDark = "*primary_500",
            ButtonSecondaryBackgroundFillHover = "*button_secondary_background_fill",
            ButtonSecondaryBackgroundFillHoverDark = "*button_secondary_background_fill",
            ButtonSecondaryTextColorDark = "*neutral_900",
            ButtonSecondaryShadow = "0px 3px 0px 0px *primary_300;",
            ButtonSecondaryShadowHover = "0px 5px 0px 0px *primary_300;",
            ButtonSecondaryShadowActive = "0px 2px 0px 0px *primary_300;",
            ButtonSecondaryShadowDark = "0px 3px 0px 0px *primary_600;",
            ButtonSecondaryShadowHoverDark = "0px 5px 0px 0px *primary_600;",
            ButtonSecondaryShadowActiveDark = "0px 2px 0px 0px *primary_600;",
        });
    }
}

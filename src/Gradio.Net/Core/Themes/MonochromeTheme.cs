namespace Gradio.Net;
/// <summary>
/// A single-hue preset with square corners and a serif body face.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Monochrome</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class MonochromeTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Monochrome</c> gives it.
    /// </remarks>
    public MonochromeTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Neutral,
            secondaryHue ?? Colors.Neutral,
            neutralHue ?? Colors.Neutral,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingLg,
            radiusSize ?? Sizes.RadiusNone,
            font ?? new Font[] { new LocalFont("Lora", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "monochrome";

        Set(new ThemeOptions
        {
            BodyTextColor = "*neutral_900",
            BackgroundFillPrimaryDark = "*neutral_900",
            BackgroundFillSecondaryDark = "*neutral_800",
            BodyTextColorSubdued = "*neutral_700",
            ShadowDropLg = "0 1px 4px 0 rgb(0 0 0 / 0.1)",
            BlockLabelTextColor = "*body_text_color",
            BlockLabelTextSize = "*text_md",
            BlockLabelTextWeight = "600",
            BlockShadowDark = "*shadow_drop_lg",
            BlockTitleTextColor = "*body_text_color",
            BlockTitleTextWeight = "600",
            CheckboxBackgroundColor = "*input_background_fill",
            CheckboxLabelBackgroundFillSelected = "*button_primary_background_fill",
            CheckboxLabelBorderColor = "*checkbox_background_color",
            CheckboxLabelBorderColorHover = "*button_secondary_border_color_hover",
            CheckboxLabelBorderColorSelected = "*button_primary_border_color",
            CheckboxLabelBorderWidth = "*button_border_width",
            CheckboxLabelPadding = "*spacing_sm",
            CheckboxLabelTextColorSelected = "*button_primary_text_color",
            SliderColor = "*neutral_900",
            SliderColorDark = "*neutral_500",
            ButtonBorderWidth = "2px",
            ButtonCancelBorderColor = "*neutral_900",
            ButtonLargePadding = "*spacing_lg",
            ButtonPrimaryBackgroundFill = "*neutral_900",
            ButtonPrimaryBackgroundFillDark = "*neutral_600",
            ButtonPrimaryBackgroundFillHover = "*neutral_700",
            ButtonPrimaryBackgroundFillHoverDark = "*neutral_600",
            ButtonPrimaryBorderColor = "*neutral_900",
            ButtonSecondaryBackgroundFill = "white",
            ButtonSecondaryBackgroundFillDark = "*neutral_700",
            ButtonSecondaryBackgroundFillHover = "white",
            ButtonSecondaryBorderColor = "*neutral_900",
            ButtonSecondaryBorderColorHover = "*neutral_400",
            ButtonSecondaryTextColor = "*neutral_900",
            ButtonSecondaryTextColorHover = "*neutral_400",
            ButtonSmallPadding = "*spacing_sm",
        });
    }
}

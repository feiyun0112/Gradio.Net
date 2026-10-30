namespace Gradio.Net;
/// <summary>
/// A quiet preset: indigo on gray, rounded and low-contrast.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Soft</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class SoftTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Soft</c> gives it.
    /// </remarks>
    public SoftTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Indigo,
            secondaryHue ?? Colors.Indigo,
            neutralHue ?? Colors.Gray,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusMd,
            font ?? new Font[] { new LocalFont("Montserrat", [400, 700]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "soft";

        Set(new ThemeOptions
        {
            BackgroundFillPrimary = "*neutral_50",
            ShadowDrop = "0 1px 4px 0 rgb(0 0 0 / 0.1)",
            ShadowDropLg = "0 2px 5px 0 rgb(0 0 0 / 0.1)",
            ShadowSpread = "6px",
            BlockBackgroundFill = "white",
            BlockBorderWidth = "0px",
            BlockLabelBackgroundFill = "*primary_100",
            BlockLabelBackgroundFillDark = "*primary_600",
            BlockLabelTextColor = "*primary_500",
            BlockLabelTextColorDark = "white",
            BlockLabelMargin = "*spacing_md",
            BlockLabelPadding = "*spacing_sm *spacing_md",
            BlockLabelRadius = "*radius_md",
            BlockLabelTextSize = "*text_md",
            BlockLabelTextWeight = "600",
            BlockTitleBackgroundFill = "*block_label_background_fill",
            BlockTitleTextColor = "*primary_500",
            BlockTitleTextColorDark = "white",
            BlockTitlePadding = "*block_label_padding",
            BlockTitleRadius = "*block_label_radius",
            BlockTitleTextWeight = "600",
            PanelBorderWidth = "1px",
            CheckboxBackgroundColorSelected = "*primary_600",
            CheckboxBackgroundColorSelectedDark = "*primary_700",
            CheckboxBorderColor = "*neutral_100",
            CheckboxBorderColorDark = "*neutral_600",
            CheckboxBorderColorFocus = "*primary_500",
            CheckboxBorderColorFocusDark = "*primary_600",
            CheckboxBorderColorSelected = "*primary_600",
            CheckboxBorderColorSelectedDark = "*primary_700",
            CheckboxBorderWidth = "1px",
            CheckboxLabelBackgroundFillSelected = "*primary_500",
            CheckboxLabelBackgroundFillSelectedDark = "*primary_600",
            CheckboxLabelShadow = "*shadow_drop_lg",
            CheckboxShadow = "none",
            CheckboxLabelTextColorSelected = "white",
            InputBackgroundFill = "white",
            InputBorderColor = "*neutral_50",
            InputShadow = "*shadow_drop",
            InputShadowFocus = "*shadow_drop_lg",
            SliderColor = "*primary_500",
            SliderColorDark = "*primary_600",
            ButtonPrimaryBackgroundFillHover = "*primary_400",
            ButtonPrimaryBackgroundFillHoverDark = "*primary_500",
            ButtonPrimaryShadow = "*shadow_drop_lg",
            ButtonPrimaryShadowHover = "*shadow_drop_lg",
            ButtonPrimaryShadowActive = "*shadow_inset",
            ButtonSecondaryBackgroundFill = "white",
            ButtonSecondaryBackgroundFillHover = "*neutral_100",
            ButtonSecondaryBackgroundFillHoverDark = "*primary_500",
            ButtonSecondaryTextColor = "*neutral_800",
            ButtonSecondaryShadow = "*shadow_drop_lg",
            ButtonSecondaryShadowHover = "*shadow_drop_lg",
            ButtonSecondaryShadowActive = "*shadow_inset",
        });
    }
}

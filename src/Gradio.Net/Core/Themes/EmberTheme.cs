namespace Gradio.Net;
/// <summary>
/// A warm preset: orange on stone, with larger corners and soft shadows.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Ember</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class EmberTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Ember</c> gives it.
    /// </remarks>
    public EmberTheme(
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
            secondaryHue ?? Colors.Orange,
            neutralHue ?? Colors.Stone,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusLg,
            font ?? new Font[] { new LocalFont("Source Sans 3", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("Source Code Pro", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "ember";

        Set(new ThemeOptions
        {
            BodyBackgroundFill = "*neutral_50",
            BodyBackgroundFillDark = "*neutral_950",
            BodyTextColorDark = "*neutral_200",
            ColorAccentSoftDark = "*primary_900",
            BackgroundFillPrimaryDark = "*neutral_900",
            BackgroundFillSecondary = "*neutral_100",
            BackgroundFillSecondaryDark = "*neutral_800",
            BodyTextColorSubdued = "*neutral_500",
            ShadowDrop = "0 1px 3px 0 rgb(0 0 0 / 0.08)",
            ShadowDropLg = "0 4px 12px 0 rgb(0 0 0 / 0.1)",
            ShadowSpread = "6px",
            BlockBackgroundFill = "white",
            BlockBackgroundFillDark = "*neutral_900",
            BlockBorderWidth = "0px",
            BlockLabelBackgroundFill = "*primary_50",
            BlockLabelBackgroundFillDark = "*primary_900",
            BlockLabelTextColor = "*primary_600",
            BlockLabelTextColorDark = "*primary_300",
            BlockLabelPadding = "*spacing_sm *spacing_md",
            BlockLabelRadius = "*radius_md",
            BlockLabelTextSize = "*text_md",
            BlockLabelTextWeight = "600",
            BlockShadow = "*shadow_drop",
            BlockShadowDark = "0 1px 4px 0 rgb(0 0 0 / 0.3)",
            BlockTitleBackgroundFill = "*primary_50",
            BlockTitleBackgroundFillDark = "*primary_900",
            BlockTitleTextColor = "*primary_600",
            BlockTitleTextColorDark = "*primary_300",
            BlockTitlePadding = "*spacing_sm *spacing_md",
            BlockTitleRadius = "*radius_md",
            BlockTitleTextWeight = "600",
            PanelBorderColor = "*neutral_200",
            PanelBorderColorDark = "*neutral_700",
            PanelBorderWidth = "1px",
            CheckboxBackgroundColorSelected = "*primary_500",
            CheckboxBackgroundColorSelectedDark = "*primary_500",
            CheckboxBorderColorDark = "*neutral_500",
            CheckboxBorderColorSelected = "*primary_500",
            CheckboxBorderColorSelectedDark = "*primary_500",
            CheckboxLabelBackgroundFill = "white",
            CheckboxLabelBackgroundFillDark = "*neutral_800",
            CheckboxLabelBackgroundFillSelected = "*primary_500",
            CheckboxLabelBackgroundFillSelectedDark = "*primary_600",
            CheckboxLabelShadow = "*shadow_drop",
            CheckboxShadow = "none",
            CheckboxLabelTextColorSelected = "white",
            InputBackgroundFill = "white",
            InputBackgroundFillDark = "*neutral_800",
            InputBorderColor = "*neutral_200",
            InputBorderColorDark = "*neutral_600",
            InputBorderColorFocus = "*primary_400",
            InputBorderColorFocusDark = "*primary_500",
            InputBorderWidth = "1px",
            InputShadowFocus = "0 0 0 3px *primary_100",
            InputShadowFocusDark = "0 0 0 3px *primary_900",
            LoaderColor = "*primary_500",
            LoaderColorDark = "*primary_400",
            SliderColor = "*primary_500",
            SliderColorDark = "*primary_400",
            TableEvenBackgroundFill = "*neutral_50",
            TableEvenBackgroundFillDark = "*neutral_800",
            TableOddBackgroundFill = "white",
            ButtonBorderWidth = "0px",
            ButtonCancelBackgroundFill = "#ef4444",
            ButtonCancelBackgroundFillDark = "#dc2626",
            ButtonCancelBackgroundFillHover = "#dc2626",
            ButtonCancelBackgroundFillHoverDark = "#b91c1c",
            ButtonCancelTextColor = "white",
            ButtonCancelTextColorDark = "white",
            ButtonTransition = "all 0.15s ease",
            ButtonPrimaryBackgroundFillHoverDark = "*primary_500",
            ButtonPrimaryShadow = "0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)",
            ButtonPrimaryShadowHover = "0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)",
            ButtonPrimaryShadowActive = "none",
            ButtonSecondaryBackgroundFill = "white",
            ButtonSecondaryBackgroundFillDark = "*neutral_800",
            ButtonSecondaryBackgroundFillHover = "*neutral_50",
            ButtonSecondaryTextColor = "*neutral_700",
            ButtonSecondaryTextColorDark = "*neutral_200",
            ButtonSecondaryShadow = "*shadow_drop",
            ButtonSecondaryShadowHover = "*shadow_drop_lg",
        });
    }
}

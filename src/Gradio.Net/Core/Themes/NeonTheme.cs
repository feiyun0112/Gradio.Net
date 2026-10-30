namespace Gradio.Net;
/// <summary>
/// A high-contrast dark preset with lime and cyan accents.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Neon</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class NeonTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Neon</c> gives it.
    /// </remarks>
    public NeonTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Lime,
            secondaryHue ?? Colors.Cyan,
            neutralHue ?? Colors.Neutral,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingLg,
            radiusSize ?? Sizes.RadiusXxl,
            font ?? new Font[] { new LocalFont("Lexend", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("Space Mono", [400]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "neon";

        Set(new ThemeOptions
        {
            BodyBackgroundFill = "*neutral_100",
            BodyBackgroundFillDark = "*neutral_950",
            BodyTextColorDark = "*neutral_200",
            ColorAccent = "*secondary_500",
            ColorAccentSoft = "*secondary_100",
            ColorAccentSoftDark = "*secondary_900",
            BackgroundFillPrimary = "*neutral_100",
            BackgroundFillPrimaryDark = "*neutral_900",
            BackgroundFillSecondary = "*neutral_200",
            BackgroundFillSecondaryDark = "*neutral_800",
            BodyTextColorSubdued = "*neutral_500",
            ShadowDrop = "none",
            ShadowDropLg = "none",
            ShadowSpread = "0px",
            BlockBackgroundFill = "white",
            BlockBackgroundFillDark = "*neutral_900",
            BlockBorderColor = "*neutral_300",
            BlockBorderColorDark = "*neutral_700",
            BlockBorderWidth = "2px",
            BlockLabelBackgroundFill = "*primary_200",
            BlockLabelBackgroundFillDark = "*primary_800",
            BlockLabelTextColor = "*primary_800",
            BlockLabelTextColorDark = "*primary_200",
            BlockLabelRadius = "0 0 calc(*radius_sm - 1px) 0",
            BlockLabelTextSize = "*text_md",
            BlockLabelTextWeight = "600",
            BlockShadow = "0px 3px 0px 0px *neutral_300",
            BlockShadowDark = "0px 3px 0px 0px *neutral_800",
            BlockTitleBackgroundFill = "*primary_200",
            BlockTitleBackgroundFillDark = "*primary_800",
            BlockTitleTextColor = "*primary_800",
            BlockTitleTextColorDark = "*primary_200",
            BlockTitlePadding = "*spacing_sm *spacing_lg",
            BlockTitleRadius = "*radius_sm",
            BlockTitleTextWeight = "600",
            PanelBorderColor = "*neutral_300",
            PanelBorderColorDark = "*neutral_700",
            PanelBorderWidth = "2px",
            CheckboxBackgroundColorSelected = "*primary_500",
            CheckboxBackgroundColorSelectedDark = "*primary_400",
            CheckboxBorderColor = "*neutral_400",
            CheckboxBorderColorDark = "*neutral_500",
            CheckboxBorderColorSelected = "*primary_600",
            CheckboxBorderColorSelectedDark = "*primary_500",
            CheckboxBorderWidth = "2px",
            CheckboxLabelBackgroundFill = "white",
            CheckboxLabelBackgroundFillDark = "*neutral_800",
            CheckboxLabelBackgroundFillHover = "*primary_50",
            CheckboxLabelBackgroundFillHoverDark = "*neutral_700",
            CheckboxLabelBackgroundFillSelected = "*primary_300",
            CheckboxLabelBackgroundFillSelectedDark = "*primary_700",
            CheckboxLabelBorderColor = "*neutral_300",
            CheckboxLabelBorderColorDark = "*neutral_600",
            CheckboxLabelBorderColorSelected = "*primary_500",
            CheckboxLabelBorderColorSelectedDark = "*primary_500",
            CheckboxLabelBorderWidth = "2px",
            CheckboxLabelShadow = "0px 4px 0px 0px *neutral_300",
            CheckboxLabelShadowDark = "0px 4px 0px 0px *neutral_700",
            CheckboxLabelShadowHover = "0px 6px 0px 0px *neutral_300",
            CheckboxLabelShadowHoverDark = "0px 6px 0px 0px *neutral_700",
            CheckboxLabelShadowActive = "0px 2px 0px 0px *neutral_300",
            CheckboxLabelShadowActiveDark = "0px 2px 0px 0px *neutral_700",
            CheckboxShadow = "none",
            CheckboxLabelTextColorSelected = "*primary_900",
            CheckboxLabelTextColorSelectedDark = "*primary_100",
            InputBackgroundFill = "white",
            InputBackgroundFillDark = "*neutral_800",
            InputBackgroundFillFocus = "*primary_50",
            InputBackgroundFillFocusDark = "*neutral_700",
            InputBorderColor = "*neutral_300",
            InputBorderColorDark = "*neutral_600",
            InputBorderColorFocus = "*primary_500",
            InputBorderColorFocusDark = "*primary_400",
            InputBorderWidth = "2px",
            InputShadowFocus = "none",
            LoaderColor = "*primary_500",
            LoaderColorDark = "*primary_400",
            SliderColor = "*primary_500",
            SliderColorDark = "*primary_400",
            TableBorderColorDark = "*neutral_600",
            TableEvenBackgroundFill = "*neutral_50",
            TableEvenBackgroundFillDark = "*neutral_800",
            TableOddBackgroundFill = "white",
            ButtonBorderWidth = "2px",
            ButtonCancelBackgroundFill = "#f87171",
            ButtonCancelBackgroundFillDark = "#ef4444",
            ButtonCancelBackgroundFillHover = "#f87171",
            ButtonCancelBackgroundFillHoverDark = "#ef4444",
            ButtonCancelBorderColor = "#ef4444",
            ButtonCancelBorderColorDark = "#dc2626",
            ButtonCancelTextColor = "white",
            ButtonCancelTextColorDark = "white",
            ButtonCancelShadow = "0px 4px 0px 0px #dc2626",
            ButtonCancelShadowHover = "0px 6px 0px 0px #dc2626",
            ButtonCancelShadowActive = "0px 2px 0px 0px #dc2626",
            ButtonCancelShadowDark = "0px 4px 0px 0px #b91c1c",
            ButtonCancelShadowHoverDark = "0px 6px 0px 0px #b91c1c",
            ButtonCancelShadowActiveDark = "0px 2px 0px 0px #b91c1c",
            ButtonTransformHover = "translateY(-2px)",
            ButtonTransformActive = "translateY(1px)",
            ButtonTransition = "all 0.1s ease",
            ButtonPrimaryBackgroundFill = "*primary_400",
            ButtonPrimaryBackgroundFillDark = "*primary_500",
            ButtonPrimaryBackgroundFillHover = "*primary_400",
            ButtonPrimaryBackgroundFillHoverDark = "*primary_500",
            ButtonPrimaryTextColor = "*primary_950",
            ButtonPrimaryTextColorDark = "*primary_950",
            ButtonPrimaryShadow = "0px 4px 0px 0px *primary_600",
            ButtonPrimaryShadowHover = "0px 6px 0px 0px *primary_600",
            ButtonPrimaryShadowActive = "0px 2px 0px 0px *primary_600",
            ButtonPrimaryShadowDark = "0px 4px 0px 0px *primary_700",
            ButtonPrimaryShadowHoverDark = "0px 6px 0px 0px *primary_700",
            ButtonPrimaryShadowActiveDark = "0px 2px 0px 0px *primary_700",
            ButtonSecondaryBackgroundFill = "white",
            ButtonSecondaryBackgroundFillDark = "*neutral_800",
            ButtonSecondaryBackgroundFillHover = "white",
            ButtonSecondaryBackgroundFillHoverDark = "*neutral_800",
            ButtonSecondaryBorderColor = "*neutral_300",
            ButtonSecondaryTextColor = "*neutral_700",
            ButtonSecondaryTextColorDark = "*neutral_200",
            ButtonSecondaryShadow = "0px 4px 0px 0px *neutral_300",
            ButtonSecondaryShadowHover = "0px 6px 0px 0px *neutral_300",
            ButtonSecondaryShadowActive = "0px 2px 0px 0px *neutral_300",
            ButtonSecondaryShadowDark = "0px 4px 0px 0px *neutral_700",
            ButtonSecondaryShadowHoverDark = "0px 6px 0px 0px *neutral_700",
            ButtonSecondaryShadowActiveDark = "0px 2px 0px 0px *neutral_700",
        });
    }
}

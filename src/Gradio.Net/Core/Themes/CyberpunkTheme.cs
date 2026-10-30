namespace Gradio.Net;
/// <summary>
/// A novelty preset with magenta and cyan accents and angular edges.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Cyberpunk</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class CyberpunkTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Cyberpunk</c> gives it.
    /// </remarks>
    public CyberpunkTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Fuchsia,
            secondaryHue ?? Colors.Cyan,
            neutralHue ?? Colors.Slate,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusSm,
            font ?? new Font[] { new LocalFont("Chakra Petch", [400, 600]), "ui-sans-serif", "system-ui", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("Share Tech Mono", [400]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "cyberpunk";

        Set(new ThemeOptions
        {
            BodyBackgroundFill = "*neutral_100",
            BodyBackgroundFillDark = "#0a0416",
            BodyTextColorDark = "*neutral_300",
            ColorAccent = "*secondary_500",
            ColorAccentSoft = "*secondary_50",
            ColorAccentSoftDark = "#164e6360",
            BackgroundFillPrimary = "*neutral_50",
            BackgroundFillPrimaryDark = "#110b22",
            BackgroundFillSecondary = "*neutral_200",
            BackgroundFillSecondaryDark = "#1a1030",
            BodyTextColorSubdued = "*neutral_500",
            ShadowDrop = "none",
            ShadowDropLg = "none",
            ShadowSpread = "0px",
            BlockBackgroundFill = "white",
            BlockBackgroundFillDark = "#110b22",
            BlockBorderColor = "*neutral_300",
            BlockBorderColorDark = "#5b21b660",
            BlockBorderWidth = "2px",
            BlockLabelBackgroundFill = "*primary_100",
            BlockLabelBackgroundFillDark = "#701a7580",
            BlockLabelTextColor = "*primary_700",
            BlockLabelTextColorDark = "*primary_300",
            BlockLabelRadius = "0",
            BlockLabelTextWeight = "700",
            BlockShadowDark = "none",
            BlockTitleBackgroundFill = "*primary_100",
            BlockTitleBackgroundFillDark = "#701a7580",
            BlockTitleTextColor = "*primary_700",
            BlockTitleTextColorDark = "*primary_300",
            BlockTitlePadding = "*spacing_sm *spacing_lg",
            BlockTitleRadius = "0",
            BlockTitleTextWeight = "700",
            PanelBorderColor = "*neutral_300",
            PanelBorderColorDark = "#5b21b660",
            PanelBorderWidth = "2px",
            CheckboxBackgroundColorSelected = "*primary_500",
            CheckboxBackgroundColorSelectedDark = "*primary_500",
            CheckboxBorderColor = "*neutral_400",
            CheckboxBorderColorDark = "#7c3aed60",
            CheckboxBorderColorSelected = "*primary_600",
            CheckboxBorderColorSelectedDark = "*primary_500",
            CheckboxBorderWidth = "2px",
            CheckboxLabelBackgroundFill = "white",
            CheckboxLabelBackgroundFillDark = "#1a1030",
            CheckboxLabelBackgroundFillHover = "*primary_50",
            CheckboxLabelBackgroundFillHoverDark = "#241540",
            CheckboxLabelBackgroundFillSelected = "*primary_600",
            CheckboxLabelBackgroundFillSelectedDark = "*primary_800",
            CheckboxLabelBorderColor = "*neutral_300",
            CheckboxLabelBorderColorDark = "#6d28d950",
            CheckboxLabelBorderColorSelected = "*primary_600",
            CheckboxLabelBorderColorSelectedDark = "*primary_600",
            CheckboxLabelBorderWidth = "2px",
            CheckboxShadow = "none",
            CheckboxLabelTextColorSelected = "white",
            CheckboxLabelTextColorSelectedDark = "*primary_200",
            InputBackgroundFill = "white",
            InputBackgroundFillDark = "#1a1030",
            InputBackgroundFillFocus = "white",
            InputBackgroundFillFocusDark = "#1e1238",
            InputBorderColor = "*neutral_300",
            InputBorderColorDark = "#6d28d950",
            InputBorderColorFocus = "*primary_500",
            InputBorderColorFocusDark = "*primary_500",
            InputBorderWidth = "2px",
            InputShadowFocus = "0 0 0 2px #f0abfc",
            InputShadowFocusDark = "0 0 0 2px #d946ef80, 0 0 16px -2px #c026d330",
            LoaderColor = "*secondary_500",
            LoaderColorDark = "*secondary_400",
            SliderColor = "*secondary_500",
            SliderColorDark = "*secondary_400",
            TableBorderColorDark = "#5b21b660",
            TableEvenBackgroundFill = "*neutral_50",
            TableEvenBackgroundFillDark = "#1a1030",
            TableOddBackgroundFill = "white",
            TableOddBackgroundFillDark = "#110b22",
            TableRowFocus = "*primary_50",
            TableRowFocusDark = "#241540",
            ButtonBorderWidth = "2px",
            ButtonCancelBackgroundFill = "white",
            ButtonCancelBackgroundFillDark = "#1a1030",
            ButtonCancelBackgroundFillHover = "#fef2f2",
            ButtonCancelBackgroundFillHoverDark = "#7f1d1d40",
            ButtonCancelBorderColor = "#fca5a5",
            ButtonCancelBorderColorDark = "#b91c1c60",
            ButtonCancelTextColor = "#dc2626",
            ButtonCancelTextColorDark = "#f87171",
            ButtonTransition = "all 0.1s ease",
            ButtonPrimaryBackgroundFillHoverDark = "*primary_500",
            ButtonPrimaryBorderColor = "*primary_600",
            ButtonPrimaryBorderColorDark = "*primary_500",
            ButtonPrimaryShadowHover = "none",
            ButtonPrimaryShadowActive = "none",
            ButtonPrimaryShadowDark = "0 0 12px -2px #d946ef50",
            ButtonPrimaryShadowHoverDark = "0 0 20px -2px #d946ef60",
            ButtonPrimaryShadowActiveDark = "none",
            ButtonSecondaryBackgroundFill = "white",
            ButtonSecondaryBackgroundFillDark = "#1a1030",
            ButtonSecondaryBackgroundFillHover = "*neutral_100",
            ButtonSecondaryBackgroundFillHoverDark = "#241540",
            ButtonSecondaryBorderColor = "*neutral_300",
            ButtonSecondaryBorderColorDark = "#6d28d950",
            ButtonSecondaryTextColor = "*neutral_700",
            ButtonSecondaryTextColorDark = "*neutral_300",
            ButtonSecondaryShadow = "none",
        });
    }
}

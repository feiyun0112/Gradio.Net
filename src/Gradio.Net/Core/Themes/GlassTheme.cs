namespace Gradio.Net;
/// <summary>
/// A cool, translucent preset built on slate.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Glass</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class GlassTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Glass</c> gives it.
    /// </remarks>
    public GlassTheme(
        Color? primaryHue = null,
        Color? secondaryHue = null,
        Color? neutralHue = null,
        Size? textSize = null,
        Size? spacingSize = null,
        Size? radiusSize = null,
        IEnumerable<Font>? font = null,
        IEnumerable<Font>? fontMono = null)
        : base(
            primaryHue ?? Colors.Blue,
            secondaryHue ?? Colors.Slate,
            neutralHue ?? Colors.Slate,
            textSize ?? Sizes.TextSm,
            spacingSize ?? Sizes.SpacingSm,
            radiusSize ?? Sizes.RadiusSm,
            font ?? new Font[] { "Optima", "Candara", "Noto Sans", "source-sans-pro", "sans-serif" },
            fontMono ?? new Font[] { new LocalFont("IBM Plex Mono", [400, 600]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "glass";

        Set(new ThemeOptions
        {
            BodyBackgroundFillDark = "*secondary_800",
            BackgroundFillPrimaryDark = "*neutral_700",
            BackgroundFillSecondary = "*secondary_100",
            BackgroundFillSecondaryDark = "*secondary_800",
            BorderColorPrimaryDark = "*secondary_600",
            ShadowInset = "rgba(0,0,0,0.05) 0px 1px 2px 0px inset",
            BlockBackgroundFill = "*secondary_50",
            BlockBackgroundFillDark = "*secondary_800",
            BlockBorderColorDark = "*secondary_600",
            BlockBorderWidth = "0px",
            BlockBorderWidthDark = "1px",
            BlockLabelBackgroundFill = "*secondary_200",
            BlockLabelBackgroundFillDark = "*secondary_700",
            BlockLabelBorderColorDark = "*secondary_600",
            BlockLabelTextColor = "*secondary_500",
            BlockLabelTextSize = "*text_md",
            BlockLabelTextWeight = "600",
            BlockTitleTextColor = "*secondary_500",
            BlockTitleTextWeight = "600",
            PanelBorderWidth = "1px",
            CheckboxBackgroundColorDark = "*secondary_400",
            CheckboxBorderWidth = "0px",
            CheckboxBorderWidthDark = "0px",
            CheckboxLabelBorderColorDark = "*secondary_700",
            CheckboxLabelBorderWidth = "1px",
            CheckboxLabelBorderWidthDark = "1px",
            InputBackgroundFill = "linear-gradient(0deg, *secondary_100 0%, white 100%)",
            InputBackgroundFillDark = "*secondary_600",
            InputBorderColorDark = "*secondary_600",
            InputBorderColorFocusDark = "*secondary_500",
            InputBorderWidth = "1px",
            TableEvenBackgroundFillDark = "*neutral_700",
            TableOddBackgroundFillDark = "*neutral_700",
            ButtonBorderWidth = "1px",
            ButtonPrimaryBackgroundFill = "linear-gradient(180deg, *primary_100 0%, *primary_200 30%)",
            ButtonPrimaryBackgroundFillDark = "linear-gradient(180deg, *primary_500 0%, *primary_600 30%)",
            ButtonPrimaryBackgroundFillHover = "linear-gradient(180deg, *primary_100 0%, *primary_200 30%)",
            ButtonPrimaryBackgroundFillHoverDark = "linear-gradient(180deg, *primary_500 0%, *primary_600 30%)",
            ButtonPrimaryBorderColor = "*primary_200",
            ButtonPrimaryBorderColorHover = "*button_primary_border_color",
            ButtonPrimaryTextColor = "*body_text_color",
            ButtonPrimaryShadowActive = "*shadow_inset",
            ButtonSecondaryBackgroundFill = "linear-gradient(180deg, *secondary_100 0%, *secondary_200 30%)",
            ButtonSecondaryBackgroundFillDark = "linear-gradient(180deg, *secondary_500 0%, *secondary_600 30%)",
            ButtonSecondaryBackgroundFillHover = "linear-gradient(180deg, *secondary_100 0%, *secondary_200 30%)",
            ButtonSecondaryBackgroundFillHoverDark = "linear-gradient(180deg, *secondary_500 0%, *secondary_600 30%)",
            ButtonSecondaryShadowActive = "*shadow_inset",
        });
    }
}

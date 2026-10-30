namespace Gradio.Net;
/// <summary>
/// A novelty preset styled after a platform game, carried almost entirely by custom CSS.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Mario</c>. It is a <see cref="BaseTheme"/> with its own constructor
/// defaults and the overrides below; the order of the overrides does not matter, because every
/// variable they set is already there and an assignment keeps its place in the stylesheet.
/// </para>
/// <para>
/// Its stylesheet is compared against the Python version's byte for byte, which is what makes these
/// values checkable rather than trusted — and what makes it safe to generate them.
/// </para>
/// </remarks>
public sealed class MarioTheme : BaseTheme
{
    /// <summary>Creates the preset with the Python version's defaults, or with the ones
    /// given.</summary>
    /// <remarks>
    /// Every parameter means what it means on <see cref="BaseTheme"/>; a parameter left out
    /// takes the value the Python version's <c>Mario</c> gives it.
    /// </remarks>
    public MarioTheme(
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
            secondaryHue ?? Colors.Green,
            neutralHue ?? Colors.Stone,
            textSize ?? Sizes.TextMd,
            spacingSize ?? Sizes.SpacingMd,
            radiusSize ?? Sizes.RadiusSm,
            font ?? new Font[] { new LocalFont("Press Start 2P", [400]), "ui-monospace", "monospace" },
            fontMono ?? new Font[] { new LocalFont("Press Start 2P", [400]), "ui-monospace", "Consolas", "monospace" })
    {
        Name = "mario";

        // One literal on one line, with the line breaks written as escapes: the value is
        // compared byte for byte, and a raw string would carry whatever line endings the
        // working copy happens to have.
        CustomCss = "\n.gradio-container {\n    background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAgAAAAEACAIAAABK8lkwAAAD0UlEQVR42u3dTY6CQBRG0Y+GZboDB+g+SNA9uE2Cox61xibd/BScMzSManLzqoSqzt0YAI7nyxIACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAQJI0loBS3C/Dy9/bW21xwAQAgAAAIAAA/FSdu9EqkNL2/d9xHgAmAAAEAAABACDOAMhe9v3jPABMAAAIAAACAIAAACAAAAIAgAAAEO8BZMP/Aff/7uO4nqY93z+sGZgAABAAAAQAgKx8BvBf33uJswEAEwAAAgCAAACQtc8A5tj3j/MAABMAAAIAgAAAkDXOAJbc949zAgATAAACAIAAAPCtsQTAlrkjxAQAgAAAIAAApIhvAU2933UO7oyFFLvXH+/6mAAAEAAABACAlHAnMIA7QkwAAAgAAAIAQJwBALHvH+cEJgAABAAAAQBAAAAQAAAEAAABAMB7AMCK3BFiAgBAAAAQAABm1VgCIPbfTQAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAgAAAIAAALK2xBPtzvwwfn2lvtYUCEwAAAgCAAACwb9W5G61CjrHvH+cBgAkAAAEAEAAABAAAAQBAAAAQAADiPQC25nr6/Ez/sE5gAgBAAAAQAADiPgAKY38fMAEAIAAACAAAAgAgAAAIAAACAIAAACAAAAgAAAIAQHwLCFjE/TK8/L291RYHEwAAAgCAAADgTmDIPvb933EegAkAAAEAQAAA4gwAyF72/eM8ABMAAAIAgAAAIAAAAgCAAAAgAADEewDAFlxP057vH9YMEwAAAgCAAADEncBAAezpYwIAQAAAEAAAUtAZwG++de475gAmAAAEAAABACBFfAvoL3ecOg8AMAEAIAAACAAAAgCAAAAgAAAIAEC8B5At33HqG+gAJgAABAAAAQAgxd0HYH8fwAQAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAIAgAAAIAAACAAAAgCAAAAgAAAIAAACAIAAACAAAAgAAAIAwOKeP55sGdNyCWcAAAAASUVORK5CYII=\") repeat !important;\n}\n\n.label-wrap {\n    background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAAAoklEQVR42u3ZIQ6DQBAAwINUICsQCJ6AQFb0QX0Uz+EJCB6AqDhRWYGoa+rhUsjNujV7meyuuGwxPG7hzFGGkwfAv+Pym9zfTx0AyHoHXjHqAAAAAAAAAAAAAAAAAECWf+I56gBA3juw9p0OAGS3A90yfZNrXe/+wFg1IeX9wQgBHP4+0DZJ6xshAAAAAAAAAAAAAAAAgMPeB9q09Y0QwMb4AKePGE54eWq6AAAAAElFTkSuQmCC\") repeat !important;\n    border: 4px solid #000000 !important;\n    box-shadow: 0px 4px 0px 0px #A0380C !important;\n    color: #FCFCFC !important;\n    border-radius: 4px !important;\n}\n\nbutton.secondary {\n    background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAAAoklEQVR42u3ZIQ6DQBAAwINUICsQCJ6AQFb0QX0Uz+EJCB6AqDhRWYGoa+rhUsjNujV7meyuuGwxPG7hzFGGkwfAv+Pym9zfTx0AyHoHXjHqAAAAAAAAAAAAAAAAAECWf+I56gBA3juw9p0OAGS3A90yfZNrXe/+wFg1IeX9wQgBHP4+0DZJ6xshAAAAAAAAAAAAAAAAgMPeB9q09Y0QwMb4AKePGE54eWq6AAAAAElFTkSuQmCC\") repeat !important;\n}\n\ninput[type=\"range\"]::-webkit-slider-thumb {\n    -webkit-appearance: none !important;\n    appearance: none !important;\n    width: 30px !important;\n    height: 30px !important;\n    background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAiUlEQVR42mNkIAIsSGH4z0ACSJjDwEhIDRPDAIEBs5iFmOCNrwkgaNDvBxuQuf8JBfvIC2pGUoMXLUixgnNnEOwbN7Cn9hEY1KQG77Il2A2KisEe1MgAOdhHCxCSUu+Q9PGoxaMWj1pM+2oRuewlBhBTPo/wapGYxp6GBmmG4gre0VQ92oWhCwAAkfwxfq/Sj9YAAAAASUVORK5CYII=\") no-repeat center / contain !important;\n    background-color: transparent !important;\n    border: none !important;\n    border-radius: 0 !important;\n    box-shadow: none !important;\n    cursor: pointer !important;\n    margin-top: -12px !important;\n}\ninput[type=\"range\"]::-moz-range-thumb {\n    appearance: none !important;\n    width: 30px !important;\n    height: 30px !important;\n    background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAiUlEQVR42mNkIAIsSGH4z0ACSJjDwEhIDRPDAIEBs5iFmOCNrwkgaNDvBxuQuf8JBfvIC2pGUoMXLUixgnNnEOwbN7Cn9hEY1KQG77Il2A2KisEe1MgAOdhHCxCSUu+Q9PGoxaMWj1pM+2oRuewlBhBTPo/wapGYxp6GBmmG4gre0VQ92oWhCwAAkfwxfq/Sj9YAAAAASUVORK5CYII=\") no-repeat center / contain !important;\n    background-color: transparent !important;\n    border: none !important;\n    border-radius: 0 !important;\n    box-shadow: none !important;\n    cursor: pointer !important;\n}\n\n.reset-button,\n.icon,\n[class*=\"icon\"] {\n    font-family: \"Segoe UI Emoji\", \"Apple Color Emoji\", \"Noto Color Emoji\", sans-serif !important;\n    font-size: 18px !important;\n}\n\n";

        Set(new ThemeOptions
        {
            BodyBackgroundFill = "#6B8CFF",
            BodyTextColor = "#000000",
            ColorAccent = "#FAC000",
            ColorAccentSoft = "#FCE4A8",
            BackgroundFillPrimary = "#6B8CFF",
            BackgroundFillSecondary = "#E4A672",
            LinkTextColor = "#00A800",
            LinkTextColorHover = "#10C010",
            BodyTextColorSubdued = "#A0380C",
            ShadowDrop = "none",
            ShadowDropLg = "none",
            ShadowInset = "rgba(0,0,0,0.3) 0px 2px 4px 0px inset",
            ShadowSpread = "0px",
            BlockBackgroundFill = "#FAC000",
            BlockBorderColor = "#000000",
            BlockBorderWidth = "4px",
            BlockLabelBackgroundFill = "#C84C0C",
            BlockLabelBorderColor = "#000000",
            BlockLabelBorderWidth = "3px",
            BlockLabelShadow = "0px 3px 0px 0px #9C6838",
            BlockLabelTextColor = "#000000",
            BlockLabelPadding = "*spacing_sm *spacing_md",
            BlockLabelRadius = "4px",
            BlockPadding = "*spacing_lg",
            BlockRadius = "4px",
            BlockShadow = "0px 4px 0px 0px #A0380C",
            BlockTitleBackgroundFill = "#C84C0C",
            BlockTitleBorderColor = "#000000",
            BlockTitleBorderWidth = "3px",
            BlockTitleTextColor = "#000000",
            BlockTitlePadding = "*spacing_sm *spacing_md",
            BlockTitleRadius = "4px",
            PanelBackgroundFill = "#C84C0C",
            PanelBorderColor = "#000000",
            PanelBorderWidth = "4px",
            CheckboxBackgroundColorSelected = "#FAC000",
            CheckboxBorderColor = "#000000",
            CheckboxBorderColorSelected = "#000000",
            CheckboxBorderWidth = "3px",
            CheckboxLabelBackgroundFill = "#FCE4A8",
            CheckboxLabelBackgroundFillHover = "#FCFCFC",
            CheckboxLabelBackgroundFillSelected = "#FAC000",
            CheckboxLabelBorderColor = "#000000",
            CheckboxLabelBorderColorSelected = "#000000",
            CheckboxLabelBorderWidth = "3px",
            CheckboxLabelShadow = "0px 3px 0px 0px #9C6838",
            CheckboxLabelShadowHover = "0px 5px 0px 0px #9C6838",
            CheckboxLabelShadowActive = "0px 1px 0px 0px #9C6838",
            CheckboxShadow = "none",
            CheckboxLabelTextColor = "#000000",
            CheckboxLabelTextColorSelected = "#000000",
            ErrorBackgroundFill = "#FCD0D0",
            ErrorBorderColor = "#dc2626",
            ErrorTextColor = "#991b1b",
            ErrorIconColor = "#dc2626",
            InputBackgroundFill = "#FCE4A8",
            InputBackgroundFillFocus = "#FCFCFC",
            InputBorderColor = "#000000",
            InputBorderColorFocus = "#C84C0C",
            InputBorderWidth = "3px",
            InputPlaceholderColor = "#9C6838",
            InputRadius = "4px",
            InputShadowFocus = "0 0 0 3px #FAC000",
            LoaderColor = "#FAC000",
            SliderColor = "#00A800",
            TableBorderColor = "#000000",
            TableEvenBackgroundFill = "#FCE4A8",
            TableOddBackgroundFill = "#E4A672",
            TableRowFocus = "#FAC000",
            ButtonBorderWidth = "4px",
            ButtonCancelBackgroundFill = "#dc2626",
            ButtonCancelBackgroundFillHover = "#ef4444",
            ButtonCancelBorderColor = "#000000",
            ButtonCancelTextColor = "#000000",
            ButtonCancelShadow = "0px 4px 0px 0px #7f1d1d",
            ButtonCancelShadowHover = "0px 6px 0px 0px #7f1d1d",
            ButtonCancelShadowActive = "0px 1px 0px 0px #7f1d1d",
            ButtonTransformHover = "translateY(-2px)",
            ButtonTransformActive = "translateY(2px)",
            ButtonTransition = "all 0.08s ease",
            ButtonPrimaryBackgroundFill = "#00A800",
            ButtonPrimaryBackgroundFillHover = "#10C010",
            ButtonPrimaryBorderColor = "#000000",
            ButtonPrimaryTextColor = "#000000",
            ButtonPrimaryShadow = "0px 4px 0px 0px #005800",
            ButtonPrimaryShadowHover = "0px 6px 0px 0px #005800",
            ButtonPrimaryShadowActive = "0px 1px 0px 0px #005800",
            ButtonSecondaryBackgroundFill = "#C84C0C",
            ButtonSecondaryBackgroundFillHover = "#D86020",
            ButtonSecondaryBorderColor = "#000000",
            ButtonSecondaryTextColor = "#000000",
            ButtonSecondaryShadow = "0px 4px 0px 0px #9C6838",
            ButtonSecondaryShadowHover = "0px 6px 0px 0px #9C6838",
            ButtonSecondaryShadowActive = "0px 1px 0px 0px #9C6838",
        });
    }
}

// Generated from the pinned Python release. Edit the generator, not this file.
//
// It is not marked auto-generated on purpose: the repository's analyzers run over every
// file, and a marker that switched them off here would be the same bypass the build
// forbids everywhere else.

namespace Gradio.Net;
/// <summary>
/// The theme values an application sets, one property per variable the Python version's
/// <c>Base.set()</c> accepts.
/// </summary>
/// <remarks>
/// <para>
/// A property left <see langword="null"/> means "whatever this theme already has, or the
/// default" — which is what the Python version's <c>x or getattr(self, "x", default)</c> does for a
/// keyword argument it was not given. The properties are declared in the order the Python version
/// assigns them, because that order is the order the variables appear in the stylesheet.
/// </para>
/// <para>
/// The values are strings, not a richer type: the Python version's variables are CSS values, and a
/// theme is free to put anything in them — a colour, a length, a shadow, or a reference to
/// another variable written <c>*name</c>. Interpreting them belongs to the theme, which is
/// where the reference rules live.
/// </para>
/// </remarks>
public sealed class ThemeOptions
{
    /// <summary>The background of the entire app.</summary>
    /// <remarks>Variable <c>body_background_fill</c>.</remarks>
    public string? BodyBackgroundFill { get; init; }

    /// <summary>The background of the entire app in dark mode.</summary>
    /// <remarks>Variable <c>body_background_fill_dark</c>.</remarks>
    public string? BodyBackgroundFillDark { get; init; }

    /// <summary>The default text color.</summary>
    /// <remarks>Variable <c>body_text_color</c>.</remarks>
    public string? BodyTextColor { get; init; }

    /// <summary>The default text color in dark mode.</summary>
    /// <remarks>Variable <c>body_text_color_dark</c>.</remarks>
    public string? BodyTextColorDark { get; init; }

    /// <summary>The default text size.</summary>
    /// <remarks>Variable <c>body_text_size</c>.</remarks>
    public string? BodyTextSize { get; init; }

    /// <summary>The default text weight.</summary>
    /// <remarks>Variable <c>body_text_weight</c>.</remarks>
    public string? BodyTextWeight { get; init; }

    /// <summary>The corner radius used for embedding when the app is embedded within a page.</summary>
    /// <remarks>Variable <c>embed_radius</c>.</remarks>
    public string? EmbedRadius { get; init; }

    /// <summary>The color used for accented items.</summary>
    /// <remarks>Variable <c>color_accent</c>.</remarks>
    public string? ColorAccent { get; init; }

    /// <summary>The softer color used for accented items.</summary>
    /// <remarks>Variable <c>color_accent_soft</c>.</remarks>
    public string? ColorAccentSoft { get; init; }

    /// <summary>The softer color used for accented items in dark mode.</summary>
    /// <remarks>Variable <c>color_accent_soft_dark</c>.</remarks>
    public string? ColorAccentSoftDark { get; init; }

    /// <summary>The background primarily used for items placed directly on the page.</summary>
    /// <remarks>Variable <c>background_fill_primary</c>.</remarks>
    public string? BackgroundFillPrimary { get; init; }

    /// <summary>The background primarily used for items placed directly on the page in dark mode.</summary>
    /// <remarks>Variable <c>background_fill_primary_dark</c>.</remarks>
    public string? BackgroundFillPrimaryDark { get; init; }

    /// <summary>The background primarily used for items placed on top of another item.</summary>
    /// <remarks>Variable <c>background_fill_secondary</c>.</remarks>
    public string? BackgroundFillSecondary { get; init; }

    /// <summary>The background primarily used for items placed on top of another item in dark mode.</summary>
    /// <remarks>Variable <c>background_fill_secondary_dark</c>.</remarks>
    public string? BackgroundFillSecondaryDark { get; init; }

    /// <summary>The border color used for accented items.</summary>
    /// <remarks>Variable <c>border_color_accent</c>.</remarks>
    public string? BorderColorAccent { get; init; }

    /// <summary>The border color used for accented items in dark mode.</summary>
    /// <remarks>Variable <c>border_color_accent_dark</c>.</remarks>
    public string? BorderColorAccentDark { get; init; }

    /// <summary>The border color primarily used for items placed directly on the page.</summary>
    /// <remarks>Variable <c>border_color_primary</c>.</remarks>
    public string? BorderColorPrimary { get; init; }

    /// <summary>The border color primarily used for items placed directly on the page in dark mode.</summary>
    /// <remarks>Variable <c>border_color_primary_dark</c>.</remarks>
    public string? BorderColorPrimaryDark { get; init; }

    /// <summary>The text color used for links.</summary>
    /// <remarks>Variable <c>link_text_color</c>.</remarks>
    public string? LinkTextColor { get; init; }

    /// <summary>The text color used for links when they are active.</summary>
    /// <remarks>Variable <c>link_text_color_active</c>.</remarks>
    public string? LinkTextColorActive { get; init; }

    /// <summary>The text color used for links when they are active in dark mode.</summary>
    /// <remarks>Variable <c>link_text_color_active_dark</c>.</remarks>
    public string? LinkTextColorActiveDark { get; init; }

    /// <summary>The text color used for links in dark mode.</summary>
    /// <remarks>Variable <c>link_text_color_dark</c>.</remarks>
    public string? LinkTextColorDark { get; init; }

    /// <summary>The text color used for links when they are hovered over.</summary>
    /// <remarks>Variable <c>link_text_color_hover</c>.</remarks>
    public string? LinkTextColorHover { get; init; }

    /// <summary>The text color used for links when they are hovered over in dark mode.</summary>
    /// <remarks>Variable <c>link_text_color_hover_dark</c>.</remarks>
    public string? LinkTextColorHoverDark { get; init; }

    /// <summary>The text color used for links when they have been visited.</summary>
    /// <remarks>Variable <c>link_text_color_visited</c>.</remarks>
    public string? LinkTextColorVisited { get; init; }

    /// <summary>The text color used for links when they have been visited in dark mode.</summary>
    /// <remarks>Variable <c>link_text_color_visited_dark</c>.</remarks>
    public string? LinkTextColorVisitedDark { get; init; }

    /// <summary>The text color used for softer, less important text.</summary>
    /// <remarks>Variable <c>body_text_color_subdued</c>.</remarks>
    public string? BodyTextColorSubdued { get; init; }

    /// <summary>The text color used for softer, less important text in dark mode.</summary>
    /// <remarks>Variable <c>body_text_color_subdued_dark</c>.</remarks>
    public string? BodyTextColorSubduedDark { get; init; }

    /// <summary>The body text color in the accordion.</summary>
    /// <remarks>Variable <c>accordion_text_color</c>.</remarks>
    public string? AccordionTextColor { get; init; }

    /// <summary>The body text color in the accordion in dark mode.</summary>
    /// <remarks>Variable <c>accordion_text_color_dark</c>.</remarks>
    public string? AccordionTextColorDark { get; init; }

    /// <summary>The body text color in the table.</summary>
    /// <remarks>Variable <c>table_text_color</c>.</remarks>
    public string? TableTextColor { get; init; }

    /// <summary>The body text color in the table in dark mode.</summary>
    /// <remarks>Variable <c>table_text_color_dark</c>.</remarks>
    public string? TableTextColorDark { get; init; }

    /// <summary>Drop shadow used by other shadowed items.</summary>
    /// <remarks>Variable <c>shadow_drop</c>.</remarks>
    public string? ShadowDrop { get; init; }

    /// <summary>Larger drop shadow used by other shadowed items.</summary>
    /// <remarks>Variable <c>shadow_drop_lg</c>.</remarks>
    public string? ShadowDropLg { get; init; }

    /// <summary>Inset shadow used by other shadowed items.</summary>
    /// <remarks>Variable <c>shadow_inset</c>.</remarks>
    public string? ShadowInset { get; init; }

    /// <summary>Size of shadow spread used by shadowed items.</summary>
    /// <remarks>Variable <c>shadow_spread</c>.</remarks>
    public string? ShadowSpread { get; init; }

    /// <summary>Size of shadow spread used by shadowed items in dark mode.</summary>
    /// <remarks>Variable <c>shadow_spread_dark</c>.</remarks>
    public string? ShadowSpreadDark { get; init; }

    /// <summary>The background around an item.</summary>
    /// <remarks>Variable <c>block_background_fill</c>.</remarks>
    public string? BlockBackgroundFill { get; init; }

    /// <summary>The background around an item in dark mode.</summary>
    /// <remarks>Variable <c>block_background_fill_dark</c>.</remarks>
    public string? BlockBackgroundFillDark { get; init; }

    /// <summary>The border color around an item.</summary>
    /// <remarks>Variable <c>block_border_color</c>.</remarks>
    public string? BlockBorderColor { get; init; }

    /// <summary>The border color around an item in dark mode.</summary>
    /// <remarks>Variable <c>block_border_color_dark</c>.</remarks>
    public string? BlockBorderColorDark { get; init; }

    /// <summary>The border width around an item.</summary>
    /// <remarks>Variable <c>block_border_width</c>.</remarks>
    public string? BlockBorderWidth { get; init; }

    /// <summary>The border width around an item in dark mode.</summary>
    /// <remarks>Variable <c>block_border_width_dark</c>.</remarks>
    public string? BlockBorderWidthDark { get; init; }

    /// <summary>The color of the info text.</summary>
    /// <remarks>Variable <c>block_info_text_color</c>.</remarks>
    public string? BlockInfoTextColor { get; init; }

    /// <summary>The color of the info text in dark mode.</summary>
    /// <remarks>Variable <c>block_info_text_color_dark</c>.</remarks>
    public string? BlockInfoTextColorDark { get; init; }

    /// <summary>The size of the info text.</summary>
    /// <remarks>Variable <c>block_info_text_size</c>.</remarks>
    public string? BlockInfoTextSize { get; init; }

    /// <summary>The weight of the info text.</summary>
    /// <remarks>Variable <c>block_info_text_weight</c>.</remarks>
    public string? BlockInfoTextWeight { get; init; }

    /// <summary>The background of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_background_fill</c>.</remarks>
    public string? BlockLabelBackgroundFill { get; init; }

    /// <summary>The background of the title label of a media element (e.g. image) in dark mode.</summary>
    /// <remarks>Variable <c>block_label_background_fill_dark</c>.</remarks>
    public string? BlockLabelBackgroundFillDark { get; init; }

    /// <summary>The border color of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_border_color</c>.</remarks>
    public string? BlockLabelBorderColor { get; init; }

    /// <summary>The border color of the title label of a media element (e.g. image) in dark mode.</summary>
    /// <remarks>Variable <c>block_label_border_color_dark</c>.</remarks>
    public string? BlockLabelBorderColorDark { get; init; }

    /// <summary>The border width of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_border_width</c>.</remarks>
    public string? BlockLabelBorderWidth { get; init; }

    /// <summary>The border width of the title label of a media element (e.g. image) in dark mode.</summary>
    /// <remarks>Variable <c>block_label_border_width_dark</c>.</remarks>
    public string? BlockLabelBorderWidthDark { get; init; }

    /// <summary>The shadow of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_shadow</c>.</remarks>
    public string? BlockLabelShadow { get; init; }

    /// <summary>The text color of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_text_color</c>.</remarks>
    public string? BlockLabelTextColor { get; init; }

    /// <summary>The text color of the title label of a media element (e.g. image) in dark mode.</summary>
    /// <remarks>Variable <c>block_label_text_color_dark</c>.</remarks>
    public string? BlockLabelTextColorDark { get; init; }

    /// <summary>The margin of the title label of a media element (e.g. image) from its surrounding container.</summary>
    /// <remarks>Variable <c>block_label_margin</c>.</remarks>
    public string? BlockLabelMargin { get; init; }

    /// <summary>The padding of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_padding</c>.</remarks>
    public string? BlockLabelPadding { get; init; }

    /// <summary>The corner radius of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_radius</c>.</remarks>
    public string? BlockLabelRadius { get; init; }

    /// <summary>The corner radius of a right-aligned helper label.</summary>
    /// <remarks>Variable <c>block_label_right_radius</c>.</remarks>
    public string? BlockLabelRightRadius { get; init; }

    /// <summary>The text size of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_text_size</c>.</remarks>
    public string? BlockLabelTextSize { get; init; }

    /// <summary>The text weight of the title label of a media element (e.g. image).</summary>
    /// <remarks>Variable <c>block_label_text_weight</c>.</remarks>
    public string? BlockLabelTextWeight { get; init; }

    /// <summary>The padding around an item.</summary>
    /// <remarks>Variable <c>block_padding</c>.</remarks>
    public string? BlockPadding { get; init; }

    /// <summary>The corner radius around an item.</summary>
    /// <remarks>Variable <c>block_radius</c>.</remarks>
    public string? BlockRadius { get; init; }

    /// <summary>The shadow under an item.</summary>
    /// <remarks>Variable <c>block_shadow</c>.</remarks>
    public string? BlockShadow { get; init; }

    /// <summary>The shadow under an item in dark mode.</summary>
    /// <remarks>Variable <c>block_shadow_dark</c>.</remarks>
    public string? BlockShadowDark { get; init; }

    /// <summary>The background of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_background_fill</c>.</remarks>
    public string? BlockTitleBackgroundFill { get; init; }

    /// <summary>The background of the title of a form element (e.g. textbox) in dark mode.</summary>
    /// <remarks>Variable <c>block_title_background_fill_dark</c>.</remarks>
    public string? BlockTitleBackgroundFillDark { get; init; }

    /// <summary>The border color of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_border_color</c>.</remarks>
    public string? BlockTitleBorderColor { get; init; }

    /// <summary>The border color of the title of a form element (e.g. textbox) in dark mode.</summary>
    /// <remarks>Variable <c>block_title_border_color_dark</c>.</remarks>
    public string? BlockTitleBorderColorDark { get; init; }

    /// <summary>The border width of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_border_width</c>.</remarks>
    public string? BlockTitleBorderWidth { get; init; }

    /// <summary>The border width of the title of a form element (e.g. textbox) in dark mode.</summary>
    /// <remarks>Variable <c>block_title_border_width_dark</c>.</remarks>
    public string? BlockTitleBorderWidthDark { get; init; }

    /// <summary>The text color of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_text_color</c>.</remarks>
    public string? BlockTitleTextColor { get; init; }

    /// <summary>The text color of the title of a form element (e.g. textbox) in dark mode.</summary>
    /// <remarks>Variable <c>block_title_text_color_dark</c>.</remarks>
    public string? BlockTitleTextColorDark { get; init; }

    /// <summary>The padding of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_padding</c>.</remarks>
    public string? BlockTitlePadding { get; init; }

    /// <summary>The corner radius of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_radius</c>.</remarks>
    public string? BlockTitleRadius { get; init; }

    /// <summary>The text size of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_text_size</c>.</remarks>
    public string? BlockTitleTextSize { get; init; }

    /// <summary>The text weight of the title of a form element (e.g. textbox).</summary>
    /// <remarks>Variable <c>block_title_text_weight</c>.</remarks>
    public string? BlockTitleTextWeight { get; init; }

    /// <summary>The corner radius of a layout component that holds other content.</summary>
    /// <remarks>Variable <c>container_radius</c>.</remarks>
    public string? ContainerRadius { get; init; }

    /// <summary>The border gap between form elements, (e.g. consecutive textboxes).</summary>
    /// <remarks>Variable <c>form_gap_width</c>.</remarks>
    public string? FormGapWidth { get; init; }

    /// <summary>The gap between items within a row or column.</summary>
    /// <remarks>Variable <c>layout_gap</c>.</remarks>
    public string? LayoutGap { get; init; }

    /// <summary>The background of a panel.</summary>
    /// <remarks>Variable <c>panel_background_fill</c>.</remarks>
    public string? PanelBackgroundFill { get; init; }

    /// <summary>The background of a panel in dark mode.</summary>
    /// <remarks>Variable <c>panel_background_fill_dark</c>.</remarks>
    public string? PanelBackgroundFillDark { get; init; }

    /// <summary>The border color of a panel.</summary>
    /// <remarks>Variable <c>panel_border_color</c>.</remarks>
    public string? PanelBorderColor { get; init; }

    /// <summary>The border color of a panel in dark mode.</summary>
    /// <remarks>Variable <c>panel_border_color_dark</c>.</remarks>
    public string? PanelBorderColorDark { get; init; }

    /// <summary>The border width of a panel.</summary>
    /// <remarks>Variable <c>panel_border_width</c>.</remarks>
    public string? PanelBorderWidth { get; init; }

    /// <summary>The border width of a panel in dark mode.</summary>
    /// <remarks>Variable <c>panel_border_width_dark</c>.</remarks>
    public string? PanelBorderWidthDark { get; init; }

    /// <summary>The text size of a section header (e.g. tab name).</summary>
    /// <remarks>Variable <c>section_header_text_size</c>.</remarks>
    public string? SectionHeaderTextSize { get; init; }

    /// <summary>The text weight of a section header (e.g. tab name).</summary>
    /// <remarks>Variable <c>section_header_text_weight</c>.</remarks>
    public string? SectionHeaderTextWeight { get; init; }

    /// <summary>The subdued border color for accented items.</summary>
    /// <remarks>Variable <c>border_color_accent_subdued</c>.</remarks>
    public string? BorderColorAccentSubdued { get; init; }

    /// <summary>The subdued border color for accented items in dark mode.</summary>
    /// <remarks>Variable <c>border_color_accent_subdued_dark</c>.</remarks>
    public string? BorderColorAccentSubduedDark { get; init; }

    /// <summary>The background color of code blocks.</summary>
    /// <remarks>Variable <c>code_background_fill</c>.</remarks>
    public string? CodeBackgroundFill { get; init; }

    /// <summary>The background color of code blocks in dark mode.</summary>
    /// <remarks>Variable <c>code_background_fill_dark</c>.</remarks>
    public string? CodeBackgroundFillDark { get; init; }

    /// <summary>The text size of the chatbot text.</summary>
    /// <remarks>Variable <c>chatbot_text_size</c>.</remarks>
    public string? ChatbotTextSize { get; init; }

    /// <summary>The background of a checkbox square or radio circle.</summary>
    /// <remarks>Variable <c>checkbox_background_color</c>.</remarks>
    public string? CheckboxBackgroundColor { get; init; }

    /// <summary>The background of a checkbox square or radio circle in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_background_color_dark</c>.</remarks>
    public string? CheckboxBackgroundColorDark { get; init; }

    /// <summary>The background of a checkbox square or radio circle when focused.</summary>
    /// <remarks>Variable <c>checkbox_background_color_focus</c>.</remarks>
    public string? CheckboxBackgroundColorFocus { get; init; }

    /// <summary>The background of a checkbox square or radio circle when focused in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_background_color_focus_dark</c>.</remarks>
    public string? CheckboxBackgroundColorFocusDark { get; init; }

    /// <summary>The background of a checkbox square or radio circle when hovered over.</summary>
    /// <remarks>Variable <c>checkbox_background_color_hover</c>.</remarks>
    public string? CheckboxBackgroundColorHover { get; init; }

    /// <summary>The background of a checkbox square or radio circle when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_background_color_hover_dark</c>.</remarks>
    public string? CheckboxBackgroundColorHoverDark { get; init; }

    /// <summary>The background of a checkbox square or radio circle when selected.</summary>
    /// <remarks>Variable <c>checkbox_background_color_selected</c>.</remarks>
    public string? CheckboxBackgroundColorSelected { get; init; }

    /// <summary>The background of a checkbox square or radio circle when selected in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_background_color_selected_dark</c>.</remarks>
    public string? CheckboxBackgroundColorSelectedDark { get; init; }

    /// <summary>The border color of a checkbox square or radio circle.</summary>
    /// <remarks>Variable <c>checkbox_border_color</c>.</remarks>
    public string? CheckboxBorderColor { get; init; }

    /// <summary>The border color of a checkbox square or radio circle in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_border_color_dark</c>.</remarks>
    public string? CheckboxBorderColorDark { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when focused.</summary>
    /// <remarks>Variable <c>checkbox_border_color_focus</c>.</remarks>
    public string? CheckboxBorderColorFocus { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when focused in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_border_color_focus_dark</c>.</remarks>
    public string? CheckboxBorderColorFocusDark { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when hovered over.</summary>
    /// <remarks>Variable <c>checkbox_border_color_hover</c>.</remarks>
    public string? CheckboxBorderColorHover { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_border_color_hover_dark</c>.</remarks>
    public string? CheckboxBorderColorHoverDark { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when selected.</summary>
    /// <remarks>Variable <c>checkbox_border_color_selected</c>.</remarks>
    public string? CheckboxBorderColorSelected { get; init; }

    /// <summary>The border color of a checkbox square or radio circle when selected in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_border_color_selected_dark</c>.</remarks>
    public string? CheckboxBorderColorSelectedDark { get; init; }

    /// <summary>The corner radius of a checkbox square.</summary>
    /// <remarks>Variable <c>checkbox_border_radius</c>.</remarks>
    public string? CheckboxBorderRadius { get; init; }

    /// <summary>The border width of a checkbox square or radio circle.</summary>
    /// <remarks>Variable <c>checkbox_border_width</c>.</remarks>
    public string? CheckboxBorderWidth { get; init; }

    /// <summary>The border width of a checkbox square or radio circle in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_border_width_dark</c>.</remarks>
    public string? CheckboxBorderWidthDark { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill</c>.</remarks>
    public string? CheckboxLabelBackgroundFill { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill_dark</c>.</remarks>
    public string? CheckboxLabelBackgroundFillDark { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element when hovered over.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill_hover</c>.</remarks>
    public string? CheckboxLabelBackgroundFillHover { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill_hover_dark</c>.</remarks>
    public string? CheckboxLabelBackgroundFillHoverDark { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element when selected.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill_selected</c>.</remarks>
    public string? CheckboxLabelBackgroundFillSelected { get; init; }

    /// <summary>The background of the surrounding button of a checkbox or radio element when selected in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_background_fill_selected_dark</c>.</remarks>
    public string? CheckboxLabelBackgroundFillSelectedDark { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color</c>.</remarks>
    public string? CheckboxLabelBorderColor { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color_dark</c>.</remarks>
    public string? CheckboxLabelBorderColorDark { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element when hovered over.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color_hover</c>.</remarks>
    public string? CheckboxLabelBorderColorHover { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color_hover_dark</c>.</remarks>
    public string? CheckboxLabelBorderColorHoverDark { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element when selected.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color_selected</c>.</remarks>
    public string? CheckboxLabelBorderColorSelected { get; init; }

    /// <summary>The border color of the surrounding button of a checkbox or radio element when selected in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_border_color_selected_dark</c>.</remarks>
    public string? CheckboxLabelBorderColorSelectedDark { get; init; }

    /// <summary>The border width of the surrounding button of a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_border_width</c>.</remarks>
    public string? CheckboxLabelBorderWidth { get; init; }

    /// <summary>The border width of the surrounding button of a checkbox or radio element in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_border_width_dark</c>.</remarks>
    public string? CheckboxLabelBorderWidthDark { get; init; }

    /// <summary>The gap consecutive checkbox or radio elements.</summary>
    /// <remarks>Variable <c>checkbox_label_gap</c>.</remarks>
    public string? CheckboxLabelGap { get; init; }

    /// <summary>The padding of the surrounding button of a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_padding</c>.</remarks>
    public string? CheckboxLabelPadding { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow</c>.</remarks>
    public string? CheckboxLabelShadow { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow_dark</c>.</remarks>
    public string? CheckboxLabelShadowDark { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element on hover.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow_hover</c>.</remarks>
    public string? CheckboxLabelShadowHover { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element on hover, in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow_hover_dark</c>.</remarks>
    public string? CheckboxLabelShadowHoverDark { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element when active.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow_active</c>.</remarks>
    public string? CheckboxLabelShadowActive { get; init; }

    /// <summary>The shadow of the surrounding button of a checkbox or radio element when active, in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_shadow_active_dark</c>.</remarks>
    public string? CheckboxLabelShadowActiveDark { get; init; }

    /// <summary>The text size of the label accompanying a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_text_size</c>.</remarks>
    public string? CheckboxLabelTextSize { get; init; }

    /// <summary>The text weight of the label accompanying a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_text_weight</c>.</remarks>
    public string? CheckboxLabelTextWeight { get; init; }

    /// <summary>The checkmark visual of a checkbox square.</summary>
    /// <remarks>Variable <c>checkbox_check</c>.</remarks>
    public string? CheckboxCheck { get; init; }

    /// <summary>The circle visual of a radio circle.</summary>
    /// <remarks>Variable <c>radio_circle</c>.</remarks>
    public string? RadioCircle { get; init; }

    /// <summary>The shadow of a checkbox square or radio circle.</summary>
    /// <remarks>Variable <c>checkbox_shadow</c>.</remarks>
    public string? CheckboxShadow { get; init; }

    /// <summary>The text color of the label accompanying a checkbox or radio element.</summary>
    /// <remarks>Variable <c>checkbox_label_text_color</c>.</remarks>
    public string? CheckboxLabelTextColor { get; init; }

    /// <summary>The text color of the label accompanying a checkbox or radio element in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_text_color_dark</c>.</remarks>
    public string? CheckboxLabelTextColorDark { get; init; }

    /// <summary>The text color of the label accompanying a checkbox or radio element when selected.</summary>
    /// <remarks>Variable <c>checkbox_label_text_color_selected</c>.</remarks>
    public string? CheckboxLabelTextColorSelected { get; init; }

    /// <summary>The text color of the label accompanying a checkbox or radio element when selected in dark mode.</summary>
    /// <remarks>Variable <c>checkbox_label_text_color_selected_dark</c>.</remarks>
    public string? CheckboxLabelTextColorSelectedDark { get; init; }

    /// <summary>The background of an error message.</summary>
    /// <remarks>Variable <c>error_background_fill</c>.</remarks>
    public string? ErrorBackgroundFill { get; init; }

    /// <summary>The background of an error message in dark mode.</summary>
    /// <remarks>Variable <c>error_background_fill_dark</c>.</remarks>
    public string? ErrorBackgroundFillDark { get; init; }

    /// <summary>The border color of an error message.</summary>
    /// <remarks>Variable <c>error_border_color</c>.</remarks>
    public string? ErrorBorderColor { get; init; }

    /// <summary>The border color of an error message in dark mode.</summary>
    /// <remarks>Variable <c>error_border_color_dark</c>.</remarks>
    public string? ErrorBorderColorDark { get; init; }

    /// <summary>The border width of an error message.</summary>
    /// <remarks>Variable <c>error_border_width</c>.</remarks>
    public string? ErrorBorderWidth { get; init; }

    /// <summary>The border width of an error message in dark mode.</summary>
    /// <remarks>Variable <c>error_border_width_dark</c>.</remarks>
    public string? ErrorBorderWidthDark { get; init; }

    /// <summary>The text color of an error message.</summary>
    /// <remarks>Variable <c>error_text_color</c>.</remarks>
    public string? ErrorTextColor { get; init; }

    /// <summary>The text color of an error message in dark mode.</summary>
    /// <remarks>Variable <c>error_text_color_dark</c>.</remarks>
    public string? ErrorTextColorDark { get; init; }

    /// <summary>The color of the icon an error message shows.</summary>
    /// <remarks>Variable <c>error_icon_color</c>.</remarks>
    public string? ErrorIconColor { get; init; }

    /// <summary>The color of the icon an error message shows in dark mode.</summary>
    /// <remarks>Variable <c>error_icon_color_dark</c>.</remarks>
    public string? ErrorIconColorDark { get; init; }

    /// <summary>The background of an input field.</summary>
    /// <remarks>Variable <c>input_background_fill</c>.</remarks>
    public string? InputBackgroundFill { get; init; }

    /// <summary>The background of an input field in dark mode.</summary>
    /// <remarks>Variable <c>input_background_fill_dark</c>.</remarks>
    public string? InputBackgroundFillDark { get; init; }

    /// <summary>The background of an input field when focused.</summary>
    /// <remarks>Variable <c>input_background_fill_focus</c>.</remarks>
    public string? InputBackgroundFillFocus { get; init; }

    /// <summary>The background of an input field when focused in dark mode.</summary>
    /// <remarks>Variable <c>input_background_fill_focus_dark</c>.</remarks>
    public string? InputBackgroundFillFocusDark { get; init; }

    /// <summary>The background of an input field when hovered over.</summary>
    /// <remarks>Variable <c>input_background_fill_hover</c>.</remarks>
    public string? InputBackgroundFillHover { get; init; }

    /// <summary>The background of an input field when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>input_background_fill_hover_dark</c>.</remarks>
    public string? InputBackgroundFillHoverDark { get; init; }

    /// <summary>The border color of an input field.</summary>
    /// <remarks>Variable <c>input_border_color</c>.</remarks>
    public string? InputBorderColor { get; init; }

    /// <summary>The border color of an input field in dark mode.</summary>
    /// <remarks>Variable <c>input_border_color_dark</c>.</remarks>
    public string? InputBorderColorDark { get; init; }

    /// <summary>The border color of an input field when focused.</summary>
    /// <remarks>Variable <c>input_border_color_focus</c>.</remarks>
    public string? InputBorderColorFocus { get; init; }

    /// <summary>The border color of an input field when focused in dark mode.</summary>
    /// <remarks>Variable <c>input_border_color_focus_dark</c>.</remarks>
    public string? InputBorderColorFocusDark { get; init; }

    /// <summary>The border color of an input field when hovered over.</summary>
    /// <remarks>Variable <c>input_border_color_hover</c>.</remarks>
    public string? InputBorderColorHover { get; init; }

    /// <summary>The border color of an input field when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>input_border_color_hover_dark</c>.</remarks>
    public string? InputBorderColorHoverDark { get; init; }

    /// <summary>The border width of an input field.</summary>
    /// <remarks>Variable <c>input_border_width</c>.</remarks>
    public string? InputBorderWidth { get; init; }

    /// <summary>The border width of an input field in dark mode.</summary>
    /// <remarks>Variable <c>input_border_width_dark</c>.</remarks>
    public string? InputBorderWidthDark { get; init; }

    /// <summary>The padding of an input field.</summary>
    /// <remarks>Variable <c>input_padding</c>.</remarks>
    public string? InputPadding { get; init; }

    /// <summary>The placeholder text color of an input field.</summary>
    /// <remarks>Variable <c>input_placeholder_color</c>.</remarks>
    public string? InputPlaceholderColor { get; init; }

    /// <summary>The placeholder text color of an input field in dark mode.</summary>
    /// <remarks>Variable <c>input_placeholder_color_dark</c>.</remarks>
    public string? InputPlaceholderColorDark { get; init; }

    /// <summary>The corner radius of an input field.</summary>
    /// <remarks>Variable <c>input_radius</c>.</remarks>
    public string? InputRadius { get; init; }

    /// <summary>The shadow of an input field.</summary>
    /// <remarks>Variable <c>input_shadow</c>.</remarks>
    public string? InputShadow { get; init; }

    /// <summary>The shadow of an input field in dark mode.</summary>
    /// <remarks>Variable <c>input_shadow_dark</c>.</remarks>
    public string? InputShadowDark { get; init; }

    /// <summary>The shadow of an input field when focused.</summary>
    /// <remarks>Variable <c>input_shadow_focus</c>.</remarks>
    public string? InputShadowFocus { get; init; }

    /// <summary>The shadow of an input field when focused in dark mode.</summary>
    /// <remarks>Variable <c>input_shadow_focus_dark</c>.</remarks>
    public string? InputShadowFocusDark { get; init; }

    /// <summary>The text size of an input field.</summary>
    /// <remarks>Variable <c>input_text_size</c>.</remarks>
    public string? InputTextSize { get; init; }

    /// <summary>The text weight of an input field.</summary>
    /// <remarks>Variable <c>input_text_weight</c>.</remarks>
    public string? InputTextWeight { get; init; }

    /// <summary>The color of the loading animation while a request is pending.</summary>
    /// <remarks>Variable <c>loader_color</c>.</remarks>
    public string? LoaderColor { get; init; }

    /// <summary>The color of the loading animation while a request is pending in dark mode.</summary>
    /// <remarks>Variable <c>loader_color_dark</c>.</remarks>
    public string? LoaderColorDark { get; init; }

    /// <summary>The text size used for markdown and other prose.</summary>
    /// <remarks>Variable <c>prose_text_size</c>.</remarks>
    public string? ProseTextSize { get; init; }

    /// <summary>The text weight used for markdown and other prose.</summary>
    /// <remarks>Variable <c>prose_text_weight</c>.</remarks>
    public string? ProseTextWeight { get; init; }

    /// <summary>The text weight of a header used for markdown and other prose.</summary>
    /// <remarks>Variable <c>prose_header_text_weight</c>.</remarks>
    public string? ProseHeaderTextWeight { get; init; }

    /// <summary>The color of the slider in a range element.</summary>
    /// <remarks>Variable <c>slider_color</c>.</remarks>
    public string? SliderColor { get; init; }

    /// <summary>The color of the slider in a range element in dark mode.</summary>
    /// <remarks>Variable <c>slider_color_dark</c>.</remarks>
    public string? SliderColorDark { get; init; }

    /// <summary>The background used for stats visuals (e.g. confidence bars in label).</summary>
    /// <remarks>Variable <c>stat_background_fill</c>.</remarks>
    public string? StatBackgroundFill { get; init; }

    /// <summary>The background used for stats visuals (e.g. confidence bars in label) in dark mode.</summary>
    /// <remarks>Variable <c>stat_background_fill_dark</c>.</remarks>
    public string? StatBackgroundFillDark { get; init; }

    /// <summary>The border color of a table.</summary>
    /// <remarks>Variable <c>table_border_color</c>.</remarks>
    public string? TableBorderColor { get; init; }

    /// <summary>The border color of a table in dark mode.</summary>
    /// <remarks>Variable <c>table_border_color_dark</c>.</remarks>
    public string? TableBorderColorDark { get; init; }

    /// <summary>The background of even rows in a table.</summary>
    /// <remarks>Variable <c>table_even_background_fill</c>.</remarks>
    public string? TableEvenBackgroundFill { get; init; }

    /// <summary>The background of even rows in a table in dark mode.</summary>
    /// <remarks>Variable <c>table_even_background_fill_dark</c>.</remarks>
    public string? TableEvenBackgroundFillDark { get; init; }

    /// <summary>The background of odd rows in a table.</summary>
    /// <remarks>Variable <c>table_odd_background_fill</c>.</remarks>
    public string? TableOddBackgroundFill { get; init; }

    /// <summary>The background of odd rows in a table in dark mode.</summary>
    /// <remarks>Variable <c>table_odd_background_fill_dark</c>.</remarks>
    public string? TableOddBackgroundFillDark { get; init; }

    /// <summary>The corner radius of a table.</summary>
    /// <remarks>Variable <c>table_radius</c>.</remarks>
    public string? TableRadius { get; init; }

    /// <summary>The background of a focused row in a table.</summary>
    /// <remarks>Variable <c>table_row_focus</c>.</remarks>
    public string? TableRowFocus { get; init; }

    /// <summary>The background of a focused row in a table in dark mode.</summary>
    /// <remarks>Variable <c>table_row_focus_dark</c>.</remarks>
    public string? TableRowFocusDark { get; init; }

    /// <summary>The border width of a button.</summary>
    /// <remarks>Variable <c>button_border_width</c>.</remarks>
    public string? ButtonBorderWidth { get; init; }

    /// <summary>The border width of a button in dark mode.</summary>
    /// <remarks>Variable <c>button_border_width_dark</c>.</remarks>
    public string? ButtonBorderWidthDark { get; init; }

    /// <summary>The background of a button of "cancel" variant.</summary>
    /// <remarks>Variable <c>button_cancel_background_fill</c>.</remarks>
    public string? ButtonCancelBackgroundFill { get; init; }

    /// <summary>The background of a button of "cancel" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_background_fill_dark</c>.</remarks>
    public string? ButtonCancelBackgroundFillDark { get; init; }

    /// <summary>The background of a button of "cancel" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_cancel_background_fill_hover</c>.</remarks>
    public string? ButtonCancelBackgroundFillHover { get; init; }

    /// <summary>The background of a button of "cancel" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_background_fill_hover_dark</c>.</remarks>
    public string? ButtonCancelBackgroundFillHoverDark { get; init; }

    /// <summary>The border color of a button of "cancel" variant.</summary>
    /// <remarks>Variable <c>button_cancel_border_color</c>.</remarks>
    public string? ButtonCancelBorderColor { get; init; }

    /// <summary>The border color of a button of "cancel" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_border_color_dark</c>.</remarks>
    public string? ButtonCancelBorderColorDark { get; init; }

    /// <summary>The border color of a button of "cancel" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_cancel_border_color_hover</c>.</remarks>
    public string? ButtonCancelBorderColorHover { get; init; }

    /// <summary>The border color of a button of "cancel" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_border_color_hover_dark</c>.</remarks>
    public string? ButtonCancelBorderColorHoverDark { get; init; }

    /// <summary>The text color of a button of "cancel" variant.</summary>
    /// <remarks>Variable <c>button_cancel_text_color</c>.</remarks>
    public string? ButtonCancelTextColor { get; init; }

    /// <summary>The text color of a button of "cancel" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_text_color_dark</c>.</remarks>
    public string? ButtonCancelTextColorDark { get; init; }

    /// <summary>The text color of a button of "cancel" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_cancel_text_color_hover</c>.</remarks>
    public string? ButtonCancelTextColorHover { get; init; }

    /// <summary>The text color of a button of "cancel" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_text_color_hover_dark</c>.</remarks>
    public string? ButtonCancelTextColorHoverDark { get; init; }

    /// <summary>The shadow under a button of "cancel" variant.</summary>
    /// <remarks>Variable <c>button_cancel_shadow</c>.</remarks>
    public string? ButtonCancelShadow { get; init; }

    /// <summary>The shadow under a button of "cancel" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_cancel_shadow_hover</c>.</remarks>
    public string? ButtonCancelShadowHover { get; init; }

    /// <summary>The shadow under a button of "cancel" variant when pressed.</summary>
    /// <remarks>Variable <c>button_cancel_shadow_active</c>.</remarks>
    public string? ButtonCancelShadowActive { get; init; }

    /// <summary>The shadow under a button of "cancel" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_shadow_dark</c>.</remarks>
    public string? ButtonCancelShadowDark { get; init; }

    /// <summary>The shadow under a button of "cancel" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_shadow_hover_dark</c>.</remarks>
    public string? ButtonCancelShadowHoverDark { get; init; }

    /// <summary>The shadow under a button of "cancel" variant when pressed in dark mode.</summary>
    /// <remarks>Variable <c>button_cancel_shadow_active_dark</c>.</remarks>
    public string? ButtonCancelShadowActiveDark { get; init; }

    /// <summary>The transform animation of a button on hover.</summary>
    /// <remarks>Variable <c>button_transform_hover</c>.</remarks>
    public string? ButtonTransformHover { get; init; }

    /// <summary>The transform animation of a button when pressed.</summary>
    /// <remarks>Variable <c>button_transform_active</c>.</remarks>
    public string? ButtonTransformActive { get; init; }

    /// <summary>The transition animation duration of a button between regular, hover, and focused states.</summary>
    /// <remarks>Variable <c>button_transition</c>.</remarks>
    public string? ButtonTransition { get; init; }

    /// <summary>The padding of a button with the default "large" size.</summary>
    /// <remarks>Variable <c>button_large_padding</c>.</remarks>
    public string? ButtonLargePadding { get; init; }

    /// <summary>The corner radius of a button with the default "large" size.</summary>
    /// <remarks>Variable <c>button_large_radius</c>.</remarks>
    public string? ButtonLargeRadius { get; init; }

    /// <summary>The text size of a button with the default "large" size.</summary>
    /// <remarks>Variable <c>button_large_text_size</c>.</remarks>
    public string? ButtonLargeTextSize { get; init; }

    /// <summary>The text weight of a button with the default "large" size.</summary>
    /// <remarks>Variable <c>button_large_text_weight</c>.</remarks>
    public string? ButtonLargeTextWeight { get; init; }

    /// <summary>The background of a button of "primary" variant.</summary>
    /// <remarks>Variable <c>button_primary_background_fill</c>.</remarks>
    public string? ButtonPrimaryBackgroundFill { get; init; }

    /// <summary>The background of a button of "primary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_background_fill_dark</c>.</remarks>
    public string? ButtonPrimaryBackgroundFillDark { get; init; }

    /// <summary>The background of a button of "primary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_primary_background_fill_hover</c>.</remarks>
    public string? ButtonPrimaryBackgroundFillHover { get; init; }

    /// <summary>The background of a button of "primary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_background_fill_hover_dark</c>.</remarks>
    public string? ButtonPrimaryBackgroundFillHoverDark { get; init; }

    /// <summary>The border color of a button of "primary" variant.</summary>
    /// <remarks>Variable <c>button_primary_border_color</c>.</remarks>
    public string? ButtonPrimaryBorderColor { get; init; }

    /// <summary>The border color of a button of "primary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_border_color_dark</c>.</remarks>
    public string? ButtonPrimaryBorderColorDark { get; init; }

    /// <summary>The border color of a button of "primary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_primary_border_color_hover</c>.</remarks>
    public string? ButtonPrimaryBorderColorHover { get; init; }

    /// <summary>The border color of a button of "primary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_border_color_hover_dark</c>.</remarks>
    public string? ButtonPrimaryBorderColorHoverDark { get; init; }

    /// <summary>The text color of a button of "primary" variant.</summary>
    /// <remarks>Variable <c>button_primary_text_color</c>.</remarks>
    public string? ButtonPrimaryTextColor { get; init; }

    /// <summary>The text color of a button of "primary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_text_color_dark</c>.</remarks>
    public string? ButtonPrimaryTextColorDark { get; init; }

    /// <summary>The text color of a button of "primary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_primary_text_color_hover</c>.</remarks>
    public string? ButtonPrimaryTextColorHover { get; init; }

    /// <summary>The text color of a button of "primary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_text_color_hover_dark</c>.</remarks>
    public string? ButtonPrimaryTextColorHoverDark { get; init; }

    /// <summary>The shadow under a primary button.</summary>
    /// <remarks>Variable <c>button_primary_shadow</c>.</remarks>
    public string? ButtonPrimaryShadow { get; init; }

    /// <summary>The shadow under a primary button when hovered over.</summary>
    /// <remarks>Variable <c>button_primary_shadow_hover</c>.</remarks>
    public string? ButtonPrimaryShadowHover { get; init; }

    /// <summary>The shadow under a primary button when pressed.</summary>
    /// <remarks>Variable <c>button_primary_shadow_active</c>.</remarks>
    public string? ButtonPrimaryShadowActive { get; init; }

    /// <summary>The shadow under a primary button in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_shadow_dark</c>.</remarks>
    public string? ButtonPrimaryShadowDark { get; init; }

    /// <summary>The shadow under a primary button when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_shadow_hover_dark</c>.</remarks>
    public string? ButtonPrimaryShadowHoverDark { get; init; }

    /// <summary>The shadow under a primary button when pressed in dark mode.</summary>
    /// <remarks>Variable <c>button_primary_shadow_active_dark</c>.</remarks>
    public string? ButtonPrimaryShadowActiveDark { get; init; }

    /// <summary>The background of a button of default "secondary" variant.</summary>
    /// <remarks>Variable <c>button_secondary_background_fill</c>.</remarks>
    public string? ButtonSecondaryBackgroundFill { get; init; }

    /// <summary>The background of a button of default "secondary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_background_fill_dark</c>.</remarks>
    public string? ButtonSecondaryBackgroundFillDark { get; init; }

    /// <summary>The background of a button of default "secondary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_secondary_background_fill_hover</c>.</remarks>
    public string? ButtonSecondaryBackgroundFillHover { get; init; }

    /// <summary>The background of a button of default "secondary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_background_fill_hover_dark</c>.</remarks>
    public string? ButtonSecondaryBackgroundFillHoverDark { get; init; }

    /// <summary>The border color of a button of default "secondary" variant.</summary>
    /// <remarks>Variable <c>button_secondary_border_color</c>.</remarks>
    public string? ButtonSecondaryBorderColor { get; init; }

    /// <summary>The border color of a button of default "secondary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_border_color_dark</c>.</remarks>
    public string? ButtonSecondaryBorderColorDark { get; init; }

    /// <summary>The border color of a button of default "secondary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_secondary_border_color_hover</c>.</remarks>
    public string? ButtonSecondaryBorderColorHover { get; init; }

    /// <summary>The border color of a button of default "secondary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_border_color_hover_dark</c>.</remarks>
    public string? ButtonSecondaryBorderColorHoverDark { get; init; }

    /// <summary>The text color of a button of default "secondary" variant.</summary>
    /// <remarks>Variable <c>button_secondary_text_color</c>.</remarks>
    public string? ButtonSecondaryTextColor { get; init; }

    /// <summary>The text color of a button of default "secondary" variant in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_text_color_dark</c>.</remarks>
    public string? ButtonSecondaryTextColorDark { get; init; }

    /// <summary>The text color of a button of default "secondary" variant when hovered over.</summary>
    /// <remarks>Variable <c>button_secondary_text_color_hover</c>.</remarks>
    public string? ButtonSecondaryTextColorHover { get; init; }

    /// <summary>The text color of a button of default "secondary" variant when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_text_color_hover_dark</c>.</remarks>
    public string? ButtonSecondaryTextColorHoverDark { get; init; }

    /// <summary>The shadow under a secondary button.</summary>
    /// <remarks>Variable <c>button_secondary_shadow</c>.</remarks>
    public string? ButtonSecondaryShadow { get; init; }

    /// <summary>The shadow under a secondary button when hovered over.</summary>
    /// <remarks>Variable <c>button_secondary_shadow_hover</c>.</remarks>
    public string? ButtonSecondaryShadowHover { get; init; }

    /// <summary>The shadow under a secondary button when pressed.</summary>
    /// <remarks>Variable <c>button_secondary_shadow_active</c>.</remarks>
    public string? ButtonSecondaryShadowActive { get; init; }

    /// <summary>The shadow under a secondary button in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_shadow_dark</c>.</remarks>
    public string? ButtonSecondaryShadowDark { get; init; }

    /// <summary>The shadow under a secondary button when hovered over in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_shadow_hover_dark</c>.</remarks>
    public string? ButtonSecondaryShadowHoverDark { get; init; }

    /// <summary>The shadow under a secondary button when pressed in dark mode.</summary>
    /// <remarks>Variable <c>button_secondary_shadow_active_dark</c>.</remarks>
    public string? ButtonSecondaryShadowActiveDark { get; init; }

    /// <summary>The padding of a button set to "small" size.</summary>
    /// <remarks>Variable <c>button_small_padding</c>.</remarks>
    public string? ButtonSmallPadding { get; init; }

    /// <summary>The corner radius of a button set to "small" size.</summary>
    /// <remarks>Variable <c>button_small_radius</c>.</remarks>
    public string? ButtonSmallRadius { get; init; }

    /// <summary>The text size of a button set to "small" size.</summary>
    /// <remarks>Variable <c>button_small_text_size</c>.</remarks>
    public string? ButtonSmallTextSize { get; init; }

    /// <summary>The text weight of a button set to "small" size.</summary>
    /// <remarks>Variable <c>button_small_text_weight</c>.</remarks>
    public string? ButtonSmallTextWeight { get; init; }

    /// <summary>The padding of a button set to "medium" size.</summary>
    /// <remarks>Variable <c>button_medium_padding</c>.</remarks>
    public string? ButtonMediumPadding { get; init; }

    /// <summary>The corner radius of a button set to "medium" size.</summary>
    /// <remarks>Variable <c>button_medium_radius</c>.</remarks>
    public string? ButtonMediumRadius { get; init; }

    /// <summary>The text size of a button set to "medium" size.</summary>
    /// <remarks>Variable <c>button_medium_text_size</c>.</remarks>
    public string? ButtonMediumTextSize { get; init; }

    /// <summary>The text weight of a button set to "medium" size.</summary>
    /// <remarks>Variable <c>button_medium_text_weight</c>.</remarks>
    public string? ButtonMediumTextWeight { get; init; }

    /// <summary>The values that were set, by the Python version's variable name, in declaration
    /// order.</summary>
    /// <remarks>
    /// This is how <see cref="Theme.Set(ThemeOptions)"/> reads the object without a dictionary per
    /// theme: the set of names is fixed by the Python version, so the enumeration can be written
    /// out once rather than looked up.
    /// </remarks>
    internal IEnumerable<KeyValuePair<string, string>> Assigned()
    {
        if (BodyBackgroundFill is { Length: > 0 } bodyBackgroundFill)
        {
            yield return new("body_background_fill", bodyBackgroundFill);
        }

        if (BodyBackgroundFillDark is { Length: > 0 } bodyBackgroundFillDark)
        {
            yield return new("body_background_fill_dark", bodyBackgroundFillDark);
        }

        if (BodyTextColor is { Length: > 0 } bodyTextColor)
        {
            yield return new("body_text_color", bodyTextColor);
        }

        if (BodyTextColorDark is { Length: > 0 } bodyTextColorDark)
        {
            yield return new("body_text_color_dark", bodyTextColorDark);
        }

        if (BodyTextSize is { Length: > 0 } bodyTextSize)
        {
            yield return new("body_text_size", bodyTextSize);
        }

        if (BodyTextWeight is { Length: > 0 } bodyTextWeight)
        {
            yield return new("body_text_weight", bodyTextWeight);
        }

        if (EmbedRadius is { Length: > 0 } embedRadius)
        {
            yield return new("embed_radius", embedRadius);
        }

        if (ColorAccent is { Length: > 0 } colorAccent)
        {
            yield return new("color_accent", colorAccent);
        }

        if (ColorAccentSoft is { Length: > 0 } colorAccentSoft)
        {
            yield return new("color_accent_soft", colorAccentSoft);
        }

        if (ColorAccentSoftDark is { Length: > 0 } colorAccentSoftDark)
        {
            yield return new("color_accent_soft_dark", colorAccentSoftDark);
        }

        if (BackgroundFillPrimary is { Length: > 0 } backgroundFillPrimary)
        {
            yield return new("background_fill_primary", backgroundFillPrimary);
        }

        if (BackgroundFillPrimaryDark is { Length: > 0 } backgroundFillPrimaryDark)
        {
            yield return new("background_fill_primary_dark", backgroundFillPrimaryDark);
        }

        if (BackgroundFillSecondary is { Length: > 0 } backgroundFillSecondary)
        {
            yield return new("background_fill_secondary", backgroundFillSecondary);
        }

        if (BackgroundFillSecondaryDark is { Length: > 0 } backgroundFillSecondaryDark)
        {
            yield return new("background_fill_secondary_dark", backgroundFillSecondaryDark);
        }

        if (BorderColorAccent is { Length: > 0 } borderColorAccent)
        {
            yield return new("border_color_accent", borderColorAccent);
        }

        if (BorderColorAccentDark is { Length: > 0 } borderColorAccentDark)
        {
            yield return new("border_color_accent_dark", borderColorAccentDark);
        }

        if (BorderColorPrimary is { Length: > 0 } borderColorPrimary)
        {
            yield return new("border_color_primary", borderColorPrimary);
        }

        if (BorderColorPrimaryDark is { Length: > 0 } borderColorPrimaryDark)
        {
            yield return new("border_color_primary_dark", borderColorPrimaryDark);
        }

        if (LinkTextColor is { Length: > 0 } linkTextColor)
        {
            yield return new("link_text_color", linkTextColor);
        }

        if (LinkTextColorActive is { Length: > 0 } linkTextColorActive)
        {
            yield return new("link_text_color_active", linkTextColorActive);
        }

        if (LinkTextColorActiveDark is { Length: > 0 } linkTextColorActiveDark)
        {
            yield return new("link_text_color_active_dark", linkTextColorActiveDark);
        }

        if (LinkTextColorDark is { Length: > 0 } linkTextColorDark)
        {
            yield return new("link_text_color_dark", linkTextColorDark);
        }

        if (LinkTextColorHover is { Length: > 0 } linkTextColorHover)
        {
            yield return new("link_text_color_hover", linkTextColorHover);
        }

        if (LinkTextColorHoverDark is { Length: > 0 } linkTextColorHoverDark)
        {
            yield return new("link_text_color_hover_dark", linkTextColorHoverDark);
        }

        if (LinkTextColorVisited is { Length: > 0 } linkTextColorVisited)
        {
            yield return new("link_text_color_visited", linkTextColorVisited);
        }

        if (LinkTextColorVisitedDark is { Length: > 0 } linkTextColorVisitedDark)
        {
            yield return new("link_text_color_visited_dark", linkTextColorVisitedDark);
        }

        if (BodyTextColorSubdued is { Length: > 0 } bodyTextColorSubdued)
        {
            yield return new("body_text_color_subdued", bodyTextColorSubdued);
        }

        if (BodyTextColorSubduedDark is { Length: > 0 } bodyTextColorSubduedDark)
        {
            yield return new("body_text_color_subdued_dark", bodyTextColorSubduedDark);
        }

        if (AccordionTextColor is { Length: > 0 } accordionTextColor)
        {
            yield return new("accordion_text_color", accordionTextColor);
        }

        if (AccordionTextColorDark is { Length: > 0 } accordionTextColorDark)
        {
            yield return new("accordion_text_color_dark", accordionTextColorDark);
        }

        if (TableTextColor is { Length: > 0 } tableTextColor)
        {
            yield return new("table_text_color", tableTextColor);
        }

        if (TableTextColorDark is { Length: > 0 } tableTextColorDark)
        {
            yield return new("table_text_color_dark", tableTextColorDark);
        }

        if (ShadowDrop is { Length: > 0 } shadowDrop)
        {
            yield return new("shadow_drop", shadowDrop);
        }

        if (ShadowDropLg is { Length: > 0 } shadowDropLg)
        {
            yield return new("shadow_drop_lg", shadowDropLg);
        }

        if (ShadowInset is { Length: > 0 } shadowInset)
        {
            yield return new("shadow_inset", shadowInset);
        }

        if (ShadowSpread is { Length: > 0 } shadowSpread)
        {
            yield return new("shadow_spread", shadowSpread);
        }

        if (ShadowSpreadDark is { Length: > 0 } shadowSpreadDark)
        {
            yield return new("shadow_spread_dark", shadowSpreadDark);
        }

        if (BlockBackgroundFill is { Length: > 0 } blockBackgroundFill)
        {
            yield return new("block_background_fill", blockBackgroundFill);
        }

        if (BlockBackgroundFillDark is { Length: > 0 } blockBackgroundFillDark)
        {
            yield return new("block_background_fill_dark", blockBackgroundFillDark);
        }

        if (BlockBorderColor is { Length: > 0 } blockBorderColor)
        {
            yield return new("block_border_color", blockBorderColor);
        }

        if (BlockBorderColorDark is { Length: > 0 } blockBorderColorDark)
        {
            yield return new("block_border_color_dark", blockBorderColorDark);
        }

        if (BlockBorderWidth is { Length: > 0 } blockBorderWidth)
        {
            yield return new("block_border_width", blockBorderWidth);
        }

        if (BlockBorderWidthDark is { Length: > 0 } blockBorderWidthDark)
        {
            yield return new("block_border_width_dark", blockBorderWidthDark);
        }

        if (BlockInfoTextColor is { Length: > 0 } blockInfoTextColor)
        {
            yield return new("block_info_text_color", blockInfoTextColor);
        }

        if (BlockInfoTextColorDark is { Length: > 0 } blockInfoTextColorDark)
        {
            yield return new("block_info_text_color_dark", blockInfoTextColorDark);
        }

        if (BlockInfoTextSize is { Length: > 0 } blockInfoTextSize)
        {
            yield return new("block_info_text_size", blockInfoTextSize);
        }

        if (BlockInfoTextWeight is { Length: > 0 } blockInfoTextWeight)
        {
            yield return new("block_info_text_weight", blockInfoTextWeight);
        }

        if (BlockLabelBackgroundFill is { Length: > 0 } blockLabelBackgroundFill)
        {
            yield return new("block_label_background_fill", blockLabelBackgroundFill);
        }

        if (BlockLabelBackgroundFillDark is { Length: > 0 } blockLabelBackgroundFillDark)
        {
            yield return new("block_label_background_fill_dark", blockLabelBackgroundFillDark);
        }

        if (BlockLabelBorderColor is { Length: > 0 } blockLabelBorderColor)
        {
            yield return new("block_label_border_color", blockLabelBorderColor);
        }

        if (BlockLabelBorderColorDark is { Length: > 0 } blockLabelBorderColorDark)
        {
            yield return new("block_label_border_color_dark", blockLabelBorderColorDark);
        }

        if (BlockLabelBorderWidth is { Length: > 0 } blockLabelBorderWidth)
        {
            yield return new("block_label_border_width", blockLabelBorderWidth);
        }

        if (BlockLabelBorderWidthDark is { Length: > 0 } blockLabelBorderWidthDark)
        {
            yield return new("block_label_border_width_dark", blockLabelBorderWidthDark);
        }

        if (BlockLabelShadow is { Length: > 0 } blockLabelShadow)
        {
            yield return new("block_label_shadow", blockLabelShadow);
        }

        if (BlockLabelTextColor is { Length: > 0 } blockLabelTextColor)
        {
            yield return new("block_label_text_color", blockLabelTextColor);
        }

        if (BlockLabelTextColorDark is { Length: > 0 } blockLabelTextColorDark)
        {
            yield return new("block_label_text_color_dark", blockLabelTextColorDark);
        }

        if (BlockLabelMargin is { Length: > 0 } blockLabelMargin)
        {
            yield return new("block_label_margin", blockLabelMargin);
        }

        if (BlockLabelPadding is { Length: > 0 } blockLabelPadding)
        {
            yield return new("block_label_padding", blockLabelPadding);
        }

        if (BlockLabelRadius is { Length: > 0 } blockLabelRadius)
        {
            yield return new("block_label_radius", blockLabelRadius);
        }

        if (BlockLabelRightRadius is { Length: > 0 } blockLabelRightRadius)
        {
            yield return new("block_label_right_radius", blockLabelRightRadius);
        }

        if (BlockLabelTextSize is { Length: > 0 } blockLabelTextSize)
        {
            yield return new("block_label_text_size", blockLabelTextSize);
        }

        if (BlockLabelTextWeight is { Length: > 0 } blockLabelTextWeight)
        {
            yield return new("block_label_text_weight", blockLabelTextWeight);
        }

        if (BlockPadding is { Length: > 0 } blockPadding)
        {
            yield return new("block_padding", blockPadding);
        }

        if (BlockRadius is { Length: > 0 } blockRadius)
        {
            yield return new("block_radius", blockRadius);
        }

        if (BlockShadow is { Length: > 0 } blockShadow)
        {
            yield return new("block_shadow", blockShadow);
        }

        if (BlockShadowDark is { Length: > 0 } blockShadowDark)
        {
            yield return new("block_shadow_dark", blockShadowDark);
        }

        if (BlockTitleBackgroundFill is { Length: > 0 } blockTitleBackgroundFill)
        {
            yield return new("block_title_background_fill", blockTitleBackgroundFill);
        }

        if (BlockTitleBackgroundFillDark is { Length: > 0 } blockTitleBackgroundFillDark)
        {
            yield return new("block_title_background_fill_dark", blockTitleBackgroundFillDark);
        }

        if (BlockTitleBorderColor is { Length: > 0 } blockTitleBorderColor)
        {
            yield return new("block_title_border_color", blockTitleBorderColor);
        }

        if (BlockTitleBorderColorDark is { Length: > 0 } blockTitleBorderColorDark)
        {
            yield return new("block_title_border_color_dark", blockTitleBorderColorDark);
        }

        if (BlockTitleBorderWidth is { Length: > 0 } blockTitleBorderWidth)
        {
            yield return new("block_title_border_width", blockTitleBorderWidth);
        }

        if (BlockTitleBorderWidthDark is { Length: > 0 } blockTitleBorderWidthDark)
        {
            yield return new("block_title_border_width_dark", blockTitleBorderWidthDark);
        }

        if (BlockTitleTextColor is { Length: > 0 } blockTitleTextColor)
        {
            yield return new("block_title_text_color", blockTitleTextColor);
        }

        if (BlockTitleTextColorDark is { Length: > 0 } blockTitleTextColorDark)
        {
            yield return new("block_title_text_color_dark", blockTitleTextColorDark);
        }

        if (BlockTitlePadding is { Length: > 0 } blockTitlePadding)
        {
            yield return new("block_title_padding", blockTitlePadding);
        }

        if (BlockTitleRadius is { Length: > 0 } blockTitleRadius)
        {
            yield return new("block_title_radius", blockTitleRadius);
        }

        if (BlockTitleTextSize is { Length: > 0 } blockTitleTextSize)
        {
            yield return new("block_title_text_size", blockTitleTextSize);
        }

        if (BlockTitleTextWeight is { Length: > 0 } blockTitleTextWeight)
        {
            yield return new("block_title_text_weight", blockTitleTextWeight);
        }

        if (ContainerRadius is { Length: > 0 } containerRadius)
        {
            yield return new("container_radius", containerRadius);
        }

        if (FormGapWidth is { Length: > 0 } formGapWidth)
        {
            yield return new("form_gap_width", formGapWidth);
        }

        if (LayoutGap is { Length: > 0 } layoutGap)
        {
            yield return new("layout_gap", layoutGap);
        }

        if (PanelBackgroundFill is { Length: > 0 } panelBackgroundFill)
        {
            yield return new("panel_background_fill", panelBackgroundFill);
        }

        if (PanelBackgroundFillDark is { Length: > 0 } panelBackgroundFillDark)
        {
            yield return new("panel_background_fill_dark", panelBackgroundFillDark);
        }

        if (PanelBorderColor is { Length: > 0 } panelBorderColor)
        {
            yield return new("panel_border_color", panelBorderColor);
        }

        if (PanelBorderColorDark is { Length: > 0 } panelBorderColorDark)
        {
            yield return new("panel_border_color_dark", panelBorderColorDark);
        }

        if (PanelBorderWidth is { Length: > 0 } panelBorderWidth)
        {
            yield return new("panel_border_width", panelBorderWidth);
        }

        if (PanelBorderWidthDark is { Length: > 0 } panelBorderWidthDark)
        {
            yield return new("panel_border_width_dark", panelBorderWidthDark);
        }

        if (SectionHeaderTextSize is { Length: > 0 } sectionHeaderTextSize)
        {
            yield return new("section_header_text_size", sectionHeaderTextSize);
        }

        if (SectionHeaderTextWeight is { Length: > 0 } sectionHeaderTextWeight)
        {
            yield return new("section_header_text_weight", sectionHeaderTextWeight);
        }

        if (BorderColorAccentSubdued is { Length: > 0 } borderColorAccentSubdued)
        {
            yield return new("border_color_accent_subdued", borderColorAccentSubdued);
        }

        if (BorderColorAccentSubduedDark is { Length: > 0 } borderColorAccentSubduedDark)
        {
            yield return new("border_color_accent_subdued_dark", borderColorAccentSubduedDark);
        }

        if (CodeBackgroundFill is { Length: > 0 } codeBackgroundFill)
        {
            yield return new("code_background_fill", codeBackgroundFill);
        }

        if (CodeBackgroundFillDark is { Length: > 0 } codeBackgroundFillDark)
        {
            yield return new("code_background_fill_dark", codeBackgroundFillDark);
        }

        if (ChatbotTextSize is { Length: > 0 } chatbotTextSize)
        {
            yield return new("chatbot_text_size", chatbotTextSize);
        }

        if (CheckboxBackgroundColor is { Length: > 0 } checkboxBackgroundColor)
        {
            yield return new("checkbox_background_color", checkboxBackgroundColor);
        }

        if (CheckboxBackgroundColorDark is { Length: > 0 } checkboxBackgroundColorDark)
        {
            yield return new("checkbox_background_color_dark", checkboxBackgroundColorDark);
        }

        if (CheckboxBackgroundColorFocus is { Length: > 0 } checkboxBackgroundColorFocus)
        {
            yield return new("checkbox_background_color_focus", checkboxBackgroundColorFocus);
        }

        if (CheckboxBackgroundColorFocusDark is { Length: > 0 } checkboxBackgroundColorFocusDark)
        {
            yield return new("checkbox_background_color_focus_dark", checkboxBackgroundColorFocusDark);
        }

        if (CheckboxBackgroundColorHover is { Length: > 0 } checkboxBackgroundColorHover)
        {
            yield return new("checkbox_background_color_hover", checkboxBackgroundColorHover);
        }

        if (CheckboxBackgroundColorHoverDark is { Length: > 0 } checkboxBackgroundColorHoverDark)
        {
            yield return new("checkbox_background_color_hover_dark", checkboxBackgroundColorHoverDark);
        }

        if (CheckboxBackgroundColorSelected is { Length: > 0 } checkboxBackgroundColorSelected)
        {
            yield return new("checkbox_background_color_selected", checkboxBackgroundColorSelected);
        }

        if (CheckboxBackgroundColorSelectedDark is { Length: > 0 } checkboxBackgroundColorSelectedDark)
        {
            yield return new("checkbox_background_color_selected_dark", checkboxBackgroundColorSelectedDark);
        }

        if (CheckboxBorderColor is { Length: > 0 } checkboxBorderColor)
        {
            yield return new("checkbox_border_color", checkboxBorderColor);
        }

        if (CheckboxBorderColorDark is { Length: > 0 } checkboxBorderColorDark)
        {
            yield return new("checkbox_border_color_dark", checkboxBorderColorDark);
        }

        if (CheckboxBorderColorFocus is { Length: > 0 } checkboxBorderColorFocus)
        {
            yield return new("checkbox_border_color_focus", checkboxBorderColorFocus);
        }

        if (CheckboxBorderColorFocusDark is { Length: > 0 } checkboxBorderColorFocusDark)
        {
            yield return new("checkbox_border_color_focus_dark", checkboxBorderColorFocusDark);
        }

        if (CheckboxBorderColorHover is { Length: > 0 } checkboxBorderColorHover)
        {
            yield return new("checkbox_border_color_hover", checkboxBorderColorHover);
        }

        if (CheckboxBorderColorHoverDark is { Length: > 0 } checkboxBorderColorHoverDark)
        {
            yield return new("checkbox_border_color_hover_dark", checkboxBorderColorHoverDark);
        }

        if (CheckboxBorderColorSelected is { Length: > 0 } checkboxBorderColorSelected)
        {
            yield return new("checkbox_border_color_selected", checkboxBorderColorSelected);
        }

        if (CheckboxBorderColorSelectedDark is { Length: > 0 } checkboxBorderColorSelectedDark)
        {
            yield return new("checkbox_border_color_selected_dark", checkboxBorderColorSelectedDark);
        }

        if (CheckboxBorderRadius is { Length: > 0 } checkboxBorderRadius)
        {
            yield return new("checkbox_border_radius", checkboxBorderRadius);
        }

        if (CheckboxBorderWidth is { Length: > 0 } checkboxBorderWidth)
        {
            yield return new("checkbox_border_width", checkboxBorderWidth);
        }

        if (CheckboxBorderWidthDark is { Length: > 0 } checkboxBorderWidthDark)
        {
            yield return new("checkbox_border_width_dark", checkboxBorderWidthDark);
        }

        if (CheckboxLabelBackgroundFill is { Length: > 0 } checkboxLabelBackgroundFill)
        {
            yield return new("checkbox_label_background_fill", checkboxLabelBackgroundFill);
        }

        if (CheckboxLabelBackgroundFillDark is { Length: > 0 } checkboxLabelBackgroundFillDark)
        {
            yield return new("checkbox_label_background_fill_dark", checkboxLabelBackgroundFillDark);
        }

        if (CheckboxLabelBackgroundFillHover is { Length: > 0 } checkboxLabelBackgroundFillHover)
        {
            yield return new("checkbox_label_background_fill_hover", checkboxLabelBackgroundFillHover);
        }

        if (CheckboxLabelBackgroundFillHoverDark is { Length: > 0 } checkboxLabelBackgroundFillHoverDark)
        {
            yield return new("checkbox_label_background_fill_hover_dark", checkboxLabelBackgroundFillHoverDark);
        }

        if (CheckboxLabelBackgroundFillSelected is { Length: > 0 } checkboxLabelBackgroundFillSelected)
        {
            yield return new("checkbox_label_background_fill_selected", checkboxLabelBackgroundFillSelected);
        }

        if (CheckboxLabelBackgroundFillSelectedDark is { Length: > 0 } checkboxLabelBackgroundFillSelectedDark)
        {
            yield return new("checkbox_label_background_fill_selected_dark", checkboxLabelBackgroundFillSelectedDark);
        }

        if (CheckboxLabelBorderColor is { Length: > 0 } checkboxLabelBorderColor)
        {
            yield return new("checkbox_label_border_color", checkboxLabelBorderColor);
        }

        if (CheckboxLabelBorderColorDark is { Length: > 0 } checkboxLabelBorderColorDark)
        {
            yield return new("checkbox_label_border_color_dark", checkboxLabelBorderColorDark);
        }

        if (CheckboxLabelBorderColorHover is { Length: > 0 } checkboxLabelBorderColorHover)
        {
            yield return new("checkbox_label_border_color_hover", checkboxLabelBorderColorHover);
        }

        if (CheckboxLabelBorderColorHoverDark is { Length: > 0 } checkboxLabelBorderColorHoverDark)
        {
            yield return new("checkbox_label_border_color_hover_dark", checkboxLabelBorderColorHoverDark);
        }

        if (CheckboxLabelBorderColorSelected is { Length: > 0 } checkboxLabelBorderColorSelected)
        {
            yield return new("checkbox_label_border_color_selected", checkboxLabelBorderColorSelected);
        }

        if (CheckboxLabelBorderColorSelectedDark is { Length: > 0 } checkboxLabelBorderColorSelectedDark)
        {
            yield return new("checkbox_label_border_color_selected_dark", checkboxLabelBorderColorSelectedDark);
        }

        if (CheckboxLabelBorderWidth is { Length: > 0 } checkboxLabelBorderWidth)
        {
            yield return new("checkbox_label_border_width", checkboxLabelBorderWidth);
        }

        if (CheckboxLabelBorderWidthDark is { Length: > 0 } checkboxLabelBorderWidthDark)
        {
            yield return new("checkbox_label_border_width_dark", checkboxLabelBorderWidthDark);
        }

        if (CheckboxLabelGap is { Length: > 0 } checkboxLabelGap)
        {
            yield return new("checkbox_label_gap", checkboxLabelGap);
        }

        if (CheckboxLabelPadding is { Length: > 0 } checkboxLabelPadding)
        {
            yield return new("checkbox_label_padding", checkboxLabelPadding);
        }

        if (CheckboxLabelShadow is { Length: > 0 } checkboxLabelShadow)
        {
            yield return new("checkbox_label_shadow", checkboxLabelShadow);
        }

        if (CheckboxLabelShadowDark is { Length: > 0 } checkboxLabelShadowDark)
        {
            yield return new("checkbox_label_shadow_dark", checkboxLabelShadowDark);
        }

        if (CheckboxLabelShadowHover is { Length: > 0 } checkboxLabelShadowHover)
        {
            yield return new("checkbox_label_shadow_hover", checkboxLabelShadowHover);
        }

        if (CheckboxLabelShadowHoverDark is { Length: > 0 } checkboxLabelShadowHoverDark)
        {
            yield return new("checkbox_label_shadow_hover_dark", checkboxLabelShadowHoverDark);
        }

        if (CheckboxLabelShadowActive is { Length: > 0 } checkboxLabelShadowActive)
        {
            yield return new("checkbox_label_shadow_active", checkboxLabelShadowActive);
        }

        if (CheckboxLabelShadowActiveDark is { Length: > 0 } checkboxLabelShadowActiveDark)
        {
            yield return new("checkbox_label_shadow_active_dark", checkboxLabelShadowActiveDark);
        }

        if (CheckboxLabelTextSize is { Length: > 0 } checkboxLabelTextSize)
        {
            yield return new("checkbox_label_text_size", checkboxLabelTextSize);
        }

        if (CheckboxLabelTextWeight is { Length: > 0 } checkboxLabelTextWeight)
        {
            yield return new("checkbox_label_text_weight", checkboxLabelTextWeight);
        }

        if (CheckboxCheck is { Length: > 0 } checkboxCheck)
        {
            yield return new("checkbox_check", checkboxCheck);
        }

        if (RadioCircle is { Length: > 0 } radioCircle)
        {
            yield return new("radio_circle", radioCircle);
        }

        if (CheckboxShadow is { Length: > 0 } checkboxShadow)
        {
            yield return new("checkbox_shadow", checkboxShadow);
        }

        if (CheckboxLabelTextColor is { Length: > 0 } checkboxLabelTextColor)
        {
            yield return new("checkbox_label_text_color", checkboxLabelTextColor);
        }

        if (CheckboxLabelTextColorDark is { Length: > 0 } checkboxLabelTextColorDark)
        {
            yield return new("checkbox_label_text_color_dark", checkboxLabelTextColorDark);
        }

        if (CheckboxLabelTextColorSelected is { Length: > 0 } checkboxLabelTextColorSelected)
        {
            yield return new("checkbox_label_text_color_selected", checkboxLabelTextColorSelected);
        }

        if (CheckboxLabelTextColorSelectedDark is { Length: > 0 } checkboxLabelTextColorSelectedDark)
        {
            yield return new("checkbox_label_text_color_selected_dark", checkboxLabelTextColorSelectedDark);
        }

        if (ErrorBackgroundFill is { Length: > 0 } errorBackgroundFill)
        {
            yield return new("error_background_fill", errorBackgroundFill);
        }

        if (ErrorBackgroundFillDark is { Length: > 0 } errorBackgroundFillDark)
        {
            yield return new("error_background_fill_dark", errorBackgroundFillDark);
        }

        if (ErrorBorderColor is { Length: > 0 } errorBorderColor)
        {
            yield return new("error_border_color", errorBorderColor);
        }

        if (ErrorBorderColorDark is { Length: > 0 } errorBorderColorDark)
        {
            yield return new("error_border_color_dark", errorBorderColorDark);
        }

        if (ErrorBorderWidth is { Length: > 0 } errorBorderWidth)
        {
            yield return new("error_border_width", errorBorderWidth);
        }

        if (ErrorBorderWidthDark is { Length: > 0 } errorBorderWidthDark)
        {
            yield return new("error_border_width_dark", errorBorderWidthDark);
        }

        if (ErrorTextColor is { Length: > 0 } errorTextColor)
        {
            yield return new("error_text_color", errorTextColor);
        }

        if (ErrorTextColorDark is { Length: > 0 } errorTextColorDark)
        {
            yield return new("error_text_color_dark", errorTextColorDark);
        }

        if (ErrorIconColor is { Length: > 0 } errorIconColor)
        {
            yield return new("error_icon_color", errorIconColor);
        }

        if (ErrorIconColorDark is { Length: > 0 } errorIconColorDark)
        {
            yield return new("error_icon_color_dark", errorIconColorDark);
        }

        if (InputBackgroundFill is { Length: > 0 } inputBackgroundFill)
        {
            yield return new("input_background_fill", inputBackgroundFill);
        }

        if (InputBackgroundFillDark is { Length: > 0 } inputBackgroundFillDark)
        {
            yield return new("input_background_fill_dark", inputBackgroundFillDark);
        }

        if (InputBackgroundFillFocus is { Length: > 0 } inputBackgroundFillFocus)
        {
            yield return new("input_background_fill_focus", inputBackgroundFillFocus);
        }

        if (InputBackgroundFillFocusDark is { Length: > 0 } inputBackgroundFillFocusDark)
        {
            yield return new("input_background_fill_focus_dark", inputBackgroundFillFocusDark);
        }

        if (InputBackgroundFillHover is { Length: > 0 } inputBackgroundFillHover)
        {
            yield return new("input_background_fill_hover", inputBackgroundFillHover);
        }

        if (InputBackgroundFillHoverDark is { Length: > 0 } inputBackgroundFillHoverDark)
        {
            yield return new("input_background_fill_hover_dark", inputBackgroundFillHoverDark);
        }

        if (InputBorderColor is { Length: > 0 } inputBorderColor)
        {
            yield return new("input_border_color", inputBorderColor);
        }

        if (InputBorderColorDark is { Length: > 0 } inputBorderColorDark)
        {
            yield return new("input_border_color_dark", inputBorderColorDark);
        }

        if (InputBorderColorFocus is { Length: > 0 } inputBorderColorFocus)
        {
            yield return new("input_border_color_focus", inputBorderColorFocus);
        }

        if (InputBorderColorFocusDark is { Length: > 0 } inputBorderColorFocusDark)
        {
            yield return new("input_border_color_focus_dark", inputBorderColorFocusDark);
        }

        if (InputBorderColorHover is { Length: > 0 } inputBorderColorHover)
        {
            yield return new("input_border_color_hover", inputBorderColorHover);
        }

        if (InputBorderColorHoverDark is { Length: > 0 } inputBorderColorHoverDark)
        {
            yield return new("input_border_color_hover_dark", inputBorderColorHoverDark);
        }

        if (InputBorderWidth is { Length: > 0 } inputBorderWidth)
        {
            yield return new("input_border_width", inputBorderWidth);
        }

        if (InputBorderWidthDark is { Length: > 0 } inputBorderWidthDark)
        {
            yield return new("input_border_width_dark", inputBorderWidthDark);
        }

        if (InputPadding is { Length: > 0 } inputPadding)
        {
            yield return new("input_padding", inputPadding);
        }

        if (InputPlaceholderColor is { Length: > 0 } inputPlaceholderColor)
        {
            yield return new("input_placeholder_color", inputPlaceholderColor);
        }

        if (InputPlaceholderColorDark is { Length: > 0 } inputPlaceholderColorDark)
        {
            yield return new("input_placeholder_color_dark", inputPlaceholderColorDark);
        }

        if (InputRadius is { Length: > 0 } inputRadius)
        {
            yield return new("input_radius", inputRadius);
        }

        if (InputShadow is { Length: > 0 } inputShadow)
        {
            yield return new("input_shadow", inputShadow);
        }

        if (InputShadowDark is { Length: > 0 } inputShadowDark)
        {
            yield return new("input_shadow_dark", inputShadowDark);
        }

        if (InputShadowFocus is { Length: > 0 } inputShadowFocus)
        {
            yield return new("input_shadow_focus", inputShadowFocus);
        }

        if (InputShadowFocusDark is { Length: > 0 } inputShadowFocusDark)
        {
            yield return new("input_shadow_focus_dark", inputShadowFocusDark);
        }

        if (InputTextSize is { Length: > 0 } inputTextSize)
        {
            yield return new("input_text_size", inputTextSize);
        }

        if (InputTextWeight is { Length: > 0 } inputTextWeight)
        {
            yield return new("input_text_weight", inputTextWeight);
        }

        if (LoaderColor is { Length: > 0 } loaderColor)
        {
            yield return new("loader_color", loaderColor);
        }

        if (LoaderColorDark is { Length: > 0 } loaderColorDark)
        {
            yield return new("loader_color_dark", loaderColorDark);
        }

        if (ProseTextSize is { Length: > 0 } proseTextSize)
        {
            yield return new("prose_text_size", proseTextSize);
        }

        if (ProseTextWeight is { Length: > 0 } proseTextWeight)
        {
            yield return new("prose_text_weight", proseTextWeight);
        }

        if (ProseHeaderTextWeight is { Length: > 0 } proseHeaderTextWeight)
        {
            yield return new("prose_header_text_weight", proseHeaderTextWeight);
        }

        if (SliderColor is { Length: > 0 } sliderColor)
        {
            yield return new("slider_color", sliderColor);
        }

        if (SliderColorDark is { Length: > 0 } sliderColorDark)
        {
            yield return new("slider_color_dark", sliderColorDark);
        }

        if (StatBackgroundFill is { Length: > 0 } statBackgroundFill)
        {
            yield return new("stat_background_fill", statBackgroundFill);
        }

        if (StatBackgroundFillDark is { Length: > 0 } statBackgroundFillDark)
        {
            yield return new("stat_background_fill_dark", statBackgroundFillDark);
        }

        if (TableBorderColor is { Length: > 0 } tableBorderColor)
        {
            yield return new("table_border_color", tableBorderColor);
        }

        if (TableBorderColorDark is { Length: > 0 } tableBorderColorDark)
        {
            yield return new("table_border_color_dark", tableBorderColorDark);
        }

        if (TableEvenBackgroundFill is { Length: > 0 } tableEvenBackgroundFill)
        {
            yield return new("table_even_background_fill", tableEvenBackgroundFill);
        }

        if (TableEvenBackgroundFillDark is { Length: > 0 } tableEvenBackgroundFillDark)
        {
            yield return new("table_even_background_fill_dark", tableEvenBackgroundFillDark);
        }

        if (TableOddBackgroundFill is { Length: > 0 } tableOddBackgroundFill)
        {
            yield return new("table_odd_background_fill", tableOddBackgroundFill);
        }

        if (TableOddBackgroundFillDark is { Length: > 0 } tableOddBackgroundFillDark)
        {
            yield return new("table_odd_background_fill_dark", tableOddBackgroundFillDark);
        }

        if (TableRadius is { Length: > 0 } tableRadius)
        {
            yield return new("table_radius", tableRadius);
        }

        if (TableRowFocus is { Length: > 0 } tableRowFocus)
        {
            yield return new("table_row_focus", tableRowFocus);
        }

        if (TableRowFocusDark is { Length: > 0 } tableRowFocusDark)
        {
            yield return new("table_row_focus_dark", tableRowFocusDark);
        }

        if (ButtonBorderWidth is { Length: > 0 } buttonBorderWidth)
        {
            yield return new("button_border_width", buttonBorderWidth);
        }

        if (ButtonBorderWidthDark is { Length: > 0 } buttonBorderWidthDark)
        {
            yield return new("button_border_width_dark", buttonBorderWidthDark);
        }

        if (ButtonCancelBackgroundFill is { Length: > 0 } buttonCancelBackgroundFill)
        {
            yield return new("button_cancel_background_fill", buttonCancelBackgroundFill);
        }

        if (ButtonCancelBackgroundFillDark is { Length: > 0 } buttonCancelBackgroundFillDark)
        {
            yield return new("button_cancel_background_fill_dark", buttonCancelBackgroundFillDark);
        }

        if (ButtonCancelBackgroundFillHover is { Length: > 0 } buttonCancelBackgroundFillHover)
        {
            yield return new("button_cancel_background_fill_hover", buttonCancelBackgroundFillHover);
        }

        if (ButtonCancelBackgroundFillHoverDark is { Length: > 0 } buttonCancelBackgroundFillHoverDark)
        {
            yield return new("button_cancel_background_fill_hover_dark", buttonCancelBackgroundFillHoverDark);
        }

        if (ButtonCancelBorderColor is { Length: > 0 } buttonCancelBorderColor)
        {
            yield return new("button_cancel_border_color", buttonCancelBorderColor);
        }

        if (ButtonCancelBorderColorDark is { Length: > 0 } buttonCancelBorderColorDark)
        {
            yield return new("button_cancel_border_color_dark", buttonCancelBorderColorDark);
        }

        if (ButtonCancelBorderColorHover is { Length: > 0 } buttonCancelBorderColorHover)
        {
            yield return new("button_cancel_border_color_hover", buttonCancelBorderColorHover);
        }

        if (ButtonCancelBorderColorHoverDark is { Length: > 0 } buttonCancelBorderColorHoverDark)
        {
            yield return new("button_cancel_border_color_hover_dark", buttonCancelBorderColorHoverDark);
        }

        if (ButtonCancelTextColor is { Length: > 0 } buttonCancelTextColor)
        {
            yield return new("button_cancel_text_color", buttonCancelTextColor);
        }

        if (ButtonCancelTextColorDark is { Length: > 0 } buttonCancelTextColorDark)
        {
            yield return new("button_cancel_text_color_dark", buttonCancelTextColorDark);
        }

        if (ButtonCancelTextColorHover is { Length: > 0 } buttonCancelTextColorHover)
        {
            yield return new("button_cancel_text_color_hover", buttonCancelTextColorHover);
        }

        if (ButtonCancelTextColorHoverDark is { Length: > 0 } buttonCancelTextColorHoverDark)
        {
            yield return new("button_cancel_text_color_hover_dark", buttonCancelTextColorHoverDark);
        }

        if (ButtonCancelShadow is { Length: > 0 } buttonCancelShadow)
        {
            yield return new("button_cancel_shadow", buttonCancelShadow);
        }

        if (ButtonCancelShadowHover is { Length: > 0 } buttonCancelShadowHover)
        {
            yield return new("button_cancel_shadow_hover", buttonCancelShadowHover);
        }

        if (ButtonCancelShadowActive is { Length: > 0 } buttonCancelShadowActive)
        {
            yield return new("button_cancel_shadow_active", buttonCancelShadowActive);
        }

        if (ButtonCancelShadowDark is { Length: > 0 } buttonCancelShadowDark)
        {
            yield return new("button_cancel_shadow_dark", buttonCancelShadowDark);
        }

        if (ButtonCancelShadowHoverDark is { Length: > 0 } buttonCancelShadowHoverDark)
        {
            yield return new("button_cancel_shadow_hover_dark", buttonCancelShadowHoverDark);
        }

        if (ButtonCancelShadowActiveDark is { Length: > 0 } buttonCancelShadowActiveDark)
        {
            yield return new("button_cancel_shadow_active_dark", buttonCancelShadowActiveDark);
        }

        if (ButtonTransformHover is { Length: > 0 } buttonTransformHover)
        {
            yield return new("button_transform_hover", buttonTransformHover);
        }

        if (ButtonTransformActive is { Length: > 0 } buttonTransformActive)
        {
            yield return new("button_transform_active", buttonTransformActive);
        }

        if (ButtonTransition is { Length: > 0 } buttonTransition)
        {
            yield return new("button_transition", buttonTransition);
        }

        if (ButtonLargePadding is { Length: > 0 } buttonLargePadding)
        {
            yield return new("button_large_padding", buttonLargePadding);
        }

        if (ButtonLargeRadius is { Length: > 0 } buttonLargeRadius)
        {
            yield return new("button_large_radius", buttonLargeRadius);
        }

        if (ButtonLargeTextSize is { Length: > 0 } buttonLargeTextSize)
        {
            yield return new("button_large_text_size", buttonLargeTextSize);
        }

        if (ButtonLargeTextWeight is { Length: > 0 } buttonLargeTextWeight)
        {
            yield return new("button_large_text_weight", buttonLargeTextWeight);
        }

        if (ButtonPrimaryBackgroundFill is { Length: > 0 } buttonPrimaryBackgroundFill)
        {
            yield return new("button_primary_background_fill", buttonPrimaryBackgroundFill);
        }

        if (ButtonPrimaryBackgroundFillDark is { Length: > 0 } buttonPrimaryBackgroundFillDark)
        {
            yield return new("button_primary_background_fill_dark", buttonPrimaryBackgroundFillDark);
        }

        if (ButtonPrimaryBackgroundFillHover is { Length: > 0 } buttonPrimaryBackgroundFillHover)
        {
            yield return new("button_primary_background_fill_hover", buttonPrimaryBackgroundFillHover);
        }

        if (ButtonPrimaryBackgroundFillHoverDark is { Length: > 0 } buttonPrimaryBackgroundFillHoverDark)
        {
            yield return new("button_primary_background_fill_hover_dark", buttonPrimaryBackgroundFillHoverDark);
        }

        if (ButtonPrimaryBorderColor is { Length: > 0 } buttonPrimaryBorderColor)
        {
            yield return new("button_primary_border_color", buttonPrimaryBorderColor);
        }

        if (ButtonPrimaryBorderColorDark is { Length: > 0 } buttonPrimaryBorderColorDark)
        {
            yield return new("button_primary_border_color_dark", buttonPrimaryBorderColorDark);
        }

        if (ButtonPrimaryBorderColorHover is { Length: > 0 } buttonPrimaryBorderColorHover)
        {
            yield return new("button_primary_border_color_hover", buttonPrimaryBorderColorHover);
        }

        if (ButtonPrimaryBorderColorHoverDark is { Length: > 0 } buttonPrimaryBorderColorHoverDark)
        {
            yield return new("button_primary_border_color_hover_dark", buttonPrimaryBorderColorHoverDark);
        }

        if (ButtonPrimaryTextColor is { Length: > 0 } buttonPrimaryTextColor)
        {
            yield return new("button_primary_text_color", buttonPrimaryTextColor);
        }

        if (ButtonPrimaryTextColorDark is { Length: > 0 } buttonPrimaryTextColorDark)
        {
            yield return new("button_primary_text_color_dark", buttonPrimaryTextColorDark);
        }

        if (ButtonPrimaryTextColorHover is { Length: > 0 } buttonPrimaryTextColorHover)
        {
            yield return new("button_primary_text_color_hover", buttonPrimaryTextColorHover);
        }

        if (ButtonPrimaryTextColorHoverDark is { Length: > 0 } buttonPrimaryTextColorHoverDark)
        {
            yield return new("button_primary_text_color_hover_dark", buttonPrimaryTextColorHoverDark);
        }

        if (ButtonPrimaryShadow is { Length: > 0 } buttonPrimaryShadow)
        {
            yield return new("button_primary_shadow", buttonPrimaryShadow);
        }

        if (ButtonPrimaryShadowHover is { Length: > 0 } buttonPrimaryShadowHover)
        {
            yield return new("button_primary_shadow_hover", buttonPrimaryShadowHover);
        }

        if (ButtonPrimaryShadowActive is { Length: > 0 } buttonPrimaryShadowActive)
        {
            yield return new("button_primary_shadow_active", buttonPrimaryShadowActive);
        }

        if (ButtonPrimaryShadowDark is { Length: > 0 } buttonPrimaryShadowDark)
        {
            yield return new("button_primary_shadow_dark", buttonPrimaryShadowDark);
        }

        if (ButtonPrimaryShadowHoverDark is { Length: > 0 } buttonPrimaryShadowHoverDark)
        {
            yield return new("button_primary_shadow_hover_dark", buttonPrimaryShadowHoverDark);
        }

        if (ButtonPrimaryShadowActiveDark is { Length: > 0 } buttonPrimaryShadowActiveDark)
        {
            yield return new("button_primary_shadow_active_dark", buttonPrimaryShadowActiveDark);
        }

        if (ButtonSecondaryBackgroundFill is { Length: > 0 } buttonSecondaryBackgroundFill)
        {
            yield return new("button_secondary_background_fill", buttonSecondaryBackgroundFill);
        }

        if (ButtonSecondaryBackgroundFillDark is { Length: > 0 } buttonSecondaryBackgroundFillDark)
        {
            yield return new("button_secondary_background_fill_dark", buttonSecondaryBackgroundFillDark);
        }

        if (ButtonSecondaryBackgroundFillHover is { Length: > 0 } buttonSecondaryBackgroundFillHover)
        {
            yield return new("button_secondary_background_fill_hover", buttonSecondaryBackgroundFillHover);
        }

        if (ButtonSecondaryBackgroundFillHoverDark is { Length: > 0 } buttonSecondaryBackgroundFillHoverDark)
        {
            yield return new("button_secondary_background_fill_hover_dark", buttonSecondaryBackgroundFillHoverDark);
        }

        if (ButtonSecondaryBorderColor is { Length: > 0 } buttonSecondaryBorderColor)
        {
            yield return new("button_secondary_border_color", buttonSecondaryBorderColor);
        }

        if (ButtonSecondaryBorderColorDark is { Length: > 0 } buttonSecondaryBorderColorDark)
        {
            yield return new("button_secondary_border_color_dark", buttonSecondaryBorderColorDark);
        }

        if (ButtonSecondaryBorderColorHover is { Length: > 0 } buttonSecondaryBorderColorHover)
        {
            yield return new("button_secondary_border_color_hover", buttonSecondaryBorderColorHover);
        }

        if (ButtonSecondaryBorderColorHoverDark is { Length: > 0 } buttonSecondaryBorderColorHoverDark)
        {
            yield return new("button_secondary_border_color_hover_dark", buttonSecondaryBorderColorHoverDark);
        }

        if (ButtonSecondaryTextColor is { Length: > 0 } buttonSecondaryTextColor)
        {
            yield return new("button_secondary_text_color", buttonSecondaryTextColor);
        }

        if (ButtonSecondaryTextColorDark is { Length: > 0 } buttonSecondaryTextColorDark)
        {
            yield return new("button_secondary_text_color_dark", buttonSecondaryTextColorDark);
        }

        if (ButtonSecondaryTextColorHover is { Length: > 0 } buttonSecondaryTextColorHover)
        {
            yield return new("button_secondary_text_color_hover", buttonSecondaryTextColorHover);
        }

        if (ButtonSecondaryTextColorHoverDark is { Length: > 0 } buttonSecondaryTextColorHoverDark)
        {
            yield return new("button_secondary_text_color_hover_dark", buttonSecondaryTextColorHoverDark);
        }

        if (ButtonSecondaryShadow is { Length: > 0 } buttonSecondaryShadow)
        {
            yield return new("button_secondary_shadow", buttonSecondaryShadow);
        }

        if (ButtonSecondaryShadowHover is { Length: > 0 } buttonSecondaryShadowHover)
        {
            yield return new("button_secondary_shadow_hover", buttonSecondaryShadowHover);
        }

        if (ButtonSecondaryShadowActive is { Length: > 0 } buttonSecondaryShadowActive)
        {
            yield return new("button_secondary_shadow_active", buttonSecondaryShadowActive);
        }

        if (ButtonSecondaryShadowDark is { Length: > 0 } buttonSecondaryShadowDark)
        {
            yield return new("button_secondary_shadow_dark", buttonSecondaryShadowDark);
        }

        if (ButtonSecondaryShadowHoverDark is { Length: > 0 } buttonSecondaryShadowHoverDark)
        {
            yield return new("button_secondary_shadow_hover_dark", buttonSecondaryShadowHoverDark);
        }

        if (ButtonSecondaryShadowActiveDark is { Length: > 0 } buttonSecondaryShadowActiveDark)
        {
            yield return new("button_secondary_shadow_active_dark", buttonSecondaryShadowActiveDark);
        }

        if (ButtonSmallPadding is { Length: > 0 } buttonSmallPadding)
        {
            yield return new("button_small_padding", buttonSmallPadding);
        }

        if (ButtonSmallRadius is { Length: > 0 } buttonSmallRadius)
        {
            yield return new("button_small_radius", buttonSmallRadius);
        }

        if (ButtonSmallTextSize is { Length: > 0 } buttonSmallTextSize)
        {
            yield return new("button_small_text_size", buttonSmallTextSize);
        }

        if (ButtonSmallTextWeight is { Length: > 0 } buttonSmallTextWeight)
        {
            yield return new("button_small_text_weight", buttonSmallTextWeight);
        }

        if (ButtonMediumPadding is { Length: > 0 } buttonMediumPadding)
        {
            yield return new("button_medium_padding", buttonMediumPadding);
        }

        if (ButtonMediumRadius is { Length: > 0 } buttonMediumRadius)
        {
            yield return new("button_medium_radius", buttonMediumRadius);
        }

        if (ButtonMediumTextSize is { Length: > 0 } buttonMediumTextSize)
        {
            yield return new("button_medium_text_size", buttonMediumTextSize);
        }

        if (ButtonMediumTextWeight is { Length: > 0 } buttonMediumTextWeight)
        {
            yield return new("button_medium_text_weight", buttonMediumTextWeight);
        }
    }
}

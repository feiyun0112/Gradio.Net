
namespace Gradio.Net.Core.Themes;

public sealed class DefaultTheme : Theme
{
    public DefaultTheme()
    {
        Name = "default";
        SetVariables(new Dictionary<string, string>
        {
            ["input_background_fill_dark"] = "*neutral_800",
            ["error_background_fill"] = ThemePalette.Red50,
            ["error_background_fill_dark"] = "*neutral_900",
            ["error_border_color"] = ThemePalette.Red700,
            ["error_border_color_dark"] = ThemePalette.Red500,
            ["error_icon_color"] = ThemePalette.Red700,
            ["error_icon_color_dark"] = ThemePalette.Red500,
            ["input_shadow_focus"] = "0 0 0 *shadow_spread *secondary_50, *shadow_inset",
            ["input_shadow_focus_dark"] = "0 0 0 *shadow_spread *neutral_700, *shadow_inset",
            ["button_border_width"] = "0px",
            ["input_border_width"] = "1px",
            ["input_background_fill"] = "white",
            ["stat_background_fill"] = "linear-gradient(to right, *primary_400, *primary_200)",
            ["stat_background_fill_dark"] = "linear-gradient(to right, *primary_400, *primary_600)",
            ["checkbox_label_background_fill"] = "*background_fill_primary",
            ["checkbox_label_background_fill_dark"] = "*neutral_800",
            ["checkbox_label_background_fill_hover"] = "*background_fill_secondary",
            ["checkbox_label_background_fill_hover_dark"] = "*checkbox_label_background_fill",
            ["button_primary_background_fill"] = "*primary_500",
            ["button_primary_background_fill_dark"] = "*primary_600",
            ["button_primary_background_fill_hover"] = "*primary_600",
            ["button_primary_background_fill_hover_dark"] = "*primary_700",
            ["button_primary_text_color"] = "white",
            ["button_primary_text_color_dark"] = "white",
            ["button_secondary_background_fill"] = "*neutral_200",
            ["button_secondary_background_fill_dark"] = "*neutral_600",
            ["button_secondary_background_fill_hover"] = "*neutral_300",
            ["button_secondary_background_fill_hover_dark"] = "*neutral_700",
            ["button_secondary_text_color"] = "black",
            ["button_secondary_text_color_dark"] = "white",
            ["button_cancel_background_fill"] = ThemePalette.Red500,
            ["button_cancel_background_fill_dark"] = ThemePalette.Red700,
            ["button_cancel_background_fill_hover"] = ThemePalette.Red600,
            ["button_cancel_background_fill_hover_dark"] = ThemePalette.Red800,
            ["button_cancel_text_color"] = "white",
            ["button_cancel_text_color_dark"] = "white",
            ["button_cancel_text_color_hover"] = "white",
            ["button_cancel_text_color_hover_dark"] = "white",
            ["border_color_accent_subdued"] = "*primary_200"
        });
    }
}

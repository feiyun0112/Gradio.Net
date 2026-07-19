
namespace Gradio.Net.Core.Themes;

public sealed class OriginTheme : Theme
{
    public OriginTheme()
    {
        Name = "origin";
        Stylesheets = new List<string>
        {
            "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:wght@400;600&display=swap"
        };

        // Python parity: neutral_hue=colors.gray (different from base zinc)
        Neutral50 = "#f9fafb";
        Neutral100 = "#f3f4f6";
        Neutral200 = "#e5e7eb";
        Neutral300 = "#d1d5db";
        Neutral400 = "#9ca3af";
        Neutral500 = "#6b7280";
        Neutral600 = "#4b5563";
        Neutral700 = "#374151";
        Neutral800 = "#1f2937";
        Neutral900 = "#111827";
        Neutral950 = "#0b0f19";

        SetVariables(new Dictionary<string, string>
        {
            ["input_background_fill_dark"] = "*neutral_800",
            ["error_background_fill"] = ThemePalette.Red50,
            ["error_background_fill_dark"] = "*neutral_900",
            ["error_border_color"] = ThemePalette.Red700,
            ["error_border_color_dark"] = ThemePalette.Red500,
            ["error_icon_color"] = ThemePalette.Red700,
            ["error_icon_color_dark"] = ThemePalette.Red500,
            ["button_transition"] = "none",
            ["button_primary_shadow"] = "*shadow_drop",
            ["button_primary_shadow_hover"] = "*shadow_drop_lg",
            ["button_primary_shadow_active"] = "*shadow_inset",
            ["button_secondary_shadow"] = "*shadow_drop",
            ["button_secondary_shadow_hover"] = "*shadow_drop_lg",
            ["button_secondary_shadow_active"] = "*shadow_inset",
            ["input_shadow"] = "0 0 0 *shadow_spread transparent, *shadow_inset",
            ["input_shadow_focus"] = "0 0 0 *shadow_spread *secondary_50, *shadow_inset",
            ["input_shadow_focus_dark"] = "0 0 0 *shadow_spread *neutral_700, *shadow_inset",
            ["checkbox_label_shadow"] = "*shadow_drop",
            ["block_shadow"] = "*shadow_drop",
            ["form_gap_width"] = "1px",
            ["button_border_width"] = "1px",
            ["button_border_width_dark"] = "1px",
            ["input_border_width"] = "1px",
            ["input_background_fill"] = "white",
            ["stat_background_fill"] = "linear-gradient(to right, *primary_400, *primary_200)",
            ["stat_background_fill_dark"] = "linear-gradient(to right, *primary_400, *primary_600)",
            ["checkbox_label_background_fill"] = "linear-gradient(to top, *neutral_50, white)",
            ["checkbox_label_background_fill_dark"] = "linear-gradient(to top, *neutral_900, *neutral_800)",
            ["checkbox_label_background_fill_hover"] = "linear-gradient(to top, *neutral_100, white)",
            ["checkbox_label_background_fill_hover_dark"] = "linear-gradient(to top, *neutral_900, *neutral_800)",
            ["button_primary_background_fill"] = "linear-gradient(to bottom right, *primary_100, *primary_300)",
            ["button_primary_background_fill_dark"] = "linear-gradient(to bottom right, *primary_500, *primary_600)",
            ["button_primary_background_fill_hover"] = "linear-gradient(to bottom right, *primary_100, *primary_200)",
            ["button_primary_background_fill_hover_dark"] = "linear-gradient(to bottom right, *primary_500, *primary_500)",
            ["button_primary_border_color"] = "*primary_200",
            ["button_primary_border_color_dark"] = "*primary_500",
            ["button_primary_border_color_hover"] = "*button_primary_border_color",
            ["button_primary_border_color_hover_dark"] = "*primary_500",
            ["button_primary_text_color"] = "*primary_600",
            ["button_primary_text_color_dark"] = "white",
            ["button_secondary_background_fill"] = "linear-gradient(to bottom right, *neutral_100, *neutral_200)",
            ["button_secondary_background_fill_dark"] = "linear-gradient(to bottom right, *neutral_600, *neutral_700)",
            ["button_secondary_background_fill_hover"] = "linear-gradient(to bottom right, *neutral_100, *neutral_100)",
            ["button_secondary_background_fill_hover_dark"] = "linear-gradient(to bottom right, *neutral_600, *neutral_600)",
            ["button_secondary_border_color"] = "*neutral_200",
            ["button_secondary_border_color_dark"] = "*neutral_600",
            ["button_secondary_border_color_hover"] = "*neutral_200",
            ["button_secondary_border_color_hover_dark"] = "*neutral_600",
            ["button_secondary_text_color"] = "*neutral_800",
            ["button_secondary_text_color_dark"] = "white",
            ["button_cancel_background_fill"] = $"linear-gradient(to bottom right, {ThemePalette.Red100}, {ThemePalette.Red200})",
            ["button_cancel_background_fill_dark"] = $"linear-gradient(to bottom right, {ThemePalette.Red600}, {ThemePalette.Red700})",
            ["button_cancel_background_fill_hover"] = $"linear-gradient(to bottom right, {ThemePalette.Red100}, {ThemePalette.Red100})",
            ["button_cancel_background_fill_hover_dark"] = $"linear-gradient(to bottom right, {ThemePalette.Red600}, {ThemePalette.Red600})",
            ["button_cancel_border_color"] = ThemePalette.Red200,
            ["button_cancel_border_color_dark"] = ThemePalette.Red600,
            ["button_cancel_border_color_hover"] = ThemePalette.Red200,
            ["button_cancel_border_color_hover_dark"] = ThemePalette.Red600,
            ["button_cancel_text_color"] = ThemePalette.Red600,
            ["button_cancel_text_color_dark"] = "white",
            ["border_color_accent_subdued"] = "*primary_200",
            ["button_transform_hover"] = "none"
        });
    }
}


namespace Gradio.Net.Core.Themes;

public sealed class OceanTheme : Theme
{
    public OceanTheme()
    {
        Name = "ocean";
        Stylesheets = new List<string>
        {
            "https://fonts.googleapis.com/css2?family=IBM+Plex+Sans:wght@400;600&display=swap",
            "https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
        };

        // Python parity: primary_hue=colors.emerald
        Primary50 = "#ecfdf5";
        Primary100 = "#d1fae5";
        Primary200 = "#a7f3d0";
        Primary300 = "#6ee7b7";
        Primary400 = "#34d399";
        Primary500 = "#10b981";
        Primary600 = "#059669";
        Primary700 = "#047857";
        Primary800 = "#065f46";
        Primary900 = "#064e3b";
        Primary950 = "#054436";

        // Python parity: secondary_hue=colors.sky
        Secondary50 = "#f0f9ff";
        Secondary100 = "#e0f2fe";
        Secondary200 = "#bae6fd";
        Secondary300 = "#7dd3fc";
        Secondary400 = "#38bdf8";
        Secondary500 = "#0ea5e9";
        Secondary600 = "#0284c7";
        Secondary700 = "#0369a1";
        Secondary800 = "#075985";
        Secondary900 = "#0c4a6e";
        Secondary950 = "#0b4165";

        // Python parity: radius_size=sizes.radius_md (larger radii than default)
        RadiusXxs = "6px";
        RadiusXs = "8px";
        RadiusSm = "10px";
        RadiusMd = "20px";
        RadiusLg = "24px";
        RadiusXl = "28px";
        RadiusXxl = "32px";

        // Python parity: font=GoogleFont("IBM Plex Sans")
        Font = "'IBM Plex Sans', ui-sans-serif, system-ui, sans-serif";
        FontMono = "'IBM Plex Mono', ui-monospace, Consolas, monospace";

        SetVariables(new Dictionary<string, string>
        {
            // Button overrides
            ["button_border_width"] = "0px",
            ["button_transform_hover"] = "scale(1.02)",
            ["button_transition"] = "all 0.1s ease-in-out",
            ["button_primary_background_fill"] = "linear-gradient(120deg, *secondary_500 0%, *primary_300 60%, *primary_400 100%)",
            ["button_primary_background_fill_hover"] = "linear-gradient(120deg, *secondary_400 0%, *primary_300 60%, *primary_300 100%)",
            ["button_primary_text_color"] = "*button_secondary_text_color",
            ["button_primary_text_color_hover"] = "*button_primary_text_color",
            ["button_primary_border_color"] = "*primary_500",
            ["button_primary_border_color_hover"] = "*primary_500",
            ["button_primary_shadow"] = "none",
            ["button_primary_shadow_hover"] = "0 1px 3px 0 *primary_200, 0 1px 2px -1px *primary_200",
            ["button_primary_shadow_active"] = "*button_primary_shadow",
            ["button_secondary_background_fill"] = "linear-gradient(120deg, *neutral_300 0%, *neutral_100 60%, *neutral_200 100%)",
            ["button_secondary_background_fill_hover"] = "linear-gradient(120deg, *neutral_200 0%, *neutral_100 60%, *neutral_100 100%)",
            ["button_secondary_border_color"] = "*neutral_200",
            ["button_secondary_border_color_hover"] = "*neutral_200",
            ["button_secondary_text_color"] = "black",
            ["button_secondary_text_color_hover"] = "*button_secondary_text_color",
            ["button_secondary_shadow"] = "*button_primary_shadow",
            ["button_secondary_shadow_hover"] = "*shadow_drop_lg",
            ["button_secondary_shadow_active"] = "*button_secondary_shadow",
            ["button_cancel_background_fill"] = "*button_secondary_background_fill",
            ["button_cancel_background_fill_hover"] = "*button_secondary_background_fill_hover",
            ["button_cancel_border_color"] = "*button_secondary_border_color",
            ["button_cancel_border_color_hover"] = "*button_secondary_border_color_hover",
            ["button_cancel_text_color"] = "*button_secondary_text_color",
            ["button_cancel_text_color_hover"] = "*button_secondary_text_color_hover",
            ["button_cancel_shadow"] = "*button_secondary_shadow",
            ["button_cancel_shadow_hover"] = "*button_secondary_shadow_hover",
            ["button_cancel_shadow_active"] = "*button_secondary_shadow_active",
            // Checkbox overrides
            ["checkbox_background_color_selected"] = "*primary_400",
            ["checkbox_label_background_fill"] = "*button_secondary_background_fill",
            ["checkbox_label_background_fill_hover"] = "*button_secondary_background_fill_hover",
            ["checkbox_label_background_fill_selected"] = "linear-gradient(120deg, *primary_400 0%, *primary_300 60%, *primary_400 100%)",
            ["checkbox_label_border_color_selected"] = "*primary_400",
            ["checkbox_label_border_width"] = "1px",
            ["checkbox_label_text_color_selected"] = "*button_secondary_text_color",
            // Block / shadow
            ["block_shadow"] = "*shadow_drop_lg",
            // Input overrides (Python ocean: no border, neutral-100 background)
            ["input_background_fill"] = "*neutral_100",
            ["input_background_fill_focus"] = "*input_background_fill",
            ["input_background_fill_hover"] = "*input_background_fill",
            ["input_border_width"] = "0px",
            ["input_border_color_focus"] = "*secondary_300",
            ["input_placeholder_color"] = "*neutral_400",
            ["input_shadow_focus"] = "*input_shadow",
            // Slider
            ["slider_color"] = "*primary_400",
            // Stat
            ["stat_background_fill"] = "*primary_300",
            // Links
            ["link_text_color"] = "*secondary_600",
            ["link_text_color_active"] = "*secondary_600",
            ["link_text_color_hover"] = "*secondary_700",
            ["link_text_color_visited"] = "*secondary_500",
            // Body text subdued
            ["body_text_color_subdued"] = "*neutral_400",
            // Border accent
            ["border_color_accent_subdued"] = "*border_color_accent",
            // Dark mode overrides
            ["slider_color_dark"] = "*primary_500",
            ["button_primary_background_fill_dark"] = "linear-gradient(120deg, *secondary_600 0%, *primary_500 60%, *primary_600 100%)",
            ["button_primary_background_fill_hover_dark"] = "linear-gradient(120deg, *secondary_500 0%, *primary_500 60%, *primary_500 100%)",
            ["button_primary_border_color_dark"] = "*primary_600",
            ["button_primary_border_color_hover_dark"] = "*primary_500",
            ["button_primary_text_color_dark"] = "*button_secondary_text_color",
            ["button_primary_shadow_dark"] = "none",
            ["button_primary_shadow_hover_dark"] = "*button_primary_shadow",
            ["button_primary_shadow_active_dark"] = "*button_primary_shadow",
            ["button_secondary_background_fill_dark"] = "linear-gradient(120deg, *neutral_700 0%, *neutral_600 60%, *neutral_700 100%)",
            ["button_secondary_background_fill_hover_dark"] = "linear-gradient(120deg, *neutral_600 0%, *neutral_600 60%, *neutral_700 100%)",
            ["button_secondary_border_color_dark"] = "*neutral_600",
            ["button_secondary_border_color_hover_dark"] = "*neutral_500",
            ["button_secondary_text_color_dark"] = "white",
            ["button_secondary_text_color_hover_dark"] = "*button_secondary_text_color",
            ["button_secondary_shadow_dark"] = "none",
            ["button_secondary_shadow_hover_dark"] = "*button_secondary_shadow",
            ["button_secondary_shadow_active_dark"] = "*button_secondary_shadow",
            ["checkbox_background_color_selected_dark"] = "*primary_600",
            ["checkbox_label_background_fill_selected_dark"] = "linear-gradient(120deg, *primary_600 0%, *primary_500 60%, *primary_600 100%)",
            ["checkbox_label_border_color_selected_dark"] = "*primary_600",
            ["checkbox_label_text_color_selected_dark"] = "*button_secondary_text_color",
            ["stat_background_fill_dark"] = "*primary_500",
            ["input_background_fill_dark"] = "*neutral_700",
            ["input_border_color_focus_dark"] = "*neutral_700",
            // Override error colors to match Python ocean theme
            ["error_border_color"] = "#b91c1c",
            ["error_text_color"] = "#b91c1c",
            ["error_icon_color"] = "#b91c1c",
            // Override checkbox label selected text color
            ["checkbox_label_text_color_selected"] = "*button_secondary_text_color",
            // Override --name CSS variable
            ["name"] = "ocean",
        });
    }
}

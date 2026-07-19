
namespace Gradio.Net.Core.Themes;

public sealed class CitrusTheme : Theme
{
    public CitrusTheme()
    {
        Name = "citrus";
        Stylesheets = new List<string>
        {
            "https://fonts.googleapis.com/css2?family=Ubuntu:wght@400;600&display=swap",
            "https://fonts.googleapis.com/css2?family=Roboto+Mono:wght@400;600&display=swap"
        };

        // Python parity: primary_hue=secondary_hue=colors.amber
        Primary50 = "#fffbeb";
        Primary100 = "#fef3c7";
        Primary200 = "#fde68a";
        Primary300 = "#fcd34d";
        Primary400 = "#fbbf24";
        Primary500 = "#f59e0b";
        Primary600 = "#d97706";
        Primary700 = "#b45309";
        Primary800 = "#92400e";
        Primary900 = "#78350f";
        Primary950 = "#6c370f";
        Secondary50 = "#fffbeb";
        Secondary100 = "#fef3c7";
        Secondary200 = "#fde68a";
        Secondary300 = "#fcd34d";
        Secondary400 = "#fbbf24";
        Secondary500 = "#f59e0b";
        Secondary600 = "#d97706";
        Secondary700 = "#b45309";
        Secondary800 = "#92400e";
        Secondary900 = "#78350f";
        Secondary950 = "#6c370f";

        // Python parity: neutral_hue=colors.stone
        Neutral50 = "#fafaf9";
        Neutral100 = "#f5f5f4";
        Neutral200 = "#e7e5e4";
        Neutral300 = "#d6d3d1";
        Neutral400 = "#a8a29e";
        Neutral500 = "#78716c";
        Neutral600 = "#57534e";
        Neutral700 = "#44403c";
        Neutral800 = "#292524";
        Neutral900 = "#1c1917";
        Neutral950 = "#0f0e0d";

        // Python parity: spacing_size=sizes.spacing_lg
        SpacingXxs = "2px";
        SpacingXs = "4px";
        SpacingSm = "6px";
        SpacingMd = "8px";
        SpacingLg = "10px";
        SpacingXl = "14px";
        SpacingXxl = "28px";

        // Python parity: font=GoogleFont("Ubuntu"), font_mono=GoogleFont("Roboto Mono")
        Font = "Ubuntu, ui-sans-serif, system-ui, sans-serif";
        FontMono = "'Roboto Mono', ui-monospace, Consolas, monospace";

        SetVariables(new Dictionary<string, string>
        {
            ["slider_color"] = "*primary_400",
            ["background_fill_primary"] = "*neutral_50",
            ["button_primary_background_fill"] = "*primary_500",
            ["button_primary_text_color"] = "*button_secondary_text_color",
            ["button_secondary_background_fill"] = "*primary_400",
            ["button_primary_background_fill_hover"] = "*button_primary_background_fill",
            ["button_secondary_background_fill_hover"] = "*button_secondary_background_fill",
            ["button_primary_shadow_hover"] = "0px 5px 0px 0px *primary_400;",
            ["button_primary_shadow"] = "0px 3px 0px 0px *primary_400;",
            ["button_primary_shadow_active"] = "0px 2px 0px 0px *primary_400;",
            ["button_secondary_shadow_hover"] = "0px 5px 0px 0px *primary_300;",
            ["button_secondary_shadow"] = "0px 3px 0px 0px *primary_300;",
            ["button_secondary_shadow_active"] = "0px 2px 0px 0px *primary_300;",
            ["input_shadow"] = "0px -1px 0px 0px *neutral_300;",
            ["input_shadow_focus"] = "0px -1px 0px 0px *primary_300;",
            ["input_background_fill"] = "*neutral_50",
            ["input_background_fill_focus"] = "*primary_50",
            ["block_shadow"] = "0px 3px 0px 0px *neutral_300;",
            ["checkbox_label_background_fill"] = "*neutral_200",
            ["checkbox_label_background_fill_hover"] = "*checkbox_label_background_fill",
            ["checkbox_label_background_fill_selected"] = "*primary_400",
            ["checkbox_label_border_color_selected"] = "*primary_300",
            ["checkbox_label_border_width"] = "2px",
            ["slider_color_dark"] = "*primary_500",
            ["button_primary_background_fill_dark"] = "*primary_600",
            ["button_secondary_text_color_dark"] = "*neutral_900",
            ["button_primary_text_color_dark"] = "*button_secondary_text_color",
            ["button_secondary_background_fill_dark"] = "*primary_500",
            ["button_primary_background_fill_hover_dark"] = "*button_primary_background_fill",
            ["button_secondary_background_fill_hover_dark"] = "*button_secondary_background_fill",
            ["button_primary_shadow_hover_dark"] = "0px 5px 0px 0px *primary_700;",
            ["button_primary_shadow_dark"] = "0px 3px 0px 0px *primary_700;",
            ["button_primary_shadow_active_dark"] = "0px 2px 0px 0px *primary_700;",
            ["button_secondary_shadow_hover_dark"] = "0px 5px 0px 0px *primary_600;",
            ["button_secondary_shadow_dark"] = "0px 3px 0px 0px *primary_600;",
            ["button_secondary_shadow_active_dark"] = "0px 2px 0px 0px *primary_600;",
            ["input_shadow_dark"] = "0px -1px 0px 0px *neutral_700;",
            ["input_shadow_focus_dark"] = "0px -1px 0px 0px *primary_600;",
            ["input_background_fill_dark"] = "*neutral_900",
            ["input_background_fill_focus_dark"] = "none",
            ["block_shadow_dark"] = "0px 3px 0px 0px *neutral_700;",
            ["checkbox_label_background_fill_dark"] = "*neutral_700",
            ["checkbox_label_background_fill_hover_dark"] = "*checkbox_label_background_fill",
            ["checkbox_label_background_fill_selected_dark"] = "*primary_500",
            ["checkbox_label_border_color_selected_dark"] = "*primary_600",
            ["checkbox_label_text_color_selected_dark"] = "*button_primary_text_color",
            ["checkbox_background_color_selected_dark"] = "*primary_600",
            ["checkbox_background_color_dark"] = "*neutral_400",
            ["checkbox_label_border_width_dark"] = "2px",
            ["button_transform_hover"] = "translateY(-2px);",
            ["button_transform_active"] = "translateY(1px);",
            ["button_transition"] = "all 0.1s;",
            ["button_border_width"] = "0px",
            ["panel_border_width"] = "1px",
            ["block_border_width"] = "1px",
            ["input_border_width"] = "1px",
            ["block_border_color"] = "*neutral_300",
            ["block_background_fill"] = "*neutral_100",
            ["block_label_shadow"] = "none",
            ["checkbox_shadow"] = "none",
            ["button_cancel_background_fill"] = ThemePalette.Red500,
            ["button_cancel_background_fill_dark"] = ThemePalette.Red700,
            ["button_cancel_background_fill_hover"] = ThemePalette.Red600,
            ["button_cancel_background_fill_hover_dark"] = ThemePalette.Red800,
            ["button_cancel_text_color"] = "white",
            ["button_cancel_text_color_dark"] = "white",
            ["button_cancel_text_color_hover"] = "white",
            ["button_cancel_text_color_hover_dark"] = "white",
            ["button_cancel_shadow"] = "0px 3px 0px 0px rgb(248 113 113)",
            ["button_cancel_shadow_hover"] = "0px 5px 0px 0px rgb(248 113 113)",
            ["button_cancel_shadow_active"] = "0px 2px 0px 0px rgb(248 113 113)",
            ["button_cancel_shadow_dark"] = "0px 3px 0px 0px rgb(220 38 38)",
            ["button_cancel_shadow_hover_dark"] = "0px 5px 0px 0px rgb(220 38 38)",
            ["button_cancel_shadow_active_dark"] = "0px 2px 0px 0px rgb(220 38 38)"
        });
    }
}

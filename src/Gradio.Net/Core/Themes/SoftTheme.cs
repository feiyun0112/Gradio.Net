
namespace Gradio.Net.Core.Themes;

public sealed class SoftTheme : Theme
{
    public SoftTheme()
    {
        Name = "soft";
        Stylesheets = new List<string>();

        // Python parity: primary_hue=secondary_hue=colors.indigo
        Primary50 = "#eef2ff";
        Primary100 = "#e0e7ff";
        Primary200 = "#c7d2fe";
        Primary300 = "#a5b4fc";
        Primary400 = "#818cf8";
        Primary500 = "#6366f1";
        Primary600 = "#4f46e5";
        Primary700 = "#4338ca";
        Primary800 = "#3730a3";
        Primary900 = "#312e81";
        Primary950 = "#2b2c5e";
        Secondary50 = "#eef2ff";
        Secondary100 = "#e0e7ff";
        Secondary200 = "#c7d2fe";
        Secondary300 = "#a5b4fc";
        Secondary400 = "#818cf8";
        Secondary500 = "#6366f1";
        Secondary600 = "#4f46e5";
        Secondary700 = "#4338ca";
        Secondary800 = "#3730a3";
        Secondary900 = "#312e81";
        Secondary950 = "#2b2c5e";

        // Python parity: neutral_hue=colors.gray
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

        // Python parity: font=LocalFont("Montserrat") (bundled in static/fonts)
        Font = "Montserrat, ui-sans-serif, system-ui, sans-serif";

        SetVariables(new Dictionary<string, string>
        {
            ["background_fill_primary"] = "*neutral_50",
            ["slider_color"] = "*primary_500",
            ["slider_color_dark"] = "*primary_600",
            ["shadow_drop"] = "0 1px 4px 0 rgb(0 0 0 / 0.1)",
            ["shadow_drop_lg"] = "0 2px 5px 0 rgb(0 0 0 / 0.1)",
            ["block_background_fill"] = "white",
            ["block_label_padding"] = "*spacing_sm *spacing_md",
            ["block_label_background_fill"] = "*primary_100",
            ["block_label_background_fill_dark"] = "*primary_600",
            ["block_label_radius"] = "*radius_md",
            ["block_label_text_size"] = "*text_md",
            ["block_label_text_weight"] = "600",
            ["block_label_text_color"] = "*primary_500",
            ["block_label_text_color_dark"] = "white",
            ["block_title_radius"] = "*block_label_radius",
            ["block_title_padding"] = "*block_label_padding",
            ["block_title_background_fill"] = "*block_label_background_fill",
            ["block_title_text_weight"] = "600",
            ["block_title_text_color"] = "*primary_500",
            ["block_title_text_color_dark"] = "white",
            ["block_label_margin"] = "*spacing_md",
            ["input_background_fill"] = "white",
            ["input_border_color"] = "*neutral_50",
            ["input_shadow"] = "*shadow_drop",
            ["input_shadow_focus"] = "*shadow_drop_lg",
            ["checkbox_shadow"] = "none",
            ["shadow_spread"] = "6px",
            ["button_primary_shadow"] = "*shadow_drop_lg",
            ["button_primary_shadow_hover"] = "*shadow_drop_lg",
            ["button_primary_shadow_active"] = "*shadow_inset",
            ["button_secondary_shadow"] = "*shadow_drop_lg",
            ["button_secondary_shadow_hover"] = "*shadow_drop_lg",
            ["button_secondary_shadow_active"] = "*shadow_inset",
            ["checkbox_label_shadow"] = "*shadow_drop_lg",
            ["button_primary_background_fill"] = "*primary_500",
            ["button_primary_background_fill_hover"] = "*primary_400",
            ["button_primary_background_fill_hover_dark"] = "*primary_500",
            ["button_primary_text_color"] = "white",
            ["button_secondary_background_fill"] = "white",
            ["button_secondary_background_fill_hover"] = "*neutral_100",
            ["button_secondary_background_fill_hover_dark"] = "*primary_500",
            ["button_secondary_text_color"] = "*neutral_800",
            ["button_cancel_background_fill"] = "*button_secondary_background_fill",
            ["button_cancel_background_fill_hover"] = "*button_secondary_background_fill_hover",
            ["button_cancel_background_fill_hover_dark"] = "*button_secondary_background_fill_hover",
            ["button_cancel_text_color"] = "*button_secondary_text_color",
            ["checkbox_label_background_fill_selected"] = "*primary_500",
            ["checkbox_label_background_fill_selected_dark"] = "*primary_600",
            ["checkbox_border_width"] = "1px",
            ["checkbox_border_color"] = "*neutral_100",
            ["checkbox_border_color_dark"] = "*neutral_600",
            ["checkbox_background_color_selected"] = "*primary_600",
            ["checkbox_background_color_selected_dark"] = "*primary_700",
            ["checkbox_border_color_focus"] = "*primary_500",
            ["checkbox_border_color_focus_dark"] = "*primary_600",
            ["checkbox_border_color_selected"] = "*primary_600",
            ["checkbox_border_color_selected_dark"] = "*primary_700",
            ["checkbox_label_text_color_selected"] = "white",
            ["block_border_width"] = "0px",
            ["panel_border_width"] = "1px"
        });
    }
}

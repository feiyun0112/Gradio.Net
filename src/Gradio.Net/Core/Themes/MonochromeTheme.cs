
namespace Gradio.Net.Core.Themes;

public sealed class MonochromeTheme : Theme
{
    public MonochromeTheme()
    {
        Name = "monochrome";
        Stylesheets = new List<string>
        {
            "https://fonts.googleapis.com/css2?family=Lora:wght@400;600&display=swap"
        };

        // Python parity: primary_hue=secondary_hue=neutral_hue=colors.neutral
        Primary50 = "#fafafa";
        Primary100 = "#f5f5f5";
        Primary200 = "#e5e5e5";
        Primary300 = "#d4d4d4";
        Primary400 = "#a3a3a3";
        Primary500 = "#737373";
        Primary600 = "#525252";
        Primary700 = "#404040";
        Primary800 = "#262626";
        Primary900 = "#171717";
        Primary950 = "#0f0f0f";
        Secondary50 = "#fafafa";
        Secondary100 = "#f5f5f5";
        Secondary200 = "#e5e5e5";
        Secondary300 = "#d4d4d4";
        Secondary400 = "#a3a3a3";
        Secondary500 = "#737373";
        Secondary600 = "#525252";
        Secondary700 = "#404040";
        Secondary800 = "#262626";
        Secondary900 = "#171717";
        Secondary950 = "#0f0f0f";
        Neutral50 = "#fafafa";
        Neutral100 = "#f5f5f5";
        Neutral200 = "#e5e5e5";
        Neutral300 = "#d4d4d4";
        Neutral400 = "#a3a3a3";
        Neutral500 = "#737373";
        Neutral600 = "#525252";
        Neutral700 = "#404040";
        Neutral800 = "#262626";
        Neutral900 = "#171717";
        Neutral950 = "#0f0f0f";

        // Python parity: radius_size=sizes.radius_none (all 0px)
        RadiusXxs = "0px";
        RadiusXs = "0px";
        RadiusSm = "0px";
        RadiusMd = "0px";
        RadiusLg = "0px";
        RadiusXl = "0px";
        RadiusXxl = "0px";

        // Python parity: spacing_size=sizes.spacing_lg
        SpacingXxs = "2px";
        SpacingXs = "4px";
        SpacingSm = "6px";
        SpacingMd = "8px";
        SpacingLg = "10px";
        SpacingXl = "14px";
        SpacingXxl = "28px";

        // Python parity: font=GoogleFont("Lora")
        Font = "Lora, ui-sans-serif, system-ui, sans-serif";

        SetVariables(new Dictionary<string, string>
        {
            ["slider_color"] = "*neutral_900",
            ["slider_color_dark"] = "*neutral_500",
            ["accordion_text_color"] = "*body_text_color",
            ["accordion_text_color_dark"] = "*body_text_color",
            ["table_text_color"] = "*body_text_color",
            ["table_text_color_dark"] = "*body_text_color",
            ["body_text_color"] = "*neutral_900",
            ["block_label_text_color"] = "*body_text_color",
            ["block_title_text_color"] = "*body_text_color",
            ["body_text_color_subdued"] = "*neutral_700",
            ["background_fill_primary_dark"] = "*neutral_900",
            ["background_fill_secondary_dark"] = "*neutral_800",
            ["block_background_fill_dark"] = "*neutral_800",
            ["input_background_fill_dark"] = "*neutral_700",
            ["button_border_width"] = "2px",
            ["button_primary_border_color"] = "*neutral_900",
            ["button_primary_background_fill"] = "*neutral_900",
            ["button_primary_background_fill_hover"] = "*neutral_700",
            ["button_primary_text_color"] = "white",
            ["button_primary_background_fill_dark"] = "*neutral_600",
            ["button_primary_background_fill_hover_dark"] = "*neutral_600",
            ["button_primary_text_color_dark"] = "white",
            ["button_secondary_border_color"] = "*neutral_900",
            ["button_secondary_background_fill"] = "white",
            ["button_secondary_background_fill_hover"] = "white",
            ["button_secondary_background_fill_dark"] = "*neutral_700",
            ["button_secondary_border_color_hover"] = "*neutral_400",
            ["button_secondary_text_color"] = "*neutral_900",
            ["button_secondary_text_color_hover"] = "*neutral_400",
            ["button_cancel_border_color"] = "*neutral_900",
            ["button_cancel_background_fill"] = "*button_secondary_background_fill",
            ["button_cancel_background_fill_hover"] = "*button_secondary_background_fill_hover",
            ["button_cancel_text_color"] = "*button_secondary_text_color",
            ["checkbox_label_border_color"] = "*checkbox_background_color",
            ["checkbox_label_border_color_hover"] = "*button_secondary_border_color_hover",
            ["checkbox_label_border_color_selected"] = "*button_primary_border_color",
            ["checkbox_label_border_width"] = "*button_border_width",
            ["checkbox_background_color"] = "*input_background_fill",
            ["checkbox_label_background_fill_selected"] = "*button_primary_background_fill",
            ["checkbox_label_text_color_selected"] = "*button_primary_text_color",
            ["checkbox_label_padding"] = "*spacing_sm",
            ["button_large_padding"] = "*spacing_lg",
            ["button_small_padding"] = "*spacing_sm",
            ["shadow_drop_lg"] = "0 1px 4px 0 rgb(0 0 0 / 0.1)",
            ["block_shadow"] = "none",
            ["block_shadow_dark"] = "*shadow_drop_lg",
            ["block_title_text_weight"] = "600",
            ["block_label_text_weight"] = "600",
            ["block_label_text_size"] = "*text_md"
        });
    }
}

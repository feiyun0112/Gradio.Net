
namespace Gradio.Net.Core.Themes;

public sealed class GlassTheme : Theme
{
    public GlassTheme()
    {
        Name = "glass";
        Stylesheets = new List<string>();

        // Python parity: primary_hue=colors.blue
        Primary50 = "#eff6ff";
        Primary100 = "#dbeafe";
        Primary200 = "#bfdbfe";
        Primary300 = "#93c5fd";
        Primary400 = "#60a5fa";
        Primary500 = "#3b82f6";
        Primary600 = "#2563eb";
        Primary700 = "#1d4ed8";
        Primary800 = "#1e40af";
        Primary900 = "#1e3a8a";
        Primary950 = "#1d3660";

        // Python parity: secondary_hue=neutral_hue=colors.slate
        Secondary50 = "#f8fafc";
        Secondary100 = "#f1f5f9";
        Secondary200 = "#e2e8f0";
        Secondary300 = "#cbd5e1";
        Secondary400 = "#94a3b8";
        Secondary500 = "#64748b";
        Secondary600 = "#475569";
        Secondary700 = "#334155";
        Secondary800 = "#1e293b";
        Secondary900 = "#0f172a";
        Secondary950 = "#0a0f1e";
        Neutral50 = "#f8fafc";
        Neutral100 = "#f1f5f9";
        Neutral200 = "#e2e8f0";
        Neutral300 = "#cbd5e1";
        Neutral400 = "#94a3b8";
        Neutral500 = "#64748b";
        Neutral600 = "#475569";
        Neutral700 = "#334155";
        Neutral800 = "#1e293b";
        Neutral900 = "#0f172a";
        Neutral950 = "#0a0f1e";

        // Python parity: radius_size=sizes.radius_sm
        RadiusXxs = "1px";
        RadiusXs = "1px";
        RadiusSm = "2px";
        RadiusMd = "4px";
        RadiusLg = "6px";
        RadiusXl = "8px";
        RadiusXxl = "12px";

        // Python parity: spacing_size=sizes.spacing_sm
        SpacingXxs = "1px";
        SpacingXs = "1px";
        SpacingSm = "2px";
        SpacingMd = "4px";
        SpacingLg = "6px";
        SpacingXl = "9px";
        SpacingXxl = "12px";

        // Python parity: text_size=sizes.text_sm
        TextXxs = "8px";
        TextXs = "9px";
        TextSm = "11px";
        TextMd = "13px";
        TextLg = "16px";
        TextXl = "20px";
        TextXxl = "24px";

        // Python parity: font=("Optima","Candara","Noto Sans","source-sans-pro","sans-serif")
        Font = "Optima, Candara, 'Noto Sans', source-sans-pro, sans-serif";

        SetVariables(new Dictionary<string, string>
        {
            ["body_background_fill_dark"] = "*secondary_800",
            ["background_fill_secondary_dark"] = "*secondary_800",
            ["block_background_fill_dark"] = "*secondary_800",
            ["button_primary_background_fill"] = "linear-gradient(180deg, *primary_100 0%, *primary_200 30%)",
            ["button_primary_background_fill_hover"] = "linear-gradient(180deg, *primary_100 0%, *primary_200 30%)",
            ["button_primary_background_fill_dark"] = "linear-gradient(180deg, *primary_500 0%, *primary_600 30%)",
            ["button_primary_background_fill_hover_dark"] = "linear-gradient(180deg, *primary_500 0%, *primary_600 30%)",
            ["button_primary_text_color"] = "*body_text_color",
            ["button_primary_border_color"] = "*primary_200",
            ["button_primary_border_color_hover"] = "*button_primary_border_color",
            ["button_secondary_background_fill"] = "linear-gradient(180deg, *secondary_100 0%, *secondary_200 30%)",
            ["button_secondary_background_fill_hover"] = "linear-gradient(180deg, *secondary_100 0%, *secondary_200 30%)",
            ["button_secondary_background_fill_dark"] = "linear-gradient(180deg, *secondary_500 0%, *secondary_600 30%)",
            ["button_secondary_background_fill_hover_dark"] = "linear-gradient(180deg, *secondary_500 0%, *secondary_600 30%)",
            ["checkbox_border_width"] = "0px",
            ["checkbox_border_width_dark"] = "0px",
            ["checkbox_label_background_fill"] = "*button_secondary_background_fill",
            ["checkbox_label_background_fill_dark"] = "*button_secondary_background_fill",
            ["checkbox_label_background_fill_hover"] = "*button_secondary_background_fill_hover",
            ["checkbox_label_background_fill_hover_dark"] = "*button_secondary_background_fill_hover",
            ["checkbox_label_border_width"] = "1px",
            ["checkbox_label_border_width_dark"] = "1px",
            ["checkbox_label_border_color_dark"] = "*secondary_700",
            ["checkbox_background_color_dark"] = "*secondary_400",
            ["button_border_width"] = "1px",
            ["shadow_inset"] = "rgba(0,0,0,0.05) 0px 1px 2px 0px inset",
            ["button_primary_shadow_active"] = "*shadow_inset",
            ["button_secondary_shadow_active"] = "*shadow_inset",
            ["input_background_fill"] = "linear-gradient(0deg, *secondary_100 0%, white 100%)",
            ["input_background_fill_dark"] = "*secondary_600",
            ["input_border_color_focus_dark"] = "*secondary_500",
            ["input_border_width"] = "1px",
            ["input_border_color_dark"] = "*secondary_600",
            ["block_label_text_color"] = "*secondary_500",
            ["block_title_text_color"] = "*secondary_500",
            ["block_label_text_weight"] = "600",
            ["block_title_text_weight"] = "600",
            ["block_label_text_size"] = "*text_md",
            ["block_title_text_size"] = "*text_md",
            ["block_label_background_fill"] = "*secondary_200",
            ["block_label_background_fill_dark"] = "*secondary_700",
            ["block_label_border_color_dark"] = "*secondary_600",
            ["block_border_width"] = "0px",
            ["block_border_width_dark"] = "1px",
            ["block_border_color_dark"] = "*secondary_600",
            ["panel_border_width"] = "1px",
            ["border_color_primary_dark"] = "*secondary_600",
            ["background_fill_primary_dark"] = "*neutral_700",
            ["background_fill_secondary"] = "*secondary_100",
            ["block_background_fill"] = "*secondary_50",
            ["table_even_background_fill_dark"] = "*neutral_700",
            ["table_odd_background_fill_dark"] = "*neutral_700"
        });
    }
}

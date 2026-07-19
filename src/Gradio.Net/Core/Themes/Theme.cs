using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Nodes;

#pragma warning disable CS8618

namespace Gradio.Net.Core;

public class Theme
{
    private readonly Dictionary<string, string> _cssVariables = new();
    private readonly Dictionary<string, string> _darkCssVariables = new();
    private readonly List<string> _fontCss = new();
    private readonly Dictionary<string, string> _customCssVariables = new();
    private readonly Dictionary<string, string> _customDarkCssVariables = new();

    public string Name { get; set; } = "default";

    public virtual List<string> Stylesheets { get; protected set; } = new List<string>
    {
        "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:wght@400;600&display=swap"
    };

    public Theme()
    {
        // Initialize with default theme values
        InitializeDefaultTheme();
    }

    private void InitializeDefaultTheme()
    {
        // Base colors
        Primary50 = "#fff7ed";
        Primary100 = "#ffedd5";
        Primary200 = "#ffddb3";
        Primary300 = "#fdba74";
        Primary400 = "#fb923c";
        Primary500 = "#f97316";
        Primary600 = "#ea580c";
        Primary700 = "#c2410c";
        Primary800 = "#9a3412";
        Primary900 = "#7c2d12";
        Primary950 = "#6c2e12";

        Secondary50 = "#eff6ff";
        Secondary100 = "#dbeafe";
        Secondary200 = "#bfdbfe";
        Secondary300 = "#93c5fd";
        Secondary400 = "#60a5fa";
        Secondary500 = "#3b82f6";
        Secondary600 = "#2563eb";
        Secondary700 = "#1d4ed8";
        Secondary800 = "#1e40af";
        Secondary900 = "#1e3a8a";
        Secondary950 = "#1d3660";

        Neutral50 = "#fafafa";
        Neutral100 = "#f4f4f5";
        Neutral200 = "#e4e4e7";
        Neutral300 = "#d4d4d8";
        Neutral400 = "#bbbbc2";
        Neutral500 = "#71717a";
        Neutral600 = "#52525b";
        Neutral700 = "#3f3f46";
        Neutral800 = "#27272a";
        Neutral900 = "#18181b";
        Neutral950 = "#0f0f11";

        // Body attributes
        BodyBackgroundColor = "#ffffff";
        BodyBackgroundColorDark = "#0f0f11";
        BodyTextColor = "#27272a";
        BodyTextColorDark = "#f4f4f5";
        BodyTextSize = "14px";
        BodyTextColorSubdued = "#71717a";
        BodyTextColorSubduedDark = "#a1a1aa";
        BodyTextWeight = "400";

        // Background fills
        BackgroundFillPrimary = "#ffffff";
        BackgroundFillPrimaryDark = "#1e1e22";
        BackgroundFillSecondary = "#f9fafb";
        BackgroundFillSecondaryDark = "#18181b";

        // Border colors
        BorderColorPrimary = "#e5e7eb";
        BorderColorPrimaryDark = "#3f3f46";
        BorderColorAccent = "#93c5fd";
        BorderColorAccentDark = "#60a5fa";

        // Accent colors
        ColorAccent = "#3b82f6";
        ColorAccentSoft = "#dbeafe";
        ColorAccentSoftDark = "#1e3a8a";

        // Link colors
        LinkTextColor = "#3b82f6";
        LinkTextColorDark = "#60a5fa";
        LinkTextColorHover = "#2563eb";
        LinkTextColorHoverDark = "#3b82f6";

        // Block styles
        BlockBackgroundColor = "#ffffff";
        BlockBackgroundColorDark = "#1e1e22";
        BlockBorderColor = "#e5e7eb";
        BlockBorderColorDark = "#3f3f46";
        BlockBorderWidth = "1px";
        BlockPadding = "16px";
        BlockRadius = "8px";
        BlockShadow = "0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)";
        BlockShadowDark = "0 1px 3px 0 rgba(0, 0, 0, 0.3), 0 1px 2px 0 rgba(0, 0, 0, 0.2)";

        // Button styles
        ButtonPrimaryBackgroundColor = "#3b82f6";
        ButtonPrimaryBackgroundColorDark = "#2563eb";
        ButtonPrimaryBackgroundColorHover = "#2563eb";
        ButtonPrimaryBackgroundColorHoverDark = "#1d4ed8";
        ButtonPrimaryBorderColor = "*primary_500";
        ButtonPrimaryBorderColorDark = "*primary_600";
        ButtonPrimaryBorderColorHover = "*primary_500";
        ButtonPrimaryBorderColorHoverDark = "*primary_500";
        ButtonPrimaryTextColor = "#ffffff";
        ButtonPrimaryTextColorDark = "#ffffff";
        ButtonPrimaryTextColorHover = "*button_primary_text_color";
        ButtonPrimaryTextColorHoverDark = "*button_primary_text_color";
        ButtonPrimaryShadow = "none";
        ButtonPrimaryShadowDark = "none";
        ButtonPrimaryShadowHover = "*button_primary_shadow";
        ButtonPrimaryShadowHoverDark = "*button_primary_shadow";
        ButtonPrimaryShadowActive = "*button_primary_shadow";
        ButtonPrimaryShadowActiveDark = "*button_primary_shadow";

        // Python parity: --button-border-width
        ButtonBorderWidth = "0px";
        ButtonTransformHover = "none";
        ButtonTransformActive = "none";
        ButtonTransition = "all 0.2s ease";
        ButtonLargePadding = "*spacing_lg calc(2 * *spacing_lg)";
        ButtonLargeRadius = "*radius_md";
        ButtonLargeTextSize = "*text_lg";
        ButtonLargeTextWeight = "600";
        ButtonSmallPadding = "*spacing_sm calc(1.5 * *spacing_sm)";
        ButtonSmallRadius = "*radius_md";
        ButtonSmallTextSize = "*text_sm";
        ButtonSmallTextWeight = "400";
        ButtonMediumPadding = "*spacing_md calc(2 * *spacing_md)";
        ButtonMediumRadius = "*radius_md";
        ButtonMediumTextSize = "*text_md";
        ButtonMediumTextWeight = "600";

        ButtonSecondaryBackgroundColor = "#f3f4f6";
        ButtonSecondaryBackgroundColorDark = "#3f3f46";
        ButtonSecondaryBackgroundColorHover = "#e5e7eb";
        ButtonSecondaryBackgroundColorHoverDark = "#52525b";
        ButtonSecondaryBorderColor = "*neutral_200";
        ButtonSecondaryBorderColorDark = "*neutral_600";
        ButtonSecondaryBorderColorHover = "*neutral_200";
        ButtonSecondaryBorderColorHoverDark = "*neutral_500";
        ButtonSecondaryTextColor = "#1f2937";
        ButtonSecondaryTextColorDark = "#f4f4f5";
        ButtonSecondaryTextColorHover = "*button_secondary_text_color";
        ButtonSecondaryTextColorHoverDark = "*button_secondary_text_color";
        ButtonSecondaryShadow = "*button_primary_shadow";
        ButtonSecondaryShadowDark = "*button_primary_shadow";
        ButtonSecondaryShadowHover = "*button_secondary_shadow";
        ButtonSecondaryShadowHoverDark = "*button_secondary_shadow";
        ButtonSecondaryShadowActive = "*button_secondary_shadow";
        ButtonSecondaryShadowActiveDark = "*button_secondary_shadow";

        ButtonCancelBackgroundColor = "#ef4444";
        ButtonCancelBackgroundColorDark = "#b91c1c";
        ButtonCancelBackgroundColorHover = "#dc2626";
        ButtonCancelBackgroundColorHoverDark = "#991b1b";
        ButtonCancelBorderColor = "*button_secondary_border_color";
        ButtonCancelBorderColorDark = "*button_secondary_border_color";
        ButtonCancelBorderColorHover = "*button_secondary_border_color_hover";
        ButtonCancelBorderColorHoverDark = "*button_secondary_border_color_hover";
        ButtonCancelTextColor = "white";
        ButtonCancelTextColorDark = "white";
        ButtonCancelTextColorHover = "white";
        ButtonCancelTextColorHoverDark = "white";
        ButtonCancelShadow = "*button_secondary_shadow";
        ButtonCancelShadowDark = "*button_secondary_shadow";
        ButtonCancelShadowHover = "*button_secondary_shadow_hover";
        ButtonCancelShadowHoverDark = "*button_secondary_shadow_hover";
        ButtonCancelShadowActive = "*button_secondary_shadow_active";
        ButtonCancelShadowActiveDark = "*button_secondary_shadow_active";

        // Input styles
        InputBackgroundColor = "#ffffff";
        InputBackgroundColorDark = "#18181b";
        InputBackgroundColorFocus = "#ffffff";
        InputBackgroundColorFocusDark = "#18181b";
        InputBorderColor = "#d1d5db";
        InputBorderColorDark = "#52525b";
        InputBorderColorFocus = "#3b82f6";
        InputBorderColorFocusDark = "#60a5fa";
        InputPlaceholderColor = "#9ca3af";
        InputPlaceholderColorDark = "#71717a";

        // Error styles
        ErrorBackgroundColor = "#fee2e2";
        ErrorBackgroundColorDark = "#451a1a";
        ErrorBorderColor = "#fca5a5";
        ErrorBorderColorDark = "#dc2626";
        ErrorTextColor = "#dc2626";
        ErrorTextColorDark = "#fca5a5";

        // Fonts
        Font = "'Source Sans Pro', ui-sans-serif, system-ui, sans-serif";
        FontMono = "'IBM Plex Mono', ui-monospace, Consolas, monospace";

        // Spacing
        SpacingXxs = "1px";
        SpacingXs = "2px";
        SpacingSm = "4px";
        SpacingMd = "6px";
        SpacingLg = "8px";
        SpacingXl = "10px";
        SpacingXxl = "16px";

        // Radius
        RadiusXxs = "1px";
        RadiusXs = "2px";
        RadiusSm = "4px";
        RadiusMd = "6px";
        RadiusLg = "8px";
        RadiusXl = "12px";
        RadiusXxl = "22px";

        // Text sizes
        TextXxs = "9px";
        TextXs = "10px";
        TextSm = "12px";
        TextMd = "14px";
        TextLg = "16px";
        TextXl = "22px";
        TextXxl = "26px";

        // Python parity aliases (canonical Gradio CSS variable names)
        _customCssVariables["body-background-fill"] = "var(--background-fill-primary)";
        _customCssVariables["name"] = "default";
        _customCssVariables["embed-radius"] = "var(--radius-sm)";
        _customCssVariables["accordion-text-color"] = "var(--body-text-color)";
        _customCssVariables["table-text-color"] = "var(--body-text-color)";
        _customCssVariables["shadow-drop"] = "rgba(0,0,0,0.05) 0px 1px 2px 0px";
        _customCssVariables["shadow-drop-lg"] = "0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)";
        _customCssVariables["shadow-inset"] = "rgba(0,0,0,0.05) 0px 2px 4px 0px inset";
        _customCssVariables["shadow-spread"] = "3px";
        _customCssVariables["block-background-fill"] = "var(--background-fill-primary)";
        _customCssVariables["block-border-color"] = "var(--border-color-primary)";
        _customCssVariables["block-border-width"] = "1px";
        _customCssVariables["block-info-text-color"] = "var(--body-text-color-subdued)";
        _customCssVariables["block-info-text-size"] = "var(--text-sm)";
        _customCssVariables["block-info-text-weight"] = "400";
        _customCssVariables["block-label-background-fill"] = "var(--background-fill-primary)";
        _customCssVariables["block-label-border-color"] = "var(--border-color-primary)";
        _customCssVariables["block-label-border-width"] = "1px";
        _customCssVariables["block-label-shadow"] = "var(--block-shadow)";
        _customCssVariables["block-label-text-color"] = "var(--neutral-500)";
        _customCssVariables["block-label-margin"] = "0";
        _customCssVariables["block-label-padding"] = "var(--spacing-sm) var(--spacing-lg)";
        _customCssVariables["block-label-radius"] = "calc(var(--radius-sm) - 1px) 0 calc(var(--radius-sm) - 1px) 0";
        _customCssVariables["block-label-right-radius"] = "0 calc(var(--radius-sm) - 1px) 0 calc(var(--radius-sm) - 1px)";
        _customCssVariables["block-label-text-size"] = "var(--text-sm)";
        _customCssVariables["block-label-text-weight"] = "400";
        _customCssVariables["block-padding"] = "var(--spacing-xl) calc(var(--spacing-xl) + 2px)";
        _customCssVariables["block-radius"] = "var(--radius-sm)";
        _customCssVariables["block-shadow"] = "none";
        _customCssVariables["block-title-background-fill"] = "none";
        _customCssVariables["block-title-border-color"] = "none";
        _customCssVariables["block-title-border-width"] = "0px";
        _customCssVariables["block-title-text-color"] = "var(--neutral-500)";
        _customCssVariables["block-title-padding"] = "0";
        _customCssVariables["block-title-radius"] = "none";
        _customCssVariables["block-title-text-size"] = "var(--text-md)";
        _customCssVariables["block-title-text-weight"] = "400";
        _customCssVariables["container-radius"] = "var(--radius-sm)";
        _customCssVariables["form-gap-width"] = "0px";
        _customCssVariables["layout-gap"] = "var(--spacing-xxl)";
        _customCssVariables["panel-background-fill"] = "var(--background-fill-secondary)";
        _customCssVariables["panel-border-color"] = "var(--border-color-primary)";
        _customCssVariables["panel-border-width"] = "0";
        _customCssVariables["section-header-text-size"] = "var(--text-md)";
        _customCssVariables["section-header-text-weight"] = "400";
        _customCssVariables["code-background-fill"] = "var(--neutral-100)";
        _customCssVariables["chatbot-text-size"] = "var(--text-lg)";
        _customCssVariables["checkbox-background-color"] = "var(--background-fill-primary)";
        _customCssVariables["checkbox-background-color-focus"] = "var(--checkbox-background-color)";
        _customCssVariables["checkbox-background-color-hover"] = "var(--checkbox-background-color)";
        _customCssVariables["checkbox-background-color-selected"] = "var(--color-accent)";
        _customCssVariables["checkbox-border-color"] = "var(--neutral-300)";
        _customCssVariables["checkbox-border-color-focus"] = "var(--color-accent)";
        _customCssVariables["checkbox-border-color-hover"] = "var(--neutral-300)";
        _customCssVariables["checkbox-border-color-selected"] = "var(--color-accent)";
        _customCssVariables["checkbox-border-radius"] = "var(--radius-sm)";
        _customCssVariables["checkbox-border-width"] = "var(--input-border-width)";
        _customCssVariables["checkbox-label-background-fill"] = "var(--background-fill-primary)";
        _customCssVariables["checkbox-label-background-fill-hover"] = "var(--background-fill-secondary)";
        _customCssVariables["checkbox-label-background-fill-selected"] = "var(--checkbox-label-background-fill)";
        _customCssVariables["checkbox-label-border-color"] = "var(--border-color-primary)";
        _customCssVariables["checkbox-label-border-color-hover"] = "var(--checkbox-label-border-color)";
        _customCssVariables["checkbox-label-border-color-selected"] = "var(--checkbox-label-border-color)";
        _customCssVariables["checkbox-label-border-width"] = "var(--input-border-width)";
        _customCssVariables["checkbox-label-gap"] = "var(--spacing-lg)";
        _customCssVariables["checkbox-label-padding"] = "var(--spacing-md) calc(2 * var(--spacing-md))";
        _customCssVariables["checkbox-label-shadow"] = "none";
        _customCssVariables["checkbox-label-text-size"] = "var(--text-md)";
        _customCssVariables["checkbox-label-text-weight"] = "400";
        _customCssVariables["checkbox-check"] = "url(\"data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3cpath d='M12.207 4.793a1 1 0 010 1.414l-5 5a1 1 0 01-1.414 0l-2-2a1 1 0 011.414-1.414L6.5 9.086l4.293-4.293a1 1 0 011.414 0z'/%3e%3c/svg%3e\")";
        _customCssVariables["radio-circle"] = "url(\"data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3ccircle cx='8' cy='8' r='3'/%3e%3c/svg%3e\")";
        _customCssVariables["checkbox-shadow"] = "var(--input-shadow)";
        _customCssVariables["checkbox-label-text-color"] = "var(--body-text-color)";
        _customCssVariables["checkbox-label-text-color-selected"] = "var(--checkbox-label-text-color)";
        _customCssVariables["error-background-fill"] = "#fef2f2";
        _customCssVariables["error-border-width"] = "1px";
        _customCssVariables["error-icon-color"] = "#b91c1c";
        _customCssVariables["input-background-fill"] = "white";
        _customCssVariables["input-background-fill-focus"] = "var(--input-background-fill)";
        _customCssVariables["input-background-fill-hover"] = "var(--input-background-fill)";
        _customCssVariables["input-border-color"] = "var(--border-color-primary)";
        _customCssVariables["input-border-color-hover"] = "var(--input-border-color)";
        _customCssVariables["input-border-width"] = "1px";
        _customCssVariables["input-padding"] = "var(--spacing-xl)";
        _customCssVariables["input-radius"] = "var(--radius-sm)";
        _customCssVariables["input-shadow"] = "none";
        _customCssVariables["input-shadow-focus"] = "0 0 0 var(--shadow-spread) var(--secondary-50), var(--shadow-inset)";
        _customCssVariables["input-text-size"] = "var(--text-md)";
        _customCssVariables["input-text-weight"] = "400";
        _customCssVariables["loader-color"] = "var(--color-accent)";
        _customCssVariables["prose-text-size"] = "var(--text-md)";
        _customCssVariables["prose-text-weight"] = "400";
        _customCssVariables["prose-header-text-weight"] = "600";
        _customCssVariables["slider-color"] = "var(--color-accent)";
        _customCssVariables["stat-background-fill"] = "linear-gradient(to right, var(--primary-400), var(--primary-200))";
        _customCssVariables["table-border-color"] = "var(--neutral-300)";
        _customCssVariables["table-even-background-fill"] = "white";
        _customCssVariables["table-odd-background-fill"] = "var(--neutral-50)";
        _customCssVariables["table-radius"] = "var(--radius-sm)";
        _customCssVariables["table-row-focus"] = "var(--color-accent-soft)";
        _customCssVariables["button-primary-background-fill"] = "var(--primary-500)";
        _customCssVariables["button-primary-background-fill-hover"] = "var(--primary-600)";
        _customCssVariables["button-secondary-background-fill"] = "var(--neutral-200)";
        _customCssVariables["button-secondary-background-fill-hover"] = "var(--neutral-300)";
        _customCssVariables["button-cancel-background-fill"] = "#ef4444";
        _customCssVariables["button-cancel-background-fill-hover"] = "#dc2626";
        _customCssVariables["color-accent"] = "var(--primary-500)";
        _customCssVariables["color-accent-soft"] = "var(--primary-50)";
        _customCssVariables["border-color-accent"] = "var(--primary-300)";
        _customCssVariables["border-color-accent-subdued"] = "var(--primary-200)";
        _customCssVariables["link-text-color-active"] = "var(--secondary-600)";
        _customCssVariables["link-text-color-visited"] = "var(--secondary-500)";

        _customDarkCssVariables["body-background-fill"] = "var(--background-fill-primary)";
        _customDarkCssVariables["block-background-fill"] = "var(--neutral-800)";
        _customDarkCssVariables["input-background-fill"] = "var(--neutral-800)";
        _customDarkCssVariables["input-border-width"] = "1px";
        _customDarkCssVariables["button-primary-background-fill"] = "var(--primary-600)";
        _customDarkCssVariables["button-primary-background-fill-hover"] = "var(--primary-700)";
        _customDarkCssVariables["button-secondary-background-fill"] = "var(--neutral-600)";
        _customDarkCssVariables["button-secondary-background-fill-hover"] = "var(--neutral-700)";
    }

    // Primary colors
    public string Primary50 { get; set; }
    public string Primary100 { get; set; }
    public string Primary200 { get; set; }
    public string Primary300 { get; set; }
    public string Primary400 { get; set; }
    public string Primary500 { get; set; }
    public string Primary600 { get; set; }
    public string Primary700 { get; set; }
    public string Primary800 { get; set; }
    public string Primary900 { get; set; }
    public string Primary950 { get; set; }

    // Secondary colors
    public string Secondary50 { get; set; }
    public string Secondary100 { get; set; }
    public string Secondary200 { get; set; }
    public string Secondary300 { get; set; }
    public string Secondary400 { get; set; }
    public string Secondary500 { get; set; }
    public string Secondary600 { get; set; }
    public string Secondary700 { get; set; }
    public string Secondary800 { get; set; }
    public string Secondary900 { get; set; }
    public string Secondary950 { get; set; }

    // Neutral colors
    public string Neutral50 { get; set; }
    public string Neutral100 { get; set; }
    public string Neutral200 { get; set; }
    public string Neutral300 { get; set; }
    public string Neutral400 { get; set; }
    public string Neutral500 { get; set; }
    public string Neutral600 { get; set; }
    public string Neutral700 { get; set; }
    public string Neutral800 { get; set; }
    public string Neutral900 { get; set; }
    public string Neutral950 { get; set; }

    // Body attributes
    public string BodyBackgroundColor { get; set; }
    public string BodyBackgroundColorDark { get; set; }
    public string BodyTextColor { get; set; }
    public string BodyTextColorDark { get; set; }
    public string BodyTextSize { get; set; }
    public string BodyTextColorSubdued { get; set; }
    public string BodyTextColorSubduedDark { get; set; }
    public string BodyTextWeight { get; set; }

    // Background fills
    public string BackgroundFillPrimary { get; set; }
    public string BackgroundFillPrimaryDark { get; set; }
    public string BackgroundFillSecondary { get; set; }
    public string BackgroundFillSecondaryDark { get; set; }

    // Border colors
    public string BorderColorPrimary { get; set; }
    public string BorderColorPrimaryDark { get; set; }
    public string BorderColorAccent { get; set; }
    public string BorderColorAccentDark { get; set; }
    public string BorderColorAccentSubdued { get; set; }
    public string BorderColorAccentSubduedDark { get; set; }

    // Accent colors
    public string ColorAccent { get; set; }
    public string ColorAccentSoft { get; set; }
    public string ColorAccentSoftDark { get; set; }

    // Link colors
    public string LinkTextColor { get; set; }
    public string LinkTextColorDark { get; set; }
    public string LinkTextColorHover { get; set; }
    public string LinkTextColorHoverDark { get; set; }
    public string LinkTextColorActive { get; set; }
    public string LinkTextColorActiveDark { get; set; }
    public string LinkTextColorVisited { get; set; }
    public string LinkTextColorVisitedDark { get; set; }

    // Block styles
    public string BlockBackgroundColor { get; set; }
    public string BlockBackgroundColorDark { get; set; }
    public string BlockBorderColor { get; set; }
    public string BlockBorderColorDark { get; set; }
    public string BlockBorderWidth { get; set; }
    public string BlockBorderWidthDark { get; set; }
    public string BlockPadding { get; set; }
    public string BlockRadius { get; set; }
    public string BlockShadow { get; set; }
    public string BlockShadowDark { get; set; }

    // Input styles
    public string InputBackgroundColor { get; set; }
    public string InputBackgroundColorDark { get; set; }
    public string InputBackgroundColorFocus { get; set; }
    public string InputBackgroundColorFocusDark { get; set; }
    public string InputBackgroundColorHover { get; set; }
    public string InputBackgroundColorHoverDark { get; set; }
    public string InputBorderColor { get; set; }
    public string InputBorderColorDark { get; set; }
    public string InputBorderColorFocus { get; set; }
    public string InputBorderColorFocusDark { get; set; }
    public string InputBorderColorHover { get; set; }
    public string InputBorderColorHoverDark { get; set; }
    public string InputBorderWidth { get; set; }
    public string InputBorderWidthDark { get; set; }
    public string InputPadding { get; set; }
    public string InputPlaceholderColor { get; set; }
    public string InputPlaceholderColorDark { get; set; }
    public string InputRadius { get; set; }
    public string InputShadow { get; set; }
    public string InputShadowDark { get; set; }
    public string InputShadowFocus { get; set; }
    public string InputShadowFocusDark { get; set; }
    public string InputTextSize { get; set; }
    public string InputTextWeight { get; set; }

    // Button styles
    public string ButtonBorderWidth { get; set; }
    public string ButtonBorderWidthDark { get; set; }
    public string ButtonTransformHover { get; set; }
    public string ButtonTransformActive { get; set; }
    public string ButtonTransition { get; set; }
    public string ButtonLargePadding { get; set; }
    public string ButtonLargeRadius { get; set; }
    public string ButtonLargeTextSize { get; set; }
    public string ButtonLargeTextWeight { get; set; }
    public string ButtonSmallPadding { get; set; }
    public string ButtonSmallRadius { get; set; }
    public string ButtonSmallTextSize { get; set; }
    public string ButtonSmallTextWeight { get; set; }
    public string ButtonMediumPadding { get; set; }
    public string ButtonMediumRadius { get; set; }
    public string ButtonMediumTextSize { get; set; }
    public string ButtonMediumTextWeight { get; set; }

    public string ButtonPrimaryBackgroundColor { get; set; }
    public string ButtonPrimaryBackgroundColorDark { get; set; }
    public string ButtonPrimaryBackgroundColorHover { get; set; }
    public string ButtonPrimaryBackgroundColorHoverDark { get; set; }
    public string ButtonPrimaryBorderColor { get; set; }
    public string ButtonPrimaryBorderColorDark { get; set; }
    public string ButtonPrimaryBorderColorHover { get; set; }
    public string ButtonPrimaryBorderColorHoverDark { get; set; }
    public string ButtonPrimaryTextColor { get; set; }
    public string ButtonPrimaryTextColorDark { get; set; }
    public string ButtonPrimaryTextColorHover { get; set; }
    public string ButtonPrimaryTextColorHoverDark { get; set; }
    public string ButtonPrimaryShadow { get; set; }
    public string ButtonPrimaryShadowDark { get; set; }
    public string ButtonPrimaryShadowHover { get; set; }
    public string ButtonPrimaryShadowHoverDark { get; set; }
    public string ButtonPrimaryShadowActive { get; set; }
    public string ButtonPrimaryShadowActiveDark { get; set; }

    public string ButtonSecondaryBackgroundColor { get; set; }
    public string ButtonSecondaryBackgroundColorDark { get; set; }
    public string ButtonSecondaryBackgroundColorHover { get; set; }
    public string ButtonSecondaryBackgroundColorHoverDark { get; set; }
    public string ButtonSecondaryBorderColor { get; set; }
    public string ButtonSecondaryBorderColorDark { get; set; }
    public string ButtonSecondaryBorderColorHover { get; set; }
    public string ButtonSecondaryBorderColorHoverDark { get; set; }
    public string ButtonSecondaryTextColor { get; set; }
    public string ButtonSecondaryTextColorDark { get; set; }
    public string ButtonSecondaryTextColorHover { get; set; }
    public string ButtonSecondaryTextColorHoverDark { get; set; }
    public string ButtonSecondaryShadow { get; set; }
    public string ButtonSecondaryShadowDark { get; set; }
    public string ButtonSecondaryShadowHover { get; set; }
    public string ButtonSecondaryShadowHoverDark { get; set; }
    public string ButtonSecondaryShadowActive { get; set; }
    public string ButtonSecondaryShadowActiveDark { get; set; }

    public string ButtonCancelBackgroundColor { get; set; }
    public string ButtonCancelBackgroundColorDark { get; set; }
    public string ButtonCancelBackgroundColorHover { get; set; }
    public string ButtonCancelBackgroundColorHoverDark { get; set; }
    public string ButtonCancelBorderColor { get; set; }
    public string ButtonCancelBorderColorDark { get; set; }
    public string ButtonCancelBorderColorHover { get; set; }
    public string ButtonCancelBorderColorHoverDark { get; set; }
    public string ButtonCancelTextColor { get; set; }
    public string ButtonCancelTextColorDark { get; set; }
    public string ButtonCancelTextColorHover { get; set; }
    public string ButtonCancelTextColorHoverDark { get; set; }
    public string ButtonCancelShadow { get; set; }
    public string ButtonCancelShadowDark { get; set; }
    public string ButtonCancelShadowHover { get; set; }
    public string ButtonCancelShadowHoverDark { get; set; }
    public string ButtonCancelShadowActive { get; set; }
    public string ButtonCancelShadowActiveDark { get; set; }

    // Error styles
    public string ErrorBackgroundColor { get; set; }
    public string ErrorBackgroundColorDark { get; set; }
    public string ErrorBorderColor { get; set; }
    public string ErrorBorderColorDark { get; set; }
    public string ErrorBorderWidth { get; set; }
    public string ErrorBorderWidthDark { get; set; }
    public string ErrorTextColor { get; set; }
    public string ErrorTextColorDark { get; set; }

    // Fonts
    public string Font { get; set; }
    public string FontMono { get; set; }

    // Spacing
    public string SpacingXxs { get; set; }
    public string SpacingXs { get; set; }
    public string SpacingSm { get; set; }
    public string SpacingMd { get; set; }
    public string SpacingLg { get; set; }
    public string SpacingXl { get; set; }
    public string SpacingXxl { get; set; }

    // Radius
    public string RadiusXxs { get; set; }
    public string RadiusXs { get; set; }
    public string RadiusSm { get; set; }
    public string RadiusMd { get; set; }
    public string RadiusLg { get; set; }
    public string RadiusXl { get; set; }
    public string RadiusXxl { get; set; }

    // Text sizes
    public string TextXxs { get; set; }
    public string TextXs { get; set; }
    public string TextSm { get; set; }
    public string TextMd { get; set; }
    public string TextLg { get; set; }
    public string TextXl { get; set; }
    public string TextXxl { get; set; }

    public string GenerateCss()
    {
        // Clear existing variables
        _cssVariables.Clear();
        _darkCssVariables.Clear();

        // Process all public properties
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.Name == nameof(Name))
                continue;

            var value = prop.GetValue(this) as string;
            if (value == null)
                continue;

            // Replace variable references (like *primary_color) with CSS variables
            var processedValue = ReplaceVariableReferences(value);

            // Convert property name to kebab-case
            var cssPropertyName = ToKebabCase(prop.Name);

            // Check if it's a dark mode property
            if (cssPropertyName.EndsWith("-dark"))
            {
                var basePropertyName = cssPropertyName.Substring(0, cssPropertyName.Length - 5);
                _darkCssVariables[basePropertyName] = processedValue;
            }
            else
            {
                _cssVariables[cssPropertyName] = processedValue;
            }
        }

        // Apply custom overrides from SetVariables()/SetVariable()
        foreach (var (k, v) in _customCssVariables)
        {
            _cssVariables[k] = ReplaceVariableReferences(v);
        }
        foreach (var (k, v) in _customDarkCssVariables)
        {
            _darkCssVariables[k] = ReplaceVariableReferences(v);
        }

        // Ensure all light variables have a dark counterpart
        foreach (var (propertyName, value) in _cssVariables)
        {
            if (!_darkCssVariables.ContainsKey(propertyName))
            {
                _darkCssVariables[propertyName] = value;
            }
        }

        // Generate CSS code
        var fontCss = string.Join("\n", _fontCss);

        var cssCode = ":root {\n";
        foreach (var (propertyName, value) in _cssVariables)
        {
            cssCode += $"  --{propertyName}: {value};\n";
        }
        cssCode += "}\n";

        var darkCssCode = ":root.dark, :root .dark {\n";
        foreach (var (propertyName, value) in _darkCssVariables)
        {
            darkCssCode += $"  --{propertyName}: {value};\n";
        }
        darkCssCode += "}\n";

        return $"{fontCss}\n{cssCode}\n{darkCssCode}";
    }

    public Theme SetVariable(string key, string value)
    {
        var cssKey = key.Replace("_", "-");
        if (cssKey.EndsWith("-dark", StringComparison.OrdinalIgnoreCase))
        {
            _customDarkCssVariables[cssKey[..^5]] = value;
        }
        else
        {
            _customCssVariables[cssKey] = value;
        }
        return this;
    }

    public Theme SetVariables(Dictionary<string, string> variables)
    {
        foreach (var kv in variables)
        {
            SetVariable(kv.Key, kv.Value);
        }
        return this;
    }

    public string GetThemeCss()
    {
        return GenerateCss();
    }

    public string GetComputedValue(string property, int depth = 0)
    {
        const int MaxDepth = 100;
        if (depth > MaxDepth)
        {
            return string.Empty;
        }

        var prop = GetType().GetProperty(property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var setValue = prop?.GetValue(this)?.ToString() ?? string.Empty;
        var pattern = new Regex(@"\*(\w+)\b");
        var computedValue = pattern.Replace(setValue, m => GetComputedValue(m.Groups[1].Value, depth + 1));
        return computedValue;
    }

    public Dictionary<string, Dictionary<string, string>> ToDictionary()
    {
        var theme = new Dictionary<string, string>();
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (prop.PropertyType != typeof(string))
            {
                continue;
            }

            var val = prop.GetValue(this) as string;
            if (val != null)
            {
                theme[prop.Name] = val;
            }
        }

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["theme"] = theme
        };
    }

    public static Theme FromDictionary(Dictionary<string, Dictionary<string, string>> schema)
    {
        var theme = new Theme();
        if (!schema.TryGetValue("theme", out var props))
        {
            return theme;
        }

        foreach (var kv in props)
        {
            var pascalKey = SnakeToPascal(kv.Key);
            var prop = theme.GetType().GetProperty(pascalKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
            {
                prop.SetValue(theme, kv.Value);
            }
            else
            {
                // Store as custom CSS variable
                var cssKey = kv.Key.Replace("_", "-");
                if (cssKey.EndsWith("-dark"))
                {
                    theme._customDarkCssVariables[cssKey[..^5]] = kv.Value;
                }
                else
                {
                    theme._customCssVariables[cssKey] = kv.Value;
                }
            }
        }

        return theme;
    }

    public static Theme Load(string path)
    {
        var json = File.ReadAllText(path);
        var node = JsonNode.Parse(json) as JsonObject;
        if (node == null)
        {
            return new Theme();
        }

        if (node["theme"] is not JsonObject themeObj)
        {
            return new Theme();
        }

        var theme = new Theme();

        // Handle _stylesheets
        if (themeObj["_stylesheets"] is JsonArray stylesheetsArr)
        {
            theme.Stylesheets = new List<string>();
            foreach (var item in stylesheetsArr)
            {
                if (item != null)
                    theme.Stylesheets.Add(item.ToString());
            }
        }

        // Handle _font → build Font string
        if (themeObj["_font"] is JsonArray fontArr)
        {
            var fontNames = new List<string>();
            foreach (var fontNode in fontArr)
            {
                if (fontNode is JsonObject fontObj && fontObj["name"] is JsonNode nameNode)
                {
                    var name = nameNode.ToString();
                    fontNames.Add(name.Contains(' ') ? $"'{name}'" : name);
                }
            }
            if (fontNames.Count > 0)
                theme.Font = string.Join(", ", fontNames);
        }

        // Handle _font_mono → build FontMono string
        if (themeObj["_font_mono"] is JsonArray fontMonoArr)
        {
            var fontNames = new List<string>();
            foreach (var fontNode in fontMonoArr)
            {
                if (fontNode is JsonObject fontObj && fontObj["name"] is JsonNode nameNode)
                {
                    var name = nameNode.ToString();
                    fontNames.Add(name.Contains(' ') ? $"'{name}'" : name);
                }
            }
            if (fontNames.Count > 0)
                theme.FontMono = string.Join(", ", fontNames);
        }

        // Process all string-valued properties
        foreach (var kv in themeObj)
        {
            if (kv.Key.StartsWith("_")) continue; // skip _font, _font_mono, _stylesheets
            if (kv.Value is not JsonValue) continue; // skip arrays/objects

            var value = kv.Value.ToString();
            var pascalKey = SnakeToPascal(kv.Key);

            var prop = theme.GetType().GetProperty(pascalKey,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
            {
                prop.SetValue(theme, value);
            }
            else
            {
                // Store as custom CSS variable (converts snake_case to kebab-case)
                var cssKey = kv.Key.Replace("_", "-");
                if (cssKey.EndsWith("-dark"))
                {
                    theme._customDarkCssVariables[cssKey[..^5]] = value;
                }
                else
                {
                    theme._customCssVariables[cssKey] = value;
                }
            }
        }

        return theme;
    }

    private static string SnakeToPascal(string snakeCase)
    {
        if (string.IsNullOrEmpty(snakeCase)) return snakeCase;

        var parts = snakeCase.Split('_');
        var sb = new System.Text.StringBuilder();
        foreach (var part in parts)
        {
            if (part.Length > 0)
            {
                if (char.IsLetter(part[0]))
                {
                    sb.Append(char.ToUpper(part[0]));
                    sb.Append(part.Substring(1));
                }
                else
                {
                    sb.Append(part);
                }
            }
        }
        return sb.ToString();
    }

    public void Dump(string filename)
    {
        var json = JsonSerializer.Serialize(ToDictionary(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filename, json);
    }

    public string PushToHub(
        string repoName,
        string? orgName = null,
        string? version = null,
        string? token = null,
        string? themeName = null,
        string? description = null,
        bool isPrivate = false)
    {
        var owner = string.IsNullOrWhiteSpace(orgName) ? "user" : orgName;
        var _ = token; // placeholder for future integration
        var __ = themeName;
        var ___ = description;
        var ____ = isPrivate;
        var resolvedVersion = string.IsNullOrWhiteSpace(version) ? "0.0.1" : version;
        return $"https://huggingface.co/spaces/{owner}/{repoName}?version={resolvedVersion}";
    }

    private string ReplaceVariableReferences(string value)
    {
        // Pattern to match *variable_name references
        var pattern = new Regex(@"\*(\w+)\b");

        return pattern.Replace(value, match =>
        {
            var variableName = match.Groups[1].Value;
            var kebabCaseName = ToKebabCase(variableName);
            return $"var(--{kebabCaseName})";
        });
    }

    private string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input.Replace("_", "-");

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            var current = input[i];
            var previous = input[i - 1];

            if (current == '-')
            {
                result.Append(current);
                continue;
            }

            if (char.IsUpper(current))
            {
                result.Append('-');
                result.Append(char.ToLower(current));
            }
            else if (char.IsDigit(current) && !char.IsDigit(previous) && previous != '-')
            {
                result.Append('-');
                result.Append(current);
            }
            else
            {
                result.Append(current);
            }
        }

        return result.ToString();
    }
}

#pragma warning restore CS8618

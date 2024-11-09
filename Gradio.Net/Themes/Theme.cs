using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Gradio.Net.Themes
{
    public sealed class Theme
    {
        public Theme Set(Dictionary<string, string> overrideStyles = null, Color? primaryHue = null,
            Color? secondaryHue = null,
            Color? neutralHue = null,
            Size? textSize = null,
            Size? spacingSize = null,
            Size? radiusSize = null,
            IEnumerable<Font> fonts = null,
            IEnumerable<Font> fontMonos = null)
        {
            Theme originTheme = this;
            if (overrideStyles == null)
            {
                overrideStyles = new Dictionary<string, string>();
            }
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in properties)
            {
                if (!overrideStyles.ContainsKey(prop.Name))
                {
                    object propValue = prop.GetValue(originTheme);
                    overrideStyles.Add(prop.Name, propValue != null ? propValue.ToString() : null);
                }
            }

            return new Theme(originTheme.Name,
              overrideStyles,
            primaryHue ?? originTheme.PrimaryHue,
           secondaryHue ?? originTheme.SecondaryHue,
             neutralHue ?? originTheme.NeutralHue,
             textSize ?? originTheme.TextSize,
             spacingSize ?? originTheme.SpacingSize,
             radiusSize ?? originTheme.RadiusSize,
             fonts ?? originTheme.Fonts,
             fontMonos ?? originTheme.FontMonos);
        }
        public Color PrimaryHue { get; init; }
        public Color SecondaryHue { get; init; }
        public Color NeutralHue { get; init; }
        public Size TextSize { get; init; }
        public Size SpacingSize { get; init; }
        public Size RadiusSize { get; init; }
        public IEnumerable<Font> Fonts { get; init; }
        public IEnumerable<Font> FontMonos { get; init; }

        public string Primary_50 { get; init; }
        public string Primary_100 { get; init; }
        public string Primary_200 { get; init; }
        public string Primary_300 { get; init; }
        public string Primary_400 { get; init; }
        public string Primary_500 { get; init; }
        public string Primary_600 { get; init; }
        public string Primary_700 { get; init; }
        public string Primary_800 { get; init; }
        public string Primary_900 { get; init; }
        public string Primary_950 { get; init; }

        public string Secondary_50 { get; init; }
        public string Secondary_100 { get; init; }
        public string Secondary_200 { get; init; }
        public string Secondary_300 { get; init; }
        public string Secondary_400 { get; init; }
        public string Secondary_500 { get; init; }
        public string Secondary_600 { get; init; }
        public string Secondary_700 { get; init; }
        public string Secondary_800 { get; init; }
        public string Secondary_900 { get; init; }
        public string Secondary_950 { get; init; }

        public string Neutral_50 { get; init; }
        public string Neutral_100 { get; init; }
        public string Neutral_200 { get; init; }
        public string Neutral_300 { get; init; }
        public string Neutral_400 { get; init; }
        public string Neutral_500 { get; init; }
        public string Neutral_600 { get; init; }
        public string Neutral_700 { get; init; }
        public string Neutral_800 { get; init; }
        public string Neutral_900 { get; init; }
        public string Neutral_950 { get; init; }

        public string SpacingXxs { get; init; }
        public string SpacingXs { get; init; }
        public string SpacingSm { get; init; }
        public string SpacingMd { get; init; }
        public string SpacingLg { get; init; }
        public string SpacingXl { get; init; }
        public string SpacingXxl { get; init; }

        public string RadiusXxs { get; init; }
        public string RadiusXs { get; init; }
        public string RadiusSm { get; init; }
        public string RadiusMd { get; init; }
        public string RadiusLg { get; init; }
        public string RadiusXl { get; init; }
        public string RadiusXxl { get; init; }

        public string TextXxs { get; init; }
        public string TextXs { get; init; }
        public string TextSm { get; init; }
        public string TextMd { get; init; }
        public string TextLg { get; init; }
        public string TextXl { get; init; }
        public string TextXxl { get; init; }



        public Theme(string name,
            Dictionary<string, string> overrideStyles,
            Color? primaryHue = null,
            Color? secondaryHue = null,
            Color? neutralHue = null,
            Size? textSize = null,
            Size? spacingSize = null,
            Size? radiusSize = null,
            IEnumerable<Font> fonts = null,
            IEnumerable<Font> fontMonos = null)
        {
            Name = name;

            primaryHue ??= Colors.Blue;
            secondaryHue ??= Colors.Blue;
            neutralHue ??= Colors.Zinc;
            textSize ??= Sizes.TextMd;
            spacingSize ??= Sizes.SpacingMd;
            radiusSize ??= Sizes.RadiusMd;
            fonts ??= new List<Font> { new LocalFont("IBM Plex Sans"), new LocalFont("ui-sans-serif"), new LocalFont("system-ui"), new LocalFont("sans-serif") };
            fontMonos ??= new List<Font> { new LocalFont("IBM Plex Mono"), new LocalFont("ui-monospace"), new LocalFont("Consolas"), new LocalFont("monospace") };

            PrimaryHue = primaryHue;
            SecondaryHue = secondaryHue;
            NeutralHue = neutralHue;
            TextSize = textSize;
            SpacingSize = spacingSize;
            RadiusSize = radiusSize;

            Fonts = fonts;
            FontMonos = fontMonos;

            Primary_50 = PrimaryHue.C50;
            Primary_100 = PrimaryHue.C100;
            Primary_200 = PrimaryHue.C200;
            Primary_300 = PrimaryHue.C300;
            Primary_400 = PrimaryHue.C400;
            Primary_500 = PrimaryHue.C500;
            Primary_600 = PrimaryHue.C600;
            Primary_700 = PrimaryHue.C700;
            Primary_800 = PrimaryHue.C800;
            Primary_900 = PrimaryHue.C900;
            Primary_950 = PrimaryHue.C950;

            Secondary_50 = SecondaryHue.C50;
            Secondary_100 = SecondaryHue.C100;
            Secondary_200 = SecondaryHue.C200;
            Secondary_300 = SecondaryHue.C300;
            Secondary_400 = SecondaryHue.C400;
            Secondary_500 = SecondaryHue.C500;
            Secondary_600 = SecondaryHue.C600;
            Secondary_700 = SecondaryHue.C700;
            Secondary_800 = SecondaryHue.C800;
            Secondary_900 = SecondaryHue.C900;
            Secondary_950 = SecondaryHue.C950;

            Neutral_50 = NeutralHue.C50;
            Neutral_100 = NeutralHue.C100;
            Neutral_200 = NeutralHue.C200;
            Neutral_300 = NeutralHue.C300;
            Neutral_400 = NeutralHue.C400;
            Neutral_500 = NeutralHue.C500;
            Neutral_600 = NeutralHue.C600;
            Neutral_700 = NeutralHue.C700;
            Neutral_800 = NeutralHue.C800;
            Neutral_900 = NeutralHue.C900;
            Neutral_950 = NeutralHue.C950;

            SpacingXxs = SpacingSize.Xxs;
            SpacingXs = SpacingSize.Xs;
            SpacingSm = SpacingSize.Sm;
            SpacingMd = SpacingSize.Md;
            SpacingLg = SpacingSize.Lg;
            SpacingXl = SpacingSize.Xl;
            SpacingXxl = SpacingSize.Xxl;

            RadiusXxs = RadiusSize.Xxs;
            RadiusXs = RadiusSize.Xs;
            RadiusSm = RadiusSize.Sm;
            RadiusMd = RadiusSize.Md;
            RadiusLg = RadiusSize.Lg;
            RadiusXl = RadiusSize.Xl;
            RadiusXxl = RadiusSize.Xxl;

            TextXxs = TextSize.Xxs;
            TextXs = TextSize.Xs;
            TextSm = TextSize.Sm;
            TextMd = TextSize.Md;
            TextLg = TextSize.Lg;
            TextXl = TextSize.Xl;
            TextXxl = TextSize.Xxl;

            SetPropertyValues(overrideStyles);


        }

        List<string> _stylesheets = null;
        public List<string> GetStylesheets()
        {
            if (_stylesheets is null)
            {
                _stylesheets = new List<string>();
                foreach (var font in Fonts.Concat(FontMonos))
                {
                    var fontStylesheet = font.Stylesheet();
                    if (fontStylesheet.ContainsKey("url") && fontStylesheet["url"] != null)
                    {
                        _stylesheets.Add(fontStylesheet["url"]);
                    }

                }
            }

            return _stylesheets;
        }


        private static readonly int MaxDepth = 100;

        public string GetComputedValue(string property, int depth = 0)
        {
            if (depth > MaxDepth)
            {
                Console.WriteLine($"Cannot resolve '{property}' - circular reference detected.");
                return "";
            }

            bool isDark = property.EndsWith("_dark");
            string propertyName = property;
            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName.ToCamelCase(), BindingFlags.Public | BindingFlags.Instance);
            string setValue = propertyInfo?.GetValue(this)?.ToString() ?? "";
            if (propertyInfo == null && isDark)
            {
                propertyName = property.Replace("_dark", "");
                propertyInfo = this.GetType().GetProperty(propertyName.ToCamelCase(), BindingFlags.Public | BindingFlags.Instance);
                setValue = propertyInfo?.GetValue(this)?.ToString() ?? "";
            }

            string pattern = @"(\*)([\w_]+)(\b)";
            Regex regex = new Regex(pattern);

            string computedValue = regex.Replace(setValue, match =>
            {
                string word = match.Groups[2].Value;
                string darkSuffix = isDark ? "_dark" : "";
                string resolvedProperty = word + darkSuffix;
                return GetComputedValue(resolvedProperty, depth + 1);
            });

            return computedValue;
        }


        private Dictionary<string, string> _css = new Dictionary<string, string>();
        private Dictionary<string, string> _darkCss = new Dictionary<string, string>();

        private string _cssContent = null;
        public string GetCss()
        {
            if (_cssContent == null)
            {
                var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in properties)
                {
                    if (prop.PropertyType != typeof(string))
                    {
                        continue;
                    }
                    var attr = prop.Name.ToSnakeCase().Replace("_", "-");

                    var val = prop.GetValue(this)?.ToString();

                    if (string.IsNullOrEmpty(attr) || string.IsNullOrEmpty(val))
                    {
                        continue;
                    }

                    if (attr.EndsWith("-dark"))
                    {
                        attr = attr.Substring(0, attr.Length - 5);
                        if (val == null)
                        {
                            _darkCss[attr] = null;
                        }
                        else
                        {
                            _darkCss[attr] = ProcessValue(val, attr);
                        }
                    }
                    else
                    {
                        if (val == null)
                        {
                            throw new ArgumentException($"Cannot set '{prop.Name}' to null - only dark mode properties can be null.");
                        }
                        _css[attr] = ProcessValue(val, attr);
                    }
                }

                // Copy missing dark mode properties from light mode  
                foreach (var kvp in _css)
                {
                    if (!_darkCss.ContainsKey(kvp.Key))
                    {
                        _darkCss[kvp.Key] = kvp.Value;
                    }
                }

                string cssCode = ":root {\n" + string.Join("\n", _css.Select(kvp => $"  --{kvp.Key}: {kvp.Value};")) + "\n}";
                string darkCssCode = ":root .dark {\n" + string.Join("\n", _darkCss.Select(kvp => $"  --{kvp.Key}: {(kvp.Value == null ? "initial" : kvp.Value)};")) + "\n}";
                List<string> font_css = new List<string>();
                foreach (var font in Fonts.Concat(FontMonos))
                {
                    var fontStylesheet = font.Stylesheet();

                    if (fontStylesheet.ContainsKey("css") && fontStylesheet["css"] != null)
                    {
                        font_css.Add(fontStylesheet["css"]);
                    }
                }
                string fontCss = string.Join("\n", font_css);

                _cssContent = $"{fontCss}\n{cssCode}\n{darkCssCode}";
            }
            return _cssContent;
        }

        private string ProcessValue(string value, string attr)
        {
            string pattern = @"(\*)([\w_]+)(\b)";
            var matches = Regex.Matches(value, pattern);

            foreach (Match match in matches)
            {
                var fullMatch = match.Value;
                var word = match.Groups[2].Value.Replace("_", "-");

                if (fullMatch.StartsWith("*") && fullMatch.EndsWith("_dark"))
                {
                    throw new ArgumentException($"Cannot refer '{attr}' to '{value}' - dark variable references are automatically used for dark mode attributes, so do not use the _dark suffix in the value.");
                }

                if (attr.EndsWith("-dark") && fullMatch.StartsWith("*") && attr.Substring(0, attr.Length - 5) == fullMatch.Substring(1, fullMatch.Length - 6))
                {
                    throw new ArgumentException($"Cannot refer '{attr}' to '{value}' - if dark and light mode values are the same, set dark mode version to null.");
                }

                value = value.Replace(fullMatch, $"var(--{word})");
            }

            return value;
        }

        public string BodyBackgroundFill { get; init; }
        public string BodyBackgroundFillDark { get; init; }
        public string BodyTextColor { get; init; }
        public string BodyTextColorDark { get; init; }
        public string BodyTextSize { get; init; }
        public string BodyTextWeight { get; init; }
        public string EmbedRadius { get; init; }
        public string ColorAccent { get; init; }
        public string ColorAccentSoft { get; init; }
        public string ColorAccentSoftDark { get; init; }
        public string BorderColorAccent { get; init; }
        public string BorderColorAccentDark { get; init; }
        public string BorderColorPrimary { get; init; }
        public string BorderColorPrimaryDark { get; init; }
        public string LinkTextColor { get; init; }
        public string LinkTextColorActive { get; init; }
        public string LinkTextColorActiveDark { get; init; }
        public string LinkTextColorDark { get; init; }
        public string LinkTextColorHover { get; init; }
        public string LinkTextColorHoverDark { get; init; }
        public string LinkTextColorVisited { get; init; }
        public string LinkTextColorVisitedDark { get; init; }
        public string BodyTextColorSubdued { get; init; }
        public string BodyTextColorSubduedDark { get; init; }
        public string AccordionTextColor { get; init; }
        public string AccordionTextColorDark { get; init; }
        public string TableTextColor { get; init; }
        public string TableTextColorDark { get; init; }
        public string ShadowDrop { get; init; }
        public string ShadowInset { get; init; }
        public string ShadowSpread { get; init; }
        public string ShadowSpreadDark { get; init; }
        public string BlockBackgroundFill { get; init; }
        public string BlockBackgroundFillDark { get; init; }
        public string BlockBorderColor { get; init; }
        public string BlockBorderColorDark { get; init; }
        public string BlockBorderWidth { get; init; }
        public string BlockInfoTextColor { get; init; }
        public string BlockInfoTextColorDark { get; init; }
        public string BlockInfoTextSize { get; init; }
        public string BlockInfoTextWeight { get; init; }
        public string BlockLabelBackgroundFill { get; init; }
        public string BlockLabelBorderColor { get; init; }
        public string BlockLabelBorderColorDark { get; init; }
        public string BlockLabelBorderWidth { get; init; }
        public string BlockLabelShadow { get; init; }
        public string BlockLabelTextColor { get; init; }
        public string BlockLabelTextColorDark { get; init; }
        public string BlockLabelMargin { get; init; }
        public string BlockLabelPadding { get; init; }
        public string BlockLabelTextSize { get; init; }
        public string BlockLabelTextWeight { get; init; }
        public string BlockPadding { get; init; }
        public string BlockRadius { get; init; }
        public string BlockShadow { get; init; }
        public string BlockTitleBackgroundFill { get; init; }
        public string BlockTitleBorderColor { get; init; }
        public string BlockTitleBorderWidth { get; init; }
        public string BlockTitleTextColor { get; init; }
        public string BlockTitleTextColorDark { get; init; }
        public string BlockTitlePadding { get; init; }
        public string BlockTitleRadius { get; init; }
        public string BlockTitleTextSize { get; init; }
        public string BlockTitleTextWeight { get; init; }
        public string ContainerRadius { get; init; }
        public string FormGapWidth { get; init; }
        public string LayoutGap { get; init; }
        public string PanelBackgroundFill { get; init; }
        public string PanelBackgroundFillDark { get; init; }
        public string PanelBorderColor { get; init; }
        public string PanelBorderColorDark { get; init; }
        public string PanelBorderWidth { get; init; }
        public string SectionHeaderTextSize { get; init; }
        public string SectionHeaderTextWeight { get; init; }
        public string BorderColorAccentSubdued { get; init; }
        public string CodeBackgroundFill { get; init; }
        public string CodeBackgroundFillDark { get; init; }
        public string ChatbotTextSize { get; init; }
        public string CheckboxBackgroundColor { get; init; }
        public string CheckboxBackgroundColorDark { get; init; }
        public string CheckboxBorderColor { get; init; }
        public string CheckboxBorderColorDark { get; init; }
        public string CheckboxBorderColorFocus { get; init; }
        public string CheckboxBorderColorHover { get; init; }
        public string CheckboxBorderColorSelected { get; init; }
        public string CheckboxBorderRadius { get; init; }
        public string CheckboxBorderWidth { get; init; }
        public string CheckboxBorderWidthDark { get; init; }
        public string CheckboxLabelBackgroundFill { get; init; }
        public string CheckboxLabelBorderColor { get; init; }
        public string CheckboxLabelBorderWidth { get; init; }
        public string CheckboxLabelGap { get; init; }
        public string CheckboxLabelPadding { get; init; }
        public string CheckboxLabelShadow { get; init; }
        public string CheckboxLabelTextSize { get; init; }
        public string CheckboxLabelTextWeight { get; init; }
        public string CheckboxShadow { get; init; }
        public string CheckboxLabelTextColor { get; init; }
        public string CheckboxLabelTextColorDark { get; init; }
        public string ErrorBackgroundFillDark { get; init; }
        public string ErrorBorderWidth { get; init; }
        public string InputBackgroundFill { get; init; }
        public string InputBackgroundFillDark { get; init; }
        public string InputBackgroundFillFocus { get; init; }
        public string InputBackgroundFillHover { get; init; }
        public string InputBorderColor { get; init; }
        public string InputBorderColorDark { get; init; }
        public string InputBorderColorFocus { get; init; }
        public string InputBorderColorFocusDark { get; init; }
        public string InputBorderColorHover { get; init; }
        public string InputBorderColorHoverDark { get; init; }
        public string InputBorderWidth { get; init; }
        public string InputPadding { get; init; }
        public string InputPlaceholderColor { get; init; }
        public string InputPlaceholderColorDark { get; init; }
        public string InputRadius { get; init; }
        public string InputShadow { get; init; }
        public string InputShadowFocus { get; init; }
        public string InputTextSize { get; init; }
        public string InputTextWeight { get; init; }
        public string LoaderColor { get; init; }
        public string ProseTextSize { get; init; }
        public string ProseTextWeight { get; init; }
        public string ProseHeaderTextWeight { get; init; }
        public string SliderColor { get; init; }
        public string StatBackgroundFill { get; init; }
        public string StatBackgroundFillDark { get; init; }
        public string TableBorderColor { get; init; }
        public string TableBorderColorDark { get; init; }
        public string TableEvenBackgroundFill { get; init; }
        public string TableOddBackgroundFill { get; init; }
        public string TableOddBackgroundFillDark { get; init; }
        public string TableRadius { get; init; }
        public string TableRowFocus { get; init; }
        public string TableRowFocusDark { get; init; }
        public string ButtonBorderWidth { get; init; }
        public string ButtonCancelBackgroundFill { get; init; }
        public string ButtonCancelBorderColor { get; init; }
        public string ButtonCancelTextColor { get; init; }
        public string ButtonCancelTextColorDark { get; init; }
        public string ButtonCancelTextColorHover { get; init; }
        public string ButtonTransformHover { get; init; }
        public string ButtonTransformActive { get; init; }
        public string ButtonTransition { get; init; }
        public string ButtonLargePadding { get; init; }
        public string ButtonLargeRadius { get; init; }
        public string ButtonLargeTextSize { get; init; }
        public string ButtonLargeTextWeight { get; init; }
        public string ButtonPrimaryBackgroundFill { get; init; }
        public string ButtonPrimaryBorderColor { get; init; }
        public string ButtonPrimaryTextColor { get; init; }
        public string ButtonPrimaryTextColorDark { get; init; }
        public string ButtonPrimaryShadow { get; init; }
        public string ButtonPrimaryShadowHover { get; init; }
        public string ButtonPrimaryShadowActive { get; init; }
        public string ButtonSecondaryBorderColor { get; init; }
        public string ButtonSecondaryTextColor { get; init; }
        public string ButtonSecondaryShadow { get; init; }
        public string ButtonSecondaryShadowHover { get; init; }
        public string ButtonSecondaryShadowActive { get; init; }
        public string ButtonSmallPadding { get; init; }
        public string ButtonSmallRadius { get; init; }
        public string ButtonSmallTextSize { get; init; }
        public string ButtonSmallTextWeight { get; init; }
        public string ShadowDropLg { get; init; }
        public string BlockBorderWidthDark { get; init; }
        public string BlockLabelBorderWidthDark { get; init; }
        public string BlockLabelRadius { get; init; }
        public string BlockLabelRightRadius { get; init; }
        public string BlockShadowDark { get; init; }
        public string BlockTitleBackgroundFillDark { get; init; }
        public string BlockTitleBorderColorDark { get; init; }
        public string BlockTitleBorderWidthDark { get; init; }
        public string PanelBorderWidthDark { get; init; }
        public string CheckboxBackgroundColorFocus { get; init; }
        public string CheckboxBackgroundColorHover { get; init; }
        public string CheckboxBackgroundColorHoverDark { get; init; }
        public string CheckboxBackgroundColorSelected { get; init; }
        public string CheckboxBackgroundColorSelectedDark { get; init; }
        public string CheckboxBorderColorFocusDark { get; init; }
        public string CheckboxBorderColorHoverDark { get; init; }
        public string CheckboxBorderColorSelectedDark { get; init; }
        public string CheckboxLabelBackgroundFillDark { get; init; }
        public string CheckboxLabelBackgroundFillHover { get; init; }
        public string CheckboxLabelBackgroundFillHoverDark { get; init; }
        public string CheckboxLabelBackgroundFillSelected { get; init; }
        public string CheckboxLabelBackgroundFillSelectedDark { get; init; }
        public string CheckboxLabelBorderColorDark { get; init; }
        public string CheckboxLabelBorderColorHover { get; init; }
        public string CheckboxLabelBorderColorHoverDark { get; init; }
        public string CheckboxLabelBorderColorSelected { get; init; }
        public string CheckboxLabelBorderColorSelectedDark { get; init; }
        public string CheckboxLabelBorderWidthDark { get; init; }
        public string CheckboxCheck { get; init; }
        public string RadioCircle { get; init; }
        public string CheckboxLabelTextColorSelected { get; init; }
        public string CheckboxLabelTextColorSelectedDark { get; init; }
        public string ErrorBackgroundFill { get; init; }
        public string ErrorBorderColor { get; init; }
        public string ErrorBorderColorDark { get; init; }
        public string ErrorBorderWidthDark { get; init; }
        public string ErrorTextColor { get; init; }
        public string ErrorTextColorDark { get; init; }
        public string ErrorIconColor { get; init; }
        public string ErrorIconColorDark { get; init; }
        public string InputBackgroundFillFocusDark { get; init; }
        public string InputBackgroundFillHoverDark { get; init; }
        public string InputBorderWidthDark { get; init; }
        public string InputShadowDark { get; init; }
        public string InputShadowFocusDark { get; init; }
        public string LoaderColorDark { get; init; }
        public string SliderColorDark { get; init; }
        public string TableEvenBackgroundFillDark { get; init; }
        public string ButtonBorderWidthDark { get; init; }
        public string ButtonCancelBackgroundFillDark { get; init; }
        public string ButtonCancelBackgroundFillHover { get; init; }
        public string ButtonCancelBackgroundFillHoverDark { get; init; }
        public string ButtonCancelBorderColorDark { get; init; }
        public string ButtonCancelBorderColorHover { get; init; }
        public string ButtonCancelBorderColorHoverDark { get; init; }
        public string ButtonCancelTextColorHoverDark { get; init; }
        public string ButtonPrimaryBackgroundFillDark { get; init; }
        public string ButtonPrimaryBackgroundFillHover { get; init; }
        public string ButtonPrimaryBackgroundFillHoverDark { get; init; }
        public string ButtonPrimaryBorderColorDark { get; init; }
        public string ButtonPrimaryBorderColorHover { get; init; }
        public string ButtonPrimaryBorderColorHoverDark { get; init; }
        public string ButtonPrimaryTextColorHover { get; init; }
        public string ButtonPrimaryTextColorHoverDark { get; init; }
        public string ButtonPrimaryShadowDark { get; init; }
        public string ButtonPrimaryShadowHoverDark { get; init; }
        public string ButtonPrimaryShadowActiveDark { get; init; }
        public string ButtonSecondaryBackgroundFill { get; init; }
        public string ButtonSecondaryBackgroundFillDark { get; init; }
        public string ButtonSecondaryBackgroundFillHover { get; init; }
        public string ButtonSecondaryBackgroundFillHoverDark { get; init; }
        public string ButtonSecondaryBorderColorDark { get; init; }
        public string ButtonSecondaryBorderColorHover { get; init; }
        public string ButtonSecondaryBorderColorHoverDark { get; init; }
        public string ButtonSecondaryTextColorDark { get; init; }
        public string ButtonSecondaryTextColorHover { get; init; }
        public string ButtonSecondaryTextColorHoverDark { get; init; }
        public string ButtonSecondaryShadowDark { get; init; }
        public string ButtonSecondaryShadowHoverDark { get; init; }
        public string ButtonSecondaryShadowActiveDark { get; init; }
        public string BackgroundFillPrimary { get; init; }
        public string BackgroundFillPrimaryDark { get; init; }
        public string BackgroundFillSecondary { get; init; }
        public string BackgroundFillSecondaryDark { get; init; }
        public string BlockLabelBackgroundFillDark { get; init; }
        public string Name { get; init; }


        private static Dictionary<string, string> _defaultPropertyValues = new Dictionary<string, string> {
            { nameof(BodyBackgroundFill) , "*background_fill_primary"},
 { nameof(BodyBackgroundFillDark) , "*background_fill_primary"},
 { nameof(BodyTextColor) , "*neutral_800"},
 { nameof(BodyTextColorDark) , "*neutral_100"},
 { nameof(BodyTextSize) , "*text_md"},
 { nameof(BodyTextWeight) , "400"},
 { nameof(EmbedRadius) , "*radius_sm"},
 { nameof(ColorAccent) , "*primary_500"},
 { nameof(ColorAccentSoft) , "*primary_50"},
 { nameof(ColorAccentSoftDark) , "*neutral_700"},
  { nameof(BackgroundFillPrimary) , "white"},
  { nameof(BackgroundFillPrimaryDark) , "*neutral_950"},
   { nameof(BackgroundFillSecondary) , "*neutral_50"},
     { nameof(BackgroundFillSecondaryDark) , "*neutral_900"},
 { nameof(BorderColorAccent) , "*primary_300"},
 { nameof(BorderColorAccentDark) , "*neutral_600"},
 { nameof(BorderColorPrimary) , "*neutral_200"},
 { nameof(BorderColorPrimaryDark) , "*neutral_700"},
 { nameof(LinkTextColor) , "*secondary_600"},
 { nameof(LinkTextColorActive) , "*secondary_600"},
 { nameof(LinkTextColorActiveDark) , "*secondary_500"},
 { nameof(LinkTextColorDark) , "*secondary_500"},
 { nameof(LinkTextColorHover) , "*secondary_700"},
 { nameof(LinkTextColorHoverDark) , "*secondary_400"},
 { nameof(LinkTextColorVisited) , "*secondary_500"},
 { nameof(LinkTextColorVisitedDark) , "*secondary_600"},
 { nameof(BodyTextColorSubdued) , "*neutral_400"},
 { nameof(BodyTextColorSubduedDark) , "*neutral_400"},
 { nameof(AccordionTextColor) , "*body_text_color"},
 { nameof(AccordionTextColorDark) , "*body_text_color"},
 { nameof(TableTextColor) , "*body_text_color"},
 { nameof(TableTextColorDark) , "*body_text_color"},
 { nameof(ShadowDrop) , "rgba(0,0,0,0.05) 0px 1px 2px 0px"},
 { nameof(ShadowDropLg) , "0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)"},
 { nameof(ShadowInset) , "rgba(0,0,0,0.05) 0px 2px 4px 0px inset"},
 { nameof(ShadowSpread) , "3px"},
 { nameof(ShadowSpreadDark) , "1px"},
 { nameof(BlockBackgroundFill) , "*background_fill_primary"},
 { nameof(BlockBackgroundFillDark) , "*neutral_800"},
 { nameof(BlockBorderColor) , "*border_color_primary"},
 { nameof(BlockBorderColorDark) , "*border_color_primary"},
 { nameof(BlockBorderWidth) , "1px"},
  { nameof(BlockBorderWidthDark) ,null},
 { nameof(BlockInfoTextColor) , "*body_text_color_subdued"},
 { nameof(BlockInfoTextColorDark) , "*body_text_color_subdued"},
 { nameof(BlockInfoTextSize) , "*text_sm"},
 { nameof(BlockInfoTextWeight) , "400"},
 { nameof(BlockLabelBackgroundFill) , "*background_fill_primary"},
  { nameof(BlockLabelBackgroundFillDark) , "*background_fill_secondary"},
 { nameof(BlockLabelBorderColor) , "*border_color_primary"},
 { nameof(BlockLabelBorderColorDark) , "*border_color_primary"},
 { nameof(BlockLabelBorderWidth) , "1px"},
  { nameof(BlockLabelBorderWidthDark) , null},
 { nameof(BlockLabelShadow) , "*block_shadow"},
 { nameof(BlockLabelTextColor) , "*neutral_500"},
 { nameof(BlockLabelTextColorDark) , "*neutral_200"},
 { nameof(BlockLabelMargin) , "0"},
 { nameof(BlockLabelPadding) , "*spacing_sm *spacing_lg"},
  { nameof(BlockLabelRadius) , "calc(*radius_sm - 1px) 0 calc(*radius_sm - 1px) 0"},
   { nameof(BlockLabelRightRadius) ,"0 calc(*radius_sm - 1px) 0 calc(*radius_sm - 1px)"},
 { nameof(BlockLabelTextSize) , "*text_sm"},
 { nameof(BlockLabelTextWeight) , "400"},
 { nameof(BlockPadding) , "*spacing_xl calc(*spacing_xl + 2px)"},
 { nameof(BlockRadius) , "*radius_sm"},
 { nameof(BlockShadow) , "none"},
  { nameof(BlockShadowDark) , null},
 { nameof(BlockTitleBackgroundFill) , "none"},
  { nameof(BlockTitleBackgroundFillDark) ,null},
 { nameof(BlockTitleBorderColor) , "none"},
  { nameof(BlockTitleBorderColorDark) , null},
 { nameof(BlockTitleBorderWidth) , "0px"},
 { nameof(BlockTitleBorderWidthDark) ,null},
 { nameof(BlockTitleTextColor) , "*neutral_500"},
 { nameof(BlockTitleTextColorDark) , "*neutral_200"},
 { nameof(BlockTitlePadding) , "0"},
 { nameof(BlockTitleRadius) , "none"},
 { nameof(BlockTitleTextSize) , "*text_md"},
 { nameof(BlockTitleTextWeight) , "400"},
 { nameof(ContainerRadius) , "*radius_sm"},
 { nameof(FormGapWidth) , "0px"},
 { nameof(LayoutGap) , "*spacing_xxl"},
 { nameof(PanelBackgroundFill) , "*background_fill_secondary"},
 { nameof(PanelBackgroundFillDark) , "*background_fill_secondary"},
 { nameof(PanelBorderColor) , "*border_color_primary"},
 { nameof(PanelBorderColorDark) , "*border_color_primary"},
 { nameof(PanelBorderWidth) , "0"},
  { nameof(PanelBorderWidthDark) ,null},
 { nameof(SectionHeaderTextSize) , "*text_md"},
 { nameof(SectionHeaderTextWeight) , "400"},
 { nameof(BorderColorAccentSubdued) , "*border_color_accent"},
 { nameof(CodeBackgroundFill) , "*neutral_100"},
 { nameof(CodeBackgroundFillDark) , "*neutral_800"},
 { nameof(ChatbotTextSize) , "*text_lg"},
 { nameof(CheckboxBackgroundColor) , "*background_fill_primary"},
 { nameof(CheckboxBackgroundColorDark) , "*neutral_800"},
  { nameof(CheckboxBackgroundColorFocus),"*checkbox_background_color"},
   { nameof(CheckboxBackgroundColorHover),"*checkbox_background_color"},
     { nameof(CheckboxBackgroundColorHoverDark),"*checkbox_background_color"},
      { nameof(CheckboxBackgroundColorSelected),"*color_accent"},
       { nameof(CheckboxBackgroundColorSelectedDark),"*color_accent"},
 { nameof(CheckboxBorderColor) , "*neutral_300"},
 { nameof(CheckboxBorderColorDark) , "*neutral_700"},
 { nameof(CheckboxBorderColorFocus) , "*color_accent"},
 { nameof(CheckboxBorderColorFocusDark) , "*color_accent"},
 { nameof(CheckboxBorderColorHover) , "*neutral_300"},
 { nameof(CheckboxBorderColorHoverDark) ,"*neutral_600"},
 { nameof(CheckboxBorderColorSelected) , "*color_accent"},
  { nameof(CheckboxBorderColorSelectedDark) , "*color_accent"},
 { nameof(CheckboxBorderRadius) , "*radius_sm"},
 { nameof(CheckboxBorderWidth) , "*input_border_width"},
 { nameof(CheckboxBorderWidthDark) , "*input_border_width"},
 { nameof(CheckboxLabelBackgroundFill) , "*button_secondary_background_fill"},
  { nameof(CheckboxLabelBackgroundFillDark) , "*button_secondary_background_fill"},
   { nameof(CheckboxLabelBackgroundFillHover) , "*button_secondary_background_fill_hover"},
    { nameof(CheckboxLabelBackgroundFillHoverDark) , "*button_secondary_background_fill_hover"},
    { nameof(CheckboxLabelBackgroundFillSelected) , "*button_secondary_background_fill"},
       { nameof(CheckboxLabelBackgroundFillSelectedDark) , "*button_secondary_background_fill"},
 { nameof(CheckboxLabelBorderColor) , "*border_color_primary"},
  { nameof(CheckboxLabelBorderColorDark) , "*border_color_primary"},
   { nameof(CheckboxLabelBorderColorHover) , "*checkbox_label_border_color"},
  { nameof(CheckboxLabelBorderColorHoverDark) , "*checkbox_label_border_color"},
    { nameof(CheckboxLabelBorderColorSelected) , "*checkbox_label_border_color"},
      { nameof(CheckboxLabelBorderColorSelectedDark) , "*checkbox_label_border_color"},
 { nameof(CheckboxLabelBorderWidth) , "*input_border_width"},
  { nameof(CheckboxLabelBorderWidthDark) , "*input_border_width"},
 { nameof(CheckboxLabelGap) , "*spacing_lg"},
 { nameof(CheckboxLabelPadding) , "*spacing_md calc(2 * *spacing_md)"},
 { nameof(CheckboxLabelShadow) , "none"},
 { nameof(CheckboxLabelTextSize) , "*text_md"},
 { nameof(CheckboxLabelTextWeight) , "400"},
 { nameof(CheckboxCheck) ,"""url("data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3cpath d='M12.207 4.793a1 1 0 010 1.414l-5 5a1 1 0 01-1.414 0l-2-2a1 1 0 011.414-1.414L6.5 9.086l4.293-4.293a1 1 0 011.414 0z'/%3e%3c/svg%3e")"""},
 { nameof(RadioCircle) ,"""url("data:image/svg+xml,%3csvg viewBox='0 0 16 16' fill='white' xmlns='http://www.w3.org/2000/svg'%3e%3ccircle cx='8' cy='8' r='3'/%3e%3c/svg%3e")"""},
            { nameof(CheckboxShadow) , "*input_shadow"},
 { nameof(CheckboxLabelTextColor) , "*body_text_color"},
 { nameof(CheckboxLabelTextColorDark) , "*body_text_color"},
  { nameof(CheckboxLabelTextColorSelected) , "*checkbox_label_text_color"},
    { nameof(CheckboxLabelTextColorSelectedDark) , "*checkbox_label_text_color"},
    { nameof(ErrorBackgroundFill) ,Colors.Red.C50},
 { nameof(ErrorBackgroundFillDark) , "*background_fill_primary"},
  { nameof(ErrorBorderColor) ,Colors.Red.C700},
    { nameof(ErrorBorderColorDark) ,Colors.Red.C500},
 { nameof(ErrorBorderWidth) , "1px"},
  { nameof(ErrorBorderWidthDark) , null},
     { nameof(ErrorTextColor) ,Colors.Red.C700},
        { nameof(ErrorTextColorDark) ,Colors.Red.C50},
          { nameof(ErrorIconColor) ,Colors.Red.C700},
             { nameof(ErrorIconColorDark) ,Colors.Red.C500},
 { nameof(InputBackgroundFill) , "*neutral_100"},
 { nameof(InputBackgroundFillDark) , "*neutral_700"},
 { nameof(InputBackgroundFillFocus) , "*input_background_fill"},
 { nameof(InputBackgroundFillFocusDark) ,null},
 { nameof(InputBackgroundFillHover) , "*input_background_fill"},
  { nameof(InputBackgroundFillHoverDark) , "*input_background_fill"},
 { nameof(InputBorderColor) , "*border_color_primary"},
 { nameof(InputBorderColorDark) , "*border_color_primary"},
 { nameof(InputBorderColorFocus) , "*secondary_300"},
 { nameof(InputBorderColorFocusDark) , "*neutral_700"},
 { nameof(InputBorderColorHover) , "*input_border_color"},
 { nameof(InputBorderColorHoverDark) , "*input_border_color"},
 { nameof(InputBorderWidth) , "0px"},
  { nameof(InputBorderWidthDark) ,null},
 { nameof(InputPadding) , "*spacing_xl"},
 { nameof(InputPlaceholderColor) , "*neutral_400"},
 { nameof(InputPlaceholderColorDark) , "*neutral_500"},
 { nameof(InputRadius) , "*radius_sm"},
 { nameof(InputShadow) , "none"},
  { nameof(InputShadowDark) ,null},
 { nameof(InputShadowFocus) , "*input_shadow"},
  { nameof(InputShadowFocusDark) , null},
 { nameof(InputTextSize) , "*text_md"},
 { nameof(InputTextWeight) , "400"},
 { nameof(LoaderColor) , "*color_accent"},
  { nameof(LoaderColorDark) ,null},
 { nameof(ProseTextSize) , "*text_md"},
 { nameof(ProseTextWeight) , "400"},
 { nameof(ProseHeaderTextWeight) , "600"},
 { nameof(SliderColor) , "*color_accent"},
  { nameof(SliderColorDark) , null},
 { nameof(StatBackgroundFill) , "*primary_300"},
 { nameof(StatBackgroundFillDark) , "*primary_500"},
 { nameof(TableBorderColor) , "*neutral_300"},
 { nameof(TableBorderColorDark) , "*neutral_700"},
 { nameof(TableEvenBackgroundFill) , "white"},
  { nameof(TableEvenBackgroundFillDark) , "*neutral_950"},
 { nameof(TableOddBackgroundFill) , "*neutral_50"},
 { nameof(TableOddBackgroundFillDark) , "*neutral_900"},
 { nameof(TableRadius) , "*radius_sm"},
 { nameof(TableRowFocus) , "*color_accent_soft"},
 { nameof(TableRowFocusDark) , "*color_accent_soft"},
 { nameof(ButtonBorderWidth) , "*input_border_width"},
  { nameof(ButtonBorderWidthDark) ,null},
 { nameof(ButtonCancelBackgroundFill) , "*button_secondary_background_fill"},
  { nameof(ButtonCancelBackgroundFillDark) , "*button_secondary_background_fill"},
   { nameof(ButtonCancelBackgroundFillHover) , "*button_secondary_background_fill_hover"},
     { nameof(ButtonCancelBackgroundFillHoverDark) , "*button_secondary_background_fill_hover"},
 { nameof(ButtonCancelBorderColorDark) , "*button_secondary_border_color"},
 { nameof(ButtonCancelBorderColor) , "*button_secondary_border_color"},
  { nameof(ButtonCancelBorderColorHover) , "*button_secondary_border_color"},
    { nameof(ButtonCancelBorderColorHoverDark) , "*button_secondary_border_color"},
 { nameof(ButtonCancelTextColor) , "*button_secondary_text_color"},
 { nameof(ButtonCancelTextColorDark) , "*button_secondary_text_color"},
 { nameof(ButtonCancelTextColorHover) , "*button_secondary_text_color_hover"},
  { nameof(ButtonCancelTextColorHoverDark) ,"white"},
 { nameof(ButtonTransformHover) , "none"},
 { nameof(ButtonTransformActive) , "none"},
 { nameof(ButtonTransition) , "all 0.2s ease"},
 { nameof(ButtonLargePadding) , "*spacing_lg calc(2 * *spacing_lg)"},
 { nameof(ButtonLargeRadius) , "*radius_md"},
 { nameof(ButtonLargeTextSize) , "*text_lg"},
 { nameof(ButtonLargeTextWeight) , "600"},
 { nameof(ButtonPrimaryBackgroundFill) , "*primary_500"},
  { nameof(ButtonPrimaryBackgroundFillDark) , "*primary_600"},
   { nameof(ButtonPrimaryBackgroundFillHover) , "*primary_600"},
    { nameof(ButtonPrimaryBackgroundFillHoverDark) , "*primary_700"},
 { nameof(ButtonPrimaryBorderColor) , "*primary_500"},
 { nameof(ButtonPrimaryBorderColorDark) , "*primary_600"},
  { nameof(ButtonPrimaryBorderColorHover) , "*primary_500"},
   { nameof(ButtonPrimaryBorderColorHoverDark) , "*primary_500"},
 { nameof(ButtonPrimaryTextColor) , "white"},
 { nameof(ButtonPrimaryTextColorDark) , "white"},
  { nameof(ButtonPrimaryTextColorHover) , "*button_primary_text_color"},
    { nameof(ButtonPrimaryTextColorHoverDark) , "*button_primary_text_color"},
 { nameof(ButtonPrimaryShadow) , "none"},
 { nameof(ButtonPrimaryShadowHover) , "*button_primary_shadow"},
 { nameof(ButtonPrimaryShadowActive) , "*button_primary_shadow"},
  { nameof(ButtonPrimaryShadowDark) , null},
   { nameof(ButtonPrimaryShadowHoverDark) , "*button_primary_shadow"},
    { nameof(ButtonPrimaryShadowActiveDark) , "*button_primary_shadow"},
     { nameof(ButtonSecondaryBackgroundFill) , "*neutral_200"},
          { nameof(ButtonSecondaryBackgroundFillDark) , "*neutral_600"},
           { nameof(ButtonSecondaryBackgroundFillHover) , "*neutral_300"},
                { nameof(ButtonSecondaryBackgroundFillHoverDark) , "*neutral_700"},
 { nameof(ButtonSecondaryBorderColor) , "*neutral_200"},
  { nameof(ButtonSecondaryBorderColorDark) , "*neutral_600"},
  { nameof(ButtonSecondaryBorderColorHover) , "*neutral_200"},
  { nameof(ButtonSecondaryBorderColorHoverDark) , "*neutral_500"},
 { nameof(ButtonSecondaryTextColor) , "black"},
  { nameof(ButtonSecondaryTextColorDark) , "white"},
  { nameof(ButtonSecondaryTextColorHover) ,"*button_secondary_text_color"},
    { nameof(ButtonSecondaryTextColorHoverDark) ,"*button_secondary_text_color"},
 { nameof(ButtonSecondaryShadow) , "*button_primary_shadow"},
 { nameof(ButtonSecondaryShadowHover) , "*button_secondary_shadow"},
 { nameof(ButtonSecondaryShadowActive) , "*button_secondary_shadow"},
  { nameof(ButtonSecondaryShadowDark) , null},
   { nameof(ButtonSecondaryShadowHoverDark) , "*button_secondary_shadow"},
    { nameof(ButtonSecondaryShadowActiveDark) , "*button_secondary_shadow"},
 { nameof(ButtonSmallPadding) , "*spacing_sm calc(1.5 * *spacing_sm)"},
 { nameof(ButtonSmallRadius) , "*radius_md"},
 { nameof(ButtonSmallTextSize) , "*text_sm"},
 { nameof(ButtonSmallTextWeight) , "400"},
        };
        protected void SetPropertyValues(Dictionary<string, string> newValues)
        {
            Type type = this.GetType();
            foreach (var item in _defaultPropertyValues)
            {
                string propertyName = item.Key;
                string propertyValue = item.Value;
                if (newValues != null && newValues.ContainsKey(item.Key))
                {
                    propertyValue = newValues[propertyName];
                }
                PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propertyInfo.SetValue(this, propertyValue);
            }
        }

        public static Theme Load(string filePath)
        {
            Dictionary<string, string> overrideStyles = new Dictionary<string, string>();
            Color? primaryHue = null;
            Color? secondaryHue = null;
            Color? neutralHue = null;
            Size? textSize = null;
            Size? spacingSize = null;
            Size? radiusSize = null;
            IEnumerable<Font> fonts = null;
            IEnumerable<Font> fontMonos = null;

            string jsonContent = System.IO.File.ReadAllText(filePath);

            using (JsonDocument document = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = document.RootElement;

                if (!root.TryGetProperty("theme", out JsonElement themeElement))
                {
                    throw new Exception($"No 'theme' property found in {filePath}");
                }

                foreach (var property in themeElement.EnumerateObject())
                {
                    Debug.WriteLine($"Property Name: {property.Name}, Property Value: {property.Value}");
                    if (property.Name == "_font")
                    {
                        fonts = LoadFonts(property.Value.EnumerateArray());
                        continue;
                    }
                    if (property.Name == "_font_mono")
                    {
                        fontMonos = LoadFonts(property.Value.EnumerateArray());
                        continue;
                    }
                    if (property.Name == "_stylesheets")
                    {
                        continue;
                    }

                    overrideStyles.Add(property.Name.ToCamelCase(), property.Value.ToString());

                }

            }

            return new Theme(Path.GetFileNameWithoutExtension(filePath),
            overrideStyles,
            primaryHue,
            secondaryHue,
            neutralHue,
            textSize,
            spacingSize,
            radiusSize,
            fonts,
            fontMonos);
        }

        private static IEnumerable<Font>? LoadFonts(JsonElement.ArrayEnumerator arrayEnumerator)
        {
            List<Font> fonts = new List<Font>();
            foreach (var element in arrayEnumerator)
            {
                if (element.GetProperty("class").ToString() == "google")
                {
                    fonts.Add(new GoogleFont(element.GetProperty("name").ToString()));
                }
                else
                {
                    fonts.Add(new LocalFont(element.GetProperty("name").ToString()));
                }
            }

            return fonts;
        }
    }
}

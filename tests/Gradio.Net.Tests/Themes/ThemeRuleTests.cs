using Xunit;

namespace Gradio.Net.Tests.Themes;

/// <summary>
/// The three rules that turn variables into a stylesheet, and the errors the Python version raises
/// around them, asserted on the output rather than on the implementation.
/// </summary>
/// <remarks>
/// These are the rules a custom theme depends on: a variable's name becomes a CSS custom property,
/// a value written <c>*name</c> becomes a reference to another variable, and a <c>_dark</c> suffix
/// moves the variable into the dark block. What these tests do is pin the rules themselves,
/// including the ways a theme can get them wrong.
/// </remarks>
public sealed class ThemeRuleTests
{
    [Fact]
    public void An_underscore_in_a_name_becomes_a_dash()
    {
        var theme = new BaseTheme();

        Assert.Contains("  --body-text-color:", theme.ToCss(), StringComparison.Ordinal);
        Assert.DoesNotContain("--body_text_color", theme.ToCss(), StringComparison.Ordinal);
    }

    [Fact]
    public void A_star_reference_becomes_a_reference_to_the_other_variable()
    {
        var theme = new BaseTheme().Set(new ThemeOptions { BodyTextColor = "*neutral_800" });

        Assert.Contains("  --body-text-color: var(--neutral-800);", theme.ToCss(), StringComparison.Ordinal);
    }

    [Fact]
    public void A_reference_inside_a_larger_value_is_replaced_where_it_stands()
    {
        var theme = new BaseTheme().Set(new ThemeOptions
        {
            BlockPadding = "*spacing_xl calc(*spacing_xl + 2px)",
        });

        Assert.Contains(
            "  --block-padding: var(--spacing-xl) calc(var(--spacing-xl) + 2px);",
            theme.ToCss(),
            StringComparison.Ordinal);
    }

    [Fact]
    public void A_dark_variable_is_written_into_the_dark_block_only()
    {
        var css = new BaseTheme().Set(new ThemeOptions { BodyTextColorDark = "#123456" }).ToCss();

        var light = css[css.IndexOf(":root {", StringComparison.Ordinal)..css.IndexOf(":root.dark", StringComparison.Ordinal)];
        var dark = css[css.IndexOf(":root.dark", StringComparison.Ordinal)..];

        Assert.Contains("  --body-text-color: #123456;", dark, StringComparison.Ordinal);
        Assert.DoesNotContain("--body-text-color: #123456", light, StringComparison.Ordinal);
    }

    [Fact]
    public void A_dark_variable_left_null_inherits_the_light_value()
    {
        // `block_border_width_dark` is one of the twenty dark variables whose default is null:
        // the Python version's way of saying "no dark value of its own".
        var css = new BaseTheme().Set(new ThemeOptions { BlockBorderWidth = "2px" }).ToCss();

        var dark = css[css.IndexOf(":root.dark", StringComparison.Ordinal)..];

        // The light value is carried into the dark block, so a dark-mode rule can override one
        // variable without the others falling back to a browser default.
        Assert.Contains("  --block-border-width: 2px;", dark, StringComparison.Ordinal);
    }

    [Fact]
    public void A_dark_variable_left_null_is_also_written_as_an_ignored_none_declaration()
    {
        // The Python version's own output: the un-suffixed name, with underscores, and the value `None`. A
        // browser drops the declaration; the light value above is the one that applies.
        var css = new BaseTheme().ToCss();

        Assert.Contains("  --block_border_width: None;", css, StringComparison.Ordinal);
    }

    [Fact]
    public void A_variable_that_is_not_a_dark_variable_cannot_be_null()
    {
        // A property left null means "not set", so the refusal is reached only by writing the
        // variable directly — which is what a theme file with a null in it would do.
        var theme = new BaseTheme().Set("body_text_color", null);

        var error = Assert.Throws<InvalidOperationException>(() => theme.ToCss());
        Assert.Contains("only dark mode variables can be null", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_reference_to_a_dark_variable_is_refused()
    {
        var theme = new BaseTheme().Set(new ThemeOptions { BodyTextColor = "*neutral_800_dark" });

        var error = Assert.Throws<InvalidOperationException>(() => theme.ToCss());
        Assert.Contains("do not use the _dark suffix in the value", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_dark_variable_that_refers_to_its_own_light_half_is_refused()
    {
        var theme = new BaseTheme().Set(new ThemeOptions { BodyTextColorDark = "*body_text_color" });

        var error = Assert.Throws<InvalidOperationException>(() => theme.ToCss());
        Assert.Contains("set dark mode version to None", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void An_assignment_keeps_the_variables_place_in_the_stylesheet()
    {
        var theme = new BaseTheme();
        var before = IndexOf(theme.ToCss(), "  --body-text-color:");

        theme.Set(new ThemeOptions { BodyTextColor = "#000000" });

        Assert.Equal(before, IndexOf(theme.ToCss(), "  --body-text-color:"));
    }

    [Fact]
    public void A_variable_set_later_in_the_stylesheet_stays_later()
    {
        // `button_medium_text_weight` is one of the last variables the Python version assigns; a theme that
        // sets it must not move it to the front of the stylesheet.
        var theme = new BaseTheme().Set(new ThemeOptions { ButtonMediumTextWeight = "700" });
        var css = theme.ToCss();

        Assert.True(
            IndexOf(css, "  --button-medium-text-weight:") > IndexOf(css, "  --body-text-color:"),
            "a set variable keeps the position its default gave it");
    }

    [Fact]
    public void An_empty_string_counts_as_not_set()
    {
        // The Python version's assignment is `value or default`, and an empty string is false in Python.
        var theme = new BaseTheme().Set(new ThemeOptions { BodyTextColor = string.Empty });

        Assert.Contains("  --body-text-color: var(--neutral-800);", theme.ToCss(), StringComparison.Ordinal);
    }

    [Fact]
    public void Custom_css_is_appended_after_the_generated_rules()
    {
        var theme = new BaseTheme { CustomCss = ".gradio-container { padding: 0; }" };
        var css = theme.ToCss();

        Assert.EndsWith("\n\n.gradio-container { padding: 0; }", css, StringComparison.Ordinal);
    }

    [Fact]
    public void A_computed_value_follows_a_chain_of_references()
    {
        var theme = new BaseTheme().Set(new ThemeOptions
        {
            BodyTextColor = "*block_title_text_color",
            BlockTitleTextColor = "*neutral_700",
        });

        Assert.Equal("#3f3f46", theme.GetComputedValue("body_text_color"));
    }

    [Fact]
    public void A_computed_dark_value_resolves_its_references_against_dark_variables()
    {
        var theme = new BaseTheme().Set(new ThemeOptions
        {
            BodyTextColor = "*neutral_800",
            BodyTextColorDark = "*neutral_200",
        });

        Assert.Equal("#27272a", theme.GetComputedValue("body_text_color"));
        Assert.Equal("#e4e4e7", theme.GetComputedValue("body_text_color_dark"));
    }

    [Fact]
    public void A_dark_reference_to_a_variable_without_a_dark_half_resolves_the_light_one()
    {
        // The default theme's `body_text_color_dark` is `*neutral_100`, and there is no
        // `neutral_100_dark`: the reference resolves through the light variable, which is what
        // makes a dark mode work for a hue that has no dark ramp of its own.
        var theme = new DefaultTheme();

        Assert.Equal("#f4f4f5", theme.GetComputedValue("body_text_color_dark"));
        Assert.Equal("#f4f4f5", theme.GetComputedValue("neutral_100"));
    }

    private static int IndexOf(string css, string declaration) =>
        css.IndexOf(declaration, StringComparison.Ordinal);
}

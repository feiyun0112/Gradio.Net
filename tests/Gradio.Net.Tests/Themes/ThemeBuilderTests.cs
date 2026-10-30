using Xunit;

namespace Gradio.Net.Tests.Themes;

/// <summary>
/// Building a theme for an application: the presets by name, and the builder for a branded one.
/// </summary>
/// <remarks>
/// The Python version's <c>builder()</c> is an interactive application that assembles a theme and
/// prints the Python code for it. What it assembles is a base theme, three hues, three size ramps,
/// two font stacks, a set of variables and some custom CSS, and that is what
/// <see cref="ThemeBuilder"/> builds directly — a C# application that wants its own colours does
/// not need a second UI to produce them.
/// </remarks>
public sealed class ThemeBuilderTests
{
    [Fact]
    public void A_built_theme_renders_the_choices_it_was_given()
    {
        var theme = ThemeBuilder.Create()
            .PrimaryHue(Colors.Violet)
            .SecondaryHue("emerald")
            .NeutralHue(Colors.Slate)
            .RadiusSize("lg")
            .TextSize(Sizes.TextSm)
            .Values(new ThemeOptions { BodyBackgroundFill = "*neutral_50" })
            .CustomCss(".gradio-container { padding: 0; }")
            .Name("brand")
            .Build();

        var css = theme.ToCss();

        Assert.Equal("brand", theme.Name);
        Assert.Contains("  --primary-500: #8b5cf6;", css, StringComparison.Ordinal);
        Assert.Contains("  --secondary-500: #10b981;", css, StringComparison.Ordinal);
        Assert.Contains("  --neutral-500: #64748b;", css, StringComparison.Ordinal);
        Assert.Contains("  --radius-md: 8px;", css, StringComparison.Ordinal);
        Assert.Contains("  --text-md: 13px;", css, StringComparison.Ordinal);
        Assert.Contains("  --body-background-fill: var(--neutral-50);", css, StringComparison.Ordinal);
        Assert.EndsWith("\n\n.gradio-container { padding: 0; }", css, StringComparison.Ordinal);
    }

    [Fact]
    public void A_builder_that_chooses_nothing_builds_the_base_theme()
    {
        var built = ThemeBuilder.Create().Build();
        var expected = new BaseTheme();

        Assert.Equal(ThemeBuilder.DefaultName, built.Name);
        Assert.Equal(expected.ToCss().Replace("--name: base;", "--name: custom;", StringComparison.Ordinal), built.ToCss());
    }

    [Fact]
    public void A_builder_can_take_a_font_stack()
    {
        var theme = ThemeBuilder.Create()
            .Font(new GoogleFont("Lora"), "ui-sans-serif", "sans-serif")
            .MonoFont(new LocalFont("Inconsolata", [400, 700]))
            .Build();

        Assert.Equal("'Lora', ui-sans-serif, sans-serif", theme.GetValue("font"));
        Assert.Equal("'Inconsolata'", theme.GetValue("font_mono"));
        Assert.Contains("@font-face {", theme.ToCss(), StringComparison.Ordinal);
    }

    [Fact]
    public void Every_built_in_name_resolves_to_its_own_theme()
    {
        var names = ThemePresets.BuiltInNames;

        Assert.Equal(12, names.Count);
        Assert.Equal(
            ["base", "default", "ember", "glass", "monochrome", "neon", "soft", "origin", "citrus", "ocean", "mario", "cyberpunk"],
            names);

        foreach (var name in names)
        {
            Assert.Equal(name, ThemePresets.Get(name).Name);
        }
    }

    [Fact]
    public void Every_call_answers_with_a_theme_of_its_own()
    {
        // The Python version keeps one instance of each preset in a process-wide dictionary, so one
        // application's customisation would leak into the next; here a theme belongs to whoever
        // asked for it.
        var first = ThemePresets.Get("soft");
        var second = ThemePresets.Get("soft");

        Assert.NotSame(first, second);

        first.Set(new ThemeOptions { BodyBackgroundFill = "#123456" });

        Assert.NotEqual(first.ToCss(), second.ToCss());
    }

    [Fact]
    public void A_name_is_matched_without_regard_to_case()
    {
        Assert.Equal("soft", ThemePresets.Get("SOFT").Name);
    }

    [Fact]
    public void A_name_that_is_not_a_theme_is_refused()
    {
        var error = Assert.Throws<ArgumentException>(() => ThemePresets.Get("solarized"));
        Assert.Contains("is not a built-in theme", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_hue_or_size_name_that_does_not_exist_is_refused()
    {
        Assert.Throws<ArgumentException>(() => ThemePresets.FindHue("burgundy"));
        Assert.Throws<ArgumentException>(() => ThemePresets.FindSize("huge", "radius"));
    }

    [Fact]
    public void The_default_theme_is_what_an_application_gets_when_it_names_none()
    {
        Assert.Equal("default", ThemePresets.Default().Name);
    }

    [Fact]
    public void A_preset_takes_the_raw_material_it_is_given()
    {
        var theme = new DefaultTheme(primaryHue: Colors.Violet);

        Assert.Contains("  --primary-500: #8b5cf6;", theme.ToCss(), StringComparison.Ordinal);

        // The rest of the preset's own choices stay: only the hue was replaced.
        Assert.Contains("  --neutral-500: #71717a;", theme.ToCss(), StringComparison.Ordinal);
    }
}

using System.Text.Json.Nodes;
using Xunit;

namespace Gradio.Net.Tests.Themes;

/// <summary>
/// A theme's serialised form: what <c>to_dict()</c> writes, and what reading it back restores.
/// </summary>
/// <remarks>
/// The document is the portable half of the theme system — it is what the Python version's
/// <c>load()</c> and <c>dump()</c> move between machines, and what an application checks into its
/// repository to get the same colours on every build. These tests prove that the round trip is
/// lossless and that a document which leaves variables out is completed rather than rejected.
/// </remarks>
public sealed class ThemeDocumentTests
{
    [Fact]
    public void A_theme_reads_back_from_its_own_document_unchanged()
    {
        var original = new CitrusTheme();
        var restored = Theme.FromDictionary(original.ToDictionary());

        // The document is alphabetical, because the Python version builds it by walking `dir()`, so a theme
        // that goes through one comes back with its declarations in a different order — the Python version
        // included. What has to survive is every declaration, and every value.
        AssertSameDeclarations(original.ToCss(), restored.ToCss());
        Assert.True(JsonNode.DeepEquals(original.ToDictionary(), restored.ToDictionary()));
        Assert.Equal(original.Name, restored.Name);
        Assert.Equal(original.Stylesheets, restored.Stylesheets);
    }

    [Fact]
    public void A_document_keeps_the_font_stacks_it_was_written_with()
    {
        var original = new BaseTheme(
            font: [new GoogleFont("Lora"), "ui-sans-serif"],
            fontMono: [new LocalFont("Inconsolata", [400, 700]), "monospace"]);

        var restored = Theme.FromDictionary(original.ToDictionary());

        Assert.Equal(original.GetValue("font"), restored.GetValue("font"));
        Assert.Equal(original.GetValue("font_mono"), restored.GetValue("font_mono"));
        AssertSameDeclarations(original.ToCss(), restored.ToCss());
    }

    [Fact]
    public void A_variable_the_document_leaves_out_is_filled_in_from_the_base_theme()
    {
        // A theme file is a set of choices, not a complete definition: what it does not mention
        // comes from the base theme, which is what makes a file written against an older release
        // still render.
        var document = new JsonObject
        {
            ["theme"] = new JsonObject
            {
                ["name"] = "trimmed",
                ["body_text_color"] = "#010203",
            },
        };

        var theme = Theme.FromDictionary(document);

        Assert.Equal("#010203", theme.GetValue("body_text_color"));
        Assert.Equal("#fef2f2", theme.GetValue("error_background_fill"));
        Assert.Equal("0px", theme.GetValue("input_border_width"));
    }

    [Fact]
    public void A_theme_survives_a_file_round_trip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"gradio-theme-{Guid.NewGuid():N}.json");

        try
        {
            var original = new EmberTheme { CustomCss = ".gradio-container { gap: 0; }" };
            original.Dump(path);

            var restored = Theme.Load(path);

            AssertSameDeclarations(original.ToCss(), restored.ToCss());
            Assert.Equal(".gradio-container { gap: 0; }", restored.CustomCss);
        }
        finally
        {
            System.IO.File.Delete(path);
        }
    }

    /// <summary>Asserts two stylesheets declare the same things, whatever order they declare them in.</summary>
    private static void AssertSameDeclarations(string expected, string actual)
    {
        static IEnumerable<string> Declarations(string css) =>
            css.Split('\n')
                .Where(line => line.TrimStart().StartsWith("--", StringComparison.Ordinal))
                .Order(StringComparer.Ordinal);

        Assert.Equal(Declarations(expected), Declarations(actual));
    }

    [Fact]
    public void A_variable_that_does_not_exist_is_refused()
    {
        var theme = new BaseTheme();

        var error = Assert.Throws<ArgumentException>(() => theme.Set("body_text_colour", "#000000"));
        Assert.Contains("is not a theme variable", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_document_that_is_not_a_theme_is_refused()
    {
        Assert.Throws<ArgumentException>(() => Theme.FromDictionary(new JsonObject { ["colours"] = new JsonObject() }));
    }

    [Fact]
    public void The_options_cover_the_whole_surface_from_its_first_variable_to_its_last()
    {
        // The two generated tables have to agree end to end: the options class is written from the
        // same extraction as the defaults, so a property that landed in neither would be a value an
        // application can set and nothing reads. The first and the last variable of the Python version's
        // assignment order are the two ends of that agreement.
        var theme = new BaseTheme().Set(new ThemeOptions
        {
            BodyBackgroundFill = "#000001",
            ButtonMediumTextWeight = "#000002",
        });

        Assert.Equal("#000001", theme.GetValue("body_background_fill"));
        Assert.Equal("#000002", theme.GetValue("button_medium_text_weight"));
        Assert.Equal("#000001", theme.ToDictionary()["theme"]!["body_background_fill"]!.GetValue<string>());
    }
}

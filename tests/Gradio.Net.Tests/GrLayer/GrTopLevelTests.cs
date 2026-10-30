using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The top level that is not a block: the themes, the functions, and the two members that draw into
/// the page being written.
/// </summary>
/// <remarks>
/// <para>
/// Everything here is a forwarding: the layer builds nothing of its own, so what is asserted is that
/// the forwarding reaches the same object the core would have built. The themes are compared by
/// their stylesheet rather than by their type, because the stylesheet is what a browser is handed —
/// a preset that lost a variable would still be a <c>SoftTheme</c>.
/// </para>
/// <para>
/// The two that find the application themselves — <see cref="gr.Examples"/> and
/// <see cref="gr.Api"/> — are the interesting ones: they are the members the Python version writes
/// without naming the page, so a test that passes them a page would not be testing the same thing.
/// </para>
/// </remarks>
public sealed class GrTopLevelTests
{
    /// <summary>Two samples for the gallery, as the Python version writes them: one list per
    /// sample.</summary>
    private static readonly string[][] Samples = [["one"], ["two"]];

    [Fact]
    public void A_theme_is_the_preset_the_core_builds()
    {
        // The twelve presets, each compared to the core type of the same name: the layer adds no
        // variable and drops none, which is what "the stylesheet is the same" says.
        Assert.Equal(new BaseTheme().ToCss(), gr.Themes.Base().ToCss());
        Assert.Equal(new DefaultTheme().ToCss(), gr.Themes.Default().ToCss());
        Assert.Equal(new MonochromeTheme().ToCss(), gr.Themes.Monochrome().ToCss());
        Assert.Equal(new SoftTheme().ToCss(), gr.Themes.Soft().ToCss());
        Assert.Equal(new GlassTheme().ToCss(), gr.Themes.Glass().ToCss());
        Assert.Equal(new OriginTheme().ToCss(), gr.Themes.Origin().ToCss());
        Assert.Equal(new CitrusTheme().ToCss(), gr.Themes.Citrus().ToCss());
        Assert.Equal(new OceanTheme().ToCss(), gr.Themes.Ocean().ToCss());
        Assert.Equal(new MarioTheme().ToCss(), gr.Themes.Mario().ToCss());
        Assert.Equal(new EmberTheme().ToCss(), gr.Themes.Ember().ToCss());
        Assert.Equal(new NeonTheme().ToCss(), gr.Themes.Neon().ToCss());
        Assert.Equal(new CyberpunkTheme().ToCss(), gr.Themes.Cyberpunk().ToCss());
    }

    [Fact]
    public void A_theme_choice_is_read_the_way_upstream_types_it()
    {
        // `gr.themes.Soft(primary_hue="violet")` is the union `Color | str`, so both spellings have
        // to reach the same theme — and a string is a *name*, not a colour literal.
        var violet = ThemePresets.FindHue("violet");

        Assert.Equal(
            new SoftTheme(primaryHue: violet).ToCss(),
            gr.Themes.Soft(primaryHue: "violet").ToCss());

        Assert.Equal(
            new SoftTheme(primaryHue: violet).ToCss(),
            gr.Themes.Soft(primaryHue: violet).ToCss());

        // The base theme takes the same choices, which is what makes `gr.Theme` and
        // `gr.Themes.Base` the same document.
        Assert.Equal(
            gr.Themes.Base(primaryHue: "violet").ToCss(),
            gr.Theme(primaryHue: "violet").ToCss());
    }

    [Fact]
    public void A_theme_choice_that_names_nothing_is_refused_where_it_was_written()
    {
        // A misspelt ramp is a colour the stylesheet cannot have; replacing it with the default
        // would hide the typo in a theme that looks almost right.
        Assert.Throws<ArgumentException>(() => gr.Themes.Soft(primaryHue: "not-a-ramp"));
        Assert.Throws<ArgumentException>(() => gr.Themes.Soft(textSize: 12));
    }

    [Fact]
    public void Examples_draws_into_the_page_being_written()
    {
        // `gr.Examples(examples, inputs=[...])` names no application: it draws into the page the
        // line is written on, and binds the endpoint a click on a sample reaches.
        using var demo = gr.Blocks();

        var text = gr.Textbox(label: "Text");

        var examples = gr.Examples(Samples, [text], label: "Try these");

        Assert.Equal("Try these", examples.Dataset.Label);
        Assert.Equal("load_example", Assert.Single(demo.Dependencies).ApiName);
    }

    [Fact]
    public void Api_exposes_a_handler_that_no_component_raises()
    {
        // The endpoint is derived from the signature rather than bound to a block, so the served
        // dependency has no target at all — which is what a client calling it by name reads.
        using var demo = gr.Blocks();

        var binding = gr.Api(
            (Func<string, string>)(value => value.ToUpperInvariant()),
            apiName: "shout");

        Assert.Empty(binding.Targets);
        Assert.Equal("shout", binding.ApiName);
        Assert.Same(binding, Assert.Single(demo.Dependencies));
    }

    [Fact]
    public void The_functions_answer_what_upstream_answers()
    {
        // The version is the release this build mirrors, not the .NET package's own number: a
        // ported application asks what gradio it is running against.
        Assert.Equal(GradioVersion.Upstream, gr.GetPackageVersion());

        // A sample file is a path that exists, and the name given selects which one.
        var image = gr.GetImage("cheetah1.jpg");

        Assert.True(System.IO.File.Exists(image), $"'{image}' is not a file");
        Assert.Equal("cheetah1.jpg", Path.GetFileName(image));

        // The progress tracker is the object the framework fills in, and it is the core's.
        Assert.IsType<Progress>(gr.Progress());

        // The sentinel is the Python version's value, because it is also a number of seconds.
        Assert.Equal(1, gr.NoReload);
    }

    [Fact]
    public void The_translations_are_the_catalogue_upstream_writes_as_keywords()
    {
        // `gr.I18n(en={"Hello": "Hello"}, fr={"Hello": "Bonjour"})` is two keyword arguments in
        // Python and two pairs here; the locale codes are the keys either way.
        var translations = gr.I18n(
            ("en", new Dictionary<string, string> { ["Hello"] = "Hello" }),
            ("fr", new Dictionary<string, string> { ["Hello"] = "Bonjour" }));

        Assert.Equal(["en", "fr"], translations.Locales);
        Assert.Equal("Bonjour", translations.TranslationsFor("fr")!["Hello"]);
    }

    [Fact]
    public void A_member_that_draws_needs_a_page_to_draw_on()
    {
        // Outside an application there is nothing to draw into, and saying so is better than a
        // gallery that lands nowhere.
        var error = Assert.Throws<InvalidOperationException>(() => gr.Api((Func<string, string>)(value => value)));

        Assert.Contains("gr.Blocks()", error.Message, StringComparison.Ordinal);
    }
}

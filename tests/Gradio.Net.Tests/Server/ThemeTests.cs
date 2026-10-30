using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Shared;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The theme as the browser meets it: the stylesheet at <c>/theme.css</c>, and the fields the config
/// carries about it.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the wiring — that the application's theme is the one served, that the
/// config describes it, and that the four values the page paints itself with are the theme's
/// rather than the template's defaults.
/// </para>
/// <para>
/// This is the HTTP seam like every other test in this directory: a real host, a real request, and
/// nothing reaching into the application object to check what it would have said.
/// </para>
/// </remarks>
public sealed class ThemeTests
{
    [Fact]
    public async Task The_stylesheet_is_the_applications_theme()
    {
        await using var host = await StartAsync(blocks => blocks.Theme = new OceanTheme());

        var theme = await host.Client.GetStringAsync("/theme.css", TestContext.Current.CancellationToken);

        Assert.Contains("--name: ocean;", theme, StringComparison.Ordinal);
        Assert.Contains("  --primary-500: #10b981;", theme, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_config_describes_the_theme()
    {
        await using var host = await StartAsync(blocks => blocks.Theme = new NeonTheme());

        var config = await ConfigAsync(host);
        var stylesheet = await host.Client.GetStringAsync("/theme.css", TestContext.Current.CancellationToken);

        Assert.Equal("neon", config["theme"]!.GetValue<string>());
        Assert.Equal(Hash(stylesheet), config["theme_hash"]!.GetValue<string>());
        Assert.Empty(config["stylesheets"]!.AsArray());

        // The page paints itself with these before the stylesheet arrives, so they are the theme's
        // values rather than references into a stylesheet that has not loaded.
        var body = config["body_css"]!.AsObject();
        Assert.Equal("#f5f5f5", body["body_background_fill"]!.GetValue<string>());
        Assert.Equal("#262626", body["body_text_color"]!.GetValue<string>());
        Assert.Equal("#0f0f0f", body["body_background_fill_dark"]!.GetValue<string>());
        Assert.Equal("#e5e5e5", body["body_text_color_dark"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_page_carries_the_themes_colours()
    {
        await using var host = await StartAsync(blocks => blocks.Theme = new NeonTheme());

        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        // The template's own `:root` block, filled from body_css: the first paint is in the
        // application's colours rather than the browser's default white.
        Assert.Contains("--bg: #f5f5f5;", page, StringComparison.Ordinal);
        Assert.Contains("--col:   #262626;", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_theme_the_application_builds_is_the_one_served()
    {
        var theme = ThemeBuilder.Create()
            .PrimaryHue("violet")
            .CustomCss(".gradio-container { padding: 0; }")
            .Build();

        await using var host = await StartAsync(blocks => blocks.Theme = theme);

        var stylesheet = await host.Client.GetStringAsync("/theme.css", TestContext.Current.CancellationToken);
        var config = await ConfigAsync(host);

        Assert.Equal(ThemeBuilder.DefaultName, config["theme"]!.GetValue<string>());
        Assert.Contains("  --primary-500: #8b5cf6;", stylesheet, StringComparison.Ordinal);
        Assert.EndsWith(".gradio-container { padding: 0; }", stylesheet, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Two_applications_in_one_process_serve_their_own_themes()
    {
        await using var first = await StartAsync(blocks => blocks.Theme = new NeonTheme());
        await using var second = await StartAsync(blocks => blocks.Theme = new MonochromeTheme());

        Assert.Contains("--name: neon;", await first.Client.GetStringAsync("/theme.css", TestContext.Current.CancellationToken), StringComparison.Ordinal);
        Assert.Contains("--name: monochrome;", await second.Client.GetStringAsync("/theme.css", TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_font_the_frontend_does_not_carry_is_listed_as_a_stylesheet()
    {
        var theme = new BaseTheme(font: [new GoogleFont("Noto Sans"), "sans-serif"]);

        await using var host = await StartAsync(blocks => blocks.Theme = theme);

        var config = await ConfigAsync(host);
        var stylesheets = config["stylesheets"]!.AsArray();

        Assert.Single(stylesheets);
        Assert.Equal(
            "https://fonts.googleapis.com/css2?family=Noto+Sans:wght@400;600&display=swap",
            stylesheets[0]!.GetValue<string>());
    }

    [Fact]
    public async Task Custom_css_the_application_sets_reaches_the_config()
    {
        await using var host = await StartAsync(blocks => blocks.Css = ".gradio-container { gap: 0; }");

        var config = await ConfigAsync(host);

        Assert.Equal(".gradio-container { gap: 0; }", config["css"]!.GetValue<string>());
    }

    /// <summary>
    /// The stylesheets the application names are read, and follow its own CSS.
    /// </summary>
    /// <remarks>
    /// The Python version concatenates rather than replaces: the <c>css</c> the application wrote
    /// comes first and each file follows it with a newline in front. The newline is the part worth
    /// pinning because it is the part nothing requires — two rules may touch — so a change that
    /// dropped it would still paint and still be wrong.
    /// </remarks>
    [Fact]
    public async Task Css_files_follow_the_css_the_application_wrote()
    {
        using var folder = new TempFolder();
        var first = folder.Write("first.css", ".a { color: red; }");
        var second = folder.Write("second.css", ".b { color: blue; }");

        await using var host = await StartAsync(blocks =>
        {
            blocks.Css = ".own { color: green; }";
            blocks.CssPaths = [first, second];
        });

        var config = await ConfigAsync(host);

        Assert.Equal(
            ".own { color: green; }\n.a { color: red; }\n.b { color: blue; }",
            config["css"]!.GetValue<string>());
    }

    /// <summary>
    /// An application with files and no CSS of its own carries the newline as well.
    /// </summary>
    /// <remarks>
    /// The Python version writes <c>self.css = self.css or ""</c> before appending, so the text
    /// begins with the separator rather than with the first rule. It is one character, and it is
    /// what the concatenation does to an application that had nothing to put in front of it.
    /// </remarks>
    [Fact]
    public async Task Css_files_with_no_css_of_its_own_still_begin_with_the_separator()
    {
        using var folder = new TempFolder();

        await using var host = await StartAsync(blocks =>
            blocks.CssPaths = [folder.Write("only.css", ".only { color: red; }")]);

        var config = await ConfigAsync(host);

        Assert.Equal("\n.only { color: red; }", config["css"]!.GetValue<string>());
    }

    /// <summary>
    /// A stylesheet written on Windows is served with the bytes one written anywhere else has.
    /// </summary>
    /// <remarks>
    /// The Python version reads with Python's text mode, which turns a carriage return into a line
    /// feed and keeps a byte-order mark as the first character — the codec it names is
    /// <c>utf-8</c>, not <c>utf-8-sig</c>. Both are visible in the served text, and both are what a
    /// stylesheet saved by a Windows editor arrives with, so the file is written with them and the
    /// answer is compared character for character.
    /// </remarks>
    [Fact]
    public async Task A_stylesheet_written_with_windows_line_endings_is_served_with_line_feeds()
    {
        using var folder = new TempFolder();

        // A byte-order mark, one CRLF, and a lone CR: what a Windows editor writes, and what
        // Python's text mode has an opinion about.
        var sheet = folder.Write("windows.css", "\uFEFF.a { color: red; }\r\n.b { color: blue; }\r");

        await using var host = await StartAsync(blocks => blocks.CssPaths = [sheet]);

        var config = await ConfigAsync(host);

        Assert.Equal(
            "\n\uFEFF.a { color: red; }\n.b { color: blue; }\n",
            config["css"]!.GetValue<string>());
    }

    /// <summary>
    /// Naming no files adds nothing, not even the separator.
    /// </summary>
    /// <remarks>
    /// An empty list is what a list built at run time looks like when it turned out empty, and it
    /// has to behave like an application that named none: the config carries the application's own
    /// CSS and nothing else, rather than a stray newline or an empty string.
    /// </remarks>
    [Fact]
    public async Task An_empty_list_of_css_files_adds_nothing()
    {
        await using var host = await StartAsync(blocks =>
        {
            blocks.Css = ".own { color: green; }";
            blocks.CssPaths = [];
        });

        var config = await ConfigAsync(host);

        Assert.Equal(".own { color: green; }", config["css"]!.GetValue<string>());
    }

    /// <summary>
    /// A stylesheet the application named but does not have fails the page rather than the styles.
    /// </summary>
    /// <remarks>
    /// The Python version opens each path when the application starts and raises when it cannot.
    /// Here the read happens when the config is built, so the failure arrives as a failed request —
    /// which is still a failure rather than a page that quietly draws without the CSS the author
    /// asked for.
    /// </remarks>
    [Fact]
    public async Task A_css_file_that_is_not_there_fails_the_request()
    {
        using var folder = new TempFolder();

        await using var host = await StartAsync(blocks =>
            blocks.CssPaths = [Path.Combine(folder.Folder, "missing.css")]);

        using var response = await host.Client.GetAsync(
            "/config",
            TestContext.Current.CancellationToken);

        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task An_application_that_sets_no_css_carries_none()
    {
        await using var host = await StartAsync(build: null);

        var config = await ConfigAsync(host);

        Assert.Null(config["css"]);
    }

    private static async Task<JsonObject> ConfigAsync(GradioTestHost host)
    {
        var body = await host.Client.GetStringAsync("/config", TestContext.Current.CancellationToken);

        return JsonNode.Parse(body)!.AsObject();
    }

    private static string Hash(string text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();

    private static Task<GradioTestHost> StartAsync(Action<Blocks>? build) =>
        GradioTestHost.StartStandaloneAsync(logs: null, TestContext.Current.CancellationToken, build);
}

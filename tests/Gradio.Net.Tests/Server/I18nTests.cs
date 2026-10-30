using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The translations an application hands the frontend, as a client reads them: the dictionary the
/// config carries, and the markers the components' props carry instead of text.
/// </summary>
/// <remarks>
/// <para>
/// Translating is the frontend's work — it holds the language, the reader's choice of it, and the
/// catalogue of its own chrome — so the server's whole part is to say which strings are keys and
/// what the application calls them in each language. That is what is asserted here: the dictionary
/// arrives whole, and a prop written as a key arrives as the marker the frontend recognises.
/// </para>
/// <para>
/// Whether a reader can switch language is the browser's question (<c>BrowserEndToEndTests</c>).
/// What is here is the shape that depends on, checkable without a browser.
/// </para>
/// </remarks>
public sealed class I18nTests
{
    /// <summary>The greeting key, so that the marker assertions name one string.</summary>
    private const string Greeting = "greeting";

    [Fact]
    public async Task The_config_carries_the_translations_the_application_was_given()
    {
        var i18n = EnglishAndSpanish();

        await using var host = await StartAsync(blocks => blocks.I18n = i18n);

        var translations = (await ConfigAsync(host))["i18n_translations"]!.AsObject();

        // The locales, in the order the application declared them: a dictionary is served in the
        // order it was written, which is what makes the document readable and the Python version's.
        Assert.Equal(["en", "es", "zh-CN"], translations.Select(entry => entry.Key));

        var spanish = translations["es"]!.AsObject();
        Assert.Equal("Hola", spanish[Greeting]!.GetValue<string>());
        Assert.Equal("Tu Nombre", spanish["name_label"]!.GetValue<string>());
        Assert.Equal("你好", translations["zh-CN"]!["greeting"]!.GetValue<string>());
    }

    [Fact]
    public async Task An_application_that_gives_no_translations_carries_none()
    {
        await using var host = await StartAsync(build: null);

        var config = await ConfigAsync(host);

        // Present and null rather than absent: "this application translates nothing" is a fact the
        // frontend reads, and it is the one the Python version serves for an application that passed none.
        Assert.True(config.ContainsKey("i18n_translations"));
        Assert.Null(config["i18n_translations"]);
    }

    [Fact]
    public async Task An_empty_catalogue_is_served_as_an_empty_dictionary()
    {
        await using var host = await StartAsync(blocks => blocks.I18n = new I18n());

        var config = await ConfigAsync(host);

        // The Python version's two states are not the same: no instance at all is null, an instance holding
        // nothing is an empty dictionary. A build that collapsed them would tell the frontend
        // something it did not say.
        Assert.Empty(config["i18n_translations"]!.AsObject());
    }

    [Fact]
    public void What_the_application_declared_can_be_read_back()
    {
        var i18n = EnglishAndSpanish();

        // The same order the config serves, because both read the one declaration: an application
        // building a language menu of its own and a client reading /config see the same list.
        Assert.Equal(["en", "es", "zh-CN"], i18n.Locales);

        Assert.Equal("Tu Nombre", i18n.TranslationsFor("es")!["name_label"]);

        // Undeclared is not the same answer as declared-and-empty, and neither is an error: the
        // first says the application offers nothing in that language, the second that it offers a
        // language it has not written any text in yet.
        Assert.Null(i18n.TranslationsFor("de"));
        Assert.Empty(new I18n(("pt-BR", new Dictionary<string, string>())).TranslationsFor("pt-BR")!);
    }

    [Fact]
    public async Task A_page_carries_the_translations_too()
    {
        var i18n = EnglishAndSpanish();

        await using var host = await StartAsync(blocks =>
        {
            blocks.I18n = i18n;

            using (blocks.BeginRender())
            {
                blocks.Add(new Markdown { Value = i18n[Greeting] });
            }

            using (blocks.Route("About"))
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Markdown { Value = "About this application" });
                }
            }
        });

        var page = await ConfigAsync(host, "?page=about");

        // The page's document is the whole application's with its components narrowed, so a
        // translation dictionary that only reached /config would leave the page's own script
        // without one — and the page is what the browser boots from.
        Assert.Equal(
            "Hola",
            page["i18n_translations"]!["es"]![Greeting]!.GetValue<string>());
    }

    [Fact]
    public void A_key_becomes_the_marker_the_frontend_translates()
    {
        var i18n = EnglishAndSpanish();

        // The marker is the Python version's own spelling, down to the spaces its JSON writer puts after the
        // colons: the frontend finds the metadata by searching for the prefix and parsing what
        // follows it, so a different spelling would be served as text rather than translated.
        Assert.Equal(
            "__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"greeting\"}",
            i18n[Greeting]);
    }

    [Fact]
    public async Task A_component_written_from_a_key_carries_the_marker()
    {
        var i18n = EnglishAndSpanish();

        await using var host = await StartAsync(blocks =>
        {
            blocks.I18n = i18n;

            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox { Label = i18n["name_label"], Value = i18n["john_doe"] });
                blocks.Add(new Button { Value = i18n["submit_button"] });
            }
        });

        var components = (await ConfigAsync(host))["components"]!.AsArray();

        var textbox = components[0]!["props"]!;
        Assert.Equal("__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"name_label\"}", textbox["label"]!.GetValue<string>());
        Assert.Equal("__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"john_doe\"}", textbox["value"]!.GetValue<string>());

        Assert.Equal(
            "__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"submit_button\"}",
            components[1]!["props"]!["value"]!.GetValue<string>());
    }

    [Fact]
    public void A_key_that_holds_a_quote_survives_the_marker()
    {
        var i18n = new I18n(("en", new Dictionary<string, string> { ["a\"b\\c"] = "text" }));

        // A key is application data. The metadata is JSON, so a quote or a backslash in it has to
        // be escaped, and what the frontend parses back out has to be the key that went in.
        Assert.Equal(
            "__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"a\\\"b\\\\c\"}",
            i18n["a\"b\\c"]);
    }

    [Fact]
    public async Task A_locale_that_is_not_a_language_tag_is_kept()
    {
        var i18n = new I18n(
            ("en", new Dictionary<string, string> { [Greeting] = "Hello" }),
            ("bad_locale", new Dictionary<string, string> { [Greeting] = "kept" }));

        await using var host = await StartAsync(blocks => blocks.I18n = i18n);

        var translations = (await ConfigAsync(host))["i18n_translations"]!.AsObject();

        // The Python version warns about a code that is not BCP 47 and includes it anyway, because a locale
        // the frontend does not know costs nothing and dropping it would be a silent edit of what
        // the application said. There is no warning channel here, so the rule is the behaviour.
        Assert.Equal("kept", translations["bad_locale"]![Greeting]!.GetValue<string>());
    }

    [Fact]
    public async Task A_locale_declared_twice_keeps_the_last_translations_in_its_first_place()
    {
        var i18n = new I18n(
            ("en", new Dictionary<string, string> { [Greeting] = "Hello" }),
            ("es", new Dictionary<string, string> { [Greeting] = "Hola" }),
            ("en", new Dictionary<string, string> { [Greeting] = "Hello again" }));

        await using var host = await StartAsync(blocks => blocks.I18n = i18n);

        var translations = (await ConfigAsync(host))["i18n_translations"]!.AsObject();

        // The Python version builds a dictionary, so the second declaration of a locale replaces the first
        // and the locale keeps the place it was first given.
        Assert.Equal(["en", "es"], translations.Select(entry => entry.Key));
        Assert.Equal("Hello again", translations["en"]![Greeting]!.GetValue<string>());
    }

    [Fact]
    public async Task A_value_a_handler_returns_is_translated_too()
    {
        var i18n = EnglishAndSpanish();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.I18n = i18n;

            Textbox name;
            Button reset;

            using (blocks.BeginRender())
            {
                name = blocks.Add(new Textbox { Label = i18n["name_label"] });
                reset = blocks.Add(new Button { Value = "Reset Name" });
            }

            // The Python version's demo resets the field to a key rather than to text, so that the reset
            // reads in the reader's language like the label does. The answer travels the queue as
            // an ordinary string, and the marker is what the frontend translates on the way in.
            reset.Click(
                (Func<object?>)(() => i18n["john_doe"]),
                [],
                [name],
                apiName: "reset_name");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("reset_name", cancellationToken);

        await queue.JoinAsync(fnIndex, [], "reset-session", cancellationToken);

        await using var stream = await queue.OpenAsync("reset-session", cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.Equal(
            "__i18n__{\"__type__\": \"translation_metadata\", \"key\": \"john_doe\"}",
            completed!["output"]!["data"]![0]!.GetValue<string>());
    }

    /// <summary>A catalogue with three locales, one of them a language tag with a region.</summary>
    private static I18n EnglishAndSpanish() => new(
        ("en", new Dictionary<string, string>
        {
            [Greeting] = "Hello",
            ["name_label"] = "Your Name",
            ["submit_button"] = "Greet",
            ["john_doe"] = "John English",
        }),
        ("es", new Dictionary<string, string>
        {
            [Greeting] = "Hola",
            ["name_label"] = "Tu Nombre",
            ["submit_button"] = "Saludar",
        }),
        ("zh-CN", new Dictionary<string, string> { [Greeting] = "你好" }));

    private static async Task<JsonObject> ConfigAsync(GradioTestHost host, string? query = null)
    {
        using var response = await host.Client.GetAsync(
            "/config" + query,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();
    }

    private static Task<GradioTestHost> StartAsync(Action<Blocks>? build) =>
        GradioTestHost.StartStandaloneAsync(logs: null, TestContext.Current.CancellationToken, build);
}

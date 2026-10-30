using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Gradio.Net.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Signing in with an identity provider: the flow, the session it leaves behind, and the ways it is
/// refused.
/// </summary>
/// <remarks>
/// <para>
/// No test here talks to Hugging Face or to any other real provider. The flow is exercised against
/// a provider the tests run — the protocol's own implementation pointed at a service that answers
/// from a script, and a hand-written provider for the seam itself. A real provider is expected to
/// work; that it does is not something this repository can show.
/// </para>
/// <para>
/// What is asserted is what a browser would see: the address it is sent to, the address it comes
/// back to, and the label the page carries once it is signed in. The provider's side of the
/// conversation is asserted from what it received, which is how a request the flow builds is
/// checked without a network.
/// </para>
/// </remarks>
public sealed class OAuthTests
{
    private const string ClientId = "the-client";
    private const string ClientSecret = "the-secret";
    private const string SignInLabel = "Sign in with Hugging Face";

    /// <summary>What a token endpoint answers, in the shape OpenID Connect describes.</summary>
    private const string TokenAnswer =
        """{"access_token":"at-1","token_type":"bearer","expires_in":3600,"scope":"openid profile","id_token":"id-1"}""";

    /// <summary>What a userinfo endpoint answers.</summary>
    private const string UserInfoAnswer =
        """{"sub":"1","name":"Alice Example","preferred_username":"alice","profile":"https://idp.test/alice","picture":"https://idp.test/alice.png"}""";

    [Fact]
    public async Task A_sign_in_goes_to_the_provider_and_comes_back_with_the_users_profile()
    {
        var identity = Identity();

        await using var host = await StartAsync(new OidcOAuthProvider(Options(), identity.Client()));
        using var browser = Browser(host);

        using var start = await browser.GetAsync("/login/huggingface?_target_url=/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, start.StatusCode);

        var location = start.Headers.Location!;
        var query = QueryOf(location);

        Assert.Equal("https", location.Scheme);
        Assert.Equal("idp.test", location.Host);
        Assert.Equal("/authorize", location.AbsolutePath);
        Assert.Equal("code", query["response_type"]);
        Assert.Equal(ClientId, query["client_id"]);
        Assert.Equal("openid profile", query["scope"]);
        Assert.StartsWith(
            host.BaseAddress.GetLeftPart(UriPartial.Authority) + "/login/callback?",
            query["redirect_uri"],
            StringComparison.Ordinal);

        var state = query["state"];

        using var callback = await browser.GetAsync(
            $"/login/callback?code=code-1&state={Uri.EscapeDataString(state)}&_target_url=/",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, callback.StatusCode);
        Assert.Equal("/", callback.Headers.Location!.ToString());

        // The code was exchanged the way the protocol asks: a form post carrying the code, the same
        // redirect address, and the application's own credentials.
        var exchange = identity.Calls[0];
        var form = PairsOf(exchange.Body);

        Assert.Equal("POST", exchange.Method);
        Assert.Equal("https://idp.test/token", exchange.Address);
        Assert.Equal("authorization_code", form["grant_type"]);
        Assert.Equal("code-1", form["code"]);
        Assert.Equal(ClientId, form["client_id"]);
        Assert.Equal(ClientSecret, form["client_secret"]);
        Assert.Equal(query["redirect_uri"], form["redirect_uri"]);

        // And the token was used to ask who it belongs to.
        var profile = identity.Calls[1];

        Assert.Equal("GET", profile.Method);
        Assert.Equal("https://idp.test/userinfo", profile.Address);
        Assert.Equal("Bearer at-1", profile.Authorization);

        // The page now says who is signed in, on the button that would sign them out.
        Assert.Equal("Logout (alice)", ButtonLabel(await ConfigAsync(browser)));
    }

    [Fact]
    public async Task Signing_out_puts_the_button_back_and_drops_the_profile()
    {
        var identity = Identity();

        await using var host = await StartAsync(new OidcOAuthProvider(Options(), identity.Client()));
        using var browser = Browser(host);

        await SignInAsync(browser);

        Assert.Equal("Logout (alice)", ButtonLabel(await ConfigAsync(browser)));

        using var response = await browser.GetAsync("/logout?_target_url=/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("/", response.Headers.Location!.ToString());
        Assert.Equal(SignInLabel, ButtonLabel(await ConfigAsync(browser)));
    }

    [Fact]
    public async Task A_callback_whose_state_does_not_match_starts_the_sign_in_again()
    {
        await using var host = await StartAsync(new FakeProvider());
        using var browser = Browser(host);

        using var response = await browser.GetAsync(
            "/login/callback?code=code-1&state=not-the-one&_target_url=/",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);

        var location = response.Headers.Location!.ToString();

        // Back to the sign-in route, with the attempt counted — which is how a browser whose
        // cookies never work stops being sent round in circles.
        Assert.StartsWith("/login/huggingface?_nb_redirects=1", location, StringComparison.Ordinal);
        Assert.Contains("_target_url=", location, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_callback_that_has_been_sent_round_enough_times_is_let_through_to_the_page()
    {
        await using var host = await StartAsync(new FakeProvider());
        using var browser = Browser(host);

        // The Python version compares the count it was handed, so three failed attempts still redirect and the
        // fourth is let through. This is the fourth.
        using var response = await browser.GetAsync(
            "/login/callback?code=code-1&state=not-the-one&_nb_redirects=3&_target_url=/about",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("/about", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task An_expired_sign_in_is_dropped_from_the_session()
    {
        var clock = new ManualTimeProvider(DateTimeOffset.UnixEpoch.AddYears(56));

        await using var host = await StartAsync(
            new OidcOAuthProvider(Options(), Identity().Client(), clock),
            clock);

        using var browser = Browser(host);

        await SignInAsync(browser);

        Assert.Equal("Logout (alice)", ButtonLabel(await ConfigAsync(browser)));

        // The token was issued for an hour; an hour and a minute later it is not one anything
        // accepts, so the session stops describing a signed-in user.
        clock.Advance(TimeSpan.FromMinutes(61));

        Assert.Equal(SignInLabel, ButtonLabel(await ConfigAsync(browser)));
    }

    [Fact]
    public async Task The_page_a_sign_in_returns_to_is_a_path_on_this_application()
    {
        await using var host = await StartAsync(new FakeProvider());
        using var browser = Browser(host);

        using var start = await browser.GetAsync(
            "/login/huggingface?_target_url=//evil.example/x",
            TestContext.Current.CancellationToken);

        var state = QueryOf(start.Headers.Location!)["state"];

        using var callback = await browser.GetAsync(
            $"/login/callback?code=code-1&state={Uri.EscapeDataString(state)}&_target_url=//evil.example/x",
            TestContext.Current.CancellationToken);

        // The address names another host, so only its path survives: a sign-in cannot be used to
        // send the browser somewhere else afterwards.
        Assert.Equal("/x", callback.Headers.Location!.ToString());
    }

    [Fact]
    public async Task A_completed_sign_in_returns_to_the_page_it_started_from()
    {
        await using var host = await StartAsync(new FakeProvider());
        using var browser = Browser(host);

        using var start = await browser.GetAsync("/login/huggingface?_target_url=/about", TestContext.Current.CancellationToken);
        var state = QueryOf(start.Headers.Location!)["state"];

        using var callback = await browser.GetAsync(
            $"/login/callback?code=code-1&state={Uri.EscapeDataString(state)}&_target_url=/about",
            TestContext.Current.CancellationToken);

        Assert.Equal("/about", callback.Headers.Location!.ToString());
    }

    [Fact]
    public async Task An_application_without_a_sign_in_button_has_no_sign_in_routes()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Name" });
                }
            });

        foreach (var path in new[] { "/login/huggingface", "/login/callback?code=a&state=b" })
        {
            using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task A_sign_in_button_without_a_provider_says_what_is_missing()
    {
        await using var host = await StartAsync(provider: null);

        using var response = await host.Client.GetAsync("/login/huggingface", TestContext.Current.CancellationToken);

        // The Python version raises at launch instead; this framework cannot tell that a LoginButton was
        // rendered before the host is listening, so the failure is the first request's — and it is
        // a failure rather than a sign-in that silently does nothing.
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task The_discovery_document_is_read_when_the_provider_publishes_one()
    {
        var identity = new FakeService()
            .Answering(
                """
                {"authorization_endpoint":"https://idp.test/authorize",
                 "token_endpoint":"https://idp.test/token",
                 "userinfo_endpoint":"https://idp.test/userinfo"}
                """)
            .Answering(TokenAnswer)
            .Answering(UserInfoAnswer);

        var options = Options();
        options.AuthorizeEndpoint = null;
        options.TokenEndpoint = null;
        options.UserInfoEndpoint = null;
        options.MetadataUrl = new Uri("https://idp.test/.well-known/openid-configuration");

        await using var host = await StartAsync(new OidcOAuthProvider(options, identity.Client()));
        using var browser = Browser(host);

        await SignInAsync(browser);

        Assert.Equal("https://idp.test/.well-known/openid-configuration", identity.Calls[0].Address);
        Assert.Equal("Logout (alice)", ButtonLabel(await ConfigAsync(browser)));
    }

    [Fact]
    public async Task A_provider_that_refuses_the_exchange_leaves_nobody_signed_in()
    {
        var identity = new FakeService()
            .Refusing(HttpStatusCode.BadRequest, """{"error":"invalid_grant"}""");

        await using var host = await StartAsync(new OidcOAuthProvider(Options(), identity.Client()));
        using var browser = Browser(host);

        using var start = await browser.GetAsync("/login/huggingface", TestContext.Current.CancellationToken);
        var state = QueryOf(start.Headers.Location!)["state"];

        using var callback = await browser.GetAsync(
            $"/login/callback?code=stale&state={Uri.EscapeDataString(state)}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, callback.StatusCode);

        // Nothing was signed in: the page still offers to sign in.
        Assert.Equal(SignInLabel, ButtonLabel(await ConfigAsync(browser)));
    }

    /// <summary>An identity provider that answers a token exchange and a userinfo request.</summary>
    private static FakeService Identity() =>
        new FakeService()
            .Answering(TokenAnswer)
            .Answering(UserInfoAnswer);

    /// <summary>The options of a provider whose endpoints are named rather than discovered.</summary>
    private static OAuthProviderOptions Options() => new()
    {
        ClientId = ClientId,
        ClientSecret = ClientSecret,
        Scopes = "openid profile",
        AuthorizeEndpoint = new Uri("https://idp.test/authorize"),
        TokenEndpoint = new Uri("https://idp.test/token"),
        UserInfoEndpoint = new Uri("https://idp.test/userinfo"),
    };

    /// <summary>An application with a sign-in button on it.</summary>
    /// <param name="provider">The identity provider, or <see langword="null"/> for an application with none.</param>
    private static Task<GradioTestHost> StartAsync(IGradioOAuthProvider? provider) =>
        StartAsync(provider, clock: null);

    /// <summary>The same, on a clock the test moves.</summary>
    /// <param name="provider">The identity provider.</param>
    /// <param name="clock">The clock, or <see langword="null"/> for the system's.</param>
    private static Task<GradioTestHost> StartAsync(IGradioOAuthProvider? provider, TimeProvider? clock) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new LoginButton());
                    blocks.Add(new Textbox { Label = "Name" });
                }
            },
            services =>
            {
                if (clock is not null)
                {
                    services.AddSingleton(clock);
                }

                if (provider is not null)
                {
                    services.AddSingleton(provider);
                }
            });

    /// <summary>A client that keeps its cookies and does not follow redirects.</summary>
    /// <param name="host">The host to talk to.</param>
    private static HttpClient Browser(GradioTestHost host) =>
        new(new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = false,
        })
        {
            BaseAddress = host.BaseAddress,
        };

    /// <summary>Walks the whole flow, and fails the test if it does not complete.</summary>
    /// <param name="browser">The browser.</param>
    private static async Task SignInAsync(HttpClient browser)
    {
        using var start = await browser.GetAsync("/login/huggingface", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, start.StatusCode);

        var state = QueryOf(start.Headers.Location!)["state"];

        using var callback = await browser.GetAsync(
            $"/login/callback?code=code-1&state={Uri.EscapeDataString(state)}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, callback.StatusCode);
    }

    /// <summary>The config the application serves.</summary>
    /// <param name="browser">The browser asking.</param>
    private static async Task<JsonObject> ConfigAsync(HttpClient browser) =>
        JsonNode.Parse(await browser.GetStringAsync("/config", TestContext.Current.CancellationToken))!.AsObject();

    /// <summary>The label the sign-in button carries, which is what the user sees.</summary>
    /// <param name="config">The config.</param>
    /// <remarks>
    /// Found by its variant rather than its name: the button is the Python version's template
    /// component, so the config calls it a button and the variant is what makes it the sign-in one.
    /// </remarks>
    private static string ButtonLabel(JsonObject config) =>
        config["components"]!
            .AsArray()
            .Select(component => component!["props"]!.AsObject())
            .Single(props => props["variant"] is JsonValue variant && variant.ToString() == "huggingface")
            ["value"]!
            .GetValue<string>();

    /// <summary>The query parameters of an address.</summary>
    /// <param name="address">The address.</param>
    private static Dictionary<string, string> QueryOf(Uri address) => PairsOf(address.Query.TrimStart('?'));

    /// <summary>The pairs of a query string or a form body.</summary>
    /// <param name="encoded">The encoded text.</param>
    private static Dictionary<string, string> PairsOf(string encoded) =>
        encoded
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]),
                parts => parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty,
                StringComparer.Ordinal);

    /// <summary>A provider the tests write themselves, which is what makes the seam a seam.</summary>
    private sealed class FakeProvider : IGradioOAuthProvider
    {
        /// <inheritdoc />
        public Task<Uri> AuthorizeUrlAsync(
            string state,
            Uri redirectUri,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new Uri($"https://idp.test/authorize?state={Uri.EscapeDataString(state)}"));

        /// <inheritdoc />
        public Task<OAuthGrant> ExchangeAsync(string code, Uri redirectUri, CancellationToken cancellationToken) =>
            Task.FromResult(new OAuthGrant(
                "at-1",
                "bearer",
                "openid profile",
                null,
                null,
                new OAuthProfile("Alice Example", "alice", "https://idp.test/alice", "https://idp.test/alice.png")));
    }
}

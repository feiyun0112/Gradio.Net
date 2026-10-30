using System.Net;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// An application that asks someone to sign in: what an unauthenticated request gets, what signing
/// in does, and what signing out ends.
/// </summary>
/// <remarks>
/// <para>
/// Everything here is observed the way a browser observes it — a real host, a cookie jar, and the
/// routes the frontend calls — because that is the one seam this framework tests on (the testing
/// decisions in the spec). What is asserted is the documents and the statuses, not the code behind
/// them.
/// </para>
/// <para>
/// The refusals are asserted body and all, because they are what a client written against the
/// Python server reads: a 401 whose detail carries the application's message is the Python
/// version's answer, and a client that shows that message depends on the shape.
/// </para>
/// </remarks>
public sealed class AuthTests
{
    private const string AuthMessage = "Sign in to see the demo";
    private const string UserName = "alice";
    private const string Password = "s3cret";

    /// <summary>The keys the page's config carries when nobody has signed in, in order.</summary>
    private static readonly string[] AuthRequiredKeys =
    [
        "auth_required", "auth_message", "space_id", "root", "page", "pages", "components",
        "dependencies", "current_page",
    ];

    [Fact]
    public async Task An_unauthenticated_page_is_the_sign_in_form_rather_than_the_application()
    {
        await using var host = await StartAsync();

        var body = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        var config = ConfigOf(body);

        // The document says only that a sign-in is needed: no component of the application, no
        // binding, no title — nothing about what is behind the form.
        Assert.True(config["auth_required"]!.GetValue<bool>());
        Assert.Equal(AuthMessage, config["auth_message"]!.GetValue<string>());
        Assert.Empty(config["components"]!.AsArray());
        Assert.Empty(config["dependencies"]!.AsArray());
        Assert.Equal(AuthRequiredKeys, config.Select(pair => pair.Key));
    }

    [Fact]
    public async Task Every_route_that_describes_or_runs_the_application_refuses_an_unauthenticated_caller()
    {
        await using var host = await StartAsync();

        (string Method, string Path)[] guarded =
        [
            ("GET", "/config"),
            ("GET", "/gradio_api/info"),
            ("GET", "/gradio_api/openapi.json"),
            ("GET", "/gradio_api/deep_link?session_hash=a"),
            ("POST", "/gradio_api/upload"),
            ("POST", "/gradio_api/queue/join"),
            ("GET", "/gradio_api/queue/status"),
            ("GET", "/gradio_api/file=a.txt"),
        ];

        foreach (var (method, path) in guarded)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            using var response = await host.Client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(
                Refusal(),
                await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        }
    }

    /// <summary>
    /// The routes the table marks as guarded are the routes that actually refuse — every one of
    /// them, walked from the table rather than from a list beside it.
    /// </summary>
    /// <remarks>
    /// This is the invariant the guard is: the set the Python version attaches <c>login_check</c>
    /// to is declared on the route, and a route whose declaration is misspelled, dropped, or never
    /// wired into the mapping loop answers something other than 401 and fails here. The page and
    /// its assets are asserted unguarded in the same breath, because guarding them would hide the
    /// form that fixes it.
    /// </remarks>
    [Fact]
    public async Task Every_route_the_table_guards_refuses_an_unauthenticated_caller()
    {
        await using var host = await StartAsync();

        var table = host.Services.GetRequiredService<GradioRouteTable>();
        var guarded = table.Routes.Where(route => route.RequiresLogin).ToList();

        Assert.NotEmpty(guarded);

        foreach (var route in guarded)
        {
            using var request = new HttpRequestMessage(
                new HttpMethod(route.Method),
                RouteSampleUrl.For(route.Path));

            using var response = await host.Client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                $"{route.Method} {route.Path} must answer 401 to a caller who has not signed in; it answered "
                + $"{(int)response.StatusCode}.");
        }

        // The page carries the sign-in form, and its assets are what draws it: neither may be
        // guarded, or the form could never be reached.
        string[] reachable =
        [
            "/", "/favicon.ico", "/theme.css", "/assets/{path:path}", "/gradio_api/user",
            "/gradio_api/login_check", "/gradio_api/token", "/gradio_api/app_id", "/login",
            "/logout", "/gradio_api/heartbeat/{session_hash}", "/gradio_api/cancel", "/gradio_api/runs",
        ];

        foreach (var path in reachable)
        {
            var route = table.Routes.Single(candidate => candidate.Path == path && candidate.Method != "HEAD");

            Assert.False(route.RequiresLogin, $"{route.Method} {route.Path} must not be guarded.");
        }
    }

    [Fact]
    public async Task An_application_that_asks_nobody_to_sign_in_answers_everyone()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Open);

        using var response = await host.Client.GetAsync("/config", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var config = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();

        // Nobody is signed in, and the document says so rather than leaving the key out.
        Assert.Null(config["username"]);
        Assert.Equal("null", await host.Client.GetStringAsync("/gradio_api/user", TestContext.Current.CancellationToken));
        Assert.Equal(
            "null",
            await host.Client.GetStringAsync("/gradio_api/login_check", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Signing_in_answers_success_and_sets_the_two_cookies_upstream_sets()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        using var response = await browser.PostAsync(
            "/login",
            Form(UserName, Password),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            """{"success":true}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var cookies = response.Headers.GetValues("Set-Cookie").ToArray();

        // Both are written: one marked for cross-site use, and a plain one for a browser that
        // refuses the first. The name carries the application's own identifier.
        Assert.Equal(2, cookies.Length);
        Assert.Contains(cookies, cookie => cookie.StartsWith("access-token-", StringComparison.Ordinal));
        Assert.Contains(cookies, cookie => cookie.StartsWith("access-token-unsecure-", StringComparison.Ordinal));
        Assert.All(cookies, cookie => Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task A_signed_in_browser_is_let_through_and_told_who_it_is()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        await SignInAsync(browser);

        var config = JsonNode.Parse(
            await browser.GetStringAsync("/config", TestContext.Current.CancellationToken))!.AsObject();

        Assert.Equal(UserName, config["username"]!.GetValue<string>());

        Assert.Equal(
            $"\"{UserName}\"",
            await browser.GetStringAsync("/gradio_api/user", TestContext.Current.CancellationToken));

        Assert.Equal(
            "null",
            await browser.GetStringAsync("/gradio_api/login_check", TestContext.Current.CancellationToken));

        var token = JsonNode.Parse(
            await browser.GetStringAsync("/gradio_api/token", TestContext.Current.CancellationToken))!.AsObject();

        Assert.False(string.IsNullOrEmpty(token["token"]!.GetValue<string>()));
        Assert.Equal(UserName, token["user"]!.GetValue<string>());
    }

    [Fact]
    public async Task Signing_out_ends_the_session_and_sends_the_browser_back_to_the_page()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        await SignInAsync(browser);

        using var response = await browser.GetAsync("/logout", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal(host.BaseAddress.ToString(), response.Headers.Location!.ToString());

        // The token the browser held is gone from the server as well, so a copy of the cookie
        // would be worth nothing.
        using var afterwards = await browser.GetAsync("/config", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, afterwards.StatusCode);
    }

    [Fact]
    public async Task Wrong_credentials_are_refused_with_upstreams_answer()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        using var response = await browser.PostAsync(
            "/login",
            Form(UserName, "wrong"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            """{"detail":"Incorrect credentials."}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_sign_in_that_names_no_credentials_is_refused_the_way_fastapi_refuses_it()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        using var response = await browser.PostAsync(
            "/login",
            new StringContent(string.Empty),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(
            """{"detail":[{"type":"missing","loc":["body","username"],"msg":"Field required","input":null},"""
            + """{"type":"missing","loc":["body","password"],"msg":"Field required","input":null}]}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_sign_in_that_sends_empty_credentials_is_a_wrong_pair_rather_than_a_malformed_request()
    {
        await using var host = await StartAsync();
        using var browser = Browser(host);

        using var response = await browser.PostAsync(
            "/login",
            Form(string.Empty, string.Empty),
            TestContext.Current.CancellationToken);

        // The fields are there, so FastAPI's validation passes and the rule is what refuses them.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            """{"detail":"Incorrect credentials."}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Posting_the_form_to_an_application_that_asks_nobody_to_sign_in_sends_the_browser_back()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Open);
        using var browser = Browser(host);

        using var response = await browser.PostAsync(
            "/login",
            Form(UserName, Password),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal(host.BaseAddress.ToString(), response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task A_rule_that_decides_for_itself_admits_and_refuses()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.Verify((name, password) => name == "bob" && password == "hunter2");

                Page(blocks);
            });

        using var browser = Browser(host);

        using var refused = await browser.PostAsync(
            "/login",
            Form("bob", "wrong"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, refused.StatusCode);

        using var accepted = await browser.PostAsync(
            "/login",
            Form("bob", "hunter2"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, accepted.StatusCode);
    }

    [Fact]
    public async Task The_app_id_is_answered_to_anyone_because_the_page_asks_before_it_can_sign_in()
    {
        await using var host = await StartAsync();

        var body = JsonNode.Parse(
            await host.Client.GetStringAsync("/gradio_api/app_id", TestContext.Current.CancellationToken))!.AsObject();

        Assert.Equal(
            host.Services.GetRequiredService<Blocks>().AppId,
            body["app_id"]!.GetValue<long>());
    }

    [Fact]
    public void An_application_cannot_defer_to_its_host_and_hold_accounts_at_once()
    {
        var blocks = new Blocks(new RenderContextAccessor())
        {
            Auth = GradioAuth.OneAccount(UserName, Password),
        };

        // The Python version refuses the pair at launch for the same reason: one question, two answers.
        Assert.Throws<InvalidOperationException>(
            () => blocks.AuthDependency = (_, _) => Task.FromResult<string?>("alice"));
    }

    /// <summary>
    /// A handler is told who is calling, which is what an application that answers differently to
    /// different people needs.
    /// </summary>
    /// <remarks>
    /// The name travels with the run rather than with the request: a queued run outlives the
    /// connection that admitted it, so the queue records who asked at join and hands that to the
    /// handler. The Python version's <c>request.username</c> is the same value, and a handler asks
    /// for it here by declaring a <see cref="Request"/> — a .NET signature's way of saying
    /// <c>request: gr.Request</c>.
    /// </remarks>
    [Fact]
    public async Task A_handler_reads_who_is_calling()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cookies = new CookieContainer();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.OneAccount(UserName, Password);

                Greeter(blocks);
            });

        using var browser = Browser(host, cookies);
        await SignInAsync(browser);

        // The queue client shares the browser's jar, so the join is made by the caller who signed
        // in rather than by a stranger.
        using var queue = new QueueClient(host.BaseAddress, cookies);
        var fnIndex = await queue.EndpointAsync("greet", cancellationToken);

        await queue.JoinAsync(fnIndex, ["Ada"], "greet-session", cancellationToken);

        await using var stream = await queue.OpenAsync("greet-session", cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.Equal(
            "alice greets Ada in greet-session",
            completed!["output"]!["data"]![0]!.GetValue<string>());
    }

    /// <summary>
    /// A handler reads the name the host's own authentication produced, and the host is asked once
    /// per request however many layers want to know.
    /// </summary>
    /// <remarks>
    /// An application that defers to its host has no sign-in of its own, so the name comes from the
    /// dependency the host wrote — here, a header its middleware would have read. The count is part
    /// of the claim: the gate asks before the route runs and the run asks again to tell the handler,
    /// and a dependency that counts its calls must not see one request twice.
    /// </remarks>
    [Fact]
    public async Task A_handler_reads_the_name_the_host_authenticated()
    {
        var asked = 0;
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                blocks.AuthDependency = (context, _) =>
                {
                    Interlocked.Increment(ref asked);

                    return Task.FromResult(context.Request.Headers["X-User"].FirstOrDefault());
                };

                Greeter(blocks);
            });

        using var request = new HttpRequestMessage(HttpMethod.Post, "/gradio_api/run/greet")
        {
            Content = new StringContent(
                """{"data":["Ada"],"session_hash":"direct-session"}""",
                System.Text.Encoding.UTF8,
                "application/json"),
        };

        request.Headers.Add("X-User", "bob");

        using var response = await host.Client.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "bob greets Ada in direct-session",
            await response.Content.ReadAsStringAsync(cancellationToken),
            StringComparison.Ordinal);
        Assert.Equal(1, Volatile.Read(ref asked));
    }

    /// <summary>
    /// An application that asks nobody to sign in still hands its handler a request, with nobody in
    /// it — which is the Python version's shape, where the object is always there and its name may
    /// be empty.
    /// </summary>
    [Fact]
    public async Task A_handler_in_an_application_that_asks_nobody_reads_a_request_with_no_name()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Greeter(blocks));

        using var request = new HttpRequestMessage(HttpMethod.Post, "/gradio_api/run/greet")
        {
            Content = new StringContent(
                """{"data":["Ada"],"session_hash":"open-session"}""",
                System.Text.Encoding.UTF8,
                "application/json"),
        };

        using var response = await host.Client.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "nobody greets Ada in open-session",
            await response.Content.ReadAsStringAsync(cancellationToken),
            StringComparison.Ordinal);
    }

    /// <summary>
    /// A validator is told who is calling, as the handler it guards is.
    /// </summary>
    /// <remarks>
    /// The Python version hands a validator the same request the handler gets, which is what lets
    /// one refuse an input on a rule about the person rather than about the value. The validator
    /// here accepts only when it can see the name, so a run that admitted it is a run that carried
    /// one.
    /// </remarks>
    [Fact]
    public async Task A_validator_reads_who_is_calling()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cookies = new CookieContainer();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.OneAccount(UserName, Password);

                Greeter(
                    blocks,
                    (Func<string, Request, object?>)((_, request) =>
                        request.UserName == UserName ? null : "only alice may greet"));
            });

        using var browser = Browser(host, cookies);
        await SignInAsync(browser);

        using var queue = new QueueClient(host.BaseAddress, cookies);
        var fnIndex = await queue.EndpointAsync("greet", cancellationToken);

        // Joining throws when the queue refuses the run, so this completing is the validator having
        // accepted it — which it only does for the name it was given.
        var eventId = await queue.JoinAsync(fnIndex, ["Ada"], "validator-session", cancellationToken);

        Assert.False(string.IsNullOrEmpty(eventId));
    }

    /// <summary>An application whose one binding answers with who is calling and what they sent.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="validator">What decides whether a run may start, or <see langword="null"/> for none.</param>
    private static void Greeter(Blocks blocks, Delegate? validator = null)
    {
        Textbox name;
        Textbox answer;
        Button greet;

        using (blocks.BeginRender())
        {
            name = blocks.Add(new Textbox { Label = "Name" });
            answer = blocks.Add(new Textbox { Label = "Answer" });
            greet = blocks.Add(new Button { Value = "Greet" });
        }

        greet.Click(
            (Func<string, Request, string>)((value, request) =>
                $"{request.UserName ?? "nobody"} greets {value} in {request.SessionHash}"),
            [name],
            [answer],
            apiName: "greet", validator: validator);
    }

    /// <summary>An application with a field on it, which is all a page needs to be worth protecting.</summary>
    private static void Page(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "Name" });
        }
    }

    /// <summary>The same application, with nobody asked to sign in.</summary>
    private static void Open(Blocks blocks) => Page(blocks);

    /// <summary>A protected application whose files go somewhere of its own.</summary>
    private static Task<GradioTestHost> StartAsync() =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.OneAccount(UserName, Password);
                blocks.AuthMessage = AuthMessage;

                Page(blocks);
            });

    /// <summary>A client that keeps its cookies and does not follow redirects.</summary>
    /// <param name="host">The host to talk to.</param>
    /// <param name="cookies">
    /// The jar to keep them in, or <see langword="null"/> for a client with one of its own. A test
    /// that needs a second client to be the same caller shares the jar.
    /// </param>
    /// <returns>The browser.</returns>
    /// <remarks>
    /// Redirects are not followed because two of these routes answer with one, and what they answer
    /// with is the subject; a browser that followed them would report where they led instead.
    /// </remarks>
    private static HttpClient Browser(GradioTestHost host, CookieContainer? cookies = null) =>
        new(new HttpClientHandler
        {
            CookieContainer = cookies ?? new CookieContainer(),
            AllowAutoRedirect = false,
        })
        {
            BaseAddress = host.BaseAddress,
        };

    /// <summary>Signs the browser in, and fails the test if the application does not accept it.</summary>
    /// <param name="browser">The browser.</param>
    private static async Task SignInAsync(HttpClient browser)
    {
        using var response = await browser.PostAsync(
            "/login",
            Form(UserName, Password),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>The sign-in form's body.</summary>
    /// <param name="userName">The username to submit.</param>
    /// <param name="password">The password to submit.</param>
    private static FormUrlEncodedContent Form(string userName, string password) =>
        new([
            new KeyValuePair<string, string>("username", userName),
            new KeyValuePair<string, string>("password", password),
        ]);

    /// <summary>The body a refusal carries, which is the Python version's 401 detail.</summary>
    private static string Refusal() =>
        """{"detail":{"error":"Not authenticated","auth_message":"AUTH_MESSAGE"}}"""
            .Replace("AUTH_MESSAGE", AuthMessage, StringComparison.Ordinal);

    /// <summary>The config a page carries.</summary>
    /// <param name="body">The page.</param>
    private static JsonObject ConfigOf(string body)
    {
        _ = EmbeddedDocuments.Replace(
            body,
            EmbeddedDocuments.ConfigMarker,
            EmbeddedDocuments.ConfigPlaceholder,
            out var document);

        Assert.NotNull(document);

        return document!.AsObject();
    }
}

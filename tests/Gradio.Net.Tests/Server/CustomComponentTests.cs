using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using Textbox = Gradio.Net.Textbox;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// A component an application defines for itself, observed the two ways it exists: what the config
/// says about it, and what the route serves for it.
/// </summary>
/// <remarks>
/// <para>
/// The two halves are one extension point. A component that brings its own frontend is a
/// class of the application's own — its own type, its own props, its own reading and writing of
/// values — plus an identity and a directory, and the identity is the only thing in its config
/// entry a built-in does not carry. Everything else about the entry is the framework's, which is
/// what makes the frontend able to draw it without having heard of it.
/// </para>
/// <para>
/// The route is the second half, and it is the Python version's:
/// <c>{id}/{environment}/{type}/{file_name}</c>, where the identity is resolved against the
/// application's own components and the last two segments are a path inside the directory the
/// component named. Its refusals were checked against the real 6.30.0 server rather than read off
/// its source, because the two disagree: FastAPI validates the environment segment before the
/// handler sees it, and the missing file and the directory come from Starlette rather than from the
/// route's own code.
/// </para>
/// </remarks>
public sealed class CustomComponentTests
{
    /// <summary>The endpoint name the test application binds its one event under.</summary>
    private const string TotalName = "total";

    /// <summary>
    /// The identity a component declares is what the browser asks for its bundle by, and it is the
    /// only thing that differs from a built-in's entry.
    /// </summary>
    /// <remarks>
    /// The comparison is against the textbox in the same application rather than against a written
    /// list of keys: "enters the config the way a built-in does" is a claim about the two entries
    /// being built the same way, and a list would keep passing after a key was added to one of them.
    /// </remarks>
    [Fact]
    public async Task A_component_that_brings_its_own_frontend_enters_the_config_like_a_built_in()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle), caption: "Requests per second");

        var components = Assert.IsType<JsonArray>((await ConfigAsync(host))["components"]);
        var sparkline = Entry(components, "sparkline");
        var textbox = Entry(components, "textbox");

        Assert.Equal(KeysOf(textbox), KeysOf(sparkline));

        // The identity the browser resolves the bundle by, and the absence of one for a component
        // the frontend already knows.
        Assert.Equal(Sparkline.Identity, sparkline["component_class_id"]!.GetValue<string>());
        Assert.Null(textbox["component_class_id"]);

        // The name the frontend's loader looks for in its registry first, which is the class name
        // as the Python version spells it, and the props the component writes for itself.
        Assert.Equal("sparkline", sparkline["props"]!["name"]!.GetValue<string>());
        Assert.Equal("Requests per second", sparkline["props"]!["caption"]!.GetValue<string>());
        Assert.Equal("string", sparkline["api_info"]!["type"]!.GetValue<string>());
        Assert.False(sparkline["skip_api"]!.GetValue<bool>());
    }

    /// <summary>
    /// The bundle is served from the address the vendored frontend asks for, with the three headers
    /// it revalidates on.
    /// </summary>
    /// <remarks>
    /// Both environments are asked because the Python version serves both from one path: the
    /// browser asks for <c>client</c> and the server-side renderer for <c>server</c>, and the
    /// segment names which of them is asking rather than a different file.
    /// </remarks>
    [Theory]
    [InlineData("client")]
    [InlineData("server")]
    public async Task The_bundle_is_served_from_the_address_the_frontend_asks_for(string environment)
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var response = await host.Client.GetAsync(
            BundleUrl(environment, "component", "index.js"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            Sparkline.Script,
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        // What the browser checks before it uses the file: the type, the hash it will revalidate
        // with, and the instruction to revalidate rather than to keep it. The two cache directives
        // are read as directives rather than as text, because ASP.NET Core rewrites their order and
        // the order is not what the browser acts on.
        Assert.Equal("application/javascript", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal(Sha256(Sparkline.Script), Header(response, "ETag"));
        Assert.True(response.Headers.CacheControl!.MustRevalidate);
        Assert.Equal(TimeSpan.Zero, response.Headers.CacheControl.MaxAge);
    }

    /// <summary>A stylesheet in the bundle is sent as a stylesheet, which is what makes it apply.</summary>
    [Fact]
    public async Task A_file_in_the_bundle_is_sent_as_the_type_its_extension_names()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var response = await host.Client.GetAsync(
            BundleUrl("client", "component", "style.css"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            Sparkline.Stylesheet,
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("text/css", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType.CharSet);
    }

    /// <summary>
    /// A bundle the browser already has is answered with 304, and only when the hash is the file's.
    /// </summary>
    /// <remarks>
    /// The Python version revalidates on the file's contents rather than on a date, which is why
    /// the header is a hash and why <c>max-age=0</c> accompanies it: a rebuilt bundle has a
    /// different hash, so a stale copy is never used. A request carrying a hash that is not this
    /// file's is a request for the file, and gets it.
    /// </remarks>
    [Fact]
    public async Task A_bundle_the_browser_already_has_is_not_sent_again()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));
        var url = BundleUrl("client", "component", "index.js");

        using var stale = await GetWithAsync(host, url, Sha256("something else"));
        Assert.Equal(HttpStatusCode.OK, stale.StatusCode);

        using var current = await GetWithAsync(host, url, Sha256(Sparkline.Script));
        Assert.Equal(HttpStatusCode.NotModified, current.StatusCode);
        Assert.Empty(await current.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// An environment that is neither of the two the Python version serves is refused as a
    /// validation failure, not as a missing component.
    /// </summary>
    /// <remarks>
    /// The route declares the segment as <c>Literal["client", "server"]</c>, so FastAPI answers
    /// before the handler runs and the handler's own <c>if environment not in [...]</c> is dead
    /// code. What a client sees is pydantic's document, which is why this asserts the document
    /// rather than a sentence: a client written against the Python server parses it.
    /// </remarks>
    [Theory]
    [InlineData("browser")]
    [InlineData("Client")]
    public async Task An_environment_upstream_does_not_serve_is_refused_as_a_validation_failure(string environment)
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var response = await host.Client.GetAsync(
            BundleUrl(environment, "component", "index.js"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(
            "{\"detail\":[{\"type\":\"literal_error\",\"loc\":[\"path\",\"environment\"],"
            + $"\"msg\":\"Input should be 'client' or 'server'\",\"input\":\"{environment}\","
            + "\"ctx\":{\"expected\":\"'client' or 'server'\"}}]}",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// An identity no component of the application claims is not found, which is also what a path
    /// that would leave the bundle gets.
    /// </summary>
    [Fact]
    public async Task An_identity_no_component_claims_is_not_found()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var response = await host.Client.GetAsync(
            "/gradio_api/custom_component/not-a-component/client/component/index.js",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(
            """{"detail":"Component not found."}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// A file the bundle does not have is answered by the framework's own miss, which is a
    /// different sentence from an unknown component.
    /// </summary>
    /// <remarks>
    /// The Python version resolves the component first and the file second, and the two refusals
    /// come from two different layers: the component's from the route, the file's from the
    /// framework's file response. Both are 404s, and a client that reads <c>detail</c> can tell
    /// them apart.
    /// </remarks>
    [Theory]
    [InlineData("component", "missing.js")]
    [InlineData("example", "index.js")]
    public async Task A_file_the_bundle_does_not_have_is_not_found(string type, string fileName)
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var response = await host.Client.GetAsync(
            BundleUrl("client", type, fileName),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(
            """{"detail":"Not Found"}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>A request naming a directory in the bundle is forbidden rather than served.</summary>
    /// <remarks>
    /// A directory is not a file a browser can use, and the Python version says so with a 403
    /// rather than letting the read fail. It matters that this is not the "no such component"
    /// answer: the request named something the bundle has.
    /// </remarks>
    [Fact]
    public async Task A_request_that_names_a_directory_is_forbidden()
    {
        using var bundle = new TempFolder();
        var templates = Sparkline.WriteBundle(bundle);
        Directory.CreateDirectory(Path.Combine(templates, "component", "nested"));

        await using var host = await StartAsync(templates);

        using var response = await host.Client.GetAsync(
            BundleUrl("client", "component", "nested"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(
            """{"detail":"Forbidden"}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Nothing outside the directory the component named is served, however the request is written.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The file is planted where a traversing path would reach — one level above the directory the
    /// component named, which is where the Python version's <c>TEMPLATE_DIR</c> sits inside a
    /// package — so the answer is evidence about the path rather than about a file that is not
    /// there.
    /// </para>
    /// <para>
    /// Both attempts are refused before the route's own guard is reached, which is worth knowing
    /// when reading this: this framework collapses a dot segment out of the path before any route is
    /// matched, and a colon names nothing on any platform this runs on. What the test states is the
    /// guarantee a client can observe — nothing outside the bundle comes back — and the test below
    /// is the one that exercises the guard itself.
    /// </para>
    /// </remarks>
    [Theory]
    [InlineData("%2e%2e", "secret.js")]
    [InlineData("component", "c%3Asecret.js")]
    public async Task Nothing_outside_the_component_directory_is_served(string type, string fileName)
    {
        using var bundle = new TempFolder();
        await System.IO.File.WriteAllTextAsync(
            Path.Combine(bundle.Folder, "secret.js"),
            "PLANTED",
            TestContext.Current.CancellationToken);

        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        var answer = await SendRawAsync(
            host.BaseAddress,
            $"/gradio_api/custom_component/{Sparkline.Identity}/client/{type}/{fileName}");

        Assert.Equal(404, answer.Status);
        Assert.DoesNotContain("PLANTED", answer.Body, StringComparison.Ordinal);
    }

    /// <summary>
    /// A backslash inside a segment is refused by the route rather than resolved, which is the check
    /// the test above cannot exercise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A bundle built on Windows names directories with a backslash, so a bundle served from Linux
    /// can contain one, and the same request would climb on Windows and name a directory on Linux.
    /// Both are refused, which is this implementation's addition to the Python version's
    /// <c>safe_join</c>; a colon is refused for the same reason and is covered above.
    /// </para>
    /// <para>
    /// The answer is the component's own rather than a miss, which is what says the route saw the
    /// request and refused it. On Windows this is also the case that would serve the file if the
    /// check were dropped, because a trailing backslash is a separator there.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_backslash_inside_a_segment_is_refused_by_the_route()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        var answer = await SendRawAsync(
            host.BaseAddress,
            $"/gradio_api/custom_component/{Sparkline.Identity}/client/component%5c/index.js");

        Assert.Equal(404, answer.Status);
        Assert.Equal("""{"detail":"Component not found."}""", answer.Body);
    }

    /// <summary>
    /// The component's own reading and writing of values run when a client calls its endpoint.
    /// </summary>
    /// <remarks>
    /// The value crosses the wire as text and reaches the handler as numbers, which only the
    /// component's own <c>Preprocess</c> can do; the handler's number comes back as the drawing the
    /// component's own <c>Postprocess</c> writes, which the base implementation would have sent as a
    /// number. Both halves are asserted in one call because they are one conversion.
    /// </remarks>
    [Fact]
    public async Task The_component_reads_and_writes_values_through_its_own_two_hooks()
    {
        using var bundle = new TempFolder();
        await using var host = await StartAsync(Sparkline.WriteBundle(bundle));

        using var content = new StringContent("""{"data":["1,2,3"]}""", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync(
            $"/gradio_api/run/{TotalName}",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var answer = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        // Read as a value rather than as text: the document escapes the markup, which is what a
        // JSON body does to a string containing '<'.
        Assert.Equal("<svg data-total=\"6\"/>", answer["data"]![0]!.GetValue<string>());
    }

    /// <summary>Starts an application holding one custom component and one built-in.</summary>
    /// <param name="templates">The directory the custom component's bundle was written into.</param>
    /// <param name="caption">The custom component's own parameter, when the test asserts it.</param>
    /// <returns>The running host.</returns>
    private static Task<GradioTestHost> StartAsync(string templates, string? caption = null) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    var sparkline = blocks.Add(new Sparkline(templates) { Caption = caption });

                    blocks.Add(new Textbox { Label = "Anything" });

                    // The object[] fallback, which is how an application binds an event of a
                    // component no generated event method exists for.
                    blocks.On(
                        [new EventTrigger(sparkline, "change")],
                        new EventBindingOptions
                        {
                            Fn = new Func<int[], int>(Total),
                            Inputs = [sparkline],
                            Outputs = [sparkline],
                            ApiName = TotalName,
                        });
                }
            });

    /// <summary>Adds up the numbers the component read.</summary>
    /// <param name="numbers">The numbers.</param>
    /// <returns>Their sum.</returns>
    private static int Total(int[] numbers) => numbers.Sum();

    /// <summary>The address of one file in a component's bundle.</summary>
    /// <param name="environment">Which side is asking.</param>
    /// <param name="type">The variant directory.</param>
    /// <param name="fileName">The file within it.</param>
    /// <returns>The path, with the identity the test's component declares.</returns>
    private static string BundleUrl(string environment, string type, string fileName) =>
        $"/gradio_api/custom_component/{Sparkline.Identity}/{environment}/{type}/{fileName}";

    /// <summary>Reads the served config.</summary>
    /// <param name="host">The running application.</param>
    /// <returns>The document.</returns>
    private static async Task<JsonObject> ConfigAsync(GradioTestHost host) =>
        JsonNode.Parse(
            await host.Client.GetStringAsync("/config", TestContext.Current.CancellationToken))!.AsObject();

    /// <summary>The config entry of the one component of a type.</summary>
    /// <param name="components">The config's component list.</param>
    /// <param name="type">The type to find.</param>
    /// <returns>The entry.</returns>
    private static JsonObject Entry(JsonArray components, string type) =>
        components.Single(component => component!["type"]!.GetValue<string>() == type)!.AsObject();

    /// <summary>The keys of a config entry, sorted, for comparing two entries' shape.</summary>
    /// <param name="entry">The entry.</param>
    /// <returns>The keys.</returns>
    private static string[] KeysOf(JsonObject entry) =>
        [.. entry.Select(pair => pair.Key).OrderBy(key => key, StringComparer.Ordinal)];

    /// <summary>Requests one address with a hash the browser already has.</summary>
    /// <param name="host">The running application.</param>
    /// <param name="url">The address.</param>
    /// <param name="etag">The hash to send.</param>
    /// <returns>The answer.</returns>
    private static async Task<HttpResponseMessage> GetWithAsync(GradioTestHost host, string url, string etag)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("If-None-Match", etag);

        return await host.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    /// <summary>A header's single value.</summary>
    /// <param name="response">The answer.</param>
    /// <param name="name">The header's name.</param>
    /// <returns>The value.</returns>
    private static string Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values)
            ? values.Single()
            : throw new InvalidOperationException($"The answer carries no '{name}' header.");

    /// <summary>Sends a GET whose target is written exactly as given.</summary>
    /// <param name="address">Where the server is listening.</param>
    /// <param name="target">The request target, percent-escapes and all.</param>
    /// <returns>The status, and the body as it arrived.</returns>
    /// <remarks>
    /// A raw request rather than <see cref="HttpClient"/>, which normalizes the path before it sends
    /// it: a segment reading <c>..</c> is collapsed by the client, so a test written through it would
    /// be testing the client's tidiness rather than the route's refusal. What a non-normalizing
    /// client sends is what the route has to answer, and this is that.
    /// </remarks>
    private static async Task<(int Status, string Body)> SendRawAsync(Uri address, string target)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(address.Host, address.Port, TestContext.Current.CancellationToken);

        await using var stream = client.GetStream();

        var head = Encoding.ASCII.GetBytes(
            $"GET {target} HTTP/1.1\r\nHost: {address.Authority}\r\nConnection: close\r\n\r\n");

        await stream.WriteAsync(head, TestContext.Current.CancellationToken);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var answer = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        var split = answer.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        Assert.True(split > 0, $"The answer was not an HTTP response: '{answer}'");

        var statusLine = answer[..answer.IndexOf("\r\n", StringComparison.Ordinal)].Split(' ');

        return (int.Parse(statusLine[1], CultureInfo.InvariantCulture), answer[(split + 4)..]);
    }

    /// <summary>The hash the route revalidates a file by.</summary>
    /// <param name="content">The file's text.</param>
    /// <returns>The lowercase hexadecimal SHA-256 of its bytes.</returns>
    private static string Sha256(string content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
}

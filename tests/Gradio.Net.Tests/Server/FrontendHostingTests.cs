using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Gradio.Net;
using Gradio.Net.Tests.Shared;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The page a browser receives and everything it loads, observed over HTTP against a real host.
/// </summary>
/// <remarks>
/// <para>
/// This is the protocol layer's half of the seam: the page is the Python version's own template
/// filled with this application's config, and the assets are the vendored tree served byte for
/// byte, so the compiled frontend runs here exactly as it runs against the Python version. What is
/// asserted here is that the page carries the config, that every asset in the vendored tree comes
/// back unchanged, and that a path that is not an asset is refused the way the Python version
/// refuses it.
/// </para>
/// <para>
/// Nothing here needs Python, and nothing needs the frontend beside the test binary: the assets are
/// embedded in the assembly, which is what makes a package reference, a publish and a
/// single-file deployment all carry the frontend.
/// </para>
/// </remarks>
public sealed partial class FrontendHostingTests
{
    private const string PageContentType = "text/html; charset=utf-8";

    [Fact]
    public async Task The_page_is_upstreams_page_carrying_this_applications_config()
    {
        await using var host = await StartAsync();
        using var response = await host.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(PageContentType, response.Content.Headers.ContentType?.ToString());

        var page = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // The document the browser boots from: the Python version's template, its doctype and its element
        // untouched. A page that had been rewritten on the way out would not be the same client
        // code running against this server.
        Assert.StartsWith("<!doctype html>", page, StringComparison.Ordinal);
        Assert.Contains("<base href=\"./\" />", page, StringComparison.Ordinal);
        Assert.Contains("<gradio-app", page, StringComparison.Ordinal);
        Assert.EndsWith("</html>", page, StringComparison.Ordinal);

        // Jinja drops the newline a template file ends with, so the rendered page has none either.
        Assert.DoesNotContain("</html>\n", page, StringComparison.Ordinal);

        var config = Embedded(page, EmbeddedDocuments.ConfigMarker).AsObject();
        Assert.Equal(ManifestPage.Title, config["title"]!.GetValue<string>());
        Assert.Equal(GradioVersion.Upstream, config["version"]!.GetValue<string>());

        // The four fields the page adds to the config it narrows: which page is being rendered,
        // who is asking, whether the link was a deep link, and where the page is served from.
        Assert.Equal(string.Empty, config["current_page"]!.GetValue<string>());
        Assert.Null(config["username"]);
        Assert.Equal("none", config["deep_link_state"]!.GetValue<string>());
        Assert.Equal(host.BaseAddress.GetLeftPart(UriPartial.Authority), config["root"]!.GetValue<string>());

        // The API info is embedded beside it, because the page's own script reads both.
        var apiInfo = Embedded(page, EmbeddedDocuments.ApiInfoMarker).AsObject();
        Assert.NotNull(apiInfo["named_endpoints"]);
        Assert.NotNull(apiInfo["unnamed_endpoints"]);
    }

    [Fact]
    public async Task The_page_answers_head_with_the_size_it_would_have_sent()
    {
        await using var host = await StartAsync();

        using var get = await host.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var body = await get.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        using var head = await host.Client.SendAsync(
            new HttpRequestMessage(HttpMethod.Head, "/"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, head.StatusCode);
        Assert.Equal(PageContentType, head.Content.Headers.ContentType?.ToString());
        Assert.Equal(body.Length, head.Content.Headers.ContentLength);

        // A HEAD response has no body: what it carries is the size, which is what a client asks
        // for when it wants to know whether the page changed.
        Assert.Empty(await head.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task The_assets_the_page_names_are_served_with_the_types_a_browser_needs()
    {
        await using var host = await StartAsync();

        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);
        var referenced = ReferencedAssets(page);

        // The page names its bundle and its stylesheet, and loads the resizer from the static
        // directory; all three have to resolve or the frontend does not start.
        Assert.Contains(referenced, path => path.EndsWith(".js", StringComparison.Ordinal));
        Assert.Contains(referenced, path => path.EndsWith(".css", StringComparison.Ordinal));

        foreach (var path in referenced)
        {
            using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(response.Content.Headers.ContentType?.MediaType));
            Assert.NotEqual(0, response.Content.Headers.ContentLength);
        }

        Assert.Equal("application/javascript", await Responses.MediaTypeOfAsync(host, referenced.First(path => path.EndsWith(".js", StringComparison.Ordinal))));
        Assert.Equal("text/css", await Responses.MediaTypeOfAsync(host, referenced.First(path => path.EndsWith(".css", StringComparison.Ordinal))));
        Assert.Equal("application/javascript", await Responses.MediaTypeOfAsync(host, "/static/js/iframeResizer.contentWindow.min.js"));
        Assert.Equal("image/svg+xml", await Responses.MediaTypeOfAsync(host, "/static/img/logo.svg"));
    }

    [Fact]
    public async Task Every_asset_in_the_vendored_tree_is_served_byte_for_byte()
    {
        var record = VendoredTree.Frontend.HashTree();
        var served = record
            .Where(entry => entry.Key.StartsWith("assets/", StringComparison.Ordinal)
                || entry.Key.StartsWith("static/", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(served);

        // Read once: a path that was refused has two possible causes, and telling them apart in the
        // failure is what keeps this test actionable - the file is not in the assembly, or it is in
        // the assembly and the route did not find it.
        var embedded = typeof(Blocks).Assembly.GetManifestResourceNames().ToHashSet(StringComparer.Ordinal);

        await using var host = await StartAsync();
        var failures = new List<string>();

        foreach (var (path, expected) in served)
        {
            using var response = await host.Client.GetAsync("/" + path, TestContext.Current.CancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                failures.Add(
                    $"/{path}: answered {(int)response.StatusCode}, "
                    + $"embedded: {embedded.Contains(FrontendAssets.ResourcePrefix + path)}");

                continue;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);
            var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            if (!string.Equals(actual, expected, StringComparison.Ordinal))
            {
                failures.Add($"/{path}: {bytes.Length} byte(s) hashing to {actual}, recorded {expected}");
            }
        }

        // The count and every line, rather than the first few: a resource the project glob dropped
        // shows up here as a run of paths with the same shape, and one stray path is a different bug.
        Assert.True(
            failures.Count == 0,
            $"{failures.Count} of {served.Count} asset(s) were not served:{Environment.NewLine}"
            + string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// The tree the assembly carries is the tree on disk.
    /// </summary>
    /// <remarks>
    /// The companion of <c>SsrTests.The_tree_in_the_assembly_is_the_vendored_tree</c>, and the half
    /// a request cannot see: a resource the project glob dropped leaves the directory intact, so
    /// serving alone cannot tell "not embedded" from "embedded and refused".
    /// </remarks>
    [Fact]
    public void The_tree_in_the_assembly_is_the_vendored_tree()
    {
        var assembly = typeof(Blocks).Assembly;

        var embedded = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(FrontendAssets.ResourcePrefix, StringComparison.Ordinal))
            .ToDictionary(
                name => name[FrontendAssets.ResourcePrefix.Length..],
                name => VendoredTree.HashBytes(ReadResource(assembly, name)),
                StringComparer.Ordinal);

        var differences = VendoredTree.Differences(VendoredTree.Frontend.HashTree(), embedded, "embedded");

        Assert.True(differences.Count == 0, string.Join(Environment.NewLine, differences));
    }

    private static byte[] ReadResource(System.Reflection.Assembly assembly, string name)
    {
        using var stream = assembly.GetManifestResourceStream(name)!;
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);

        return buffer.ToArray();
    }

    [Fact]
    public async Task The_assets_are_served_from_the_assembly_rather_than_from_files_beside_it()
    {
        // The build does not copy the frontend next to the test binary, so a page that loads is a
        // page whose assets came out of the assembly — which is what lets a package reference and a
        // single-file deployment both work without knowing where anything was unpacked.
        Assert.False(
            Directory.Exists(Path.Combine(AppContext.BaseDirectory, "frontend")),
            "the frontend was found beside the test binary, so this test can no longer tell where it came from");

        await using var host = await StartAsync();
        var page = await host.Client.GetStringAsync("/", TestContext.Current.CancellationToken);

        Assert.Contains("assets/", page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_stylesheet_the_robots_file_and_the_default_icon_are_served_with_upstreams_answers()
    {
        await using var host = await StartAsync();

        var robots = await Responses.GetAsync(host, "/robots.txt");
        Assert.Equal("text/plain", robots.ContentType);
        Assert.Equal("User-agent: *\nDisallow: ", robots.Body);

        // The stylesheet is the theme's, rendered on the request: the default theme's variables,
        // its dark block, and the @font-face rules its fonts need. What this asserts is that the
        // endpoint serves a stylesheet rather than an empty answer.
        var theme = await Responses.GetAsync(host, "/theme.css");
        Assert.Equal("text/css", theme.ContentType);
        Assert.Contains("@font-face {", theme.Body, StringComparison.Ordinal);
        Assert.Contains("--name: default;", theme.Body, StringComparison.Ordinal);
        Assert.Contains(":root.dark, :root .dark {", theme.Body, StringComparison.Ordinal);

        // The icon is the Python version's own logo until an application names one. The manifest — the other
        // document a browser reads before it installs the page — is PwaTests' subject, because which
        // icon it names is a rule about the icon rather than about serving a file.
        var favicon = await Responses.GetAsync(host, "/favicon.ico");
        Assert.Equal("image/svg+xml", favicon.ContentType);
        Assert.Equal(
            VendoredTree.HashFile(Path.Combine(VendoredTree.Frontend.RootPath, "static", "img", "logo.svg")),
            Hash(favicon.Bytes));
    }

    [Fact]
    public async Task A_path_that_is_not_an_asset_is_refused_the_way_upstream_refuses_it()
    {
        await using var host = await StartAsync();

        // An empty path asks the route itself for a file, which is a malformed request.
        await Responses.RefusedAsync(host, "/assets/", HttpStatusCode.BadRequest, "Bad Request");
        await Responses.RefusedAsync(host, "/static/", HttpStatusCode.BadRequest, "Bad Request");

        // A directory is not a file, and the Python version will not list one.
        await Responses.RefusedAsync(host, "/static/img", HttpStatusCode.Forbidden, "Forbidden");

        // A path that names a location rather than a file in the tree — a URL, a UNC share, an
        // absolute path — is an attempt to make the server fetch something, refused before the tree
        // is consulted. The URL arrives percent-encoded, which is why the route unescapes first.
        await Responses.RefusedAsync(
            host,
            "/assets/http%3A%2F%2Fexample.com%2Fa.png",
            HttpStatusCode.Forbidden,
            "Forbidden");
        await Responses.RefusedAsync(host, "/assets/ftp%3A%2F%2Fexample.com%2Fa.png", HttpStatusCode.Forbidden, "Forbidden");
        await Responses.RefusedAsync(host, "/assets//etc/passwd", HttpStatusCode.Forbidden, "Forbidden");

        await Responses.RefusedAsync(host, "/assets/a.txt", HttpStatusCode.NotFound, "Not Found");
        await Responses.RefusedAsync(host, "/static/a.txt", HttpStatusCode.NotFound, "Not Found");

        // This release ships no compiled Svelte components. What an application with no icon answers
        // on the icon routes is PwaTests' subject, where the icon is the case rather than an aside.
        await Responses.RefusedAsync(host, "/svelte/a.js", HttpStatusCode.NotFound, "Not Found");
    }

    private static string Hash(byte[] bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    /// <summary>The assets the page names, as URLs this host serves.</summary>
    private static List<string> ReferencedAssets(string page) =>
        [.. AssetReference().Matches(page).Select(match => "/" + match.Groups["path"].Value.TrimStart('.', '/'))];

    /// <summary>Reads one of the JSON documents the page carries inside its script elements.</summary>
    private static JsonNode Embedded(string page, string marker)
    {
        var remainder = EmbeddedDocuments.Replace(page, marker, "{{document}}", out var document);

        Assert.True(remainder is not null && document is not null, $"the page carries no '{marker.Trim()}'");

        return document!;
    }

    private static Task<GradioTestHost> StartAsync() =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ManifestPage.Build);

    /// <summary>A reference the page makes to one of its own assets.</summary>
    [GeneratedRegex("(?:src|href)=\"\\./(?<path>assets/[^\"]+)\"", RegexOptions.CultureInvariant)]
    private static partial Regex AssetReference();
}

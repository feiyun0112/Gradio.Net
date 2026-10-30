using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Shared;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A multipage application, observed the way a browser and a client observe it: one URL per page,
/// each served the part of the application that belongs to it.
/// </summary>
/// <remarks>
/// <para>
/// These assert what the pages have to be for the navbar to work: every page is a URL, each URL
/// carries that page's components and bindings and the whole application's map of pages, and a path
/// that names no page is refused.
/// </para>
/// <para>
/// The application is <see cref="Multipage"/>: three pages, two of them with a binding, one of them
/// left out of the navbar.
/// </para>
/// </remarks>
public sealed class MultipageTests
{
    [Theory]
    [InlineData("/", "")]
    [InlineData("/about", "about")]
    [InlineData("/internal", "internal")]
    public async Task Every_page_is_served_as_itself(string url, string page)
    {
        var config = await GetPageConfigAsync(url);

        Assert.Equal(page, config["current_page"]!.GetValue<string>());

        // The page's own components, in the application's order — and only those. A page that
        // carried every component would render the whole application on every URL.
        var components = LayoutIds(config["components"]!);
        var pageComponents = Numbers(config["page"]![page]!["components"]!);

        Assert.Equal(pageComponents, components);
        Assert.NotEmpty(components);

        // The tree the frontend draws is the root with this page's children under it.
        var layoutChildren = LayoutIds(config["layout"]!["children"]!);

        Assert.Equal(LayoutIds(config["page"]![page]!["layout"]!["children"]!), layoutChildren);
    }

    [Fact]
    public async Task Every_page_carries_the_whole_application_map_of_pages()
    {
        var home = await GetPageConfigAsync("/");
        var about = await GetPageConfigAsync("/about");

        // The navbar navigates with this: a page that only knew about itself could not draw a link
        // to any other, and the frontend reads it from the page it booted from.
        Assert.Equal(
            "[[\"\",\"Home\",true],[\"about\",\"About\",true],[\"internal\",\"Internal\",false]]",
            home["pages"]!.ToJsonString());

        Assert.Equal(home["pages"]!.ToJsonString(), about["pages"]!.ToJsonString());

        // And the map of pages is the application's, not the page's: every page's layout and every
        // page's list of components are in it, which is what makes a page's copy of the document
        // the application's with one page selected.
        Assert.Equal(home["page"]!.ToJsonString(), about["page"]!.ToJsonString());
    }

    [Fact]
    public async Task A_page_answers_a_head_request()
    {
        await using var host = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Head, "/about");
        using var response = await host.Client.SendAsync(request, TestContext.Current.CancellationToken);

        // The Python version registers a page with `@app.get`, and Starlette answers HEAD for every GET route
        // it is given — so HEAD is part of what the Python version serves rather than an extra here. The
        // answer is the GET answer without its body: the same status and the same length, no bytes.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("/missing")]
    [InlineData("/missing/")]
    public async Task A_path_that_names_no_page_is_refused(string url)
    {
        await using var host = await StartAsync();

        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);

        // With or without its slash: the Python version registers the redirect only for pages it has, so a
        // path that names none is a miss either way.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task A_page_written_with_a_trailing_slash_redirects_to_it()
    {
        await using var host = await StartAsync();

        // A client that follows redirects would report the page's answer rather than the
        // redirect, and the redirect is the thing being asserted.
        using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            BaseAddress = host.BaseAddress,
        };

        using var response = await client.GetAsync("/about/", TestContext.Current.CancellationToken);

        // The Python version registers the redirect beside every page route: the address a page is served at
        // is the one the navbar links to, and a second address for the same document is a second
        // thing to keep right.
        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal("/about", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task The_navbar_says_what_each_page_shows_and_offers()
    {
        var home = await GetPageConfigAsync("/");
        var about = await GetPageConfigAsync("/about");

        var homeNavbar = Props(home, "navbar");
        Assert.Equal("Start", homeNavbar["main_page_name"]!.GetValue<string>());
        Assert.Equal("[[\"Docs\",\"https://example.com/docs\"]]", homeNavbar["value"]!.ToJsonString());

        // The second page configures the navbar differently: no home page link, and no navbar at
        // all — the props are the page's answer, not the application's.
        var aboutNavbar = Props(about, "navbar");
        Assert.False(aboutNavbar["main_page_name"]!.GetValue<bool>());
        Assert.False(aboutNavbar["visible"]!.GetValue<bool>());
        Assert.False(aboutNavbar.ContainsKey("value"));
    }

    [Fact]
    public async Task A_page_describes_the_endpoints_bound_on_it()
    {
        await using var host = await StartAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        using var all = await host.Client.GetAsync("/gradio_api/info", cancellationToken);
        var allInfo = JsonNode.Parse(await all.Content.ReadAsStringAsync(cancellationToken))!;

        Assert.Equal(["/about_changed", "/shout"], Endpoints(allInfo));

        // The same document asked for one page: what a client can call from the page it is looking
        // at. Both bindings are on different pages, so the filter is visible either way.
        using var about = await host.Client.GetAsync("/gradio_api/info?page=about", cancellationToken);
        var aboutInfo = JsonNode.Parse(await about.Content.ReadAsStringAsync(cancellationToken))!;

        Assert.Equal(["/about_changed"], Endpoints(aboutInfo));

        using var internalPage = await host.Client.GetAsync("/gradio_api/info?page=internal", cancellationToken);
        var internalInfo = JsonNode.Parse(await internalPage.Content.ReadAsStringAsync(cancellationToken))!;

        // A page with no binding of its own describes none; a page the application does not have is
        // the same answer, because the narrowing is a filter over the bindings rather than a lookup.
        Assert.Empty(Endpoints(internalInfo));

        // A page the application does not have describes the whole application instead, which is
        // what the Python version answers: the argument narrows the document when it names something.
        using var missing = await host.Client.GetAsync("/gradio_api/info?page=missing", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, missing.StatusCode);
        Assert.Equal(
            ["/about_changed", "/shout"],
            Endpoints(JsonNode.Parse(await missing.Content.ReadAsStringAsync(cancellationToken))!));
    }

    [Fact]
    public async Task A_single_page_application_registers_no_page_route()
    {
        // The page route exists for an application that has pages, which is what keeps a
        // single-page host's endpoint set equal to the Python route table's. Here the
        // question is asked of a host whose application has no second page.
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ManifestPage.Build);

        var registered = GradioEndpointExport.From(host.Services)
            .Select(endpoint => endpoint.Path)
            .ToArray();

        Assert.DoesNotContain("/{page}", registered);
    }

    private static async Task<GradioTestHost> StartAsync() =>
        await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Multipage.Build);

    private static async Task<JsonObject> GetPageConfigAsync(string url)
    {
        await using var host = await StartAsync();

        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var config = EmbeddedDocuments.Replace(
            body,
            EmbeddedDocuments.ConfigMarker,
            EmbeddedDocuments.ConfigPlaceholder,
            out var document);

        Assert.NotNull(config);

        return document!.AsObject();
    }

    private static JsonObject Props(JsonObject config, string name) =>
        config["components"]!
            .AsArray()
            .Select(component => component!.AsObject())
            .Single(component => component["props"]!["name"]!.GetValue<string>() == name)
            ["props"]!
            .AsObject();

    private static string[] Endpoints(JsonNode info) =>
        [.. info["named_endpoints"]!.AsObject().Select(pair => pair.Key).Order(StringComparer.Ordinal)];

    /// <summary>The ids of a layout node's children, which are written as <c>{"id": n}</c> nodes.</summary>
    private static int[] LayoutIds(JsonNode node) =>
        [.. node.AsArray().Select(item => item!["id"]!.GetValue<int>())];

    /// <summary>The ids a page names, which are written as plain numbers.</summary>
    private static int[] Numbers(JsonNode node) =>
        [.. node.AsArray().Select(item => item!.GetValue<int>())];
}

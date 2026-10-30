using System.Net;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The application as something a browser will install: the manifest it reads, the icon the
/// manifest names, and the sizes the launcher asks that icon for.
/// </summary>
/// <remarks>
/// <para>
/// The manifest is the one document a browser reads before it offers to put an application on a
/// home screen, and every field in it has to be one a browser understands: a relative
/// <c>start_url</c>, a <c>display</c> of <c>standalone</c>, and at least one icon whose <c>src</c>
/// resolves to an image. The bodies asserted here were measured against the Python version 6.30.0
/// rather than read off its source — the three icon shapes it chooses between, in the order it
/// writes their fields, byte for byte.
/// </para>
/// <para>
/// The icon routes are the other half: what the manifest names has to be servable, and a raster
/// icon is scaled to the size the launcher asked for. A vector is not scaled — the Python version
/// tries and raises, which is a connection dropped rather than an answer — so it is served as it
/// is, at any size, and the difference is declared in the ledger.
/// </para>
/// </remarks>
[Collection(PwaEnvironmentDefinition.Name)]
public sealed class PwaTests
{
    /// <summary>The title the page carries, which is the manifest's <c>name</c>.</summary>
    private const string Title = "Pwa page";

    /// <summary>The size of the icon the tests draw, which is smaller than either size asked for.</summary>
    private const int IconSize = 64;

    /// <summary>A small vector icon, served as it is.</summary>
    private const string Svg =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"10\" height=\"10\">"
        + "<rect width=\"10\" height=\"10\"/></svg>";

    [Fact]
    public async Task The_manifest_names_the_icon_the_application_named_at_the_sizes_a_launcher_asks_for()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.png", TestImages.Png(IconSize, IconSize, red: 255, green: 0, blue: 0));

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        var manifest = await Responses.GetAsync(host, "/manifest.json");

        Assert.Equal("application/manifest+json", manifest.ContentType);
        Assert.Equal(
            $"{{\"name\":\"{Title}\",\"icons\":["
            + "{\"src\":\"/pwa_icon/192\",\"sizes\":\"192x192\",\"type\":\"image/png\",\"purpose\":\"any\"},"
            + "{\"src\":\"/pwa_icon/512\",\"sizes\":\"512x512\",\"type\":\"image/png\",\"purpose\":\"any\"}],"
            + "\"start_url\":\"./\",\"display\":\"standalone\"}",
            manifest.Body);
    }

    [Fact]
    public async Task The_manifest_offers_a_vector_icon_as_one_entry_good_at_any_size()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.svg", Svg);

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        var manifest = await Responses.GetAsync(host, "/manifest.json");

        Assert.Equal(
            $"{{\"name\":\"{Title}\",\"icons\":["
            + "{\"src\":\"/pwa_icon\",\"sizes\":\"any\",\"type\":\"image/svg+xml\",\"purpose\":\"any\"}],"
            + "\"start_url\":\"./\",\"display\":\"standalone\"}",
            manifest.Body);
    }

    /// <summary>
    /// An application that names no icon still gets a valid manifest, naming the Python version's
    /// own logo.
    /// </summary>
    /// <remarks>
    /// The entry's <c>src</c> is relative, which is the Python version's spelling rather than an
    /// oversight: a browser resolves it against the manifest's own address, so it reaches
    /// <c>/static/img/logo_nosize.svg</c>, which the frontend capability serves.
    /// </remarks>
    [Fact]
    public async Task The_manifest_offers_upstreams_logo_when_the_application_names_no_icon()
    {
        await using var host = await StartAsync();

        var manifest = await Responses.GetAsync(host, "/manifest.json");

        Assert.Equal(
            $"{{\"name\":\"{Title}\",\"icons\":["
            + "{\"src\":\"static/img/logo_nosize.svg\",\"sizes\":\"any\","
            + "\"type\":\"image/svg+xml\",\"purpose\":\"any\"}],"
            + "\"start_url\":\"./\",\"display\":\"standalone\"}",
            manifest.Body);

        // The address the relative src resolves to is one this server answers.
        var logo = await Responses.GetAsync(host, "/static/img/logo_nosize.svg");
        Assert.Equal("image/svg+xml", logo.ContentType);
    }

    [Fact]
    public async Task The_manifest_calls_an_application_with_no_title_gradio()
    {
        await using var host = await StartAsync(blocks => blocks.Title = string.Empty);

        var manifest = await Responses.GetAsync(host, "/manifest.json");

        Assert.Equal(
            "{\"name\":\"Gradio\",\"icons\":["
            + "{\"src\":\"static/img/logo_nosize.svg\",\"sizes\":\"any\","
            + "\"type\":\"image/svg+xml\",\"purpose\":\"any\"}],"
            + "\"start_url\":\"./\",\"display\":\"standalone\"}",
            manifest.Body);
    }

    /// <summary>
    /// The icon the application named is served as it lies on disk, at both addresses that name it.
    /// </summary>
    /// <remarks>
    /// <c>/favicon.ico</c> is not a second icon: it is the same one, which is what the Python
    /// version's <c>favicon</c> helper does with a configured path. A page asks for it on every
    /// load, so an application whose icon answered at one address and not the other would be an
    /// application with a broken tab.
    /// </remarks>
    [Fact]
    public async Task The_icon_routes_answer_the_file_the_application_named()
    {
        using var folder = new TempFolder();
        var png = TestImages.Png(IconSize, IconSize, red: 0, green: 0, blue: 255);
        var icon = folder.Write("icon.png", png);

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        var served = await Responses.GetAsync(host, "/pwa_icon");
        Assert.Equal("image/png", served.ContentType);
        Assert.Equal(png, served.Bytes);

        var favicon = await Responses.GetAsync(host, "/favicon.ico");
        Assert.Equal("image/png", favicon.ContentType);
        Assert.Equal(png, favicon.Bytes);
    }

    [Fact]
    public async Task A_vector_icon_is_served_verbatim_at_both_addresses_that_name_it()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.svg", Svg);

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        var served = await Responses.GetAsync(host, "/pwa_icon");
        Assert.Equal("image/svg+xml", served.ContentType);
        Assert.Equal(Svg, served.Body);

        var favicon = await Responses.GetAsync(host, "/favicon.ico");
        Assert.Equal("image/svg+xml", favicon.ContentType);
        Assert.Equal(Svg, favicon.Body);
    }

    /// <summary>
    /// A raster icon is scaled to the size the request named, and it is still the same picture.
    /// </summary>
    /// <remarks>
    /// The size is the promise; the pixels are the icon's own. A resize that produced a correctly
    /// sized blank image would pass a size-only assertion, so the colour the icon was drawn in is
    /// read back out of the middle of the answer.
    /// </remarks>
    [Fact]
    public async Task A_raster_icon_is_scaled_to_the_size_the_request_named()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.png", TestImages.Png(IconSize, IconSize, red: 255, green: 0, blue: 0));

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        int[] sizes = [192, 512];

        foreach (var size in sizes)
        {
            var scaled = await Responses.GetAsync(host, $"/pwa_icon/{size}");

            Assert.Equal("image/png", scaled.ContentType);

            var image = RasterImage.Open(scaled.Bytes);
            Assert.Equal(size, image.Width);
            Assert.Equal(size, image.Height);
            Assert.Equal((255, 0, 0), CentreOf(image));
        }
    }

    [Fact]
    public async Task A_vector_icon_is_answered_verbatim_at_any_size()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.svg", Svg);

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        int[] sizes = [16, 192, 512];

        foreach (var size in sizes)
        {
            var served = await Responses.GetAsync(host, $"/pwa_icon/{size}");

            Assert.Equal("image/svg+xml", served.ContentType);
            Assert.Equal(Svg, served.Body);
        }
    }

    [Fact]
    public async Task The_icon_routes_answer_not_found_when_the_application_names_no_icon()
    {
        await using var host = await StartAsync();

        await Responses.RefusedAsync(host, "/pwa_icon", HttpStatusCode.NotFound, "Not Found");
        await Responses.RefusedAsync(host, "/pwa_icon/192", HttpStatusCode.NotFound, "Not Found");
    }

    /// <summary>
    /// A size that is not a number is refused before the icon is looked for.
    /// </summary>
    /// <remarks>
    /// The order is the Python version's: FastAPI validates a path parameter before the handler
    /// runs, so the framework answers 422 and the handler — which would have answered 404 for the
    /// missing icon — never runs. An application with no icon therefore answers 422 to
    /// <c>/pwa_icon/abc</c> and 404 to <c>/pwa_icon/192</c>, and the two are not the same refusal.
    /// </remarks>
    [Fact]
    public async Task A_size_that_is_not_a_number_is_refused_before_the_icon_is_looked_for()
    {
        await using var host = await StartAsync();

        await AssertValidationAsync(
            host,
            "/pwa_icon/abc",
            "{\"type\":\"int_parsing\",\"loc\":[\"path\",\"size\"],"
            + "\"msg\":\"Input should be a valid integer, unable to parse string as an integer\","
            + "\"input\":\"abc\"}");
    }

    /// <summary>
    /// An application that named an icon which is not there is refused rather than given a default.
    /// </summary>
    /// <remarks>
    /// The same choice <see cref="Blocks.CssPaths"/> makes about a stylesheet that is not there: an
    /// application that named a file it does not have has a mistake, and answering with somebody
    /// else's icon would hide it. The Python version raises instead — the connection drops — so the
    /// status is this port's, and the ledger says so.
    /// </remarks>
    [Fact]
    public async Task The_icon_routes_answer_not_found_for_an_icon_file_that_is_not_there()
    {
        using var folder = new TempFolder();
        var missing = Path.Combine(folder.Folder, "gone.png");

        await using var host = await StartAsync(blocks => blocks.FaviconPath = missing);

        await Responses.RefusedAsync(host, "/pwa_icon", HttpStatusCode.NotFound, "Not Found");
        await Responses.RefusedAsync(host, "/pwa_icon/192", HttpStatusCode.NotFound, "Not Found");
        await Responses.RefusedAsync(host, "/favicon.ico", HttpStatusCode.NotFound, "Not Found");
    }

    /// <summary>
    /// A file that is not an image is served as the icon it was named as, and refused when a size
    /// is asked for.
    /// </summary>
    /// <remarks>
    /// The two addresses answer differently on purpose: <c>/pwa_icon</c> sends bytes and a guessed
    /// type, which is all it can know, and the sized address is the one that has to draw the file —
    /// which a file that is not an image cannot be. The Python version raises there.
    /// </remarks>
    [Fact]
    public async Task A_file_that_is_not_an_image_cannot_be_made_into_a_sized_icon()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.png", "this is not a PNG");

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        var served = await Responses.GetAsync(host, "/pwa_icon");
        Assert.Equal("image/png", served.ContentType);
        Assert.Equal("this is not a PNG", served.Body);

        await Responses.RefusedAsync(
            host,
            "/pwa_icon/192",
            HttpStatusCode.UnprocessableEntity,
            "The icon is not an image, so it cannot be scaled.");
    }

    /// <summary>
    /// A size no icon can be made at is refused as a malformed request, in the shape FastAPI
    /// refuses one.
    /// </summary>
    /// <remarks>
    /// A size that is not a number is the Python version's own refusal, measured: FastAPI validates
    /// the path parameter before the handler runs and answers a pydantic document. Zero and
    /// negatives are where the Python version raises and the connection drops, so what is answered
    /// here is the refusal rather than the crash, and a size larger than any launcher asks for is
    /// refused for the same reason — the route is unauthenticated and allocates a bitmap of the
    /// requested size.
    /// </remarks>
    [Fact]
    public async Task The_sized_icon_route_refuses_a_size_no_icon_can_be_made_at()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.png", TestImages.Png(IconSize, IconSize, red: 0, green: 255, blue: 0));

        await using var host = await StartAsync(blocks => blocks.FaviconPath = icon);

        await AssertValidationAsync(
            host,
            "/pwa_icon/abc",
            "{\"type\":\"int_parsing\",\"loc\":[\"path\",\"size\"],"
            + "\"msg\":\"Input should be a valid integer, unable to parse string as an integer\","
            + "\"input\":\"abc\"}");

        await AssertValidationAsync(
            host,
            "/pwa_icon/0",
            "{\"type\":\"greater_than\",\"loc\":[\"path\",\"size\"],"
            + "\"msg\":\"Input should be greater than 0\",\"input\":0,\"ctx\":{\"gt\":0}}");

        await AssertValidationAsync(
            host,
            "/pwa_icon/-5",
            "{\"type\":\"greater_than\",\"loc\":[\"path\",\"size\"],"
            + "\"msg\":\"Input should be greater than 0\",\"input\":-5,\"ctx\":{\"gt\":0}}");

        await AssertValidationAsync(
            host,
            "/pwa_icon/4097",
            "{\"type\":\"less_than_equal\",\"loc\":[\"path\",\"size\"],"
            + "\"msg\":\"Input should be less than or equal to 4096\","
            + "\"input\":4097,\"ctx\":{\"le\":4096}}");
    }

    /// <summary>
    /// The config's <c>pwa</c> switch is the application's, not a constant.
    /// </summary>
    /// <remarks>
    /// The Python version's launch parameter defaults to <c>None</c>, which means "on when the
    /// application is running on a Space" — so the field is true for a Space and false everywhere
    /// else. Both halves are asserted: the constant it used to be was always false.
    /// </remarks>
    [Fact]
    public async Task The_config_carries_the_pwa_switch_the_application_was_started_with()
    {
        await using var off = await StartAsync();
        Assert.False(await PwaOfAsync(off));

        await using var on = await StartAsync(blocks => blocks.Pwa = true);
        Assert.True(await PwaOfAsync(on));
    }

    /// <summary>
    /// An application started on a Space is a PWA without being asked, which is the Python
    /// version's default.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version's test is that the Space's name is not <see langword="null"/> rather than
    /// that it is not empty, so a name set to the empty string still counts: the platform said
    /// where it is running, and that is what the default follows.
    /// </para>
    /// <para>
    /// The collection runs alone because the two variables are process-wide, and
    /// <see cref="Blocks"/> reads them when it is constructed.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task An_application_running_on_a_space_offers_itself_as_a_pwa()
    {
        using (EnvironmentVariable.Set("SYSTEM", "spaces"))
        using (EnvironmentVariable.Set("SPACE_ID", "someone/something"))
        {
            await using var host = await StartAsync();

            Assert.True(await PwaOfAsync(host));
        }

        using (EnvironmentVariable.Set("SYSTEM", "spaces"))
        using (EnvironmentVariable.Set("SPACE_ID", string.Empty))
        {
            await using var host = await StartAsync();

            Assert.True(await PwaOfAsync(host));
        }
    }

    /// <summary>Starts an application holding one field, with whatever the test adds to it.</summary>
    /// <param name="configure">What to set on the application before it is served.</param>
    /// <returns>The host, listening on a loopback address.</returns>
    private static Task<GradioTestHost> StartAsync(Action<Blocks>? configure = null) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.Title = Title;
                configure?.Invoke(blocks);

                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Name" });
                }
            });

    private static async Task<bool> PwaOfAsync(GradioTestHost host)
    {
        var config = await ConfigDocument.ReadAsync(
            host,
            "/config",
            TestContext.Current.CancellationToken);

        return config["pwa"]!.GetValue<bool>();
    }

    /// <summary>The colour of the pixel in the middle of an image.</summary>
    /// <param name="image">The image.</param>
    /// <returns>The three channels, the alpha dropped.</returns>
    private static (byte Red, byte Green, byte Blue) CentreOf(RasterImage image)
    {
        var offset = (((image.Height / 2) * image.Width) + (image.Width / 2)) * 4;
        var pixels = image.Pixels.Span;

        return (pixels[offset], pixels[offset + 1], pixels[offset + 2]);
    }

    /// <summary>Asserts the one-entry validation document FastAPI answers for a malformed request.</summary>
    /// <param name="host">The host.</param>
    /// <param name="url">The URL to ask.</param>
    /// <param name="error">The entry, as JSON.</param>
    private static async Task AssertValidationAsync(GradioTestHost host, string url, string error)
    {
        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(
            $"{{\"detail\":[{error}]}}",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

}

/// <summary>
/// The collection the Space environment test runs in: alone, because it sets process-wide
/// variables that <see cref="Blocks"/> reads when it is constructed.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class PwaEnvironmentDefinition
{
    /// <summary>The collection's name, as the tests declare it.</summary>
    public const string Name = "pwa-environment";
}

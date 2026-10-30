using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Which responses are compressed, with what, and which are left alone.
/// </summary>
/// <remarks>
/// <para>
/// The rules are the Python version's and they are all about the request's path: only a path that
/// names a file whose type benefits is compressed, so a page's stylesheet and a download's CSV are,
/// and a document served under a name with no extension — <c>/config</c> — is not. That last rule
/// is why nothing here has to buffer an event stream: <c>/gradio_api/queue/data</c> names no file.
/// </para>
/// <para>
/// The bodies are read as bytes rather than through <see cref="HttpClient"/>'s own decompression,
/// because what is asserted is the header and the bytes behind it, not that a client can undo them.
/// </para>
/// </remarks>
public sealed class BrotliMiddlewareTests
{
    [Fact]
    public async Task A_stylesheet_is_compressed_with_brotli()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        var plain = await host.Client.GetByteArrayAsync("/theme.css", cancellationToken);
        using var response = await GetAsync(host, "/theme.css", "br, gzip, deflate", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("br", response.Content.Headers.ContentEncoding.Single());

        var compressed = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        // The body is smaller than what it stands for — a "compressed" response that grew would be
        // worse than none — and the length the client was told, if it was told one, is the length
        // that arrived. The middleware drops the length the endpoint set, because the compressed
        // size is not known until the body has been written.
        Assert.True(compressed.Length < plain.Length, $"A {plain.Length}-byte stylesheet compressed to {compressed.Length}.");

        if (response.Content.Headers.ContentLength is { } declared)
        {
            Assert.Equal(compressed.Length, declared);
        }

        // The bytes behind the header decode to exactly what the uncompressed response carries.
        Assert.Equal(plain, await InflateAsync(compressed, "br"));

        // A cache in front of the application has to know the answer depends on this header.
        Assert.Contains("Accept-Encoding", response.Headers.Vary);
    }

    [Fact]
    public async Task A_client_that_cannot_take_brotli_is_answered_with_gzip()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        var plain = await host.Client.GetByteArrayAsync("/theme.css", cancellationToken);
        using var response = await GetAsync(host, "/theme.css", "gzip", cancellationToken);

        Assert.Equal("gzip", response.Content.Headers.ContentEncoding.Single());
        Assert.Equal(plain, await InflateAsync(await response.Content.ReadAsByteArrayAsync(cancellationToken), "gzip"));
    }

    [Fact]
    public async Task A_client_that_asks_for_nothing_is_answered_as_it_is()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        using var response = await GetAsync(host, "/theme.css", null, cancellationToken);

        Assert.Empty(response.Content.Headers.ContentEncoding);
        Assert.Contains("body", await response.Content.ReadAsStringAsync(cancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_response_smaller_than_the_floor_is_answered_as_it_is()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        var plain = await host.Client.GetByteArrayAsync("/robots.txt", cancellationToken);
        using var response = await GetAsync(host, "/robots.txt", "br", cancellationToken);

        // Below four hundred bytes the framing costs more than the saving, which is the Python version's
        // reason for the floor and the reason this is not compressed even though it is text.
        Assert.True(plain.Length < 400, $"The robots file is {plain.Length} bytes, so it no longer tests the floor.");
        Assert.Empty(response.Content.Headers.ContentEncoding);
        Assert.Equal(plain, await response.Content.ReadAsByteArrayAsync(cancellationToken));
    }

    [Fact]
    public async Task A_path_that_names_no_file_is_answered_as_it_is()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        // A document, not a file: the Python version's whitelist is by extension, so `/config` is never
        // compressed however large it is.
        using var response = await GetAsync(host, "/config", "br", cancellationToken);

        Assert.Empty(response.Content.Headers.ContentEncoding);
    }

    [Fact]
    public async Task A_file_whose_type_does_not_benefit_is_answered_as_it_is()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync();

        // An icon is already compressed. The whitelist is what says so, and it is a whitelist so
        // that a type nobody thought about is passed through rather than damaged.
        using var response = await GetAsync(host, "/favicon.ico", "br", cancellationToken);

        Assert.Empty(response.Content.Headers.ContentEncoding);
    }

    /// <summary>Starts a host whose application has a stylesheet and an icon to serve.</summary>
    /// <returns>The started host.</returns>
    private static Task<GradioTestHost> StartAsync() =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "A" });
                }
            });

    /// <summary>Requests a path with the encoding the test wants to advertise.</summary>
    /// <param name="host">The host to ask.</param>
    /// <param name="path">What to ask for.</param>
    /// <param name="acceptEncoding">The encodings the client claims, or <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The response, with the body as it arrived.</returns>
    private static Task<HttpResponseMessage> GetAsync(
        GradioTestHost host,
        string path,
        string? acceptEncoding,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);

        if (acceptEncoding is not null)
        {
            request.Headers.AcceptEncoding.ParseAdd(acceptEncoding);
        }

        return host.Client.SendAsync(request, cancellationToken);
    }

    /// <summary>Undoes an encoding, so that the bytes can be compared with what was written.</summary>
    /// <param name="body">The bytes as they arrived.</param>
    /// <param name="encoding"><c>br</c> or <c>gzip</c>.</param>
    /// <returns>The bytes that were compressed.</returns>
    private static async Task<byte[]> InflateAsync(byte[] body, string encoding)
    {
        await using var input = new MemoryStream(body);
        await using Stream decoder = string.Equals(encoding, "br", StringComparison.Ordinal)
            ? new BrotliStream(input, CompressionMode.Decompress)
            : new GZipStream(input, CompressionMode.Decompress);

        using var output = new MemoryStream();
        await decoder.CopyToAsync(output);

        return output.ToArray();
    }
}

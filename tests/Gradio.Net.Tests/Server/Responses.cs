using System.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What a host answered a GET with, and the two shapes an answer takes: a body, or a refusal.
/// </summary>
/// <remarks>
/// The reader is one for the whole suite because two classes now ask the same questions of the same
/// host — the page's own assets and the icon — and a second copy would be a second opinion about
/// what a refusal looks like. It is a copy of nothing: the earlier classes that read a body inline
/// still do, and are welcome to move here.
/// </remarks>
internal static class Responses
{
    /// <summary>Reads a GET, as the bytes it came back with and as the text they spell.</summary>
    /// <param name="host">The host to ask.</param>
    /// <param name="url">The address to ask for.</param>
    /// <returns>The media type, the body, and the bytes.</returns>
    internal static async Task<Answer> GetAsync(GradioTestHost host, string url)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);

        var bytes = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        return new Answer(
            response.Content.Headers.ContentType?.MediaType,
            System.Text.Encoding.UTF8.GetString(bytes),
            bytes);
    }

    /// <summary>The media type of a GET, for the assertions that are only about the type.</summary>
    /// <param name="host">The host to ask.</param>
    /// <param name="url">The address to ask for.</param>
    /// <returns>The media type, or <see langword="null"/> when the answer carried none.</returns>
    internal static async Task<string?> MediaTypeOfAsync(GradioTestHost host, string url)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);

        return response.Content.Headers.ContentType?.MediaType;
    }

    /// <summary>
    /// Asserts that a GET was refused with the Python version's own body, which is one sentence
    /// under <c>detail</c>.
    /// </summary>
    /// <param name="host">The host to ask.</param>
    /// <param name="url">The address to ask for.</param>
    /// <param name="status">The status the refusal carries.</param>
    /// <param name="detail">The sentence.</param>
    internal static async Task RefusedAsync(
        GradioTestHost host,
        string url,
        HttpStatusCode status,
        string detail)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var response = await host.Client.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(
            $"{{\"detail\":\"{detail}\"}}",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>What a request was answered with, as the assertions read it.</summary>
    /// <param name="ContentType">The media type, without its parameters.</param>
    /// <param name="Body">The bytes, read as UTF-8 — which is only meaningful for a text answer.</param>
    /// <param name="Bytes">The bytes, as they arrived.</param>
    internal sealed record Answer(string? ContentType, string Body, byte[] Bytes);
}

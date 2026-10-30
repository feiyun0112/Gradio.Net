using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The address an application is being served from, as the request that arrived names it.
/// </summary>
/// <remarks>
/// <para>
/// The page, the config and the API info all carry this address, and they all carry the same one:
/// it is the scheme, host and path base the browser used, not something the application configures.
/// The Python version derives it per request for the same reason, which is why it lives here rather
/// than on the application.
/// </para>
/// <para>
/// Two headers are read before the request's own address, because a request can arrive on an
/// address the browser never used. <c>x-forwarded-host</c> is what a reverse proxy reports, and
/// <c>x-gradio-server</c> is what the SSR runtime reports: it renders the page itself and asks the
/// application for the config from an internal port, naming the address the browser is on so that
/// the page it builds points there. A document that carried the internal port instead would send
/// every browser that is not on this machine to an address it cannot reach — which is what the
/// theme link is, so the page would arrive unstyled. The Python version resolves the same three
/// sources in the same order, and prefers the first value when a header names several.
/// </para>
/// </remarks>
internal static class RootUrl
{
    /// <summary>The header a proxy reports the browser's host in.</summary>
    private const string ForwardedHostHeader = "x-forwarded-host";

    /// <summary>The header the SSR runtime reports the browser's origin in.</summary>
    private const string ServerHeader = "x-gradio-server";

    /// <summary>The header a proxy reports the browser's scheme in.</summary>
    private const string ForwardedProtoHeader = "x-forwarded-proto";

    private const string HttpScheme = "http://";

    private const string HttpsScheme = "https://";

    /// <summary>Reads the address from the request.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The address, without a trailing slash.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal static string Of(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var request = context.Request;
        var pathBase = request.PathBase.Value ?? string.Empty;

        var forwardedHost = FirstValue(request, ForwardedHostHeader);
        var origin = FirstValue(request, ServerHeader);

        // A forwarded host names only the host, so the scheme has to be assumed; the SSR runtime
        // names a whole origin. Only when neither is there is the address the request arrived on
        // the right answer.
        var root = forwardedHost is not null
            ? HttpScheme + forwardedHost
            : origin ?? $"{request.Scheme}://{request.Host}{pathBase}";

        if (string.Equals(FirstValue(request, ForwardedProtoHeader), "https", StringComparison.Ordinal)
            && root.StartsWith(HttpScheme, StringComparison.Ordinal))
        {
            root = HttpsScheme + root[HttpScheme.Length..];
        }

        root = root.TrimEnd('/');

        // A reported origin names a host and maybe a path; the application's own mount path is
        // still part of the address a browser uses, so it is added when the report does not
        // already carry it - which is the Python version's rule for a configured root path.
        if (pathBase.Length > 0
            && Uri.TryCreate(root, UriKind.Absolute, out var reported)
            && !string.Equals(reported.AbsolutePath.TrimEnd('/'), pathBase.TrimEnd('/'), StringComparison.Ordinal))
        {
            root = new UriBuilder(reported) { Path = pathBase }.Uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
        }

        return root;
    }

    /// <summary>The first value of a header that may name several.</summary>
    /// <param name="request">The request.</param>
    /// <param name="name">The header's name.</param>
    /// <returns>The first value, trimmed, or <see langword="null"/> when the header says nothing.</returns>
    private static string? FirstValue(HttpRequest request, string name)
    {
        if (!request.Headers.TryGetValue(name, out var values))
        {
            return null;
        }

        var first = values.ToString().Split(',')[0].Trim();

        return first.Length == 0 ? null : first;
    }
}

using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Where the browser is sent once a sign-in has finished, taken from the request that carried it.
/// </summary>
/// <remarks>
/// <para>
/// The address travels in the callback's query as <c>_target_url</c>, and it is reduced to a path
/// before it is used: a sign-in that honoured a whole address would let a link to the application
/// send the user somewhere else afterwards, which is the open redirect the Python version fixed for
/// the same reason.
/// </para>
/// <para>
/// The leading slashes are collapsed to one as well, because a browser reads <c>//host</c> as an
/// address on another host rather than as a path.
/// </para>
/// </remarks>
internal static class OAuthRedirect
{
    /// <summary>The page the browser is sent back to, reduced to a path on this application.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A path, with the query and fragment the target carried.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal static string TargetOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var raw = context.Request.Query["_target_url"].ToString();

        if (raw.Length == 0)
        {
            return "/";
        }

        var fragment = string.Empty;
        var hash = raw.IndexOf('#', StringComparison.Ordinal);

        if (hash >= 0)
        {
            fragment = raw[hash..];
            raw = raw[..hash];
        }

        var query = string.Empty;
        var question = raw.IndexOf('?', StringComparison.Ordinal);

        if (question >= 0)
        {
            query = raw[question..];
            raw = raw[..question];
        }

        return "/" + PathOf(raw).TrimStart('/', '\\') + query + fragment;
    }

    /// <summary>The path part of an address, with any scheme and host dropped.</summary>
    /// <param name="address">The address as it arrived.</param>
    private static string PathOf(string address)
    {
        var scheme = address.IndexOf("://", StringComparison.Ordinal);

        if (scheme >= 0)
        {
            var slash = address.IndexOf('/', scheme + 3);

            return slash >= 0 ? address[slash..] : string.Empty;
        }

        // A protocol-relative address names another host too: what follows the host is its path.
        if (address.StartsWith("//", StringComparison.Ordinal))
        {
            var slash = address.IndexOf('/', 2);

            return slash >= 0 ? address[slash..] : string.Empty;
        }

        return address;
    }
}

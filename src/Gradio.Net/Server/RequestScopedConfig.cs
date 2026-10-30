using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The fields a served config carries because of the request rather than because of the
/// application: who is asking, whether the link was a deep link, and where the application is
/// being served from.
/// </summary>
/// <remarks>
/// <para>
/// Two endpoints answer with a config — <c>/config</c> and the page, which embeds one — and both
/// append these fields at the end, after everything the application declares. They are here rather
/// than in either endpoint because the order is part of the answer: the config is embedded in the
/// page as JSON, so the order the keys are added in is the order they appear in the bytes a browser
/// parses.
/// </para>
/// <para>
/// The caller's name is the field that changes per request: an application that asks nobody to sign
/// in, or a caller who has not, is described with a null one, which is what the Python version's
/// config says and what the frontend reads to decide whether to offer a sign-out.
/// </para>
/// </remarks>
internal static class RequestScopedConfig
{
    /// <summary>Appends the request's fields to a config, in the Python version's order.</summary>
    /// <param name="config">The config the application declared.</param> <param name="context">The
    /// request being answered.</param> <param name="userName">Who is asking, or
    /// <see langword="null"/> for nobody.</param> <param name="deepLinkState">The deep-link state
    /// to report, or <see langword="null"/> for none.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static void Append(JsonObject config, HttpContext context, string? userName, string? deepLinkState = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(context);

        config["username"] = userName;

        if (deepLinkState is not null)
        {
            config["deep_link_state"] = deepLinkState;
        }

        config["root"] = RootUrl.Of(context);
    }
}

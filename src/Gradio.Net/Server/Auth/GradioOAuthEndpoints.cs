using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Signing in with an identity provider: sending the browser to it, and taking the answer back.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>/login/huggingface</c> and <c>/login/callback</c>, which are the two
/// halves of the OAuth 2.0 authorization-code flow. The first writes a state into the session and
/// sends the browser to the provider with it; the second checks that the provider echoed the same
/// state, asks the provider to turn the code into a token, asks who the token belongs to, and keeps
/// the answer in a signed cookie.
/// </para>
/// <para>
/// The state is what makes the flow safe: a callback that carries a state this browser did not
/// start is a callback someone else started, and it is refused. A refused callback is not an error
/// page — the browser is sent back to the sign-in route, which starts a fresh attempt, which is
/// what the Python version does. The count of attempts travels in the query so that a browser whose
/// cookies never work stops being sent round in circles.
/// </para>
/// <para>
/// The address the browser is returned to travels in the callback's query as <c>_target_url</c>,
/// and it is reduced to a path before it is used: a callback that honoured a whole address would
/// let a link to the application send the user somewhere else afterwards (the Python version's own
/// fix for exactly that).
/// </para>
/// <para>
/// An application without a <c>LoginButton</c> has neither route — the Python version registers
/// them only for one that has, and answers 404 otherwise, which is what this does. The route table
/// lists them unconditionally because a route that is sometimes present is still a route.
/// </para>
/// </remarks>
internal sealed class GradioOAuthEndpoints : IGradioEndpoint
{
    private const string AuthorizePath = "/login/huggingface";
    private const string CallbackPath = "/login/callback";

    /// <summary>The session field the address the provider returns the browser to is kept in.</summary>
    private const string RedirectKey = "oauth_redirect_uri";

    /// <summary>How many times a browser is sent round the sign-in route before it is let through.</summary>
    /// <remarks>
    /// The Python version's <c>MAX_REDIRECTS</c>, which exists because a browser that refuses the
    /// session cookie would otherwise be redirected forever.
    /// </remarks>
    private const int MaxRedirects = 2;

    private readonly Blocks _blocks;
    private readonly GradioOAuthSession _oauth;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose users sign in.</param>
    /// <param name="oauth">The session a sign-in is kept in, and the provider it is kept with.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioOAuthEndpoints(Blocks blocks, GradioOAuthSession oauth)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(oauth);

        _blocks = blocks;
        _oauth = oauth;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("GET", AuthorizePath).Key,
        new GradioRoute("GET", CallbackPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = GradioRouteMetadata.PathOf(context);

        return route switch
        {
            AuthorizePath => AuthorizeAsync(context),
            CallbackPath => CallbackAsync(context),
            _ => Answers.NotFoundAsync(context),
        };
    }

    /// <summary>Sends the browser to the identity provider, with a state this browser started.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the redirect is written.</returns>
    private async Task AuthorizeAsync(HttpContext context)
    {
        if (!_blocks.ExpectsOAuth)
        {
            await Answers.NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        var provider = _oauth.Provider;
        var redirectUri = CallbackUrl(context);
        var state = Answers.UrlSafeToken(32);

        var session = _oauth.Read(context);
        session[GradioOAuthSession.StateKey] = state;
        session[RedirectKey] = redirectUri.ToString();

        // Written before the redirect is answered, so that the browser holds the state by the time
        // the provider sends it back.
        _oauth.Write(context, session);

        var address = await provider
            .AuthorizeUrlAsync(state, redirectUri, context.RequestAborted)
            .ConfigureAwait(false);

        Answers.Redirect(context, address.ToString());
    }

    /// <summary>Takes the provider's answer, and keeps the session it stands for.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the redirect is written.</returns>
    /// <exception cref="OAuthProviderException">The provider refused the exchange.</exception>
    private async Task CallbackAsync(HttpContext context)
    {
        if (!_blocks.ExpectsOAuth)
        {
            await Answers.NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        var provider = _oauth.Provider;
        var session = _oauth.Read(context);
        var expected = session[GradioOAuthSession.StateKey]?.ToString();
        var given = context.Request.Query["state"].ToString();

        if (string.IsNullOrEmpty(expected) || !string.Equals(expected, given, StringComparison.Ordinal))
        {
            await StartOverAsync(context, session).ConfigureAwait(false);

            return;
        }

        var code = context.Request.Query["code"].ToString();

        if (code.Length == 0)
        {
            await Answers
                .JsonAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    new JsonObject { ["detail"] = "The identity provider sent no authorization code." })
                .ConfigureAwait(false);

            return;
        }

        var redirectUri = session[RedirectKey]?.ToString() is { Length: > 0 } recorded
            ? new Uri(recorded, UriKind.Absolute)
            : CallbackUrl(context);

        var grant = await provider
            .ExchangeAsync(code, redirectUri, context.RequestAborted)
            .ConfigureAwait(false);

        GradioOAuthSession.Record(session, grant);
        session.Remove(RedirectKey);
        _oauth.Write(context, session);

        Answers.Redirect(context, OAuthRedirect.TargetOf(context));
    }

    /// <summary>Sends the browser back to the sign-in route after a callback it cannot accept.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="session">The session the callback carried, which held no usable state.</param>
    /// <returns>A task that completes once the redirect is written.</returns>
    /// <remarks>
    /// <para>
    /// The state this browser held is the one that did not match, so it is unusable: it is removed
    /// before the next attempt writes its own, which is the Python version's cleanup of the keys a
    /// failed sign-in left behind.
    /// </para>
    /// <para>
    /// A browser that has been sent round this loop too many times is let through to the page
    /// instead of being sent again: the alternative is a redirect chain that never ends, and the
    /// page at least says the application needs a sign-in it cannot complete. The Python version
    /// sends that browser to the provider's own page instead, which is a Space's way of escaping a
    /// frame it cannot use.
    /// </para>
    /// </remarks>
    private Task StartOverAsync(HttpContext context, JsonObject session)
    {
        var incoming = int.TryParse(context.Request.Query["_nb_redirects"], out var counted) ? counted : 0;
        var target = context.Request.Query["_target_url"].ToString();

        session.Remove(GradioOAuthSession.StateKey);
        session.Remove(RedirectKey);

        // A cookie the browser sent but this server cannot read is a cookie from a previous secret,
        // or one truncated on the way; the Python version deletes it here so that the next attempt starts
        // from nothing.
        if (context.Request.Cookies.ContainsKey("session") && session.Count == 0)
        {
            OAuthSessionCookie.Delete(context);
        }
        else
        {
            _oauth.Write(context, session);
        }

        // The count that arrived is the one compared, which is the Python version's: it takes three failed
        // attempts to reach the limit and the fourth to be let through.
        if (incoming > MaxRedirects)
        {
            Answers.Redirect(context, OAuthRedirect.TargetOf(context));

            return Task.CompletedTask;
        }

        var query = new StringBuilder("_nb_redirects=").Append(incoming + 1);

        if (target.Length > 0)
        {
            query.Append("&_target_url=").Append(Uri.EscapeDataString(target));
        }

        Answers.Redirect(context, AuthorizePath + "?" + query);

        return Task.CompletedTask;
    }

    /// <summary>Where the provider sends the browser back to, with the page it came from.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The absolute address of the callback.</returns>
    /// <remarks>
    /// The page the user started from travels as <c>_target_url</c>: the route was reached from that
    /// page with its query string, and the callback has to know it to send the user back there. A
    /// request that already names a target keeps it, which is what makes the second attempt of a
    /// failed sign-in return to the same page as the first.
    /// </remarks>
    private static Uri CallbackUrl(HttpContext context)
    {
        var target = context.Request.Query["_target_url"].ToString();

        if (target.Length == 0)
        {
            target = "/?" + context.Request.QueryString.Value?.TrimStart('?');
        }

        var query = new StringBuilder("_target_url=").Append(Uri.EscapeDataString(target));

        if (context.Request.Query.TryGetValue("_nb_redirects", out var redirects) && redirects.Count > 0)
        {
            query.Append("&_nb_redirects=").Append(Uri.EscapeDataString(redirects.ToString()));
        }

        return new Uri(RootUrl.Of(context) + CallbackPath + "?" + query, UriKind.Absolute);
    }
}

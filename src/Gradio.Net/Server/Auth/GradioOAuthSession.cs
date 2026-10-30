using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Who a request is signed in as, with the application's identity provider.
/// </summary>
/// <remarks>
/// <para>
/// An application that offers a <c>LoginButton</c> signs its users in through an identity provider,
/// and the answer to "who is this" then lives in a signed cookie rather than in the host's own
/// authentication. This is where that answer is read: <see cref="ProfileOf"/> for the person and
/// <see cref="TokenOf"/> for the token their profile came with.
/// </para>
/// <para>
/// It is a service rather than a parameter on a handler because a handler cannot see the request:
/// the run it belongs to was started by a different one. An application that needs the profile
/// takes this from its container — in a middleware of its own, or in a handler that resolved it
/// while serving the page (see the scope ledger for what that leaves out).
/// </para>
/// <para>
/// A session whose token has expired is not reported: it is dropped, and the request counts as
/// signed out, which is the Python version's rule. Leaving a stale token in a cookie would keep a
/// page saying "signed in" for a token nothing accepts.
/// </para>
/// </remarks>
public sealed class GradioOAuthSession
{
    /// <summary>The session field the signed-in user is kept in, which is the Python version's name
    /// for it.</summary>
    private const string InfoKey = "oauth_info";

    /// <summary>The session field the state of a sign-in attempt is kept in.</summary>
    internal const string StateKey = "oauth_state";

    private readonly IGradioOAuthProvider? _provider;
    private readonly OAuthSessionCookie _cookie;
    private readonly TimeProvider _time;

    /// <summary>Creates the service.</summary>
    /// <param name="provider">The identity provider, or <see langword="null"/> for an application that has none.</param>
    /// <param name="clientSecret">
    /// The client secret the cookie is keyed from, or <see langword="null"/> for a host without one —
    /// in which case the key lasts as long as the process.
    /// </param>
    /// <param name="time">The clock that decides when a token expired, or <see langword="null"/> for the system's.</param>
    internal GradioOAuthSession(IGradioOAuthProvider? provider, string? clientSecret, TimeProvider? time = null)
    {
        _provider = provider;
        _cookie = OAuthSessionCookie.For(clientSecret);
        _time = time ?? TimeProvider.System;
    }

    /// <summary>The identity provider, for the routes that start and finish a sign-in.</summary>
    /// <exception cref="InvalidOperationException">The application has no identity provider.</exception>
    internal IGradioOAuthProvider Provider =>
        _provider ?? throw new InvalidOperationException(
            "The application offers a LoginButton but no identity provider is configured. Register an "
            + "IGradioOAuthProvider — OidcOAuthProvider talks to any provider that publishes a discovery "
            + "document — or set OAUTH_CLIENT_ID, OAUTH_CLIENT_SECRET, OAUTH_SCOPES and OPENID_PROVIDER_URL.");

    /// <summary>Who the request is signed in as, or <see langword="null"/> when it is not.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The profile, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    public OAuthProfile? ProfileOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (InfoOf(context) is not { } info || info["userinfo"] is not JsonObject user)
        {
            return null;
        }

        return new OAuthProfile(
            Name: Text(user, "name") ?? string.Empty,
            UserName: Text(user, "preferred_username") ?? string.Empty,
            ProfileUrl: Text(user, "profile") ?? string.Empty,
            PictureUrl: Text(user, "picture") ?? string.Empty);
    }

    /// <summary>The token the request's session was signed in with, or <see langword="null"/> when it was not.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The token, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    public OAuthToken? TokenOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (InfoOf(context) is not { } info || Text(info, "access_token") is not { Length: > 0 } token)
        {
            return null;
        }

        return new OAuthToken(token, Text(info, "scope"), ExpiryOf(info));
    }

    /// <summary>The session a request carries.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The session's fields, which is empty for a request that carries none.</returns>
    internal JsonObject Read(HttpContext context) => _cookie.Read(context);

    /// <summary>Keeps a session in the browser.</summary>
    /// <param name="context">The request whose response carries it.</param>
    /// <param name="session">The fields to keep.</param>
    internal void Write(HttpContext context, JsonObject session) => _cookie.Write(context, session);

    /// <summary>Records a completed sign-in in a session.</summary>
    /// <param name="session">The session to write into.</param>
    /// <param name="grant">What the provider answered.</param>
    /// <remarks>
    /// The shape is the Python version's: the provider's token answer, with the profile under
    /// <c>userinfo</c> and the expiry as a Unix timestamp under <c>expires_at</c>.
    /// </remarks>
    internal static void Record(JsonObject session, OAuthGrant grant)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(grant);

        session[InfoKey] = new JsonObject
        {
            ["access_token"] = grant.AccessToken,
            ["token_type"] = grant.TokenType,
            ["scope"] = grant.Scope,
            ["id_token"] = grant.IdToken,
            ["expires_at"] = grant.ExpiresAt?.ToUnixTimeSeconds(),
            ["userinfo"] = new JsonObject
            {
                ["name"] = grant.Profile.Name,
                ["preferred_username"] = grant.Profile.UserName,
                ["profile"] = grant.Profile.ProfileUrl,
                ["picture"] = grant.Profile.PictureUrl,
            },
        };

        session.Remove(StateKey);
    }

    /// <summary>Forgets the signed-in user of a session, keeping the rest of it.</summary>
    /// <param name="session">The session.</param>
    internal static void Forget(JsonObject session)
    {
        ArgumentNullException.ThrowIfNull(session);

        session.Remove(InfoKey);
    }

    /// <summary>The signed-in user of a request, or <see langword="null"/> when there is none or the token expired.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The session's <c>oauth_info</c>, or <see langword="null"/>.</returns>
    /// <remarks>
    /// An expired sign-in is dropped from the session rather than merely reported as absent, which
    /// is the Python version's <c>session.pop("oauth_info")</c>: leaving it there would keep a page
    /// saying "signed in" for a token nothing accepts, and the cookie would carry the stale token
    /// for a fortnight.
    /// </remarks>
    private JsonObject? InfoOf(HttpContext context)
    {
        var session = Read(context);

        if (session[InfoKey] is not JsonObject info)
        {
            return null;
        }

        if (ExpiryOf(info) is { } expiresAt && expiresAt <= _time.GetUtcNow())
        {
            Forget(session);
            Write(context, session);

            return null;
        }

        return info;
    }

    /// <summary>When a session's token expires.</summary>
    /// <param name="info">The session's <c>oauth_info</c>.</param>
    /// <returns>The moment, or <see langword="null"/> when the provider did not say.</returns>
    private static DateTimeOffset? ExpiryOf(JsonObject info) =>
        info["expires_at"] is JsonValue value && value.TryGetValue<long>(out var seconds)
            ? DateTimeOffset.FromUnixTimeSeconds(seconds)
            : null;

    /// <summary>A string field of a session document, or <see langword="null"/> when it is absent.</summary>
    private static string? Text(JsonObject document, string key) =>
        document[key] is JsonValue value ? value.ToString() : null;
}

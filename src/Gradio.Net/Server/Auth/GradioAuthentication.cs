using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Who is calling, and the tokens that say so: what signing in with a username and password
/// produces.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps a dictionary of tokens on its application object and puts one in a
/// cookie when someone signs in; this is that dictionary and that cookie, per application, drawn
/// when the container is built. The cookie's name carries a per-process identifier so that two
/// applications on one host do not read each other's sessions — which is what the Python version's
/// <c>cookie_id</c> is for.
/// </para>
/// <para>
/// Two cookies are written, exactly as the Python version writes them: one marked
/// <c>SameSite=None</c> for the cross-site case a Space lives in, and one plain, because a browser
/// that refuses the first still sends the second. Reading prefers the marked one, so the two only
/// differ where the browser made them differ.
/// </para>
/// <para>
/// A sign-in is a token in this process's memory, so it ends when the process does. That is the
/// Python version's behaviour too, and it is why the store is not abstracted: a distributed
/// deployment would need a different answer for the cookie as well, which is a decision rather than
/// a detail. The word for it here is token rather than session, because a **Session** in this
/// framework is the frontend's session and the state it owns — and an **OAuth Session** is a
/// different cookie again.
/// </para>
/// </remarks>
internal sealed class GradioAuthentication
{
    /// <summary>The prefix of both cookie names, which is the Python version's.</summary>
    private const string CookiePrefix = "access-token-";

    /// <summary>The prefix of the cookie marked for cross-site use.</summary>
    private const string SecurePrefix = "access-token-unsecure-";

    private readonly Blocks _blocks;
    private readonly ConcurrentDictionary<string, string> _tokens = new(StringComparer.Ordinal);

    /// <summary>The key a request's answer is remembered under.</summary>
    /// <remarks>
    /// FastAPI caches a dependency's answer for the length of the request, and this is the same
    /// question asked by several layers — the gate that lets a request through, the page and the
    /// config it carries, and the run the request admits. A host's own rule that counts its calls
    /// would otherwise see one request several times.
    /// </remarks>
    private static readonly object CachedName = new();

    /// <summary>Creates the service.</summary>
    /// <param name="blocks">The application whose users are signed in.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    public GradioAuthentication(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <summary>Whether the application asks anyone to sign in.</summary>
    internal bool IsEnabled => _blocks.RequiresLogin;

    /// <summary>The identifier this application's cookies carry.</summary>
    /// <remarks>
    /// Drawn per process rather than configured, as the Python version draws it: its job is to keep
    /// one application's sessions out of another's, and a value that is the same everywhere would
    /// not.
    /// </remarks>
    internal string CookieId { get; } = Answers.UrlSafeToken(32);

    /// <summary>The cookie a signed-in browser carries, marked for cross-site use.</summary>
    private string SecureCookie => CookiePrefix + CookieId;

    /// <summary>The cookie a signed-in browser carries that works where the marked one is refused.</summary>
    private string PlainCookie => SecurePrefix + CookieId;

    /// <summary>Who is calling, or <see langword="null"/> when nobody is signed in.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The caller's name, or <see langword="null"/>.</returns>
    /// <remarks>
    /// <para>
    /// When the host authenticates its own requests, that answer is the only one: the tokens this
    /// application would have issued are not consulted, because an application that defers to the
    /// host has nobody signing in through it.
    /// </para>
    /// <para>
    /// The answer is remembered on the request, so that asking twice — once to let the request
    /// through, once to tell a run who it is for — asks the host once.
    /// </para>
    /// </remarks>
    internal async Task<string?> UserNameOfAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Items.TryGetValue(CachedName, out var remembered))
        {
            return remembered as string;
        }

        var userName = _blocks.AuthDependency is { } dependency
            ? await dependency(context, context.RequestAborted).ConfigureAwait(false)
            : NameOfToken(TokenOf(context));

        context.Items[CachedName] = userName;

        return userName;
    }

    /// <summary>The token a request carries, or <see langword="null"/> when it carries none.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The token as the cookie spells it, whether or not it names a sign-in.</returns>
    internal string? TokenOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Request.Cookies[SecureCookie] ?? context.Request.Cookies[PlainCookie];
    }

    /// <summary>The token the marked cookie carries, or <see langword="null"/> when there is none.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The token, whether or not it names a sign-in.</returns>
    /// <remarks>
    /// The marked cookie only, without the fallback <see cref="TokenOf"/> makes: the Python
    /// version's <c>/token</c> reads that one cookie and <c>/user</c> reads both, and the asymmetry
    /// is kept because a client written against one of them expects what that one answers.
    /// </remarks>
    internal string? SecureTokenOf(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Request.Cookies[SecureCookie];
    }

    /// <summary>The name a token was issued for, or <see langword="null"/> when it names no sign-in.</summary>
    /// <param name="token">The token, or <see langword="null"/>.</param>
    internal string? NameOfToken(string? token) =>
        token is not null && _tokens.TryGetValue(token, out var userName) ? userName : null;

    /// <summary>Issues a token for a sign-in and puts it in the browser's cookies.</summary>
    /// <param name="context">The request whose response carries the cookies.</param>
    /// <param name="userName">Who signed in.</param>
    /// <remarks>
    /// Both cookies carry the same token, which is the Python version's arrangement: the marked one
    /// is what a Space's cross-site page can use, and the plain one is what a browser that blocks
    /// it falls back to.
    /// </remarks>
    internal void SignIn(HttpContext context, string userName)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(userName);

        var token = Answers.UrlSafeToken(16);

        _tokens[token] = userName;

        context.Response.Cookies.Append(
            SecureCookie,
            token,
            new CookieOptions
            {
                Path = "/",
                HttpOnly = true,

                // A cookie marked SameSite=None is one a browser refuses unless it is also marked
                // Secure, so a host on plain HTTP gets Lax instead: the Python version can hardcode None
                // because a Space is always HTTPS.
                SameSite = context.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Secure = context.Request.IsHttps,
            });

        context.Response.Cookies.Append(
            PlainCookie,
            token,
            new CookieOptions { Path = "/", HttpOnly = true });
    }

    /// <summary>Ends the request's sign-in.</summary>
    /// <param name="context">The request whose response ends it.</param>
    /// <param name="allSessions">
    /// Whether to end every sign-in the same user has, or only this browser's — the Python
    /// version's <c>all_session</c>, which defaults to the former.
    /// </param>
    /// <remarks>
    /// Deleting the cookie alone would leave the token usable by anyone who had copied it, so the
    /// tokens are dropped as well: signing out is meant to be the end of the sign-in, not of the
    /// browser's copy of it.
    /// </remarks>
    internal void SignOut(HttpContext context, bool allSessions)
    {
        ArgumentNullException.ThrowIfNull(context);

        var userName = NameOfToken(TokenOf(context));

        context.Response.Cookies.Delete(SecureCookie, new CookieOptions { Path = "/" });
        context.Response.Cookies.Delete(PlainCookie, new CookieOptions { Path = "/" });

        if (userName is null)
        {
            return;
        }

        if (allSessions)
        {
            foreach (var (token, name) in _tokens)
            {
                if (string.Equals(name, userName, StringComparison.Ordinal))
                {
                    _tokens.TryRemove(token, out _);
                }
            }

            return;
        }

        if (TokenOf(context) is { } current)
        {
            _tokens.TryRemove(current, out _);
        }
    }
}

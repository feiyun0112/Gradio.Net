using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The cookie that carries a signed-in user's identity-provider session, signed so that it cannot be
/// forged.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps this in a Starlette session cookie: a JSON document, signed with a
/// secret derived from the OAuth client secret. This is the same thing written out — the document
/// in the clear, base64url-encoded, with an HMAC-SHA256 of it beside it — because a browser-side
/// session that a client could edit would be a session a client could grant itself.
/// </para>
/// <para>
/// The key is the SHA-256 of the client secret with the Python version's own version suffix, which
/// is the Python version's derivation: a host that changes its client secret signs everyone out,
/// and one that keeps it keeps sessions across a restart. A host with no secret — one whose
/// provider does not have one — gets a key drawn for the process instead, and its sessions end when
/// it does.
/// </para>
/// <para>
/// The <c>Secure</c> attribute is set from the request rather than always: the Python version can
/// hardcode it because a Space is always HTTPS, while this framework is also run on plain HTTP
/// locally — where a cookie marked <c>Secure</c> is never sent back, and the flow would end at the
/// callback.
/// </para>
/// </remarks>
internal sealed class OAuthSessionCookie
{
    /// <summary>The cookie's name, which is the Python version's.</summary>
    private const string CookieName = "session";

    /// <summary>How long a signed-in session is kept, which is Starlette's default.</summary>
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(14);

    /// <summary>The Python version's suffix on the session secret; bumping it invalidates every
    /// cookie.</summary>
    private const string Version = "-v4";

    private readonly byte[] _key;

    private OAuthSessionCookie(byte[] key) => _key = key;

    /// <summary>The cookie's key for a host with the given client secret, or without one.</summary>
    /// <param name="clientSecret">The OAuth client secret, or <see langword="null"/> for a host without one.</param>
    /// <returns>The key.</returns>
    internal static OAuthSessionCookie For(string? clientSecret) =>
        new(string.IsNullOrEmpty(clientSecret)
            ? RandomNumberGenerator.GetBytes(32)
            : SHA256.HashData(Encoding.UTF8.GetBytes(clientSecret + Version)));

    /// <summary>The session a request carries, or an empty one.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The session's fields, empty when there is no cookie or it does not verify.</returns>
    /// <remarks>
    /// A cookie that does not verify is treated as no cookie rather than as an error: it is what a
    /// browser sends after the secret changed, and refusing the request would lock that browser out
    /// of the page that would have fixed it.
    /// </remarks>
    internal JsonObject Read(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.Cookies.TryGetValue(CookieName, out var value) || string.IsNullOrEmpty(value))
        {
            return new JsonObject();
        }

        var separator = value.LastIndexOf('.');
        if (separator <= 0 || separator == value.Length - 1)
        {
            return new JsonObject();
        }

        var payload = value[..separator];
        var signature = value[(separator + 1)..];

        if (!CryptographicOperations.FixedTimeEquals(Sign(payload), Decode(signature)))
        {
            return new JsonObject();
        }

        try
        {
            return JsonNode.Parse(Decode(payload)) as JsonObject ?? new JsonObject();
        }
        catch (JsonException)
        {
            return new JsonObject();
        }
    }

    /// <summary>Writes a session into the response's cookies.</summary>
    /// <param name="context">The request whose response carries it.</param>
    /// <param name="session">The fields to keep.</param>
    internal void Write(HttpContext context, JsonObject session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        var payload = Base64(Encoding.UTF8.GetBytes(JsonText.Serialize(session)));

        context.Response.Cookies.Append(
            CookieName,
            payload + "." + Base64(Sign(payload)),
            Options(context));
    }

    /// <summary>Removes the session from the browser.</summary>
    /// <param name="context">The request whose response carries the removal.</param>
    /// <remarks>
    /// Deleted rather than overwritten with an empty session: the Python version deletes the cookie
    /// when the callback finds it corrupted, and a browser that keeps an empty one would send it
    /// forever.
    /// </remarks>
    internal static void Delete(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.Cookies.Delete(CookieName, Options(context));
    }

    /// <summary>The attributes the cookie is written with.</summary>
    /// <param name="context">The request being answered.</param>
    private static CookieOptions Options(HttpContext context) => new()
    {
        Path = "/",
        HttpOnly = true,

        // SameSite=None is only valid on a cookie marked Secure, so a host on plain HTTP gets Lax:
        // a browser drops a None cookie without Secure, and the flow would end at the callback.
        SameSite = context.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
        Secure = context.Request.IsHttps,
        MaxAge = Lifetime,
        IsEssential = true,
    };

    /// <summary>The signature over a payload.</summary>
    /// <param name="payload">The encoded session.</param>
    private byte[] Sign(string payload) =>
        HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes(payload));

    /// <summary>A byte string in the URL-safe alphabet, without padding.</summary>
    private static string Base64(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    /// <summary>The bytes a URL-safe string spells, or none when it does not spell any.</summary>
    private static byte[] Decode(string text)
    {
        var padded = text.Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - (padded.Length % 4)) % 4);

        try
        {
            return Convert.FromBase64String(padded);
        }
        catch (FormatException)
        {
            return [];
        }
    }
}

using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// Signs users in with any provider that speaks OpenID Connect: Hugging Face, or anything else that
/// publishes a discovery document.
/// </summary>
/// <remarks>
/// <para>
/// This is the provider the Python version binds to Hugging Face, written against the protocol
/// rather than against one vendor, so that an application can point it at its own identity provider
/// by configuring <see cref="OAuthProviderOptions"/>. What the Python version's <c>authlib</c> does
/// for it — read the discovery document, send the browser to the authorization endpoint, exchange
/// the code, read the userinfo — is what this does, with <see cref="HttpClient"/> and nothing else.
/// </para>
/// <para>
/// The discovery document is read on the first sign-in rather than when the application starts, so
/// that a host whose provider is temporarily unreachable still starts and still serves everything
/// that is not a sign-in. A failed read is not remembered: the next sign-in asks again.
/// </para>
/// <para>
/// Nothing here is exercised against a real provider by the test suite — the flow is verified
/// against a local provider the tests run, and that limitation is written down here rather than
/// left for a reader to discover.
/// </para>
/// </remarks>
public sealed class OidcOAuthProvider : IGradioOAuthProvider
{
    private readonly OAuthProviderOptions _options;
    private readonly HttpClient _client;
    private readonly TimeProvider _time;
    private readonly object _gate = new();
    private Endpoints? _endpoints;
    private Task<Endpoints>? _discovering;

    /// <summary>Creates the provider.</summary>
    /// <param name="options">Who the application is to the provider, and where it is.</param>
    /// <param name="client">The client to call the provider with, or <see langword="null"/> for one of its own.</param>
    /// <param name="time">The clock that decides when a token expires, or <see langword="null"/> for the system's.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The options name neither the three endpoints nor a discovery document, or they name no client.
    /// </exception>
    public OidcOAuthProvider(OAuthProviderOptions options, HttpClient? client = null, TimeProvider? time = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrEmpty(options.ClientId))
        {
            throw new ArgumentException("An identity provider needs a client id.", nameof(options));
        }

        if (options.AuthorizeEndpoint is null
            && options.TokenEndpoint is null
            && options.UserInfoEndpoint is null
            && options.MetadataUrl is null)
        {
            throw new ArgumentException(
                "An identity provider needs its endpoints, or the discovery document that publishes them.",
                nameof(options));
        }

        _options = options;
        _client = client ?? new HttpClient();
        _time = time ?? TimeProvider.System;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The four parameters every OAuth 2.0 authorization request carries, plus the scope the
    /// application asked for. The provider sends the browser back to <paramref name="redirectUri"/>
    /// with a code and the same state, which is what the callback checks.
    /// </remarks>
    public async Task<Uri> AuthorizeUrlAsync(
        string state,
        Uri redirectUri,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(redirectUri);

        var endpoints = await EndpointsAsync().ConfigureAwait(false);

        var query = new (string Key, string Value)[]
        {
            ("response_type", "code"),
            ("client_id", _options.ClientId),
            ("redirect_uri", redirectUri.ToString()),
            ("scope", _options.Scopes),
            ("state", state),
        };

        var builder = new UriBuilder(endpoints.Authorize);
        var existing = builder.Query.TrimStart('?');
        var built = string.Join("&", query.Select(pair => $"{Escape(pair.Key)}={Escape(pair.Value)}"));

        builder.Query = existing.Length == 0 ? built : existing + "&" + built;

        return builder.Uri;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The exchange is the form-encoded POST of the OAuth 2.0 token endpoint, followed by the
    /// userinfo request that turns a token into a person: the token says what may be read, and the
    /// profile is what the application shows.
    /// </remarks>
    public async Task<OAuthGrant> ExchangeAsync(
        string code,
        Uri redirectUri,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(redirectUri);

        var endpoints = await EndpointsAsync().ConfigureAwait(false);

        using var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri.ToString()),
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
        ]);

        using var response = await _client
            .PostAsync(endpoints.Token, form, cancellationToken)
            .ConfigureAwait(false);

        var tokenDocument = await ReadAsync(response, "token", cancellationToken).ConfigureAwait(false);

        var accessToken = Text(tokenDocument, "access_token")
            ?? throw new OAuthProviderException("The identity provider's token answer carries no access token.");

        var tokenType = Text(tokenDocument, "token_type") ?? "bearer";
        var scope = Text(tokenDocument, "scope");
        var idToken = Text(tokenDocument, "id_token");
        var expiresAt = ExpiryOf(tokenDocument);

        var profile = await ProfileAsync(accessToken, endpoints.UserInfo, cancellationToken).ConfigureAwait(false);

        return new OAuthGrant(accessToken, tokenType, scope, idToken, expiresAt, profile);
    }

    /// <summary>Reads the userinfo the token grants access to.</summary>
    /// <param name="accessToken">The token the provider issued.</param>
    /// <param name="endpoint">Where the provider describes the token's owner.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The profile.</returns>
    /// <exception cref="OAuthProviderException">The provider answered with an error, or without a profile.</exception>
    private async Task<OAuthProfile> ProfileAsync(
        string accessToken,
        Uri endpoint,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _client
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        var document = await ReadAsync(response, "userinfo", cancellationToken).ConfigureAwait(false);

        return new OAuthProfile(
            Name: Required(document, "name"),
            UserName: Required(document, "preferred_username"),
            ProfileUrl: Required(document, "profile"),
            PictureUrl: Required(document, "picture"));
    }

    /// <summary>Reads a provider's answer as JSON, or says what it refused.</summary>
    /// <param name="response">The answer.</param>
    /// <param name="what">Which request it answers, for the message.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The document.</returns>
    /// <exception cref="OAuthProviderException">The answer was not a successful JSON document.</exception>
    private static async Task<JsonObject> ReadAsync(
        HttpResponseMessage response,
        string what,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthProviderException(
                $"The identity provider refused the {what} request with HTTP {(int)response.StatusCode}: {body}");
        }

        try
        {
            return JsonNode.Parse(body) as JsonObject
                ?? throw new OAuthProviderException($"The identity provider's {what} answer is not a JSON object.");
        }
        catch (JsonException error)
        {
            throw new OAuthProviderException($"The identity provider's {what} answer is not JSON.", error);
        }
    }

    /// <summary>When the token the provider issued stops being accepted.</summary>
    /// <param name="token">The token answer.</param>
    /// <returns>The moment, or <see langword="null"/> when the provider did not say.</returns>
    /// <remarks>
    /// <c>expires_in</c> is seconds from now, which is what the Python version reads it as. A
    /// provider that says nothing is taken at its word: the token is used until something refuses
    /// it.
    /// </remarks>
    private DateTimeOffset? ExpiryOf(JsonObject token)
    {
        if (token["expires_in"] is not JsonValue value)
        {
            return null;
        }

        var seconds = value.TryGetValue<double>(out var asNumber)
            ? asNumber
            : double.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : double.NaN;

        return double.IsNaN(seconds) ? null : _time.GetUtcNow().AddSeconds(seconds);
    }

    /// <summary>A string field of a provider's answer, or <see langword="null"/> when it is absent.</summary>
    private static string? Text(JsonObject document, string key) =>
        document[key] is JsonValue value ? value.ToString() : null;

    /// <summary>A string field the flow cannot do without.</summary>
    /// <exception cref="OAuthProviderException">The field is absent or empty.</exception>
    private static string Required(JsonObject document, string key) =>
        Text(document, key) is { Length: > 0 } text
            ? text
            : throw new OAuthProviderException(
                $"The identity provider's userinfo answer carries no '{key}'. A profile needs the name, the "
                + "preferred username, the profile address and the picture address.");

    /// <summary>The endpoints, discovered once.</summary>
    /// <exception cref="OAuthProviderException">The discovery document cannot be read.</exception>
    private async Task<Endpoints> EndpointsAsync()
    {
        if (_endpoints is { } known)
        {
            return known;
        }

        Task<Endpoints> discovery;

        lock (_gate)
        {
            if (_endpoints is { } raced)
            {
                return raced;
            }

            discovery = _discovering ??= DiscoverAsync();
        }

        return await discovery.ConfigureAwait(false);
    }

    /// <summary>Reads the discovery document, and forgets the attempt if it fails.</summary>
    /// <exception cref="OAuthProviderException">The document cannot be read.</exception>
    private async Task<Endpoints> DiscoverAsync()
    {
        try
        {
            var found = _options.AuthorizeEndpoint is { } authorize
                && _options.TokenEndpoint is { } token
                && _options.UserInfoEndpoint is { } userInfo
                ? new Endpoints(authorize, token, userInfo)
                : await ReadMetadataAsync().ConfigureAwait(false);

            _endpoints = found;

            return found;
        }
        finally
        {
            lock (_gate)
            {
                _discovering = null;
            }
        }
    }

    /// <summary>Reads the three endpoints out of the provider's discovery document.</summary>
    /// <exception cref="OAuthProviderException">The document cannot be read or does not name them.</exception>
    private async Task<Endpoints> ReadMetadataAsync()
    {
        var url = _options.MetadataUrl
            ?? throw new OAuthProviderException(
                "The identity provider names neither its endpoints nor the discovery document that publishes them.");

        using var response = await _client.GetAsync(url).ConfigureAwait(false);

        var document = await ReadAsync(response, "discovery", CancellationToken.None).ConfigureAwait(false);

        return new Endpoints(
            Address(document, "authorization_endpoint"),
            Address(document, "token_endpoint"),
            Address(document, "userinfo_endpoint"));
    }

    /// <summary>An absolute address named by the discovery document.</summary>
    /// <exception cref="OAuthProviderException">The field is absent or is not an absolute address.</exception>
    private static Uri Address(JsonObject document, string key) =>
        Text(document, key) is { Length: > 0 } text && Uri.TryCreate(text, UriKind.Absolute, out var address)
            ? address
            : throw new OAuthProviderException(
                $"The identity provider's discovery document names no usable '{key}'.");

    /// <summary>A value as it is written into a query string.</summary>
    private static string Escape(string value) => Uri.EscapeDataString(value);

    /// <summary>The three addresses a sign-in needs.</summary>
    /// <param name="Authorize">Where the browser is sent.</param>
    /// <param name="Token">Where a code is exchanged.</param>
    /// <param name="UserInfo">Where the token's owner is described.</param>
    private sealed record Endpoints(Uri Authorize, Uri Token, Uri UserInfo);
}

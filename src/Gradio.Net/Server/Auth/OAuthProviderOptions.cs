namespace Gradio.Net;
/// <summary>
/// How an application's identity provider is reached: who the application is to it, and where its
/// endpoints are.
/// </summary>
/// <remarks>
/// <para>
/// The Python version reads these from four environment variables — <c>OAUTH_CLIENT_ID</c>,
/// <c>OAUTH_CLIENT_SECRET</c>, <c>OAUTH_SCOPES</c> and <c>OPENID_PROVIDER_URL</c> — because in a
/// Space they are set by the platform. They are properties here so that a host can configure them
/// in code, and <see cref="FromEnvironment"/> is the reading of those four variables, which is what
/// the framework registers by default.
/// </para>
/// <para>
/// The three endpoints are optional because a provider that publishes a discovery document does not
/// need them: <see cref="MetadataUrl"/> is where the document is, and the endpoints are read out of
/// it on the first sign-in. A provider without a discovery document names them instead.
/// </para>
/// </remarks>
public sealed class OAuthProviderOptions
{
    /// <summary>The application's client id with the provider.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>The application's client secret with the provider.</summary>
    /// <remarks>
    /// It also keys the cookie that carries the signed-in session, which is why a host that changes
    /// it signs everyone out: the Python version derives its session secret from the same value,
    /// for the same reason.
    /// </remarks>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>What the application asks the provider for.</summary>
    /// <value><c>openid profile</c>, which is what reading a user's name and picture takes.</value>
    public string Scopes { get; set; } = "openid profile";

    /// <summary>Where the browser is sent to sign in, or <see langword="null"/> to discover it.</summary>
    public Uri? AuthorizeEndpoint { get; set; }

    /// <summary>Where a code is exchanged for a token, or <see langword="null"/> to discover it.</summary>
    public Uri? TokenEndpoint { get; set; }

    /// <summary>Where the token's owner is described, or <see langword="null"/> to discover it.</summary>
    public Uri? UserInfoEndpoint { get; set; }

    /// <summary>The provider's discovery document, or <see langword="null"/> when its endpoints are named.</summary>
    /// <value>The <c>/.well-known/openid-configuration</c> address, when the options come from the environment.</value>
    public Uri? MetadataUrl { get; set; }

    /// <summary>The options the environment describes, or <see langword="null"/> when it describes none.</summary>
    /// <returns>The options, or <see langword="null"/> when a variable the flow needs is missing.</returns>
    /// <remarks>
    /// <para>
    /// The Python version raises when one of the four is missing and OAuth is enabled, because a
    /// Space with <c>hf_oauth: true</c> always has them. Answering <see langword="null"/> here says
    /// the same thing in the form this framework can act on: no provider is configured, so the
    /// framework registers none, and an application that needs one has to say where it is.
    /// </para>
    /// <para>
    /// The metadata address is the provider's base URL with the well-known suffix, which is how
    /// the Python version spells it.
    /// </para>
    /// </remarks>
    public static OAuthProviderOptions? FromEnvironment()
    {
        var clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");
        var scopes = Environment.GetEnvironmentVariable("OAUTH_SCOPES");
        var providerUrl = Environment.GetEnvironmentVariable("OPENID_PROVIDER_URL");

        if (string.IsNullOrEmpty(clientId)
            || string.IsNullOrEmpty(clientSecret)
            || string.IsNullOrEmpty(scopes)
            || string.IsNullOrEmpty(providerUrl))
        {
            return null;
        }

        return new OAuthProviderOptions
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scopes = scopes,
            MetadataUrl = new Uri(providerUrl.TrimEnd('/') + "/.well-known/openid-configuration", UriKind.Absolute),
        };
    }
}

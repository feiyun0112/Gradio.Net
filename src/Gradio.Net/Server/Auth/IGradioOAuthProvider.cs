namespace Gradio.Net;
/// <summary>
/// The identity provider an application signs its users in with.
/// </summary>
/// <remarks>
/// <para>
/// The Python version binds its OAuth flow to Hugging Face through <c>authlib</c>. The flow itself
/// — send the browser to the provider with a state token, take the code back, exchange it for a
/// token, ask who it belongs to — is the same for every provider that speaks OAuth 2.0 with an
/// OpenID Connect discovery document, so it is the provider that is pluggable and the flow that is
/// fixed .
/// </para>
/// <para>
/// Two methods is the whole contract, because two calls is all the flow makes: one to build the
/// address the browser is sent to, and one to turn the code that comes back into a token and a
/// profile. An implementation that needs a discovery document fetches it inside those calls rather
/// than at startup, so that an application starts without its provider being reachable.
/// </para>
/// <para>
/// The default implementation is <see cref="OidcOAuthProvider"/>, which talks to any provider that
/// publishes a discovery document. A host with a provider that does not — or with one it wants to
/// drive differently — registers its own, and nothing else in the framework changes.
/// </para>
/// </remarks>
public interface IGradioOAuthProvider
{
    /// <summary>The address the browser is sent to so that the user can sign in.</summary>
    /// <param name="state">
    /// The value that ties the callback to this attempt. The provider echoes it back, and the
    /// callback refuses an answer that carries a different one.
    /// </param>
    /// <param name="redirectUri">
    /// Where the provider sends the browser once the user has signed in, query included — it is the
    /// callback route with the page the user started from.
    /// </param>
    /// <param name="cancellationToken">Cancels the answer.</param>
    /// <returns>The address to redirect to.</returns>
    Task<Uri> AuthorizeUrlAsync(string state, Uri redirectUri, CancellationToken cancellationToken = default);

    /// <summary>Turns the code the provider sent back into a token and a profile.</summary>
    /// <param name="code">The authorization code from the callback.</param>
    /// <param name="redirectUri">The same address that was sent to the provider.</param>
    /// <param name="cancellationToken">Cancels the exchange.</param>
    /// <returns>The grant, with the profile the token belongs to.</returns>
    /// <exception cref="OAuthProviderException">The provider refused the exchange.</exception>
    Task<OAuthGrant> ExchangeAsync(string code, Uri redirectUri, CancellationToken cancellationToken);
}

/// <summary>
/// What an identity provider answered when a code was exchanged: the token, and who it belongs to.
/// </summary>
/// <param name="AccessToken">The token the provider's API accepts.</param>
/// <param name="TokenType">How the token is presented — <c>bearer</c>, for every provider that matters.</param>
/// <param name="Scope">What the token grants, as the provider spelled it, or <see langword="null"/>.</param>
/// <param name="IdToken">The signed identity document, or <see langword="null"/> when none was issued.</param>
/// <param name="ExpiresAt">When the token stops being accepted, or <see langword="null"/> when the provider did not say.</param>
/// <param name="Profile">Who the token belongs to.</param>
/// <remarks>
/// The Python version keeps the provider's whole token answer in the session and adds
/// <c>userinfo</c> and <c>expires_at</c> to it. This is that answer as a type: the members are the
/// ones the flow reads, and the rest of the provider's document is not carried because nothing
/// would read it.
/// </remarks>
public sealed record OAuthGrant(
    string AccessToken,
    string TokenType,
    string? Scope,
    string? IdToken,
    DateTimeOffset? ExpiresAt,
    OAuthProfile Profile);

/// <summary>
/// An identity provider refused something the flow asked of it.
/// </summary>
/// <remarks>
/// Raised for a token exchange the provider answered with an error, and for an answer that does not
/// carry the token and the profile the flow needs. The callback lets it out rather than signing
/// someone in on half an answer: a session built from a failed exchange would be a session nobody
/// can use.
/// </remarks>
public sealed class OAuthProviderException : GradioException
{
    /// <summary>Creates the exception.</summary>
    public OAuthProviderException()
        : base("The identity provider refused the request.")
    {
    }

    /// <summary>Creates the exception with a message.</summary>
    /// <param name="message">What the provider refused.</param>
    public OAuthProviderException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with a message and the failure behind it.</summary>
    /// <param name="message">What the provider refused.</param>
    /// <param name="innerException">The failure behind it.</param>
    public OAuthProviderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

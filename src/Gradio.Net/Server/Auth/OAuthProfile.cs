namespace Gradio.Net;
/// <summary>
/// Who signed in with the application's identity provider.
/// </summary>
/// <param name="Name">Their full name, as the provider spells it.</param>
/// <param name="UserName">The name they are known by — the provider's <c>preferred_username</c>.</param>
/// <param name="ProfileUrl">The address of their profile page.</param>
/// <param name="PictureUrl">The address of their picture.</param>
/// <remarks>
/// <para>
/// The Python version's <c>OAuthProfile</c>, whose four members are read from the identity
/// provider's userinfo answer: <c>name</c>, <c>preferred_username</c>, <c>profile</c> and
/// <c>picture</c>. They are properties here rather than dictionary keys, which is the difference
/// between a language where a missing key is a runtime error and one where the compiler asks for
/// it.
/// </para>
/// <para>
/// A request that is not signed in has no profile rather than an empty one: <see langword="null"/>
/// is the answer everywhere, which is what the Python version's <c>Optional[gr.OAuthProfile]</c>
/// reads as "nobody".
/// </para>
/// </remarks>
public sealed record OAuthProfile(string Name, string UserName, string ProfileUrl, string PictureUrl);

/// <summary>
/// The token the identity provider issued for the signed-in user.
/// </summary>
/// <param name="Token">The access token, which the provider's API accepts as a bearer token.</param>
/// <param name="Scope">What the token grants, as the provider spelled it, or <see langword="null"/>.</param>
/// <param name="ExpiresAt">When the token stops being accepted, or <see langword="null"/> when the provider did not say.</param>
/// <remarks>
/// The Python version's <c>OAuthToken</c>. An expired token is not handed out: the session that
/// carried it is treated as signed out, which is what the Python version's
/// <c>_get_valid_oauth_info_from_session</c> does rather than leaving a stale token in a cookie.
/// </remarks>
public sealed record OAuthToken(string Token, string? Scope, DateTimeOffset? ExpiresAt);

namespace Gradio.Net.Core;

public class OAuthToken
{
    public string Token { get; set; }
    public string Scope { get; set; }
    public long ExpiresAt { get; set; }

    public OAuthToken(string token, string scope, long expiresAt)
    {
        Token = token;
        Scope = scope;
        ExpiresAt = expiresAt;
    }
}

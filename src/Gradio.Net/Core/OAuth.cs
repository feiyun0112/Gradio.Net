using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net.Core;

public static class OAuth
{
    public static readonly string? OAuthClientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");
    public static readonly string? OAuthClientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");
    public static readonly string? OAuthScopes = Environment.GetEnvironmentVariable("OAUTH_SCOPES");
    public static readonly string? OpenIdProviderUrl = Environment.GetEnvironmentVariable("OPENID_PROVIDER_URL");
    public const int MaxRedirects = 2;

    private const string OAuthInfoSessionKey = "oauth_info";
    private const string StatePrefix = "_state_huggingface";

    public static void AttachOAuth(WebApplication app)
    {
        // Add session middleware first
        var sessionSecret = (OAuthClientSecret ?? "") + "-v4";
        // Session middleware should be added in ConfigureServices, but we include configuration here

        // Check if running in a Space
        if (Gradio.Net.Utils.Utils.GetSpace() != null)
        {
            AddOAuthRoutes(app);
        }
        else
        {
            AddMockedOAuthRoutes(app);
        }
    }

    private static void AddOAuthRoutes(WebApplication app)
    {
        // Validate environment variables
        if (string.IsNullOrEmpty(OAuthClientId))
        {
            throw new InvalidOperationException(
                "OAuth is required but OAUTH_CLIENT_ID environment variable is not set. " +
                "Make sure you've enabled OAuth in your Space by setting `hf_oauth: true` in the Space metadata.");
        }
        if (string.IsNullOrEmpty(OAuthClientSecret))
        {
            throw new InvalidOperationException(
                "OAuth is required but OAUTH_CLIENT_SECRET environment variable is not set. " +
                "Make sure you've enabled OAuth in your Space by setting `hf_oauth: true` in the Space metadata.");
        }
        if (string.IsNullOrEmpty(OAuthScopes))
        {
            throw new InvalidOperationException(
                "OAuth is required but OAUTH_SCOPES environment variable is not set. " +
                "Make sure you've enabled OAuth in your Space by setting `hf_oauth: true` in the Space metadata.");
        }
        if (string.IsNullOrEmpty(OpenIdProviderUrl))
        {
            throw new InvalidOperationException(
                "OAuth is required but OPENID_PROVIDER_URL environment variable is not set. " +
                "Make sure you've enabled OAuth in your Space by setting `hf_oauth: true` in the Space metadata.");
        }

        // Login endpoint - redirects to HuggingFace OAuth
        app.MapGet("/login/huggingface", async (HttpContext context) =>
        {
            var redirectUri = GenerateRedirectUri(context.Request);

            // Build OAuth authorization URL
            var state = GenerateState();
            context.Session.SetString($"{StatePrefix}_{state}", state);

            var authUrl = $"{OpenIdProviderUrl}/authorize?" +
                $"client_id={Uri.EscapeDataString(OAuthClientId)}&" +
                $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                $"scope={Uri.EscapeDataString(OAuthScopes ?? "openid profile")}&" +
                $"response_type=code&" +
                $"state={Uri.EscapeDataString(state)}";

            context.Response.Redirect(authUrl);
            return Results.Ok();
        });

        // Callback endpoint - handles OAuth callback
        app.MapGet("/login/callback", async (HttpContext context) =>
        {
            try
            {
                var code = context.Request.Query["code"].ToString();
                var state = context.Request.Query["state"].ToString();

                // Verify state to prevent CSRF
                var sessionState = context.Session.GetString($"{StatePrefix}_{state}");
                if (string.IsNullOrEmpty(sessionState) || sessionState != state)
                {
                    // State mismatch - likely corrupted cookie
                    return HandleStateMismatch(context);
                }

                // Exchange code for token
                var oauthInfo = await ExchangeCodeForToken(code, GenerateRedirectUri(context.Request));

                // Store OAuth info in session
                context.Session.SetString(OAuthInfoSessionKey, JsonSerializer.Serialize(oauthInfo));

                // Clean up state
                context.Session.Remove($"{StatePrefix}_{state}");

                return RedirectToTarget(context.Request);
            }
            catch (Exception ex)
            {
                return Results.Redirect("/");
            }
        });

        // Logout endpoint
        app.MapGet("/logout", (HttpContext context) =>
        {
            context.Session.Remove(OAuthInfoSessionKey);
            return RedirectToTarget(context.Request);
        });
    }

    private static void AddMockedOAuthRoutes(WebApplication app)
    {
        var mockedOAuthInfo = GetMockedOAuthInfo();

        // Mocked login endpoint
        app.MapGet("/login/huggingface", (HttpContext context) =>
        {
            var redirectUri = GenerateRedirectUri(context.Request);
            var callbackUrl = $"/login/callback?_target_url={Uri.EscapeDataString(redirectUri)}";
            return Results.Redirect(callbackUrl);
        });

        // Mocked callback endpoint
        app.MapGet("/login/callback", (HttpContext context) =>
        {
            context.Session.SetString(OAuthInfoSessionKey, JsonSerializer.Serialize(mockedOAuthInfo));
            return RedirectToTarget(context.Request);
        });

        // Mocked logout endpoint
        app.MapGet("/logout", (HttpContext context) =>
        {
            context.Session.Remove(OAuthInfoSessionKey);
            return RedirectToTarget(context.Request);
        });
    }

    private static IResult HandleStateMismatch(HttpContext context)
    {
        // Delete all state keys
        var keys = context.Session.Keys.Where(k => k.StartsWith(StatePrefix)).ToList();
        foreach (var key in keys)
        {
            context.Session.Remove(key);
        }

        // Parse query params
        var nbRedirects = int.TryParse(context.Request.Query["_nb_redirects"], out var n) ? n : 0;
        var targetUrl = context.Request.Query["_target_url"].ToString();

        // Build login URI with same query params and bump redirect count
        var queryParams = new Dictionary<string, string>
        {
            ["_nb_redirects"] = (nbRedirects + 1).ToString()
        };
        if (!string.IsNullOrEmpty(targetUrl))
        {
            queryParams["_target_url"] = targetUrl;
        }

        var loginUri = $"/login/huggingface?{string.Join("&", queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}";

        // If redirected more than MAX_REDIRECTS times, redirect to non-iframe view
        if (nbRedirects > MaxRedirects)
        {
            var host = Environment.GetEnvironmentVariable("SPACE_HOST");
            if (string.IsNullOrEmpty(host))
            {
                throw new InvalidOperationException(
                    "Gradio is not running in a Space (SPACE_HOST environment variable is not set). " +
                    "Cannot redirect to non-iframe view.");
            }
            var hostUrl = "https://" + host.TrimEnd('/');
            return Results.Redirect(hostUrl + loginUri);
        }

        return Results.Redirect(loginUri);
    }

    private static async Task<Dictionary<string, object>> ExchangeCodeForToken(string code, string redirectUri)
    {
        using var httpClient = new HttpClient();

        var tokenEndpoint = $"{OpenIdProviderUrl}/token";
        var requestBody = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["client_id"] = OAuthClientId!,
            ["client_secret"] = OAuthClientSecret!
        };

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

        if (tokenResponse == null)
        {
            throw new InvalidOperationException("Failed to parse token response");
        }

        // Get userinfo
        if (tokenResponse.TryGetValue("access_token", out var accessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.ToString());

            var userinfoResponse = await httpClient.GetAsync($"{OpenIdProviderUrl}/userinfo");
            userinfoResponse.EnsureSuccessStatusCode();

            var userinfoContent = await userinfoResponse.Content.ReadAsStringAsync();
            var userinfo = JsonSerializer.Deserialize<Dictionary<string, object>>(userinfoContent);

            if (userinfo != null)
            {
                tokenResponse["userinfo"] = userinfo;
            }
        }

        return tokenResponse;
    }

    private static string GenerateState()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    private static string GenerateRedirectUri(HttpRequest request)
    {
        string target;

        if (request.Query.ContainsKey("_target_url"))
        {
            // If _target_url already in query params => respect it
            target = request.Query["_target_url"].ToString();
        }
        else
        {
            // Otherwise => keep query params
            var queryString = request.QueryString.HasValue ? request.QueryString.Value : "";
            target = "/?" + queryString.TrimStart('?');
        }

        // On Spaces, the redirect URI must always be https://<space_host>/login/callback
        var spaceHost = Environment.GetEnvironmentVariable("SPACE_HOST");
        if (!string.IsNullOrEmpty(spaceHost))
        {
            // When custom domain is used, SPACE_HOST is a comma-separated list
            spaceHost = spaceHost.Split(',')[0];

            var redirectUri = $"https://{spaceHost}/login/callback?" +
                $"_target_url={Uri.EscapeDataString(target)}";
            return redirectUri;
        }

        // Local development
        var scheme = request.Scheme;
        var host = request.Host.ToString();
        var redirectUriStr = $"{scheme}://{host}/login/callback?_target_url={Uri.EscapeDataString(target)}";
        return redirectUriStr;
    }

    private static IResult RedirectToTarget(HttpRequest request, string defaultTarget = "/")
    {
        var target = request.Query["_target_url"].FirstOrDefault() ?? defaultTarget;
        return Results.Redirect(target);
    }

    private static Dictionary<string, object> GetMockedOAuthInfo()
    {
        // In a real implementation, integrate with Hugging Face Hub to get actual user info
        // For now, return mock data

        var token = Environment.GetEnvironmentVariable("HF_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            token = "mock_token_" + Guid.NewGuid().ToString();
        }

        return new Dictionary<string, object>
        {
            ["access_token"] = token,
            ["token_type"] = "bearer",
            ["expires_in"] = 3600,
            ["id_token"] = "AAAAAAAAAAAAAAAAAAAAAAAAAA",
            ["scope"] = "openid profile",
            ["expires_at"] = DateTimeOffset.Now.AddHours(8).ToUnixTimeSeconds(),
            ["userinfo"] = new Dictionary<string, object>
            {
                ["sub"] = "11111111111111111111111",
                ["name"] = Environment.GetEnvironmentVariable("HF_USERNAME") ?? "Mock User",
                ["preferred_username"] = Environment.GetEnvironmentVariable("HF_USERNAME") ?? "mockuser",
                ["profile"] = $"https://huggingface.co/{Environment.GetEnvironmentVariable("HF_USERNAME") ?? "mockuser"}",
                ["picture"] = "https://huggingface.co/avatars/default.png",
                ["website"] = "",
                ["aud"] = "00000000-0000-0000-0000-000000000000",
                ["auth_time"] = DateTimeOffset.Now.ToUnixTimeSeconds(),
                ["nonce"] = "aaaaaaaaaaaaaaaaaaa",
                ["iat"] = DateTimeOffset.Now.ToUnixTimeSeconds(),
                ["exp"] = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds(),
                ["iss"] = "https://huggingface.co"
            }
        };
    }

    public static Dictionary<string, object>? GetOAuthInfo(HttpContext context)
    {
        var oauthInfoJson = context.Session.GetString(OAuthInfoSessionKey);
        if (string.IsNullOrEmpty(oauthInfoJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Dictionary<string, object>>(oauthInfoJson);
    }
}


namespace Gradio.Net.Core;

public class OAuthProfile : Dictionary<string, object>
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Profile { get; set; }
    public string Picture { get; set; }

    public OAuthProfile(Dictionary<string, object> data)
    {
        foreach (var kvp in data)
            this[kvp.Key] = kvp.Value;

        Name = data.TryGetValue("name", out var nameValue) ? nameValue?.ToString() ?? string.Empty : string.Empty;
        Username = data.TryGetValue("preferred_username", out var usernameValue) ? usernameValue?.ToString() ?? string.Empty : string.Empty;
        Profile = data.TryGetValue("profile", out var profileValue) ? profileValue?.ToString() ?? string.Empty : string.Empty;
        Picture = data.TryGetValue("picture", out var pictureValue) ? pictureValue?.ToString() ?? string.Empty : string.Empty;
    }
}

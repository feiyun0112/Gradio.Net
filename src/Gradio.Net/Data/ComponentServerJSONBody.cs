using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class ComponentServerJSONBody
{
    [JsonPropertyName("session_hash")]
    public string SessionHash { get; set; } = string.Empty;

    [JsonPropertyName("component_id")]
    public int ComponentId { get; set; }

    [JsonPropertyName("fn_name")]
    public string FnName { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();
}


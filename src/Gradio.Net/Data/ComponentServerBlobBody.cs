using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class ComponentServerBlobBody
{
    [JsonPropertyName("session_hash")]
    public string SessionHash { get; set; } = string.Empty;

    [JsonPropertyName("component_id")]
    public int ComponentId { get; set; }

    [JsonPropertyName("fn_name")]
    public string FnName { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public DataWithFiles Data { get; set; } = new DataWithFiles();
}

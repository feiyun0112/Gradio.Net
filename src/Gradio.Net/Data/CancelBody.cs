using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class CancelBody
{
    [JsonPropertyName("session_hash")]
    public string SessionHash { get; set; } = string.Empty;

    [JsonPropertyName("fn_index")]
    public int FnIndex { get; set; }

    [JsonPropertyName("event_id")]
    public string EventId { get; set; } = string.Empty;
}

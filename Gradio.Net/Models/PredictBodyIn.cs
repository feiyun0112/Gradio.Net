using System.Text.Json.Serialization;

namespace Gradio.Net;

public class PredictBodyIn
{
    [JsonPropertyName("session_hash")]
    public string SessionHash { get; set; }
    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
    [JsonPropertyName("data")]
    public object[] Data { get; set; }
    [JsonPropertyName("fn_index")]
    public int FnIndex { get; set; }
    [JsonPropertyName("trigger_id")]
    public int? TriggerId { get; set; }
    [JsonPropertyName("simple_format")]
    public bool SimpleFormat { get; set; }
    [JsonPropertyName("batched")]
    public bool Batched { get; set; }
}

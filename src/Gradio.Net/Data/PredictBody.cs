using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class PredictBody
{
    [JsonPropertyName("session_hash")]
    public string? SessionHash { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }

    [JsonPropertyName("data")]
    public List<object> Data { get; set; } = new List<object>();

    [JsonPropertyName("event_data")]
    public object? EventData { get; set; }

    [JsonPropertyName("fn_index")]
    public int? FnIndex { get; set; }

    [JsonPropertyName("trigger_id")]
    public int? TriggerId { get; set; }

    [JsonPropertyName("simple_format")]
    public bool SimpleFormat { get; set; } = false;

    [JsonPropertyName("batched")]
    public bool? Batched { get; set; } = false;

    [JsonPropertyName("api_name")]
    public string? ApiName { get; set; }
}

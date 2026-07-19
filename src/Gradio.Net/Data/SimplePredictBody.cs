using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class SimplePredictBody
{
    [JsonPropertyName("data")]
    public List<object> Data { get; set; } = new List<object>();

    [JsonPropertyName("session_hash")]
    public string? SessionHash { get; set; }
}


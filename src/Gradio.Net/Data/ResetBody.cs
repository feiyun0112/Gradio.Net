using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class ResetBody
{
    [JsonPropertyName("event_id")]
    public string EventId { get; set; } = string.Empty;
}

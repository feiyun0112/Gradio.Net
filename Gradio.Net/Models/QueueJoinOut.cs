using System.Text.Json.Serialization;

namespace Gradio.Net;

public class QueueJoinOut
{
    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

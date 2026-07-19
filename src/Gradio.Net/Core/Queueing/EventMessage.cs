using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public abstract class EventMessage
{
    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; protected set; } = string.Empty;
}

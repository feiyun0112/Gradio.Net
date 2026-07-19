using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class ProcessStartsMessage : EventMessage
{
    public ProcessStartsMessage()
    {
        Msg = "process_starts";
    }

    [JsonPropertyName("rank")]
    public int? Rank { get; set; }

    [JsonPropertyName("queue_size")]
    public int? QueueSize { get; set; }

    [JsonPropertyName("eta")]
    public double? Eta { get; set; }
}

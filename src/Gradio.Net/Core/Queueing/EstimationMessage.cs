using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class EstimationMessage : EventMessage
{
    public EstimationMessage()
    {
        Msg = "estimation";
    }

    [JsonPropertyName("rank")]
    public int? Rank { get; set; }

    [JsonPropertyName("queue_size")]
    public int QueueSize { get; set; }

    [JsonPropertyName("rank_eta")]
    public double? RankEta { get; set; }
}

using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class ProcessGeneratingMessage : EventMessage
{
    public ProcessGeneratingMessage()
    {
        Msg = "process_generating";
    }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("output")]
    public Dictionary<string, object> Output { get; set; } = new();

    [JsonPropertyName("progress_data")]
    public List<ProgressData>? ProgressData { get; set; }

    [JsonPropertyName("time_limit")]
    public double? TimeLimit { get; set; }
}

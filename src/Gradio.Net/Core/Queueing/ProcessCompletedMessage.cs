using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class ProcessCompletedMessage : EventMessage
{
    public ProcessCompletedMessage()
    {
        Msg = "process_completed";
    }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("output")]
    public Dictionary<string, object> Output { get; set; } = new();

    [JsonPropertyName("progress_data")]
    public List<ProgressData>? ProgressData { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Title { get; set; }
}

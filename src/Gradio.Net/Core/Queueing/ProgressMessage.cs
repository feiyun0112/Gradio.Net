using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class ProgressMessage : EventMessage
{
    public ProgressMessage()
    {
        Msg = "progress";
    }

    [JsonPropertyName("progress_data")]
    public List<ProgressData>? ProgressData { get; set; }
}

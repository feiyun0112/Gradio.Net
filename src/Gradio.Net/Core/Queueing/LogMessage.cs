using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class LogMessage : EventMessage
{
    public LogMessage()
    {
        Msg = "log";
    }

    [JsonPropertyName("log")]
    public string Log { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public string Level { get; set; } = "info";

    [JsonPropertyName("duration")]
    public double? Duration { get; set; } = 10;

    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

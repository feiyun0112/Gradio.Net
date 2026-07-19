using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class ProgressData
{
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    [JsonPropertyName("length")]
    public int? Length { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("progress")]
    public double? Progress { get; set; }

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

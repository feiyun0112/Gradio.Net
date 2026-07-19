using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class PredictBodyInternal : PredictBody
{
    [JsonPropertyName("request")]
    public object? Request { get; set; }

    [JsonIgnore]
    public Gradio.Net.Core.Request? GrRequest { get; set; }
}


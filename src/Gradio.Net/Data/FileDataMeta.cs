using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class FileDataMeta
{
    [JsonPropertyName("_type")]
    public string Type { get; set; } = "gradio.FileData";
}

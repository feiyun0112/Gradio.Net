using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class DataWithFiles
{
    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();

    [JsonPropertyName("files")]
    public List<Tuple<string, byte[]>> Files { get; set; } = new List<Tuple<string, byte[]>>();
}

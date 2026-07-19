using System.Text.Json.Serialization;
using Gradio.Net.Data;

namespace Gradio.Net.Components
{
    public class FileMessage
    {
        public FileData File { get; set; }
        public string AltText { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } = "file";
    }
}

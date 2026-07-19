using System.Text.Json.Serialization;

namespace Gradio.Net.Components
{
    public class TextMessage
    {
        public string Text { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";
    }
}

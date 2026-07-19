using System.Text.Json.Serialization;

namespace Gradio.Net.Core;

public class VibeEditBody
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;
}

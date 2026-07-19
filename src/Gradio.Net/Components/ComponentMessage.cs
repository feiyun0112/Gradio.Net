using System.Text.Json.Serialization;

namespace Gradio.Net.Components
{
    public class ComponentMessage
    {
        public string Component { get; set; }
        public object Value { get; set; }
        public Dictionary<string, object> ConstructorArgs { get; set; }
        public Dictionary<string, object> Props { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } = "component";
    }
}

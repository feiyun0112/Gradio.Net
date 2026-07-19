using System.Text.Json.Serialization;

namespace Gradio.Net.Components
{
    public class Message
    {
        public string Role { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public MetadataDict Metadata { get; set; }
        public List<object> Content { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public List<OptionDict> Options { get; set; }
    }
}

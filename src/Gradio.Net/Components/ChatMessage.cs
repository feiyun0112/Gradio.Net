
namespace Gradio.Net.Components
{
    public class ChatMessage
    {
        public object Content { get; set; }
        public string Role { get; set; } = "assistant";
        public MetadataDict Metadata { get; set; }
        public List<OptionDict> Options { get; set; }

        public ChatMessage()
        {
            Metadata = new MetadataDict();
            Options = new List<OptionDict>();
        }
    }
}

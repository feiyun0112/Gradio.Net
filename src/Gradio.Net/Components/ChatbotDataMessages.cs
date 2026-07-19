
namespace Gradio.Net.Components
{
    public class ChatbotDataMessages
    {
        public List<Message> Root { get; set; }

        public ChatbotDataMessages()
        {
            Root = new List<Message>();
        }

        public ChatbotDataMessages(List<Message> root)
        {
            Root = root;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public class ChatMessage
    {
        public string FilePath { get; set; }
        public string AltText { get; set; }
        public string TextMessage { get; set; }
    }

    public class ChatbotMessagePair
    {
        public ChatbotMessagePair(ChatMessage message, ChatMessage botMessage)
        {
            HumanMessage=message;
            AiMessage=botMessage;
        }
        public ChatbotMessagePair(string message, string botMessage)
        {
            HumanMessage = new ChatMessage { TextMessage = message };
            AiMessage = new ChatMessage { TextMessage = botMessage };
        }

        public ChatMessage HumanMessage { get; set; }
        public ChatMessage AiMessage { get; set; }
    }
}

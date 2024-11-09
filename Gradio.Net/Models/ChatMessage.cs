namespace Gradio.Net;

public class ChatMessage
{
    public string Role { get; set; }
    public string FilePath { get; set; }
    public string AltText { get; set; }
    public string TextMessage { get; set; }
}

public class ChatbotMessagePair
{
    public ChatbotMessagePair(ChatMessage message, ChatMessage botMessage)
    {
        if (message != null)
        {
            message.Role = "user";
        }
        if (botMessage != null)
        {
            botMessage.Role = "assistant";
        }
        HumanMessage = message;
        AiMessage = botMessage;
    }
    public ChatbotMessagePair(string message, string botMessage) : this(new ChatMessage { TextMessage = message }, new ChatMessage { TextMessage = botMessage })
    {
    }

    public ChatMessage HumanMessage { get; set; }
    public ChatMessage AiMessage { get; set; }
}

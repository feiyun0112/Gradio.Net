using Gradio.Net;

namespace demo;

public static class ChatbotDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Chatbot Demo");

        Chatbot chatbot = gr.Chatbot();
        Textbox msg = gr.Textbox(placeholder:"Enter to Submit");

        await msg.Submit(streamingFn: (input) => Respond(Textbox.Payload(input.Data[0]), Chatbot.Payload(input.Data[1])),
            inputs: [msg, chatbot], outputs: [msg, chatbot]);
    }

    static async IAsyncEnumerable<Output> Respond(string message, IList<ChatbotMessagePair> chatHistory)
    {
        chatHistory.Add(new ChatbotMessagePair(message,""));
        message = "You typed: " + message;
        for (int i = 0; i < message.Length; i++)
        {
            await Task.Delay(500);
            chatHistory.Last().AiMessage.TextMessage += message[i];

            yield return gr.Output("", chatHistory);
        }
    }
}

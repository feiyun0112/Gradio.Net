using Gradio.Net;
using System;

namespace demo
{
    public static class ChatbotDemo
    {
        public static async Task Create()
        {
            gr.Markdown("# Chatbot Demo");


            var chatbot = gr.Chatbot();
            var msg = gr.Textbox();

            var btn = gr.Button("Submit");
            await btn.Click(streamingFn: (input) => Respond(Textbox.Payload(input.Data[0]), Chatbot.Payload(input.Data[1])),
                inputs: new Component[] { msg, chatbot }, outputs: new Component[] { msg, chatbot });
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
}

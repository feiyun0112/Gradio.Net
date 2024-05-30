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
            await btn.Click(fn: async (input) => await Respond(Textbox.Payload(input.Data[0]), Chatbot.Payload(input.Data[1])),
                inputs: new Component[] { msg, chatbot }, outputs: new Component[] { msg, chatbot });
        }


        static string[] _botMessages = ["How are you?", "I love you", "I'm very hungry"];
        static Random _rnd= new Random(DateTime.Now.Millisecond);
        static async Task<Output> Respond(string message, IList<ChatbotMessagePair> chatHistory)
        {

            var botMessage = _botMessages[_rnd.Next(_botMessages.Length)];

            chatHistory.Add(new ChatbotMessagePair( message, botMessage));
            await  Task.Delay(2000);
            return gr.Output("", chatHistory );
        }
    }
}

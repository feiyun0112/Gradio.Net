using Gradio.Net;
using Gradio.Net.Components;

namespace demo;

public static class ChatbotDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Chatbot Demo");

        Chatbot chatbot = gr.Chatbot();
        Textbox msg = gr.Textbox(placeholder: "Enter to Submit");

        msg.Submit(
            fn: new Func<string, object, IAsyncEnumerable<object>>(Respond),
            inputs: new object[] { msg, chatbot }, outputs: new object[] { msg, chatbot });
    }

    static async IAsyncEnumerable<object> Respond(string message, object chatHistoryArg)
    {
        var historyList = new List<object>();
        if (chatHistoryArg is System.Collections.IEnumerable enumerable && chatHistoryArg is not string)
        {
            foreach (var item in enumerable)
                historyList.Add(item);
        }

        historyList.Add(new Dictionary<string, object> { { "role", "user" }, { "content", message } });
        historyList.Add(new Dictionary<string, object> { { "role", "assistant" }, { "content", "" } });

        string response = "You typed: " + message;
        for (int i = 0; i < response.Length; i++)
        {
            await Task.Delay(50);
            var lastMsg = historyList.Last() as Dictionary<string, object>;
            if (lastMsg != null) lastMsg["content"] = response.Substring(0, i + 1);

            yield return Tuple.Create("", (object)historyList);
        }
    }
}

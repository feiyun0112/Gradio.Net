using Gradio.Net;
using Gradio.Net.Components;

namespace demo;

public static class FirstDemo
{
    public static async Task Create()
    {
        gr.Markdown("# First Demo");

        gr.Markdown("Start typing below and then click **Run** to see the output.");
        Textbox input, output;
        using (gr.Row())
        {
            input = gr.Textbox(placeholder: "What is your name?");
            output = gr.Textbox();
        }
        Button btn = gr.Button("Run");
        btn.Click(fn: new Func<string, (string, string)>(name => ($"Welcome to Gradio.Net, {name}!", "")), inputs: new[] { input }, outputs: new[] { output, input });
    }
}

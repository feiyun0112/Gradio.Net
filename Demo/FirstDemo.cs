using Gradio.Net;

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
        await btn.Click(fn: async (input) => gr.Output($"Welcome to Gradio.Net, {Textbox.Payload(input.Data[0])}!", ""), inputs: [input], outputs: [output, input]);
    }
}

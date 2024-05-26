using Gradio.Net;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddGradio();

//var app = builder.Build();

//app.UseGradio(await CreateBlocks());

//app.Run();

App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    using (var blocks = gr.Blocks())
    {
        gr.Markdown("Start typing below and then click **Run** to see the output.");
        Textbox input, output;
        using (gr.Row())
        {
            input = gr.Textbox(placeholder: "What is your name?");
            output = gr.Textbox();
        }
        var btn = gr.Button("Run");
        await btn.Click(fn: async (input) => gr.Output( $"Welcome to Gradio.Net, {input.Data[0]}!"), inputs: new[] { input }, outputs: new[] { output });

        return blocks;
    }
}
using demo;
using Gradio.Net;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddGradio();

//var app = builder.Build();

//app.UseGradio(await CreateBlocks());

//app.Run();

App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    using (Blocks blocks = gr.Blocks())
    {
        await FirstDemo.Create();

        await ImageDemo.Create();

        await LayoutDemo.Create();

        await ChatbotDemo.Create();

        await FormDemo.Create();

        await ProgressDemo.Create();

        return blocks;
    }
}

 
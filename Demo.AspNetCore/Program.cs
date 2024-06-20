using demo;
using Gradio.Net;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddGradio();

//var app = builder.Build();

//app.UseGradio(await CreateBlocks());

//app.Run();

App.Launch(await CreateBlocks(), config =>
{
    config.Stylesheets = new string[] {
            "https://fonts.font.im/css2?family=Source+Sans+Pro:wght@400;600&display=swap",
            "https://fonts.font.im/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
        };
});

async Task<Blocks> CreateBlocks()
{
    using (Blocks blocks = gr.Blocks())
    {
        await FirstDemo.Create();

        await MediaDemo.Create();

        await LayoutDemo.Create();

        await ChatbotDemo.Create();

        await FormDemo.Create();

        await ProgressDemo.Create();

        return blocks;
    }
}
using demo;
using Gradio.Net;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

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
        await FirstDemo.Create();

        await ImageDemo.Create();

        await LayoutDemo.Create();

        return blocks;
    }
}

 
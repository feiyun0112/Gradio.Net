using demo;
using Gradio.Net;
using Gradio.Net.Themes;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddGradio();

//var app = builder.Build();

//app.UseGradio(await CreateBlocks());

//app.Run();

App.Launch(await CreateBlocks(), config =>
{
    config.GoogleFontUrlTemplate = @"https://fonts.font.im/css2?family={name}:wght@{weight}&display=swap";
    //config.Theme = Themes.Citrus;
    //config.Theme = Themes.Default.Set(overrideStyles: new Dictionary<string, string> { { nameof(Theme.ButtonSecondaryBackgroundFill), "#FF0000" } });

    config.Theme = Theme.Load("miku_theme.json");
});

async Task<Blocks> CreateBlocks()
{
    using (Blocks blocks = gr.Blocks())
    {
        using (gr.Row())
        {
            using (gr.Column(scale: 10))
            { }
            using (gr.Column(scale: 3))
            {
                var btn = gr.Button("Toggle Dark");
                btn.Click(fn: null,
                    js: """
                        () => {
                            document.body.classList.toggle('dark');
                        }
                        """
                            );
            }
        }

        await FirstDemo.Create();

        await MediaDemo.Create();

        await LayoutDemo.Create();

        await ChatbotDemo.Create();

        await FormDemo.Create();

        await ProgressDemo.Create();

        return blocks;
    }
}
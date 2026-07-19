using demo;
using Gradio.Net;
using Gradio.Net.Core;

await (await CreateBlocks()).Launch(
    theme: Gradio.Net.Core.Theme.Load("miku_theme.json"));

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
using Gradio.Net;
using Gradio.Net.Components;
using Gradio.Net.Core;

namespace demo;

public static class ProgressDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Progress Demo");

        Button load = gr.Button("Load");
        Label label = gr.Label(label: "Loader");
        load.Click(LoadSet, outputs: new[] { label });
    }

    static async Task<string> LoadSet(Progress progress)
    {
        const int count = 24;
        for (int i = 0; i < count; i++)
        {
            progress.Update((double)i / count, desc: "Loading...");
            await Task.Delay(100);
        }
        return "Loaded";
    }
}

using Gradio.Net;
using System.Reflection.Emit;

namespace demo
{
    public static class ProgressDemo
    {
        public static async Task Create()
        {
            gr.Markdown("# Progress Demo");

            var load = gr.Button("Load");
            var label = gr.Label(label: "Loader");
            load.Click(LoadSet, outputs: new[] { label });
        }

        static async Task<Output> LoadSet(Input input)
        {
            const int count = 24;
            input.Progress = gr.Progress(count);
            for (int i = 0; i < count; i++)
            {
                input.Progress.Report(i, desc: "Loading...");
                await Task.Delay(100);
            }
            return gr.Output("Loaded");
        }
    }
}

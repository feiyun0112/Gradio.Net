
![demo](./progress_demo.gif)

```C#
App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    using (var blocks = gr.Blocks())
    {
         gr.Markdown("# Progress Demo");

        var load = gr.Button("Load");
        var label = gr.Label(label: "Loader");
        load.Click(LoadSet, outputs: new[] { label });

        return blocks;
    }
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
```

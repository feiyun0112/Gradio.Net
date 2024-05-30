
![demo](./image_demo.gif)

```C#
App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    using (var blocks = gr.Blocks())
    {
        gr.Markdown("**Image Demo** upload a image and click button");
        Gradio.Net.Image input, output;
        using (gr.Row())
        {
            input = gr.Image();
            
            output = gr.Image();
        }
        var btn = gr.Button("Submit");
        await btn.Click(fn: async (input) => gr.Output(DrawWaterMarkOnImage(Gradio.Net.Image.Payload(input.Data[0]))), inputs: new[] { input }, outputs: new[] { output });

        return blocks;
    }
}


static string DrawWaterMarkOnImage(string inputImageFilePath)
{
    using (var img = SixLabors.ImageSharp.Image.Load(inputImageFilePath))
    {
        var outputFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
        Font font = SystemFonts.CreateFont("Arial", 10); // for scaling water mark size is largely ignored.

        using (var img2 = img.Clone(ctx => ApplyScalingWaterMarkSimple(ctx, font, "Gradio.Net", Color.HotPink, 5)))
        {
            img2.Save(outputFilePath);
        }
        return outputFilePath;
    }

}
static IImageProcessingContext ApplyScalingWaterMarkSimple(IImageProcessingContext processingContext,
          Font font,
          string text,
          Color color,
          float padding)
{
    Size imgSize = processingContext.GetCurrentSize();

    float targetWidth = imgSize.Width - (padding * 2);
    float targetHeight = imgSize.Height - (padding * 2);

    // Measure the text size
    FontRectangle size = TextMeasurer.MeasureSize(text, new TextOptions(font));

    // Find out how much we need to scale the text to fill the space (up or down)
    float scalingFactor = Math.Min(targetWidth / size.Width, targetHeight / size.Height);

    // Create a new font
    Font scaledFont = new Font(font, scalingFactor * font.Size);

    var center = new PointF(imgSize.Width / 2, imgSize.Height / 2);
    var textOptions = new RichTextOptions(scaledFont)
    {
        Origin = center,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };
    return processingContext.DrawText(textOptions, text, color);
}
```

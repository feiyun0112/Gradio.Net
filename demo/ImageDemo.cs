using Gradio.Net;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace demo;

public static class ImageDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Image Demo");

        gr.Markdown("upload a image and click button");
        Gradio.Net.Image input, outputFilePath, outputUrl;
        using (gr.Row())
        {
            input = gr.Image();

            outputFilePath = gr.Image();
            outputUrl = gr.Image();

        }
        Button btn = gr.Button("Submit");
        await btn.Click(fn: async (input) => gr.Output(DrawWaterMarkOnImage(Gradio.Net.Image.Payload(input.Data[0])), "https://www.nuget.org/profiles/MyIO/avatar?imageSize=64"), inputs: [input], outputs: [outputFilePath, outputUrl]);
    }

    static string DrawWaterMarkOnImage(string inputImageFilePath)
    {
        using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(inputImageFilePath))
        {
            string outputFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
            Font font = SystemFonts.CreateFont("Arial", 10); // for scaling water mark size is largely ignored.

            using (SixLabors.ImageSharp.Image img2 = img.Clone(ctx => ApplyScalingWaterMarkSimple(ctx, font, "Gradio.Net", Color.HotPink, 5)))
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
        Font scaledFont = new(font, scalingFactor * font.Size);

        PointF center = new(imgSize.Width / 2, imgSize.Height / 2);
        RichTextOptions textOptions = new(scaledFont)
        {
            Origin = center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        return processingContext.DrawText(textOptions, text, color);
    }
}

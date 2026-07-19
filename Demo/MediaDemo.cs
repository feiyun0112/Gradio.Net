using Gradio.Net;
using Gradio.Net.Components;
using Gradio.Net.Events;
using GrImage = Gradio.Net.Components.Image;
using GrNumber = Gradio.Net.Components.Number;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace demo;

public static class MediaDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Media Demo");

        GrImage input, outputFilePath, outputUrl;
        using (gr.Row())
        {
            input = gr.Image();

            outputFilePath = gr.Image();
            outputUrl = gr.Image();

        }
        Button btnImage = gr.Button("Submit");
        btnImage.Click(
            fn: new Func<string, (string, string)>(img => (DrawWaterMarkOnImage(img), "https://www.nuget.org/profiles/MyIO/avatar?imageSize=64")),
            inputs: new[] { input }, outputs: new[] { outputFilePath, outputUrl });


        Video inputVideo, outputVideo;
        using (gr.Row())
        {
            using (gr.Column())
            {
                inputVideo = gr.Video();
            }
            using (gr.Column())
            {
                outputVideo = gr.Video();
            }
            using (gr.Column())
            {
                GrNumber numChange = gr.Number(label: "# Change Events", value: 0);
                GrNumber numLoad = gr.Number(label: "# Upload Events", value: 0);

                inputVideo.Upload(fn: new Func<object, double, (object, double)>((vid, n) => (vid, n + 1)), inputs: new object[] { inputVideo, numLoad }, outputs: new object[] { outputVideo, numLoad });
                inputVideo.Change(fn: new Func<double, double>(n => n + 1), inputs: new[] { numChange }, outputs: new[] { numChange });

            }
        }


        Audio inputAudio, outputAudio;
        using (gr.Row())
        {
            using (gr.Column())
            {
                inputAudio = gr.Audio(type: "filepath", label: "Input Audio", sources: new List<string> { "upload", "microphone" });
            }
            using (gr.Column())
            {
                outputAudio = gr.Audio(label: "Output Audio", sources: new List<string> { "upload", "microphone" });

            }

            GrNumber inputNumChange, inputStop;
            GrNumber inputRecord;
            GrNumber outputNumStop;
            using (gr.Row())
            {
                using (gr.Column())
                {
                    inputNumChange = gr.Number(label: "# Input Change Events", value: 0);
                    inputStop = gr.Number(label: "# Input Stop Events", value: 0);
                }

                using (gr.Column())
                {
                    inputRecord = gr.Number(label: "# Input Start Recording Events", value: 0);
                }


                using (gr.Column())
                {
                    outputNumStop = gr.Number(label: "# Output Stop Events", value: 0);

                    inputAudio.Change(fn: new Func<double, double>(n => n + 1), inputs: new[] { inputNumChange }, outputs: new[] { inputNumChange });
                    inputAudio.Stop(fn: new Func<double, double>(n => n + 1), inputs: new[] { inputStop }, outputs: new[] { inputStop });

                    inputAudio.StartRecording(fn: new Func<double, double>(n => n + 1), inputs: new[] { inputRecord }, outputs: new[] { inputRecord });
                    inputAudio.StopRecording(fn: new Func<double, double>(n => n + 1), inputs: new[] { inputStop }, outputs: new[] { inputStop });

                    outputAudio.Stop(fn: new Func<double, double>(n => n + 1), inputs: new[] { outputNumStop }, outputs: new[] { outputNumStop });


                }
            }
        }
        FileComponent fileComponent, outputFile1;
        using (gr.Row())
        {
            using (gr.Column())
            {
                fileComponent = gr.File(label: "Upload Single File", fileCount: "single");
            }
            using (gr.Column())
            {
                outputFile1 = gr.File(label: "Upload Single File Output", fileCount: "single");

                var numLoadBtn1 = gr.Number(label: "# Load Upload Single File", value: 0);
                fileComponent.Upload(fn: new Func<object, double, (object, double)>((file, n) => (file, n + 1)), new object[] { fileComponent, numLoadBtn1 }, new object[] { outputFile1, numLoadBtn1 });
            }
        }

        FileComponent fileComponentMultiple, outputFile2;
        using (gr.Row())
        {
            using (gr.Column())
            {
                fileComponentMultiple = gr.File(label: "Upload Multiple File", fileCount: "multiple");
            }
            using (gr.Column())
            {
                outputFile2 = gr.File(label: "Upload Multiple File Output", fileCount: "multiple");

                var numLoadBtn2 = gr.Number(label: "# Load Upload Single File", value: 0);
                fileComponentMultiple.Upload(fn: new Func<object, double, (object, double)>((file, n) => (file, n + 1)), new object[] { fileComponentMultiple, numLoadBtn2 }, new object[] { outputFile2, numLoadBtn2 });
            }
        }
    }
    static string DrawWaterMarkOnImage(string inputImageFilePath)
    {
        if (inputImageFilePath == null)
        {
            return null;
        }

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

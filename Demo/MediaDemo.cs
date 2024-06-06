using Gradio.Net;
using Gradio.Net.Enums;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Reflection.Emit;

namespace demo;

public static class MediaDemo
{
    public static async Task Create()
    {
        gr.Markdown("# Media Demo");

        Gradio.Net.Image input, outputFilePath, outputUrl;
        using (gr.Row())
        {
            input = gr.Image();

            outputFilePath = gr.Image();
            outputUrl = gr.Image();

        }
        Button btnImage = gr.Button("Submit");
        await btnImage.Click(fn: async (input) => gr.Output(DrawWaterMarkOnImage(Gradio.Net.Image.Payload(input.Data[0])), "https://www.nuget.org/profiles/MyIO/avatar?imageSize=64"), inputs: [input], outputs: [outputFilePath, outputUrl]);


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
                Gradio.Net.Number numChange = gr.Number(label: "# Change Events", value: 0);
                Gradio.Net.Number numLoad = gr.Number(label: "# Upload Events", value: 0);
                Gradio.Net.Number numPlay = gr.Number(label: "# Play Events", value: 0);
                Gradio.Net.Number numPause = gr.Number(label: "# Pause Events", value: 0);

                inputVideo.Upload(fn: async (input) => gr.Output(input.Data[0], Gradio.Net.Number.Payload(input.Data[1]) + 1), inputs: [inputVideo, numLoad], outputs: [outputVideo, numLoad]);
                inputVideo.Change(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [numChange], outputs: [numChange]);
                inputVideo.Play(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [numPlay], outputs: [numPlay]);
                inputVideo.Pause(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [numPause], outputs: [numPause]);

            }
        }


        Audio inputAudio, outputAudio;
        using (gr.Row())
        {
            using (gr.Column())
            {
                inputAudio = gr.Audio(type: Gradio.Net.Enums.AudioType.Filepath, label: "Input Audio", sources: new List<AudioSource> { AudioSource.Upload, AudioSource.Microphone });
            }
            using (gr.Column())
            {
                outputAudio = gr.Audio(label: "Output Audio", sources: new List<AudioSource> { AudioSource.Upload, AudioSource.Microphone });

            }

            Gradio.Net.Number inputNumChange, inputNumLoad, inputNumPlay, inputNumPause;
            Gradio.Net.Number inputRecord, inputPause, inputStop;
            Gradio.Net.Number outputNumPlay, outputNumPause, outputNumStop;
            using (gr.Row())
            {
                using (gr.Column())
                {
                    inputNumChange = gr.Number(label: "# Input Change Events", value: 0);
                    inputNumLoad = gr.Number(label: "# Input Upload Events", value: 0);
                    inputNumPlay = gr.Number(label: "# Input Play Events", value: 0);
                    inputNumPause = gr.Number(label: "# Input Pause Events", value: 0);
                }

                using (gr.Column())
                {
                    inputRecord = gr.Number(label: "# Input Start Recording Events", value: 0);
                    inputPause = gr.Number(label: "# Input Pause Recording Events", value: 0);
                    inputStop = gr.Number(label: "# Input Stop Recording Events", value: 0);
                }


                using (gr.Column())
                {
                    outputNumPlay = gr.Number(label: "# Output Play Events", value: 0);
                    outputNumPause = gr.Number(label: "# Output Pause Events", value: 0);
                    outputNumStop = gr.Number(label: "# Output Stop Events", value: 0);

                    inputAudio.Upload(fn: async (input) => gr.Output(input.Data[0],Gradio.Net.Number.Payload(input.Data[1]) + 1), inputs: [inputAudio, inputNumLoad], outputs: [outputAudio, inputNumLoad]);
                    inputAudio.Change(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputNumChange], outputs: [inputNumChange]);
                    inputAudio.Play(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputNumPlay], outputs: [inputNumPlay]);
                    inputAudio.Pause(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputNumPause], outputs: [inputNumPause]);
                    inputAudio.Change(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputNumChange], outputs: [inputNumChange]);


                    inputAudio.StartRecording(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputRecord], outputs: [inputRecord]);
                    inputAudio.PauseRecording(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputPause], outputs: [inputPause]);
                    inputAudio.StopRecording(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [inputStop], outputs: [inputStop]);


                    outputAudio.Play(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [outputNumPlay], outputs: [outputNumPlay]);
                    outputAudio.Pause(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [outputNumPause], outputs: [outputNumPause]);
                    outputAudio.Stop(fn: async (input) => gr.Output(Gradio.Net.Number.Payload(input.Data[0]) + 1), inputs: [outputNumStop], outputs: [outputNumStop]);


                }
            }
        }
    }
    static string DrawWaterMarkOnImage(string inputImageFilePath)
    {
        if (inputImageFilePath == null) {
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

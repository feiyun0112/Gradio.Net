using System.Text;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;

namespace Gradio.Net.Components;

[Event("change")]
[Event("clear")]
[Event("start_recording")]
[Event("stop_recording")]
[Event("stop")]
[Event("play")]
[Event("pause")]
[Event("end")]
[Event("upload")]
public class Video : Component, IStreamingOutput
{
    public List<string> Sources { get; set; }
    public string? Format { get; set; }
    public bool Autoplay { get; set; }
    public bool Loop { get; set; }
    public List<string>? Buttons { get; set; }
    public object? Height { get; set; }
    public object? Width { get; set; }
    public object? Subtitles { get; set; }
    public WebcamOptions WebcamOptions { get; set; }
    public WatermarkOptions Watermark { get; set; }
    public bool IncludeAudio { get; set; }
    public bool Streaming { get; set; }

    public Video(
        object? value = null,
        object? sources = null,
        string? format = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        WebcamOptions? webcamOptions = null,
        bool? includeAudio = null,
        bool autoplay = false,
        List<string>? buttons = null,
        bool loop = false,
        bool streaming = false,
        object? watermark = null,
        object? subtitles = null,
        object? height = null,
        object? width = null)
    {
        Value = value;
        Label = label;
        ShowLabel = showLabel ?? true;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;

        var validSources = new List<string> { "upload", "webcam" };
        if (sources == null)
        {
            Sources = validSources;
        }
        else if (sources is string sourceStr && validSources.Contains(sourceStr))
        {
            Sources = new List<string> { sourceStr };
        }
        else if (sources is List<string> sourceList)
        {
            if (sourceList.Any(s => !validSources.Contains(s)))
            {
                throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
            }
            Sources = sourceList;
        }
        else if (sources is IEnumerable<object> sourceObjList)
        {
            Sources = sourceObjList.Select(s => s?.ToString() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (Sources.Any(s => !validSources.Contains(s)))
            {
                throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
            }
        }
        else
        {
            throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
        }

        Format = format;
        Autoplay = autoplay;
        Loop = loop;
        Buttons = buttons;
        Height = height;
        Width = width;
        WebcamOptions = webcamOptions ?? new WebcamOptions();
        Watermark = watermark as WatermarkOptions ?? new WatermarkOptions();
        if (watermark is string)
        {
            Watermark.Watermark = watermark;
        }

        IncludeAudio = includeAudio ?? Sources.Contains("upload");
        Streaming = streaming;
        Subtitles = ProcessSubtitles(subtitles);

        if (visible is bool vb)
        {
            Visible = vb;
        }
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "video";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["sources"] = Sources;
        config["format"] = Format;
        config["autoplay"] = Autoplay;
        config["loop"] = Loop;
        config["buttons"] = Buttons;
        config["height"] = Height;
        config["width"] = Width;
        config["subtitles"] = Subtitles;
        config["webcam_options"] = WebcamOptions;
        config["include_audio"] = IncludeAudio;
        config["streaming"] = Streaming;
        config["watermark"] = Watermark;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null) return null;

        if (payload is FileData fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData.Path))
            {
                throw new ArgumentException("Payload path missing");
            }

            var filePath = fileData.Path;
            if (!string.IsNullOrWhiteSpace(Format))
            {
                var ext = Path.GetExtension(filePath)?.TrimStart('.').ToLowerInvariant();
                if (ext != Format!.ToLowerInvariant() && System.IO.File.Exists(filePath))
                {
                    filePath = ProcessingUtils.ConvertVideoToPlayableMp4(filePath);
                }
            }

            return filePath;
        }

        return payload;
    }

    public override object? Postprocess(object? value)
    {
        if (Streaming)
        {
            return value;
        }

        if (value == null || (value is object[] arr && arr.Length == 2 && arr[0] == null && arr[1] == null))
            return null;

        if (value is string filePath)
        {
            return FormatVideo(filePath);
        }

        return value;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["description"] = "video file data"
        };
    }

    public override object ExamplePayload()
    {
        var sample = "https://github.com/gradio-app/gradio/raw/main/gradio/media_assets/videos/world.mp4";
        return new Dictionary<string, object> { ["path"] = sample, ["url"] = sample };
    }

    public override object ExampleValue()
    {
        return "https://github.com/gradio-app/gradio/raw/main/gradio/media_assets/videos/world.mp4";
    }

    public Task<Dictionary<string, object>> StreamOutput(object value, string outputId, bool firstChunk)
    {
        var output = new Dictionary<string, object>
        {
            ["path"] = outputId,
            ["is_stream"] = true,
            ["orig_name"] = "video-stream.mp4",
            ["meta"] = new Dictionary<string, object> { ["_type"] = "gradio.FileData" }
        };

        if (value is string path)
        {
            if (!path.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) &&
                !path.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Video must be in .mp4 or .ts format to be streamed as chunks");
            }

            var bytes = File.ReadAllBytes(path);
            output["chunk"] = bytes;
            output["extension"] = path.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) ? ".ts" : ".mp4";
            output["duration"] = ProcessingUtils.GetVideoLength(path);
        }

        return Task.FromResult(output);
    }

    public Task<object> CombineStream(List<byte[]> stream, string? desiredOutputFormat = null, bool onlyFile = false)
    {
        var ext = string.IsNullOrWhiteSpace(desiredOutputFormat) ? "mp4" : desiredOutputFormat.TrimStart('.').ToLowerInvariant();
        var joined = stream == null || stream.Count == 0
            ? Array.Empty<byte>()
            : stream.SelectMany(s => s ?? Array.Empty<byte>()).ToArray();

        var outPath = ProcessingUtils.SaveBytesToCache(joined, $"video-stream.{ext}", GRADIO_CACHE);
        var file = new FileData
        {
            Path = outPath,
            IsStream = false,
            OrigName = $"video-stream.{ext}"
        };

        return Task.FromResult<object>(file);
    }

    private FileData? FormatVideo(string? video)
    {
        if (string.IsNullOrWhiteSpace(video))
        {
            return null;
        }

        var isUrl = FileData.IsHttpUrl(video);
        var conversionNeeded = !string.IsNullOrWhiteSpace(Format) &&
                               Path.GetExtension(video).TrimStart('.').ToLowerInvariant() != Format!.ToLowerInvariant();

        if (!isUrl && !conversionNeeded)
        {
            return new FileData { Path = video, OrigName = Path.GetFileName(video) };
        }

        if (isUrl)
        {
            video = ProcessingUtils.SaveUrlToCache(video, GRADIO_CACHE);
        }

        if (ProcessingUtils.FfmpegInstalled() && !ProcessingUtils.VideoIsPlayable(video))
        {
            video = ProcessingUtils.ConvertVideoToPlayableMp4(video);
        }

        if (conversionNeeded && System.IO.File.Exists(video))
        {
            video = ProcessingUtils.ConvertVideoToPlayableMp4(video);
        }

        return new FileData { Path = video, OrigName = Path.GetFileName(video) };
    }

    private static object? ProcessSubtitles(object? subtitles)
    {
        if (subtitles == null)
        {
            return null;
        }

        if (subtitles is string subtitlePath)
        {
            return FormatSubtitles(subtitlePath);
        }

        if (subtitles is IEnumerable<object> list)
        {
            return ProcessJsonSubtitles(list);
        }

        return subtitles;
    }

    private static FileData ProcessJsonSubtitles(IEnumerable<object> subtitles)
    {
        string SecondsToVtt(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }

        var upload = global::Gradio.Net.Utils.Utils.GetUploadFolder();
        Directory.CreateDirectory(upload);
        var outPath = Path.Combine(upload, $"subs_{Guid.NewGuid():N}.vtt");
        var sb = new StringBuilder();
        sb.AppendLine("WEBVTT");
        sb.AppendLine();

        int idx = 0;
        foreach (var item in subtitles)
        {
            if (item is not IDictionary<string, object> dict)
                throw new ArgumentException($"Subtitle at index {idx} must be a dictionary");
            if (!dict.TryGetValue("text", out var textObj))
                throw new ArgumentException($"Subtitle at index {idx} missing required 'text' field");
            if (!dict.TryGetValue("timestamp", out var tsObj) || tsObj is not IEnumerable<object> tsEnum)
                throw new ArgumentException($"Subtitle at index {idx} missing required 'timestamp' field");

            var tsList = tsEnum.ToList();
            if (tsList.Count != 2)
                throw new ArgumentException($"Subtitle at index {idx} 'timestamp' must be a list/tuple of [start, end]");

            var start = Convert.ToDouble(tsList[0]);
            var end = Convert.ToDouble(tsList[1]);
            var text = textObj?.ToString() ?? string.Empty;

            sb.AppendLine($"{SecondsToVtt(start)} --> {SecondsToVtt(end)}");
            sb.AppendLine(text);
            sb.AppendLine();
            idx++;
        }

        File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);
        return new FileData { Path = outPath, OrigName = Path.GetFileName(outPath) };
    }

    private static FileData FormatSubtitles(string subtitle)
    {
        var ext = Path.GetExtension(subtitle).ToLowerInvariant();
        var valid = new[] { ".srt", ".vtt", ".json" };
        if (!valid.Contains(ext))
        {
            throw new ArgumentException($"Invalid value for parameter `subtitle`: {subtitle}. Please choose a file with one of these extensions: (.srt, .vtt, .json)");
        }

        if (ext == ".vtt")
        {
            return new FileData { Path = subtitle, OrigName = Path.GetFileName(subtitle) };
        }

        if (ext == ".json")
        {
            return new FileData { Path = subtitle, OrigName = Path.GetFileName(subtitle) };
        }

        var upload = global::Gradio.Net.Utils.Utils.GetUploadFolder();
        Directory.CreateDirectory(upload);
        var outPath = Path.Combine(upload, $"subs_{Guid.NewGuid():N}.vtt");

        var srtContent = File.ReadAllText(subtitle);
        var lines = srtContent.Replace("\r", string.Empty).Split('\n');
        var sb = new StringBuilder();
        sb.AppendLine("WEBVTT");
        sb.AppendLine();
        foreach (var line in lines)
        {
            if (line.Contains(" --> "))
            {
                sb.AppendLine(line.Replace(',', '.'));
            }
            else if (!int.TryParse(line.Trim(), out _))
            {
                sb.AppendLine(line);
            }
        }

        File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);
        return new FileData { Path = outPath, OrigName = Path.GetFileName(outPath) };
    }
}

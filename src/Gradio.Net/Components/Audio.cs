using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Events;

namespace Gradio.Net.Components
{
    [Event("stream")]
    [Event("change")]
    [Event("clear")]
    [Event("play")]
    [Event("pause")]
    [Event("stop")]
    [Event("start_recording")]
    [Event("pause_recording")]
    [Event("stop_recording")]
    [Event("upload")]
    [Event("input")]
    public class Audio : Component, IStreamingInput, IStreamingOutput
    {
        public List<string> Sources { get; set; }
        public string Type { get; set; }
        public bool Streaming { get; set; }
        public bool Autoplay { get; set; }
        public bool Editable { get; set; }
        public List<string> Buttons { get; set; }
        public WaveformOptions WaveformOptions { get; set; }
        public bool Loop { get; set; }
        public bool Recording { get; set; }
        public bool Streamable { get; set; }
        public double PlaybackPosition { get; set; }
        public object Subtitles { get; set; }
        public string Format { get; set; }

        public Audio(
            object value = null,
            object sources = null,
            string type = "numpy",
            string label = null,
            object every = null,
            object inputs = null,
            bool? showLabel = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            bool? interactive = null,
            object visible = null,
            bool streaming = false,
            string elemId = null,
            object elemClasses = null,
            bool render = true,
            object key = null,
            object preservedByKey = null,
            string format = null,
            bool autoplay = false,
            bool editable = true,
            List<string> buttons = null,
            WaveformOptions waveformOptions = null,
            bool loop = false,
            bool recording = false,
            bool streamable = false,
            double playbackPosition = 0,
            object subtitles = null)
        {
            Value = value;
            Label = label;
            ShowLabel = showLabel ?? true;
            Container = container;
            Scale = scale;
            MinWidth = minWidth;
            Interactive = interactive;

            var validSources = new List<string> { "upload", "microphone" };
            if (sources == null)
            {
                Sources = streaming ? new List<string> { "microphone" } : validSources;
            }
            else if (sources is string sourceStr && validSources.Contains(sourceStr))
            {
                Sources = new List<string> { sourceStr };
            }
            else if (sources is List<string> sourceList)
            {
                Sources = sourceList;
            }
            else if (sources is IEnumerable<object> sourceObjList)
            {
                Sources = sourceObjList.Select(s => s?.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }
            else
            {
                throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
            }

            foreach (var source in Sources)
            {
                if (!validSources.Contains(source))
                {
                    throw new ArgumentException($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
                }
            }

            var validTypes = new List<string> { "numpy", "filepath" };
            if (!validTypes.Contains(type))
            {
                throw new ArgumentException($"Invalid value for parameter `type`: {type}. Please choose from one of: {string.Join(" ", validTypes)}");
            }
            Type = type;

            Streaming = streaming;
            if (Streaming && !Sources.Contains("microphone"))
            {
                throw new ArgumentException("Audio streaming only available if sources includes 'microphone'.");
            }

            if (format != null)
            {
                var fmt = format.ToLowerInvariant();
                if (fmt != "wav" && fmt != "mp3")
                {
                    throw new ArgumentException("Invalid value for parameter `format`. Please choose one of: wav mp3");
                }
                Format = fmt;
            }

            Autoplay = autoplay;
            Editable = editable;
            Buttons = buttons ?? new List<string> { "download", "share" };
            WaveformOptions = waveformOptions ?? new WaveformOptions();
            Loop = loop;
            Recording = recording;
            Streamable = streamable;
            PlaybackPosition = playbackPosition;
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

        public override string GetBlockName() => "audio";

        public override Dictionary<string, object> GetConfig(Type cls = null)
        {
            var config = base.GetConfig(cls);
            config["sources"] = Sources;
            config["type"] = Type;
            config["streaming"] = Streaming;
            config["format"] = Format;
            config["autoplay"] = Autoplay;
            config["editable"] = Editable;
            config["buttons"] = Buttons;
            config["waveform_options"] = new Dictionary<string, object?>
            {
                ["waveform_color"] = WaveformOptions?.WaveformColor,
                ["waveform_progress_color"] = WaveformOptions?.WaveformProgressColor,
                ["trim_region_color"] = WaveformOptions?.TrimRegionColor,
                ["show_recording_waveform"] = WaveformOptions?.ShowRecordingWaveform ?? true,
                ["skip_length"] = WaveformOptions?.SkipLength ?? 5,
                ["sample_rate"] = WaveformOptions?.SampleRate ?? 44100,
            };
            config["loop"] = Loop;
            config["recording"] = Recording;
            config["streamable"] = Streamable;
            config["playback_position"] = PlaybackPosition;
            config["subtitles"] = Subtitles;
            return config;
        }

        public override object Preprocess(object payload)
        {
            if (payload == null)
                return null;

            if (payload is FileData fileData)
            {
                if (string.IsNullOrWhiteSpace(fileData.Path))
                    throw new ArgumentException("payload path missing");

                bool needsConversion = false;
                var originalSuffix = Path.GetExtension(fileData.Path)?.ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(Format) && originalSuffix != $".{Format}")
                    needsConversion = true;

                if (Type == "numpy")
                    return ProcessingUtils.AudioFromFile(fileData.Path);

                if (Type == "filepath")
                {
                    if (!needsConversion)
                        return fileData.Path;

                    var tuple = ProcessingUtils.AudioFromFile(fileData.Path);
                    var outPath = Path.ChangeExtension(fileData.Path, $".{Format}");
                    ProcessingUtils.AudioToFile(tuple.Item1, tuple.Item2, outPath, Format ?? "wav");
                    return outPath;
                }
            }

            if (payload is string s)
                return Type == "numpy" ? ProcessingUtils.AudioFromFile(s) : s;

            return payload;
        }

        public override object Postprocess(object value)
        {
            if (value == null)
                return null;

            if (value is byte[] byteValue)
            {
                if (Streaming)
                    return byteValue;

                var cacheFilePath = ProcessingUtils.SaveBytesToCache(byteValue, "audio", GRADIO_CACHE);
                return new FileData { Path = cacheFilePath, OrigName = Path.GetFileName(cacheFilePath) };
            }

            if (value is Tuple<int, float[]> audioTuple)
            {
                var format = Format ?? "wav";
                var outPath = ProcessingUtils.SaveAudioToCache(audioTuple.Item2, audioTuple.Item1, format, GRADIO_CACHE);
                return new FileData { Path = outPath, OrigName = Path.GetFileName(outPath) };
            }

            if (value is Tuple<int, Array> audioArrayTuple && audioArrayTuple.Item2 is float[] farr)
            {
                var format = Format ?? "wav";
                var outPath = ProcessingUtils.SaveAudioToCache(farr, audioArrayTuple.Item1, format, GRADIO_CACHE);
                return new FileData { Path = outPath, OrigName = Path.GetFileName(outPath) };
            }

            if (value is string filePath)
            {
                var outputPath = filePath;
                var originalSuffix = Path.GetExtension(filePath)?.ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(Format) && originalSuffix != $".{Format}" && File.Exists(filePath))
                {
                    var tuple = ProcessingUtils.AudioFromFile(filePath);
                    outputPath = ProcessingUtils.SaveAudioToCache(tuple.Item2, tuple.Item1, Format, GRADIO_CACHE);
                }

                return new FileData
                {
                    Path = outputPath,
                    OrigName = File.Exists(outputPath) ? Path.GetFileName(outputPath) : null
                };
            }

            throw new ArgumentException($"Cannot process {value} as Audio");
        }

        private static object ProcessSubtitles(object subtitles)
        {
            if (subtitles == null)
                return null;

            if (subtitles is string || subtitles is FileData)
                return subtitles;

            if (subtitles is IEnumerable<object> list)
            {
                var normalized = new List<Dictionary<string, object>>();
                int idx = 0;
                foreach (var item in list)
                {
                    if (item is not IDictionary<string, object> dict)
                        throw new ArgumentException($"Subtitle at index {idx} must be a dictionary");

                    if (!dict.ContainsKey("text"))
                        throw new ArgumentException($"Subtitle at index {idx} missing required 'text' field");

                    if (!dict.ContainsKey("timestamp") || dict["timestamp"] is not IEnumerable<object> ts)
                        throw new ArgumentException($"Subtitle at index {idx} missing required 'timestamp' field");

                    var tsList = ts.ToList();
                    if (tsList.Count != 2)
                        throw new ArgumentException($"Subtitle at index {idx} 'timestamp' must be a list/tuple of [start, end]");

                    normalized.Add(new Dictionary<string, object>
                    {
                        ["start"] = tsList[0],
                        ["end"] = tsList[1],
                        ["text"] = dict["text"]
                    });
                    idx++;
                }

                return normalized;
            }

            return subtitles;
        }

        public override object ProcessExample(object value)
        {
            if (value == null) return string.Empty;
            if (value is string path) return Path.GetFileName(path);
            return "(audio)";
        }

        public override object ExamplePayload()
        {
            return new FileData
            {
                Path = "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav",
                Url = "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav"
            };
        }

        public override object ExampleValue()
        {
            return "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav";
        }

        public void CheckStreamable()
        {
            if (Streaming && (Sources == null || !Sources.Contains("microphone")))
                throw new ArgumentException("Audio streaming only available if source includes 'microphone'.");
        }

        public Task<Dictionary<string, object>> StreamOutput(object value, string outputId, bool firstChunk)
        {
            var output = new Dictionary<string, object>
            {
                ["path"] = outputId,
                ["is_stream"] = true,
                ["orig_name"] = "audio-stream.mp3",
                ["meta"] = new Dictionary<string, object> { ["_type"] = "gradio.FileData" }
            };

            if (value is byte[] bytes)
            {
                output["chunk"] = bytes;
                output["extension"] = ".mp3";
            }
            else if (value is FileData fileData)
            {
                output["path"] = fileData.Path;
                output["orig_name"] = fileData.OrigName ?? "audio-stream.mp3";
            }

            return Task.FromResult(output);
        }

        public Task<object> CombineStream(List<byte[]> stream, string desiredOutputFormat = null, bool onlyFile = false)
        {
            var joined = stream == null || stream.Count == 0
                ? Array.Empty<byte>()
                : stream.SelectMany(s => s ?? Array.Empty<byte>()).ToArray();

            var ext = string.IsNullOrWhiteSpace(desiredOutputFormat) ? "mp3" : desiredOutputFormat;
            var outPath = ProcessingUtils.SaveBytesToCache(joined, $"audio-stream.{ext}", GRADIO_CACHE);

            var file = new FileData
            {
                Path = outPath,
                IsStream = false,
                OrigName = $"audio-stream.{ext}"
            };

            return Task.FromResult<object>(file);
        }
    }
}

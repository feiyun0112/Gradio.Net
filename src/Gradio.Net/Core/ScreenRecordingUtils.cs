using System.Diagnostics;
using System.Text.Json;

namespace Gradio.Net.Core
{
    public class ScreenRecordingUtils
    {
        public static readonly string DEFAULT_TEMP_DIR = Environment.GetEnvironmentVariable("GRADIO_TEMP_DIR") ??
            Path.Combine(Path.GetTempPath(), "gradio");

        static ScreenRecordingUtils()
        {
            // Ensure temp directory exists
            Directory.CreateDirectory(DEFAULT_TEMP_DIR);
        }

        public static async Task<(string OutputPath, List<string> TempFiles)> ProcessVideoWithFfmpeg(
            string inputPath,
            string outputPath,
            Dictionary<string, object> @params)
        {
            var tempFiles = new List<string> { inputPath };
            string currentInput = inputPath;

            try
            {
                // Create output directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

                // Process video trimming if specified
                if (@params.TryGetValue("remove_segment_start", out var startObj) &&
                    @params.TryGetValue("remove_segment_end", out var endObj))
                {
                    if (double.TryParse(startObj.ToString(), out double start) &&
                        double.TryParse(endObj.ToString(), out double end) &&
                        start < end)
                    {
                        var segmentResult = await RemoveVideoSegment(currentInput, start, end);
                        currentInput = segmentResult.OutputPath;
                        tempFiles.AddRange(segmentResult.TempFiles);
                    }
                }

                // Process zoom effects if specified
                if (@params.TryGetValue("zoom_effects", out var zoomEffectsObj))
                {
                    if (zoomEffectsObj is JsonElement zoomEffectsElement && zoomEffectsElement.ValueKind == JsonValueKind.Array)
                    {
                        var zoomEffects = zoomEffectsElement.EnumerateArray().ToList();
                        for (int i = 0; i < zoomEffects.Count; i++)
                        {
                            var effect = zoomEffects[i];
                            if (effect.TryGetProperty("boundingBox", out var boundingBox) &&
                                boundingBox.TryGetProperty("topLeft", out var topLeft) &&
                                boundingBox.TryGetProperty("bottomRight", out var bottomRight))
                            {
                                var zoomResult = await ZoomIn(
                                    currentInput,
                                    topLeft,
                                    bottomRight,
                                    effect.TryGetProperty("duration", out var duration) ? double.Parse(duration.ToString()) : 2.0,
                                    effect.TryGetProperty("start_frame", out var startFrame) ? startFrame.ToString() : null);

                                currentInput = zoomResult.OutputPath;
                                tempFiles.AddRange(zoomResult.TempFiles);
                            }
                        }
                    }
                }

                // Final processing
                await RunFfmpegCommand(
                    $"-i {currentInput} -c:v libx264 -preset fast -crf 22 -c:a aac -r 30 -vsync cfr -y {outputPath}");

                // Trim the final output slightly
                string finalTrimmedOutput = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(outputPath)}_final_trimmed.mp4");
                tempFiles.Add(finalTrimmedOutput);

                await RunFfmpegCommand(
                    $"-i {outputPath} -ss 0.5 -c:v libx264 -preset fast -crf 22 -c:a aac -r 30 -y {finalTrimmedOutput}");

                // Replace the original output with the trimmed version if successful
                if (File.Exists(finalTrimmedOutput) && new FileInfo(finalTrimmedOutput).Length > 0)
                {
                    File.Copy(finalTrimmedOutput, outputPath, true);
                }

                return (outputPath, tempFiles);
            }
            catch (Exception ex)
            {
                return (inputPath, tempFiles);
            }
        }

        private static async Task<(string OutputPath, List<string> TempFiles)> RemoveVideoSegment(string inputPath, double start, double end)
        {
            var tempFiles = new List<string>();
            string segmentOutput = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(inputPath)}_trimmed.mp4");
            tempFiles.Add(segmentOutput);

            // Create before segment if start > 0
            string beforeSegment = string.Empty;
            if (start > 0)
            {
                beforeSegment = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(inputPath)}_before.mp4");
                tempFiles.Add(beforeSegment);
                await RunFfmpegCommand(
                    $"-i {inputPath} -t {start} -c:v libx264 -preset fast -crf 22 -c:a aac -r 30 -y {beforeSegment}");
            }

            // Create after segment
            string afterSegment = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(inputPath)}_after.mp4");
            tempFiles.Add(afterSegment);
            await RunFfmpegCommand(
                $"-i {inputPath} -ss {end} -c:v libx264 -preset fast -crf 22 -c:a aac -r 30 -y {afterSegment}");

            // Create concat file
            string concatFile = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(inputPath)}_concat.txt");
            tempFiles.Add(concatFile);

            using (var writer = new StreamWriter(concatFile))
            {
                if (!string.IsNullOrEmpty(beforeSegment) && File.Exists(beforeSegment) && new FileInfo(beforeSegment).Length > 0)
                {
                    writer.WriteLine($"file '{beforeSegment}'");
                }
                if (File.Exists(afterSegment) && new FileInfo(afterSegment).Length > 0)
                {
                    writer.WriteLine($"file '{afterSegment}'");
                }
            }

            // Concatenate segments
            if (File.Exists(concatFile) && new FileInfo(concatFile).Length > 0)
            {
                await RunFfmpegCommand(
                    $"-f concat -safe 0 -i {concatFile} -c copy -y {segmentOutput}");
            }

            return (segmentOutput, tempFiles);
        }

        public static async Task<(string OutputPath, List<string> TempFiles)> ZoomIn(
            string inputPath,
            object topLeftObj,
            object bottomRightObj,
            double zoomDuration = 2.0,
            string? zoomStartFrame = null)
        {
            var tempFiles = new List<string>();

            try
            {
                if (!File.Exists(inputPath))
                {
                    return (inputPath, tempFiles);
                }

                // Parse zoom start frame
                double startFrame = 60;
                if (!string.IsNullOrEmpty(zoomStartFrame))
                {
                    double.TryParse(zoomStartFrame, out startFrame);
                }

                // Parse bounding box coordinates
                (double x1, double y1) = ParsePoint(topLeftObj) ?? (0.25, 0.25);
                (double x2, double y2) = ParsePoint(bottomRightObj) ?? (0.75, 0.75);

                // Validate and clamp coordinates
                x1 = Math.Max(0.0, Math.Min(0.9, x1));
                y1 = Math.Max(0.0, Math.Min(0.9, y1));
                x2 = Math.Max(0.1, Math.Min(1.0, x2));
                y2 = Math.Max(0.1, Math.Min(1.0, y2));

                if (x2 <= x1) x1 = 0.25; x2 = 0.75;
                if (y2 <= y1) y1 = 0.25; y2 = 0.75;

                double boxWidth = x2 - x1;
                double boxHeight = y2 - y1;

                double boxCenterX = (x1 + x2) / 2;
                double boxCenterY = (y1 + y2) / 2;

                // Calculate zoom center
                double zoomCenterX = CalculateProportionalOffset(boxCenterX, boxWidth);
                double zoomCenterY = CalculateProportionalOffset(boxCenterY, boxHeight);

                // Calculate zoom parameters
                double targetZoom = 3.0;
                double maxZoomBySize = Math.Min(1.0 / boxWidth, 1.0 / boxHeight);
                double safetyMargin = 0.9;
                double dynamicMaxZoom = Math.Min(maxZoomBySize * safetyMargin, targetZoom);
                dynamicMaxZoom = Math.Max(dynamicMaxZoom, 1.3);

                // Get video duration
                double videoDuration = await GetVideoDuration(inputPath);
                zoomDuration = Math.Min(zoomDuration, videoDuration);

                // Create output file
                string zoomOutput = Path.Combine(DEFAULT_TEMP_DIR, $"{Path.GetFileNameWithoutExtension(inputPath)}_zoomed.mp4");
                tempFiles.Add(zoomOutput);

                double fps = 30.0;
                int zoomInFrames = (int)(fps / 2);
                int zoomOutFrames = (int)(fps / 2);
                int holdFrames = (int)(zoomDuration * fps);

                int width = 1920;
                int height = 1080;

                // Create complex filter for zoom effect
                string complexFilter = $"[0:v]zoompan=" +
                    $"z='if(between(on,{startFrame},{startFrame + zoomInFrames + holdFrames + zoomOutFrames})," +
                    $"if(lt(on-{startFrame},{zoomInFrames})," +
                    $"1+(({dynamicMaxZoom}-1)*(on-{startFrame})/{zoomInFrames})," +
                    $"if(lt(on-{startFrame},{zoomInFrames + holdFrames})," +
                    $"{dynamicMaxZoom}," +
                    $"{dynamicMaxZoom}-(({dynamicMaxZoom}-1)*((on-{startFrame}-{zoomInFrames}-{holdFrames}))/{zoomOutFrames})" +
                    $"),1)':" +
                    $"x='iw*{zoomCenterX}-iw/zoom*{zoomCenterX}':" +
                    $"y='ih*{zoomCenterY}-ih/zoom*{zoomCenterY}':" +
                    $"d=1:" +
                    $"fps={fps}:" +
                    $"s={width}x{height}[outv]";

                // Run FFmpeg command
                await RunFfmpegCommand(
                    $"-i {inputPath} -filter_complex \"{complexFilter}\" -map \"[outv]\" -map 0:a? " +
                    $"-c:v libx264 -pix_fmt yuv420p -movflags +faststart -preset fast -r 30 -c:a aac -y {zoomOutput}");

                return (zoomOutput, tempFiles);
            }
            catch (Exception ex)
            {
                return (inputPath, tempFiles);
            }
        }

        private static (double x, double y)? ParsePoint(object pointObj)
        {
            try
            {
                if (pointObj is JsonElement pointElement)
                {
                    if (pointElement.TryGetProperty("0", out var xElement) &&
                        pointElement.TryGetProperty("1", out var yElement))
                    {
                        double x = xElement.GetDouble();
                        double y = yElement.GetDouble();
                        return (x, y);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static double CalculateProportionalOffset(double center, double size)
        {
            if (center < 0.5)
            {
                double distanceFromCenter = 0.5 - center;
                return center - (size * (distanceFromCenter / 0.5));
            }
            else if (center > 0.5)
            {
                double distanceFromCenter = center - 0.5;
                return center + (size * (distanceFromCenter / 0.5));
            }
            return center;
        }

        private static async Task<double> GetVideoDuration(string inputPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffprobe",
                        Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (double.TryParse(output.Trim(), out double duration))
                {
                    return duration;
                }
            }
            catch
            {
                // Ignore errors, return default duration
            }

            return 10.0;
        }

        private static async Task RunFfmpegCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.StandardOutput.ReadToEndAsync();
            await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
        }
    }
}

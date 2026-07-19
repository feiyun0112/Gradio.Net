using System.Collections;
using System.Text.Json;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Data;
using Gradio.Net.Utils;

namespace Gradio.Net.Core.Flagging
{
    public class CSVLogger : FlaggingCallback
    {
        public bool SimplifyFileData { get; set; } = true;

        public bool Verbose { get; set; } = true;

        public string? DatasetFileName { get; set; }

        protected IEnumerable<Components.Component> Components { get; set; } = Enumerable.Empty<Components.Component>();

        protected string FlaggingDir { get; set; } = string.Empty;

        protected bool FirstTime { get; set; } = true;

        protected readonly object Lock = new object();

        protected string DatasetFilePath { get; set; } = string.Empty;

        public CSVLogger(bool simplifyFileData = true, bool verbose = true, string? datasetFileName = null)
        {
            SimplifyFileData = simplifyFileData;
            Verbose = verbose;
            DatasetFileName = datasetFileName;
        }

        public override void Setup(IEnumerable<Components.Component> components, string flaggingDir)
        {
            Components = components;
            FlaggingDir = flaggingDir;
            FirstTime = true;
            if (!string.IsNullOrWhiteSpace(FlaggingDir))
            {
                Directory.CreateDirectory(FlaggingDir);
            }
        }

        protected void CreateDatasetFile(List<string>? additionalHeaders = null)
        {
            Directory.CreateDirectory(FlaggingDir);
            additionalHeaders ??= new List<string>();

            var componentList = Components.ToList();
            var headers = componentList.Select((component, idx) => component.Label ?? $"component {idx}").ToList();
            headers.AddRange(additionalHeaders);
            headers.Add("timestamp");

            var sanitizedHeaders = Gradio.Net.Utils.Utils.SanitizeListForCsv(headers.Cast<object>().ToList()).Select(h => h.ToString()).ToList();

            if (!string.IsNullOrEmpty(DatasetFileName))
            {
                DatasetFilePath = Path.Combine(FlaggingDir, DatasetFileName);
            }
            else
            {
                var datasetFiles = Directory.GetFiles(FlaggingDir, "dataset*.csv").ToList();
                if (datasetFiles.Any())
                {
                    try
                    {
                        var latestFile = datasetFiles.OrderByDescending(f =>
                        {
                            var match = Regex.Match(Path.GetFileNameWithoutExtension(f), @"dataset(\d+)");
                            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
                        }).First();

                        var latestNum = int.Parse(Regex.Match(Path.GetFileNameWithoutExtension(latestFile), @"dataset(\d+)", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Groups[1].Value);

                        using (var reader = new StreamReader(latestFile))
                        {
                            var existingHeaders = reader.ReadLine()?.Split(',').Select(h => h.Trim('"')).ToList();
                            if (existingHeaders != sanitizedHeaders)
                            {
                                var newNum = latestNum + 1;
                                DatasetFilePath = Path.Combine(FlaggingDir, $"dataset{newNum}.csv");
                            }
                            else
                            {
                                DatasetFilePath = latestFile;
                            }
                        }
                    }
                    catch
                    {
                        DatasetFilePath = Path.Combine(FlaggingDir, "dataset1.csv");
                    }
                }
                else
                {
                    DatasetFilePath = Path.Combine(FlaggingDir, "dataset1.csv");
                }
            }

            if (!File.Exists(DatasetFilePath))
            {
                using (var writer = new StreamWriter(DatasetFilePath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(string.Join(",", sanitizedHeaders.Select(h => $"\"{h}\"")));
                }
                if (Verbose)
                {
                }
            }
            else if (Verbose)
            {
            }
        }

        public override int Flag(List<object> flagData, string? flagOption = null, string? username = null)
        {
            if (FirstTime)
            {
                var additionalHeaders = new List<string>();
                if (flagOption != null)
                {
                    additionalHeaders.Add("flag");
                }
                if (username != null)
                {
                    additionalHeaders.Add("username");
                }
                CreateDatasetFile(additionalHeaders);
                FirstTime = false;
            }

            var componentList = Components.ToList();
            var csvData = new List<string>();

            for (int idx = 0; idx < componentList.Count; idx++)
            {
                var component = componentList[idx];
                var sample = flagData[idx];
                var saveDir = Path.Combine(FlaggingDir, Gradio.Net.Utils.Utils.StripInvalidFilenameCharacters(component.Label ?? $"component {idx}"));
                Directory.CreateDirectory(saveDir);

                string data;
                if (Gradio.Net.Utils.Utils.IsPropUpdate(sample))
                {
                    data = sample.ToString() ?? string.Empty;
                }
                else
                {
                    if (TryCopyFilePayload(sample, saveDir, out var copiedPayload))
                    {
                        data = copiedPayload is string copiedStr
                            ? copiedStr
                            : JsonSerializer.Serialize(copiedPayload);
                    }
                    else
                    {
                        data = sample != null ? component.Flag(sample, saveDir).ToString() : string.Empty;
                    }

                    if (SimplifyFileData)
                    {
                        data = Gradio.Net.Utils.Utils.SimplifyFileDataInString(data);
                    }
                }
                csvData.Add(data);
            }

            if (flagOption != null)
            {
                csvData.Add(flagOption);
            }
            if (username != null)
            {
                csvData.Add(username);
            }
            csvData.Add(DateTime.Now.ToString());

            lock (Lock)
            {
                using (var writer = new StreamWriter(DatasetFilePath, true, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(string.Join(",", csvData.Select(d => $"\"{d}\"")));
                }

                using (var reader = new StreamReader(DatasetFilePath, System.Text.Encoding.UTF8))
                {
                    var lineCount = 0;
                    while (reader.ReadLine() != null)
                    {
                        lineCount++;
                    }
                    return lineCount - 1;
                }
            }
        }

        private static bool TryCopyFilePayload(object? sample, string saveDir, out object? copiedPayload)
        {
            copiedPayload = null;
            var normalized = NormalizePayload(sample);
            if (normalized == null)
            {
                return false;
            }

            if (!ContainsFileData(normalized))
            {
                return false;
            }

            copiedPayload = GradioModel.Traverse(
                normalized,
                obj =>
                {
                    if (obj is not Dictionary<string, object> fileDict)
                    {
                        return obj;
                    }

                    var sourcePath = fileDict.TryGetValue("path", out var pathObj) ? pathObj?.ToString() : null;
                    if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    {
                        return obj;
                    }

                    var uniqueDir = Path.Combine(saveDir, Guid.NewGuid().ToString("N")[..20]);
                    var copied = new FileData
                    {
                        Path = sourcePath,
                        Url = fileDict.TryGetValue("url", out var urlObj) ? urlObj?.ToString() : null,
                        Size = fileDict.TryGetValue("size", out var sizeObj) && int.TryParse(sizeObj?.ToString(), out var sizeVal) ? sizeVal : null,
                        OrigName = fileDict.TryGetValue("orig_name", out var origObj) ? origObj?.ToString() : null,
                        MimeType = fileDict.TryGetValue("mime_type", out var mimeObj) ? mimeObj?.ToString() : null,
                        IsStream = fileDict.TryGetValue("is_stream", out var streamObj) && bool.TryParse(streamObj?.ToString(), out var streamVal) && streamVal,
                        Meta = new FileDataMeta()
                    }.CopyToDir(uniqueDir);

                    return copied.ToDictionary();
                },
                obj => obj is Dictionary<string, object> d && FileData.IsFileData(d)
            );

            return true;
        }

        private static bool ContainsFileData(object value)
        {
            if (value is Dictionary<string, object> dict)
            {
                if (FileData.IsFileData(dict))
                {
                    return true;
                }

                foreach (var kv in dict.Values)
                {
                    if (kv != null && ContainsFileData(kv))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (value is List<object> list)
            {
                return list.Any(item => item != null && ContainsFileData(item));
            }

            return false;
        }

        private static object? NormalizePayload(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string str)
            {
                var trimmed = str.TrimStart();
                if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(str);
                        return NormalizePayload(doc.RootElement);
                    }
                    catch
                    {
                        return str;
                    }
                }
                return str;
            }

            if (value is JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        {
                            var dict = new Dictionary<string, object>();
                            foreach (var prop in element.EnumerateObject())
                            {
                                dict[prop.Name] = NormalizePayload(prop.Value) ?? string.Empty;
                            }
                            return dict;
                        }
                    case JsonValueKind.Array:
                        {
                            var list = new List<object>();
                            foreach (var item in element.EnumerateArray())
                            {
                                list.Add(NormalizePayload(item) ?? string.Empty);
                            }
                            return list;
                        }
                    case JsonValueKind.String:
                        return element.GetString() ?? string.Empty;
                    case JsonValueKind.Number:
                        if (element.TryGetInt64(out var i64)) return i64;
                        if (element.TryGetDouble(out var d)) return d;
                        return element.ToString();
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return element.GetBoolean();
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return null;
                    default:
                        return element.ToString();
                }
            }

            if (value is IDictionary dictObj)
            {
                var dict = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in dictObj)
                {
                    var key = entry.Key?.ToString();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }
                    dict[key] = NormalizePayload(entry.Value) ?? string.Empty;
                }
                return dict;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var list = new List<object>();
                foreach (var item in enumerable)
                {
                    list.Add(NormalizePayload(item) ?? string.Empty);
                }
                return list;
            }

            return value;
        }
    }
}

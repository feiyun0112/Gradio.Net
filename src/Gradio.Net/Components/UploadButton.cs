using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Data;
using Gradio.Net.Utils;

namespace Gradio.Net.Components;

[Events.Event("click")]
[Events.Event("upload")]
public class UploadButton : Component
{
    public string Variant { get; set; }
    public string Size { get; set; }
    public string? Icon { get; set; }
    public string Type { get; set; }
    public string FileCount { get; set; }
    public List<string>? FileTypes { get; set; }

    public UploadButton(
        string label = "Upload a File",
        object? value = null,
        object? every = null,
        object? inputs = null,
        string variant = "secondary",
        object? visible = null,
        string size = "lg",
        string? icon = null,
        int? scale = null,
        int? minWidth = null,
        bool interactive = true,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        string type = "filepath",
        string fileCount = "single",
        List<string>? fileTypes = null)
    {
        var validTypes = new[] { "filepath", "binary" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        if (fileTypes != null && fileTypes is not List<string>)
        {
            throw new Error($"Parameter file_types must be a list. Received {fileTypes.GetType().Name}");
        }

        Type = type;
        FileCount = fileCount;
        FileTypes = fileTypes;
        Variant = variant;
        Size = size;
        Icon = icon;

        Label = label;
        Value = value;
        Interactive = interactive;
        Scale = scale;
        MinWidth = minWidth ?? MinWidth;
        if (visible is bool vb) Visible = vb;
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

    public override string GetBlockName() => "uploadbutton";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["variant"] = Variant;
        config["size"] = Size;
        config["icon"] = Icon;
        config["type"] = Type;
        config["file_count"] = FileCount;
        config["file_types"] = FileTypes;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        if (FileCount == "single")
        {
            return new Dictionary<string, object>
            {
                ["type"] = "object",
                ["description"] = "single file data"
            };
        }
        return new Dictionary<string, object>
        {
            ["type"] = "array",
            ["description"] = "list of file data"
        };
    }

    public override object ExamplePayload()
    {
        var sample = "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";
        if (FileCount == "single")
        {
            return new Dictionary<string, object> { ["path"] = sample, ["url"] = sample };
        }

        return new List<Dictionary<string, object>>
        {
            new() { ["path"] = sample, ["url"] = sample }
        };
    }

    public override object ExampleValue()
    {
        var sample = "https://github.com/gradio-app/gradio/raw/main/test/test_files/sample_file.pdf";
        if (FileCount == "single")
        {
            return sample;
        }

        return new List<string> { sample };
    }

    private object ProcessSingleFile(FileData file)
    {
        var fileName = file.Path;

        if (Type == "filepath")
        {
            if (FileTypes != null && FileTypes.Count > 0 && !IsValidFileType(fileName, FileTypes))
            {
                throw new Error($"Invalid file type. Please upload a file that is one of these formats: [{string.Join(", ", FileTypes)}]");
            }

            return new NamedString(fileName);
        }

        if (Type == "binary")
        {
            return System.IO.File.ReadAllBytes(fileName);
        }

        throw new Error($"Unknown type: {Type}. Please choose from: 'filepath', 'binary'.");
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        if (FileCount == "single")
        {
            if (payload is ListFiles lf && lf.Root != null && lf.Root.Count > 0)
            {
                return ProcessSingleFile(lf.Root[0]);
            }

            var fd = ToFileData(payload);
            return fd == null ? null : ProcessSingleFile(fd);
        }

        if (payload is ListFiles files && files.Root != null)
        {
            return files.Root.Select(ProcessSingleFile).ToList();
        }

        if (payload is IEnumerable enumerable && payload is not string)
        {
            var result = new List<object>();
            foreach (var item in enumerable)
            {
                var fd = ToFileData(item);
                if (fd != null)
                {
                    result.Add(ProcessSingleFile(fd));
                }
            }
            return result;
        }

        var single = ToFileData(payload);
        return single == null ? null : new List<object> { ProcessSingleFile(single) };
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        value = DownloadFiles(value);

        if (value is IEnumerable enumerable && value is not string)
        {
            var list = new List<FileData>();
            foreach (var item in enumerable)
            {
                var path = item?.ToString() ?? string.Empty;
                list.Add(CreateFileData(path));
            }

            return new ListFiles { Root = list };
        }

        return CreateFileData(value.ToString() ?? string.Empty);
    }

    private object DownloadFiles(object value)
    {
        if (value is IEnumerable enumerable && value is not string)
        {
            var downloaded = new List<string>();
            foreach (var item in enumerable)
            {
                var s = item?.ToString() ?? string.Empty;
                if (FileData.IsHttpUrl(s))
                {
                    downloaded.Add(ProcessingUtils.SaveUrlToCache(s, GRADIO_CACHE));
                }
                else
                {
                    downloaded.Add(s);
                }
            }
            return downloaded;
        }

        var str = value?.ToString() ?? string.Empty;
        if (FileData.IsHttpUrl(str))
        {
            return ProcessingUtils.SaveUrlToCache(str, GRADIO_CACHE);
        }
        return str;
    }

    private static FileData CreateFileData(string path)
    {
        int? size = null;
        try
        {
            if (System.IO.File.Exists(path))
            {
                size = (int)new FileInfo(path).Length;
            }
        }
        catch
        {
        }

        return new FileData
        {
            Path = path,
            OrigName = Path.GetFileName(path),
            Size = size
        };
    }

    private static FileData? ToFileData(object? payload)
    {
        if (payload is null)
        {
            return null;
        }

        if (payload is FileData fd)
        {
            return fd;
        }

        if (payload is Dictionary<string, object> dict)
        {
            return new FileData
            {
                Path = dict.TryGetValue("path", out var p) ? p?.ToString() ?? string.Empty : string.Empty,
                Url = dict.TryGetValue("url", out var u) ? u?.ToString() : null,
                OrigName = dict.TryGetValue("orig_name", out var n) ? n?.ToString() : null
            };
        }

        return new FileData { Path = payload.ToString() ?? string.Empty };
    }

    private static bool IsValidFileType(string filePath, List<string> fileTypes)
    {
        var ext = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;

        foreach (var ft in fileTypes)
        {
            if (string.IsNullOrWhiteSpace(ft))
            {
                continue;
            }

            var t = ft.ToLowerInvariant();
            if (t.StartsWith(".") && t == ext)
            {
                return true;
            }

            if (t == "file")
            {
                return true;
            }

            if (t == "image" && new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".svg" }.Contains(ext)) return true;
            if (t == "audio" && new[] { ".mp3", ".wav", ".flac", ".ogg", ".m4a" }.Contains(ext)) return true;
            if (t == "video" && new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" }.Contains(ext)) return true;
            if (t == "text" && new[] { ".txt", ".md", ".csv", ".json", ".xml", ".yaml", ".yml" }.Contains(ext)) return true;
        }

        return false;
    }
}

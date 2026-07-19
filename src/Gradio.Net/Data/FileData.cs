using System.Text.Json.Serialization;

namespace Gradio.Net.Data;

public class FileData
{
    public string Path { get; set; }

    public string? Url { get; set; }

    public int? Size { get; set; }

    public string? OrigName { get; set; }

    public string? MimeType { get; set; }

    public bool IsStream { get; set; } = false;

    public FileDataMeta Meta { get; set; } = new FileDataMeta();

    public bool IsNone => Path == null && Url == null && Size == null && OrigName == null && MimeType == null;

    public static FileData FromPath(string path)
    {
        return new FileData { Path = path };
    }

    public FileData CopyToDir(string dir)
    {
        Directory.CreateDirectory(dir);
        if (string.IsNullOrEmpty(Path))
        {
            throw new ArgumentException("Source file path is not set");
        }
        string newName = System.IO.Path.Combine(dir, System.IO.Path.GetFileName(Path));
        File.Copy(Path, newName, true);
        return new FileData
        {
            Path = newName,
            Url = Url,
            Size = Size,
            OrigName = OrigName,
            MimeType = MimeType,
            IsStream = IsStream,
            Meta = Meta
        };
    }

    public static bool IsFileData(object obj)
    {
        if (obj is Dictionary<string, object> dict)
        {
            try
            {
                var fileData = new FileData
                {
                    Path = dict.TryGetValue("path", out var pathVal) ? pathVal.ToString() : null,
                    Url = dict.TryGetValue("url", out var urlVal) ? urlVal.ToString() : null,
                    Size = dict.TryGetValue("size", out var sizeVal) ? Convert.ToInt32(sizeVal) : null,
                    OrigName = dict.TryGetValue("orig_name", out var origNameVal) ? origNameVal.ToString() : null,
                    MimeType = dict.TryGetValue("mime_type", out var mimeTypeVal) ? mimeTypeVal.ToString() : null,
                    IsStream = dict.TryGetValue("is_stream", out var isStreamVal) && Convert.ToBoolean(isStreamVal),
                    Meta = dict.TryGetValue("meta", out var metaVal) && metaVal is Dictionary<string, object> metaDict
                        ? new FileDataMeta { Type = metaDict.TryGetValue("_type", out var typeVal) ? typeVal.ToString() : null }
                        : new FileDataMeta()
                };
                return !fileData.IsNone;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    public static bool IsFileObj(object obj)
    {
        if (obj is Dictionary<string, object> dict)
        {
            return dict.ContainsKey("path") || dict.ContainsKey("url");
        }
        return false;
    }

    public static bool IsFilePath(object obj)
    {
        if (obj is string str)
        {
            // Check if it looks like a file path (contains path separators or is rooted)
            return !string.IsNullOrEmpty(str) &&
                   (str.Contains(System.IO.Path.DirectorySeparatorChar) ||
                    str.Contains(System.IO.Path.AltDirectorySeparatorChar) ||
                    System.IO.Path.IsPathRooted(str)) &&
                   !IsHttpUrl(str);
        }
        return false;
    }

    public static bool IsHttpUrl(object obj)
    {
        if (obj is string str)
        {
            return str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   str.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public static bool ValueIsFile(Dictionary<string, object> apiInfo)
    {
        if (apiInfo == null)
            return false;

        // Check if the type indicates a file
        if (apiInfo.TryGetValue("type", out var typeObj))
        {
            var typeStr = typeObj?.ToString()?.ToLower();
            if (typeStr != null &&
                (typeStr.Contains("file") ||
                 typeStr.Contains("image") ||
                 typeStr.Contains("audio") ||
                 typeStr.Contains("video")))
            {
                return true;
            }
        }

        return false;
    }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();

        if (Path != null) dict["path"] = Path;
        if (Url != null) dict["url"] = Url;
        dict["size"] = Size.HasValue ? (object)Size.Value : null;
        if (OrigName != null) dict["orig_name"] = OrigName;
        dict["mime_type"] = MimeType;
        dict["is_stream"] = IsStream;

        dict["meta"] = new Dictionary<string, object>
        {
            ["_type"] = Meta?.Type ?? "gradio.FileData"
        };

        return dict;
    }
}

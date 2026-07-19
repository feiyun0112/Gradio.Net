using Gradio.Net.Core.Exceptions;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
[Events.Event("submit")]
[Events.Event("focus")]
[Events.Event("blur")]
[Events.Event("stop")]
public class MultimodalTextbox : FormComponent
{
    public List<string> Sources { get; set; }
    public List<string> FileTypes { get; set; }
    public string FileCount { get; set; }
    public int Lines { get; set; }
    public int MaxLines { get; set; }
    public object Placeholder { get; set; }
    public bool Autofocus { get; set; }
    public bool Autoscroll { get; set; }
    public object SubmitBtn { get; set; }
    public object StopBtn { get; set; }
    public int MaxPlainTextLength { get; set; }
    public InputHTMLAttributes HtmlAttributes { get; set; }
    public string TextAlign { get; set; }
    public bool Rtl { get; set; }

    public MultimodalTextbox(
        object value = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool autofocus = false,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        object sources = null,
        List<string> fileTypes = null,
        string fileCount = "single",
        int lines = 1,
        int maxLines = 20,
        object placeholder = null,
        object submitBtn = null,
        object stopBtn = null,
        bool autoscroll = true,
        string textAlign = null,
        bool rtl = false,
        int maxPlainTextLength = 1000,
        InputHTMLAttributes htmlAttributes = null,
        string info = null
    )
    {
        Value = value ?? new MultimodalData("", new List<FileData>());
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? true;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string s => new List<string> { s },
            List<string> list => list,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string s => new List<string> { s },
            List<string> list => list,
            _ => PreservedByKey
        };

        var validSources = new List<string> { "upload", "microphone" };
        if (sources == null)
        {
            Sources = new List<string> { "upload" };
        }
        else if (sources is string sourceStr && validSources.Contains(sourceStr))
        {
            Sources = new List<string> { sourceStr };
        }
        else if (sources is List<string> sourceList)
        {
            Sources = sourceList;
        }
        else
        {
            throw new Exception($"`sources` must be a list consisting of elements in {string.Join(", ", validSources)}");
        }

        foreach (var source in Sources)
        {
            if (!validSources.Contains(source))
            {
                throw new Exception($"`sources` must a list consisting of elements in {string.Join(", ", validSources)}");
            }
        }

        FileTypes = fileTypes;
        FileCount = fileCount;

        if (fileTypes != null && !(fileTypes is List<string>))
        {
            throw new Exception($"Parameter file_types must be a list. Received {fileTypes.GetType().Name}");
        }

        Lines = lines;
        MaxLines = Math.Max(lines, maxLines);
        Placeholder = placeholder;
        SubmitBtn = submitBtn ?? true;
        StopBtn = stopBtn ?? false;
        Autofocus = autofocus;
        Autoscroll = autoscroll;
        MaxPlainTextLength = maxPlainTextLength;
        HtmlAttributes = htmlAttributes;
        TextAlign = textAlign;
        Rtl = rtl;
    }

    public override object Preprocess(object payload)
    {
        if (payload == null)
        {
            return null;
        }

        // Deserialize JsonElement into MultimodalData
        if (payload is System.Text.Json.JsonElement je)
        {
            payload = ConvertJsonElementToMultimodalData(je);
        }
        else if (payload is Dictionary<string, object> rawDict)
        {
            payload = ConvertDictToMultimodalData(rawDict);
        }

        if (payload is MultimodalData multimodalData)
        {
            if (FileTypes != null)
            {
                foreach (var f in multimodalData.Files)
                {
                    var path = f?.Path;
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        continue;
                    }

                    var mimeType = f.MimeType;
                    if (string.IsNullOrWhiteSpace(mimeType))
                    {
                        mimeType = GetMimeType(path);
                    }

                    if (!IsValidFile(path, mimeType, FileTypes))
                    {
                        throw new Error(
                            $"Invalid file type: {mimeType}. Please upload a file that is one of these formats: [{string.Join(", ", FileTypes)}]"
                        );
                    }
                }
            }

            var result = new Dictionary<string, object>
                {
                    { "text", multimodalData.Text },
                    { "files", multimodalData.Files.Select(f => f.Path).ToList() }
                };

            return result;
        }

        throw new Exception($"Invalid payload type for MultimodalTextbox: {payload.GetType().Name}");
    }

    private static MultimodalData ConvertJsonElementToMultimodalData(System.Text.Json.JsonElement je)
    {
        string text = string.Empty;
        var files = new List<FileData>();

        if (je.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            if (je.TryGetProperty("text", out var textProp))
                text = textProp.GetString() ?? string.Empty;

            if (je.TryGetProperty("files", out var filesProp) &&
                filesProp.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var fileEl in filesProp.EnumerateArray())
                {
                    files.Add(ConvertJsonElementToFileData(fileEl));
                }
            }
        }
        else if (je.ValueKind == System.Text.Json.JsonValueKind.String)
        {
            text = je.GetString() ?? string.Empty;
        }

        return new MultimodalData(text, files);
    }

    private static FileData ConvertJsonElementToFileData(System.Text.Json.JsonElement el)
    {
        if (el.ValueKind == System.Text.Json.JsonValueKind.String)
        {
            var p = el.GetString() ?? string.Empty;
            return new FileData { Path = p, OrigName = Path.GetFileName(p), MimeType = GetMimeType(p) };
        }

        var fd = new FileData();
        if (el.TryGetProperty("path", out var pathProp)) fd.Path = pathProp.GetString();
        if (el.TryGetProperty("url", out var urlProp)) fd.Url = urlProp.GetString();
        if (el.TryGetProperty("orig_name", out var origProp)) fd.OrigName = origProp.GetString();
        if (el.TryGetProperty("mime_type", out var mimeProp)) fd.MimeType = mimeProp.GetString();
        if (el.TryGetProperty("size", out var sizeProp) && sizeProp.TryGetInt32(out var sz)) fd.Size = sz;
        return fd;
    }

    private static MultimodalData ConvertDictToMultimodalData(Dictionary<string, object> dict)
    {
        var text = dict.TryGetValue("text", out var t) ? t?.ToString() ?? string.Empty : string.Empty;
        var files = new List<FileData>();
        if (dict.TryGetValue("files", out var fObj) && fObj is IEnumerable<object> fList)
        {
            foreach (var f in fList)
            {
                if (f is FileData fd) files.Add(fd);
                else if (f is string fs) files.Add(new FileData { Path = fs, OrigName = Path.GetFileName(fs), MimeType = GetMimeType(fs) });
                else if (f is System.Text.Json.JsonElement fje) files.Add(ConvertJsonElementToFileData(fje));
                else if (f is Dictionary<string, object> fileDict)
                {
                    var path = fileDict.TryGetValue("path", out var pathObj) ? pathObj?.ToString() : null;
                    var origName = fileDict.TryGetValue("orig_name", out var origObj) ? origObj?.ToString() : null;
                    var mimeType = fileDict.TryGetValue("mime_type", out var mimeObj) ? mimeObj?.ToString() : null;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        files.Add(new FileData
                        {
                            Path = path,
                            OrigName = origName ?? Path.GetFileName(path),
                            MimeType = mimeType ?? GetMimeType(origName ?? path)
                        });
                    }
                }
            }
        }
        return new MultimodalData(text, files);
    }

    public override object Postprocess(object value)
    {
        if (value == null)
        {
            return new MultimodalData("", new List<FileData>());
        }

        if (value is MultimodalData md)
        {
            return md;
        }

        if (value is string strValue)
        {
            return new MultimodalData(strValue, new List<FileData>());
        }

        if (value is IDictionary<string, object> dictValue)
        {
            var text = dictValue.TryGetValue("text", out var textObj) ? textObj?.ToString() ?? string.Empty : string.Empty;
            var files = new List<FileData>();

            if (dictValue.TryGetValue("files", out var filesObj) && filesObj is IEnumerable<object> fileList)
            {
                foreach (var file in fileList)
                {
                    if (file is FileData fileData)
                    {
                        files.Add(fileData);
                    }
                    else if (file is IDictionary<string, object> fileDict)
                    {
                        // Accept dict-like FileData
                        var path = fileDict.TryGetValue("path", out var p) ? p?.ToString() : null;
                        var origName = fileDict.TryGetValue("orig_name", out var on) ? on?.ToString() : null;
                        var mimeType = fileDict.TryGetValue("mime_type", out var mt) ? mt?.ToString() : null;

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            files.Add(new FileData
                            {
                                Path = path,
                                OrigName = origName ?? Path.GetFileName(path),
                                MimeType = mimeType ?? GetMimeType(origName ?? path)
                            });
                        }
                    }
                    else if (file is string filePath)
                    {
                        var origName = Path.GetFileName(filePath);
                        var newFileData = new FileData
                        {
                            Path = filePath,
                            OrigName = origName,
                            MimeType = GetMimeType(origName)
                        };
                        files.Add(newFileData);
                    }
                }
            }

            if (textObj != null && textObj is not string)
            {
                throw new Exception($"Expected 'text' to be a string, but got {textObj.GetType().Name}");
            }

            return new MultimodalData(text, files);
        }

        throw new Exception($"MultimodalTextbox expects a string or a dictionary with optional keys 'text' and 'files'. Received {value.GetType().Name}");
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        var mimeTypes = new Dictionary<string, string>
            {
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".png", "image/png"},
                {".gif", "image/gif"},
                {".webp", "image/webp"},
                {".mp4", "video/mp4"},
                {".webm", "video/webm"},
                {".mp3", "audio/mpeg"},
                {".wav", "audio/wav"},
                {".pdf", "application/pdf"},
                {".txt", "text/plain"},
                {".json", "application/json"},
                {".html", "text/html"},
                {".css", "text/css"},
                {".js", "text/javascript"}
            };

        return mimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
    }

    private static bool IsValidFile(string filePath, string mimeType, List<string> fileTypes)
    {
        if (fileTypes == null || fileTypes.Count == 0)
        {
            return true;
        }

        var ext = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;
        var mt = (mimeType ?? string.Empty).ToLowerInvariant();

        foreach (var ftRaw in fileTypes)
        {
            var ft = (ftRaw ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ft))
            {
                continue;
            }

            // Explicit extension match: ".json", ".mp4", ...
            if (ft.StartsWith("."))
            {
                if (ft == ext)
                {
                    return true;
                }
                continue;
            }

            // Allow any file
            if (ft == "file")
            {
                return true;
            }

            // MIME patterns: "image", "audio", "video", "text", "application" (prefix match)
            if (!ft.Contains('/'))
            {
                if (mt.StartsWith(ft + "/"))
                {
                    return true;
                }
                continue;
            }

            // Full MIME type or wildcard (e.g. "image/*")
            if (ft.EndsWith("/*"))
            {
                var prefix = ft.Substring(0, ft.Length - 1); // keep trailing '/'
                if (mt.StartsWith(prefix))
                {
                    return true;
                }
            }
            else
            {
                if (mt == ft)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);

        config["sources"] = Sources;
        config["file_types"] = FileTypes;
        config["file_count"] = FileCount;
        config["lines"] = Lines;
        config["max_lines"] = MaxLines;
        config["placeholder"] = Placeholder;
        config["submit_btn"] = SubmitBtn;
        config["stop_btn"] = StopBtn;
        config["autofocus"] = Autofocus;
        config["autoscroll"] = Autoscroll;
        config["max_plain_text_length"] = MaxPlainTextLength;
        config["html_attributes"] = Textbox.SerializeHtmlAttributes(HtmlAttributes);
        config["text_align"] = TextAlign;
        config["rtl"] = Rtl;

        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        // Best-effort: represent input/output as a dict with {text, files}
        return new Dictionary<string, object>
            {
                { "type", "object" },
                {
                    "properties",
                    new Dictionary<string, object>
                    {
                        { "text", new Dictionary<string, object> { { "type", "string" } } },
                        { "files", new Dictionary<string, object> { { "type", "array" }, { "items", new Dictionary<string, object> { { "type", "string" } } } } }
                    }
                }
            };
    }

    public override List<object> ExampleInputs()
    {
        // JSON-serializable example similar to Python example_value()/example_payload()
        return new List<object>
            {
                new Dictionary<string, object>
                {
                    { "text", "Describe this image" },
                    {
                        "files",
                        new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                { "path", "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png" },
                                { "orig_name", "bus.png" },
                                { "mime_type", "image/png" }
                            }
                        }
                    }
                }
            };
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
            {
                { "text", "Describe this image" },
                { "files", new List<string> { "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png" } }
            };
    }

    public override object ExampleValue()
    {
        return new Dictionary<string, object>
            {
                { "text", "Describe this image" },
                { "files", new List<string> { "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png" } }
            };
    }
}

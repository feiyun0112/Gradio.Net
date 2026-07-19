using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("select")]
[Events.Event("upload")]
[Events.Event("change")]
[Events.Event("delete")]
[Events.Event("preview_close")]
[Events.Event("preview_open")]
public class Gallery : Component
{
    public string Format { get; set; }
    public List<string>? FileTypes { get; set; }
    public int? Columns { get; set; }
    public int? Rows { get; set; }
    public object? Height { get; set; }
    public bool AllowPreview { get; set; }
    public bool? Preview { get; set; }
    public int? SelectedIndex { get; set; }
    public string? ObjectFit { get; set; }
    public List<string> Buttons { get; set; }
    public string Type { get; set; }
    public bool FitColumns { get; set; }

    public Gallery(
        object? value = null,
        string format = "webp",
        List<string>? fileTypes = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        int? columns = 2,
        int? rows = null,
        object? height = null,
        bool allowPreview = true,
        bool? preview = null,
        int? selectedIndex = null,
        string? objectFit = null,
        List<string>? buttons = null,
        bool? interactive = null,
        string type = "filepath",
        bool fitColumns = true)
    {
        var validTypes = new[] { "numpy", "pil", "filepath" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid type: {type}. Must be one of ['numpy', 'pil', 'filepath']");
        }

        Format = format;
        FileTypes = fileTypes;
        Columns = columns;
        Rows = rows;
        Height = height;
        AllowPreview = allowPreview;
        Preview = preview;
        SelectedIndex = selectedIndex;
        ObjectFit = objectFit;
        Buttons = buttons ?? new List<string> { "download", "fullscreen" };
        Type = type;
        FitColumns = fitColumns;

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
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

    public override string GetBlockName() => "gallery";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["format"] = Format;
        config["file_types"] = FileTypes;
        config["columns"] = Columns;
        config["rows"] = Rows;
        config["height"] = Height;
        config["allow_preview"] = AllowPreview;
        config["preview"] = Preview;
        config["selected_index"] = SelectedIndex;
        config["object_fit"] = ObjectFit;
        config["buttons"] = Buttons;
        config["type"] = Type;
        config["fit_columns"] = FitColumns;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        var model = ToGalleryData(payload);
        if (model == null || model.Root.Count == 0)
        {
            return null;
        }

        var data = new List<Tuple<object?, string?>>();
        foreach (var item in model.Root)
        {
            string? filePath;
            string? caption;

            if (item is GalleryVideoData gv)
            {
                filePath = gv.Video?.Path;
                caption = gv.Caption;
            }
            else if (item is GalleryImageData gi)
            {
                filePath = gi.Image?.Path;
                caption = gi.Caption;
            }
            else
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                continue;
            }

            if (FileTypes != null && FileTypes.Count > 0 && !IsValidFile(filePath, FileTypes))
            {
                throw new Error($"Invalid file type. Please upload a file that is one of these formats: [{string.Join(", ", FileTypes)}]");
            }

            if (item is GalleryVideoData)
            {
                data.Add(Tuple.Create<object?, string?>(filePath, caption));
            }
            else
            {
                data.Add(Tuple.Create(ConvertToType(filePath, Type), caption));
            }
        }

        return data;
    }

    public override object Postprocess(object value)
    {
        if (value == null)
        {
            return new List<object>();
        }

        if (value is string)
        {
            throw new Error("The `value` passed into `gr.Gallery` must be a list of images or videos, or list of (media, caption) tuples.");
        }

        if (value is not IEnumerable enumerable)
        {
            throw new Error("Gallery output must be iterable.");
        }

        var output = new List<object>();
        foreach (var item in enumerable)
        {
            output.Add(SaveGalleryItemAsDict(item));
        }

        return output;
    }

    public static object ConvertToType(string img, string type)
    {
        // Best-effort parity in C#: return filepath for all supported input types.
        // (Project currently passes file path for image-type conversions in multiple components.)
        return img;
    }

    public override object ExamplePayload()
    {
        return new List<Dictionary<string, object>>
        {
            new()
            {
                ["image"] = new Dictionary<string, object>
                {
                    ["path"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png",
                    ["url"] = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png"
                }
            }
        };
    }

    public override object ExampleValue()
    {
        return new List<string>
        {
            "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png"
        };
    }

    private static object SaveGalleryItem(object item)
    {
        string? caption = null;
        object? media = item;

        if (item is Tuple<object, string> t)
        {
            media = t.Item1;
            caption = t.Item2;
        }
        else if (item is object[] arr && arr.Length >= 2)
        {
            media = arr[0];
            caption = arr[1]?.ToString();
        }
        else if (item is IList list && list.Count >= 2)
        {
            media = list[0];
            caption = list[1]?.ToString();
        }

        var mediaPath = NormalizeToPath(media);
        var mimeType = ProcessingUtils.GetMimeType(mediaPath);
        var isVideo = mimeType.Contains("video", StringComparison.OrdinalIgnoreCase);

        if (isVideo)
        {
            return new GalleryVideoData
            {
                Video = new FileData
                {
                    Path = mediaPath,
                    Url = FileData.IsHttpUrl(mediaPath) ? mediaPath : null,
                    OrigName = GetOrigName(mediaPath),
                    MimeType = mimeType
                },
                Caption = caption
            };
        }

        return new GalleryImageData
        {
            Image = new ImageData
            {
                Path = mediaPath,
                Url = FileData.IsHttpUrl(mediaPath) ? mediaPath : null,
                OrigName = GetOrigName(mediaPath),
                MimeType = mimeType
            },
            Caption = caption
        };
    }

    private static Dictionary<string, object> SaveGalleryItemAsDict(object item)
    {
        var galleryItem = SaveGalleryItem(item);

        if (galleryItem is GalleryVideoData gvd)
        {
            return new Dictionary<string, object>
            {
                ["video"] = BuildFileDict(gvd.Video?.Path, gvd.Video?.Url, gvd.Video?.OrigName, gvd.Video?.MimeType),
                ["caption"] = gvd.Caption
            };
        }

        if (galleryItem is GalleryImageData gid)
        {
            return new Dictionary<string, object>
            {
                ["image"] = BuildFileDict(gid.Image?.Path, gid.Image?.Url, gid.Image?.OrigName, gid.Image?.MimeType),
                ["caption"] = gid.Caption
            };
        }

        // Fallback: empty image entry
        return new Dictionary<string, object>
        {
            ["image"] = BuildFileDict(null, null, null, null),
            ["caption"] = null
        };
    }

    private static Dictionary<string, object> BuildFileDict(string? path, string? url, string? origName, string? mimeType)
    {
        return new Dictionary<string, object>
        {
            ["path"] = path,
            ["url"] = url,
            ["orig_name"] = origName,
            ["mime_type"] = mimeType,
            ["is_stream"] = false,
            ["meta"] = new Dictionary<string, object> { ["_type"] = "gradio.FileData" },
            ["size"] = null
        };
    }

    private static string NormalizeToPath(object? media)
    {
        if (media == null)
        {
            throw new Error("Cannot process null media in Gallery item.");
        }

        if (media is string s)
        {
            return s;
        }

        if (media is FileData fd && !string.IsNullOrWhiteSpace(fd.Path))
        {
            return fd.Path;
        }

        if (media is ImageData id && !string.IsNullOrWhiteSpace(id.Path))
        {
            return id.Path;
        }

        if (media is Uri uri)
        {
            return uri.ToString();
        }

        return media.ToString() ?? string.Empty;
    }

    private static string? GetOrigName(string pathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl)) return null;
        try
        {
            if (FileData.IsHttpUrl(pathOrUrl))
            {
                return Path.GetFileName(new Uri(pathOrUrl).AbsolutePath);
            }
            return Path.GetFileName(pathOrUrl);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsValidFile(string filePath, List<string> fileTypes)
    {
        var ext = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;
        var mime = ProcessingUtils.GetMimeType(filePath).ToLowerInvariant();

        foreach (var t in fileTypes)
        {
            if (string.IsNullOrWhiteSpace(t))
            {
                continue;
            }

            var needle = t.Trim().ToLowerInvariant();
            if (needle.StartsWith("."))
            {
                if (ext == needle) return true;
            }
            else if (needle == "image")
            {
                if (mime.StartsWith("image/")) return true;
            }
            else if (needle == "video")
            {
                if (mime.StartsWith("video/")) return true;
            }
            else
            {
                // fallback: extension keyword or mime fragment match
                if (ext == "." + needle || mime.Contains(needle)) return true;
            }
        }

        return false;
    }

    private static GalleryData? ToGalleryData(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        if (payload is GalleryData gd)
        {
            return gd;
        }

        var root = new List<object>();

        if (payload is IEnumerable enumerable && payload is not string)
        {
            foreach (var item in enumerable)
            {
                if (item is Dictionary<string, object> dict)
                {
                    var fromDict = FromDict(dict);
                    if (fromDict != null) root.Add(fromDict);
                    continue;
                }

                if (item is GalleryImageData || item is GalleryVideoData)
                {
                    root.Add(item);
                }
            }
        }

        return new GalleryData { Root = root };
    }

    private static object? FromDict(Dictionary<string, object> dict)
    {
        if (dict.TryGetValue("video", out var videoObj))
        {
            var fd = ToFileData(videoObj);
            if (fd != null)
            {
                return new GalleryVideoData
                {
                    Video = fd,
                    Caption = dict.TryGetValue("caption", out var c) ? c?.ToString() : null
                };
            }
        }

        if (dict.TryGetValue("image", out var imageObj))
        {
            var img = ToImageData(imageObj);
            if (img != null)
            {
                return new GalleryImageData
                {
                    Image = img,
                    Caption = dict.TryGetValue("caption", out var c) ? c?.ToString() : null
                };
            }
        }

        return null;
    }

    private static FileData? ToFileData(object? obj)
    {
        if (obj is FileData fd) return fd;
        if (obj is string s) return new FileData { Path = s };
        if (obj is Dictionary<string, object> dict)
        {
            return new FileData
            {
                Path = dict.TryGetValue("path", out var p) ? p?.ToString() ?? string.Empty : string.Empty,
                Url = dict.TryGetValue("url", out var u) ? u?.ToString() : null,
                OrigName = dict.TryGetValue("orig_name", out var o) ? o?.ToString() : null,
                MimeType = dict.TryGetValue("mime_type", out var m) ? m?.ToString() : null
            };
        }

        return null;
    }

    private static ImageData? ToImageData(object? obj)
    {
        if (obj is ImageData id) return id;
        if (obj is string s) return new ImageData { Path = s };
        if (obj is Dictionary<string, object> dict)
        {
            return new ImageData
            {
                Path = dict.TryGetValue("path", out var p) ? p?.ToString() : null,
                Url = dict.TryGetValue("url", out var u) ? u?.ToString() : null,
                OrigName = dict.TryGetValue("orig_name", out var o) ? o?.ToString() : null,
                MimeType = dict.TryGetValue("mime_type", out var m) ? m?.ToString() : null
            };
        }

        return null;
    }
}

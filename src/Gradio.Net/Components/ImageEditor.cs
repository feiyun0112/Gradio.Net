using Gradio.Net.Core;
using Gradio.Net.Data;
using SixLabors.ImageSharp;

namespace Gradio.Net.Components;

[Events.Event("clear")]
[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
[Events.Event("upload")]
[Events.Event("apply")]
public class ImageEditor : Component
{
    public bool _selectable { get; set; } = false;
    public WebcamOptions WebcamOptions { get; set; }
    public string Type { get; set; } = "numpy";
    public object Height { get; set; }
    public object Width { get; set; }
    public string ImageMode { get; set; } = "RGBA";
    public List<string> Sources { get; set; }
    public List<string> Buttons { get; set; }
    public List<string> Transforms { get; set; }
    public Eraser Eraser { get; set; }
    public Brush Brush { get; set; }
    public string Format { get; set; } = "webp";
    public LayerOptions Layers { get; set; }
    public Tuple<int, int> CanvasSize { get; set; } = new Tuple<int, int>(800, 800);
    public bool FixedCanvas { get; set; } = false;
    public string Placeholder { get; set; }
    public Dictionary<string, EditorDataBlobs> BlobStorage { get; set; } = new Dictionary<string, EditorDataBlobs>();

    public ImageEditor(
        object value = null,
        object height = null,
        object width = null,
        string imageMode = "RGBA",
        object sources = null,
        string type = "numpy",
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        List<string> buttons = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string placeholder = null,
        bool _selectable = false,
        object transforms = null,
        object eraser = null,
        object brush = null,
        string format = "webp",
        object layers = null,
        Tuple<int, int> canvasSize = null,
        bool fixedCanvas = false,
        WebcamOptions webcamOptions = null
    ) : base()
    {
        this._selectable = _selectable;
        this.WebcamOptions = webcamOptions ?? new WebcamOptions();

        // Validate type
        var validTypes = new List<string> { "numpy", "pil", "filepath" };
        if (!validTypes.Contains(type))
        {
            throw new ArgumentException($"Invalid value for parameter 'type': {type}. Please choose from one of: {string.Join(", ", validTypes)}");
        }
        this.Type = type;
        this.Height = height;
        this.Width = width;
        this.ImageMode = imageMode;

        // Validate sources
        var validSources = new List<string> { "upload", "webcam", "clipboard" };
        if (sources is string sourcesStr)
        {
            if (!validSources.Contains(sourcesStr))
            {
                throw new ArgumentException($"`sources` must be a list consisting of elements in [{string.Join(", ", validSources)}]");
            }
            this.Sources = new List<string> { sourcesStr };
        }
        else if (sources is IEnumerable<string> sourcesEnum)
        {
            this.Sources = sourcesEnum.ToList();
            foreach (var src in this.Sources)
            {
                if (!validSources.Contains(src))
                {
                    throw new ArgumentException($"`sources` must be a list consisting of elements in [{string.Join(", ", validSources)}]");
                }
            }
        }
        else
        {
            this.Sources = new List<string> { "upload", "webcam", "clipboard" };
        }

        this.Buttons = buttons;
        this.Transforms = transforms is IEnumerable<string> transformsEnum ? transformsEnum.ToList() : new List<string> { "crop", "resize" };
        this.Eraser = eraser as Eraser ?? new Eraser();
        this.Brush = brush as Brush ?? new Brush();
        this.Format = format;

        // Validate layers
        if (layers is bool layersBool)
        {
            this.Layers = layersBool ? new LayerOptions() : new LayerOptions { Disabled = true };
        }
        else if (layers is LayerOptions layersOptions)
        {
            this.Layers = layersOptions;
        }
        else
        {
            this.Layers = new LayerOptions();
        }

        this.CanvasSize = canvasSize ?? new Tuple<int, int>(800, 800);
        this.FixedCanvas = fixedCanvas;
        this.Placeholder = placeholder;

        // Set base properties
        this.Label = label;
        this.ShowLabel = showLabel ?? true;
        this.Container = container;
        this.Scale = scale;
        this.MinWidth = minWidth;
        this.Interactive = interactive;
        this.Value = value;
    }

    public object ConvertAndFormatImage(object file)
    {
        if (file == null)
        {
            return null;
        }

        string name = "image";
        string suffix = Format;
        object source = file;

        if (file is FileData fd)
        {
            source = fd.Path;
            if (!string.IsNullOrWhiteSpace(fd.OrigName))
            {
                var p = new FileInfo(fd.OrigName);
                name = Path.GetFileNameWithoutExtension(p.Name);
                suffix = p.Extension.TrimStart('.').ToLowerInvariant();
            }
            else if (!string.IsNullOrWhiteSpace(fd.Path))
            {
                name = Path.GetFileNameWithoutExtension(fd.Path);
                suffix = Path.GetExtension(fd.Path).TrimStart('.').ToLowerInvariant();
            }
        }

        if (suffix == "jpg")
        {
            suffix = "jpeg";
        }

        var im = ImageUtils.OpenImage(source);
        try
        {
            return ImageUtils.FormatImage(im, Type, GRADIO_CACHE, name, suffix);
        }
        finally
        {
            // If returning filepath/numpy representation we can dispose temporary image object.
            if (Type != "pil")
            {
                im.Dispose();
            }
        }
    }

    public override object Preprocess(object payload)
    {
        if (payload == null)
        {
            return null;
        }

        if (payload is EditorData editorData)
        {
            EditorDataBlobs _payload = null;
            if (!string.IsNullOrEmpty(editorData.Id))
            {
                if (BlobStorage.TryGetValue(editorData.Id, out var cached))
                {
                    _payload = cached;
                }
            }
            else
            {
                _payload = new EditorDataBlobs
                {
                    Background = editorData.Background?.Path != null ? File.ReadAllBytes(editorData.Background.Path) : null,
                    Layers = editorData.Layers.Select(l => l.Path != null ? File.ReadAllBytes(l.Path) : null).ToList(),
                    Composite = editorData.Composite?.Path != null ? File.ReadAllBytes(editorData.Composite.Path) : null
                };
            }

            if (_payload != null)
            {
                var bgSource = _payload.Background != null ? (object)_payload.Background : editorData.Background;
                var bg = ConvertAndFormatImage(bgSource);
                var layers = _payload.Layers != null
                    ? _payload.Layers.Select(l => ConvertAndFormatImage(l)).Where(x => x != null).ToList()
                    : new List<object>();
                var compositeSource = _payload.Composite != null ? (object)_payload.Composite : editorData.Composite;
                var composite = ConvertAndFormatImage(compositeSource);

                if (!string.IsNullOrEmpty(editorData.Id) && BlobStorage.ContainsKey(editorData.Id))
                {
                    BlobStorage.Remove(editorData.Id);
                }

                return new Dictionary<string, object>
                {
                    ["background"] = bg,
                    ["layers"] = layers,
                    ["composite"] = composite
                };
            }
        }

        return null;
    }

    public override object Postprocess(object value)
    {
        if (value == null)
        {
            return null;
        }

        Dictionary<string, object> asDict;

        if (value is EditorValue editorValue)
        {
            asDict = new Dictionary<string, object>
            {
                ["background"] = editorValue.Background,
                ["layers"] = editorValue.Layers?.Cast<object>().ToList() ?? new List<object>(),
                ["composite"] = editorValue.Composite
            };
        }
        else if (value is Dictionary<string, object> dict)
        {
            asDict = dict;
        }
        else if (value is string || value is byte[] || value is Stream)
        {
            asDict = new Dictionary<string, object>
            {
                ["background"] = value,
                ["layers"] = new List<object>(),
                ["composite"] = value
            };
        }

        else
        {
            throw new ArgumentException("The value to `gr.ImageEditor` must be a dictionary of images or a single image.");
        }

        var layers = new List<FileData>();
        if (asDict.TryGetValue("layers", out var layersObj) && layersObj is IEnumerable<object> layerEnumerable)
        {
            foreach (var layer in layerEnumerable)
            {
                if (layer == null) continue;
                layers.Add(new FileData
                {
                    Path = ImageUtils.SaveImage(layer, GRADIO_CACHE, Format)
                });
            }
        }

        FileData bgData = null;
        if (asDict.TryGetValue("background", out var bgObj) && bgObj != null)
        {
            bgData = new FileData
            {
                Path = ImageUtils.SaveImage(bgObj, GRADIO_CACHE, Format)
            };
        }

        FileData compositeData = null;
        if (asDict.TryGetValue("composite", out var compObj) && compObj != null)
        {
            compositeData = new FileData
            {
                Path = ImageUtils.SaveImage(compObj, GRADIO_CACHE, Format)
            };
        }

        return new EditorData
        {
            Background = bgData,
            Layers = layers,
            Composite = compositeData
        };
    }

    public override object ExamplePayload()
    {
        return new EditorData
        {
            Background = new FileData { Path = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png" },
            Layers = new List<FileData>(),
            Composite = null
        };
    }

    public override object ExampleValue()
    {
        return new EditorValue
        {
            Background = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png",
            Layers = new List<object>(),
            Composite = null
        };
    }

    public override List<object> ExampleInputs()
    {
        return new List<object> { ExampleValue() };
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["height"] = Height;
        config["width"] = Width;
        config["image_mode"] = ImageMode;
        config["sources"] = Sources;
        config["type"] = Type;
        config["buttons"] = Buttons;
        config["placeholder"] = Placeholder;
        config["_selectable"] = _selectable;
        config["transforms"] = Transforms;
        config["eraser"] = Eraser;
        config["brush"] = Brush;
        config["format"] = Format;
        config["layers"] = Layers;
        config["canvas_size"] = new object[] { CanvasSize.Item1, CanvasSize.Item2 };
        config["fixed_canvas"] = FixedCanvas;
        config["webcam_options"] = WebcamOptions;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["description"] = "{background, layers, composite} image editor payload"
        };
    }

    public void AcceptBlobs(AcceptBlobs data)
    {
        var type = data.Data.Type;
        var index = data.Data.Index;
        var file = data.Files[0].Item2;
        var id = data.Data.Id;

        var current = BlobStorage.TryGetValue(id, out var existing)
            ? existing
            : new EditorDataBlobs { Background = null, Layers = new List<byte[]>(), Composite = null };

        if (type == "layer" && index.HasValue)
        {
            if (index.Value >= current.Layers.Count)
            {
                current.Layers.AddRange(Enumerable.Repeat<byte[]>(null, index.Value + 1 - current.Layers.Count));
            }
            current.Layers[index.Value] = file;
        }
        else if (type == "background")
        {
            current.Background = file;
        }
        else if (type == "composite")
        {
            current.Composite = file;
        }

        BlobStorage[id] = current;
    }
}

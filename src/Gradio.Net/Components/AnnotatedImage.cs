using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace Gradio.Net.Components
{
    [Event("select")]
    public class AnnotatedImage : Component
    {
        public string Format { get; set; } = "webp";
        public bool ShowLegend { get; set; } = true;
        public object Height { get; set; }
        public object Width { get; set; }
        public Dictionary<string, string> ColorMap { get; set; }
        public List<string> Buttons { get; set; }

        public AnnotatedImage(
            object value = null,
            string format = "webp",
            bool showLegend = true,
            object height = null,
            object width = null,
            Dictionary<string, string> colorMap = null,
            string label = null,
            object every = null,
            object inputs = null,
            bool? showLabel = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            object visible = null,
            string elemId = null,
            object elemClasses = null,
            bool render = true,
            object key = null,
            object preservedByKey = null,
            List<string> buttons = null)
            : base()
        {
            Value = value;
            Format = string.IsNullOrWhiteSpace(format) ? "webp" : format;
            ShowLegend = showLegend;
            Height = height;
            Width = width;
            ColorMap = colorMap;
            Buttons = buttons ?? new List<string> { "fullscreen" };

            Label = label;
            ShowLabel = showLabel ?? true;
            Container = container;
            Scale = scale;
            MinWidth = minWidth;

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

        public override string GetBlockName() => "annotatedimage";

        public override Dictionary<string, object> GetConfig(Type cls = null)
        {
            var config = base.GetConfig(cls);
            config["format"] = Format;
            config["show_legend"] = ShowLegend;
            config["height"] = Height;
            config["width"] = Width;
            config["color_map"] = ColorMap;
            config["buttons"] = Buttons;
            return config;
        }

        public override object Preprocess(object payload)
        {
            if (payload == null)
            {
                return null;
            }

            if (payload is AnnotatedImageData aid)
            {
                var baseImg = aid.Image?.Path;
                var ann = aid.Annotations
                    .Where(a => a?.Image?.Path != null)
                    .Select(a => Tuple.Create(a.Image.Path, a.Label ?? string.Empty))
                    .ToList();
                return Tuple.Create(baseImg, ann);
            }

            if (payload is IDictionary<string, object> dict)
            {
                var imagePath = (dict.TryGetValue("image", out var imageObj) ? ExtractFilePath(imageObj) : null) ?? string.Empty;
                var outAnnotations = new List<Tuple<string, string>>();
                if (dict.TryGetValue("annotations", out var annsObj) && annsObj is IEnumerable anns)
                {
                    foreach (var a in anns)
                    {
                        if (a is IDictionary<string, object> ad)
                        {
                            var maskPath = ad.TryGetValue("image", out var mo) ? ExtractFilePath(mo) : null;
                            var label = ad.TryGetValue("label", out var lo) ? lo?.ToString() ?? string.Empty : string.Empty;
                            if (!string.IsNullOrWhiteSpace(maskPath))
                            {
                                outAnnotations.Add(Tuple.Create(maskPath, label));
                            }
                        }
                    }
                }

                return Tuple.Create(imagePath, outAnnotations);
            }

            return payload;
        }

        public override object Postprocess(object value)
        {
            if (value == null)
            {
                return null;
            }

            var (baseImageObj, annotationsObj) = ExtractPostprocessTuple(value);
            if (baseImageObj == null)
            {
                return null;
            }

            var baseImagePath = ResolveBaseImagePath(baseImageObj, out var width, out var height);

            var sections = new List<Annotation>();
            foreach (var (maskObj, label) in EnumerateAnnotationPairs(annotationsObj))
            {
                var alpha = BuildMaskAlpha(maskObj, width, height);
                var rgb = ResolveColor(label);

                using var maskImage = new Image<Rgba32>(width, height);
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var a = Math.Clamp(alpha[y, x], 0f, 1f);
                        var ap = (byte)(a * 255f);
                        maskImage[x, y] = new Rgba32(rgb.r, rgb.g, rgb.b, ap);
                    }
                }

                var maskPath = SavePngToCache(maskImage);
                sections.Add(new Annotation
                {
                    Image = new FileData { Path = maskPath },
                    Label = label
                });
            }

            return new AnnotatedImageData
            {
                Image = new FileData { Path = baseImagePath },
                Annotations = sections
            };
        }

        public override object ExamplePayload()
        {
            return new Dictionary<string, object>
            {
                ["image"] = new FileData
                {
                    Path = "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png"
                },
                ["annotations"] = new List<object>()
            };
        }

        public override object ExampleValue()
        {
            return Tuple.Create<object, object>(
                "https://raw.githubusercontent.com/gradio-app/gradio/main/test/test_files/bus.png",
                new List<object> { new object[] { new[] { 0, 0, 100, 100 }, "bus" } }
            );
        }

        private static (object baseImage, object annotations) ExtractPostprocessTuple(object value)
        {
            if (value is Tuple<object, object> t)
            {
                return (t.Item1, t.Item2);
            }
            if (value is object[] arr && arr.Length >= 2)
            {
                return (arr[0], arr[1]);
            }
            if (value is IList list && list.Count >= 2)
            {
                return (list[0], list[1]);
            }

            throw new ArgumentException("AnnotatedImage expects a tuple/list of (base_image, annotations)");
        }

        private static IEnumerable<(object mask, string label)> EnumerateAnnotationPairs(object annotations)
        {
            if (annotations is not IEnumerable enumerable)
            {
                yield break;
            }

            foreach (var item in enumerable)
            {
                if (item is Tuple<object, string> t)
                {
                    yield return (t.Item1, t.Item2 ?? string.Empty);
                    continue;
                }

                if (item is object[] arr && arr.Length >= 2)
                {
                    yield return (arr[0], arr[1]?.ToString() ?? string.Empty);
                    continue;
                }

                if (item is IList list && list.Count >= 2)
                {
                    yield return (list[0], list[1]?.ToString() ?? string.Empty);
                }
            }
        }

        private string ResolveBaseImagePath(object baseImageObj, out int width, out int height)
        {
            string path;

            if (baseImageObj is string s)
            {
                if (FileData.IsHttpUrl(s))
                {
                    s = ProcessingUtils.SaveUrlToCache(s, GRADIO_CACHE);
                }

                path = Path.GetFullPath(s);
                using var img = ImageSharpImage.Load(path);
                width = img.Width;
                height = img.Height;
                return path;
            }

            if (baseImageObj is ImageSharpImage imgSharp)
            {
                width = imgSharp.Width;
                height = imgSharp.Height;
                path = SaveImageToCache(imgSharp, Format);
                return path;
            }

            throw new ArgumentException("AnnotatedImage only accepts filepaths/URLs or ImageSharp images for the base image.");
        }

        private float[,] BuildMaskAlpha(object maskObj, int width, int height)
        {
            var alpha = new float[height, width];

            if (TryGetBoundingBox(maskObj, out var x1, out var y1, out var x2, out var y2))
            {
                x1 = Math.Clamp(x1, 0, width);
                x2 = Math.Clamp(x2, 0, width);
                y1 = Math.Clamp(y1, 0, height);
                y2 = Math.Clamp(y2, 0, height);

                var border = 3;
                for (var y = y1; y < y2; y++)
                {
                    for (var x = x1; x < x2; x++)
                    {
                        alpha[y, x] = 0.5f;
                    }
                }

                for (var y = y1; y < y2; y++)
                {
                    for (var x = x1; x < Math.Min(x1 + border, x2); x++) alpha[y, x] = 1f;
                    for (var x = Math.Max(x2 - border, x1); x < x2; x++) alpha[y, x] = 1f;
                }
                for (var y = y1; y < Math.Min(y1 + border, y2); y++)
                {
                    for (var x = x1; x < x2; x++) alpha[y, x] = 1f;
                }
                for (var y = Math.Max(y2 - border, y1); y < y2; y++)
                {
                    for (var x = x1; x < x2; x++) alpha[y, x] = 1f;
                }

                return alpha;
            }

            if (maskObj is float[,] f2)
            {
                CopyMaskToAlpha(f2, alpha);
                return alpha;
            }
            if (maskObj is double[,] d2)
            {
                CopyMaskToAlpha(d2, alpha);
                return alpha;
            }
            if (maskObj is byte[,] b2)
            {
                CopyMaskToAlpha(b2, alpha, 255f);
                return alpha;
            }

            return alpha;
        }

        private static void CopyMaskToAlpha<T>(T[,] src, float[,] dst, float scale = 1f) where T : struct
        {
            var h = Math.Min(src.GetLength(0), dst.GetLength(0));
            var w = Math.Min(src.GetLength(1), dst.GetLength(1));
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var v = Convert.ToSingle(src[y, x]);
                    dst[y, x] = scale == 1f ? v : (v / scale);
                }
            }
        }

        private static bool TryGetBoundingBox(object mask, out int x1, out int y1, out int x2, out int y2)
        {
            x1 = y1 = x2 = y2 = 0;

            if (mask is int[] ia && ia.Length >= 4)
            {
                x1 = ia[0]; y1 = ia[1]; x2 = ia[2]; y2 = ia[3];
                return true;
            }
            if (mask is long[] la && la.Length >= 4)
            {
                x1 = (int)la[0]; y1 = (int)la[1]; x2 = (int)la[2]; y2 = (int)la[3];
                return true;
            }
            if (mask is object[] oa && oa.Length >= 4)
            {
                x1 = Convert.ToInt32(oa[0]); y1 = Convert.ToInt32(oa[1]); x2 = Convert.ToInt32(oa[2]); y2 = Convert.ToInt32(oa[3]);
                return true;
            }
            if (mask is IList list && list.Count >= 4)
            {
                x1 = Convert.ToInt32(list[0]); y1 = Convert.ToInt32(list[1]); x2 = Convert.ToInt32(list[2]); y2 = Convert.ToInt32(list[3]);
                return true;
            }

            return false;
        }

        private (byte r, byte g, byte b) ResolveColor(string label)
        {
            if (ColorMap != null && label != null && ColorMap.TryGetValue(label, out var hex) && TryParseHex(hex, out var rgb))
            {
                return rgb;
            }

            return (255, 0, 0);
        }

        private static bool TryParseHex(string hex, out (byte r, byte g, byte b) rgb)
        {
            rgb = (255, 0, 0);
            if (string.IsNullOrWhiteSpace(hex))
            {
                return false;
            }

            var s = hex.Trim().TrimStart('#');
            if (s.Length == 3)
            {
                s = string.Concat(s[0], s[0], s[1], s[1], s[2], s[2]);
            }
            if (s.Length != 6)
            {
                return false;
            }

            try
            {
                rgb = (
                    Convert.ToByte(s.Substring(0, 2), 16),
                    Convert.ToByte(s.Substring(2, 2), 16),
                    Convert.ToByte(s.Substring(4, 2), 16)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string SavePngToCache(Image<Rgba32> image)
        {
            Directory.CreateDirectory(GRADIO_CACHE);
            var path = Path.Combine(GRADIO_CACHE, $"annotated_mask_{Guid.NewGuid():N}.png");
            image.Save(path);
            return Path.GetFullPath(path);
        }

        private string SaveImageToCache(ImageSharpImage image, string format)
        {
            Directory.CreateDirectory(GRADIO_CACHE);
            var ext = string.IsNullOrWhiteSpace(format) ? "png" : format.ToLowerInvariant();
            var path = Path.Combine(GRADIO_CACHE, $"annotated_base_{Guid.NewGuid():N}.{ext}");
            image.Save(path);
            return Path.GetFullPath(path);
        }

        private static string ExtractFilePath(object imageObj)
        {
            if (imageObj is FileData fd)
            {
                return fd.Path;
            }

            if (imageObj is IDictionary<string, object> dict && dict.TryGetValue("path", out var p))
            {
                return p?.ToString();
            }

            return imageObj?.ToString();
        }
    }
}

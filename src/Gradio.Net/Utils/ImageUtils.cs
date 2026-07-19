using System.Text.Json;
using System.Text;
using Gradio.Net.Core;
using Gradio.Net.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Gradio.Net
{
    public static class ImageUtils
    {
        public static Image OpenImage(object origImg)
        {
            if (origImg == null)
            {
                throw new ArgumentNullException(nameof(origImg));
            }

            if (origImg is byte[] byteArray)
            {
                return Image.Load(byteArray);
            }
            else if (origImg is string stringPath)
            {
                return Image.Load(stringPath);
            }
            else if (origImg is Stream stream)
            {
                return Image.Load(stream);
            }
            else if (origImg is Image image)
            {
                return image;
            }
            else
            {
                throw new ArgumentException(
                    "Expected image or path to image of type webp, png, bmp or jpeg; ImageSharp image; or byte array. Received "
                    + origImg.GetType().FullName
                );
            }
        }

        public static object FormatImage(
            Image im,
            string type,
            string cacheDir,
            string name = "image",
            string format = "webp"
        )
        {
            if (im == null)
            {
                return null;
            }

            switch (type)
            {
                case "pil":
                    return im;
                case "numpy":
                    // Convert to byte array as a representation
                    using (var stream = new MemoryStream())
                    {
                        im.Save(stream, GetImageFormat(format));
                        return stream.ToArray();
                    }
                case "filepath":
                    // Save to file
                    Directory.CreateDirectory(cacheDir);
                    var filePath = Path.Combine(cacheDir, $"{name}.{format}");
                    im.Save(filePath, GetImageFormat(format));
                    return filePath;
                default:
                    throw new ArgumentException($"Unknown type: {type}. Please choose from: 'numpy', 'pil', 'filepath'.");
            }
        }

        private static IImageEncoder GetImageFormat(string format)
        {
            switch (format.ToLower())
            {
                case "webp":
                    return new WebpEncoder();
                case "png":
                    return new PngEncoder();
                case "jpeg":
                case "jpg":
                    return new JpegEncoder();
                default:
                    return new WebpEncoder();
            }
        }

        public static string SaveImage(
            object y,
            string cacheDir,
            string format = "webp"
        )
        {
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            // Compute content hash for unique cache directory (matches Python behavior)
            byte[] imageBytes;
            if (y is Image imgForHash)
            {
                using var ms = new System.IO.MemoryStream();
                imgForHash.Save(ms, GetImageFormat(format));
                imageBytes = ms.ToArray();
            }
            else if (y is byte[] rawBytes)
            {
                imageBytes = rawBytes;
            }
            else
            {
                imageBytes = null;
            }

            string hash = imageBytes != null ? Core.ProcessingUtils.HashBytes(imageBytes) : Guid.NewGuid().ToString("N");
            string hashDir = System.IO.Path.Combine(cacheDir, hash);
            Directory.CreateDirectory(hashDir);
            string filePath = System.IO.Path.Combine(hashDir, $"image.{format}");

            if (y is Image img)
            {
                img.Save(filePath, GetImageFormat(format));
            }
            else if (y is byte[] byteArray)
            {
                using (var loadedImage = Image.Load(byteArray))
                {
                    loadedImage.Save(filePath, GetImageFormat(format));
                }
            }
            else if (y is string stringPath)
            {
                // Match Python behavior: string values are treated as already-usable
                // image references (local path or URL) and passed through as-is.
                if (stringPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    stringPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return stringPath;
                }

                if (File.Exists(stringPath))
                {
                    return stringPath;
                }

                throw new FileNotFoundException("Image file not found", stringPath);
            }
            else
            {
                throw new ArgumentException(
                    "Cannot process this value as an Image, it is of type: " + y.GetType().FullName
                );
            }

            return filePath;
        }

        public static Image AddWatermark(
            object baseImg,
            Components.WatermarkOptions watermarkOption
        )
        {
            if (baseImg == null)
            {
                throw new ArgumentNullException(nameof(baseImg));
            }

            if (watermarkOption == null)
            {
                throw new ArgumentNullException(nameof(watermarkOption));
            }

            var baseImage = OpenImage(baseImg);
            var watermarkImage = OpenImage(watermarkOption.Watermark);

            int baseWidth = baseImage.Width;
            int baseHeight = baseImage.Height;
            int watermarkWidth = watermarkImage.Width;
            int watermarkHeight = watermarkImage.Height;

            int x, y;
            const int padding = 10;

            // Calculate position based on watermark option
            if (watermarkOption.Position is string positionStr)
            {
                switch (positionStr.ToLower())
                {
                    case "top-left":
                        x = padding;
                        y = padding;
                        break;
                    case "top-right":
                        x = baseWidth - watermarkWidth - padding;
                        y = padding;
                        break;
                    case "bottom-left":
                        x = padding;
                        y = baseHeight - watermarkHeight - padding;
                        break;
                    case "bottom-right":
                    default:
                        x = baseWidth - watermarkWidth - padding;
                        y = baseHeight - watermarkHeight - padding;
                        break;
                }
            }
            else if (watermarkOption.Position is (int posX, int posY))
            {
                x = posX;
                y = posY;
            }
            else
            {
                // Default to bottom-right
                x = baseWidth - watermarkWidth - padding;
                y = baseHeight - watermarkHeight - padding;
            }

            // Validate and adjust position if out of bounds
            if (x < 0 || x + watermarkWidth > baseWidth || y < 0 || y + watermarkHeight > baseHeight)
            {
                x = baseWidth - watermarkWidth - 10;
                y = baseHeight - watermarkHeight - 10;
            }

            // Create result image as RGBA for alpha compositing
            var result = baseImage.CloneAs<Rgba32>();
            var watermark = watermarkImage.CloneAs<Rgba32>();

            // Overlay watermark with alpha blending
            result.Mutate(ctx => ctx.DrawImage(watermark, new Point(x, y), 1f));

            return result;
        }

        public static Image CropScale(
            object img,
            int finalWidth,
            int finalHeight
        )
        {
            if (img == null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            var image = OpenImage(img);

            int originalWidth = image.Width;
            int originalHeight = image.Height;
            double targetAspectRatio = (double)finalWidth / finalHeight;

            int cropWidth, cropHeight;

            // Calculate crop dimensions to match target aspect ratio
            if ((double)originalWidth / originalHeight > targetAspectRatio)
            {
                // Image is wider than target ratio - crop width
                cropHeight = originalHeight;
                cropWidth = (int)(cropHeight * targetAspectRatio);
            }
            else
            {
                // Image is taller than target ratio - crop height
                cropWidth = originalWidth;
                cropHeight = (int)(cropWidth / targetAspectRatio);
            }

            // Center the crop
            int left = (originalWidth - cropWidth) / 2;
            int top = (originalHeight - cropHeight) / 2;

            // Convert to Rgba32 for processing
            var processable = image.CloneAs<Rgba32>();
            processable.Mutate(ctx =>
            {
                // Crop the image
                ctx.Crop(new Rectangle(left, top, cropWidth, cropHeight));
                // Resize to final dimensions
                ctx.Resize(finalWidth, finalHeight);
            });

            return processable;
        }

        public static Image DecodeBase64ToImage(string encoding)
        {
            if (string.IsNullOrEmpty(encoding))
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            // Extract base64 data part (remove data:image/xxx;base64, prefix)
            string base64Data = encoding;
            if (encoding.StartsWith("data:"))
            {
                int commaIndex = encoding.IndexOf(",");
                if (commaIndex > 0)
                {
                    base64Data = encoding.Substring(commaIndex + 1);
                }
            }

            // Decode base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Load byte array as image
            using (var stream = new MemoryStream(imageBytes))
            {
                return Image.Load(stream);
            }
        }

        public static byte[] DecodeBase64ToImageArray(string encoding)
        {
            var img = DecodeBase64ToImage(encoding);

            // Convert image to byte array
            using (var stream = new MemoryStream())
            {
                img.Save(stream, new PngEncoder());
                return stream.ToArray();
            }
        }

        public static string DecodeBase64ToFile(
            string encoding,
            string cacheDir,
            string format = "webp"
        )
        {
            var img = DecodeBase64ToImage(encoding);
            return SaveImage(img, cacheDir, format);
        }

        public static string EncodeImageArrayToBase64(object imageArray)
        {
            if (imageArray == null)
            {
                throw new ArgumentNullException(nameof(imageArray));
            }

            byte[] imageBytes;

            if (imageArray is byte[] byteArray)
            {
                imageBytes = byteArray;
            }
            else if (imageArray is Image image)
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, new JpegEncoder());
                    imageBytes = stream.ToArray();
                }
            }
            else
            {
                throw new ArgumentException(
                    "Expected byte array or Image object. Received " + imageArray.GetType().FullName
                );
            }

            string base64Str = Convert.ToBase64String(imageBytes);
            return "data:image/jpeg;base64," + base64Str;
        }

        public static string EncodeImageToBase64(object image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            Image img;
            if (image is Image imageObj)
            {
                img = imageObj;
            }
            else if (image is byte[] byteArray)
            {
                using (var stream = new MemoryStream(byteArray))
                {
                    img = Image.Load(stream);
                }
            }
            else
            {
                throw new ArgumentException(
                    "Expected Image object or byte array. Received " + image.GetType().FullName
                );
            }

            using (var stream = new MemoryStream())
            {
                img.Save(stream, new JpegEncoder());
                string base64Str = Convert.ToBase64String(stream.ToArray());
                return "data:image/jpeg;base64," + base64Str;
            }
        }

        public static string EncodeImageFileToBase64(object imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            string filePath;
            if (imageFile is string path)
            {
                filePath = path;
            }
            else
            {
                throw new ArgumentException(
                    "Expected string path to image file. Received " + imageFile.GetType().FullName
                );
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image file not found", filePath);
            }

            // Determine MIME type based on file extension
            string extension = Path.GetExtension(filePath).ToLower();
            string mimeType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "image/jpeg"
            };

            // Read file and encode to base64
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64Str = Convert.ToBase64String(fileBytes);

            return $"data:{mimeType};base64,{base64Str}";
        }

        public static string ExtractSvgContent(object imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            string filePath;
            if (imageFile is string path)
            {
                filePath = path;
            }
            else
            {
                throw new ArgumentException(
                    "Expected string path or URL to SVG file. Received " + imageFile.GetType().FullName
                );
            }

            if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
            {
                // Fetch SVG from URL
                using (var client = new HttpClient())
                {
                    return client.GetStringAsync(filePath).Result;
                }
            }
            else
            {
                // Read SVG from local file
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("SVG file not found", filePath);
                }
                return File.ReadAllText(filePath);
            }
        }

        public static object PreprocessImage(
            object payload,
            string cacheDir,
            string format,
            string imageMode,
            string type
        )
        {
            if (payload == null)
            {
                return null;
            }

            static string? GetStringFromDict(IDictionary<string, object> dict, params string[] keys)
            {
                foreach (var key in keys)
                {
                    if (dict.TryGetValue(key, out var value) && value != null)
                    {
                        if (value is string s)
                        {
                            return s;
                        }
                        if (value is JsonElement je && je.ValueKind == JsonValueKind.String)
                        {
                            return je.GetString();
                        }
                        return value.ToString();
                    }
                }

                return null;
            }

            static string? GetStringFromJsonElement(JsonElement obj, params string[] keys)
            {
                if (obj.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                foreach (var key in keys)
                {
                    if (obj.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
                    {
                        return value.GetString();
                    }
                }

                return null;
            }

            string url = null;
            string path = null;
            string origName = null;

            // Handle plain string inputs (MCP passes file URLs or paths as raw strings).
            if (payload is string payloadStr)
            {
                if (payloadStr.StartsWith("data:"))
                {
                    // Inline base64 data — handle directly without going through path logic.
                    switch (type)
                    {
                        case "pil":
                            return DecodeBase64ToImage(payloadStr);
                        case "numpy":
                            return DecodeBase64ToImageArray(payloadStr);
                        case "filepath":
                            return DecodeBase64ToFile(payloadStr, cacheDir, format);
                        default:
                            throw new ArgumentException($"Unknown type: {type}. Please choose from: 'numpy', 'pil', 'filepath'.");
                    }
                }
                else if (payloadStr.StartsWith("http://") || payloadStr.StartsWith("https://"))
                {
                    // Remote URL — download to local cache so ImageSharp can open it.
                    url = payloadStr;
                    path = Core.ProcessingUtils.SaveUrlToCache(payloadStr, cacheDir);
                }
                else
                {
                    // Local file path.
                    path = payloadStr;
                }
            }
            // Python-compatible payloads may come in as dictionaries with snake_case keys.
            else if (payload is IDictionary<string, object> dictPayload)
            {
                url = GetStringFromDict(dictPayload, "Url", "url");
                path = GetStringFromDict(dictPayload, "Path", "path");
                origName = GetStringFromDict(dictPayload, "OrigName", "orig_name", "origName");
            }
            else if (payload is JsonElement jePayload)
            {
                url = GetStringFromJsonElement(jePayload, "Url", "url");
                path = GetStringFromJsonElement(jePayload, "Path", "path");
                origName = GetStringFromJsonElement(jePayload, "OrigName", "orig_name", "origName");
            }
            else
            {
                // Assume payload has Url and Path properties similar to Python's ImageData
                var payloadType = payload.GetType();
                var urlProperty = payloadType.GetProperty("Url");
                var pathProperty = payloadType.GetProperty("Path");
                var origNameProperty = payloadType.GetProperty("OrigName");

                url = urlProperty?.GetValue(payload) as string;
                path = pathProperty?.GetValue(payload) as string;
                origName = origNameProperty?.GetValue(payload) as string;
            }

            if (!string.IsNullOrEmpty(url) && url.StartsWith("data:"))
            {
                switch (type)
                {
                    case "pil":
                        return DecodeBase64ToImage(url);
                    case "numpy":
                        return DecodeBase64ToImageArray(url);
                    case "filepath":
                        return DecodeBase64ToFile(url, cacheDir, format);
                    default:
                        throw new ArgumentException($"Unknown type: {type}. Please choose from: 'numpy', 'pil', 'filepath'.");
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Image path is null or empty.");
            }

            // If path is still an HTTP URL (e.g. from a FileData dict that bypassed cache download),
            // download it now so ImageSharp can open it as a local file.
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                path = Core.ProcessingUtils.SaveUrlToCache(path, cacheDir);
            }

            var filePath = new FileInfo(path);
            string name = "image";
            string suffix = "webp";

            if (!string.IsNullOrEmpty(origName))
            {
                var origPath = new FileInfo(origName);
                name = origPath.Name.Replace(origPath.Extension, "");
                suffix = origPath.Extension.TrimStart('.').ToLower();
                if (suffix == "jpg") suffix = "jpeg";
            }

            if (suffix == "svg")
            {
                if (type == "filepath")
                {
                    return path;
                }
                throw new InvalidOperationException("SVG files are not supported as input images for this app.");
            }

            using (var im = Image.Load(path))
            {
                if (type == "filepath" && (string.IsNullOrEmpty(imageMode) || imageMode == im.Metadata.DecodedImageFormat.Name))
                {
                    return path;
                }

                // Handle image mode conversion if needed
                Image processedImage = im;
                if (!string.IsNullOrEmpty(imageMode) && suffix.ToLower() != "gif")
                {
                    // Convert image to specified mode (RGB, RGBA, L, etc.)
                    processedImage = ConvertImageMode(im, imageMode);
                }

                return FormatImage(processedImage, type, cacheDir, name, suffix);
            }
        }

        private static Image ConvertImageMode(Image image, string targetMode)
        {
            // Convert image to target pixel format
            // Common modes: RGB, RGBA, L (grayscale)
            return targetMode.ToUpper() switch
            {
                "RGB" => image.CloneAs<Rgb24>(),
                "RGBA" => image.CloneAs<Rgba32>(),
                "L" or "GRAYSCALE" => image.CloneAs<L8>(),
                _ => image.CloneAs<Rgba32>() // Return Rgba32 copy for unsupported modes
            };
        }

        public static object PostprocessImage(
            object value,
            string cacheDir,
            string format,
            Components.WatermarkOptions watermark = null
        )
        {
            if (value == null)
            {
                return null;
            }

            // Handle SVG files specially
            if (value is string stringValue && stringValue.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string svgContent = ExtractSvgContent(stringValue);
                    if (watermark != null && watermark.Watermark != null)
                    {
                    }
                    string origName = Path.GetFileName(stringValue);
                    string encodedSvg = Uri.EscapeDataString(svgContent);
                    return new FileData { Path = stringValue, Url = $"data:image/svg+xml,{encodedSvg}" };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to process SVG image", ex);
                }
            }

            // Apply watermark if provided
            if (watermark != null && watermark.Watermark != null)
            {
                value = AddWatermark(value, watermark);
            }

            // Save image to cache
            string savedPath = SaveImage(value, cacheDir, format);
            string origNameFromPath = File.Exists(savedPath) ? Path.GetFileName(savedPath) : null;

            // Return FileData with path and original name
            return new FileData { Path = savedPath, OrigName = origNameFromPath };
        }
    }
}

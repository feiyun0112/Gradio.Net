using System.Net;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Gradio.Net.Core;

/// <summary>///</summary>
public static class ProcessingUtils
{
    private static readonly List<string> PUBLIC_HOSTNAME_WHITELIST = new List<string>
        {
            "hf.co",
            "huggingface.co",
            "*.hf.co",
            "*.huggingface.co"
        };

    #region GENERAL

    /// <summary>///</summary>
    public static byte[] ToBinary(object x)
    {
        string base64str;

        if (x is Dictionary<string, object> dict)
        {
            if (dict.TryGetValue("data", out var dataValue) && dataValue != null)
            {
                base64str = dataValue.ToString();
            }
            else if (dict.TryGetValue("path", out var pathValue) && pathValue != null)
            {
                base64str = EncodeUrlOrFileToBase64(pathValue.ToString());
            }
            else
            {
                throw new ArgumentException("Dictionary must contain either 'data' or 'path' key");
            }
        }
        else if (x is string str)
        {
            base64str = str;
        }
        else
        {
            throw new ArgumentException("Input must be a string or dictionary");
        }

        return Convert.FromBase64String(ExtractBase64Data(base64str));
    }

    /// <summary>///</summary>
    public static string ExtractBase64Data(string x)
    {
        return x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Last();
    }

    /// <summary>///</summary>
    private static string EncodeUrlOrFileToBase64(string urlOrFile)
    {
        // Check if it's a URL
        if (Uri.TryCreate(urlOrFile, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            // Download the URL and convert to base64
            using (var client = new HttpClient())
            {
                var bytes = client.GetByteArrayAsync(urlOrFile).Result;
                return Convert.ToBase64String(bytes);
            }
        }
        // Check if it's a file path
        else if (File.Exists(urlOrFile))
        {
            // Read the file and convert to base64
            var bytes = File.ReadAllBytes(urlOrFile);
            return Convert.ToBase64String(bytes);
        }
        else
        {
            throw new FileNotFoundException($"File or URL not found: {urlOrFile}");
        }
    }

    #endregion

    #region IMAGE PRE-PROCESSING

    /// <summary>///</summary>
    public static string EncodePlotToBase64(object plt, string format = "png")
    {
        if (plt == null)
        {
            return string.Empty;
        }

        try
        {
            // Try to handle common chart types
            var pltType = plt.GetType();

            // Check if it has a SaveTo or SaveAs method (common in charting libraries)
            var saveMethod = pltType.GetMethod("SaveTo") ?? pltType.GetMethod("SaveAs") ?? pltType.GetMethod("Save");

            if (saveMethod != null)
            {
                using (var ms = new MemoryStream())
                {
                    // Try to invoke the save method with the stream
                    try
                    {
                        saveMethod.Invoke(plt, new object[] { ms });
                        ms.Position = 0;
                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch
                    {
                        // Try with format parameter
                        saveMethod.Invoke(plt, new object[] { ms, format });
                        ms.Position = 0;
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            // Check if it has ToImage method (like ScottPlot)
            var toImageMethod = pltType.GetMethod("ToImage");
            if (toImageMethod != null)
            {
                var image = toImageMethod.Invoke(plt, null);
                if (image != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        var saveImageMethod = image.GetType().GetMethod("Save", new[] { typeof(Stream), typeof(string) });
                        if (saveImageMethod != null)
                        {
                            saveImageMethod.Invoke(image, new object[] { ms, format });
                            ms.Position = 0;
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }

            // If all else fails, try to serialize as JSON or return empty
            return string.Empty;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    /// <summary>///</summary>
    public static byte[] GetPilExifBytes(object image)
    {
        if (image is SixLabors.ImageSharp.Image img)
        {
            try
            {
                // Try to get EXIF profile from ImageSharp metadata
                var exifProfile = img.Metadata.ExifProfile;
                if (exifProfile != null)
                {
                    // Convert EXIF profile to bytes
                    using (var stream = new MemoryStream())
                    {
                        exifProfile.ToByteArray();
                        return exifProfile.ToByteArray();
                    }
                }
            }
            catch
            {
                // If EXIF extraction fails, return null
            }
        }
        return null;
    }

    /// <summary>///</summary>
    public static object GetPilMetadata(object image)
    {
        if (image is SixLabors.ImageSharp.Image img)
        {
            // Return the ImageSharp metadata object
            return img.Metadata;
        }
        return null;
    }

    /// <summary>///</summary>
    public static byte[] EncodePilToBytes(object image, string format = "png")
    {
        if (image is SixLabors.ImageSharp.Image img)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = GetImageEncoder(format);
                img.Save(stream, encoder);
                return stream.ToArray();
            }
        }
        throw new ArgumentException("Expected ImageSharp Image object");
    }

    /// <summary>///</summary>
    private static SixLabors.ImageSharp.Formats.IImageEncoder GetImageEncoder(string format)
    {
        switch (format.ToLower())
        {
            case "png":
                return new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            case "jpg":
            case "jpeg":
                return new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
            case "webp":
                return new SixLabors.ImageSharp.Formats.Webp.WebpEncoder();
            case "gif":
                return new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
            default:
                return new SixLabors.ImageSharp.Formats.Png.PngEncoder();
        }
    }

    #endregion

    #region HASH FUNCTIONS

    /// <summary>///</summary>
    public static string HashFile(string filePath, int chunkNumBlocks = 128)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashSeed = Encoding.UTF8.GetBytes(GetHashSeed());
            sha256.TransformBlock(hashSeed, 0, hashSeed.Length, hashSeed, 0);

            using (var stream = File.OpenRead(filePath))
            {
                var buffer = new byte[chunkNumBlocks * sha256.InputBlockSize];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                }
            }

            sha256.TransformFinalBlock(new byte[0], 0, 0);
            return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
        }
    }

    /// <summary>///</summary>
    public static string HashUrl(string url)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashSeed = Encoding.UTF8.GetBytes(GetHashSeed());
            sha256.TransformBlock(hashSeed, 0, hashSeed.Length, hashSeed, 0);

            byte[] urlBytes = Encoding.UTF8.GetBytes(url);
            sha256.TransformBlock(urlBytes, 0, urlBytes.Length, urlBytes, 0);

            sha256.TransformFinalBlock(new byte[0], 0, 0);
            return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
        }
    }

    /// <summary>///</summary>
    public static string HashBytes(byte[] bytes)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashSeed = Encoding.UTF8.GetBytes(GetHashSeed());
            sha256.TransformBlock(hashSeed, 0, hashSeed.Length, hashSeed, 0);

            sha256.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
            sha256.TransformFinalBlock(new byte[0], 0, 0);

            return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
        }
    }

    /// <summary>///</summary>
    public static string HashBase64(string base64Encoding, int chunkNumBlocks = 128)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashSeed = Encoding.UTF8.GetBytes(GetHashSeed());
            sha256.TransformBlock(hashSeed, 0, hashSeed.Length, hashSeed, 0);

            byte[] base64Bytes = Encoding.UTF8.GetBytes(base64Encoding);
            int blockSize = chunkNumBlocks * sha256.InputBlockSize;

            for (int i = 0; i < base64Bytes.Length; i += blockSize)
            {
                int currentBlockSize = Math.Min(blockSize, base64Bytes.Length - i);
                sha256.TransformBlock(base64Bytes, i, currentBlockSize, base64Bytes, i);
            }

            sha256.TransformFinalBlock(new byte[0], 0, 0);
            return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
        }
    }

    /// <summary>///</summary>
    public static string GetHashSeed()
    {
        // Implement a simple hash seed generation similar to Python's utils.get_hash_seed()
        // This ensures consistency between Python and C# implementations
        return System.Environment.MachineName + System.Environment.UserName;
    }

    #endregion

    #region CACHE MANAGEMENT

    /// <summary>///</summary>
    public static string SavePilToCache(object img, string cacheDir, string name = "image", string format = "webp")
    {
        if (img is SixLabors.ImageSharp.Image image)
        {
            byte[] bytesData = EncodePilToBytes(image, format);
            string hash = HashBytes(bytesData);
            string tempDir = Path.Combine(cacheDir, hash);
            Directory.CreateDirectory(tempDir);
            string filename = Path.Combine(tempDir, $"{name}.{format}");
            filename = Path.GetFullPath(filename);

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                var encoder = GetImageEncoder(format);
                image.Save(stream, encoder);
            }

            return filename;
        }
        throw new ArgumentException("Expected ImageSharp Image object");
    }

    /// <summary>///</summary>
    public static string SaveImgArrayToCache(object arr, string cacheDir, string format = "webp")
    {
        if (arr is byte[] byteArray)
        {
            // Convert byte array to ImageSharp image
            using (var stream = new MemoryStream(byteArray))
            {
                var image = SixLabors.ImageSharp.Image.Load(stream);
                return SavePilToCache(image, cacheDir, format: format);
            }
        }
        throw new ArgumentException("Expected byte array for image");
    }

    /// <summary>///</summary>
    public static string SaveAudioToCache(object data, int sampleRate, string format, string cacheDir)
    {
        if (data is float[] floatArray)
        {
            byte[] byteData = ConvertFloatArrayToBytes(floatArray);
            string hash = HashBytes(byteData);
            string tempDir = Path.Combine(cacheDir, hash);
            Directory.CreateDirectory(tempDir);
            string filename = Path.Combine(tempDir, $"audio.{format}");
            filename = Path.GetFullPath(filename);

            // For now, we'll just save the raw bytes
            // In a real implementation, we would use a library like NAudio to properly save audio files
            File.WriteAllBytes(filename, byteData);

            return filename;
        }
        throw new ArgumentException("Expected float array for audio data");
    }

    /// <summary>///</summary>
    private static byte[] ConvertFloatArrayToBytes(float[] floatArray)
    {
        byte[] byteArray = new byte[floatArray.Length * sizeof(float)];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    /// <summary>///</summary>
    public static string DetectAudioFormat(byte[] data)
    {
        if (data.Length >= 12 &&
            data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F' &&
            data[8] == 'W' && data[9] == 'A' && data[10] == 'V' && data[11] == 'E')
        {
            return ".wav";
        }
        else if (data.Length >= 3 && data[0] == 'I' && data[1] == 'D' && data[2] == '3')
        {
            return ".mp3";
        }
        else if (data.Length >= 2 && data[0] == 0xff && data[1] == 0xfb)
        {
            return ".mp3";
        }
        return "";
    }

    /// <summary>///</summary>
    public static string SaveBytesToCache(byte[] data, string fileName, string cacheDir)
    {
        string hash = HashBytes(data);
        string cachePath = Path.Combine(cacheDir, hash);
        Directory.CreateDirectory(cachePath);

        if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
        {
            string detectedExtension = DetectAudioFormat(data);
            fileName += detectedExtension;
        }

        string filePath = Path.Combine(cachePath, Path.GetFileName(fileName));
        File.WriteAllBytes(filePath, data);

        return Path.GetFullPath(filePath);
    }

    /// <summary>///</summary>
    public static string SaveFileToCache(string filePath, string cacheDir)
    {
        string hash = HashFile(filePath);
        string cachePath = Path.Combine(cacheDir, hash);
        Directory.CreateDirectory(cachePath);

        string fileName = Path.GetFileName(filePath);
        string cacheFilePath = Path.Combine(cachePath, fileName);
        cacheFilePath = Path.GetFullPath(cacheFilePath);

        if (!File.Exists(cacheFilePath))
        {
            File.Copy(filePath, cacheFilePath, true);
        }

        return cacheFilePath;
    }

    /// <summary>///</summary>
    public static bool IsPublicIp(string ip)
    {
        try
        {
            var ipObj = System.Net.IPAddress.Parse(ip);

            // Check if IP is private, loopback, link-local, multicast, or reserved
            return !(ipObj.IsPrivate() || System.Net.IPAddress.IsLoopback(ipObj) ||
                     ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && IsIPv4LinkLocal(ipObj) ||
                     ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && (ipObj.IsIPv6LinkLocal || ipObj.IsIPv6SiteLocal) ||
                     ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && ipObj.IsIPv6Multicast);
        }
        catch (System.FormatException)
        {
            return false;
        }
    }

    /// <summary>///</summary>
    public static Func<Task<T>> LruCacheAsync<T>(int maxSize = 128)
    {
        // This is a simplified implementation. In a real-world scenario,
        // you would use a proper LRU cache implementation
        var cache = new Dictionary<string, T>();
        var keys = new List<string>();

        return async () =>
        {
            // This is a placeholder. A real implementation would
            // generate a cache key based on the method parameters
            string key = "default";

            if (cache.ContainsKey(key))
            {
                // Move to front of list
                keys.Remove(key);
                keys.Insert(0, key);
                return cache[key];
            }

            // This is a placeholder. In a real implementation,
            // you would call the original async function
            T result = default;

            // Add to cache
            cache[key] = result;
            keys.Insert(0, key);

            // Evict if over capacity
            if (keys.Count > maxSize)
            {
                string evictKey = keys[keys.Count - 1];
                keys.RemoveAt(keys.Count - 1);
                cache.Remove(evictKey);
            }

            return result;
        };
    }



    /// <summary>///</summary>
    private static bool IsIPv4LinkLocal(System.Net.IPAddress ipAddress)
    {
        if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            return false;

        byte[] bytes = ipAddress.GetAddressBytes();
        return bytes[0] == 169 && bytes[1] == 254; // 169.254.0.0/16
    }

    /// <summary>///</summary>
    public static async Task<string> AsyncSsrfProtectedDownload(string url, string cacheDir)
    {
        string hash = HashUrl(url);
        string tempDir = Path.Combine(cacheDir, hash);
        Directory.CreateDirectory(tempDir);

        Uri parsedUrl = new Uri(url);
        string basePath = parsedUrl.AbsolutePath.TrimEnd('/');
        string filename = Path.GetFileName(basePath) ?? "file";
        filename = StripInvalidFilenameCharacters(filename);

        string fullTempFilePath = Path.Combine(tempDir, filename);
        fullTempFilePath = Path.GetFullPath(fullTempFilePath);

        if (File.Exists(fullTempFilePath))
        {
            return fullTempFilePath;
        }

        string hostname = parsedUrl.Host;

        // Check if hostname is in whitelist
        bool isWhitelisted = false;
        foreach (string whitelistEntry in PUBLIC_HOSTNAME_WHITELIST)
        {
            if (whitelistEntry.StartsWith("*"))
            {
                // Wildcard match
                string domain = whitelistEntry.Substring(1);
                if (hostname.EndsWith(domain))
                {
                    isWhitelisted = true;
                    break;
                }
            }
            else
            {
                // Exact match
                if (hostname == whitelistEntry)
                {
                    isWhitelisted = true;
                    break;
                }
            }
        }

        if (!isWhitelisted)
        {
            // Check if IP is public
            IPAddress ipAddress;
            if (IPAddress.TryParse(hostname, out ipAddress))
            {
                if (!IsPublicIp(ipAddress.ToString()))
                {
                    throw new InvalidOperationException($"Cannot download from private IP address: {ipAddress}");
                }
            }
            else
            {
                // Resolve hostname to IP and check
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(hostname);
                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    if (!IsPublicIp(ip.ToString()))
                    {
                        throw new InvalidOperationException($"Cannot download from private IP address: {ip}");
                    }
                }
            }
        }

        using (var client = new HttpClient())
        {
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                await using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await using (var fileStream = new FileStream(fullTempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
        }

        return fullTempFilePath;
    }

    /// <summary>///</summary>
    public static string UnsafeDownload(string url, string cacheDir)
    {
        string hash = HashUrl(url);
        string tempDir = Path.Combine(cacheDir, hash);
        Directory.CreateDirectory(tempDir);

        string filename = Path.GetFileName(url);
        filename = string.IsNullOrEmpty(filename) ? "file" : StripInvalidFilenameCharacters(filename);
        string fullTempFilePath = Path.Combine(tempDir, filename);
        fullTempFilePath = Path.GetFullPath(fullTempFilePath);

        using (var client = new HttpClient())
        {
            using (var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
            {
                response.EnsureSuccessStatusCode();
                using (var contentStream = response.Content.ReadAsStreamAsync().Result)
                {
                    using (var fileStream = new FileStream(fullTempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        contentStream.CopyTo(fileStream);
                    }
                }
            }
        }

        long fileSize = new FileInfo(fullTempFilePath).Length;
        // Log implementation would go here

        return fullTempFilePath;
    }

    /// <summary>///</summary>
    public static string SsrfProtectedDownload(string url, string cacheDir)
    {
        return AsyncSsrfProtectedDownload(url, cacheDir).Result;
    }

    /// <summary>///</summary>
    public static string SaveUrlToCache(string url, string cacheDir)
    {
        return SsrfProtectedDownload(url, cacheDir);
    }

    /// <summary>///</summary>
    public static string SaveBase64ToCache(string base64Encoding, string cacheDir, string fileName = null)
    {
        string tempDir = HashBase64(base64Encoding);
        tempDir = Path.Combine(cacheDir, tempDir);
        Directory.CreateDirectory(tempDir);

        string guessExtension = GetExtension(base64Encoding);
        if (!string.IsNullOrEmpty(fileName))
        {
            fileName = StripInvalidFilenameCharacters(fileName);
        }
        else if (!string.IsNullOrEmpty(guessExtension))
        {
            fileName = $"file.{guessExtension}";
        }
        else
        {
            fileName = "file";
        }

        string fullTempFilePath = Path.Combine(tempDir, fileName);
        fullTempFilePath = Path.GetFullPath(fullTempFilePath);

        if (!File.Exists(fullTempFilePath))
        {
            byte[] data = Convert.FromBase64String(ExtractBase64Data(base64Encoding));
            File.WriteAllBytes(fullTempFilePath, data);
        }

        return fullTempFilePath;
    }

    /// <summary>///</summary>
    private static string GetExtension(string base64Encoding)
    {
        // Extract the MIME type from the base64 string if present
        int commaIndex = base64Encoding.IndexOf(',');
        if (commaIndex > 0)
        {
            string header = base64Encoding.Substring(0, commaIndex);
            if (header.Contains("image/png")) return "png";
            if (header.Contains("image/jpeg")) return "jpg";
            if (header.Contains("image/gif")) return "gif";
            if (header.Contains("image/webp")) return "webp";
            if (header.Contains("audio/wav")) return "wav";
            if (header.Contains("audio/mp3")) return "mp3";
            if (header.Contains("video/mp4")) return "mp4";
            if (header.Contains("application/pdf")) return "pdf";
        }
        return "";
    }

    /// <summary>///</summary>
    private static string StripInvalidFilenameCharacters(string fileName)
    {
        // Remove characters that are invalid in Windows filenames
        string invalidChars = new string(System.IO.Path.GetInvalidFileNameChars());
        string invalidRegStr = string.Format("[{0}]", System.Text.RegularExpressions.Regex.Escape(invalidChars));
        return System.Text.RegularExpressions.Regex.Replace(fileName, invalidRegStr, "");
    }

    #endregion

    #region FILE MANAGEMENT

    /// <summary>///</summary>
    public static void CheckAllFilesInCache(object data)
    {
        void _inCache(Dictionary<string, object> d)
        {
            if (d.TryGetValue("path", out var pathValue) && pathValue is string path)
            {
                if (!IsHttpUrlLike(path) && !IsInOrEqual(path, GetUploadFolder()))
                {
                    throw new Core.Exceptions.Error($"File {path} is not in the cache folder and cannot be accessed.");
                }
            }
        }

        Traverse(data, _inCache, IsFileObj);
    }

    /// <summary>///</summary>
    private static void Traverse(object data, Action<Dictionary<string, object>> action, Func<Dictionary<string, object>, bool> isFileObj)
    {
        if (data is Dictionary<string, object> dict)
        {
            if (isFileObj(dict))
            {
                action(dict);
            }
            else
            {
                foreach (var kvp in dict)
                {
                    Traverse(kvp.Value, action, isFileObj);
                }
            }
        }
        else if (data is List<object> list)
        {
            foreach (var item in list)
            {
                Traverse(item, action, isFileObj);
            }
        }
        else if (data is object[] array)
        {
            foreach (var item in array)
            {
                Traverse(item, action, isFileObj);
            }
        }
    }

    /// <summary>///</summary>
    private static bool IsFileObj(Dictionary<string, object> dict)
    {
        return dict.ContainsKey("path") || dict.ContainsKey("url");
    }

    /// <summary>///</summary>
    private static bool IsInOrEqual(string path, string targetPath)
    {
        try
        {
            var fullPath = Path.GetFullPath(path);
            var fullTargetPath = Path.GetFullPath(targetPath);
            return fullPath.StartsWith(fullTargetPath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>///</summary>
    private static string GetUploadFolder()
    {
        // This should be implemented to return the actual upload folder path
        // For now, we'll return a temporary directory
        return Path.GetTempPath();
    }



    /// <summary>///</summary>
    public static object MoveFilesToCache(object data, object block, bool postprocess = false, bool checkInUploadFolder = false, bool keepInCache = false)
    {
        if (data is Dictionary<string, object> dict)
        {
            return TraverseDictForFiles(dict, block, postprocess, checkInUploadFolder, keepInCache);
        }
        else if (data is List<object> list)
        {
            return TraverseListForFiles(list, block, postprocess, checkInUploadFolder, keepInCache);
        }
        return data;
    }

    /// <summary>///</summary>
    private static Dictionary<string, object> TraverseDictForFiles(Dictionary<string, object> dict, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new Dictionary<string, object>();
        bool isFileObj = IsFileObjWithMeta(dict);

        foreach (var kvp in dict)
        {
            if (isFileObj)
            {
                // This is a file object, process it
                result = ProcessFileObject(dict, block, postprocess, checkInUploadFolder, keepInCache);
                break;
            }
            else if (kvp.Value is Dictionary<string, object> nestedDict)
            {
                result[kvp.Key] = TraverseDictForFiles(nestedDict, block, postprocess, checkInUploadFolder, keepInCache);
            }
            else if (kvp.Value is List<object> list)
            {
                result[kvp.Key] = TraverseListForFiles(list, block, postprocess, checkInUploadFolder, keepInCache);
            }
            else
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static List<object> TraverseListForFiles(List<object> list, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new List<object>();
        foreach (var item in list)
        {
            if (item is Dictionary<string, object> dict)
            {
                result.Add(TraverseDictForFiles(dict, block, postprocess, checkInUploadFolder, keepInCache));
            }
            else if (item is List<object> nestedList)
            {
                result.Add(TraverseListForFiles(nestedList, block, postprocess, checkInUploadFolder, keepInCache));
            }
            else
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static bool IsFileObjWithMeta(Dictionary<string, object> dict)
    {
        // A file object typically has either "path" or "url" keys
        return dict.ContainsKey("path") || dict.ContainsKey("url");
    }

    /// <summary>///</summary>
    private static Dictionary<string, object> ProcessFileObject(Dictionary<string, object> fileObj, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new Dictionary<string, object>(fileObj);

        // Check if this is a file object with URL from postprocess
        if (result.TryGetValue("url", out var urlValue) && postprocess && urlValue is string url && IsHttpUrlLike(url))
        {
            result["path"] = url;
            return result;
        }

        // Check if it's a static file
        if (IsStaticFile(result))
        {
            return result;
        }

        // Get block properties using reflection
        string proxyUrl = GetBlockProperty<string>(block, "proxy_url");

        if (string.IsNullOrEmpty(proxyUrl))
        {
            // If the file is on a remote server, do not move it to cache
            if (result.TryGetValue("path", out var pathValue) && pathValue is string filePath && !IsHttpUrlLike(filePath))
            {
                CheckAllowed(filePath, checkInUploadFolder);
            }

            // Check if it's a stream
            bool isStream = result.TryGetValue("is_stream", out var isStreamValue) && isStreamValue is bool && (bool)isStreamValue;

            if (!isStream)
            {
                // Move resource to block cache
                string tempFilePath = null;
                if (result.TryGetValue("path", out var pathVal) && pathVal is string path)
                {
                    tempFilePath = MoveResourceToBlockCache(path, block);
                }
                else if (result.TryGetValue("url", out var urlVal) && urlVal is string urlPath)
                {
                    tempFilePath = MoveResourceToBlockCache(urlPath, block);
                }

                if (tempFilePath != null)
                {
                    result["path"] = tempFilePath;

                    if (keepInCache)
                    {
                        // Add to keep_in_cache set
                        var keepInCacheSet = GetBlockProperty<HashSet<string>>(block, "keep_in_cache");
                        if (keepInCacheSet != null)
                        {
                            keepInCacheSet.Add(tempFilePath);
                        }
                    }
                }
            }
        }

        // Add URL prefix
        string urlPrefix = result.TryGetValue("is_stream", out var streamVal) && streamVal is bool && (bool)streamVal
            ? $"{RouteUtils.ApiPrefix}/stream/" : $"{RouteUtils.ApiPrefix}/file=";

        if (!string.IsNullOrEmpty(proxyUrl))
        {
            proxyUrl = proxyUrl.TrimEnd('/');
            result["url"] = $"/api/proxy={proxyUrl}{urlPrefix}{result.GetValueOrDefault("path", "")}";
        }
        else if (result.TryGetValue("path", out var pathValue2) && pathValue2 is string path2 && (IsHttpUrlLike(path2) || path2.StartsWith(urlPrefix)))
        {
            result["url"] = path2;
        }
        else
        {
            result["url"] = $"{urlPrefix}{result.GetValueOrDefault("path", "")}";
        }

        // Mark SVG as safe if needed
        if (result.TryGetValue("path", out var svgPathValue) && svgPathValue is string svgPath)
        {
            MarkSvgAsSafe(svgPath);
        }

        return result;
    }

    /// <summary>///</summary>
    private static bool IsStaticFile(Dictionary<string, object> fileObj)
    {
        // Simplified implementation - in a real scenario, you would check if the file is in a static directory
        return false;
    }

    /// <summary>///</summary>
    private static void CheckAllowed(string path, bool checkInUploadFolder)
    {
        // Simplified implementation - in a real scenario, you would check if the path is allowed
        // This would involve checking against blocked paths, allowed paths, etc.
    }

    /// <summary>///</summary>
    public static string MoveResourceToBlockCache(string urlOrFilePath, object block)
    {
        // Simplified implementation - in a real scenario, you would call the block's move_resource_to_block_cache method
        // For now, we'll just return the path as-is
        return urlOrFilePath;
    }

    /// <summary>///</summary>
    private static void MarkSvgAsSafe(string path)
    {
        // Simplified implementation - in a real scenario, you would set the path as a static path
        // This is mainly so that svg files can be displayed inline for button/chatbot icons
    }

    /// <summary>///</summary>
    private static T GetBlockProperty<T>(object block, string propertyName)
    {
        if (block == null)
            return default;

        var property = block.GetType().GetProperty(propertyName);
        if (property == null)
            return default;

        var value = property.GetValue(block);
        return value is T ? (T)value : default;
    }

    /// <summary>///</summary>
    public static async Task<object> AsyncMoveFilesToCache(object data, object block, bool postprocess = false, bool checkInUploadFolder = false, bool keepInCache = false)
    {
        if (data is Dictionary<string, object> dict)
        {
            return await TraverseDictAsync(dict, block, postprocess, checkInUploadFolder, keepInCache);
        }
        else if (data is List<object> list)
        {
            return await TraverseListAsync(list, block, postprocess, checkInUploadFolder, keepInCache);
        }
        else if (data is object[] array)
        {
            var result = new List<object>();
            foreach (var item in array)
            {
                var processedItem = await AsyncMoveFilesToCache(item, block, postprocess, checkInUploadFolder, keepInCache);
                result.Add(processedItem);
            }
            return result.ToArray();
        }
        return data;
    }

    /// <summary>///</summary>
    private static async Task<Dictionary<string, object>> TraverseDictAsync(Dictionary<string, object> dict, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new Dictionary<string, object>();
        bool isFileObj = IsFileObjWithMeta(dict);

        foreach (var kvp in dict)
        {
            if (isFileObj)
            {
                // This is a file object, process it
                result = await ProcessFileObjectAsync(dict, block, postprocess, checkInUploadFolder, keepInCache);
                break;
            }
            else if (kvp.Value is Dictionary<string, object> nestedDict)
            {
                result[kvp.Key] = await TraverseDictAsync(nestedDict, block, postprocess, checkInUploadFolder, keepInCache);
            }
            else if (kvp.Value is List<object> list)
            {
                result[kvp.Key] = await TraverseListAsync(list, block, postprocess, checkInUploadFolder, keepInCache);
            }
            else if (kvp.Value is object[] array)
            {
                var processedArray = new List<object>();
                foreach (var item in array)
                {
                    var processedItem = await AsyncMoveFilesToCache(item, block, postprocess, checkInUploadFolder, keepInCache);
                    processedArray.Add(processedItem);
                }
                result[kvp.Key] = processedArray.ToArray();
            }
            else
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static async Task<List<object>> TraverseListAsync(List<object> list, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new List<object>();
        foreach (var item in list)
        {
            if (item is Dictionary<string, object> dict)
            {
                result.Add(await TraverseDictAsync(dict, block, postprocess, checkInUploadFolder, keepInCache));
            }
            else if (item is List<object> nestedList)
            {
                result.Add(await TraverseListAsync(nestedList, block, postprocess, checkInUploadFolder, keepInCache));
            }
            else if (item is object[] array)
            {
                var processedArray = new List<object>();
                foreach (var arrayItem in array)
                {
                    var processedItem = await AsyncMoveFilesToCache(arrayItem, block, postprocess, checkInUploadFolder, keepInCache);
                    processedArray.Add(processedItem);
                }
                result.Add(processedArray.ToArray());
            }
            else
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static async Task<Dictionary<string, object>> ProcessFileObjectAsync(Dictionary<string, object> fileObj, object block, bool postprocess, bool checkInUploadFolder, bool keepInCache)
    {
        var result = new Dictionary<string, object>(fileObj);

        // Check if this is a file object with URL from postprocess
        if (result.TryGetValue("url", out var urlValue) && postprocess && urlValue is string url && IsHttpUrlLike(url))
        {
            result["path"] = url;
            return result;
        }

        // Check if it's a static file
        if (IsStaticFile(result))
        {
            return result;
        }

        // Get block properties using reflection
        string proxyUrl = GetBlockProperty<string>(block, "proxy_url");

        if (string.IsNullOrEmpty(proxyUrl))
        {
            // If the file is on a remote server, do not move it to cache
            if (result.TryGetValue("path", out var pathValue) && pathValue is string filePath && !IsHttpUrlLike(filePath))
            {
                CheckAllowed(filePath, checkInUploadFolder);
            }

            // Check if it's a stream
            bool isStream = result.TryGetValue("is_stream", out var isStreamValue) && isStreamValue is bool && (bool)isStreamValue;

            if (!isStream)
            {
                // Move resource to block cache
                string tempFilePath = null;
                if (result.TryGetValue("path", out var pathVal) && pathVal is string path)
                {
                    tempFilePath = await AsyncMoveResourceToBlockCache(path, block);
                }
                else if (result.TryGetValue("url", out var urlVal) && urlVal is string urlPath)
                {
                    tempFilePath = await AsyncMoveResourceToBlockCache(urlPath, block);
                }

                if (tempFilePath != null)
                {
                    result["path"] = tempFilePath;

                    if (keepInCache)
                    {
                        // Add to keep_in_cache set
                        var keepInCacheSet = GetBlockProperty<HashSet<string>>(block, "keep_in_cache");
                        if (keepInCacheSet != null)
                        {
                            keepInCacheSet.Add(tempFilePath);
                        }
                    }
                }
            }
        }

        // Add URL prefix
        string urlPrefix = result.TryGetValue("is_stream", out var streamVal) && streamVal is bool && (bool)streamVal
            ? $"{RouteUtils.ApiPrefix}/stream/" : $"{RouteUtils.ApiPrefix}/file=";

        if (!string.IsNullOrEmpty(proxyUrl))
        {
            proxyUrl = proxyUrl.TrimEnd('/');
            result["url"] = $"/api/proxy={proxyUrl}{urlPrefix}{result.GetValueOrDefault("path", "")}";
        }
        else if (result.TryGetValue("path", out var pathValue2) && pathValue2 is string path2 && (IsHttpUrlLike(path2) || path2.StartsWith(urlPrefix)))
        {
            result["url"] = path2;
        }
        else
        {
            result["url"] = $"{urlPrefix}{result.GetValueOrDefault("path", "")}";
        }

        // Mark SVG as safe if needed
        if (result.TryGetValue("path", out var svgPathValue) && svgPathValue is string svgPath)
        {
            MarkSvgAsSafe(svgPath);
        }

        return result;
    }

    /// <summary>///</summary>
    private static async Task<string> AsyncMoveResourceToBlockCache(string urlOrFilePath, object block)
    {
        if (string.IsNullOrEmpty(urlOrFilePath))
            return urlOrFilePath;

        // Download remote URLs to the local block cache (mirrors Python's async_move_resource_to_block_cache).
        if (urlOrFilePath.StartsWith("http://") || urlOrFilePath.StartsWith("https://"))
        {
            try
            {
                // Resolve cache directory from the block (falls back to system temp).
                string cacheDir = null;
                var cacheProp = block?.GetType().GetProperty("GRADIO_CACHE");
                if (cacheProp != null)
                    cacheDir = cacheProp.GetValue(block) as string;
                if (string.IsNullOrEmpty(cacheDir))
                    cacheDir = Utils.Utils.GetUploadFolder();

                var localPath = await Task.Run(() => SaveUrlToCache(urlOrFilePath, cacheDir));

                // Register in TempFiles so the block tracks it.
                var tempFilesProp = block?.GetType().GetProperty("TempFiles");
                if (tempFilesProp?.GetValue(block) is System.Collections.Generic.HashSet<string> tempFiles)
                    tempFiles.Add(localPath);

                return localPath;
            }
            catch
            {
                // On download failure return the original URL so the caller can surface the error.
                return urlOrFilePath;
            }
        }

        return await Task.FromResult(urlOrFilePath);
    }

    /// <summary>///</summary>
    public static object AddRootUrl(object data, string rootUrl, string previousRootUrl = null)
    {
        if (data is Dictionary<string, object> dict)
        {
            return TraverseDict(dict, rootUrl, previousRootUrl);
        }
        else if (data is List<object> list)
        {
            return TraverseList(list, rootUrl, previousRootUrl);
        }
        return data;
    }

    /// <summary>///</summary>
    private static Dictionary<string, object> TraverseDict(Dictionary<string, object> dict, string rootUrl, string previousRootUrl = null)
    {
        var result = new Dictionary<string, object>();
        bool isFileObj = IsFileObjWithUrl(dict);

        foreach (var kvp in dict)
        {
            if (isFileObj && kvp.Key == "url" && kvp.Value is string url)
            {
                // This is a file object's url, add root URL
                if (!string.IsNullOrEmpty(previousRootUrl) && url.StartsWith(previousRootUrl))
                {
                    url = url.Substring(previousRootUrl.Length);
                    result[kvp.Key] = rootUrl + url;
                }
                else if (IsHttpUrlLike(url))
                {
                    result[kvp.Key] = kvp.Value;
                }
                else
                {
                    result[kvp.Key] = rootUrl + url;
                }
            }
            else if (kvp.Value is Dictionary<string, object> nestedDict)
            {
                result[kvp.Key] = TraverseDict(nestedDict, rootUrl, previousRootUrl);
            }
            else if (kvp.Value is List<object> list)
            {
                result[kvp.Key] = TraverseList(list, rootUrl, previousRootUrl);
            }
            else
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static List<object> TraverseList(List<object> list, string rootUrl, string previousRootUrl = null)
    {
        var result = new List<object>();
        foreach (var item in list)
        {
            if (item is Dictionary<string, object> dict)
            {
                result.Add(TraverseDict(dict, rootUrl, previousRootUrl));
            }
            else if (item is List<object> nestedList)
            {
                result.Add(TraverseList(nestedList, rootUrl, previousRootUrl));
            }
            else
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>///</summary>
    private static bool IsFileObjWithUrl(Dictionary<string, object> obj)
    {
        return obj.ContainsKey("url") && obj["url"] is string;
    }

    /// <summary>///</summary>
    private static bool IsHttpUrlLike(string url)
    {
        return url.StartsWith("http://") || url.StartsWith("https://");
    }

    /// <summary>///</summary>
    public static object ResizeAndCrop(object img, Tuple<int?, int?> size, string cropType = "center")
    {
        if (img is SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image)
        {
            int width = size.Item1 ?? image.Width;
            int height = size.Item2 ?? image.Height;

            // Determine anchor position based on crop type
            SixLabors.ImageSharp.Processing.AnchorPositionMode anchorPosition;
            switch (cropType.ToLower())
            {
                case "top":
                    anchorPosition = SixLabors.ImageSharp.Processing.AnchorPositionMode.Top;
                    break;
                case "bottom":
                    anchorPosition = SixLabors.ImageSharp.Processing.AnchorPositionMode.Bottom;
                    break;
                case "center":
                default:
                    anchorPosition = SixLabors.ImageSharp.Processing.AnchorPositionMode.Center;
                    break;
            }

            // Clone and process the image
            var resultImage = image.Clone();
            resultImage.Mutate(ctx =>
            {
                // Use ImageSharp's Resize operation with Crop mode
                var options = new SixLabors.ImageSharp.Processing.ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(width, height),
                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop,
                    Position = anchorPosition
                };
                ctx.Resize(options);
            });

            return resultImage;
        }
        else if (img is SixLabors.ImageSharp.Image nonGenericImage)
        {
            // Try to handle non-generic Image type
            // Convert to Rgba32 and process
            var rgba32Image = nonGenericImage as SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;
            if (rgba32Image == null)
            {
                // Cannot convert, return original
                return nonGenericImage;
            }
            return ResizeAndCrop(rgba32Image, size, cropType);
        }
        throw new ArgumentException("Expected ImageSharp Image object");
    }

    #endregion

    #region AUDIO PROCESSING

    /// <summary>///</summary>
    public static Tuple<int, float[]> AudioFromFile(string filename, float cropMin = 0, float cropMax = 100)
    {
        try
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File not found: {filename}");
            }

            // This is a simplified implementation
            // In a real-world scenario, you would use a library like NAudio
            // to properly load audio files

            // For now, we'll return a placeholder with 44.1kHz sample rate
            // and empty data
            int sampleRate = 44100;
            float[] data = new float[0];

            return Tuple.Create(sampleRate, data);
        }
        catch (FileNotFoundException e)
        {
            bool isFile = File.Exists(filename);
            string msg = $"Cannot load audio from file: {(isFile ? "ffprobe" : filename)} not found."
                + " Please install ffmpeg in your system to use non-WAV audio file formats"
                + " and make sure ffprobe is in your PATH.";
            throw new Exception(msg, e);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /// <summary>///</summary>
    public static void AudioToFile(int sampleRate, float[] data, string filename, string format = "wav")
    {
        if (format == "wav")
        {
            data = ConvertTo16BitWav(data);
        }

        // This is a simplified implementation
        // In a real-world scenario, you would use a library like NAudio
        // to properly save audio files

        // For now, we'll just save the raw bytes
        byte[] byteData = ConvertFloatArrayToBytes(data);
        File.WriteAllBytes(filename, byteData);
    }

    /// <summary>///</summary>
    public static float[] ConvertTo16BitWav(float[] data)
    {
        float[] result = new float[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            // Clamp to [-1, 1]
            float value = Math.Max(-1.0f, Math.Min(1.0f, data[i]));
            // Convert to 16-bit range [-32768, 32767]
            result[i] = value * 32767.0f;
        }
        return result;
    }

    #endregion

    #region OUTPUT

    /// <summary>///</summary>
    public static object ConvertImage(object image, Type dtype, bool forceCopy = false, bool uniform = false)
    {
        if (image is SixLabors.ImageSharp.Image img)
        {
            // For ImageSharp, we handle pixel format conversions
            // This is a simplified version since ImageSharp handles most conversions internally

            if (forceCopy || dtype != image.GetType())
            {
                // Create a copy with the appropriate pixel format
                using (var stream = new MemoryStream())
                {
                    // Determine encoder based on target type
                    var encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                    img.Save(stream, encoder);
                    stream.Position = 0;

                    // Load back with the same format
                    var newImage = SixLabors.ImageSharp.Image.Load(stream);
                    return newImage;
                }
            }
            return img;
        }
        else if (image is byte[] byteArray)
        {
            // Convert byte array to ImageSharp image
            using (var stream = new MemoryStream(byteArray))
            {
                var imageSharpImage = SixLabors.ImageSharp.Image.Load(stream);
                return ConvertImage(imageSharpImage, dtype, forceCopy, uniform);
            }
        }
        else if (image is Array array)
        {
            // Handle multi-dimensional arrays (similar to numpy arrays)
            // This is a basic implementation for common cases
            return ConvertArrayImage(array, dtype, forceCopy, uniform);
        }

        throw new ArgumentException("Expected ImageSharp Image, byte array, or multi-dimensional array");
    }

    /// <summary>///</summary>
    private static object ConvertArrayImage(Array array, Type dtype, bool forceCopy, bool uniform)
    {
        // This is a simplified implementation
        // For full numpy-like functionality, consider using a library like NumSharp

        if (array.GetType().GetElementType() == dtype && !forceCopy)
        {
            return array;
        }

        // Create a new array with the target type
        var length = array.Length;
        var targetArray = Array.CreateInstance(dtype, length);

        // Copy and convert elements
        for (int i = 0; i < length; i++)
        {
            var value = array.GetValue(i);
            var convertedValue = Convert.ChangeType(value, dtype);
            targetArray.SetValue(convertedValue, i);
        }

        return targetArray;
    }

    #endregion

    #region IP ADDRESS EXTENSIONS

    /// <summary>///</summary>
    public static bool IsPrivate(this System.Net.IPAddress ipAddress)
    {
        // Check for private IPv4 addresses
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            return (
                (bytes[0] == 10) || // 10.0.0.0/8
                (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) || // 172.16.0.0/12
                (bytes[0] == 192 && bytes[1] == 168) // 192.168.0.0/16
            );
        }
        // Check for private IPv6 addresses
        else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return ipAddress.IsIPv6UniqueLocal;
        }
        return false;
    }

    #endregion

    #region VIDEO PROCESSING

    /// <summary>///</summary>
    public static bool FfmpegInstalled()
    {
        try
        {
            // Try to run ffmpeg -version to check if it's installed
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit(1000); // Wait up to 1 second
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>///</summary>
    public static bool VideoIsPlayable(string videoFilepath)
    {
        try
        {
            string container = Path.GetExtension(videoFilepath).ToLower();

            // Check if container is in our list of playable formats
            if (!new[] { ".mp4", ".webm", ".ogg" }.Contains(container))
            {
                return false;
            }

            // For a complete implementation, we would use ffprobe to check the codec
            // For now, we'll assume it's playable if the container is correct
            return true;
        }
        catch
        {
            // If anything goes wrong, assume the video can be played
            return true;
        }
    }

    /// <summary>///</summary>
    public static string ConvertVideoToPlayableMp4(string videoPath)
    {
        try
        {
            if (!FfmpegInstalled())
            {
                return videoPath;
            }

            string outputPath = Path.ChangeExtension(videoPath, ".mp4");
            string tempFile = Path.GetTempFileName();

            // Copy original file to temp
            File.Copy(videoPath, tempFile, true);

            // Run ffmpeg to convert to mp4
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-y -loglevel quiet -i \"{tempFile}\" \"{outputPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return videoPath;
            }

            return outputPath;
        }
        catch (Exception e)
        {
            return videoPath;
        }
        finally
        {
            // Clean up temp files if any
        }
    }

    /// <summary>///</summary>
    public static float GetVideoLength(string videoPath)
    {
        try
        {
            if (!FfmpegInstalled())
            {
                throw new Exception("ffmpeg is not installed");
            }

            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = $"-i \"{videoPath}\" -show_entries format=duration -v quiet -of csv=p=0",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            float duration = float.Parse(output.Trim());
            return duration;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting video length", e);
        }
    }

    public static bool HasAllowedExtension(string filePath, IEnumerable<string> allowedExtensions)
    {
        if (string.IsNullOrEmpty(filePath) || allowedExtensions == null)
            return false;

        var extension = Path.GetExtension(filePath)?.ToLowerInvariant().TrimStart('.');
        return allowedExtensions.Any(ext => ext.ToLowerInvariant().TrimStart('.') == extension);
    }

    public static string GetMimeType(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return "application/octet-stream";

        var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

        return extension switch
        {
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".pdf" => "application/pdf",
            ".zip" => "application/zip",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".avi" => "video/x-msvideo",
            ".csv" => "text/csv",
            ".md" => "text/markdown",
            _ => "application/octet-stream"
        };
    }

    public static bool IsFileSizeValid(string filePath, long maxSizeBytes)
    {
        if (!File.Exists(filePath))
            return false;

        var fileInfo = new FileInfo(filePath);
        return fileInfo.Length <= maxSizeBytes;
    }

    public static string GenerateUniqueFilename(string directory, string filename)
    {
        var fullPath = Path.Combine(directory, filename);

        if (!File.Exists(fullPath))
            return fullPath;

        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
        var extension = Path.GetExtension(filename);
        int counter = 1;

        do
        {
            filename = $"{nameWithoutExt}_{counter}{extension}";
            fullPath = Path.Combine(directory, filename);
            counter++;
        }
        while (File.Exists(fullPath));

        return fullPath;
    }

    #endregion

    #region FILE CACHE OPERATIONS

    public static async Task<object> AsyncMoveFilesToCache(
        object data,
        Block block,
        bool postprocess = false,
        bool checkInUploadFolder = false,
        bool keepInCache = false)
    {
        // Handle ListFiles (multiple-file component output): process each FileData
        // individually and return a List<object> so the result serialises to a JSON
        // array instead of {"root":[...]} (Python parity with Pydantic RootModel).
        if (data is Data.ListFiles listFiles)
        {
            var items = new List<object>();
            foreach (var fileData in listFiles.Root ?? new List<Data.FileData>())
            {
                var processed = await AsyncMoveFilesToCache(fileData, block, postprocess, checkInUploadFolder, keepInCache);
                items.Add(processed);
            }
            return items;
        }

        object MoveToCache(object d)
        {
            Data.FileData? payload = null;
            if (d is Dictionary<string, object> dict)
            {
                payload = CreateFileDataFromDict(dict);
            }
            else if (d is Data.FileData fd)
            {
                payload = fd;
            }

            if (payload == null)
                return d;

            // If URL is returned from postprocess and component can display URLs, keep as-is.
            // But don't override path if the URL was already generated by us (Gradio API file/stream URL),
            // since that would overwrite the local file path with the HTTP URL (Python parity: path stays local).
            if (!string.IsNullOrEmpty(payload.Url) && postprocess && Data.FileData.IsHttpUrl(payload.Url)
                && !payload.Url.Contains($"{RouteUtils.ApiPrefix}/file=")
                && !payload.Url.Contains($"{RouteUtils.ApiPrefix}/stream/"))
            {
                payload.Path = payload.Url;
            }
            else if (!string.IsNullOrEmpty(block.ProxyUrl))
            {
                // Handle proxy URL case
            }
            else
            {
                // Check if file is on remote server
                if (!Data.FileData.IsHttpUrl(payload.Path))
                {
                    CheckAllowed(payload.Path, checkInUploadFolder, block);
                }

                if (!payload.IsStream)
                {
                    string tempFilePath = block.MoveResourceToBlockCache(payload.Path);
                    if (tempFilePath == null)
                        throw new InvalidOperationException("Did not determine a file path for the resource.");

                    payload.Path = tempFilePath;
                    if (keepInCache)
                    {
                        block.KeepInCache.Add(payload.Path);
                    }
                }
            }

            // Set URL prefix
            string urlPrefix = payload.IsStream ? $"{RouteUtils.ApiPrefix}/stream/" : $"{RouteUtils.ApiPrefix}/file=";
            string url;

            if (!string.IsNullOrEmpty(block.ProxyUrl))
            {
                string proxyUrl = block.ProxyUrl.TrimEnd('/');
                url = $"/api/proxy={proxyUrl}{urlPrefix}{payload.Path}";
            }
            else if (Data.FileData.IsHttpUrl(payload.Path) || payload.Path.StartsWith(urlPrefix))
            {
                url = payload.Path;
            }
            else
            {
                var localUrl = LocalContext.Blocks?.LocalUrl?.TrimEnd('/') ?? string.Empty;
                url = $"{localUrl}{urlPrefix}{payload.Path}";
            }

            payload.Url = url;
            return payload.ToDictionary();
        }

        // Use TraverseHelper to process the data
        return await Task.Run(() =>
            Data.GradioModel.Traverse(
                data,
                MoveToCache,
                obj => Data.FileData.IsFileObj(obj) || obj is Data.FileData)
        );
    }

    public static void CheckAllFilesInCache(object data, Block block)
    {
        void CheckInCache(object d)
        {
            if (d is Dictionary<string, object> dict && dict.TryGetValue("path", out var pathObj))
            {
                string path = pathObj?.ToString();
                if (!string.IsNullOrEmpty(path) &&
                    !Data.FileData.IsHttpUrl(path) &&
                    !IsInOrEqual(path, Utils.Utils.GetUploadFolder()))
                {
                    throw new InvalidOperationException(
                        $"File {path} is not in the cache folder and cannot be accessed.");
                }
            }
        }

        Data.GradioModel.Traverse(data, obj => { CheckInCache(obj); return obj; }, Data.FileData.IsFileObj);
    }

    private static Data.FileData CreateFileDataFromDict(Dictionary<string, object> dict)
    {
        var fileData = new Data.FileData();

        if (dict.TryGetValue("path", out var path))
            fileData.Path = path?.ToString();
        if (dict.TryGetValue("url", out var url))
            fileData.Url = url?.ToString();
        if (dict.TryGetValue("size", out var size))
            fileData.Size = size != null ? (int?)Convert.ToInt32(size) : null;
        if (dict.TryGetValue("orig_name", out var origName))
            fileData.OrigName = origName?.ToString();
        if (dict.TryGetValue("mime_type", out var mimeType))
            fileData.MimeType = mimeType?.ToString();
        if (dict.TryGetValue("is_stream", out var isStream))
            fileData.IsStream = Convert.ToBoolean(isStream);

        return fileData;
    }

    private static void CheckAllowed(string path, bool checkInUploadFolder, Block block)
    {
        var blocks = LocalContext.Blocks;
        if (blocks == null || !blocks.HasLaunched)
            return;

        string absPath = Path.GetFullPath(path);

        // For upload folder check, only allow files in upload folder
        if (checkInUploadFolder)
        {
            bool inUploadFolder = IsInOrEqual(absPath, Utils.Utils.GetUploadFolder());
            if (!inUploadFolder)
            {
                throw new Exceptions.InvalidPathError(
                    $"Cannot move {absPath} to the gradio cache dir because it was not uploaded by a user.");
            }
            return;
        }

        // For general check, allow files in current dir, temp dir, or allowed paths
        var allowedPaths = new List<string>(blocks.AllowedPaths ?? new List<string>())
            {
                Directory.GetCurrentDirectory(),
                Path.GetTempPath()
            };

        bool allowed = allowedPaths.Any(allowed => IsInOrEqual(absPath, allowed));

        if (!allowed)
        {
            string msg = $"Cannot move {absPath} to the gradio cache dir because " +
                       "it was not created by the application or it is not located in either " +
                       $"the current working directory or your system's temp directory. " +
                       $"To fix this error, please add the file's directory to allowed_paths.";
            throw new Exceptions.InvalidPathError(msg);
        }
    }

    #endregion
}
